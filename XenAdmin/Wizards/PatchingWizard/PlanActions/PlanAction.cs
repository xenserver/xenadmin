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
using System.Reflection;
using System.Threading;
using log4net;
using XenAdmin.Network;
using XenAPI;
using System.Diagnostics;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public abstract class PlanAction
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private int _percentComplete;
        public event EventHandler OnPercentCompleteChange;
        public event EventHandler OnActionError;
        public event Action<PlanAction, Host> StatusChanged;
        public Exception Error;
        protected bool Cancelling = false;

        protected bool visible = true;
        public bool Visible
        {
            get
            {
                return visible;
            }
        }
        
        private string status;
        public string Status
        {
            get { return status; }
            protected set
            {
                if(value != status)
                {
                    status = value;
                    if (StatusChanged != null)
                        StatusChanged(this, CurrentHost);
                }
            }
        }

        protected virtual Host CurrentHost { get { return null; } }

        public int PercentComplete
        {
            get
            {
                return _percentComplete;
            }

            protected set
            {
                _percentComplete = value;
                if (OnPercentCompleteChange != null)
                    OnPercentCompleteChange(this, new EventArgs());
            }
        }

        protected string _title;

        public string Title
        {
            get
            {
                return _title;
            }
        }

        public string TitlePlan { get; set; }

        private bool _showProgressBar = false;
        public event EventHandler OnShowProgressBarChange;

        public virtual bool ShowProgressBar
        {
            get
            {
                return _showProgressBar;
            }

            set
            {
                _showProgressBar = value;
                if (OnShowProgressBarChange != null)
                    OnShowProgressBarChange(this, new EventArgs());
            }
        }

        protected PlanAction(string title)
        {
            _percentComplete = 0;
            _title = title;
        }

        protected abstract void _Run();

        private bool _running = false;

        public virtual void Run()
        {
            _running = true;

            try
            {
                _Run();
            }
            catch (Exception e)
            {
                Failure f = e as Failure;
                if (f != null && f.ErrorDescription != null && f.ErrorDescription.Count > 1 && f.ErrorDescription[1].Contains(FriendlyErrorNames.SR_BACKEND_FAILURE_432))
                {
                    // ignore this exception (CA-62989) in order to allow the Upgrade wizard to continue upgrading all the hosts in a pool. The detached SRs will be reported on Finish
                    /*var dialog = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Exclamation, Messages.STORAGELINK_SR_NEEDS_REATTACH));
                    Program.Invoke(Program.MainWindow, () => dialog.ShowDialog(Program.MainWindow));
                    throw new Exception(Messages.STORAGELINK_SR_NEEDS_REATTACH);*/
                    log.Warn(Messages.STORAGELINK_SR_NEEDS_REATTACH, f);
                }
                else
                {
                    Error = e;
                    if (OnActionError != null)
                        OnActionError(this, new EventArgs());
                    throw;
                }
            }
            finally
            {
                _running = false;
            }

            PercentComplete = 100;
        }


        protected delegate void UpdateProgressDelegate(int progress);

        private void UpdateProgress(int progress)
        {
            this.PercentComplete = progress;
        }

        protected string PollTaskForResultAndDestroy(IXenConnection connection, ref Session session, XenRef<Task> task)
        {
            return PollTaskForResultAndDestroy(connection, ref session, task, 0, 100);
        }

        protected string PollTaskForResultAndDestroy(IXenConnection connection, ref Session session, XenRef<Task> task, int min, int max)
        {
            try
            {
                return PollTaskForResult(connection, ref session, task, UpdateProgress, min, max);
            }
            finally
            {
                Task.destroy(session, task);
            }
        }

        protected static string PollTaskForResult(IXenConnection connection, ref Session session,
                                                  XenRef<Task> task, UpdateProgressDelegate updateProgressDelegate)
        {
            return PollTaskForResult(connection, ref session, task, updateProgressDelegate, 0, 100);
        }

        protected static String PollTaskForResult(IXenConnection connection, ref Session session,
                                                  XenRef<Task> task, UpdateProgressDelegate updateProgressDelegate, int min, int max)
        {
            Program.AssertOffEventThread();

            task_status_type status;
            int progress;

            do
            {
                status = (task_status_type)Task.DoWithSessionRetry(connection, ref session,
                                                                   (Task.TaskStatusOp)Task.get_status, task.opaque_ref);
                progress = min + (int)((max - min) * (double)Task.DoWithSessionRetry(connection, ref session,
                                                                                     (Task.TaskProgressOp)Task.get_progress, task.opaque_ref));

                updateProgressDelegate(progress);

                Thread.Sleep(100);
            }
            while (status == task_status_type.pending || status == task_status_type.cancelling);

            if (status == task_status_type.failure)
            {
                throw new Failure(Task.get_error_info(session, task));
            }
            else
            {
                return Task.get_result(session, task);
            }
        }

        public bool IsComplete
        {
            get
            {
                return (PercentComplete == 100 && !_running) || Error != null;
            }
        }

        public override string ToString()
        {
            return this.Title;
        }

        public virtual void Cancel()
        {
            Cancelling = true;
        }
    }
}


