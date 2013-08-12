namespace XenAdmin.TabPages
{
    partial class HistoryPage
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
            ConnectionsManager.XenConnections.CollectionChanged -= History_CollectionChanged;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripTop = new System.Windows.Forms.ToolStrip();
            this.toolStripSplitButtonDismiss = new System.Windows.Forms.ToolStripSplitButton();
            this.tsmiDismissAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDismissSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnExpander = new System.Windows.Forms.DataGridViewImageColumn();
            this.columnStatus = new System.Windows.Forms.DataGridViewImageColumn();
            this.columnMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnActions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStripTop.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripTop
            // 
            resources.ApplyResources(this.toolStripTop, "toolStripTop");
            this.toolStripTop.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toolStripTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButtonDismiss});
            this.toolStripTop.Name = "toolStripTop";
            // 
            // toolStripSplitButtonDismiss
            // 
            this.toolStripSplitButtonDismiss.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButtonDismiss.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDismissAll,
            this.tsmiDismissSelected});
            resources.ApplyResources(this.toolStripSplitButtonDismiss, "toolStripSplitButtonDismiss");
            this.toolStripSplitButtonDismiss.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
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
            this.tsmiDismissSelected.Click += new System.EventHandler(this.tsmiDismissSelected_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.Gainsboro;
            this.panel1.Controls.Add(this.dataGridView);
            this.panel1.Name = "panel1";
            // 
            // dataGridView
            // 
            this.dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnExpander,
            this.columnStatus,
            this.columnMessage,
            this.columnLocation,
            this.columnDate,
            this.columnActions});
            resources.ApplyResources(this.dataGridView, "dataGridView");
            this.dataGridView.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // columnExpander
            // 
            this.columnExpander.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle1.NullValue")));
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.columnExpander.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.columnExpander, "columnExpander");
            this.columnExpander.Name = "columnExpander";
            // 
            // columnStatus
            // 
            this.columnStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle2.NullValue")));
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.columnStatus.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.columnStatus, "columnStatus");
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // columnMessage
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.columnMessage.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.columnMessage, "columnMessage");
            this.columnMessage.Name = "columnMessage";
            // 
            // columnLocation
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.columnLocation.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.columnLocation, "columnLocation");
            this.columnLocation.Name = "columnLocation";
            // 
            // columnDate
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.columnDate.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.columnDate, "columnDate");
            this.columnDate.Name = "columnDate";
            // 
            // columnActions
            // 
            this.columnActions.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnActions, "columnActions");
            this.columnActions.Name = "columnActions";
            this.columnActions.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // HistoryPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.toolStripTop);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "HistoryPage";
            this.toolStripTop.ResumeLayout(false);
            this.toolStripTop.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStrip toolStripTop;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonDismiss;
        private System.Windows.Forms.ToolStripMenuItem tsmiDismissAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiDismissSelected;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridViewImageColumn columnExpander;
        private System.Windows.Forms.DataGridViewImageColumn columnStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnActions;
    }
}
