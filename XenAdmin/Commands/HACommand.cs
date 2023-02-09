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
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Wizards;


namespace XenAdmin.Commands
{
    /// <summary>
    /// IF HA is not enabled, it launches the HA wizard, otherwise the HA config dialog
    /// </summary>
    internal class HAConfigureCommand : HACommand
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private RbacMethodList HA_PERMISSION_CHECKS = new RbacMethodList(
            "pool.set_ha_host_failures_to_tolerate",
            "pool.sync_database",
            "vm.set_ha_restart_priority",
            "pool.ha_compute_hypothetical_max_host_failures_to_tolerate"
        );

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public HAConfigureCommand()
        {
        }

        public HAConfigureCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public HAConfigureCommand(IMainWindow mainWindow, IXenConnection connection)
            : base(mainWindow, Helpers.GetPoolOfOne(connection))
        {
        }

        public HAConfigureCommand(IMainWindow mainWindow, Pool pool)
            : base(mainWindow, pool)
        {
        }

        private void Run(IXenConnection connection)
        {
            if (connection == null)
                return;

            Pool pool = Helpers.GetPool(connection);
            if (pool == null)
                return;

            if (Helpers.FeatureForbidden(pool, Host.RestrictHA))
            {
                UpsellDialog.ShowUpsellDialog(Messages.UPSELL_BLURB_HA, Parent);
            }
            else if (pool.ha_enabled)
            {
                if (pool.ha_statefiles.All(sf => pool.Connection.Resolve(new XenRef<VDI>(sf)) == null))//empty gives true, which is correct
                {
                    log.ErrorFormat("Cannot resolve HA statefile VDI (pool {0} has {1} statefiles).",
                        pool.Name(), pool.ha_statefiles.Length);

                    using (var dlg = new ErrorDialog(string.Format(Messages.HA_CONFIGURE_NO_STATEFILE, Helpers.GetName(pool).Ellipsise(30)),
                        ThreeButtonDialog.ButtonOK)
                    {
                        WindowTitle = Messages.CONFIGURE_HA
                    })
                    {
                        dlg.ShowDialog(Program.MainWindow);
                    }
                }
                else if (!Role.CanPerform(HA_PERMISSION_CHECKS, pool.Connection, out _))
                {
                    var msg = string.Format(Messages.RBAC_HA_CONFIGURE_WARNING,
                        Role.FriendlyCSVRoleList(Role.ValidRoleList(HA_PERMISSION_CHECKS, pool.Connection)),
                        Role.FriendlyCSVRoleList(pool.Connection.Session.Roles));

                    using (var dlg = new ErrorDialog(msg))
                        dlg.ShowDialog(Parent);
                }
                else
                {
                    MainWindowCommandInterface.ShowPerConnectionWizard(connection, new EditVmHaPrioritiesDialog(pool));
                }
            }
            else
            {
                MainWindowCommandInterface.ShowPerConnectionWizard(connection, new HAWizard(pool));
            }
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            Run(selection[0].Connection);
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return CanRunHACommand(selection);
        }

        protected override string GetCantRunReasonCore(IXenObject item)
        {
            var reason = base.GetCantRunReasonCore(item);
            if (!string.IsNullOrEmpty(reason) && reason != Messages.UNKNOWN)
                return reason;

            Pool pool = item == null ? null : Helpers.GetPool(item.Connection);

            if (pool != null && !pool.Connection.Cache.Hosts.Any(Host.RestrictPoolSecretRotation) && pool.is_psr_pending)
                return Messages.ROTATE_POOL_SECRET_PENDING_HA;

            return Messages.UNKNOWN;
        }

        protected override bool CanRun(Pool pool)
        {
            return pool.Connection.Cache.Hosts.Any(Host.RestrictPoolSecretRotation) || !pool.is_psr_pending;
        }

