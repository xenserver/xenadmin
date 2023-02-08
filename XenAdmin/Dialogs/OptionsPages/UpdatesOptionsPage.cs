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

using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Core;


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class UpdatesOptionsPage : UserControl, IOptionsPage
    {
        public UpdatesOptionsPage()
        {
            InitializeComponent();
            UpdatesBlurb.Text = string.Format(UpdatesBlurb.Text, BrandManager.BrandConsole);
            AllowXenCenterUpdatesCheckBox.Text = string.Format(AllowXenCenterUpdatesCheckBox.Text, BrandManager.BrandConsole);
        }

        #region IOptionsPage Members

        public void Build()
        {
            // XenCenter updates
            AllowXenCenterUpdatesCheckBox.Checked = Properties.Settings.Default.AllowXenCenterUpdates;
        }

        public bool IsValidToSave()
        {
            return true;
        }

        public void ShowValidationMessages()
        {
        }
        public void HideValidationMessages()
        {
        }

        public void Save()
        {
            bool checkXenCenterUpdatesChanged = AllowXenCenterUpdatesCheckBox.Checked != Properties.Settings.Default.AllowXenCenterUpdates;

            if (checkXenCenterUpdatesChanged)
            {
                Properties.Settings.Default.AllowXenCenterUpdates = AllowXenCenterUpdatesCheckBox.Checked;

                if (Properties.Settings.Default.AllowXenCenterUpdates)
                    Updates.CheckForUpdates(true);
            }
        }

        #endregion

        #region IVerticalTab Members

        public override string Text => Messages.UPDATES;

        public string SubText => string.Format(Messages.UPDATES_DESC, BrandManager.ProductBrand, BrandManager.BrandConsole);

        public Image Image => Images.StaticImages._000_Patch_h32bit_16;

        #endregion
    }
}
