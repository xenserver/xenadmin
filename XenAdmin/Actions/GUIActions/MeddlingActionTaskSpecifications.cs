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
using XenAPI;

namespace XenAdmin.Actions.GUIActions
{
    /// <summary>
    /// Specifcation for a task
    /// </summary>
    public interface ITaskSpecification
    {
        bool IsSatisfiedBy(Task task);
    }

    /// <summary>
    /// Is the task unwanted by XenCenter?
    /// </summary>
    public class TaskIsUnwantedSpecification : ITaskSpecification
    {
        public bool IsSatisfiedBy(Task task)
        {
            return (task.XenCenterUUID == Program.XenCenterUUID ||
                    task.Connection.Resolve(task.subtask_of) != null ||
                    task.Hidden);
        }
    }

    /// <summary>
    /// Is the task suitable to create a meddling action from?
    /// </summary>
    public class TaskIsSuitableForMeddlingActionSpecification : ITaskSpecification
    {
        /// <summary>
        /// Heuristic to determine whether a new task was created by a client aware of our task.AppliesTo scheme, or by some other client.
        /// </summary>
        private readonly TimeSpan awareClientHeuristic = TimeSpan.FromSeconds(5);

        public bool IsSatisfiedBy(Task task)
        {
            return (task.AppliesTo != null ||
                    task.created + task.Connection.ServerTimeOffset < DateTime.UtcNow - awareClientHeuristic);
        }
    }

    /// <summary>
    /// Has the task successfully finished?
    /// </summary>
    public class TaskHasFinishedSuccessfullySpecification : ITaskSpecification
    {
        public bool IsSatisfiedBy(Task task)
        {
            return task.status == task_status_type.success;
        }
    }

}
