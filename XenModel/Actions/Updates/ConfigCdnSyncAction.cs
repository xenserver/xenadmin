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
    public class ConfigCdnSyncAction : AsyncAction
    {
        private readonly Pool _pool;
        private readonly update_sync_frequency _frequency;
        private readonly int _syncDay;

        public ConfigCdnSyncAction(IXenConnection connection, update_sync_frequency frequency, int syncDay)
            : base(connection, string.Empty)
        {
            _pool = Helpers.GetPoolOfOne(connection);
            _frequency = frequency;
            _syncDay = syncDay;

            Title = Description = Messages.YUM_REPO_ACTION_SYNC_SCHEDULE_CONFIG_TITLE;

            ApiMethodsToRoleCheck.Add("pool.configure_update_sync");
        }

        protected override void Run()
        {
            Pool.configure_update_sync(Session, _pool.opaque_ref, _frequency, _syncDay);

            //wait until the cache has been updated so that the config panel can show
            //the new value if the action was triggered by hitting the Apply button

            _pool.Connection.WaitFor(() => _pool.update_sync_frequency == _frequency &&
                                           _pool.update_sync_day == _syncDay, null);
        }
    }
}
