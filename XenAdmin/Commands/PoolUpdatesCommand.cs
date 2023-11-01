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


using System.Collections.Generic;
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs.ServerUpdates;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Commands
{
    internal class PoolUpdatesCommand : Command
    {
        public PoolUpdatesCommand()
        {
        }

        public PoolUpdatesCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public PoolUpdatesCommand(IMainWindow mainWindow, IXenConnection connection)
            : base(mainWindow, Helpers.GetPoolOfOne(connection))
        {
        }

        public PoolUpdatesCommand(IMainWindow mainWindow, Pool pool)
            : base(mainWindow, pool)
        {
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return new ConfigUpdatesCommand(MainWindowCommandInterface, selection).CanRun() ||
                   new SynchronizeCommand(MainWindowCommandInterface, selection).CanRun();
        }

        public override string MenuText => Messages.UPDATES_MENU_ITEM;
    }


    internal class ConfigUpdatesCommand : Command
    {
        public ConfigUpdatesCommand()
        {
        }

        public ConfigUpdatesCommand(IMainWindow mainWindow)
            : base(mainWindow)
        {
        }

        public ConfigUpdatesCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            using (var dialog = new ConfigUpdatesDialog())
                dialog.ShowDialog(Parent);
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            foreach (var item in selection)
            {
                if ((item.XenObject is Pool || item.XenObject is Host && item.PoolAncestor == null) &&
                    Helpers.CloudOrGreater(item.Connection))
                    continue;

                return false;
            }

            return true;
        }

        public override string ContextMenuText => Messages.CONFIG_UPDATES_MENU_ITEM;
    }


    internal class SynchronizeCommand : Command
    {
        public SynchronizeCommand()
        {
        }

        public SynchronizeCommand(IMainWindow mainWindow)
            : base(mainWindow)
        {
        }

        public SynchronizeCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            var pools = (from SelectedItem item in selection
                let pool = Helpers.GetPoolOfOne(item.Connection)
                select pool).Distinct().ToList();

            foreach (var pool in pools)
            {
                var syncAction = Updates.CreateSyncWithCdnAction(pool);
                syncAction.RunAsync();
            }
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            var syncActions = ConnectionsManager.History.Where(a =>
                !a.IsCompleted && a is SyncWithCdnAction syncA && !syncA.Cancelled).ToList();

            foreach (var item in selection)
            {
                if (Helpers.CloudOrGreater(item.Connection) && syncActions.All(a => a.Connection != item.Connection))
                {
                    Pool pool;

                    switch (item.XenObject)
                    {
                        case Pool selPool:
                            pool = selPool;
                            break;
                        case Host _ when item.PoolAncestor == null:
                            pool = Helpers.GetPoolOfOne(item.Connection);
                            break;
                        default:
                            pool = null;
                            break;
                    }

                    if (pool != null && pool.repositories.Count > 0 &&
                        pool.allowed_operations.Contains(pool_allowed_operations.sync_updates))
                        continue;
                }

                return false;
            }

            return true;
        }

        public override string ContextMenuText => Messages.YUM_REPO_SYNC_MENU_ITEM;
    }
}
