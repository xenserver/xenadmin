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
using System.Diagnostics;
using XenAdmin.Actions;
using XenAPI;

namespace XenAdmin.Commands
{
	internal class UpdateVIFCommand : BaseVIFCommand
    {
        private VM _vm;
        private VIF _vif;
        private Proxy_VIF _proxyVIF;

        public event EventHandler Completed;

        public UpdateVIFCommand(IMainWindow mainWindow, VM vm, VIF vif, Proxy_VIF proxyVIF)
            : base(mainWindow, vm)
        {
            _vm = vm;
            _vif = vif;
            _proxyVIF = proxyVIF;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Trace.Assert(selection.Count == 1);

            var action = new UpdateVIFAction(_vm, _vif, _proxyVIF);
            action.Completed += action_Completed;
            action.RunAsync();
        }

        protected override void action_Completed(ActionBase sender)
        {
            EventHandler handler = Completed;

            if (handler != null)
                handler(this, new EventArgs());

            base.action_Completed(sender);
        }
    }
}
