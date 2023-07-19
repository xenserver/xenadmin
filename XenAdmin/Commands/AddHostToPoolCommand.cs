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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// A Command for adding a host to a pool. This Command does not use the current selection for specifying the host and pool.
    /// The host and pool are set in the constructor for this class.
    /// </summary>
    internal class AddHostToPoolCommand : Command
    {
        private readonly List<Host> _hosts;
        private readonly Pool _pool;
        private readonly bool _confirm;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddHostToPoolCommand"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window interface. It can be found at Program.MainWindow.CommandInterface.</param>
        /// <param name="hosts">The hosts which are to be added to the pool.</param>
        /// <param name="pool">The pool the host should be added to.</param>
        /// <param name="confirm">if set to <c>true</c> a confirmation dialog is shown.</param>
        public AddHostToPoolCommand(IMainWindow mainWindow, IEnumerable<Host> hosts, Pool pool, bool confirm)
            : base(mainWindow)
        {
            Util.ThrowIfParameterNull(hosts, "hosts");
            Util.ThrowIfParameterNull(pool, "pool");
            _hosts = new List<Host>(hosts);
            _confirm = confirm;
            _pool = pool;
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            var reasons = new Dictionary<IXenObject, string>();
            foreach (Host host in _hosts)
            {
                PoolJoinRules.Reason reason = PoolJoinRules.CanJoinPool(host.Connection, _pool.Connection, true, true, _hosts.Count);
                if (reason != PoolJoinRules.Reason.Allowed)
                    reasons[host] = PoolJoinRules.ReasonMessage(reason);
            }

            if (reasons.Count > 0)
            {
                string title = Messages.ERROR_DIALOG_ADD_TO_POOL_TITLE;
                string text = string.Format(Messages.ERROR_DIALOG_ADD_TO_POOL_TEXT, Helpers.GetName(_pool).Ellipsise(500));

                new CommandErrorDialog(title, text, reasons).ShowDialog(Parent);
                return;
            }

            if (_confirm)
            {
                var msg = _hosts.Count > 1
                    ? string.Format(Messages.MAINWINDOW_CONFIRM_MOVE_TO_POOL_MULTIPLE, _pool.Name().Ellipsise(500))
                    : string.Format(Messages.MAINWINDOW_CONFIRM_MOVE_TO_POOL, _hosts[0].Name().Ellipsise(500), _pool.Name().Ellipsise(500));

                using (var dialog = new WarningDialog(msg, ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                    {WindowTitle = Messages.POOLCREATE_ADDING})
                {
                    if (dialog.ShowDialog(Parent ?? Program.MainWindow) == DialogResult.No)
                        return;
                }
            }

            if (_pool == null || _pool.Connection == null || !_pool.Connection.IsConnected)
            {
                string message = _hosts.Count == 1
                                     ? string.Format(Messages.ADD_HOST_TO_POOL_DISCONNECTED_POOL,
                                                     Helpers.GetName(_hosts[0]).Ellipsise(500), Helpers.GetName(_pool).Ellipsise(500))
                                     : string.Format(Messages.ADD_HOST_TO_POOL_DISCONNECTED_POOL_MULTIPLE,
                                                     Helpers.GetName(_pool).Ellipsise(500));

                using (var dlg = new ErrorDialog(message))
                    dlg.ShowDialog(Parent);

                return;
            }

            // Check supp packs and warn
            List<string> badSuppPacks = PoolJoinRules.HomogeneousSuppPacksDiffering(_hosts, _pool);

            if (!Program.RunInAutomatedTestMode && badSuppPacks.Count > 0)
            {
                string msg = string.Format(badSuppPacks.Count == 1 ? Messages.ADD_HOST_TO_POOL_SUPP_PACK : Messages.ADD_HOST_TO_POOL_SUPP_PACKS,
                    string.Join("\n", badSuppPacks));

                using (var dlg = new WarningDialog(msg,
                        new ThreeButtonDialog.TBDButton(Messages.PROCEED, DialogResult.OK, selected: false),
                        new ThreeButtonDialog.TBDButton(Messages.CANCEL, DialogResult.Cancel, selected: true))
                    {HelpNameSetter = "PoolJoinSuppPacks"})
                {
                    if (dlg.ShowDialog(Program.MainWindow) == DialogResult.Cancel)
                        return;
                }
            }

            // Are there any hosts which are forbidden from masking their CPUs for licensing reasons?
            // If so, we need to show upsell.
            Host coordinator = Helpers.GetCoordinator(_pool);
            if (null != _hosts.Find(host =>
                !PoolJoinRules.CompatibleCPUs(host, coordinator) &&
                Helpers.FeatureForbidden(host, Host.RestrictCpuMasking) &&
                !PoolJoinRules.FreeHostPaidCoordinator(host, coordinator, false)))  // in this case we can upgrade the license and then mask the CPU
            {
                UpsellDialog.ShowUpsellDialog(Messages.UPSELL_BLURB_CPUMASKING, Parent);
                return;
            }

            // Get permission for any fix-ups:
            // 1) Licensing free hosts
            // 2) CPU masking
            // 3) Ad configuration
            // 4) CPU feature levelling (Dundee or higher only)
            // (We already know that these things are fixable because we have been through CanJoinPool() above).
            
            if (!Program.RunInAutomatedTestMode)
            {
                var hosts1 = _hosts.FindAll(host => PoolJoinRules.FreeHostPaidCoordinator(host, coordinator, false));
                if (hosts1.Count > 0)
                {
                    string msg = string.Format(hosts1.Count == 1
                            ? Messages.ADD_HOST_TO_POOL_LICENSE_MESSAGE
                            : Messages.ADD_HOST_TO_POOL_LICENSE_MESSAGE_MULTIPLE,
                        string.Join("\n", hosts1.Select(h => h.Name())));

                    using (var dlg = new WarningDialog(msg, ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                        {HelpNameSetter = "PoolJoinRelicensing"})
                    {
                        if (dlg.ShowDialog(Program.MainWindow) == DialogResult.No)
                            return;
                    }
                }

                var hosts2 = _hosts.FindAll(host => !PoolJoinRules.CompatibleCPUs(host, coordinator));
                if (hosts2.Count > 0)
                {
                    string msg = string.Format(hosts2.Count == 1
                            ? Messages.ADD_HOST_TO_POOL_CPU_MASKING_MESSAGE
                            : Messages.ADD_HOST_TO_POOL_CPU_MASKING_MESSAGE_MULTIPLE,
                        string.Join("\n", hosts2.Select(h => h.Name())), BrandManager.ProductBrand);

                    using (var dlg = new WarningDialog(msg, ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                        {HelpNameSetter = "PoolJoinCpuMasking"})
                    {
                        if (dlg.ShowDialog(Program.MainWindow) == DialogResult.No)
                            return;
                    }
                }

                var hosts3 = _hosts.FindAll(host => !PoolJoinRules.CompatibleAdConfig(host, coordinator, false));
                if (hosts3.Count > 0)
                {
                    string msg = string.Format(hosts3.Count == 1
                            ? Messages.ADD_HOST_TO_POOL_AD_MESSAGE
                            : Messages.ADD_HOST_TO_POOL_AD_MESSAGE_MULTIPLE,
                        string.Join("\n", hosts3.Select(h => h.Name())));

                    using (var dlg = new WarningDialog(msg, ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                        {HelpNameSetter = "PoolJoinAdConfiguring"})
                    {
                        if (dlg.ShowDialog(Program.MainWindow) == DialogResult.No)
                            return;
                    }
                }
            }

            if (!HelpersGUI.GetPermissionForCpuFeatureLevelling(_hosts, _pool))
                return;

            MainWindowCommandInterface.SelectObjectInTree(_pool);

            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (Host host in _hosts)
            {
                string opaque_ref = host.opaque_ref;
                var action = new AddHostToPoolAction(_pool, host, GetAdPrompt,
                    (licenseFailures, exceptionMessage) =>
                    {
                        if (licenseFailures.Count > 0)
                        {
                            Program.Invoke(Program.MainWindow, () =>
                            {
                                using (var dlg = new CommandErrorDialog(Messages.LICENSE_ERROR_TITLE, exceptionMessage,
                                    licenseFailures.ToDictionary<LicenseFailure, IXenObject, string>(f => f.Host, f => f.AlertText)))
                                {
                                    dlg.ShowDialog(Program.MainWindow);
                                }
                            });
                        }
                    });
                action.Completed += s => XenAdminConfigManager.Provider.ShowObject(opaque_ref);
                actions.Add(action);

                // hide connection. If the action fails, re-show it.
                XenAdminConfigManager.Provider.HideObject(opaque_ref);
            }

            RunMultipleActions(actions, string.Format(Messages.ADDING_SERVERS_TO_POOL, _pool.Name()), Messages.POOLCREATE_ADDING, Messages.POOLCREATE_ADDED, true);
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            if (_hosts.Count > 0)
            {
                foreach (Host host in _hosts)
                {
                    // only allowed to add standalone hosts.
                    if (Helpers.GetPool(host.Connection) != null || Host.RestrictPooling(host))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static PoolAbstractAction.AdUserAndPassword GetAdPrompt(Host poolCoordinator)
        {
            AdPasswordPrompt adPrompt = new AdPasswordPrompt(true, poolCoordinator.external_auth_service_name);

            Program.Invoke(Program.MainWindow, delegate
                                                   {
                                                       if (adPrompt.ShowDialog(Program.MainWindow) == DialogResult.Cancel)
                                                           throw new CancelledException();
                                                   });
            return new PoolAbstractAction.AdUserAndPassword(adPrompt.Username, adPrompt.Password);
        }

        public static bool EnableNtolDialog(Pool pool, Host host, long currentNtol, long max)
        {
            bool doit = false;
            Program.Invoke(Program.MainWindow, delegate()
            {
                string poolName = Helpers.GetName(pool).Ellipsise(500);
                string hostName = Helpers.GetName(host).Ellipsise(500);
                string msg = string.Format(Messages.HA_HOST_ENABLE_NTOL_RAISE_QUERY, poolName, hostName, currentNtol, max);
                using (var dlg = new NoIconDialog(msg,
                        ThreeButtonDialog.ButtonYes,
                        new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, selected: true))
                    {WindowTitle = Messages.HIGH_AVAILABILITY})
                {
                    if (dlg.ShowDialog(Program.MainWindow) == DialogResult.Yes)
                    {
                        doit = true;
                    }
                }
            });
            return doit;
        }

        public static bool NtolDialog(HostAbstractAction action, Pool pool, long currentNtol, long targetNtol)
        {
            bool cancel = false;
            Program.Invoke(Program.MainWindow, delegate()
            {
                string poolName = Helpers.GetName(pool).Ellipsise(500);
                string hostName = Helpers.GetName(action.Host).Ellipsise(500);

                string msg;
                if (targetNtol == 0)
                {
                    string f;
                    if (action is EvacuateHostAction)
                    {
                        f = Messages.HA_HOST_DISABLE_NTOL_ZERO;
                    }
                    else if (action is RebootHostAction)
                    {
                        f = Messages.HA_HOST_REBOOT_NTOL_ZERO;
                    }
                    else
                    {
                        f = Messages.HA_HOST_SHUTDOWN_NTOL_ZERO;
                    }

                    msg = string.Format(f, poolName, hostName);
                }
                else
                {
                    string f;
                    if (action is EvacuateHostAction)
                    {
                        f = Messages.HA_HOST_DISABLE_NTOL_DROP;
                    }
                    else if (action is RebootHostAction)
                    {
                        f = Messages.HA_HOST_REBOOT_NTOL_DROP;
                    }
                    else
                    {
                        f = Messages.HA_HOST_SHUTDOWN_NTOL_DROP;
                    }

                    msg = string.Format(f, poolName, currentNtol, hostName, targetNtol);
                }

                using (var dlg = new WarningDialog(msg, ThreeButtonDialog.ButtonYes,
                        new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, selected: true)
                        ){WindowTitle = Messages.HIGH_AVAILABILITY})
                {
                    if (dlg.ShowDialog(Program.MainWindow) == DialogResult.No)
                    {
                        cancel = true;
                    }
                }

            });
            return cancel;
        }

    }
}
