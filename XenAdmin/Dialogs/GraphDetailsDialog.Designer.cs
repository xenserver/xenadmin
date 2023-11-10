namespace XenAdmin.Dialogs
{
    partial class GraphDetailsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphDetailsDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.GraphNameLabel = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.GraphNameTextBox = new System.Windows.Forms.TextBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxProgress = new System.Windows.Forms.PictureBox();
            this.labelProgress = new System.Windows.Forms.Label();
            this.searchTextBox = new XenAdmin.Controls.SearchTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dropDownButtonShow = new XenAdmin.Controls.DropDownButton();
            this.contextMenuStripShow = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemHidden = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDisabled = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.buttonEnable = new System.Windows.Forms.Button();
            this.ColumnDisplayOnGraph = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnEnabled = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnColour = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuStripShow.SuspendLayout();
            this.SuspendLayout();
            // 
            // GraphNameLabel
            // 
            resources.ApplyResources(this.GraphNameLabel, "GraphNameLabel");
            this.GraphNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.GraphNameLabel.Name = "GraphNameLabel";
            // 
            // SaveButton
            // 
            resources.ApplyResources(this.SaveButton, "SaveButton");
            this.SaveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // GraphNameTextBox
            // 
            resources.ApplyResources(this.GraphNameTextBox, "GraphNameTextBox");
            this.GraphNameTextBox.Name = "GraphNameTextBox";
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnDisplayOnGraph,
            this.ColumnName,
            this.Description,
            this.ColumnType,
            this.ColumnEnabled,
            this.ColumnColour});
            resources.ApplyResources(this.dataGridView, "dataGridView");
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.datasourcesGridView_CellClick);
            this.dataGridView.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.datasourcesGridView_CellPainting);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridView_SortCompare);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Window;
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxProgress, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelProgress, 1, 0);
            this.tableLayoutPanel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // pictureBoxProgress
            // 
            resources.ApplyResources(this.pictureBoxProgress, "pictureBoxProgress");
            this.pictureBoxProgress.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBoxProgress.Image = global::XenAdmin.Properties.Resources.ajax_loader;
            this.pictureBoxProgress.Name = "pictureBoxProgress";
            this.pictureBoxProgress.TabStop = false;
            // 
            // labelProgress
            // 
            resources.ApplyResources(this.labelProgress, "labelProgress");
            this.labelProgress.BackColor = System.Drawing.SystemColors.Window;
            this.labelProgress.Name = "labelProgress";
            // 
            // searchTextBox
            // 
            resources.ApplyResources(this.searchTextBox, "searchTextBox");
            this.tableLayoutPanel2.SetColumnSpan(this.searchTextBox, 6);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel2.SetColumnSpan(this.label1, 6);
            this.label1.Name = "label1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.searchTextBox, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.dropDownButtonShow, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.buttonClearAll, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.buttonEnable, 2, 4);
            this.tableLayoutPanel2.Controls.Add(this.SaveButton, 4, 5);
            this.tableLayoutPanel2.Controls.Add(this.CloseButton, 5, 5);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel3, 6);
            this.tableLayoutPanel3.Controls.Add(this.GraphNameLabel, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.GraphNameTextBox, 1, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // panel1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.panel1, 6);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Controls.Add(this.dataGridView);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // dropDownButtonShow
            // 
            this.dropDownButtonShow.ContextMenuStrip = this.contextMenuStripShow;
            resources.ApplyResources(this.dropDownButtonShow, "dropDownButtonShow");
            this.dropDownButtonShow.Name = "dropDownButtonShow";
            this.dropDownButtonShow.UseVisualStyleBackColor = true;
            // 
            // contextMenuStripShow
            // 
            this.contextMenuStripShow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemHidden,
            this.toolStripMenuItemDisabled});
            this.contextMenuStripShow.Name = "contextMenuStripShow";
            resources.ApplyResources(this.contextMenuStripShow, "contextMenuStripShow");
            // 
            // toolStripMenuItemHidden
            // 
            this.toolStripMenuItemHidden.CheckOnClick = true;
            this.toolStripMenuItemHidden.Name = "toolStripMenuItemHidden";
            resources.ApplyResources(this.toolStripMenuItemHidden, "toolStripMenuItemHidden");
            this.toolStripMenuItemHidden.CheckStateChanged += new System.EventHandler(this.toolStripMenuItemHidden_CheckedChanged);
            // 
            // toolStripMenuItemDisabled
            // 
            this.toolStripMenuItemDisabled.CheckOnClick = true;
            this.toolStripMenuItemDisabled.Name = "toolStripMenuItemDisabled";
            resources.ApplyResources(this.toolStripMenuItemDisabled, "toolStripMenuItemDisabled");
            this.toolStripMenuItemDisabled.CheckStateChanged += new System.EventHandler(this.toolStripMenuItemDisabled_CheckedChanged);
            // 
            // buttonClearAll
            // 
            resources.ApplyResources(this.buttonClearAll, "buttonClearAll");
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.UseVisualStyleBackColor = true;
            this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
            // 
            // buttonEnable
            // 
            resources.ApplyResources(this.buttonEnable, "buttonEnable");
            this.buttonEnable.Name = "buttonEnable";
            this.buttonEnable.UseVisualStyleBackColor = true;
            this.buttonEnable.Click += new System.EventHandler(this.buttonEnable_Click);
            // 
            // ColumnDisplayOnGraph
            // 
            this.ColumnDisplayOnGraph.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = false;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            this.ColumnDisplayOnGraph.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.ColumnDisplayOnGraph, "ColumnDisplayOnGraph");
            this.ColumnDisplayOnGraph.Name = "ColumnDisplayOnGraph";
            this.ColumnDisplayOnGraph.ReadOnly = true;
            this.ColumnDisplayOnGraph.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnDisplayOnGraph.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ColumnName
            // 
            resources.ApplyResources(this.ColumnName, "ColumnName");
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            // 
            // Description
            // 
            resources.ApplyResources(this.Description, "Description");
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            // 
            // ColumnType
            // 
            this.ColumnType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnType, "ColumnType");
            this.ColumnType.Name = "ColumnType";
            this.ColumnType.ReadOnly = true;
            // 
            // ColumnEnabled
            // 
            this.ColumnEnabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnEnabled, "ColumnEnabled");
            this.ColumnEnabled.Name = "ColumnEnabled";
            this.ColumnEnabled.ReadOnly = true;
            // 
            // ColumnColour
            // 
            this.ColumnColour.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnColour, "ColumnColour");
            this.ColumnColour.Name = "ColumnColour";
            this.ColumnColour.ReadOnly = true;
            this.ColumnColour.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnColour.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // GraphDetailsDialog
            // 
            this.AcceptButton = this.SaveButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.CloseButton;
            this.Controls.Add(this.tableLayoutPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "GraphDetailsDialog";
            this.Load += new System.EventHandler(this.GraphDetailsDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStripShow.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label GraphNameLabel;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.TextBox GraphNameTextBox;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.PictureBox pictureBoxProgress;
        private Controls.SearchTextBox searchTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemHidden;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDisabled;
        private System.Windows.Forms.Button buttonEnable;
        private Controls.DropDownButton dropDownButtonShow;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripShow;
        private System.Windows.Forms.Button buttonClearAll;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnDisplayOnGraph;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnColour;
    }
}
