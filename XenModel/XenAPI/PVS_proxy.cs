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
    /// a proxy connects a VM/VIF with a PVS site
    /// </summary>
    public partial class PVS_proxy : XenObject<PVS_proxy>
    {
        public PVS_proxy()
        {
        }

        public PVS_proxy(string uuid,
            XenRef<PVS_site> site,
            XenRef<VIF> VIF,
            bool currently_attached,
            pvs_proxy_status status)
        {
            this.uuid = uuid;
            this.site = site;
            this.VIF = VIF;
            this.currently_attached = currently_attached;
            this.status = status;
        }

        /// <summary>
        /// Creates a new PVS_proxy from a Proxy_PVS_proxy.
        /// </summary>
        /// <param name="proxy"></param>
        public PVS_proxy(Proxy_PVS_proxy proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(PVS_proxy update)
        {
            uuid = update.uuid;
            site = update.site;
            VIF = update.VIF;
            currently_attached = update.currently_attached;
            status = update.status;
        }

        internal void UpdateFromProxy(Proxy_PVS_proxy proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            site = proxy.site == null ? null : XenRef<PVS_site>.Create(proxy.site);
            VIF = proxy.VIF == null ? null : XenRef<VIF>.Create(proxy.VIF);
            currently_attached = (bool)proxy.currently_attached;
            status = proxy.status == null ? (pvs_proxy_status) 0 : (pvs_proxy_status)Helper.EnumParseDefault(typeof(pvs_proxy_status), (string)proxy.status);
        }

        public Proxy_PVS_proxy ToProxy()
        {
            Proxy_PVS_proxy result_ = new Proxy_PVS_proxy();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.site = (site != null) ? site : "";
            result_.VIF = (VIF != null) ? VIF : "";
            result_.currently_attached = currently_attached;
            result_.status = pvs_proxy_status_helper.ToString(status);
            return result_;
        }

        /// <summary>
        /// Creates a new PVS_proxy from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public PVS_proxy(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            site = Marshalling.ParseRef<PVS_site>(table, "site");
            VIF = Marshalling.ParseRef<VIF>(table, "VIF");
            currently_attached = Marshalling.ParseBool(table, "currently_attached");
            status = (pvs_proxy_status)Helper.EnumParseDefault(typeof(pvs_proxy_status), Marshalling.ParseString(table, "status"));
        }

        public bool DeepEquals(PVS_proxy other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._site, other._site) &&
                Helper.AreEqual2(this._VIF, other._VIF) &&
                Helper.AreEqual2(this._currently_attached, other._currently_attached) &&
                Helper.AreEqual2(this._status, other._status);
        }

        public override string SaveChanges(Session session, string opaqueRef, PVS_proxy server)
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
        /// Get a record containing the current state of the given PVS_proxy.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static PVS_proxy get_record(Session session, string _pvs_proxy)
        {
            return new PVS_proxy((Proxy_PVS_proxy)session.proxy.pvs_proxy_get_record(session.uuid, (_pvs_proxy != null) ? _pvs_proxy : "").parse());
        }

        /// <summary>
        /// Get a reference to the PVS_proxy instance with the specified UUID.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<PVS_proxy> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<PVS_proxy>.Create(session.proxy.pvs_proxy_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given PVS_proxy.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static string get_uuid(Session session, string _pvs_proxy)
        {
            return (string)session.proxy.pvs_proxy_get_uuid(session.uuid, (_pvs_proxy != null) ? _pvs_proxy : "").parse();
        }

        /// <summary>
        /// Get the site field of the given PVS_proxy.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static XenRef<PVS_site> get_site(Session session, string _pvs_proxy)
        {
            return XenRef<PVS_site>.Create(session.proxy.pvs_proxy_get_site(session.uuid, (_pvs_proxy != null) ? _pvs_proxy : "").parse());
        }

        /// <summary>
        /// Get the VIF field of the given PVS_proxy.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static XenRef<VIF> get_VIF(Session session, string _pvs_proxy)
        {
            return XenRef<VIF>.Create(session.proxy.pvs_proxy_get_vif(session.uuid, (_pvs_proxy != null) ? _pvs_proxy : "").parse());
        }

        /// <summary>
        /// Get the currently_attached field of the given PVS_proxy.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static bool get_currently_attached(Session session, string _pvs_proxy)
        {
            return (bool)session.proxy.pvs_proxy_get_currently_attached(session.uuid, (_pvs_proxy != null) ? _pvs_proxy : "").parse();
        }

        /// <summary>
        /// Get the status field of the given PVS_proxy.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static pvs_proxy_status get_status(Session session, string _pvs_proxy)
        {
            return (pvs_proxy_status)Helper.EnumParseDefault(typeof(pvs_proxy_status), (string)session.proxy.pvs_proxy_get_status(session.uuid, (_pvs_proxy != null) ? _pvs_proxy : "").parse());
        }

        /// <summary>
        /// Configure a VM/VIF to use a PVS proxy
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_site">PVS site that we proxy for</param>
        /// <param name="_vif">VIF for the VM that needs to be proxied</param>
        public static XenRef<PVS_proxy> create(Session session, string _site, string _vif)
        {
            return XenRef<PVS_proxy>.Create(session.proxy.pvs_proxy_create(session.uuid, (_site != null) ? _site : "", (_vif != null) ? _vif : "").parse());
        }

        /// <summary>
        /// Configure a VM/VIF to use a PVS proxy
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_site">PVS site that we proxy for</param>
        /// <param name="_vif">VIF for the VM that needs to be proxied</param>
        public static XenRef<Task> async_create(Session session, string _site, string _vif)
        {
            return XenRef<Task>.Create(session.proxy.async_pvs_proxy_create(session.uuid, (_site != null) ? _site : "", (_vif != null) ? _vif : "").parse());
        }

        /// <summary>
        /// remove (or switch off) a PVS proxy for this VM
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static void destroy(Session session, string _pvs_proxy)
        {
            session.proxy.pvs_proxy_destroy(session.uuid, (_pvs_proxy != null) ? _pvs_proxy : "").parse();
        }

        /// <summary>
        /// remove (or switch off) a PVS proxy for this VM
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static XenRef<Task> async_destroy(Session session, string _pvs_proxy)
        {
            return XenRef<Task>.Create(session.proxy.async_pvs_proxy_destroy(session.uuid, (_pvs_proxy != null) ? _pvs_proxy : "").parse());
        }

        /// <summary>
        /// Return a list of all the PVS_proxys known to the system.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<PVS_proxy>> get_all(Session session)
        {
            return XenRef<PVS_proxy>.Create(session.proxy.pvs_proxy_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the PVS_proxy Records at once, in a single XML RPC call
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<PVS_proxy>, PVS_proxy> get_all_records(Session session)
        {
            return XenRef<PVS_proxy>.Create<Proxy_PVS_proxy>(session.proxy.pvs_proxy_get_all_records(session.uuid).parse());
        }

        /// <summary>
        /// Unique identifier/object reference
        /// Experimental. First published in .
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

        /// <summary>
        /// PVS site this proxy is part of
        /// Experimental. First published in .
        /// </summary>
        public virtual XenRef<PVS_site> site
        {
            get { return _site; }
            set
            {
                if (!Helper.AreEqual(value, _site))
                {
                    _site = value;
                    Changed = true;
                    NotifyPropertyChanged("site");
                }
            }
        }
        private XenRef<PVS_site> _site;

        /// <summary>
        /// VIF of the VM using the proxy
        /// Experimental. First published in .
        /// </summary>
        public virtual XenRef<VIF> VIF
        {
            get { return _VIF; }
            set
            {
                if (!Helper.AreEqual(value, _VIF))
                {
                    _VIF = value;
                    Changed = true;
                    NotifyPropertyChanged("VIF");
                }
            }
        }
        private XenRef<VIF> _VIF;

        /// <summary>
        /// true = VM is currently proxied
        /// Experimental. First published in .
        /// </summary>
        public virtual bool currently_attached
        {
            get { return _currently_attached; }
            set
            {
                if (!Helper.AreEqual(value, _currently_attached))
                {
                    _currently_attached = value;
                    Changed = true;
                    NotifyPropertyChanged("currently_attached");
                }
            }
        }
        private bool _currently_attached;

        /// <summary>
        /// The run-time status of the proxy
        /// Experimental. First published in .
        /// </summary>
        public virtual pvs_proxy_status status
        {
            get { return _status; }
            set
            {
                if (!Helper.AreEqual(value, _status))
                {
                    _status = value;
                    Changed = true;
                    NotifyPropertyChanged("status");
                }
            }
        }
        private pvs_proxy_status _status;
    }
}
