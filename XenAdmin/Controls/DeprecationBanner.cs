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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Dialogs;

namespace XenAdmin.Controls
{
    public partial class DeprecationBanner : UserControl
    {
        public enum Type
        {
            Deprecation,
            Removal
        }

        public Type BannerType { private get; set; }
        public string FeatureName { private get; set; }
        public string AppliesToVersion { private get; set; }
        public string WarningMessage { private get; set; }
        public string LinkText { set => helperLink.Text = value; }
        public Uri LinkUri { set; private get; }
        public bool HelperLinkVisible { set => helperLink.Visible = value; }

        public DeprecationBanner()
        {
            InitializeComponent();
            HelperLinkVisible = !Core.HiddenFeatures.LinkLabelHidden;
            LinkUri = new Uri(InvisibleMessages.DEPRECATION_URL);
            Visible = false;
        }

        private void helperLink_Click(object sender, EventArgs e)
        {
            if (LinkUri == null)
                return;

            try
            {
                Process.Start(LinkUri.AbsoluteUri);
            }
            catch (Exception)
            {
                using (var dlg = new ErrorDialog(string.Format(Messages.COULD_NOT_OPEN_URL, LinkUri.AbsoluteUri)))
                    dlg.ShowDialog(Program.MainWindow);
            }
        }

        public new bool Visible
        {
            set
            {
                if (value)
                {
                    SetMessageText();
                    BackColor = BannerType == Type.Removal ? Color.LightCoral : Color.LemonChiffon;
                }

                base.Visible = value;
            }
        }

        private void SetMessageText()
        {
            if (!string.IsNullOrEmpty(WarningMessage))
                message.Text = WarningMessage;
            else
                switch (BannerType)
                {
                    case Type.Deprecation:
                        message.Text = string.Format(Messages.X_IS_DEPRECATED_IN_X, FeatureName, AppliesToVersion);
                        break;
                    case Type.Removal:
                        message.Text = string.Format(Messages.X_IS_REMOVED_IN_X, FeatureName, AppliesToVersion);
                        break;
                    default:
                        message.Text = string.Empty;
                        break;
                }
        }
    }
}
