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
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace XenAPI
{
    /// <summary>
    /// The metrics associated with a virtual block device
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class VBD_metrics : XenObject<VBD_metrics>
    {
        public VBD_metrics()
        {
        }

        public VBD_metrics(string uuid,
            double io_read_kbs,
            double io_write_kbs,
            DateTime last_updated,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.io_read_kbs = io_read_kbs;
            this.io_write_kbs = io_write_kbs;
            this.last_updated = last_updated;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new VBD_metrics from a Proxy_VBD_metrics.
        /// </summary>
        /// <param name="proxy"></param>
        public VBD_metrics(Proxy_VBD_metrics proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VBD_metrics.
        /// </summary>
        public override void UpdateFrom(VBD_metrics update)
        {
            uuid = update.uuid;
            io_read_kbs = update.io_read_kbs;
            io_write_kbs = update.io_write_kbs;
            last_updated = update.last_updated;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_VBD_metrics proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            io_read_kbs = Convert.ToDouble(proxy.io_read_kbs);
            io_write_kbs = Convert.ToDouble(proxy.io_write_kbs);
            last_updated = proxy.last_updated;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_VBD_metrics ToProxy()
        {
            Proxy_VBD_metrics result_ = new Proxy_VBD_metrics();
            result_.uuid = uuid ?? "";
            result_.io_read_kbs = io_read_kbs;
            result_.io_write_kbs = io_write_kbs;
            result_.last_updated = last_updated;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new VBD_metrics from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VBD_metrics(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VBD_metrics
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("io_read_kbs"))
                io_read_kbs = Marshalling.ParseDouble(table, "io_read_kbs");
            if (table.ContainsKey("io_write_kbs"))
                io_write_kbs = Marshalling.ParseDouble(table, "io_write_kbs");
            if (table.ContainsKey("last_updated"))
                last_updated = Marshalling.ParseDateTime(table, "last_updated");
            if (table.ContainsKey("other_config"))
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(VBD_metrics other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._io_read_kbs, other._io_read_kbs) &&
                Helper.AreEqual2(this._io_write_kbs, other._io_write_kbs) &&
                Helper.AreEqual2(this._last_updated, other._last_updated) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        internal static List<VBD_metrics> ProxyArrayToObjectList(Proxy_VBD_metrics[] input)
        {
            var result = new List<VBD_metrics>();
            foreach (var item in input)
                result.Add(new VBD_metrics(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, VBD_metrics server)
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
                    VBD_metrics.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given VBD_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd_metrics">The opaque_ref of the given vbd_metrics</param>
        public static VBD_metrics get_record(Session session, string _vbd_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_metrics_get_record(session.opaque_ref, _vbd_metrics);
            else
                return new VBD_metrics((Proxy_VBD_metrics)session.proxy.vbd_metrics_get_record(session.opaque_ref, _vbd_metrics ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the VBD_metrics instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VBD_metrics> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_metrics_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<VBD_metrics>.Create(session.proxy.vbd_metrics_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VBD_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd_metrics">The opaque_ref of the given vbd_metrics</param>
        public static string get_uuid(Session session, string _vbd_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_metrics_get_uuid(session.opaque_ref, _vbd_metrics);
            else
                return session.proxy.vbd_metrics_get_uuid(session.opaque_ref, _vbd_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the io/read_kbs field of the given VBD_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd_metrics">The opaque_ref of the given vbd_metrics</param>
        public static double get_io_read_kbs(Session session, string _vbd_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_metrics_get_io_read_kbs(session.opaque_ref, _vbd_metrics);
            else
                return Convert.ToDouble(session.proxy.vbd_metrics_get_io_read_kbs(session.opaque_ref, _vbd_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the io/write_kbs field of the given VBD_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd_metrics">The opaque_ref of the given vbd_metrics</param>
        public static double get_io_write_kbs(Session session, string _vbd_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_metrics_get_io_write_kbs(session.opaque_ref, _vbd_metrics);
            else
                return Convert.ToDouble(session.proxy.vbd_metrics_get_io_write_kbs(session.opaque_ref, _vbd_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the last_updated field of the given VBD_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd_metrics">The opaque_ref of the given vbd_metrics</param>
        public static DateTime get_last_updated(Session session, string _vbd_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_metrics_get_last_updated(session.opaque_ref, _vbd_metrics);
            else
                return session.proxy.vbd_metrics_get_last_updated(session.opaque_ref, _vbd_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the other_config field of the given VBD_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd_metrics">The opaque_ref of the given vbd_metrics</param>
        public static Dictionary<string, string> get_other_config(Session session, string _vbd_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_metrics_get_other_config(session.opaque_ref, _vbd_metrics);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vbd_metrics_get_other_config(session.opaque_ref, _vbd_metrics ?? "").parse());
        }

        /// <summary>
        /// Set the other_config field of the given VBD_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd_metrics">The opaque_ref of the given vbd_metrics</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _vbd_metrics, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_metrics_set_other_config(session.opaque_ref, _vbd_metrics, _other_config);
            else
                session.proxy.vbd_metrics_set_other_config(session.opaque_ref, _vbd_metrics ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VBD_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd_metrics">The opaque_ref of the given vbd_metrics</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _vbd_metrics, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_metrics_add_to_other_config(session.opaque_ref, _vbd_metrics, _key, _value);
            else
                session.proxy.vbd_metrics_add_to_other_config(session.opaque_ref, _vbd_metrics ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VBD_metrics.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd_metrics">The opaque_ref of the given vbd_metrics</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _vbd_metrics, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_metrics_remove_from_other_config(session.opaque_ref, _vbd_metrics, _key);
            else
                session.proxy.vbd_metrics_remove_from_other_config(session.opaque_ref, _vbd_metrics ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Return a list of all the VBD_metrics instances known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<VBD_metrics>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_metrics_get_all(session.opaque_ref);
            else
                return XenRef<VBD_metrics>.Create(session.proxy.vbd_metrics_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the VBD_metrics Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<VBD_metrics>, VBD_metrics> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_metrics_get_all_records(session.opaque_ref);
            else
                return XenRef<VBD_metrics>.Create<Proxy_VBD_metrics>(session.proxy.vbd_metrics_get_all_records(session.opaque_ref).parse());
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
        private string _uuid = "";

        /// <summary>
        /// Read bandwidth (KiB/s)
        /// </summary>
        public virtual double io_read_kbs
        {
            get { return _io_read_kbs; }
            set
            {
                if (!Helper.AreEqual(value, _io_read_kbs))
                {
                    _io_read_kbs = value;
                    Changed = true;
                    NotifyPropertyChanged("io_read_kbs");
                }
            }
        }
        private double _io_read_kbs;

        /// <summary>
        /// Write bandwidth (KiB/s)
        /// </summary>
        public virtual double io_write_kbs
        {
            get { return _io_write_kbs; }
            set
            {
                if (!Helper.AreEqual(value, _io_write_kbs))
                {
                    _io_write_kbs = value;
                    Changed = true;
                    NotifyPropertyChanged("io_write_kbs");
                }
            }
        }
        private double _io_write_kbs;

        /// <summary>
        /// Time at which this information was last updated
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime last_updated
        {
            get { return _last_updated; }
            set
            {
                if (!Helper.AreEqual(value, _last_updated))
                {
                    _last_updated = value;
                    Changed = true;
                    NotifyPropertyChanged("last_updated");
                }
            }
        }
        private DateTime _last_updated;

        /// <summary>
        /// additional configuration
        /// First published in XenServer 5.0.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
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
        private Dictionary<string, string> _other_config = new Dictionary<string, string>() {};
    }
}
