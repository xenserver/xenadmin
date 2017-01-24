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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;


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
        public readonly AsyncAction action;
        private bool showTryAgain = true;
        private bool showCancel;
        
        /// <summary>
        /// Default value is false.
        /// </summary>
        public bool ShowCancel
        {
            get
            {
                return showCancel;
            }
            set
            {
                showCancel = value;
                buttonCancel.Visible = showCancel;
            }
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

        public ActionProgressDialog(AsyncAction action, ProgressBarStyle progressBarStyle, bool showTryAgain) :
            this(action, progressBarStyle)
        {
            this.showTryAgain = showTryAgain;
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
                UpdateLabel(labelSubActionStatus, multipleAction.SubActionDescription, multipleAction.SubActionTitle);
        }

        private void action_Completed(ActionBase sender)
        {
            if (Disposing || IsDisposed || Program.Exiting)
                return;

            Program.Invoke(this, action_Completed_);
        }

        private void action_Completed_()
        {
            Program.AssertOnEventThread();

            if (action.Succeeded || action.Cancelled)
            {
                this.Close();
                return;
            }

            SwitchDialogToShowErrorState();
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
            labelException.Visible = true;
            icon.Visible = true;
            icon.Image = SystemIcons.Error.ToBitmap();
            labelBottom.Visible = showTryAgain;

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
            if (action != null)
                action.RunAsync();
            base.OnShown(e);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public event EventHandler CancelClicked;

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
