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
using System.ComponentModel;
using System.Drawing;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;


namespace XenAdmin.Controls
{
    public class ActionRow : CustomHistoryRow
    {
        /// <summary>
        /// should never be null
        /// </summary>
        public ActionBase Action;
        public List<string> AppliesTo = new List<string>();
        public ActionType Type;

        private DateTime finished = DateTime.MinValue;

        protected override string TimeTaken
        {
            get
            {
                if (!Action.IsCompleted || finished == DateTime.MinValue)
                {
                    TimeSpan time = DateTime.Now.Subtract(Action.Started);
                    time = new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds, 0);
                    return time.ToString();
                }
                else
                {
                    TimeSpan time = finished.Subtract(Action.Started);
                    time = new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds, 0);
                    return time.ToString();
                }
            }
        }

        public ActionRow(ActionBase action)
        {
            Type = action.Type;
            AppliesTo = action.AppliesTo;
            Action = action;
            this.Image = getImage();
            TimeOccurred = HelpersGUI.DateTimeToString(Action.Started, Messages.DATEFORMAT_DMY_HMS, true);
            this.CancelButtonClicked += new EventHandler<EventArgs>(CancelAction);
            setupRowDetails();
            if (!Action.IsCompleted)
            {
                Action.Changed += new EventHandler<EventArgs>(Action_Changed);
                Action.Completed += new EventHandler<EventArgs>(Action_Completed);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(Action != null)
                {
                    Action.Changed -= Action_Changed;
                    Action.Completed -= Action_Completed;
                }
            }

            base.Dispose(disposing);

        }

        private void Action_Completed(object sender, EventArgs e)
        {
            setupRowDetails();
        }

        private void CancelAction(object sender, EventArgs e)
        {
            Action.Cancel();
            setupRowDetails();
        }

        private void setupRowDetails()
        {
            AppliesTo = Action.AppliesTo;

            AsyncAction asyncAction = Action as AsyncAction;

            if (Action.Title == null)
            {
                // Should occur rarely if ever
                IXenConnection conn = asyncAction == null ? null : asyncAction.Connection;

                string conn_name = conn == null ? null : Helpers.GetName(conn);
                string vm_name = Action.VM == null ? null : Action.VM.Name;
                string desc = Action.Description;

                if (conn_name != null && vm_name != null && desc != null)
                {
                    this.Title = string.Format(Messages.HISTORYROW_ON_WITH, conn_name, vm_name, desc);
                }
                else if (vm_name != null && desc != null)
                {
                    this.Title = string.Format(Messages.HISTORYROW_WITH, vm_name, desc);
                }
                else if (conn_name != null && desc != null)
                {
                    this.Title = string.Format(Messages.HISTORYROW_ON, conn_name, desc);
                }
                else if (desc != null)
                {
                    this.Title = desc;
                }
                else
                {
                    this.Title = "";
                }
            }
            else
            {
                this.Title = Action.Title;
            }

            Error = (Action.Exception != null || Action.Type == ActionType.Error) && !(Action.Exception is CancelledException);

            Image = getImage();

            ShowTime = Action.Type == ActionType.Action || Action.Type == ActionType.Meddling || Action.Type == ActionType.Error;

            if (Action.Exception == null)
            {
                Description = Action.Description;
            }
            else if (Action.Exception is CancelledException)
            {
                Description = Messages.EXCEPTION_USER_CANCELLED;
            }
            else if (Action.Exception is I18NException)
            {
                Description = ((I18NException)Action.Exception).I18NMessage;
            }
            else
            {
                Description = Action.Exception.Message;
            }
            ShowProgress = Action.ShowProgress && !Action.IsCompleted;
            ShowCancel = !Action.IsCompleted;
            Progress = Action.PercentComplete;
            CancelEnabled = Action.CanCancel;

            if (Action.IsCompleted)
            {
                Type = Action.Type;
                finished = Action.Finished;
                if (ParentPanel != null)
                {
                    Program.BeginInvoke(ParentPanel, delegate()
                    {
                        ParentPanel.Refresh();
                    });
                }
            }
        }

        private void Action_Changed(object sender, EventArgs e)
        {
            if (Action is AsyncAction)
                (Action as AsyncAction).RecomputeCanCancel();
            setupRowDetails();
        }

        private Image getImage()
        {
            switch (Action.Type)
            {
                case ActionType.Information:
                    return Properties.Resources._000_Info3_h32bit_16;
                case ActionType.Action:
                case ActionType.Meddling:
                    return Properties.Resources.commands_16;
                case ActionType.Error:
                    return Properties.Resources._000_error_h32bit_16;
                case ActionType.Alert:
                    return Properties.Resources._000_Alert2_h32bit_16;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}
