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
        //Change parameter to increase the number of concurrent actions running
        private const int DEFAULT_MAX_NUMBER_OF_PARALLEL_ACTIONS = 25;

        private Dictionary<IXenConnection, List<AsyncAction>> actionsByConnection = new Dictionary<IXenConnection, List<AsyncAction>>();
        private Dictionary<IXenConnection, ProduceConsumerQueue> queuesByConnection = new Dictionary<IXenConnection, ProduceConsumerQueue>();

        private List<AsyncAction> actionsWithNoConnection = new List<AsyncAction>();
        private ProduceConsumerQueue queueWithNoConnection;

        private readonly int maxNumberOfParallelActions;
        private int actionsCount;

        public ParallelAction(IXenConnection connection, string title, string startDescription, string endDescription, List<AsyncAction> subActions, bool suppressHistory, bool showSubActionsDetails, int maxNumberOfParallelActions = DEFAULT_MAX_NUMBER_OF_PARALLEL_ACTIONS)
            : base(connection, title, startDescription, endDescription, subActions, suppressHistory, showSubActionsDetails)
        {
            if (Connection != null)
            {
                actionsByConnection.Add(Connection, subActions);
                actionsCount = subActions.Count;
            }
            else
                GroupActionsByConnection(); 
            this.maxNumberOfParallelActions = maxNumberOfParallelActions;
        }

        public ParallelAction(IXenConnection connection, string title, string startDescription, string endDescription, List<AsyncAction> subActions, int maxNumberOfParallelActions = DEFAULT_MAX_NUMBER_OF_PARALLEL_ACTIONS)
            : this(connection, title, startDescription, endDescription, subActions, false, false, maxNumberOfParallelActions)
        { }
      
        /// <summary>
        /// Use this constructor to create a cross connection ParallelAction.
        /// It takes a list of any number of actions, separates them by connections 
        /// and runs a certain number of them simultaneously on each connection, all connections in parallel.
        /// Once one simultaneous action is finished the next one in the queue is started until all are complete.
        /// </summary>
        public ParallelAction(string title, string startDescription, string endDescription, List<AsyncAction> subActions, bool suppressHistory, bool showSubActionsDetails, int maxNumberOfParallelActions = DEFAULT_MAX_NUMBER_OF_PARALLEL_ACTIONS)
            : base(null, title, startDescription, endDescription, subActions, suppressHistory, showSubActionsDetails)
        {
            GroupActionsByConnection();
            this.maxNumberOfParallelActions = maxNumberOfParallelActions;
        }

        public ParallelAction(string title, string startDescription, string endDescription, List<AsyncAction> subActions, int maxNumberOfParallelActions = DEFAULT_MAX_NUMBER_OF_PARALLEL_ACTIONS)
            : this(title, startDescription, endDescription, subActions, false, false, maxNumberOfParallelActions)
        { }

        private void GroupActionsByConnection()
        {
            actionsCount = 0;
            foreach (AsyncAction action in subActions)
            {
                if (action.Connection != null)
                {
                    if (action.Connection.IsConnected)
                    {
                        if (!actionsByConnection.ContainsKey(action.Connection))
                        {
                            actionsByConnection.Add(action.Connection, new List<AsyncAction>());
                        }

                        actionsByConnection[action.Connection].Add(action);
                        actionsCount++;
                    }
                }
                else
                {
                    actionsWithNoConnection.Add(action);
                    actionsCount++;
                }
            }
        }


        protected override void RunSubActions(List<Exception> exceptions)
        {
            if (actionsCount == 0)
                return;

            foreach (IXenConnection connection in actionsByConnection.Keys)
            {
                queuesByConnection[connection] = new ProduceConsumerQueue(Math.Min(maxNumberOfParallelActions, actionsByConnection[connection].Count));
                foreach (AsyncAction subAction in actionsByConnection[connection])
                {
                    EnqueueAction(subAction, queuesByConnection[connection], exceptions);
                }
            }

            if (actionsWithNoConnection.Count > 0)
                queueWithNoConnection = new ProduceConsumerQueue(Math.Min(maxNumberOfParallelActions, actionsWithNoConnection.Count));

            foreach (AsyncAction subAction in actionsWithNoConnection)
            {
                EnqueueAction(subAction, queueWithNoConnection, exceptions);
            }

            lock (_lock)
            {
                Monitor.Wait(_lock);
            }
        }

        void EnqueueAction(AsyncAction action, ProduceConsumerQueue queue, List<Exception> exceptions)
        {
            action.Completed += action_Completed;
            queue.EnqueueItem(
                () =>
                {
                    if (Cancelling) // don't start any more actions
                        return;
                    try
                    {
                        action.RunExternal(action.Session);
                    }
                    catch (Exception e)
                    {
                        Failure f = e as Failure;
                        if (f != null && Connection != null &&
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
            int total = 0;
            foreach (IXenConnection connection in actionsByConnection.Keys)
            {
                foreach (var action in actionsByConnection[connection]) 
                    total += action.PercentComplete;
            }
            foreach (var action in actionsWithNoConnection)
                total += action.PercentComplete;
            PercentComplete = (int)(total / actionsCount);
        }

        private readonly object _lock = new object();
        private volatile int i = 0;
        
        void action_Completed(ActionBase sender)
        {
            sender.Completed -= action_Completed;
            lock (_lock)
            {
                i++;
                if (i == actionsCount)
                {
                    Monitor.Pulse(_lock);
                    PercentComplete = 100;
                }
            }
        }

        protected override void MultipleAction_Completed(ActionBase sender)
        {
            base.MultipleAction_Completed(sender);

            foreach (IXenConnection connection in queuesByConnection.Keys)
            {
                queuesByConnection[connection].StopWorkers(false);
            }

            if (queueWithNoConnection != null)
                queueWithNoConnection.StopWorkers(false);
        }
    }
}
