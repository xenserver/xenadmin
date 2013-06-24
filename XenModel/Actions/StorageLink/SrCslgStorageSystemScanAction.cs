/* Copyright (c) Citrix Systems Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Network.StorageLink;
using System.Threading;

namespace XenAdmin.Actions
{
    /// <summary>
    /// Performs a scan of a CSLG server for storage systems.
    /// </summary>
    public class SrCslgStorageSystemScanAction : SrCslgScanAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ReadOnlyCollection<CslgSystemStorage> _cslgSystemStorages;
        public StorageLinkConnection StorageLinkConnection { get; private set; }
        private List<StorageLinkConnection> _SLConnections;
        private System.ComponentModel.ISynchronizeInvoke _invoker;
        private string _adapterid;

        /// <summary>
        /// Initializes a new instance of the <see cref="SrCslgStorageSystemScanAction"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="username">The username.</param>
        /// <param name="passwordSecret">The password secret.</param>
        public SrCslgStorageSystemScanAction(System.ComponentModel.ISynchronizeInvoke invoker, IXenConnection connection, List<StorageLinkConnection> storageLinkConnections, string hostname, string username, string passwordSecret)
            : base(connection, hostname, username, passwordSecret)
        {
            _SLConnections = storageLinkConnections;
            _invoker = invoker;
        }

        /// <summary>
        /// Boston or greater constructor
        /// </summary>
        /// <param name="connection"></param>

        public SrCslgStorageSystemScanAction(IXenConnection connection, string adapterid, string target, string user, string password)
            : base(connection, target, user, password)
        {
            if (!Helpers.BostonOrGreater(connection))
                throw new ArgumentException(@"Invalid connection, it has to be a boston or greater connection", connection.Name);
            _adapterid = adapterid;
        }

        /// <summary>
        /// Gets the CSLG system storages. Returns null before the action has been run.
        /// </summary>
        /// <value>The CSLG system storages.</value>
        public ReadOnlyCollection<CslgSystemStorage> CslgSystemStorages
        {
            get
            {
                return _cslgSystemStorages;
            }
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            Dictionary<String, String> dconf = GetAuthenticationDeviceConfig();

            Log.DebugFormat("Attempting to find SRs on CSLG {0}.", dconf["target"]);

            if (Connection != null && Helpers.MidnightRideOrGreater(Connection) && !Helpers.CowleyOrGreater(Connection))
            {
                RunProbe(dconf);

                _cslgSystemStorages = new ReadOnlyCollection<CslgSystemStorage>(ParseStorageSystemsXml(Util.GetContentsOfValueNode(Result)));
            }
            else if (Connection != null && Helpers.BostonOrGreater(Connection))
            {
                dconf["adapterid"] = _adapterid;
                RunProbe(dconf);
                if (!string.IsNullOrEmpty(Result))
                    _cslgSystemStorages = new ReadOnlyCollection<CslgSystemStorage>(ParseStorageSystemsXml(Util.GetContentsOfValueNode(Result)));
            }
            else
            {
                bool created = false;

                if (Connection != null)
                {
                    // There will be no connection if a storagelink-object is selected in the tree.

                    string secretRef = Secret.get_by_uuid(Connection.Session, dconf["password_secret"]);
                    string password = Secret.get_value(Connection.Session, secretRef);

                    StorageLinkConnection = _SLConnections.Find(c => c.Host == dconf["target"] && c.Username == dconf["username"] && c.Password == password);

                    if (StorageLinkConnection == null)
                    {
                        // the user has clicked the "test connection" button in the properties dialog then
                        // the storagelink connection won't exist in the Program.StorageLinkConnections collection.

                        StorageLinkConnection = new StorageLinkConnection(_invoker, dconf["target"], dconf["username"], password);
                        StorageLinkConnection.BeginConnect();
                        created = true;
                    }
                }
                else
                {
                    StorageLinkConnection = _SLConnections.Find(c => c.Host == dconf["target"] && c.Username == dconf["username"]);
                }

                try
                {
                    var list = new List<CslgSystemStorage>();

                    // wait for storagelink connection to finish populating
                    for (int i = 0; i < 600 && StorageLinkConnection.ConnectionState == StorageLinkConnectionState.Connecting; i++)
                    {
                        Thread.Sleep(100);
                    }

                    if (!string.IsNullOrEmpty(StorageLinkConnection.Error))
                    {
                        throw new InvalidOperationException(StorageLinkConnection.Error);
                    }

                    if (StorageLinkConnection.ConnectionState != StorageLinkConnectionState.Connected)
                    {
                        throw new InvalidOperationException(string.Format(Messages.STORAGELINK_UNABLE_TO_CONNECT, dconf["target"]));
                    }

                    foreach (StorageLinkSystem s in StorageLinkConnection.Cache.StorageSystems)
                    {
                        var provisioningOptions = new List<CslgParameter> { new CslgParameter(null, Messages.NEWSR_CSLG_NONE) };

                        if ((s.Capabilities & StorageLinkEnums.StorageSystemCapabilities.POOL_LEVEL_DEDUPLICATION) != 0)
                        {
                            provisioningOptions.Add(new CslgParameter("DEDUP", Messages.NEWSR_CSLG_DEDUPLICATION));
                        }

                        var protocols = new List<CslgParameter> { new CslgParameter(null, Messages.NEWSR_CSLG_AUTO) };

                        if ((s.Capabilities & StorageLinkEnums.StorageSystemCapabilities.ISCSI) != 0)
                        {
                            protocols.Add(new CslgParameter("ISCSI", Messages.NEWSR_CSLG_ISCSI));
                        }
                        if ((s.Capabilities & StorageLinkEnums.StorageSystemCapabilities.FIBRE_CHANNEL) != 0)
                        {
                            protocols.Add(new CslgParameter("FC", Messages.NEWSR_CSLG_FC));
                        }

                        list.Add(new CslgSystemStorage(s.ToString(), s.StorageSystemId, protocols, provisioningOptions, false, s));
                    }

                    _cslgSystemStorages = new ReadOnlyCollection<CslgSystemStorage>(list);
                }
                finally
                {
                    if (created)
                    {
                        StorageLinkConnection.EndConnect();
                    }
                }
            }
        }




        private List<CslgSystemStorage> ParseStorageSystemsXml(String xml)
        {
            List<CslgSystemStorage> output = new List<CslgSystemStorage>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode storageSystemInfo in doc.GetElementsByTagName("csl__storageSystemInfo"))
            {
                string displayName = GetXmlNodeInnerText(storageSystemInfo, "displayName");
                string storageSystemId = GetXmlNodeInnerText(storageSystemInfo, "storageSystemId");
                List<CslgParameter> protocols = new List<CslgParameter>();
                protocols.Add(new CslgParameter(null, Messages.NEWSR_CSLG_AUTO));
                List<CslgParameter> provisioningOptions = new List<CslgParameter>();
                provisioningOptions.Add(new CslgParameter(null, Messages.NEWSR_CSLG_NONE));
                bool supportsCHAP = false;

                foreach (string innerText in GetXmlChildNodeInnerTexts(storageSystemInfo, "systemCapabilities/capabilities"))
                {
                    if (innerText == "DEDUPLICATION")
                    {
                        provisioningOptions.Add(new CslgParameter(innerText, Messages.NEWSR_CSLG_DEDUPLICATION));
                    }

                    if (innerText == "SUPPORTS_CHAP")
                    {
                        supportsCHAP = true;
                    }
                }

                foreach (string innerText in GetXmlChildNodeInnerTexts(storageSystemInfo, "protocolSupport/capabilities"))
                {
                    if (innerText == "ISCSI")
                    {
                        protocols.Add(new CslgParameter(innerText, Messages.NEWSR_CSLG_ISCSI));
                    }
                    else if (innerText == "FC")
                    {
                        protocols.Add(new CslgParameter(innerText, Messages.NEWSR_CSLG_FC));
                    }
                }

                output.Add(new CslgSystemStorage(displayName, storageSystemId, protocols, provisioningOptions,
                                                 supportsCHAP, null));
            }
            return output;
        }
    }

    #region CslgSystemStorage class

    /// <summary>
    /// Represents a System Storage from a CSLG server.
    /// </summary>
    public class CslgSystemStorage
    {
        private readonly string _displayName;
        private readonly string _storageSystemId;
        private readonly ReadOnlyCollection<CslgParameter> _protocols;
        private readonly ReadOnlyCollection<CslgParameter> _provisioningOptions;
        private readonly bool _supportsCHAP;

        public StorageLinkSystem StorageLinkSystem { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CslgSystemStorage"/> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="storageSystemId">The storage system id.</param>
        /// <param name="protocols">The available protocols.</param>
        /// <param name="provisioningOptions">The available provisioning options.</param>
        /// <param name="supportsCHAP">Indicates whether CHAP is supported.</param>
        public CslgSystemStorage(string displayName, string storageSystemId, IEnumerable<CslgParameter> protocols,
            IEnumerable<CslgParameter> provisioningOptions, bool supportsCHAP,
            StorageLinkSystem storageLinkSystem)
        {
            Util.ThrowIfStringParameterNullOrEmpty(displayName, "displayName");
            Util.ThrowIfStringParameterNullOrEmpty(storageSystemId, "storageSystemId");
            Util.ThrowIfParameterNull(protocols, "protocols");
            Util.ThrowIfParameterNull(protocols, "provisioningOptions");

            _displayName = displayName;
            _storageSystemId = storageSystemId;
            _protocols = new ReadOnlyCollection<CslgParameter>(new List<CslgParameter>(protocols));
            _provisioningOptions = new ReadOnlyCollection<CslgParameter>(new List<CslgParameter>(provisioningOptions));
            _supportsCHAP = supportsCHAP;
            StorageLinkSystem = storageLinkSystem;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        /// <summary>
        /// Gets the available protocols.
        /// </summary>
        /// <value>The available protocols.</value>
        public ReadOnlyCollection<CslgParameter> Protocols
        {
            get
            {
                return _protocols;
            }
        }

        /// <summary>
        /// Gets the storage system id.
        /// </summary>
        /// <value>The storage system id.</value>
        public string StorageSystemId
        {
            get
            {
                return _storageSystemId;
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }

        /// <summary>
        /// Gets the available provisioning options.
        /// </summary>
        /// <value>The available provisioning options.</value>
        public ReadOnlyCollection<CslgParameter> ProvisioningOptions
        {
            get
            {
                return _provisioningOptions;
            }
        }

        /// <summary>
        /// Gets supports CHAP value.
        /// </summary>
        /// <value>Indicates whether CHAP is supported.</value>
        public bool SupportsCHAP
        {
            get
            {
                return _supportsCHAP;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as CslgSystemStorage;
            return other != null && other.StorageSystemId == StorageSystemId;
        }

        public override int GetHashCode()
        {
            return StorageSystemId.GetHashCode();
        }
    }

    #endregion

    #region CslgParameter class

    /// <summary>
    /// Represents a parameter required to configure CSLG storage repository which has a string value and string display-name for that value.
    /// </summary>
    public class CslgParameter
    {
        private readonly string _name;
        private readonly string _displayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CslgParameter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        public CslgParameter(string name, string displayName)
        {
            Util.ThrowIfStringParameterNullOrEmpty(displayName, "displayName");

            _name = name;
            _displayName = displayName;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        public override string ToString()
        {
            return _displayName;
        }
    }

    #endregion
}
