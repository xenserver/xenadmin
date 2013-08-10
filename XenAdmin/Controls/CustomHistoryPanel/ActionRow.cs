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
using XenAdmin.TabPages;

namespace XenAdmin.Controls
{
    public class ActionRow : CustomHistoryRow
    {
        /// <summary>
        /// should never be null
        /// </summary>
        public ActionBase Action;
        public List<string> AppliesTo = new List<string>();

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
            AppliesTo = action.AppliesTo;
            Action = action;
            Image = action.GetImage();
            TimeOccurred = HelpersGUI.DateTimeToString(Action.Started, Messages.DATEFORMAT_DMY_HMS, true);
            CancelButtonClicked += CancelAction;
            setupRowDetails();

            if (!Action.IsCompleted)
            {
                Action.Changed += Action_Changed;
                Action.Completed += Action_Completed;
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

        private void Action_Completed(ActionBase sender)
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

            Title = Action.GetTitle();

            Error = (Action.Exception != null) && !(Action.Exception is CancelledException);
            Image = Action.GetImage();
            ShowTime = Action.Finished - Action.Started >= TimeSpan.FromSeconds(1);
            Description = Action.GetDescription();
            ShowProgress = Action.ShowProgress && !Action.IsCompleted;
            ShowCancel = !Action.IsCompleted;
            Progress = Action.PercentComplete;
            CancelEnabled = Action.CanCancel;

            if (Action.IsCompleted)
            {
                finished = Action.Finished;
                if (ParentPanel != null)
                    Program.BeginInvoke(ParentPanel, () => ParentPanel.Refresh());
            }
        }

        private void Action_Changed(ActionBase sender)
        {
            if (Action is AsyncAction)
                (Action as AsyncAction).RecomputeCanCancel();
            setupRowDetails();
        }
    }
}
