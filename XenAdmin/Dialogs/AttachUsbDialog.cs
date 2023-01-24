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

using System;
using System.Collections.Generic;
using XenAdmin.Controls;
using XenAdmin.Actions;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class AttachUsbDialog : XenDialogBase
    {
        private VM _vm;
        private List<Host> possibleHosts;
        
        public AttachUsbDialog(VM vm): base(vm.Connection)
        {
            _vm = vm;
            possibleHosts = new List<Host>();
            InitializeComponent();
            BuildList();
            treeUsbList_SelectedIndexChanged(null, null);

            DelegatedAsyncAction action = new DelegatedAsyncAction(_vm.Connection,
                string.Format(Messages.FETCH_POSSIBLE_HOSTS, _vm.Name()),
                string.Format(Messages.FETCHING_POSSIBLE_HOSTS, _vm.Name()),
                string.Format(Messages.FETCHED_POSSIBLE_HOSTS, _vm.Name()),
                delegate (Session session)
                {
                    List<XenRef<Host>> possibleHostRefs = VM.get_possible_hosts(_vm.Connection.Session, _vm.opaque_ref);
                    possibleHosts = _vm.Connection.ResolveAll(possibleHostRefs);
                },
                true);
            action.Completed += delegate
            {
                Program.Invoke(Program.MainWindow, BuildList);
            };
            action.RunAsync();
        }

        private void BuildList()
        {
            Program.AssertOnEventThread();

            labelWarningLine3.Visible = !_vm.UsingUpstreamQemu();
            
            treeUsbList.ClearAllNodes();
            treeUsbList.BeginUpdate();
            try
            {
                List<UsbItem> usbNodeList = new List<UsbItem>();
                foreach (Host host in possibleHosts)
                {
                    // Add a host node to tree list.
                    HostItem hostNode = new HostItem(host);
                    List<PUSB> pusbs = host.Connection.ResolveAll(host.PUSBs);
                    foreach (PUSB pusb in pusbs)
                    {
                        // Add a USB in the host to tree list.
                        // Determin if the USB is valid to attach.
                        USB_group usbGroup = pusb.Connection.Resolve(pusb.USB_group);
                        bool attached = (usbGroup != null) && (usbGroup.VUSBs != null) && (usbGroup.VUSBs.Count > 0);

                        if (pusb.passthrough_enabled && !attached)
                        {
                            UsbItem usbNode = new UsbItem(pusb);
                            usbNodeList.Add(usbNode);
                        }
                    }
                    // Show host node only when it contains available USB devices.
                    if (usbNodeList.Count > 0)
                    {
                        treeUsbList.AddNode(hostNode);
                        foreach (UsbItem item in usbNodeList)
                        {
                            treeUsbList.AddChildNode(hostNode, item);
                        }
                    }
                    usbNodeList.Clear();
                }
                
                if (treeUsbList.Nodes.Count == 0)
                {
                    CustomTreeNode noDeviceNode = new CustomTreeNode(false);
                    noDeviceNode.Text = Messages.DIALOG_ATTACH_USB_NO_DEVICES_AVAILABLE;
                    treeUsbList.AddNode(noDeviceNode);
                }
            }
            finally
            {
                treeUsbList.EndUpdate();
            }
        }

        private void treeUsbList_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonAttach.Enabled = treeUsbList.SelectedItem is UsbItem;
        }

        private void buttonAttach_Click(object sender, EventArgs e)
        {
            UsbItem item = treeUsbList.SelectedItem as UsbItem;
            if (item != null)
                new XenAdmin.Actions.CreateVUSBAction(item.Pusb, _vm).RunAsync();
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
            public PUSB Pusb { get; private set; }

            public UsbItem(PUSB pusb) :base(true)
            {
                Pusb = pusb;
                Text = String.Format(Messages.STRING_SPACE_STRING, Pusb.path, Pusb.Description());
            }
        }
    }
}
