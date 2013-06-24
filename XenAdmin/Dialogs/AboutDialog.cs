/* Copyright (c) Citrix Systems Inc. 
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
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;


namespace XenAdmin.Dialogs
{
    public partial class AboutDialog : XenDialogBase
    {
        private static LegalNoticesDialog TheLegalDialog = null;

        public AboutDialog()
        {
            InitializeComponent();

            string buildText = Helpers.CommonCriteriaCertificationRelease
                                   ? string.Format("{0}: {1}", Program.Version.Revision, Messages.COMMON_CRITERIA_TEXT)
                                   : Program.Version.Revision.ToString();

            VersionLabel.Text = string.Format(Messages.VERSION_NUMBER, Branding.PRODUCT_VERSION_TEXT, buildText);
            label2.Text = string.Format(Messages.COPYRIGHT, Branding.COMPANY_NAME_LEGAL);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Close();
            TheLegalDialog = null;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (TheLegalDialog == null || TheLegalDialog.IsDisposed)
            {
                TheLegalDialog = new LegalNoticesDialog();
                TheLegalDialog.Show(this);
            }
            else
            {
                TheLegalDialog.BringToFront();
                TheLegalDialog.Focus();
            }
        }
    }
}
