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
    /// A new piece of functionality
    /// First published in .
    /// </summary>
    public partial class Feature : XenObject<Feature>
    {
        public Feature()
        {
        }

        public Feature(string uuid,
            string name_label,
            string name_description,
            bool enabled,
            bool experimental,
            string version,
            XenRef<Host> host)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.enabled = enabled;
            this.experimental = experimental;
            this.version = version;
            this.host = host;
        }

        /// <summary>
        /// Creates a new Feature from a Proxy_Feature.
        /// </summary>
        /// <param name="proxy"></param>
        public Feature(Proxy_Feature proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Feature update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            enabled = update.enabled;
            experimental = update.experimental;
            version = update.version;
            host = update.host;
        }

        internal void UpdateFromProxy(Proxy_Feature proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name_label = proxy.name_label == null ? null : (string)proxy.name_label;
            name_description = proxy.name_description == null ? null : (string)proxy.name_description;
            enabled = (bool)proxy.enabled;
            experimental = (bool)proxy.experimental;
            version = proxy.version == null ? null : (string)proxy.version;
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
        }

        public Proxy_Feature ToProxy()
        {
            Proxy_Feature result_ = new Proxy_Feature();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name_label = (name_label != null) ? name_label : "";
            result_.name_description = (name_description != null) ? name_description : "";
            result_.enabled = enabled;
            result_.experimental = experimental;
            result_.version = (version != null) ? version : "";
            result_.host = (host != null) ? host : "";
            return result_;
        }

        /// <summary>
        /// Creates a new Feature from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Feature(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            name_label = Marshalling.ParseString(table, "name_label");
            name_description = Marshalling.ParseString(table, "name_description");
            enabled = Marshalling.ParseBool(table, "enabled");
            experimental = Marshalling.ParseBool(table, "experimental");
            version = Marshalling.ParseString(table, "version");
            host = Marshalling.ParseRef<Host>(table, "host");
        }

        public bool DeepEquals(Feature other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._enabled, other._enabled) &&
                Helper.AreEqual2(this._experimental, other._experimental) &&
                Helper.AreEqual2(this._version, other._version) &&
                Helper.AreEqual2(this._host, other._host);
        }

        public override string SaveChanges(Session session, string opaqueRef, Feature server)
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
        /// Get a record containing the current state of the given Feature.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static Feature get_record(Session session, string _feature)
        {
            return new Feature((Proxy_Feature)session.proxy.feature_get_record(session.uuid, (_feature != null) ? _feature : "").parse());
        }

        /// <summary>
        /// Get a reference to the Feature instance with the specified UUID.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Feature> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Feature>.Create(session.proxy.feature_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Get all the Feature instances with the given label.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        public static List<XenRef<Feature>> get_by_name_label(Session session, string _label)
        {
            return XenRef<Feature>.Create(session.proxy.feature_get_by_name_label(session.uuid, (_label != null) ? _label : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given Feature.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static string get_uuid(Session session, string _feature)
        {
            return (string)session.proxy.feature_get_uuid(session.uuid, (_feature != null) ? _feature : "").parse();
        }

        /// <summary>
        /// Get the name/label field of the given Feature.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static string get_name_label(Session session, string _feature)
        {
            return (string)session.proxy.feature_get_name_label(session.uuid, (_feature != null) ? _feature : "").parse();
        }

        /// <summary>
        /// Get the name/description field of the given Feature.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static string get_name_description(Session session, string _feature)
        {
            return (string)session.proxy.feature_get_name_description(session.uuid, (_feature != null) ? _feature : "").parse();
        }

        /// <summary>
        /// Get the enabled field of the given Feature.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static bool get_enabled(Session session, string _feature)
        {
            return (bool)session.proxy.feature_get_enabled(session.uuid, (_feature != null) ? _feature : "").parse();
        }

        /// <summary>
        /// Get the experimental field of the given Feature.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static bool get_experimental(Session session, string _feature)
        {
            return (bool)session.proxy.feature_get_experimental(session.uuid, (_feature != null) ? _feature : "").parse();
        }

        /// <summary>
        /// Get the version field of the given Feature.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static string get_version(Session session, string _feature)
        {
            return (string)session.proxy.feature_get_version(session.uuid, (_feature != null) ? _feature : "").parse();
        }

        /// <summary>
        /// Get the host field of the given Feature.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static XenRef<Host> get_host(Session session, string _feature)
        {
            return XenRef<Host>.Create(session.proxy.feature_get_host(session.uuid, (_feature != null) ? _feature : "").parse());
        }

        /// <summary>
        /// Return a list of all the Features known to the system.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Feature>> get_all(Session session)
        {
            return XenRef<Feature>.Create(session.proxy.feature_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the Feature Records at once, in a single XML RPC call
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Feature>, Feature> get_all_records(Session session)
        {
            return XenRef<Feature>.Create<Proxy_Feature>(session.proxy.feature_get_all_records(session.uuid).parse());
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
        /// a human-readable name
        /// </summary>
        public virtual string name_label
        {
            get { return _name_label; }
            set
            {
                if (!Helper.AreEqual(value, _name_label))
                {
                    _name_label = value;
                    Changed = true;
                    NotifyPropertyChanged("name_label");
                }
            }
        }
        private string _name_label;

        /// <summary>
        /// a notes field containing human-readable description
        /// </summary>
        public virtual string name_description
        {
            get { return _name_description; }
            set
            {
                if (!Helper.AreEqual(value, _name_description))
                {
                    _name_description = value;
                    Changed = true;
                    NotifyPropertyChanged("name_description");
                }
            }
        }
        private string _name_description;

        /// <summary>
        /// Indicates whether the feature is enabled
        /// </summary>
        public virtual bool enabled
        {
            get { return _enabled; }
            set
            {
                if (!Helper.AreEqual(value, _enabled))
                {
                    _enabled = value;
                    Changed = true;
                    NotifyPropertyChanged("enabled");
                }
            }
        }
        private bool _enabled;

        /// <summary>
        /// Indicates whether the feature is experimental (as opposed to stable and fully supported)
        /// </summary>
        public virtual bool experimental
        {
            get { return _experimental; }
            set
            {
                if (!Helper.AreEqual(value, _experimental))
                {
                    _experimental = value;
                    Changed = true;
                    NotifyPropertyChanged("experimental");
                }
            }
        }
        private bool _experimental;

        /// <summary>
        /// The version of this feature
        /// </summary>
        public virtual string version
        {
            get { return _version; }
            set
            {
                if (!Helper.AreEqual(value, _version))
                {
                    _version = value;
                    Changed = true;
                    NotifyPropertyChanged("version");
                }
            }
        }
        private string _version;

        /// <summary>
        /// The host where this feature is available
        /// </summary>
        public virtual XenRef<Host> host
        {
            get { return _host; }
            set
            {
                if (!Helper.AreEqual(value, _host))
                {
                    _host = value;
                    Changed = true;
                    NotifyPropertyChanged("host");
                }
            }
        }
        private XenRef<Host> _host;
    }
}
