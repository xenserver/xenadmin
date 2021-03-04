﻿/* Copyright (c) Citrix Systems, Inc. 
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

using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.ServerDBs.FakeAPI
{
    internal class fakePool : fakeXenObject
    {
        public fakePool(DbProxy proxy)
            : base("pool", proxy)
        {

        }

        public Response<string> designate_new_master(string session, string host)
        {
            Pool pool = Helpers.GetPoolOfOne(proxy.connection);
            proxy.EditObject_(DbProxy.EditTypes.Replace, "pool", pool.opaque_ref, "master", host);
            return new Response<string>("");
        }

        public Response<string> join(string session, string master_address, string master_username, string master_password)
        {
            DbProxy destination = null;
            string hostRef = "OpaqueRef:179a6549-c043-772a-404e-5f6c874369f2";
            
            foreach (DbProxy proxy in DbProxy.proxys.Values)
            {
                if (proxy.connection.Hostname == master_address)
                {
                    destination = proxy;
                    break;
                }
            }

            // add host to pool
            destination.db.Tables["host"].Rows.Add(hostRef, this.proxy.db.Tables["host"].Rows[hostRef].CopyOf());
            destination.SendCreateObject("host", hostRef);

            // remove host from this db.
            return destroyObj("host", hostRef);
        }
    }
}
