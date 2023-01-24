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

using System.Collections.Generic;
using System.ComponentModel;
using XenAPI;
using XenAdmin.Network;


namespace XenAdmin.Commands
{
    internal class ResumeVMOnHostToolStripMenuItem : VMOperationToolStripMenuItem
    {
        public ResumeVMOnHostToolStripMenuItem()
            : base(new ResumeVMOnHostCommand2(), false, vm_operations.resume_on)
        {
        }

        public ResumeVMOnHostToolStripMenuItem(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, bool inContextMenu)
            : base(new ResumeVMOnHostCommand2(mainWindow, selection), inContextMenu, vm_operations.resume_on)
        {
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new Command Command
        {
            get
            {
                return base.Command;
            }
        }

        private class ResumeVMOnHostCommand2 : Command
        {
            public ResumeVMOnHostCommand2()
            {
            }

            public ResumeVMOnHostCommand2(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
                : base(mainWindow, selection)
            {
            }

            private bool CanRun(SelectedItem selection)
            {
                VM vm = selection.XenObject as VM;
                
                if (vm != null && !vm.is_a_template && !vm.Locked)
                {
                    if (vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.suspend))
                    {
                        return false;
                    }

                    if (vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.resume) && EnabledTargetExists(selection.HostAncestor, selection.Connection))
                    {
                        // CA-19215: "Resume on" option present and enabled for VM on standalone host.
                        return selection.Connection != null && selection.Connection.Cache.HostCount > 1;
                    }
                }
                return false;
            }

            protected override bool CanRunCore(SelectedItemCollection selection)
            {
                IXenConnection connection = null;

                var atLeastOneCanRun = false;
                foreach (var item in selection)
                {
                    if (!(item.XenObject is VM vm))
                        return false;
                    
                    // all VMs must be on the same connection
                    if (connection != null && vm.Connection != connection)
                    {
                        return false;
                    }
                    connection = vm.Connection;

                    if (CanRun(item))
                    {
                        atLeastOneCanRun = true;
                    }
                }
                return atLeastOneCanRun;
            }

            private static bool EnabledTargetExists(Host host, IXenConnection connection)
            {
                if (host != null)
                    return host.enabled;

                foreach (Host h in connection.Cache.Hosts)
                {
                    if (h.enabled)
                        return true;
                }
                return false;
            }

            public override string MenuText
            {
                get
                {
                    return Messages.MAINWINDOW_RESUME_ON_HOST;
                }
            }
        }
    }
}
