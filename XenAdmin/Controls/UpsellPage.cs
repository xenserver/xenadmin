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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.SettingsPanels;
using XenAPI;


namespace XenAdmin.Controls
{
    public partial class UpsellPage : UserControl, IEditPage
    {
        public UpsellPage()
        {
            InitializeComponent();
            LearnMoreButton.Visible = !HiddenFeatures.LearnMoreButtonHidden;
        }

        public void EnableOkButton()
        {
            OKButton.Visible = true;
        }

        public string BlurbText
        {
            set => Blurb.Text = HiddenFeatures.LinkLabelHidden
                ? value
                : value + string.Format(Messages.UPSELL_BLURB_TRIAL, BrandManager.ProductBrand);
        }

        private void LearnMoreButton_Clicked(object sender, EventArgs e)
        {
            NavigateTo(InvisibleMessages.UPSELL_LEARNMOREURL_TRIAL);
        }

        private void NavigateTo(string url)
        {
            if (url == null)
                return;
            Program.OpenURL(url);
        }

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
        }

        public bool ValidToSave => true;

        public void ShowLocalValidationMessages()
        {
        }

        public void HideLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
        }

        public bool HasChanged => false;


        #region IVerticalTab Members

        public string SubText => Messages.XENSERVER_UPGRADE_REQUIRED;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image Image { get; set; } = Images.StaticImages.Logo; //serving as default value; never really shown

        #endregion

        #endregion

        private void OKButton_Click(object sender, EventArgs e)
        {
            ParentForm?.Close();
        }
    }
}
