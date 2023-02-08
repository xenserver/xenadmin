/* Copyright (c) Cloud Software Group, Inc.
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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;


namespace XenAPI
{
    /// <summary>
    /// A virtual TPM device
    /// First published in .
    /// </summary>
    public partial class VTPM : XenObject<VTPM>
    {
        #region Constructors

        public VTPM()
        {
        }

        public VTPM(string uuid,
            XenRef<VM> VM,
            XenRef<VM> backend,
            persistence_backend persistence_backend,
            bool is_unique,
            bool is_protected)
        {
            this.uuid = uuid;
            this.VM = VM;
            this.backend = backend;
            this.persistence_backend = persistence_backend;
            this.is_unique = is_unique;
            this.is_protected = is_protected;
        }

        /// <summary>
        /// Creates a new VTPM from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VTPM(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Creates a new VTPM from a Proxy_VTPM.
        /// </summary>
        /// <param name="proxy"></param>
        public VTPM(Proxy_VTPM proxy)
        {
            UpdateFrom(proxy);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VTPM.
        /// </summary>
        public override void UpdateFrom(VTPM record)
        {
            uuid = record.uuid;
            VM = record.VM;
            backend = record.backend;
            persistence_backend = record.persistence_backend;
            is_unique = record.is_unique;
            is_protected = record.is_protected;
        }

        internal void UpdateFrom(Proxy_VTPM proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            VM = proxy.VM == null ? null : XenRef<VM>.Create(proxy.VM);
            backend = proxy.backend == null ? null : XenRef<VM>.Create(proxy.backend);
            persistence_backend = proxy.persistence_backend == null ? (persistence_backend) 0 : (persistence_backend)Helper.EnumParseDefault(typeof(persistence_backend), (string)proxy.persistence_backend);
            is_unique = (bool)proxy.is_unique;
            is_protected = (bool)proxy.is_protected;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VTPM
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("VM"))
                VM = Marshalling.ParseRef<VM>(table, "VM");
            if (table.ContainsKey("backend"))
                backend = Marshalling.ParseRef<VM>(table, "backend");
            if (table.ContainsKey("persistence_backend"))
                persistence_backend = (persistence_backend)Helper.EnumParseDefault(typeof(persistence_backend), Marshalling.ParseString(table, "persistence_backend"));
            if (table.ContainsKey("is_unique"))
                is_unique = Marshalling.ParseBool(table, "is_unique");
            if (table.ContainsKey("is_protected"))
                is_protected = Marshalling.ParseBool(table, "is_protected");
        }

        public Proxy_VTPM ToProxy()
        {
            Proxy_VTPM result_ = new Proxy_VTPM();
            result_.uuid = uuid ?? "";
            result_.VM = VM ?? "";
            result_.backend = backend ?? "";
            result_.persistence_backend = persistence_backend_helper.ToString(persistence_backend);
            result_.is_unique = is_unique;
            result_.is_protected = is_protected;
            return result_;
        }

        public bool DeepEquals(VTPM other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._VM, other._VM) &&
                Helper.AreEqual2(this._backend, other._backend) &&
                Helper.AreEqual2(this._persistence_backend, other._persistence_backend) &&
                Helper.AreEqual2(this._is_unique, other._is_unique) &&
                Helper.AreEqual2(this._is_protected, other._is_protected);
        }

        public override string SaveChanges(Session session, string opaqueRef, VTPM server)
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
        /// Get a record containing the current state of the given VTPM.
        /// Experimental. First published in 22.26.0-next.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static VTPM get_record(Session session, string _vtpm)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_record(session.opaque_ref, _vtpm);
            else
                return new VTPM(session.XmlRpcProxy.vtpm_get_record(session.opaque_ref, _vtpm ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the VTPM instance with the specified UUID.
        /// Experimental. First published in 22.26.0-next.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VTPM> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<VTPM>.Create(session.XmlRpcProxy.vtpm_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VTPM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static string get_uuid(Session session, string _vtpm)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_uuid(session.opaque_ref, _vtpm);
            else
                return session.XmlRpcProxy.vtpm_get_uuid(session.opaque_ref, _vtpm ?? "").parse();
        }

        /// <summary>
        /// Get the VM field of the given VTPM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static XenRef<VM> get_VM(Session session, string _vtpm)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_vm(session.opaque_ref, _vtpm);
            else
                return XenRef<VM>.Create(session.XmlRpcProxy.vtpm_get_vm(session.opaque_ref, _vtpm ?? "").parse());
        }

        /// <summary>
        /// Get the backend field of the given VTPM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static XenRef<VM> get_backend(Session session, string _vtpm)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_backend(session.opaque_ref, _vtpm);
            else
                return XenRef<VM>.Create(session.XmlRpcProxy.vtpm_get_backend(session.opaque_ref, _vtpm ?? "").parse());
        }

        /// <summary>
        /// Get the persistence_backend field of the given VTPM.
        /// Experimental. First published in 22.26.0-next.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static persistence_backend get_persistence_backend(Session session, string _vtpm)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_persistence_backend(session.opaque_ref, _vtpm);
            else
                return (persistence_backend)Helper.EnumParseDefault(typeof(persistence_backend), (string)session.XmlRpcProxy.vtpm_get_persistence_backend(session.opaque_ref, _vtpm ?? "").parse());
        }

        /// <summary>
        /// Get the is_unique field of the given VTPM.
        /// Experimental. First published in 22.26.0-next.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static bool get_is_unique(Session session, string _vtpm)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_is_unique(session.opaque_ref, _vtpm);
            else
                return (bool)session.XmlRpcProxy.vtpm_get_is_unique(session.opaque_ref, _vtpm ?? "").parse();
        }

        /// <summary>
        /// Get the is_protected field of the given VTPM.
        /// Experimental. First published in 22.26.0-next.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static bool get_is_protected(Session session, string _vtpm)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_is_protected(session.opaque_ref, _vtpm);
            else
                return (bool)session.XmlRpcProxy.vtpm_get_is_protected(session.opaque_ref, _vtpm ?? "").parse();
        }

        /// <summary>
        /// Create a new VTPM instance, and return its handle.
        /// Experimental. First published in 22.26.0-next.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The VM reference the VTPM will be attached to</param>
        /// <param name="_is_unique">Whether the VTPM must be unique</param>
        public static XenRef<VTPM> create(Session session, string _vm, bool _is_unique)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_create(session.opaque_ref, _vm, _is_unique);
            else
                return XenRef<VTPM>.Create(session.XmlRpcProxy.vtpm_create(session.opaque_ref, _vm ?? "", _is_unique).parse());
        }

        /// <summary>
        /// Create a new VTPM instance, and return its handle.
        /// Experimental. First published in 22.26.0-next.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The VM reference the VTPM will be attached to</param>
        /// <param name="_is_unique">Whether the VTPM must be unique</param>
        public static XenRef<Task> async_create(Session session, string _vm, bool _is_unique)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vtpm_create(session.opaque_ref, _vm, _is_unique);
          else
              return XenRef<Task>.Create(session.XmlRpcProxy.async_vtpm_create(session.opaque_ref, _vm ?? "", _is_unique).parse());
        }

        /// <summary>
        /// Destroy the specified VTPM instance, along with its state.
        /// Experimental. First published in 22.26.0-next.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static void destroy(Session session, string _vtpm)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vtpm_destroy(session.opaque_ref, _vtpm);
            else
                session.XmlRpcProxy.vtpm_destroy(session.opaque_ref, _vtpm ?? "").parse();
        }

        /// <summary>
        /// Destroy the specified VTPM instance, along with its state.
        /// Experimental. First published in 22.26.0-next.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static XenRef<Task> async_destroy(Session session, string _vtpm)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vtpm_destroy(session.opaque_ref, _vtpm);
          else
              return XenRef<Task>.Create(session.XmlRpcProxy.async_vtpm_destroy(session.opaque_ref, _vtpm ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the VTPMs known to the system.
        /// Experimental. First published in 22.26.0-next.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<VTPM>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_all(session.opaque_ref);
            else
                return XenRef<VTPM>.Create(session.XmlRpcProxy.vtpm_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the VTPM Records at once, in a single XML RPC call
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<VTPM>, VTPM> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_all_records(session.opaque_ref);
            else
                return XenRef<VTPM>.Create<Proxy_VTPM>(session.XmlRpcProxy.vtpm_get_all_records(session.opaque_ref).parse());
        }

        /// <summary>
        /// Unique identifier/object reference
        /// First published in XenServer 4.0.
        /// </summary>
        public virtual string uuid
        {
            get { return _uuid; }
            set
            {
                if (!Helper.AreEqual(value, _uuid))
                {
                    _uuid = value;
                    NotifyPropertyChanged("uuid");
                }
            }
        }
        private string _uuid = "";

        /// <summary>
        /// The virtual machine the TPM is attached to
        /// First published in XenServer 4.0.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VM>))]
        public virtual XenRef<VM> VM
        {
            get { return _VM; }
            set
            {
                if (!Helper.AreEqual(value, _VM))
                {
                    _VM = value;
                    NotifyPropertyChanged("VM");
                }
            }
        }
        private XenRef<VM> _VM = new XenRef<VM>(Helper.NullOpaqueRef);

        /// <summary>
        /// The domain where the backend is located (unused)
        /// First published in XenServer 4.0.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VM>))]
        public virtual XenRef<VM> backend
        {
            get { return _backend; }
            set
            {
                if (!Helper.AreEqual(value, _backend))
                {
                    _backend = value;
                    NotifyPropertyChanged("backend");
                }
            }
        }
        private XenRef<VM> _backend = new XenRef<VM>("OpaqueRef:NULL");

        /// <summary>
        /// The backend where the vTPM is persisted
        /// Experimental. First published in 22.26.0-next.
        /// </summary>
        [JsonConverter(typeof(persistence_backendConverter))]
        public virtual persistence_backend persistence_backend
        {
            get { return _persistence_backend; }
            set
            {
                if (!Helper.AreEqual(value, _persistence_backend))
                {
                    _persistence_backend = value;
                    NotifyPropertyChanged("persistence_backend");
                }
            }
        }
        private persistence_backend _persistence_backend = persistence_backend.xapi;

        /// <summary>
        /// Whether the contents are never copied, satisfying the TPM spec
        /// Experimental. First published in 22.26.0-next.
        /// </summary>
        public virtual bool is_unique
        {
            get { return _is_unique; }
            set
            {
                if (!Helper.AreEqual(value, _is_unique))
                {
                    _is_unique = value;
                    NotifyPropertyChanged("is_unique");
                }
            }
        }
        private bool _is_unique = false;

        /// <summary>
        /// Whether the contents of the VTPM are secured according to the TPM spec
        /// Experimental. First published in 22.26.0-next.
        /// </summary>
        public virtual bool is_protected
        {
            get { return _is_protected; }
            set
            {
                if (!Helper.AreEqual(value, _is_protected))
                {
                    _is_protected = value;
                    NotifyPropertyChanged("is_protected");
                }
            }
        }
        private bool _is_protected = false;
    }
}
