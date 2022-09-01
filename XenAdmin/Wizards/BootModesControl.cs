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

using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using BootMode = XenAdmin.Actions.VMActions.BootMode;

namespace XenAdmin.Wizards
{
    public partial class BootModesControl : UserControl
    {
        public BootModesControl()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public VM TemplateVM
        {
            get { return _templateVM; }
            set
            {
                if (_templateVM == value)
                    return;

                _templateVM = value;

                if (_templateVM != null)
                {
                    _connection = _templateVM.Connection;
                }
                UpdateControls();
            }
        }
        private VM _templateVM;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IXenConnection Connection
        {
            get { return _connection; }
            set
            {
                if (_connection == value)
                    return;

                _connection = value;
                UpdateControls();
            }
        }
        private IXenConnection _connection;

        public BootMode SelectedOption
        {
            get { return radioButtonUEFISecureBoot.Checked ? BootMode.UEFI_SECURE_BOOT : (radioButtonUEFIBoot.Checked ? BootMode.UEFI_BOOT : BootMode.BIOS_BOOT); }
        }

        public void CheckBIOSBootMode()
        {
            radioButtonBIOSBoot.Checked = true;
        }

        private void UpdateControls()
        {
            radioButtonBIOSBoot.Enabled = true;
            radioButtonUEFIBoot.Visible = !Helpers.FeatureForbidden(_connection, Host.UefiBootDisabled);
            radioButtonUEFISecureBoot.Visible = !Helpers.FeatureForbidden(_connection, Host.UefiSecureBootDisabled);

            // ensure that a visible option is selected
            if (radioButtonUEFIBoot.Checked && !radioButtonUEFIBoot.Visible)
                radioButtonBIOSBoot.Checked = true;

            if (radioButtonUEFISecureBoot.Checked && !radioButtonUEFISecureBoot.Visible)
                radioButtonBIOSBoot.Checked = true;

            if (_templateVM != null)
            {
                radioButtonUEFIBoot.Enabled = _templateVM.CanSupportUEFIBoot();
                radioButtonUEFISecureBoot.Enabled = _templateVM.CanSupportUEFISecureBoot();

                if (_templateVM.IsUEFIEnabled())
                {
                    if (_templateVM.IsSecureBootEnabled())
                        radioButtonUEFISecureBoot.Checked = true;
                    else
                        radioButtonUEFIBoot.Checked = true;

                    if (!_templateVM.CanChangeBootMode())
                        radioButtonBIOSBoot.Enabled = false;
                }
                else
                {
                    radioButtonBIOSBoot.Checked = true;
                    if (!_templateVM.CanChangeBootMode())
                        radioButtonUEFIBoot.Enabled = radioButtonUEFISecureBoot.Enabled = false;
                }
            }
            else
            {
                radioButtonBIOSBoot.Checked = true;
                radioButtonUEFIBoot.Checked = false;
                radioButtonUEFISecureBoot.Checked = false;
            }
            
            ShowTemplateWarning();
        }

        private void ShowTemplateWarning()
        {
            if (_templateVM == null)
            {
                imgUnsupported.Visible = labelUnsupported.Visible = false;
                return;
            }

            if (radioButtonBIOSBoot.Visible && !radioButtonBIOSBoot.Enabled)
            {
                imgUnsupported.Visible = labelUnsupported.Visible = true;
                labelUnsupported.Text = Messages.BIOS_BOOT_MODE_UNSUPPORTED_WARNING;
                return;
            }

            var uefiNotSupported = radioButtonUEFIBoot.Visible && !radioButtonUEFIBoot.Enabled;
            var uefiSecureNotSupported = radioButtonUEFISecureBoot.Visible && !radioButtonUEFISecureBoot.Enabled;
            if (uefiNotSupported || uefiSecureNotSupported)
            {
                imgUnsupported.Visible = labelUnsupported.Visible = true;
                labelUnsupported.Text = uefiNotSupported && uefiSecureNotSupported
                    ? Messages.GUEFI_BOOT_MODES_UNSUPPORTED_WARNING
                    : uefiNotSupported
                        ? Messages.GUEFI_BOOT_MODE_UNSUPPORTED_WARNING
                        : Messages.GUEFI_SECUREBOOT_MODE_UNSUPPORTED_WARNING;
            }
            else
            {
                imgUnsupported.Visible = labelUnsupported.Visible = false;
            }
        }

        public static bool ShowBootModeOptions(IXenConnection connection)
        {
            return Helpers.NaplesOrGreater(connection) && 
                   (!Helpers.FeatureForbidden(connection, Host.UefiBootDisabled) || !Helpers.FeatureForbidden(connection, Host.UefiSecureBootDisabled));
        }
    }
}
