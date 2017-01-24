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
    internal class fakeSession
    {
        private readonly DbProxy _proxy;
        public fakeSession(DbProxy proxy)
        {
            _proxy = proxy;
        }

        public Response<String> login_with_password(string username, string password, string version)
        {
            _proxy.SetIsSuperUser(username != "readonly");
            return new Response<String>("dummy");
        }

        public Response<String> logout()
        {
            return new Response<String>("dummy");
        }

        // For Session.login_with_password on George
        public Response<bool> get_is_local_superuser()
        {
            return new Response<bool>(_proxy.IsSuperUser);
        }

        public Response<string[]> get_all_subject_identifiers()
        {
            return new Response<string[]>(new string[] { "SID1", "SID2", "SID3" });
        }

        public Response<string> get_subject()
        {
            return new Response<string>("dummy");
        }

        public Response<string> get_auth_user_sid(string self)
        {
            return new Response<string>("dummy");
        }

        public Response<string[]> get_rbac_permissions(string uuid)
        {
            return new Response<string[]>(new string[] { "dummy" });
        }
    }
}
