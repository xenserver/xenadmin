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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Dialogs;

namespace XenAdmin.Diagnostics.Problems
{
    public abstract class Problem : IComparable<Problem>, IDisposable
    {
        protected Problem(Check check)
        {
            Check = check ?? throw new ArgumentNullException();
        }

        private void SolutionAction_Completed(ActionBase sender)
        {
            SolutionActionCompleted = true;
        }

        private void RegisterSolutionActionEvent()
        {
            if (SolutionAction != null)
                SolutionAction.Completed += SolutionAction_Completed;
        }

        private void DeregisterSolutionActionEvent()
        {
            if (SolutionAction != null)
                SolutionAction.Completed -= SolutionAction_Completed;
        }

        public virtual bool IsFixable => true;
        public abstract string Title { get; }
        public abstract string Description { get; }
        public bool SolutionActionCompleted { get; private set; }
        public AsyncAction SolutionAction { get; private set; }

        protected virtual AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            return null;
        }

        public AsyncAction GetSolutionAction(out bool cancelled)
        {
            DeregisterSolutionActionEvent();
            SolutionAction = CreateAction(out cancelled);
            SolutionActionCompleted = false;
            RegisterSolutionActionEvent();
            return SolutionAction;
        }

        public abstract string HelpMessage { get; }

        public Check Check { get; }

        public virtual AsyncAction CreateUnwindChangesAction()
        {
            return null;
        }

        public int CompareTo(Problem other)
        {
            if (other == null)
                return 1;

            var result = string.Compare(Description, other.Description);

            if (result == 0)
                result = string.Compare(Title, other.Title);

            if (result == 0 && Check != null && Check.XenObject != null && other.Check != null)
                result = Check.XenObject.CompareTo(other.Check.XenObject);

            return result;
        }

        public override bool Equals(object obj)
        {
            Problem other = obj as Problem;
            if (other == null)
                return false;

            if (GetType() != other.GetType())
                return false;

            return CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return Check.GetHashCode();
        }

        public virtual Image Image => Images.GetImage16For(Icons.Error);

        #region IDisposable Members

        public void Dispose()
        {
            DeregisterSolutionActionEvent();
        }

        #endregion

        public static List<AsyncAction> GetUnwindChangesActions(List<Problem> problems)
        {
            if (problems == null)
                return new List<AsyncAction>();

            var actions = from problem in problems
                          where problem.SolutionActionCompleted
                          let action = problem.CreateUnwindChangesAction()
                          where action != null && action.Connection != null && action.Connection.IsConnected
                          select action;

            return actions.ToList();
        }
    }


    public abstract class ProblemWithMoreInfo : Problem
    {
        protected ProblemWithMoreInfo(Check check)
            : base(check)
        {
        }

        public override bool IsFixable => false;

        public override string Title => Check.Description;
        public override string HelpMessage => Messages.MORE_INFO;

        public abstract string Message { get; }

        public virtual string LinkData => null;
        public virtual string LinkText => LinkData;

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                using (var dlg = new ErrorDialog(Message))
                {
                    if (!string.IsNullOrEmpty(LinkText) && !string.IsNullOrEmpty(LinkData))
                    {
                        dlg.LinkText = LinkText;
                        dlg.LinkData = LinkData;
                        dlg.ShowLinkLabel = true;
                    }
                    dlg.ShowDialog();
                }
            });

            cancelled = true;
            return null;
        }
    }


    public abstract class ProblemWithInformationUrl : Problem
    {
        protected ProblemWithInformationUrl(Check check) : base(check)
        {
        }

        public abstract Uri UriToLaunch { get; }

        public override bool IsFixable => false;

        public virtual string LinkText => UriToLaunch != null ? Messages.DETAILS : string.Empty;

        public void LaunchUrlInBrowser()
        {
            try
            {
                if (UriToLaunch != null)
                    Process.Start(UriToLaunch.AbsoluteUri);
            }
            catch (Exception)
            {
                using (var dlg = new ErrorDialog(string.Format(Messages.COULD_NOT_OPEN_URL,
                        UriToLaunch != null ? UriToLaunch.AbsoluteUri : string.Empty)))
                {
                    dlg.ShowDialog(Program.MainWindow);
                }
            }
        }
    }
}
