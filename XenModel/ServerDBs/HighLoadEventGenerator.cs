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
using System.Collections.ObjectModel;
using System.Text;
using XenAdmin.Core;
using XenAPI;
using System.Collections;
using XenAdmin.ServerDBs.FakeAPI;

namespace XenAdmin.ServerDBs
{
    internal class HighLoadEventGenerator
    {
        private readonly DbProxy _proxy;
        private DateTime lastHighLoadEvent;
        private bool _highLoadMode;
        
        public HighLoadEventGenerator(DbProxy proxy)
        {
            Util.ThrowIfParameterNull(proxy, "proxy");
            _proxy = proxy;
        }

        /// <summary>
        /// Gets or sets a value indicating whether high-load mode should be turned on.
        /// </summary>
        /// <value><c>true</c> if [high load mode]; otherwise, <c>false</c>.</value>
        private bool HighLoadMode
        {
            get
            {
                return _highLoadMode;
            }
            set
            {
                _highLoadMode = value;
            }
        }

        public List<Proxy_Event> GetHighLoadEvents()
        {
            return GetHighLoadEvents(new string[] {"*"}, "mod");
        }

        public List<Proxy_Event> GetHighLoadEvents(string[] classes, string eventOperation)
        {
            // this fires an event every 2 seconds (for this connection). This is roughly equivalent to the rate at which events occur
            // during a VM boot up.

            ReadOnlyCollection<string> typeNames = classes.Length == 0 || (classes.Length == 1 && classes[0] == "*")
                                               ? SimpleProxyMethodParser.AllTypes
                                               : new ReadOnlyCollection<string>(classes);

            List<Proxy_Event> output = new List<Proxy_Event>();

            if (HighLoadMode && DateTime.Now - lastHighLoadEvent > TimeSpan.FromSeconds(2))
            {
                lastHighLoadEvent = DateTime.Now;

                foreach (string typeName in typeNames)
                {
                    Response<object> resp;
                    if (TryGetAllRecords(typeName, out resp))
                    {
                        foreach (string opaqueRef in ((Hashtable)resp.Value).Keys)
                        {
                            output.Add(DbProxy.MakeProxyEvent(typeName, opaqueRef, eventOperation, TypeCache.GetProxyType(typeName), _proxy.get_record(typeName, opaqueRef, false)));
                        }
                    }
                }
            }
            return output;
        }

        public List<Proxy_Event> GetAddEvents(string[] classes)
        {
            var oldHighLoadMode = HighLoadMode;
            HighLoadMode = true;
            var output = GetHighLoadEvents(classes, "add");
            HighLoadMode = oldHighLoadMode;
            return output;
        }

        private bool TryGetAllRecords(string clazz, out Response<Object> response)
        {
            response = new Response<object>();
            if (_proxy.db.Tables.Keys.Contains(clazz.ToLower()))
            {
                fakeUnknown fu = new fakeUnknown(clazz, _proxy);
                response = fu.get_all_records();
                return true;
            }
            return false;
        }
    }
}
