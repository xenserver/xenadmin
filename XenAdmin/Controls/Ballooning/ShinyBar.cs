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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;


namespace XenAdmin.Controls.Ballooning
{
    public partial class ShinyBar : UserControl
    {
        private static readonly Font font = new Font("Segoe UI", 9);
        //private static readonly Font font = FormFontFixer.DefaultFont;
        private const int radius = 5;
        private const int pad = 2;
        private const int TEXT_PAD = 3;
        private const int TEXT_FADE = 8;

        // Because one bar has several regions with different tooltips, we handle the tooltip timing etc. ourselves.
        private ToolTip toolTip = new ToolTip();
        private Dictionary<Rectangle, string> toolTipRegions = new Dictionary<Rectangle, string>();
        private Timer toolTipTimer = new Timer();
        private const int TOOLTIP_DELAY = 500;
        private Point lastMouseLocation;
        private Point toolTipLocation;
        private string toolTipText;
        private bool ignoreNextOnMouseMove;

        public ShinyBar()
        {
            InitializeComponent();
            toolTip.Popup += toolTip_Popup;
            toolTipTimer.Interval = TOOLTIP_DELAY;
            toolTipTimer.Tick += toolTipTimer_Tick;
        }

        protected void DrawToTarget(Graphics g, Rectangle barBounds, Rectangle segmentBounds, Color color)
        {
            DrawToTarget(g, barBounds, segmentBounds, color, "", Color.Empty, HorizontalAlignment.Left, "");
        }

        protected void DrawToTarget(Graphics g, Rectangle barBounds, Rectangle segmentBounds, Color color, string text, Color textColor, HorizontalAlignment alignment, string toolTipText)
        {
            // The width shouldn't normally be < 0, but free space can be transiently < 0 while the metrics catch up,
            // especially across a reboot of a VM.
            if (segmentBounds.Width <= 0)
                return;

            // We draw the whole bar for each segment, but clip to the region corresponding to that segment (as if we
            // were viewing it through a succession of windows). This is the easiest way to make it the right shape.
            g.Clip = new Region(segmentBounds);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath outerPath = GetRoundedPath(barBounds, radius))
            {
                // Outer rounded rectangle
                {
                    Color topColor = color;
                    Color bottomColor = ControlPaint.LightLight(color);
                    using (LinearGradientBrush outerBrush = new LinearGradientBrush(barBounds, topColor, bottomColor, LinearGradientMode.Vertical))
                    {
                        g.FillPath(outerBrush, outerPath);
                    }
                }
            }

            // Render text
            // Text is rendered behind the 'reflection'. It is centered within the segment, unless the segment is too narrow to fit all the text,
            // in which case the text is rendered TEXT_PAD pixels from the left edge. If the text would extend into the TEXT_PAD pixels from
            // the right edge of the segment, then the right TEXT_FADE pixels of the text are faded out.
            if (!String.IsNullOrEmpty(text))
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                StringFormat sf = new StringFormat();
                SizeF textSize = g.MeasureString(text, font);
                float horizPos;
                switch (alignment)
                {
                    case HorizontalAlignment.Left:
                        horizPos = segmentBounds.Left + TEXT_PAD;
                        break;
                    case HorizontalAlignment.Right:
                        // NB If the caption is multi-line, it's still left-aligned within its box:
                        // just the box is right-aligned within the segment.
                        horizPos = segmentBounds.Right - textSize.Width - TEXT_PAD;
                        break;
                    default:  // HorizontalAlignment.Center:
                        horizPos = segmentBounds.Left + (segmentBounds.Width - textSize.Width) / 2;
                        break;
                }
                RectangleF textRect = new RectangleF(horizPos,
                    segmentBounds.Top + (segmentBounds.Height - textSize.Height * 0.9f) / 2,
                    textSize.Width,
                    textSize.Height);

                if (textRect.X < segmentBounds.X + TEXT_PAD)
                {
                    textRect.Offset((segmentBounds.X + TEXT_PAD) - textRect.X, 0);
                }

                // Render text to a bitmap...
                using (Bitmap bitmap = new Bitmap((int)(segmentBounds.Width + 1), (int)(segmentBounds.Height + 1), PixelFormat.Format32bppArgb))
                {
                    using (Graphics gBitmap = Graphics.FromImage(bitmap))
                    {
                        gBitmap.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                        gBitmap.Clip = new Region(new Rectangle(TEXT_PAD, 0, bitmap.Width - 2 * TEXT_PAD, bitmap.Height));
                        textRect.X -= segmentBounds.Left;
                        textRect.Y -= segmentBounds.Top;
                        using (SolidBrush textBrush = new SolidBrush(textColor))
                            gBitmap.DrawString(text, font, textBrush, textRect, sf);

                        // ...if we can't fit all the text in, overlay fade...
                        if (textSize.Width + (2 * TEXT_PAD) > bitmap.Width)
                        {
                            for (int x = bitmap.Width - TEXT_FADE - TEXT_PAD; x < bitmap.Width - TEXT_PAD; x++)
                            {
                                if (x < 0)
                                {
                                    // For very narrow segments, don't try to paint outside segment
                                    continue;
                                }

                                double i = (x - bitmap.Width + TEXT_FADE + TEXT_PAD) / (double)TEXT_FADE;
                                for (int y = 0; y < bitmap.Height; y++)
                                {
                                    Color c = bitmap.GetPixel(x, y);
                                    int alpha = (int)(c.A - (i * c.A));
                                    c = Color.FromArgb(alpha, c.R, c.G, c.B);
                                    bitmap.SetPixel(x, y, c);
                                }
                            }
                        }
                    }

                    // ...and render bitmap to final image
                    Point location = new Point((int)segmentBounds.X, (int)segmentBounds.Y);
                    g.DrawImage(bitmap, location);
                }
            }

