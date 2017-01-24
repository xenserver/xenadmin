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
using System.Threading;
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Actions.GUIActions
{

    /// <summary>
    /// A "meddling" Action is one being performed by someone else -- in other words, they are ones that we've inferred by the
    /// presence of task instances on the pool.
    /// </summary>
    public class MeddlingAction : CancellingAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MeddlingAction(Task task)
            : base(task.Title, task.Description, false, false)
        {
            RelatedTask = new XenRef<Task>(task.opaque_ref);

            this.Host = task.Connection.Resolve(task.resident_on);

            if (this.Host == null)
                this.Host = Helpers.GetMaster(task.Connection);

            Started = (task.created + task.Connection.ServerTimeOffset).ToLocalTime();
            SetAppliesToData(task);
            VM = VMFromAppliesTo(task);
            Connection = task.Connection;
            Update(task, false);
        }

        public void Update(Task task, bool deleting)
        {
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 if (!deleting)
                                                 {
                                                     RecomputeCanCancel();
                                                 }

                                                 if (task == null || IsCompleted)
                                                     return;

                                                 log.DebugFormat("Updating task {0} : {1} - {2}", task.opaque_ref,
                                                                 task.created, task.finished);
                                                 
                                                 int percentComplete = task.progress < 0 ? 0 : (int) (100.0*task.progress);
                                                 if (PercentComplete < percentComplete)
                                                    PercentComplete = percentComplete;

                                                 SetFatalErrorData(task);

                                                 DetermineIfTaskIsComplete(task, deleting);

                                                 if (IsCompleted)
                                                     Description = Messages.COMPLETED;

                                                 DestroyUnwantedOperations(task);

                                                 if (deleting)
                                                     LogoutCancelSession();
                                             });
        }

        private void DetermineIfTaskIsComplete(Task task, bool deleting)
        {
            if (task.finished.Year > 1970)
            {
                DateTime t = task.finished + task.Connection.ServerTimeOffset;
                Finished = t.ToLocalTime();
                IsCompleted = true;
            }
            else if (deleting)
            {
                Finished = DateTime.Now;
                IsCompleted = true;
            }
            else
            {
                StartedRunning = true;
            }
        }

        private void SetFatalErrorData(Task task)
        {
            string[] err = task.error_info;
            if (err != null && err.Length > 0)
                Exception = new Failure(err);
            else if (task.status == task_status_type.cancelled)
                Exception = new CancelledException();
        }

        private void SetAppliesToData(Task task)
        {
            List<string> applies_to = task.AppliesTo;
            if (applies_to != null)
            {
                AppliesTo.AddRange(applies_to);
                Description = task.Name;
            }
            else
            {
                // A non-aware client has created this task.  We'll create a new action for this, and place it under
                // the task.resident_on host, or if that doesn't resolve, the pool master.
                Host host = task.Connection.Resolve(task.resident_on) ?? Helpers.GetMaster(task.Connection);
                if (host != null)
                    AppliesTo.Add(host.opaque_ref);
            }
        }

        private VM VMFromAppliesTo(Task task)
        {
            foreach (string r in AppliesTo)
            {
                VM vm = task.Connection.Resolve(new XenRef<VM>(r));
                if (vm != null)
                    return vm;
            }
            return null;
        }

        private void DestroyUnwantedOperations(Task task)
        {
            string[] err = task.error_info;
            if (task.Name == "SR.create" && err != null && err.Length > 0 && err[0] == Failure.SR_BACKEND_FAILURE_107)
            {
                // This isn't an SR create at all, it is a scan for LUNs. Hide it, since the 'error' info contains loads of XML,
                // and is not useful. We don't know this until the error occurs though. Destroy the MeddlingAction.
                task.PropertyChanged -= MeddlingActionManager.Task_PropertyChanged;
                ConnectionsManager.History.Remove(this);
            }
        }
    }
}
