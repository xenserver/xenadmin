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

using XenAdmin.Network.StorageLink;

namespace XenAPI
{
    public class StorageLinkAdapter : StorageLinkObject<StorageLinkAdapter>
    {
        private string _adapterName;
        private readonly uint _inputOptions;
        public bool IsSMIS { get; private set; }

        public StorageLinkAdapter()
        {
        }

        public StorageLinkAdapter(StorageLinkConnection storageLinkConnection, string opaque_ref, string friendlyName,
            string adapterName, uint inputOptions, string defaultNamespace, int defaultPort,bool isSMIS)
            : base(storageLinkConnection, opaque_ref, friendlyName)
        {
            _adapterName = adapterName;
            _inputOptions = inputOptions;
            DefaultNamespace = defaultNamespace;
            DefaultPort = defaultPort;
            IsSMIS = isSMIS;
        }

        public override void UpdateFrom(StorageLinkAdapter update)
        {
            base.UpdateFrom(update);
            AdapterName = update.AdapterName;
        }

        public string AdapterName
        {
            get
            {
                return _adapterName;
            }
            private set
            {
                if (value != _adapterName)
                {
                    _adapterName = value;
                    NotifyPropertyChanged("AdapterName");
                }
            }
        }

        public string DefaultNamespace { get; private set; }
        public int DefaultPort { get; private set; }

        private bool Requires(StorageLinkEnums.ModuleCredInputOptions options)
        {
            return (_inputOptions & (uint)options) != 0;
        }
        
        public bool RequiresPort
        {
            get
            {
                return Requires(StorageLinkEnums.ModuleCredInputOptions.STORAGE_ADAPTER_CRED_REQUIRES_PORTNUM);
            }
        }

        public bool SupportsPort
        {
            get
            {
                return Requires(StorageLinkEnums.ModuleCredInputOptions.STORAGE_ADAPTER_CRED_SUPPORTS_PORTNUM);
            }
        }

        public bool RequiresIPAddress
        {
            get
            {
                return Requires(StorageLinkEnums.ModuleCredInputOptions.STORAGE_ADAPTER_CRED_REQUIRES_IPADDRESS);
            }
        }

        public bool SupportsIPAddress
        {
            get
            {
                return Requires(StorageLinkEnums.ModuleCredInputOptions.STORAGE_ADAPTER_CRED_SUPPORTS_IPADDRESS);
            }
        }

        public bool RequiresUsername
        {
            get
            {
                return Requires(StorageLinkEnums.ModuleCredInputOptions.STORAGE_ADAPTER_CRED_REQUIRES_USERNAME);
            }
        }

        public bool SupportsUsername
        {
            get
            {
                return Requires(StorageLinkEnums.ModuleCredInputOptions.STORAGE_ADAPTER_CRED_SUPPORTS_USERNAME);
            }
        }

        public bool RequiresPassword
        {
            get
            {
                return Requires(StorageLinkEnums.ModuleCredInputOptions.STORAGE_ADAPTER_CRED_REQUIRES_PASSWORD);
            }
        }

        public bool SupportsPassword
        {
            get
            {
                return Requires(StorageLinkEnums.ModuleCredInputOptions.STORAGE_ADAPTER_CRED_SUPPORTS_PASSWORD);
            }
        }

        public bool RequiresNamespace
        {
            get
            {
                return Requires(StorageLinkEnums.ModuleCredInputOptions.STORAGE_ADAPTER_CRED_REQUIRES_NAMESPACE);
            }
        }

        public bool SupportsNamespace
        {
            get
            {
                return Requires(StorageLinkEnums.ModuleCredInputOptions.STORAGE_ADAPTER_CRED_SUPPORTS_NAMESPACE);
            }
        }

        public bool RequiresContext
        {
            get
            {
                return Requires(StorageLinkEnums.ModuleCredInputOptions.STORAGE_ADAPTER_CRED_REQUIRES_CONTEXT);
            }
        }

        public bool SupportsContext
        {
            get
            {
                return Requires(StorageLinkEnums.ModuleCredInputOptions.STORAGE_ADAPTER_CRED_SUPPORTS_CONTEXT);
            }
        }
    }
}
