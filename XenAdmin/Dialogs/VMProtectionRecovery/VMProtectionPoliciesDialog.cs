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
using XenAdmin.Alerts;
using System.Linq;

namespace XenAdmin.Dialogs.VMProtection_Recovery
{
    public partial class VMProtectionPoliciesDialog: XenDialogBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly Pool Pool;
        public VMProtectionPoliciesDialog(Pool pool)
            : base(pool.Connection)
        {
            Pool = pool;
            InitializeComponent();      
            localServerTime1.Pool = pool;
            chevronButton1.Text = Messages.SHOW_RUN_HISTORY;
            chevronButton1.Image = Properties.Resources.PDChevronDown;
            policyHistory1.Visible = false;
            policyHistory1.pool = pool;
            
        }
        public VMProtectionPoliciesDialog() { }

        protected virtual void chevronButton1_KeyDown(object sender, KeyEventArgs e) {}
        protected virtual void chevronButton1_ButtonClick(object sender, EventArgs e) { }
        protected virtual void buttonProperties_Click(object sender, EventArgs e) { }
        protected virtual void button1_Click(object sender, EventArgs e) { }
        protected virtual void buttonEnable_Click(object sender, EventArgs e) { }
        protected virtual void VMProtectionPoliciesDialog_FormClosed(object sender, FormClosedEventArgs e) { }
        protected virtual void dataGridView1_SelectionChanged(object sender, EventArgs e) { }
        protected virtual void VMProtectionPoliciesDialog_Load(object sender, EventArgs e) { }
        protected virtual void buttonCancel_Click(object sender, System.EventArgs e) { }
        protected virtual void buttonNew_Click(object sender, System.EventArgs e) { }
        protected virtual void buttonDelete_Click(object sender, EventArgs e) { }
        

        public class PolicyRow : DataGridViewRow
        {
            private DataGridViewTextBoxCell _name = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _numVMs = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _nextRunTime = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _status = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _nextArchiveRuntime = new DataGridViewTextBoxCell();
            private DataGridViewTextAndImageCell _lastResult = new DataGridViewTextAndImageCell();
            public readonly IVMPolicy _policy;
            public PolicyRow(IVMPolicy policy)
            {
                Cells.Add(_name);
                Cells.Add(_status);
                Cells.Add(_numVMs);
                Cells.Add(_nextRunTime);
                Cells.Add(_nextArchiveRuntime);
                Cells.Add(_lastResult);
                _policy = policy;
                RefreshRow();
            }

            private DateTime? GetVMPPDateTime(Func<DateTime> getDateTimeFunc)
            {
                try
                {
                    return getDateTimeFunc();
                }
                catch (Exception e)
                {
                    log.Error("An error occurred while obtaining VMPP date time: ", e);
                    return null;
                }
            }

            private void RefreshRow()
            {
                _name.Value = _policy.Name;
                _numVMs.Value = _policy.VMs.FindAll(vm => _policy.Connection.Resolve(vm).is_a_real_vm).Count;
                _status.Value = _policy.is_enabled ? Messages.ENABLED : Messages.DISABLED;
                if (_policy.is_running)
                    _status.Value = Messages.RUNNING_SNAPSHOTS;
                if (this._policy.hasArchive)
                {
                    if (_policy.is_archiving)
                        _status.Value = Messages.RUNNING_ARCHIVE;
                }
                _lastResult.Value = _policy.LastResult;
                if (_policy.LastResult == Messages.FAILED)
                    _lastResult.Image = Properties.Resources._075_WarningRound_h32bit_16;
                else if (_policy.LastResult == Messages.NOT_YET_RUN)
                    _lastResult.Image = null;
                else
                    _lastResult.Image = Properties.Resources._075_TickRound_h32bit_16;

                DateTime? nextRunTime = GetVMPPDateTime(() => _policy.GetNextRunTime());
                _nextRunTime.Value = nextRunTime.HasValue
                                         ? HelpersGUI.DateTimeToString(nextRunTime.Value, Messages.DATEFORMAT_DMY_HM,
                                                                       true)
                                         : Messages.VM_PROTECTION_POLICY_HOST_NOT_LIVE;
                if (this._policy.hasArchive)
                {
                    DateTime? nextArchiveRuntime = GetVMPPDateTime(() => _policy.GetNextArchiveRunTime());
                    _nextArchiveRuntime.Value = nextArchiveRuntime.HasValue
                                                    ? nextArchiveRuntime == DateTime.MinValue
                                                            ? Messages.NEVER
                                                            : HelpersGUI.DateTimeToString(nextArchiveRuntime.Value,
                                                                                        Messages.DATEFORMAT_DMY_HM, true)
                                                    : Messages.VM_PROTECTION_POLICY_HOST_NOT_LIVE;
                }

            }
        }
    }

}

