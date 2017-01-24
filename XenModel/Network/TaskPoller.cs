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
using XenAdmin.Actions;
using System.Text.RegularExpressions;
using XenAPI;

namespace XenAdmin.Network
{
    internal class TaskPoller
    {
        private const int SLEEP_TIME = 900;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private AsyncAction _action;
        private readonly int _lo;
        private readonly double _scale;
        private bool taskCompleted = false;

        /// <summary>
        /// Polls the action regularly and updates the history item's progress from the task's progress,
        /// scaled to a value between lo and hi.
        /// </summary>
        public TaskPoller(AsyncAction action, int lo, int hi)
        {
            _action = action;
            _lo = lo;
            if (hi < lo)
            {
                log.Warn("Squelching progress bar reversal.");
                hi = lo + 1;
            }
            _scale = hi - lo;
        }

        public void PollToCompletion()
        {
            try
            {
                DateTime startTime = DateTime.Now;
                int lastDebug = 0;
                log.DebugFormat("Polling for action {0}", _action.Description);//log once we start
                
                while (!taskCompleted)
                {
                    if (AsyncAction.ForcedExiting && !_action.SafeToExit)
                        throw new CancelledException();

                    //then log every 30seconds
                    int currDebug = (int)((DateTime.Now - startTime).TotalSeconds) / 30;
                    if (currDebug > lastDebug)
                    {
                        lastDebug = currDebug;
                        log.DebugFormat("Polling for action {0}", _action.Description);
                    }

                    poll();
                    Thread.Sleep(SLEEP_TIME);
                }
            }
            finally
            {
                _action.DestroyTask();
            }
        }

        private void poll()
        {
            try
            {
                XenAPI.Task task = GetTask();
                _action.Tick((int)(task.progress * _scale + _lo),
                             task.Description == "" ? _action.Description : task.Description);
                switch (task.status)
                {
                    case XenAPI.task_status_type.failure:
                        log.Warn("Action failed due to API failure:\n" + Environment.StackTrace);
                        throw new XenAPI.Failure(new List<string>(task.error_info));

                    case XenAPI.task_status_type.success:
                        taskCompleted = true;
                        _action.Result = task.result;
                        // Work around CA-6597.
                        if (_action.Result != "")
                        {
                            Match m = Regex.Match(_action.Result, "<value>(.*)</value>");
                            if (m.Success)
                            {
                                _action.Result = m.Groups[1].Value;
                            }
                        }
                        break;

                    case XenAPI.task_status_type.cancelled:
                        log.Debug("Action cancelled");
                        throw new CancelledException();

                    case XenAPI.task_status_type.cancelling:
                    case XenAPI.task_status_type.pending:
                        break;
                }
            }
            catch (XenAPI.Failure exn)
            {
                if (exn.ErrorDescription.Count > 1 &&
                    exn.ErrorDescription[0] == XenAPI.Failure.HANDLE_INVALID &&
                    exn.ErrorDescription[1] == "task")
                {
                    // Task has gone away, which means it's finished.
                    taskCompleted = true;
                    _action.PercentComplete = (int)(_scale + _lo);
                    _action.Result = "";
                }
                else
                {
                    throw;
                }
            }
        }

        private Task GetTask()
        {
            Session session = _action.Session;

            try
            {
                return (Task)_action.DoWithSessionRetry(ref session, (Task.TaskGetRecordOp)Task.get_record, _action.RelatedTask.opaque_ref);
            }
            finally
            {
                _action.Session = session;
            }
        }
    }
}
