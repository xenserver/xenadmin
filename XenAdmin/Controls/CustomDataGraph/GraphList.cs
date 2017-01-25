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
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Core;
using System.Linq;

namespace XenAdmin.Controls.CustomDataGraph
{
    public partial class GraphList : DoubleBufferedPanel
    {
        public const int GRAPH_PADDING = 20;
        public const int GRAPH_HEIGHT = 150;
        public const int GRAPH_YAXIS_LABEL_WIDTH = 30;
        public const int KEY_WIDTH = 200;
        public const int CONTROL_PADDING = 10;
        public const int CONTROL_LEFT = 3;
        public const int GRAPH_MIN_WIDTH = 400;

        public event Action SelectedGraphChanged;

        private ArchiveMaintainer _archiveMaintainer;
        private DataPlotNav _dataPlotNav;
        private DataEventList _dataEventList;
        private List<DataPlot> Plots = new List<DataPlot>();
        private List<DataKey> Keys = new List<DataKey>();
        private List<DesignedGraph> Graphs = new List<DesignedGraph>();
        private bool showingDefaultGraphs;
        private bool isSettingGraphs = false;

        public bool ShowingDefaultGraphs
        { get { return showingDefaultGraphs; } }

        private DesignedGraph selectedGraph;

        private void ChangeSelection(int index, bool isSelected)
        {
            if (Plots.IndexInRange(index))
            {
                Plots[index].IsSelected = isSelected;
            }
        }

        public DesignedGraph SelectedGraph
        {
            get { return selectedGraph; }
            set
            {
                if (selectedGraph != value)
                {
                    SuspendDrawing();
                    try
                    {
                        ChangeSelection(SelectedGraphIndex, false);
                        selectedGraph = value;
                        ChangeSelection(SelectedGraphIndex, true);
                    }
                    finally
                    {
                        ResumeDrawing();
                        RefreshBuffer();
                    }

                    if (SelectedGraphChanged != null)
                        SelectedGraphChanged();
                }
            }
        }

        public int SelectedGraphIndex
        {
            get { return Graphs.IndexOf(selectedGraph); }
        }

        public int Count
        {
            get { return Graphs.Count; }
        }

        public DataEventList DataEventList
        {
            get { return _dataEventList; }
            set
            {
                _dataEventList = value;
                foreach (DataPlot plot in Plots)
                    plot.DataEventList = DataEventList;
            }
        }

        public DataPlotNav DataPlotNav
        {
            get { return _dataPlotNav; }
            set
            {
                _dataPlotNav = value;
                foreach (DataPlot plot in Plots)
                    plot.DataPlotNav = DataPlotNav;
            }
        }

        public ArchiveMaintainer ArchiveMaintainer
        {
            get { return _archiveMaintainer; }
            set
            {
                _archiveMaintainer = value;
                foreach (DataPlot plot in Plots)
                    plot.ArchiveMaintainer = ArchiveMaintainer;
                foreach (DataKey key in Keys)
                    key.ArchiveMaintainer = ArchiveMaintainer;
            }
        }

        public GraphList()
        {
            InitializeComponent();
        }

        private void AddGraphDetails(DesignedGraph designedGraph, int index)
        {
            int y = (GRAPH_HEIGHT + CONTROL_PADDING) * index;
            if (VScroll)
                y = y - VerticalScroll.Value;

            DataPlot newplot =
                CreatePlot(new Point(CONTROL_LEFT - HorizontalScroll.Value, CONTROL_PADDING + y));

            int left = ClientSize.Width - (KEY_WIDTH + (2 * CONTROL_PADDING));
            if (left < GRAPH_MIN_WIDTH + (2 * CONTROL_PADDING))
            {
                left = GRAPH_MIN_WIDTH + (2 * CONTROL_PADDING);
            }
            DataKey newkey =
                CreateKey(new Point(left - HorizontalScroll.Value, CONTROL_PADDING + y + GRAPH_PADDING));
            foreach (DataSourceItem item in designedGraph.DataSources)
                newkey.DataSourceUUIDsToShow.Add(item.Uuid);
            newplot.DataKey = newkey;
            newkey.Enter += new EventHandler(dataKey_Enter);
            newkey.MouseDown += new MouseEventHandler(dataKey_MouseDown);
            newkey.MouseDoubleClick += new MouseEventHandler(dataKey_MouseDoubleClick);
            newkey.UpdateItems();
            newplot.DisplayName = designedGraph.DisplayName;
            newplot.MouseDown += new MouseEventHandler(dataPlot_MouseDown);
            newplot.MouseDoubleClick += new MouseEventHandler(dataPlot_MouseDoubleClick);
            newplot.RefreshBuffer();
        }

