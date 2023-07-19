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
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs.OptionsPages;
using XenAdmin.Plugins;

namespace XenAdmin.Dialogs
{
    public partial class OptionsDialog : VerticallyTabbedDialog
    {
        internal OptionsDialog(PluginManager pluginManager)
        {
            InitializeComponent();
            pluginOptionsPage1.SetPluginManager(pluginManager, false);//do not request rebuild; it will be done on load
            verticalTabs.SelectedItem = securityOptionsPage1;

            if (!Application.RenderWithVisualStyles)
                ContentPanel.BackColor = SystemColors.Control;
            // call save serverlist on OK
            saveAndRestoreOptionsPage1.SaveAllAfter = true;

            if (Helpers.CommonCriteriaCertificationRelease)
                verticalTabs.Items.Remove(updatesOptionsPage1);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (IOptionsPage page in verticalTabs.Items)
                page.Build();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            foreach (IOptionsPage page in verticalTabs.Items)
            {
                if (!page.IsValidToSave(out Control control, out string invalidReason))
                {
                    SelectPage(page);
                    page.ShowValidationMessages(control, invalidReason);
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            foreach (IOptionsPage page in verticalTabs.Items)
                page.Save();

            Settings.TrySaveSettings();
            Settings.Log();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void SelectConnectionOptionsPage()
        {
            SelectPage(connectionOptionsPage1);
        }

        public void SelectExternalToolsPage()
        {
            SelectPage(externalToolsOptionsPage1);
        }

        public void SelectUpdateOptionsPage()
        {
            SelectPage(updatesOptionsPage1);
        }

        private void OptionsDialog_Move(object sender, EventArgs e)
        {
            HideValidationToolTips();
        }

        private void verticalTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideValidationToolTips();
        }

        private void HideValidationToolTips()
        {
            foreach (IOptionsPage page in verticalTabs.Items)
            {
                page.HideValidationMessages();
            }
        }
    }
}

