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
    /// A physical network interface (note separate VLANs are represented as several PIFs)
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class PIF : XenObject<PIF>
    {
        public PIF()
        {
        }

        public PIF(string uuid,
            string device,
            XenRef<Network> network,
            XenRef<Host> host,
            string MAC,
            long MTU,
            long VLAN,
            XenRef<PIF_metrics> metrics,
            bool physical,
            bool currently_attached,
            ip_configuration_mode ip_configuration_mode,
            string IP,
            string netmask,
            string gateway,
            string DNS,
            XenRef<Bond> bond_slave_of,
            List<XenRef<Bond>> bond_master_of,
            XenRef<VLAN> VLAN_master_of,
            List<XenRef<VLAN>> VLAN_slave_of,
            bool management,
            Dictionary<string, string> other_config,
            bool disallow_unplug,
            List<XenRef<Tunnel>> tunnel_access_PIF_of,
            List<XenRef<Tunnel>> tunnel_transport_PIF_of,
            ipv6_configuration_mode ipv6_configuration_mode,
            string[] IPv6,
            string ipv6_gateway,
            primary_address_type primary_address_type,
            bool managed,
            Dictionary<string, string> properties,
            string[] capabilities)
        {
            this.uuid = uuid;
            this.device = device;
            this.network = network;
            this.host = host;
            this.MAC = MAC;
            this.MTU = MTU;
            this.VLAN = VLAN;
            this.metrics = metrics;
            this.physical = physical;
            this.currently_attached = currently_attached;
            this.ip_configuration_mode = ip_configuration_mode;
            this.IP = IP;
            this.netmask = netmask;
            this.gateway = gateway;
            this.DNS = DNS;
            this.bond_slave_of = bond_slave_of;
            this.bond_master_of = bond_master_of;
            this.VLAN_master_of = VLAN_master_of;
            this.VLAN_slave_of = VLAN_slave_of;
            this.management = management;
            this.other_config = other_config;
            this.disallow_unplug = disallow_unplug;
            this.tunnel_access_PIF_of = tunnel_access_PIF_of;
            this.tunnel_transport_PIF_of = tunnel_transport_PIF_of;
            this.ipv6_configuration_mode = ipv6_configuration_mode;
            this.IPv6 = IPv6;
            this.ipv6_gateway = ipv6_gateway;
            this.primary_address_type = primary_address_type;
            this.managed = managed;
            this.properties = properties;
            this.capabilities = capabilities;
        }

        /// <summary>
        /// Creates a new PIF from a Proxy_PIF.
        /// </summary>
        /// <param name="proxy"></param>
        public PIF(Proxy_PIF proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(PIF update)
        {
            uuid = update.uuid;
            device = update.device;
            network = update.network;
            host = update.host;
            MAC = update.MAC;
            MTU = update.MTU;
            VLAN = update.VLAN;
            metrics = update.metrics;
            physical = update.physical;
            currently_attached = update.currently_attached;
            ip_configuration_mode = update.ip_configuration_mode;
            IP = update.IP;
            netmask = update.netmask;
            gateway = update.gateway;
            DNS = update.DNS;
            bond_slave_of = update.bond_slave_of;
            bond_master_of = update.bond_master_of;
            VLAN_master_of = update.VLAN_master_of;
            VLAN_slave_of = update.VLAN_slave_of;
            management = update.management;
            other_config = update.other_config;
            disallow_unplug = update.disallow_unplug;
            tunnel_access_PIF_of = update.tunnel_access_PIF_of;
            tunnel_transport_PIF_of = update.tunnel_transport_PIF_of;
            ipv6_configuration_mode = update.ipv6_configuration_mode;
            IPv6 = update.IPv6;
            ipv6_gateway = update.ipv6_gateway;
            primary_address_type = update.primary_address_type;
            managed = update.managed;
            properties = update.properties;
            capabilities = update.capabilities;
        }

        internal void UpdateFromProxy(Proxy_PIF proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            device = proxy.device == null ? null : (string)proxy.device;
            network = proxy.network == null ? null : XenRef<Network>.Create(proxy.network);
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
            MAC = proxy.MAC == null ? null : (string)proxy.MAC;
            MTU = proxy.MTU == null ? 0 : long.Parse((string)proxy.MTU);
            VLAN = proxy.VLAN == null ? 0 : long.Parse((string)proxy.VLAN);
            metrics = proxy.metrics == null ? null : XenRef<PIF_metrics>.Create(proxy.metrics);
            physical = (bool)proxy.physical;
            currently_attached = (bool)proxy.currently_attached;
            ip_configuration_mode = proxy.ip_configuration_mode == null ? (ip_configuration_mode) 0 : (ip_configuration_mode)Helper.EnumParseDefault(typeof(ip_configuration_mode), (string)proxy.ip_configuration_mode);
            IP = proxy.IP == null ? null : (string)proxy.IP;
            netmask = proxy.netmask == null ? null : (string)proxy.netmask;
            gateway = proxy.gateway == null ? null : (string)proxy.gateway;
            DNS = proxy.DNS == null ? null : (string)proxy.DNS;
            bond_slave_of = proxy.bond_slave_of == null ? null : XenRef<Bond>.Create(proxy.bond_slave_of);
            bond_master_of = proxy.bond_master_of == null ? null : XenRef<Bond>.Create(proxy.bond_master_of);
            VLAN_master_of = proxy.VLAN_master_of == null ? null : XenRef<VLAN>.Create(proxy.VLAN_master_of);
            VLAN_slave_of = proxy.VLAN_slave_of == null ? null : XenRef<VLAN>.Create(proxy.VLAN_slave_of);
            management = (bool)proxy.management;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            disallow_unplug = (bool)proxy.disallow_unplug;
            tunnel_access_PIF_of = proxy.tunnel_access_PIF_of == null ? null : XenRef<Tunnel>.Create(proxy.tunnel_access_PIF_of);
            tunnel_transport_PIF_of = proxy.tunnel_transport_PIF_of == null ? null : XenRef<Tunnel>.Create(proxy.tunnel_transport_PIF_of);
            ipv6_configuration_mode = proxy.ipv6_configuration_mode == null ? (ipv6_configuration_mode) 0 : (ipv6_configuration_mode)Helper.EnumParseDefault(typeof(ipv6_configuration_mode), (string)proxy.ipv6_configuration_mode);
            IPv6 = proxy.IPv6 == null ? new string[] {} : (string [])proxy.IPv6;
            ipv6_gateway = proxy.ipv6_gateway == null ? null : (string)proxy.ipv6_gateway;
            primary_address_type = proxy.primary_address_type == null ? (primary_address_type) 0 : (primary_address_type)Helper.EnumParseDefault(typeof(primary_address_type), (string)proxy.primary_address_type);
            managed = (bool)proxy.managed;
            properties = proxy.properties == null ? null : Maps.convert_from_proxy_string_string(proxy.properties);
            capabilities = proxy.capabilities == null ? new string[] {} : (string [])proxy.capabilities;
        }

        public Proxy_PIF ToProxy()
        {
            Proxy_PIF result_ = new Proxy_PIF();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.device = (device != null) ? device : "";
            result_.network = (network != null) ? network : "";
            result_.host = (host != null) ? host : "";
            result_.MAC = (MAC != null) ? MAC : "";
            result_.MTU = MTU.ToString();
            result_.VLAN = VLAN.ToString();
            result_.metrics = (metrics != null) ? metrics : "";
            result_.physical = physical;
            result_.currently_attached = currently_attached;
            result_.ip_configuration_mode = ip_configuration_mode_helper.ToString(ip_configuration_mode);
            result_.IP = (IP != null) ? IP : "";
            result_.netmask = (netmask != null) ? netmask : "";
            result_.gateway = (gateway != null) ? gateway : "";
            result_.DNS = (DNS != null) ? DNS : "";
            result_.bond_slave_of = (bond_slave_of != null) ? bond_slave_of : "";
            result_.bond_master_of = (bond_master_of != null) ? Helper.RefListToStringArray(bond_master_of) : new string[] {};
            result_.VLAN_master_of = (VLAN_master_of != null) ? VLAN_master_of : "";
            result_.VLAN_slave_of = (VLAN_slave_of != null) ? Helper.RefListToStringArray(VLAN_slave_of) : new string[] {};
            result_.management = management;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.disallow_unplug = disallow_unplug;
            result_.tunnel_access_PIF_of = (tunnel_access_PIF_of != null) ? Helper.RefListToStringArray(tunnel_access_PIF_of) : new string[] {};
            result_.tunnel_transport_PIF_of = (tunnel_transport_PIF_of != null) ? Helper.RefListToStringArray(tunnel_transport_PIF_of) : new string[] {};
            result_.ipv6_configuration_mode = ipv6_configuration_mode_helper.ToString(ipv6_configuration_mode);
            result_.IPv6 = IPv6;
            result_.ipv6_gateway = (ipv6_gateway != null) ? ipv6_gateway : "";
            result_.primary_address_type = primary_address_type_helper.ToString(primary_address_type);
            result_.managed = managed;
            result_.properties = Maps.convert_to_proxy_string_string(properties);
            result_.capabilities = capabilities;
            return result_;
        }

        /// <summary>
        /// Creates a new PIF from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public PIF(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            device = Marshalling.ParseString(table, "device");
            network = Marshalling.ParseRef<Network>(table, "network");
            host = Marshalling.ParseRef<Host>(table, "host");
            MAC = Marshalling.ParseString(table, "MAC");
            MTU = Marshalling.ParseLong(table, "MTU");
            VLAN = Marshalling.ParseLong(table, "VLAN");
            metrics = Marshalling.ParseRef<PIF_metrics>(table, "metrics");
            physical = Marshalling.ParseBool(table, "physical");
            currently_attached = Marshalling.ParseBool(table, "currently_attached");
            ip_configuration_mode = (ip_configuration_mode)Helper.EnumParseDefault(typeof(ip_configuration_mode), Marshalling.ParseString(table, "ip_configuration_mode"));
            IP = Marshalling.ParseString(table, "IP");
            netmask = Marshalling.ParseString(table, "netmask");
            gateway = Marshalling.ParseString(table, "gateway");
            DNS = Marshalling.ParseString(table, "DNS");
            bond_slave_of = Marshalling.ParseRef<Bond>(table, "bond_slave_of");
            bond_master_of = Marshalling.ParseSetRef<Bond>(table, "bond_master_of");
            VLAN_master_of = Marshalling.ParseRef<VLAN>(table, "VLAN_master_of");
            VLAN_slave_of = Marshalling.ParseSetRef<VLAN>(table, "VLAN_slave_of");
            management = Marshalling.ParseBool(table, "management");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            disallow_unplug = Marshalling.ParseBool(table, "disallow_unplug");
            tunnel_access_PIF_of = Marshalling.ParseSetRef<Tunnel>(table, "tunnel_access_PIF_of");
            tunnel_transport_PIF_of = Marshalling.ParseSetRef<Tunnel>(table, "tunnel_transport_PIF_of");
            ipv6_configuration_mode = (ipv6_configuration_mode)Helper.EnumParseDefault(typeof(ipv6_configuration_mode), Marshalling.ParseString(table, "ipv6_configuration_mode"));
            IPv6 = Marshalling.ParseStringArray(table, "IPv6");
            ipv6_gateway = Marshalling.ParseString(table, "ipv6_gateway");
            primary_address_type = (primary_address_type)Helper.EnumParseDefault(typeof(primary_address_type), Marshalling.ParseString(table, "primary_address_type"));
            managed = Marshalling.ParseBool(table, "managed");
            properties = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "properties"));
            capabilities = Marshalling.ParseStringArray(table, "capabilities");
        }

        public bool DeepEquals(PIF other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._device, other._device) &&
                Helper.AreEqual2(this._network, other._network) &&
                Helper.AreEqual2(this._host, other._host) &&
                Helper.AreEqual2(this._MAC, other._MAC) &&
                Helper.AreEqual2(this._MTU, other._MTU) &&
                Helper.AreEqual2(this._VLAN, other._VLAN) &&
                Helper.AreEqual2(this._metrics, other._metrics) &&
                Helper.AreEqual2(this._physical, other._physical) &&
                Helper.AreEqual2(this._currently_attached, other._currently_attached) &&
                Helper.AreEqual2(this._ip_configuration_mode, other._ip_configuration_mode) &&
                Helper.AreEqual2(this._IP, other._IP) &&
                Helper.AreEqual2(this._netmask, other._netmask) &&
                Helper.AreEqual2(this._gateway, other._gateway) &&
                Helper.AreEqual2(this._DNS, other._DNS) &&
                Helper.AreEqual2(this._bond_slave_of, other._bond_slave_of) &&
                Helper.AreEqual2(this._bond_master_of, other._bond_master_of) &&
                Helper.AreEqual2(this._VLAN_master_of, other._VLAN_master_of) &&
                Helper.AreEqual2(this._VLAN_slave_of, other._VLAN_slave_of) &&
                Helper.AreEqual2(this._management, other._management) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._disallow_unplug, other._disallow_unplug) &&
                Helper.AreEqual2(this._tunnel_access_PIF_of, other._tunnel_access_PIF_of) &&
                Helper.AreEqual2(this._tunnel_transport_PIF_of, other._tunnel_transport_PIF_of) &&
                Helper.AreEqual2(this._ipv6_configuration_mode, other._ipv6_configuration_mode) &&
                Helper.AreEqual2(this._IPv6, other._IPv6) &&
                Helper.AreEqual2(this._ipv6_gateway, other._ipv6_gateway) &&
                Helper.AreEqual2(this._primary_address_type, other._primary_address_type) &&
                Helper.AreEqual2(this._managed, other._managed) &&
                Helper.AreEqual2(this._properties, other._properties) &&
                Helper.AreEqual2(this._capabilities, other._capabilities);
        }

        public override string SaveChanges(Session session, string opaqueRef, PIF server)
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
                    PIF.set_other_config(session, opaqueRef, _other_config);
                }
                if (!Helper.AreEqual2(_disallow_unplug, server._disallow_unplug))
                {
                    PIF.set_disallow_unplug(session, opaqueRef, _disallow_unplug);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static PIF get_record(Session session, string _pif)
        {
            return new PIF((Proxy_PIF)session.proxy.pif_get_record(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get a reference to the PIF instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<PIF> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<PIF>.Create(session.proxy.pif_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_uuid(Session session, string _pif)
        {
            return (string)session.proxy.pif_get_uuid(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the device field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_device(Session session, string _pif)
        {
            return (string)session.proxy.pif_get_device(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the network field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Network> get_network(Session session, string _pif)
        {
            return XenRef<Network>.Create(session.proxy.pif_get_network(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the host field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Host> get_host(Session session, string _pif)
        {
            return XenRef<Host>.Create(session.proxy.pif_get_host(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the MAC field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_MAC(Session session, string _pif)
        {
            return (string)session.proxy.pif_get_mac(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the MTU field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static long get_MTU(Session session, string _pif)
        {
            return long.Parse((string)session.proxy.pif_get_mtu(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the VLAN field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static long get_VLAN(Session session, string _pif)
        {
            return long.Parse((string)session.proxy.pif_get_vlan(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the metrics field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<PIF_metrics> get_metrics(Session session, string _pif)
        {
            return XenRef<PIF_metrics>.Create(session.proxy.pif_get_metrics(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the physical field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static bool get_physical(Session session, string _pif)
        {
            return (bool)session.proxy.pif_get_physical(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the currently_attached field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static bool get_currently_attached(Session session, string _pif)
        {
            return (bool)session.proxy.pif_get_currently_attached(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the ip_configuration_mode field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static ip_configuration_mode get_ip_configuration_mode(Session session, string _pif)
        {
            return (ip_configuration_mode)Helper.EnumParseDefault(typeof(ip_configuration_mode), (string)session.proxy.pif_get_ip_configuration_mode(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the IP field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_IP(Session session, string _pif)
        {
            return (string)session.proxy.pif_get_ip(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the netmask field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_netmask(Session session, string _pif)
        {
            return (string)session.proxy.pif_get_netmask(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the gateway field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_gateway(Session session, string _pif)
        {
            return (string)session.proxy.pif_get_gateway(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the DNS field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_DNS(Session session, string _pif)
        {
            return (string)session.proxy.pif_get_dns(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the bond_slave_of field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Bond> get_bond_slave_of(Session session, string _pif)
        {
            return XenRef<Bond>.Create(session.proxy.pif_get_bond_slave_of(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the bond_master_of field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static List<XenRef<Bond>> get_bond_master_of(Session session, string _pif)
        {
            return XenRef<Bond>.Create(session.proxy.pif_get_bond_master_of(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the VLAN_master_of field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<VLAN> get_VLAN_master_of(Session session, string _pif)
        {
            return XenRef<VLAN>.Create(session.proxy.pif_get_vlan_master_of(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the VLAN_slave_of field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static List<XenRef<VLAN>> get_VLAN_slave_of(Session session, string _pif)
        {
            return XenRef<VLAN>.Create(session.proxy.pif_get_vlan_slave_of(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the management field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static bool get_management(Session session, string _pif)
        {
            return (bool)session.proxy.pif_get_management(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the other_config field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static Dictionary<string, string> get_other_config(Session session, string _pif)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pif_get_other_config(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the disallow_unplug field of the given PIF.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static bool get_disallow_unplug(Session session, string _pif)
        {
            return (bool)session.proxy.pif_get_disallow_unplug(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the tunnel_access_PIF_of field of the given PIF.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static List<XenRef<Tunnel>> get_tunnel_access_PIF_of(Session session, string _pif)
        {
            return XenRef<Tunnel>.Create(session.proxy.pif_get_tunnel_access_pif_of(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the tunnel_transport_PIF_of field of the given PIF.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static List<XenRef<Tunnel>> get_tunnel_transport_PIF_of(Session session, string _pif)
        {
            return XenRef<Tunnel>.Create(session.proxy.pif_get_tunnel_transport_pif_of(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the ipv6_configuration_mode field of the given PIF.
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static ipv6_configuration_mode get_ipv6_configuration_mode(Session session, string _pif)
        {
            return (ipv6_configuration_mode)Helper.EnumParseDefault(typeof(ipv6_configuration_mode), (string)session.proxy.pif_get_ipv6_configuration_mode(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the IPv6 field of the given PIF.
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string[] get_IPv6(Session session, string _pif)
        {
            return (string [])session.proxy.pif_get_ipv6(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the ipv6_gateway field of the given PIF.
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_ipv6_gateway(Session session, string _pif)
        {
            return (string)session.proxy.pif_get_ipv6_gateway(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the primary_address_type field of the given PIF.
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static primary_address_type get_primary_address_type(Session session, string _pif)
        {
            return (primary_address_type)Helper.EnumParseDefault(typeof(primary_address_type), (string)session.proxy.pif_get_primary_address_type(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the managed field of the given PIF.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static bool get_managed(Session session, string _pif)
        {
            return (bool)session.proxy.pif_get_managed(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Get the properties field of the given PIF.
        /// First published in XenServer 6.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static Dictionary<string, string> get_properties(Session session, string _pif)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pif_get_properties(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Get the capabilities field of the given PIF.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string[] get_capabilities(Session session, string _pif)
        {
            return (string [])session.proxy.pif_get_capabilities(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Set the other_config field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _pif, Dictionary<string, string> _other_config)
        {
            session.proxy.pif_set_other_config(session.uuid, (_pif != null) ? _pif : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _pif, string _key, string _value)
        {
            session.proxy.pif_add_to_other_config(session.uuid, (_pif != null) ? _pif : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given PIF.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _pif, string _key)
        {
            session.proxy.pif_remove_from_other_config(session.uuid, (_pif != null) ? _pif : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Set the disallow_unplug field of the given PIF.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_disallow_unplug">New value to set</param>
        public static void set_disallow_unplug(Session session, string _pif, bool _disallow_unplug)
        {
            session.proxy.pif_set_disallow_unplug(session.uuid, (_pif != null) ? _pif : "", _disallow_unplug).parse();
        }

        /// <summary>
        /// Create a VLAN interface from an existing physical interface. This call is deprecated: use VLAN.create instead
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device">physical interface on which to create the VLAN interface</param>
        /// <param name="_network">network to which this interface should be connected</param>
        /// <param name="_host">physical machine to which this PIF is connected</param>
        /// <param name="_vlan">VLAN tag for the new interface</param>
        [Deprecated("XenServer 4.1")]
        public static XenRef<PIF> create_VLAN(Session session, string _device, string _network, string _host, long _vlan)
        {
            return XenRef<PIF>.Create(session.proxy.pif_create_vlan(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", (_host != null) ? _host : "", _vlan.ToString()).parse());
        }

        /// <summary>
        /// Create a VLAN interface from an existing physical interface. This call is deprecated: use VLAN.create instead
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device">physical interface on which to create the VLAN interface</param>
        /// <param name="_network">network to which this interface should be connected</param>
        /// <param name="_host">physical machine to which this PIF is connected</param>
        /// <param name="_vlan">VLAN tag for the new interface</param>
        [Deprecated("XenServer 4.1")]
        public static XenRef<Task> async_create_VLAN(Session session, string _device, string _network, string _host, long _vlan)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_create_vlan(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", (_host != null) ? _host : "", _vlan.ToString()).parse());
        }

        /// <summary>
        /// Destroy the PIF object (provided it is a VLAN interface). This call is deprecated: use VLAN.destroy or Bond.destroy instead
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        [Deprecated("XenServer 4.1")]
        public static void destroy(Session session, string _pif)
        {
            session.proxy.pif_destroy(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Destroy the PIF object (provided it is a VLAN interface). This call is deprecated: use VLAN.destroy or Bond.destroy instead
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        [Deprecated("XenServer 4.1")]
        public static XenRef<Task> async_destroy(Session session, string _pif)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_destroy(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Reconfigure the IP address settings for this interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_mode">whether to use dynamic/static/no-assignment</param>
        /// <param name="_ip">the new IP address</param>
        /// <param name="_netmask">the new netmask</param>
        /// <param name="_gateway">the new gateway</param>
        /// <param name="_dns">the new DNS settings</param>
        public static void reconfigure_ip(Session session, string _pif, ip_configuration_mode _mode, string _ip, string _netmask, string _gateway, string _dns)
        {
            session.proxy.pif_reconfigure_ip(session.uuid, (_pif != null) ? _pif : "", ip_configuration_mode_helper.ToString(_mode), (_ip != null) ? _ip : "", (_netmask != null) ? _netmask : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "").parse();
        }

        /// <summary>
        /// Reconfigure the IP address settings for this interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_mode">whether to use dynamic/static/no-assignment</param>
        /// <param name="_ip">the new IP address</param>
        /// <param name="_netmask">the new netmask</param>
        /// <param name="_gateway">the new gateway</param>
        /// <param name="_dns">the new DNS settings</param>
        public static XenRef<Task> async_reconfigure_ip(Session session, string _pif, ip_configuration_mode _mode, string _ip, string _netmask, string _gateway, string _dns)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_reconfigure_ip(session.uuid, (_pif != null) ? _pif : "", ip_configuration_mode_helper.ToString(_mode), (_ip != null) ? _ip : "", (_netmask != null) ? _netmask : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "").parse());
        }

        /// <summary>
        /// Reconfigure the IPv6 address settings for this interface
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_mode">whether to use dynamic/static/no-assignment</param>
        /// <param name="_ipv6">the new IPv6 address (in <addr>/<prefix length> format)</param>
        /// <param name="_gateway">the new gateway</param>
        /// <param name="_dns">the new DNS settings</param>
        public static void reconfigure_ipv6(Session session, string _pif, ipv6_configuration_mode _mode, string _ipv6, string _gateway, string _dns)
        {
            session.proxy.pif_reconfigure_ipv6(session.uuid, (_pif != null) ? _pif : "", ipv6_configuration_mode_helper.ToString(_mode), (_ipv6 != null) ? _ipv6 : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "").parse();
        }

        /// <summary>
        /// Reconfigure the IPv6 address settings for this interface
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_mode">whether to use dynamic/static/no-assignment</param>
        /// <param name="_ipv6">the new IPv6 address (in <addr>/<prefix length> format)</param>
        /// <param name="_gateway">the new gateway</param>
        /// <param name="_dns">the new DNS settings</param>
        public static XenRef<Task> async_reconfigure_ipv6(Session session, string _pif, ipv6_configuration_mode _mode, string _ipv6, string _gateway, string _dns)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_reconfigure_ipv6(session.uuid, (_pif != null) ? _pif : "", ipv6_configuration_mode_helper.ToString(_mode), (_ipv6 != null) ? _ipv6 : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "").parse());
        }

        /// <summary>
        /// Change the primary address type used by this PIF
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_primary_address_type">Whether to prefer IPv4 or IPv6 connections</param>
        public static void set_primary_address_type(Session session, string _pif, primary_address_type _primary_address_type)
        {
            session.proxy.pif_set_primary_address_type(session.uuid, (_pif != null) ? _pif : "", primary_address_type_helper.ToString(_primary_address_type)).parse();
        }

        /// <summary>
        /// Change the primary address type used by this PIF
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_primary_address_type">Whether to prefer IPv4 or IPv6 connections</param>
        public static XenRef<Task> async_set_primary_address_type(Session session, string _pif, primary_address_type _primary_address_type)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_set_primary_address_type(session.uuid, (_pif != null) ? _pif : "", primary_address_type_helper.ToString(_primary_address_type)).parse());
        }

        /// <summary>
        /// Scan for physical interfaces on a host and create PIF objects to represent them
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host on which to scan</param>
        public static void scan(Session session, string _host)
        {
            session.proxy.pif_scan(session.uuid, (_host != null) ? _host : "").parse();
        }

        /// <summary>
        /// Scan for physical interfaces on a host and create PIF objects to represent them
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host on which to scan</param>
        public static XenRef<Task> async_scan(Session session, string _host)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_scan(session.uuid, (_host != null) ? _host : "").parse());
        }

        /// <summary>
        /// Create a PIF object matching a particular network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host on which the interface exists</param>
        /// <param name="_mac">The MAC address of the interface</param>
        /// <param name="_device">The device name to use for the interface</param>
        public static XenRef<PIF> introduce(Session session, string _host, string _mac, string _device)
        {
            return XenRef<PIF>.Create(session.proxy.pif_introduce(session.uuid, (_host != null) ? _host : "", (_mac != null) ? _mac : "", (_device != null) ? _device : "").parse());
        }

        /// <summary>
        /// Create a PIF object matching a particular network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host on which the interface exists</param>
        /// <param name="_mac">The MAC address of the interface</param>
        /// <param name="_device">The device name to use for the interface</param>
        public static XenRef<Task> async_introduce(Session session, string _host, string _mac, string _device)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_introduce(session.uuid, (_host != null) ? _host : "", (_mac != null) ? _mac : "", (_device != null) ? _device : "").parse());
        }

        /// <summary>
        /// Create a PIF object matching a particular network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host on which the interface exists</param>
        /// <param name="_mac">The MAC address of the interface</param>
        /// <param name="_device">The device name to use for the interface</param>
        /// <param name="_managed">Indicates whether the interface is managed by xapi (defaults to "true") First published in XenServer 6.2 SP1.</param>
        public static XenRef<PIF> introduce(Session session, string _host, string _mac, string _device, bool _managed)
        {
            return XenRef<PIF>.Create(session.proxy.pif_introduce(session.uuid, (_host != null) ? _host : "", (_mac != null) ? _mac : "", (_device != null) ? _device : "", _managed).parse());
        }

        /// <summary>
        /// Create a PIF object matching a particular network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host on which the interface exists</param>
        /// <param name="_mac">The MAC address of the interface</param>
        /// <param name="_device">The device name to use for the interface</param>
        /// <param name="_managed">Indicates whether the interface is managed by xapi (defaults to "true") First published in XenServer 6.2 SP1.</param>
        public static XenRef<Task> async_introduce(Session session, string _host, string _mac, string _device, bool _managed)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_introduce(session.uuid, (_host != null) ? _host : "", (_mac != null) ? _mac : "", (_device != null) ? _device : "", _managed).parse());
        }

        /// <summary>
        /// Destroy the PIF object matching a particular network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static void forget(Session session, string _pif)
        {
            session.proxy.pif_forget(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Destroy the PIF object matching a particular network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Task> async_forget(Session session, string _pif)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_forget(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Attempt to bring down a physical interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static void unplug(Session session, string _pif)
        {
            session.proxy.pif_unplug(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Attempt to bring down a physical interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Task> async_unplug(Session session, string _pif)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_unplug(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Attempt to bring up a physical interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static void plug(Session session, string _pif)
        {
            session.proxy.pif_plug(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Attempt to bring up a physical interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Task> async_plug(Session session, string _pif)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_plug(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        public static XenRef<PIF> db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug)
        {
            return XenRef<PIF>.Create(session.proxy.pif_db_introduce(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", (_host != null) ? _host : "", (_mac != null) ? _mac : "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), (_ip != null) ? _ip : "", (_netmask != null) ? _netmask : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "", (_bond_slave_of != null) ? _bond_slave_of : "", (_vlan_master_of != null) ? _vlan_master_of : "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        public static XenRef<Task> async_db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_db_introduce(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", (_host != null) ? _host : "", (_mac != null) ? _mac : "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), (_ip != null) ? _ip : "", (_netmask != null) ? _netmask : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "", (_bond_slave_of != null) ? _bond_slave_of : "", (_vlan_master_of != null) ? _vlan_master_of : "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        /// <param name="_ipv6_configuration_mode"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6_gateway"> First published in XenServer 6.0.</param>
        /// <param name="_primary_address_type"> First published in XenServer 6.0.</param>
        public static XenRef<PIF> db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type)
        {
            return XenRef<PIF>.Create(session.proxy.pif_db_introduce(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", (_host != null) ? _host : "", (_mac != null) ? _mac : "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), (_ip != null) ? _ip : "", (_netmask != null) ? _netmask : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "", (_bond_slave_of != null) ? _bond_slave_of : "", (_vlan_master_of != null) ? _vlan_master_of : "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug, ipv6_configuration_mode_helper.ToString(_ipv6_configuration_mode), _ipv6, (_ipv6_gateway != null) ? _ipv6_gateway : "", primary_address_type_helper.ToString(_primary_address_type)).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        /// <param name="_ipv6_configuration_mode"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6_gateway"> First published in XenServer 6.0.</param>
        /// <param name="_primary_address_type"> First published in XenServer 6.0.</param>
        public static XenRef<Task> async_db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_db_introduce(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", (_host != null) ? _host : "", (_mac != null) ? _mac : "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), (_ip != null) ? _ip : "", (_netmask != null) ? _netmask : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "", (_bond_slave_of != null) ? _bond_slave_of : "", (_vlan_master_of != null) ? _vlan_master_of : "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug, ipv6_configuration_mode_helper.ToString(_ipv6_configuration_mode), _ipv6, (_ipv6_gateway != null) ? _ipv6_gateway : "", primary_address_type_helper.ToString(_primary_address_type)).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        /// <param name="_ipv6_configuration_mode"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6_gateway"> First published in XenServer 6.0.</param>
        /// <param name="_primary_address_type"> First published in XenServer 6.0.</param>
        /// <param name="_managed"> First published in XenServer 6.2 SP1.</param>
        public static XenRef<PIF> db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type, bool _managed)
        {
            return XenRef<PIF>.Create(session.proxy.pif_db_introduce(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", (_host != null) ? _host : "", (_mac != null) ? _mac : "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), (_ip != null) ? _ip : "", (_netmask != null) ? _netmask : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "", (_bond_slave_of != null) ? _bond_slave_of : "", (_vlan_master_of != null) ? _vlan_master_of : "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug, ipv6_configuration_mode_helper.ToString(_ipv6_configuration_mode), _ipv6, (_ipv6_gateway != null) ? _ipv6_gateway : "", primary_address_type_helper.ToString(_primary_address_type), _managed).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        /// <param name="_ipv6_configuration_mode"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6_gateway"> First published in XenServer 6.0.</param>
        /// <param name="_primary_address_type"> First published in XenServer 6.0.</param>
        /// <param name="_managed"> First published in XenServer 6.2 SP1.</param>
        public static XenRef<Task> async_db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type, bool _managed)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_db_introduce(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", (_host != null) ? _host : "", (_mac != null) ? _mac : "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), (_ip != null) ? _ip : "", (_netmask != null) ? _netmask : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "", (_bond_slave_of != null) ? _bond_slave_of : "", (_vlan_master_of != null) ? _vlan_master_of : "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug, ipv6_configuration_mode_helper.ToString(_ipv6_configuration_mode), _ipv6, (_ipv6_gateway != null) ? _ipv6_gateway : "", primary_address_type_helper.ToString(_primary_address_type), _managed).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        /// <param name="_ipv6_configuration_mode"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6_gateway"> First published in XenServer 6.0.</param>
        /// <param name="_primary_address_type"> First published in XenServer 6.0.</param>
        /// <param name="_managed"> First published in XenServer 6.2 SP1.</param>
        /// <param name="_properties"> First published in XenServer 6.5.</param>
        public static XenRef<PIF> db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type, bool _managed, Dictionary<string, string> _properties)
        {
            return XenRef<PIF>.Create(session.proxy.pif_db_introduce(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", (_host != null) ? _host : "", (_mac != null) ? _mac : "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), (_ip != null) ? _ip : "", (_netmask != null) ? _netmask : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "", (_bond_slave_of != null) ? _bond_slave_of : "", (_vlan_master_of != null) ? _vlan_master_of : "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug, ipv6_configuration_mode_helper.ToString(_ipv6_configuration_mode), _ipv6, (_ipv6_gateway != null) ? _ipv6_gateway : "", primary_address_type_helper.ToString(_primary_address_type), _managed, Maps.convert_to_proxy_string_string(_properties)).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        /// <param name="_ipv6_configuration_mode"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6_gateway"> First published in XenServer 6.0.</param>
        /// <param name="_primary_address_type"> First published in XenServer 6.0.</param>
        /// <param name="_managed"> First published in XenServer 6.2 SP1.</param>
        /// <param name="_properties"> First published in XenServer 6.5.</param>
        public static XenRef<Task> async_db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type, bool _managed, Dictionary<string, string> _properties)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_db_introduce(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", (_host != null) ? _host : "", (_mac != null) ? _mac : "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), (_ip != null) ? _ip : "", (_netmask != null) ? _netmask : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "", (_bond_slave_of != null) ? _bond_slave_of : "", (_vlan_master_of != null) ? _vlan_master_of : "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug, ipv6_configuration_mode_helper.ToString(_ipv6_configuration_mode), _ipv6, (_ipv6_gateway != null) ? _ipv6_gateway : "", primary_address_type_helper.ToString(_primary_address_type), _managed, Maps.convert_to_proxy_string_string(_properties)).parse());
        }

        /// <summary>
        /// Destroy a PIF database record.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static void db_forget(Session session, string _pif)
        {
            session.proxy.pif_db_forget(session.uuid, (_pif != null) ? _pif : "").parse();
        }

        /// <summary>
        /// Destroy a PIF database record.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Task> async_db_forget(Session session, string _pif)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_db_forget(session.uuid, (_pif != null) ? _pif : "").parse());
        }

        /// <summary>
        /// Set the value of a property of the PIF
        /// First published in XenServer 6.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_name">The property name</param>
        /// <param name="_value">The property value</param>
        public static void set_property(Session session, string _pif, string _name, string _value)
        {
            session.proxy.pif_set_property(session.uuid, (_pif != null) ? _pif : "", (_name != null) ? _name : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Set the value of a property of the PIF
        /// First published in XenServer 6.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_name">The property name</param>
        /// <param name="_value">The property value</param>
        public static XenRef<Task> async_set_property(Session session, string _pif, string _name, string _value)
        {
            return XenRef<Task>.Create(session.proxy.async_pif_set_property(session.uuid, (_pif != null) ? _pif : "", (_name != null) ? _name : "", (_value != null) ? _value : "").parse());
        }

        /// <summary>
        /// Return a list of all the PIFs known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<PIF>> get_all(Session session)
        {
            return XenRef<PIF>.Create(session.proxy.pif_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the PIF Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<PIF>, PIF> get_all_records(Session session)
        {
            return XenRef<PIF>.Create<Proxy_PIF>(session.proxy.pif_get_all_records(session.uuid).parse());
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
        /// machine-readable name of the interface (e.g. eth0)
        /// </summary>
        public virtual string device
        {
            get { return _device; }
            set
            {
                if (!Helper.AreEqual(value, _device))
                {
                    _device = value;
                    Changed = true;
                    NotifyPropertyChanged("device");
                }
            }
        }
        private string _device;

        /// <summary>
        /// virtual network to which this pif is connected
        /// </summary>
        public virtual XenRef<Network> network
        {
            get { return _network; }
            set
            {
                if (!Helper.AreEqual(value, _network))
                {
                    _network = value;
                    Changed = true;
                    NotifyPropertyChanged("network");
                }
            }
        }
        private XenRef<Network> _network;

        /// <summary>
        /// physical machine to which this pif is connected
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
        /// ethernet MAC address of physical interface
        /// </summary>
        public virtual string MAC
        {
            get { return _MAC; }
            set
            {
                if (!Helper.AreEqual(value, _MAC))
                {
                    _MAC = value;
                    Changed = true;
                    NotifyPropertyChanged("MAC");
                }
            }
        }
        private string _MAC;

        /// <summary>
        /// MTU in octets
        /// </summary>
        public virtual long MTU
        {
            get { return _MTU; }
            set
            {
                if (!Helper.AreEqual(value, _MTU))
                {
                    _MTU = value;
                    Changed = true;
                    NotifyPropertyChanged("MTU");
                }
            }
        }
        private long _MTU;

        /// <summary>
        /// VLAN tag for all traffic passing through this interface
        /// </summary>
        public virtual long VLAN
        {
            get { return _VLAN; }
            set
            {
                if (!Helper.AreEqual(value, _VLAN))
                {
                    _VLAN = value;
                    Changed = true;
                    NotifyPropertyChanged("VLAN");
                }
            }
        }
        private long _VLAN;

        /// <summary>
        /// metrics associated with this PIF
        /// </summary>
        public virtual XenRef<PIF_metrics> metrics
        {
            get { return _metrics; }
            set
            {
                if (!Helper.AreEqual(value, _metrics))
                {
                    _metrics = value;
                    Changed = true;
                    NotifyPropertyChanged("metrics");
                }
            }
        }
        private XenRef<PIF_metrics> _metrics;

        /// <summary>
        /// true if this represents a physical network interface
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual bool physical
        {
            get { return _physical; }
            set
            {
                if (!Helper.AreEqual(value, _physical))
                {
                    _physical = value;
                    Changed = true;
                    NotifyPropertyChanged("physical");
                }
            }
        }
        private bool _physical;

        /// <summary>
        /// true if this interface is online
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual bool currently_attached
        {
            get { return _currently_attached; }
            set
            {
                if (!Helper.AreEqual(value, _currently_attached))
                {
                    _currently_attached = value;
                    Changed = true;
                    NotifyPropertyChanged("currently_attached");
                }
            }
        }
        private bool _currently_attached;

        /// <summary>
        /// Sets if and how this interface gets an IP address
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual ip_configuration_mode ip_configuration_mode
        {
            get { return _ip_configuration_mode; }
            set
            {
                if (!Helper.AreEqual(value, _ip_configuration_mode))
                {
                    _ip_configuration_mode = value;
                    Changed = true;
                    NotifyPropertyChanged("ip_configuration_mode");
                }
            }
        }
        private ip_configuration_mode _ip_configuration_mode;

        /// <summary>
        /// IP address
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual string IP
        {
            get { return _IP; }
            set
            {
                if (!Helper.AreEqual(value, _IP))
                {
                    _IP = value;
                    Changed = true;
                    NotifyPropertyChanged("IP");
                }
            }
        }
        private string _IP;

        /// <summary>
        /// IP netmask
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual string netmask
        {
            get { return _netmask; }
            set
            {
                if (!Helper.AreEqual(value, _netmask))
                {
                    _netmask = value;
                    Changed = true;
                    NotifyPropertyChanged("netmask");
                }
            }
        }
        private string _netmask;

        /// <summary>
        /// IP gateway
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual string gateway
        {
            get { return _gateway; }
            set
            {
                if (!Helper.AreEqual(value, _gateway))
                {
                    _gateway = value;
                    Changed = true;
                    NotifyPropertyChanged("gateway");
                }
            }
        }
        private string _gateway;

        /// <summary>
        /// IP address of DNS servers to use
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual string DNS
        {
            get { return _DNS; }
            set
            {
                if (!Helper.AreEqual(value, _DNS))
                {
                    _DNS = value;
                    Changed = true;
                    NotifyPropertyChanged("DNS");
                }
            }
        }
        private string _DNS;

        /// <summary>
        /// Indicates which bond this interface is part of
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual XenRef<Bond> bond_slave_of
        {
            get { return _bond_slave_of; }
            set
            {
                if (!Helper.AreEqual(value, _bond_slave_of))
                {
                    _bond_slave_of = value;
                    Changed = true;
                    NotifyPropertyChanged("bond_slave_of");
                }
            }
        }
        private XenRef<Bond> _bond_slave_of;

        /// <summary>
        /// Indicates this PIF represents the results of a bond
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual List<XenRef<Bond>> bond_master_of
        {
            get { return _bond_master_of; }
            set
            {
                if (!Helper.AreEqual(value, _bond_master_of))
                {
                    _bond_master_of = value;
                    Changed = true;
                    NotifyPropertyChanged("bond_master_of");
                }
            }
        }
        private List<XenRef<Bond>> _bond_master_of;

        /// <summary>
        /// Indicates wich VLAN this interface receives untagged traffic from
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual XenRef<VLAN> VLAN_master_of
        {
            get { return _VLAN_master_of; }
            set
            {
                if (!Helper.AreEqual(value, _VLAN_master_of))
                {
                    _VLAN_master_of = value;
                    Changed = true;
                    NotifyPropertyChanged("VLAN_master_of");
                }
            }
        }
        private XenRef<VLAN> _VLAN_master_of;

        /// <summary>
        /// Indicates which VLANs this interface transmits tagged traffic to
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual List<XenRef<VLAN>> VLAN_slave_of
        {
            get { return _VLAN_slave_of; }
            set
            {
                if (!Helper.AreEqual(value, _VLAN_slave_of))
                {
                    _VLAN_slave_of = value;
                    Changed = true;
                    NotifyPropertyChanged("VLAN_slave_of");
                }
            }
        }
        private List<XenRef<VLAN>> _VLAN_slave_of;

        /// <summary>
        /// Indicates whether the control software is listening for connections on this interface
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual bool management
        {
            get { return _management; }
            set
            {
                if (!Helper.AreEqual(value, _management))
                {
                    _management = value;
                    Changed = true;
                    NotifyPropertyChanged("management");
                }
            }
        }
        private bool _management;

        /// <summary>
        /// Additional configuration
        /// First published in XenServer 4.1.
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

        /// <summary>
        /// Prevent this PIF from being unplugged; set this to notify the management tool-stack that the PIF has a special use and should not be unplugged under any circumstances (e.g. because you're running storage traffic over it)
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual bool disallow_unplug
        {
            get { return _disallow_unplug; }
            set
            {
                if (!Helper.AreEqual(value, _disallow_unplug))
                {
                    _disallow_unplug = value;
                    Changed = true;
                    NotifyPropertyChanged("disallow_unplug");
                }
            }
        }
        private bool _disallow_unplug;

        /// <summary>
        /// Indicates to which tunnel this PIF gives access
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        public virtual List<XenRef<Tunnel>> tunnel_access_PIF_of
        {
            get { return _tunnel_access_PIF_of; }
            set
            {
                if (!Helper.AreEqual(value, _tunnel_access_PIF_of))
                {
                    _tunnel_access_PIF_of = value;
                    Changed = true;
                    NotifyPropertyChanged("tunnel_access_PIF_of");
                }
            }
        }
        private List<XenRef<Tunnel>> _tunnel_access_PIF_of;

        /// <summary>
        /// Indicates to which tunnel this PIF provides transport
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        public virtual List<XenRef<Tunnel>> tunnel_transport_PIF_of
        {
            get { return _tunnel_transport_PIF_of; }
            set
            {
                if (!Helper.AreEqual(value, _tunnel_transport_PIF_of))
                {
                    _tunnel_transport_PIF_of = value;
                    Changed = true;
                    NotifyPropertyChanged("tunnel_transport_PIF_of");
                }
            }
        }
        private List<XenRef<Tunnel>> _tunnel_transport_PIF_of;

        /// <summary>
        /// Sets if and how this interface gets an IPv6 address
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        public virtual ipv6_configuration_mode ipv6_configuration_mode
        {
            get { return _ipv6_configuration_mode; }
            set
            {
                if (!Helper.AreEqual(value, _ipv6_configuration_mode))
                {
                    _ipv6_configuration_mode = value;
                    Changed = true;
                    NotifyPropertyChanged("ipv6_configuration_mode");
                }
            }
        }
        private ipv6_configuration_mode _ipv6_configuration_mode;

        /// <summary>
        /// IPv6 address
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        public virtual string[] IPv6
        {
            get { return _IPv6; }
            set
            {
                if (!Helper.AreEqual(value, _IPv6))
                {
                    _IPv6 = value;
                    Changed = true;
                    NotifyPropertyChanged("IPv6");
                }
            }
        }
        private string[] _IPv6;

        /// <summary>
        /// IPv6 gateway
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        public virtual string ipv6_gateway
        {
            get { return _ipv6_gateway; }
            set
            {
                if (!Helper.AreEqual(value, _ipv6_gateway))
                {
                    _ipv6_gateway = value;
                    Changed = true;
                    NotifyPropertyChanged("ipv6_gateway");
                }
            }
        }
        private string _ipv6_gateway;

        /// <summary>
        /// Which protocol should define the primary address of this interface
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        public virtual primary_address_type primary_address_type
        {
            get { return _primary_address_type; }
            set
            {
                if (!Helper.AreEqual(value, _primary_address_type))
                {
                    _primary_address_type = value;
                    Changed = true;
                    NotifyPropertyChanged("primary_address_type");
                }
            }
        }
        private primary_address_type _primary_address_type;

        /// <summary>
        /// Indicates whether the interface is managed by xapi. If it is not, then xapi will not configure the interface, the commands PIF.plug/unplug/reconfigure_ip(v6) can not be used, nor can the interface be bonded or have VLANs based on top through xapi.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        public virtual bool managed
        {
            get { return _managed; }
            set
            {
                if (!Helper.AreEqual(value, _managed))
                {
                    _managed = value;
                    Changed = true;
                    NotifyPropertyChanged("managed");
                }
            }
        }
        private bool _managed;

        /// <summary>
        /// Additional configuration properties for the interface.
        /// First published in XenServer 6.5.
        /// </summary>
        public virtual Dictionary<string, string> properties
        {
            get { return _properties; }
            set
            {
                if (!Helper.AreEqual(value, _properties))
                {
                    _properties = value;
                    Changed = true;
                    NotifyPropertyChanged("properties");
                }
            }
        }
        private Dictionary<string, string> _properties;

        /// <summary>
        /// Additional capabilities on the interface.
        /// First published in XenServer Dundee.
        /// </summary>
        public virtual string[] capabilities
        {
            get { return _capabilities; }
            set
            {
                if (!Helper.AreEqual(value, _capabilities))
                {
                    _capabilities = value;
                    Changed = true;
                    NotifyPropertyChanged("capabilities");
                }
            }
        }
        private string[] _capabilities;
    }
}
