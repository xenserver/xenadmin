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
    /// A physical CPU
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class Host_cpu : XenObject<Host_cpu>
    {
        public Host_cpu()
        {
        }

        public Host_cpu(string uuid,
            XenRef<Host> host,
            long number,
            string vendor,
            long speed,
            string modelname,
            long family,
            long model,
            string stepping,
            string flags,
            string features,
            double utilisation,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.host = host;
            this.number = number;
            this.vendor = vendor;
            this.speed = speed;
            this.modelname = modelname;
            this.family = family;
            this.model = model;
            this.stepping = stepping;
            this.flags = flags;
            this.features = features;
            this.utilisation = utilisation;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Host_cpu from a Proxy_Host_cpu.
        /// </summary>
        /// <param name="proxy"></param>
        public Host_cpu(Proxy_Host_cpu proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Host_cpu update)
        {
            uuid = update.uuid;
            host = update.host;
            number = update.number;
            vendor = update.vendor;
            speed = update.speed;
            modelname = update.modelname;
            family = update.family;
            model = update.model;
            stepping = update.stepping;
            flags = update.flags;
            features = update.features;
            utilisation = update.utilisation;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_Host_cpu proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
            number = proxy.number == null ? 0 : long.Parse((string)proxy.number);
            vendor = proxy.vendor == null ? null : (string)proxy.vendor;
            speed = proxy.speed == null ? 0 : long.Parse((string)proxy.speed);
            modelname = proxy.modelname == null ? null : (string)proxy.modelname;
            family = proxy.family == null ? 0 : long.Parse((string)proxy.family);
            model = proxy.model == null ? 0 : long.Parse((string)proxy.model);
            stepping = proxy.stepping == null ? null : (string)proxy.stepping;
            flags = proxy.flags == null ? null : (string)proxy.flags;
            features = proxy.features == null ? null : (string)proxy.features;
            utilisation = Convert.ToDouble(proxy.utilisation);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_Host_cpu ToProxy()
        {
            Proxy_Host_cpu result_ = new Proxy_Host_cpu();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.host = (host != null) ? host : "";
            result_.number = number.ToString();
            result_.vendor = (vendor != null) ? vendor : "";
            result_.speed = speed.ToString();
            result_.modelname = (modelname != null) ? modelname : "";
            result_.family = family.ToString();
            result_.model = model.ToString();
            result_.stepping = (stepping != null) ? stepping : "";
            result_.flags = (flags != null) ? flags : "";
            result_.features = (features != null) ? features : "";
            result_.utilisation = utilisation;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new Host_cpu from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Host_cpu(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            host = Marshalling.ParseRef<Host>(table, "host");
            number = Marshalling.ParseLong(table, "number");
            vendor = Marshalling.ParseString(table, "vendor");
            speed = Marshalling.ParseLong(table, "speed");
            modelname = Marshalling.ParseString(table, "modelname");
            family = Marshalling.ParseLong(table, "family");
            model = Marshalling.ParseLong(table, "model");
            stepping = Marshalling.ParseString(table, "stepping");
            flags = Marshalling.ParseString(table, "flags");
            features = Marshalling.ParseString(table, "features");
            utilisation = Marshalling.ParseDouble(table, "utilisation");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Host_cpu other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._host, other._host) &&
                Helper.AreEqual2(this._number, other._number) &&
                Helper.AreEqual2(this._vendor, other._vendor) &&
                Helper.AreEqual2(this._speed, other._speed) &&
                Helper.AreEqual2(this._modelname, other._modelname) &&
                Helper.AreEqual2(this._family, other._family) &&
                Helper.AreEqual2(this._model, other._model) &&
                Helper.AreEqual2(this._stepping, other._stepping) &&
                Helper.AreEqual2(this._flags, other._flags) &&
                Helper.AreEqual2(this._features, other._features) &&
                Helper.AreEqual2(this._utilisation, other._utilisation) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, Host_cpu server)
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
                    Host_cpu.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given host_cpu.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        [Deprecated("XenServer 5.6")]
        public static Host_cpu get_record(Session session, string _host_cpu)
        {
            return new Host_cpu((Proxy_Host_cpu)session.proxy.host_cpu_get_record(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse());
        }

        /// <summary>
        /// Get a reference to the host_cpu instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        [Deprecated("XenServer 5.6")]
        public static XenRef<Host_cpu> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Host_cpu>.Create(session.proxy.host_cpu_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        public static string get_uuid(Session session, string _host_cpu)
        {
            return (string)session.proxy.host_cpu_get_uuid(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse();
        }

        /// <summary>
        /// Get the host field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        public static XenRef<Host> get_host(Session session, string _host_cpu)
        {
            return XenRef<Host>.Create(session.proxy.host_cpu_get_host(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse());
        }

        /// <summary>
        /// Get the number field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        public static long get_number(Session session, string _host_cpu)
        {
            return long.Parse((string)session.proxy.host_cpu_get_number(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse());
        }

        /// <summary>
        /// Get the vendor field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        public static string get_vendor(Session session, string _host_cpu)
        {
            return (string)session.proxy.host_cpu_get_vendor(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse();
        }

        /// <summary>
        /// Get the speed field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        public static long get_speed(Session session, string _host_cpu)
        {
            return long.Parse((string)session.proxy.host_cpu_get_speed(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse());
        }

        /// <summary>
        /// Get the modelname field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        public static string get_modelname(Session session, string _host_cpu)
        {
            return (string)session.proxy.host_cpu_get_modelname(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse();
        }

        /// <summary>
        /// Get the family field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        public static long get_family(Session session, string _host_cpu)
        {
            return long.Parse((string)session.proxy.host_cpu_get_family(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse());
        }

        /// <summary>
        /// Get the model field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        public static long get_model(Session session, string _host_cpu)
        {
            return long.Parse((string)session.proxy.host_cpu_get_model(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse());
        }

        /// <summary>
        /// Get the stepping field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        public static string get_stepping(Session session, string _host_cpu)
        {
            return (string)session.proxy.host_cpu_get_stepping(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse();
        }

        /// <summary>
        /// Get the flags field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        public static string get_flags(Session session, string _host_cpu)
        {
            return (string)session.proxy.host_cpu_get_flags(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse();
        }

        /// <summary>
        /// Get the features field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        public static string get_features(Session session, string _host_cpu)
        {
            return (string)session.proxy.host_cpu_get_features(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse();
        }

        /// <summary>
        /// Get the utilisation field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        public static double get_utilisation(Session session, string _host_cpu)
        {
            return Convert.ToDouble(session.proxy.host_cpu_get_utilisation(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse());
        }

        /// <summary>
        /// Get the other_config field of the given host_cpu.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        public static Dictionary<string, string> get_other_config(Session session, string _host_cpu)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.host_cpu_get_other_config(session.uuid, (_host_cpu != null) ? _host_cpu : "").parse());
        }

        /// <summary>
        /// Set the other_config field of the given host_cpu.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _host_cpu, Dictionary<string, string> _other_config)
        {
            session.proxy.host_cpu_set_other_config(session.uuid, (_host_cpu != null) ? _host_cpu : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given host_cpu.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _host_cpu, string _key, string _value)
        {
            session.proxy.host_cpu_add_to_other_config(session.uuid, (_host_cpu != null) ? _host_cpu : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given host_cpu.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _host_cpu, string _key)
        {
            session.proxy.host_cpu_remove_from_other_config(session.uuid, (_host_cpu != null) ? _host_cpu : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Return a list of all the host_cpus known to the system.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        [Deprecated("XenServer 5.6")]
        public static List<XenRef<Host_cpu>> get_all(Session session)
        {
            return XenRef<Host_cpu>.Create(session.proxy.host_cpu_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the host_cpu Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Host_cpu>, Host_cpu> get_all_records(Session session)
        {
            return XenRef<Host_cpu>.Create<Proxy_Host_cpu>(session.proxy.host_cpu_get_all_records(session.uuid).parse());
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
        /// the host the CPU is in
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

        /// <summary>
        /// the number of the physical CPU within the host
        /// </summary>
        public virtual long number
        {
            get { return _number; }
            set
            {
                if (!Helper.AreEqual(value, _number))
                {
                    _number = value;
                    Changed = true;
                    NotifyPropertyChanged("number");
                }
            }
        }
        private long _number;

        /// <summary>
        /// the vendor of the physical CPU
        /// </summary>
        public virtual string vendor
        {
            get { return _vendor; }
            set
            {
                if (!Helper.AreEqual(value, _vendor))
                {
                    _vendor = value;
                    Changed = true;
                    NotifyPropertyChanged("vendor");
                }
            }
        }
        private string _vendor;

        /// <summary>
        /// the speed of the physical CPU
        /// </summary>
        public virtual long speed
        {
            get { return _speed; }
            set
            {
                if (!Helper.AreEqual(value, _speed))
                {
                    _speed = value;
                    Changed = true;
                    NotifyPropertyChanged("speed");
                }
            }
        }
        private long _speed;

        /// <summary>
        /// the model name of the physical CPU
        /// </summary>
        public virtual string modelname
        {
            get { return _modelname; }
            set
            {
                if (!Helper.AreEqual(value, _modelname))
                {
                    _modelname = value;
                    Changed = true;
                    NotifyPropertyChanged("modelname");
                }
            }
        }
        private string _modelname;

        /// <summary>
        /// the family (number) of the physical CPU
        /// </summary>
        public virtual long family
        {
            get { return _family; }
            set
            {
                if (!Helper.AreEqual(value, _family))
                {
                    _family = value;
                    Changed = true;
                    NotifyPropertyChanged("family");
                }
            }
        }
        private long _family;

        /// <summary>
        /// the model number of the physical CPU
        /// </summary>
        public virtual long model
        {
            get { return _model; }
            set
            {
                if (!Helper.AreEqual(value, _model))
                {
                    _model = value;
                    Changed = true;
                    NotifyPropertyChanged("model");
                }
            }
        }
        private long _model;

        /// <summary>
        /// the stepping of the physical CPU
        /// </summary>
        public virtual string stepping
        {
            get { return _stepping; }
            set
            {
                if (!Helper.AreEqual(value, _stepping))
                {
                    _stepping = value;
                    Changed = true;
                    NotifyPropertyChanged("stepping");
                }
            }
        }
        private string _stepping;

        /// <summary>
        /// the flags of the physical CPU (a decoded version of the features field)
        /// </summary>
        public virtual string flags
        {
            get { return _flags; }
            set
            {
                if (!Helper.AreEqual(value, _flags))
                {
                    _flags = value;
                    Changed = true;
                    NotifyPropertyChanged("flags");
                }
            }
        }
        private string _flags;

        /// <summary>
        /// the physical CPU feature bitmap
        /// </summary>
        public virtual string features
        {
            get { return _features; }
            set
            {
                if (!Helper.AreEqual(value, _features))
                {
                    _features = value;
                    Changed = true;
                    NotifyPropertyChanged("features");
                }
            }
        }
        private string _features;

        /// <summary>
        /// the current CPU utilisation
        /// </summary>
        public virtual double utilisation
        {
            get { return _utilisation; }
            set
            {
                if (!Helper.AreEqual(value, _utilisation))
                {
                    _utilisation = value;
                    Changed = true;
                    NotifyPropertyChanged("utilisation");
                }
            }
        }
        private double _utilisation;

        /// <summary>
        /// additional configuration
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual Dictionary<string, string> other_config
        {
            get { return _other_config; }
            set
            {
                if (!Helper.AreEqual(value, _other_config))
                {
                    _other_config = value;
                    Changed = true;
                    NotifyPropertyChanged("other_config");
                }
            }
        }
        private Dictionary<string, string> _other_config;
    }
}
