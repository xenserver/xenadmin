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
    public partial class DataPlot : DoubleBufferedPanel
    {
        private DataPlotNav _plotNav;
        private DataKey _plotKey;
        private DataEventList _eventList;
        private ArchiveMaintainer _archivemaintainer;
        private bool _isSelected = false;
        private bool _disposed;
        
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RefreshBuffer();
            }
        }
        public string DisplayName;

        public ArchiveMaintainer ArchiveMaintainer
        {
            get { return _archivemaintainer; }
            set
            {
                _archivemaintainer = value;

                XAxis = new DataAxisX(ArchiveMaintainer);
                YAxis = new DataAxisY(ArchiveMaintainer);
            }
        }

        public DataAxisX XAxis;
        public DataAxisY YAxis;

        public DataPoint SelectedPoint;

        public static DataRange DefaultYRange = new DataRange(1, 0, 1, Unit.None){ScaleMode = RangeScaleMode.Auto};

        public DataPlot()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, false);
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_plotNav != null)
                    _plotNav.RangeChanged -= PlotNav_RangeChanged;

                if (_plotKey != null)
                {
                    _plotKey.SetsChanged -= PlotKey_SetsChanged;
                    _plotKey.SelectedIndexChanged -= PlotKey_SelectedIndexChanged;
                }

                if (_eventList != null)
                    _eventList.SelectedIndexChanged -= EventList_SelectedIndexChanged;

                if (components != null)
                    components.Dispose();

                _disposed = true;
            }
            base.Dispose(disposing);
        }

        [Browsable(true),
        Description("Controls the ranges of axes of the DataPlot")]
        public DataPlotNav DataPlotNav
        {
            get
            {
                return _plotNav;
            }
            set
            {
                if (_plotNav != null)
                {
                    _plotNav.RangeChanged -= PlotNav_RangeChanged;
                }
                _plotNav = value;
                if (_plotNav != null)
                {
                    _plotNav.RangeChanged += PlotNav_RangeChanged;
                }
            }
        }

        [Browsable(true),
        Description("Controls the sets of the DataPlot")]
        public DataKey DataKey
        {
            get
            {
                return _plotKey;
            }
            set
            {
                if (_plotKey != null)
                {
                    _plotKey.SetsChanged -= PlotKey_SetsChanged;
                    _plotKey.SelectedIndexChanged -= PlotKey_SelectedIndexChanged;
                }
                _plotKey = value;
                if (_plotKey != null)
                {
                    _plotKey.DataPlot = this;
                    _plotKey.SetsChanged += PlotKey_SetsChanged;
                    _plotKey.SelectedIndexChanged += PlotKey_SelectedIndexChanged;
                }
            }
        }

        [Browsable(true),
        Description("Controls the sets of the DataPlot")]
        public DataEventList DataEventList
        {
            get
            {
                return _eventList;
            }
            set
            {
                if (_eventList != null)
                {
                    _eventList.SelectedIndexChanged -= EventList_SelectedIndexChanged;
                }
                _eventList = value;
                if (_eventList != null)
                {
                    _eventList.DataPlot = this;
                    _eventList.SelectedIndexChanged += EventList_SelectedIndexChanged;
                }
            }
        }

        private bool _showlabels = true;

        [Browsable(true)]
        public bool ShowLabels
        {
            get { return _showlabels; }
            set { _showlabels = value; }
        }

        void EventList_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshBuffer();
        }

        void PlotKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshBuffer();
        }

        private void PlotNav_RangeChanged()
        {
            DataKey.UpdateItems();
            RefreshBuffer();
        }

        private void PlotKey_SetsChanged()
        {
            RefreshBuffer();
        }

        private Rectangle GraphRectangle()
        {
            return GraphRectangle(ClientRectangle);
        }

        private Rectangle GraphRectangle(Rectangle orig)
        {
            return new Rectangle(orig.Left + Padding.Left, orig.Top + Padding.Top, orig.Width - (Padding.Horizontal + 1), orig.Height - (Padding.Vertical + 1));
        }

        private DataRange SelectedYRange
        {
            get
            {
                return DataKey.SelectedDataSet != null && DataKey.SelectedDataSet.CustomYRange != null ? DataKey.SelectedDataSet.CustomYRange : DataKey.Items.Count > 0 ? ((DataSetCollectionWrapper)DataKey.Items[0]).YRange : DefaultYRange;
            }
        }

        protected override void OnDrawToBuffer(PaintEventArgs paintEventArgs)
        {
            Program.AssertOnEventThread();

            Rectangle SlightlySmaller = GraphRectangle(paintEventArgs.ClipRectangle);
            // Fill BG
            paintEventArgs.Graphics.FillRectangle(Palette.PaperBrush, SlightlySmaller);

            if (ArchiveMaintainer == null || DataKey == null || DataPlotNav == null)
                return;

            if (IsSelected)
            {
                Rectangle rect = Rectangle.Inflate(paintEventArgs.ClipRectangle, 1, 1);
                paintEventArgs.Graphics.FillRectangle(Palette.GraphShadow, rect);
                paintEventArgs.Graphics.FillRectangle(Palette.PaperBrush, SlightlySmaller);
            }

            // Draw Rectangle around graph area
            paintEventArgs.Graphics.DrawRectangle(Palette.GridPen, SlightlySmaller);

            if (ArchiveMaintainer.LoadingInitialData)
            {
                paintEventArgs.Graphics.DrawString(Messages.GRAPH_LOADING, Palette.LabelFont, Palette.LabelBrush, SlightlySmaller.Left + 10, SlightlySmaller.Top + 10);
                return;
            }

            bool require_tools = true;
            foreach (DataSetCollectionWrapper set in DataKey.CurrentKeys)
            {
                if (set.Sets[ArchiveInterval.FiveSecond].TypeString != "memory")
                {
                    require_tools = false;
                    break;
                }
            }
            if (require_tools && DataKey.CurrentKeys.Count > 0)
            {
                Rectangle messageRect = Rectangle.Inflate(SlightlySmaller, -10, -10);
                paintEventArgs.Graphics.DrawString(Messages.GRAPH_NEEDS_TOOLS, Palette.LabelFont, Palette.LabelBrush,
                                                   messageRect);
                return;
            }

            // Refresh all sets
            foreach (DataSet set in DataPlotNav.CurrentArchive.Sets.ToArray())
            {
                if (!set.Draw || !DataKey.DataSourceUUIDsToShow.Contains(set.Uuid))
                    continue;

                List<DataPoint> todraw;
                ArchiveInterval current = DataPlotNav.GetCurrentLeftArchiveInterval();
                ArchiveInterval currentwidth = DataPlotNav.GetCurrentWidthArchiveInterval();
                if (current == currentwidth)
                {
                    todraw = new List<DataPoint>(set.Points);
                    if (current != ArchiveInterval.FiveSecond)
                    {
                        if (todraw.Count > 0 && todraw[0].X < DataPlotNav.GraphRight.Ticks)
                        {
                            todraw.InsertRange(0, DataPlotNav.GetFinerPoints(
                                set,
                                new DataTimeRange(todraw[0].X, DataPlotNav.GraphRight.Ticks, DataPlotNav.XRange.Resolution),
                                current));
                        }
                    }
                }
                else // currentwidth must be a higer resolution archive
                {
                    int setindex = ArchiveMaintainer.Archives[currentwidth].Sets.IndexOf(set);
                    todraw = new List<DataPoint>(ArchiveMaintainer.Archives[currentwidth].Sets[setindex].Points);
                    if (todraw.Count > 0)
                        set.MergePointCollection(set.BinaryChop(set.Points, new DataTimeRange(DataPlotNav.GraphLeft.Ticks, todraw[todraw.Count - 1].X, DataPlotNav.GraphResolution.Ticks)), todraw);
                }
                set.RefreshCustomY(DataPlotNav.XRange, todraw);
                DataRange ymax = DataRange.UnitRange;
                foreach (DataSetCollectionWrapper wrapper in DataKey.CurrentKeys)
                {
                    if (wrapper.Sets.ContainsKey(current))
                    {
                        DataSet dataSet = wrapper.Sets[current];
                        if (!dataSet.Hide && dataSet.CustomYRange != null &&
                            dataSet.CustomYRange.Units == set.CustomYRange.Units)
                        {
                            ymax.ScaleMode = dataSet.CustomYRange.ScaleMode;
                            if (dataSet.CustomYRange.ScaleMode != RangeScaleMode.Auto)
                            {
                                if (dataSet.CustomYRange.Max > ymax.Max)
                                    ymax = dataSet.CustomYRange;
                            }
                            else
                            {
                                double maxY = DataSet.GetMaxY(dataSet.BinaryChop(dataSet.CurrentlyDisplayed, DataPlotNav.XRange));
                                if (maxY < 1)
                                    maxY = 1;
                                if (maxY >= ymax.Max)
                                {
                                    ymax = dataSet.CustomYRange;
                                    ymax.Max = maxY;
                                }
                            }
                        }
                    }
                }
                if (set.CustomYRange.ScaleMode == RangeScaleMode.Auto)
                    ymax.RoundToNearestPowerOf10();
                foreach (DataSetCollectionWrapper wrapper in DataKey.CurrentKeys)
                {
                    foreach (DataSet ds in wrapper.Sets.Values)
                    {
                        if (ds.Hide || ds.CustomYRange == null || ds.CustomYRange.Units != set.CustomYRange.Units)
                            continue;

                        ds.CustomYRange.Max = ymax.Max;
                        ds.CustomYRange.Min = ymax.Min;
                        ds.CustomYRange.Resolution = ymax.Resolution;
                    }
                }
            }

            // Draw Axes
            XAxis.DrawToBuffer(new DrawAxisXArgs(paintEventArgs.Graphics, SlightlySmaller, DataPlotNav != null ? DataPlotNav.XRange : DataTimeRange.MaxRange, ShowLabels));
            YAxis.DrawToBuffer(new DrawAxisYArgs(paintEventArgs.Graphics, SlightlySmaller, SelectedYRange, ShowLabels));

            // Draw Sets
            DataSet[] sets_to_show = DataPlotNav.CurrentArchive.Sets.ToArray();
            Array.Sort(sets_to_show);
            Array.Reverse(sets_to_show);
            foreach (DataSet set in sets_to_show)
            {
                if (!set.Draw || DataKey == null || !DataKey.DataSourceUUIDsToShow.Contains(set.Uuid))
                    continue;

                lock (Palette.PaletteLock)
                {
                    using (var thickPen = Palette.CreatePen(set.Uuid, Palette.PEN_THICKNESS_THICK))
                    {
                        using (var normalPen = Palette.CreatePen(set.Uuid, Palette.PEN_THICKNESS_NORMAL))
                        {
                            using (var shadowBrush = Palette.CreateBrush(set.Uuid))
                            {
                                LineRenderer.Render(paintEventArgs.Graphics, SlightlySmaller, DataPlotNav.XRange, set.CustomYRange ?? SelectedYRange, set.Selected ? thickPen : normalPen, shadowBrush, set.CurrentlyDisplayed, true);
                            }
                        }
                    }
                }
            }

            if (DataEventList != null)
                DataEventList.RenderEvents(paintEventArgs.Graphics, DataPlotNav.XRange, new Rectangle(SlightlySmaller.Left, SlightlySmaller.Top + 2, SlightlySmaller.Width, SlightlySmaller.Height - 2), 16);

            SizeF labelsize = new SizeF(0,0);
            if (SelectedPoint != null && DataKey.SelectedDataSet != null)
            {
                string label = string.Format(string.Format("{0} - {1} = {2}", DataPlotNav.XRange.GetString(SelectedPoint.X + ArchiveMaintainer.ClientServerOffset.Ticks), DataKey.SelectedDataSet.Name,
                                                           SelectedPoint.Y >= 0 ? SelectedYRange.GetString(SelectedPoint.Y) : Messages.GRAPHS_NO_DATA));
                labelsize = paintEventArgs.Graphics.MeasureString(label,Palette.LabelFont);
                paintEventArgs.Graphics.DrawString(label, Palette.LabelFont, Palette.LabelBrush, SlightlySmaller.Right - labelsize.Width, SlightlySmaller.Top - (labelsize.Height + 1));
            }

            // Draw graph's name
            if (DisplayName != String.Empty)
            {
                Rectangle rect = new Rectangle(SlightlySmaller.Location, SlightlySmaller.Size);
                rect.Width -= Convert.ToInt32(labelsize.Width);
                string nameLabel = DisplayName.Ellipsise(rect, Palette.LabelFont);
                SizeF nameLabelSize = paintEventArgs.Graphics.MeasureString(nameLabel, Palette.LabelFont);
                paintEventArgs.Graphics.DrawString(nameLabel, Palette.LabelFont, Palette.LabelBrush, SlightlySmaller.Left, SlightlySmaller.Top - (nameLabelSize.Height + 1));
            }

            // Draw to screen
            Refresh();
        }

        Point ScrollStart;
        bool HaveMoved;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (ArchiveMaintainer == null || ArchiveMaintainer.LoadingInitialData)
                return;

            if (e.Button == MouseButtons.Left && GraphRectangle().Contains(e.Location))
            {
                //ScrollStart = DataRange.DeTranslatePoint(new LongPoint(e.Location), PlotNav.XRange, SelectedYRange, new LongRectangle(GraphRectangle));
                ScrollStart = e.Location;
            }
            else
            {
                ScrollStart = new Point(Int16.MinValue, Int16.MinValue);
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            if (ArchiveMaintainer == null || ArchiveMaintainer.LoadingInitialData || !GraphRectangle().Contains(PointToClient(MousePosition)))
            {
                Cursor = Cursors.Default;
                return;
            }

            Cursor = Cursors.SizeAll;
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            Cursor = Cursors.Default;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (GraphRectangle().Contains(e.Location))
                Cursor = Cursors.SizeAll;
            else
                Cursor = Cursors.Default;

            if (ArchiveMaintainer == null || ArchiveMaintainer.LoadingInitialData)
                return;

            if (e.Button == MouseButtons.None)
            {
                if (DataPlotNav == null || DataKey == null || ArchiveMaintainer == null)
                    return;
                if (SelectedPoint != null)
                    SelectedPoint.Show = false;
                if (DataKey.SelectedSet == null || !DataKey.SelectedSet.Sets.ContainsKey(DataPlotNav.GetCurrentLeftArchiveInterval()))
                    return;
                DataPoint found = DataKey.SelectedSet.Sets[DataPlotNav.GetCurrentLeftArchiveInterval()].OnMouseMove(new MouseActionArgs(e.Location, GraphRectangle(), DataPlotNav != null ? DataPlotNav.XRange : DataTimeRange.MaxRange, SelectedYRange));
                if (found == null)
                    return;
                found.Show = true;
                if (found != SelectedPoint)
                {
                    SelectedPoint = found;
                    RefreshBuffer();
                }
            }
            else if (e.Button == MouseButtons.Left && ScrollStart.X != Int16.MinValue && ScrollStart.Y != Int16.MinValue)
            {
                HaveMoved = true;
                long vdelta = DataTimeRange.DeTranslateDelta(e.Location.X - ScrollStart.X, DataPlotNav.GraphWidth.Ticks, GraphRectangle().Width);
                
                if (DataPlotNav.GraphOffset.Ticks + vdelta <= 0)
                {
                    if (DataPlotNav.GraphOffset == TimeSpan.Zero && DataPlotNav.ScrollViewOffset == TimeSpan.Zero)
                        return;
                    DataPlotNav.ScrollViewOffset = TimeSpan.Zero;
                    DataPlotNav.GraphOffset = TimeSpan.Zero;
                }
                else
                {
                    if (DataPlotNav.ScrollViewOffset.Ticks + DataPlotNav.ScrollViewWidth.Ticks < DataPlotNav.GraphOffset.Ticks + DataPlotNav.GraphWidth.Ticks + vdelta)
                        DataPlotNav.ScrollViewOffset = TimeSpan.FromTicks(DataPlotNav.GraphOffset.Ticks + DataPlotNav.GraphWidth.Ticks + vdelta - DataPlotNav.ScrollViewWidth.Ticks);
                    else if (DataPlotNav.ScrollViewOffset.Ticks > DataPlotNav.GraphOffset.Ticks + vdelta)
                        DataPlotNav.ScrollViewOffset = TimeSpan.FromTicks(DataPlotNav.GraphOffset.Ticks + vdelta);

                    DataPlotNav.GraphOffset += TimeSpan.FromTicks(vdelta);
                }

                ScrollStart = e.Location;
                DataPlotNav.RefreshXRange(false);
            }
        }

        /// <summary>
        /// Uses Sets (which is used for drawing) so must be on the event thread)
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            Program.AssertOnEventThread();

            base.OnMouseClick(e);
            Select();

            if (ArchiveMaintainer == null || ArchiveMaintainer.LoadingInitialData)
                return;

            if (HaveMoved || DataPlotNav == null || DataKey == null || ArchiveMaintainer == null)
            {
                HaveMoved = false;
                return;
            }
            if (DataEventList != null && e.Location.Y < Padding.Top + 18)
            {
                DataEventList.OnEventsMouseClick(new MouseActionArgs(e.Location, GraphRectangle(), DataPlotNav.XRange, SelectedYRange));
                DataKey.SelectedItem = null;
            }
            else if(GraphRectangle().Contains(e.Location))
            {
                foreach (DataSet set in DataPlotNav.CurrentArchive.Sets.ToArray())
                {
                    if (!set.Draw || DataKey == null || !DataKey.DataSourceUUIDsToShow.Contains(set.Uuid))
                        continue;
                    if (set.OnMouseClick(new MouseActionArgs(e.Location, GraphRectangle(), DataPlotNav.XRange, SelectedYRange)))
                    {
                        DataKey.SelectDataSet(set);
                        return;
                    }
                }
                DataKey.SelectedItem = null;
            }
            else
            {
                DataKey.SelectedItem = null;
            }
        }
    }
}
