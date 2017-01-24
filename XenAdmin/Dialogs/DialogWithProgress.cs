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
using System.Threading;

using XenAdmin.Actions;
using XenAdmin.Network;


namespace XenAdmin.Dialogs
{
    public partial class DialogWithProgress : XenDialogBase
    {
        private int dx = 0;
        private bool shrunk = false;

        List<Control> ProgressControls; //List of controls not to undock
        Dictionary<Control, AnchorStyles> PreviousAnchors; //State of previous anchors (for resizing window)

        /// <summary>
        /// All dialog that extend this one MUST be set to the same size as this, otherwise layout will break.
        /// If you want I different size, I suggest you do it in you derived forms on_load.
        /// </summary>
        /// <param name="connection"></param>
        public DialogWithProgress(IXenConnection connection) 
            : base(connection)
        {
            InitializeComponent();

            RegisterProgressControls();
        }

        public DialogWithProgress()
            : base()
        {
            InitializeComponent();

            RegisterProgressControls();
            SucceededWithWarningDescription = String.Empty;
            SucceededWithWarning = false;
        }

        private void RegisterProgressControls()
        {
            ProgressControls = new List<Control>();
            ProgressControls.Add(this.ActionProgressBar);
            ProgressControls.Add(this.ActionStatusLabel);
            ProgressControls.Add(this.ProgressSeparator);

            dx = ClientSize.Height - ProgressSeparator.Top;
        }

        private void Unanchor()
        {
            PreviousAnchors = new Dictionary<Control, AnchorStyles>();

            foreach (Control control in Controls)
            {
                if (ProgressControls.Contains(control))
                    continue;

                PreviousAnchors.Add(control, control.Anchor);
                control.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }
        }

        private void Reanchor()
        {
            foreach (Control control in Controls)
            {
                if (ProgressControls.Contains(control))
                    continue;

                control.Anchor = PreviousAnchors[control];
            }
        }

        protected void Shrink()
        {
            Program.AssertOnEventThread();

            if (!shrunk)
            {
                shrunk = true;

                //First, clear all the anchors.
                Unanchor();

                //Next, hide the progress bar and label 
                ClearAction();
                ActionStatusLabel.Hide();
                ActionProgressBar.Hide();
                ProgressSeparator.Hide();

                //Finnally, shrink the window and put the anchors back
                MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height - dx);
                ClientSize = new Size(ClientSize.Width, ClientSize.Height - dx);

                Reanchor();
            }
        }

        protected void Grow(MethodInvoker afterwards)
        {
            Program.AssertOnEventThread();

            if (shrunk)
            {
                shrunk = false;

                //First, clear all the anchors.
                Unanchor();

                //Next, grow the window
                BackgroundWorker worker = new BackgroundWorker();

                worker.DoWork += new DoWorkEventHandler(delegate(object o, DoWorkEventArgs e)
                {
                    int expectedHeight = ClientSize.Height + dx;

                    while (ClientSize.Height < expectedHeight)
                    {
                        Program.Invoke(this, delegate()
                        {
                            ClientSize = new Size(ClientSize.Width, (int) (((2.0 * ClientSize.Height) / 3.0) + (expectedHeight / 3.0) + 1.0));
                        });
                        Thread.Sleep(50);
                    }
                });

                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object o, RunWorkerCompletedEventArgs e)
                {
                    Program.Invoke(this, delegate()
                    {
                        MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height + dx);

                        //and put the anchors back
                        Reanchor();

                        //Finnally, show the progress bar and label 
                        ClearAction();
                        ActionStatusLabel.Show();
                        ActionProgressBar.Show();
                        ProgressSeparator.Show();

                        afterwards();
                    });
                });

                worker.RunWorkerAsync();
            }
            else
            {
                afterwards();
            }
        }

        protected void FinalizeProgressControls(ActionBase action)
        {
            if (action == null)
                return;

            Program.AssertOnEventThread();

            if (action.Succeeded)
            {
                if (SucceededWithWarning && !String.IsNullOrEmpty(SucceededWithWarningDescription))
                    SetActionLabelText(String.Format(Messages.X_WITH_WARNING_X, action.Description, SucceededWithWarningDescription), Color.OrangeRed);
                else
                    SetActionLabelText(action.Description, Color.Green);
            }
            else
            {
                string text = action.Exception is CancelledException ? Messages.CANCELLED_BY_USER : action.Exception.Message;
                SetActionLabelText(text, Color.Red);
            }
        }

        protected bool SucceededWithWarning { private get; set; }
        protected string SucceededWithWarningDescription { private get; set; }

        private void SetActionLabelText(string text, Color color)
        {
            ExceptionToolTip.RemoveAll();
            if (string.IsNullOrEmpty(text))
            {
                ActionStatusLabel.Text = "";
                return;
            }

            // take first line, adding elipses if needed to show that text has been cut
            string[] parts = text.Replace("\r", "").Split('\n');
            string clippedText = parts[0];
            if (parts.Length > 1)
                clippedText = clippedText.AddEllipsis();

            ActionStatusLabel.ForeColor = color;
            ActionStatusLabel.Text = clippedText;
            // use original text for tooltip
            ExceptionToolTip.SetToolTip(ActionStatusLabel, text);
            
        }
        
        protected void UpdateProgressControls(ActionBase action)
        {
            if (action == null)
                return;

            Program.AssertOnEventThread();

            SetActionLabelText(action.Description, SystemColors.ControlText);
            ActionProgressBar.Value = action.PercentComplete;
        }

        private void ClearAction()
        {
            SetActionLabelText("", SystemColors.ControlText);
            this.ActionProgressBar.Value = 0;
        }
    }
}