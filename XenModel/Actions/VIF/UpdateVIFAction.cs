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
using XenAdmin.Actions;
using XenAPI;


namespace XenAdmin.Actions
{
    public class UpdateVIFAction : AsyncAction
    {
        readonly private VIF vif;
        private readonly Proxy_VIF proxyVIF;

        private List<string> xmlRpcMethods = new List<string>();

        /// <summary>
        /// Update the VIF
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="vif"></param>
        /// <param name="proxyVIF"></param>
        public UpdateVIFAction(VM vm, VIF vif, Proxy_VIF proxyVIF)
            : base(vm.Connection, String.Format(Messages.ACTION_VIF_UPDATING_TITLE, vif.NetworkName(), vm.Name))
        {
            this.vif = vif;
            VM = vm;
            this.proxyVIF = proxyVIF;
            Initialise();
            xmlRpcMethods.ForEach(method => ApiMethodsToRoleCheck.Add(method));
        }

        private void Initialise()
        {
            xmlRpcMethods.AddRange(DeleteVIFAction.XmlRpcMethods);
            xmlRpcMethods.AddRange(CreateVIFAction.XmlRpcMethods);
        }

        private void UpdateVIF()
        {
            new DeleteVIFAction(vif).RunExternal(Session);
            var createAction = new CreateVIFAction(VM, proxyVIF);
            createAction.RunExternal(Session);
            Result = createAction.Result;
        }

        protected override void Run()
        {
            Description = Messages.ACTION_VIF_UPDATING;
            UpdateVIF();
            Description = Messages.ACTION_VIF_UPDATED;
        }
    }
}
