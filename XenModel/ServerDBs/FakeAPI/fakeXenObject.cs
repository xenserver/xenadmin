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
using System.Reflection;
using XenAdmin.Core;

namespace XenAdmin.ServerDBs.FakeAPI
{
    internal abstract class fakeXenObject
    {
        protected DbProxy proxy;
        private string clazz;

        protected fakeXenObject(string clazz, DbProxy proxy)
        {
            this.proxy = proxy;
            this.clazz = clazz;
        }

        protected Response<string> destroyObj(string clazz, string opaque_ref)
        {
            proxy.db.Tables[clazz].Rows.Remove(opaque_ref);
            proxy.SendDestroyObject(clazz, opaque_ref);
            return new Response<string>("");
        }

        protected Response<string> createObj(string clazz, object obj)
        {
            Db.Table t = proxy.db.Tables[clazz];
            string opaque_ref = proxy.CreateOpaqueRef();
            Db.Row r = t.Rows.Add(opaque_ref);
            r.PopulateFrom(DbProxy.ProxyToHashtable(obj.GetType(), obj));
            proxy.SendCreateObject(clazz, opaque_ref);
            return new Response<string>(opaque_ref);
        }

        public Response<object> get_other_config(string session, string opaque_ref)
        {
            return new Response<object>(proxy.db.GetValue(clazz, opaque_ref, "other_config"));
        }

        public Response<string> set_other_config(string session, string opaque_ref, Hashtable other_config)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, clazz, opaque_ref, "other_config", other_config);
            return new Response<string>("");
        }

        public Response<Object> get_all_records()
        {
            Db.Table table = proxy.db.Tables[clazz.ToLower()];
            Hashtable result = new Hashtable();

            foreach (KeyValuePair<string, Db.Row> row in table.Rows)
            {
                Hashtable o = new Hashtable();

                foreach (string propName in row.Value.Props.Keys)
                {
                    o[propName] = row.Value.Props[propName].XapiObjectValue;
                }

                result[row.Key] = o;
            }

            return new Response<Object>(result);
        }

        protected void ThrowIfReadOnly()
        {
            if (!proxy.IsSuperUser)
            {
                throw new Failure("read only");
            }
        }
    }
}
