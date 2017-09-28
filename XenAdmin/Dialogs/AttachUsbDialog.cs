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
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class AttachUsbDialog : XenDialogBase
    {
        private VM _vm;

        public AttachUsbDialog(VM vm): base(vm.Connection)
        {
            _vm = vm;
            InitializeComponent();
            BuildList();
            treeUsbList_SelectedIndexChanged(null, null);
        }

        private void BuildList()
        {
            Program.AssertOnEventThread();

            treeUsbList.BeginUpdate();
            try
            {
                VM.HA_Restart_Priority SelectedPriority = _vm.HARestartPriority();
                Pool pool = Helpers.GetPool(_vm.Connection);
                // Check if HA was enabled on pool and Restart priority was set on VM.
                bool haEnabled = (pool != null &&
                    pool.ha_enabled &&
                    VM.HaPriorityIsRestart(_vm.Connection, SelectedPriority));
                List<XenRef<Host>> possibleHostRefs = VM.get_possible_hosts(_vm.Connection.Session, _vm.opaque_ref);
                List<Host> possibleHosts = new List<Host>();
                foreach (XenRef<Host> possibleHostRef in possibleHostRefs)
                {
                    possibleHosts.Add(_vm.Connection.Resolve(possibleHostRef));
                }

                foreach (Host host in possibleHosts)
                {
                    // Add a host node to tree list.
                    HostItem hostNode = new HostItem(host);
                    treeUsbList.AddNode(hostNode);
                    List<PUSB> pusbs = _vm.Connection.ResolveAll(host.PUSBs);
                    foreach (PUSB pusb in pusbs)
                    {
                        // Add a USB in the host to tree list.
                        // Determin if the USB is valid to attach.
                        if ((haEnabled == false) &&
                            (pusb != null) && 
                            (pusb.passthrough_enabled == true) &&
                            (pusb.attached == null))
                        {
                            UsbItem usbNode = new UsbItem(pusb);
                            treeUsbList.AddChildNode(hostNode, usbNode);
                        }
                    }
                }
            }
            finally
            {
                treeUsbList.EndUpdate();
            }
        }

        private void treeUsbList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UsbItem item = treeUsbList.SelectedItem as UsbItem;
            if (item != null)
            {
                buttonAttach.Enabled = true;
            }
            else
            {
                buttonAttach.Enabled = false;
            }
        }

        private void buttonAttach_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private class HostItem : CustomTreeNode
        {
            private Host _host;

            public HostItem(Host host) :base(false)
            {
                _host = host;
                Text = host.name_label;
            }
        }

        private class UsbItem : CustomTreeNode
        {
            private PUSB _pusb;

            public UsbItem(PUSB pusb) :base(true)
            {
                _pusb = pusb;
                Text = String.Format(Messages.STRING_SPACE_STRING, _pusb.path, _pusb.description);
            }
        }
    }
}
