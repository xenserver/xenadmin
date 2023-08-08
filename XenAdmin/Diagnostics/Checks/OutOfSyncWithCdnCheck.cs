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
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAPI;


namespace XenAdmin.Diagnostics.Checks
{
    public class OutOfSyncWithCdnCheck : PoolCheck
    {
        private const int DAYS_SINCE_LAST_SYNC = 7;

        public OutOfSyncWithCdnCheck(Pool pool)
            : base (pool)
        {
        }

        public override bool CanRun()
        {
            return Helpers.CloudOrGreater(Pool.Connection);
        }

        protected override Problem RunCheck()
        {
            var syncCount = ConnectionsManager.History.Count(a =>
                !a.IsCompleted && a is SyncWithCdnAction syncA && !syncA.Cancelled && syncA.Connection == Pool.Connection);

            if (syncCount > 0 || Pool.current_operations.Values.Contains(pool_allowed_operations.sync_updates))
                return new SyncInProgressProblem(this, Pool);

            if (Helpers.XapiEqualOrGreater_23_18_0(Pool.Connection))
            {
                if (DateTime.UtcNow - Pool.last_update_sync >= TimeSpan.FromDays(DAYS_SINCE_LAST_SYNC))
                    return new CdnOutOfSyncProblem(this, Pool, DAYS_SINCE_LAST_SYNC);
            }

            return null;
        }

        public override string Description => Messages.CHECKING_LAST_CDN_SYNC_DESCRIPTION;
    }
}
