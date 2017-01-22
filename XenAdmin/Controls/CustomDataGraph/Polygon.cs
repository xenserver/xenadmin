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
    public class Polygon
    {
        public List<LongPoint> Points = new List<LongPoint>();

        public Polygon(List<LongPoint> points)
        {
            Points = points;
        }

        public bool Contains(LongPoint p)
        {
            // draw a line horizontally right to infinity
            // count the number of times a line crosses this line
            // if odd return true
            // if even return false
            int crosses = 0;
            int j = Points.Count - 1;
            for(int i = 0; i < Points.Count; i++)
            {
                double x1 = Points[i].X;
                double x2 = Points[j].X;
                double y1 = Points[i].Y;
                double y2 = Points[j].Y;
                double x = (((x2 - x1) * (p.Y - y1)) / (y2 - y1)) + x1;
                
                if (((y1 <= p.Y && p.Y < y2) || (y2 <= p.Y && p.Y < y1)) && p.X < x)
                    crosses++;
                j = i;
            }

            return crosses % 2 != 0;
        }
    }
}
