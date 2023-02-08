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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Controls;

using XenAdmin.Actions;

namespace XenAdmin.Dialogs
{
    public partial class NewPoolDialog : XenDialogBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<ConnectionWrapperWithMoreStuff> connections = new List<ConnectionWrapperWithMoreStuff>();
        private List<IXenConnection> newConnections = new List<IXenConnection>();

        public NewPoolDialog(Host host) 
        {
            InitializeComponent();
            customTreeViewServers.NodeIndent = 3;
            customTreeViewServers.ShowCheckboxes = true;
            customTreeViewServers.ShowDescription = true;
            customTreeViewServers.ShowImages = false;
            getAllCurrentConnections();
            ConnectionsManager.XenConnections.CollectionChanged += XenConnections_CollectionChanged;
            poolName = string.Empty; //forcing user to enter something before the Next button is enabled
            selectCoordinator(host);
            updateButtons();
        }

        public NewPoolDialog(List<Host> hosts)
            : this(hosts.Count >= 1 ? hosts[0] : null)
        {
            selectSupporters(hosts);
        }

        private enum InvalidReasons { NONE, EMPTY_POOL_NAME, NO_COORDINATOR, MAX_POOL_SIZE_EXCEEDED };
        private InvalidReasons validToClose
        {
            get
            {
                if (poolName == "")
                    return InvalidReasons.EMPTY_POOL_NAME;

                if (comboBoxServers.SelectedIndex < 0)
                    return InvalidReasons.NO_COORDINATOR;

                if (comboBoxServers.Items.Count <= 0)
                    return InvalidReasons.NO_COORDINATOR;

                Host coordinator = getCoordinator();
                if (coordinator != null)
                {
                    List<Host> supporters = getSupporters();
                    if (PoolJoinRules.WillExceedPoolMaxSize(coordinator.Connection, supporters.Count))
                        return InvalidReasons.MAX_POOL_SIZE_EXCEEDED;
                }

                return InvalidReasons.NONE;
            }
        }

        private string poolName
        {
            get { return textBoxName.Text.Trim(); }
            set { textBoxName.Text = value; }
        }

        private string poolDescription
        {
            get { return textBoxDescription.Text; }
        }

        private void updateButtons()
        {
            switch (validToClose)
            {
                case InvalidReasons.NONE:
                    buttonCreate.Enabled = true;
                    toolTipContainerCreate.RemoveAll();
                    break;
                case InvalidReasons.NO_COORDINATOR:
                    buttonCreate.Enabled = false;
                    toolTipContainerCreate.SetToolTip(Messages.NO_ELIGIBLE_COORDINATOR);
                    break;
                case InvalidReasons.EMPTY_POOL_NAME:
                    buttonCreate.Enabled = false;
                    toolTipContainerCreate.SetToolTip(Messages.POOL_NAME_EMPTY);
                    break;
                case InvalidReasons.MAX_POOL_SIZE_EXCEEDED:
                    buttonCreate.Enabled = false;
                    toolTipContainerCreate.SetToolTip(Messages.NEWPOOL_WILL_EXCEED_POOL_MAX_SIZE);
                    break;
            }
        }

