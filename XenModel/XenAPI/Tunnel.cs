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
    /// A tunnel for network traffic
    /// First published in XenServer 5.6 FP1.
    /// </summary>
    public partial class Tunnel : XenObject<Tunnel>
    {
        public Tunnel()
        {
        }

        public Tunnel(string uuid,
            XenRef<PIF> access_PIF,
            XenRef<PIF> transport_PIF,
            Dictionary<string, string> status,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.access_PIF = access_PIF;
            this.transport_PIF = transport_PIF;
            this.status = status;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Tunnel from a Proxy_Tunnel.
        /// </summary>
        /// <param name="proxy"></param>
        public Tunnel(Proxy_Tunnel proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Tunnel update)
        {
            uuid = update.uuid;
            access_PIF = update.access_PIF;
            transport_PIF = update.transport_PIF;
            status = update.status;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_Tunnel proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            access_PIF = proxy.access_PIF == null ? null : XenRef<PIF>.Create(proxy.access_PIF);
            transport_PIF = proxy.transport_PIF == null ? null : XenRef<PIF>.Create(proxy.transport_PIF);
            status = proxy.status == null ? null : Maps.convert_from_proxy_string_string(proxy.status);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_Tunnel ToProxy()
        {
            Proxy_Tunnel result_ = new Proxy_Tunnel();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.access_PIF = (access_PIF != null) ? access_PIF : "";
            result_.transport_PIF = (transport_PIF != null) ? transport_PIF : "";
            result_.status = Maps.convert_to_proxy_string_string(status);
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new Tunnel from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Tunnel(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            access_PIF = Marshalling.ParseRef<PIF>(table, "access_PIF");
            transport_PIF = Marshalling.ParseRef<PIF>(table, "transport_PIF");
            status = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "status"));
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Tunnel other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._access_PIF, other._access_PIF) &&
                Helper.AreEqual2(this._transport_PIF, other._transport_PIF) &&
                Helper.AreEqual2(this._status, other._status) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, Tunnel server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
                if (!Helper.AreEqual2(_status, server._status))
                {
                    Tunnel.set_status(session, opaqueRef, _status);
                }
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    Tunnel.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        public static Tunnel get_record(Session session, string _tunnel)
        {
            return new Tunnel((Proxy_Tunnel)session.proxy.tunnel_get_record(session.uuid, (_tunnel != null) ? _tunnel : "").parse());
        }

        /// <summary>
        /// Get a reference to the tunnel instance with the specified UUID.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Tunnel> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Tunnel>.Create(session.proxy.tunnel_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        public static string get_uuid(Session session, string _tunnel)
        {
            return (string)session.proxy.tunnel_get_uuid(session.uuid, (_tunnel != null) ? _tunnel : "").parse();
        }

        /// <summary>
        /// Get the access_PIF field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        public static XenRef<PIF> get_access_PIF(Session session, string _tunnel)
        {
            return XenRef<PIF>.Create(session.proxy.tunnel_get_access_pif(session.uuid, (_tunnel != null) ? _tunnel : "").parse());
        }

        /// <summary>
        /// Get the transport_PIF field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        public static XenRef<PIF> get_transport_PIF(Session session, string _tunnel)
        {
            return XenRef<PIF>.Create(session.proxy.tunnel_get_transport_pif(session.uuid, (_tunnel != null) ? _tunnel : "").parse());
        }

        /// <summary>
        /// Get the status field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        public static Dictionary<string, string> get_status(Session session, string _tunnel)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.tunnel_get_status(session.uuid, (_tunnel != null) ? _tunnel : "").parse());
        }

        /// <summary>
        /// Get the other_config field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        public static Dictionary<string, string> get_other_config(Session session, string _tunnel)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.tunnel_get_other_config(session.uuid, (_tunnel != null) ? _tunnel : "").parse());
        }

        /// <summary>
        /// Set the status field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <param name="_status">New value to set</param>
        public static void set_status(Session session, string _tunnel, Dictionary<string, string> _status)
        {
            session.proxy.tunnel_set_status(session.uuid, (_tunnel != null) ? _tunnel : "", Maps.convert_to_proxy_string_string(_status)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the status field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_status(Session session, string _tunnel, string _key, string _value)
        {
            session.proxy.tunnel_add_to_status(session.uuid, (_tunnel != null) ? _tunnel : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the status field of the given tunnel.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_status(Session session, string _tunnel, string _key)
        {
            session.proxy.tunnel_remove_from_status(session.uuid, (_tunnel != null) ? _tunnel : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Set the other_config field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _tunnel, Dictionary<string, string> _other_config)
        {
            session.proxy.tunnel_set_other_config(session.uuid, (_tunnel != null) ? _tunnel : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _tunnel, string _key, string _value)
        {
            session.proxy.tunnel_add_to_other_config(session.uuid, (_tunnel != null) ? _tunnel : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given tunnel.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _tunnel, string _key)
        {
            session.proxy.tunnel_remove_from_other_config(session.uuid, (_tunnel != null) ? _tunnel : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Create a tunnel
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_transport_pif">PIF which receives the tagged traffic</param>
        /// <param name="_network">Network to receive the tunnelled traffic</param>
        public static XenRef<Tunnel> create(Session session, string _transport_pif, string _network)
        {
            return XenRef<Tunnel>.Create(session.proxy.tunnel_create(session.uuid, (_transport_pif != null) ? _transport_pif : "", (_network != null) ? _network : "").parse());
        }

        /// <summary>
        /// Create a tunnel
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_transport_pif">PIF which receives the tagged traffic</param>
        /// <param name="_network">Network to receive the tunnelled traffic</param>
        public static XenRef<Task> async_create(Session session, string _transport_pif, string _network)
        {
            return XenRef<Task>.Create(session.proxy.async_tunnel_create(session.uuid, (_transport_pif != null) ? _transport_pif : "", (_network != null) ? _network : "").parse());
        }

        /// <summary>
        /// Destroy a tunnel
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        public static void destroy(Session session, string _tunnel)
        {
            session.proxy.tunnel_destroy(session.uuid, (_tunnel != null) ? _tunnel : "").parse();
        }

        /// <summary>
        /// Destroy a tunnel
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        public static XenRef<Task> async_destroy(Session session, string _tunnel)
        {
            return XenRef<Task>.Create(session.proxy.async_tunnel_destroy(session.uuid, (_tunnel != null) ? _tunnel : "").parse());
        }

        /// <summary>
        /// Return a list of all the tunnels known to the system.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Tunnel>> get_all(Session session)
        {
            return XenRef<Tunnel>.Create(session.proxy.tunnel_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the tunnel Records at once, in a single XML RPC call
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Tunnel>, Tunnel> get_all_records(Session session)
        {
            return XenRef<Tunnel>.Create<Proxy_Tunnel>(session.proxy.tunnel_get_all_records(session.uuid).parse());
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
        /// The interface through which the tunnel is accessed
        /// </summary>
        public virtual XenRef<PIF> access_PIF
        {
            get { return _access_PIF; }
            set
            {
                if (!Helper.AreEqual(value, _access_PIF))
                {
                    _access_PIF = value;
                    Changed = true;
                    NotifyPropertyChanged("access_PIF");
                }
            }
        }
        private XenRef<PIF> _access_PIF;

        /// <summary>
        /// The interface used by the tunnel
        /// </summary>
        public virtual XenRef<PIF> transport_PIF
        {
            get { return _transport_PIF; }
            set
            {
                if (!Helper.AreEqual(value, _transport_PIF))
                {
                    _transport_PIF = value;
                    Changed = true;
                    NotifyPropertyChanged("transport_PIF");
                }
            }
        }
        private XenRef<PIF> _transport_PIF;

        /// <summary>
        /// Status information about the tunnel
        /// </summary>
        public virtual Dictionary<string, string> status
        {
            get { return _status; }
            set
            {
                if (!Helper.AreEqual(value, _status))
                {
                    _status = value;
                    Changed = true;
                    NotifyPropertyChanged("status");
                }
            }
        }
        private Dictionary<string, string> _status;

        /// <summary>
        /// Additional configuration
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
