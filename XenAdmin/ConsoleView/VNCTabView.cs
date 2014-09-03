﻿/* Copyright (c) Citrix Systems Inc. 
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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Commands;


namespace XenAdmin.ConsoleView
{
    public partial class VNCTabView : UserControl
    {
        private static readonly string UseRDP = Messages.VNC_RDESKTOP;
        // public only for the automated tests.
        public static readonly string UseVNC = Messages.VNC_VIRTUAL_CONSOLE;
        public static readonly string UseXVNC = Messages.VNC_X_CONSOLE;
        private static readonly string UseStandardDesktop = Messages.VNC_DEFAULT_DESKTOP;

        public const int INS_KEY_TIMEOUT = 500;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly XSVNCScreen vncScreen;
        private readonly VNCView parentVNCView;
        private readonly VM source;
        private readonly Host targetHost;
        private Form fullscreenForm = null;
        private Form fullscreenHint = null;
        private Size LastDesktopSize;
        private bool switchOnTabOpened = false;

        /// <summary>
        /// Whether to ignore VNC resize events.  We turn this on when changing
        /// the scaling settings, because this in turn triggers a VNC resize.
        /// </summary>
        private bool ignoringResizes = false;

        private bool ignoreScaleChange = false;

        internal readonly ConsoleKeyHandler KeyHandler = new ConsoleKeyHandler();

        public VNCTabView(VNCView parent, VM source, string elevatedUsername, string elevatedPassword)
        {
            Program.AssertOnEventThread();

            InitializeComponent();

            HostLabel.Font = Program.HeaderGradientFont;
            HostLabel.ForeColor = Program.HeaderGradientForeColor;
            multipleDvdIsoList1.SetTextColor(Program.HeaderGradientForeColor);

#pragma warning disable 0219
            // Force the handle to be created, because resize events
            // could be fired before this component is placed on-screen.
            IntPtr _ = Handle;
#pragma warning restore 0219

            this.parentVNCView = parent;
            this.scaleCheckBox.Checked = false;
            this.source = source;
            log.DebugFormat("'{0}' console: Register Server_PropertyChanged event listener on {0}", this.source.Name);
            this.source.PropertyChanged += Server_PropertyChanged;
            Host_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(Host_CollectionChanged);
            VM_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(VM_CollectionChanged);
            source.Connection.Cache.RegisterCollectionChanged<VM>(VM_CollectionChangedWithInvoke);

            if (source.is_control_domain)
            {
                Host host = source.Connection.Resolve(source.resident_on);
                if (host != null)
                {
                    log.DebugFormat("'{0}' console: Register Server_PropertyChanged event listener on {1}", this.source.Name, host.Name);
                    host.PropertyChanged += Server_PropertyChanged;

                    Host_metrics hostMetrics = source.Connection.Resolve(host.metrics);
                    if (hostMetrics != null)
                    {
                        log.DebugFormat("'{0}' console: Register Server_PropertyChanged event listener on host metrics", this.source.Name);
                        hostMetrics.PropertyChanged += Server_PropertyChanged;
                    }

                    HostLabel.Text = string.Format(Messages.CONSOLE_HOST, host.Name);
                    HostLabel.Visible = true;
                }
            }
            else
            {
                source.Connection.Cache.RegisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
                targetHost = source.GetStorageHost(false);
                
                foreach (Host cachedHost in source.Connection.Cache.Hosts)
                {
                    log.DebugFormat("'{0}' console: Register Server_EnabledPropertyChanged event listener on {1}",
                                    source.Name, cachedHost.Name);
                    cachedHost.PropertyChanged += Server_EnabledPropertyChanged;
                }
                
                HostLabel.Visible = false;
            }

            log.DebugFormat("'{0}' console: Update power state (on VNCTabView constructor)", this.source.Name);
            updatePowerState();
            this.vncScreen = new XSVNCScreen(source, new EventHandler(RDPorVNCResizeHandler), this, elevatedUsername, elevatedPassword);
            ShowGpuWarningIfRequired();

            if (source.is_control_domain || source.IsHVM && !source.HasRDP) //Linux HVM guests should only have one console: the console switch button vanishes altogether.
            {
                toggleConsoleButton.Visible = false;
            }
            else
            {
                toggleConsoleButton.Visible = true;
                this.vncScreen.OnDetectRDP = this.OnDetectRDP;
                this.vncScreen.OnDetectVNC = this.OnDetectVNC;
                this.vncScreen.UserCancelledAuth += this.OnUserCancelledAuth;
            }

            LastDesktopSize = vncScreen.DesktopSize;

            this.insKeyTimer = new System.Threading.Timer(new TimerCallback(notInsKeyPressed));

            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;

            registerShortCutKeys();

            //
            // Ctlr - Alt - Ins send Ctrl - Alt - Delete, and cancels and pending full screen.
            //

            KeyHandler.AddKeyHandler(ConsoleShortcutKey.CTRL_ALT_INS, cancelWaitForInsKeyAndSendCAD);

            this.vncScreen.Parent = this.contentPanel;
            this.vncScreen.Dock = DockStyle.Fill;

            this.Dock = DockStyle.Fill;

            string rdpLabel = GuessNativeConsoleLabel(source);
            this.toggleConsoleButton.Text = rdpLabel;

            UpdateFullScreenButton();

            UpdateDockButton();

            setupCD();

            UpdateParentMinimumSize();

            toggleConsoleButton.EnabledChanged += toggleConsoleButton_EnabledChanged;
        }

        //CA-75479 - add to aid debugging
        private void toggleConsoleButton_EnabledChanged(object sender, EventArgs e)
        {
            ButtonBase button = sender as ButtonBase;
            if(button == null)
                return;

            string format = "Console tab 'Switch to...' button disabled for VM '{0}'";
            
            if (button.Enabled)
                format = "Console tab 'Switch to...' button enabled for VM '{0}'";

            log.DebugFormat(format, source == null ? "unknown/null" : source.name_label);

        }

        private void VM_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Remove)
            {
                VM vm = e.Element as VM;
                if (source != null && vm.uuid == source.uuid)
                {
                    // the VM we are looking at has gone away. We should redock if necessary, otherwise it
                    // avoids the destroy (and re-create in the case of dom0) when the tab itself goes.
                    if (!parentVNCView.isDocked)
                        parentVNCView.DockUnDock();
                }
            }
        }

        private void Host_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (source.is_control_domain)
                return;

            Host host = e.Element as Host;
            if (host != null)
            {
                if (e.Action == CollectionChangeAction.Add)
                {
                    log.DebugFormat("'{0}' console: Register Server_EnabledPropertyChanged event listener on {1}",
                                    source.Name, host.Name);
                    host.PropertyChanged -= Server_EnabledPropertyChanged;
                    host.PropertyChanged += Server_EnabledPropertyChanged;
                }
                else if (e.Action == CollectionChangeAction.Remove)
                {
                    log.DebugFormat("'{0}' console: Unregister Server_EnabledPropertyChanged event listener on {1}",
                                    source.Name, host.Name);
                    host.PropertyChanged -= Server_EnabledPropertyChanged;
                }
            }
        }

        private void UnregisterEventListeners()
        {
            Properties.Settings.Default.PropertyChanged -= new PropertyChangedEventHandler(Default_PropertyChanged);

            if (source == null)
                return;

            log.DebugFormat("'{0}' console: Unregister Server_PropertyChanged event listener on {0}", source.Name);
            source.PropertyChanged -= new PropertyChangedEventHandler(Server_PropertyChanged);
            source.Connection.Cache.DeregisterCollectionChanged<VM>(VM_CollectionChangedWithInvoke);

            if (source.is_control_domain)
            {
                Host host = source.Connection.Resolve<Host>(source.resident_on);
                if (host != null)
                {
                    log.DebugFormat("'{0}' console: Unregister Server_PropertyChanged event listener on {1}",
                                    source.Name, host.Name);
                    host.PropertyChanged -= Server_PropertyChanged;

                    Host_metrics hostMetrics = source.Connection.Resolve<Host_metrics>(host.metrics);
                    if (hostMetrics != null)
                    {
                        log.DebugFormat("'{0}' console: Unregister Server_PropertyChanged event listener on host metrics",
                                        source.Name);
                        hostMetrics.PropertyChanged -= Server_PropertyChanged;
                    }
                }
            }
            else
            {
                source.Connection.Cache.DeregisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);

                foreach (Host cachedHost in source.Connection.Cache.Hosts)
                {
                    log.DebugFormat("'{0}' console: Unregister Server_EnabledPropertyChanged event listener on {1}",
                                    source.Name, cachedHost.Name);
                    cachedHost.PropertyChanged -= Server_EnabledPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Occurs when one of the properties in the preferences / options window changes.
        /// </summary>
        private void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Program.Invoke(this, delegate
            {
                deregisterShortCutKeys();
                registerShortCutKeys();
                UpdateParentMinimumSize();
            });
        }

        internal void UpdateParentMinimumSize()
        {
            if (Parent != null)
            {
                int[] bottomTableWidths = tableLayoutPanel1.GetColumnWidths();
                int bottomPanelWidth = bottomTableWidths.Where((t, i) => tableLayoutPanel1.ColumnStyles[i].SizeType != SizeType.Percent).Sum();
                Parent.MinimumSize = new Size(bottomPanelWidth + 20, 400);
            }
        }

        private void registerShortCutKeys()
        {
            Program.AssertOnEventThread();

            if (vncScreen == null)
                return;

            if (Properties.Settings.Default.FullScreenShortcutKey == 0)
            {
                // Ctrl + Alt
                KeyHandler.AddKeyHandler(ConsoleShortcutKey.CTRL_ALT, waitForInsKey);
            }
            else if (Properties.Settings.Default.FullScreenShortcutKey == 1)
            {
                // Ctrl + Alt + F
                KeyHandler.AddKeyHandler(ConsoleShortcutKey.CTRL_ALT_F, toggleFullscreen);
            }
            else if (Properties.Settings.Default.FullScreenShortcutKey == 2)
            {
                // F12
                KeyHandler.AddKeyHandler(ConsoleShortcutKey.F12, toggleFullscreen);
            }
            else if (Properties.Settings.Default.FullScreenShortcutKey == 3)
            {
                // Ctrl + Enter
                KeyHandler.AddKeyHandler(ConsoleShortcutKey.CTRL_ENTER, toggleFullscreen);
            }

            UpdateFullScreenButton();

            // CA-10943
            if (Properties.Settings.Default.DockShortcutKey == 1)
            {
                // Alt + Shift + U
                KeyHandler.AddKeyHandler(ConsoleShortcutKey.ALT_SHIFT_U, toggleDockUnDock);}
            else if (Properties.Settings.Default.DockShortcutKey == 2)
            {
                // F11
                KeyHandler.AddKeyHandler(ConsoleShortcutKey.F11, toggleDockUnDock); 
            }
            else if (Properties.Settings.Default.DockShortcutKey == 0)
            {
                // <none>
                KeyHandler.RemoveKeyHandler(ConsoleShortcutKey.ALT_SHIFT_U);
                KeyHandler.RemoveKeyHandler(ConsoleShortcutKey.F11); 
            }

            UpdateDockButton();

            // Uncapture keyboard and mouse Key
            if (Properties.Settings.Default.UncaptureShortcutKey == 0)
            {
                // Right Ctrl
                KeyHandler.AddKeyHandler(ConsoleShortcutKey.RIGHT_CTRL, ToogleConsoleFocus); 
            }
            else if (Properties.Settings.Default.UncaptureShortcutKey == 1)
            {
                // Left Alt
                KeyHandler.AddKeyHandler(ConsoleShortcutKey.LEFT_ALT, ToogleConsoleFocus); 
            }
        }

        private void deregisterShortCutKeys()
        {
            Program.AssertOnEventThread();

            if (vncScreen == null)
                return;

            if (Properties.Settings.Default.FullScreenShortcutKey != 0)
            {
                // Ctrl + Alt
                KeyHandler.RemoveKeyHandler(ConsoleShortcutKey.CTRL_ALT);
            }

            if (Properties.Settings.Default.FullScreenShortcutKey != 1)
            {
                // Ctrl + Alt + F
                KeyHandler.RemoveKeyHandler(ConsoleShortcutKey.CTRL_ALT_F);
            }

            if (Properties.Settings.Default.FullScreenShortcutKey != 2)
            {
                // F12
                KeyHandler.RemoveKeyHandler(ConsoleShortcutKey.F12);
            }
            if (Properties.Settings.Default.FullScreenShortcutKey != 3)
            {
                // Ctrl + Enter
                KeyHandler.RemoveKeyHandler(ConsoleShortcutKey.CTRL_ENTER); 
            }

            if (Properties.Settings.Default.DockShortcutKey != 1)
            {
                // Alt + Shift + U
                KeyHandler.RemoveKeyHandler(ConsoleShortcutKey.ALT_SHIFT_U); 
            }

            if (Properties.Settings.Default.DockShortcutKey != 2)
            {
                // F11
                KeyHandler.RemoveKeyHandler(ConsoleShortcutKey.F11); 
            }

            // Uncapture keyboard and mouse Key
            if (Properties.Settings.Default.UncaptureShortcutKey != 0)
            {
                // Right Ctrl
                KeyHandler.RemoveKeyHandler(ConsoleShortcutKey.RIGHT_CTRL); 
            }
            if (Properties.Settings.Default.UncaptureShortcutKey != 1)
            {
                // Left Alt
                KeyHandler.RemoveKeyHandler(ConsoleShortcutKey.LEFT_ALT); 
            }
        }

        public void UpdateDockButton()
        {
            dockButton.Text = parentVNCView.isDocked ? Messages.VNC_UNDOCK : Messages.VNC_REDOCK;
            if (Properties.Settings.Default.DockShortcutKey == 1)
            {
                dockButton.Text += Messages.VNC_DOCK_ALT_SHIFT_U;
            }
            else if (Properties.Settings.Default.DockShortcutKey == 2)
            {
                dockButton.Text += Messages.VNC_DOCK_F11;
            }
            dockButton.Image = parentVNCView.isDocked ? Properties.Resources.detach_24 : Properties.Resources.attach_24;
        }

        public void UpdateFullScreenButton()
        {
            switch (Properties.Settings.Default.FullScreenShortcutKey)
            {
                case 0:
                    fullscreenButton.Text = Messages.VNC_FULLSCREEN_CTRL_ALT;
                    break;
                case 1:
                    fullscreenButton.Text = Messages.VNC_FULLSCREEN_CTRL_ALT_F;
                    break;
                case 2:
                    fullscreenButton.Text = Messages.VNC_FULLSCREEN_F12;
                    break;
                default:
                    fullscreenButton.Text = Messages.VNC_FULLSCREEN_CTRL_ENTER;
                    break;
            }
        }

        private void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "power_state"
                || e.PropertyName == "live"
                || e.PropertyName == "allowed_operations")
            {
                log.DebugFormat("'{0}' console: Update power state, after receiving property change notification, PropertyName='{1}'", sender.ToString(), e.PropertyName);
                updatePowerState();
            }
            else if (e.PropertyName == "VBDs")
            {
                //The CD device may have changed
                Program.Invoke(this, setupCD);
            }
            if (source.is_control_domain && e.PropertyName == "name_label")
            {
                HostLabel.Text = string.Format(Messages.CONSOLE_HOST, source.AffinityServerString);
                if (parentVNCView != null && parentVNCView.undockedForm != null)
                    parentVNCView.undockedForm.Text = source.AffinityServerString;
            }
        }

        private void Server_EnabledPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "enabled" || source.is_control_domain)
                return;

            Host host = sender as Host;
            if (host == null)
                return;

            if (targetHost == null || targetHost.Equals(host))
            {
                log.DebugFormat(
                    "'{0}' console: Update power state, after receiving property change notification, PropertyName='{1}'",
                    sender.ToString(), e.PropertyName);
                updatePowerState();
            }
        }

        public void setupCD()
        {
            multipleDvdIsoList1.VM = source;
        }

        private void cdLabel_Click(object sender, EventArgs e)
        {
            new InstallToolsCommand(Program.MainWindow, source, this).Execute();
        }

        private void updatePowerState()
        {
            if (source.is_control_domain)
            {
                Host host = source.Connection.Resolve<Host>(source.resident_on);
                if (host == null)
                    return;

                Host_metrics hostMetrics = source.Connection.Resolve<Host_metrics>(host.metrics);
                if (hostMetrics == null)
                    return;

                if (hostMetrics.live)
                {
                    Program.Invoke(this, showTopBarContents);
                }
                else
                {
                    Program.Invoke(this, hideTopBarContents);
                }
            }
            else
            {
                switch (source.power_state)
                {
                    case vm_power_state.Halted:
                    case vm_power_state.Paused:
                    case vm_power_state.Suspended:
                        Program.Invoke(this, hideTopBarContents);
                        break;
                    case vm_power_state.Running:
                        Program.Invoke(this, showTopBarContents);
                        Program.Invoke(this, maybeEnableButton);
                        break;
                }
            }
        }

        /// <summary>
        /// CA-8966: No way to get from graphical to text console if vm's networking is broken on startup
        /// </summary>
        private void maybeEnableButton()
        {
            if (vncScreen != null && (!vncScreen.UseVNC || !vncScreen.UseSource))
            {
                toggleConsoleButton.Enabled = true;
            }
        }

        private void EnablePowerStateLabel(String label)
        {
            powerStateLabel.Enabled = true;
            powerStateLabel.Text = label;
            powerStateLabel.Cursor = Cursors.Hand;
        }

        private void DisablePowerStateLabel(String label)
        {
            powerStateLabel.Enabled = false;
            powerStateLabel.Text = label;
            powerStateLabel.Cursor = Cursors.Default;
        }

        private void hideTopBarContents()
        {
            VMPowerOff();
            if (source.is_control_domain)
            {
                log.DebugFormat("'{0}' console: Hide top bar contents, server is unavailable", source.Name);
                DisablePowerStateLabel(Messages.CONSOLE_HOST_DEAD);
            }
            else
            {
                log.DebugFormat("'{0}' console: Hide top bar contents, powerstate='{1}'", source.Name, vm_power_state_helper.ToString(source.power_state));
                if (source.power_state == vm_power_state.Halted)
                {
                    if (source.allowed_operations.Contains(vm_operations.start) &&
                        Helpers.EnabledTargetExists(targetHost, source.Connection))
                    {
                        EnablePowerStateLabel(Messages.CONSOLE_POWER_STATE_HALTED_START);
                    }
                    else
                    {
                        DisablePowerStateLabel(Messages.CONSOLE_POWER_STATE_HALTED);
                    }
                }
                else if (source.power_state == vm_power_state.Paused)
                {
                    if (source.allowed_operations.Contains(vm_operations.unpause))
                    {
                        //EnablePowerStateLabel(Messages.CONSOLE_POWER_STATE_PAUSED_UNPAUSE);
                        // CA-12637: Pause/UnPause is not supported in the GUI.  Comment out
                        // the EnablePowerStateLabel because it gives the appearance that we
                        // support unpause via the GUI.
                        DisablePowerStateLabel(Messages.CONSOLE_POWER_STATE_PAUSED);
                    }
                    else
                    {
                        DisablePowerStateLabel(Messages.CONSOLE_POWER_STATE_PAUSED);
                    }
                }
                else if (source.power_state == vm_power_state.Suspended)
                {
                    if (source.allowed_operations.Contains(vm_operations.resume) &&
                        Helpers.EnabledTargetExists(targetHost, source.Connection))
                    {
                        EnablePowerStateLabel(Messages.CONSOLE_POWER_STATE_SUSPENDED_RESUME);
                    }
                    else
                    {
                        DisablePowerStateLabel(Messages.CONSOLE_POWER_STATE_SUSPENDED);
                    }
                }
            }
            powerStateLabel.Show();
        }

        private void showTopBarContents()
        {
            log.DebugFormat("'{0}' console: Show top bar contents, source is running", source.Name);
            Program.AssertOnEventThread();
            VMPowerOn();
            powerStateLabel.Hide();
        }

        private void powerStateLabel_Click(object sender, EventArgs e)
        {
            if (!powerStateLabel.Enabled)
            {
                return;
            }

            switch (source.power_state)
            {
                case vm_power_state.Halted:
                    DisablePowerStateLabel(Messages.CONSOLE_POWER_STATE_HALTED);
                    if (source.allowed_operations.Contains(vm_operations.start))
                    {
                        DisablePowerStateLabel(powerStateLabel.Text);

                        new StartVMCommand(Program.MainWindow, source).Execute();
                    }
                    break;
                case vm_power_state.Paused:
                    DisablePowerStateLabel(Messages.CONSOLE_POWER_STATE_PAUSED);
                    //if(source.allowed_operations.Contains(vm_operations.unpause))
                    //    new Actions.VmAction(source.Connection, VmActionKind.UnpauseActiveView, source, null).RunAsync();
                    break;
                case vm_power_state.Suspended:
                    DisablePowerStateLabel(Messages.CONSOLE_POWER_STATE_SUSPENDED);
                    if (source.allowed_operations.Contains(vm_operations.resume))
                    {
                        DisablePowerStateLabel(powerStateLabel.Text);
                        new ResumeVMCommand(Program.MainWindow, source).Execute();
                    }
                    break;
            }
        }

        public bool isPaused
        {
            get
            {
                if (vncScreen != null && !isFullscreen)
                    return vncScreen.wasPaused;
                else
                    return false;
            }
        }

        public void Pause()
        {
            if (vncScreen != null && !isFullscreen)
                vncScreen.Pause();
        }

        public void Unpause()
        {
            if (vncScreen != null)
                vncScreen.Unpause();
        }

        // Make the 'enable RDP' button show something sensible if we can...
        private string GuessNativeConsoleLabel(VM source)
        {
            string label = Messages.VNC_LOOKING;

            if (source == null)
                return label;

            XenRef<VM_guest_metrics> gm = source.guest_metrics;
            if (gm == null)
                return label;

            if (Program.MainWindow.SelectionManager.Selection.GetConnectionOfFirstItem() == null)
                return label;

            VM_guest_metrics gmo = Program.MainWindow.SelectionManager.Selection.GetConnectionOfFirstItem().Resolve<VM_guest_metrics>(gm);
            if (gmo == null)
                return label;

            if (gmo != null && gmo.os_version != null)
            {
                if (gmo.os_version.ContainsKey("name"))
                {
                    string osString = gmo.os_version["name"];
                    if (osString != null)
                    {
                        if (osString.Contains("Microsoft"))
                            label = UseRDP;
                        else
                            label = UseXVNC;
                    }
                }
            }

            return label;
        }

        private void RDPorVNCResizeHandler(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();

            if (vncScreen == null)
                return; // This is the first event when vncScreen is created.

            VNCResizeHandler_();
        }

        private void VNCResizeHandler_()
        {
            if (!ignoringResizes &&
                DesktopSizeHasChanged() &&
                !scaleCheckBox.Checked)
            {
                MaybeScale();
            }
        }

        public void MaybeScale()
        {
            Program.AssertOnEventThread();

            if (vncScreen.DesktopSize.Width > 10 &&
                contentPanel.Width < vncScreen.DesktopSize.Width)
            {
                if (!Properties.Settings.Default.PreserveScaleWhenSwitchBackToVNC)
                {
                    scaleCheckBox.Checked = true;
                }
                else
                {
                    scaleCheckBox.Checked = oldScaleValue || firstTime;
                    firstTime = false;
                }
            }
            else if (Properties.Settings.Default.PreserveScaleWhenSwitchBackToVNC)
            {
                scaleCheckBox.Checked = oldScaleValue;
            }
            scaleCheckBox_CheckedChanged(null, null);
        }

        private bool DesktopSizeHasChanged()
        {
            if (!LastDesktopSize.Equals(vncScreen.DesktopSize))
            {
                LastDesktopSize = vncScreen.DesktopSize;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Size GrowToFit()
        {
            Program.AssertOnEventThread();
            ignoringResizes = true;
            try
            {
                scaleCheckBox.Checked = false;
                Size working_area = Screen.FromControl(this).WorkingArea.Size;
                if (vncScreen.DesktopSize.Width > 10 &&
                    vncScreen.DesktopSize.Width < working_area.Width &&
                    vncScreen.DesktopSize.Height < working_area.Height)
                {
                    int twoTimeBorderPadding = VNCGraphicsClient.BORDER_PADDING * 2;

                    return new Size(vncScreen.DesktopSize.Width + twoTimeBorderPadding,
                                    vncScreen.DesktopSize.Height + buttonPanel.Height + bottomPanel.Height + twoTimeBorderPadding);
                }
                else
                {
                    scaleCheckBox.Checked = true;
                    return new Size((working_area.Width * 2) / 3, (working_area.Height * 2) / 3);
                }
            }
            finally
            {
                ignoringResizes = false;
            }
        }

        private void scaleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreScaleChange)
                return;

            try
            {
                ignoringResizes = true;
                this.vncScreen.Scaling = this.scaleCheckBox.Checked;
            }
            finally
            {
                ignoringResizes = false;
            }

            FocusVNC();
        }

        private void sendCAD_Click(object sender, EventArgs e)
        {
            this.vncScreen.SendCAD();
            FocusVNC();
        }

        private void dockButton_Click(object sender, EventArgs e)
        {
            if (isFullscreen)
                return;
            this.parentVNCView.DockUnDock();
        }

        private void fullscreenButton_Click(object sender, EventArgs e)
        {
            toggleFullscreen();
        }

        private System.Threading.Timer insKeyTimer;

        private void waitForInsKey()
        {
            lock (this.insKeyTimer)
            {
                this.insKeyTimer.Change(INS_KEY_TIMEOUT, System.Threading.Timeout.Infinite);
            }
        }

        private void cancelWaitForInsKeyAndSendCAD()
        {
            lock (this.insKeyTimer)
            {
                // We have seen the INS key, so lets cancel the timer and send CAD

                this.insKeyTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                this.vncScreen.SendCAD();
            }
        }

        private void notInsKeyPressed(Object o)
        {
            Program.AssertOffEventThread();

            Program.Invoke(this, delegate()
            {
                lock (this.insKeyTimer)
                {
                    // We have not seen the INS key, so lets toggleFullscreen and cancel the timer

                    this.toggleFullscreen();
                    this.insKeyTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                }
            });
        }

        private bool isFullscreen
        {
            get
            {
                return fullscreenForm != null;
            }
        }

        // CA-10943
        private bool inToggleDockUnDock = false;
        private void toggleDockUnDock()
        {
            Program.AssertOnEventThread();

            if (inToggleDockUnDock)
                return;

            inToggleDockUnDock = true;
            dockButton_Click(null, null);
            inToggleDockUnDock = false;
        }

        private bool inToggleFullscreen = false;
        private void toggleFullscreen()
        {
            Program.AssertOnEventThread();

            if (inToggleFullscreen)
                return;

            inToggleFullscreen = true;

            if (!isFullscreen)
            {
                if (vncScreen.showConnectionBar) 
                    fullscreenForm = new XenAdmin.Controls.ConsoleTab.FullScreenForm(vncScreen);
                else
                    fullscreenForm = new Form();
                fullscreenForm.ShowIcon = false;
                fullscreenForm.ShowInTaskbar = false;

                fullscreenForm.FormBorderStyle = FormBorderStyle.None;
                fullscreenForm.FormClosing += new FormClosingEventHandler(
                    delegate(Object o, FormClosingEventArgs a)
                    {
                        toggleFullscreen();
                    });
                //fullscreenForm.Deactivate += new EventHandler(
                //    delegate(Object o, EventArgs e)
                //    {
                //        toggleFullscreen();
                //    });
                if (source != null && source.Connection != null)
                    source.Connection.BeforeConnectionEnd += Connection_BeforeConnectionEnd;

                vncScreen.Parent = fullscreenForm is Controls.ConsoleTab.FullScreenForm
                                       ? (Control) ((Controls.ConsoleTab.FullScreenForm) fullscreenForm).contentPanel
                                       : fullscreenForm;
                vncScreen.DisplayFocusRectangle = false;

                Screen screen = Screen.FromControl(this);
                fullscreenForm.StartPosition = FormStartPosition.Manual;
                fullscreenForm.Location = screen.WorkingArea.Location;
                fullscreenForm.Size = screen.Bounds.Size;

                fullscreenHint = new Controls.ConsoleTab.FullScreenHint(GetFullScreenMessage());                
                
                fullscreenHint.Show(fullscreenForm);
                fullscreenForm.Show();

                FocusVNC();
                vncScreen.CaptureKeyboardAndMouse();
            }
            else
            {
                if (source != null && source.Connection != null)
                    source.Connection.BeforeConnectionEnd -= Connection_BeforeConnectionEnd;

                vncScreen.Parent = this.contentPanel;
                vncScreen.DisplayFocusRectangle = true;
                FocusVNC();
                vncScreen.CaptureKeyboardAndMouse();

                fullscreenForm.Hide();
                fullscreenForm.Dispose();
                fullscreenForm = null;
            }

            //Everytime we toggle full screen I'm going to force an unpause to make sure we don't acidentally undock / dock a pause VNC
            vncScreen.Unpause();

            inToggleFullscreen = false;

            // CA-30477: This refresh stops a scroll bar being painted on the fullscreen form under vista
            if (fullscreenForm != null)
                fullscreenForm.Refresh();
        }

        string GetFullScreenMessage()
        {
            switch (Properties.Settings.Default.FullScreenShortcutKey)
            {
                case 0:
                    return Messages.VNC_FULLSCREEN_MESSAGE_CTRL_ALT;
                case 1:
                    return Messages.VNC_FULLSCREEN_MESSAGE_CTRL_ALT_F;
                case 2:
                    return Messages.VNC_FULLSCREEN_MESSAGE_F12;
                default:
                    return Messages.VNC_FULLSCREEN_MESSAGE_CTRL_ENTER;
            }
  }
        void Connection_BeforeConnectionEnd(object sender, EventArgs e)
        {
            Program.Invoke(this, toggleFullscreen);
        }

        private const bool RDP = true;
        private const bool XVNC = false;
        private bool toggleToXVNCorRDP = RDP;

        public void DisableToggleVNCButton()
        {
            Program.AssertOnEventThread();
            toggleConsoleButton.Enabled = false;
        }

        private void OnDetectRDP()
        {
            Program.Invoke(this, OnDetectRDP_);
        }

        private void OnDetectRDP_()
        {
            try
            {
                log.DebugFormat("RDP detected for VM '{0}'", source == null ? "unknown/null" : source.name_label);
                this.toggleToXVNCorRDP = RDP;
                if (vncScreen.UseVNC) 
                    this.toggleConsoleButton.Text = UseRDP;
                this.toggleConsoleButton.Enabled = true;
                tip.SetToolTip(this.toggleConsoleButton, null);
                if (!vncScreen.UserWantsToSwitchProtocol&& Properties.Settings.Default.AutoSwitchToRDP)
                {
                    if (Program.MainWindow.TheTabControl.SelectedTab == Program.MainWindow.TabPageConsole)
                        toggleConsoleButton_Click(null, null);
                    else
                        switchOnTabOpened = true;
                }
            }
            catch (InvalidOperationException exn)
            {
                log.Warn(exn, exn);
            }
        }

        private void OnDetectVNC()
        {
            Program.Invoke(this, OnDetectVNC_);
        }

        private void OnDetectVNC_()
        {
            try
            {
                log.DebugFormat("VNC detected for VM '{0}'", source == null ? "unknown/null" : source.name_label);
                this.toggleToXVNCorRDP = XVNC;
                this.toggleConsoleButton.Text = vncScreen.UseSource ? UseXVNC : UseVNC;
                this.toggleConsoleButton.Enabled = true;
                tip.SetToolTip(this.toggleConsoleButton, null);
            }
            catch (InvalidOperationException exn)
            {
                log.Warn(exn, exn);
            }
        }

        private bool firstTime = true;
        private bool oldScaleValue = false;

        /// <summary>
        /// Switch between graphical and text consoles.
        /// </summary>
        private void toggleConsoleButton_Click(object sender, EventArgs e)
        {
            bool rdp = (toggleToXVNCorRDP == RDP);

            try
            {
                if (rdp)
                {
                    if (vncScreen.UseVNC)
                        oldScaleValue = scaleCheckBox.Checked;

                    vncScreen.UseVNC = !vncScreen.UseVNC;
                    vncScreen.UserWantsToSwitchProtocol = true;
                    
                    if (vncScreen.rdpIP == null && vncScreen.UseVNC && Properties.Settings.Default.EnableRDPPolling)
                    {
                        toggleConsoleButton.Enabled = false;
                    }
                    else
                    {
                        if (vncScreen.rdpIP == null) // disable toggleConsoleButton; it will be re-enabled in TryToConnectRDP() when rdp port polling is complete (CA-102755)
                            toggleConsoleButton.Enabled = false;
                        ThreadPool.QueueUserWorkItem(TryToConnectRDP);

                    }

                }
                else
                {
                    oldScaleValue = scaleCheckBox.Checked;
                    vncScreen.UseSource = !vncScreen.UseSource;
                    
                    if (vncScreen.vncIP == null && vncScreen.UseSource && Properties.Settings.Default.EnableRDPPolling)
                    {
                        toggleConsoleButton.Enabled = false;
                    }
                }
                Unpause();
                UpdateButtons();
            }
            catch(COMException ex)
            {
                log.DebugFormat("Disabling toggle-console button as COM related exception thrown: {0}", ex.Message);
                toggleConsoleButton.Enabled = false;
            }
        }

        private void UpdateButtons()
        {
            bool rdp = (toggleToXVNCorRDP == RDP);
            if (rdp)
                toggleConsoleButton.Text = vncScreen.UseVNC ? UseRDP : UseStandardDesktop;
            else
                toggleConsoleButton.Text = vncScreen.UseSource ? UseXVNC : UseVNC;
            scaleCheckBox.Visible = !rdp || vncScreen.UseVNC;
            sendCAD.Enabled = !rdp || vncScreen.UseVNC;
            FocusVNC();
            ignoreScaleChange = true;
            if (!Properties.Settings.Default.PreserveScaleWhenSwitchBackToVNC)
            {
                scaleCheckBox.Checked = false;
            }
            ignoreScaleChange = false;
            scaleCheckBox_CheckedChanged(null, null);  // make sure scale setting is now correct: CA-84324
        }


        private void TryToConnectRDP(object x)
        {
            bool hasToReconnect = vncScreen.rdpIP == null;
            vncScreen.rdpIP = vncScreen.PollPort(XSVNCScreen.RDP_PORT, true);
            Program.Invoke(this, (MethodInvoker)(() =>
            {
                if (hasToReconnect)
                {
                    vncScreen.UseVNC = true;
                    vncScreen.UseVNC = false;
                }
                Unpause();
                UpdateButtons();
                toggleConsoleButton.Enabled = true; // make sure the toggleConsoleButton is enabled after rdp port polling (CA-102755) 
            }));

        }

        /// <summary>
        /// Called when the user cancelled VNC authentication.
        /// </summary>
        private void OnUserCancelledAuth(object sender, EventArgs e)
        {
            // Switch back to the text console
            toggleConsoleButton_Click(null, null);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            FocusVNC();
        }

        internal void SendCAD()
        {
            if (this.vncScreen != null)
                this.vncScreen.SendCAD();
        }

        internal void focus_vnc()
        {
            if (this.vncScreen != null)
                FocusVNC();
        }

        // Focus the VNC screen, as long as we're in the right place at the moment.
        // Otherwise ignore the request to focus or it will steal the keyboard from
        // the rightful owner: see CA-41120.
        private void FocusVNC()
        {
            if (Program.MainWindow.ContainsFocus && Program.MainWindow.TheTabControl.SelectedTab == Program.MainWindow.TabPageConsole)
                vncScreen.Focus();
        }

        internal void VMPowerOff()
        {
            toggleConsoleButton.Enabled = false;

            VBD cddrive = source.FindVMCDROM();
            bool allowEject = cddrive != null ? cddrive.allowed_operations.Contains(vbd_operations.eject) : false;
            bool allowInsert = cddrive != null ? cddrive.allowed_operations.Contains(vbd_operations.insert) : false;
            multipleDvdIsoList1.Enabled = (source.power_state == vm_power_state.Halted) && (allowEject || allowInsert);

            sendCAD.Enabled = false;
        }

        internal void VMPowerOn()
        {
            //No need to reenable toggleConsoleButton, polling will do it.
            multipleDvdIsoList1.Enabled = true;
            sendCAD.Enabled = true;
        }

        internal void RdpDisconnectedHandler(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();

            if (vncScreen == null)
                return; // This is the first event when vncScreen is created.

            rdpDisconnected();
        }

        private void rdpDisconnected()
        {
            if (!vncScreen.UseVNC)
                toggleConsoleButton_Click(null, null);

            toggleConsoleButton.Enabled = false;
            vncScreen.imediatelyPollForConsole();
        }

        internal void SwitchIfRequired()
        {
            if (switchOnTabOpened)
            {
                toggleConsoleButton_Click(null, null);
                switchOnTabOpened = false;
            }
        }

        bool droppedDown = false;
        private readonly CollectionChangeEventHandler Host_CollectionChangedWithInvoke;
        private readonly CollectionChangeEventHandler VM_CollectionChangedWithInvoke;

        private void LifeCycleButton_MouseClick(object sender, EventArgs e)
        {
            if (droppedDown)
            {
                LifeCycleMenuStrip.Hide();
                return;
            }
            if (source == null)
            {
                return;
            }
            Host host = source.Connection.Resolve<Host>(source.resident_on);
            if (host == null)
            {
                return;
            }

            ContextMenuItemCollection contextMenuItems = new ContextMenuItemCollection();

            if (source.is_control_domain)
            {
                // We're looking at the host console
                if (host.Connection.IsConnected)
                {
                    contextMenuItems.Add(new ShutDownHostCommand(Program.MainWindow, host, this));
                    contextMenuItems.Add(new RebootHostCommand(Program.MainWindow, host, this));
                }
            }
            else
            {
                contextMenuItems.AddIfEnabled(new ShutDownVMCommand(Program.MainWindow, source, this));
                contextMenuItems.AddIfEnabled(new RebootVMCommand(Program.MainWindow, source, this));
                contextMenuItems.AddIfEnabled(new SuspendVMCommand(Program.MainWindow, source, this));
                contextMenuItems.AddIfEnabled(new InstallToolsCommand(Program.MainWindow, source, this));
                contextMenuItems.AddIfEnabled(new ForceVMShutDownCommand(Program.MainWindow, source, this));
                contextMenuItems.AddIfEnabled(new ForceVMRebootCommand(Program.MainWindow, source, this));
            }

            LifeCycleMenuStrip.Items.Clear();
            LifeCycleMenuStrip.Items.AddRange(contextMenuItems.ToArray());
            LifeCycleMenuStrip.Show(pictureBox1, pictureBox1.Left, pictureBox1.Bottom);
        }

        public void showHeaderBar(bool showHeaderBar, bool showLifecycleIcon)
        {
            panel2.Visible = showHeaderBar;
            pictureBox1.Visible = showLifecycleIcon;
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.lifecycle_hot;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            if (droppedDown)
                pictureBox1.Image = Properties.Resources.lifecycle_pressed;
            else
                pictureBox1.Image = Properties.Resources._001_LifeCycle_h32bit_24;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox1.Image = Properties.Resources.lifecycle_pressed;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (droppedDown)
                pictureBox1.Image = Properties.Resources.lifecycle_pressed;
            else
                pictureBox1.Image = Properties.Resources.lifecycle_hot;
        }

        private void LifeCycleMenuStrip_Opened(object sender, EventArgs e)
        {
            droppedDown = true;
        }

        private void LifeCycleMenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason != ToolStripDropDownCloseReason.AppClicked || !pictureBox1.ClientRectangle.Contains(this.PointToClient(MousePosition)))
            {
                droppedDown = false;
                pictureBox1.Image = Properties.Resources._001_LifeCycle_h32bit_24;
            }
            else
            {
                e.Cancel = true;
            }
        }

        internal Image Snapshot()
        {
            if (vncScreen != null)
            {
                return vncScreen.Snapshot();
            }

            return null;
        }

        private bool inToogleConsoleFocus = false;
        private void ToogleConsoleFocus()
        {
            Program.AssertOnEventThread();

            if (inToogleConsoleFocus)
                return;

            inToogleConsoleFocus = true;

            if (vncScreen.Focused && vncScreen.ActiveControl == null)
                vncScreen.CaptureKeyboardAndMouse(); // focus console
            else
            {
                vncScreen.UncaptureKeyboardAndMouse(); // defocus console
                vncScreen.Refresh();
            }

            inToogleConsoleFocus = false;
        }

        internal void ShowGpuWarningIfRequired()
        {
            dedicatedGpuWarning.Visible = vncScreen != null && (vncScreen.UseVNC || string.IsNullOrEmpty(vncScreen.rdpIP)) &&
                vncScreen.Source.HasGPUPassthrough && vncScreen.Source.power_state == vm_power_state.Running;
        }

        internal bool IsVNC
        {
            get { return vncScreen.UseVNC; }
        }
    }
}
