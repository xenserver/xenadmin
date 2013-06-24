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
    public partial interface Proxy : IXmlRpcProxy
    {
        #region pre-6.2 compatibility

        [XmlRpcMethod("session.login_with_password")]
        Response<string>
        session_login_with_password(string _uname, string _pwd, string _version);

        #endregion

        #region pre-6.1 compatibility
        
        [XmlRpcMethod("Bond.create")]
        Response<string>
        bond_create(string session, string _network, string[] _members, string _mac, string _mode);

        [XmlRpcMethod("Async.Bond.create")]
        Response<string>
        async_bond_create(string session, string _network, string[] _members, string _mac, string _mode);
        
        [XmlRpcMethod("pool.create_new_blob")]
        Response<string>
        pool_create_new_blob(string session, string _pool, string _name, string _mime_type);
        
        [XmlRpcMethod("Async.pool.create_new_blob")]
        Response<string>
        async_pool_create_new_blob(string session, string _pool, string _name, string _mime_type);
        
        [XmlRpcMethod("VM.create_new_blob")]
        Response<string>
        vm_create_new_blob(string session, string _vm, string _name, string _mime_type);

        [XmlRpcMethod("Async.VM.create_new_blob")]
        Response<string>
        async_vm_create_new_blob(string session, string _vm, string _name, string _mime_type);
        
        [XmlRpcMethod("host.create_new_blob")]
        Response<string>
        host_create_new_blob(string session, string _host, string _name, string _mime_type);

        [XmlRpcMethod("Async.host.create_new_blob")]
        Response<string>
        async_host_create_new_blob(string session, string _host, string _name, string _mime_type);
        
        [XmlRpcMethod("network.create_new_blob")]
        Response<string>
        network_create_new_blob(string session, string _network, string _name, string _mime_type);

        [XmlRpcMethod("Async.network.create_new_blob")]
        Response<string>
        async_network_create_new_blob(string session, string _network, string _name, string _mime_type);
        
        [XmlRpcMethod("PIF.db_introduce")]
        Response<string>
        pif_db_introduce(string session, string _device, string _network, string _host, string _mac, string _mtu, string _vlan, bool _physical, string _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Object _other_config, bool _disallow_unplug);

        [XmlRpcMethod("Async.PIF.db_introduce")]
        Response<string>
        async_pif_db_introduce(string session, string _device, string _network, string _host, string _mac, string _mtu, string _vlan, bool _physical, string _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Object _other_config, bool _disallow_unplug);
        
        [XmlRpcMethod("SR.create_new_blob")]
        Response<string>
        sr_create_new_blob(string session, string _sr, string _name, string _mime_type);

        [XmlRpcMethod("Async.SR.create_new_blob")]
        Response<string>
        async_sr_create_new_blob(string session, string _sr, string _name, string _mime_type);
        
        [XmlRpcMethod("VDI.introduce")]
        Response<string>
        vdi_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, string _type, bool _sharable, bool _read_only, Object _other_config, string _location, Object _xenstore_data, Object _sm_config);

        [XmlRpcMethod("Async.VDI.introduce")]
        Response<string>
        async_vdi_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, string _type, bool _sharable, bool _read_only, Object _other_config, string _location, Object _xenstore_data, Object _sm_config);

        [XmlRpcMethod("VDI.db_introduce")]
        Response<string>
        vdi_db_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, string _type, bool _sharable, bool _read_only, Object _other_config, string _location, Object _xenstore_data, Object _sm_config);

        [XmlRpcMethod("Async.VDI.db_introduce")]
        Response<string>
        async_vdi_db_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, string _type, bool _sharable, bool _read_only, Object _other_config, string _location, Object _xenstore_data, Object _sm_config);
        
        [XmlRpcMethod("blob.create")]
        Response<string>
        blob_create(string session, string _mime_type);

        #endregion
        
        #region pre-6.0 compatibility

        [XmlRpcMethod("Bond.create")]
        Response<string>
        bond_create(string session, string _network, string[] _members, string _mac);

        [XmlRpcMethod("Async.Bond.create")]
        Response<string>
        async_bond_create(string session, string _network, string[] _members, string _mac);

        #endregion
    }
}
