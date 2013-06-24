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
    public partial class DR_task : XenObject<DR_task>
    {
        public DR_task()
        {
        }

        public DR_task(string uuid,
            List<XenRef<SR>> introduced_SRs)
        {
            this.uuid = uuid;
            this.introduced_SRs = introduced_SRs;
        }

        /// <summary>
        /// Creates a new DR_task from a Proxy_DR_task.
        /// </summary>
        /// <param name="proxy"></param>
        public DR_task(Proxy_DR_task proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(DR_task update)
        {
            uuid = update.uuid;
            introduced_SRs = update.introduced_SRs;
        }

        internal void UpdateFromProxy(Proxy_DR_task proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            introduced_SRs = proxy.introduced_SRs == null ? null : XenRef<SR>.Create(proxy.introduced_SRs);
        }

        public Proxy_DR_task ToProxy()
        {
            Proxy_DR_task result_ = new Proxy_DR_task();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.introduced_SRs = (introduced_SRs != null) ? Helper.RefListToStringArray(introduced_SRs) : new string[] {};
            return result_;
        }

        /// <summary>
        /// Creates a new DR_task from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public DR_task(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            introduced_SRs = Marshalling.ParseSetRef<SR>(table, "introduced_SRs");
        }

        public bool DeepEquals(DR_task other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._introduced_SRs, other._introduced_SRs);
        }

        public override string SaveChanges(Session session, string opaqueRef, DR_task server)
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

        public static DR_task get_record(Session session, string _dr_task)
        {
            return new DR_task((Proxy_DR_task)session.proxy.dr_task_get_record(session.uuid, (_dr_task != null) ? _dr_task : "").parse());
        }

        public static XenRef<DR_task> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<DR_task>.Create(session.proxy.dr_task_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static string get_uuid(Session session, string _dr_task)
        {
            return (string)session.proxy.dr_task_get_uuid(session.uuid, (_dr_task != null) ? _dr_task : "").parse();
        }

        public static List<XenRef<SR>> get_introduced_SRs(Session session, string _dr_task)
        {
            return XenRef<SR>.Create(session.proxy.dr_task_get_introduced_srs(session.uuid, (_dr_task != null) ? _dr_task : "").parse());
        }

        public static XenRef<DR_task> create(Session session, string _type, Dictionary<string, string> _device_config, string[] _whitelist)
        {
            return XenRef<DR_task>.Create(session.proxy.dr_task_create(session.uuid, (_type != null) ? _type : "", Maps.convert_to_proxy_string_string(_device_config), _whitelist).parse());
        }

        public static XenRef<Task> async_create(Session session, string _type, Dictionary<string, string> _device_config, string[] _whitelist)
        {
            return XenRef<Task>.Create(session.proxy.async_dr_task_create(session.uuid, (_type != null) ? _type : "", Maps.convert_to_proxy_string_string(_device_config), _whitelist).parse());
        }

        public static void destroy(Session session, string _self)
        {
            session.proxy.dr_task_destroy(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_dr_task_destroy(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static List<XenRef<DR_task>> get_all(Session session)
        {
            return XenRef<DR_task>.Create(session.proxy.dr_task_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<DR_task>, DR_task> get_all_records(Session session)
        {
            return XenRef<DR_task>.Create<Proxy_DR_task>(session.proxy.dr_task_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private List<XenRef<SR>> _introduced_SRs;
        public virtual List<XenRef<SR>> introduced_SRs {
             get { return _introduced_SRs; }
             set { if (!Helper.AreEqual(value, _introduced_SRs)) { _introduced_SRs = value; Changed = true; NotifyPropertyChanged("introduced_SRs"); } }
         }


    }
}
