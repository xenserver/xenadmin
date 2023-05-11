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
using XenAdmin.Core;


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class ConsolesOptionsPage : UserControl, IOptionsPage
    {
        public ConsolesOptionsPage()
        {
            InitializeComponent();
            label1.Text = string.Format(label1.Text, BrandManager.BrandConsole);
        }

        public void Build()
        {
            // Fullscreen shortcut keys
            buildKeyCodeListBox();

            // Dock-undock shortcut keys
            buildDockKeyCodeComboBox();

            // Uncapture keyboard and mouse shortcut keys
            buildUncaptureKeyCodeComboBox();

            // Windows Remote Desktop console
            WindowsKeyCheckBox.Checked = Properties.Settings.Default.WindowsShortcuts;
            SoundCheckBox.Checked = Properties.Settings.Default.ReceiveSoundFromRDP;
            AutoSwitchCheckBox.Checked = Properties.Settings.Default.AutoSwitchToRDP;
            ClipboardCheckBox.Checked = Properties.Settings.Default.ClipboardAndPrinterRedirection;
            ConnectToServerConsoleCheckBox.Checked = Properties.Settings.Default.ConnectToServerConsole;

            // Console scaling
            PreserveUndockedScaleCheckBox.Checked = Properties.Settings.Default.PreserveScaleWhenUndocked;
            PreserveVNCConsoleScalingCheckBox.Checked = Properties.Settings.Default.PreserveScaleWhenSwitchBackToVNC;
        }

        private void buildKeyCodeListBox()
        {
            KeyComboListBox.Items.Clear();
            KeyComboListBox.Items.Add("Ctrl+Alt");
            KeyComboListBox.Items.Add("Ctrl+Alt+F");
            KeyComboListBox.Items.Add("F12");
            KeyComboListBox.Items.Add("Ctrl+Enter");
            selectKeyCombo();
        }

        private void buildDockKeyCodeComboBox()
        {
            DockKeyComboBox.Items.Clear();
            DockKeyComboBox.Items.Add(Messages.NONE);
            DockKeyComboBox.Items.Add("Alt+Shift+U");
            DockKeyComboBox.Items.Add("F11");
            selectDockKeyCombo();
        }

        private void selectDockKeyCombo()
        {
            DockKeyComboBox.SelectedIndex = Properties.Settings.Default.DockShortcutKey;
        }

        private void selectKeyCombo()
        {
            KeyComboListBox.SelectedIndex = Properties.Settings.Default.FullScreenShortcutKey;
        }

        private void buildUncaptureKeyCodeComboBox()
        {
            UncaptureKeyComboBox.Items.Clear();
            UncaptureKeyComboBox.Items.Add(Messages.RIGHT_CTRL);
            UncaptureKeyComboBox.Items.Add(Messages.LEFT_ALT);
            selectUncaptureKeyCombo();
        }

        private void selectUncaptureKeyCombo()
        {
            UncaptureKeyComboBox.SelectedIndex = Properties.Settings.Default.UncaptureShortcutKey;
        }

        #region IOptionsPage Members

        public bool IsValidToSave(out Control control, out string invalidReason)
        {
            control = null;
            invalidReason = null;
            return true;
        }

        public void ShowValidationMessages(Control control, string message)
        {
        }

        public void HideValidationMessages()
        {
        }

        public void Save()
        {
            // Fullscreen shortcut keys
            if (KeyComboListBox.SelectedIndex != Properties.Settings.Default.FullScreenShortcutKey)
                Properties.Settings.Default.FullScreenShortcutKey = KeyComboListBox.SelectedIndex;
            // Dock-undock shortcut keys
            if (DockKeyComboBox.SelectedIndex != Properties.Settings.Default.DockShortcutKey)
                Properties.Settings.Default.DockShortcutKey = DockKeyComboBox.SelectedIndex;
            // Uncapture keyboard and mouse shortcut keys
            if (UncaptureKeyComboBox.SelectedIndex != Properties.Settings.Default.UncaptureShortcutKey)
                Properties.Settings.Default.UncaptureShortcutKey = UncaptureKeyComboBox.SelectedIndex;

            // Windows Remote Desktop console
            if (WindowsKeyCheckBox.Checked != Properties.Settings.Default.WindowsShortcuts)
                Properties.Settings.Default.WindowsShortcuts = WindowsKeyCheckBox.Checked;
            if (SoundCheckBox.Checked != Properties.Settings.Default.ReceiveSoundFromRDP)
                Properties.Settings.Default.ReceiveSoundFromRDP = SoundCheckBox.Checked;
            if (AutoSwitchCheckBox.Checked != Properties.Settings.Default.AutoSwitchToRDP)
                Properties.Settings.Default.AutoSwitchToRDP = AutoSwitchCheckBox.Checked;
            if (ClipboardCheckBox.Checked != Properties.Settings.Default.ClipboardAndPrinterRedirection)
                Properties.Settings.Default.ClipboardAndPrinterRedirection = ClipboardCheckBox.Checked;
            if (ConnectToServerConsoleCheckBox.Checked != Properties.Settings.Default.ConnectToServerConsole)
                Properties.Settings.Default.ConnectToServerConsole = ConnectToServerConsoleCheckBox.Checked;

            // Console scaling
            if (PreserveUndockedScaleCheckBox.Checked != Properties.Settings.Default.PreserveScaleWhenUndocked)
                Properties.Settings.Default.PreserveScaleWhenUndocked = PreserveUndockedScaleCheckBox.Checked;
            if (PreserveVNCConsoleScalingCheckBox.Checked != Properties.Settings.Default.PreserveScaleWhenSwitchBackToVNC)
                Properties.Settings.Default.PreserveScaleWhenSwitchBackToVNC = PreserveVNCConsoleScalingCheckBox.Checked;
        }

        #endregion

        #region IVerticalTab Members

        public override string Text => Messages.CONSOLE;

        public string SubText => Messages.CONSOLE_DESC;

        public Image Image => Images.StaticImages.console_16;

        #endregion

    }
}
