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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Wizards.NewPolicyWizard;
using XenAPI;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using XenAdmin.Core;
using XenAdmin.Dialogs.VMProtectionRecovery;
using XenCenterLib;


namespace XenAdmin.Dialogs.VMSSPolicy
{
    public partial class VMSSDialog : XenDialogBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly Pool Pool;
        public VMSSDialog(Pool pool)
        {
            Pool = pool;
            InitializeComponent();
            RefreshPoolTitle(pool);
            localServerTime1.Pool = pool;
            chevronButton1.Text = Messages.SHOW_RUN_HISTORY;
            chevronButton1.Image = Properties.Resources.PDChevronDown;
            policyHistory1.Visible = false;
        }

        private void RefreshPoolTitle(Pool pool)
        {
            int protectedVMs = 0;
            int realVMs = 0;
            foreach (var vm in pool.Connection.Cache.VMs)
            {
                if (vm.is_a_real_vm && vm.Show(Properties.Settings.Default.ShowHiddenVMs))
                {
                    realVMs++;
                    if (vm.Connection.Resolve(vm.snapshot_policy) != null)
                        protectedVMs++;
                }
            }
            labelPolicyTitle.Text = string.Format(Helpers.IsPool(pool.Connection)
                                                      ? Messages.VMSS_SCHEDULED_SNAPSHOTS_DEFINED_FOR_POOL
                                                      : Messages.VMSS_SCHEDULED_SNAPSHOTS_DEFINED_FOR_SERVER,
                                                  pool.Name.Ellipsise(45), protectedVMs, realVMs);
        }
      
        void VMSSCollectionChanged(object sender, EventArgs e)
        {
            LoadPolicies();
        }

        private void LoadPolicies()
        {
            dataGridView1.SuspendLayout();
            var selectedPolicies = currentSelected;
            dataGridView1.Rows.Clear();
            foreach (var policy in Pool.Connection.Cache.VMSSs)
            {
                if (dataGridView1.ColumnCount > 0)
                    dataGridView1.Rows.Add(new VMSSPolicyRow(policy));
            }
            RefreshButtons();
            if (selectedPolicies != null)
            {
                foreach (VMSSPolicyRow row in dataGridView1.Rows)
                {

                    var VMSS = row.Policies as VMSS;
                    var _selectedPolicies = selectedPolicies as VMSS;
                    if (VMSS.uuid == _selectedPolicies.uuid)
                    {
                        dataGridView1.ClearSelection();
                        row.Selected = true;
                        break;
                    }
                }
            }
            RefreshPoolTitle(Pool);
            dataGridView1.ResumeLayout();
        }

        private class VMSSPolicyRow : DataGridViewRow
        {
            private DataGridViewTextBoxCell _name = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _numVMs = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _nextRunTime = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _status = new DataGridViewTextBoxCell();
            private DataGridViewTextAndImageCell _lastResult = new DataGridViewTextAndImageCell();
            public readonly VMSS Policies;
            public VMSSPolicyRow(VMSS policy)
            {
                Cells.Add(_name);
                Cells.Add(_status);
                Cells.Add(_numVMs);
                Cells.Add(_nextRunTime);
                Cells.Add(_lastResult);
                Policies = policy;
                RefreshRow();
            }

            private DateTime? GetPolicyDateTime(Func<DateTime> getDateTimeFunc)
            {
                try
                {
                    return getDateTimeFunc();
                }
                catch (Exception e)
                {
                    log.Error("An error occurred while obtaining Policy date time: ", e);
                    return null;
                }
            }

            private void RefreshRow()
            {
                _name.Value = Policies.Name;
                _numVMs.Value = Policies.VMs.FindAll(vm => Policies.Connection.Resolve(vm).is_a_real_vm).Count;
                _status.Value = Policies.is_policy_enabled ? Messages.ENABLED : Messages.DISABLED;
                if (Policies.is_backup_running)
                    _status.Value = Messages.RUNNING_SNAPSHOTS;
                _lastResult.Value = Policies.LastResult;
                if (Policies.LastResult == Messages.FAILED)
                    _lastResult.Image = Properties.Resources._075_WarningRound_h32bit_16;
                else if (Policies.LastResult == Messages.NOT_YET_RUN)
                    _lastResult.Image = null;
                else
                    _lastResult.Image = Properties.Resources._075_TickRound_h32bit_16;

                DateTime? nextRunTime = GetPolicyDateTime(() => Policies.GetNextRunTime());
                _nextRunTime.Value = nextRunTime.HasValue
                                            ? HelpersGUI.DateTimeToString(nextRunTime.Value, Messages.DATEFORMAT_DMY_HM,
                                                                        true)
                                            : Messages.VM_PROTECTION_POLICY_HOST_NOT_LIVE;
            }
        }

