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
    public partial class Crashdump : XenObject<Crashdump>
    {
        public Crashdump()
        {
        }

        public Crashdump(string uuid,
            XenRef<VM> VM,
            XenRef<VDI> VDI,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.VM = VM;
            this.VDI = VDI;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Crashdump from a Proxy_Crashdump.
        /// </summary>
        /// <param name="proxy"></param>
        public Crashdump(Proxy_Crashdump proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Crashdump update)
        {
            uuid = update.uuid;
            VM = update.VM;
            VDI = update.VDI;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_Crashdump proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            VM = proxy.VM == null ? null : XenRef<VM>.Create(proxy.VM);
            VDI = proxy.VDI == null ? null : XenRef<VDI>.Create(proxy.VDI);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_Crashdump ToProxy()
        {
            Proxy_Crashdump result_ = new Proxy_Crashdump();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.VM = (VM != null) ? VM : "";
            result_.VDI = (VDI != null) ? VDI : "";
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new Crashdump from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Crashdump(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            VM = Marshalling.ParseRef<VM>(table, "VM");
            VDI = Marshalling.ParseRef<VDI>(table, "VDI");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Crashdump other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._VM, other._VM) &&
                Helper.AreEqual2(this._VDI, other._VDI) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, Crashdump server)
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
                    Crashdump.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }

        public static Crashdump get_record(Session session, string _crashdump)
        {
            return new Crashdump((Proxy_Crashdump)session.proxy.crashdump_get_record(session.uuid, (_crashdump != null) ? _crashdump : "").parse());
        }

        public static XenRef<Crashdump> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Crashdump>.Create(session.proxy.crashdump_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static string get_uuid(Session session, string _crashdump)
        {
            return (string)session.proxy.crashdump_get_uuid(session.uuid, (_crashdump != null) ? _crashdump : "").parse();
        }

        public static XenRef<VM> get_VM(Session session, string _crashdump)
        {
            return XenRef<VM>.Create(session.proxy.crashdump_get_vm(session.uuid, (_crashdump != null) ? _crashdump : "").parse());
        }

        public static XenRef<VDI> get_VDI(Session session, string _crashdump)
        {
            return XenRef<VDI>.Create(session.proxy.crashdump_get_vdi(session.uuid, (_crashdump != null) ? _crashdump : "").parse());
        }

        public static Dictionary<string, string> get_other_config(Session session, string _crashdump)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.crashdump_get_other_config(session.uuid, (_crashdump != null) ? _crashdump : "").parse());
        }

        public static void set_other_config(Session session, string _crashdump, Dictionary<string, string> _other_config)
        {
            session.proxy.crashdump_set_other_config(session.uuid, (_crashdump != null) ? _crashdump : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _crashdump, string _key, string _value)
        {
            session.proxy.crashdump_add_to_other_config(session.uuid, (_crashdump != null) ? _crashdump : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _crashdump, string _key)
        {
            session.proxy.crashdump_remove_from_other_config(session.uuid, (_crashdump != null) ? _crashdump : "", (_key != null) ? _key : "").parse();
        }

        public static void destroy(Session session, string _self)
        {
            session.proxy.crashdump_destroy(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_crashdump_destroy(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static List<XenRef<Crashdump>> get_all(Session session)
        {
            return XenRef<Crashdump>.Create(session.proxy.crashdump_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<Crashdump>, Crashdump> get_all_records(Session session)
        {
            return XenRef<Crashdump>.Create<Proxy_Crashdump>(session.proxy.crashdump_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private XenRef<VM> _VM;
        public virtual XenRef<VM> VM {
             get { return _VM; }
             set { if (!Helper.AreEqual(value, _VM)) { _VM = value; Changed = true; NotifyPropertyChanged("VM"); } }
         }

        private XenRef<VDI> _VDI;
        public virtual XenRef<VDI> VDI {
             get { return _VDI; }
             set { if (!Helper.AreEqual(value, _VDI)) { _VDI = value; Changed = true; NotifyPropertyChanged("VDI"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }


    }
}
