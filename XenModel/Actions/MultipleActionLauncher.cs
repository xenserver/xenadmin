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
using System.Text;
using XenAdmin.Actions;
using System.Collections.ObjectModel;
using XenAdmin.Network;

namespace XenAdmin.Actions
{
    /// <summary>
    /// A class for running <see cref="AsyncAction"/>s such that they are synchronous per connection but asynchronous across connections.
    /// </summary>
    public class MultipleActionLauncher
    {
        private readonly ReadOnlyCollection<AsyncAction> _actions;
        private readonly string _title;
        private readonly string _startDescription;
        private readonly string _endDescription;
        private readonly bool _runActionInParallel;

        public MultipleActionLauncher(IEnumerable<AsyncAction> actions, string title, string startDescription, string endDescription, bool runActionInParallel)
        {
            Util.ThrowIfParameterNull(actions, "actions");

            _actions = new ReadOnlyCollection<AsyncAction>(new List<AsyncAction>(actions));
            _title = title;
            _startDescription = startDescription;
            _endDescription = endDescription;
            _runActionInParallel = runActionInParallel;
        }

        public void Run()
        {
            Dictionary<IXenConnection, List<AsyncAction>> actionsByConnection = new Dictionary<IXenConnection, List<AsyncAction>>();
            List<AsyncAction> actionsWithNoConnection = new List<AsyncAction>();

            foreach (AsyncAction action in _actions)
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
                    }
                }
                else
                {
                    actionsWithNoConnection.Add(action);
                }
            }

            foreach (IXenConnection connection in actionsByConnection.Keys)
            {
                if (actionsByConnection[connection].Count == 1)
                {
                    actionsByConnection[connection][0].RunAsync();
                }
                else
                {
                    if (_runActionInParallel)
                        new ParallelAction(connection, _title, _startDescription, _endDescription, actionsByConnection[connection]).RunAsync();
                    else
                        new MultipleAction(connection, _title, _startDescription, _endDescription, actionsByConnection[connection]).RunAsync();
                }
            }

            if (actionsWithNoConnection.Count == 1)
            {
                actionsWithNoConnection[0].RunAsync();
            }
            else if (actionsWithNoConnection.Count > 1)
            {
                if (_runActionInParallel)
                    new ParallelAction(null, _title, _startDescription, _endDescription, actionsWithNoConnection).RunAsync();
                else
                    new MultipleAction(null, _title, _startDescription, _endDescription, actionsWithNoConnection).RunAsync();
            }
        }
    }
}
