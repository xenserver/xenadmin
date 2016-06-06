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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.ConsoleView;
using XenAPI;
using XenAdmin.Core;
using System.Drawing;


namespace XenAdmin.Controls
{
    public class ConsolePanel : UserControl
    {
        private const int MAX_ACTIVE_VM_CONSOLES = 10;

        private List<VM> activeVMConsoles;
        private Dictionary<VM, VNCView> vncViews;
        private Label errorLabel;
        private static string CouldNotConnect = Messages.VNC_COULD_NOT_CONNECT_CONSOLE;
        private static string CouldNotFindConsole = Messages.VNC_COULD_NOT_FIND_CONSOLES;
        public VNCView activeVNCView = null;
        private Panel RbacWarningPanel;
        private TableLayoutPanel tableLayoutPanel3;
        private PictureBox pictureBox2;
        private Label lableRbacWarning;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ConsolePanel()
            : base()
        {
            InitializeComponent();
            this.vncViews = new Dictionary<VM, VNCView>();
            this.activeVMConsoles = new List<VM>();
            this.Dock = DockStyle.Fill;
            pictureBox2.Image = SystemIcons.Warning.ToBitmap();
        }

        public void PauseAllViews()
        {
            // We're going to pause all of our VNCViews here, as this gets called when the VNC tab is not selected.
            // The VNCView deals with undocked cases.

            foreach (VNCView vncView in vncViews.Values)
            {
                vncView.Pause();
            }
        }

        public void ResetAllViews()
        {
            vncViews.Clear();
        }

        public void UnpauseActiveView()
        {
            // We're going to explicitly pause all the consoles 
            // except the active one, then explicitly unpause the active one.

            foreach (VNCView vncView in vncViews.Values)
            {
                if (vncView != activeVNCView)
                    vncView.Pause();
            }

            if (activeVNCView != null)
            {
                activeVNCView.Unpause();
            }
        }

        /// <summary>
        /// Gives focus to the console, as if the user had clicked it.
        /// </summary>
        internal void FocusActiveView()
        {
            if (activeVNCView != null)
                activeVNCView.FocusConsole();
        }

        internal void setCurrentSource(VM source)
        {
            Program.AssertOnEventThread();

            // activeVNCView is going to change, so the current activeVNCView will become inactive
            // Start a timer for closing the inactive VNC connection after an interval (20 seconds)
            StartCloseVNCTimer(activeVNCView);

            this.Controls.Clear();

            if (source == null)
            {
                activeVNCView = null;
                return;
            }
            List<Role> allowedRoles = null;
            if (RbacDenied(source, out allowedRoles))
            {
                lableRbacWarning.Text = String.Format(allowedRoles.Count == 1 ? Messages.RBAC_CONSOLE_WARNING_ONE : Messages.RBAC_CONSOLE_WARNING_MANY,
                                                      Role.FriendlyCSVRoleList(source.Connection.Session.Roles),
                                                      Role.FriendlyCSVRoleList(allowedRoles));

                this.Controls.Add(RbacWarningPanel);
                if (activeVNCView != null)
                    this.Controls.Remove(activeVNCView);
                return;
            }
            activeVMConsoles.Remove(source);
            activeVMConsoles.Add(source);

            StopCloseVncTimer(source);
            
            while (activeVMConsoles.Count > MAX_ACTIVE_VM_CONSOLES)
            {
                closeVNCForSource(activeVMConsoles[0]);
            }

            if (vncViews.ContainsKey(source))
            {
                activeVNCView = vncViews[source];
            }
            else
            {
                activeVNCView = new VNCView(source, null, null);
                vncViews[source] = activeVNCView;
            }
            activeVNCView.refreshIsoList();
            this.Controls.Add(activeVNCView);
            this.ClearErrorMessage();
        }

