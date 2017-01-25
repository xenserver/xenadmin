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
using XenAdmin.Controls;
using XenAPI;

namespace XenAdmin.Wizards.BallooningWizard_Pages
{
    public partial class ChooseVMs : XenTabPage
    {
        public ChooseVMs()
        {
            InitializeComponent();
            CheckedVMs = new List<VM>();
        }

        public override string Text { get { return Messages.BALLOONING_PAGE_CHOOSEVMS_TEXT; } }

        public override string PageTitle { get { return Messages.BALLOONING_PAGE_CHOOSEVMS_PAGETITLE; } }
        
        public override string HelpID
        {
            get { return "VMs"; }
        }

        public List<VM> VMs
        {
            set
            {
                // Fill the list box with the given items, and check them all
                listBox.Items.Clear();
                listBox.Items.AddRange(value.ToArray());
                CheckAll(true);
            }
        }

        public List<VM> CheckedVMs { get; private set; }

        public override bool EnableNext()
        {
            clearAllButton.Enabled = CheckedVMs.Count > 0;
            selectAllButton.Enabled = CheckedVMs.Count < listBox.Items.Count;
            return CheckedVMs.Count >= 1;
        }

        private void CheckAll(bool check)
        {
            for (int i = 0; i < listBox.Items.Count; ++i)
                listBox.SetItemChecked(i, check);
        }

        private void listBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // The ItemCheck event is called before the check changes,
            // and there is no event afterwards. Suggestions on the web for workarounds
            // (SelectedIndexChanged and MouseUp) don't work with keyboard access. So we
            // will calculate the new list ourselves.
           
            VM changedVm = (VM)listBox.Items[e.Index];
            
            CheckState newValue = e.NewValue;
            
            if (newValue == CheckState.Unchecked)
                CheckedVMs.Remove(changedVm);
            else
                CheckedVMs.Add(changedVm);

            OnPageUpdated();
        }

        private void selectAllButton_Click(object sender, EventArgs e)
        {
            CheckAll(true);
        }

        private void clearAllButton_Click(object sender, EventArgs e)
        {
            CheckAll(false);
        }
    }
}

