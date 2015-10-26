/* Copyright (c) Citrix Systems Inc. 
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
using System.Threading;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Network.StorageLink;


namespace XenAdmin.Actions
{
    class SetCslgCredentialsAction : AsyncAction
    {
        private readonly string _host;
        private readonly string _username;
        private readonly string _password;
        private readonly List<IXenConnection> _connections;

        public SetCslgCredentialsAction(IEnumerable<IXenConnection> connections, string host, string username, string password, bool suppressHistory)
            : base(null, Messages.SET_STORAGELINK_CREDS_ACTION_TITLE, Messages.SET_STORAGELINK_CREDS_ACTION_DESCRIPTION, suppressHistory)
        {
            Util.ThrowIfEnumerableParameterNullOrEmpty(connections, "connections");

            _connections = Util.GetList(connections).FindAll(c => c.IsConnected && Helpers.GetPoolOfOne(c) != null && !Helpers.FeatureForbidden(c, XenAPI.Host.RestrictStorageChoices));
            _host = host;
            _username = username;
            _password = password;

            foreach (IXenConnection c in connections)
            {
                XenAPI.Pool pool = Helpers.GetPool(c);

                if (pool != null)
                {
                    AppliesTo.Add(pool.opaque_ref);
                }
                else
                {
                    XenAPI.Host master = Helpers.GetMaster(c);

                    if (master != null)
                    {
                        AppliesTo.Add(master.opaque_ref);
                    }
                }
            }
        }

        protected override void Run()
        {
            var actions = new List<AsyncAction>();
            foreach (IXenConnection c in _connections)
            {
                actions.Add(new SetCslgCredentialsToPoolAction(c, _host, _username, _password));
            }

            new MultipleAction(null, Messages.SET_STORAGELINK_CREDS_ACTION_TITLE, Messages.SET_STORAGELINK_CREDS_ACTION_DESCRIPTION, Messages.SET_STORAGELINK_CREDS_ACTION_TITLE, actions).RunExternal(null);
        }
    }
}
