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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using XenAdmin.Core;


namespace XenAdmin.Dialogs
{
    public partial class LoadSessionDialog : XenDialogBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LoadSessionDialog() : this(false)
        {
        }

        public LoadSessionDialog(bool isRetry)
        {
            InitializeComponent();
            Icon = Properties.Resources.AppIcon;

            passwordFailure1.Visible = isRetry;
        }

        public byte[] PasswordHash
        {
            get
            {
                byte[] result = null;
                if (this.passBox.Text != null && passBox.Text.Length > 0)
                {
                    try
                    {
                        result = EncryptionUtils.ComputeHash(passBox.Text);
                    }
                    catch (Exception exp)
                    {
                        log.Error("Could not hash entered password.", exp);
                        result = null;
                    }
                }

                return result;
            }
        }

        private void LoadSessionDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Help.HelpManager.Launch("LoadSessionDialog");
            hlpevent.Handled = true;
        }

        private void LoadSessionDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Help.HelpManager.Launch("LoadSessionDialog");
            e.Cancel = true; 
        }

        private void passBox_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = !string.IsNullOrEmpty(passBox.Text);
        }

        private void LoadSessionDialog_Load(object sender, EventArgs e)
        {
            if (Location.IsEmpty)
            {
                Screen s = Screen.FromControl(this);
                Location = new Point((s.Bounds.Width - Width) / 2, (s.Bounds.Height - Height) / 2);
            }
        }
    }
}