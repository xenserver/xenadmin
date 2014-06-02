namespace XenAdmin.Wizards.NewVMWizard
{
    partial class Page_Template
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_Template));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.TemplatesGridView = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.DescriptionBox = new System.Windows.Forms.GroupBox();
            this.checkBoxCopyBiosStrings = new System.Windows.Forms.CheckBox();
            this.searchTextBox1 = new XenAdmin.Controls.SearchTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.TemplatesGridView)).BeginInit();
            this.panel1.SuspendLayout();
            this.DescriptionBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // TemplatesGridView
            // 
            resources.ApplyResources(this.TemplatesGridView, "TemplatesGridView");
            this.TemplatesGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.TemplatesGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.TemplatesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.TemplatesGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImageColumn,
            this.NameColumn,
            this.TypeColumn});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.TemplatesGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.TemplatesGridView.Name = "TemplatesGridView";
            this.TemplatesGridView.SelectionChanged += new System.EventHandler(this.TemplatesGridView_SelectionChanged);
            // 
            // ImageColumn
            // 
            this.ImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ImageColumn, "ImageColumn");
            this.ImageColumn.Name = "ImageColumn";
            this.ImageColumn.ReadOnly = true;
            this.ImageColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // NameColumn
            // 
            this.NameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.NameColumn, "NameColumn");
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            // 
            // TypeColumn
            // 
            this.TypeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.TypeColumn, "TypeColumn");
            this.TypeColumn.Name = "TypeColumn";
            this.TypeColumn.ReadOnly = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.DescriptionLabel);
            this.panel1.Name = "panel1";
            // 
            // DescriptionLabel
            // 
            resources.ApplyResources(this.DescriptionLabel, "DescriptionLabel");
            this.DescriptionLabel.Name = "DescriptionLabel";
            // 
            // DescriptionBox
            // 
            resources.ApplyResources(this.DescriptionBox, "DescriptionBox");
            this.DescriptionBox.Controls.Add(this.panel1);
            this.DescriptionBox.Name = "DescriptionBox";
            this.DescriptionBox.TabStop = false;
            // 
            // checkBoxCopyBiosStrings
            // 
            resources.ApplyResources(this.checkBoxCopyBiosStrings, "checkBoxCopyBiosStrings");
            this.checkBoxCopyBiosStrings.Name = "checkBoxCopyBiosStrings";
            this.checkBoxCopyBiosStrings.UseVisualStyleBackColor = true;
            // 
            // searchTextBox1
            // 
            resources.ApplyResources(this.searchTextBox1, "searchTextBox1");
            this.searchTextBox1.Name = "searchTextBox1";
            this.searchTextBox1.TextChanged += new System.EventHandler(this.searchTextBox1_TextChanged);
            // 
            // Page_Template
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.searchTextBox1);
            this.Controls.Add(this.checkBoxCopyBiosStrings);
            this.Controls.Add(this.TemplatesGridView);
            this.Controls.Add(this.DescriptionBox);
            this.Name = "Page_Template";
            ((System.ComponentModel.ISupportInitialize)(this.TemplatesGridView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.DescriptionBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx TemplatesGridView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.GroupBox DescriptionBox;
        private System.Windows.Forms.DataGridViewImageColumn ImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeColumn;
        private System.Windows.Forms.CheckBox checkBoxCopyBiosStrings;
        private XenAdmin.Controls.SearchTextBox searchTextBox1;
    }
}
