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
    public partial class Role : XenObject<Role>
    {
        public Role()
        {
        }

        public Role(string uuid,
            string name_label,
            string name_description,
            List<XenRef<Role>> subroles)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.subroles = subroles;
        }

        /// <summary>
        /// Creates a new Role from a Proxy_Role.
        /// </summary>
        /// <param name="proxy"></param>
        public Role(Proxy_Role proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Role update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            subroles = update.subroles;
        }

        internal void UpdateFromProxy(Proxy_Role proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name_label = proxy.name_label == null ? null : (string)proxy.name_label;
            name_description = proxy.name_description == null ? null : (string)proxy.name_description;
            subroles = proxy.subroles == null ? null : XenRef<Role>.Create(proxy.subroles);
        }

        public Proxy_Role ToProxy()
        {
            Proxy_Role result_ = new Proxy_Role();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name_label = (name_label != null) ? name_label : "";
            result_.name_description = (name_description != null) ? name_description : "";
            result_.subroles = (subroles != null) ? Helper.RefListToStringArray(subroles) : new string[] {};
            return result_;
        }

        /// <summary>
        /// Creates a new Role from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Role(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            name_label = Marshalling.ParseString(table, "name_label");
            name_description = Marshalling.ParseString(table, "name_description");
            subroles = Marshalling.ParseSetRef<Role>(table, "subroles");
        }

        public bool DeepEquals(Role other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._subroles, other._subroles);
        }

        public override string SaveChanges(Session session, string opaqueRef, Role server)
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

        public static Role get_record(Session session, string _role)
        {
            return new Role((Proxy_Role)session.proxy.role_get_record(session.uuid, (_role != null) ? _role : "").parse());
        }

        public static XenRef<Role> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Role>.Create(session.proxy.role_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static List<XenRef<Role>> get_by_name_label(Session session, string _label)
        {
            return XenRef<Role>.Create(session.proxy.role_get_by_name_label(session.uuid, (_label != null) ? _label : "").parse());
        }

        public static string get_uuid(Session session, string _role)
        {
            return (string)session.proxy.role_get_uuid(session.uuid, (_role != null) ? _role : "").parse();
        }

        public static string get_name_label(Session session, string _role)
        {
            return (string)session.proxy.role_get_name_label(session.uuid, (_role != null) ? _role : "").parse();
        }

        public static string get_name_description(Session session, string _role)
        {
            return (string)session.proxy.role_get_name_description(session.uuid, (_role != null) ? _role : "").parse();
        }

        public static List<XenRef<Role>> get_subroles(Session session, string _role)
        {
            return XenRef<Role>.Create(session.proxy.role_get_subroles(session.uuid, (_role != null) ? _role : "").parse());
        }

        public static List<XenRef<Role>> get_permissions(Session session, string _self)
        {
            return XenRef<Role>.Create(session.proxy.role_get_permissions(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static string[] get_permissions_name_label(Session session, string _self)
        {
            return (string [])session.proxy.role_get_permissions_name_label(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static List<XenRef<Role>> get_by_permission(Session session, string _permission)
        {
            return XenRef<Role>.Create(session.proxy.role_get_by_permission(session.uuid, (_permission != null) ? _permission : "").parse());
        }

        public static List<XenRef<Role>> get_by_permission_name_label(Session session, string _label)
        {
            return XenRef<Role>.Create(session.proxy.role_get_by_permission_name_label(session.uuid, (_label != null) ? _label : "").parse());
        }

        public static List<XenRef<Role>> get_all(Session session)
        {
            return XenRef<Role>.Create(session.proxy.role_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<Role>, Role> get_all_records(Session session)
        {
            return XenRef<Role>.Create<Proxy_Role>(session.proxy.role_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private string _name_label;
        public virtual string name_label {
             get { return _name_label; }
             set { if (!Helper.AreEqual(value, _name_label)) { _name_label = value; Changed = true; NotifyPropertyChanged("name_label"); } }
         }

        private string _name_description;
        public virtual string name_description {
             get { return _name_description; }
             set { if (!Helper.AreEqual(value, _name_description)) { _name_description = value; Changed = true; NotifyPropertyChanged("name_description"); } }
         }

        private List<XenRef<Role>> _subroles;
        public virtual List<XenRef<Role>> subroles {
             get { return _subroles; }
             set { if (!Helper.AreEqual(value, _subroles)) { _subroles = value; Changed = true; NotifyPropertyChanged("subroles"); } }
         }


    }
}
