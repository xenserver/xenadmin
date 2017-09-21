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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class UsbUsageDialog : XenDialogBase
    {
        private PUSB _pusb;

        public UsbUsageDialog(PUSB pusb)
        {
            _pusb = pusb;
            InitializeComponent();
            refreshControls();
        }

        private void refreshControls()
        {
            if (_pusb.passthrough_enabled)
            {
                labelNote.Text = String.Format(Messages.DIALOG_USB_USAGE_NOTE_FORMAT,
                                               Messages.DIALOG_USB_USAGE_NOTE_DISABLE,
                                               Messages.DIALOG_USB_USAGE_NOTE_DENY);
                buttonOK.Text = Messages.DIALOG_USB_USAGE_OKBUTTON_DISABLE;
            }
            else
            {
                labelNote.Text = String.Format(Messages.DIALOG_USB_USAGE_NOTE_FORMAT,
                                               Messages.DIALOG_USB_USAGE_NOTE_ENABLE,
                                               Messages.DIALOG_USB_USAGE_NOTE_ALLOW);
                buttonOK.Text = Messages.DIALOG_USB_USAGE_OKBUTTON_ENABLE;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}