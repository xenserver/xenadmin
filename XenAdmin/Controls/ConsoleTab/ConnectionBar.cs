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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using XenAdmin.ConsoleView;
using XenAdmin.Core;

namespace XenAdmin.Controls.ConsoleTab
{
    public partial class ConnectionBar : UserControl
    {
        private const int HIDDEN_HEIGHT = 1;
        private const double ACCEL = 2.5;

        private readonly Timer timer = new Timer();

        private string _connectionName;

        public ConnectionBar()
        {
            InitializeComponent();
            toolStrip1.Renderer = new CustomToolStripSystemRenderer();
            timer.Tick += timer_Tick;
            AttachMouseOnChildren(this);
        }

        public string ConnectionName
        {
            set
            {
                _connectionName = value;
                DisplayConnectionName();
            }
        }

        private void DisplayConnectionName()
        {
            if (string.IsNullOrEmpty(_connectionName))
                return;

            using (Graphics g = toolStrip1.CreateGraphics())
            {
                int width = Drawing.MeasureText(g, _connectionName, labelConnection.Font, TextFormatFlags.NoPadding).Width;
                if (width > labelConnection.ContentRectangle.Width)
                {
                    labelConnection.ToolTipText = _connectionName;
                    labelConnection.Text = _connectionName.Ellipsise(labelConnection.ContentRectangle, labelConnection.Font);
                }
                else
                {
                    labelConnection.ToolTipText = string.Empty;
                    labelConnection.Text = _connectionName;
                }
            }
        }

        private void AttachMouseOnChildren(Control control)
        {
            foreach (Control child in control.Controls)
            {
                child.MouseLeave += child_MouseLeave;
                child.MouseEnter += child_MouseEnter;
                AttachMouseOnChildren(child);
            }
        }

        private void child_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        private void child_MouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            ShowAnimated();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Point clientMouse = PointToClient(MousePosition);
            var r = new Rectangle(0, 0, Width, Height - HIDDEN_HEIGHT);

            if (!r.Contains(clientMouse))
                HideAnimated();
        }

        private void buttonExitFullScreen_Click(object sender, EventArgs e)
        {
            if (ParentForm != null)
                ParentForm.Close();
        }

        #region Animated Show/Hide

        private Animating state = Animating.Open;

        private int Y
        {
            get { return Location.Y; }
            set { Location = new Point(Location.X, value); }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (state == Animating.Showing && Y < 0)
            {
                Y -= (int)(Y / ACCEL) - 1;
                if (Y == 0)
                {
                    timer.Stop();
                    state = Animating.Open;
                }
                Debug.WriteLine("Opening " + Y);
            }
            if (state == Animating.Hiding && Y > -(Height - HIDDEN_HEIGHT))
            {
                Y -= (int)((Y + Height - HIDDEN_HEIGHT) / ACCEL) + 1;
                if (Y == -(Height - HIDDEN_HEIGHT))
                {
                    timer.Stop();
                    state = Animating.Closed;
                }
                Debug.WriteLine("Closing " + Y);
            }
            timer.Interval = 100;
        }


        private void ShowAnimated()
        {
            if (state == Animating.Open)
                return;

            if (Properties.Settings.Default.PinConnectionBar)
            {
                ShowPinned();
                return;
            }
            state = Animating.Showing;
            timer.Stop();
            timer.Start();
        }

        public void HideAnimated()
        {
            if (state == Animating.Closed)
                return;
            if (Properties.Settings.Default.PinConnectionBar)
                return;
            state = Animating.Hiding;
            timer.Stop();
            timer.Start();
        }

        private enum Animating
        {
            Showing,
            Hiding,
            Open,
            Closed
        } 

        #endregion

        public void ShowPinned()
        {
            if (!buttonPin.Checked)
                buttonPin.Checked = true;

            if (state == Animating.Open)
                return;

            Y = 0;
            state = Animating.Open;
        }

        private void toolStrip1_Resize(object sender, EventArgs e)
        {
            labelConnection.Width = toolStrip1.Width - buttonPin.Width - buttonExitFullScreen.Width - 12;
            DisplayConnectionName();
        }

        private void buttonPin_CheckedChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.PinConnectionBar != buttonPin.Checked)
            {
                Properties.Settings.Default.PinConnectionBar = buttonPin.Checked;
                Settings.TrySaveSettings();
            }
            if (buttonPin.Checked)
                ShowPinned();
        }
    }

    public class CustomToolStripSystemRenderer : ToolStripSystemRenderer
    {
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (var brush = new LinearGradientBrush(e.AffectedBounds, Color.FromArgb(64, 64, 64), Color.Gray, 90))
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds); 
            }
            
            using (var borderPen = new Pen(Color.Black, 1))
            {
                e.Graphics.DrawRectangle(borderPen,
                    new Rectangle(e.AffectedBounds.Left, e.AffectedBounds.Top, e.AffectedBounds.Width - 1, e.AffectedBounds.Height - 2));
            }
        }
    }
}
