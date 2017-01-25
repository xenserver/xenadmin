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
using System.Drawing.Drawing2D;


namespace XenAdmin.Controls.CustomDataGraph
{
    public partial class DataPlotNav : DoubleBufferedPanel
    {
        public DataTimeRange XRange = new DataTimeRange(DateTime.Now - (TimeSpan.FromSeconds(5) + TimeSpan.FromMinutes(15)), DateTime.Now - TimeSpan.FromSeconds(5), -TimeSpan.FromMinutes(3));
        public event Action RangeChanged;
        public Timer Tick = new Timer();

        private DataAxisNav Axis;

        private ArchiveMaintainer _archivemaintainer;

        public ArchiveMaintainer ArchiveMaintainer
        {
            get { return _archivemaintainer; }
            set
            {
                _archivemaintainer = value;
                Axis = new DataAxisNav(_archivemaintainer);
            }
        }

        private DataEventList _dataEventList;

        public DataEventList DataEventList
        {
            get { return _dataEventList; }
            set { _dataEventList = value; }
        }

        private TimeSpan _graphWidth = TimeSpan.FromMinutes(10);

        [Browsable(true)]
        public TimeSpan GraphWidth
        {
            get { return _graphWidth; }
            set { _graphWidth = value; }
        }

        private TimeSpan _gridSpacing = TimeSpan.FromMinutes(5);

        [Browsable(true)]
        public TimeSpan GridSpacing
        {
            get { return _gridSpacing; }
            set { _gridSpacing = value; }
        }

        private TimeSpan _graphOffset = TimeSpan.Zero;

        [Browsable(true)]
        public TimeSpan GraphOffset
        {
            get { return _graphOffset; }
            set { _graphOffset = value; }
        }

        private List<string> _displayedUuids = new List<string>();

        public List<string> DisplayedUuids
        {
            get { return _displayedUuids; }
            set { _displayedUuids = value; }
        }

        public DataPlotNav()
        {
            InitializeComponent();

            Scroller.Interval = 50;
            Scroller.Tick += new EventHandler(Scroller_Tick);

            ResizeAnimation.Interval = 50;
            ResizeAnimation.Tick += new EventHandler(ResizeAnimation_Tick);

            Tick.Interval = 1000;
            Tick.Tick += new EventHandler(Tick_Tick);
            Tick.Start();
        }

        void ResizeAnimation_Tick(object sender, EventArgs e)
        {
            if (AnimateCurrentWidth == ScrollViewWidth || Math.Abs(ScrollViewWidth.Ticks - AnimateCurrentWidth.Ticks) <= ScrollViewWidth.Ticks * 0.01)
            {
                Animating = false;
                ResizeAnimation.Stop();
            }
            else
            {
                TimeSpan deltaoffset = TimeSpan.FromTicks((ScrollViewOffset - AnimateCurrentOffset).Ticks / 2);
                if (AnimateCurrentOffset + deltaoffset > TimeSpan.Zero)
                    AnimateCurrentOffset += deltaoffset;
                else
                    AnimateCurrentOffset = TimeSpan.Zero;
                AnimateCurrentWidth += TimeSpan.FromTicks((ScrollViewWidth - AnimateCurrentWidth).Ticks / 2);
            }

            RefreshXRange(false);
        }

        private void Scroller_Tick(object sender, EventArgs e)
        {
            if (TimerScroll == ScrollMode.None || (TimerScroll == ScrollMode.Right && ScrollViewOffset == TimeSpan.Zero))
                return;

            TimeSpan inc = TimeSpan.FromTicks(TimerScroll == ScrollMode.Left ? (long)(ScrollStrength.Ticks * 1) : (long)(ScrollStrength.Ticks * -1));

            if (ScrollViewOffset.Ticks + inc.Ticks < 0)
            {
                ScrollViewOffset = TimeSpan.Zero;
            }
            else
            {
                ScrollViewOffset += inc;
                GraphOffset += inc;
            }

            RefreshXRange(false);
        }

        private bool skip_tick;

        public void RefreshXRange(bool from_tick)
        {
            if (skip_tick && from_tick)
            {
                skip_tick = false;
                return;
            }

            if (!from_tick)
                skip_tick = true;

            XRange = new DataTimeRange(GraphLeft, GraphRight, GraphResolution);

            if (RangeChanged != null)
                RangeChanged();

            RefreshBuffer();
        }

        public void Tick_Tick(object sender, EventArgs e)
        {
            if (!Visible)
                return;
            RefreshXRange(true);
        }

