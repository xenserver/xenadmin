namespace XenAdmin.Controls.Wlb
{
    partial class WlbOptimizePool
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbOptimizePool));
            this.applyButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.linkLabelReportHistory = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.optimizePoolListView = new XenAdmin.Controls.DoubleBufferedListView();
            this.columnHeader0 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.customListPanel = new XenAdmin.Controls.CustomListPanel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // applyButton
            // 
            resources.ApplyResources(this.applyButton, "applyButton");
            this.applyButton.Name = "applyButton";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.ButtonApply_Click);
            // 
            // statusLabel
            // 
            resources.ApplyResources(this.statusLabel, "statusLabel");
            this.statusLabel.BackColor = System.Drawing.Color.Transparent;
            this.statusLabel.Name = "statusLabel";
            // 
            // TitleLabel
            // 
            resources.ApplyResources(this.TitleLabel, "TitleLabel");
            this.TitleLabel.Name = "TitleLabel";
            // 
            // linkLabelReportHistory
            // 
            resources.ApplyResources(this.linkLabelReportHistory, "linkLabelReportHistory");
            this.linkLabelReportHistory.Name = "linkLabelReportHistory";
            this.linkLabelReportHistory.TabStop = true;
            this.linkLabelReportHistory.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabelReportHistory.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelReportHistory_LinkClicked);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.statusLabel);
            this.panel1.Controls.Add(this.optimizePoolListView);
            this.panel1.Controls.Add(this.TitleLabel);
            this.panel1.Controls.Add(this.linkLabelReportHistory);
            this.panel1.Controls.Add(this.applyButton);
            this.panel1.Name = "panel1";
            // 
            // optimizePoolListView
            // 
            this.optimizePoolListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            resources.ApplyResources(this.optimizePoolListView, "optimizePoolListView");
            this.optimizePoolListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader0,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.optimizePoolListView.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.optimizePoolListView.FullRowSelect = true;
            this.optimizePoolListView.MultiSelect = false;
            this.optimizePoolListView.Name = "optimizePoolListView";
            this.optimizePoolListView.OwnerDraw = true;
            this.optimizePoolListView.ShowItemToolTips = true;
            this.optimizePoolListView.UseCompatibleStateImageBehavior = false;
            this.optimizePoolListView.View = System.Windows.Forms.View.Details;
            this.optimizePoolListView.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.OptimizePoolListView_DrawColumnHeader);
            this.optimizePoolListView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.OptimizePoolListView_DrawItem);
            this.optimizePoolListView.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.optimizePoolListView_ColumnWidthChanged);
            this.optimizePoolListView.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.OptimizePoolListView_DrawSubItem);
            // 
            // columnHeader0
            // 
            this.columnHeader0.Tag = "hidden";
            resources.ApplyResources(this.columnHeader0, "columnHeader0");
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // customListPanel
            // 
            resources.ApplyResources(this.customListPanel, "customListPanel");
            this.customListPanel.BackColor = System.Drawing.Color.Transparent;
            this.customListPanel.Name = "customListPanel";
            // 
            // WlbOptimizePool
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.customListPanel);
            this.DoubleBuffered = true;
            this.Name = "WlbOptimizePool";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private XenAdmin.Controls.CustomListPanel customListPanel;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader0;
        private System.Windows.Forms.LinkLabel linkLabelReportHistory;
        private System.Windows.Forms.Panel panel1;
        private DoubleBufferedListView optimizePoolListView;

    }
}
