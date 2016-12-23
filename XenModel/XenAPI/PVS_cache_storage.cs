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
    /// Describes the storage that is available to a PVS site for caching purposes
    /// </summary>
    public partial class PVS_cache_storage : XenObject<PVS_cache_storage>
    {
        public PVS_cache_storage()
        {
        }

        public PVS_cache_storage(string uuid,
            XenRef<Host> host,
            XenRef<SR> SR,
            XenRef<PVS_site> site,
            long size,
            XenRef<VDI> VDI)
        {
            this.uuid = uuid;
            this.host = host;
            this.SR = SR;
            this.site = site;
            this.size = size;
            this.VDI = VDI;
        }

        /// <summary>
        /// Creates a new PVS_cache_storage from a Proxy_PVS_cache_storage.
        /// </summary>
        /// <param name="proxy"></param>
        public PVS_cache_storage(Proxy_PVS_cache_storage proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(PVS_cache_storage update)
        {
            uuid = update.uuid;
            host = update.host;
            SR = update.SR;
            site = update.site;
            size = update.size;
            VDI = update.VDI;
        }

        internal void UpdateFromProxy(Proxy_PVS_cache_storage proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
            SR = proxy.SR == null ? null : XenRef<SR>.Create(proxy.SR);
            site = proxy.site == null ? null : XenRef<PVS_site>.Create(proxy.site);
            size = proxy.size == null ? 0 : long.Parse((string)proxy.size);
            VDI = proxy.VDI == null ? null : XenRef<VDI>.Create(proxy.VDI);
        }

        public Proxy_PVS_cache_storage ToProxy()
        {
            Proxy_PVS_cache_storage result_ = new Proxy_PVS_cache_storage();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.host = (host != null) ? host : "";
            result_.SR = (SR != null) ? SR : "";
            result_.site = (site != null) ? site : "";
            result_.size = size.ToString();
            result_.VDI = (VDI != null) ? VDI : "";
            return result_;
        }

        /// <summary>
        /// Creates a new PVS_cache_storage from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public PVS_cache_storage(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            host = Marshalling.ParseRef<Host>(table, "host");
            SR = Marshalling.ParseRef<SR>(table, "SR");
            site = Marshalling.ParseRef<PVS_site>(table, "site");
            size = Marshalling.ParseLong(table, "size");
            VDI = Marshalling.ParseRef<VDI>(table, "VDI");
        }

        public bool DeepEquals(PVS_cache_storage other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._host, other._host) &&
                Helper.AreEqual2(this._SR, other._SR) &&
                Helper.AreEqual2(this._site, other._site) &&
                Helper.AreEqual2(this._size, other._size) &&
                Helper.AreEqual2(this._VDI, other._VDI);
        }

        public override string SaveChanges(Session session, string opaqueRef, PVS_cache_storage server)
        {
            if (opaqueRef == null)
            {
                Proxy_PVS_cache_storage p = this.ToProxy();
                return session.proxy.pvs_cache_storage_create(session.uuid, p).parse();
            }
            else
            {
              throw new InvalidOperationException("This type has no read/write properties");
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given PVS_cache_storage.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static PVS_cache_storage get_record(Session session, string _pvs_cache_storage)
        {
            return new PVS_cache_storage((Proxy_PVS_cache_storage)session.proxy.pvs_cache_storage_get_record(session.uuid, (_pvs_cache_storage != null) ? _pvs_cache_storage : "").parse());
        }

        /// <summary>
        /// Get a reference to the PVS_cache_storage instance with the specified UUID.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<PVS_cache_storage> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<PVS_cache_storage>.Create(session.proxy.pvs_cache_storage_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Create a new PVS_cache_storage instance, and return its handle.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<PVS_cache_storage> create(Session session, PVS_cache_storage _record)
        {
            return XenRef<PVS_cache_storage>.Create(session.proxy.pvs_cache_storage_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Create a new PVS_cache_storage instance, and return its handle.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Task> async_create(Session session, PVS_cache_storage _record)
        {
            return XenRef<Task>.Create(session.proxy.async_pvs_cache_storage_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Destroy the specified PVS_cache_storage instance.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static void destroy(Session session, string _pvs_cache_storage)
        {
            session.proxy.pvs_cache_storage_destroy(session.uuid, (_pvs_cache_storage != null) ? _pvs_cache_storage : "").parse();
        }

        /// <summary>
        /// Destroy the specified PVS_cache_storage instance.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static XenRef<Task> async_destroy(Session session, string _pvs_cache_storage)
        {
            return XenRef<Task>.Create(session.proxy.async_pvs_cache_storage_destroy(session.uuid, (_pvs_cache_storage != null) ? _pvs_cache_storage : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given PVS_cache_storage.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static string get_uuid(Session session, string _pvs_cache_storage)
        {
            return (string)session.proxy.pvs_cache_storage_get_uuid(session.uuid, (_pvs_cache_storage != null) ? _pvs_cache_storage : "").parse();
        }

        /// <summary>
        /// Get the host field of the given PVS_cache_storage.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static XenRef<Host> get_host(Session session, string _pvs_cache_storage)
        {
            return XenRef<Host>.Create(session.proxy.pvs_cache_storage_get_host(session.uuid, (_pvs_cache_storage != null) ? _pvs_cache_storage : "").parse());
        }

        /// <summary>
        /// Get the SR field of the given PVS_cache_storage.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static XenRef<SR> get_SR(Session session, string _pvs_cache_storage)
        {
            return XenRef<SR>.Create(session.proxy.pvs_cache_storage_get_sr(session.uuid, (_pvs_cache_storage != null) ? _pvs_cache_storage : "").parse());
        }

        /// <summary>
        /// Get the site field of the given PVS_cache_storage.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static XenRef<PVS_site> get_site(Session session, string _pvs_cache_storage)
        {
            return XenRef<PVS_site>.Create(session.proxy.pvs_cache_storage_get_site(session.uuid, (_pvs_cache_storage != null) ? _pvs_cache_storage : "").parse());
        }

        /// <summary>
        /// Get the size field of the given PVS_cache_storage.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static long get_size(Session session, string _pvs_cache_storage)
        {
            return long.Parse((string)session.proxy.pvs_cache_storage_get_size(session.uuid, (_pvs_cache_storage != null) ? _pvs_cache_storage : "").parse());
        }

        /// <summary>
        /// Get the VDI field of the given PVS_cache_storage.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static XenRef<VDI> get_VDI(Session session, string _pvs_cache_storage)
        {
            return XenRef<VDI>.Create(session.proxy.pvs_cache_storage_get_vdi(session.uuid, (_pvs_cache_storage != null) ? _pvs_cache_storage : "").parse());
        }

        /// <summary>
        /// Return a list of all the PVS_cache_storages known to the system.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<PVS_cache_storage>> get_all(Session session)
        {
            return XenRef<PVS_cache_storage>.Create(session.proxy.pvs_cache_storage_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the PVS_cache_storage Records at once, in a single XML RPC call
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<PVS_cache_storage>, PVS_cache_storage> get_all_records(Session session)
        {
            return XenRef<PVS_cache_storage>.Create<Proxy_PVS_cache_storage>(session.proxy.pvs_cache_storage_get_all_records(session.uuid).parse());
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
        /// The host on which this object defines PVS cache storage
        /// Experimental. First published in .
        /// </summary>
        public virtual XenRef<Host> host
        {
            get { return _host; }
            set
            {
                if (!Helper.AreEqual(value, _host))
                {
                    _host = value;
                    Changed = true;
                    NotifyPropertyChanged("host");
                }
            }
        }
        private XenRef<Host> _host;

        /// <summary>
        /// SR providing storage for the PVS cache
        /// Experimental. First published in .
        /// </summary>
        public virtual XenRef<SR> SR
        {
            get { return _SR; }
            set
            {
                if (!Helper.AreEqual(value, _SR))
                {
                    _SR = value;
                    Changed = true;
                    NotifyPropertyChanged("SR");
                }
            }
        }
        private XenRef<SR> _SR;

        /// <summary>
        /// The PVS_site for which this object defines the storage
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
        /// The size of the cache VDI (in bytes)
        /// Experimental. First published in .
        /// </summary>
        public virtual long size
        {
            get { return _size; }
            set
            {
                if (!Helper.AreEqual(value, _size))
                {
                    _size = value;
                    Changed = true;
                    NotifyPropertyChanged("size");
                }
            }
        }
        private long _size;

        /// <summary>
        /// The VDI used for caching
        /// Experimental. First published in .
        /// </summary>
        public virtual XenRef<VDI> VDI
        {
            get { return _VDI; }
            set
            {
                if (!Helper.AreEqual(value, _VDI))
                {
                    _VDI = value;
                    Changed = true;
                    NotifyPropertyChanged("VDI");
                }
            }
        }
        private XenRef<VDI> _VDI;
    }
}
