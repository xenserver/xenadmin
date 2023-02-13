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
using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Wizards
{
    public partial class BootModesControl : UserControl
    {
        private VM _templateVM;
        private IXenConnection _connection;
        private bool _poolHasCertificates;

        public BootModesControl()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public VM TemplateVM
        {
            get => _templateVM;
            set
            {
                if (_templateVM == value)
                    return;

                _templateVM = value;

                if (_templateVM != null)
                {
                    _connection = _templateVM.Connection;
                    var pool = Helpers.GetPoolOfOne(_connection);
                    _poolHasCertificates = !string.IsNullOrEmpty(pool?.uefi_certificates);
                }

                UpdateControls();
                UpdateTpmControls();
            }
        }
        

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IXenConnection Connection
        {
            get => _connection;
            set
            {
                if (_connection == value)
                    return;

                _connection = value;
                var pool = Helpers.GetPoolOfOne(_connection);
                _poolHasCertificates = !string.IsNullOrEmpty(pool?.uefi_certificates);
                
                UpdateControls();
                UpdateTpmControls();
            }
        }

        public VmBootMode SelectedBootMode =>
            radioButtonUEFISecureBoot.Checked
                ? VmBootMode.SecureUefi
                : radioButtonUEFIBoot.Checked
                    ? VmBootMode.Uefi
                    : VmBootMode.Bios;

        public bool AssignVtpm => !IsVtpmTemplate && checkBoxVtpm.Checked;

        private bool IsVtpmTemplate => _templateVM != null && _templateVM.is_a_template &&
                                       _templateVM.platform.TryGetValue("vtpm", out var result) &&
                                       result.ToLower() == "true";

        public void CheckBIOSBootMode()
        {
            radioButtonBIOSBoot.Checked = true;
        }

        private void UpdateBiosWarning(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                imgBios.Visible = labelBios.Visible = false;
                return;
            }

            imgBios.Visible = labelBios.Visible = true;
            labelBios.Text = text;
        }

        private void UpdateUefiWarning(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                imgUefi.Visible = labelUefi.Visible = false;
                return;
            }

            imgUefi.Visible = labelUefi.Visible = true;
            labelUefi.Text = text;
        }

        private void UpdateSecureUefiWarning(string text, bool isInfo = true)
        {
            if (string.IsNullOrEmpty(text))
            {
                imgSecureUefi.Visible = labelSecureUefi.Visible = false;
                return;
            }

            imgSecureUefi.Visible = labelSecureUefi.Visible = true;
            labelSecureUefi.Text = text;
            imgSecureUefi.Image = isInfo ? Images.StaticImages._000_Info3_h32bit_16 : Images.StaticImages._000_Alert2_h32bit_16;
        }

        private void UpdateControls()
        {
            UpdateBiosWarning(null);
            UpdateUefiWarning(null);
            UpdateSecureUefiWarning(null);

            radioButtonUEFIBoot.Visible = !Helpers.FeatureForbidden(_connection, Host.UefiBootDisabled);
            radioButtonUEFISecureBoot.Visible = !Helpers.FeatureForbidden(_connection, Host.UefiSecureBootDisabled);

            if (_templateVM == null)
            {
                radioButtonBIOSBoot.Enabled = radioButtonUEFIBoot.Enabled = radioButtonUEFISecureBoot.Enabled = true;
                radioButtonBIOSBoot.Checked = true;
                return;
            }

            radioButtonBIOSBoot.Enabled = true;
            
            radioButtonUEFIBoot.Enabled = _templateVM.SupportsUefiBoot();
            if (!radioButtonUEFIBoot.Enabled)
                UpdateUefiWarning(Messages.BOOT_MODE_UNSUPPORTED_WARNING);

            radioButtonUEFISecureBoot.Enabled = _templateVM.SupportsSecureUefiBoot();
            if (!radioButtonUEFISecureBoot.Enabled)
                UpdateSecureUefiWarning(Messages.BOOT_MODE_UNSUPPORTED_WARNING);

            if (_templateVM.IsHVM() && _templateVM.IsDefaultBootModeUefi())
            {
                if (!_templateVM.CanChangeBootMode() || IsVtpmTemplate)
                {
                    radioButtonBIOSBoot.Enabled = false;
                    UpdateBiosWarning(Messages.BOOT_MODE_UNSUPPORTED_WARNING);
                }

                var secureBoot = _templateVM.GetSecureBootMode();

                if (secureBoot == "true" || secureBoot == "auto" && _poolHasCertificates)
                    radioButtonUEFISecureBoot.Checked = true;
                else
                    radioButtonUEFIBoot.Checked = true;

                if (radioButtonUEFISecureBoot.Enabled && radioButtonUEFISecureBoot.Checked && !_poolHasCertificates)
                    UpdateSecureUefiWarning(Messages.GUEFI_SECUREBOOT_MODE_MISSING_CERTIFICATES, false);
            }
            else
            {
                if (!_templateVM.CanChangeBootMode())
                    radioButtonUEFIBoot.Enabled = radioButtonUEFISecureBoot.Enabled = false;

                radioButtonBIOSBoot.Checked = true;
            }
        }

        private void UpdateTpmControls()
        {
            if (_templateVM != null)
            {
                var vtpmSupported = !Helpers.FeatureForbidden(_connection, Host.RestrictVtpm) &&
                                    Helpers.XapiEqualOrGreater_22_26_0(_connection) &&
                                    (radioButtonUEFIBoot.Visible || radioButtonUEFISecureBoot.Visible) &&
                                    (radioButtonUEFIBoot.Enabled || radioButtonUEFISecureBoot.Enabled);

                groupBoxDevSecurity.Visible = vtpmSupported;
                checkBoxVtpm.Enabled = vtpmSupported && !radioButtonBIOSBoot.Checked && !IsVtpmTemplate &&
                                       _templateVM.VTPMs.Count < VM.MAX_ALLOWED_VTPMS;
                checkBoxVtpm.Checked = vtpmSupported && !radioButtonBIOSBoot.Checked && IsVtpmTemplate &&
                                       _templateVM.VTPMs.Count < VM.MAX_ALLOWED_VTPMS;

                if (_templateVM.VTPMs.Count == VM.MAX_ALLOWED_VTPMS)
                    labelTpm.Text = _templateVM.VTPMs.Count == 1
                        ? string.Format(Messages.VTPM_MAX_REACHED_CUSTOM_TEMPLATE_ONE, VM.MAX_ALLOWED_VTPMS)
                        : string.Format(Messages.VTPM_MAX_REACHED_CUSTOM_TEMPLATE_MANY, VM.MAX_ALLOWED_VTPMS);
                else if (radioButtonBIOSBoot.Checked)
                    labelTpm.Text = Messages.COMMAND_VTPM_DISABLED_NON_UEFI;
            }
            else
            {
                groupBoxDevSecurity.Visible = false;
                checkBoxVtpm.Enabled = false;
                checkBoxVtpm.Checked = false;
            }

            labelTpm.Visible = imgTpm.Visible = groupBoxDevSecurity.Visible && !checkBoxVtpm.Enabled && !checkBoxVtpm.Checked;
        }

        public static bool ShowBootModeOptions(IXenConnection connection)
        {
            return Helpers.NaplesOrGreater(connection) && 
                   (!Helpers.FeatureForbidden(connection, Host.UefiBootDisabled) || !Helpers.FeatureForbidden(connection, Host.UefiSecureBootDisabled));
        }

        private void radioButtonBIOSBoot_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTpmControls();
        }

        private void radioButtonUEFISecureBoot_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonUEFISecureBoot.Checked && !_poolHasCertificates)
                UpdateSecureUefiWarning(Messages.GUEFI_SECUREBOOT_MODE_MISSING_CERTIFICATES, false);
            else
                UpdateSecureUefiWarning(null);
        }
    }
}