        public DataArchive CurrentArchive
        {
            get
            {
                if (ArchiveMaintainer != null)
                    return ArchiveMaintainer.Archives[GetCurrentLeftArchiveInterval()];
                return new DataArchive(1);
            }
        }

        public DataArchive OneFinerThanCurrentArchive
        {
            get
            {
                if (ArchiveMaintainer != null)
                {
                    switch (GetCurrentLeftArchiveInterval())
                    {
                        case ArchiveInterval.FiveSecond:
                            return ArchiveMaintainer.Archives[ArchiveInterval.FiveSecond];
                        case ArchiveInterval.OneMinute:
                            return ArchiveMaintainer.Archives[ArchiveInterval.FiveSecond];
                        case ArchiveInterval.OneHour:
                            return ArchiveMaintainer.Archives[ArchiveInterval.OneMinute];
                        case ArchiveInterval.OneDay:
                            return ArchiveMaintainer.Archives[ArchiveInterval.OneHour];
                    }
                }
                return new DataArchive(1);
            }
        }

        public TimeSpan GetArchiveSpan()
        {
            ArchiveInterval interval = GetCurrentLeftArchiveInterval();

            if (interval == ArchiveInterval.FiveSecond)
                return TimeSpan.FromTicks(ArchiveMaintainer.TicksInTenMinutes);
            if (interval == ArchiveInterval.OneMinute)
                return TimeSpan.FromTicks(ArchiveMaintainer.TicksInTwoHours);
            if (interval == ArchiveInterval.OneHour)
                return TimeSpan.FromTicks(ArchiveMaintainer.TicksInSevenDays);
            return TimeSpan.FromTicks(ArchiveMaintainer.TicksInOneYear);
        }

        public ArchiveInterval GetCurrentLeftArchiveInterval()
        {
            //TimeSpan t = ArchiveMaintainer != null ? ArchiveMaintainer.ClientServerOffset : TimeSpan.Zero;
            if (GraphOffset.Ticks + GraphWidth.Ticks < ArchiveMaintainer.TicksInTenMinutes)
                return ArchiveInterval.FiveSecond;
            if (GraphOffset.Ticks + GraphWidth.Ticks < ArchiveMaintainer.TicksInTwoHours)
                return ArchiveInterval.OneMinute;
            if (GraphOffset.Ticks + GraphWidth.Ticks < ArchiveMaintainer.TicksInSevenDays)
                return ArchiveInterval.OneHour;
            return ArchiveInterval.OneDay;
        }

        public ArchiveInterval GetCurrentWidthArchiveInterval()
        {
            //TimeSpan t = ArchiveMaintainer != null ? ArchiveMaintainer.ClientServerOffset : TimeSpan.Zero;
            if (GraphWidth.Ticks < ArchiveMaintainer.TicksInTenMinutes)
                return ArchiveInterval.FiveSecond;
            if (GraphWidth.Ticks < ArchiveMaintainer.TicksInTwoHours)
                return ArchiveInterval.OneMinute;
            if (GraphWidth.Ticks < ArchiveMaintainer.TicksInSevenDays)
                return ArchiveInterval.OneHour;
            return ArchiveInterval.OneDay;
        }

        private TimeSpan BodgeSpacing(TimeSpan span)
        {
            if (span.TotalSeconds < 5)
                return TimeSpan.FromSeconds(1);
            if (span.TotalSeconds < 10)
                return TimeSpan.FromSeconds(2);
            if (span.TotalSeconds < 30)
                return TimeSpan.FromSeconds(5);
            if (span.TotalSeconds < 60)
                return TimeSpan.FromSeconds(10);
            if (span.TotalSeconds < 120)
                return TimeSpan.FromSeconds(15);
            if (span.TotalSeconds < 180)
                return TimeSpan.FromSeconds(30);
            if (span.TotalMinutes < 5)
                return TimeSpan.FromMinutes(1);
            if (span.TotalMinutes < 10)
                return TimeSpan.FromMinutes(2);
            if (span.TotalMinutes < 30)
                return TimeSpan.FromMinutes(5);
            if (span.TotalSeconds < 60)
                return TimeSpan.FromMinutes(10);
            if (span.TotalSeconds < 120)
                return TimeSpan.FromMinutes(15);
            if (span.TotalMinutes < 180)
                return TimeSpan.FromMinutes(30);
            if (span.TotalHours < 4)
                return TimeSpan.FromHours(1);
            if (span.TotalHours < 12)
                return TimeSpan.FromHours(2);
            if (span.TotalHours < 48)
                return TimeSpan.FromHours(6);
            if (span.TotalDays < 7)
                return TimeSpan.FromDays(1);
            if (span.TotalDays < 14)
                return TimeSpan.FromDays(2);
            if (span.TotalDays < 30)
                return TimeSpan.FromDays(5);
            if (span.TotalDays < 60)
                return TimeSpan.FromDays(10);
            if (span.TotalDays < 180)
                return TimeSpan.FromDays(30);
            return TimeSpan.FromDays(90);
        }

