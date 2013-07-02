namespace XenAdmin.Dialogs
{
    partial class AlertSummaryDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlertSummaryDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ButtonClose = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.GridViewAlerts = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnExpand = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnType = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnAppliesTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDetails = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContextMenuAlertGridView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemFix = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemDismiss = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LabelCappingEntries = new System.Windows.Forms.Label();
            this.LabelDialogBlurb = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButtonServerFilter = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButtonDateFilter = new XenAdmin.Controls.FilterDatesToolStripDropDownButton();
            this.toolStripDropDownSeveritiesFilter = new System.Windows.Forms.ToolStripDropDownButton();
            this.dataLossImminentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serviceLossImminentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serviceDegradedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serviceRecoveredToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.informationalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unknownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonExportAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDismissAll = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButtonFix = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonHelp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDismiss = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewAlerts)).BeginInit();
            this.ContextMenuAlertGridView.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonClose
            // 
            resources.ApplyResources(this.ButtonClose, "ButtonClose");
            this.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources.alerts_32;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // GridViewAlerts
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            this.GridViewAlerts.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.GridViewAlerts, "GridViewAlerts");
            this.GridViewAlerts.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.GridViewAlerts.BackgroundColor = System.Drawing.SystemColors.Window;
            this.GridViewAlerts.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.GridViewAlerts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.GridViewAlerts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnExpand,
            this.ColumnType,
            this.ColumnAppliesTo,
            this.ColumnDetails,
            this.ColumnDate});
            this.GridViewAlerts.ContextMenuStrip = this.ContextMenuAlertGridView;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.GridViewAlerts.DefaultCellStyle = dataGridViewCellStyle7;
            this.GridViewAlerts.GridColor = System.Drawing.SystemColors.ControlDark;
            this.GridViewAlerts.MultiSelect = true;
            this.GridViewAlerts.Name = "GridViewAlerts";
            this.GridViewAlerts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GridViewAlerts_MouseDown);
            this.GridViewAlerts.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.AlertsGridView_SortCompare);
            this.GridViewAlerts.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.AlertsGridView_CellDoubleClick);
            this.GridViewAlerts.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.AlertsGridView_ColumnHeaderMouseClick);
            this.GridViewAlerts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.AlertsGridView_CellClick);
            this.GridViewAlerts.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GridViewAlerts_KeyDown);
            this.GridViewAlerts.SelectionChanged += new System.EventHandler(this.GridViewAlerts_SelectionChanged);
            // 
            // ColumnExpand
            // 
            this.ColumnExpand.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle2.NullValue = null;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.ColumnExpand.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnExpand.FillWeight = 49.51523F;
            resources.ApplyResources(this.ColumnExpand, "ColumnExpand");
            this.ColumnExpand.Name = "ColumnExpand";
            this.ColumnExpand.ReadOnly = true;
            this.ColumnExpand.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnType
            // 
            this.ColumnType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.ColumnType.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColumnType.FillWeight = 109.5524F;
            resources.ApplyResources(this.ColumnType, "ColumnType");
            this.ColumnType.Name = "ColumnType";
            this.ColumnType.ReadOnly = true;
            this.ColumnType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ColumnAppliesTo
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnAppliesTo.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColumnAppliesTo.FillWeight = 109.5524F;
            resources.ApplyResources(this.ColumnAppliesTo, "ColumnAppliesTo");
            this.ColumnAppliesTo.Name = "ColumnAppliesTo";
            this.ColumnAppliesTo.ReadOnly = true;
            // 
            // ColumnDetails
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnDetails.DefaultCellStyle = dataGridViewCellStyle5;
            this.ColumnDetails.FillWeight = 380F;
            resources.ApplyResources(this.ColumnDetails, "ColumnDetails");
            this.ColumnDetails.Name = "ColumnDetails";
            this.ColumnDetails.ReadOnly = true;
            // 
            // ColumnDate
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnDate.DefaultCellStyle = dataGridViewCellStyle6;
            this.ColumnDate.FillWeight = 130F;
            resources.ApplyResources(this.ColumnDate, "ColumnDate");
            this.ColumnDate.Name = "ColumnDate";
            this.ColumnDate.ReadOnly = true;
            this.ColumnDate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ContextMenuAlertGridView
            // 
            this.ContextMenuAlertGridView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemFix,
            this.ToolStripMenuItemHelp,
            this.ToolStripMenuItemDismiss,
            this.toolStripSeparator2,
            this.copyToolStripMenuItem});
            this.ContextMenuAlertGridView.Name = "ContextMenuAlertGridView";
            resources.ApplyResources(this.ContextMenuAlertGridView, "ContextMenuAlertGridView");
            this.ContextMenuAlertGridView.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuAlertGridView_Opening);
            // 
            // ToolStripMenuItemFix
            // 
            this.ToolStripMenuItemFix.Name = "ToolStripMenuItemFix";
            resources.ApplyResources(this.ToolStripMenuItemFix, "ToolStripMenuItemFix");
            this.ToolStripMenuItemFix.Click += new System.EventHandler(this.ToolStripMenuItemFix_Click);
            // 
            // ToolStripMenuItemHelp
            // 
            this.ToolStripMenuItemHelp.Name = "ToolStripMenuItemHelp";
            resources.ApplyResources(this.ToolStripMenuItemHelp, "ToolStripMenuItemHelp");
            this.ToolStripMenuItemHelp.Click += new System.EventHandler(this.ToolStripMenuItemHelp_Click);
            // 
            // ToolStripMenuItemDismiss
            // 
            this.ToolStripMenuItemDismiss.Image = global::XenAdmin.Properties.Resources._000_DeleteMessage_h32bit_16;
            this.ToolStripMenuItemDismiss.Name = "ToolStripMenuItemDismiss";
            resources.ApplyResources(this.ToolStripMenuItemDismiss, "ToolStripMenuItemDismiss");
            this.ToolStripMenuItemDismiss.Click += new System.EventHandler(this.ToolStripMenuItemDismiss_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::XenAdmin.Properties.Resources.copy_16;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // LabelCappingEntries
            // 
            resources.ApplyResources(this.LabelCappingEntries, "LabelCappingEntries");
            this.LabelCappingEntries.ForeColor = System.Drawing.Color.Crimson;
            this.LabelCappingEntries.Name = "LabelCappingEntries";
            // 
            // LabelDialogBlurb
            // 
            resources.ApplyResources(this.LabelDialogBlurb, "LabelDialogBlurb");
            this.LabelDialogBlurb.Name = "LabelDialogBlurb";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.LabelDialogBlurb, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButtonServerFilter,
            this.toolStripDropDownButtonDateFilter,
            this.toolStripDropDownSeveritiesFilter,
            this.toolStripSeparator3,
            this.toolStripButtonRefresh,
            this.toolStripSeparator1,
            this.toolStripButtonExportAll,
            this.toolStripButtonDismissAll});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripDropDownButtonServerFilter
            // 
            this.toolStripDropDownButtonServerFilter.AutoToolTip = false;
            resources.ApplyResources(this.toolStripDropDownButtonServerFilter, "toolStripDropDownButtonServerFilter");
            this.toolStripDropDownButtonServerFilter.Image = global::XenAdmin.Properties.Resources._000_FilterServer_h32bit_16;
            this.toolStripDropDownButtonServerFilter.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.toolStripDropDownButtonServerFilter.Name = "toolStripDropDownButtonServerFilter";
            // 
            // toolStripDropDownButtonDateFilter
            // 
            this.toolStripDropDownButtonDateFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripDropDownButtonDateFilter, "toolStripDropDownButtonDateFilter");
            this.toolStripDropDownButtonDateFilter.Name = "toolStripDropDownButtonDateFilter";
            // 
            // toolStripDropDownSeveritiesFilter
            // 
            this.toolStripDropDownSeveritiesFilter.AutoToolTip = false;
            this.toolStripDropDownSeveritiesFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataLossImminentToolStripMenuItem,
            this.serviceLossImminentToolStripMenuItem,
            this.serviceDegradedToolStripMenuItem,
            this.serviceRecoveredToolStripMenuItem,
            this.informationalToolStripMenuItem,
            this.unknownToolStripMenuItem});
            resources.ApplyResources(this.toolStripDropDownSeveritiesFilter, "toolStripDropDownSeveritiesFilter");
            this.toolStripDropDownSeveritiesFilter.Image = global::XenAdmin.Properties.Resources._000_FilterSeverity_h32bit_16;
            this.toolStripDropDownSeveritiesFilter.Name = "toolStripDropDownSeveritiesFilter";
            this.toolStripDropDownSeveritiesFilter.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripDropDownSeveritiesFilter_DropDownItemClicked);
            // 
            // dataLossImminentToolStripMenuItem
            // 
            this.dataLossImminentToolStripMenuItem.Checked = true;
            this.dataLossImminentToolStripMenuItem.CheckOnClick = true;
            this.dataLossImminentToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dataLossImminentToolStripMenuItem.Image = global::XenAdmin.Properties.Resources._000_PiiYes_h32bit_16;
            this.dataLossImminentToolStripMenuItem.Name = "dataLossImminentToolStripMenuItem";
            resources.ApplyResources(this.dataLossImminentToolStripMenuItem, "dataLossImminentToolStripMenuItem");
            // 
            // serviceLossImminentToolStripMenuItem
            // 
            this.serviceLossImminentToolStripMenuItem.Checked = true;
            this.serviceLossImminentToolStripMenuItem.CheckOnClick = true;
            this.serviceLossImminentToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.serviceLossImminentToolStripMenuItem.Image = global::XenAdmin.Properties.Resources._000_PiiMaybe_h32bit_16;
            this.serviceLossImminentToolStripMenuItem.Name = "serviceLossImminentToolStripMenuItem";
            resources.ApplyResources(this.serviceLossImminentToolStripMenuItem, "serviceLossImminentToolStripMenuItem");
            // 
            // serviceDegradedToolStripMenuItem
            // 
            this.serviceDegradedToolStripMenuItem.Checked = true;
            this.serviceDegradedToolStripMenuItem.CheckOnClick = true;
            this.serviceDegradedToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.serviceDegradedToolStripMenuItem.Image = global::XenAdmin.Properties.Resources._000_PiiCustomised_h32bit_16;
            this.serviceDegradedToolStripMenuItem.Name = "serviceDegradedToolStripMenuItem";
            resources.ApplyResources(this.serviceDegradedToolStripMenuItem, "serviceDegradedToolStripMenuItem");
            // 
            // serviceRecoveredToolStripMenuItem
            // 
            this.serviceRecoveredToolStripMenuItem.Checked = true;
            this.serviceRecoveredToolStripMenuItem.CheckOnClick = true;
            this.serviceRecoveredToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.serviceRecoveredToolStripMenuItem.Image = global::XenAdmin.Properties.Resources._000_PiiNo_h32bit_16;
            this.serviceRecoveredToolStripMenuItem.Name = "serviceRecoveredToolStripMenuItem";
            resources.ApplyResources(this.serviceRecoveredToolStripMenuItem, "serviceRecoveredToolStripMenuItem");
            // 
            // informationalToolStripMenuItem
            // 
            this.informationalToolStripMenuItem.Checked = true;
            this.informationalToolStripMenuItem.CheckOnClick = true;
            this.informationalToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.informationalToolStripMenuItem.Image = global::XenAdmin.Properties.Resources._000_Severity5_h32bit_16;
            this.informationalToolStripMenuItem.Name = "informationalToolStripMenuItem";
            resources.ApplyResources(this.informationalToolStripMenuItem, "informationalToolStripMenuItem");
            // 
            // unknownToolStripMenuItem
            // 
            this.unknownToolStripMenuItem.Checked = true;
            this.unknownToolStripMenuItem.CheckOnClick = true;
            this.unknownToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.unknownToolStripMenuItem.Name = "unknownToolStripMenuItem";
            resources.ApplyResources(this.unknownToolStripMenuItem, "unknownToolStripMenuItem");
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.AutoToolTip = false;
            resources.ApplyResources(this.toolStripButtonRefresh, "toolStripButtonRefresh");
            this.toolStripButtonRefresh.Image = global::XenAdmin.Properties.Resources.Refresh16;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonExportAll
            // 
            this.toolStripButtonExportAll.AutoToolTip = false;
            resources.ApplyResources(this.toolStripButtonExportAll, "toolStripButtonExportAll");
            this.toolStripButtonExportAll.Image = global::XenAdmin.Properties.Resources._000_ExportMessages_h32bit_16;
            this.toolStripButtonExportAll.Name = "toolStripButtonExportAll";
            this.toolStripButtonExportAll.Click += new System.EventHandler(this.toolStripButtonExportAll_Click);
            // 
            // toolStripButtonDismissAll
            // 
            this.toolStripButtonDismissAll.AutoToolTip = false;
            resources.ApplyResources(this.toolStripButtonDismissAll, "toolStripButtonDismissAll");
            this.toolStripButtonDismissAll.Image = global::XenAdmin.Properties.Resources._000_DeleteAllMessages_h32bit_16;
            this.toolStripButtonDismissAll.Name = "toolStripButtonDismissAll";
            this.toolStripButtonDismissAll.Click += new System.EventHandler(this.DismissAllButton_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel2.Controls.Add(this.toolStrip2);
            this.panel2.Name = "panel2";
            // 
            // toolStrip2
            // 
            resources.ApplyResources(this.toolStrip2, "toolStrip2");
            this.toolStrip2.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripButtonFix,
            this.toolStripButtonHelp,
            this.toolStripButtonDismiss});
            this.toolStrip2.Name = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            resources.ApplyResources(this.toolStripLabel1, "toolStripLabel1");
            this.toolStripLabel1.Margin = new System.Windows.Forms.Padding(2, 1, 4, 2);
            this.toolStripLabel1.Name = "toolStripLabel1";
            // 
            // toolStripButtonFix
            // 
            this.toolStripButtonFix.AutoToolTip = false;
            this.toolStripButtonFix.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonFix, "toolStripButtonFix");
            this.toolStripButtonFix.Name = "toolStripButtonFix";
            this.toolStripButtonFix.Click += new System.EventHandler(this.ButtonFix_Click);
            // 
            // toolStripButtonHelp
            // 
            this.toolStripButtonHelp.AutoToolTip = false;
            this.toolStripButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonHelp, "toolStripButtonHelp");
            this.toolStripButtonHelp.Image = global::XenAdmin.Properties.Resources._000_DeleteAllMessages_h32bit_16;
            this.toolStripButtonHelp.Name = "toolStripButtonHelp";
            this.toolStripButtonHelp.Click += new System.EventHandler(this.ButtonHelp_Click);
            // 
            // toolStripButtonDismiss
            // 
            this.toolStripButtonDismiss.AutoToolTip = false;
            resources.ApplyResources(this.toolStripButtonDismiss, "toolStripButtonDismiss");
            this.toolStripButtonDismiss.Image = global::XenAdmin.Properties.Resources._000_DeleteMessage_h32bit_16;
            this.toolStripButtonDismiss.Name = "toolStripButtonDismiss";
            this.toolStripButtonDismiss.Click += new System.EventHandler(this.ButtonDismiss_Click);
            // 
            // AlertSummaryDialog
            // 
            this.AcceptButton = this.ButtonClose;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.ButtonClose;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.GridViewAlerts);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ButtonClose);
            this.Controls.Add(this.LabelCappingEntries);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "AlertSummaryDialog";
            this.Load += new System.EventHandler(this.AlertSummaryDialog_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AlertSummaryDialog_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewAlerts)).EndInit();
            this.ContextMenuAlertGridView.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonClose;
        private System.Windows.Forms.PictureBox pictureBox1;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx GridViewAlerts;
        private System.Windows.Forms.Label LabelCappingEntries;
        private System.Windows.Forms.Label LabelDialogBlurb;
        private System.Windows.Forms.ContextMenuStrip ContextMenuAlertGridView;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemFix;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemHelp;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemDismiss;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panel1;
        private XenAdmin.Controls.FilterDatesToolStripDropDownButton toolStripDropDownButtonDateFilter;
        private System.Windows.Forms.ToolStripButton toolStripButtonDismissAll;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonDismiss;
        private System.Windows.Forms.ToolStripButton toolStripButtonHelp;
        private System.Windows.Forms.ToolStripButton toolStripButtonFix;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonServerFilter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownSeveritiesFilter;
        private System.Windows.Forms.ToolStripMenuItem dataLossImminentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serviceLossImminentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serviceDegradedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serviceRecoveredToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem informationalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unknownToolStripMenuItem;
        private System.Windows.Forms.DataGridViewImageColumn ColumnExpand;
        private System.Windows.Forms.DataGridViewImageColumn ColumnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnAppliesTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDetails;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDate;
    }
}