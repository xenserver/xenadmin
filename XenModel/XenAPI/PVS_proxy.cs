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
            bool prepopulate,
            bool currently_attached,
            XenRef<SR> cache_SR)
        {
            this.uuid = uuid;
            this.site = site;
            this.VIF = VIF;
            this.prepopulate = prepopulate;
            this.currently_attached = currently_attached;
            this.cache_SR = cache_SR;
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
            prepopulate = update.prepopulate;
            currently_attached = update.currently_attached;
            cache_SR = update.cache_SR;
        }

        internal void UpdateFromProxy(Proxy_PVS_proxy proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            site = proxy.site == null ? null : XenRef<PVS_site>.Create(proxy.site);
            VIF = proxy.VIF == null ? null : XenRef<VIF>.Create(proxy.VIF);
            prepopulate = (bool)proxy.prepopulate;
            currently_attached = (bool)proxy.currently_attached;
            cache_SR = proxy.cache_SR == null ? null : XenRef<SR>.Create(proxy.cache_SR);
        }

        public Proxy_PVS_proxy ToProxy()
        {
            Proxy_PVS_proxy result_ = new Proxy_PVS_proxy();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.site = (site != null) ? site : "";
            result_.VIF = (VIF != null) ? VIF : "";
            result_.prepopulate = prepopulate;
            result_.currently_attached = currently_attached;
            result_.cache_SR = (cache_SR != null) ? cache_SR : "";
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
            prepopulate = Marshalling.ParseBool(table, "prepopulate");
            currently_attached = Marshalling.ParseBool(table, "currently_attached");
            cache_SR = Marshalling.ParseRef<SR>(table, "cache_SR");
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
                Helper.AreEqual2(this._prepopulate, other._prepopulate) &&
                Helper.AreEqual2(this._currently_attached, other._currently_attached) &&
                Helper.AreEqual2(this._cache_SR, other._cache_SR);
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
                if (!Helper.AreEqual2(_prepopulate, server._prepopulate))
                {
                    PVS_proxy.set_prepopulate(session, opaqueRef, _prepopulate);
                }

                return null;
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
        /// Get the prepopulate field of the given PVS_proxy.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static bool get_prepopulate(Session session, string _pvs_proxy)
        {
            return (bool)session.proxy.pvs_proxy_get_prepopulate(session.uuid, (_pvs_proxy != null) ? _pvs_proxy : "").parse();
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
        /// Get the cache_SR field of the given PVS_proxy.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static XenRef<SR> get_cache_SR(Session session, string _pvs_proxy)
        {
            return XenRef<SR>.Create(session.proxy.pvs_proxy_get_cache_sr(session.uuid, (_pvs_proxy != null) ? _pvs_proxy : "").parse());
        }

        /// <summary>
        /// Configure a VM/VIF to use a PVS proxy
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_site">PVS site that we proxy for</param>
        /// <param name="_vif">VIF for the VM that needs to be proxied</param>
        /// <param name="_prepopulate">if true, prefetch whole disk for VM</param>
        public static XenRef<PVS_proxy> create(Session session, string _site, string _vif, bool _prepopulate)
        {
            return XenRef<PVS_proxy>.Create(session.proxy.pvs_proxy_create(session.uuid, (_site != null) ? _site : "", (_vif != null) ? _vif : "", _prepopulate).parse());
        }

        /// <summary>
        /// Configure a VM/VIF to use a PVS proxy
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_site">PVS site that we proxy for</param>
        /// <param name="_vif">VIF for the VM that needs to be proxied</param>
        /// <param name="_prepopulate">if true, prefetch whole disk for VM</param>
        public static XenRef<Task> async_create(Session session, string _site, string _vif, bool _prepopulate)
        {
            return XenRef<Task>.Create(session.proxy.async_pvs_proxy_create(session.uuid, (_site != null) ? _site : "", (_vif != null) ? _vif : "", _prepopulate).parse());
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
        /// change the value of the prepopulate field
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        /// <param name="_value">set to this value</param>
        public static void set_prepopulate(Session session, string _pvs_proxy, bool _value)
        {
            session.proxy.pvs_proxy_set_prepopulate(session.uuid, (_pvs_proxy != null) ? _pvs_proxy : "", _value).parse();
        }

        /// <summary>
        /// change the value of the prepopulate field
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        /// <param name="_value">set to this value</param>
        public static XenRef<Task> async_set_prepopulate(Session session, string _pvs_proxy, bool _value)
        {
            return XenRef<Task>.Create(session.proxy.async_pvs_proxy_set_prepopulate(session.uuid, (_pvs_proxy != null) ? _pvs_proxy : "", _value).parse());
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
        /// true = proxy prefetches whole disk for the VM
        /// Experimental. First published in .
        /// </summary>
        public virtual bool prepopulate
        {
            get { return _prepopulate; }
            set
            {
                if (!Helper.AreEqual(value, _prepopulate))
                {
                    _prepopulate = value;
                    Changed = true;
                    NotifyPropertyChanged("prepopulate");
                }
            }
        }
        private bool _prepopulate;

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
        /// SR used by this proxy
        /// Experimental. First published in .
        /// </summary>
        public virtual XenRef<SR> cache_SR
        {
            get { return _cache_SR; }
            set
            {
                if (!Helper.AreEqual(value, _cache_SR))
                {
                    _cache_SR = value;
                    Changed = true;
                    NotifyPropertyChanged("cache_SR");
                }
            }
        }
        private XenRef<SR> _cache_SR;
    }
}
