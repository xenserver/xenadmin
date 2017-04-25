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
using XenAPI;

using XenAdmin.Actions;

namespace XenAdmin.Actions
{
    public class CreateVIFAction : AsyncAction
    {
        private Proxy_VIF _proxyVIF;

        public CreateVIFAction(VM vm, Proxy_VIF proxyVIF)
            : base(vm.Connection, String.Format(Messages.ACTION_VIF_CREATING_TITLE, vm.Name))
        {
            _proxyVIF = proxyVIF;
            VM = vm;
            XmlRpcMethods.ForEach( method => ApiMethodsToRoleCheck.Add( method ) );
        }

        /// <summary>
        /// A List of XML RPC methods used by this class
        /// </summary>
        public static readonly List<string> XmlRpcMethods = new List<string>()
        {
            "Async_VIF.create",
            "VIF.plug"                        
        };

        protected override void Run()
        {
            Description = Messages.ACTION_VIF_CREATING;
            CreateVIF();
            Description = Messages.ACTION_VIF_CREATED;
        }

        private void CreateVIF()
        {
            VIF newVIF = new VIF(_proxyVIF);
            RelatedTask = XenAPI.VIF.async_create(Session, newVIF);
            
            PollToCompletion();
            string newVifRef = Result;

            if (VM.power_state == XenAPI.vm_power_state.Running)
            {
                if (XenAPI.VIF.get_allowed_operations(Session, newVifRef).Contains(XenAPI.vif_operations.plug))
                {
                    // Try hotplug
                    XenAPI.VIF.plug(Session, newVifRef);
                    Result = true.ToString();
                }
                else
                {
                    Result = false.ToString();
                }
            }
            else
                Result = true.ToString();
        }



    }
}
