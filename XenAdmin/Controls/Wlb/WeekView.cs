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
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using XenAdmin.Core;
using XenAdmin.Wlb;


namespace XenAdmin.Controls.Wlb
{

    public partial class WeekView : UserControl
    {
        #region Public Events
        public event MouseEventHandler OnTriggerPointClick;
        public event MouseEventHandler OnTriggerPointDoubleClick;
        #endregion

        #region Private Fields
        private const int CONTROL_MIMIMUM_WIDTH = 350;
        private const int CONTROL_MINIMUM_HEIGHT = 45;
        private const int DAYS_IN_WEEK = 7;
        private const int HOURS_IN_DAY = 24;
        private const int TOOLTIP_DELAY = 500;

        private Color _gridColor = SystemColors.Control; //Color.Black;
        private Color _hourLineColor = Color.DarkGray;
        private int _hourLineInterval = 6; //one tick line every 6 hours
        private int _smallTickHeight = 3;
        private int _largeTickHeight = 5;

        private Color _hourLabelColor = Color.DarkGray;
        private Padding _hourLabelPadding = new Padding(3);
        private Font _hourLabelFont;
        private int _hourLabelInterval = 6; //one hour label every 6 hours

        private Color _dayLabelColor = Color.DarkGray;
        private Padding _dayLabelPadding = new Padding(3);

        private Color _currentTimeColor = Color.Red;
        private bool _showCurrenttimeMarkWidth = true;

        private Color _hightlightColor = Color.Yellow;
        private HighlightType _highlightType = HighlightType.None;

        private Padding _barPadding = new Padding(3);
        private int _barHeight = 10;

        private TriggerPoint _selectedTriggerPoint = null;
        #endregion
        
        #region Local Variables
        private int _gridWidth;
        private int _gridHeight;
        
        private float _dayLabelHeight;
        private float _hourLabelHeight;
        
        private float _dayWidth;
        private float _hourWidth;
        private float _hourIntervalWidth;
        
        private int _tickLineY;
        private int _barLineY;

        private int _barAreaHeight;

        private Point _lastMouseLocation = new Point();
        private bool _ignoreNextOnMouseMove;
        private TriggerPoints _triggerPoints;
        private Dictionary<RectangleF, TriggerPoint> _taskMap = new Dictionary<RectangleF, TriggerPoint>();

        private Point _toolTipLocation = new Point();
        private ToolTip _toolTip = new ToolTip();
        private Timer _toolTipTimer = new Timer();
        private string _toolTipText = string.Empty;
        #endregion

        #region Public Enums
        public enum HighlightType
        {
            None,
            Bar,
            Box
        }
        #endregion

        #region Public Properties
        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), Bindable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color GridColor
        {
            get { return _gridColor; }
            set { _gridColor = value; }
        }
        public Color HourLineColor
        {
            get { return _hourLineColor; }
            set { _hourLineColor = value; }
        }
        public Color HourLabelColor
        {
            get { return _hourLabelColor; }
            set { _hourLabelColor = value; }
        }
        public Color DayLabelColor
        {
            get { return _dayLabelColor; }
            set { _dayLabelColor = value; }
        }
        public Color CurrentTimeMarkColor
        {
            get { return _currentTimeColor; }
            set { _currentTimeColor = value; }
        }
        public Color HightlightColor
        {
            get { return _hightlightColor; }
            set { _hightlightColor = value; }
        }

        public HighlightType SelectedItemHighlightType
        {
            get { return _highlightType; }
            set { _highlightType = value; }
        }

        public Padding DayLabelPadding
        {
            get { return _dayLabelPadding; }
            set { _dayLabelPadding = value; }
        }
        public Padding HourLabelPadding
        {
            get { return _hourLabelPadding; }
            set { _hourLabelPadding = value; }
        }
        public Padding BarPadding
        {
            get { return _barPadding; }
            set { _barPadding = value; }
        }

        //Day font uses the control's Font property
        public Font HourLabelFont
        {
            get { return _hourLabelFont; }
            set { _hourLabelFont = value; }
        }

        public int HourLineInterval
        {
            get { return _hourLineInterval; }
            set { _hourLineInterval = value; }
        }
        public int HourLabelInterval
        {
            get { return _hourLabelInterval; }
            set { _hourLabelInterval = value; }
        }

