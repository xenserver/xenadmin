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
    public partial class Host_crashdump : XenObject<Host_crashdump>
    {
        public Host_crashdump()
        {
        }

        public Host_crashdump(string uuid,
            XenRef<Host> host,
            DateTime timestamp,
            long size,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.host = host;
            this.timestamp = timestamp;
            this.size = size;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Host_crashdump from a Proxy_Host_crashdump.
        /// </summary>
        /// <param name="proxy"></param>
        public Host_crashdump(Proxy_Host_crashdump proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Host_crashdump update)
        {
            uuid = update.uuid;
            host = update.host;
            timestamp = update.timestamp;
            size = update.size;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_Host_crashdump proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
            timestamp = proxy.timestamp;
            size = proxy.size == null ? 0 : long.Parse((string)proxy.size);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_Host_crashdump ToProxy()
        {
            Proxy_Host_crashdump result_ = new Proxy_Host_crashdump();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.host = (host != null) ? host : "";
            result_.timestamp = timestamp;
            result_.size = size.ToString();
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new Host_crashdump from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Host_crashdump(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            host = Marshalling.ParseRef<Host>(table, "host");
            timestamp = Marshalling.ParseDateTime(table, "timestamp");
            size = Marshalling.ParseLong(table, "size");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Host_crashdump other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._host, other._host) &&
                Helper.AreEqual2(this._timestamp, other._timestamp) &&
                Helper.AreEqual2(this._size, other._size) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, Host_crashdump server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    Host_crashdump.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }

        public static Host_crashdump get_record(Session session, string _host_crashdump)
        {
            return new Host_crashdump((Proxy_Host_crashdump)session.proxy.host_crashdump_get_record(session.uuid, (_host_crashdump != null) ? _host_crashdump : "").parse());
        }

        public static XenRef<Host_crashdump> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Host_crashdump>.Create(session.proxy.host_crashdump_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static string get_uuid(Session session, string _host_crashdump)
        {
            return (string)session.proxy.host_crashdump_get_uuid(session.uuid, (_host_crashdump != null) ? _host_crashdump : "").parse();
        }

        public static XenRef<Host> get_host(Session session, string _host_crashdump)
        {
            return XenRef<Host>.Create(session.proxy.host_crashdump_get_host(session.uuid, (_host_crashdump != null) ? _host_crashdump : "").parse());
        }

        public static DateTime get_timestamp(Session session, string _host_crashdump)
        {
            return session.proxy.host_crashdump_get_timestamp(session.uuid, (_host_crashdump != null) ? _host_crashdump : "").parse();
        }

        public static long get_size(Session session, string _host_crashdump)
        {
            return long.Parse((string)session.proxy.host_crashdump_get_size(session.uuid, (_host_crashdump != null) ? _host_crashdump : "").parse());
        }

        public static Dictionary<string, string> get_other_config(Session session, string _host_crashdump)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.host_crashdump_get_other_config(session.uuid, (_host_crashdump != null) ? _host_crashdump : "").parse());
        }

        public static void set_other_config(Session session, string _host_crashdump, Dictionary<string, string> _other_config)
        {
            session.proxy.host_crashdump_set_other_config(session.uuid, (_host_crashdump != null) ? _host_crashdump : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _host_crashdump, string _key, string _value)
        {
            session.proxy.host_crashdump_add_to_other_config(session.uuid, (_host_crashdump != null) ? _host_crashdump : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _host_crashdump, string _key)
        {
            session.proxy.host_crashdump_remove_from_other_config(session.uuid, (_host_crashdump != null) ? _host_crashdump : "", (_key != null) ? _key : "").parse();
        }

        public static void destroy(Session session, string _self)
        {
            session.proxy.host_crashdump_destroy(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_host_crashdump_destroy(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static void upload(Session session, string _self, string _url, Dictionary<string, string> _options)
        {
            session.proxy.host_crashdump_upload(session.uuid, (_self != null) ? _self : "", (_url != null) ? _url : "", Maps.convert_to_proxy_string_string(_options)).parse();
        }

        public static XenRef<Task> async_upload(Session session, string _self, string _url, Dictionary<string, string> _options)
        {
            return XenRef<Task>.Create(session.proxy.async_host_crashdump_upload(session.uuid, (_self != null) ? _self : "", (_url != null) ? _url : "", Maps.convert_to_proxy_string_string(_options)).parse());
        }

        public static List<XenRef<Host_crashdump>> get_all(Session session)
        {
            return XenRef<Host_crashdump>.Create(session.proxy.host_crashdump_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<Host_crashdump>, Host_crashdump> get_all_records(Session session)
        {
            return XenRef<Host_crashdump>.Create<Proxy_Host_crashdump>(session.proxy.host_crashdump_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private XenRef<Host> _host;
        public virtual XenRef<Host> host {
             get { return _host; }
             set { if (!Helper.AreEqual(value, _host)) { _host = value; Changed = true; NotifyPropertyChanged("host"); } }
         }

        private DateTime _timestamp;
        public virtual DateTime timestamp {
             get { return _timestamp; }
             set { if (!Helper.AreEqual(value, _timestamp)) { _timestamp = value; Changed = true; NotifyPropertyChanged("timestamp"); } }
         }

        private long _size;
        public virtual long size {
             get { return _size; }
             set { if (!Helper.AreEqual(value, _size)) { _size = value; Changed = true; NotifyPropertyChanged("size"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }


    }
}
