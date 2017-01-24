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
using System.Text;
using System.Drawing;

namespace XenAdmin.Controls.CustomDataGraph
{
    public class LongPoint
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int ARBITRARY_MICROSOFT_LINE_MAX_LENGTH = 0x4000007F;
        public long X;
        public long Y;

        public LongPoint(long x, long y)
        {
            X = x;
            Y = y;
        }

        public LongPoint(Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        public Point Point
        {
            get
            {
                return new Point((int)X, (int)Y);
            }
        }

        /// <summary>
        /// Virtual to Screen XY
        /// </summary>
        public static LongPoint TranslateToScreen(LongPoint virtualpoint, DataTimeRange xrange, DataRange yrange, LongRectangle rectangle)
        {
            if (xrange.Delta == 0 || yrange.Delta == 0)
            {
                // have seen this happen when we try to render graphs after the host failed to install a vm
                // doesn't matter too much what we return as the VM entry is going to get removed. Just don't crash.
                log.ErrorFormat("Tried to translate datapoint through range of zero. xrange.Delta: {0}, yrange.Delta: {1}",
                    xrange.Delta, yrange.Delta);
                return new LongPoint(0, 0);
            }
            // work out x, assume origin is bottom right
            double x = rectangle.Right - ((rectangle.Width * (virtualpoint.X - xrange.Min)) / xrange.Delta);

            // work out y
            double y = rectangle.Bottom - ((rectangle.Height * (virtualpoint.Y - yrange.Min)) / yrange.Delta);

            y = y > rectangle.Bottom ? rectangle.Bottom : y < rectangle.Y ? rectangle.Y : y;

            if (x >= ARBITRARY_MICROSOFT_LINE_MAX_LENGTH && y >= ARBITRARY_MICROSOFT_LINE_MAX_LENGTH)
            {
                log.DebugFormat("Point translated to more than max line length: x={0} y={1} vx={2} vy={3} xrange_min={4} xrange_max={5} yrange_min={6} yrange_max={7} rectangle_x={8} rectangle_y={9} rectangle_w={10} rectangle_h={11}",
                    x, y,
                    virtualpoint.X, virtualpoint.Y,
                    xrange.Min, xrange.Max,
                    yrange.Min, yrange.Max,
                    rectangle.X, rectangle.Y,
                    rectangle.Width, rectangle.Height);

                // draw a random line as this is better than crashing
                return new LongPoint(0, 0);
            }

            return new LongPoint((long)x, (long)y);
        }

        /// <summary>
        /// Screen XY to Virtual
        /// </summary>
        public static LongPoint DeTranslateFromScreen(LongPoint screenpoint, DataTimeRange xrange, DataRange yrange, LongRectangle rectangle)
        {
            // work out x, assume origin is bottom right
            double x = xrange.Min + (xrange.Delta * (rectangle.Right - screenpoint.X)) / (double)rectangle.Width;
            
            // work out y
            double y = yrange.Min + (yrange.Delta * (rectangle.Bottom - screenpoint.Y)) / (double)rectangle.Height;
            
            return new LongPoint((long)x, (long)y);
        }
    }
}
