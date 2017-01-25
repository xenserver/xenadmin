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
    public abstract class DataAxis
    {
        protected DataGrid Grid;

        public virtual void DrawToBuffer(DrawAxisArgs args)
        {
            Grid.DrawToBuffer(args);

            args.Graphics.DrawLine(Palette.AxisPen,
                new Point(args.Rectangle.Right, args.Rectangle.Bottom),
                new Point(args.Rectangle.Left, args.Rectangle.Bottom));
        }
    }

    public class DataAxisX : DataAxis
    {
        public DataAxisX(ArchiveMaintainer archivemaintainer)
        {
            Grid = new DataGridHorizontal(archivemaintainer);
        }
    }

    public class DataAxisY : DataAxis
    {
        public DataAxisY(ArchiveMaintainer archivemaintainer)
        {
            Grid = new DataGridVertical(archivemaintainer);
        }
    }

    public class DataAxisNav : DataAxis
    {
        public DataAxisNav(ArchiveMaintainer archivemaintainer)
        {
            Grid = new DataGridNav(archivemaintainer);
        }

        public override void DrawToBuffer(DrawAxisArgs args)
        {
            Grid.DrawToBuffer(args);
        }
    }


    public abstract class DrawAxisArgs
    {
        public readonly Graphics Graphics;
        public readonly Rectangle Rectangle;
        public readonly bool ShowLabels;

        protected DrawAxisArgs(Graphics g, Rectangle r, bool showlabels)
        {
            Graphics = g;
            Rectangle = r;
            ShowLabels = showlabels;
        }
    }

    public class DrawAxisXArgs : DrawAxisArgs
    {
        public readonly DataTimeRange Range;

        public DrawAxisXArgs(Graphics g, Rectangle r, DataTimeRange range, bool showlabels)
            : base(g, r, showlabels)
        {
            Range = range;
        }
    }

    public class DrawAxisYArgs : DrawAxisArgs
    {
        public readonly DataRange Range;

        public DrawAxisYArgs(Graphics g, Rectangle r, DataRange range, bool showlabels)
            : base(g, r, showlabels)
        {
            Range = range;
        }
    }

    public class DrawAxisNavArgs : DrawAxisArgs
    {
        public readonly DataTimeRange Range;

        public DrawAxisNavArgs(Graphics g, Rectangle r, DataTimeRange range, bool showlabels)
            : base(g, r, showlabels)
        {
            Range = range;
        }
    }
}
