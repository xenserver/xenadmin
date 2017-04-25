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

namespace XenAdmin.ServerDBs.FakeAPI
{
    internal class fakePool_Patch : fakeXenObject
    {
        public fakePool_Patch(DbProxy proxy)
            : base("pool_patch", proxy)
        {

        }

        public Response<string> create(string session, Proxy_Pool_patch patch)
        {
            return createObj("pool_patch", patch);
        }

        public Response<string> precheck(string session, string opaque_ref)
        {
            return new Response<string>("");
        }

        public Response<string> apply(string session, string opaque_ref, string host_ref)
        {
            Proxy_Host_patch host_patch = new Proxy_Host_patch();
            host_patch.applied = true;
            host_patch.host = host_ref;
            host_patch.name_label = "fake-patch";
            host_patch.name_description = "";
            host_patch.pool_patch = opaque_ref;
            host_patch.size = "0";
            host_patch.timestamp_applied = DateTime.Now;
            host_patch.uuid = Guid.NewGuid().ToString();
            host_patch.version = "1.0";

            new fakeHost_Patch(proxy).create(session, host_patch);
            proxy.SendModObject("pool_patch", opaque_ref);
            proxy.SendModObject("host", host_ref);
            return new Response<string>("");
        }
    }
}
