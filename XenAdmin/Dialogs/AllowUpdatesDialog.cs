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
using System.Windows.Forms;
using XenAdmin.Plugins;

namespace XenAdmin.Dialogs
{
    public partial class AllowUpdatesDialog : XenDialogBase
    {
        private readonly PluginManager m_pluginManager;

        public AllowUpdatesDialog(PluginManager pluginManager)
        {
            InitializeComponent();
            m_pluginManager = pluginManager;
            ControlBox = true;
        }

        private void SetAllowUpdates(bool value)
        {
            Properties.Settings.Default.AllowXenCenterUpdates = value;
            Properties.Settings.Default.AllowPatchesUpdates = value;
        }

        private void YesButtonClicked()
        {
            SetAllowUpdates(true);
            Properties.Settings.Default.SeenAllowUpdatesDialog = true;
            if (checkBox1.Checked)
            {
                using (var dialog = new OptionsDialog(m_pluginManager))
                {
                    dialog.SelectConnectionOptionsPage();
                    dialog.ShowDialog(this);
                }
            }
            Settings.TrySaveSettings();
        }

        private void NoButtonClicked()
        {
            SetAllowUpdates(false);
            Properties.Settings.Default.SeenAllowUpdatesDialog = true;
            Settings.TrySaveSettings();
        }

        private void AllowUpdatesDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (DialogResult)
            {
                case DialogResult.OK:
                    YesButtonClicked();
                    break;
                case DialogResult.Cancel:
                    NoButtonClicked();
                    break;
            }
        }
    }
}