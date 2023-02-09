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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Wizards.NewSRWizard_Pages;
using XenAPI;

namespace XenAdmin.Dialogs.WarningDialogs
{
    public partial class LVMoHBAWarningDialog : XenDialogBase
    {
        private Panel highlightedPanel;
        private string deviceDetails;
        private int remainingDevicesCount;
        private bool foundExistingSR;
        private readonly SR.SRTypes existingSrType;
        private readonly SR.SRTypes requestedSrType;
        private readonly IXenConnection _connection;

        public UserSelectedOption SelectedOption { get; private set; }
        public bool RepeatForRemainingLUNs => checkBoxRepeat.Checked;

        public LVMoHBAWarningDialog(IXenConnection connection, string deviceDetails, int remainingDevicesCount,
            bool foundExistingSR, SR.SRTypes existingSrType, SR.SRTypes requestedSrType)
        {
            InitializeComponent();
            labelWarning.Text = string.Format(labelWarning.Text, BrandManager.ProductBrand, BrandManager.BrandConsole);
            _connection = connection;
            this.deviceDetails = deviceDetails;
            this.remainingDevicesCount = remainingDevicesCount;
            this.foundExistingSR = foundExistingSR;
            this.existingSrType = existingSrType;
            this.requestedSrType = requestedSrType;
            PopulateControls();
            ActiveControl = buttonCancel;
        }

        private Panel HighlightedPanel
        {
            set
            {
                if (highlightedPanel == value)
                    return;

                SetPanelColor(highlightedPanel, false);
                SetPanelColor(value, true);
                highlightedPanel = value;
            }
        }

        private void SetPanelColor(Panel panel, bool highlighted)
        {
            if (panel == null)
                return;

            Color color = highlighted ? SystemColors.ControlLight : SystemColors.Control;
            panel.BackColor = color;

            foreach (var control in panel.Controls)
            {
                if (control is Button button)
                {
                    button.FlatAppearance.MouseOverBackColor = color;
                    button.FlatAppearance.MouseDownBackColor = color;
                }
            }
        }

        private void panelReattach_MouseEnter(object sender, EventArgs e)
        {
            HighlightedPanel = panelReattach;
        }

        private void panelFormat_MouseEnter(object sender, EventArgs e)
        {
            HighlightedPanel = panelFormat;
        }

        private void LVMoHBAWarningDialog_MouseEnter(object sender, EventArgs e)
        {
            HighlightedPanel = null;
        }

        private void PopulateControls()
        {
            labelHeader.Text = foundExistingSR
                ? string.Format(Messages.LVMOHBA_WARNING_DIALOG_HEADER_FOUND_EXISTING_SR, SR.GetFriendlyTypeName(existingSrType))
                : Messages.LVMOHBA_WARNING_DIALOG_HEADER_NO_EXISTING_SRS;

            checkBoxRepeat.Text = foundExistingSR
                ? Messages.LVMOHBA_WARNING_DIALOG_REPEAT_FOR_REMAINING_WITH_SR
                : Messages.LVMOHBA_WARNING_DIALOG_REPEAT_FOR_REMAINING_NO_SR;
            checkBoxRepeat.Visible = remainingDevicesCount > 0;

            labelLUNDetails.Text = deviceDetails;
            labelSrDetails.Visible = false;

            var isGfs2 = existingSrType == SR.SRTypes.gfs2;
            var clusteringEnabled = _connection.Cache.Clusters.Any();
            var restrictGfs2 = Helpers.FeatureForbidden(_connection, Host.RestrictCorosync);

            if (foundExistingSR)
            {
                if (isGfs2 && restrictGfs2)
                    labelReattachInfo.Text = Messages.GFS2_INCORRECT_POOL_LICENSE;
                else if (isGfs2 && !clusteringEnabled)
                    labelReattachInfo.Text = Messages.GFS2_REQUIRES_CLUSTERING_ENABLED;
                else
                    labelReattachInfo.Text = string.Format(Messages.LVMOHBA_WARNING_DIALOG_REATTACH_INFO,
                        SR.GetFriendlyTypeName(existingSrType));
            }
            else
            {
                labelReattachInfo.Text = Messages.LVMOHBA_WARNING_DIALOG_REATTACH_LABEL_TEXT;
            }

            labelFormatInfo.Text = string.Format(Messages.LVMOHBA_WARNING_DIALOG_FORMAT_INFO,
                SR.GetFriendlyTypeName(requestedSrType));

            panelReattach.Enabled = foundExistingSR && (!isGfs2 || clusteringEnabled && !restrictGfs2);
            if (!panelReattach.Enabled)
                pictureBoxArrowReattach.Image = Drawing.ConvertToGreyScale(pictureBoxArrowReattach.Image);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            SelectedOption = UserSelectedOption.Cancel;
        }

        private void panelReattach_Click(object sender, EventArgs e)
        {
            HighlightedPanel = panelReattach;
            SelectedOption = UserSelectedOption.Reattach;
            DialogResult = DialogResult.OK;
        }

        private void panelFormat_Click(object sender, EventArgs e)
        {
            HighlightedPanel = panelFormat;
            SelectedOption = UserSelectedOption.Format;
            DialogResult = DialogResult.OK;
        }


        public enum UserSelectedOption
        {
            Cancel,
            Reattach,
            Format,
            Skip
        }
    }

    public class LVMoIsciWarningDialog : LVMoHBAWarningDialog
    {
        public LVMoIsciWarningDialog(IXenConnection connection, SR.SRInfo srInfo, SR.SRTypes existingSrType, SR.SRTypes requestedSrType)
            : base(connection, "", 0, srInfo != null, existingSrType, requestedSrType)
        {
            labelLUNDetails.Visible = false;
            labelSrDetails.Visible = srInfo != null;

            // CA-17230: if the found SR is used by other connected pools, offer only to attach it

            if (srInfo != null)
            {
                SR sr = SrWizardHelpers.SrInUse(srInfo.UUID);
                if (sr != null)
                {
                    panelFormat.Visible = false;
                    labelWarning.Text = GetSrInUseMessage(sr);
                }

                labelSrDetails.Text = string.Format(Messages.ISCSI_DIALOG_SR_DETAILS, Util.DiskSizeString(srInfo.Size), srInfo.UUID);
            }
        }

        public static string GetSrInUseMessage(SR sr)
        {
            Pool pool = Helpers.GetPool(sr.Connection);
            
            if (pool != null)
                return string.Format(Messages.NEWSR_LUN_IN_USE_ON_POOL, sr.Name(), pool.Name());
            
            Host coordinator = Helpers.GetCoordinator(sr.Connection);
            
            if (coordinator != null)
                return string.Format(Messages.NEWSR_LUN_IN_USE_ON_SERVER, sr.Name(), coordinator.Name());

            return Messages.NEWSR_LUN_IN_USE;
        }
    }
}
