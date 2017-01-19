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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class ControlDomainMemoryDialog : XenDialogBase
    {
        private Host host;
        private Host_metrics hostMetrics;
        private double origMemory;

        public const int MAXIMUM_DOM0_MEMORY_GB =  256;

        public ControlDomainMemoryDialog(Host host)
            : base(host.Connection)
        {
            if (host == null) throw new ArgumentNullException("host");

            InitializeComponent();
            this.host = host;
            this.host.PropertyChanged += Server_PropertyChanged;
            hostMetrics = connection.Resolve(this.host.metrics);
            if (hostMetrics != null)
                hostMetrics.PropertyChanged += Server_PropertyChanged;
            Text = string.Format(Messages.CONTROL_DOMAIN_MEMORY_DIALOG_TITLE, this.host.Name);
            Populate();
        }

        private void UpdateMaintenanceWarning()
        {
            Host_metrics metrics = host.Connection.Resolve(host.metrics);
            bool maintenanceMode = host.MaintenanceMode || (metrics != null && !metrics.live);

            maintenanceWarningImage.Visible = maintenanceWarningLabel.Visible = maintenanceModeLinkLabel.Visible = !maintenanceMode;
            hostRebootWarningImage.Visible = hostRebootWarningLabel.Visible = maintenanceMode;

            memorySpinner.Enabled = maintenanceMode;
        }

        private void Populate()
        {
            VM vm = host.ControlDomainZero;

            // Since updates come in dribs and drabs, avoid error if new max and min arrive
            // out of sync and maximum < minimum.
            if (vm.memory_dynamic_max >= vm.memory_dynamic_min &&
                vm.memory_static_max >= vm.memory_static_min)
            {
                double min = vm.memory_static_min;
                double max = Math.Min(vm.memory_dynamic_min + host.memory_available_calc, MAXIMUM_DOM0_MEMORY_GB * Util.BINARY_GIGA);
                double value = vm.memory_dynamic_min;
                // Avoid setting the range to exclude the current value: CA-40041
                if (value > max)
                    max = value;
                if (value < min)
                    min = value;
                memorySpinner.SetRange(0, MAXIMUM_DOM0_MEMORY_GB * Util.BINARY_GIGA); // reset spinner limits
                memorySpinner.Initialize(Messages.CONTROL_DOMAIN_MEMORY_LABEL, null, value, max);
                memorySpinner.SetRange(min, max);
            }
            origMemory = memorySpinner.Value;
            UpdateMaintenanceWarning();
        }

        private bool HasChanged()
        {
            return memorySpinner.Value != origMemory;
        }

        private bool SaveChanges()
        {
            if (!HasChanged())
                return false;

            var mem = memorySpinner.Value;

            DialogResult dialogResult;
            using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.CONFIRM_CHANGE_CONTROL_DOMAIN_MEMORY, Messages.XENCENTER),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo))
            {
                dialogResult = dlg.ShowDialog(this);
            }

            if (DialogResult.Yes != dialogResult)
                return false;

            var actions = new List<AsyncAction>();
            var action = new ChangeControlDomainMemoryAction(host, (long)mem, false);
            actions.Add(action);

            actions.Add(new RebootHostAction(host, AddHostToPoolCommand.NtolDialog));

            var multipleAction = new MultipleAction(connection, 
                string.Format(Messages.ACTION_CHANGE_CONTROL_DOMAIN_MEMORY, host.Name), 
                string.Format(Messages.ACTION_CHANGE_CONTROL_DOMAIN_MEMORY, host.Name), 
                Messages.COMPLETED, actions, true, false, true);

            multipleAction.RunAsync();
            return true;
        }

        private void Cleanup()
        {
            host.PropertyChanged -= Server_PropertyChanged;
            if (hostMetrics != null)
                hostMetrics.PropertyChanged -= Server_PropertyChanged;
        }

        private void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Program.Invoke(this, Populate);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (SaveChanges())
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void maintenanceModeLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new HostMaintenanceModeCommand(Program.MainWindow, host, HostMaintenanceModeCommandParameter.Enter).Execute();
        }

        private void ControlDomainMemoryDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cleanup();
        }
    }
}
