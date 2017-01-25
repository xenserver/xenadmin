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
        public string LinkText { set { helperLink.Text = value; } }
        public Uri LinkUri { set; private get; }
        public bool HelperLinkVisible { set { helperLink.Visible = value; } }
        public Color BackgroundColour { set; private get; }
        private readonly Color defaultBackgroundColour = Color.LemonChiffon;

        public DeprecationBanner()
        {
            InitializeComponent();
            HelperLinkVisible = !XenAdmin.Core.HiddenFeatures.LinkLabelHidden;
            Visible = false;
            helperLink.Click += helperLink_Click;
            BackgroundColour = defaultBackgroundColour;
        }

        private void helperLink_Click(object sender, EventArgs e)
        {
            if(LinkUri == null)
                return;

            try
            {
                Process.Start(LinkUri.AbsoluteUri);
            }
            catch (Exception)
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(
                        SystemIcons.Error,
                        string.Format(Messages.COULD_NOT_OPEN_URL,
                                      LinkUri.AbsoluteUri),
                        Messages.XENCENTER)))
                {
                    dlg.ShowDialog(Program.MainWindow);
                }
            }
        }

        public new bool Visible
        {
            set
            {
                BackColor = BackgroundColour;
                SetMessageText();
                base.Visible = value;
            }
        }

        private void SetMessageText()
        {
            if (BannerType == Type.Deprecation)
            {
                message.Text = String.Format(Messages.X_IS_DEPRECATED_IN_X, FeatureName, AppliesToVersion);
                return;
            }
                
            
            if (BannerType == Type.Removal)
            {
                message.Text = String.Format(Messages.X_IS_REMOVED_IN_X, FeatureName, AppliesToVersion);
                return;
            } 
            
            message.Text = String.Empty;
        }

    }
}
