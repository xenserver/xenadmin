namespace XenAdmin.TabPages
{
    partial class ManageUpdatesPage
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
            if (disposing)
            {
                XenAdmin.Core.Updates.DeregisterCollectionChanged(UpdatesCollectionChanged);
                XenAdmin.Core.Updates.RestoreDismissedUpdatesStarted -= Updates_RestoreDismissedUpdatesStarted;
                XenAdmin.Core.Updates.CheckForUpdatesStarted -= CheckForUpdates_CheckForUpdatesStarted;
                XenAdmin.Core.Updates.CheckForUpdatesCompleted -= CheckForUpdates_CheckForUpdatesCompleted;

                if (components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageUpdatesPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.informationLabelIcon = new System.Windows.Forms.PictureBox();
            this.informationLabel = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.AutoCheckForUpdatesDisabledLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkForUpdatesNowButton2 = new System.Windows.Forms.LinkLabel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.checkForUpdatesNowButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.labelProgress = new System.Windows.Forms.Label();
            this.pictureBoxProgress = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new XenAdmin.Controls.ToolStripEx();
            this.toolStripDropDownButtonServerFilter = new XenAdmin.Controls.FilterLocationToolStripDropDownButton();
            this.toolStripDropDownButtonDateFilter = new XenAdmin.Controls.FilterDatesToolStripDropDownButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonExportAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelFiltersOnOff = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSplitButtonDismiss = new System.Windows.Forms.ToolStripSplitButton();
            this.dismissAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dismissSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonRestoreDismissed = new System.Windows.Forms.ToolStripButton();
            this.dataGridViewUpdates = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnExpand = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnWebPage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.informationLabelIcon)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).BeginInit();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUpdates)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.informationLabelIcon, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.informationLabel, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // informationLabelIcon
            // 
            resources.ApplyResources(this.informationLabelIcon, "informationLabelIcon");
            this.informationLabelIcon.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.informationLabelIcon.InitialImage = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.informationLabelIcon.Name = "informationLabelIcon";
            this.informationLabelIcon.TabStop = false;
            // 
            // informationLabel
            // 
            resources.ApplyResources(this.informationLabel, "informationLabel");
            this.informationLabel.Name = "informationLabel";
            this.informationLabel.TabStop = true;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel2.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.AutoCheckForUpdatesDisabledLabel, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.checkForUpdatesNowButton2, 2, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // AutoCheckForUpdatesDisabledLabel
            // 
            resources.ApplyResources(this.AutoCheckForUpdatesDisabledLabel, "AutoCheckForUpdatesDisabledLabel");
            this.AutoCheckForUpdatesDisabledLabel.Name = "AutoCheckForUpdatesDisabledLabel";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // checkForUpdatesNowButton2
            // 
            resources.ApplyResources(this.checkForUpdatesNowButton2, "checkForUpdatesNowButton2");
            this.checkForUpdatesNowButton2.LinkColor = System.Drawing.Color.Black;
            this.checkForUpdatesNowButton2.Name = "checkForUpdatesNowButton2";
            this.checkForUpdatesNowButton2.TabStop = true;
            this.checkForUpdatesNowButton2.Click += new System.EventHandler(this.checkForUpdatesNowButton2_Click);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel3.Controls.Add(this.checkForUpdatesNowButton, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.Resize += new System.EventHandler(this.tableLayoutPanel3_Resize);
            // 
            // checkForUpdatesNowButton
            // 
            resources.ApplyResources(this.checkForUpdatesNowButton, "checkForUpdatesNowButton");
            this.checkForUpdatesNowButton.Name = "checkForUpdatesNowButton";
            this.checkForUpdatesNowButton.UseVisualStyleBackColor = true;
            this.checkForUpdatesNowButton.Click += new System.EventHandler(this.checkForUpdatesNowButton_Click);
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.labelProgress, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.pictureBoxProgress, 0, 0);
            this.tableLayoutPanel4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // labelProgress
            // 
            resources.ApplyResources(this.labelProgress, "labelProgress");
            this.labelProgress.Name = "labelProgress";
            // 
            // pictureBoxProgress
            // 
            resources.ApplyResources(this.pictureBoxProgress, "pictureBoxProgress");
            this.pictureBoxProgress.Name = "pictureBoxProgress";
            this.pictureBoxProgress.TabStop = false;
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toolStrip1.ClickThrough = true;
            this.tableLayoutPanel2.SetColumnSpan(this.toolStrip1, 3);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButtonServerFilter,
            this.toolStripDropDownButtonDateFilter,
            this.toolStripSeparator3,
            this.toolStripButtonRefresh,
            this.toolStripSeparator1,
            this.toolStripButtonExportAll,
            this.toolStripLabelFiltersOnOff,
            this.toolStripSplitButtonDismiss,
            this.toolStripButtonRestoreDismissed});
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.TabStop = true;
            // 
            // toolStripDropDownButtonServerFilter
            // 
            this.toolStripDropDownButtonServerFilter.AutoToolTip = false;
            resources.ApplyResources(this.toolStripDropDownButtonServerFilter, "toolStripDropDownButtonServerFilter");
            this.toolStripDropDownButtonServerFilter.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.toolStripDropDownButtonServerFilter.Name = "toolStripDropDownButtonServerFilter";
            this.toolStripDropDownButtonServerFilter.FilterChanged += new System.Action(this.toolStripDropDownButtonServerFilter_FilterChanged);
            // 
            // toolStripDropDownButtonDateFilter
            // 
            this.toolStripDropDownButtonDateFilter.AutoToolTip = false;
            this.toolStripDropDownButtonDateFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripDropDownButtonDateFilter, "toolStripDropDownButtonDateFilter");
            this.toolStripDropDownButtonDateFilter.Name = "toolStripDropDownButtonDateFilter";
            this.toolStripDropDownButtonDateFilter.FilterChanged += new System.Action(this.toolStripDropDownButtonDateFilter_FilterChanged);
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
            // toolStripLabelFiltersOnOff
            // 
            this.toolStripLabelFiltersOnOff.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.toolStripLabelFiltersOnOff, "toolStripLabelFiltersOnOff");
            this.toolStripLabelFiltersOnOff.Name = "toolStripLabelFiltersOnOff";
            // 
            // toolStripSplitButtonDismiss
            // 
            this.toolStripSplitButtonDismiss.AutoToolTip = false;
            this.toolStripSplitButtonDismiss.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButtonDismiss.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dismissAllToolStripMenuItem,
            this.dismissSelectedToolStripMenuItem});
            resources.ApplyResources(this.toolStripSplitButtonDismiss, "toolStripSplitButtonDismiss");
            this.toolStripSplitButtonDismiss.Name = "toolStripSplitButtonDismiss";
            this.toolStripSplitButtonDismiss.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripSplitButtonDismiss_DropDownItemClicked);
            // 
            // dismissAllToolStripMenuItem
            // 
            this.dismissAllToolStripMenuItem.Name = "dismissAllToolStripMenuItem";
            resources.ApplyResources(this.dismissAllToolStripMenuItem, "dismissAllToolStripMenuItem");
            this.dismissAllToolStripMenuItem.Click += new System.EventHandler(this.dismissAllToolStripMenuItem_Click);
            // 
            // dismissSelectedToolStripMenuItem
            // 
            this.dismissSelectedToolStripMenuItem.Name = "dismissSelectedToolStripMenuItem";
            resources.ApplyResources(this.dismissSelectedToolStripMenuItem, "dismissSelectedToolStripMenuItem");
            this.dismissSelectedToolStripMenuItem.Click += new System.EventHandler(this.dismissSelectedToolStripMenuItem_Click);
            // 
            // toolStripButtonRestoreDismissed
            // 
            this.toolStripButtonRestoreDismissed.AutoToolTip = false;
            this.toolStripButtonRestoreDismissed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonRestoreDismissed, "toolStripButtonRestoreDismissed");
            this.toolStripButtonRestoreDismissed.Name = "toolStripButtonRestoreDismissed";
            this.toolStripButtonRestoreDismissed.Click += new System.EventHandler(this.toolStripButtonRestoreDismissed_Click);
            // 
            // dataGridViewUpdates
            // 
            resources.ApplyResources(this.dataGridViewUpdates, "dataGridViewUpdates");
            this.dataGridViewUpdates.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewUpdates.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewUpdates.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewUpdates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewUpdates.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnExpand,
            this.ColumnMessage,
            this.ColumnLocation,
            this.ColumnDate,
            this.ColumnWebPage});
            this.dataGridViewUpdates.MultiSelect = true;
            this.dataGridViewUpdates.Name = "dataGridViewUpdates";
            this.dataGridViewUpdates.ReadOnly = true;
            this.dataGridViewUpdates.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewUpdates_CellClick);
            this.dataGridViewUpdates.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewUpdates_CellDoubleClick);
            this.dataGridViewUpdates.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewUpdates_ColumnHeaderMouseClick);
            this.dataGridViewUpdates.SelectionChanged += new System.EventHandler(this.dataGridViewUpdates_SelectionChanged);
            this.dataGridViewUpdates.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridViewUpdates_SortCompare);
            this.dataGridViewUpdates.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewUpdates_KeyDown);
            // 
            // ColumnExpand
            // 
            this.ColumnExpand.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle1.NullValue")));
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.ColumnExpand.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.ColumnExpand, "ColumnExpand");
            this.ColumnExpand.Name = "ColumnExpand";
            this.ColumnExpand.ReadOnly = true;
            this.ColumnExpand.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnMessage
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnMessage.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnMessage.FillWeight = 40F;
            resources.ApplyResources(this.ColumnMessage, "ColumnMessage");
            this.ColumnMessage.Name = "ColumnMessage";
            this.ColumnMessage.ReadOnly = true;
            // 
            // ColumnLocation
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnLocation.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColumnLocation.FillWeight = 20F;
            resources.ApplyResources(this.ColumnLocation, "ColumnLocation");
            this.ColumnLocation.Name = "ColumnLocation";
            this.ColumnLocation.ReadOnly = true;
            this.ColumnLocation.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnDate
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnDate.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColumnDate.FillWeight = 20F;
            resources.ApplyResources(this.ColumnDate, "ColumnDate");
            this.ColumnDate.Name = "ColumnDate";
            this.ColumnDate.ReadOnly = true;
            // 
            // ColumnWebPage
            // 
            this.ColumnWebPage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnWebPage, "ColumnWebPage");
            this.ColumnWebPage.Name = "ColumnWebPage";
            this.ColumnWebPage.ReadOnly = true;
            this.ColumnWebPage.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnWebPage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ManageUpdatesPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.dataGridViewUpdates);
            this.Name = "ManageUpdatesPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.informationLabelIcon)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUpdates)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewUpdates;
        private System.Windows.Forms.PictureBox pictureBoxProgress;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox informationLabelIcon;
        private System.Windows.Forms.LinkLabel informationLabel;
        private System.Windows.Forms.Button button2;
        private Controls.ToolStripEx toolStrip1;
        private Controls.FilterLocationToolStripDropDownButton toolStripDropDownButtonServerFilter;
        private Controls.FilterDatesToolStripDropDownButton toolStripDropDownButtonDateFilter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportAll;
        private System.Windows.Forms.ToolStripLabel toolStripLabelFiltersOnOff;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonDismiss;
        private System.Windows.Forms.ToolStripMenuItem dismissAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dismissSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonRestoreDismissed;
        private System.Windows.Forms.Label AutoCheckForUpdatesDisabledLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel checkForUpdatesNowButton2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button checkForUpdatesNowButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.DataGridViewImageColumn ColumnExpand;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnWebPage;
    }
}