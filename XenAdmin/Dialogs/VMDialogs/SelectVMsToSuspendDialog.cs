/* Copyright (c) Citrix Systems, Inc. 
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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using XenAdmin.Actions;

using XenAPI;
using XenAdmin.Actions.HostActions;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;


namespace XenAdmin.Dialogs.VMDialogs
{
    public partial class SelectVMsToSuspendDialog : XenDialogBase
    {
        private Host Server;
        private readonly VMsWhichCanBeMigratedAction VmsToMigrateAction;
        private long PoolMemoryFree = 0;

        public SelectVMsToSuspendDialog(Host server)
        {
            InitializeComponent();
            Server = server;

            AvailableLabel.Font = Program.DefaultFontBold;
            RequiredLabel.Font = Program.DefaultFontBold;

            AvailableLabel.Text = PoolMemoryAvailableExcludingHost(Server);

            VmsToMigrateAction = new VMsWhichCanBeMigratedAction(Server.Connection, Server);
            VmsToMigrateAction.Completed += action_Completed;
            VmsToMigrateAction.RunAsync();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            var shortName = Server.Name;
            if (shortName.Length > 25)
                shortName = Server.Name.Ellipsise(25);

            Text = string.Format(Messages.SELECT_VMS_TO_SUSPEND_DLOG_TITLE, shortName);
        }

        private string PoolMemoryAvailableExcludingHost(Host Server)
        {
            PoolMemoryFree = 0;
            foreach (Host host in Server.Connection.Cache.Hosts)
            {
                if (host.Equals(Server))
                    continue;

                PoolMemoryFree += host.memory_free_calc;
            }

            return Util.MemorySizeStringSuitableUnits(PoolMemoryFree, true);
        }

        private void UpdateRequiredMemory()
        {
            long required = 0;
            foreach (VmGridRow row in VmsGridView.Rows)
            {
                if (row.Action != ActionCellAction.Migrate)
                    continue;

                required += (long)row.Vm.memory_dynamic_max;
            }
            RequiredLabel.ForeColor = required > PoolMemoryFree ? Color.Red : ForeColor;
            RequiredLabel.Text = Util.MemorySizeStringSuitableUnits(required, true);

            OKButton.Enabled = required == 0 ? true : required < PoolMemoryFree;
        }

        void action_Completed(ActionBase sender)
        {
            Program.Invoke(Program.MainWindow, PopulateGridView);
        }

        private void PopulateGridView()
        {
            foreach (VM vm in VmsToMigrateAction.MigratableVms)
            {
                VmsGridView.Rows.Add(new VmGridRow(vm));
            }
            UpdateRequiredMemory();
        }

        public List<VM> SelectedVMsToSuspend
        {
            get
            {
                List<VM> vms = new List<VM>();

                foreach (VmGridRow row in VmsGridView.Rows)
                {
                    if (row.Action != ActionCellAction.Suspend)
                        continue;

                    vms.Add(row.Vm);
                }

                return vms;
            }
        }

        public List<VM> SelectedVMsToShutdown
        {
            get
            {
                List<VM> vms = new List<VM>();

                foreach (VmGridRow row in VmsGridView.Rows)
                {
                    if (row.Action != ActionCellAction.Shutdown)
                        continue;

                    vms.Add(row.Vm);
                }

                return vms;
            }
        }

        private void VmsGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            UpdateRequiredMemory();
        }

        private void VmsGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (VmsGridView.IsCurrentCellDirty)
            {
                VmsGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }

        }

    }

    public class VmGridRow : DataGridViewRow
    {
        public VM Vm;
        private DataGridViewExImageCell ImageCell = new DataGridViewExImageCell();
        private DataGridViewTextBoxCell NameCell = new DataGridViewTextBoxCell();
        private DataGridViewTextBoxCell MemoryCell = new DataGridViewTextBoxCell();
        private DataGridViewComboBoxCell ActionCell = new DataGridViewComboBoxCell();

        public VmGridRow(VM vm)
        {
            Vm = vm;

            Cells.Add(ImageCell);
            Cells.Add(NameCell);
            Cells.Add(MemoryCell);
            Cells.Add(ActionCell);
            UpdateDetails();
        }

        public ActionCellAction Action
        {
            get
            {
                return ((ActionCellItem)ActionCell.Value).Action;
            }
        }

        private void UpdateDetails()
        {
            ImageCell.Value = Images.GetImage16For(Images.GetIconFor(Vm));
            NameCell.Value = Vm.Name;
            MemoryCell.Value = Util.MemorySizeStringSuitableUnits(Vm.memory_dynamic_max, true);
            ActionCell.ValueType = typeof(ActionCellItem);
            ActionCell.ValueMember = "ActionCell";
            ActionCell.DisplayMember = "Text";
            ActionCell.Items.Clear();
            ActionCell.Items.Add(ActionCellItem.MigrateAction);
            ActionCell.Items.Add(ActionCellItem.SuspendAction);
            ActionCell.Items.Add(ActionCellItem.ShutdownAction);
            ActionCell.Value = ActionCell.Items[0];
        }
    }

    public class ActionCellItem
    {
        public static ActionCellItem MigrateAction = new ActionCellItem(ActionCellAction.Migrate);
        public static ActionCellItem SuspendAction = new ActionCellItem(ActionCellAction.Suspend);
        public static ActionCellItem ShutdownAction = new ActionCellItem(ActionCellAction.Shutdown);

        public readonly ActionCellAction Action;

        public ActionCellItem(ActionCellAction action)
        {
            Action = action;
        }

        public ActionCellItem ActionCell
        {
            get
            {
                return this;
            }
        }

        public string Text
        {
            get
            {
                switch (Action)
                {
                    case ActionCellAction.Migrate:
                        return Messages.MIGRATE;
                    case ActionCellAction.Suspend:
                        return Messages.SUSPEND;
                    case ActionCellAction.Shutdown:
                        return Messages.SHUTDOWN;
                }
                return Action.ToString();
            }
        }
    }

    public enum ActionCellAction { Migrate, Suspend, Shutdown }
}