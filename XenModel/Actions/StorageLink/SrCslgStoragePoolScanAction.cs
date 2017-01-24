/* Copyright (c) Citrix Systems, Inc. 
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
using System.Text.RegularExpressions;

namespace XenAdmin.Actions
{
    /// <summary>
    /// Performs a scan of a CSLG server for storage pools given a storage system.
    /// </summary>
    public class SrCslgStoragePoolScanAction : SrCslgScanAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _storageSystemId;
        private ReadOnlyCollection<CslgStoragePool> _cslgStoragePools;

        /// <summary>
        /// Initializes a new instance of the <see cref="SrCslgStoragePoolScanAction"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="username">The username.</param>
        /// <param name="passwordSecret">The password secret.</param>
        /// <param name="storageSystemId">The storage system id.</param>
        public SrCslgStoragePoolScanAction(IXenConnection connection, string hostname, string username, string passwordSecret, string storageSystemId)
            : base(connection, hostname, username, passwordSecret)
        {
            Util.ThrowIfStringParameterNullOrEmpty(storageSystemId, "storageSystemId");
            _storageSystemId = storageSystemId;
        }

        private readonly string _adapterId;
        public SrCslgStoragePoolScanAction(IXenConnection connection, string hostname, string username, string password, string storagesystemid, string adapterid)
            : base(connection, hostname, username, password)
        {
            _storageSystemId = storagesystemid;
            _adapterId = adapterid;
        }

        /// <summary>
        /// Gets the CSLG system storages. Returns null before the action has been run.
        /// </summary>
        /// <value>The CSLG system storages.</value>
        public ReadOnlyCollection<CslgStoragePool> CslgStoragePools
        {
            get
            {
                return _cslgStoragePools;
            }
        }

        private List<CslgStoragePool> ParseStoragePoolXml(String xml)
        {
            List<CslgStoragePool> output = new List<CslgStoragePool>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode storagePoolInfo in doc.GetElementsByTagName("csl__storagePoolInfo"))
            {
                string displayName = GetXmlNodeInnerText(storagePoolInfo, "displayName").Trim();
                string storagePoolId = GetXmlNodeInnerText(storagePoolInfo, "storagePoolId").Trim();
                List<string> raidTypes = new List<string>();
                foreach (var raid in GetXmlChildNodeInnerTexts(storagePoolInfo, "provisioningOptions/supportedRaidTypes/raidType"))
                {
                    raidTypes.Add(raid.Trim());
                }
                List<CslgParameter> provisioningTypes = new List<CslgParameter>();
                string parentStoragePoolId = GetXmlNodeInnerText(storagePoolInfo, "parentStoragePoolId");

                foreach (string innerText in GetXmlChildNodeInnerTexts(storagePoolInfo, "provisioningOptions/supportedProvisioningTypes/provisioningType"))
                {
                    var trimmedInnerText = innerText.Trim();
                    // CVSM-3277: bridge returns "FULL" for some versions but "THICK" is required.
                    if (trimmedInnerText == "FULL" || trimmedInnerText == "THICK")
                    {
                        provisioningTypes.Add(new CslgParameter("THICK", Messages.NEWSR_CSLG_THICK_PROVISIONING));
                    }
                    else if (trimmedInnerText == "THIN")
                    {
                        provisioningTypes.Add(new CslgParameter(trimmedInnerText, Messages.NEWSR_CSLG_THIN_PROVISIONING));
                    }
                }

                if (raidTypes.Count == 0)
                {
                    raidTypes.Add("RAID_NONE");
                }

                if (provisioningTypes.Count == 0)
                {
                    provisioningTypes.Add(new CslgParameter(null, Messages.NEWSR_CSLG_DEFAULT_PROVISIONING));
                }

                uint capacity = 0;
                uint usedSpace = 0;
                try
                {
                    capacity = UInt32.Parse(GetXmlNodeInnerText(storagePoolInfo, "sizeInMB").Trim());
                    usedSpace = capacity - UInt32.Parse(GetXmlNodeInnerText(storagePoolInfo, "freeSpaceInMB").Trim());
                }
                catch { }
                    
                StorageLinkPool storageLinkPool = new StorageLinkPool(storagePoolId, displayName, parentStoragePoolId, _storageSystemId, capacity, usedSpace,
                    (StorageLinkEnums.RaidType)Enum.Parse(typeof(StorageLinkEnums.RaidType), raidTypes[0].ToUpper()),
                    (StorageLinkEnums.ProvisioningType)Enum.Parse(typeof(StorageLinkEnums.ProvisioningType), provisioningTypes[0].Name.ToUpper()));
                output.Add(new CslgStoragePool(displayName, storagePoolId, raidTypes, provisioningTypes, !string.IsNullOrEmpty(parentStoragePoolId), storageLinkPool));
            }

            return output;

        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            Dictionary<string, string> dconf = GetAuthenticationDeviceConfig();
            dconf["storageSystemId"] = _storageSystemId;
            if (!string.IsNullOrEmpty(_adapterId))
                dconf["adapterid"] = _adapterId;
            Log.DebugFormat("Attempting to find pools on {0}.", _storageSystemId);

            RunProbe(dconf);
            if (!string.IsNullOrEmpty(Result))
                _cslgStoragePools = new ReadOnlyCollection<CslgStoragePool>(ParseStoragePoolXml(Util.GetContentsOfValueNode(Result)));
        }

        private static List<string> GetRaidTypes(StorageLinkEnums.RaidType raidType)
        {
            List<string> output = new List<string>();
            foreach (StorageLinkEnums.RaidType r in Enum.GetValues(typeof(StorageLinkEnums.RaidType)))
            {
                if ((raidType & r) != 0)
                {
                    output.Add(r.ToString());
                }
            }

            if (output.Count == 0)
            {
                output.Add("RAID_NONE");
            }

            return output;
        }

        private static List<CslgParameter> GetProvisioningTypes(StorageLinkEnums.ProvisioningType provisioningType)
        {
            List<CslgParameter> output = new List<CslgParameter>();
            if ((provisioningType & StorageLinkEnums.ProvisioningType.THICK) != 0)
            {
                output.Add(new CslgParameter("THICK", Messages.NEWSR_CSLG_THICK_PROVISIONING));
            }
            if ((provisioningType & StorageLinkEnums.ProvisioningType.THIN) != 0)
            {
                output.Add(new CslgParameter("THIN", Messages.NEWSR_CSLG_THIN_PROVISIONING));
            }
            return output;
        }
    }

    #region CslgStoragePool class

    /// <summary>
    /// Represents of storoage pool on a storage system on a CSLG server.
    /// </summary>
    public class CslgStoragePool
    {
        private readonly string _displayName;
        private readonly string _storagePoolId;
        private readonly ReadOnlyCollection<string> _raidTypes;
        private readonly ReadOnlyCollection<CslgParameter> _provisioningTypes;
        private readonly bool _hasParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CslgStoragePool"/> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="storagePoolId">The storage pool id.</param>
        /// <param name="raidTypes">The raid types.</param>
        /// <param name="provisioningTypes">The provisioning types.</param>
        /// <param name="hasParent">if set to <c>true</c> if this pool has a parent pool.</param>
        public CslgStoragePool(string displayName, string storagePoolId, IEnumerable<string> raidTypes, IEnumerable<CslgParameter> provisioningTypes, bool hasParent, StorageLinkPool storageLinkPool)
        {
            Util.ThrowIfStringParameterNullOrEmpty(displayName, "displayName");
            Util.ThrowIfStringParameterNullOrEmpty(storagePoolId, "storagePoolId");

            _displayName = displayName;
            _storagePoolId = storagePoolId;
            _raidTypes = new ReadOnlyCollection<string>(new List<string>(raidTypes));
            _provisioningTypes = new ReadOnlyCollection<CslgParameter>(new List<CslgParameter>(provisioningTypes));
            _hasParent = hasParent;
            StorageLinkPool = storageLinkPool;
        }

        public StorageLinkPool StorageLinkPool { get; private set; }

        /// <summary>
        /// Gets the available raid types for this pool.
        /// </summary>
        /// <value>The available raid types for this pool.</value>
        public ReadOnlyCollection<string> RaidTypes
        {
            get
            {
                return _raidTypes;
            }
        }

        /// <summary>
        /// Gets the available provisioning types for this pool.
        /// </summary>
        /// <value>The available provisioning types for this pool.</value>
        public ReadOnlyCollection<CslgParameter> ProvisioningTypes
        {
            get
            {
                return _provisioningTypes;
            }
        }

        /// <summary>
        /// Gets the storage pool id.
        /// </summary>
        /// <value>The storage pool id.</value>
        public string StoragePoolId
        {
            get
            {
                return _storagePoolId;
            }
        }

        public override string ToString()
        {
            return _displayName;
        }

        /// <summary>
        /// Gets a value indicating whether this pool has a parent pool.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has a parent pool; otherwise, <c>false</c>.
        /// </value>
        public bool HasParent
        {
            get { return _hasParent; }
        }
    }

    #endregion

}
