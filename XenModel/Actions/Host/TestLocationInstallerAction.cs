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
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Actions.HostActions
{
    public class TestLocationInstallerAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<string, string> Config;

        public TestLocationInstallerAction(Host host, Dictionary<string, string> config)
            : base(host.Connection, string.Empty, true)
        {
            Host = host;
            Config = config;
        }

        protected override void Run()
        {
            string result = null;

            try
            {
                result = Host.call_plugin(Session, Host.opaque_ref, "prepare_host_upgrade.py", "testUrl", Config);
            }
            catch (Failure failure)
            {
                if (failure.ErrorDescription.Count == 4)
                {
                    var key = failure.ErrorDescription[3];
                    if (key.StartsWith("REPO_SERVER_ERROR_"))
                        key = "REPO_SERVER_ERROR_5XX";

                    string fromResources = FriendlyNameManager.GetFriendlyName($"PREPARE_HOST_UPGRADE_{key}");
                    Exception = string.IsNullOrEmpty(fromResources) ? failure : new Exception(fromResources);
                }
                else
                    Exception = failure;
            }
            catch (Exception e)
            {
                Exception = e;
            }
            finally
            {
                if (string.IsNullOrEmpty(result) || result.ToLower() != "true")
                {
                    if (Exception == null)
                        Exception = new Exception(Messages.INSTALL_FILES_CANNOT_BE_FOUND);

                    log.ErrorFormat("Error testing upgrade hotfix: {0}", Exception);
                }
            }
        }
    }
}
