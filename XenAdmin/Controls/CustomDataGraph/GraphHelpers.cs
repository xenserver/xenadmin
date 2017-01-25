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
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Controls.CustomDataGraph
{
    public class DesignedGraph : IEquatable<DesignedGraph>
    {
        private static int count;
        private readonly int uid;

        public List<DataSourceItem> DataSources = new List<DataSourceItem>();
        public string DisplayName;

        public DesignedGraph()
        {
            uid = count++;
            DisplayName = string.Empty;
        }

        public DesignedGraph(DesignedGraph sourceGraph) : this()
        {
            DisplayName = sourceGraph.DisplayName;
            foreach (DataSourceItem dsi in sourceGraph.DataSources)
            {
                DataSources.Add(new DataSourceItem(dsi.DataSource, dsi.ToString(), dsi.Color, dsi.Uuid));
            }
        }

        public override string ToString()
        {
            List<string> strs = new List<string>();
            foreach (DataSourceItem item in DataSources)
            {
                strs.Add(item.GetDataSource());
            }
            return string.Join(",", strs.ToArray());
        }

        public bool Equals(DesignedGraph other)
        {
            return uid == other.uid;
        }

        public bool IsSame(DesignedGraph other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;
            
            if ((this.DisplayName ?? string.Empty) != (other.DisplayName ?? string.Empty))
                return false;

            if (this.DataSources.Count != other.DataSources.Count)
                return false;

            for (int i = 0; i < this.DataSources.Count; i++)
            {
                if (this.DataSources[i].Uuid != other.DataSources[i].Uuid)
                    return false;
            }

            return true;
        }
    }

    public class DesignedGraphEqualityComparer : EqualityComparer<DesignedGraph>
    {
        public override bool Equals(DesignedGraph graph1, DesignedGraph graph2)
        {
            if (Object.ReferenceEquals(graph1, null))
                return false;

            return graph1.IsSame(graph2);
        }

        public override int GetHashCode(DesignedGraph graph)
        {
            if (Object.ReferenceEquals(graph, null))
                return 0;

            return graph.DisplayName.GetHashCode();
        }
    }

    public class DataSourceItem : IComparable<DataSourceItem>, IEquatable<DataSourceItem>
    {
        /// <summary>
        /// may be empty if DataSourceListItem is part of a DesignedGraph
        /// </summary>
        public Data_source DataSource;

        public bool Enabled;
        private string friendlyName;
        public Color Color;
        public bool ColorChanged;
        public string Uuid;
        public DataType DataType;

        public DataSourceItem(Data_source ds, string friendlyname, Color color, string uuid)
        {
            DataSource = ds;
            Enabled = DataSource.enabled;
            friendlyName = friendlyname;
            Color = color;
            Uuid = uuid;
        }

        public DataSourceItem(Data_source ds, string friendlyname, Color color, string uuid, IXenObject xo)
            : this(ds, friendlyname, color, uuid)
        {
            DataSet dataSet = DataSet.Create(uuid, xo);
            DataType = dataSet.Type;
        }

        public string GetDataSource()
        {
            string[] lst = Uuid.Split(':');
            return lst.Length == 0 ? "" : lst[lst.Length - 1];
        }

        public override string ToString()
        {
            return friendlyName;
        }

        public int CompareTo(DataSourceItem other)
        {
            return ToString().CompareTo(other.ToString());
        }

        public bool Equals(DataSourceItem other)
        {
            return Uuid.Equals(other.Uuid);
        }
    }

    public static class DataSourceItemList
    {
        private static Regex io_throughput_rw_regex = new Regex("^io_throughput_(read|write)_([a-f0-9]{8})$"); // old SR read/write datasources
        private static Regex sr_rw_regex = new Regex("^(read|write)_([a-f0-9]{8})$"); // replacement SR read/write datasources

        public static List<DataSourceItem> BuildList(IXenObject xenObject, List<Data_source> dataSources)
        {
            List<DataSourceItem> dataSourceItems = new List<DataSourceItem>();

            foreach (Data_source dataSource in dataSources)
            {
                if (dataSource.name_label == "memory_total_kib" || dataSource.name_label == "memory" || dataSource.name_label == "xapi_open_fds" || dataSource.name_label == "pool_task_count" || dataSource.name_label == "pool_session_count")
                    continue;

                string friendlyName;
                if (dataSource.name_label == "memory_free_kib" && xenObject is Host)
                    friendlyName = Helpers.GetFriendlyDataSourceName("memory_used_kib", xenObject);
                else if (dataSource.name_label == "memory_internal_free" && xenObject is VM)
                    friendlyName = Helpers.GetFriendlyDataSourceName("memory_internal_used", xenObject);
                else
                    friendlyName = Helpers.GetFriendlyDataSourceName(dataSource.name_label, xenObject);

                string itemUuid = Palette.GetUuid(dataSource.name_label, xenObject);
                dataSourceItems.Add(new DataSourceItem(dataSource, friendlyName, Palette.GetColour(itemUuid), itemUuid, xenObject));
            }

            // Filter old datasources only if we have their replacement ones
            if (dataSourceItems.Any(dsi => sr_rw_regex.IsMatch(dsi.DataSource.name_label)))
            {
                // Remove any old style data sources
                dataSourceItems.RemoveAll(dsi => io_throughput_rw_regex.IsMatch(dsi.DataSource.name_label));
            }

            return dataSourceItems;
        }
    }

    public static class GraphHelpers
    {
        public static bool IndexInRange<T>(this List<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        private static void MoveControlsUp<T>(List<T> controls, int index) where T : Control
        {
            if (controls.IndexInRange(index))
            {
                Point location = controls[index].Location;
                for (int i = index + 1; i < controls.Count; i++)
                {
                    Point point = controls[i].Location;
                    controls[i].Location = location;
                    location = point;
                }
            }
        }

        public static void DeleteControlAt<T>(this List<T> controls, int index) where T : Control
        {
            if (controls.IndexInRange(index))
            {
                T control = controls[index];
                if (index < controls.Count - 1)
                    MoveControlsUp(controls, index);
                controls.Remove(control);
                control.Dispose();
            }
        }

        public static void SwapListElements<T>(this List<T> list, int index1, int index2)
        {
            if (list.IndexInRange(index1) && list.IndexInRange(index2))
            {
                T element1 = list[index2];
                T element2 = list[index1];
                list[index1] = element1;
                list[index2] = element2;
            }
        }

        private static void SwapControlsLocation<T>(T control1, T control2) where T : Control
        {
            if (control1 != null && control2 != null)
            {
                Point location = control1.Location;
                control1.Location = control2.Location;
                control2.Location = location;
            }
        }

        public static void SwapControls<T>(this List<T> list, int index1, int index2) where T : Control
        {
            if (list.IndexInRange(index1) || list.IndexInRange(index2))
            {
                list.SwapListElements(index1, index2);
                SwapControlsLocation(list[index1], list[index2]);
            }
        }
    }
}