        internal void setCurrentSource(Host source)
        {
            // sanity...
            if (source == null)
            {
                log.Error("null source when attempting to connect to host VNC");
                SetErrorMessage(CouldNotConnect);
                return;
            }

            if (source == null)
            {
                log.Error("No local copy of host information when connecting to host VNC console...");
                SetErrorMessage(CouldNotConnect);
                return;
            }

            if (source.resident_VMs == null)
            {
                log.Error("No dom0 on host when connecting to host VNC console.");
                SetErrorMessage(CouldNotConnect);
                return;
            }

            List<XenRef<VM>> controlVMs =
                source.resident_VMs.FindAll((Predicate<XenRef<VM>>)delegate(XenRef<VM> vmRef)
            {
                VM vm = source.Connection.Resolve<VM>(vmRef);

                if (vm == null)
                {
                    return false;
                }
                else
                {
                    return vm.is_control_domain;
                }
            });

            if (controlVMs.Count > 0)
            {
                VM vm = source.Connection.Resolve<VM>(controlVMs[0]);
                if (vm == null)
                {
                    SetErrorMessage(CouldNotFindConsole);
                }
                else
                {
                    this.setCurrentSource(vm);
                }
            }
            else
            {
                SetErrorMessage(CouldNotFindConsole);
            }
        }

        public static bool RbacDenied(VM source, out List<Role> AllowedRoles)
        {

            if (source == null || source.Connection == null)
            {
                AllowedRoles = null;
                return false;
            }
            else
            {
                var session = source.Connection.Session;
                if (session != null && session.IsLocalSuperuser)
                {
                    AllowedRoles = null;
                    return false;
                }
            }

            List<Role> allowedRoles = null;
            if (source.is_control_domain)
                allowedRoles = Role.ValidRoleList("http/connect_console/host_console", source.Connection);
            else
                allowedRoles = Role.ValidRoleList("http/connect_console", source.Connection);

            if (source.Connection.Session.Roles.Find(delegate(Role r) { return allowedRoles.Contains(r); }) != null)
            {
                AllowedRoles = allowedRoles;
                return false;
            }
            AllowedRoles = allowedRoles;
            return true;
        }

        internal Image Snapshot(VM vm, string elevatedUsername, string elevatedPassword)
        {
            Program.AssertOffEventThread();

            VNCView view = null;

            bool useElevatedCredentials = false;

            //*
            if (!vncViews.ContainsKey(vm))
            {
                Program.Invoke(this, delegate
                {
                    // use elevated credentials, if provided, to create a vncView (CA-91132)
                    useElevatedCredentials = !String.IsNullOrEmpty(elevatedUsername) && !String.IsNullOrEmpty(elevatedPassword);
                    if (useElevatedCredentials)
                        view = new VNCView(vm, elevatedUsername, elevatedPassword);
                    else
                    {
                        setCurrentSource(vm);
                        if (vncViews.ContainsKey(vm))
                            view = vncViews[vm];
                    }
                });
            }
            else
            {
                view = vncViews[vm];
            }
            //*/

            //view = new VNCView(vm, elevatedUsername, elevatedPassword);

            if (view == null)
                return null;

            Image snapshot = view.Snapshot();

            // TODO: only pause the view if we're not currently using it.
            // view.Pause();

            if (useElevatedCredentials)
            {
                //used the elevated credentials for snapshot, need to close vnc when finished
                Program.Invoke(this, () => view.Dispose());
            }

            return snapshot;
        }

        public void closeVNCForSource(VM source)
        {
            Program.AssertOnEventThread();

            if (!vncViews.ContainsKey(source))
                return;

            VNCView vncView = vncViews[source];

            if (!vncView.isDocked)
                return;

            vncViews.Remove(source);
            vncView.Dispose();

            if (activeVMConsoles.Contains(source)) 
                activeVMConsoles.Remove(source);
        }

        public void closeVNCForSource(VM source, bool vncOnly)
        {
            if (!vncViews.ContainsKey(source) || vncViews[source] == null
                || (vncOnly && !vncViews[source].IsVNC))
                return;
            closeVNCForSource(source);
        }

        public bool isVNCPausedForSource(VM source)
        {
            Program.AssertOnEventThread();
            VNCView vncView = null;
            if (vncViews.ContainsKey(source))
            {
                vncView = vncViews[source];
                return vncView.isPaused;
            }
            return false;
        }

