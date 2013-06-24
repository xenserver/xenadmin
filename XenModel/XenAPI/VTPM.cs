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
    public partial class VTPM : XenObject<VTPM>
    {
        public VTPM()
        {
        }

        public VTPM(string uuid,
            XenRef<VM> VM,
            XenRef<VM> backend)
        {
            this.uuid = uuid;
            this.VM = VM;
            this.backend = backend;
        }

        /// <summary>
        /// Creates a new VTPM from a Proxy_VTPM.
        /// </summary>
        /// <param name="proxy"></param>
        public VTPM(Proxy_VTPM proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(VTPM update)
        {
            uuid = update.uuid;
            VM = update.VM;
            backend = update.backend;
        }

        internal void UpdateFromProxy(Proxy_VTPM proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            VM = proxy.VM == null ? null : XenRef<VM>.Create(proxy.VM);
            backend = proxy.backend == null ? null : XenRef<VM>.Create(proxy.backend);
        }

        public Proxy_VTPM ToProxy()
        {
            Proxy_VTPM result_ = new Proxy_VTPM();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.VM = (VM != null) ? VM : "";
            result_.backend = (backend != null) ? backend : "";
            return result_;
        }

        /// <summary>
        /// Creates a new VTPM from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public VTPM(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            VM = Marshalling.ParseRef<VM>(table, "VM");
            backend = Marshalling.ParseRef<VM>(table, "backend");
        }

        public bool DeepEquals(VTPM other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._VM, other._VM) &&
                Helper.AreEqual2(this._backend, other._backend);
        }

        public override string SaveChanges(Session session, string opaqueRef, VTPM server)
        {
            if (opaqueRef == null)
            {
                Proxy_VTPM p = this.ToProxy();
                return session.proxy.vtpm_create(session.uuid, p).parse();
            }
            else
            {
              throw new InvalidOperationException("This type has no read/write properties");
            }
        }

        public static VTPM get_record(Session session, string _vtpm)
        {
            return new VTPM((Proxy_VTPM)session.proxy.vtpm_get_record(session.uuid, (_vtpm != null) ? _vtpm : "").parse());
        }

        public static XenRef<VTPM> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<VTPM>.Create(session.proxy.vtpm_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static XenRef<VTPM> create(Session session, VTPM _record)
        {
            return XenRef<VTPM>.Create(session.proxy.vtpm_create(session.uuid, _record.ToProxy()).parse());
        }

        public static XenRef<Task> async_create(Session session, VTPM _record)
        {
            return XenRef<Task>.Create(session.proxy.async_vtpm_create(session.uuid, _record.ToProxy()).parse());
        }

        public static void destroy(Session session, string _vtpm)
        {
            session.proxy.vtpm_destroy(session.uuid, (_vtpm != null) ? _vtpm : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _vtpm)
        {
            return XenRef<Task>.Create(session.proxy.async_vtpm_destroy(session.uuid, (_vtpm != null) ? _vtpm : "").parse());
        }

        public static string get_uuid(Session session, string _vtpm)
        {
            return (string)session.proxy.vtpm_get_uuid(session.uuid, (_vtpm != null) ? _vtpm : "").parse();
        }

        public static XenRef<VM> get_VM(Session session, string _vtpm)
        {
            return XenRef<VM>.Create(session.proxy.vtpm_get_vm(session.uuid, (_vtpm != null) ? _vtpm : "").parse());
        }

        public static XenRef<VM> get_backend(Session session, string _vtpm)
        {
            return XenRef<VM>.Create(session.proxy.vtpm_get_backend(session.uuid, (_vtpm != null) ? _vtpm : "").parse());
        }

        public static Dictionary<XenRef<VTPM>, VTPM> get_all_records(Session session)
        {
            return XenRef<VTPM>.Create<Proxy_VTPM>(session.proxy.vtpm_get_all_records(session.uuid).parse());
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

        private XenRef<VM> _backend;
        public virtual XenRef<VM> backend {
             get { return _backend; }
             set { if (!Helper.AreEqual(value, _backend)) { _backend = value; Changed = true; NotifyPropertyChanged("backend"); } }
         }


    }
}