        private void buttonNew_Click(object sender, System.EventArgs e)
        {
            new NewPolicyWizardSpecific<VMSS>(Pool).Show(this);
        }

        private void buttonCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void VMSSDialog_Load(object sender, EventArgs e)
        {
            LoadPolicies();
            localServerTime1.GetServerTime();
            Pool.Connection.Cache.RegisterBatchCollectionChanged<VMSS>(VMSSCollectionChanged);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            var selectedPolicies = new List<VMSS>();
            int numberOfProtectedVMs = 0;
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                var policy = ((VMSSPolicyRow)row).Policies;
                selectedPolicies.Add(policy);
                numberOfProtectedVMs += policy.VMs.Count;
            }
            string text = "";
            if (selectedPolicies.Count == 1)
            {
                text = String.Format(numberOfProtectedVMs == 0 ? Messages.CONFIRM_DELETE_POLICY_0 : Messages.CONFIRM_DELETE_POLICY, selectedPolicies[0].Name, numberOfProtectedVMs);
            }
            else
            {
                text = string.Format(numberOfProtectedVMs == 0 ? Messages.CONFIRM_DELETE_POLICIES_0 : Messages.CONFIRM_DELETE_POLICIES, numberOfProtectedVMs);
            }

            if (new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, text, Messages.DELETE_VM_PROTECTION_TITLE),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo).ShowDialog(this) == DialogResult.Yes)
            {
                (new DestroyPolicyAction<VMSS>(Pool.Connection, selectedPolicies)).RunAsync();
                
            }

        }

        private VMSS currentSelected = null;

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            RefreshButtons();
        }

        private void RefreshButtons()
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                currentSelected = ((VMSSPolicyRow)dataGridView1.SelectedRows[0]).Policies;
                buttonEnable.Text = currentSelected.is_policy_enabled ? Messages.DISABLE : Messages.ENABLE;
                buttonEnable.Enabled = currentSelected.VMs.Count == 0 && !currentSelected.is_policy_enabled ? false : true;
                buttonProperties.Enabled = true;
                buttonRunNow.Enabled = currentSelected.is_policy_enabled && !currentSelected.is_backup_running;

            }
            else
            {
                currentSelected = null;
                buttonProperties.Enabled = buttonEnable.Enabled = buttonRunNow.Enabled = false;
                policyHistory1.Clear();
            }
            policyHistory1.RefreshTab(currentSelected);
            buttonDelete.Enabled = (dataGridView1.SelectedRows.Count != 0);
        }

        private void VMProtectionPoliciesDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Pool.Connection.Cache.DeregisterBatchCollectionChanged<VMPP>(VMSSCollectionChanged);
        }

        private void buttonEnable_Click(object sender, EventArgs e)
        {
            if (currentSelected != null)
            {
                var action = new ChangePolicyEnabledAction<VMSS>(currentSelected);
                action.RunAsync();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                var vmss = ((VMSSPolicyRow)dataGridView1.SelectedRows[0]).Policies;
                var action = new RunPolicyNowAction<VMSS>(vmss);
                action.Completed += action_Completed;
                buttonRunNow.Enabled = false;
                action.RunAsync();
            }
        }

        void action_Completed(ActionBase sender)
        {
            Program.Invoke(Program.MainWindow, RefreshButtons);
        }

        private void buttonProperties_Click(object sender, EventArgs e)
        {
            using (PropertiesDialog propertiesDialog = new PropertiesDialog(currentSelected))
            {
                propertiesDialog.ShowDialog(this);
            }
        }


        private void chevronButton1_ButtonClick(object sender, EventArgs e)
        {
            if (chevronButton1.Text == Messages.HIDE_RUN_HISTORY)
            {
                chevronButton1.Text=Messages.SHOW_RUN_HISTORY;
                chevronButton1.Image=Properties.Resources.PDChevronDown;
                policyHistory1.Visible = false;
            }
            else
            {
                chevronButton1.Text=Messages.HIDE_RUN_HISTORY;
                chevronButton1.Image=Properties.Resources.PDChevronUp;
                policyHistory1.Visible = true;
            }
        }

        private void chevronButton1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                chevronButton1_ButtonClick(sender, e);
        }
    }


}
