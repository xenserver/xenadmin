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
using System.Linq;
using System.Text;
using System.Drawing;


namespace XenAdmin.Controls.CustomDataGraph
{
    public static class LineRenderer
    {
        public static void Render(Graphics g, Rectangle r, DataTimeRange xrange, DataRange yrange, Pen pen, Brush graphShadow, List<DataPoint> points, bool showselections)
        {
            if (points.Count == 0)
                return;

            int index = points.FindIndex(dataPoint => dataPoint.Y < 0);
            if (index >= 0)
            {
                //CA-38898: when there are no data from the server we do not want
                //to show anything; leave gaps instead of making up data (zeros or inf)
                DoRender(g, r, xrange, yrange, pen, graphShadow, points.Where((val, idx) => idx < index).ToList(), showselections);
                Render(g, r, xrange, yrange, pen, graphShadow, points.Where((val, idx) => idx > index).ToList(), showselections);
            }
            else
            {
                DoRender(g, r, xrange, yrange, pen, graphShadow, points, showselections);
            }
        }

        private static void DoRender(Graphics g, Rectangle r, DataTimeRange xrange, DataRange yrange, Pen pen, Brush graphShadow, List<DataPoint> points, bool showselections)
        {
            if (points.Count == 0)
                return;

            // draw line 'tween points
            LongPoint locbase = LongPoint.TranslateToScreen(points[0].Point, xrange, yrange, new LongRectangle(r));

            List<Point> PointsToDraw = new List<Point>();

            for (int i = 1; i < points.Count; i++)
            {
                LongPoint loc = LongPoint.TranslateToScreen(points[i].Point, xrange, yrange, new LongRectangle(r));

                if (locbase.X == loc.X) continue;

                if (locbase.X > r.Right && loc.X < r.Left)
                {
                    // trim it to the Y axis
                    double delta = (double)(locbase.Y - loc.Y) / (locbase.X - loc.X);
                    int dxl = r.Left - (int)locbase.X;
                    int dxr = r.Right - (int)loc.X;
                    int newyr = (int)(delta * dxr);
                    int newyl = (int)(delta * dxl);
                    
                    PointsToDraw.Add(new Point(r.Right, newyr + (int)loc.Y));
                    PointsToDraw.Add(new Point(r.Left, newyl + (int)locbase.Y));
                    break;
                }
                else if (locbase.X > r.Right)
                {
                    // trim it to the Y axis
                    double delta = (double)(locbase.Y - loc.Y) / (locbase.X - loc.X);
                    int dx = r.Right - (int)loc.X;
                    int newy = (int)(delta * dx);
                    
                    PointsToDraw.Add(new Point(r.Right, newy + (int)loc.Y));
                }
                else if (loc.X < r.Left)
                {
                    // trim it to the Y axis
                    double delta = (double)(locbase.Y - loc.Y) / (locbase.X - loc.X);
                    int dx = r.Left - (int)locbase.X;
                    int newy = (int)(delta * dx);
                    
                    PointsToDraw.Add(locbase.Point);
                    PointsToDraw.Add(new Point(r.Left, newy + (int)locbase.Y));
                    break;
                }
                else
                {
                    PointsToDraw.Add(locbase.Point);
                    if (i == points.Count - 1)
                        PointsToDraw.Add(loc.Point);
                }
                if (points[i].Show && showselections)
                {
                    g.DrawRectangle(pen, new Rectangle((int)loc.X - 2, (int)loc.Y - 2, 4, 4));
                }
                locbase = loc;
            }
            if (PointsToDraw.Count <= 1)
                return;

            g.DrawLines(pen, PointsToDraw.ToArray());

            if (graphShadow == null)
                return;

            PointsToDraw.Add(new Point(PointsToDraw[PointsToDraw.Count - 1].X, r.Bottom));
            PointsToDraw.Add(new Point(PointsToDraw[0].X, r.Bottom));
            g.FillPolygon(graphShadow, PointsToDraw.ToArray());
        }
    }
}