        public DateTime GraphLeft
        {
            get
            {
                return (ArchiveMaintainer != null ? ArchiveMaintainer.GraphNow : DateTime.Now) - (GraphWidth + GraphOffset);
            }
        }

        public DateTime GraphRight
        {
            get
            {
                return (ArchiveMaintainer != null ? ArchiveMaintainer.GraphNow : DateTime.Now) - GraphOffset;
            }
        }

        public TimeSpan GraphResolution
        {
            get
            {
                return -GridSpacing;
            }
        }

        public void ZoomToPoint(DataPoint dp)
        {
            if (dp.X + (GraphWidth.Ticks / 2) < (ArchiveMaintainer != null ? ArchiveMaintainer.GraphNow : DateTime.Now).Ticks)
                GraphOffset = new TimeSpan((ArchiveMaintainer != null ? ArchiveMaintainer.GraphNow : DateTime.Now).Ticks - (dp.X + (GraphWidth.Ticks / 2)));
            else
                GraphOffset = TimeSpan.Zero;

            if (GraphOffset.Ticks - ScrollViewOffset.Ticks < 0)
            {
                ScrollViewOffset = GraphOffset;
            }
            else if (ScrollViewOffset.Ticks + ScrollViewWidth.Ticks - (GraphOffset.Ticks + GraphWidth.Ticks) < 0)
            {
                ScrollViewOffset = GraphOffset + GraphWidth - ScrollViewWidth;
            }

            RefreshXRange(false);
        }

        public void ZoomToFit(DataSet set)
        {
            if (set.Points.Count > 0)
            {
                if (set.Points[0].X > (ArchiveMaintainer != null ? ArchiveMaintainer.GraphNow : DateTime.Now).Ticks)
                {
                    GraphOffset = TimeSpan.Zero;
                    GraphWidth = new TimeSpan((ArchiveMaintainer != null ? ArchiveMaintainer.GraphNow : DateTime.Now).Ticks - set.Points[set.Points.Count - 1].X);
                }
                else
                {
                    GraphOffset = new TimeSpan((ArchiveMaintainer != null ? ArchiveMaintainer.GraphNow : DateTime.Now).Ticks - set.Points[0].X);
                    GraphWidth = new TimeSpan(set.Points[0].X - set.Points[set.Points.Count - 1].X);
                }
                RefreshXRange(false);
            }
        }

        public void ZoomToRange(TimeSpan offset, TimeSpan width)
        {
            GraphOffset = offset;
            GraphWidth = width;

            AutoScaleGraph();

            GridSpacing = BodgeSpacing(GraphWidth);
            RefreshXRange(false);
        }

        public DataTimeRange CurrentArchiveRange
        {
            get
            {
                return new DataTimeRange(ScrollViewRight - ScrollViewWidth, ScrollViewRight, BodgeSpacing(ScrollViewWidth));
            }
        }

        private DataArchive ScrollWideArchive
        {
            get
            {
                return ArchiveMaintainer.Archives[ScrollViewLeftArchiveInterval];
            }
        }

        private ArchiveInterval ScrollViewLeftArchiveInterval
        {
            get
            {
                TimeSpan width = Animating ? AnimateCurrentWidth : ScrollViewWidth;
                TimeSpan offset = Animating ? AnimateCurrentOffset : ScrollViewOffset;
                if (offset.Ticks + width.Ticks <= ArchiveMaintainer.TicksInTenMinutes)
                    return ArchiveInterval.FiveSecond;
                else if (offset.Ticks + width.Ticks <= ArchiveMaintainer.TicksInTwoHours)
                    return ArchiveInterval.OneMinute;
                else if (offset.Ticks + width.Ticks <= ArchiveMaintainer.TicksInSevenDays)
                    return ArchiveInterval.OneHour;
                else
                    return ArchiveInterval.OneDay;
            }
        }

