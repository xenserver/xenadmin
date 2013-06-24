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
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAdmin.Commands;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Dialogs
{
    public partial class EvacuateHostDialog : DialogWithProgress
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Host host;
        private EvacuateHostAction hostAction;
        private Dictionary<string, VMListBoxItem> vms;
        private Dictionary<XenRef<VM>, String[]> reasons;
        private string elevatedUName;
        private string elevatedPass;
        private Session elevatedSession;

        private readonly string OriginalText;

        /* README
         * 
         * This dialog inherits from DialogWithProgress. Be extremely careful about resizing it, as the DialogWithProgress
         * class asserts that the controls which inherit it must match its dimensions... otherwise the expanding progress
         * bar may not show.
         *
         */

        public EvacuateHostDialog(Host host)
            : base(host.Connection)
        {
            this.host = host;

            InitializeComponent();

            Shrink();

            this.OriginalText = this.Text;
            vmsListBox.Sorted = true;
            vmsListBox.SelectionMode = SelectionMode.None;
            vmsListBox.DrawMode = DrawMode.OwnerDrawFixed;
            vmsListBox.DrawItem += new DrawItemEventHandler(vmsListBox_DrawItem);
            vmsListBox.MouseMove += new MouseEventHandler(vmsListBox_MouseMove);
            //vmsListBox.MouseDoubleClick += new MouseEventHandler(vmsListBox_MouseDoubleClick);
            vmsListBox.MouseClick += new MouseEventHandler(vmsListBox_MouseDoubleClick);
            vmsListBox.ItemHeight = 16;

            NewMasterComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            NewMasterComboBox.DrawItem += new DrawItemEventHandler(NewMasterComboBox_DrawItem);
            NewMasterComboBox.ItemHeight = 16;

            if (!host.IsMaster() || connection.Cache.HostCount <= 1)
            {
                NewMasterComboBox.Enabled = false;
                NewMasterComboBox.Visible = false;
                NewMasterLabel.Visible = false;
                labelMasterBlurb.Visible = false;

                // Move the panel containing the VM listbox up to fill the gap where the labels were
                const int pad = 6;
                int extraSize = panel2.Top - labelMasterBlurb.Top - pad;
                panel2.Top = labelMasterBlurb.Top + pad;
                panel2.Height += extraSize;
            }

            Pool pool = Core.Helpers.GetPool(host.Connection);
            if (Helpers.WlbEnabled(host.Connection) && WlbServerState.GetState(pool) == WlbServerState.ServerState.Enabled)
                lableWLBEnabled.Visible = true;

            vms = new Dictionary<string, VMListBoxItem>();
            this.host.PropertyChanged += new PropertyChangedEventHandler(hostUpdate);

            Program.AssertOnEventThread();
            this.Text = OriginalText + " - " + host.Name;
            populateVMs();
            populateHosts();
            ActiveControl = CloseButton;
        }

        void NewMasterComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (NewMasterComboBox.Enabled)
            {
                using (SolidBrush backBrush = new SolidBrush(NewMasterComboBox.BackColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.Bounds);
                }

                if (e.Index == -1)
                    return;

                ToStringWrapper<Host> host = NewMasterComboBox.Items[e.Index] as ToStringWrapper<Host>;

                if (host == null)
                    return;

                Graphics g = e.Graphics;
                Rectangle bounds = e.Bounds;

                Image icon = Images.GetImage16For(host.item);

                // Now draw the image
                g.DrawImage(icon, bounds.Left + 1, bounds.Top + 1, bounds.Height - 2, bounds.Height - 2);

                Rectangle bump = new Rectangle(bounds.Height, bounds.Y, bounds.Width - bounds.Height - RIGHT_PADDING, bounds.Height);

                // And the text
                Drawing.DrawText(g, host.ToString(), e.Font,
                    bump, e.ForeColor, e.BackColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
            else
            {
                using (SolidBrush backBrush = new SolidBrush(SystemColors.Control))
                {
                    e.Graphics.FillRectangle(backBrush, e.Bounds);
                }

                if (e.Index == -1)
                    return;

                ToStringWrapper<Host> host = NewMasterComboBox.Items[e.Index] as ToStringWrapper<Host>;

                if (host == null)
                    return;

                Graphics g = e.Graphics;
                Rectangle bounds = e.Bounds;

                Image icon = Images.GetImage16For(host.item);

                // Now draw the image
                g.DrawImage(icon, new Rectangle(bounds.Left + 1, bounds.Top + 1, bounds.Height - 2, bounds.Height - 2), 0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel, Core.Drawing.GreyScaleAttributes);

                Rectangle bump = new Rectangle(bounds.Height, bounds.Y, bounds.Width - bounds.Height - RIGHT_PADDING, bounds.Height);

                // And the text
                Drawing.DrawText(g, host.ToString(), e.Font,
                    bump, SystemColors.GrayText, SystemColors.Control, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }

        const int RIGHT_PADDING = 5;

        void vmsListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            using (SolidBrush backBrush = new SolidBrush(vmsListBox.BackColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            if (e.Index == -1)
                return;

            VMListBoxItem item = vmsListBox.Items[e.Index] as VMListBoxItem;
            using (Font basicFont = new Font(e.Font, FontStyle.Regular))
            {
                vmsListBox.WilkieSpecial(Images.GetImage16For(item.Icon), item.ToString(), item.GetError(), Color.Red, basicFont, e);
            }
        }

        private VMListBoxItem isOnErrorLabelFor(Point p)
        {
            int i = vmsListBox.IndexFromPoint(p);
            if (i == ListBox.NoMatches)
                return null;

            VMListBoxItem item = vmsListBox.Items[i] as VMListBoxItem;
            if (item == null)
                return null;

            Rectangle bounds = vmsListBox.GetItemRectangle(i);

            Size s;
            using (Font boldFont = new Font(vmsListBox.Font, FontStyle.Bold))
            {
                using (Graphics graphics = vmsListBox.CreateGraphics())
                {
                    s = Drawing.MeasureText(graphics, item.GetError(),
                                                 boldFont, bounds.Size, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
                }
            }

            Rectangle ErrorTextBounds = new Rectangle(bounds.Right - s.Width - RIGHT_PADDING, bounds.Top, s.Width, s.Height);

            return ErrorTextBounds.Contains(p) ? item : null;
        }

        void vmsListBox_MouseMove(object sender, MouseEventArgs e)
        {
            VMListBoxItem item = isOnErrorLabelFor(e.Location);
            if (item != null && item.hasSolution())
            {
                Cursor.Current = Cursors.Hand;
            }
            else
            {
                Cursor.Current = Cursors.Default;
            }
        }

        void vmsListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            VMListBoxItem vmlbi = isOnErrorLabelFor(e.Location);
            if (vmlbi == null || !vmlbi.hasSolution())
                return;

            AsyncAction a = vmlbi.Solve();
            if (a != null)
                a.Completed += new EventHandler<EventArgs>(solveActionCompleted);

            vmsListBox.Refresh();
        }

        void solveActionCompleted(object sender, EventArgs e)
        {
            // this should rescan the vm errors and update the dialog.
            Program.Invoke(this, update);
        }

        private void hostUpdate(object o, PropertyChangedEventArgs args)
        {
            if (args == null || args.PropertyName == "name_label" || args.PropertyName == "resident_VMs")
            {
                Program.Invoke(this, update);
            }
        }

        private void update()
        {
            Program.AssertOnEventThread();

            this.Text = OriginalText + " - " + host.Name;
            populateVMs();
            populateHosts();
            Scan();
        }

        private void Scan()
        {
            DelegatedAsyncAction saveVMsAction = new DelegatedAsyncAction(connection, Messages.SAVING_VMS_ACTION_TITLE,
                Messages.SAVING_VMS_ACTION_DESC, Messages.COMPLETED, delegate(Session session)
                {
                    //Save Evacuated VMs for later
                    host.SaveEvacuatedVMs(session);
                }, "host.remove_from_other_config", "host.add_to_other_config");
            DelegatedAsyncAction action = new DelegatedAsyncAction(connection, Messages.MAINTENANCE_MODE,
                Messages.SCANNING_VMS, Messages.SCANNING_VMS, delegate(Session session)
                {
                    reasons = new Dictionary<XenRef<VM>, string[]>();

                    // WLB: get host wlb evacuate recommendation if wlb is enabled
                    if (Helpers.WlbEnabled(host.Connection))
                    {
                        try
                        {
                            reasons = XenAPI.Host.retrieve_wlb_evacuate_recommendations(session, host.opaque_ref);
                        }
                        catch (Exception ex)
                        {
                            log.Debug(ex.Message, ex);
                        }
                    }


                    // WLB: in case wlb has no recommendations or get errors when retrieve recommendation, 
                    //      assume retrieve_wlb_evacuate_recommendations returns 0 recommendation 
                    //      or return recommendations for all running vms on this host
                    if (reasons.Count == 0 || !ValidRecommendation(reasons))
                        reasons = Host.get_vms_which_prevent_evacuation(session, host.opaque_ref);

                    // take care of errors
                    Program.Invoke(this, delegate()
                    {
                        foreach (KeyValuePair<XenRef<VM>, String[]> kvp in reasons)
                        {
                            //WLB: filter out errors
                            if (string.Compare(kvp.Value[0].Trim(), "wlb", true) != 0)
                                ProcessError(kvp.Key.opaque_ref, kvp.Value);

                            //Update NewMasterComboBox for host power on recommendation
                            if ((session.Connection.Resolve(kvp.Key)).is_control_domain)
                            {
                                Host powerOnHost = session.Connection.Cache.Find_By_Uuid<Host>(kvp.Value[(int)RecProperties.ToHost]);
                                if (powerOnHost != null)
                                {
                                    ToStringWrapper<Host> previousSelection = NewMasterComboBox.SelectedItem as ToStringWrapper<Host>;
                                    ToStringWrapper<Host> hostToAdd = new ToStringWrapper<Host>(powerOnHost, (ToStringDelegate<Host>)getHostName);
                                    if (NewMasterComboBox.Items.Count == 0)
                                    {
                                        powerOnHost.PropertyChanged -= new PropertyChangedEventHandler(host_PropertyChanged);
                                        powerOnHost.PropertyChanged += new PropertyChangedEventHandler(host_PropertyChanged);
                                        NewMasterComboBox.Items.Add(hostToAdd);
                                    }
                                    else
                                    {
                                        foreach (ToStringWrapper<Host> tswh in NewMasterComboBox.Items)
                                        {
                                            if (tswh.item.CompareTo(powerOnHost) != 0)
                                            {
                                                powerOnHost.PropertyChanged -= new PropertyChangedEventHandler(host_PropertyChanged);
                                                powerOnHost.PropertyChanged += new PropertyChangedEventHandler(host_PropertyChanged);
                                                NewMasterComboBox.Items.Add(hostToAdd);
                                            }
                                        }
                                    }
                                    SelectProperItemInNewMasterComboBox(previousSelection);
                                }
                            }
                        }
                    });

                }, true);
            SetSession(saveVMsAction);
            SetSession(action);
            saveVMsAction.RunAsync();
            new ActionProgressDialog(action, ProgressBarStyle.Blocks).ShowDialog(this);
            RefreshEntermaintenanceButton();
        }

        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "resident_on" || args.PropertyName == "allowed_operations")
            {
                Program.Invoke(this, vmsListBox.Refresh);
            }
            else if (args.PropertyName == "virtualisation_status")
            {
                Program.Invoke(this, update);
            }
            else if (args.PropertyName == "guest_metrics")
            {
                VM v = sender as VM;
                VM_guest_metrics gm = connection.Resolve(v.guest_metrics);
                if (gm == null)
                    return;

                gm.PropertyChanged -= new PropertyChangedEventHandler(gm_PropertyChanged);
                gm.PropertyChanged += new PropertyChangedEventHandler(gm_PropertyChanged);
            }
        }

        void gm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PV_drivers_version")
                Program.Invoke(this, update);
        }

        private void host_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            populateHosts();
            Program.Invoke(this, NewMasterComboBox.Refresh);
        }

        private void populateVMs()
        {
            Program.AssertOnEventThread();

            deregisterVMEvents();
            Dictionary<string, AsyncAction> solveActionsByUuid = new Dictionary<string,AsyncAction>();
            foreach (VMListBoxItem i in vms.Values)
            {
                solveActionsByUuid.Add(i.GetVM().uuid, i.solutionAction);
            }
            vms = new Dictionary<string, VMListBoxItem>();

            vmsListBox.BeginUpdate();
            try
            {
                vmsListBox.Items.Clear();

                foreach (VM vm in connection.ResolveAll(host.resident_VMs))
                {
                    if (vm.is_control_domain || vm.is_a_template)
                        continue;

                    vm.PropertyChanged += new PropertyChangedEventHandler(VM_PropertyChanged);
                    VMListBoxItem vmlbi = new VMListBoxItem(this, vm);
                    if (solveActionsByUuid.ContainsKey(vm.uuid))
                    {
                        vmlbi.solutionAction = solveActionsByUuid[vm.uuid];
                    }
                    vms.Add(vm.opaque_ref, vmlbi);
                    vmsListBox.Items.Add(vmlbi);
                }
            }
            finally
            {
                vmsListBox.EndUpdate();
                RefreshEntermaintenanceButton();
            }
        }

        private void RefreshEntermaintenanceButton()
        {
            this.EvacuateButton.Enabled = true;
            foreach (VMListBoxItem item in vmsListBox.Items)
            {
                if (item.hasSolution())
                {
                    this.EvacuateButton.Enabled = false;
                    break;
                }
            }
        }

        private void deregisterVMEvents()
        {

            //Deregister event handlers from these VMs
            foreach (VMListBoxItem vmlbi in vms.Values)
            {
                VM v = vmlbi.GetVM();
                v.PropertyChanged -= new PropertyChangedEventHandler(VM_PropertyChanged);
                VM_guest_metrics gm = connection.Resolve(v.guest_metrics);
                if (gm == null)
                    return;
                gm.PropertyChanged -= new PropertyChangedEventHandler(gm_PropertyChanged);

            }
        }

        private void populateHosts()
        {
            Program.AssertOnEventThread();


            ToStringWrapper<Host> previousSelection = NewMasterComboBox.SelectedItem as ToStringWrapper<Host>;

            NewMasterComboBox.BeginUpdate();
            try
            {
                NewMasterComboBox.Items.Clear();

                foreach (Host host in connection.Cache.Hosts)
                {
                    Host_metrics metrics = connection.Resolve<Host_metrics>(host.metrics);
                    if (host.opaque_ref == this.host.opaque_ref)
                        continue;

                    host.PropertyChanged -= new PropertyChangedEventHandler(host_PropertyChanged);
                    host.PropertyChanged += new PropertyChangedEventHandler(host_PropertyChanged);
                    if (host.enabled && metrics != null && metrics.live)
                        NewMasterComboBox.Items.Add(new ToStringWrapper<Host>(host,
                            (ToStringDelegate<Host>)getHostName));
                }

                SelectProperItemInNewMasterComboBox(previousSelection);
            }
            finally
            {
                NewMasterComboBox.EndUpdate();
            }
        }

        private String getHostName(Host host)
        {
            return host.Name;
        }

        private class VMListBoxItem : IComparable<VMListBoxItem>
        {
            VM vm;
            EvacuateHostDialog dialog;
            Solution solution;
            public AsyncAction solutionAction;
            string error;

            public VMListBoxItem(EvacuateHostDialog dialog, VM vm)
            {
                this.dialog = dialog;
                this.vm = vm;
                this.error = "";
                this.solution = Solution.None;
            }

            public Icons Icon
            {
                get { return Images.GetIconFor(vm); }
            }

            public override string ToString()
            {
                return vm.Name;
            }

            public int CompareTo(VMListBoxItem otherVM)
            {
                return vm.CompareTo(otherVM.vm);
            }

            public VM GetVM()
            {
                return vm;
            }

            public void UpdateError(string message, Solution solution)
            {
                // still running action to solve a previous error, no point in overwriting or we could end up 'solving' it twice
                if (solutionAction != null && !solutionAction.IsCompleted)
                {
                    this.error = Messages.EVACUATE_SOLUTION_IN_PROGRESS;
                    return;
                }
                 
                this.solution = solution;

                switch (solution)
                {
                    case Solution.EjectCD:
                        error = String.Format(Messages.EVACUATE_HOST_EJECT_CD_PROMPT, message);
                        break;

                    case Solution.Suspend:
                        error = String.Format(Messages.EVACUATE_HOST_SUSPEND_VM_PROMPT, message);
                        break;

                    case Solution.InstallPVDrivers:
                        error = String.Format(Messages.EVACUATE_HOST_INSTALL_TOOLS_PROMPT, message);
                        break;

                    case Solution.InstallPVDriversNoSolution:
                        // if the state is not unknown we have metrics and can show a detailed message. Otherwise go with the server and just
                        // say they arent installed
                        error = vm.GetVirtualisationStatus != VM.VirtualisationStatus.UNKNOWN ? vm.GetVirtualisationWarningMessages()
                            : Messages.PV_DRIVERS_NOT_INSTALLED;
                        break;
                }
            }

            public string GetError()
            {
                return this.error;
            }

            public AsyncAction Solve()
            {
                AsyncAction a = null;
                switch (solution)
                {
                    case Solution.EjectCD:
                        a = new ChangeVMISOAction(vm.Connection, vm, null, vm.FindVMCDROM());
                        dialog.SetSession(a);
                        dialog.DoAction(a);
                        break;

                    case Solution.Suspend:
                        a = new VMSuspendAction(vm);
                        dialog.SetSession(a);
                        dialog.DoAction(a);
                        break;

                    case Solution.InstallPVDrivers:
                        a = new InstallToolsCommand(Program.MainWindow.CommandInterface, vm).ExecuteGetAction();
                        // The install pv tools action is marked as complete after they have taken the user to the console and loaded the disc
                        // Rescanning when the action is 'complete' in this case doesn't gain us anything then. Keep showing the "Click here to install PV drivers" text.
                        dialog.SetSession(a);
                        solutionAction = a;
                        return a;
                }
                solutionAction = a;
                solution = Solution.None;
                // we show this, then register an event handler on the action completed to re-update the text/solution
                this.error = Messages.EVACUATE_SOLUTION_IN_PROGRESS;
                return a;
            }

            internal bool hasSolution()
            {
                return solution != Solution.None 
                    && solution != Solution.InstallPVDriversNoSolution;
            }
        }

        private void RepairButton_Click(object sender, EventArgs e)
        {
            CloseButton.Text = Messages.CLOSE;

            NewMasterComboBox.Enabled = false;
            ToStringWrapper<Host> newMaster = NewMasterComboBox.SelectedItem as ToStringWrapper<Host>;

            hostAction = new EvacuateHostAction(host, newMaster != null ? newMaster.item : null, reasons ?? new Dictionary<XenRef<VM>, string[]>(), AddHostToPoolCommand.NtolDialog, AddHostToPoolCommand.EnableNtolDialog);
            hostAction.Completed += Program.MainWindow.action_Completed;
            SetSession(hostAction);
            Program.MainWindow.UpdateToolbars();

            //Closes all per-Connection and per-VM wizards for the given connection.
            Program.MainWindow.closeActiveWizards(host.Connection);

            EvacuateButton.Enabled = false; // disable evac button, it will get re-enabled when action completes

            DoAction(hostAction);
        }

        private void SetSession(AsyncAction action)
        {
            Program.AssertOnEventThread();
            if (elevatedUName == null)
                return;
            
            action.sudoPassword = elevatedPass;
            action.sudoUsername = elevatedUName;
            if (elevatedSession != null)
            {
                // we still have the session from the role elevation dialog
                // use this first
                action.Session = elevatedSession;
                elevatedSession = null;
            }
            else
                action.Session = action.NewSession();
        }

        protected override void action_Completed(object sender, EventArgs e)
        {
            Program.Invoke(this, delegate()
            {
                if (sender == null)
                    return;

                if (sender != hostAction)
                {
                    //re-enable the buttons
                    this.EvacuateButton.Enabled = true;
                    this.CloseButton.Enabled = true;
                    return;
                }

                if (hostAction.Exception == null)
                {
                    Close();
                }

                this.EvacuateButton.Enabled = true;
                this.CloseButton.Enabled = true;
                this.NewMasterComboBox.Enabled = true;

                Failure failure = hostAction.Exception as Failure;
                if (failure == null)
                    return;

                if (failure.ErrorDescription.Count > 0 &&
                    failure.ErrorDescription[0] == Failure.HOST_NOT_ENOUGH_FREE_MEMORY)
                {
                    failure.ErrorDescription[0] = Failure.HA_NO_PLAN;
                    failure.Setup();
                }

                ProcessError(null, failure.ErrorDescription.ToArray());
            });

            base.action_Completed(sender, e);
        }

        private void ProcessError(String vmRef, String[] ErrorDescription)
        {
            try
            {
                if (ErrorDescription.Length == 0)
                    return;

                switch (ErrorDescription[0])
                {
                    case Failure.VM_REQUIRES_SR:
                        vmRef = ErrorDescription[1];
                        SR sr = connection.Resolve(new XenRef<SR>(ErrorDescription[2]));
                        if (sr == null)
                            return;

                        if (sr.content_type == SR.Content_Type_ISO)
                        {
                            UpdateVMWithError(vmRef, Messages.EVACUATE_HOST_LOCAL_CD, Solution.EjectCD);
                        }
                        else
                        {
                            UpdateVMWithError(vmRef, Messages.EVACUATE_HOST_LOCAL_STORAGE, Solution.Suspend);
                        }

                        break;

                    case Failure.VM_MISSING_PV_DRIVERS:
                        vmRef = ErrorDescription[1];

                        VM vm = connection.Resolve(new XenRef<VM>(vmRef));
                        if (vm != null && InstallToolsCommand.CanExecute(vm))
                            UpdateVMWithError(vmRef, String.Empty, Solution.InstallPVDrivers);
                        else
                            UpdateVMWithError(vmRef, String.Empty, Solution.InstallPVDriversNoSolution);

                        break;

                    case Failure.HOST_NOT_ENOUGH_FREE_MEMORY:
                        if (vmRef == null)
                            new ThreeButtonDialog(
                               new ThreeButtonDialog.Details(
                                   SystemIcons.Error,
                                   Messages.EVACUATE_HOST_NOT_ENOUGH_MEMORY,
                                   Messages.EVACUATE_HOST_NOT_ENOUGH_MEMORY_TITLE)).ShowDialog(this);

                        vmRef = ErrorDescription[1];
                        UpdateVMWithError(vmRef, String.Empty, Solution.Suspend);

                        break;

                    case Failure.HA_NO_PLAN:

                        foreach (string _vmRef in vms.Keys)
                        {
                            UpdateVMWithError(_vmRef, String.Empty, Solution.Suspend);
                        }

                        if (vmRef == null)
                            new ThreeButtonDialog(
                               new ThreeButtonDialog.Details(
                                   SystemIcons.Error,
                                   Messages.EVACUATE_HOST_NO_OTHER_HOSTS,
                                   Messages.EVACUATE_HOST_NO_OTHER_HOSTS_TITLE)).ShowDialog(this);

                        break;

                    default:
                        AddDefaultSuspendOperation(vmRef);
                        break;
                }
            }
            catch (Exception e)
            {
                log.Debug("Exception processing exception", e);
                log.Debug(e, e);

                AddDefaultSuspendOperation(vmRef);
            }
        }

        private void AddDefaultSuspendOperation(String vmRef)
        {
            if (vmRef == null)
                return;

            UpdateVMWithError(vmRef, String.Empty, Solution.Suspend);
        }

        enum Solution { None, EjectCD, Suspend, InstallPVDrivers, InstallPVDriversNoSolution };

        void UpdateVMWithError(string opaqueRef, string message, Solution solution)
        {
            VMListBoxItem vmlbi = vms[opaqueRef];

            vmlbi.UpdateError(message, solution);

            Program.Invoke(this, vmsListBox.Refresh);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            //This dialog uses several different actions all of which might need an elevated session
            //We sudo once for all of them and store the session, or close the dialog.
            List<Role> validRoles = new List<Role>();
            if (!connection.Session.IsLocalSuperuser && Helpers.MidnightRideOrGreater(connection) && !Registry.DontSudo
                && !Role.CanPerform(new RbacMethodList(

                    "host.remove_from_other_config", // save VM list
                    "host.add_to_other_config", 

                    "host.disable", // disable the host

                    "pbd.plug", // Repair SR and install tools
                    "pbd.create",
                    "vbd.eject",
                    "vbd.insert",

                    "vm.suspend", // Suspend VMs

                    "vbd.async_eject", // Change ISO
                    "vbd.async_insert"
                    
                    ), connection, out validRoles))
            {
                var sudoDialog = XenAdminConfigManager.Provider.SudoDialogDelegate;
                var result = sudoDialog(validRoles, connection, Text);
                if (!result.Result)
                {
                    Close();
                    return;
                }

                elevatedPass = result.ElevatedPassword;
                elevatedUName = result.ElevatedUsername;
                elevatedSession = result.ElevatedSession;
            }

            Scan();
        }

        private void SelectProperItemInNewMasterComboBox(ToStringWrapper<Host> previousSelection)
        {
            bool selected = false;

            if (previousSelection != null && !selected)
            {
                foreach (ToStringWrapper<Host> host in NewMasterComboBox.Items)
                {
                    if (host.item.opaque_ref == previousSelection.item.opaque_ref)
                    {
                        NewMasterComboBox.SelectedItem = host;
                        selected = true;
                        break;
                    }
                }
            }

            if (NewMasterComboBox.Items.Count > 0 && !selected)
            {
                NewMasterComboBox.SelectedIndex = 0;
                selected = true;
            }
        }

        private bool ValidRecommendation(Dictionary<XenRef<VM>, String[]> reasons)
        {
            bool valid = true;
            List<Host> controlDomain = new List<Host>();

            foreach (KeyValuePair<XenRef<VM>, String[]> kvp in reasons)
            {
                if ((this.connection.Resolve(kvp.Key)).is_control_domain)
                {
                    controlDomain.Add(this.connection.Cache.Find_By_Uuid<Host>(kvp.Value[(int)RecProperties.ToHost]));
                }
            }

            foreach (KeyValuePair<XenRef<VM>, String[]> kvp in reasons)
            {
                if (string.Compare(kvp.Value[0].Trim(), "wlb", true) == 0)
                {
                    Host toHost = this.connection.Cache.Find_By_Uuid<Host>(kvp.Value[(int)RecProperties.ToHost]);
                    if (!(this.connection.Resolve(kvp.Key)).is_control_domain && !toHost.IsLive && !controlDomain.Contains(toHost))
                    {
                        valid = false;
                        break;
                    }
                }
            }

            return valid;
        }

        private void EvacuateHostDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (Host host in connection.Cache.Hosts)
            {
                host.PropertyChanged -= new PropertyChangedEventHandler(host_PropertyChanged);
            }
            deregisterVMEvents();
            if (elevatedSession != null && elevatedSession.uuid != null)
            {
                // NOTE: This doesnt happen currently, as we always scan once. Here as cheap insurance.
                // we still have the session from the role elevation dialog
                // it hasn't been used by an action so needs to be logged out
                elevatedSession.logout();
            }
        }
    }
}
