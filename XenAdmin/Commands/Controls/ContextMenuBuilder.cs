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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Model;
using XenAdmin.Plugins;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// A class for building up the context menu for XenObjects.
    /// </summary>
    internal class ContextMenuBuilder
    {
        private readonly PluginManager _pluginManager;
        private readonly IMainWindow _mainWindow;
        private static readonly ReadOnlyCollection<Builder> Builders;

        static ContextMenuBuilder()
        {
            var list = new List<Builder>
            {
                new MixedPoolsAndStandaloneHosts(),
                new MultiplePools(),
                new MultipleDifferentXenObjectTypes(),
                new MultipleSRs(),
                new SingleVDI(),
                new MultipleVDI(),
                new SingleNetwork(),
                new DisconnectedHosts(),
                new MixedVMsAndTemplates(),
                new MultipleAliveHosts(),
                new SingleAliveHostInPool(),
                new SingleAliveStandaloneHost(),
                new MultipleHostsSomeDeadSomeAlive(),
                new DeadHosts(),
                new SinglePool(),
                new SingleSnapshot(),
                new SingleTemplate(),
                new SingleVmAppliance(),
                new MultipleVmAppliance(),
                new SingleVM(),
                new SingleSR(),
                new SingleFolder(),
                new MultipleTemplates(),
                new MultipleSnapshots(),
                new MultipleVMsInPool(),
                new MultipleVMsOverMultiplePools(),
                new MultipleFolders(),
                new SingleTag(),
                new MultipleTags(),
                new SingleDockerContainer(),
                new MultipleDockerContainers()
            };

            Builders = new ReadOnlyCollection<Builder>(list);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenuBuilder"/> class.
        /// </summary>
        /// <param name="pluginManager">The plugin manager. This can be found on MainWindow.</param>
        /// <param name="mainWindow">The main window command interface. This can be found on mainwindow.</param>
        public ContextMenuBuilder(PluginManager pluginManager, IMainWindow mainWindow)
        {
            _pluginManager = pluginManager ?? throw new ArgumentNullException(nameof(pluginManager));
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
        }

        /// <summary>
        /// Builds the context menu for the specified XenObject.
        /// </summary>
        /// <param name="xenObject">The xen object for which the context menu items are required.</param>
        /// <returns>The context menu items.</returns>
        public ToolStripItem[] Build(IXenObject xenObject)
        {
            return Build(new SelectedItem(xenObject));
        }

        /// <summary>
        /// Builds the context menu for the specified selection.
        /// </summary>
        /// <param name="selection">The selection for which the context menu items are required.</param>
        /// <returns>The context menu items.</returns>
        public ToolStripItem[] Build(SelectedItem selection)
        {
            return Build(new[] { selection });
        }

        /// <summary>
        /// Builds the context menu for the specified selection.
        /// </summary>
        /// <param name="selection">The selection for which the context menu items are required.</param>
        /// <returns>The context menu items.</returns>
        public ToolStripItem[] Build(IEnumerable<SelectedItem> selection)
        {
            var selectionList = new SelectedItemCollection(selection ?? throw new ArgumentNullException(nameof(selection)));

            foreach (Builder builder in Builders)
            {
                if (builder.IsValid(selectionList))
                {
                    ContextMenuItemCollection items = new ContextMenuItemCollection(_mainWindow, _pluginManager);
                    builder.Build(_mainWindow, selectionList, items);
                    CheckAccessKeys(items);

                    items.RemoveInvalidSeparators();

                    return items.ToArray();
                }
            }

            return Array.Empty<ToolStripItem>();
        }

        [Conditional("DEBUG")]
        private void CheckAccessKeys(ContextMenuItemCollection items)
        {
            var usedKeys = new Dictionary<string, List<string>>();

            foreach (ToolStripItem item in items)
            {
                string text = item.Text.Replace("&&", "");

                int index = text.IndexOf("&");
                if (index >= 0)
                {
                    string c = text[index + 1].ToString().ToLower();

                    if (usedKeys.ContainsKey(c))
                        usedKeys[c].Add(text);
                    else
                        usedKeys[c] = new List<string> { text };
                }
            }

            if (usedKeys.Any(kvp => kvp.Value.Count > 1))
                Debug.Fail("Found duplicate access key. Here are the used ones:\n" +
                           string.Join("\n", usedKeys.Select(kvp => $"{kvp.Key} => {string.Join(", ", kvp.Value)}")));
        }

        #region Nested Classes

        private abstract class Builder
        {
            public abstract void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items);
            public abstract bool IsValid(SelectedItemCollection selection);
        }

        private class MixedPoolsAndStandaloneHosts : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new DisconnectHostsAndPoolsCommand(mainWindow, selection), true);
                items.AddIfEnabled(new ReconnectHostCommand(mainWindow, selection), true);

                var updatesItem = new CommandToolStripMenuItem(new PoolUpdatesCommand(mainWindow, selection), true);
                if (updatesItem.Command.CanRun())
                {
                    updatesItem.DropDownItems.Add(new CommandToolStripMenuItem(new ConfigUpdatesCommand(mainWindow, selection), true));
                    updatesItem.DropDownItems.Add(new CommandToolStripMenuItem(new SynchronizeCommand(mainWindow, selection), true));
                    items.Add(updatesItem);
                }

                items.AddIfEnabled(new EditTagsCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                bool containsPool = false;
                bool containsHost = false;

                if (selection.Count > 1)
                {
                    foreach (SelectedItem item in selection)
                    {
                        if (item.XenObject is Pool)
                        {
                            containsPool = true;
                        }
                        else if (item.XenObject is Host && item.PoolAncestor == null)
                        {
                            containsHost = true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return containsPool && containsHost;
            }
        }

        private class MultiplePools : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new DisconnectPoolCommand(mainWindow, selection), true);

                var updatesItem = new CommandToolStripMenuItem(new PoolUpdatesCommand(mainWindow, selection), true);
                if (updatesItem.Command.CanRun())
                {
                    updatesItem.DropDownItems.Add(new CommandToolStripMenuItem(new ConfigUpdatesCommand(mainWindow, selection), true));
                    updatesItem.DropDownItems.Add(new CommandToolStripMenuItem(new SynchronizeCommand(mainWindow, selection), true));
                    items.Add(updatesItem);
                }

                items.AddIfEnabled(new EditTagsCommand(mainWindow, selection));
                items.AddPluginItems(PluginContextMenu.pool, selection);
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                if (selection.Count > 1)
                {
                    foreach (SelectedItem item in selection)
                    {
                        Pool pool = item.XenObject as Pool;

                        if (pool == null)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        private class MultipleDifferentXenObjectTypes : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new EditTagsCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                List<string> types = new List<string>();

                if (selection.AllItemsAre<IXenObject>())
                {
                    foreach (SelectedItem item in selection)
                    {
                        string name = item.XenObject.GetType().Name;

                        VM vm = item.XenObject as VM;

                        if (vm != null && vm.is_a_template)
                        {
                            name = vm.is_a_snapshot ? "snapshot" : "template";
                        }

                        if (!types.Contains(name))
                        {
                            types.Add(name);
                        }
                    }
                }

                if (types.Count > 1)
                {
                    // if types only contains a mix of vms and templates then don't use this Builder. MixedVMsAndTemplates should be used instead.
                    types.Remove("VM");
                    types.Remove("template");
                    return types.Count > 0;
                }
                return false;
            }
        }

        private class MultipleSRs : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new RepairSRCommand(mainWindow, selection));
                items.AddIfEnabled(new DetachSRCommand(mainWindow, selection));
                items.AddIfEnabled(new ForgetSRCommand(mainWindow, selection));
                items.AddIfEnabled(new DestroySRCommand(mainWindow, selection));
                items.AddIfEnabled(new EditTagsCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                return selection.Count > 1 && selection.AllItemsAre<SR>();
            }
        }

        private class SingleVDI : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.Add(MoveVirtualDiskDialog.MoveMigrateCommand(mainWindow, selection));
                // Default behaviour of this command is very conservative, they wont be able to delete if there are multi vbds,
                // or if any of the vbds are plugged on top of the other constraints.
                items.Add(new DeleteVirtualDiskCommand(mainWindow, selection));
                items.AddSeparator();
                items.Add(new PropertiesCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                return selection.Count == 1 && selection[0].XenObject is VDI;
            }
        }

        private class MultipleVDI : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.Add(MoveVirtualDiskDialog.MoveMigrateCommand(mainWindow, selection));
                // Default behaviour of this command is very conservative, they wont be able to delete if there are multi vbds,
                // or if any of the vbds are plugged on top of the other constraints.
                items.Add(new DeleteVirtualDiskCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                return selection.AllItemsAre<VDI>();
            }
        }

        private class SingleNetwork : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.Add(new PropertiesCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                return selection.ContainsOneItemOfType<XenAPI.Network>() &&
                    !(selection.FirstAsXenObject as XenAPI.Network).IsGuestInstallerNetwork();
                // CA-218956 - Expose HIMN when showing hidden objects
                // HIMN should not be editable
            }
        }

        private class DisconnectedHosts : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                List<SelectedItem> inProgressItems = new List<SelectedItem>();

                foreach (SelectedItem item in selection)
                {
                    if (item.Connection != null && !item.Connection.IsConnected && item.Connection.InProgress)
                    {
                        inProgressItems.Add(item);
                    }
                }

                if (inProgressItems.Count > 0)
                {
                    items.Add(new CancelHostConnectionCommand(mainWindow, inProgressItems));
                }
                else
                {
                    items.AddIfEnabled(new DisconnectHostsAndPoolsCommand(mainWindow, selection), true);
                    items.Add(new ReconnectHostCommand(mainWindow, selection), true);
                    items.Add(new ForgetSavedPasswordCommand(mainWindow, selection));
                    items.Add(new RemoveHostCommand(mainWindow, selection));
                    items.AddIfEnabled(new RestartToolstackCommand(mainWindow, selection));
                }
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                foreach (SelectedItem item in selection)
                {
                    if (item.Connection != null && !item.Connection.IsConnected)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private class MixedVMsAndTemplates : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new DeleteVMsAndTemplatesCommand(mainWindow, selection));
                items.AddIfEnabled(new EditTagsCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                if (selection.Count > 1)
                {
                    bool vmFound = false, templateFound = false;

                    foreach (SelectedItem item in selection)
                    {
                        VM vm = item.XenObject as VM;

                        if (vm != null)
                        {
                            if (vm.is_a_template && !vm.is_a_snapshot)
                            {
                                templateFound = true;
                            }
                            else
                            {
                                vmFound = true;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }

                    return vmFound && templateFound;
                }
                return false;
            }
        }

        private class MultipleAliveHosts : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new AddSelectedHostToPoolToolStripMenuItem(mainWindow, selection, true));
                items.AddIfEnabled(new DisconnectHostCommand(mainWindow, selection));
                items.AddIfEnabled(new RebootHostCommand(mainWindow, selection));
                items.AddIfEnabled(new ShutDownHostCommand(mainWindow, selection));
                items.AddIfEnabled(new RestartToolstackCommand(mainWindow, selection));
                items.AddSeparator();
                items.AddIfEnabled(new EditTagsCommand(mainWindow, selection));
                items.AddSeparator();
                items.AddPluginItems(PluginContextMenu.server, selection);
                items.AddSeparator();
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                if (selection.Count > 1)
                {
                    foreach (SelectedItem item in selection)
                    {
                        Host host = item.XenObject as Host;

                        if (host == null || !host.IsLive())
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        private class SingleAliveHostInPool : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new NewVMCommand(mainWindow, selection));
                items.AddIfEnabled(new NewSRCommand(mainWindow, selection));
				items.AddIfEnabled(new ImportCommand(mainWindow, selection));
                items.AddSeparator();

                var certItem = new CommandToolStripMenuItem(new CertificateCommand(mainWindow, selection), true);
                if (certItem.Command.CanRun())
                {
                    certItem.DropDownItems.Add(new CommandToolStripMenuItem(new InstallCertificateCommand(mainWindow, selection), true));
                    certItem.DropDownItems.Add(new CommandToolStripMenuItem(new ResetCertificateCommand(mainWindow, selection), true));
                    items.Add(certItem);
                }

                items.AddIfEnabled(new HostMaintenanceModeCommand(mainWindow, selection));
                items.AddSeparator();

                items.AddIfEnabled(new RebootHostCommand(mainWindow, selection));
                items.AddIfEnabled(new ShutDownHostCommand(mainWindow, selection));
                items.AddIfEnabled(new RestartToolstackCommand(mainWindow, selection));
                items.AddSeparator();
                                
                items.AddIfEnabled(new RemoveHostCrashDumpsCommand(mainWindow, selection));

                var cmd = new RemoveHostFromPoolCommand(mainWindow, selection);
                if (cmd.CanRun())
                {
                    items.AddSeparator();
                    items.Add(cmd);
                }

                items.AddPluginItems(PluginContextMenu.server, selection);
                items.AddSeparator();
                items.AddIfEnabled(new HostPropertiesCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                if (selection.Count == 1 && selection[0].PoolAncestor != null)
                {
                    Host host = selection[0].XenObject as Host;

                    return host != null && host.IsLive();
                }
                return false;
            }
        }

        private class SingleAliveStandaloneHost : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new NewVMCommand(mainWindow, selection));
                items.AddIfEnabled(new NewSRCommand(mainWindow, selection));
				items.AddIfEnabled(new ImportCommand(mainWindow, selection));
                items.AddSeparator();

                items.AddIfEnabled(new AddSelectedHostToPoolToolStripMenuItem(mainWindow, selection, true));
                items.AddSeparator();
                
                var certItem = new CommandToolStripMenuItem(new CertificateCommand(mainWindow, selection), true);
                if (certItem.Command.CanRun())
                {
                    certItem.DropDownItems.Add(new CommandToolStripMenuItem(new InstallCertificateCommand(mainWindow, selection), true));
                    certItem.DropDownItems.Add(new CommandToolStripMenuItem(new ResetCertificateCommand(mainWindow, selection), true));
                    items.Add(certItem);
                }

                items.AddIfEnabled(new HostMaintenanceModeCommand(mainWindow, selection));

                var updatesItem = new CommandToolStripMenuItem(new PoolUpdatesCommand(mainWindow, selection), true);
                if (updatesItem.Command.CanRun())
                {
                    updatesItem.DropDownItems.Add(new CommandToolStripMenuItem(new ConfigUpdatesCommand(mainWindow, selection), true));
                    updatesItem.DropDownItems.Add(new CommandToolStripMenuItem(new SynchronizeCommand(mainWindow, selection), true));
                    items.Add(updatesItem);
                }
                items.AddSeparator();

                items.AddIfEnabled(new RebootHostCommand(mainWindow, selection));
                items.AddIfEnabled(new ShutDownHostCommand(mainWindow, selection));
                items.AddIfEnabled(new PowerOnHostCommand(mainWindow, selection));
                items.AddIfEnabled(new RestartToolstackCommand(mainWindow, selection));
                items.AddSeparator();
                items.AddIfEnabled(new RemoveHostCrashDumpsCommand(mainWindow, selection));
                items.AddSeparator();

                items.AddIfEnabled(new DisconnectHostCommand(mainWindow, selection));
                items.AddIfEnabled(new HostReconnectAsCommand(mainWindow, selection));
                items.AddPluginItems(PluginContextMenu.server, selection);
                items.AddSeparator();

                items.AddIfEnabled(new HostPropertiesCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                if (selection.Count == 1 && selection[0].PoolAncestor == null)
                {
                    Host host = selection[0].XenObject as Host;
                    return host != null && host.IsLive();
                }
                return false;
            }
        }

        private class MultipleHostsSomeDeadSomeAlive : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new ShutDownHostCommand(mainWindow, selection));
                items.AddIfEnabled(new PowerOnHostCommand(mainWindow, selection));
                items.AddIfEnabled(new RestartToolstackCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                bool foundAlive = false;
                bool foundDead = false;

                foreach (SelectedItem item in selection)
                {
                    Host host = item.XenObject as Host;

                    if (host == null)
                    {
                        return false;
                    }

                    bool isHostLive = host.IsLive();

                    foundAlive |= isHostLive;
                    foundDead |= !isHostLive;
                }
                return foundAlive && foundDead;
            }
        }

        private class DeadHosts : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new PowerOnHostCommand(mainWindow, selection));
                items.AddIfEnabled(new DestroyHostCommand(mainWindow, selection));

                if (selection.Count == 1)
                {
                    items.AddSeparator();
                    items.AddIfEnabled(new HostPropertiesCommand(mainWindow, selection));
                }
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                if (selection.Count > 0)
                {
                    foreach (SelectedItem item in selection)
                    {
                        Host host = item.XenObject as Host;

                        if (host == null || host.IsLive())
                        {
                            return false;
                        }
                    }
                    return true;
                }

                return false;
            }
        }

        private class SinglePool : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.Add(new NewVMCommand(mainWindow, selection));
                items.Add(new NewSRCommand(mainWindow, selection));
				items.Add(new ImportCommand(mainWindow, selection));
                items.AddSeparator();

                var haItem = new CommandToolStripMenuItem(new HACommand(mainWindow, selection), true);
                if (haItem.Command.CanRun())
                {
                    haItem.DropDownItems.Add(new CommandToolStripMenuItem(new HAConfigureCommand(mainWindow, selection), true));
                    haItem.DropDownItems.Add(new CommandToolStripMenuItem(new HADisableCommand(mainWindow, selection), true));
                    items.Add(haItem);
                }

                items.AddIfEnabled(new VMGroupCommand<VMSS>(mainWindow, selection));
                items.AddIfEnabled(new VMGroupCommand<VM_appliance>(mainWindow, selection));

                var drItem = new CommandToolStripMenuItem(new DRCommand(mainWindow, selection), true);
                if (drItem.Command.CanRun())
                {
                    drItem.DropDownItems.Add(new CommandToolStripMenuItem(new DRConfigureCommand(mainWindow, selection), true));
                    drItem.DropDownItems.Add(new CommandToolStripMenuItem(new DisasterRecoveryCommand(mainWindow, selection), true));
                    items.Add(drItem);
                }

                items.AddIfEnabled(new LaunchConversionManagerCommand(mainWindow, selection));

                var updatesItem = new CommandToolStripMenuItem(new PoolUpdatesCommand(mainWindow, selection), true);
                if (updatesItem.Command.CanRun())
                {
                    updatesItem.DropDownItems.Add(new CommandToolStripMenuItem(new ConfigUpdatesCommand(mainWindow, selection), true));
                    updatesItem.DropDownItems.Add(new CommandToolStripMenuItem(new SynchronizeCommand(mainWindow, selection), true));
                    items.Add(updatesItem);
                }

                if (selection.FirstAsXenObject is Pool pool && !pool.IsPoolFullyUpgraded())
                    items.Add(new RollingUpgradeCommand(mainWindow));

                items.AddSeparator();
                items.Add(new AddHostToSelectedPoolToolStripMenuItem(mainWindow, selection, true));
                items.Add(new DisconnectPoolCommand(mainWindow, selection));
                items.Add(new PoolReconnectAsCommand(mainWindow, selection));
                items.AddSeparator();
                items.AddPluginItems(PluginContextMenu.pool, selection);
                items.AddSeparator();
                items.Add(new PoolPropertiesCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                return selection.ContainsOneItemOfType<Pool>();
            }
        }

        private class SingleSnapshot : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new NewVMFromSnapshotCommand(mainWindow, selection));
                items.AddIfEnabled(new NewTemplateFromSnapshotCommand(mainWindow, selection));
                items.AddIfEnabled(new ExportSnapshotAsTemplateCommand(mainWindow, selection));
                items.AddIfEnabled(new DeleteSnapshotCommand(mainWindow, selection));
                items.AddPluginItems(PluginContextMenu.vm, selection);
                items.AddSeparator();
                items.AddIfEnabled(new PropertiesCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                if (selection.Count == 1)
                {
                    VM vm = selection[0].XenObject as VM;

                    return vm != null && vm.is_a_snapshot;
                }
                return false;
            }
        }

        private class SingleTemplate : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new NewVMFromTemplateCommand(mainWindow, selection), true);
                items.AddIfEnabled(new InstantVMFromTemplateCommand(mainWindow, selection));
                items.AddSeparator();

                items.AddIfEnabled(new ExportTemplateCommand(mainWindow, selection));
                items.AddIfEnabled(new CopyTemplateCommand(mainWindow, selection));
                items.AddSeparator();

                items.AddIfEnabled(new DeleteTemplateCommand(mainWindow, selection));

                items.AddPluginItems(PluginContextMenu.template, selection);
                items.AddSeparator();




                items.AddIfEnabled(new PropertiesCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                if (selection.Count == 1)
                {
                    VM vm = selection[0].XenObject as VM;

                    return vm != null && vm.is_a_template;
                }
                return false;
            }
        }

		private class SingleVmAppliance : Builder
		{
			public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
			{
                items.AddIfEnabled(new VappStartCommand(mainWindow,selection));
                items.AddIfEnabled(new VappShutDownCommand(mainWindow, selection));
                items.AddSeparator();
				items.AddIfEnabled(new ExportCommand(mainWindow, selection));
				items.AddSeparator();
				items.AddIfEnabled(new PropertiesCommand(mainWindow, selection));
			}

			public override bool IsValid(SelectedItemCollection selection)
			{
				if (selection.Count == 1)
				{
					VM_appliance app = selection[0].XenObject as VM_appliance;
					return app != null;
				}
				return false;
			}
		}

        private class MultipleVmAppliance : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new VappStartCommand(mainWindow, selection));
                items.AddIfEnabled(new VappShutDownCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                return selection.AllItemsAre<VM_appliance>();
            }
        }

        private class SingleVM : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                VM vm = (VM)selection[0].XenObject;

                items.AddIfEnabled(new ShutDownVMCommand(mainWindow, selection));
                items.AddIfEnabled(new StartVMCommand(mainWindow, selection), vm.power_state == vm_power_state.Halted);
                items.AddIfEnabled(new ResumeVMCommand(mainWindow, selection));
                items.AddIfEnabled(new SuspendVMCommand(mainWindow, selection));
                items.AddIfEnabled(new RebootVMCommand(mainWindow, selection));
                items.AddSeparator();

                items.AddIfEnabled(new ForceVMShutDownCommand(mainWindow, selection));
                items.AddIfEnabled(new ForceVMRebootCommand(mainWindow, selection));
                items.AddSeparator();

                items.AddIfEnabled(new VappStartCommand(mainWindow, selection));
                items.AddIfEnabled(new VappShutDownCommand(mainWindow, selection));
                items.AddSeparator();

                items.AddIfEnabled(new StartVMOnHostToolStripMenuItem(mainWindow, selection, true));
                items.AddIfEnabled(new ResumeVMOnHostToolStripMenuItem(mainWindow, selection, true));
                items.AddIfEnabled(new MigrateVMToolStripMenuItem(mainWindow, selection, true));
                items.AddSeparator();

                items.AddIfEnabled(new CopyVMCommand(mainWindow, selection));
                items.AddIfEnabled(new MoveVMCommand(mainWindow, selection));
                items.AddIfEnabled(new ExportCommand(mainWindow, selection));
                items.AddIfEnabled(new TakeSnapshotCommand(mainWindow, selection));
                items.AddIfEnabled(new ConvertVMToTemplateCommand(mainWindow, selection));
                items.AddIfEnabled(new AssignGroupToolStripMenuItem<VMSS>(mainWindow, selection, true));
                items.AddIfEnabled(new AssignGroupToolStripMenuItem<VM_appliance>(mainWindow, selection, true));
                items.AddSeparator();

                items.AddIfEnabled(new VtpmCommand(mainWindow, selection));
                items.AddIfEnabled(new InstallToolsCommand(mainWindow, selection));
                items.AddSeparator();

                items.AddIfEnabled(new DeleteVMCommand(mainWindow, selection));

                items.AddPluginItems(PluginContextMenu.vm, selection);
                items.AddSeparator();
                items.AddIfEnabled(new PropertiesCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                if (selection.Count == 1)
                {
                    VM vm = selection[0].XenObject as VM;

                    return vm != null && !vm.is_a_snapshot && !vm.is_a_template;
                }
                return false;
            }
        }

        private class SingleSR : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new RepairSRCommand(mainWindow, selection));
                items.AddIfEnabled(new SetAsDefaultSRCommand(mainWindow, selection));
                items.AddSeparator();

                items.AddIfEnabled(new DetachSRCommand(mainWindow, selection));
                items.AddIfEnabled(new ReattachSRCommand(mainWindow, selection));
                items.AddIfEnabled(new ForgetSRCommand(mainWindow, selection));
                items.AddIfEnabled(new DestroySRCommand(mainWindow, selection));

                items.AddPluginItems(PluginContextMenu.storage, selection);
                items.AddSeparator();

                items.AddIfEnabled(new PropertiesCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                return selection.ContainsOneItemOfType<SR>();
            }
        }

        private class SingleFolder : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new NewFolderCommand(mainWindow, selection));
                items.AddIfEnabled(new PutFolderIntoRenameModeCommand(mainWindow, selection));
                items.AddIfEnabled(new DeleteFolderCommand(mainWindow, selection));

                items.AddPluginItems(PluginContextMenu.folder, selection);
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                return selection.ContainsOneItemOfType<Folder>();
            }
        }

        private abstract class MultipleVMs : Builder
        {
            public override bool IsValid(SelectedItemCollection selection)
            {
                if (selection.Count > 1)
                {
                    foreach (SelectedItem item in selection)
                    {
                        VM vm = item.XenObject as VM;

                        if (vm == null || vm.is_a_template || vm.is_a_snapshot)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                return false;
            }
        }

        private class MultipleTemplates : Builder
        {
            public override bool IsValid(SelectedItemCollection selection)
            {
                if (selection.Count > 1)
                {
                    foreach (SelectedItem item in selection)
                    {
                        VM vm = item.XenObject as VM;

                        if (vm == null || !vm.is_a_template || vm.is_a_snapshot)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                return false;
            }

            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new DeleteTemplateCommand(mainWindow, selection));
                items.AddIfEnabled(new EditTagsCommand(mainWindow, selection));
                items.AddPluginItems(PluginContextMenu.template, selection);
            }
        }

        private class MultipleSnapshots : Builder
        {
            public override bool IsValid(SelectedItemCollection selection)
            {
                if (selection.Count > 1)
                {
                    foreach (SelectedItem item in selection)
                    {
                        VM vm = item.XenObject as VM;

                        if (vm == null || !vm.is_a_snapshot)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                return false;
            }

            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new EditTagsCommand(mainWindow, selection));
                items.AddIfEnabled(new DeleteSnapshotCommand(mainWindow, selection));
            }
        }

        private class MultipleVMsInPool : MultipleVMs
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new ShutDownVMCommand(mainWindow, selection));
                items.AddIfEnabled(new StartVMCommand(mainWindow, selection));
                items.AddIfEnabled(new ResumeVMCommand(mainWindow, selection));
                items.AddIfEnabled(new SuspendVMCommand(mainWindow, selection));
                items.AddIfEnabled(new RebootVMCommand(mainWindow, selection));
                items.AddSeparator();
                items.AddIfEnabled(new ForceVMShutDownCommand(mainWindow, selection));
                items.AddIfEnabled(new ForceVMRebootCommand(mainWindow, selection));
                items.AddSeparator();

                items.AddIfEnabled(new VappStartCommand(mainWindow, selection));
                items.AddIfEnabled(new VappShutDownCommand(mainWindow, selection));
                items.AddSeparator();

                items.AddIfEnabled(new StartVMOnHostToolStripMenuItem(mainWindow, selection, true));
                items.AddIfEnabled(new ResumeVMOnHostToolStripMenuItem(mainWindow, selection, true));
                items.AddIfEnabled(new MoveVMCommand(mainWindow, selection));
                items.AddIfEnabled(new MigrateVMToolStripMenuItem(mainWindow, selection, true));
				items.AddIfEnabled(new ExportCommand(mainWindow, selection));
                items.AddIfEnabled(new AssignGroupToolStripMenuItem<VMSS>(mainWindow, selection, true));
                items.AddIfEnabled(new AssignGroupToolStripMenuItem<VM_appliance>(mainWindow, selection, true));
                items.AddSeparator();

                items.AddIfEnabled(new InstallToolsCommand(mainWindow, selection));
                items.AddIfEnabled(new DeleteVMCommand(mainWindow, selection));
                items.AddIfEnabled(new EditTagsCommand(mainWindow, selection));
                items.AddPluginItems(PluginContextMenu.vm, selection);
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                if (base.IsValid(selection))
                {
                    Pool firstPool = Helpers.GetPoolOfOne(selection[0].Connection);

                    if (firstPool != null)
                    {
                        foreach (SelectedItem item in selection)
                        {
							Pool pool = Helpers.GetPoolOfOne(item.Connection);

                            if (pool == null || !firstPool.Equals(pool))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        private class MultipleVMsOverMultiplePools : MultipleVMs
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new ShutDownVMCommand(mainWindow, selection));
                items.AddIfEnabled(new StartVMCommand(mainWindow, selection));
                items.AddIfEnabled(new ResumeVMCommand(mainWindow, selection));
                items.AddIfEnabled(new SuspendVMCommand(mainWindow, selection));
                items.AddIfEnabled(new RebootVMCommand(mainWindow, selection));
                items.AddSeparator();
                items.AddIfEnabled(new ForceVMShutDownCommand(mainWindow, selection));
                items.AddIfEnabled(new ForceVMRebootCommand(mainWindow, selection));
                items.AddSeparator();

                items.AddIfEnabled(new InstallToolsCommand(mainWindow, selection));
                items.AddIfEnabled(new DeleteVMCommand(mainWindow, selection));
                items.AddIfEnabled(new EditTagsCommand(mainWindow, selection));
                items.AddPluginItems(PluginContextMenu.vm, selection);
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                if (base.IsValid(selection))
                {
                    return !new MultipleVMsInPool().IsValid(selection);
                }
                return false;
            }
        }

        private class MultipleFolders : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new DeleteFolderCommand(mainWindow, selection));

                items.AddPluginItems(PluginContextMenu.folder, selection);
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                return selection.AllItemsAre<Folder>() && selection.Count > 1;
            }
        }

        private class SingleTag : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new DeleteTagCommand(mainWindow, selection));
                items.AddIfEnabled(new PutTagIntoRenameModeCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                return selection.AllItemsAre<GroupingTag>(IsValid) && selection.Count == 1;
            }

            private static bool IsValid(GroupingTag groupingTag)
            {
                return groupingTag.Grouping.GroupingName == Messages.TAGS;
            }
        }

        private class MultipleTags : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new DeleteTagCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                return selection.AllItemsAre<GroupingTag>(IsValid) && selection.Count > 1;
            }

            private static bool IsValid(GroupingTag groupingTag)
            {
                return groupingTag.Grouping.GroupingName == Messages.TAGS;
            }
        }

        private class SingleDockerContainer : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new StartDockerContainerCommand(mainWindow, selection));
                items.AddIfEnabled(new StopDockerContainerCommand(mainWindow, selection));
                items.AddIfEnabled(new PauseDockerContainerCommand(mainWindow, selection));
                items.AddIfEnabled(new ResumeDockerContainerCommand(mainWindow, selection));
                items.AddIfEnabled(new RestartDockerContainerCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                if (selection.Count == 1)
                {
                    DockerContainer dockerContainer = selection[0].XenObject as DockerContainer;

                    return dockerContainer != null;
                }
                return false;
            }
        }

        private class MultipleDockerContainers : Builder
        {
            public override void Build(IMainWindow mainWindow, SelectedItemCollection selection, ContextMenuItemCollection items)
            {
                items.AddIfEnabled(new StartDockerContainerCommand(mainWindow, selection));
                items.AddIfEnabled(new StopDockerContainerCommand(mainWindow, selection));
                items.AddIfEnabled(new PauseDockerContainerCommand(mainWindow, selection));
                items.AddIfEnabled(new ResumeDockerContainerCommand(mainWindow, selection));
                items.AddIfEnabled(new RestartDockerContainerCommand(mainWindow, selection));
            }

            public override bool IsValid(SelectedItemCollection selection)
            {
                return selection.AllItemsAre<DockerContainer>();
            }
        }

        #endregion
    }
}
