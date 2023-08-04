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
using XenAdmin.Dialogs.OptionsPages;


namespace XenAdmin.Dialogs.ServerUpdates
{
    public partial class ConfigUpdatesDialog : XenDialogBase
    {
        private readonly OptionsTabPage _configYumRepoTab = new OptionsTabPage(new ConfigCdnUpdatesPage());
        private readonly OptionsTabPage _configLcmTab = new OptionsTabPage(new ConfigLcmUpdatesPage());

        public ConfigUpdatesDialog()
        {
            InitializeComponent();

            components = new Container();
            var imageList = new ImageList(components)
            {
                ColorDepth = ColorDepth.Depth32Bit,
                TransparentColor = Color.Transparent
            };
            imageList.Images.Add(Images.StaticImages.notif_updates_16);
            imageList.Images.Add(Images.StaticImages._000_Patch_h32bit_16);
            
            tabControl1.ImageList = imageList;
            
            tabControl1.TabPages.Add(_configYumRepoTab);
            tabControl1.TabPages.Add(_configLcmTab);

            _configYumRepoTab.ImageIndex = 0;
            _configLcmTab.ImageIndex = 1;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            foreach (var tab in tabControl1.TabPages)
            {
                if (tab is OptionsTabPage otp)
                    otp.ContentPage.Build();
            }
        }

        private void ConfigUpdatesDialog_Move(object sender, EventArgs e)
        {
            foreach (var tab in tabControl1.TabPages)
            {
                if (tab is OptionsTabPage otp)
                    otp.ContentPage.HideValidationMessages();
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            //the double iteration through the pages is required because
            //we don't want to save anything unless all pages are valid

            foreach (var tab in tabControl1.TabPages)
            {
                if (tab is OptionsTabPage otp && !otp.ContentPage.IsValidToSave(out var control, out var invalidReason))
                {
                    otp.ContentPage.ShowValidationMessages(control, invalidReason);
                    return;
                }
            }

            foreach (var tab in tabControl1.TabPages)
            {
                if (tab is OptionsTabPage otp)
                    otp.ContentPage.Save();
            }

            Close();
        }

        public void SelectLcmTab()
        {
            if (tabControl1.TabPages.Contains(_configLcmTab))
                tabControl1.SelectTab(_configLcmTab);
        }


        private sealed class OptionsTabPage : TabPage
        {
            public OptionsTabPage(IOptionsPage contentPage)
            {
                ContentPage = contentPage;
                Text = contentPage.Text;
                UseVisualStyleBackColor = true;
                Padding = new Padding(0, 8, 0, 0);

                if (contentPage is UserControl control)
                {
                    Controls.Add(control);
                    control.Dock = DockStyle.Fill;
                }
            }

            public override string Text { get; set; }

            public IOptionsPage ContentPage { get; }

            protected override void Dispose(bool disposing)
            {
                if (ContentPage is UserControl control)
                    control.Dispose();

                base.Dispose(disposing);
            }
        }
    }
}
