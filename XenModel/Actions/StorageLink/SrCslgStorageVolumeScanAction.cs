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


namespace XenAdmin.Actions
{
    /// <summary>
    /// Represents a storage volume of a storage system on a CSLG server.
    /// </summary>
    internal class SrCslgStorageVolumeScanAction : SrCslgScanAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _storageSystemId;
        private readonly string _storagePoolId;
        private ReadOnlyCollection<CslgStorageVolume> _cslgStorageVolumes;

        /// <summary>
        /// Initializes a new instance of the <see cref="SrCslgStorageVolumeScanAction"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="username">The username.</param>
        /// <param name="passwordSecret">The password secret.</param>
        /// <param name="storageSystemId">The storage system id.</param>
        /// <param name="storagePoolId">The storage pool to be queried.</param>
        public SrCslgStorageVolumeScanAction(IXenConnection connection, string hostname, string username, string passwordSecret, string storageSystemId, string storagePoolId)
            : base(connection, hostname, username, passwordSecret)
        {
            Util.ThrowIfStringParameterNullOrEmpty(storageSystemId, "storageSystemId");
            Util.ThrowIfStringParameterNullOrEmpty(storagePoolId, "storagePoolId");

            _storageSystemId = storageSystemId;
            _storagePoolId = storagePoolId;
        }

        private List<CslgStorageVolume> ParseStorageVolumeXml(String xml)
        {
            List<CslgStorageVolume> output = new List<CslgStorageVolume>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode storagePoolInfo in doc.GetElementsByTagName("csl__storageVolumeInfo"))
            {
                string displayName = GetXmlNodeInnerText(storagePoolInfo, "displayName");
                string sizeInMB = string.Format(Messages.VAL_MB, GetXmlNodeInnerText(storagePoolInfo, "sizeInMB"));
                string storageVolumeId = GetXmlNodeInnerText(storagePoolInfo, "storageVolumeId");
                string storageSystemId = GetXmlNodeInnerText(storagePoolInfo, "storageSystemId");
                string storagePoolId = GetXmlNodeInnerText(storagePoolInfo, "storagePoolId");

                int size;
                int.TryParse(sizeInMB, out size);

                output.Add(new CslgStorageVolume(displayName, storagePoolId, storageSystemId, storageVolumeId, size));
            }

            return output;
        }

        /// <summary>
        /// Gets the CSLG storage volumes.
        /// </summary>
        /// <value>The CSLG storage volumes.</value>
        public ReadOnlyCollection<CslgStorageVolume> CslgStorageVolumes
        {
            get
            {
                return _cslgStorageVolumes;
            }
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            Dictionary<string, string> dconf = GetAuthenticationDeviceConfig();

            dconf["storageSystemId"] = _storageSystemId;
            dconf["storagePoolId"] = _storagePoolId;

            Log.DebugFormat("Attempting to find volumes on pool {0}.", _storagePoolId);

            RunProbe(dconf);
            _cslgStorageVolumes = new ReadOnlyCollection<CslgStorageVolume>(new List<CslgStorageVolume>(ParseStorageVolumeXml(Util.GetContentsOfValueNode(Result))));
        }
    }

    #region CslgStorageVolume class

    /// <summary>
    /// Represents a storage volume on a storage system on a CSLG server.
    /// </summary>
    internal class CslgStorageVolume
    {
        private readonly string _displayName;
        private readonly string _storagePoolId;
        private readonly string _storageSystemId;
        private readonly string _storageVolumeId;
        private readonly int _sizeInMB;

        /// <summary>
        /// Initializes a new instance of the <see cref="CslgStorageVolume"/> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="storagePoolId">The storage pool id.</param>
        /// <param name="storageSystemId">The storage system id.</param>
        /// <param name="storageVolumeId">The storage volume id.</param>
        /// <param name="sizeInMB">The size in MB.</param>
        public CslgStorageVolume(string displayName, string storagePoolId, string storageSystemId, string storageVolumeId, int sizeInMB)
        {
            Util.ThrowIfStringParameterNullOrEmpty(displayName, "displayName");
            Util.ThrowIfStringParameterNullOrEmpty(storageSystemId, "storageSystemId");

            _displayName = displayName;
            _storagePoolId = storagePoolId;
            _storageSystemId = storageSystemId;
            _storageVolumeId = storageVolumeId;
            _sizeInMB = sizeInMB;
        }

        /// <summary>
        /// Gets the size in MB.
        /// </summary>
        /// <value>The size in MB.</value>
        public int SizeInMB
        {
            get
            {
                return _sizeInMB;
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

        /// <summary>
        /// Gets the storage pool ID.
        /// </summary>
        /// <value>The storage pool ID.</value>
        public string StoragePoolId
        {
            get
            {
                return _storagePoolId;
            }
        }

        /// <summary>
        /// Gets the storage system ID.
        /// </summary>
        /// <value>The storage system ID.</value>
        public string StorageSystemId
        {
            get
            {
                return _storageSystemId;
            }
        }

        /// <summary>
        /// Gets the storage volume ID.
        /// </summary>
        /// <value>The storage volume ID.</value>
        public string StorageVolumeId
        {
            get
            {
                return _storageVolumeId;
            }
        }
    }

    #endregion

}
