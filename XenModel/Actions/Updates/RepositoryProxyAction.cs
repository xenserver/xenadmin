/* Copyright (c) Cloud Software Group, Inc. 
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

using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Actions.Updates
{
    public class RepositoryProxyAction : AsyncAction
    {
        private readonly string _proxyUrl;
        private readonly string _username;
        private string _password;
        private readonly Pool _pool;

        public const string DUMMY_PASSWORD = "dummy";

        public RepositoryProxyAction(IXenConnection connection, string proxyUrl = null, string username = null, string password = null)
            : base(connection, string.Empty)
        {
            _proxyUrl = proxyUrl;
            _username = username;
            _password = password;
            _pool = Helpers.GetPoolOfOne(connection);

            if (string.IsNullOrEmpty(proxyUrl))
            {
                Title = Description = Messages.YUM_REPO_ACTION_PROXY_DISABLE_TITLE;
                ApiMethodsToRoleCheck.Add("pool.disable_repository_proxy");
            }
            else
            {
                Title = Description = Messages.YUM_REPO_ACTION_PROXY_CONFIG_TITLE;
                ApiMethodsToRoleCheck.AddRange("Secret.get_record", "pool.configure_repository_proxy");
            }
        }

        protected override void Run()
        {
            //after sending the calls, wait until the cache has been updated so that the config panel
            //can show the new value if the action was triggered by hitting the Apply button
            
            if (string.IsNullOrWhiteSpace(_proxyUrl))
            {
                Pool.disable_repository_proxy(Session, _pool.opaque_ref);
                Connection.WaitFor(() => string.IsNullOrWhiteSpace(_pool.repository_proxy_url), null);
            }
            else
            {
                if (_password == DUMMY_PASSWORD)
                    _password = Secret.get_record(Connection.Session, _pool.repository_proxy_password).value;

                Pool.configure_repository_proxy(Session, _pool.opaque_ref, _proxyUrl, _username, _password);

                Connection.WaitFor(() => _pool.repository_proxy_url == _proxyUrl &&
                                         _pool.repository_proxy_username == _username &&
                                         _pool.repository_proxy_password == _password, null);
            }
        }
    }
}
