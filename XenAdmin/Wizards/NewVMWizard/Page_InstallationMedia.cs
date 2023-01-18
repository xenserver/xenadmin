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
using XenAdmin.Actions.VMActions;
using XenAPI;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenCenterLib;
using System.Windows.Forms;
using System.Drawing;

namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_InstallationMedia : XenTabPage
    {
        private VM m_template;

        public Page_InstallationMedia()
        {
            InitializeComponent();
            CdDropDownBox.SrsRefreshed += CdDropDownBox_SrsRefreshed;
        }

        
        #region Accessors

        public Host Affinity { private get; set; }

        public bool AssignVtpm => bootModesControl1.AssignVtpm;

        public VmBootMode SelectedBootMode => bootModesControl1.SelectedBootMode;

        public InstallMethod SelectedInstallMethod
        {
            get
            {
                if (CdRadioButton.Enabled && CdRadioButton.Checked)
                    return InstallMethod.CD;

                if (UrlRadioButton.Enabled && UrlRadioButton.Checked)
                    return InstallMethod.Network;

                return InstallMethod.None;
            }
        }

        public string SelectedPvArgs => SelectedTemplate.IsHVM() ? string.Empty : PvBootTextBox.Text;

        public VDI SelectedCD => CdRadioButton.Enabled && CdRadioButton.Checked
            ? CdDropDownBox.SelectedCD
            : null;

        public string SelectedUrl => UrlRadioButton.Enabled && UrlRadioButton.Checked && UrlTextBox.Visible
            ? UrlTextBox.Text
            : string.Empty;

        public VM SelectedTemplate { private get; set; }

        #endregion


        private void AnalyseTemplate(out bool isUserTemplate, out bool hvm, out bool eli,
            out string installMethods, out bool installCd, out bool installUrl)
        {
            isUserTemplate = !m_template.DefaultTemplate();
            hvm = m_template.IsHVM();
            eli = !hvm && m_template.PV_bootloader == "eliloader";
            installMethods = m_template.InstallMethods();
            installCd = installMethods != null && installMethods.Contains("cdrom");
            installUrl = installMethods != null &&
                         (installMethods.Contains("http") || installMethods.Contains("ftp") || installMethods.Contains("nfs"));
        }

        private void EnableCdRadioButton(bool isUserTemplate, bool hvm, bool eli, string installMethods, bool installCd)
        {
            if (isUserTemplate && m_template.GetBootOrder().StartsWith("N"))
                CdRadioButton.Enabled = false;
            else
                CdRadioButton.Enabled = isUserTemplate || string.IsNullOrEmpty(installMethods) || installCd && (hvm || eli);
        }

        private VDI GetAffinityDvdDrive()
        {
            if (Affinity == null || Affinity.Connection == null)
                return null;

            List<SR> dvdSRs = Affinity.Connection.Cache.SRs.Where(sr => sr.content_type == SR.Content_Type_ISO && sr.Physical() && sr.GetStorageHost() == Affinity).ToList();

            if (dvdSRs.Count > 0 && dvdSRs[0].VDIs.Count > 0)
                return Affinity.Connection.Resolve(dvdSRs[0].VDIs[0]);

            return null;
        }


        #region XenTabPage overrides
        
        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (m_template == SelectedTemplate)
                return;

            /* This method is supposed to do the following:
             * 
             * PV      -> Install from DVD drive (selecting <empty> disables Next)
             *         -> Url
             * 
             * HVM     -> Install from DVD drive (selecting <empty> disables Next)
             *         -> Boot from network
             * 
             * Custom with DVD drive    -> DVD drive (selecting <empty> does not disable Next)
             *
             * Custom without DVD drive -> Disable Installation method section
             */

            m_template = SelectedTemplate;
            AnalyseTemplate(out bool isUserTemplate, out bool hvm, out bool eli,
                out string installMethods, out bool installCd, out bool installUrl);

            VBD cdDrive = m_template.FindVMCDROM();

            PvBootBox.Visible = !hvm;
            PvBootTextBox.Text = hvm ? string.Empty : m_template.PV_args;

            bootModesControl1.Visible = hvm && BootModesControl.ShowBootModeOptions(SelectedTemplate.Connection);
            bootModesControl1.TemplateVM = m_template;

            if (isUserTemplate && cdDrive == null ||
                !isUserTemplate && string.IsNullOrEmpty(m_template.InstallMethods()))
            {
                CdRadioButton.Checked = UrlRadioButton.Checked = false;
                CdDropDownBox.Items.Clear();
                UrlTextBox.Text = string.Empty;
                panelInstallationMethod.Enabled = false;
                OnPageUpdated();
                return;
            }

            panelInstallationMethod.Enabled = true;

            CdRadioButton.Text = isUserTemplate || string.IsNullOrEmpty(installMethods)
                ? Messages.NEWVMWIZARD_INSTALLMEDIA_DVD
                : Messages.NEWVMWIZARD_INSTALLMEDIA_INSTALLDVD;

            EnableCdRadioButton(isUserTemplate, hvm, eli, installMethods, installCd);
            CdDropDownBox.Enabled = CdRadioButton.Enabled;

            UrlRadioButton.Text = hvm ? Messages.NEWVMWIZARD_INSTALLMEDIA_INSTALLPXE : Messages.NEWVMWIZARD_INSTALLMEDIA_INSTALLURL;

            UrlRadioButton.Enabled = isUserTemplate && m_template.GetBootOrder().StartsWith("N") ||
                                     !isUserTemplate && !string.IsNullOrEmpty(installMethods) && (hvm || eli && installUrl);

            UrlTextBox.Enabled = UrlRadioButton.Enabled;
            UrlTextBox.Visible = !hvm;
            UrlTextBox.Text = isUserTemplate || string.IsNullOrEmpty(installMethods) ? "" : m_template.InstallRepository() ?? "";

            if (CdRadioButton.Enabled)
            {
                CdRadioButton.Checked = true;
                CdDropDownBox.Select();
            }
            else if (UrlRadioButton.Enabled)
            {
                UrlRadioButton.Checked = true;
                UrlTextBox.Select();
            }

            CdDropDownBox.VM = m_template;

            VDI cd = null;

            if (cdDrive != null && !cdDrive.empty)
                cd = Connection.Resolve(cdDrive.VDI);
            else if (!isUserTemplate && !string.IsNullOrEmpty(installMethods))
                cd = GetAffinityDvdDrive();

            CdDropDownBox.SelectedCD = cd != null && Connection.Resolve(cd.SR) != null ? cd : null;

            OnPageUpdated();
        }

        public override bool EnableNext()
        {
            if (!panelInstallationMethod.Enabled)
                return true;

            if (CdRadioButton.Enabled && CdRadioButton.Checked)
                return !m_template.DefaultTemplate() ||
                       string.IsNullOrEmpty(m_template.InstallMethods()) ||
                       CdDropDownBox.SelectedCD != null;

            if (UrlRadioButton.Enabled && UrlRadioButton.Checked)
                return !UrlTextBox.Visible || !string.IsNullOrEmpty(UrlTextBox.Text);

            return false;
        }

        public override string Text => Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_NAME;

        public override string PageTitle => Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_TITLE;

        public override string HelpID => "InstallationMedia";

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                List<KeyValuePair<string, string>> sum = new List<KeyValuePair<string, string>>();
                switch (SelectedInstallMethod)
                {
                    case InstallMethod.CD:
                        sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_INSTALLMETHOD, Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_CD));
                        sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CDMEDIAPAGE_INSTALLATIONSOURCE, SelectedCD != null ? SelectedCD.Name() : Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_NONE));
                        break;
                    case InstallMethod.Network:
                        sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_INSTALLMETHOD, Messages.NEWVMWIZARD_INSTALLATIONMEDIAPAGE_NETWORK));
                        sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_NETWORKMEDIAPAGE_INSTALLATIONURL, SelectedUrl));
                        break;
                }

                if (Helpers.NaplesOrGreater(SelectedTemplate.Connection) && SelectedTemplate.IsHVM())
                    sum.Add(new KeyValuePair<string, string>(Messages.BOOT_MODE, SelectedBootMode.StringOf()));
                return sum;
            }
        }

        #endregion


        #region Event Handlers

        private void CdRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (CdRadioButton.Checked)
                OnPageUpdated();
        }

        private void UrlRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (UrlRadioButton.Checked)
                OnPageUpdated();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CdRadioButton.Enabled = CdDropDownBox.Enabled = false; //disable until the SRs are refreshed

            using (var wizard = new NewSRWizard(Connection))
            {
                wizard.CheckNFSISORadioButton();

                if (wizard.ShowDialog() == DialogResult.Cancel)
                    CdDropDownBox_SrsRefreshed();
            }
        }

        private void CdDropDownBox_SrsRefreshed()
        {
            AnalyseTemplate(out bool isUserTemplate, out bool hvm, out bool eli,
                out string installMethods, out bool installCd, out bool _);

            EnableCdRadioButton(isUserTemplate, hvm, eli, installMethods, installCd);
            CdDropDownBox.Enabled = CdRadioButton.Enabled;
            OnPageUpdated();
        }

        private void CdDropDownBox_DropDownClosed(object sender, EventArgs e)
        {
            comboBoxToolTip.Hide(CdDropDownBox);
        }

        private void CdDropDownBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index > CdDropDownBox.Items.Count - 1)
            {
                comboBoxToolTip.Hide(CdDropDownBox);
                return;
            }

            string selectedText = CdDropDownBox.GetItemText(CdDropDownBox.Items[e.Index]);

            Font font = CdDropDownBox.Items[e.Index] is ToStringWrapper<SR> ? Program.DefaultFontBold : Program.DefaultFont;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected &&
                CdDropDownBox.DroppedDown &&
                TextRenderer.MeasureText(selectedText, font).Width > CdDropDownBox.DropDownWidth)
            {
                comboBoxToolTip.Show(selectedText, CdDropDownBox, e.Bounds.Right, e.Bounds.Bottom);
            }
            else
            {
                comboBoxToolTip.Hide(CdDropDownBox);
            }
        }

        private void CdDropDownBox_Enter(object sender, EventArgs e)
        {
            CdRadioButton.Checked = true;
        }

        private void CdDropDownBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        private void UrlTextBox_Enter(object sender, EventArgs e)
        {
            UrlRadioButton.Checked = true;
        }

        private void UrlTextBox_TextChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        #endregion
    }
}
