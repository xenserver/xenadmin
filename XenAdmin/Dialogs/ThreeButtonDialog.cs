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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XenAdmin.Core;


namespace XenAdmin.Dialogs
{
    public partial class ThreeButtonDialog : XenDialogBase
    {
        private bool closedFromButton;
        private string helpName = "DefaultHelpTopic";
        private string _windowTitle = BrandManager.BrandConsole;

        /// <summary>
        /// Gives you a dialog with the specified buttons.
        /// </summary>
        /// <param name="buttons">>Must be between 1 and 3 buttons</param>
        protected ThreeButtonDialog(Image image, string mainMessage, params TBDButton[] buttons)
        {
            InitializeComponent();

            Debug.Assert(buttons.Length <= 3);

            if (buttons.Length == 0)
                buttons = new[] {ButtonOK};

            if (image == null)
                pictureBoxIcon.Visible = false;
            else
                pictureBoxIcon.Image = image;

            labelMessage.Text = mainMessage;

            var allButtons = new List<Button> {button1, button2, button3};

            int i = 0;
            while (i < buttons.Length)
            {
                allButtons[i].Visible = true;
                allButtons[i].Text = buttons[i].label;
                allButtons[i].DialogResult = buttons[i].result;

                if (buttons[i].defaultAction == ButtonType.ACCEPT)
                    AcceptButton = allButtons[i];
                else if (buttons[i].defaultAction == ButtonType.CANCEL)
                    CancelButton = allButtons[i];

                if (buttons[i].selected)
                    allButtons[i].Select();

                i++;
            }

            if (buttons.Length == 1)
            {
                CancelButton = allButtons[0];
                allButtons[0].Select();
            }

            while (i < allButtons.Count)
            {
                allButtons[i].Visible = false;
                i++;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = _windowTitle;
        }

        public string WindowTitle
        {
            get => _windowTitle;
            set => _windowTitle = string.IsNullOrEmpty(value) ? BrandManager.BrandConsole : value;
        }

        public bool ShowLinkLabel
        {
            get { return linkLabel1.Visible; }
            set { linkLabel1.Visible = value; }
        }

        public string LinkText
        {
            get { return linkLabel1.Text; }
            set { linkLabel1.Text = value; }
        }

        public string LinkData { get; set; }
        
        public Action LinkAction { get; set; }

        public bool ShowCheckbox
        {
            get => checkBoxOption.Visible;
            set => checkBoxOption.Visible = value;
        }

        public string CheckboxCaption
        {
            get => checkBoxOption.Text;
            set => checkBoxOption.Text = value;
        }

        public bool IsCheckBoxChecked
        {
            get => checkBoxOption.Checked;
            set => checkBoxOption.Checked = value;
        }

        /// <summary>
        /// The message displayed on the dialog
        /// </summary>
        public string Message => labelMessage.Text;

        /// <summary>
        /// Use this to set the get-only base class property HelpName
        /// </summary>
        internal string HelpNameSetter
        {
            set
            {
                helpName = value;
                HelpButton = true;
            }
        }

        internal override string HelpName => helpName;

        public class TBDButton
        {
            public readonly string label;
            public readonly DialogResult result;
            public readonly ButtonType defaultAction = ButtonType.NONE;
            public readonly bool selected;

            /// <summary>
            /// Describes a button for the three button dialog.
            /// </summary>
            /// <param name="label">The label for the button</param>
            /// <param name="result">The result to return on click.</param>
            /// <param name="defaultButtonType">The role the button plays in the dialog</param>
            /// <param name="selected">Whether the button is selected by default</param>
            public TBDButton(string label, DialogResult result, ButtonType? defaultButtonType = null, bool selected = false)
            {
                this.label = label;
                this.result = result;

                if (defaultButtonType.HasValue)
                    defaultAction = defaultButtonType.Value;
                else if (result == DialogResult.OK || result == DialogResult.Yes)
                    defaultAction = ButtonType.ACCEPT;
                else if (result == DialogResult.No || result == DialogResult.Cancel)
                    defaultAction = ButtonType.CANCEL;

                this.selected = selected;
            }
        }

        /// <summary>
        /// Using Accept and Cancel results in this button becoming
        /// the DefaultAcceptButton and DefaultCancelButton respectively
        /// </summary>
        public enum ButtonType { NONE, ACCEPT, CANCEL }

        /// <summary>
        /// Retrieves a button with label Messages.YES_BUTTON_CAPTION and result DialogResult.Yes
        /// </summary>
        public static TBDButton ButtonYes => new TBDButton(Messages.YES_BUTTON_CAPTION, DialogResult.Yes);

        /// <summary>
        /// Retrieves a button with label Messages.NO_BUTTON_CAPTION and result DialogResult.No
        /// </summary>
        public static TBDButton ButtonNo => new TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No);

        /// <summary>
        /// Retrieves a button with label Messages.OK and result DialogResult.OK)
        /// </summary>
        public static TBDButton ButtonOK => new TBDButton(Messages.OK, DialogResult.OK);

        /// <summary>
        /// Retrieves a button with label Messages.CANCEL and result DialogResult.Cancel
        /// </summary>
        public static TBDButton ButtonCancel => new TBDButton(Messages.CANCEL, DialogResult.Cancel);

        private void ThreeButtonDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing || closedFromButton)
                return;

            // User has closed without pressing a button (e.g. the cross)

            var visibleButtons = new List<Button> {button1, button2, button3}.Where(b => b.Visible).ToList();

            if (CancelButton != null)
            {
                DialogResult = CancelButton.DialogResult;
            }
            else if (visibleButtons.Find(b => b.DialogResult == DialogResult.Cancel) != null)
            {
                DialogResult = DialogResult.Cancel;
            }
            else if (visibleButtons.Count == 1)
            {
                DialogResult = visibleButtons[0].DialogResult;
            }
            else if (visibleButtons.Count == 2 && visibleButtons[0].DialogResult == DialogResult.Yes &&
                     visibleButtons[1].DialogResult == DialogResult.No)
            {
                DialogResult = DialogResult.No;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            closedFromButton = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            closedFromButton = true;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            closedFromButton = true;
            Close();
        }

        private void labelMessage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
            }
            catch
            {
                // Best effort
            } 
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(LinkData))
                    System.Diagnostics.Process.Start(LinkData);
                else if (LinkAction != null)
                    LinkAction.Invoke();
            }
            catch
            {
                // ignored
            }
        }
    }

    public class ErrorDialog : ThreeButtonDialog
    {
        public ErrorDialog(string mainMessage, params TBDButton[] buttons)
            : base(Images.StaticImages._000_error_h32bit_32, mainMessage, buttons)
        {
        }
    }

    public class WarningDialog : ThreeButtonDialog
    {
        public WarningDialog(string mainMessage, params TBDButton[] buttons)
            : base(Images.StaticImages._000_WarningAlert_h32bit_32, mainMessage, buttons)
        {
        }
    }

    public class InformationDialog : ThreeButtonDialog
    {
        public InformationDialog(string mainMessage, params TBDButton[] buttons)
            : base(SystemIcons.Information.ToBitmap(), mainMessage, buttons)
        {
        }
    }

    public class NoIconDialog : ThreeButtonDialog
    {
        public NoIconDialog(string mainMessage, params TBDButton[] buttons)
            : base(null, mainMessage, buttons)
        {
        }
    }
}
