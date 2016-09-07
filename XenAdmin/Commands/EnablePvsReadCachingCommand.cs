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
using System.Text;
using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Properties;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Linq;
using XenAdmin.Model;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows the properties dialog for the selected XenObject.
    /// </summary>
    internal class EnablePvsReadCachingCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        internal EnablePvsReadCachingCommand()
        {
        }

        public EnablePvsReadCachingCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public EnablePvsReadCachingCommand(IMainWindow mainWindow, IXenObject xenObject)
            : base(mainWindow, xenObject)
        {
        }

        protected virtual void Execute(IList<VM> vms)
        {
            using (var dlg = new EnablePvsReadCachingDialog(vms))
            {
                dlg.ShowDialog(Program.MainWindow); // TODO: use result
            }
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Execute(selection.AsXenObjects<VM>());
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            // Can execute criteria: A selection of VMs in the same pool, where at least one doesn't have PVS read caching enabled
            if (selection.Any() &&  selection.AllItemsAre<VM>() && selection.GetConnectionOfAllItems() != null)
            {
                var vms = selection.AsXenObjects<VM>();
                if (vms.Any(vm => !PvsProxyAlreadyEnabled(vm)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// A VM can be enabled if there isn't already a PVS_Proxy on its VIF
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        private bool PvsProxyAlreadyEnabled(VM vm)
        {
            var connection = vm.Connection;

            var pvsProxies = connection.Cache.PVS_proxies;

            foreach (var pvsProxy in pvsProxies)
            {
                if (vm.Equals(pvsProxy.VM))
                {
                    return true; // Already got a PVS proxy on this vm
                }
            }

            return false; // No PVS proxy is on this VM
        }

        public override string ContextMenuText
        {
            get { return "Enable PVS read-caching"; }
        }

        public override Image MenuImage
        {
            get
            {
                return Resources.edit_16;
            }
        }
    }
}
