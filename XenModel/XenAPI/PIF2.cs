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
    public partial class PIF
    {
        /// <summary>
        /// Backward compatibility for PIF.db_introduce in XenServer 6.0.
        /// </summary>
        public static XenRef<PIF> db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug)
        {
            if (Helper.APIVersionMeets(session, API_Version.API_1_10))
                System.Diagnostics.Debug.Assert(false, "Cannot use this call on XenServer 6.1 or newer.");

            return XenRef<PIF>.Create(session.proxy.pif_db_introduce(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", (_host != null) ? _host : "", (_mac != null) ? _mac : "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), (_ip != null) ? _ip : "", (_netmask != null) ? _netmask : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "", (_bond_slave_of != null) ? _bond_slave_of : "", (_vlan_master_of != null) ? _vlan_master_of : "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug).parse());
        }

        /// <summary>
        /// Backward compatibility for PIF.async_db_introduce in XenServer 6.0.
        /// </summary>
        public static XenRef<Task> async_db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug)
        {
            if (Helper.APIVersionMeets(session, API_Version.API_1_10))
                System.Diagnostics.Debug.Assert(false, "Cannot use this call on XenServer 6.1 or newer.");

            return XenRef<Task>.Create(session.proxy.async_pif_db_introduce(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", (_host != null) ? _host : "", (_mac != null) ? _mac : "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), (_ip != null) ? _ip : "", (_netmask != null) ? _netmask : "", (_gateway != null) ? _gateway : "", (_dns != null) ? _dns : "", (_bond_slave_of != null) ? _bond_slave_of : "", (_vlan_master_of != null) ? _vlan_master_of : "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug).parse());
        }
    }
}
