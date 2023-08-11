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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs.ServerUpdates;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Alerts
{
    public class OutOfSyncWithCdnAlert : Alert
    {
        private readonly TimeSpan _outOfSyncSpan;
        private readonly Pool _pool;

        private OutOfSyncWithCdnAlert(Pool pool, DateTime timestamp)
        {
            _timestamp = timestamp;
            _pool = pool;
            Connection = _pool.Connection;

            _outOfSyncSpan = _timestamp - _pool.last_update_sync;

            if (_outOfSyncSpan >= TimeSpan.FromDays(180))
                Priority = AlertPriority.Priority1;
            else if (_outOfSyncSpan >= TimeSpan.FromDays(90))
                Priority = AlertPriority.Priority2;
        }

        public static bool TryCreate(IXenConnection connection, out Alert alert)
        {
            if (Helpers.XapiEqualOrGreater_23_18_0(connection))
            {
                var pool = Helpers.GetPoolOfOne(connection);
                var timestamp = DateTime.UtcNow - connection.ServerTimeOffset;

                if (timestamp - pool.last_update_sync >= TimeSpan.FromDays(90))
                {
                    alert = new OutOfSyncWithCdnAlert(pool, timestamp);
                    return true;
                }
            }

            alert = null;
            return false;
        }

        public override AlertPriority Priority { get; }

        public override string AppliesTo => Helpers.GetName(_pool);

        public override string Description => string.Format(Messages.ALERT_CDN_OUT_OF_SYNC_DESCRIPTION,
            AlertExtensions.GetGuiDate(_pool.last_update_sync));

        public override Action FixLinkAction
        {
            get
            {
                return () =>
                {
                    var syncAction = new SyncWithCdnAction(_pool);
                    syncAction.Completed += a => Updates.CheckForCdnUpdates(a.Connection);
                    syncAction.RunAsync();
                };
            }
        }

        public override string FixLinkText => Messages.UPDATES_GENERAL_TAB_SYNC_NOW;

        public override string HelpID => "TODO";

        public override string Title => string.Format(Messages.ALERT_CDN_OUT_OF_SYNC_TITLE, _outOfSyncSpan.Days);
    }

 
    public class YumRepoNotConfiguredAlert : Alert
    {
        private readonly Pool _pool;

        private YumRepoNotConfiguredAlert(Pool pool, DateTime timestamp)
        {
            _timestamp = timestamp;
            _pool = pool;
            Connection = _pool.Connection;
        }

        public static bool TryCreate(IXenConnection connection, out Alert alert)
        {
            var pool = Helpers.GetPoolOfOne(connection);
            var timestamp = DateTime.UtcNow - connection.ServerTimeOffset;

            if (pool.repositories.Count == 0)
            {
                alert = new YumRepoNotConfiguredAlert(pool, timestamp);
                return true;
            }

            alert = null;
            return false;
        }

        public override AlertPriority Priority => AlertPriority.Priority3;

        public override string AppliesTo => Helpers.GetName(_pool);

        public override string Description => Messages.ALERT_CDN_REPO_NOT_CONFIGURED_DESCRIPTION;

        public override Action FixLinkAction
        {
            get
            {
                return () =>
                {
                    using (var dialog = new ConfigUpdatesDialog())
                        dialog.ShowDialog(Program.MainWindow);
                };
            }
        }

        public override string FixLinkText => Messages.ALERT_CDN_REPO_NOT_CONFIGURED_ACTION_LINK;

        public override string HelpID => "TODO";

        public override string Title => string.Format(Messages.ALERT_CDN_REPO_NOT_CONFIGURED_TITLE, Connection.Name);
    }
}
