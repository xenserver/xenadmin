namespace XenAdmin.Wizards.NewCPUGroupWizard
{
    partial class NewCPUGroupCPUCoresPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewCPUGroupCPUCoresPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.labelCounterVMs = new System.Windows.Forms.Label();
            this.searchTextBox1 = new XenAdmin.Controls.SearchTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.expansionColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrentCPUGroupColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.buttonClearAll, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonSelectAll, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelCounterVMs, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.searchTextBox1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // buttonClearAll
            // 
            resources.ApplyResources(this.buttonClearAll, "buttonClearAll");
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.UseVisualStyleBackColor = true;
            // 
            // buttonSelectAll
            // 
            resources.ApplyResources(this.buttonSelectAll, "buttonSelectAll");
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            // 
            // labelCounterVMs
            // 
            resources.ApplyResources(this.labelCounterVMs, "labelCounterVMs");
            this.labelCounterVMs.Name = "labelCounterVMs";
            // 
            // searchTextBox1
            // 
            resources.ApplyResources(this.searchTextBox1, "searchTextBox1");
            this.tableLayoutPanel1.SetColumnSpan(this.searchTextBox1, 2);
            this.searchTextBox1.Name = "searchTextBox1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.expansionColumn,
            this.NameColumn,
            this.DescriptionColumn,
            this.CurrentCPUGroupColumn});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridView1, 3);
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.ShowRowErrors = false;
            this.dataGridView1.StandardTab = true;
            // 
            // expansionColumn
            // 
            this.expansionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.expansionColumn.FillWeight = 25.80645F;
            resources.ApplyResources(this.expansionColumn, "expansionColumn");
            this.expansionColumn.Name = "expansionColumn";
            this.expansionColumn.ReadOnly = true;
            this.expansionColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // NameColumn
            // 
            resources.ApplyResources(this.NameColumn, "NameColumn");
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            // 
            // DescriptionColumn
            // 
            resources.ApplyResources(this.DescriptionColumn, "DescriptionColumn");
            this.DescriptionColumn.Name = "DescriptionColumn";
            this.DescriptionColumn.ReadOnly = true;
            // 
            // CurrentCPUGroupColumn
            // 
            resources.ApplyResources(this.CurrentCPUGroupColumn, "CurrentCPUGroupColumn");
            this.CurrentCPUGroupColumn.Name = "CurrentCPUGroupColumn";
            this.CurrentCPUGroupColumn.ReadOnly = true;
            // 
            // NewCPUGroupCPUCoresPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NewCPUGroupCPUCoresPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private Controls.SearchTextBox searchTextBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelCounterVMs;
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.Button buttonClearAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrentCPUGroupColumn;
        private System.Windows.Forms.DataGridViewImageColumn expansionColumn;
    }
}
