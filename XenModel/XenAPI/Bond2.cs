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
    public partial class Bond
    {
        // Backward compatibility for Bond.create in XenServer 5.x.

        public static XenRef<Bond> create(Session session, string _network, List<XenRef<PIF>> _members, string _mac)
        {
            if (Helper.APIVersionMeets(session, API_Version.API_1_9))
                System.Diagnostics.Debug.Assert(false, "Cannot use this call on XenServer 6.0 or newer.");

            return XenRef<Bond>.Create(session.proxy.bond_create(session.uuid, (_network != null) ? _network : "", (_members != null) ? Helper.RefListToStringArray(_members) : new string[] { }, (_mac != null) ? _mac : "").parse());
        }

        public static XenRef<Task> async_create(Session session, string _network, List<XenRef<PIF>> _members, string _mac)
        {
            if (Helper.APIVersionMeets(session, API_Version.API_1_9))
                System.Diagnostics.Debug.Assert(false, "Cannot use this call on XenServer 6.0 or newer.");

            return XenRef<Task>.Create(session.proxy.async_bond_create(session.uuid, (_network != null) ? _network : "", (_members != null) ? Helper.RefListToStringArray(_members) : new string[] { }, (_mac != null) ? _mac : "").parse());
        }
        
        // Backward compatibility for Bond.create in XenServer 6.0.

        public static XenRef<Bond> create(Session session, string _network, List<XenRef<PIF>> _members, string _mac, bond_mode _mode)
        {
            if (Helper.APIVersionMeets(session, API_Version.API_1_10))
                System.Diagnostics.Debug.Assert(false, "Cannot use this call on XenServer 6.1 or newer.");

            return XenRef<Bond>.Create(session.proxy.bond_create(session.uuid, (_network != null) ? _network : "", (_members != null) ? Helper.RefListToStringArray(_members) : new string[] { }, (_mac != null) ? _mac : "", bond_mode_helper.ToString(_mode)).parse());
        }

        public static XenRef<Task> async_create(Session session, string _network, List<XenRef<PIF>> _members, string _mac, bond_mode _mode)
        {
            if (Helper.APIVersionMeets(session, API_Version.API_1_10))
                System.Diagnostics.Debug.Assert(false, "Cannot use this call on XenServer 6.1 or newer.");

            return XenRef<Task>.Create(session.proxy.async_bond_create(session.uuid, (_network != null) ? _network : "", (_members != null) ? Helper.RefListToStringArray(_members) : new string[] { }, (_mac != null) ? _mac : "", bond_mode_helper.ToString(_mode)).parse());
        }
    }
}
