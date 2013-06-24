/* Copyright (c) Citrix Systems Inc. 
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
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Actions
{
    /// <summary>
    /// Batch the VDI migrate in batches of a suitable size and using a certain
    /// action. eg. It may be possible to batch the migrate queue up as VM migrates and VDI migrates
    /// or just VDI migrates etc..
    /// 
    /// Batched-up actions are returned as a parallel action
    /// </summary>
    public class BatchingMigrateVirtualDiskManager
    {
        private const int BATCH_SIZE = 3;

        /// <summary>
        /// How to catch up the multiple VDI actions
        /// </summary>
        public enum Batching
        {
            VdiMigrate,
            VdiMove
        }

        private readonly IXenConnection connection;
        private string startDescription;
        private string endDescription;

        public BatchingMigrateVirtualDiskManager(IXenConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Description for the action when started - has a default
        /// </summary>
        public string CreatedActionStartDescription
        {
            private get
            {
                if (String.IsNullOrEmpty(startDescription))
                    return Messages.STARTED;
                return startDescription;
            }
            set { startDescription = value; }
        }

        
        /// <summary>
        /// Description for the action when finished - has a default
        /// </summary>
        public string CreatedActionEndDescription
        {
            private get
            {
                if (String.IsNullOrEmpty(startDescription))
                    return Messages.COMPLETED;
                
                return endDescription;
            }
            set { endDescription = value; }
        }

        private string CreatedActionTitle { get; set; }

        /// <summary>
        /// Create an action batched up in a specific manner
        /// </summary>
        /// <param name="batching"></param>
        /// <param name="vdis"></param>
        /// <param name="sr"></param>
        /// <returns></returns>
        public ParallelAction BatchAs(Batching batching, List<VDI> vdis, SR sr)
        {
            if (batching == Batching.VdiMigrate)
            {
                CreatedActionTitle = String.Format(Messages.ACTION_MIGRATING_X_VDIS, Convert.ToString(vdis.Count), sr.name_label);
                return CreateNewParallelAction(BatchAsVdiMigrate(vdis, sr));
            }

            if (batching == Batching.VdiMove)
            {
                CreatedActionTitle = String.Format(Messages.ACTION_MOVING_X_VDIS, Convert.ToString(vdis.Count), sr.name_label);
                return CreateNewParallelAction(BatchAsVdiMove(vdis, sr));
            }

            throw new NotSupportedException("The provided option was not supported");
        }

        private ParallelAction CreateNewParallelAction(List<AsyncAction> batch)
        {
            return new ParallelAction(connection, CreatedActionTitle, CreatedActionStartDescription, 
                                      CreatedActionEndDescription, batch)
                       {
                           NumberOfSimultaneousActions = BATCH_SIZE
                       };
        }

        private List<AsyncAction> BatchAsVdiMigrate(List<VDI> vdis, SR sr)
        {
            List<AsyncAction> batch = new List<AsyncAction>();
            vdis.ForEach(vdi => batch.Add(new MigrateVirtualDiskAction(connection, vdi, sr)));
            return batch;
        }

        private List<AsyncAction> BatchAsVdiMove(List<VDI> vdis, SR sr)
        {
            List<AsyncAction> batch = new List<AsyncAction>();
            vdis.ForEach(vdi => batch.Add(new MoveVirtualDiskAction(connection, vdi, sr)));
            return batch;
        }

    }
}
