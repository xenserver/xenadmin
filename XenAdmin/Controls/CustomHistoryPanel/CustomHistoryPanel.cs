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

using XenAdmin.Core;


namespace XenAdmin.Controls
{
    public partial class CustomHistoryPanel : Panel
    {
        public readonly ChangeableList<CustomHistoryRow> Rows = new ChangeableList<CustomHistoryRow>();

        internal const int col1 = 35;
        internal int col2 = -1;
        private int col4;

        internal int col3;

        public bool SuspendDraw = false;

        public int ScrollTop = 0;

        public CustomHistoryPanel()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (SuspendDraw)
                return;

            Graphics g = e.Graphics;

            if (col2 == -1)
            {
                col2 = Helpers.Max(
                    Measure(g, Messages.HISTORYROW_DETAILS),
                    Measure(g, Messages.HISTORYROW_ERROR),
                    Measure(g, Messages.HISTORYROW_PROGRESS),
                    Measure(g, Messages.HISTORYROW_TIME));
                col4 = 10 + (int)Drawing.MeasureText(g, HelpersGUI.DateTimeToString(new DateTime(9999, 12, 30, 20, 58, 58), Messages.DATEFORMAT_DMY_HMS, true), Font).Width;
            }

            col3 = Width - (6 + col1 + col2 + col4);

            g.FillRectangle(Application.RenderWithVisualStyles ? SystemBrushes.ControlLightLight : SystemBrushes.Control, Bounds);

            int top = 0;
            foreach (CustomHistoryRow row in Rows)
            {
                if (row.Visible)
                {
                    int rowHeight = DrawRow(row, g, top, ScrollTop, ScrollTop + Parent.Height);
                    top += rowHeight + CustomHistoryRow.Margin.Vertical;
                    if (top > Int16.MaxValue)
                        break;
                }
            }
            Height = top != 0 ? top : 1;
        }

        private int Measure(Graphics g, string s)
        {
            return (int)Drawing.MeasureText(g, s, Font).Width;
        }

        private int DrawRow(CustomHistoryRow row, Graphics g, int top, int visibleTop, int visibleBottom)
        {
            Rectangle rowbounds = new Rectangle(CustomHistoryRow.Margin.Left, top + CustomHistoryRow.Margin.Top,
                                                Width - CustomHistoryRow.Margin.Horizontal, Int32.MaxValue);
            return row.DrawSelf(g, rowbounds, visibleTop, visibleBottom);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            int top = 0;
            Point mousePoint = PointToClient(MousePosition);
            CustomHistoryRow row = getRowFromPoint(mousePoint, ref top);
            if(row != null)
                row.Click(new Point(mousePoint.X, mousePoint.Y - top), e.Button);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            int top = 0;
            Point mousePoint = PointToClient(MousePosition);
            CustomHistoryRow row = getRowFromPoint(mousePoint, ref top);
            if (row != null)
                row.MouseUp(new Point(mousePoint.X, mousePoint.Y - top), e.Button);

            foreach (CustomHistoryRow r in Rows)
            {
                r.ButtonPressed = false;
            }
            Invalidate();
            Refresh();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            foreach (CustomHistoryRow row in Rows)
            {
                row.ButtonPressed = false;
            }
            Invalidate();
            base.OnMouseLeave(e);
        }
        

        private CustomHistoryRow getRowFromPoint(Point point, ref int top)
        {
            foreach (CustomHistoryRow row in Rows)
            {
                if (row.Visible)
                {
                    int rowHeight = DrawRow(row, null, top, -1, -1);
                    if (top + rowHeight >= point.Y)
                        return row;
                    top += rowHeight + CustomHistoryRow.Margin.Vertical;
                }
            }
            return null;
        }

        public void AddItem(CustomHistoryRow row)
        {
            row.ParentPanel = this;
            Rows.Insert(0,row);
            Refresh();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Parent.Focus();
            if (e.Button == MouseButtons.Right)
            {
                int top = 0;
                ActionRow clickedRow = getRowFromPoint(PointToClient(MousePosition), ref top) as ActionRow;
                if (clickedRow != null)
                {
                    ContextMenuStrip menu = new ContextMenuStrip();
                    ToolStripMenuItem copyItem = new ToolStripMenuItem(Messages.COPY, Properties.Resources.copy_16);
                    copyItem.Click += new EventHandler(delegate(object sender, EventArgs eve)
                    {
                        Clipboard.SetText(string.Format(Messages.HISTORYPANEL_COPY_FORMAT, Core.PropertyManager.GetFriendlyName(string.Format("Label-Action.{0}", clickedRow.Type.ToString())), clickedRow.Title, clickedRow.Description, clickedRow.TimeOccurred));
                    });
                    menu.Items.Add(copyItem);
                    menu.Show(this, PointToClient(MousePosition));
                }
            }
            base.OnMouseClick(e);
        }
    }
}
