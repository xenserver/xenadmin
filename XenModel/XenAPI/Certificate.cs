/*
 * Copyright (c) Cloud Software Group, Inc.
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
    /// Description
    /// First published in Citrix Hypervisor 8.2.
    /// </summary>
    public partial class Certificate : XenObject<Certificate>
    {
        #region Constructors

        public Certificate()
        {
        }

        public Certificate(string uuid,
            string name,
            certificate_type type,
            XenRef<Host> host,
            DateTime not_before,
            DateTime not_after,
            string fingerprint)
        {
            this.uuid = uuid;
            this.name = name;
            this.type = type;
            this.host = host;
            this.not_before = not_before;
            this.not_after = not_after;
            this.fingerprint = fingerprint;
        }

        /// <summary>
        /// Creates a new Certificate from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Certificate(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Certificate.
        /// </summary>
        public override void UpdateFrom(Certificate record)
        {
            uuid = record.uuid;
            name = record.name;
            type = record.type;
            host = record.host;
            not_before = record.not_before;
            not_after = record.not_after;
            fingerprint = record.fingerprint;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Certificate
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("name"))
                name = Marshalling.ParseString(table, "name");
            if (table.ContainsKey("type"))
                type = (certificate_type)Helper.EnumParseDefault(typeof(certificate_type), Marshalling.ParseString(table, "type"));
            if (table.ContainsKey("host"))
                host = Marshalling.ParseRef<Host>(table, "host");
            if (table.ContainsKey("not_before"))
                not_before = Marshalling.ParseDateTime(table, "not_before");
            if (table.ContainsKey("not_after"))
                not_after = Marshalling.ParseDateTime(table, "not_after");
            if (table.ContainsKey("fingerprint"))
                fingerprint = Marshalling.ParseString(table, "fingerprint");
        }

        public bool DeepEquals(Certificate other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name, other._name) &&
                Helper.AreEqual2(_type, other._type) &&
                Helper.AreEqual2(_host, other._host) &&
                Helper.AreEqual2(_not_before, other._not_before) &&
                Helper.AreEqual2(_not_after, other._not_after) &&
                Helper.AreEqual2(_fingerprint, other._fingerprint);
        }

        public override string SaveChanges(Session session, string opaqueRef, Certificate server)
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
        /// Get a record containing the current state of the given Certificate.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_certificate">The opaque_ref of the given certificate</param>
        public static Certificate get_record(Session session, string _certificate)
        {
            return session.JsonRpcClient.certificate_get_record(session.opaque_ref, _certificate);
        }

        /// <summary>
        /// Get a reference to the Certificate instance with the specified UUID.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Certificate> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.certificate_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given Certificate.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_certificate">The opaque_ref of the given certificate</param>
        public static string get_uuid(Session session, string _certificate)
        {
            return session.JsonRpcClient.certificate_get_uuid(session.opaque_ref, _certificate);
        }

        /// <summary>
        /// Get the name field of the given Certificate.
        /// First published in 1.294.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_certificate">The opaque_ref of the given certificate</param>
        public static string get_name(Session session, string _certificate)
        {
            return session.JsonRpcClient.certificate_get_name(session.opaque_ref, _certificate);
        }

        /// <summary>
        /// Get the type field of the given Certificate.
        /// First published in 1.294.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_certificate">The opaque_ref of the given certificate</param>
        public static certificate_type get_type(Session session, string _certificate)
        {
            return session.JsonRpcClient.certificate_get_type(session.opaque_ref, _certificate);
        }

        /// <summary>
        /// Get the host field of the given Certificate.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_certificate">The opaque_ref of the given certificate</param>
        public static XenRef<Host> get_host(Session session, string _certificate)
        {
            return session.JsonRpcClient.certificate_get_host(session.opaque_ref, _certificate);
        }

        /// <summary>
        /// Get the not_before field of the given Certificate.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_certificate">The opaque_ref of the given certificate</param>
        public static DateTime get_not_before(Session session, string _certificate)
        {
            return session.JsonRpcClient.certificate_get_not_before(session.opaque_ref, _certificate);
        }

        /// <summary>
        /// Get the not_after field of the given Certificate.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_certificate">The opaque_ref of the given certificate</param>
        public static DateTime get_not_after(Session session, string _certificate)
        {
            return session.JsonRpcClient.certificate_get_not_after(session.opaque_ref, _certificate);
        }

        /// <summary>
        /// Get the fingerprint field of the given Certificate.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_certificate">The opaque_ref of the given certificate</param>
        public static string get_fingerprint(Session session, string _certificate)
        {
            return session.JsonRpcClient.certificate_get_fingerprint(session.opaque_ref, _certificate);
        }

        /// <summary>
        /// Return a list of all the Certificates known to the system.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Certificate>> get_all(Session session)
        {
            return session.JsonRpcClient.certificate_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Get all the Certificate Records at once, in a single XML RPC call
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Certificate>, Certificate> get_all_records(Session session)
        {
            return session.JsonRpcClient.certificate_get_all_records(session.opaque_ref);
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
                    NotifyPropertyChanged("uuid");
                }
            }
        }
        private string _uuid = "";

        /// <summary>
        /// The name of the certificate, only present on certificates of type 'ca'
        /// First published in 1.294.0.
        /// </summary>
        public virtual string name
        {
            get { return _name; }
            set
            {
                if (!Helper.AreEqual(value, _name))
                {
                    _name = value;
                    NotifyPropertyChanged("name");
                }
            }
        }
        private string _name = "";

        /// <summary>
        /// The type of the certificate, either 'ca', 'host' or 'host_internal'
        /// First published in 1.294.0.
        /// </summary>
        [JsonConverter(typeof(certificate_typeConverter))]
        public virtual certificate_type type
        {
            get { return _type; }
            set
            {
                if (!Helper.AreEqual(value, _type))
                {
                    _type = value;
                    NotifyPropertyChanged("type");
                }
            }
        }
        private certificate_type _type = certificate_type.host;

        /// <summary>
        /// The host where the certificate is installed
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Host>))]
        public virtual XenRef<Host> host
        {
            get { return _host; }
            set
            {
                if (!Helper.AreEqual(value, _host))
                {
                    _host = value;
                    NotifyPropertyChanged("host");
                }
            }
        }
        private XenRef<Host> _host = new XenRef<Host>("OpaqueRef:NULL");

        /// <summary>
        /// Date after which the certificate is valid
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime not_before
        {
            get { return _not_before; }
            set
            {
                if (!Helper.AreEqual(value, _not_before))
                {
                    _not_before = value;
                    NotifyPropertyChanged("not_before");
                }
            }
        }
        private DateTime _not_before = DateTime.ParseExact("19700101T00:00:00Z", "yyyyMMddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        /// <summary>
        /// Date before which the certificate is valid
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime not_after
        {
            get { return _not_after; }
            set
            {
                if (!Helper.AreEqual(value, _not_after))
                {
                    _not_after = value;
                    NotifyPropertyChanged("not_after");
                }
            }
        }
        private DateTime _not_after = DateTime.ParseExact("19700101T00:00:00Z", "yyyyMMddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        /// <summary>
        /// The certificate's fingerprint / hash
        /// </summary>
        public virtual string fingerprint
        {
            get { return _fingerprint; }
            set
            {
                if (!Helper.AreEqual(value, _fingerprint))
                {
                    _fingerprint = value;
                    NotifyPropertyChanged("fingerprint");
                }
            }
        }
        private string _fingerprint = "";
    }
}