        private ArchiveInterval ScrollViewWidthArchiveInterval
        {
            get
            {
                TimeSpan width = Animating ? AnimateCurrentWidth : ScrollViewWidth;
                if (width.Ticks <= ArchiveMaintainer.TicksInTenMinutes)
                    return ArchiveInterval.FiveSecond;
                else if (width.Ticks <= ArchiveMaintainer.TicksInTwoHours)
                    return ArchiveInterval.OneMinute;
                else if (width.Ticks <= ArchiveMaintainer.TicksInSevenDays)
                    return ArchiveInterval.OneHour;
                else
                    return ArchiveInterval.OneDay;
            }
        }

        private DateTime ScrollViewRight
        {
            get
            {
                TimeSpan offset = Animating ? AnimateCurrentOffset : ScrollViewOffset;
                return ArchiveMaintainer.GraphNow - offset;
            }
        }

        private DateTime ScrollViewLeft
        {
            get
            {
                TimeSpan offset = Animating ? AnimateCurrentOffset : ScrollViewOffset;
                TimeSpan width = Animating ? AnimateCurrentWidth : ScrollViewWidth;
                return ArchiveMaintainer.GraphNow - (offset + width);
            }
        }

        private DataTimeRange AnimateTimeRange
        {
            get
            {
                return new DataTimeRange(ScrollViewRight - AnimateCurrentWidth, ScrollViewRight, BodgeSpacing(ScrollViewWidth));
            }
        }

        private Rectangle ScrollViewRectangle
        {
            get
            {
                return new Rectangle(ClientRectangle.Left + 5, ClientRectangle.Top, ClientRectangle.Width - 10, ClientRectangle.Height);
            }
        }

        protected override void OnDrawToBuffer(PaintEventArgs paintEventArgs)
        {
            Program.AssertOnEventThread();
            Rectangle rect = new Rectangle(ScrollViewRectangle.Left, ScrollViewRectangle.Top, ScrollViewRectangle.Width - 1, ScrollViewRectangle.Height - 1);
            paintEventArgs.Graphics.FillRectangle(Palette.PaperBrush, ScrollViewRectangle);
            paintEventArgs.Graphics.DrawRectangle(SystemPens.ActiveBorder, rect);


            if (ArchiveMaintainer == null || Axis == null)
                return;

            if (ArchiveMaintainer.LoadingInitialData)
            {
                paintEventArgs.Graphics.DrawString(Messages.GRAPH_LOADING, Palette.LabelFont, Palette.LabelBrush, ScrollViewRectangle.Left + 10, ScrollViewRectangle.Top + 10);
                return;
            }

            DataTimeRange everything = Animating ? AnimateTimeRange : CurrentArchiveRange;

            RectangleF clip = paintEventArgs.Graphics.ClipBounds;

            paintEventArgs.Graphics.SetClip(rect);

            foreach (DataSet set in ScrollWideArchive.Sets.ToArray())
            {
                if (!set.Draw || !DisplayedUuids.Contains(set.Uuid))
                    continue;

                List<DataPoint> todraw;
                ArchiveInterval current = ScrollViewLeftArchiveInterval;
                ArchiveInterval currentwidth = ScrollViewWidthArchiveInterval;
                if (current == currentwidth)
                {
                    todraw = new List<DataPoint>(set.Points);
                    if (current != ArchiveInterval.FiveSecond)
                    {
                        if (todraw.Count > 0 && todraw[0].X < ScrollViewRight.Ticks)
                        {
                            todraw.InsertRange(0, GetFinerPoints(
                                set,
                                new DataTimeRange(todraw[0].X, ScrollViewRight.Ticks, XRange.Resolution),
                                current));
                        }
                    }
                }
                else // currentwidth must be a higer resolution archive
                {
                    int setindex = ArchiveMaintainer.Archives[currentwidth].Sets.IndexOf(set);
                    todraw = new List<DataPoint>(ArchiveMaintainer.Archives[currentwidth].Sets[setindex].Points);

                    if (todraw.Count > 0)
                        set.MergePointCollection(set.BinaryChop(set.Points, new DataTimeRange(ScrollViewLeft.Ticks, todraw[todraw.Count - 1].X, GraphResolution.Ticks)), todraw);
                }

                set.RefreshCustomY(everything, todraw);
                if (set.CustomYRange.ScaleMode == RangeScaleMode.Auto)
                {
                    set.CustomYRange.Max = DataSet.GetMaxY(set.CurrentlyDisplayed);
                    set.CustomYRange.RoundToNearestPowerOf10();
                }
            }

            Axis.DrawToBuffer(new DrawAxisNavArgs(paintEventArgs.Graphics, ScrollViewRectangle, new DataTimeRange(everything.Max, everything.Min, -BodgeSpacing(new TimeSpan(-everything.Delta)).Ticks), true));

            foreach (DataSet set in ScrollWideArchive.Sets.ToArray())
            {
                if (!set.Draw || !DisplayedUuids.Contains(set.Uuid))
                    continue;

                lock (Palette.PaletteLock)
                {
                    using (var normalPen = Palette.CreatePen(set.Uuid, Palette.PEN_THICKNESS_NORMAL))
                    {
                        LineRenderer.Render(paintEventArgs.Graphics, ScrollViewRectangle, everything, set.CustomYRange, normalPen, null, set.CurrentlyDisplayed, false);
                    }
                }
            }

            if (DataEventList != null)
                DataEventList.RenderEvents(paintEventArgs.Graphics, everything, ScrollViewRectangle, 5);
            paintEventArgs.Graphics.SetClip(clip);

            long selectedleft = LongPoint.TranslateToScreen(new LongPoint(GraphLeft.Ticks, 1), everything, DataRange.UnitRange, new LongRectangle(ScrollViewRectangle)).X;
            long selectedright = LongPoint.TranslateToScreen(new LongPoint(GraphRight.Ticks, 1), everything, DataRange.UnitRange, new LongRectangle(ScrollViewRectangle)).X;

            paintEventArgs.Graphics.FillRectangle(Palette.ShadowRangeBrush, ScrollViewRectangle.Left, ScrollViewRectangle.Top, selectedleft - (ScrollViewRectangle.Left), ScrollViewRectangle.Height);
            paintEventArgs.Graphics.FillRectangle(Palette.ShadowRangeBrush, selectedright, ScrollViewRectangle.Top, ScrollViewRectangle.Width + (ScrollViewRectangle.Left) - selectedright, ScrollViewRectangle.Height);
            DrawGripper(paintEventArgs.Graphics, selectedright);
            DrawGripper(paintEventArgs.Graphics, selectedleft);
        }

