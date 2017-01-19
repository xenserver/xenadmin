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
using XenAdmin.Actions;
using XenAdmin.Core;

namespace XenAdmin.Commands
{
    class RescanPIFsCommand : Command
    {
        /// <summary>
        /// Executes a PIF scan on a host
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="vbd"></param>
        public RescanPIFsCommand(IMainWindow mainWindow, Host host)
            : base(mainWindow, host)
        {

        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.Count == 1 && CanExecute(selection[0].XenObject as Host);
        }

        private bool CanExecute(Host host)
        {
            if (host == null)
                return false;

            return true;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Host h = selection[0].XenObject as Host;
            RescanPIFsAction action = new RescanPIFsAction(h);
            action.RunAsync();
        }
    }
}