        public override string MenuText => Messages.CONFIGURE_HA_ELLIPSIS;
    }

    internal class HADisableCommand : HACommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public HADisableCommand()
        {
        }

        public HADisableCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public HADisableCommand(IMainWindow mainWindow, IXenConnection connection)
            : base(mainWindow, Helpers.GetPoolOfOne(connection))
        {
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            Pool pool = selection.Count == 1 ? selection[0].PoolAncestor : null;
            if (pool == null || !pool.ha_enabled)
                return;

            if (pool.ha_statefiles.All(sf => pool.Connection.Resolve(new XenRef<VDI>(sf)) == null)) //empty gives true, which is correct
            {
                using (var dlg = new ErrorDialog(string.Format(Messages.HA_DISABLE_NO_STATEFILE,
                        Helpers.GetName(pool).Ellipsise(30)),
                    ThreeButtonDialog.ButtonOK)
                {
                    WindowTitle = Messages.DISABLE_HA
                })
                {
                    dlg.ShowDialog(Parent);
                    return;
                }
            }

            // Confirm the user wants to disable HA
            using (var dlg = new NoIconDialog(string.Format(Messages.HA_DISABLE_QUERY, 
                    Helpers.GetName(pool).Ellipsise(30)),
                ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
            {
                HelpNameSetter = "HADisable",
                WindowTitle = Messages.DISABLE_HA
            })
            {
                if (dlg.ShowDialog(Parent) != DialogResult.Yes)
                    return;
            }

            var action = new DisableHAAction(pool);
            // We will need to re-enable buttons when the action completes
            action.Completed += Program.MainWindow.action_Completed;
            action.RunAsync();
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return CanRunHACommand(selection);
        }

        protected override bool CanRun(Pool pool)
        {
            return pool.ha_enabled;
        }

        public override string MenuText => Messages.DISABLE_HA_HOTKEY;
    }


    internal class HACommand : Command
    {
        public HACommand()
        {
        }

        public HACommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public HACommand(IMainWindow mainWindow, IXenConnection connection)
            : base(mainWindow, Helpers.GetPoolOfOne(connection))
        {
        }

        public HACommand(IMainWindow mainWindow, Pool pool)
            : base(mainWindow, pool)
        {
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return new HAConfigureCommand(MainWindowCommandInterface, selection).CanRun() ||
                   new HADisableCommand(MainWindowCommandInterface, selection).CanRun();
        }

        protected override string GetCantRunReasonCore(IXenObject item)
        {
            Pool pool = item == null ? null : Helpers.GetPoolOfOne(item.Connection);

            if (pool == null)
                return string.Format(Messages.POOL_GONE, BrandManager.BrandConsole);

            if (!pool.IsVisible())
                return Messages.HA_STANDALONE_SERVER;
 
            Host coordinator = Helpers.GetCoordinator(pool.Connection);
            if (coordinator == null)
                return string.Format(Messages.POOL_COORDINATOR_GONE, BrandManager.BrandConsole);

            if (pool.Locked)
                return Messages.POOL_EDIT_IN_PROGRESS;

            var action = HelpersGUI.FindActiveHaAction(pool.Connection);
            if (action != null)
                return string.Format(action is EnableHAAction ? Messages.HA_PAGE_ENABLING : Messages.HA_PAGE_DISABLING,
                    Helpers.GetName(pool.Connection));

            return Messages.UNKNOWN;
        }

        protected bool CanRunHACommand(SelectedItemCollection selection)
        {
            if (selection.Count != 1)
                return false;

            Pool pool = selection[0].PoolAncestor;

            if (pool == null || pool.Locked ||
                pool.Connection == null || !pool.Connection.IsConnected ||
                Helpers.GetCoordinator(pool.Connection) == null ||
                HelpersGUI.FindActiveHaAction(pool.Connection) != null)
                return false;

            return CanRun(pool);
        }

        protected virtual bool CanRun(Pool pool)
        {
            return true;
        }

        public override string MenuText => Messages.MAINWINDOW_HIGH_AVAILABILITY;
    }
}
