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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;

namespace XenAdmin.Controls.CustomDataGraph
{
    public partial class DataEventList : FlickerFreeListBox
    {
        public List<DataEvent> DataEvents = new List<DataEvent>();
        public DataPlot DataPlot;
        private static int ITEM_PADDING = 3;
        private DataPlotNav PlotNav;

        public DataEventList()
        {
            InitializeComponent();  
            // we are using 16pixel icons
            ItemHeight = 16 + ITEM_PADDING * 2;
            DrawItem += new DrawItemEventHandler(DataEventList_DrawItem);
            
        }

        public void SetPlotNav(DataPlotNav PlotNav)
        {
            this.PlotNav = PlotNav;
        }

        public void RenderEvents(Graphics graphics, DataTimeRange xrange, Rectangle bounds, int spaceheight)
        {
            if (DataEvents == null)
                return;

            foreach (DataEvent vent in DataEvents)
            {
                if (vent.X < xrange.Max || vent.X > xrange.Min)
                    continue;

                LongPoint loc = LongPoint.TranslateToScreen(vent.Point, xrange, DataRange.UnitRange, new LongRectangle(bounds));
                vent.DrawToBuffer(graphics, loc.Point, bounds, spaceheight);
            }
        }

        void DataEventList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if(e.Index == -1)
                return;
            DataEvent vent = Items[e.Index] as DataEvent;

            Image image = vent.TypeImage;

            e.Graphics.DrawImage(image, new Rectangle(e.Bounds.Left + ITEM_PADDING, e.Bounds.Top + ITEM_PADDING, e.Bounds.Height - ITEM_PADDING * 2, e.Bounds.Height - ITEM_PADDING * 2));

            int width = Drawing.MeasureText(e.Graphics, vent.ToString(), Palette.LabelFont).Width + e.Bounds.Height; ;
            if (width > HorizontalExtent)
                HorizontalExtent = width;

            Drawing.DrawText(e.Graphics, vent.ToString(), Palette.LabelFont, new Point(e.Bounds.Location.X + e.Bounds.Height - ITEM_PADDING, e.Bounds.Location.Y + ITEM_PADDING), e.ForeColor, e.BackColor);
        }

        public void AddEvent(DataEvent dataEvent)
        {
            DataEvents.Add(dataEvent);

            if (Items.Count == 0)
            {
                Items.Add(dataEvent);
                return;
            }

            int index = FindInsersionPlace(dataEvent.Message.timestamp.Ticks, 0, Items.Count - 1);

            Items.Insert(index, dataEvent);
        }

        private int FindInsersionPlace(long p, int i, int j)
        {
            if (p > (Items[i] as DataEvent).Message.timestamp.Ticks)
                return i;

            if (p <= (Items[j] as DataEvent).Message.timestamp.Ticks)
                return j+1;

            int mid = (j + i) / 2;
            if (mid == i)
                return j;
            if (p < (Items[mid] as DataEvent).Message.timestamp.Ticks)
                return FindInsersionPlace(p, mid, j);
            else
                return FindInsersionPlace(p, i, mid);
        }

        public DataEvent SelectedEvent = null;

        private void DataEventList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedEvent != null)
                SelectedEvent.Selected = false;
            SelectedEvent = (DataEvent)SelectedItem;
            if (SelectedEvent != null)
                SelectedEvent.Selected = true;
        }

        internal void OnEventsMouseClick(MouseActionArgs args)
        {
            foreach(DataEvent e in DataEvents)
            {
                LongPoint pt = LongPoint.TranslateToScreen(e.Point, args.XRange, args.YRange, new LongRectangle(args.Rectangle));
                if(pt.X - 8 <= args.Point.X && args.Point.X <= pt.X + 8)
                {
                    SelectedItem = e;
                    return;
                }
            }
            SelectedItem = null;
        }

        /// <summary>
        /// For some reason mouse 'moves' are registered even though the mouse is stationary over the control
        /// need to be able to ignore them if the user is trying to get the tooltip with the arrow keys
        /// </summary>
        private bool ignoreMouse = true;
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            ignoreMouse = false;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            EventListToolTip.Hide(this);
            toolTipIndex = -1;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            EventListToolTip.Hide(this);
            toolTipIndex = -1;
        }

        private int toolTipIndex = -1;
        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);
            if (ignoreMouse)
                return;
            try
            {
                int itemIndex = -1;
                if (ItemHeight != 0)
                {
                    itemIndex = args.Y / ItemHeight;
                    itemIndex += TopIndex;
                }
                if ((itemIndex >= 0) && (itemIndex < Items.Count))
                {
                    if (itemIndex == toolTipIndex)
                        return;
                    DataEvent e = Items[itemIndex] as DataEvent;
                    if (e == null)
                    {
                        EventListToolTip.Hide(this);
                        toolTipIndex = -1;
                    }
                    else
                    {
                        toolTipIndex = itemIndex;
                        EventListToolTip.Show(string.Format(XenAPI.Message.FriendlyBody(e.Message.Type.ToString()), Helpers.GetName(e.xo)),
                            this,
                            -10,
                            (toolTipIndex - TopIndex + 1) * ItemHeight + 2);
                    }
                }
                else
                {
                    EventListToolTip.Hide(this);
                    toolTipIndex = -1;
                }
            }
            catch
            {
                EventListToolTip.Hide(this);
                toolTipIndex = -1;
            }
        }

        protected override void OnKeyUp(KeyEventArgs args)
        {
            base.OnKeyUp(args);
            if (args.KeyCode == Keys.Enter)
            {
                DataEventList_MouseDoubleClick(null, null);
                return;
            }
            if (args.KeyCode != Keys.Up 
                    && args.KeyCode != Keys.Down 
                    && args.KeyCode != Keys.Left 
                    && args.KeyCode != Keys.Right)
                return;
            toolTipIndex = SelectedIndex;
            if (toolTipIndex < 0)
                return;
            ignoreMouse = true;
            DataEvent e = Items[toolTipIndex] as DataEvent;
            EventListToolTip.Show(string.Format(XenAPI.Message.FriendlyBody(e.Message.Type.ToString()), Helpers.GetName(e.xo)), 
                this, 
                -10,
                (toolTipIndex - TopIndex + 1) * ItemHeight + 2);
        }

        private void DataEventList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SelectedEvent == null)
                return;
            if (PlotNav != null)
                PlotNav.ZoomToPoint(SelectedEvent);
        }

        internal void Clear()
        {
            DataEvents.Clear();
            Items.Clear();
            HorizontalExtent = 0;
        }

        internal void RemoveEvent(DataEvent dataEvent)
        {
            DataEvents.Remove(dataEvent);
            Items.Remove(dataEvent);
        }
    }
}
