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
using System.Data;
using System.Text;
using System.Windows.Forms;

using XenAdmin.Actions;

namespace XenAdmin.Controls
{
    public partial class CustomHistoryContainer : Panel
    {
        /// <summary>
        /// List to keep track of refresh Timers to prevent them being GC'd.
        /// May only be accessed under the protection of the refreshTimersLock.
        /// </summary>
        private static readonly List<Timer> refreshTimers = new List<Timer>();
        private static readonly object refreshTimersLock = new object();

        public CustomHistoryContainer()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Red;
            CustomHistoryPanel.Rows.CollectionChanged += Rows_CollectionChanged;
        }

        /// <summary>
        /// When a new row is added, we start a new timer that ticks each second and causes the panel to repaint. This ensures
        /// the 'time' field ticks smoothly. When the timer detects the Action has completed, it disposes itself.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rows_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (Program.MainWindow != null)
                Program.BeginInvoke(Program.MainWindow, () => RowsChanged(e));
            else
                RowsChanged(e);
        }

        private void RowsChanged(CollectionChangeEventArgs e)
        {
            ActionRow row = (ActionRow) e.Element;
            if (e.Action == CollectionChangeAction.Add &&
                !row.Action.IsCompleted)
            {
                Timer refreshTimer = new Timer();
                refreshTimer.Tick += refreshTick;
                refreshTimer.Interval = 1000;
                refreshTimer.Tag = row;
                // Add to list to prevent GCing
                lock (refreshTimersLock)
                {
                    refreshTimers.Add(refreshTimer);
                }
                Form f = FindForm();
                Program.Invoke(f ?? Program.MainWindow,refreshTimer.Start);
            }
        }

        void refreshTick(object obj, EventArgs args)
        {
            Program.AssertOnEventThread();

            Timer timer = (Timer)obj;
            ActionRow row = (ActionRow)timer.Tag;

            bool redraw = !Disposing && !IsDisposed && !Program.Exiting;
            bool kill = row.Action == null || row.Action.IsCompleted || !redraw;

            if (redraw)
            {
                // Redraw panel
                CustomHistoryPanel.Invalidate();
                CustomHistoryPanel.Refresh();
            }

            if (kill)
            {
                // Kill timer
                lock (refreshTimersLock)
                {
                    refreshTimers.Remove(timer);
                }
                timer.Stop();
                timer.Dispose();
            }
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            CustomHistoryPanel.ScrollTop = se.NewValue;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            CustomHistoryPanel.ScrollTop = -this.AutoScrollPosition.Y;
            CustomHistoryPanel.Refresh();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            CustomHistoryPanel.ScrollTop = -this.AutoScrollPosition.Y;
            CustomHistoryPanel.Refresh();
        }
    }
}
