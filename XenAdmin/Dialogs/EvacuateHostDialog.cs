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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAdmin.Commands;
using XenAdmin.Actions.VMActions;
using XenAdmin.Actions.Wlb;
using XenCenterLib;

namespace XenAdmin.Dialogs
{
    public partial class EvacuateHostDialog : XenDialogBase
    {
        #region Private fields
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Host _host;
        private readonly Pool _pool;
        private WlbEvacuateRecommendationsAction vmErrorsAction;
        private EvacuateHostAction hostAction;
        private ToStringWrapper<Host> hostSelection;
        private Dictionary<string, AsyncAction> solveActionsByVmUuid;
        private Dictionary<XenRef<VM>, String[]> reasons;
        private string elevatedUsername;
        private string elevatedPassword;
        private Session elevatedSession;
        private volatile bool _rebuildInProgress;
        private volatile bool _rebuildRequired;

        public static readonly string[] RbacMethods =
        {
            "host.remove_from_other_config", // save VM list
            "host.add_to_other_config",
            "host.disable", // disable the host

            "pbd.plug", // Repair SR and install tools
            "pbd.create",

            "vm.suspend", // Suspend VMs

            "vbd.async_eject", // Change ISO
            "vbd.async_insert",
            "vbd.eject",
            "vbd.insert",
        };

        #endregion

        public EvacuateHostDialog(Host host, string elevatedUserName, string elevatedPassword, Session elevatedSession)
            : base(host.Connection)
        {
            InitializeComponent();
            labelCoordinatorBlurb.Text = string.Format(labelCoordinatorBlurb.Text, BrandManager.BrandConsole);

            this.elevatedUsername = elevatedUserName;
            this.elevatedPassword = elevatedPassword;
            this.elevatedSession = elevatedSession;

            _host = host;
            _pool = Helpers.GetPoolOfOne(_host.Connection);

            if (!_host.IsCoordinator() || connection.Cache.HostCount <= 1)
                tableLayoutPanelNewCoordinator.Visible = false;
            else
                tableLayoutPanelPSr.Visible = false;

            tableLayoutPanelWlb.Visible = _pool.IsVisible() && Helpers.WlbEnabled(_host.Connection) &&
                                          WlbServerState.GetState(_pool) == WlbServerState.ServerState.Enabled;

            tableLayoutPanelSpinner.Visible = false;
            tableLayoutPanelStatus.Visible = false;
            progressBar1.Visible = false;

            _host.PropertyChanged += hostUpdate;
            _pool.PropertyChanged += poolUpdate;

            ActiveControl = CloseButton;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Text = string.Format(Messages.EVACUATE_HOST_DIALOG_TITLE, _host.Name());
            Rebuild();
        }

        private void Rebuild()
        {
            if (_rebuildInProgress || hostAction != null && !hostAction.IsCompleted)
            {
                _rebuildRequired = true;
                return;
            }

            Program.Invoke(this, () =>
            {
                _rebuildInProgress = true;
                tableLayoutPanelStatus.Visible = false;
                progressBar1.Visible = false;
                DisableButtons();
                ClearVMs();
                ClearHosts();
                labelSpinner.Text = Messages.SCANNING_VMS;
                spinnerIcon1.StartSpinning();
                tableLayoutPanelSpinner.Visible = true;
                Scan();
            });
        }

        private void Scan()
        {
            //Save Evacuated VMs for later
            var saveVMsAction = new DelegatedAsyncAction(connection,
                Messages.SAVING_VM_PROPERTIES_ACTION_TITLE, Messages.SAVING_VM_PROPERTIES_ACTION_DESC, Messages.COMPLETED,
                session => _host.SaveEvacuatedVMs(session), true, "host.remove_from_other_config", "host.add_to_other_config");
            
            saveVMsAction.RunAsync(GetSudoElevationResult());

            vmErrorsAction = new WlbEvacuateRecommendationsAction(_host);
            vmErrorsAction.Completed += VmErrorsAction_Completed;
            vmErrorsAction.RunAsync(GetSudoElevationResult());
        }

