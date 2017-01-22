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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.ConsoleView
{
    public partial class VNCView : UserControl
    {
        private readonly VM source;
        private readonly VNCTabView vncTabView;
        public Form undockedForm = null;

        public bool isDocked
        {
            get
            {
                return this.undockedForm == null || !this.undockedForm.Visible;
            }
        }

        public void Pause()
        {
            if (vncTabView != null && isDocked)
                vncTabView.Pause();
        }

        public void Unpause()
        {
            if(vncTabView != null)
                vncTabView.Unpause();
        }

        public VNCView(VM source, string elevatedUsername, string elevatedPassword)
        {
            Program.AssertOnEventThread();

            this.source = source;
            this.vncTabView = new VNCTabView(this, source, elevatedUsername, elevatedPassword) {Dock = DockStyle.Fill};

            InitializeComponent();
            this.Controls.Add(this.vncTabView);
        }

        private void UnregisterEventListeners()
        {
            if(source != null)
                source.PropertyChanged -= new PropertyChangedEventHandler(Server_PropertyChanged);
        }

        private Size oldUndockedSize = Size.Empty;
        private Point oldUndockedLocation = Point.Empty;
        private bool oldScaledSetting = false;

        public void DockUnDock()
        {
            if (this.isDocked)
            {
                if (this.undockedForm == null)
                {
                    undockedForm = new Form();
                    undockedForm.Text = UndockedWindowTitle(source);
                    source.PropertyChanged -= Server_PropertyChanged;
                    source.PropertyChanged += Server_PropertyChanged;
                    undockedForm.Icon = Program.MainWindow.Icon;
                    undockedForm.FormClosing += new FormClosingEventHandler(delegate(object sender, FormClosingEventArgs e)
                    {
                        this.DockUnDock();
                    });
                    undockedForm.StartPosition = FormStartPosition.CenterScreen;
                    undockedForm.Resize += new EventHandler(
                         delegate(Object o, EventArgs a)
                         {
                             if (undockedForm.WindowState == FormWindowState.Minimized)
                             {
                                 vncTabView.Pause();
                             }
                             else
                             {
                                 vncTabView.Unpause();
                             }
                         });
                }

                this.Controls.Remove(vncTabView);
                undockedForm.Controls.Add(vncTabView);

                oldScaledSetting = vncTabView.IsScaled;

                vncTabView.showHeaderBar(!source.is_control_domain, true);

                undockedForm.ClientSize = vncTabView.GrowToFit();

                if (oldUndockedSize != Size.Empty 
                    && oldUndockedLocation != Point.Empty
                    && HelpersGUI.WindowIsOnScreen(oldUndockedLocation, oldUndockedSize))
                {
                    undockedForm.Size = oldUndockedSize;
                    undockedForm.StartPosition = FormStartPosition.Manual;
                    undockedForm.Location = oldUndockedLocation;
                }

                undockedForm.HelpButton = true;
                undockedForm.HelpButtonClicked += undockedForm_HelpButtonClicked;
                undockedForm.HelpRequested += undockedForm_HelpRequested;

                undockedForm.Show();

                if(Properties.Settings.Default.PreserveScaleWhenUndocked)
                    vncTabView.IsScaled = oldScaledSetting;

                this.reattachConsoleButton.Show();
                this.findConsoleButton.Show();
            }
            else
            {
                //save location of undock vnc control
                this.oldUndockedLocation = undockedForm.Location;
                this.oldUndockedSize = undockedForm.Size;
                
                if (!Properties.Settings.Default.PreserveScaleWhenUndocked)
                    vncTabView.IsScaled = oldScaledSetting;

                this.reattachConsoleButton.Hide();
                this.findConsoleButton.Hide();

                undockedForm.Hide();
                vncTabView.showHeaderBar(true, false);
                undockedForm.Controls.Remove(vncTabView);
                this.Controls.Add(vncTabView);

                undockedForm.Dispose();
                undockedForm = null;
            }

            vncTabView.UpdateDockButton();

            vncTabView.UpdateParentMinimumSize();

            //Every time we dock / undock I'm going to force an unpause to make sure we don't ever pause a visible one.
            vncTabView.Unpause();
            vncTabView.focus_vnc();
        }

        private string UndockedWindowTitle(VM source)
        {
            if (source.is_control_domain)
            {
                Host host = source.Connection.Resolve(source.resident_on);
                if (host == null)
                    return source.Name;

                return string.Format(source.IsControlDomainZero ? Messages.CONSOLE_HOST : Messages.CONSOLE_HOST_NUTANIX, host.Name);
            }
            else
            {
                return source.Name;
            }
        }

        private void undockedForm_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Help.HelpManager.Launch("TabPageConsole");
            e.Cancel = true;
        }

        private void undockedForm_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Help.HelpManager.Launch("TabPageConsole");
            hlpevent.Handled = true;
        }

        void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" && undockedForm != null)
            {
                undockedForm.Text = source.name_label;
            }
        }

        private void findConsoleButton_Click(object sender, EventArgs e)
        {
            if (!this.isDocked)
                undockedForm.BringToFront();
            if (undockedForm.WindowState == FormWindowState.Minimized)
                undockedForm.WindowState = FormWindowState.Normal;
        }

        private void reattachConsoleButton_Click(object sender, EventArgs e)
        {
            DockUnDock();
        }

        internal void SendCAD()
        {
            if (this.vncTabView != null)
                this.vncTabView.SendCAD();
        }

        internal void FocusConsole()
        {
            if (this.vncTabView != null)
                vncTabView.focus_vnc();
        }

        internal void SwitchIfRequired()
        {
            vncTabView.SwitchIfRequired();
        }

        internal Image Snapshot()
        {
            return vncTabView.Snapshot();
        }

        public void refreshIsoList()
        {
            vncTabView.setupCD();
        }

        internal bool IsVNC
        {
            get { return vncTabView.IsVNC; }
        }
    }
}