        public List<DataPoint> GetFinerPoints(DataSet set, DataTimeRange range, ArchiveInterval current)
        {
            return set.GetRange(range, ArchiveMaintainer.ToTicks(current), ArchiveMaintainer.ToTicks(ArchiveMaintainer.NextArchiveDown(current)));
        }

        private void DrawGripper(Graphics g, long pos)
        {
            g.DrawLine(Palette.GridPen, pos, 0, pos, ScrollViewRectangle.Height);
            g.FillRectangle(SystemBrushes.ControlDarkDark, pos - 2, (ScrollViewRectangle.Height / 2) - 10, 5, 20);
        }

        private DragMode MouseDragging = DragMode.None;
        private int MouseDragStart;
        private ResizeMode MouseResizing = ResizeMode.None;

        private TimeSpan _scrollViewOffset = TimeSpan.Zero;

        public TimeSpan ScrollViewOffset
        {
            get { return _scrollViewOffset; }
            set { _scrollViewOffset = value; }
        }

        private TimeSpan _scrollViewWidth = TimeSpan.FromTicks(ArchiveMaintainer.TicksInTwoHours);

        public TimeSpan ScrollViewWidth
        {
            get { return _scrollViewWidth; }
            set { _scrollViewWidth = value; }
        }
        private ScrollMode TimerScroll = ScrollMode.None;
        private Timer Scroller = new Timer();
        private TimeSpan AnimateCurrentWidth;
        private TimeSpan AnimateCurrentOffset;
        private bool Animating = false;
        private Timer ResizeAnimation = new Timer();
        public TimeSpan ScrollStrength = TimeSpan.Zero;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (ArchiveMaintainer == null || ArchiveMaintainer.LoadingInitialData)
                return;

            DataTimeRange everything = CurrentArchiveRange;

            long selectedleft = LongPoint.TranslateToScreen(new LongPoint(GraphLeft.Ticks, 1), everything, DataRange.UnitRange, new LongRectangle(ScrollViewRectangle)).X;
            long selectedright = LongPoint.TranslateToScreen(new LongPoint(GraphRight.Ticks, 1), everything, DataRange.UnitRange, new LongRectangle(ScrollViewRectangle)).X;