        private void VmErrorsAction_Completed(ActionBase obj)
        {
            if (!(obj is WlbEvacuateRecommendationsAction action))
                return;

            try
            {
                Program.Invoke(this, () =>
                {
                    if (action.Succeeded)
                    {
                        reasons = action.CantEvacuateReasons;
                        tableLayoutPanelSpinner.Visible = false;
                        spinnerIcon1.StopSpinning();
                        PopulateVMs();
                        PopulateHosts();
                        EnableButtons();
                    }
                    else
                    {
                        spinnerIcon1.ShowFailureImage();
                        labelSpinner.Text = action.Exception.Message;
                    }
                });
            }
            finally
            {
                _rebuildInProgress = false;
                if (_rebuildRequired)
                {
                    _rebuildRequired = false;
                    Rebuild();
                }
            }
        }

        
        private void hostUpdate(object o, PropertyChangedEventArgs args)
        {
            if (args == null || args.PropertyName == "name_label" || args.PropertyName == "resident_VMs")
                Rebuild();
        }

        private void poolUpdate(object o, PropertyChangedEventArgs args)
        {
            if (args == null || args.PropertyName == "is_psr_pending" ||
                args.PropertyName == "allowed_operations" || args.PropertyName == "current_operations")
                Program.Invoke(this, ()=>
                {
                    EnableComboBox();
                    EnableButtons();
                });
        }
        
        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "name_label" || args.PropertyName == "allowed_operations" ||
                args.PropertyName == "current_operations")
            {
                Program.Invoke(this, () =>
                {
                    if (sender is VM vm)
                        dataGridViewVms.Rows.Cast<VmPrecheckRow>()
                            .FirstOrDefault(r => r.vm.opaque_ref == vm.opaque_ref)?.Refresh();
                });
            }
            else if (args.PropertyName == "virtualisation_status" || args.PropertyName == "resident_on")
            {
                Rebuild();
            }
            else if (args.PropertyName == "guest_metrics")
            {
                VM v = sender as VM;
                VM_guest_metrics gm = connection.Resolve(v?.guest_metrics);
                if (gm == null)
                    return;

                gm.PropertyChanged -= gm_PropertyChanged;
                gm.PropertyChanged += gm_PropertyChanged;
            }
        }

        private void gm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PV_drivers_version")
                Rebuild();
        }

        private void host_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args == null || args.PropertyName == "name_label" ||
                args.PropertyName == "enabled" || args.PropertyName == "metrics")
            {
                Program.Invoke(this, () =>
                {
                    ClearHosts();
                    PopulateHosts();
                });
            }
        }

        
        private void ClearVMs()
        {
            solveActionsByVmUuid = new Dictionary<string, AsyncAction>();

            foreach (DataGridViewRow row in dataGridViewVms.Rows)
            {
                if (row is VmPrecheckRow precheckRow)
                {
                    DeregisterVmRowEvents(precheckRow);
                    solveActionsByVmUuid.Add(precheckRow.vm.uuid, precheckRow.SolutionAction);
                }
            }

            dataGridViewVms.Rows.Clear();
        }

        private void PopulateVMs()
        {
            Program.AssertOnEventThread();

            try
            {
                dataGridViewVms.SuspendLayout();

                foreach (VM vm in connection.ResolveAll(_host.resident_VMs))
                {
                    if (vm.is_control_domain || vm.is_a_template)
                        continue;

                    vm.PropertyChanged += VM_PropertyChanged;
                    solveActionsByVmUuid.TryGetValue(vm.uuid, out var action);
                    var row = new VmPrecheckRow(vm) {SolutionAction = action};
                    dataGridViewVms.Rows.Add(row);
                }

                foreach (KeyValuePair<XenRef<VM>, string[]> kvp in reasons)
                {
                    if (kvp.Value[0].Trim().ToLower() != "wlb")
                        UpdateVMWithError(kvp.Value, kvp.Key.opaque_ref);
                }
            }
            finally
            {
                dataGridViewVms.ResumeLayout();
            }
        }

        private void ClearHosts()
        {
            if (tableLayoutPanelNewCoordinator.Visible)
            {
                NewCoordinatorComboBox.Enabled = false;
                hostSelection = NewCoordinatorComboBox.SelectedItem as ToStringWrapper<Host>;
                NewCoordinatorComboBox.Items.Clear();
            }
        }

        private void PopulateHosts()
        {
            Program.AssertOnEventThread();

            if (!tableLayoutPanelNewCoordinator.Visible)
                return;

            try
            {
                NewCoordinatorComboBox.BeginUpdate();
                var hosts = connection.Cache.Hosts.Where(h => h.opaque_ref != _host.opaque_ref).ToList();
                hosts.Sort();

                foreach (Host host in hosts)
                {
                    host.PropertyChanged -= host_PropertyChanged;
                    host.PropertyChanged += host_PropertyChanged;

                    Host_metrics metrics = connection.Resolve(host.metrics);
                    if (host.enabled && metrics != null && metrics.live)
                    {
                        var item = new ToStringWrapper<Host>(host, host.Name());
                        NewCoordinatorComboBox.Items.Add(item);

                        if (hostSelection != null && host.opaque_ref == hostSelection.item.opaque_ref)
                            NewCoordinatorComboBox.SelectedItem = item;
                    }
                }

                //Update NewCoordinatorComboBox for host power on recommendation
                foreach (KeyValuePair<XenRef<VM>, string[]> kvp in reasons)
                {
                    var vm = connection.Resolve(kvp.Key);
                    if (vm != null && vm.is_control_domain)
                    {
                        Host powerOnHost = connection.Cache.Hosts.FirstOrDefault(h =>
                            h.uuid == kvp.Value[(int)RecProperties.ToHost]);

                        if (powerOnHost != null)
                        {
                            var hostToAdd = new ToStringWrapper<Host>(powerOnHost, powerOnHost.Name());

                            if (NewCoordinatorComboBox.Items.Cast<ToStringWrapper<Host>>().FirstOrDefault(i =>
                                i.item.opaque_ref == powerOnHost.opaque_ref) == null)
                            {
                                powerOnHost.PropertyChanged -= host_PropertyChanged;
                                powerOnHost.PropertyChanged += host_PropertyChanged;
                                NewCoordinatorComboBox.Items.Add(hostToAdd);
                            }
                        }
                    }
                }

                if (NewCoordinatorComboBox.SelectedItem == null && NewCoordinatorComboBox.Items.Count > 0)
                    NewCoordinatorComboBox.SelectedIndex = 0;
            }
            finally
            {
                EnableComboBox();
                NewCoordinatorComboBox.EndUpdate();
            }
        }

        private void EnableComboBox()
        {
            if (tableLayoutPanelNewCoordinator.Visible)
            {
                bool enable = connection.Cache.Hosts.Any(Host.RestrictPoolSecretRotation) ||
                              !_pool.is_psr_pending &&
                              _pool.allowed_operations.Contains(pool_allowed_operations.designate_new_master);

                var msg = string.Empty;

                if (!enable)
                {
                    if (_pool.is_psr_pending)
                        msg = Messages.ROTATE_POOL_SECRET_PENDING_NEW_COORDINATOR;
                    else if (_pool.current_operations.Values.Contains(pool_allowed_operations.ha_enable))
                        msg = Messages.EVACUATE_HOST_HA_ENABLING;
                    else if (_pool.current_operations.Values.Contains(pool_allowed_operations.ha_disable))
                        msg = Messages.EVACUATE_HOST_HA_DISABLING;
                    else if (_pool.current_operations.Values.Contains(pool_allowed_operations.cluster_create))
                        msg = Messages.EVACUATE_HOST_CLUSER_CREATING;
                }

                NewCoordinatorComboBox.Enabled = enable;
                labelWarning.Text = msg;
                tableLayoutPanelPSr.Visible = !enable && !string.IsNullOrEmpty(labelWarning.Text);
            }
        }

        private void EnableButtons()
        {
            buttonCheckAgain.Enabled = true;

            var canMigrate = dataGridViewVms.Rows.Cast<VmPrecheckRow>().All(r => !r.HasSolution() && !r.HasSolutionActionInProgress());
            //empty returns true, which is correct

            EvacuateButton.Enabled = canMigrate && (!tableLayoutPanelNewCoordinator.Visible || NewCoordinatorComboBox.Enabled);
        }

        private void DisableButtons()
        {
            buttonCheckAgain.Enabled = false;
            EvacuateButton.Enabled = false;
        }

        private void DeregisterVmRowEvents(VmPrecheckRow row)
        {
            if (row.vm != null)
            {
                row.vm.PropertyChanged -= VM_PropertyChanged;
                VM_guest_metrics gm = connection.Resolve(row.vm.guest_metrics);
                if (gm == null)
                    return;
                gm.PropertyChanged -= gm_PropertyChanged;
            }
        }

        private bool CanSuspendVm(String vmRef)
        {
            if (vmRef == null)
                return false;
            VM vm = connection.Resolve(new XenRef<VM>(vmRef));
            return vm != null && vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.suspend);
        }

        private void UpdateVMWithError(string[] errorDescription, string vmRef = null)
        {
            if (errorDescription.Length == 0)
                return;

            Solution solution;
            string error = "";

            switch (errorDescription[0])
            {
                case Failure.VM_REQUIRES_SR:
                    vmRef = errorDescription[1];
                    SR sr = connection.Resolve(new XenRef<SR>(errorDescription[2]));

                    if (sr != null && sr.content_type == SR.Content_Type_ISO)
                    {
                        error = Messages.EVACUATE_HOST_LOCAL_CD;
                        solution = Solution.EjectCD;
                    }
                    else
                    {
                        error = Messages.EVACUATE_HOST_LOCAL_STORAGE;
                        solution = CanSuspendVm(vmRef) ? Solution.Suspend : Solution.Shutdown;
                    }
                    break;

                case Failure.VM_MISSING_PV_DRIVERS:
                    vmRef = errorDescription[1];
                    VM vm = connection.Resolve(new XenRef<VM>(vmRef));
                    solution = InstallToolsCommand.CanRun(vm) && !Helpers.StockholmOrGreater(connection)
                        ? Solution.InstallPVDrivers
                        : Solution.InstallPVDriversNoSolution;
                    break;

                case Failure.HA_NO_PLAN:
                    dataGridViewVms.Rows.Cast<VmPrecheckRow>().ToList().ForEach(r =>
                        r.SetError("", CanSuspendVm(r.vm.opaque_ref) ? Solution.Suspend : Solution.Shutdown));
                    return; //exit the method

                case Failure.HOST_NOT_ENOUGH_FREE_MEMORY:
                case Failure.VM_FAILED_SHUTDOWN_ACKNOWLEDGMENT:
                    vmRef = errorDescription[1];
                    solution = CanSuspendVm(vmRef) ? Solution.Suspend : Solution.Shutdown;
                    break;

                default:
                    solution = CanSuspendVm(vmRef) ? Solution.Suspend : Solution.Shutdown;
                    break;
            }

            var row = dataGridViewVms.Rows.Cast<VmPrecheckRow>().FirstOrDefault(r => r.vm.opaque_ref == vmRef);
            row?.SetError(error, solution);
        }

        private AsyncAction.SudoElevationResult GetSudoElevationResult()
        {
            if (elevatedUsername == null)
                return null;

            var ser = new AsyncAction.SudoElevationResult(elevatedUsername, elevatedPassword, elevatedSession);
            //use the session from the role elevation dialog, but only the first time
            elevatedSession = null;
            return ser;
        }


        #region Control event handlers

        private void NewCoordinatorComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            var backColor = NewCoordinatorComboBox.Enabled ? NewCoordinatorComboBox.BackColor : SystemColors.Control;

            using (SolidBrush backBrush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(backBrush, e.Bounds);

            if (e.Index < 0 || e.Index > NewCoordinatorComboBox.Items.Count - 1)
                return;

            if (!(NewCoordinatorComboBox.Items[e.Index] is ToStringWrapper<Host> host))
                return;

            Image icon = Images.GetImage16For(host.item);

            var imageRectangle = new Rectangle(e.Bounds.Left + 1, e.Bounds.Top + 1, 16, 16);
            Rectangle textRectangle = new Rectangle(e.Bounds.Height, e.Bounds.Y,
                e.Bounds.Width - e.Bounds.Height - 5, e.Bounds.Height);

            using (var g = e.Graphics)
            {
                if (NewCoordinatorComboBox.Enabled)
                {
                    g.DrawImage(icon, imageRectangle);

                    Drawing.DrawText(g, host.ToString(), e.Font, textRectangle,
                        e.ForeColor, e.BackColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                }
                else
                {
                    g.DrawImage(icon, imageRectangle, 0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel, Drawing.GreyScaleAttributes);

                    Drawing.DrawText(g, host.ToString(), e.Font, textRectangle,
                        SystemColors.GrayText, SystemColors.Control, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                }
            }
        }

        
        private void dataGridViewVms_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewVms.RowCount &&
                e.ColumnIndex == columnAction.Index &&
                dataGridViewVms.Rows[e.RowIndex] is VmPrecheckRow row && row.HasSolution())
            {
                Cursor.Current = Cursors.Hand;
                return;
            }

            Cursor.Current = Cursors.Default;
        }

        private void dataGridViewVms_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewVms.RowCount &&
                e.ColumnIndex == columnAction.Index &&
                dataGridViewVms.Rows[e.RowIndex] is VmPrecheckRow row && row.HasSolution())
            {
                AsyncAction action = null;
                switch (row.Solution)
                {
                    case Solution.EjectCD:
                        action = new ChangeVMISOAction(row.vm.Connection, row.vm, null, row.vm.FindVMCDROM());
                        break;
                    case Solution.Suspend:
                        action = new VMSuspendAction(row.vm);
                        break;
                    case Solution.Shutdown:
                        action = new VMHardShutdown(row.vm);
                        break;
                    case Solution.InstallPVDrivers:
                        var cmd = new InstallToolsCommand(Program.MainWindow, row.vm, dataGridViewVms);
                        cmd.Run();
                        // The install pv tools action is marked as complete after they have taken the user to the
                        // console and loaded the disc. Rescanning when the action is 'complete' in this case
                        // doesn't gain us anything then. Keep showing the "Click here to install PV drivers" text.
                        return;
                }

                if (action != null)
                {
                    action.Completed += a => Rebuild();
                    action.RunAsync(GetSudoElevationResult());
                }
                //set this after starting the action so that the row is updated correctly
                row.SolutionAction = action;
            }
        }


        private void buttonCheckAgain_Click(object sender, EventArgs e)
        {
            Rebuild();
        }

        private void EvacuateButton_Click(object sender, EventArgs e)
        {
            var newCoordinator = tableLayoutPanelNewCoordinator.Visible
                ? NewCoordinatorComboBox.SelectedItem as ToStringWrapper<Host>
                : null;

            hostAction = new EvacuateHostAction(_host, newCoordinator?.item,
                reasons ?? new Dictionary<XenRef<VM>, string[]>(),
                AddHostToPoolCommand.NtolDialog, AddHostToPoolCommand.EnableNtolDialog);

            hostAction.Completed += Program.MainWindow.action_Completed;
            hostAction.Changed += HostAction_Changed;
            hostAction.Completed += HostAction_Completed;

            //Closes all per-Connection and per-VM wizards for the given connection.
            Program.MainWindow.CloseActiveWizards(_host.Connection);

            DisableButtons();
            progressBar1.Visible = true;
            pictureBoxStatus.Visible = false;
            tableLayoutPanelStatus.Visible = true;
            hostAction.RunAsync(GetSudoElevationResult());
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void EvacuateHostDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            vmErrorsAction?.Cancel();
            hostAction?.Cancel();

            if (_host != null)
                _host.PropertyChanged -= hostUpdate;
            if (_pool != null)
                _pool.PropertyChanged -= poolUpdate;
            
            foreach (var h in connection.Cache.Hosts)
                h.PropertyChanged -= host_PropertyChanged;

            foreach (DataGridViewRow row in dataGridViewVms.Rows)
            {
                if (row is VmPrecheckRow precheckRow)
                {
                    DeregisterVmRowEvents(precheckRow);
                    precheckRow.SolutionAction?.Cancel();
                }
            }

            if (elevatedSession != null && elevatedSession.opaque_ref != null)
            {
                // NOTE: This doesnt happen currently, as we always scan once. Here as cheap insurance.
                // we still have the session from the role elevation dialog
                // it hasn't been used by an action so needs to be logged out
                elevatedSession.logout();
            }
        }

        #endregion

        
        private void HostAction_Changed(ActionBase obj)
        {
            if (obj != hostAction)
                return;

            Program.Invoke(this, UpdateProgress);
        }

        private void HostAction_Completed(ActionBase obj)
        {
            if (obj != hostAction)
                return;

            hostAction.Changed -= HostAction_Changed;
            hostAction.Completed -= HostAction_Completed;

            Program.Invoke(this, () =>
            {
                if (hostAction.Succeeded)
                {
                    Close();
                    return;
                }

                UpdateProgress();
                EnableButtons();
                pictureBoxStatus.Visible = true;
                tableLayoutPanelStatus.Visible = true;
                progressBar1.Visible = false;

                if (hostAction.Exception is Failure failure && failure.ErrorDescription.Count > 0 &&
                    (failure.ErrorDescription[0] == Failure.HOST_NOT_ENOUGH_FREE_MEMORY ||
                     failure.ErrorDescription[0] == Failure.HA_NO_PLAN))
                {
                    if (failure.ErrorDescription[0] == Failure.HOST_NOT_ENOUGH_FREE_MEMORY)
                        labelProgress.Text = Messages.EVACUATE_HOST_NOT_ENOUGH_MEMORY;
                    else if (failure.ErrorDescription[0] == Failure.HA_NO_PLAN)
                        labelProgress.Text = Messages.EVACUATE_HOST_NO_OTHER_HOSTS;

                    UpdateVMWithError(failure.ErrorDescription.ToArray());
                }
                else
                    labelProgress.Text = hostAction.Exception.Message;

                if (_rebuildRequired)
                    Rebuild();
            });
        }

        private void UpdateProgress()
        {
            labelProgress.Text = hostAction.Description;

            if (hostAction.PercentComplete < 0)
                progressBar1.Value = 0;
            else if (hostAction.PercentComplete > 100)
                progressBar1.Value = 100;
            else
                progressBar1.Value = hostAction.PercentComplete;
        }

 
        #region Nested items

        enum Solution { None, EjectCD, Suspend, InstallPVDrivers, InstallPVDriversNoSolution, Shutdown }

        private class VmPrecheckRow : DataGridViewRow, IComparable<VmPrecheckRow>
        {
            public readonly VM vm;
            private AsyncAction _solutionAction;
            private string error = string.Empty;

            public Solution Solution { get; private set; } = Solution.None;

            public AsyncAction SolutionAction
            {
                get => _solutionAction;
                set
                {
                    _solutionAction = value;

                    if (HasSolutionActionInProgress())
                    {
                        Solution = Solution.None;
                        error = _solutionAction.Title;
                    }
                    
                    Refresh();
                }
            }

            private readonly DataGridViewImageCell cellImage = new DataGridViewImageCell();
            private readonly DataGridViewTextBoxCell cellVm = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell cellAction = new DataGridViewTextBoxCell();

            public VmPrecheckRow(VM vm)
            {
                this.vm = vm;
                Cells.AddRange(cellImage, cellVm, cellAction);
                Refresh();
            }

            public void Refresh()
            {
                cellImage.Value = Images.GetImage16For(vm);
                cellVm.Value = ToString();
                cellAction.Value = error;

                if (HasSolution())
                {
                    cellAction.Style.Font = new Font(Program.DefaultFont, FontStyle.Underline);
                    cellAction.Style.ForeColor = Color.Blue;
                }
                else
                {
                    cellAction.Style.Font = Program.DefaultFont;
                    cellAction.Style.ForeColor = DefaultForeColor;
                }

                cellAction.Style.SelectionForeColor = cellAction.Style.ForeColor;
                cellAction.Style.SelectionBackColor = cellAction.Style.BackColor;
            }

            public override string ToString()
            {
                return vm.Name();
            }

            public int CompareTo(VmPrecheckRow otherVM)
            {
                return vm.CompareTo(otherVM?.vm);
            }

            public void SetError(string message, Solution solution)
            {
                // still running action to solve a previous error, no point in overwriting or we could end up 'solving' it twice
                if (HasSolutionActionInProgress())
                {
                    Solution = Solution.None;
                    error = SolutionAction.Title;
                    Refresh();
                    return;
                }
                 
                Solution = solution;

                switch (solution)
                {
                    case Solution.EjectCD:
                        error = String.Format(Messages.EVACUATE_HOST_EJECT_CD_PROMPT, message);
                        break;

                    case Solution.Suspend:
                        error = String.Format(Messages.EVACUATE_HOST_SUSPEND_VM_PROMPT, message);
                        break;

                    case Solution.Shutdown:
                        error = String.Format(Messages.EVACUATE_HOST_SHUTDOWN_VM_PROMPT, message);
                        break;

                    case Solution.InstallPVDrivers:
                        error = vm.HasNewVirtualizationStates()
                            ? string.Format(Messages.EVACUATE_HOST_INSTALL_MGMNT_PROMPT, message)
                            : string.Format(Messages.EVACUATE_HOST_INSTALL_TOOLS_PROMPT, message, BrandManager.VmTools);
                        break;

                    case Solution.InstallPVDriversNoSolution:
                        // if the state is not unknown we have metrics and can show a detailed message.
                        // Otherwise go with the server and just say they aren't installed
                        error = !vm.GetVirtualizationStatus(out _).HasFlag(VM.VirtualizationStatus.Unknown)
                            ? vm.GetVirtualizationWarningMessages()
                            : string.Format(Messages.PV_DRIVERS_NOT_INSTALLED, BrandManager.VmTools);
                        break;
                }

                Refresh();
            }

            internal bool HasSolution()
            {
                return Solution != Solution.None 
                    && Solution != Solution.InstallPVDriversNoSolution;
            }

            internal bool HasSolutionActionInProgress()
            {
                return SolutionAction != null && !SolutionAction.IsCompleted;
            }
        }

        #endregion
    }
}
