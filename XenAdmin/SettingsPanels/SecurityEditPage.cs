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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.SettingsPanels
{
    public partial class SecurityEditPage : UserControl, IEditPage
    {
        private Pool pool;

        public SecurityEditPage()
        {
            InitializeComponent();
        }

        private void ShowHideWarning()
        {
            labelDisruption.Visible = pictureBoxDisruption.Visible = HasChanged;
        }

        private void radioButtonTLS_CheckedChanged(object sender, EventArgs e)
        {
            ShowHideWarning();
        }

        public override string Text => Messages.SECURITY;

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            return new SetSslLegacyAction(pool, radioButtonSSL.Checked);
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            // clone could be a pool or a host
            if (clone is Host)
            {
                labelRubricPool.Text = Messages.SECURITYEDITPAGE_RUBRIC_HOST;
                labelDisruption.Text = Messages.SECURITYEDITPAGE_WARNING_HOST;
            }

            pool = Helpers.GetPoolOfOne(clone.Connection);

            if (pool.ssl_legacy())
                radioButtonSSL.Checked = true;
            else
                radioButtonTLS.Checked = true;
            ShowHideWarning();
        }

        public bool ValidToSave => true;

        public void ShowLocalValidationMessages()
        { }

        public void HideLocalValidationMessages()
        { }

        public void Cleanup()
        { }

        public bool HasChanged => radioButtonSSL.Checked != pool.ssl_legacy();

        #endregion

        #region IVerticalTab Members

        public string SubText =>
            radioButtonTLS.Checked ? Messages.SECURITYEDITPAGE_SUBTEXT_TLS : Messages.SECURITYEDITPAGE_SUBTEXT_SSL;

        public Image Image => Images.StaticImages.padlock;

        #endregion
    }
}
