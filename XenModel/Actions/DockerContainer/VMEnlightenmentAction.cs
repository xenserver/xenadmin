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
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Actions
{
    public class VMEnlightenmentAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string action;

        public VMEnlightenmentAction(VM vm, string action, string title, string description, bool suppressHistory)
            : base(vm.Connection, title, description, suppressHistory)
        {
            VM = vm;
            this.action = action;
        }

        protected override void Run()
        {
            var host = Helpers.GetMaster(Connection);

            var args = new Dictionary<string, string> { { "vmuuid", VM.uuid } };
            Result = Host.call_plugin(Session, host.opaque_ref, "xscontainer", action, args);

            if (Result.ToLower().StartsWith("true"))
                Description = Messages.ACTION_STATUS_SUCCEEDED;
            else
            {
                Exception = new Exception(Result ?? Messages.ERROR_UNKNOWN);
                log.WarnFormat("Plugin call xscontainer.{0}({1}) on {2} failed with {3}", action, VM.uuid, Host.Name, Exception.Message);
            }
        }
    }

    public class EnableVMEnlightenmentAction : VMEnlightenmentAction
    {
        public EnableVMEnlightenmentAction(VM vm, bool suppressHistory)
            : base(vm, "register", String.Format(Messages.ACTION_ENABLE_VM_ENLIGHTENMENT_TITLE, vm.Name), 
                Messages.ACTION_ENABLE_VM_ENLIGHTENMENT_DESCRIPTION, suppressHistory)
        { }
    }

    public class DisableVMEnlightenmentAction : VMEnlightenmentAction
    {
        public DisableVMEnlightenmentAction(VM vm, bool suppressHistory)
            : base(vm, "deregister", String.Format(Messages.ACTION_DISABLE_VM_ENLIGHTENMENT_TITLE, vm.Name), 
                Messages.ACTION_DISABLE_VM_ENLIGHTENMENT_DESCRIPTION, suppressHistory)
        { }
    }
}