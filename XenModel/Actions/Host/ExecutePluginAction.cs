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
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class ExecutePluginAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public class Failed : Exception
        {
            public object Result;
            public Failed(object result) : base(result.ToString())
            {
                Result = result;
            }
        }

        private string Plugin;
        private string Function;
        private Dictionary<string, string> Args;

        public ExecutePluginAction(IXenConnection connection, Host host, string plugin, string function, Dictionary<string, string> args, bool hide_from_logs)
            : base(connection, string.Format(Messages.PLUGIN_TITLE, plugin),hide_from_logs)
        {
            Host = host;
            Plugin = plugin;
            Function = function;
            Args = args;
        }

        protected override void Run()
        {
            Description = string.Format(Messages.PLUGIN_CALLING, Plugin);
            Result = XenAPI.Host.call_plugin(Session, Host.opaque_ref, Plugin, Function, Args);

            if (Result == "True")
            {
                Description = string.Format(Messages.PLUGIN_SUCCEED, Plugin);
                return;
            }

            if (Result.StartsWith("True"))
            {
                Description = string.Format(Messages.PLUGIN_SUCCEED, Plugin);
                Result = Result.Remove(0, 4);
                return;
            }

            log.WarnFormat("Plugin call {0}.{1}({2}) on {3} failed with {4}", Plugin, Function, ArgString(), Host.uuid, Result);
            Exception = new Failed(Result);
        }

        private string ArgString()
        {
            List<string> l = new List<string>();
            foreach (string key in Args.Keys)
            {
                l.Add(key + "=" + Args[key]);
            }
            return string.Join(", ", l.ToArray());
        }
    }
}
