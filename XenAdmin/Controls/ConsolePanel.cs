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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.ConsoleView;
using XenAPI;
using System.Drawing;
using XenAdmin.Help;


namespace XenAdmin.Controls
{
    public partial class ConsolePanel : UserControl, IControlWithHelp
    {
        private const int MAX_ACTIVE_VM_CONSOLES = 10;

        private VNCView activeVNCView;
        private Dictionary<VM, VNCView> vncViews = new Dictionary<VM, VNCView>();

        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ConsolePanel()
        {
            InitializeComponent();
            tableLayoutPanelRbac.Visible = false;
            ClearErrorMessage();
        }

        public virtual string HelpID => "TabPageConsole";

        public void PauseAllDockedViews()
        {
            foreach (VNCView vncView in vncViews.Values)
            {
                if (vncView.IsDocked)
                    vncView.Pause();
            }
        }

        public void ResetAllViews()
        {
            vncViews.Clear();
        }

        public void UnpauseActiveView(bool focus)
        {
            // We're going to explicitly pause all the consoles 
            // except the active one, then explicitly unpause the active one.

            foreach (VNCView vncView in vncViews.Values)
            {
                if (vncView != activeVNCView && vncView.IsDocked)
                    vncView.Pause();
            }

            if (activeVNCView != null)
            {
                activeVNCView.Unpause();

                if (focus)
                {
                    activeVNCView.FocusConsole();
                    activeVNCView.SwitchIfRequired();
                }
            }
        }

        public void UpdateRDPResolution(bool fullscreen = false)
        {
            if (activeVNCView != null)
                activeVNCView.UpdateRDPResolution(fullscreen);
        }

        internal void SetCurrentSource(VM source)
        {
            Program.AssertOnEventThread();

            tableLayoutPanelRbac.Visible = false;

            if (source == null)
            {
                if (activeVNCView != null)
                {
                    Controls.Remove(activeVNCView);
                    activeVNCView = null;
                }
                return;
            }

            if (RbacDenied(source, out var allowedRoles))
            {
                if (activeVNCView != null)
                {
                    Controls.Remove(activeVNCView);
                    activeVNCView = null;
                }

                string msg = allowedRoles.Count == 1 ? Messages.RBAC_CONSOLE_WARNING_ONE : Messages.RBAC_CONSOLE_WARNING_MANY;
                lableRbacWarning.Text = string.Format(msg,
                    Role.FriendlyCSVRoleList(source.Connection.Session.Roles),
                    Role.FriendlyCSVRoleList(allowedRoles));

                tableLayoutPanelRbac.Visible = true;
                return;
            }

            if (!vncViews.ContainsKey(source))
            {
                //remove one more as we're adding the selected further down
                //Take(arg) returns empty list if the arg <= 0
                var viewsToRemove = vncViews.Take(vncViews.Count - MAX_ACTIVE_VM_CONSOLES + 1).ToList();

                foreach (var view in viewsToRemove)
                {
                    if (!view.Value.IsDocked)
                    {
                        vncViews.Remove(view.Key);
                        view.Value.Dispose();
                    }
                }

                vncViews[source] = new VNCView(source, null, null) {Dock = DockStyle.Fill};
            }

            if (activeVNCView != vncViews[source])
            {
                Controls.Remove(activeVNCView);
                activeVNCView = vncViews[source];
                Controls.Add(activeVNCView);
            }

            activeVNCView.refreshIsoList();
            ClearErrorMessage();
        }

        internal virtual void SetCurrentSource(Host source)
        {
            if (source == null)
            {
                log.Error("No local copy of host information when connecting to host VNC console.");
                SetErrorMessage(Messages.VNC_COULD_NOT_CONNECT_CONSOLE);
                return;
            }

            VM dom0 = source.ControlDomainZero();
            if (dom0 == null)
            {
                log.Error("No dom0 on host when connecting to host VNC console.");
                SetErrorMessage(Messages.VNC_COULD_NOT_FIND_CONSOLES);
            }
            else
                SetCurrentSource(dom0);
        }

        private static bool RbacDenied(VM source, out List<Role> allowedRoles)
        {

            if (source == null || source.Connection == null)
            {
                allowedRoles = null;
                return false;
            }
            
            var session = source.Connection.Session;
            if (session == null || session.IsLocalSuperuser)
            {
                allowedRoles = null;
                return false;
            }

            string roleList = source.IsControlDomainZero(out _) ? "http/connect_console/host_console" : "http/connect_console";
            List<Role> validRoles = Role.ValidRoleList(roleList, source.Connection);
            allowedRoles = validRoles;
            return session.Roles.Find(r => validRoles.Contains(r)) == null;
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
                        view = new VNCView(vm, elevatedUsername, elevatedPassword) { Dock = DockStyle.Fill };
                    else
                    {
                        SetCurrentSource(vm);
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

        public void CloseVncForSource(VM source)
        {
            Program.AssertOnEventThread();

            if (!vncViews.ContainsKey(source))
                return;

            VNCView vncView = vncViews[source];

            if (!vncView.IsDocked)
                return;

            vncViews.Remove(source);
            vncView.Dispose();
        }

        protected void SetErrorMessage(string message)
        {
            errorLabel.Text = message;
            tableLayoutPanelError.Visible = true;
            SetCurrentSource((VM)null);
        }

        private void ClearErrorMessage()
        {
            tableLayoutPanelError.Visible = false;
        }

        public void SendCAD()
        {
            if (activeVNCView != null)
                activeVNCView.SendCAD();
        }
    }

    internal class CvmConsolePanel : ConsolePanel
    {
        internal override void SetCurrentSource(Host source)
        {
            if (source == null)
            {
                log.Error("No local copy of host information when connecting to host VNC console.");
                SetErrorMessage(Messages.VNC_COULD_NOT_CONNECT_CONSOLE);
                return;
            }

            VM cvm = source.OtherControlDomains().FirstOrDefault();
            if (cvm == null)
            {
                log.Error("Could not find CVM console on host.");
                SetErrorMessage(Messages.VNC_COULD_NOT_FIND_CONSOLES);
            }
            else
                SetCurrentSource(cvm);
        }

        public override string HelpID => "TabPageCvmConsole";
    }
}
