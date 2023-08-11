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

using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.Updates
{
    public class ToggleCdnSyncAction : AsyncAction
    {
        private readonly Pool _pool;
        private readonly bool _enable;

        public ToggleCdnSyncAction(IXenConnection connection, bool enable)
            : base(connection, string.Empty)
        {
            _pool = Helpers.GetPoolOfOne(connection);
            _enable = enable;

            Title = Description = enable ? Messages.YUM_REPO_ACTION_SYNC_SCHEDULE_ENABLE_TITLE : Messages.YUM_REPO_ACTION_SYNC_SCHEDULE_DISABLE_TITLE;

            ApiMethodsToRoleCheck.Add("pool.set_update_sync_enabled");
        }

        protected override void Run()
        {
            Pool.set_update_sync_enabled(Session, _pool.opaque_ref, _enable);

            //wait until the cache has been updated so that the config panel can show
            //the new value if the action was triggered by hitting the Apply button

            Connection.WaitFor(() => _pool.update_sync_enabled == _enable, null);
        }
    }
}
