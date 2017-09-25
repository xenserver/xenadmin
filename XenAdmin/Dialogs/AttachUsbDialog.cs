﻿/* Copyright (c) Citrix Systems, Inc. 
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

            Text = Messages.DIALOG_USB_ATTACH_TITLE;
            labelNote.Text = Messages.DIALOG_USB_ATTACH_NOTE;
            labelWarningLine1.Text = Messages.DIALOG_USB_ATTACH_WARNING_LINE1;
            labelWarningLine2.Text = Messages.DIALOG_USB_ATTACH_WARNING_LINE2;
            labelWarningLine3.Text = Messages.DIALOG_USB_ATTACH_WARNING_LINE3;

            BuildList();
            treeUsbList_SelectedIndexChanged(null, null);
        }

        private void BuildList()
        {
            Program.AssertOnEventThread();

            treeUsbList.BeginUpdate();
            try
            {
                Host[] hosts = _vm.Connection.Cache.Hosts;
                foreach (Host host in hosts)
                {
                    HostItem hostNode = new HostItem(host);
                    treeUsbList.AddNode(hostNode);
                    List<PUSB> pusbs = _vm.Connection.ResolveAll(host.PUSBs);
                    foreach (PUSB pusb in pusbs)
                    {
                        UsbItem usbNode = new UsbItem(pusb);
                        treeUsbList.AddChildNode(hostNode, usbNode);
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
                Description = null;
                Image = Images.GetImage16For(Images.GetIconFor(_host));
            }
        }

        private class UsbItem : CustomTreeNode
        {
            private PUSB _pusb;

            public UsbItem(PUSB pusb) :base(true)
            {
                _pusb = pusb;
                Text = String.Format("{0} {1}", _pusb.path, _pusb.description);
                Description = null;
                Image = null;
            }
        }
    }
}