        public void SetGraphs(List<DesignedGraph> items)
        {
            isSettingGraphs = true;
            try
            {
                DesignedGraphEqualityComparer comparer = new DesignedGraphEqualityComparer();
                if (!items.SequenceEqual(Graphs, comparer))
                {
                    SuspendLayout();
                    try
                    {
                        SelectedGraph = null;
                        ClearKeys();
                        ClearPlots();
                        Graphs = items;
                        for (int i = 0; i < items.Count; i++)
                        {
                            AddGraphDetails(items[i], i);
                        }
                    }
                    finally
                    {
                        ResumeLayout();
                        RefreshBuffer();
                    }
                }
            }
            finally
            {
                isSettingGraphs = false;
            }
        }

        void dataKey_MouseDown(object sender, MouseEventArgs e)
        {
            DataKey dataKey = sender as DataKey;
            if (dataKey != null)
            {
                Point screenPoint = dataKey.PointToScreen(e.Location);
                SelectGraphAtPoint(screenPoint);
                Point clientPoint = PointToClient(screenPoint);
                base.OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, clientPoint.X, clientPoint.Y, e.Delta));
            }
        }

        private void dataKey_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMouseDoubleClick(e);
        }

        private void dataPlot_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMouseDoubleClick(e);
        }

        private void dataKey_Enter(object sender, EventArgs e)
        {
            if (isSettingGraphs)
                return;

            DataKey dataKey = sender as DataKey;
            if (dataKey != null)
            {
                ScrollControlIntoView(dataKey);
                int index = Keys.IndexOf(dataKey);
                if (index != SelectedGraphIndex && Graphs.IndexInRange(index))
                    SelectedGraph = Graphs[index];
            }
        }

        private void dataPlot_MouseDown(object sender, MouseEventArgs e)
        {
            DataPlot dataPlot = sender as DataPlot;
            if (dataPlot != null)
            {
                Point screenPoint = dataPlot.PointToScreen(e.Location);
                SelectGraphAtPoint(screenPoint);
                Point clientPoint = PointToClient(screenPoint);
                base.OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, clientPoint.X, clientPoint.Y, e.Delta));
            }
        }

        private void ClearPlots()
        {
            foreach (DataPlot plot in Plots)
            {
                Controls.Remove(plot);
                plot.Dispose();
            }
            Plots.Clear();
        }

        private void ClearKeys()
        {
            foreach (DataKey key in Keys)
            {
                Controls.Remove(key);
                key.Dispose();
            }
            Keys.Clear();
        }

        private DataKey CreateKey(Point location)
        {
            DataKey key = new DataKey();
            key.Location = location;
            key.Size = new Size(KEY_WIDTH, GRAPH_HEIGHT - (2 * GRAPH_PADDING));
            key.ArchiveMaintainer = ArchiveMaintainer;
            key.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            Keys.Add(key);
            this.Controls.Add(key);
            return key;
        }

        private DataPlot CreatePlot(Point location)
        {
            DataPlot plot = new DataPlot();
            plot.ArchiveMaintainer = ArchiveMaintainer;
            plot.DataPlotNav = DataPlotNav;
            plot.DataEventList = DataEventList;
            plot.Location = location;
            plot.Size = new Size(ClientSize.Width - (KEY_WIDTH + (CONTROL_PADDING * 3)), GRAPH_HEIGHT);
            plot.MinimumSize = new Size(GRAPH_MIN_WIDTH, GRAPH_HEIGHT);
            plot.Padding = new Padding(GRAPH_PADDING, GRAPH_PADDING, GRAPH_PADDING + GRAPH_YAXIS_LABEL_WIDTH, GRAPH_PADDING);
            plot.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            Plots.Add(plot);
            this.Controls.Add(plot);
            return plot;
        }

        public IXenObject XenObject { get; private set; }
        
        public void LoadGraphs(IXenObject xmo)
        {
            if (xmo == null)
                return;

            XenObject = xmo;

            List<string> dsuuids = new List<string>();
            Pool pool = Helpers.GetPoolOfOne(xmo.Connection);

            if (pool != null)
            {
                List<DesignedGraph> dglist = new List<DesignedGraph>();
                Dictionary<string, string> gui_config = Helpers.GetGuiConfig(pool);
                int i = 0;
                while (true)
                {
                    string key = Palette.GetLayoutKey(i,xmo);
                    if (!gui_config.ContainsKey(key))
                        break;

                    DesignedGraph dg = new DesignedGraph();
                    string[] dslist = gui_config[key].Split(',');
                    foreach (string ds in dslist)
                    {
                        AddDataSource(string.Format("{0}:{1}:{2}", xmo is Host ? "host" : "vm", Helpers.GetUuid(xmo), ds), dsuuids, dg);
                    }

                    key = Palette.GetGraphNameKey(i, xmo);
                    if (gui_config.ContainsKey(key))
                    {
                        dg.DisplayName = gui_config[key];
                    }

                    dglist.Add(dg);
                    i++;
                }

                showingDefaultGraphs = false;
                if (dglist.Count == 0)
                {
                    LoadDefaultGraphs();
                    return; // return because loaddefaultgraphs sets the displayed uuids
                }
                SetGraphs(dglist);
            }
            DataPlotNav.DisplayedUuids = dsuuids;
        }

        private void LoadDefaultGraphs()
        {
            List<string> dsuuids = new List<string>();
            if (XenObject is Host)
            {
                List<DesignedGraph> dg = new List<DesignedGraph>();

                Host host = (Host)XenObject;

                DesignedGraph cpudg = new DesignedGraph();
                cpudg.DisplayName = Messages.GRAPHS_DEFAULT_NAME_CPU;
                foreach (Host_cpu cpu in host.Connection.ResolveAll<Host_cpu>(host.host_CPUs))
                {
                    AddDataSource(string.Format("host:{0}:cpu{1}", host.uuid, cpu.number), dsuuids, cpudg);
                }

                DesignedGraph memdg = new DesignedGraph();
                memdg.DisplayName = Messages.GRAPHS_DEFAULT_NAME_MEMORY;
                AddDataSource(string.Format("host:{0}:memory_total_kib", host.uuid), dsuuids, memdg);
                AddDataSource(string.Format("host:{0}:memory_free_kib", host.uuid), dsuuids, memdg);

                DesignedGraph netdg = new DesignedGraph();
                netdg.DisplayName = Messages.GRAPHS_DEFAULT_NAME_NETWORK;
                foreach (PIF pif in host.Connection.ResolveAll(host.PIFs))
                {
                    AddDataSource(string.Format("host:{0}:pif_{1}_tx", host.uuid, pif.device), dsuuids, netdg);
                    AddDataSource(string.Format("host:{0}:pif_{1}_rx", host.uuid, pif.device), dsuuids, netdg);
                }

                dg.Add(cpudg);
                dg.Add(memdg);
                dg.Add(netdg);
                SetGraphs(dg);
            }

            if (XenObject is VM)
            {
                List<DesignedGraph> dg = new List<DesignedGraph>();

                VM vm = (VM)XenObject;

                DesignedGraph cpudg = new DesignedGraph();
                cpudg.DisplayName = Messages.GRAPHS_DEFAULT_NAME_CPU;
                for (int i = 0; i < vm.VCPUs_at_startup; i++)
                {
                    AddDataSource(string.Format("vm:{0}:cpu{1}", vm.uuid, i), dsuuids, cpudg);
                }

                DesignedGraph memdg = new DesignedGraph();
                memdg.DisplayName = Messages.GRAPHS_DEFAULT_NAME_MEMORY;
                AddDataSource(string.Format("vm:{0}:memory", vm.uuid), dsuuids, memdg);
                AddDataSource(string.Format("vm:{0}:memory_internal_free", vm.uuid), dsuuids, memdg);

                DesignedGraph netdg = new DesignedGraph();
                netdg.DisplayName = Messages.GRAPHS_DEFAULT_NAME_NETWORK;
                foreach (VIF vif in vm.Connection.ResolveAll<VIF>(vm.VIFs))
                {
                    AddDataSource(string.Format("vm:{0}:vif_{1}_tx", vm.uuid, vif.device), dsuuids, netdg);
                    AddDataSource(string.Format("vm:{0}:vif_{1}_rx", vm.uuid, vif.device), dsuuids, netdg);
                }

                DesignedGraph diskdg = new DesignedGraph();
                diskdg.DisplayName = Messages.GRAPHS_DEFAULT_NAME_DISK;
                foreach (VBD vbd in vm.Connection.ResolveAll<VBD>(vm.VBDs))
                {
                    AddDataSource(string.Format("vm:{0}:vbd_{1}_read", vm.uuid, vbd.device), dsuuids, diskdg);
                    AddDataSource(string.Format("vm:{0}:vbd_{1}_write", vm.uuid, vbd.device), dsuuids, diskdg);
                }

                dg.Add(cpudg);
                dg.Add(memdg);
                dg.Add(netdg);
                dg.Add(diskdg);
                SetGraphs(dg);
            }
            DataPlotNav.DisplayedUuids = dsuuids;
            showingDefaultGraphs = true;
        }

        void AddDataSource(string uuid, List<string> dsuuids, DesignedGraph dg)
        {
            dsuuids.Add(uuid);
            dg.DataSources.Add(new DataSourceItem(new Data_source(), "", Palette.GetColour(uuid), uuid));
        }

        private string elevatedUsername;
        private string elevatedPassword;
        private Session elevatedSession;

        private bool RbacRequired
        {
            get
            {
                return !XenObject.Connection.Session.IsLocalSuperuser &&
                       !Registry.DontSudo;
            }
        }

        public bool AuthorizedRole
        {
            get
            {
                elevatedPassword = string.Empty;
                elevatedUsername = string.Empty;
                elevatedSession = null;

                if (RbacRequired)
                {
                    SaveDataSourceStateAction action = new SaveDataSourceStateAction(XenObject.Connection, XenObject,
                                                                                     null, null);
                    List<Role> validRoles = new List<Role>();
                    if (!Role.CanPerform(action.GetApiMethodsToRoleCheck, XenObject.Connection, out validRoles))
                    {
                        var sudoDialog = XenAdminConfigManager.Provider.SudoDialogDelegate;
                        var result = sudoDialog(validRoles, action.Connection, action.Title);
                        if (!result.Result)
                            return false;

                        elevatedPassword = result.ElevatedUsername;
                        elevatedUsername = result.ElevatedPassword;
                        elevatedSession = result.ElevatedSession;
                    }
                }
                return true;
            }
        }

        private void UpdateDataSources(List<DataSourceItem> datasources)
        {
            foreach (DataSourceItem dsi in datasources)
            {
                bool found = false;
                foreach (DesignedGraph graph in Graphs)
                {
                    found = graph.DataSources.Contains(dsi);
                    if (found)
                    {
                        if (!Palette.HasCustomColour(dsi.Uuid))
                        {
                            dsi.ColorChanged = true;
                            Palette.SetCustomColor(dsi.Uuid, dsi.Color);
                        }
                        break;
                    }
                }

                if (!dsi.DataSource.standard && dsi.DataSource.name_label != "avg_cpu")
                    dsi.Enabled = found;
            }
        }

        private List<DataSourceItem> GetGraphsDataSources()
        {
            List<DataSourceItem> dataSources = new List<DataSourceItem>();
            foreach (DesignedGraph designedGraph in Graphs)
            {
                foreach (DataSourceItem dsi in designedGraph.DataSources)
                {
                    string datasourceName = dsi.GetDataSource();
                    if (datasourceName == "memory_total_kib" || datasourceName == "memory")
                        continue;

                    if (!Palette.HasCustomColour(dsi.Uuid))
                    {
                        dsi.DataSource.name_label = datasourceName;
                        dsi.ColorChanged = true;
                        Palette.SetCustomColor(dsi.Uuid, dsi.Color);
                        dataSources.Add(dsi);
                    }
                }
            }
            if (dataSources.Count > 0)
                return dataSources;

            return null;
        }

        private void SetSessionDetails(AsyncAction action)
        {
            if (elevatedSession == null || string.IsNullOrEmpty(elevatedUsername) || string.IsNullOrEmpty(elevatedPassword))
                return;

            action.sudoUsername = elevatedUsername;
            action.sudoPassword = elevatedPassword;
            action.Session = elevatedSession;
        }

        private void RunSaveGraphsAction(List<DesignedGraph> graphs, List<DataSourceItem> dataSources)
        {
            SaveDataSourceStateAction action = new SaveDataSourceStateAction(XenObject.Connection, XenObject,
                                                                             dataSources, graphs);
            SetSessionDetails(action);
            action.RunAsync();
        }

        public void SaveGraphs(List<DataSourceItem> dataSources)
        {
            if (dataSources != null)
            {
                UpdateDataSources(dataSources);
            }
            else
            {
                dataSources = GetGraphsDataSources();
            }

            List<DesignedGraph> graphs;
            if (ShowingDefaultGraphs)
            {
                graphs = new List<DesignedGraph>();
            }
            else
                graphs = Graphs;

            RunSaveGraphsAction(graphs, dataSources);
        }

        private void SwapGraphDetails(int index1, int index2)
        {
            Plots.SwapControls(index1, index2);
            Keys.SwapControls(index1, index2);
        }

        public void ExchangeGraphs(int index1, int index2)
        {
            if (!Graphs.IndexInRange(index1) || !Graphs.IndexInRange(index2))
                return;

            SuspendDrawing();
            SuspendLayout();
            try
            {
                Graphs.SwapListElements(index1, index2);
                SwapGraphDetails(index1, index2);

                if (SelectedGraphChanged != null)
                    SelectedGraphChanged();

                showingDefaultGraphs = false;
            }
            finally
            {
                if (VScroll)
                    ScrollControlIntoView(Plots[SelectedGraphIndex]);
                ResumeLayout();
                ResumeDrawing();
                RefreshBuffer();
            }
        }

        private void DeleteGraphDetailsAt(int index)
        {
            Controls.Remove(Plots[index]);
            Controls.Remove(Keys[index]);
            Plots.DeleteControlAt(index);
            Keys.DeleteControlAt(index);
        }

        public void DeleteGraph(DesignedGraph graph)
        {
            if (graph == null)
                return;

            SuspendLayout();
            try
            {
                int index = Graphs.IndexOf(graph);

                DesignedGraph newSelectedGraph = null;
                if (SelectedGraph == graph && Count > 1)
                {
                    newSelectedGraph = SelectedGraphIndex == Count - 1
                                           ? Graphs[SelectedGraphIndex - 1]
                                           : Graphs[SelectedGraphIndex + 1];
                }
                Graphs.Remove(graph);               
                DeleteGraphDetailsAt(index);

                if (newSelectedGraph != null)
                    SelectedGraph = newSelectedGraph;

                showingDefaultGraphs = false;
            }
            finally
            {
                if (VScroll)
                {
                    AdjustFormScrollbars(true);
                    ScrollControlIntoView(Plots[SelectedGraphIndex]);
                }
                ResumeLayout();
                RefreshBuffer();
            }
        }

        public void AddGraph(DesignedGraph graph)
        {
            SuspendLayout();
            try
            {
                Graphs.Add(graph);
                AddGraphDetails(graph, Graphs.Count - 1);

                SelectedGraph = graph;
                showingDefaultGraphs = false;
            }
            finally
            {
                if (VScroll)
                {
                    AdjustFormScrollbars(true);
                    ScrollControlIntoView(Plots[SelectedGraphIndex]);
                }
                ResumeLayout();
                RefreshBuffer();                
            }
        }

        public void ReplaceGraphAt(int index, DesignedGraph newGraph)
        {
            if (!Graphs.IndexInRange(index))
                return;

            SuspendLayout();
            try
            {
                bool isSelected = (SelectedGraph == Graphs[index]);

                Graphs.RemoveAt(index);
                Graphs.Insert(index, newGraph);

                Plots[index].DisplayName = newGraph.DisplayName;
                Keys[index].DataSourceUUIDsToShow.Clear();
                foreach (DataSourceItem item in newGraph.DataSources)
                    Keys[index].DataSourceUUIDsToShow.Add(item.Uuid);
                Keys[index].UpdateItems();

                if (isSelected)
                {
                    SelectedGraph = newGraph;
                }
                showingDefaultGraphs = false;
            }
            finally
            {
                ResumeLayout();
            }
        }

        public void LoadDataSources(Action<ActionBase> completedEventHandler)
        {
            if (XenObject == null)
                return;
            GetDataSourcesAction action = new GetDataSourcesAction(XenObject.Connection, XenObject);
            action.Completed += completedEventHandler;
            action.RunAsync();
        }

        public void RestoreDefaultGraphs()
        {
            LoadDefaultGraphs();
        }

        public List<string> DisplayNames
        {
            get { return Graphs.ConvertAll(g => g.DisplayName); }
        }

        protected override void OnDrawToBuffer(PaintEventArgs paintEventArgs)
        {
            if (Plots.IndexInRange(SelectedGraphIndex))
            {
                Rectangle rect =
                    RectangleToClient(
                        Plots[SelectedGraphIndex].RectangleToScreen(Plots[SelectedGraphIndex].ClientRectangle));

                rect.Y -= CONTROL_PADDING;
                rect.Height += CONTROL_PADDING;
                rect = Rectangle.FromLTRB(paintEventArgs.ClipRectangle.Left-1, rect.Top,
                                          paintEventArgs.ClipRectangle.Right, rect.Bottom);
                paintEventArgs.Graphics.FillRectangle(Palette.GraphShadow, rect);
            }
        }

        private DataPlot GetDataPlotAtPoint(Point point)
        {
            point.X = CONTROL_LEFT;
            DataPlot dataPlot = GetChildAtPoint(point) as DataPlot;
            if (dataPlot == null)
            {
                point.Y += CONTROL_PADDING;
                dataPlot = GetChildAtPoint(point) as DataPlot;
            }
            return dataPlot;
        }

        private void SelectGraphAtPoint(Point screenPoint)
        {
            Point point = PointToClient(screenPoint);

            DataPlot dataPlot = GetDataPlotAtPoint(point);
            if (dataPlot != null)
            {
                int index = Plots.IndexOf((DataPlot) dataPlot);
                Point dataPlotPoint = dataPlot.PointToClient(screenPoint);
                if (dataPlotPoint.X > dataPlot.ClientRectangle.Right)
                {
                    if (Keys.IndexInRange(index))
                        ScrollControlIntoView(Keys[index]);
                }
                else
                {
                    ScrollControlIntoView(dataPlot);
                }

                if (index != SelectedGraphIndex && Graphs.IndexInRange(index))
                {
                    SelectedGraph = Graphs[index];
                }
            }
        }

        private void ClearDataKeySelectedItemAt(int index)
        {
            if (Keys.IndexInRange(index))
            {
                DataKey dataKey = Keys[index];
                if (dataKey != null)
                {
                    dataKey.SelectedItem = null;
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Point point = e.Location;
            SelectGraphAtPoint(PointToScreen(e.Location));
            ClearDataKeySelectedItemAt(SelectedGraphIndex);
            RefreshBuffer();            
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            RefreshBuffer();
        }

        private int suspendCounter = 0;

        private void SuspendDrawing()
        {
            if (suspendCounter == 0)
                HelpersGUI.SuspendDrawing(this);
            suspendCounter++;
        }

        private void ResumeDrawing()
        {
            suspendCounter--;
            if (suspendCounter == 0)
                HelpersGUI.ResumeDrawing(this);
        }
    }
}