            if (e.Location.X > selectedleft - 3 && e.Location.X < selectedleft + 3)
            {
                MouseResizing = ResizeMode.Left;
            }
            else if (e.Location.X > selectedright - 3 && e.Location.X < selectedright + 3)
            {
                MouseResizing = ResizeMode.Right;
            }
            else if (e.Location.X > selectedleft - 3 && e.Location.X < selectedright + 3)
            {
                MouseDragging = DragMode.Existing;
            }
            else
            {
                MouseDragStart = e.X;
                MouseDragging = DragMode.New;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (ArchiveMaintainer == null || ArchiveMaintainer.LoadingInitialData)
                return;

            // Do the same thing for everything here for consistency
            DataTimeRange everything = CurrentArchiveRange;

            if (MouseDragging == DragMode.New && e.X == MouseDragStart)
            {
                LongPoint virtualclicked = LongPoint.DeTranslateFromScreen(new LongPoint(e.Location), everything, DataRange.UnitRange, new LongRectangle(ScrollViewRectangle));
                long proposedoffset = ArchiveMaintainer.GraphNow.Ticks - (virtualclicked.X + (GraphWidth.Ticks / 2));

                GraphOffset = TimeSpan.FromTicks(proposedoffset > 0 ? proposedoffset : 0);
            }

            TimerScroll = ScrollMode.None;
            Scroller.Stop();

            AutoScaleGraph();

            GridSpacing = BodgeSpacing(GraphWidth);
            RefreshXRange(false);

            MouseDragging = DragMode.None;
            MouseResizing = ResizeMode.None;
        }

