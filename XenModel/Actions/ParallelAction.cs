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
using System.Threading;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    /// <summary>
    /// ParallelAction takes a list of any number of actions and runs a certain number of them simultaneously.
    /// Once one simultaneous action is finished the next one in the queue is started until all are complete
    /// </summary>
    public class ParallelAction : MultipleAction
    {
        /// <summary>
        /// Change this to increase the number of concurrent actions running
        /// </summary>
        private const int DEFAULT_MAX_NUMBER_OF_PARALLEL_ACTIONS = 25;

        private readonly Dictionary<IXenConnection, List<AsyncAction>> _actionsByConnection = new Dictionary<IXenConnection, List<AsyncAction>>();
        private readonly Dictionary<IXenConnection, ProduceConsumerQueue> _queuesByConnection = new Dictionary<IXenConnection, ProduceConsumerQueue>();

        private readonly List<AsyncAction> _actionsWithNoConnection = new List<AsyncAction>();
        private ProduceConsumerQueue _queueWithNoConnection;

        private readonly int _maxNumberOfParallelActions;
        private readonly int _actionsCount;
        private readonly object _lock = new object();
        private volatile int _completedActionsCount ;

        /// <summary>
        /// Use this constructor to create a cross connection ParallelAction.
        /// It takes a list of any number of actions, separates them by connections 
        /// and runs a certain number of them simultaneously on each connection, all connections in parallel.
        /// Once a simultaneous action is finished the next one in the queue is started until all are complete.
        /// </summary>
        public ParallelAction(string title, string startDescription, string endDescription,
            List<AsyncAction> subActions, IXenConnection connection = null,
            bool suppressHistory = false, bool showSubActionsDetails = false,
            int maxNumberOfParallelActions = DEFAULT_MAX_NUMBER_OF_PARALLEL_ACTIONS)
            : base(connection, title, startDescription, endDescription, subActions, suppressHistory, showSubActionsDetails)
        {
            if (Connection == null)
            {
                foreach (var action in subActions)
                {
                    if (action.Connection == null)
                    {
                        _actionsWithNoConnection.Add(action);
                        _actionsCount++;
                    }
                    else if (action.Connection.IsConnected)
                    {
                        if (!_actionsByConnection.ContainsKey(action.Connection))
                            _actionsByConnection.Add(action.Connection, new List<AsyncAction>());

                        _actionsByConnection[action.Connection].Add(action);
                        _actionsCount++;
                    }
                }
            }
            else
            {
                _actionsByConnection.Add(Connection, subActions);
                _actionsCount = subActions.Count;
            }

            _maxNumberOfParallelActions = maxNumberOfParallelActions;
        }


        protected override void RunSubActions(List<Exception> exceptions)
        {
            if (_actionsCount == 0)
                return;

            foreach (var connection in _actionsByConnection.Keys)
            {
                _queuesByConnection[connection] = new ProduceConsumerQueue(Math.Min(_maxNumberOfParallelActions, _actionsByConnection[connection].Count));
                foreach (AsyncAction subAction in _actionsByConnection[connection])
                {
                    EnqueueAction(subAction, _queuesByConnection[connection], exceptions);
                }
            }

            if (_actionsWithNoConnection.Count > 0)
                _queueWithNoConnection = new ProduceConsumerQueue(Math.Min(_maxNumberOfParallelActions, _actionsWithNoConnection.Count));

            foreach (var subAction in _actionsWithNoConnection)
            {
                EnqueueAction(subAction, _queueWithNoConnection, exceptions);
            }

            lock (_lock)
            {
                Monitor.Wait(_lock);
            }
        }

        private void EnqueueAction(AsyncAction action, ProduceConsumerQueue queue, List<Exception> exceptions)
        {
            action.Completed += action_Completed;
            queue.EnqueueItem(
                () =>
                {
                    if (Cancelling) // don't start any more actions
                        return;
                    try
                    {
                        action.RunSync(action.Session);
                    }
                    catch (Exception e)
                    {
                        if (e is Failure f && Connection != null &&
                            f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                        {
                            Failure.ParseRBACFailure(f, action.Connection, action.Session ?? action.Connection.Session);
                        }

                        exceptions.Add(e);
                        // Record the first exception we come to. Though later if there are more than one we will replace this with non specific one.
                        if (Exception == null)
                            Exception = e;
                    }
                });
        }

        protected override void RecalculatePercentComplete()
        {
            if (_actionsCount == 0)
                return;

            int total = 0;

            foreach (var connection in _actionsByConnection.Keys)
            {
                foreach (var action in _actionsByConnection[connection]) 
                    total += action.PercentComplete;
            }

            foreach (var action in _actionsWithNoConnection)
                total += action.PercentComplete;

            PercentComplete = total / _actionsCount;
        }

        private void action_Completed(ActionBase sender)
        {
            sender.Completed -= action_Completed;
            lock (_lock)
            {
                _completedActionsCount++;
                if (_completedActionsCount == _actionsCount)
                {
                    Monitor.Pulse(_lock);
                    PercentComplete = 100;
                }
            }
        }

        protected override void MultipleAction_Completed(ActionBase sender)
        {
            base.MultipleAction_Completed(sender);

            foreach (var connection in _queuesByConnection.Keys)
                _queuesByConnection[connection].StopWorkers(false);


            _queueWithNoConnection?.StopWorkers(false);
        }
    }
}
