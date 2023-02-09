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
using System.Windows.Forms;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Controls.DataGridViewEx;
using System.Collections.Generic;


namespace XenAdmin.Controls
{
    /// <summary>
    /// This control is used by both the New VM Wizard and the Import VM Wizard. 
    /// </summary>
    public partial class AffinityPicker : UserControl
    {
        private IXenConnection Connection;
        private Host SrHost;
        private Host Affinity;
        private bool selectedOnVisibleChanged;

        /// <summary>
        /// Should always be true if the AffinityPicker is used to create a VM.
        /// If set to false (e.g. on Edit VM) and Affinity is null, then the "no home server" radio button remains enabled, 
        /// meaning that the VM already has no affinity and should not try to automatically select one.
        /// </summary>
        internal bool AutoSelectAffinity = true;

        public event Action SelectedAffinityChanged;

        public AffinityPicker()
        {
            InitializeComponent();
        }

        public void SetAffinity(IXenConnection connection, Host affinity, Host srhost)
        {
            Connection = connection;
            Affinity = affinity;
            SrHost = srhost;
            tableLayoutPanelWlbWarning.Visible = Helpers.WlbEnabledAndConfigured(connection);
            LoadServers();
            UpdateControl();
            SelectRadioButtons();
            SelectedAffinityChanged?.Invoke();
        }

        private void LoadServers()
        {
            ServersGridView.Rows.Clear();

            List<Host> hosts = new List<Host>(Connection.Cache.Hosts);
            hosts.Sort();
            foreach (Host host in hosts)
                ServersGridView.Rows.Add(new ServerGridRow(host, true));
        }

        private void UpdateControl()
        {
            if (Connection == null)
                return;

            // Update enablement
            DynamicRadioButton.Enabled = (Helpers.HasFullyConnectedSharedStorage(Connection) && SrHost == null) ||
                                         (Affinity == null && !AutoSelectAffinity);
            ServersGridView.Enabled = StaticRadioButton.Checked;
            DynamicRadioButton.Text = Helpers.HasFullyConnectedSharedStorage(Connection)
                                          ? Messages.AFFINITY_PICKER_DYNAMIC_SHARED_SR
                                          : Messages.AFFINITY_PICKER_DYNAMIC_NOT_SHARED_SR;
        }

        private void SelectRadioButtons()
        {
            if (!SelectAffinityServer() && DynamicRadioButton.Enabled)
            {
                //Trace.Assert(DynamicRadioButton.Enabled, "Could not select any hosts or find shared storage");
                DynamicRadioButton.Checked = true; // always set dynamic check state before static because static check has an event handler
                StaticRadioButton.Checked = false;
            }
            else
            {
                DynamicRadioButton.Checked = false; // always set dynamic check state before static because static check has an event handler
                StaticRadioButton.Checked = true;
            }
        }

        public Host SelectedAffinity => DynamicRadioButton.Checked ? null : SelectedServer();


        private Host SelectedServer()
        {
            if (ServersGridView.SelectedRows.Count > 0)
                return ((ServerGridRow)ServersGridView.SelectedRows[0]).Server;

            return null;
        }

        private bool SelectServer(Host host)
        {
            foreach (ServerGridRow row in ServersGridView.Rows)
            {
                if (row.Server.opaque_ref != host.opaque_ref)
                    continue;

                if (row.Enabled)
                {

                    row.Selected = true;
                    return true;
                }

                return false;
            }
            return false;
        }

        private bool SelectAffinityServer()
        {
            return Affinity != null && SelectServer(Affinity);
        }

        private bool SelectSomething()
        {
            bool selected = false;

            if (Affinity != null)
                selected = SelectServer(Affinity);

            if (!selected && SrHost != null)
                selected = SelectServer(SrHost);

            return selected;
        }

        public bool ValidState()
        {
            return SelectedAffinity != null || DynamicRadioButton.Checked;
        }

        // we dont need to bother firing events if the other radio button gets checked or unchecked
        private void StaticRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (StaticRadioButton.Checked && SelectedServer() == null)
                SelectSomething();

            UpdateControl();
            SelectedAffinityChanged?.Invoke();

            if (StaticRadioButton.Checked)
                ServersGridView.Select();
        }

        protected override void OnEnter(EventArgs e)
        {
            if (DynamicRadioButton.Checked)
                DynamicRadioButton.Select();
            else
                ServersGridView.Select();
        }

        private void ServersGridView_VisibleChanged(object sender, EventArgs e)
        {
            if (!selectedOnVisibleChanged)
            {
                selectedOnVisibleChanged = true;
                SelectSomething();//CA-213728
            }
        }

        private void ServersGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateControl();
            SelectedAffinityChanged?.Invoke();
        }
    }

    internal class ServerGridRow : DataGridViewExRow
    {
        public readonly Host Server;

        private readonly DataGridViewExImageCell ImageCell = new DataGridViewExImageCell();
        private readonly DataGridViewTextBoxCell NameCell = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell ReasonCell = new DataGridViewTextBoxCell();

        private readonly bool showReason;

        public ServerGridRow(Host server, bool showReason)
        {
            Server = server;
            this.showReason = showReason;

            Cells.Add(ImageCell);
            Cells.Add(NameCell);

            if (showReason)
            {
                Cells.Add(ReasonCell);
            }

            UpdateDetails();
        }

        private void UpdateDetails()
        {
            ImageCell.Value = Images.GetImage16For(Server);
            NameCell.Value = Server.Name();

            bool isLiveHost = Server.IsLive();
            Enabled = isLiveHost;

            if (showReason)
            {
                ReasonCell.Value = isLiveHost ? Server.HostMemoryString() : Messages.HOMESERVER_PICKER_HOST_NOT_LIVE;
            }

        }
    }
}
