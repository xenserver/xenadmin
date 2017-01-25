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
    /// individual machine serving provisioning (block) data
    /// </summary>
    public partial class PVS_server : XenObject<PVS_server>
    {
        public PVS_server()
        {
        }

        public PVS_server(string uuid,
            string[] addresses,
            long first_port,
            long last_port,
            XenRef<PVS_site> site)
        {
            this.uuid = uuid;
            this.addresses = addresses;
            this.first_port = first_port;
            this.last_port = last_port;
            this.site = site;
        }

        /// <summary>
        /// Creates a new PVS_server from a Proxy_PVS_server.
        /// </summary>
        /// <param name="proxy"></param>
        public PVS_server(Proxy_PVS_server proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(PVS_server update)
        {
            uuid = update.uuid;
            addresses = update.addresses;
            first_port = update.first_port;
            last_port = update.last_port;
            site = update.site;
        }

        internal void UpdateFromProxy(Proxy_PVS_server proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            addresses = proxy.addresses == null ? new string[] {} : (string [])proxy.addresses;
            first_port = proxy.first_port == null ? 0 : long.Parse((string)proxy.first_port);
            last_port = proxy.last_port == null ? 0 : long.Parse((string)proxy.last_port);
            site = proxy.site == null ? null : XenRef<PVS_site>.Create(proxy.site);
        }

        public Proxy_PVS_server ToProxy()
        {
            Proxy_PVS_server result_ = new Proxy_PVS_server();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.addresses = addresses;
            result_.first_port = first_port.ToString();
            result_.last_port = last_port.ToString();
            result_.site = (site != null) ? site : "";
            return result_;
        }

        /// <summary>
        /// Creates a new PVS_server from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public PVS_server(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            addresses = Marshalling.ParseStringArray(table, "addresses");
            first_port = Marshalling.ParseLong(table, "first_port");
            last_port = Marshalling.ParseLong(table, "last_port");
            site = Marshalling.ParseRef<PVS_site>(table, "site");
        }

        public bool DeepEquals(PVS_server other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._addresses, other._addresses) &&
                Helper.AreEqual2(this._first_port, other._first_port) &&
                Helper.AreEqual2(this._last_port, other._last_port) &&
                Helper.AreEqual2(this._site, other._site);
        }

        public override string SaveChanges(Session session, string opaqueRef, PVS_server server)
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
        /// Get a record containing the current state of the given PVS_server.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        public static PVS_server get_record(Session session, string _pvs_server)
        {
            return new PVS_server((Proxy_PVS_server)session.proxy.pvs_server_get_record(session.uuid, (_pvs_server != null) ? _pvs_server : "").parse());
        }

        /// <summary>
        /// Get a reference to the PVS_server instance with the specified UUID.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<PVS_server> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<PVS_server>.Create(session.proxy.pvs_server_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given PVS_server.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        public static string get_uuid(Session session, string _pvs_server)
        {
            return (string)session.proxy.pvs_server_get_uuid(session.uuid, (_pvs_server != null) ? _pvs_server : "").parse();
        }

        /// <summary>
        /// Get the addresses field of the given PVS_server.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        public static string[] get_addresses(Session session, string _pvs_server)
        {
            return (string [])session.proxy.pvs_server_get_addresses(session.uuid, (_pvs_server != null) ? _pvs_server : "").parse();
        }

        /// <summary>
        /// Get the first_port field of the given PVS_server.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        public static long get_first_port(Session session, string _pvs_server)
        {
            return long.Parse((string)session.proxy.pvs_server_get_first_port(session.uuid, (_pvs_server != null) ? _pvs_server : "").parse());
        }

        /// <summary>
        /// Get the last_port field of the given PVS_server.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        public static long get_last_port(Session session, string _pvs_server)
        {
            return long.Parse((string)session.proxy.pvs_server_get_last_port(session.uuid, (_pvs_server != null) ? _pvs_server : "").parse());
        }

        /// <summary>
        /// Get the site field of the given PVS_server.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        public static XenRef<PVS_site> get_site(Session session, string _pvs_server)
        {
            return XenRef<PVS_site>.Create(session.proxy.pvs_server_get_site(session.uuid, (_pvs_server != null) ? _pvs_server : "").parse());
        }

        /// <summary>
        /// introduce new PVS server
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_addresses">IPv4 addresses of the server</param>
        /// <param name="_first_port">first UDP port accepted by this server</param>
        /// <param name="_last_port">last UDP port accepted by this server</param>
        /// <param name="_site">PVS site this server is a part of</param>
        public static XenRef<PVS_server> introduce(Session session, string[] _addresses, long _first_port, long _last_port, string _site)
        {
            return XenRef<PVS_server>.Create(session.proxy.pvs_server_introduce(session.uuid, _addresses, _first_port.ToString(), _last_port.ToString(), (_site != null) ? _site : "").parse());
        }

        /// <summary>
        /// introduce new PVS server
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_addresses">IPv4 addresses of the server</param>
        /// <param name="_first_port">first UDP port accepted by this server</param>
        /// <param name="_last_port">last UDP port accepted by this server</param>
        /// <param name="_site">PVS site this server is a part of</param>
        public static XenRef<Task> async_introduce(Session session, string[] _addresses, long _first_port, long _last_port, string _site)
        {
            return XenRef<Task>.Create(session.proxy.async_pvs_server_introduce(session.uuid, _addresses, _first_port.ToString(), _last_port.ToString(), (_site != null) ? _site : "").parse());
        }

        /// <summary>
        /// forget a PVS server
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        public static void forget(Session session, string _pvs_server)
        {
            session.proxy.pvs_server_forget(session.uuid, (_pvs_server != null) ? _pvs_server : "").parse();
        }

        /// <summary>
        /// forget a PVS server
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        public static XenRef<Task> async_forget(Session session, string _pvs_server)
        {
            return XenRef<Task>.Create(session.proxy.async_pvs_server_forget(session.uuid, (_pvs_server != null) ? _pvs_server : "").parse());
        }

        /// <summary>
        /// Return a list of all the PVS_servers known to the system.
        /// Experimental. First published in .
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<PVS_server>> get_all(Session session)
        {
            return XenRef<PVS_server>.Create(session.proxy.pvs_server_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the PVS_server Records at once, in a single XML RPC call
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<PVS_server>, PVS_server> get_all_records(Session session)
        {
            return XenRef<PVS_server>.Create<Proxy_PVS_server>(session.proxy.pvs_server_get_all_records(session.uuid).parse());
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
        /// IPv4 addresses of this server
        /// Experimental. First published in .
        /// </summary>
        public virtual string[] addresses
        {
            get { return _addresses; }
            set
            {
                if (!Helper.AreEqual(value, _addresses))
                {
                    _addresses = value;
                    Changed = true;
                    NotifyPropertyChanged("addresses");
                }
            }
        }
        private string[] _addresses;

        /// <summary>
        /// First UDP port accepted by this server
        /// Experimental. First published in .
        /// </summary>
        public virtual long first_port
        {
            get { return _first_port; }
            set
            {
                if (!Helper.AreEqual(value, _first_port))
                {
                    _first_port = value;
                    Changed = true;
                    NotifyPropertyChanged("first_port");
                }
            }
        }
        private long _first_port;

        /// <summary>
        /// Last UDP port accepted by this server
        /// Experimental. First published in .
        /// </summary>
        public virtual long last_port
        {
            get { return _last_port; }
            set
            {
                if (!Helper.AreEqual(value, _last_port))
                {
                    _last_port = value;
                    Changed = true;
                    NotifyPropertyChanged("last_port");
                }
            }
        }
        private long _last_port;

        /// <summary>
        /// PVS site this server is part of
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
    }
}
