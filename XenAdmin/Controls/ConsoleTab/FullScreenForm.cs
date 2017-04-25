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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.ConsoleView;


namespace XenAdmin.Controls.ConsoleTab
{
    public partial class FullScreenForm : Form
    {
        private Control _referenceControl;

        public FullScreenForm()
        {
            InitializeComponent();
        }

        public FullScreenForm(Control referenceControl)
        {
            InitializeComponent();
            _referenceControl = referenceControl;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Screen screen = Screen.FromControl(_referenceControl);
            StartPosition = FormStartPosition.Manual;
            Location = screen.WorkingArea.Location;
            Size = screen.Bounds.Size;
            WindowState = FormWindowState.Maximized;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (Properties.Settings.Default.PinConnectionBar)
                connectionBar1.ShowPinned();
            else
                connectionBar1.HideAnimated();
        }

        public void AttachVncScreen(XSVNCScreen screen)
        {
            screen.Parent = contentPanel;
            SetConnectionName(screen.ConnectionName);
            screen.ConnectionNameChanged += SetConnectionName;
        }

        public void DetachVncScreen(XSVNCScreen screen)
        {
            screen.ConnectionNameChanged -= SetConnectionName;
        }

        public Size GetContentSize()
        {
            return contentPanel.Size;
        }

        private void SetConnectionName(string connectionName)
        {
            connectionBar1.ConnectionName = connectionName;
        }
    }
}
