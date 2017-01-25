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
    internal class fakeSecret : fakeXenObject
    {
        public fakeSecret(DbProxy proxy)
            : base("secret", proxy)
        {
        }

        public Response<string> create(string session, Proxy_Secret secret)
        {
            secret.uuid = secret.value;
            return createObj("Secret", secret); 
        }

        public Response<string> get_uuid(string session, string opaque_ref)
        {
            return new Response<string>((string)proxy.db.GetValue("secret", opaque_ref, "uuid"));
        }

        public Response<string> get_by_uuid(string session, string uuid)
        {
            return new Response<string>((string)proxy.db.ObjectWithFieldValue("secret", "uuid", uuid));
        }

        public Response<string> get_value(string session, string opaque_ref)
        {
            try
            {
                return new Response<string>(proxy.db.Tables["secret"].Rows[opaque_ref].Props["value"].XapiObjectValue.ToString());
            }
            catch (Exception e)
            {
                return new Response<string>(true, new[] { e.Message });
            }
        }
    }
}
