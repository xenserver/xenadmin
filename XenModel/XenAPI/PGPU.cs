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
    public partial class PGPU : XenObject<PGPU>
    {
        public PGPU()
        {
        }

        public PGPU(string uuid,
            XenRef<PCI> PCI,
            XenRef<GPU_group> GPU_group,
            XenRef<Host> host,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.PCI = PCI;
            this.GPU_group = GPU_group;
            this.host = host;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new PGPU from a Proxy_PGPU.
        /// </summary>
        /// <param name="proxy"></param>
        public PGPU(Proxy_PGPU proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(PGPU update)
        {
            uuid = update.uuid;
            PCI = update.PCI;
            GPU_group = update.GPU_group;
            host = update.host;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_PGPU proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            PCI = proxy.PCI == null ? null : XenRef<PCI>.Create(proxy.PCI);
            GPU_group = proxy.GPU_group == null ? null : XenRef<GPU_group>.Create(proxy.GPU_group);
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_PGPU ToProxy()
        {
            Proxy_PGPU result_ = new Proxy_PGPU();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.PCI = (PCI != null) ? PCI : "";
            result_.GPU_group = (GPU_group != null) ? GPU_group : "";
            result_.host = (host != null) ? host : "";
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new PGPU from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public PGPU(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            PCI = Marshalling.ParseRef<PCI>(table, "PCI");
            GPU_group = Marshalling.ParseRef<GPU_group>(table, "GPU_group");
            host = Marshalling.ParseRef<Host>(table, "host");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(PGPU other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._PCI, other._PCI) &&
                Helper.AreEqual2(this._GPU_group, other._GPU_group) &&
                Helper.AreEqual2(this._host, other._host) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, PGPU server)
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
                    PGPU.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }

        public static PGPU get_record(Session session, string _pgpu)
        {
            return new PGPU((Proxy_PGPU)session.proxy.pgpu_get_record(session.uuid, (_pgpu != null) ? _pgpu : "").parse());
        }

        public static XenRef<PGPU> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<PGPU>.Create(session.proxy.pgpu_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static string get_uuid(Session session, string _pgpu)
        {
            return (string)session.proxy.pgpu_get_uuid(session.uuid, (_pgpu != null) ? _pgpu : "").parse();
        }

        public static XenRef<PCI> get_PCI(Session session, string _pgpu)
        {
            return XenRef<PCI>.Create(session.proxy.pgpu_get_pci(session.uuid, (_pgpu != null) ? _pgpu : "").parse());
        }

        public static XenRef<GPU_group> get_GPU_group(Session session, string _pgpu)
        {
            return XenRef<GPU_group>.Create(session.proxy.pgpu_get_gpu_group(session.uuid, (_pgpu != null) ? _pgpu : "").parse());
        }

        public static XenRef<Host> get_host(Session session, string _pgpu)
        {
            return XenRef<Host>.Create(session.proxy.pgpu_get_host(session.uuid, (_pgpu != null) ? _pgpu : "").parse());
        }

        public static Dictionary<string, string> get_other_config(Session session, string _pgpu)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pgpu_get_other_config(session.uuid, (_pgpu != null) ? _pgpu : "").parse());
        }

        public static void set_other_config(Session session, string _pgpu, Dictionary<string, string> _other_config)
        {
            session.proxy.pgpu_set_other_config(session.uuid, (_pgpu != null) ? _pgpu : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _pgpu, string _key, string _value)
        {
            session.proxy.pgpu_add_to_other_config(session.uuid, (_pgpu != null) ? _pgpu : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _pgpu, string _key)
        {
            session.proxy.pgpu_remove_from_other_config(session.uuid, (_pgpu != null) ? _pgpu : "", (_key != null) ? _key : "").parse();
        }

        public static List<XenRef<PGPU>> get_all(Session session)
        {
            return XenRef<PGPU>.Create(session.proxy.pgpu_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<PGPU>, PGPU> get_all_records(Session session)
        {
            return XenRef<PGPU>.Create<Proxy_PGPU>(session.proxy.pgpu_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private XenRef<PCI> _PCI;
        public virtual XenRef<PCI> PCI {
             get { return _PCI; }
             set { if (!Helper.AreEqual(value, _PCI)) { _PCI = value; Changed = true; NotifyPropertyChanged("PCI"); } }
         }

        private XenRef<GPU_group> _GPU_group;
        public virtual XenRef<GPU_group> GPU_group {
             get { return _GPU_group; }
             set { if (!Helper.AreEqual(value, _GPU_group)) { _GPU_group = value; Changed = true; NotifyPropertyChanged("GPU_group"); } }
         }

        private XenRef<Host> _host;
        public virtual XenRef<Host> host {
             get { return _host; }
             set { if (!Helper.AreEqual(value, _host)) { _host = value; Changed = true; NotifyPropertyChanged("host"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }


    }
}
