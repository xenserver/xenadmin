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
    /// A virtual TPM device
    /// First published in XenServer 4.0.
    /// </summary>
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
        /// <summary>
        /// Get a record containing the current state of the given VTPM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static VTPM get_record(Session session, string _vtpm)
        {
            return new VTPM((Proxy_VTPM)session.proxy.vtpm_get_record(session.uuid, (_vtpm != null) ? _vtpm : "").parse());
        }

        /// <summary>
        /// Get a reference to the VTPM instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VTPM> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<VTPM>.Create(session.proxy.vtpm_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Create a new VTPM instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<VTPM> create(Session session, VTPM _record)
        {
            return XenRef<VTPM>.Create(session.proxy.vtpm_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Create a new VTPM instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Task> async_create(Session session, VTPM _record)
        {
            return XenRef<Task>.Create(session.proxy.async_vtpm_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Destroy the specified VTPM instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static void destroy(Session session, string _vtpm)
        {
            session.proxy.vtpm_destroy(session.uuid, (_vtpm != null) ? _vtpm : "").parse();
        }

        /// <summary>
        /// Destroy the specified VTPM instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static XenRef<Task> async_destroy(Session session, string _vtpm)
        {
            return XenRef<Task>.Create(session.proxy.async_vtpm_destroy(session.uuid, (_vtpm != null) ? _vtpm : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VTPM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static string get_uuid(Session session, string _vtpm)
        {
            return (string)session.proxy.vtpm_get_uuid(session.uuid, (_vtpm != null) ? _vtpm : "").parse();
        }

        /// <summary>
        /// Get the VM field of the given VTPM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static XenRef<VM> get_VM(Session session, string _vtpm)
        {
            return XenRef<VM>.Create(session.proxy.vtpm_get_vm(session.uuid, (_vtpm != null) ? _vtpm : "").parse());
        }

        /// <summary>
        /// Get the backend field of the given VTPM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static XenRef<VM> get_backend(Session session, string _vtpm)
        {
            return XenRef<VM>.Create(session.proxy.vtpm_get_backend(session.uuid, (_vtpm != null) ? _vtpm : "").parse());
        }

        /// <summary>
        /// Unique identifier/object reference
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
        /// the virtual machine
        /// </summary>
        public virtual XenRef<VM> VM
        {
            get { return _VM; }
            set
            {
                if (!Helper.AreEqual(value, _VM))
                {
                    _VM = value;
                    Changed = true;
                    NotifyPropertyChanged("VM");
                }
            }
        }
        private XenRef<VM> _VM;

        /// <summary>
        /// the domain where the backend is located
        /// </summary>
        public virtual XenRef<VM> backend
        {
            get { return _backend; }
            set
            {
                if (!Helper.AreEqual(value, _backend))
                {
                    _backend = value;
                    Changed = true;
                    NotifyPropertyChanged("backend");
                }
            }
        }
        private XenRef<VM> _backend;
    }
}