        private void createPool()
        {
            try
            {
                Host coordinator = getCoordinator();
                if (coordinator == null)
                {
                    log.Error("Disconnected during create pool");
                    return;
                }

                List<Host> supporters = getSupporters();
                // Check supp packs and warn
                List<string> badSuppPacks = PoolJoinRules.HomogeneousSuppPacksDiffering(supporters, coordinator);

                if (!Program.RunInAutomatedTestMode && badSuppPacks.Count > 0)
                {
                    string msg = string.Format(badSuppPacks.Count == 1 ? Messages.NEW_POOL_SUPP_PACK : Messages.NEW_POOL_SUPP_PACKS,
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
                if (null != supporters.Find(host =>
                    !PoolJoinRules.CompatibleCPUs(host, coordinator, false) &&
                    Helpers.FeatureForbidden(host, Host.RestrictCpuMasking) &&
                    !PoolJoinRules.FreeHostPaidCoordinator(host, coordinator, false)))  // in this case we can upgrade the license and then mask the CPU
                {
                    UpsellDialog.ShowUpsellDialog(Messages.UPSELL_BLURB_CPUMASKING, this);
                    return;
                }

                if (!Program.RunInAutomatedTestMode)
                {
                    var hosts1 = supporters.FindAll(host => PoolJoinRules.FreeHostPaidCoordinator(host, coordinator, false));
                    if (hosts1.Count > 0)
                    {
                        string msg = string.Format(hosts1.Count == 1
                                ? Messages.NEW_POOL_LICENSE_MESSAGE
                                : Messages.NEW_POOL_LICENSE_MESSAGE_MULTIPLE,
                            string.Join("\n", hosts1.Select(h => h.Name())));

                        using (var dlg = new WarningDialog(msg, ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                            {HelpNameSetter = "PoolJoinRelicensing"})
                        {
                            if (dlg.ShowDialog(Program.MainWindow) == DialogResult.No)
                                return;
                        }
                    }

                    var hosts2 = supporters.FindAll(host => !PoolJoinRules.CompatibleCPUs(host, coordinator, false));
                    if (hosts2.Count > 0)
                    {
                        string msg = string.Format(hosts2.Count == 1
                                ? Messages.NEW_POOL_CPU_MASKING_MESSAGE
                                : Messages.NEW_POOL_CPU_MASKING_MESSAGE_MULTIPLE,
                            string.Join("\n", hosts2.Select(h => h.Name())), BrandManager.ProductBrand);

                        using (var dlg = new WarningDialog(msg, ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                            {HelpNameSetter = "PoolJoinCpuMasking"})
                        {
                            if (dlg.ShowDialog(Program.MainWindow) == DialogResult.No)
                                return;
                        }
                    }

                    var hosts3 = supporters.FindAll(host => !PoolJoinRules.CompatibleAdConfig(host, coordinator, false));
                    if (hosts3.Count > 0)
                    {
                        string msg = string.Format(hosts3.Count == 1
                                ? Messages.NEW_POOL_AD_MESSAGE
                                : Messages.NEW_POOL_AD_MESSAGE_MULTIPLE,
                            string.Join("\n", hosts3.Select(h => h.Name())));

                        using (var dlg = new WarningDialog(msg, ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                            {HelpNameSetter = "PoolJoinAdConfiguring"})
                        {
                            if (dlg.ShowDialog(Program.MainWindow) == DialogResult.No)
                                return;
                        }
                    }
                }

                if (!HelpersGUI.GetPermissionForCpuFeatureLevelling(supporters, Helpers.GetPoolOfOne(coordinator.Connection)))
                    return;

                log.DebugFormat("Creating new pool {0} ({1}) with coordinator {2} and supporters {3}",
                    poolName, 
                    poolDescription, 
                    Helpers.GetName(coordinator),
                    string.Join(", ", supporters.Select(Helpers.GetName).ToList())
                );

                new CreatePoolAction(coordinator, supporters, poolName, poolDescription, AddHostToPoolCommand.GetAdPrompt,
                    AddHostToPoolCommand.NtolDialog,
                    (licenseFailures, exceptionMessage) =>
                    {
                        if (licenseFailures.Count > 0)
                        {
                            Program.Invoke(this, () =>
                            {
                                using (var dlg = new CommandErrorDialog(Messages.LICENSE_ERROR_TITLE, exceptionMessage,
                                    licenseFailures.ToDictionary<LicenseFailure, IXenObject, string>(f => f.Host, f => f.AlertText)))
                                {
                                    dlg.ShowDialog(this);
                                }
                            });
                        }
                    }).RunAsync();
            }
            catch (System.Net.WebException exn)
            {
                log.Debug(exn, exn);
            }
        }

        private void addConnectionsToListBox()
        {
            ConnectionWrapperWithMoreStuff coordinator = (comboBoxServers.Items.Count > 0 ? (ConnectionWrapperWithMoreStuff)comboBoxServers.SelectedItem : null);
            foreach (ConnectionWrapperWithMoreStuff c in connections)
            {
                c.TheCoordinator = coordinator;
                c.Refresh();
            }
            customTreeViewServers.BeginUpdate();
            try
            {
                customTreeViewServers.ClearAllNodes();
                foreach (ConnectionWrapperWithMoreStuff c in connections)
                {
                    customTreeViewServers.AddNode(c);
                }
                customTreeViewServers.Resort();
            }
            finally
            {
                customTreeViewServers.EndUpdate();
                customTreeViewServers.Invalidate();
            }
        }

        private void getAllCurrentConnections()
        {
            List<IXenConnection> toRemove = new List<IXenConnection>();
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (connection != null&&connection.IsConnected)
                {
                    bool contains = false;
                    foreach (ConnectionWrapperWithMoreStuff cw in connections)
                    {
                        if (cw.Connection == connection)
                        {
                            contains = true;
                            break;
                        }
                    }
                    if (!contains)
                    {
                        ConnectionWrapperWithMoreStuff c = new ConnectionWrapperWithMoreStuff(connection);
                        connections.Add(c);
                        connection.CachePopulated -= Connection_CachePopulated;
                        connection.CachePopulated += Connection_CachePopulated;
                        connection.ConnectionStateChanged -= connection_ConnectionStateChanged;
                        connection.ConnectionStateChanged += connection_ConnectionStateChanged;
                        if (newConnections.Contains(connection))
                        {
                            c.State = CheckState.Checked;
                            toRemove.Add(connection);
                        }
                    }
                }
            }
            foreach (IXenConnection connection in toRemove)
            {
                newConnections.Remove(connection);
            }
            connections.RemoveAll((Predicate<ConnectionWrapperWithMoreStuff>)delegate(ConnectionWrapperWithMoreStuff c)
            {
                return !ConnectionsManager.XenConnectionsCopy.Contains(c.Connection);
            });
            addConnectionsToComboBox();
        }

        private void addConnectionsToComboBox()
        {
            comboBoxServers.Items.Clear();
            ConnectionWrapperWithMoreStuff coordinator = null;
            foreach (ConnectionWrapperWithMoreStuff wrappedConnection in connections)
            {
                wrappedConnection.Refresh();
                if (wrappedConnection.CanBeCoordinator)
                {
                    comboBoxServers.Items.Add(wrappedConnection);
                    if (wrappedConnection.WillBeCoordinator)
                        coordinator = wrappedConnection;
                }
            }
            if (coordinator != null)
            {
                comboBoxServers.SelectedItem = coordinator;
            }
            else if (comboBoxServers.Items.Count > 0)
            {
                ConnectionWrapperWithMoreStuff defaultCoordinator = comboBoxServers.Items[0] as ConnectionWrapperWithMoreStuff;
                setAsCoordinator(defaultCoordinator);
                comboBoxServers.SelectedItem = defaultCoordinator;
            }
            addConnectionsToListBox();
            updateButtons();
        }

        private void selectCoordinator(Host coordinator)
        {
            if (coordinator == null || Helpers.GetPool(coordinator.Connection) != null)
                return;
            foreach (ConnectionWrapperWithMoreStuff c in connections)
            {
                if (c.WillBeCoordinator)
                    c.State = CheckState.Unchecked;
            }
            foreach (ConnectionWrapperWithMoreStuff c in connections)
            {
                if (c.Connection == coordinator.Connection)
                {
                    if (c.CanBeCoordinator)
                        setAsCoordinator(c);
                    return;
                }
            }
        }

        private void selectSupporters(List<Host> supporters)
        {
            foreach (ConnectionWrapperWithMoreStuff c in connections)
            {
                if (c.AllowedAsSupporter &&
                    supporters.Find(supporter => (c.Connection == supporter.Connection)) != null)  // one of the supporters is the connection we're looking at
                {
                    c.State = CheckState.Checked;
                }
            }
        }

        private void setAsCoordinator(ConnectionWrapperWithMoreStuff defaultCoordinator)
        {
            foreach (ConnectionWrapperWithMoreStuff connectionWrapper in connections)
                connectionWrapper.TheCoordinator = defaultCoordinator;

            comboBoxServers.SelectedItem = defaultCoordinator;
            addConnectionsToListBox();
        }

        private Host getCoordinator()
        {
            ConnectionWrapperWithMoreStuff connectionToCoordinator = null;
            foreach (ConnectionWrapperWithMoreStuff connection in connections)
            {
                if (connection.WillBeCoordinator)
                {
                    connectionToCoordinator = connection;
                    break;
                }
            }
            return connectionToCoordinator != null ? Helpers.GetCoordinator(connectionToCoordinator.Connection) : null;
        }

        private List<Host> getSupporters()
        {
            List<Host> supporterConnections = new List<Host>();
            foreach (ConnectionWrapperWithMoreStuff connection in connections)
            {
                if (connection.State == CheckState.Checked && !connection.WillBeCoordinator && connection.AllowedAsSupporter)
                {
                    supporterConnections.Add(Helpers.GetCoordinator(connection.Connection));
                }
            }
            return supporterConnections;
        }

        private void comboBoxServers_SelectionChangeCommitted(object sender, EventArgs e)
        {
            setAsCoordinator(comboBoxServers.SelectedItem as ConnectionWrapperWithMoreStuff);
        }

        private void buttonAddNewServer_Click(object sender, EventArgs e)
        {
            IXenConnection newConnection = new XenConnection();
            newConnections.Add(newConnection);
            new AddServerTask(this, newConnection, null).Start();
        }

        private void comboBoxServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateButtons();
        }

        private void customTreeViewServers_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Space || !(customTreeViewServers.SelectedItem is CustomTreeNode))
                return;
            CustomTreeNode node = (CustomTreeNode)customTreeViewServers.SelectedItem;
            if (!node.Enabled)
                return;
            node.State = node.State == CheckState.Unchecked ? CheckState.Checked : CheckState.Unchecked;
            customTreeViewServers.Refresh();
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            updateButtons();
        }

        void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.BeginInvoke(this, getAllCurrentConnections);
        }

        void Connection_CachePopulated(IXenConnection conn)
        {
            Program.BeginInvoke(this, addConnectionsToComboBox);
        }

        void connection_ConnectionStateChanged(IXenConnection conn)
        {
            Program.BeginInvoke(this, addConnectionsToComboBox);
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            createPool();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void customTreeViewServers_ItemCheckChanged(object sender, EventArgs e)
        {
            updateButtons();
        }
    }
}
