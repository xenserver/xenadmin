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
            while (activeVMConsoles.Count > MAX_ACTIVE_VM_CONSOLES)
            {
                closeVNCForSource(activeVMConsoles[0]);
                activeVMConsoles.RemoveAt(0);
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

            if (source == null || source.Connection == null || !Helpers.MidnightRideOrGreater(source.Connection))
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


    }
}