            // Inner rounded rectangle
            RectangleF rect = new RectangleF(barBounds.X + pad, barBounds.Y + pad, barBounds.Width - (2f * pad), barBounds.Height * 0.49f);
            using (GraphicsPath innerPath = GetRoundedPath(rect, radius - pad))
            {
                int alphaTop = 120, alphaBottom = 30;
                Color topColor = Color.FromArgb(alphaTop, Color.White);
                Color bottomColor = Color.FromArgb(alphaBottom, Color.White);
                rect = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height + pad + 1);
                using (LinearGradientBrush lighterBrush = new LinearGradientBrush(rect, topColor, bottomColor, LinearGradientMode.Vertical))
                {
                    g.FillPath(lighterBrush, innerPath);
                }
            }

            // Remember the tooltip text
            if (!string.IsNullOrEmpty(toolTipText))
                toolTipRegions[segmentBounds] = toolTipText;

            // Reset the clip region
            g.Clip = new Region();
        }

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

        protected static void DrawGrid(Graphics g, Rectangle barArea, double bytesPerPixel, double max)
        {
            const int min_gap = 40;  // min gap between consecutive labels (which are on alternate ticks)
            const int line_height = 12;

            int line_bottom = barArea.Top + barArea.Height / 2;
            int line_top = barArea.Top - line_height;
            int text_bottom = line_top - 2;

            // Find the size of the longest label
            string label = string.Format(Messages.VAL_MB, Util.ToMB(max, RoundingBehaviour.Nearest));
            Size labelSize = Drawing.MeasureText(g, label, Program.DefaultFont, TextFormatFlags.NoPadding);
            int longest = labelSize.Width;
            int text_top = text_bottom - labelSize.Height;

            // Calculate a suitable increment
            long incr = Util.BINARY_MEGA / 2;
            while((double)incr / bytesPerPixel * 2 < min_gap + longest)
                incr *= 2;

            // Draw the grid
            using (Pen pen = new Pen(BallooningColors.Grid))
            {
                bool withLabel = true;
                for (long x = 0; x <= max; x += incr)
                {
                    // Tick
                    int pos = barArea.Left + (int)((double)x / bytesPerPixel);
                    g.DrawLine(pen, pos, line_top, pos, line_bottom);

                    // Label
                    if (withLabel)
                    {
                        label = Util.MemorySizeStringSuitableUnits(x, false);
                        Size size = Drawing.MeasureText(g, label, Program.DefaultFont);
                        Rectangle rect = new Rectangle(new Point(pos - size.Width/2, text_top), size);

                        if (LabelShouldBeShown(max, label, x))
                        {
                            Drawing.DrawText(g, label, Program.DefaultFont, rect, BallooningColors.Grid, Color.Transparent);
                        }
                    }
                    withLabel = !withLabel;
                }
            }
        }

        /// <summary>
        /// There are 2 cases:
        /// 1. If the maximum is smaller or equal to 1 GB, then show all the labels.
        /// 2. If the maximum is greater than 1 GB, then show only the labels that are a multiple of half a GB.
        /// </summary>
        /// <param name="max"></param>
        /// <param name="label"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static bool LabelShouldBeShown(double max, string label, long x)
        {
            return max <= Util.BINARY_GIGA  || (x % (0.5 * Util.BINARY_GIGA)) == 0;
        }

        protected virtual Rectangle barRect
        {
            get
            {
                return new Rectangle(20, 30, this.Width - 45, barHeight);
            }
        }

        protected virtual int barHeight
        {
            get { return 20; }
        }

        public override void Refresh()
        {
            toolTipRegions = new Dictionary<Rectangle, string>();
            base.Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (lastMouseLocation != e.Location)
            {
                // CA-38305: on some PCs OnMouseMove gets repeatedly called while the mouse cursor is still.
                // this if statement ensures that the code below is only called while the mouse is actually moving.
                
                if (ignoreNextOnMouseMove)
                {
                    // The reason for this is that when the tooltip pops up, we get an
                    // additional onMouseMove which would otherwise hide the tooltip again.
                    ignoreNextOnMouseMove = false;
                    return;
                }
                toolTipTimer.Stop();
                toolTip.Hide(this);
                lastMouseLocation = e.Location;
                toolTipTimer.Start();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            toolTipTimer.Stop();
            toolTip.Hide(this);
        }

        private void toolTipTimer_Tick(object sender, EventArgs e)
        {
            foreach (KeyValuePair<Rectangle, string> pair in toolTipRegions)
            {
                if (pair.Key.Contains(lastMouseLocation))
                {
                    ShowTooltip(pair.Value, lastMouseLocation);
                    toolTipTimer.Stop();
                }
            }
        }

        private void ShowTooltip(string text, Point location)
        {
            toolTipText = text;
            toolTipLocation = location;
            toolTip.Show(text, this, location);
        }

        private void toolTip_Popup(object sender, PopupEventArgs e)
        {
            Point correctLocation = lastMouseLocation + new Size(1, -e.ToolTipSize.Height);  // put it above-right of the mouse
            if (toolTipLocation != correctLocation)
            {
                ShowTooltip(toolTipText, correctLocation);
                return;
            }
            ignoreNextOnMouseMove = true;
        }
    }
}
