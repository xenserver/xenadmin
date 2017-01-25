/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 * 
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */


using System;
using System.Collections;
using System.Collections.Generic;

using CookComputing.XmlRpc;


namespace XenAPI
{
    /// <summary>
    /// LVHD SR specific operations
    /// First published in XenServer Dundee.
    /// </summary>
    public partial class LVHD : XenObject<LVHD>
    {
        public LVHD()
        {
        }

        public LVHD(string uuid)
        {
            this.uuid = uuid;
        }

        /// <summary>
        /// Creates a new LVHD from a Proxy_LVHD.
        /// </summary>
        /// <param name="proxy"></param>
        public LVHD(Proxy_LVHD proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(LVHD update)
        {
            uuid = update.uuid;
        }

        internal void UpdateFromProxy(Proxy_LVHD proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
        }

        public Proxy_LVHD ToProxy()
        {
            Proxy_LVHD result_ = new Proxy_LVHD();
            result_.uuid = (uuid != null) ? uuid : "";
            return result_;
        }

        /// <summary>
        /// Creates a new LVHD from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public LVHD(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
        }

        public bool DeepEquals(LVHD other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid);
        }

        public override string SaveChanges(Session session, string opaqueRef, LVHD server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
              throw new InvalidOperationException("This type has no read/write properties");
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given LVHD.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_lvhd">The opaque_ref of the given lvhd</param>
        public static LVHD get_record(Session session, string _lvhd)
        {
            return new LVHD((Proxy_LVHD)session.proxy.lvhd_get_record(session.uuid, (_lvhd != null) ? _lvhd : "").parse());
        }

        /// <summary>
        /// Get a reference to the LVHD instance with the specified UUID.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<LVHD> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<LVHD>.Create(session.proxy.lvhd_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given LVHD.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_lvhd">The opaque_ref of the given lvhd</param>
        public static string get_uuid(Session session, string _lvhd)
        {
            return (string)session.proxy.lvhd_get_uuid(session.uuid, (_lvhd != null) ? _lvhd : "").parse();
        }

        /// <summary>
        /// Upgrades an LVHD SR to enable thin-provisioning. Future VDIs created in this SR will be thinly-provisioned, although existing VDIs will be left alone. Note that the SR must be attached to the SRmaster for upgrade to work.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The LVHD Host to upgrade to being thin-provisioned.</param>
        /// <param name="_sr">The LVHD SR to upgrade to being thin-provisioned.</param>
        /// <param name="_initial_allocation">The initial amount of space to allocate to a newly-created VDI in bytes</param>
        /// <param name="_allocation_quantum">The amount of space to allocate to a VDI when it needs to be enlarged in bytes</param>
        public static string enable_thin_provisioning(Session session, string _host, string _sr, long _initial_allocation, long _allocation_quantum)
        {
            return (string)session.proxy.lvhd_enable_thin_provisioning(session.uuid, (_host != null) ? _host : "", (_sr != null) ? _sr : "", _initial_allocation.ToString(), _allocation_quantum.ToString()).parse();
        }

        /// <summary>
        /// Upgrades an LVHD SR to enable thin-provisioning. Future VDIs created in this SR will be thinly-provisioned, although existing VDIs will be left alone. Note that the SR must be attached to the SRmaster for upgrade to work.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The LVHD Host to upgrade to being thin-provisioned.</param>
        /// <param name="_sr">The LVHD SR to upgrade to being thin-provisioned.</param>
        /// <param name="_initial_allocation">The initial amount of space to allocate to a newly-created VDI in bytes</param>
        /// <param name="_allocation_quantum">The amount of space to allocate to a VDI when it needs to be enlarged in bytes</param>
        public static XenRef<Task> async_enable_thin_provisioning(Session session, string _host, string _sr, long _initial_allocation, long _allocation_quantum)
        {
            return XenRef<Task>.Create(session.proxy.async_lvhd_enable_thin_provisioning(session.uuid, (_host != null) ? _host : "", (_sr != null) ? _sr : "", _initial_allocation.ToString(), _allocation_quantum.ToString()).parse());
        }

        /// <summary>
        /// Unique identifier/object reference
        /// </summary>
        public virtual string uuid
        {
            get { return _uuid; }
            set
            {
                if (!Helper.AreEqual(value, _uuid))
                {
                    _uuid = value;
                    Changed = true;
                    NotifyPropertyChanged("uuid");
                }
            }
        }
        private string _uuid;
    }
}
