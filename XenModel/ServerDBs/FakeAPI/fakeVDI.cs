/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Text;
using XenAPI;
using System.Collections;

namespace XenAdmin.ServerDBs.FakeAPI
{
    internal class fakeVDI : fakeXenObject
    {
        public fakeVDI(DbProxy proxy)
            : base("vdi", proxy)
        {

        }

        public Response<string> destroy(string session, string opaque_ref)
        {
            return destroyObj("vdi", opaque_ref);
        }

        public Response<string> create(string session, Proxy_VDI vdi)
        {
            return new Response<string>(createObj("vdi", vdi).Value);
        }

        public Response<string> copy(string session, string opaque_ref, string target_sr)
        {
            string new_vdi = proxy.CopyObject("vdi", opaque_ref);
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vdi", new_vdi, "SR", target_sr);
            proxy.SendCreateObject("vdi", new_vdi);
            return new Response<string>(new_vdi);
        }

        public Response<string> set_sm_config(string session, string opaque_ref, Hashtable sm_config)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vdi", opaque_ref, "sm_config", sm_config);
            return new Response<string>("");
        }

        public Response<object> get_sm_config(string session, string opaque_ref)
        {
            return new Response<object>(proxy.db.GetValue("vdi", opaque_ref, "sm_config"));
        }
    }
}
