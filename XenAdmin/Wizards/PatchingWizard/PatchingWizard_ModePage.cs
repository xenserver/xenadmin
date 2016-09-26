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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Alerts;

namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_ModePage : XenTabPage
    {
        public XenServerPatchAlert SelectedUpdateAlert { private get; set; }

        public PatchingWizard_ModePage()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get { return Messages.PATCHINGWIZARD_MODEPAGE_TEXT; }
        }

        public override string PageTitle
        {
            get { return Messages.PATCHINGWIZARD_MODEPAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "UpdateMode"; }
        }

        public override bool EnablePrevious()
        {
            return true;
        }

        public override string NextText(bool isLastPage)
        {
            return Messages.UPDATES_WIZARD_APPLY_UPDATE;
        }

        public Dictionary<string, LivePatchCode> LivePatchCodesByHost
        {
            get;
            set;
        }
        
        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            textBoxLog.Clear();

            switch (SelectedUpdateType)
            {
                case UpdateType.NewRetail:
                case UpdateType.Existing:
                    textBoxLog.Text = PatchingWizardModeGuidanceBuilder.ModeRetailPatch(SelectedServers, Patch, LivePatchCodesByHost);
                    AutomaticRadioButton.Enabled = true;
                    AutomaticRadioButton.Checked = true;
                    break;
                case UpdateType.NewSuppPack:
                    AutomaticRadioButton.Enabled = true;
                    AutomaticRadioButton.Checked = true;
                    textBoxLog.Text = PatchingWizardModeGuidanceBuilder.ModeSuppPack(SelectedServers);
                    break;
            }

            if (SelectedUpdateType == UpdateType.NewSuppPack || SelectedServers.Exists(server => !Helpers.ClearwaterOrGreater(server)))
            {
                removeUpdateFileCheckBox.Checked = false;
                removeUpdateFileCheckBox.Visible = false;
            }
        }

        public override bool EnableNext()
        {
            return AutomaticRadioButton.Checked || ManualRadioButton.Checked;
        }

        private void UpdateEnablement()
        {
            textBoxLog.Enabled = ManualRadioButton.Checked;
            OnPageUpdated();
        }

        #region Accessors

        public string ManualTextInstructions
        {
            get
            {
                return textBoxLog.Text;
            }
        }

        public bool IsAutomaticMode
        {
            get
            {
                return AutomaticRadioButton.Checked;
            }
        }

        public bool RemoveUpdateFile
        {
            get
            {
                return removeUpdateFileCheckBox.Checked;
            }
        }

        public List<Host> SelectedServers { private get; set; }
        public Pool_patch Patch { private get; set; }
        public UpdateType SelectedUpdateType { private get; set; }

        #endregion

        private void AutomaticRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private void ManualRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filePath = Path.GetTempFileName();
            FileStream fileStream = File.Create(filePath);
            StreamWriter sw = new StreamWriter(fileStream);
            sw.Write(textBoxLog.Text);
            sw.Close();
            fileStream.Close();
            Process.Start("notepad.exe", filePath);
        }
    }


}