        public int LargeTickHeight
        {
            get { return _largeTickHeight; }
            set
            {
                if (value > _smallTickHeight)
                {
                    _largeTickHeight = value;
                }
            }
        }
        public int SmalltickHeight
        {
            get { return _smallTickHeight; }
            set
            {
                if (value < _largeTickHeight)
                {
                    _smallTickHeight = value;
                }
            }
        }
        public int BarHeight
        {
            get { return _barHeight; }
            set { _barHeight = value; }
        }


        public TriggerPoints TriggerPoints
        {
            get { return _triggerPoints; }
            set
            {
                _triggerPoints = value;
                _taskMap = new Dictionary<RectangleF, TriggerPoint>();
                this.Refresh();
            }
        }

        public bool ShowCurrentTimeMark
        {
            get { return _showCurrenttimeMarkWidth; }
            set { _showCurrenttimeMarkWidth = value; }
        }


        public override Size MinimumSize
        {
            get
            {
                return new Size(CONTROL_MIMIMUM_WIDTH, CONTROL_MINIMUM_HEIGHT);
            }
            set
            {
                if (value.Width >= CONTROL_MIMIMUM_WIDTH && value.Height >= CONTROL_MINIMUM_HEIGHT)
                {
                    base.MinimumSize = value;
                }
            }
        }

        public TriggerPoint SelectedTriggerPoint
        {
            get { return _selectedTriggerPoint; }
        }

        #endregion

        #region Public ctor
        public WeekView()
        {
            InitializeComponent();
            this.GridColor = Color.Black;
            this.HourLabelFont = this.Font;

            _triggerPoints = new TriggerPoints();

            _toolTip.Popup += toolTip_Popup;
            _toolTipTimer.Interval = TOOLTIP_DELAY;
            _toolTipTimer.Tick += toolTipTimer_Tick;
        }
        #endregion

        #region Public Methods
        public TriggerPoint AddTriggerPoint(TriggerPoint triggerPoint)
        {
            _triggerPoints.Add(triggerPoint);
            this.Refresh();
            return _triggerPoints.List[triggerPoint.SortKey];
        }

        public void ClearTriggerPoints()
        {
            _triggerPoints.Clear();
            this.Refresh();
        }
        #endregion

        #region Control Event Handlers
        private void WeekView_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            //initialize grid width
            _gridWidth = this.Width - this.Margin.Left - this.Margin.Right;
            _gridHeight = this.Height - this.Margin.Top - this.Margin.Bottom;
            
            //Virtual spacing elements
            _dayWidth = _gridWidth / (float)DAYS_IN_WEEK;
            _hourWidth = _dayWidth / (float)HOURS_IN_DAY;
            _hourIntervalWidth = _hourWidth * (float)_hourLabelInterval;
            
            //text label elements
            _dayLabelHeight = this.Font.GetHeight(graphics) + _dayLabelPadding.Top + _dayLabelPadding.Bottom;
            _hourLabelHeight = _hourLabelFont.GetHeight(graphics) + _hourLabelPadding.Top + _hourLabelPadding.Bottom;
            
            //determine the bar area height to make life easier. 
            //_barAreaHeight = _barHeight + _barPadding.Top + _barPadding.Bottom; // = (int)_gridHeight - (int)_dayLabelHeight - _largeTickHeight;
            _barAreaHeight = _gridHeight - (int)_hourLabelHeight - _largeTickHeight;
            _barHeight = _barAreaHeight - _barPadding.Top - _barPadding.Bottom;

            _barLineY = this.Margin.Top + (_barAreaHeight / 2);
            _tickLineY = _gridHeight - (int)_hourLabelHeight;

