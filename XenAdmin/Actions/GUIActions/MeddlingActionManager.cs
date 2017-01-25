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
using System.ComponentModel;
using XenAPI;

namespace XenAdmin.Actions.GUIActions
{

    /// <summary>
    /// MeddlingActionManager handles task events, categorises tasks depending upon whether they
    /// are ours or not, and creates MeddlingAction instances when necessary
    /// </summary>
    public class MeddlingActionManager
    {
        #region Predicates
        private static readonly ITaskSpecification taskFinishedSuccessfully =
            new TaskHasFinishedSuccessfullySpecification();

        private static readonly ITaskSpecification taskIsSuitableForMeddlingAction =
            new TaskIsSuitableForMeddlingActionSpecification();

        private static readonly ITaskSpecification taskIsUnwanted =
            new TaskIsUnwantedSpecification();
        #endregion

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Tasks that we've seen, but haven't categorised yet.  May only be acccesed under the DictionaryLock.
        /// </summary>
        private static readonly List<string> UnmatchedTasks = new List<string>();

        /// <summary>
        /// Tasks that we've categorised as not being ours, and the corresponding MeddlingAction that we created.  May
        /// only be accessed under the DictionaryLock.
        /// </summary>
        private static readonly Dictionary<string, MeddlingAction> MatchedTasks = new Dictionary<string, MeddlingAction>();

        private static readonly object DictionaryLock = new object();

        public static void TaskCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            lock (DictionaryLock)
            {
                Task task = (Task)e.Element;
                if (e.Action == CollectionChangeAction.Add)
                {
                    if (!UnmatchedTasks.Contains(task.opaque_ref))
                        AddTaskToUnmatchedList(task);
                }
                else if (e.Action == CollectionChangeAction.Remove)
                {
                    CompletelyForgetTask(task);
                }
                else
                {
                    log.DebugFormat(String.Format("Unmatched action from sender -- Action: {0}; Task: {1}", e.Action, task.opaque_ref));
                }
            }
        }

        private static void AddTaskToUnmatchedList(Task task)
        {
            task.PropertyChanged += Task_PropertyChanged;
            UnmatchedTasks.Add(task.opaque_ref);
        }

        private static void CompletelyForgetTask(Task task)
        {
            task.PropertyChanged -= Task_PropertyChanged;
            if (MatchedTasks.ContainsKey(task.opaque_ref))
            {
                MatchedTasks[task.opaque_ref].Update(task, true);
            }
            UnmatchedTasks.Remove(task.opaque_ref);
            MatchedTasks.Remove(task.opaque_ref);
        }

        public static void ForceAddTask(Task task)
        {
            TaskCollectionChanged(task, new CollectionChangeEventArgs(CollectionChangeAction.Add, task));
        }

        public static void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Task task = (Task)sender;
            lock (DictionaryLock)
            {
                if (UnmatchedTasks.Contains(task.opaque_ref))
                {
                    if (taskIsUnwanted.IsSatisfiedBy(task))
                    {
                        RemoveUnmatchedTask(task);
                    }
                    else if (taskIsSuitableForMeddlingAction.IsSatisfiedBy(task))
                    {
                        CreateMeddlingActionForTask(task);
                    }
                    else
                    {
                        log.DebugFormat("Unmatched meddling task skipped -- " + task.opaque_ref);
                    }
                }
                else if (MatchedTasks.ContainsKey(task.opaque_ref))
                {
                    if (taskFinishedSuccessfully.IsSatisfiedBy(task))
                    {
                        UpdateAndDeleteTask(task);
                    }
                    else
                        UpdateTask(task);
                }
                else
                {
                    // else - it is a hidden (etc..) task that has called already and been removed from unmatched 
                    //(and not put into matched) - so ignore it
                    log.DebugFormat("Uncategorised meddling task skipped -- " + task.opaque_ref);
                }
            }
        }

        private static void UpdateTask(Task task)
        {
            MatchedTasks[task.opaque_ref].Update(task, false);
        }

        private static void UpdateAndDeleteTask(Task task)
        {
            task.PropertyChanged -= Task_PropertyChanged;
            MatchedTasks[task.opaque_ref].Update(task, true);
            MatchedTasks.Remove(task.opaque_ref);
        }

        private static void CreateMeddlingActionForTask(Task task)
        {
            // If AppliesTo is set, then the client that created this task knows about our scheme for passing
            // info between clients.  We give the client a window (AwareClientHeuristic) to set this field
            // before deciding that it's a non-aware client.  Having decided that it's one of those two cases,
            // we make a MeddlingAction.

            MeddlingAction a = new MeddlingAction(task);
            UnmatchedTasks.Remove(task.opaque_ref);
            MatchedTasks[task.opaque_ref] = a;
        }

        private static void RemoveUnmatchedTask(Task task)
        {
            // This is one of our tasks, or it's a subtask of something else, or it's one that we
            // just don't care about.  We're going to do no more with it.
            task.PropertyChanged -= Task_PropertyChanged;
            UnmatchedTasks.Remove(task.opaque_ref);
        }
    }
}
