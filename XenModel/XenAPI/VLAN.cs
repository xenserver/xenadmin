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
    public partial class VLAN : XenObject<VLAN>
    {
        public VLAN()
        {
        }

        public VLAN(string uuid,
            XenRef<PIF> tagged_PIF,
            XenRef<PIF> untagged_PIF,
            long tag,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.tagged_PIF = tagged_PIF;
            this.untagged_PIF = untagged_PIF;
            this.tag = tag;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new VLAN from a Proxy_VLAN.
        /// </summary>
        /// <param name="proxy"></param>
        public VLAN(Proxy_VLAN proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(VLAN update)
        {
            uuid = update.uuid;
            tagged_PIF = update.tagged_PIF;
            untagged_PIF = update.untagged_PIF;
            tag = update.tag;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_VLAN proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            tagged_PIF = proxy.tagged_PIF == null ? null : XenRef<PIF>.Create(proxy.tagged_PIF);
            untagged_PIF = proxy.untagged_PIF == null ? null : XenRef<PIF>.Create(proxy.untagged_PIF);
            tag = proxy.tag == null ? 0 : long.Parse((string)proxy.tag);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_VLAN ToProxy()
        {
            Proxy_VLAN result_ = new Proxy_VLAN();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.tagged_PIF = (tagged_PIF != null) ? tagged_PIF : "";
            result_.untagged_PIF = (untagged_PIF != null) ? untagged_PIF : "";
            result_.tag = tag.ToString();
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new VLAN from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public VLAN(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            tagged_PIF = Marshalling.ParseRef<PIF>(table, "tagged_PIF");
            untagged_PIF = Marshalling.ParseRef<PIF>(table, "untagged_PIF");
            tag = Marshalling.ParseLong(table, "tag");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(VLAN other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._tagged_PIF, other._tagged_PIF) &&
                Helper.AreEqual2(this._untagged_PIF, other._untagged_PIF) &&
                Helper.AreEqual2(this._tag, other._tag) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, VLAN server)
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
                    VLAN.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }

        public static VLAN get_record(Session session, string _vlan)
        {
            return new VLAN((Proxy_VLAN)session.proxy.vlan_get_record(session.uuid, (_vlan != null) ? _vlan : "").parse());
        }

        public static XenRef<VLAN> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<VLAN>.Create(session.proxy.vlan_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static string get_uuid(Session session, string _vlan)
        {
            return (string)session.proxy.vlan_get_uuid(session.uuid, (_vlan != null) ? _vlan : "").parse();
        }

        public static XenRef<PIF> get_tagged_PIF(Session session, string _vlan)
        {
            return XenRef<PIF>.Create(session.proxy.vlan_get_tagged_pif(session.uuid, (_vlan != null) ? _vlan : "").parse());
        }

        public static XenRef<PIF> get_untagged_PIF(Session session, string _vlan)
        {
            return XenRef<PIF>.Create(session.proxy.vlan_get_untagged_pif(session.uuid, (_vlan != null) ? _vlan : "").parse());
        }

        public static long get_tag(Session session, string _vlan)
        {
            return long.Parse((string)session.proxy.vlan_get_tag(session.uuid, (_vlan != null) ? _vlan : "").parse());
        }

        public static Dictionary<string, string> get_other_config(Session session, string _vlan)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vlan_get_other_config(session.uuid, (_vlan != null) ? _vlan : "").parse());
        }

        public static void set_other_config(Session session, string _vlan, Dictionary<string, string> _other_config)
        {
            session.proxy.vlan_set_other_config(session.uuid, (_vlan != null) ? _vlan : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _vlan, string _key, string _value)
        {
            session.proxy.vlan_add_to_other_config(session.uuid, (_vlan != null) ? _vlan : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _vlan, string _key)
        {
            session.proxy.vlan_remove_from_other_config(session.uuid, (_vlan != null) ? _vlan : "", (_key != null) ? _key : "").parse();
        }

        public static XenRef<VLAN> create(Session session, string _tagged_pif, long _tag, string _network)
        {
            return XenRef<VLAN>.Create(session.proxy.vlan_create(session.uuid, (_tagged_pif != null) ? _tagged_pif : "", _tag.ToString(), (_network != null) ? _network : "").parse());
        }

        public static XenRef<Task> async_create(Session session, string _tagged_pif, long _tag, string _network)
        {
            return XenRef<Task>.Create(session.proxy.async_vlan_create(session.uuid, (_tagged_pif != null) ? _tagged_pif : "", _tag.ToString(), (_network != null) ? _network : "").parse());
        }

        public static void destroy(Session session, string _self)
        {
            session.proxy.vlan_destroy(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_vlan_destroy(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static List<XenRef<VLAN>> get_all(Session session)
        {
            return XenRef<VLAN>.Create(session.proxy.vlan_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<VLAN>, VLAN> get_all_records(Session session)
        {
            return XenRef<VLAN>.Create<Proxy_VLAN>(session.proxy.vlan_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private XenRef<PIF> _tagged_PIF;
        public virtual XenRef<PIF> tagged_PIF {
             get { return _tagged_PIF; }
             set { if (!Helper.AreEqual(value, _tagged_PIF)) { _tagged_PIF = value; Changed = true; NotifyPropertyChanged("tagged_PIF"); } }
         }

        private XenRef<PIF> _untagged_PIF;
        public virtual XenRef<PIF> untagged_PIF {
             get { return _untagged_PIF; }
             set { if (!Helper.AreEqual(value, _untagged_PIF)) { _untagged_PIF = value; Changed = true; NotifyPropertyChanged("untagged_PIF"); } }
         }

        private long _tag;
        public virtual long tag {
             get { return _tag; }
             set { if (!Helper.AreEqual(value, _tag)) { _tag = value; Changed = true; NotifyPropertyChanged("tag"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }


    }
}
