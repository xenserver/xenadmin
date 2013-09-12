namespace XenAdmin.TabPages
{
    partial class PerformancePage
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerformancePage));
            this.panel1 = new System.Windows.Forms.Panel();
            this.GraphList = new XenAdmin.Controls.CustomDataGraph.GraphList();
            this.DataEventList = new XenAdmin.Controls.CustomDataGraph.DataEventList();
            this.DataPlotNav = new XenAdmin.Controls.CustomDataGraph.DataPlotNav();
            this.EventsLabel = new System.Windows.Forms.Label();
            this.gradientPanel2 = new XenAdmin.Controls.GradientPanel.GradientPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.zoomButton = new System.Windows.Forms.Button();
            this.moveDownButton = new System.Windows.Forms.Button();
            this.moveUpButton = new System.Windows.Forms.Button();
            this.graphActionsButton = new System.Windows.Forms.Button();
            this.graphActionsMenuStrip = new XenAdmin.Controls.NonReopeningContextMenuStrip(this.components);
            this.newGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.restoreDefaultGraphsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomMenuStrip = new XenAdmin.Controls.NonReopeningContextMenuStrip(this.components);
            this.lastYearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastMonthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastWeekToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastDayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastHourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastTenMinutesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGraphToolStripContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editGraphToolStripContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteGraphToolStripContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.restoreDefaultGraphsToolStripContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastYearToolStripContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastMonthToolStripContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastWeekToolStripContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastDayToolStripContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastHourToolStripContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastTenMinutesToolStripContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pageContainerPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gradientPanel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.graphActionsMenuStrip.SuspendLayout();
            this.zoomMenuStrip.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.panel3);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.GraphList);
            this.panel1.Name = "panel1";
            // 
            // GraphList
            // 
            this.GraphList.ArchiveMaintainer = null;
            resources.ApplyResources(this.GraphList, "GraphList");
            this.GraphList.BackColor = System.Drawing.SystemColors.Window;
            this.GraphList.DataEventList = this.DataEventList;
            this.GraphList.DataPlotNav = this.DataPlotNav;
            this.GraphList.Name = "GraphList";
            this.GraphList.SelectedGraph = null;
            this.GraphList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.GraphList_MouseDoubleClick);
            // 
            // DataEventList
            // 
            resources.ApplyResources(this.DataEventList, "DataEventList");
            this.DataEventList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.DataEventList.FormattingEnabled = true;
            this.DataEventList.Name = "DataEventList";
            // 
            // DataPlotNav
            // 
            resources.ApplyResources(this.DataPlotNav, "DataPlotNav");
            this.DataPlotNav.ArchiveMaintainer = null;
            this.DataPlotNav.DataEventList = this.DataEventList;
            this.DataPlotNav.DisplayedUuids = ((System.Collections.Generic.List<string>)(resources.GetObject("DataPlotNav.DisplayedUuids")));
            this.DataPlotNav.GraphOffset = System.TimeSpan.Parse("00:00:00");
            this.DataPlotNav.GraphWidth = System.TimeSpan.Parse("00:09:59");
            this.DataPlotNav.GridSpacing = System.TimeSpan.Parse("00:02:00");
            this.DataPlotNav.MinimumSize = new System.Drawing.Size(410, 0);
            this.DataPlotNav.Name = "DataPlotNav";
            this.DataPlotNav.ScrollViewOffset = System.TimeSpan.Parse("00:00:00");
            this.DataPlotNav.ScrollViewWidth = System.TimeSpan.Parse("02:00:00");
            // 
            // EventsLabel
            // 
            this.EventsLabel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.EventsLabel, "EventsLabel");
            this.EventsLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.EventsLabel.Name = "EventsLabel";
            // 
            // gradientPanel2
            // 
            resources.ApplyResources(this.gradientPanel2, "gradientPanel2");
            this.gradientPanel2.Controls.Add(this.EventsLabel);
            this.gradientPanel2.Name = "gradientPanel2";
            this.gradientPanel2.Scheme = XenAdmin.Controls.GradientPanel.GradientPanel.Schemes.Title;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.zoomButton);
            this.panel3.Controls.Add(this.moveDownButton);
            this.panel3.Controls.Add(this.moveUpButton);
            this.panel3.Controls.Add(this.graphActionsButton);
            this.panel3.Controls.Add(this.gradientPanel2);
            this.panel3.Controls.Add(this.DataPlotNav);
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Controls.Add(this.DataEventList);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // zoomButton
            // 
            resources.ApplyResources(this.zoomButton, "zoomButton");
            this.zoomButton.Image = global::XenAdmin.Properties.Resources.expanded_triangle;
            this.zoomButton.Name = "zoomButton";
            this.zoomButton.UseVisualStyleBackColor = true;
            this.zoomButton.Click += new System.EventHandler(this.zoomButton_Click);
            // 
            // moveDownButton
            // 
            resources.ApplyResources(this.moveDownButton, "moveDownButton");
            this.moveDownButton.Name = "moveDownButton";
            this.moveDownButton.UseVisualStyleBackColor = true;
            this.moveDownButton.Click += new System.EventHandler(this.moveDownButton_Click);
            // 
            // moveUpButton
            // 
            resources.ApplyResources(this.moveUpButton, "moveUpButton");
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.UseVisualStyleBackColor = true;
            this.moveUpButton.Click += new System.EventHandler(this.moveUpButton_Click);
            // 
            // graphActionsButton
            // 
            this.graphActionsButton.Image = global::XenAdmin.Properties.Resources.expanded_triangle;
            resources.ApplyResources(this.graphActionsButton, "graphActionsButton");
            this.graphActionsButton.Name = "graphActionsButton";
            this.graphActionsButton.UseVisualStyleBackColor = true;
            this.graphActionsButton.Click += new System.EventHandler(this.graphActionsButton_Click);
            // 
            // graphActionsMenuStrip
            // 
            this.graphActionsMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGraphToolStripMenuItem,
            this.editGraphToolStripMenuItem,
            this.deleteGraphToolStripMenuItem,
            this.toolStripSeparator3,
            this.restoreDefaultGraphsToolStripMenuItem});
            this.graphActionsMenuStrip.Name = "saveMenuStrip";
            resources.ApplyResources(this.graphActionsMenuStrip, "graphActionsMenuStrip");
            this.graphActionsMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.graphActionsMenuStrip_Opening);
            // 
            // newGraphToolStripMenuItem
            // 
            this.newGraphToolStripMenuItem.Name = "newGraphToolStripMenuItem";
            resources.ApplyResources(this.newGraphToolStripMenuItem, "newGraphToolStripMenuItem");
            this.newGraphToolStripMenuItem.Click += new System.EventHandler(this.addGraphToolStripMenuItem_Click);
            // 
            // editGraphToolStripMenuItem
            // 
            this.editGraphToolStripMenuItem.Name = "editGraphToolStripMenuItem";
            resources.ApplyResources(this.editGraphToolStripMenuItem, "editGraphToolStripMenuItem");
            this.editGraphToolStripMenuItem.Click += new System.EventHandler(this.editGraphToolStripMenuItem_Click);
            // 
            // deleteGraphToolStripMenuItem
            // 
            this.deleteGraphToolStripMenuItem.Name = "deleteGraphToolStripMenuItem";
            resources.ApplyResources(this.deleteGraphToolStripMenuItem, "deleteGraphToolStripMenuItem");
            this.deleteGraphToolStripMenuItem.Click += new System.EventHandler(this.deleteGraphToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // restoreDefaultGraphsToolStripMenuItem
            // 
            this.restoreDefaultGraphsToolStripMenuItem.Name = "restoreDefaultGraphsToolStripMenuItem";
            resources.ApplyResources(this.restoreDefaultGraphsToolStripMenuItem, "restoreDefaultGraphsToolStripMenuItem");
            this.restoreDefaultGraphsToolStripMenuItem.Click += new System.EventHandler(this.restoreDefaultGraphsToolStripMenuItem_Click);
            // 
            // zoomMenuStrip
            // 
            this.zoomMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lastYearToolStripMenuItem,
            this.lastMonthToolStripMenuItem,
            this.lastWeekToolStripMenuItem,
            this.lastDayToolStripMenuItem,
            this.lastHourToolStripMenuItem,
            this.lastTenMinutesToolStripMenuItem});
            this.zoomMenuStrip.Name = "saveMenuStrip";
            resources.ApplyResources(this.zoomMenuStrip, "zoomMenuStrip");
            // 
            // lastYearToolStripMenuItem
            // 
            this.lastYearToolStripMenuItem.Name = "lastYearToolStripMenuItem";
            resources.ApplyResources(this.lastYearToolStripMenuItem, "lastYearToolStripMenuItem");
            this.lastYearToolStripMenuItem.Click += new System.EventHandler(this.lastYearToolStripMenuItem_Click);
            // 
            // lastMonthToolStripMenuItem
            // 
            this.lastMonthToolStripMenuItem.Name = "lastMonthToolStripMenuItem";
            resources.ApplyResources(this.lastMonthToolStripMenuItem, "lastMonthToolStripMenuItem");
            this.lastMonthToolStripMenuItem.Click += new System.EventHandler(this.lastMonthToolStripMenuItem_Click);
            // 
            // lastWeekToolStripMenuItem
            // 
            this.lastWeekToolStripMenuItem.Name = "lastWeekToolStripMenuItem";
            resources.ApplyResources(this.lastWeekToolStripMenuItem, "lastWeekToolStripMenuItem");
            this.lastWeekToolStripMenuItem.Click += new System.EventHandler(this.lastWeekToolStripMenuItem_Click);
            // 
            // lastDayToolStripMenuItem
            // 
            this.lastDayToolStripMenuItem.Name = "lastDayToolStripMenuItem";
            resources.ApplyResources(this.lastDayToolStripMenuItem, "lastDayToolStripMenuItem");
            this.lastDayToolStripMenuItem.Click += new System.EventHandler(this.lastDayToolStripMenuItem_Click);
            // 
            // lastHourToolStripMenuItem
            // 
            this.lastHourToolStripMenuItem.Name = "lastHourToolStripMenuItem";
            resources.ApplyResources(this.lastHourToolStripMenuItem, "lastHourToolStripMenuItem");
            this.lastHourToolStripMenuItem.Click += new System.EventHandler(this.lastHourToolStripMenuItem_Click);
            // 
            // lastTenMinutesToolStripMenuItem
            // 
            this.lastTenMinutesToolStripMenuItem.Name = "lastTenMinutesToolStripMenuItem";
            resources.ApplyResources(this.lastTenMinutesToolStripMenuItem, "lastTenMinutesToolStripMenuItem");
            this.lastTenMinutesToolStripMenuItem.Click += new System.EventHandler(this.lastTenMinutesToolStripMenuItem_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.toolStripSeparator1,
            this.actionsToolStripMenuItem,
            this.toolStripSeparator2,
            this.zoomToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            resources.ApplyResources(this.moveUpToolStripMenuItem, "moveUpToolStripMenuItem");
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            resources.ApplyResources(this.moveDownToolStripMenuItem, "moveDownToolStripMenuItem");
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGraphToolStripContextMenuItem,
            this.editGraphToolStripContextMenuItem,
            this.deleteGraphToolStripContextMenuItem,
            this.toolStripSeparator4,
            this.restoreDefaultGraphsToolStripContextMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            resources.ApplyResources(this.actionsToolStripMenuItem, "actionsToolStripMenuItem");
            // 
            // newGraphToolStripContextMenuItem
            // 
            this.newGraphToolStripContextMenuItem.Name = "newGraphToolStripContextMenuItem";
            resources.ApplyResources(this.newGraphToolStripContextMenuItem, "newGraphToolStripContextMenuItem");
            this.newGraphToolStripContextMenuItem.Click += new System.EventHandler(this.newGraphToolStripContextMenuItem_Click);
            // 
            // editGraphToolStripContextMenuItem
            // 
            this.editGraphToolStripContextMenuItem.Name = "editGraphToolStripContextMenuItem";
            resources.ApplyResources(this.editGraphToolStripContextMenuItem, "editGraphToolStripContextMenuItem");
            this.editGraphToolStripContextMenuItem.Click += new System.EventHandler(this.editGraphToolStripContextMenuItem_Click);
            // 
            // deleteGraphToolStripContextMenuItem
            // 
            this.deleteGraphToolStripContextMenuItem.Name = "deleteGraphToolStripContextMenuItem";
            resources.ApplyResources(this.deleteGraphToolStripContextMenuItem, "deleteGraphToolStripContextMenuItem");
            this.deleteGraphToolStripContextMenuItem.Click += new System.EventHandler(this.deleteGraphToolStripContextMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // restoreDefaultGraphsToolStripContextMenuItem
            // 
            this.restoreDefaultGraphsToolStripContextMenuItem.Name = "restoreDefaultGraphsToolStripContextMenuItem";
            resources.ApplyResources(this.restoreDefaultGraphsToolStripContextMenuItem, "restoreDefaultGraphsToolStripContextMenuItem");
            this.restoreDefaultGraphsToolStripContextMenuItem.Click += new System.EventHandler(this.restoreDefaultGraphsToolStripContextMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // zoomToolStripMenuItem
            // 
            this.zoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lastYearToolStripContextMenuItem,
            this.lastMonthToolStripContextMenuItem,
            this.lastWeekToolStripContextMenuItem,
            this.lastDayToolStripContextMenuItem,
            this.lastHourToolStripContextMenuItem,
            this.lastTenMinutesToolStripContextMenuItem});
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            resources.ApplyResources(this.zoomToolStripMenuItem, "zoomToolStripMenuItem");
            // 
            // lastYearToolStripContextMenuItem
            // 
            this.lastYearToolStripContextMenuItem.Name = "lastYearToolStripContextMenuItem";
            resources.ApplyResources(this.lastYearToolStripContextMenuItem, "lastYearToolStripContextMenuItem");
            this.lastYearToolStripContextMenuItem.Click += new System.EventHandler(this.lastYearToolStripContextMenuItem_Click);
            // 
            // lastMonthToolStripContextMenuItem
            // 
            this.lastMonthToolStripContextMenuItem.Name = "lastMonthToolStripContextMenuItem";
            resources.ApplyResources(this.lastMonthToolStripContextMenuItem, "lastMonthToolStripContextMenuItem");
            this.lastMonthToolStripContextMenuItem.Click += new System.EventHandler(this.lastMonthToolStripContextMenuItem_Click);
            // 
            // lastWeekToolStripContextMenuItem
            // 
            this.lastWeekToolStripContextMenuItem.Name = "lastWeekToolStripContextMenuItem";
            resources.ApplyResources(this.lastWeekToolStripContextMenuItem, "lastWeekToolStripContextMenuItem");
            this.lastWeekToolStripContextMenuItem.Click += new System.EventHandler(this.lastWeekToolStripContextMenuItem_Click);
            // 
            // lastDayToolStripContextMenuItem
            // 
            this.lastDayToolStripContextMenuItem.Name = "lastDayToolStripContextMenuItem";
            resources.ApplyResources(this.lastDayToolStripContextMenuItem, "lastDayToolStripContextMenuItem");
            this.lastDayToolStripContextMenuItem.Click += new System.EventHandler(this.lastDayToolStripContextMenuItem_Click);
            // 
            // lastHourToolStripContextMenuItem
            // 
            this.lastHourToolStripContextMenuItem.Name = "lastHourToolStripContextMenuItem";
            resources.ApplyResources(this.lastHourToolStripContextMenuItem, "lastHourToolStripContextMenuItem");
            this.lastHourToolStripContextMenuItem.Click += new System.EventHandler(this.lastHourToolStripContextMenuItem_Click);
            // 
            // lastTenMinutesToolStripContextMenuItem
            // 
            this.lastTenMinutesToolStripContextMenuItem.Name = "lastTenMinutesToolStripContextMenuItem";
            resources.ApplyResources(this.lastTenMinutesToolStripContextMenuItem, "lastTenMinutesToolStripContextMenuItem");
            this.lastTenMinutesToolStripContextMenuItem.Click += new System.EventHandler(this.lastTenMinutesToolStripContextMenuItem_Click);
            // 
            // PerformancePage
            // 
            resources.ApplyResources(this, "$this");
            this.DoubleBuffered = true;
            this.Name = "PerformancePage";
            this.Controls.SetChildIndex(this.pageContainerPanel, 0);
            this.pageContainerPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.gradientPanel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.graphActionsMenuStrip.ResumeLayout(false);
            this.zoomMenuStrip.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public XenAdmin.Controls.CustomDataGraph.DataEventList DataEventList;
        public XenAdmin.Controls.CustomDataGraph.DataPlotNav DataPlotNav;
        private XenAdmin.Controls.CustomDataGraph.GraphList GraphList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label EventsLabel;
        private XenAdmin.Controls.GradientPanel.GradientPanel gradientPanel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button graphActionsButton;
        private XenAdmin.Controls.NonReopeningContextMenuStrip graphActionsMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem newGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editGraphToolStripMenuItem;
        private System.Windows.Forms.Button moveDownButton;
        private System.Windows.Forms.Button moveUpButton;
        private XenAdmin.Controls.NonReopeningContextMenuStrip zoomMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem lastYearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastMonthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastWeekToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastDayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastHourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastTenMinutesToolStripMenuItem;
        private System.Windows.Forms.Button zoomButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newGraphToolStripContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editGraphToolStripContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastYearToolStripContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastMonthToolStripContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastWeekToolStripContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastDayToolStripContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastHourToolStripContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastTenMinutesToolStripContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem restoreDefaultGraphsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteGraphToolStripContextMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem restoreDefaultGraphsToolStripContextMenuItem;
    }
}
