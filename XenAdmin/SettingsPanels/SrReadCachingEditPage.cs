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
using XenAdmin.Actions;
using XenAdmin.Help;
using XenAPI;

namespace XenAdmin.SettingsPanels
{
    public partial class SrReadCachingEditPage : UserControl, IEditPage
    {
        private SR sr;
        public SrReadCachingEditPage()
        {
            InitializeComponent();
            Text = Messages.READ_CACHING;
        }

        public string SubText => checkBoxEnableReadCaching.Checked ? Messages.ENABLED : Messages.DISABLED;

        public Image Image => Images.StaticImages._000_Storage_h32bit_16;

        public AsyncAction SaveSettings()
        {
            sr.SetReadCachingEnabled(checkBoxEnableReadCaching.Checked);
            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            sr = clone as SR;
            if (sr == null)
                return;
            checkBoxEnableReadCaching.Checked = sr.GetReadCachingEnabled();
        }

        public bool ValidToSave => true;

        public void ShowLocalValidationMessages()
        { }

        public void HideLocalValidationMessages()
        { }

        public void Cleanup()
        { }

        public bool HasChanged => checkBoxEnableReadCaching.Checked != sr.GetReadCachingEnabled();

        private void linkLabelTellMeMore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpManager.Launch("StorageReadCaching");
        }
    }
}
