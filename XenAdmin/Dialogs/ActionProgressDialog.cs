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
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;


namespace XenAdmin.Dialogs
{
    /// <summary>
    /// Monitors an AsyncAction, showing a progress bar and the action's Description. Shows the exception
    /// to the user if the Action fails, and waits for the user to click OK. Has a cancel button: make
    /// sure if enabled that the Action has sensible cancel behaviour. The cancel button is not shown by
    /// default.
    /// </summary>
    internal partial class ActionProgressDialog : XenDialogBase
    {
        public event EventHandler CancelClicked;
        public readonly AsyncAction action;

        public bool ShowTryAgainMessage { private get; set; } = true;

        public bool ShowException { private get; set; } = true;

        public bool ShowCancel
        {
            set => buttonCancel.Visible = value;
        }

        private void HideTitleBarIcons()
        {
            ControlBox = false;
            MinimizeBox = false;
            MaximizeBox = false;
        }

        public ActionProgressDialog(String text)
        {
            InitializeComponent();
            labelStatus.Text = text;
            labelSubActionStatus.Visible = false;
            progressBar1.Style = ProgressBarStyle.Marquee;
            ShowIcon = false;
            HideTitleBarIcons();
        }

        public ActionProgressDialog(AsyncAction action, ProgressBarStyle progressBarStyle)
        {
            InitializeComponent();
            this.action = action;
            action.Completed += action_Completed;
            action.Changed += action_Changed;
            progressBar1.Style = progressBarStyle;
            updateStatusLabel();
            buttonCancel.Enabled = action.CanCancel;
            ShowIcon = false;
            HideTitleBarIcons();
        }

        private void action_Changed(ActionBase sender)
        {
            Program.AssertOffEventThread();

            if (Disposing || IsDisposed || Program.Exiting)
                return;

            action.RecomputeCanCancel();
            Program.Invoke(this, action_Changed_);
        }

        private void action_Changed_()
        {
            Program.AssertOnEventThread();
            progressBar1.Value = action.PercentComplete;
            updateStatusLabel();
            buttonCancel.Enabled = action.CanCancel;
        }

        private void UpdateLabel(Label label, string description, string title)
        {
            label.Text = !string.IsNullOrEmpty(description)
                             ? description
                             : !string.IsNullOrEmpty(title) ? title : string.Empty;
        }

        private void updateStatusLabel()
        {
            UpdateLabel(labelStatus, action.Description, action.Title);
            UpdateSubActionStatusLabel();
        }

        private void UpdateSubActionStatusLabel()
        {
            var multipleAction = action as MultipleAction;
            labelSubActionStatus.Visible = multipleAction != null && multipleAction.ShowSubActionsDetails;
            if (labelSubActionStatus.Visible)
            {
                UpdateLabel(labelSubActionStatus, multipleAction?.SubActionDescription, multipleAction?.SubActionTitle);
            }
        }

        private void action_Completed(ActionBase sender)
        {
            if (Disposing || IsDisposed || Program.Exiting)
                return;

            Program.Invoke(this, () =>
            {
                if (action.Succeeded || action.Cancelled)
                {
                    Close();
                    return;
                }

                SwitchDialogToShowErrorState();
            });
        }

        private void SwitchDialogToShowErrorState()
        {
            this.SuspendLayout();

            progressBar1.Visible = false;
            buttonCancel.Visible = false;
            buttonClose.Visible = true;
            CancelButton = buttonClose;
            ControlBox = true;
            ShowIcon = true;
            labelException.Visible = ShowException;
            icon.Visible = true;
            icon.Image = Images.StaticImages._000_error_h32bit_32;
            labelBottom.Visible = ShowTryAgainMessage;

            if (action.Exception == null)
            {
                labelException.Text = Messages.WIZARD_INTERNAL_ERROR;
            }
            else if (action.Exception is CancelledException)
            {
                labelException.Text = Messages.CANCELLED_BY_USER;
            }
            else
            {
                labelException.Text = action.Exception.Message;
            }

            this.ResumeLayout();
            this.CenterToParent();
        }

        protected override void OnShown(EventArgs e)
        {
            Text = BrandManager.BrandConsole;
            action?.RunAsync();
            base.OnShown(e);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            buttonCancel.Enabled = false;

            if (CancelClicked != null)
                CancelClicked(sender, e);

            if (action != null)
                action.Cancel();
        }

        private void buttonClose_VisibleChanged(object sender, EventArgs e)
        {
            if (buttonClose.Visible)
                buttonClose.Focus();
        }
    }
}