namespace XenAdmin.Dialogs.VMPolicies
{
    public class VMPoliciesDialogSpecific<T> : XenAdmin.Dialogs.VMProtection_Recovery.VMProtectionPoliciesDialog where T : XenObject<T>
    {
        public VMPoliciesDialogSpecific() { }
        public VMPoliciesDialogSpecific(Pool pool)
            : base(pool)
        {
            RefreshPoolTitle(pool);
            if (VMGroup<T>.isVMPolicyVMPP)
                SetupDeprecationBanner();

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
                    if (vm.Connection.Resolve(VMGroup<T>.VmToGroup(vm)) != null)
                        protectedVMs++;
                }
            }
            this.Text = VMGroup<T>.VMPolicyDialogTitle;
            label2.Text = VMGroup<T>.VMPolicyDialogText;
            labelPolicyTitle.Text = string.Format(Helpers.IsPool(pool.Connection)
                                                        ? VMGroup<T>.VMPolicyDialogSchedulesInPool
                                                        : VMGroup<T>.VMPolicyDialogSchedulesInServer,
                                                    pool.Name.Ellipsise(45), protectedVMs, realVMs);
        }

        void VMPPCollectionChanged(object sender, EventArgs e)
        {
            LoadPolicies();
        }

        private void LoadPolicies()
        {
            dataGridView1.SuspendLayout();
            var selectedPolicy = currentSelected;
            dataGridView1.Rows.Clear();
	    var policyList = VMGroup<T>.VMPolicies(Pool.Connection.Cache);

            /* creating a dictionary to hold (policy_uuid, message list) */

            Dictionary<string, List<XenAPI.Message>> policyMessage = new Dictionary<string, List<XenAPI.Message>>();
    
            /* populate the dictionary with policy uuid */

            foreach (var policy in policyList)
            {
                policy.PolicyAlerts.Clear();
                List<XenAPI.Message> messageList = new List<XenAPI.Message>();
                policyMessage.Add(policy.uuid, messageList);
            }
       
            /* iterate through all messages and populate the dictionary with message list */

            if (!VMGroup<T>.isVMPolicyVMPP)
            {
                var messages = Pool.Connection.Cache.Messages;
                List<XenAPI.Message> value = new List<XenAPI.Message>();

                foreach (var message in messages)
                {
                    if (message.cls == cls.VMSS)
                    {
                        if (policyMessage.TryGetValue(message.obj_uuid, out value))
                        {
                            value.Add(message);
                        }

                    }
                }
            }

            /* add only 10 messages for each policy and referesh the rows*/
            
            foreach (var policy in policyList)
            {
                /* message list need not be always sorted */

                var messageListSorted = policyMessage[policy.uuid].OrderByDescending(message => message.timestamp).ToList();
                for (int messageCount = 0; messageCount < 10 && messageCount < messageListSorted.Count; messageCount++)
                {
                    policy.PolicyAlerts.Add(new PolicyAlert(messageListSorted[messageCount].priority, messageListSorted[messageCount].name, messageListSorted[messageCount].timestamp, messageListSorted[messageCount].body, policy.Name));
                }
                if (dataGridView1.ColumnCount > 0)
                    dataGridView1.Rows.Add(new PolicyRow(policy));
            }

           RefreshButtons();
            if (selectedPolicy != null)
            {
                foreach (PolicyRow row in dataGridView1.Rows)
                {
                    if (row._policy.uuid == selectedPolicy.uuid)
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



        protected override void buttonNew_Click(object sender, System.EventArgs e)
        {
            new NewPolicyWizardSpecific<T>(Pool).Show(this);
        }

        protected override void buttonCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        protected override void VMProtectionPoliciesDialog_Load(object sender, EventArgs e)
        {
            if (!VMGroup<T>.isVMPolicyVMPP)
            {
                this.dataGridView1.Columns["ColumnNextArchive"].Visible = false;
            }
            LoadPolicies();
            localServerTime1.GetServerTime();
            Pool.Connection.Cache.RegisterBatchCollectionChanged<T>(VMPPCollectionChanged);
        }

        protected override void buttonDelete_Click(object sender, EventArgs e)
        {
            var selectedPolicies = new List<IVMPolicy>();
            int numberOfProtectedVMs = 0;
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                var policy = (((PolicyRow)row)._policy);
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

            using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, text, Messages.DELETE_VM_PROTECTION_TITLE),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo))
            {
                if (dlg.ShowDialog(this) == DialogResult.Yes)
                    new DestroyPolicyAction<T>(Pool.Connection, selectedPolicies).RunAsync();
            }
        }

        private IVMPolicy currentSelected = null;

        protected override void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            RefreshButtons();
        }

        private void RefreshButtons()
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                currentSelected = ((PolicyRow)dataGridView1.SelectedRows[0])._policy;
                buttonEnable.Text = currentSelected.is_enabled? Messages.DISABLE : Messages.ENABLE;
                buttonEnable.Enabled = currentSelected.VMs.Count == 0 && !currentSelected.is_enabled? false : true;
                buttonProperties.Enabled = true;
                buttonRunNow.Enabled = currentSelected.is_enabled && !currentSelected.is_archiving && !currentSelected.is_running;

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

        protected override void VMProtectionPoliciesDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Pool.Connection.Cache.DeregisterBatchCollectionChanged<T>(VMPPCollectionChanged);
        }

        protected override void buttonEnable_Click(object sender, EventArgs e)
        {
            if (currentSelected != null)
            {
                var action = new ChangePolicyEnabledAction<T>(currentSelected);
                action.RunAsync();
            }
        }
        
        protected override void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                var policy = ((PolicyRow)dataGridView1.SelectedRows[0])._policy;
                var action = new RunPolicyNowAction<T>(policy);
                action.Completed += action_Completed;
                buttonRunNow.Enabled = false;
                action.RunAsync();
            }
        }

        void action_Completed(ActionBase sender)
        {
            Program.Invoke(Program.MainWindow, RefreshButtons);
        }

        private void SetupDeprecationBanner()
        {
            if (deprecationBanner != null && !Helpers.ClearwaterOrGreater(Pool.Connection))
            {
                deprecationBanner.AppliesToVersion = Messages.XENSERVER_6_2;
                deprecationBanner.BannerType = DeprecationBanner.Type.Removal;
                deprecationBanner.FeatureName = Messages.VMPP;
                deprecationBanner.LinkUri = HiddenFeatures.LinkLabelHidden ? null : new Uri(InvisibleMessages.VMPR_DEPRECATION_URL);
                deprecationBanner.Visible = true;
            }
        }

        protected override void buttonProperties_Click(object sender, EventArgs e)
        {
            using (PropertiesDialog propertiesDialog = new PropertiesDialog((T)currentSelected))
            {
                propertiesDialog.ShowDialog(this);
            }
        }


        protected override void chevronButton1_ButtonClick(object sender, EventArgs e)
        {
            if (chevronButton1.Text == Messages.HIDE_RUN_HISTORY)
            {
                chevronButton1.Text = Messages.SHOW_RUN_HISTORY;
                chevronButton1.Image = Properties.Resources.PDChevronDown;
                policyHistory1.Visible = false;
            }
            else
            {
                chevronButton1.Text = Messages.HIDE_RUN_HISTORY;
                chevronButton1.Image = Properties.Resources.PDChevronUp;
                policyHistory1.Visible = true;
            }
        }

        protected override void chevronButton1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                chevronButton1_ButtonClick(sender, e);
        }

        internal override string HelpName
        {
            get
            {
                if (!VMGroup<T>.isVMPolicyVMPP)
                {
                    return "VMSnapshotSchedulesDialog";
                }
                return "VMProtectionPoliciesDialog";
            }
        }

    }
}
