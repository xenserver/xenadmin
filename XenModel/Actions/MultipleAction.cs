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
using System.Collections.Generic;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using System.Linq;


namespace XenAdmin.Actions
{
    /// <summary>
    /// Run several actions. The outer action is asynchronous, but the sub-actions are run synchronously within it.
    /// </summary>
    public class MultipleAction : AsyncAction, IDisposable
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<AsyncAction> _subActions;
        private readonly string _endDescription;
        private readonly bool _stopOnFirstException;

        public bool ShowSubActionsDetails { get; }
        public string SubActionTitle { get; private set; }
        public string SubActionDescription { get; private set; }

        public MultipleAction(IXenConnection connection, string title, string startDescription,
            string endDescription, List<AsyncAction> subActions, bool suppressHistory = false,
            bool showSubActionsDetails = false, bool stopOnFirstException = false)
            : base(connection, title, startDescription, suppressHistory)
        {
            _endDescription = endDescription;
            _subActions = subActions;
            ShowSubActionsDetails = showSubActionsDetails;
            _stopOnFirstException = stopOnFirstException;
            Completed += MultipleAction_Completed;
            RegisterEvents();
        }

        /// <summary>
        /// The multiple action gets its ApiMethodsToRoleCheck by accumulating the lists from each of the sub-actions
        /// </summary>
        public override RbacMethodList GetApiMethodsToRoleCheck
        {
            get
            {
                var list = new RbacMethodList();
                foreach (var subAction in _subActions)
                    list.AddRange(subAction.GetApiMethodsToRoleCheck);
                return list;
            }
        }
        
        protected sealed override void Run()
        {
            PercentComplete = 0;
            var exceptions = new List<Exception>();
            RecomputeCanCancel();

            RunSubActions(exceptions);

            Tick(100, _endDescription);

            if (exceptions.Count > 1)
            {
                foreach (var e in exceptions)
                    _log.Error(e);

                Exception = new Exception(Messages.SOME_ERRORS_ENCOUNTERED);
            }
            if (Cancelling)
                Exception = new CancelledException();
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !IsCompleted && _subActions.Any(a => !a.IsCompleted);
        }

        protected override void CancelRelatedTask()
        {
            foreach (var subAction in _subActions.Where(a => !a.IsCompleted))
            {
                subAction.Cancel();
            }
        }

        private void RegisterEvents()
        {
            foreach (var subAction in _subActions)
                subAction.Changed += SubActionChanged;
        }

        private void DeRegisterEvents()
        {
            foreach (var subAction in _subActions)
                subAction.Changed -= SubActionChanged;
        }

        private void SubActionChanged(ActionBase sender)
        {
            if (sender is AsyncAction subAction)
            {
                SubActionTitle = subAction.Title;
                SubActionDescription = subAction.Description;
                RecalculatePercentComplete();
                OnChanged();
            }
        }

        protected virtual void RecalculatePercentComplete()
        {
            int total = 0;
            int n = _subActions.Count;
            foreach (var action in _subActions)
                total += action.PercentComplete;
            PercentComplete = total / n;
        }

        protected virtual void RunSubActions(List<Exception> exceptions)
        {
            foreach (var subAction in _subActions)
            {
                if (Cancelling) // don't start any more actions
                    break;
                try
                {
                    SubActionTitle = subAction.Title;
                    subAction.RunSync(Session);
                }
                catch (Exception e)
                {
                    if (e is Failure f && Connection != null && f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                        Failure.ParseRBACFailure(f, Connection, Session ?? Connection.Session);

                    exceptions.Add(e);
                    // Record the first exception we come to. Though later if there are more than one we will replace this with non specific one.
                    if (Exception == null)
                        Exception = e;
                    if (_stopOnFirstException)
                        break;
                }
            }
        }

        protected virtual void MultipleAction_Completed(ActionBase sender)
        {
            // The MultipleAction can sometimes complete prematurely, for example
            // if the sudo dialog is cancelled, of (less likely) if one of the
            // sub-actions throws an exception in its GetApiMethodsToRoleCheck.
            // In this case, we need to cancel this action's sub-actions.
            foreach (var subAction in _subActions)
            {
                if (!subAction.IsCompleted)
                    subAction.Cancel();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            DeRegisterEvents();
        }

        #endregion
    }
}
