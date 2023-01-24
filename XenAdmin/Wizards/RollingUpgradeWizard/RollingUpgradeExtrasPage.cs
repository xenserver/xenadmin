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
using XenAdmin.Controls;
using XenAdmin.Core;


namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeExtrasPage : XenTabPage
    {
        public string SelectedSuppPack;

        public RollingUpgradeExtrasPage()
        {
            InitializeComponent();
            label4.Text = string.Format(label4.Text, BrandManager.BrandConsole, BrandManager.ProductBrand);
        }

        #region XenTabPage overrides
        public override string Text => Messages.ROLLING_UPGRADE_EXTRAS_PAGE_TEXT;

        public override string HelpID => "Upgradeextras";

        public override string PageTitle => Messages.ROLLING_UPGRADE_EXTRAS_PAGE_TITLE;

        public override string NextText(bool isLastPage)
        {
            return Messages.RUN_PRECHECKS_WITH_ACCESS_KEY;
        }

        public override bool EnableNext()
        {
            if (ApplySuppPackAfterUpgrade && !WizardHelpers.IsValidFile(FilePath, out _))
                return false;

            return true;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward && ApplySuppPackAfterUpgrade && !string.IsNullOrEmpty(FilePath))
            {
                SelectedSuppPack = WizardHelpers.ParseSuppPackFile(FilePath, this, ref cancel);
            }
        }
        #endregion

        private string FilePath
        {
            get => fileNameTextBox.Text;
            set => fileNameTextBox.Text = value;
        }

        public bool ApplySuppPackAfterUpgrade => checkBoxInstallSuppPack.Checked;

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            checkBoxInstallSuppPack.Checked = true;
            var suppPack = WizardHelpers.GetSuppPackFromDisk(this);
            if (!string.IsNullOrEmpty(suppPack))
                FilePath = suppPack;
            OnPageUpdated();
        }

        private void fileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            checkBoxInstallSuppPack.Checked = true;
            OnPageUpdated();
        }

        private void fileNameTextBox_Enter(object sender, EventArgs e)
        {
            checkBoxInstallSuppPack.Checked = true;
            OnPageUpdated();
        }

        private void checkBoxInstallSuppPack_CheckedChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }
    }
}