        private void SetErrorMessage(String message)
        {
            this.errorLabel.Text = message;
            this.errorLabel.Visible = true;
            this.Controls.Add(this.errorLabel);
            this.setCurrentSource((VM)null);
        }

        private void ClearErrorMessage()
        {
            this.errorLabel.Text = "";
            this.errorLabel.Visible = false;
            this.Controls.Remove(this.errorLabel);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsolePanel));
            this.errorLabel = new System.Windows.Forms.Label();
            this.RbacWarningPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lableRbacWarning = new System.Windows.Forms.Label();
            this.RbacWarningPanel.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // errorLabel
            // 
            resources.ApplyResources(this.errorLabel, "errorLabel");
            this.errorLabel.Name = "errorLabel";
            // 
            // RbacWarningPanel
            // 
            this.RbacWarningPanel.BackColor = System.Drawing.Color.Transparent;
            this.RbacWarningPanel.Controls.Add(this.tableLayoutPanel3);
            resources.ApplyResources(this.RbacWarningPanel, "RbacWarningPanel");
            this.RbacWarningPanel.Name = "RbacWarningPanel";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.pictureBox2, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lableRbacWarning, 1, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // lableRbacWarning
            // 
            resources.ApplyResources(this.lableRbacWarning, "lableRbacWarning");
            this.lableRbacWarning.Name = "lableRbacWarning";
            // 
            // ConsolePanel
            // 
            this.Controls.Add(this.RbacWarningPanel);
            this.Name = "ConsolePanel";
            resources.ApplyResources(this, "$this");
            this.RbacWarningPanel.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        public void SendCAD()
        {
            if (this.activeVNCView != null)
                this.activeVNCView.SendCAD();
        }

        internal void SwitchIfRequired()
        {
            if (activeVNCView != null)
                activeVNCView.SwitchIfRequired();
        }

        #region Close VNC connection

        private const int CLOSE_VNC_INTERVAL = 20000; //20 milliseconds 

        private static readonly Dictionary<VM, Timer> CloseVNCTimers = new Dictionary<VM, Timer>();

        public void StartCloseVNCTimer(VNCView vncView)
        {
            if (vncView == null)
                return;

            // find the <VM, VNCView> pair in vncViews and start timer on the vm 
            foreach (var kvp in vncViews.Where(kvp => kvp.Value == vncView))
            {
                StartCloseVNCTimer(kvp.Key);
                break;
            }
        }

        private void StartCloseVNCTimer(VM vm)
        {
            Program.AssertOnEventThread();

            if (CloseVNCTimers.ContainsKey(vm) || !vncViews.ContainsKey(vm))
                return;

            var t = new Timer {Interval = CLOSE_VNC_INTERVAL};

            t.Tick += delegate
                          {
                              Program.AssertOnEventThread();
                              try
                              {
                                  log.DebugFormat("ConsolePanel: closeVNCForSource({0}) in delegate", vm.Name);
                                  closeVNCForSource(vm, true);
                              }
                              catch (Exception exception)
                              {
                                  log.ErrorFormat("ConsolePanel: Exception closing the VNC console for {0}: {1}",
                                                  vm.Name, exception.Message);
                              }

                              t.Stop();
                              CloseVNCTimers.Remove(vm);
                              log.DebugFormat(
                                  "ConsolePanel: CloseVNCTimer({0}): Timer stopped and removed in delegate",
                                  vm.Name);
                          };

            CloseVNCTimers.Add(vm, t);
            log.DebugFormat("ConsolePanel: CloseVNCTimer({0}): Start timer (timers count {1})", vm.Name, CloseVNCTimers.Count);
            t.Start();
        }

        private static void StopCloseVncTimer(VM vm)
        {
            Program.AssertOnEventThread();

            if (!CloseVNCTimers.ContainsKey(vm) || CloseVNCTimers[vm] == null) 
                return;

            CloseVNCTimers[vm].Stop();
            CloseVNCTimers.Remove(vm);
            log.DebugFormat("ConsolePanel: StopCloseVncTimer({0}): Timer stopped and removed", vm.Name);
        }

        #endregion
    }
}
