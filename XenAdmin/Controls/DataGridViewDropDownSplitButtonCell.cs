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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace XenAdmin.Controls
{
    public class DataGridViewDropDownSplitButtonCell : DataGridViewTextBoxCell
    {
        private bool active;
        private bool disposed;
        private ToolStripItem _defaultItem;

        /// <summary>
        /// Excluding right padding of cell
        /// </summary>
        private const int SPLITTER_FROM_RIGHT = 20;

        private static readonly Padding CELL_PADDING = new Padding(0, 0, 1, 1);
        /// <summary>
        /// Cell height without padding
        /// </summary>
        private const int CELL_HEIGHT = 20;

        /// <summary>
        /// Should be set on the row where the cell will be added
        /// </summary>
        public const int MIN_ROW_HEIGHT = 22;

        private static readonly Color BUTTON_BORDER_OUTER = Color.FromArgb(112, 112, 112);
        private static readonly Color BUTTON_BORDER_INNER = Color.FromArgb(252, 252, 252);
        private static readonly Color BUTTON_FACE_TOP = Color.FromArgb(242, 242, 242);
        private static readonly Color BUTTON_FACE_MIDDLE_TOP = Color.FromArgb(235, 235, 235);
        private static readonly Color BUTTON_FACE_MIDDLE_BOTTOM = Color.FromArgb(221, 221, 221);
        private static readonly Color BUTTON_FACE_BOTTOM = Color.FromArgb(207, 207, 207);

        public DataGridViewDropDownSplitButtonCell()
        {
            Style.Padding = CELL_PADDING;
            Style.Alignment = DataGridViewContentAlignment.TopLeft;
            ContextMenu = new ContextMenuStrip();
            ContextMenu.ItemClicked += _contextMenu_ItemClicked;
        }

        public DataGridViewDropDownSplitButtonCell(params ToolStripItem[] dropDownItems)
            : this()
        {
            RefreshItems(dropDownItems);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                ContextMenu.Dispose();
                disposed = true;
            }
            base.Dispose(disposing);
        }

        public readonly ContextMenuStrip ContextMenu;
       
        public ToolStripItem DefaultItem
        {
            get { return _defaultItem; }
            set
            {
                _defaultItem = value;
                if (_defaultItem != null)
                    Value = _defaultItem.Text;
            }
        }

        public void RefreshItems(params ToolStripItem[] dropDownItems)
        {
            ContextMenu.Items.Clear();
            ContextMenu.Items.AddRange(dropDownItems);

            foreach (ToolStripItem item in ContextMenu.Items)
            {
                if (item is ToolStripMenuItem)
                {
                    DefaultItem = item;
                    break;
                }
            }
        }

        private void _contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            DefaultItem = e.ClickedItem;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            cellStyle.Alignment = DataGridViewContentAlignment.TopLeft;

            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            
            if (active)
            {
                var mode = graphics.SmoothingMode;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                var rec = new Rectangle(cellBounds.Left + cellStyle.Padding.Left,
                                        cellBounds.Top + cellStyle.Padding.Top,
                                        cellBounds.Width - cellStyle.Padding.Left - cellStyle.Padding.Right,
                                        CELL_HEIGHT);

                var innerRec = new Rectangle(rec.Location, rec.Size);
                innerRec.Inflate(-1, -1);

                using (GraphicsPath path = new GraphicsPath())
                {
                    int diameter = 4;
                    var arc = new Rectangle(rec.Location, new Size(diameter, diameter));

                    //top left corner
                    path.AddArc(arc, 180, 90);

                    // top right corner
                    arc.X = rec.Right - diameter;
                    path.AddArc(arc, 270, 90);

                    // bottom right corner 
                    arc.Y = rec.Bottom - diameter;
                    path.AddArc(arc, 0, 90);

                    // bottom left corner
                    arc.X = rec.Left;
                    path.AddArc(arc, 90, 90);

                    path.CloseFigure();

                    using (var brush = new LinearGradientBrush(rec, BUTTON_FACE_TOP, BUTTON_FACE_BOTTOM, LinearGradientMode.Vertical))
                    {
                        ColorBlend cb = new ColorBlend();
                        cb.Positions = new[] { 0, 1 / 2f, 1 / 2f, 1 };
                        cb.Colors = new[] { BUTTON_FACE_TOP, BUTTON_FACE_MIDDLE_TOP, BUTTON_FACE_MIDDLE_BOTTOM, BUTTON_FACE_BOTTOM };
                        brush.InterpolationColors = cb;
                        graphics.FillPath(brush, path);
                    }

                    using (Pen pen = new Pen(BUTTON_BORDER_OUTER, 1))
                        graphics.DrawPath(pen, path);
                }

                using (GraphicsPath path = new GraphicsPath())
                {
                    int diameter = 4;
                    var arc = new Rectangle(innerRec.Location, new Size(diameter, diameter));

                    //top left corner
                    path.AddArc(arc, 180, 90);

                    // top right corner
                    arc.X = innerRec.Right - diameter;
                    path.AddArc(arc, 270, 90);

                    // bottom right corner 
                    arc.Y = innerRec.Bottom - diameter;
                    path.AddArc(arc, 0, 90);

                    // bottom left corner
                    arc.X = innerRec.Left;
                    path.AddArc(arc, 90, 90);

                    path.CloseFigure();

                    using (Pen pen = new Pen(BUTTON_BORDER_INNER, 1))
                        graphics.DrawPath(pen, path);
                }

                using (var font = new Font(DataGridView.DefaultCellStyle.Font, FontStyle.Regular))
                    graphics.DrawString(value as string, font, SystemBrushes.ControlText, cellBounds);

                using (Pen pen = new Pen(BUTTON_BORDER_OUTER, 1))
                    graphics.DrawLine(pen,
                                      cellBounds.Right - cellStyle.Padding.Right - SPLITTER_FROM_RIGHT,
                                      cellBounds.Top + cellStyle.Padding.Top + 2,
                                      cellBounds.Right - cellStyle.Padding.Right - SPLITTER_FROM_RIGHT,
                                      cellBounds.Top + cellStyle.Padding.Top + CELL_HEIGHT - 2);

                //reset graphics mode
                graphics.SmoothingMode = mode;
            }

            var img = Properties.Resources.expanded_triangle;
            graphics.DrawImage(img,
                cellBounds.Right - cellStyle.Padding.Right - img.Width - (SPLITTER_FROM_RIGHT - img.Width) / 2,
                cellBounds.Top + (CELL_HEIGHT - img.Height) / 2);
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            active = true;
            DataGridView.InvalidateCell(this);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            active = false;
            DataGridView.InvalidateCell(this);
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            var cell = DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            var rec = DataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

            var buttonArea = new Rectangle(cell.ContentBounds.Left,
                cell.ContentBounds.Top,
                cell.Size.Width - SPLITTER_FROM_RIGHT - cell.Style.Padding.Left - cell.Style.Padding.Right,
                CELL_HEIGHT);

            var arrowArea = new Rectangle(cell.ContentBounds.Left + cell.Size.Width - SPLITTER_FROM_RIGHT - cell.Style.Padding.Left - cell.Style.Padding.Right,
                cell.ContentBounds.Top,
                SPLITTER_FROM_RIGHT,
                CELL_HEIGHT);

            //for half displayed cell we want the dropdown to appear at the end
            //of the visible area rather than the hidden of the cell
            var showPoint = new Point(rec.Left, rec.Top + Math.Min(rec.Height, CELL_HEIGHT));

            if (buttonArea.Contains(e.Location))
            {
                if (DefaultItem != null)
                    DefaultItem.PerformClick();
                else if (ContextMenu.Items.Count > 0)
                    ContextMenu.Show(DataGridView, showPoint, ToolStripDropDownDirection.Default);
            }
            else if (arrowArea.Contains(e.Location) && ContextMenu.Items.Count > 0)
                ContextMenu.Show(DataGridView, showPoint, ToolStripDropDownDirection.Default);
        }

        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            var orig = base.GetPreferredSize(graphics, cellStyle, rowIndex, constraintSize);
            return new Size(orig.Width + SPLITTER_FROM_RIGHT, orig.Height);
        }
    }
}
