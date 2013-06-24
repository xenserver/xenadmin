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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using XenAdmin.Core;
using XenAdmin.TabPages;
using XenAdmin.Controls;

namespace XenAdmin
{
    public partial class HistoryWindow : Form
    {
        public static HistoryWindow TheHistoryWindow;
        private HistoryPage TheHistoryPanel = new HistoryPage();

        public static void OpenHistoryWindow()
        {
            if (TheHistoryWindow != null)
            {
                TheHistoryWindow.BringToFront();
                if (TheHistoryWindow.WindowState == FormWindowState.Minimized)
                {
                    TheHistoryWindow.WindowState = FormWindowState.Normal;
                }
            }
            else
            {
                new HistoryWindow().Show();
            }
        }

        public HistoryWindow()
        {
            InitializeComponent();
            Icon = Properties.Resources.AppIcon;
            TheHistoryPanel.ShowAll = true;
            TheHistoryPanel.BuildRowList();
            TheHistoryPanel.Dock = DockStyle.Fill;
            //TheHistoryPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            FormFontFixer.Fix(this);
            this.Controls.Add(TheHistoryPanel);
            TheHistoryWindow = this;

            // Stop flashing when we un-minimize the window
            TheHistoryWindow.Layout += new LayoutEventHandler(TheHistoryWindow_Layout);
        }

        private void TheHistoryWindow_Layout(object sender, LayoutEventArgs e)
        {
            if (TheHistoryWindow.WindowState != FormWindowState.Minimized)
            {
                Win32.StopFlashing(Handle);
            }
        }

        private void HistoryWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            TheHistoryWindow = null;
        }

        private void HistoryWindow_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Help.HelpManager.Launch("LogWindow");
            hlpevent.Handled = true;
        }

        private void HistoryWindow_Load(object sender, EventArgs e)
        {
            Owner = Program.MainWindow;
            CenterToParent();
            // We don't set Owner permanently -- we'll stay on top of it that way.  We only
            // set it so that CenterToParent works.
            Owner = null;
        }
    }
}