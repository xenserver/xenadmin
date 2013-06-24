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
using XenAdmin;
using XenAdmin.Network.StorageLink;

namespace XenAPI
{
    public abstract class StorageLinkObject<T> : XenObject<T>, IStorageLinkObject where T : StorageLinkObject<T>
    {
        private readonly StorageLinkConnection _storageLinkConnection;
        private string _friendlyName;

        public StorageLinkObject()
        {
        }

        public StorageLinkObject(StorageLinkConnection storageLinkConnection, string opaque_ref, string friendlyName)
        {
            //Util.ThrowIfParameterNull(storageLinkConnection, "storageLinkConnection");
            Util.ThrowIfStringParameterNullOrEmpty(opaque_ref, "opaque_ref");
            Util.ThrowIfParameterNull(friendlyName, "friendlyName");

            _storageLinkConnection = storageLinkConnection;
            base.opaque_ref = opaque_ref;
            _friendlyName = friendlyName;
        }

        public override void UpdateFrom(T update)
        {
            FriendlyName = update.FriendlyName;
        }

        public override string SaveChanges(Session session, string serverOpaqueRef, T serverObject)
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return FriendlyName;
        }

        public override string Name
        {
            get
            {
                return FriendlyName;
            }
        }


        #region IStorageLinkObject Members

        public string FriendlyName
        {
            get
            {
                return _friendlyName;
            }
            private set
            {
                if (value != _friendlyName)
                {
                    _friendlyName = value;
                    NotifyPropertyChanged("FriendlyName");
                }
            }
        }

        public StorageLinkConnection StorageLinkConnection
        {
            get
            {
                return _storageLinkConnection;
            }
        }

        public virtual string uuid
        {
            get
            {
                return opaque_ref;
            }
        }

        #endregion
    }

    public interface IStorageLinkObject : IXenObject
    {
        string FriendlyName { get; }
        StorageLinkConnection StorageLinkConnection { get; }
        string uuid { get; }
    }
}