            //Draw the control grid
            DrawControl(graphics);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_lastMouseLocation != e.Location)
            {
                // CA-38305: on some PCs OnMouseMove gets repeatedly called while the mouse cursor is still.
                // this if statement ensures that the code below is only called while the mouse is actually moving.
                if (_ignoreNextOnMouseMove)
                {
                    // The reason for this is that when the tooltip pops up, we get an
                    // additional onMouseMove which would otherwise hide the tooltip again.
                    _ignoreNextOnMouseMove = false;
                    return;
                }
                _toolTipTimer.Stop();
                _toolTip.Hide(this);
                _lastMouseLocation = e.Location;
                _toolTipTimer.Start();
            }
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            _toolTipTimer.Stop();
            _toolTip.Hide(this);
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            MouseEventHandler handler = OnTriggerPointClick;
            _selectedTriggerPoint = GetSelectedTriggerPoint(e.Location);
            if (null != handler && null != _selectedTriggerPoint)
            {
                handler(this, e);
            }
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            MouseEventHandler handler = OnTriggerPointDoubleClick;
            _selectedTriggerPoint = GetSelectedTriggerPoint(e.Location);
            if (null != handler && null != _selectedTriggerPoint)
            {
                handler(this, e);
            }
        }

        private void toolTipTimer_Tick(object sender, EventArgs e)
        {
            TriggerPoint triggerPoint = GetSelectedTriggerPoint(_lastMouseLocation);
            if (null != triggerPoint)
            {
                string toolTipText = triggerPoint.ToolTip;
                ShowTooltip(toolTipText, _lastMouseLocation);
                _toolTipTimer.Stop();
            }
        }

        private void toolTip_Popup(object sender, PopupEventArgs e)
        {
            Point correctLocation = _lastMouseLocation + new Size(1, -e.ToolTipSize.Height);  // put it above-right of the mouse
            if (_toolTipLocation != correctLocation)
            {
                ShowTooltip(_toolTipText, correctLocation);
                return;
            }
            _ignoreNextOnMouseMove = true;
        }

        #endregion

        #region Private Methods
        private void DrawControl(Graphics graphics)
        {
            //Draw the box
            DrawControlBorder(graphics);

             //Draw hour lines
            DrawHourLines(graphics);

            //Draw the hours
            DrawDateTimeLabels(graphics);

            //Draw the schedule data
            DrawTriggerPoints(graphics);
        }

        private void DrawControlBorder(Graphics graphics)
        {
            using (Pen gridPen = new Pen(_gridColor))
            {
                Rectangle rect = new Rectangle(this.Margin.Left, this.Margin.Top, (int)_gridWidth, (int)_gridHeight);
                graphics.DrawRectangle(gridPen, rect);
            }
        }

        private void DrawHourLines(Graphics graphics)
        {
            using (Pen pen = new Pen(_hourLineColor))
            {
                graphics.DrawLine(pen, this.Margin.Left + 1, _tickLineY, this.Width - this.Margin.Right - 1, _tickLineY);

                //Draw day divider lines
                for (int i = 1; i < DAYS_IN_WEEK; i++)
                {
                    float dayLineX = this.Margin.Left + _dayWidth * (float)i;
                    graphics.DrawLine(pen, dayLineX, this.Margin.Top, dayLineX, _gridHeight);
                }

                for (int dayIncr = 0; dayIncr < DAYS_IN_WEEK; dayIncr++)
                {
                    //Draw small hour ticks
                    for (float hourIncr = _hourWidth; hourIncr < _dayWidth - 1; hourIncr += _hourWidth)
                    {
                        float hourLineX = this.Margin.Left + (_dayWidth * dayIncr) + hourIncr;
                        float hourLineY1 = _tickLineY - _smallTickHeight;
                        float hourLineY2 = _tickLineY; // _hourLineY + _hourTickSize;
                        float markerX = this.Margin.Left + (int)DateTime.Now.DayOfWeek * _dayWidth + DateTime.Now.Hour * _hourWidth;
                        if (_showCurrenttimeMarkWidth && (int)hourLineX == (int)markerX)
                        {
                            using (Pen markerPen = new Pen(_currentTimeColor))
                            {
                                graphics.DrawLine(markerPen, hourLineX, hourLineY1, hourLineX, hourLineY2);
                            }
                        }
                        else
                        {
                            graphics.DrawLine(pen, hourLineX, hourLineY1, hourLineX, hourLineY2);
                        }
                    }

                    //Draw large hour marks
                    for (float hourIncr = _hourIntervalWidth; hourIncr < _dayWidth - 1; hourIncr += _hourIntervalWidth)
                    {
                        float hourLineX = this.Margin.Left + (_dayWidth * dayIncr) + hourIncr;
                        float hourLineY1 = _tickLineY - _largeTickHeight;
                        float hourLineY2 = _tickLineY + 2; // _hourLineY + _hourTickSize;
                        float markerX = this.Margin.Left + (int)DateTime.Now.DayOfWeek * _dayWidth + DateTime.Now.Hour * _hourWidth;
                        if (_showCurrenttimeMarkWidth && (int)hourLineX == (int)markerX)
                        {
                            using (Pen markerPen = new Pen(_currentTimeColor))
                            {
                                graphics.DrawLine(markerPen, hourLineX, hourLineY1, hourLineX, hourLineY2);
                            }
                        }
                        else
                        {
                            graphics.DrawLine(pen, hourLineX, hourLineY1, hourLineX, hourLineY2);
                        }
                    }
                }
            }
        }

        private void DrawDateTimeLabels(Graphics graphics)
        {
            float dayLabelY = this.Margin.Top + _gridHeight - _hourLabelHeight;
            for (int dayIncr = 0; dayIncr < DAYS_IN_WEEK; dayIncr++) 
            {
                for (float hourIncr = 0; hourIncr < _dayWidth - 1; hourIncr += _hourIntervalWidth)
                {
                        float dayLabelX = this.Margin.Left + _dayWidth * dayIncr + hourIncr;
                        RectangleF textRectF = new RectangleF(dayLabelX, dayLabelY, _hourIntervalWidth * HOURS_IN_DAY / _hourLabelInterval, _hourLabelHeight);
                        StringFormat textFormat = new StringFormat(StringFormatFlags.NoWrap);
                        textFormat.Alignment = StringAlignment.Near;
                        textFormat.LineAlignment = StringAlignment.Near;
                        if (hourIncr == 0)
                        {
                            using (Brush brush = new SolidBrush(_dayLabelColor))
                            {
                                string dayString = AbbreviatedDayNames(dayIncr);
                                graphics.DrawString(dayString, _hourLabelFont, brush, textRectF, textFormat);
                            }
                        }
                        else
                        {
                        DateTime time = new DateTime(DateTime.Now.Year, 1, 1, (int)(_hourLabelInterval * ((int)hourIncr / (int)_hourIntervalWidth)), 0, 0);
                        using (Brush brush = new SolidBrush(_hourLabelColor))
                        {
                            string timeString = HelpersGUI.DateTimeToString(time, Messages.DATEFORMAT_H_SHORT, true);
                            graphics.DrawString(timeString, _hourLabelFont, brush, textRectF, textFormat);
                        }
                    }
                }
            }
        }

        private string AbbreviatedDayNames(int dayIncr)
        {
            switch (dayIncr)
            {
                case 0:
                    return Messages.SUNDAY_SHORT;
                case 1:
                    return Messages.MONDAY_SHORT;
                case 2:
                    return Messages.TUESDAY_SHORT;
                case 3:
                    return Messages.WEDNESDAY_SHORT;
                case 4:
                    return Messages.THURSDAY_SHORT;
                case 5:
                    return Messages.FRIDAY_SHORT;
                default:
                    return Messages.SATURDAY_SHORT;
            }
        }

        private void DrawTriggerPoints(Graphics graphics)
        {
            try
            {
                _taskMap.Clear();

                foreach (TriggerPoint triggerPoint in _triggerPoints.List.Values)
                {
                    DrawTriggerPoint(graphics, triggerPoint);
                }

                // unless the first trigger starts at Sunday 12a, we need to fill in the beginning of the bar
                //  with the "end" of the last trigger of the week, so insert a fake trigger at Sunday 12a.
                // Also, if there is only one trigger defined, we need to add the same fake Sunday trigger

                if (_triggerPoints.List.Count > 0)
                {
                    TriggerPoint firstTriggerPoint = _triggerPoints.List.Values[0];
                    TriggerPoint lastTriggerPoint = _triggerPoints.List.Values[_triggerPoints.List.Values.Count - 1];

                    if (firstTriggerPoint.Hour > 0 || _triggerPoints.List.Keys.Count == 1)
                    {
                        TriggerPoint fillerTriggerPoint = new TriggerPoint();
                        fillerTriggerPoint.Day = DayOfWeek.Sunday;
                        fillerTriggerPoint.Hour = 0;
                        fillerTriggerPoint.Color = lastTriggerPoint.Color;
                        fillerTriggerPoint.ToolTip = lastTriggerPoint.ToolTip;
                        fillerTriggerPoint.Tag = lastTriggerPoint.Tag;
                        fillerTriggerPoint.IsSelected = lastTriggerPoint.IsSelected;
                        
                        DrawTriggerPoint(graphics, fillerTriggerPoint);
                    }
                }
            }
            catch (Exception)
            {
            }

        }
       
        private void DrawTriggerPoint(Graphics graphics, TriggerPoint triggerPoint)
        {
            int lineX = (int)this.Margin.Left + (int)triggerPoint.Day * (int)_dayWidth + (int)triggerPoint.Hour * (int)_hourWidth;
            int lineY1 = (int)_barLineY - (_barHeight / 2) + _barPadding.Top;
            int lineY2 = (int)_barLineY + (_barHeight / 2) - _barPadding.Bottom;

            TriggerPoint nextTriggerPoint = _triggerPoints.GetNextTriggerPoint(triggerPoint);
            bool isLastTriggerPoint = (nextTriggerPoint.SortKey <= triggerPoint.SortKey);

            //If this is the last trigger point, we need to draw up to the end of the control
            int barWidth;
            if (isLastTriggerPoint)
            {
                barWidth = (int)this.Margin.Left + (int)_gridWidth - lineX;
            }
            else  //otherwise, draw up to the beginning of the next trigger
            {
                barWidth = (this.Margin.Left +
                                (int)nextTriggerPoint.Day * (int)_dayWidth +
                                nextTriggerPoint.Hour * (int)_hourWidth) -
                                lineX;
            }

            //Draw the main bar with Gradient
            RectangleF triggerRectangle = new RectangleF(lineX, lineY1, barWidth, (lineY2 - lineY1));
            graphics.Clip = new Region(triggerRectangle);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color barColor;

            if (_highlightType == WeekView.HighlightType.Bar && triggerPoint.IsSelected)
            {
                barColor = _hightlightColor;
            }
            else
            {
                barColor = triggerPoint.Color;
            }

            using (GraphicsPath outerPath = GetRoundedPath(triggerRectangle, 2))
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(triggerRectangle,
                                                                barColor,
                                                                ControlPaint.LightLight(barColor),
                                                                LinearGradientMode.Vertical))
                {
                    graphics.FillPath(brush, outerPath);
                }
            }

            //Draw the second gradient to make it shiny
            RectangleF shinyRectangle = new RectangleF(triggerRectangle.X + _barPadding.Left, triggerRectangle.Y + _barPadding.Top, triggerRectangle.Width - (_barPadding.Left + _barPadding.Right), triggerRectangle.Height * 0.49f);
            using (GraphicsPath innerPath = GetRoundedPath(shinyRectangle, 2))
            {
                int alphaTop = 120, alphaBottom = 30;
                Color topColor = Color.FromArgb(alphaTop, Color.White);
                Color bottomColor = Color.FromArgb(alphaBottom, Color.White);
                shinyRectangle = new RectangleF(shinyRectangle.X, shinyRectangle.Y, shinyRectangle.Width, shinyRectangle.Height + 1); // + _barPadding.Top + _barPadding.Bottom + 1);

                using (LinearGradientBrush lighterBrush = new LinearGradientBrush(shinyRectangle,
                                                                topColor,
                                                                bottomColor,
                                                                LinearGradientMode.Vertical))
                {
                    graphics.FillPath(lighterBrush, innerPath);
                }
            }

            //Draw hightlight for selected TriggerPoint
            if (_highlightType == HighlightType.Box && triggerPoint.IsSelected)
            {
                Rectangle highlightRectangle = new Rectangle(lineX, lineY1, barWidth - 1, (lineY2 - lineY1) - 1);
                using (Pen highlightPen = new Pen(_hightlightColor))
                {
                    graphics.DrawRectangle(highlightPen, highlightRectangle);
                }

            }

            if (!_taskMap.ContainsKey(triggerRectangle))
            {
                _taskMap.Add(triggerRectangle, triggerPoint);
            }
        }

        private void ShowTooltip(string text, Point location)
        {
            _toolTipText = text;
            _toolTipLocation = location;
            _toolTip.Show(text, this, location);
        }

        private TriggerPoint GetSelectedTriggerPoint(Point point)
        {
            TriggerPoint triggerPoint = null;

            foreach (KeyValuePair<RectangleF, TriggerPoint> pair in _taskMap)
            {
                if (pair.Key.Contains(_lastMouseLocation))
                {
                    triggerPoint = pair.Value;
                }
            }
            return triggerPoint;
        }

        #endregion

        #region Private Static Methods
        private static GraphicsPath GetRoundedPath(RectangleF rect, float radius)
        {
            float diameter = 2 * radius;

            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter - 1, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter - 1, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
        #endregion

    }

    public class TriggerPoint
    {
        #region Private Fields
        private DayOfWeek _day = DayOfWeek.Sunday;
        private int _hour = 0;
        private Color _color = Color.Transparent;
        private int _sortKey = 0;
        private bool _isSelected = false;
        private object _tag = null;
        private string _toolTip = string.Empty;
        #endregion

        #region Public ctors
        public TriggerPoint() { ;}

        public TriggerPoint(DayOfWeek Day, int Hour, Color Color)
        {
            this.Day = Day;
            this.Hour = Hour;
            this.Color = Color;
        }

        public TriggerPoint(DayOfWeek Day, int Hour, Color Color, string ToolTip)
        {
            this.Day = Day;
            this.Hour = Hour;
            this.Color = Color;
            this.ToolTip = ToolTip;
        }

        #endregion

        #region Public Properties
        public DayOfWeek Day
        {
            get { return _day; }
            set
            {
                _day = value;
                CalcSortKey();
            }
        }

        public int Hour
        {
            get { return _hour; }
            set
            {
                _hour = value;
                CalcSortKey();
            }
        }
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            internal set { _isSelected = value; }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public string ToolTip
        {
            get { return _toolTip; }
            set { _toolTip = value; }
        }

        public int SortKey
        {
            get { return _sortKey; }
        }
        #endregion

        #region Private Methods
        private void CalcSortKey()
        {
            _sortKey = (int)_day * 1000 + _hour;
        }
        #endregion
    }

    public class TriggerPoints
    {
        #region Private Fields
        private SortedList<int, TriggerPoint> _triggerPoints = new SortedList<int, TriggerPoint>();
        #endregion

        #region Public ctor
        public TriggerPoints() { ;}
        #endregion

        #region Public Properties
        public TriggerPoint Selected
        {
            get
            {
                foreach (TriggerPoint triggerPoint in _triggerPoints.Values)
                {
                    if (triggerPoint.IsSelected)
                    {
                        return triggerPoint;
                    }
                }
                return null;
            }
            set
            {
                if (null != value)
                {
                    foreach (TriggerPoint triggerPoint in _triggerPoints.Values)
                    {
                        triggerPoint.IsSelected = (triggerPoint.SortKey == value.SortKey);
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public SortedList<int, TriggerPoint> List
        {
            get { return _triggerPoints; }
        }

        internal void Clear()
        {
            _triggerPoints = new SortedList<int, TriggerPoint>();
        }

        public void ClearSelected()
        {
            foreach (TriggerPoint triggerPoint in _triggerPoints.Values)
            {
                triggerPoint.IsSelected = false;
            }
        }

        public TriggerPoint Add(TriggerPoint triggerPoint)
        {
            _triggerPoints.Add(triggerPoint.SortKey, triggerPoint);
            return triggerPoint;
        }

        public TriggerPoint Add(DayOfWeek day, int hour, Color color)
        {
            TriggerPoint triggerPoint = new TriggerPoint(day, hour, color);
            this.Add(triggerPoint);
            return triggerPoint;
        }

        public TriggerPoint Add(DayOfWeek day, int hour, Color color, string toolTip)
        {
            TriggerPoint triggerPoint = new TriggerPoint(day, hour, color, toolTip);
            this.Add(triggerPoint);
            return triggerPoint;
        }

        public TriggerPoints FindByTag(object tag)
        {
            TriggerPoints triggerPoints = null;
            foreach (TriggerPoint triggerPoint in _triggerPoints.Values)
            {
                if (object.Equals(triggerPoint.Tag, tag))
                {
                    if (triggerPoints == null)
                    {
                        triggerPoints = new TriggerPoints();
                    }
                    triggerPoints.Add(triggerPoint);
                }
            }
            return triggerPoints;
        }

        public IEnumerator<KeyValuePair<int, TriggerPoint>> GetEnumerator()
        {
            return _triggerPoints.GetEnumerator();
        }

        public TriggerPoint GetNextTriggerPoint(TriggerPoint triggerPoint)
        {
            TriggerPoint firstTriggerPoint = null;
            TriggerPoint nextTriggerPoint = null;
            int currentIndex = _triggerPoints.IndexOfKey(triggerPoint.SortKey);
            foreach (int key in _triggerPoints.Keys)
            {
                if (null == firstTriggerPoint)
                {
                    firstTriggerPoint = _triggerPoints[key];
                }

                if (key > triggerPoint.SortKey)
                {
                    nextTriggerPoint = _triggerPoints[key];
                    break;
                }
            }
            if (null == nextTriggerPoint)
            {
                nextTriggerPoint = firstTriggerPoint;
            }

            return nextTriggerPoint;
        }
        #endregion
    }

}
