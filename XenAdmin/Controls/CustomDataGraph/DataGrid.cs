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

using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace XenAdmin.Controls.CustomDataGraph
{
    public abstract class DataGrid
    {
        protected GridLabels Labels;
        protected ArchiveMaintainer ArchiveMaintainer;

        protected DataGrid(ArchiveMaintainer archivemaintainer)
        {
            ArchiveMaintainer = archivemaintainer;
        }

        public abstract void DrawToBuffer(DrawAxisArgs args);
    }

    public class DataGridHorizontal : DataGrid
    {
        public DataGridHorizontal(ArchiveMaintainer archivemaintainer)
            : base(archivemaintainer)
        {
            Labels = GridLabels.All;
        }

        public override void DrawToBuffer(DrawAxisArgs drawArgs)
        {
            var args = drawArgs as DrawAxisXArgs;
            if (args == null)
                return;

            Dictionary<long, string> labels = new Dictionary<long, string>();
            long last_i = long.MinValue;

            for (long i = args.Range.Min - (args.Range.Min % -args.Range.Resolution); i >= args.Range.Max; i += args.Range.Resolution)
            {
                string label = args.Range.GetRelativeString(i + ArchiveMaintainer.ClientServerOffset.Ticks, ArchiveMaintainer.GraphNow);
                if (last_i != long.MinValue && label == labels[last_i])
                    labels.Remove(last_i);
                labels[i] = label;
                last_i = i;
            }

            for (long i = args.Range.Min - (args.Range.Min % -args.Range.Resolution); i >= args.Range.Max; i += args.Range.Resolution)
            {
                LongPoint pt = LongPoint.TranslateToScreen(new LongPoint(i, 0), args.Range, DataRange.UnitRange, new LongRectangle(args.Rectangle));
                if (i != 0)
                    args.Graphics.DrawLine(Palette.GridPen, new Point((int)pt.X, args.Rectangle.Bottom), new Point((int)pt.X, args.Rectangle.Top));

                if (!args.ShowLabels)
                    continue;

                if (!labels.ContainsKey(i))
                    continue;
                string label = labels[i];

                if (Labels == GridLabels.MinMax && (i == args.Range.Min - (args.Range.Min % -args.Range.Resolution) || i + args.Range.Resolution < args.Range.Max))
                {
                    SizeF labelsize = args.Graphics.MeasureString(label, Palette.LabelFont);
                    args.Graphics.DrawString(label, Palette.LabelFont, Palette.LabelBrush, new PointF(i == args.Range.Min - (args.Range.Min % -args.Range.Resolution) ? (int)pt.X - labelsize.Width : (int)pt.X, args.Rectangle.Bottom));
                }
                else if (Labels == GridLabels.All)
                {
                    SizeF labelsize = args.Graphics.MeasureString(label, Palette.LabelFont);
                    args.Graphics.DrawString(label, Palette.LabelFont, Palette.LabelBrush, new PointF(i == args.Range.Min ? (int)pt.X - labelsize.Width : i == args.Range.Max ? (int)pt.X : (int)pt.X - (labelsize.Width / 2), args.Rectangle.Bottom));
                }
            }
        }
    }

    public class DataGridVertical : DataGrid
    {
        public DataGridVertical(ArchiveMaintainer archivemaintainer)
            : base(archivemaintainer)
        {
            Labels = GridLabels.MinMax;
        }

        public override void DrawToBuffer(DrawAxisArgs drawArgs)
        {
            var args = drawArgs as DrawAxisYArgs;
            if (args == null)
                return;

            for (double i = args.Range.Min; i <= args.Range.Max; i += args.Range.Resolution)
            {
                // make sure that the last point is args.Range.Max
                if (i + args.Range.Resolution > args.Range.Max)
                    i = args.Range.Max; 

                string label = args.Range.GetRelativeString(i);
                LongPoint pt = LongPoint.TranslateToScreen(new LongPoint(0, (long)i), DataTimeRange.UnitRange, args.Range, new LongRectangle(args.Rectangle));

                if (i != 0)
                    args.Graphics.DrawLine(Palette.GridPen, new Point(args.Rectangle.Right, (int)pt.Y), new Point(args.Rectangle.Left, (int)pt.Y));

                if (!args.ShowLabels)
                    continue;

                if (Labels == GridLabels.All)
                {
                    SizeF labelsize = args.Graphics.MeasureString(label, Palette.LabelFont);
                    args.Graphics.DrawString(label, Palette.LabelFont, Palette.LabelBrush, new PointF(args.Rectangle.Right, i == args.Range.Min ? (int)pt.Y - labelsize.Height : i == args.Range.Max ? (int)pt.Y : (int)pt.Y - (labelsize.Height / 2)));
                }
                else if (Labels == GridLabels.MinMax)
                {
                    if (i == args.Range.Min || i + args.Range.Resolution > args.Range.Max)
                    {
                        SizeF labelsize = args.Graphics.MeasureString(label, Palette.LabelFont);
                        args.Graphics.DrawString(label, Palette.LabelFont, Palette.LabelBrush, new PointF(args.Rectangle.Right, i == args.Range.Min ? (int)pt.Y - labelsize.Height : i == args.Range.Max ? (int)pt.Y : (int)pt.Y - (labelsize.Height / 2)));
                    }

                    // draw units
                    string unitString = args.Range.UnitString;
                    if (!string.IsNullOrEmpty(unitString))
                    {
                        SizeF unitssize = args.Graphics.MeasureString(unitString, Palette.LabelFont);
                        args.Graphics.DrawString(unitString,
                            Palette.LabelFont,
                            Palette.LabelBrush,
                            new PointF(args.Rectangle.Right, args.Rectangle.Top + (args.Rectangle.Height / 2) - (unitssize.Height / 2)));
                    }
                }
            }
        }
    }


    public class DataGridNav : DataGrid
    {
        public DataGridNav(ArchiveMaintainer archivemaintainer)
            : base(archivemaintainer)
        {
            Labels = GridLabels.All;
        }

        public override void DrawToBuffer(DrawAxisArgs drawArgs)
        {
            var args = drawArgs as DrawAxisNavArgs;
            if (args == null)
                return;

            for (long i = args.Range.Min - (args.Range.Min % -args.Range.Resolution); i >= args.Range.Max; i += args.Range.Resolution)
            {
                string label = args.Range.GetRelativeString(i + ArchiveMaintainer.ClientServerOffset.Ticks, ArchiveMaintainer.GraphNow);
                LongPoint pt = LongPoint.TranslateToScreen(new LongPoint(i, 0), args.Range, DataRange.UnitRange, new LongRectangle(args.Rectangle));
                if (i != 0)
                    args.Graphics.DrawLine(Palette.GridPen, new Point((int)pt.X, args.Rectangle.Bottom), new Point((int)pt.X, args.Rectangle.Top));
                if (args.ShowLabels && Labels == GridLabels.All)
                {
                    args.Graphics.DrawString(label, Palette.LabelFont, Palette.LabelBrush, new PointF((int)pt.X, args.Rectangle.Top));
                }

            }
        }
    }

    public enum GridLabels { None, MinMax, All }
}
