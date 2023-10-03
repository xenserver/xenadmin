/* Copyright (c) Cloud Software Group, Inc. 
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
using XenAPI;
using XenCenterLib;
using XenAdmin.Core;

namespace XenAdmin.Controls.CustomDataGraph
{
    public class DataSet : IComparable<DataSet>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int NEGATIVE_VALUE = -1;

        /// <summary>
        /// Things can only be added to the beginning or end of this list; it should
        /// be sorted by X co-ordinate (which will be larger at the beginning of the list)
        /// </summary>
        public List<DataPoint> Points = new List<DataPoint>();
        public bool Selected;
        public List<DataPoint> CurrentlyDisplayed = new List<DataPoint>();
        public readonly IXenObject XenObject;
        public readonly string Id = "";
        public readonly string DataSourceName;
        public string FriendlyName { get; }
        private readonly int _multiplyingFactor = 1;
        public readonly DataRange CustomYRange = new DataRange(1, 0, 1, Unit.None, RangeScaleMode.Auto);
        public bool Hide { get; }

        public DataSet(IXenObject xo, bool hide, string datasourceName, List<Data_source> datasources)
        {
            XenObject = xo;
            Hide = datasourceName == "memory" || datasourceName == "memory_total_kib" || hide;

            DataSourceName = datasourceName;

            if (xo is Host host)
                Id = $"host:{host.uuid}:{datasourceName}";
            else if (xo is VM vm)
                Id = $"vm:{vm.uuid}:{datasourceName}";

            if (datasourceName == "memory_free_kib")
                FriendlyName = Helpers.GetFriendlyDataSourceName("memory_used_kib", xo);
            else if (datasourceName == "memory_internal_free")
                FriendlyName = Helpers.GetFriendlyDataSourceName("memory_internal_used", xo);
            else
                FriendlyName = Helpers.GetFriendlyDataSourceName(datasourceName, xo);

            var units = datasources.FirstOrDefault(d => datasourceName == d.name_label)?.units;

            switch (units)
            {
                case "count":
                case "requests":
                case "sessions":
                case "tasks":
                case "file descriptors":
                    break;
                case "percent":
                    CustomYRange = new DataRange(100, 0, 10, Unit.Percentage, RangeScaleMode.Auto);
                    break;
                case "(fraction)":
                    //CP-34000: use Auto instead of Fixed scale
                    CustomYRange = new DataRange(100, 0, 10, Unit.Percentage, RangeScaleMode.Auto);
                    _multiplyingFactor = 100;
                    break;
                case "MHz":
                    CustomYRange = new DataRange(1, 0, 1, Unit.MegaHertz, RangeScaleMode.Auto);
                    break;
                case "requests/s":
                case "hits/s":
                case "misses/s":
                case "err/s":
                case "sessions/s":
                case "reads/s":
                    CustomYRange = new DataRange(1, 0, 1, Unit.CountsPerSecond, RangeScaleMode.Auto);
                    break;
                case "s/s":
                    CustomYRange = new DataRange(1, 0, 1, Unit.SecondsPerSecond, RangeScaleMode.Auto);
                    break;
                case "s":
                    CustomYRange = new DataRange(1, 0, 1, Unit.NanoSeconds, RangeScaleMode.Auto);
                    _multiplyingFactor = (int)Util.DEC_GIGA;
                    break;
                case "ms":
                    CustomYRange = new DataRange(1, 0, 1, Unit.NanoSeconds, RangeScaleMode.Auto);
                    _multiplyingFactor = (int)Util.DEC_MEGA;
                    break;
                case "μs":
                    CustomYRange = new DataRange(1, 0, 1, Unit.NanoSeconds, RangeScaleMode.Auto);
                    _multiplyingFactor = (int)Util.DEC_KILO;
                    break;
                case "B":
                    CustomYRange = new DataRange(1, 0, 1, Unit.Bytes, RangeScaleMode.Auto);
                    break;
                case "KiB":
                    CustomYRange = new DataRange(1, 0, 1, Unit.Bytes, RangeScaleMode.Auto);
                    _multiplyingFactor = (int)Util.BINARY_KILO;
                    break;
                case "B/s":
                    CustomYRange = new DataRange(1, 0, 1, Unit.BytesPerSecond, RangeScaleMode.Auto);
                    break;
                case "MiB/s":
                    _multiplyingFactor = (int)Util.BINARY_MEGA;
                    CustomYRange = new DataRange(1, 0, 1, Unit.BytesPerSecond, RangeScaleMode.Auto);
                    break;
                case "mW":
                    CustomYRange = new DataRange(1, 0, 1, Unit.MilliWatt, RangeScaleMode.Auto);
                    break;
                case "Centigrade":
                    CustomYRange = new DataRange(1, 0, 1, Unit.Centigrade, RangeScaleMode.Auto);
                    break;
                case "unknown":
                    CustomYRange = new DataRange(1, 0, 1, Unit.Unknown, RangeScaleMode.Auto);
                    break;
                default:
                    if (string.IsNullOrEmpty(units))
                        CustomYRange = new DataRange(1, 0, 1, Unit.Unknown, RangeScaleMode.Auto);
                    else
                        System.Diagnostics.Debug.Assert(false, $"Unhandled units {units}!");
                    break;
            }

            if (datasourceName == "memory_free_kib" || datasourceName == "memory_internal_free")
            {
                var max = GetMemoryMax(xo);
                var resolution = GetMemoryResolution(max);
                    
                CustomYRange = new DataRange(max, 0, resolution, Unit.Bytes, RangeScaleMode.Delegate)
                {
                    UpdateMax = GetMemoryMax,
                    UpdateResolution = GetMemoryResolution
                };
            }
        }

        public static bool ParseId(string id, out string objType, out string objUuid, out string dataSourceName)
        {
            var bits = id.Split(':').ToList();

            if (bits.Count > 3)
                bits.RemoveAt(0);

            if (bits.Count >= 3)
            {
                objType = bits[0];
                objUuid = bits[1];
                dataSourceName = bits[2];
                return true;
            }

            objType = null;
            objUuid = null;
            dataSourceName = null;
            return false;
        }

        public List<DataPoint> GetRange(DataTimeRange xrange, long intervalneed, long intervalat)
        {
            List<DataPoint> fine = BinaryChop(Points, xrange);
            if (fine.Count == 0)
                return new List<DataPoint>();
            fine.Reverse();
            List<DataPoint> listout = new List<DataPoint>();
            double cumulativey = 0;
            int count = 0;
            foreach (DataPoint p in fine)
            {
                var secSince1970 = Util.TicksToSecondsSince1970(new DateTime(p.X).ToUniversalTime().Ticks);

                if (secSince1970 % (intervalneed / TimeSpan.TicksPerSecond) == secSince1970 % (intervalat / TimeSpan.TicksPerSecond))
                {
                    listout.Insert(0,new DataPoint(p.X,count != 0 ? cumulativey / count : p.Y));
                    cumulativey = 0;
                    count = 0;
                }
                else
                {
                    cumulativey += p.Y;
                    count++;
                }
            }
            if(count != 0)
                listout.Insert(0, new DataPoint(fine[fine.Count - 1].X, cumulativey / count));
            return listout;
        }

        private static double GetMemoryMax(IXenObject xo)
        {
            if (xo is Host host)
                return host.Connection.Resolve(host.metrics)?.memory_total ?? 100;

            if (xo is VM vm)
                return (vm.Connection.Resolve(vm.metrics))?.memory_actual ?? vm.memory_dynamic_max;

            return 100;
        }

        private static double GetMemoryResolution(IXenObject xmo)
        {
            return GetMemoryMax(xmo) / 10d;
        }

        private static double GetMemoryResolution(double max)
        {
            return max / 10d;
        }

        public static double GetMaxY(List<DataPoint> dataPoints)
        {
            return dataPoints == null || dataPoints.Count == 0
                       ? double.MinValue
                       : dataPoints.Max(dataPoint => dataPoint.Y);
        }

        public void RefreshCustomY(DataTimeRange range, List<DataPoint> points)
        {
            // find last element before beginning of xrange => binary chop
            CurrentlyDisplayed = BinaryChop(points, range);
        }

        public List<DataPoint> BinaryChop(List<DataPoint> points, DataTimeRange xrange)
        {
            if (xrange.Delta > 0)
            {
                log.DebugFormat("Get range: Delta should be negative, max={0}, min={1}", xrange.Max, xrange.Min);
                return new List<DataPoint>();
            }
            // if there are no points or none are in range
            if (points == null || points.Count == 0 || points[points.Count - 1].X >= xrange.Min || points[0].X <= xrange.Max)
                return new List<DataPoint>();
            
            int startindex = GetStart(points, xrange.Min, 0, points.Count);
            
            int endindex = GetStart(points, xrange.Max, 0, points.Count) + 1;
            
            if (endindex > 0 && endindex != points.Count)
                endindex++;
            
            if (startindex == -1 && endindex <= 0)
                return new List<DataPoint>();

            if (startindex == -1)
                if (endindex <= points.Count)
                    return points.GetRange(0, endindex);
                else
                    return new List<DataPoint>();

            if (endindex <= 0)
                if (startindex < points.Count)
                    return points.GetRange(startindex, points.Count - startindex);
                else
                    return new List<DataPoint>();

            System.Diagnostics.Trace.Assert(startindex >= 0 && startindex < points.Count && endindex >= 0 && endindex <= points.Count && endindex > startindex, string.Format("Argument exception: startindex={0}; endindex={1}; points.Count={2}", startindex, endindex, points.Count));
            
            return points.GetRange(startindex, endindex - startindex);
        }

        private int GetStart(List<DataPoint> points, long p, int start, int end)
        {
            if (start + 1 == end)
                return start;
            int halfway = (end - start) / 2;
            if (p <= points[start + halfway].X)
                return GetStart(points, p, start + halfway, end);
            else
                return GetStart(points, p, start, end - halfway);
        }

        public override string ToString()
        {
            return FriendlyName;
        }

        public DataPoint OnMouseMove(MouseActionArgs args)
        {
            LongPoint p = LongPoint.DeTranslateFromScreen(new LongPoint(args.Point), args.XRange, args.YRange, new LongRectangle(args.Rectangle));
            return ClosestPointTo(p);
        }

        private DataPoint ClosestPointTo(LongPoint p)
        {
            if (CurrentlyDisplayed.Count == 0 || CurrentlyDisplayed[CurrentlyDisplayed.Count - 1].X > p.X || CurrentlyDisplayed[0].X < p.X)
                return null;
            for (int i = 1; i < CurrentlyDisplayed.Count; i++)
            {
                if (CurrentlyDisplayed[i - 1].X >= p.X && CurrentlyDisplayed[i].X < p.X)
                {
                    if (p.X - CurrentlyDisplayed[i].X > (CurrentlyDisplayed[i - 1].X - CurrentlyDisplayed[i].X) / 2)
                        return CurrentlyDisplayed[i - 1];
                    else
                        return CurrentlyDisplayed[i];
                }
            }
            return null;
        }

        public bool OnMouseClick(MouseActionArgs args)
        {
            DataRange yrange = this.CustomYRange ?? args.YRange;
            List<DataPoint> range = BinaryChop(Points, args.XRange);
            if (range.Count == 0)
                return false;
            List<LongPoint> polypoints = new List<LongPoint>();
            foreach (DataPoint p in range)
            {
                LongPoint lp = LongPoint.TranslateToScreen(p.Point, args.XRange, yrange, new LongRectangle(args.Rectangle));
                polypoints.Add(new LongPoint(lp.X,lp.Y + 10));
                polypoints.Insert(0,new LongPoint(lp.X, lp.Y - 10));
            }
            Polygon poly = new Polygon(polypoints);
            
            return poly.Contains(new LongPoint(args.Point));
        }

        public void AddPoint(string str, long currentTime, List<DataSet> setsAdded, List<Data_source> dataSources)
        {
            double value = Helpers.StringToDouble(str);
            bool isNanOrInfinity = double.IsNaN(value) || double.IsInfinity(value);
            double yValue = isNanOrInfinity ? NEGATIVE_VALUE : value * _multiplyingFactor;

            #region memory

            if (DataSourceName == "memory_total_kib")
            {
                DataSet other = setsAdded.FirstOrDefault(s => s.DataSourceName == "memory_free_kib");
                if (other != null && other.Points.Count - 1 == Points.Count)
                {
                    yValue = isNanOrInfinity || other.Points[other.Points.Count - 1].Y < 0
                                 ? NEGATIVE_VALUE
                                 : value * _multiplyingFactor - other.Points[other.Points.Count - 1].Y;
                    other.Points[other.Points.Count - 1].Y = yValue;
                }
            }
            else if (DataSourceName == "memory_free_kib")
            {
                DataSet other = setsAdded.FirstOrDefault(s => s.DataSourceName == "memory_total_kib");
                if (other != null && other.Points.Count - 1 == Points.Count)
                {
                    yValue = isNanOrInfinity || other.Points[other.Points.Count - 1].Y < 0
                                 ? NEGATIVE_VALUE
                                 : other.Points[other.Points.Count - 1].Y - value * _multiplyingFactor;
                }
            }
            else if (DataSourceName == "memory")
            {
                DataSet other = setsAdded.FirstOrDefault(s => s.DataSourceName == "memory_internal_free");
                if (other != null && other.Points.Count - 1 == Points.Count)
                {
                    yValue = isNanOrInfinity || other.Points[other.Points.Count - 1].Y < 0
                                 ? NEGATIVE_VALUE
                                 : value * _multiplyingFactor - other.Points[other.Points.Count - 1].Y;
                    other.Points[other.Points.Count - 1].Y = yValue;
                }
            }
            else if (DataSourceName == "memory_internal_free")
            {
                DataSet other = setsAdded.FirstOrDefault(s => s.DataSourceName == "memory");
                if (other != null && other.Points.Count - 1 == Points.Count)
                {
                    yValue = isNanOrInfinity || other.Points[other.Points.Count - 1].Y < 0
                                 ? NEGATIVE_VALUE
                                 : other.Points[other.Points.Count - 1].Y - value * _multiplyingFactor;
                }
            }

            #endregion

            AddPoint(new DataPoint(currentTime, yValue));
        }

        public void AddPoint(DataPoint dataPoint)
        {
            Points.Add(dataPoint);

            if (CustomYRange == null || CustomYRange.ScaleMode == RangeScaleMode.Fixed)
                return;

            if (CustomYRange.ScaleMode == RangeScaleMode.Auto)
            {
                if (dataPoint.Y > CustomYRange.Max)
                {
                    CustomYRange.Max = dataPoint.Y * 1.05;
                    CustomYRange.Resolution = CustomYRange.Delta / 10 > 0 ? CustomYRange.Delta / 10 : 1;
                }
            }
            else if (CustomYRange.ScaleMode == RangeScaleMode.Delegate)
            {
                CustomYRange.UpdateAll(XenObject);
            }
        }

        private void InsertPoint(int index, DataPoint dataPoint)
        {
            Points.Insert(index, dataPoint);

            if (CustomYRange == null || CustomYRange.ScaleMode == RangeScaleMode.Fixed)
                return;

            if (CustomYRange.ScaleMode == RangeScaleMode.Auto)
            {
                if (dataPoint.Y > CustomYRange.Max)
                {
                    CustomYRange.Max = dataPoint.Y * 1.05;
                    CustomYRange.Resolution = CustomYRange.Delta / 10 > 0 ? CustomYRange.Delta / 10 : 1;
                }
            }
            else if (CustomYRange.ScaleMode == RangeScaleMode.Delegate)
            {
                CustomYRange.UpdateAll(XenObject);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DataSet other))
                return false;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(Id) ? 0 : Id.GetHashCode();
        }

        internal void InsertPointCollection(List<DataPoint> list)
        {
            if (list.Count == 0)
                return;

            if (Points.Count == 0)
            {
                Points = list;
                return;
            }

            if (list[list.Count - 1].X < Points[Points.Count - 1].X)
            {
                // filter out interesting shizzle
                List<DataPoint> interesting = BinaryChop(list, new DataTimeRange(list[list.Count - 1].X, Points[Points.Count - 1].X, 1));
                if (interesting.Count == 0)
                    return;
                interesting.RemoveAt(0);
                foreach (DataPoint point in interesting)
                {
                    AddPoint(point);
                }
            }
            if (list[0].X > Points[0].X)
            {
                //List<DataPoint> interesting = BinaryChop(list, new DataRange(Points[0].X, list[0].X, 1));
                if (list.Count == 0)
                    return;
                list.Reverse();
                foreach (DataPoint point in list)
                {
                    if(Points.Count > 0 && Points[0].X <point.X)
                        InsertPoint(0,point);
                }
            }
        }

        public void MergePointCollection(List<DataPoint> list, List<DataPoint> pointsin)
        {
            if (list.Count == 0)
                return;

            if (pointsin.Count == 0)
            {
                pointsin = list;
                return;
            }

            if (list[list.Count - 1].X < pointsin[pointsin.Count - 1].X)
            {
                // filter out interesting shizzle
                List<DataPoint> interesting = BinaryChop(list, new DataTimeRange(list[list.Count - 1].X, pointsin[pointsin.Count - 1].X, 1));
                if (interesting.Count == 0)
                    return;
                interesting.RemoveAt(0);
                foreach (DataPoint point in interesting)
                {
                    pointsin.Add(point);
                }
            }
            if (list[0].X > pointsin[0].X)
            {
                List<DataPoint> interesting = BinaryChop(list, new DataTimeRange(pointsin[0].X, list[0].X, 1));
                if (interesting.Count == 0)
                    return;
                interesting.Reverse();
                interesting.RemoveAt(0);
                foreach (DataPoint point in interesting)
                {
                    if (pointsin.Count > 0 && pointsin[0].X < point.X)
                        pointsin.Insert(0, point);
                }
            }
        }

        public void TrimEnd(int maxPoints)
        {
            if (Points.Count > maxPoints)
                Points.RemoveRange(maxPoints, Points.Count - maxPoints);
        }

        private double DisplayArea
        {
            get
            {
                if (CurrentlyDisplayed.Count == 0 || Selected)
                    return 0;
                double sum = 0;
                foreach (DataPoint dp in CurrentlyDisplayed)
                    sum += dp.Y;
                
                return sum / (CurrentlyDisplayed.Count * CustomYRange.Max);
            }
        }

        public int CompareTo(DataSet other)
        {
            if (Id == other.Id)
                return 0;

            int comp = DisplayArea.CompareTo(other.DisplayArea);
            if (comp == 0)
                return StringUtility.NaturalCompare(FriendlyName, other.FriendlyName);
            return comp;
        }

        private DataPoint GetPointAt(long currentTime)
        {
            List<DataPoint> pts = BinaryChop(Points, new DataTimeRange(currentTime - 1, currentTime + 1, 1));
            foreach (DataPoint p in pts)
                if (p.X == currentTime)
                    return p;

            return null;
        }
    }
}
