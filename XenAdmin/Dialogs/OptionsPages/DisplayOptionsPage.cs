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


using System.Drawing;
using System.Windows.Forms;


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class DisplayOptionsPage : UserControl, IOptionsPage
    {
        public DisplayOptionsPage()
        {
            InitializeComponent();
        }

        public void Build()
        {
            GraphAreasRadioButton.Checked = Properties.Settings.Default.FillAreaUnderGraphs;
            GraphLinesRadioButton.Checked = !Properties.Settings.Default.FillAreaUnderGraphs;
            checkBoxStoreTab.Checked = Properties.Settings.Default.RememberLastSelectedTab;
            showTimestampsCheckBox.Checked = Properties.Settings.Default.ShowTimestampsInUpdatesLog;
        }

        #region IOptionsPage Members

        public bool IsValidToSave()
        {
            return true;
        }

        public void ShowValidationMessages()
        {
            // no message
        }

        public void HideValidationMessages()
        {
            // no message
        }

        public void Save()
        {
            if (GraphAreasRadioButton.Checked != Properties.Settings.Default.FillAreaUnderGraphs)
                Properties.Settings.Default.FillAreaUnderGraphs = GraphAreasRadioButton.Checked;

            if (checkBoxStoreTab.Checked != Properties.Settings.Default.RememberLastSelectedTab)
                Properties.Settings.Default.RememberLastSelectedTab = checkBoxStoreTab.Checked;

            if (showTimestampsCheckBox.Checked != Properties.Settings.Default.ShowTimestampsInUpdatesLog)
                Properties.Settings.Default.ShowTimestampsInUpdatesLog = showTimestampsCheckBox.Checked;
        }

        #endregion

        #region IVerticalTab Members

        public override string Text => Messages.DISPLAY;

        public string SubText => Messages.DISPLAY_DETAILS;

        public Image Image => Images.StaticImages._001_PerformanceGraph_h32bit_16;

        #endregion
    }
}
