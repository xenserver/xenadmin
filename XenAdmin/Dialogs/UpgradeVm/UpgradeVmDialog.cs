/* Copyright (c) Citrix Systems Inc. 
 * All rights reserved. 
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAdmin.Dialogs.VMProtectionRecovery;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAPI;
using XenCenterLib;


namespace XenAdmin.Dialogs
{
    public partial class UpgradeVmDialog : XenDialogBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SelectedItemCollection selection = null;

        private const bool SELECT_ALL_EFFECTS_NON_VISIBLE = true;
        private const bool CLEAR_ALL_EFFECTS_NON_VISIBLE = true;

        private readonly IMainWindow mainWindow = null;
        private readonly IXenConnection xenConnection = null;

        private bool firstRun = true;

        public UpgradeVmDialog(SelectedItemCollection selection, IMainWindow mainWindow)
        {
            this.selection = selection;
            this.mainWindow = mainWindow;

            IXenConnection connection = null;

            if (selection != null && selection.Count == 1 && selection[0].XenObject is Pool || selection[0].XenObject is Host)
                    connection = selection[0].XenObject.Connection;

            if (connection != null && connection.IsConnected && Helpers.DundeeOrGreater(connection))
                xenConnection = connection;

            InitializeComponent();
        }

        private class VmRow : DataGridViewRow
        {
            private DataGridViewCheckBoxCell checkboxCell = new DataGridViewCheckBoxCell() { TrueValue = true, FalseValue = false, Value = false };
            private DataGridViewTextAndImageCell nameCell = new DataGridViewTextAndImageCell();
            private DataGridViewTextAndImageCell statusCell = new DataGridViewTextAndImageCell();
            private DataGridViewLinkCell actionCell = new DataGridViewLinkCell();

            private static VmUpgradeState[] CAN_UPGRADE_STATES = new VmUpgradeState[] { VmUpgradeState.INSTALL_MANAGEMENT_AGENT, VmUpgradeState.UPGRADABLE };

            public VM VM { get; private set; }

            public VmRow(VM vm)
            {
                VM = vm;
                vm.PropertyChanged += vm_PropertyChanged;

                Cells.Add(checkboxCell);

                Cells.Add(nameCell);
                Cells.Add(statusCell);
                Cells.Add(actionCell);

                RefreshRow();
            }

            protected override void Dispose(bool disposing)
            {
                if (VM != null)
                    VM.PropertyChanged -= vm_PropertyChanged;

                base.Dispose(disposing);
            }

            void vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                RefreshRow();
            }

            public bool Checked
            {
                get
                {
                    return checkboxCell.Value as bool? ?? false;
                }
                set
                {
                    if (!checkboxCell.ReadOnly)
                        checkboxCell.Value = value;
                }
            }

            public bool Selectable
            {
                get
                {
                    return !checkboxCell.ReadOnly;
                }
                set
                {
                    Checked = Checked & value;
                    checkboxCell.ReadOnly = !value;
                }
            }

            public void RefreshRow()
            {
                Program.Invoke(Program.MainWindow, delegate()
                {
                    this.Selectable = CanBeUpgraded();

                    nameCell.Value = VM.Name;
                    statusCell.Value = GetStatusStringForVm();

                    nameCell.Image = GetImage();

                    actionCell.Value = GetActionStringForVm();
                    actionCell.TrackVisitedState = true;
                });
            }

            private Image GetImage()
            {
                return Images.GetImage16For(VM);
            }

            private string GetStatusStringForVm()
            {
                var status = VMStateHelper.GetUpgradeStateForVm(VM);

                switch (status)
                {
                    case VmUpgradeState.INITIALIZING : return Messages.UPGRADE_VMS_STATE_INITIALIZING;
                    case VmUpgradeState.INSTALL_MANAGEMENT_AGENT : return Messages.UPGRADE_VMS_STATE_INSTALL_MANAGEMENT_AGENT;
                    case VmUpgradeState.UPGRADABLE : return Messages.UPGRADE_VMS_STATE_UPGRADABLE;
                    case VmUpgradeState.UPGRADED : return Messages.UPGRADE_VMS_STATE_UPGRADED;
                    case VmUpgradeState.UPGRADING: return Messages.UPGRADE_VMS_STATE_UPGRADING;
                    case VmUpgradeState.UPGRADING_GO_TO_CONSOLE: return Messages.UPGRADE_VMS_STATE_UPGRADING_GO_TO_CONSOLE;
                    case VmUpgradeState.ERROR: return Messages.UPGRADE_VMS_STATE_ERROR;
                    case VmUpgradeState.UNKNOWN: return Messages.UPGRADE_VMS_STATE_UNKNOWN;
                    case VmUpgradeState.UPDATEABLE: return Messages.UPGRADE_VMS_STATE_UPDATEABLE;
                }

                return status.ToString();
            }

            private string GetActionStringForVm()
            {
                var status = VMStateHelper.GetUpgradeStateForVm(VM);

                if (status == VmUpgradeState.UPGRADING_GO_TO_CONSOLE)
                    return Messages.UPGRADE_VMS_ACTIONS_GO_TO_CONSOLE;

                if (status == VmUpgradeState.ERROR)
                    return Messages.UPGRADE_VMS_ACTIONS_ERROR;

                return string.Empty;
            }

            private bool CanBeUpgraded()
            {
                var status = VMStateHelper.GetUpgradeStateForVm(VM);
                
                return CAN_UPGRADE_STATES.Contains(status);
            }
        }

        private enum VmUpgradeState 
        {
            UNKNOWN,
            ERROR,
            UPGRADABLE,
            INSTALL_MANAGEMENT_AGENT,
            UPGRADING,
            UPGRADING_GO_TO_CONSOLE,
            UPGRADED,
            INITIALIZING,
            NOT_SHOWN,
            UPDATEABLE,
        }

        private static class VMStateHelper
        {
            internal static VmUpgradeState GetUpgradeStateForVm(VM vm)
            {
                //if (!vm.is_a_real_vm || !vm.IsWindows || ???)
                //    return VmUpgradeState.NOT_SHOWN;
                
                var guestToClient = vm.GuestToClientValue;
                var clientToGuest = vm.ClientToGuestValue;

                if (clientToGuest == VM.ClientToGuestMessage.EMPTY && guestToClient == VM.GuestToClientMessage.EMPTY)
                    return VmUpgradeState.UPGRADABLE;

                if (clientToGuest == VM.ClientToGuestMessage.INITIALIZING)
                    return VmUpgradeState.INITIALIZING;

                if (clientToGuest == VM.ClientToGuestMessage.GOTOCONSOLE)
                    return VmUpgradeState.UPGRADING_GO_TO_CONSOLE;

                if (clientToGuest == VM.ClientToGuestMessage.UPGRADING
                    || guestToClient == VM.GuestToClientMessage.STARTED
                    || guestToClient == VM.GuestToClientMessage.RUNNING
                    )
                    return VmUpgradeState.UPGRADING;

                if (guestToClient == VM.GuestToClientMessage.DONE)
                    return VmUpgradeState.UPGRADED;

                if (guestToClient == VM.GuestToClientMessage.ERROR)
                    return VmUpgradeState.ERROR;

                //if (mgmnt agent running, version < version_Dundee) //case for old management agent
                //    return VmUpgradeState.UPDATEABLE;

                return VmUpgradeState.UNKNOWN;
            }
        }

        private void ShowVmConsole(VM vm)
        {
            if (mainWindow.SelectObjectInTree(vm))
            {
                mainWindow.SwitchToTab(MainWindow.Tab.Console);
            }
            else
            {
                log.Error("Could not find VM in the TreeView.");
            }

            var mw = mainWindow.Form as MainWindow;
            var vncView = mw.ConsolePanel.activeVNCView;
            
            vncView.UnDockVncView();
            vncView.FocusConsole(true); 
            
            if (vncView.undockedForm != null)
                vncView.undockedForm.FormClosed += UndockedConsoleForm_FormClosed;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                log.Error(e.Error);
                return;
            }

            Program.Invoke(this, () => RefreshGrid((List<VmRow>)e.Result));
        }

        private void RefreshGrid(List<VmRow> rows)
        {
            Program.AssertOnEventThread();

            vmsDataGridView.SuspendLayout();
            try
            {
                List<VM> checkedVMs = new List<VM>();

                foreach (var row in vmsDataGridView.Rows)
                {
                    var vmrow = row as VmRow;
                    if (vmrow != null && vmrow.Checked)
                        checkedVMs.Add(vmrow.VM);
                }

                vmsDataGridView.Rows.Clear();

                foreach (var row in rows)
                {
                    if (vmsDataGridView.ColumnCount > 0)
                    {
                        vmsDataGridView.Rows.Add(row);

                        if (checkedVMs.Contains(row.VM) || firstRun)
                            row.Checked = true;
                    }
                }

                RefreshButtons();
                RefreshWarning();

            }
            finally
            {
                firstRun = false;
                vmsDataGridView.ResumeLayout();
            }
        }

        object worker_DoWork(object sender, object argument)
        {
            var list = new List<VmRow>();

            if (xenConnection == null || !xenConnection.IsConnected || !Helpers.DundeeOrGreater(xenConnection))
                return list;
                
            foreach (var vm in xenConnection.Cache.VMs)
            {
                if (vm.is_a_real_vm && vm.IsRunning)
                {
                    var vmRow = new VmRow(vm);
                    vmRow.Visible = searchTextBox1.Text.Length == 0 || searchTextBox1.Matches(vm.Name);
                    list.Add(vmRow);
                }
            }

            return list;
        }

        void VM_CollectionChanged(object sender, EventArgs e)
        {
            LoadVms();
        }


        QueuedBackgroundWorker worker = new QueuedBackgroundWorker();
        private void LoadVms()
        {

            var sortedColumn = vmsDataGridView.SortedColumn;
            var sortOrder = vmsDataGridView.SortOrder;

            worker.RunWorkerAsync(worker_DoWork, worker_RunWorkerCompleted);

            if (sortOrder != SortOrder.None && sortedColumn != null)
                vmsDataGridView.Sort(sortedColumn, (sortOrder == SortOrder.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending);

        }

        private void UpgradeVmDialog_Load(object sender, EventArgs e)
        {
            LoadVms();

            foreach (var row in vmsDataGridView.Rows)
            {
                var vmRow = row as VmRow;
                if (vmRow != null)
                    vmRow.Checked = true;
            }

            foreach (IXenConnection xenConnection in ConnectionsManager.XenConnectionsCopy)
            {
                xenConnection.Cache.RegisterCollectionChanged<VM>(VM_CollectionChanged);
            }
        }

        private void RefreshButtons()
        {

        }

        private void RefreshWarning()
        {

        }

        private void UpgradeVmDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (IXenConnection xenConnection in ConnectionsManager.XenConnectionsCopy)
            {
                xenConnection.Cache.DeregisterCollectionChanged<VM>(VM_CollectionChanged);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void RefreshView(List<IXenObject> selectedItems)
        {

            LoadVms();
        }

        private void UpgradeButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(this, Messages.UPGRADEVM_CONFIRM_UPGRADE_MESSAGE, Messages.UPGRADEVM_CONFIRM_UPGRADE_MESSAGE_CAPTION, MessageBoxButtons.YesNo);
            if (result != System.Windows.Forms.DialogResult.Yes)
                return;

            var actions = new List<XenAdmin.Actions.AsyncAction>();

            foreach (var row in vmsDataGridView.Rows)
            {
                var vmRow = row as VmRow;
                if (vmRow != null)
                {
                    if (vmRow.Checked)
                    {
                        actions.Add(new UpgradeVmInsertIsoAction(vmRow.VM, true));
                    }
                }
            }

            new ParallelAction(xenConnection, Messages.UPGRADEVM_MULTIACTION_TITLE, Messages.UPGRADEVM_MULTIACTION_START, Messages.UPGRADEVM_MULTIACTION_END, actions).RunAsync();
        }

        void UndockedConsoleForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Focus();
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            foreach (var row in vmsDataGridView.Rows)
            {
                var vmRow = row as VmRow;

                if (vmRow != null && !vmRow.ReadOnly)
                    vmRow.Checked = SELECT_ALL_EFFECTS_NON_VISIBLE ? true : vmRow.Visible || vmRow.Checked;
            }
        }

        private void ClearAllButton_Click(object sender, EventArgs e)
        {
            foreach (var row in vmsDataGridView.Rows)
            {
                var vmRow = row as VmRow;

                if (vmRow != null && !vmRow.ReadOnly)
                    vmRow.Checked = CLEAR_ALL_EFFECTS_NON_VISIBLE ? false : !vmRow.Visible && vmRow.Checked;
            }
        }


        protected void searchTextBox1_TextChanged(object sender, System.EventArgs e)
        {
            LoadVms();
        }

        private void vmsDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var row = (sender as DataGridView).Rows[e.RowIndex] as VmRow;
            if (row != null)
            {
                if (e.ColumnIndex == 0) //Checkbox coulmn was clicked
                    row.Checked = !row.Checked;
                else if (e.ColumnIndex == 3) //Action column was clicked
                    ShowVmConsole(row.VM);
            }
        }
    }
}

