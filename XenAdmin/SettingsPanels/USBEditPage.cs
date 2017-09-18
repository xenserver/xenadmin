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

using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Controls;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.SettingsPanels
{
    public partial class USBEditPage : XenTabPage, IEditPage
    {
        private UsbList _usblist;
        private VM _vm;

        public USBEditPage()
        {
            InitializeComponent();
            _usblist = new UsbList();
            this.Controls.Add(_usblist);
        }

        #region override IEditPage

        public bool ValidToSave
        {
            get { return true; }
        }

        public bool HasChanged
        {
            get { return true; }
        }

        public override string Text
        {
            get { return Messages.USB_EDIT_TEXT; }
        }

        public string SubText
        { 
            get
            {
                if (null != _vm)
                {
                    return string.Format(Messages.USB_EDIT_SUBTEXT, _vm.VUSBs.Count);
                }
                else
                    return "";
            }
        }

        public Image Image
        {
            get { return Resources._001_Tools_h32bit_16; }
        }

        public AsyncAction SaveSettings()
        {
            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            Trace.Assert(clone is VM);

            _vm = (VM)clone;
            if (Connection == null)
                Connection = _vm.Connection;

            _usblist.XenObject = clone;
        }

        public void ShowLocalValidationMessages()
        {

        }

        public void Cleanup()
        {

        }

        
        #endregion
    }
}
