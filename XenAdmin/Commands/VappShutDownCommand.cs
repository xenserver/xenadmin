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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.Commands
{
    class VappShutDownCommand : Command
    {
        public VappShutDownCommand()
        { }

        public VappShutDownCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        { }

        public override string MenuText { get { return Messages.VAPP_SHUT_DOWN_MENU; } }

        public override string ContextMenuText { get { return Messages.VAPP_SHUTDOWN_CONTEXT_MENU; } }

        public override Image MenuImage { get { return Images.StaticImages._001_ShutDown_h32bit_16; } }

        protected override bool ConfirmationRequired { get { return true; } }

        protected override string ConfirmationDialogTitle { get { return Messages.VM_APPLIANCE_SHUT_DOWN; } }

        protected override string ConfirmationDialogText
        {
            get
            {
                var selection = GetSelection();
                var appsToShutDown = new List<VM_appliance>();

                if (selection.AllItemsAre<VM_appliance>())
                {
                    appsToShutDown = (from IXenObject obj in selection.AsXenObjects()
                                      let app = (VM_appliance)obj
                                      where CanShutDownAppliance(app)
                                      select app).ToList();
                }
                else if (selection.AllItemsAre<VM>())
                {
                    var firstVm = (VM)selection.First;
                    appsToShutDown.Add(firstVm.Connection.Resolve(firstVm.appliance));
                }

                var appNames = (from VM_appliance app in appsToShutDown select app.Name).ToArray();
                return string.Format(Messages.CONFIRM_SHUT_DOWN_APPLIANCES, string.Join(", ", appNames));
            }
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.AllItemsAre<VM_appliance>())
                return selection.AtLeastOneXenObjectCan<VM_appliance>(CanShutDownAppliance);

            if (selection.AllItemsAre<VM>())
            {
                var firstVm = (VM)selection.First;
                if (firstVm.IsAssignedToVapp)
                {
                    var firstVapp = firstVm.appliance;
                    if (selection.AsXenObjects<VM>().All(vm => vm.appliance != null && vm.appliance.opaque_ref == firstVapp.opaque_ref))
                        return CanShutDownAppliance(firstVm.Connection.Resolve(firstVapp));
                }
            }

            return false;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var appsToShutDown = new List<VM_appliance>();

            if (selection.AllItemsAre<VM_appliance>())
            {
                appsToShutDown = (from IXenObject obj in selection.AsXenObjects()
                                  let app = (VM_appliance)obj
                                  where CanShutDownAppliance(app)
                                  select app).ToList();
            }
            else if (selection.AllItemsAre<VM>())
            {
                var firstVm = (VM)selection.First;
                appsToShutDown.Add(firstVm.Connection.Resolve(firstVm.appliance));
            }

            foreach (var app in appsToShutDown)
                (new ShutDownApplianceAction(app)).RunAsync();
        }

        private bool CanShutDownAppliance(VM_appliance app)
		{
		    return app != null && (app.allowed_operations.Contains(vm_appliance_operation.clean_shutdown)
		           || app.allowed_operations.Contains(vm_appliance_operation.hard_shutdown));
		}
    }
}
