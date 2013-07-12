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
            this.ContextMenuAlertGridView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemFix = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemDismiss = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LabelCappingEntries = new System.Windows.Forms.Label();
            this.LabelDialogBlurb = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new XenAdmin.Controls.ToolStripEx();
            this.toolStripDropDownSeveritiesFilter = new XenAdmin.Controls.FilterSeveritiesToolStripDropDownButton();
            this.toolStripDropDownButtonServerFilter = new XenAdmin.Controls.FilterLocationToolStripDropDownButton();
            this.toolStripDropDownButtonDateFilter = new XenAdmin.Controls.FilterDatesToolStripDropDownButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonExportAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSplitButtonDismiss = new System.Windows.Forms.ToolStripSplitButton();
            this.tsmiDismissAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDismissSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLabelFiltersOnOff = new System.Windows.Forms.ToolStripLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButtonFix = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonHelp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDismiss = new System.Windows.Forms.ToolStripButton();
            this.ColumnExpand = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnSeverity = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.ColumnSeverity,
            this.ColumnMessage,
            this.ColumnLocation,
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
            this.toolStrip1.ClickThrough = true;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownSeveritiesFilter,
            this.toolStripDropDownButtonServerFilter,
            this.toolStripDropDownButtonDateFilter,
            this.toolStripSeparator3,
            this.toolStripButtonRefresh,
            this.toolStripSeparator1,
            this.toolStripButtonExportAll,
            this.toolStripSplitButtonDismiss,
            this.toolStripLabelFiltersOnOff});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripDropDownSeveritiesFilter
            // 
            resources.ApplyResources(this.toolStripDropDownSeveritiesFilter, "toolStripDropDownSeveritiesFilter");
            this.toolStripDropDownSeveritiesFilter.Name = "toolStripDropDownSeveritiesFilter";
            // 
            // toolStripDropDownButtonServerFilter
            // 
            this.toolStripDropDownButtonServerFilter.AutoToolTip = false;
            resources.ApplyResources(this.toolStripDropDownButtonServerFilter, "toolStripDropDownButtonServerFilter");
            this.toolStripDropDownButtonServerFilter.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.toolStripDropDownButtonServerFilter.Name = "toolStripDropDownButtonServerFilter";
            // 
            // toolStripDropDownButtonDateFilter
            // 
            this.toolStripDropDownButtonDateFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripDropDownButtonDateFilter, "toolStripDropDownButtonDateFilter");
            this.toolStripDropDownButtonDateFilter.Name = "toolStripDropDownButtonDateFilter";
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
            this.toolStripButtonExportAll.Name = "toolStripButtonExportAll";
            this.toolStripButtonExportAll.Click += new System.EventHandler(this.toolStripButtonExportAll_Click);
            // 
            // toolStripSplitButtonDismiss
            // 
            this.toolStripSplitButtonDismiss.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButtonDismiss.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDismissAll,
            this.tsmiDismissSelected});
            resources.ApplyResources(this.toolStripSplitButtonDismiss, "toolStripSplitButtonDismiss");
            this.toolStripSplitButtonDismiss.Name = "toolStripSplitButtonDismiss";
            this.toolStripSplitButtonDismiss.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripSplitButtonDismiss_DropDownItemClicked);
            // 
            // tsmiDismissAll
            // 
            this.tsmiDismissAll.Name = "tsmiDismissAll";
            resources.ApplyResources(this.tsmiDismissAll, "tsmiDismissAll");
            this.tsmiDismissAll.Click += new System.EventHandler(this.tsmiDismissAll_Click);
            // 
            // tsmiDismissSelected
            // 
            this.tsmiDismissSelected.Name = "tsmiDismissSelected";
            resources.ApplyResources(this.tsmiDismissSelected, "tsmiDismissSelected");
            this.tsmiDismissSelected.Click += new System.EventHandler(this.ButtonDismiss_Click);
            // 
            // toolStripLabelFiltersOnOff
            // 
            this.toolStripLabelFiltersOnOff.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.toolStripLabelFiltersOnOff, "toolStripLabelFiltersOnOff");
            this.toolStripLabelFiltersOnOff.Name = "toolStripLabelFiltersOnOff";
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
            this.toolStripButtonDismiss.Name = "toolStripButtonDismiss";
            this.toolStripButtonDismiss.Click += new System.EventHandler(this.ButtonDismiss_Click);
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
            // ColumnSeverity
            // 
            this.ColumnSeverity.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.ColumnSeverity.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColumnSeverity.FillWeight = 109.5524F;
            resources.ApplyResources(this.ColumnSeverity, "ColumnSeverity");
            this.ColumnSeverity.Name = "ColumnSeverity";
            this.ColumnSeverity.ReadOnly = true;
            this.ColumnSeverity.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnSeverity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ColumnMessage
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnMessage.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColumnMessage.FillWeight = 380F;
            resources.ApplyResources(this.ColumnMessage, "ColumnMessage");
            this.ColumnMessage.Name = "ColumnMessage";
            this.ColumnMessage.ReadOnly = true;
            // 
            // ColumnLocation
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnLocation.DefaultCellStyle = dataGridViewCellStyle5;
            this.ColumnLocation.FillWeight = 109.5524F;
            resources.ApplyResources(this.ColumnLocation, "ColumnLocation");
            this.ColumnLocation.Name = "ColumnLocation";
            this.ColumnLocation.ReadOnly = true;
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
        private XenAdmin.Controls.ToolStripEx toolStrip1;
        private System.Windows.Forms.Panel panel1;
        private XenAdmin.Controls.FilterDatesToolStripDropDownButton toolStripDropDownButtonDateFilter;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonDismiss;
        private System.Windows.Forms.ToolStripButton toolStripButtonHelp;
        private System.Windows.Forms.ToolStripButton toolStripButtonFix;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private XenAdmin.Controls.FilterLocationToolStripDropDownButton toolStripDropDownButtonServerFilter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private XenAdmin.Controls.FilterSeveritiesToolStripDropDownButton toolStripDropDownSeveritiesFilter;
        private System.Windows.Forms.ToolStripLabel toolStripLabelFiltersOnOff;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonDismiss;
        private System.Windows.Forms.ToolStripMenuItem tsmiDismissAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiDismissSelected;
        private System.Windows.Forms.DataGridViewImageColumn ColumnExpand;
        private System.Windows.Forms.DataGridViewImageColumn ColumnSeverity;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDate;
    }
}