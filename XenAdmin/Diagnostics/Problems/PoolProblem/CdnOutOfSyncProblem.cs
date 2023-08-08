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

using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    public class CdnOutOfSyncProblem : PoolProblem
    {
        private readonly int _days;

        public CdnOutOfSyncProblem(Check check, Pool pool, int days)
            : base(check, pool)
        {
            _days = days;
        }

        public override string Description => $"{Pool.Connection.Name}: {string.Format(Messages.ALERT_CDN_OUT_OF_SYNC_TITLE, _days)}";

        public override string HelpMessage => Messages.UPDATES_GENERAL_TAB_SYNC_NOW;

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            var syncAction = new SyncWithCdnAction(Pool);
            syncAction.Completed += a => Updates.CheckForCdnUpdates(a.Connection);
            return syncAction;
        }
    }

    public class SyncInProgressProblem : PoolProblem
    {
        public SyncInProgressProblem(Check check, Pool pool)
            : base(check, pool)
        {
        }

        public override string Description => $"{Pool.Connection.Name}: {Messages.YUM_REPO_SYNC_IN_PROGRESS}";

        public override string HelpMessage => null;
    }
}