        private void AutoScaleGraph()
        {
            while (true)
            {
                if (GraphWidth.Ticks < ScrollViewWidth.Ticks * 0.1 && ScrollViewWidth.Ticks / 7 > ArchiveMaintainer.TicksInTenMinutes)
                {
                    AnimateCurrentWidth = ScrollViewWidth;
                    Animating = true;

                    if (ScrollViewWidth.Ticks / 7 > ArchiveMaintainer.TicksInTenMinutes)
                    {
                        ScrollViewWidth = TimeSpan.FromTicks(ScrollViewWidth.Ticks / 7);
                    }
                }
                else if (GraphWidth.Ticks < ScrollViewWidth.Ticks * 0.1)
                {
                    AnimateCurrentWidth = ScrollViewWidth;
                    Animating = true;
                    ScrollViewWidth = TimeSpan.FromTicks(ArchiveMaintainer.TicksInTenMinutes);
                    break;
                }
                else if (GraphWidth.Ticks > ScrollViewWidth.Ticks * 0.7 && ScrollViewWidth.Ticks * 7 < ArchiveMaintainer.TicksInOneYear)
                {
                    AnimateCurrentWidth = ScrollViewWidth;
                    Animating = true;
                    ScrollViewWidth = TimeSpan.FromTicks((long)(ScrollViewWidth.Ticks * 7));
                }
                else if (GraphWidth.Ticks > ScrollViewWidth.Ticks * 0.7)
                {
                    AnimateCurrentWidth = ScrollViewWidth;
                    Animating = true;
                    ScrollViewWidth = TimeSpan.FromTicks(ArchiveMaintainer.TicksInOneYear);
                    break;
                }
                else
                {
                    break;
                }
            }


            if (Animating)
            {
                AnimateCurrentOffset = ScrollViewOffset;
                long edgeoffset = MouseResizing == ResizeMode.Left ? 0 : MouseResizing == ResizeMode.Right ? GraphWidth.Ticks : (GraphWidth.Ticks / 2);
                long olddistancetomid = (GraphOffset.Ticks + edgeoffset) - ScrollViewOffset.Ticks;
                long newdistancetomid = (long)(((double)olddistancetomid / AnimateCurrentWidth.Ticks) * ScrollViewWidth.Ticks);
                TimeSpan proposedoffset = TimeSpan.FromTicks(GraphOffset.Ticks + edgeoffset - newdistancetomid);
                ScrollViewOffset = proposedoffset < TimeSpan.Zero ? TimeSpan.Zero : proposedoffset > GraphOffset ? GraphOffset : GraphOffset + GraphWidth > proposedoffset + ScrollViewWidth ? GraphOffset + GraphWidth - ScrollViewWidth : proposedoffset;
                ResizeAnimation.Start();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (ArchiveMaintainer == null || ArchiveMaintainer.LoadingInitialData)
                return;

            DataTimeRange everything = CurrentArchiveRange;
            LongPoint virtualclicked = LongPoint.DeTranslateFromScreen(new LongPoint(e.Location), everything, DataRange.UnitRange, new LongRectangle(ScrollViewRectangle));
            long selectedleft = LongPoint.TranslateToScreen(new LongPoint(GraphLeft.Ticks, 1), everything, DataRange.UnitRange, new LongRectangle(ScrollViewRectangle)).X;
            long selectedright = LongPoint.TranslateToScreen(new LongPoint(GraphRight.Ticks, 1), everything, DataRange.UnitRange, new LongRectangle(ScrollViewRectangle)).X;

            if (MouseDragging == DragMode.Existing)
            {
                long proposedoffset = ArchiveMaintainer.GraphNow.Ticks - (virtualclicked.X + (GraphWidth.Ticks / 2));

                if (proposedoffset + GraphWidth.Ticks > ScrollViewOffset.Ticks + ScrollViewWidth.Ticks)
                {
                    //  -------------------------
                    // |  _______________________|____________________________________ 
                    // | |      -------------    |           _______--------          |
                    // | |     /             \___|__________/               \         |
                    // | |----                   |                           ----    -|
                    // | |_______________________|_______________________________\__/_|
                    //  -------------------------
                    //
                    // we have scrolled too far left (off the scroll view)

                    GraphOffset = TimeSpan.FromTicks(ScrollViewWidth.Ticks + ScrollViewOffset.Ticks - GraphWidth.Ticks);
                }
                else if (proposedoffset < ScrollViewOffset.Ticks)
                {
                    //                                       -------------------------
                    //   ___________________________________|_______________________  |
                    // |      -------------                _|_____--------          | |
                    // |     /             \______________/ |             \         | |
                    // |----                                |              ----    -| |
                    // |____________________________________|__________________\__/_| |
                    //                                       -------------------------
                    //
                    // we have scrolled too far right (off the scroll view)
                    if (GraphOffset == ScrollViewOffset)
                        return;
                    GraphOffset = ScrollViewOffset;
                }
                else
                {
                    //              -------------------------
                    //  ___________|_________________________|______________________ 
                    // |      -----|-------                __|____--------          |
                    // |     /     |       \______________/  |            \         |
                    // |----       |                         |             ----    -|
                    // |___________|_________________________|_________________\__/_|
                    //              -------------------------
                    //
                    // everything is fine, we are in the middle

                    GraphOffset = TimeSpan.FromTicks(proposedoffset > 0 ? proposedoffset : 0);
                }

                if (GraphOffset.Ticks + GraphWidth.Ticks - ScrollViewOffset.Ticks > 0.90 * ScrollViewWidth.Ticks)
                {
                    ScrollStrength = TimeSpan.FromTicks(GraphOffset.Ticks + GraphWidth.Ticks - (ScrollViewOffset.Ticks + (long)(0.90 * ScrollViewWidth.Ticks)));
                    TimerScroll = ScrollMode.Left;
                    Scroller.Start();
                }
                else if (GraphOffset.Ticks - ScrollViewOffset.Ticks < 0.1 * ScrollViewWidth.Ticks)
                {
                    ScrollStrength = TimeSpan.FromTicks((long)(0.1 * ScrollViewWidth.Ticks) - (GraphOffset.Ticks - ScrollViewOffset.Ticks));
                    TimerScroll = ScrollMode.Right;
                    Scroller.Start();
                }
                else
                {
                    TimerScroll = ScrollMode.None;
                }
            }
            else if (MouseDragging == DragMode.New)
            {
                LongPoint virtualstart = LongPoint.DeTranslateFromScreen(new LongPoint(MouseDragStart, 1), everything, DataRange.UnitRange, new LongRectangle(ScrollViewRectangle));
                if (e.X > MouseDragStart && e.X < ScrollViewRectangle.Width)
                {
                    GraphOffset = new TimeSpan(ArchiveMaintainer.GraphNow.Ticks - virtualclicked.X);
                    GraphWidth = new TimeSpan(virtualclicked.X - virtualstart.X);
                }
                else if (e.X < MouseDragStart && e.X > 0)
                {
                    GraphOffset = new TimeSpan(ArchiveMaintainer.GraphNow.Ticks - virtualstart.X);
                    GraphWidth = new TimeSpan(virtualstart.X - virtualclicked.X);
                }
                else if (e.X < MouseDragStart)
                {
                    GraphOffset = new TimeSpan(ArchiveMaintainer.GraphNow.Ticks - virtualstart.X);
                    GraphWidth = ScrollViewOffset + ScrollViewWidth - GraphOffset;
                }
                else if (e.X > MouseDragStart)
                {
                    GraphOffset = ScrollViewOffset;
                    GraphWidth = new TimeSpan(ScrollViewRight.Ticks - virtualstart.X);
                }
            }
            else if (MouseResizing == ResizeMode.Left)
            {
                long proposedwidth = ArchiveMaintainer.GraphNow.Ticks - (virtualclicked.X + GraphOffset.Ticks);
                if (((proposedwidth * ScrollViewRectangle.Width) / ScrollViewWidth.Ticks) > 2)
                    GraphWidth = proposedwidth + GraphOffset.Ticks < ScrollViewWidth.Ticks + ScrollViewOffset.Ticks ? TimeSpan.FromTicks(proposedwidth) : ScrollViewWidth + ScrollViewOffset - GraphOffset;
                else
                    GraphWidth = TimeSpan.FromTicks((2 * ScrollViewWidth.Ticks) / ScrollViewRectangle.Width);
            }
            else if (MouseResizing == ResizeMode.Right)
            {
                long proposedwidth = virtualclicked.X - GraphRight.Ticks;
                if (ArchiveMaintainer.GraphNow.Ticks - virtualclicked.X < ScrollViewOffset.Ticks)
                {
                    GraphWidth += GraphOffset - ScrollViewOffset;
                    GraphOffset = ScrollViewOffset;
                }
                else if ((((GraphWidth.Ticks + proposedwidth) * ScrollViewRectangle.Width) / ScrollViewWidth.Ticks) > 2)
                {
                    GraphWidth += TimeSpan.FromTicks(virtualclicked.X - GraphRight.Ticks);
                    GraphOffset = TimeSpan.FromTicks(ArchiveMaintainer.GraphNow.Ticks - virtualclicked.X);
                }
                else
                {
                    long oldwidth = GraphWidth.Ticks;
                    GraphWidth = TimeSpan.FromTicks((2 * ScrollViewWidth.Ticks) / ScrollViewRectangle.Width);
                    GraphOffset += TimeSpan.FromTicks(oldwidth - GraphWidth.Ticks);
                }
            }
            else if (e.Location.X > selectedleft - 3 && e.Location.X < selectedleft + 3)
            {
                Cursor = Cursors.SizeWE;
                return;
            }
            else if (e.Location.X > selectedright - 3 && e.Location.X < selectedright + 3)
            {
                Cursor = Cursors.SizeWE;
                return;
            }
            else if (e.Location.X > selectedleft - 3 && e.Location.X < selectedright + 3)
            {
                Cursor = Cursors.SizeAll;
                return;
            }
            else
            {
                Cursor = Cursors.Cross;
                return;
            }

            GridSpacing = BodgeSpacing(GraphWidth);
            RefreshXRange(false);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            Select();
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            if (ArchiveMaintainer == null || ArchiveMaintainer.LoadingInitialData)
            {
                Cursor = Cursors.Default;
                return;
            }

            Cursor = Cursors.Cross;
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            if (ArchiveMaintainer == null || ArchiveMaintainer.LoadingInitialData)
            {
                Cursor = Cursors.Default;
                return;
            }

            Cursor = Cursors.Default;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (ArchiveMaintainer == null || ArchiveMaintainer.LoadingInitialData)
                return;

            long delta = (long)(GraphWidth.Ticks * 0.1 * e.Delta / 120);

            if (GraphOffset.Ticks - delta < 0)
            {
                if (GraphOffset == TimeSpan.Zero)
                    return;
                GraphOffset = TimeSpan.Zero;
            }
            else if (GraphOffset.Ticks + GraphWidth.Ticks - delta > ArchiveMaintainer.TicksInOneYear)
            {
                if (GraphOffset.Ticks == ArchiveMaintainer.TicksInOneYear - GraphWidth.Ticks)
                    return;
                GraphOffset = TimeSpan.FromTicks(ArchiveMaintainer.TicksInOneYear - GraphWidth.Ticks);
            }
            else
            {
                GraphOffset -= TimeSpan.FromTicks(delta);
            }

            if (GraphOffset.Ticks - ScrollViewOffset.Ticks < 0)
            {
                ScrollViewOffset = GraphOffset;
            }
            else if (ScrollViewOffset.Ticks + ScrollViewWidth.Ticks - (GraphOffset.Ticks + GraphWidth.Ticks) < 0)
            {
                ScrollViewOffset = GraphOffset + GraphWidth - ScrollViewWidth;
            }

            RefreshXRange(false);
        }
    }

    public enum ResizeMode { None, Left, Right }
    public enum DragMode { None, Existing, New }
    public enum ScrollMode { None, Left, Right }
}
