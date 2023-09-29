namespace XenAdmin.Wizards.BugToolWizard
{
    partial class BugToolPageSelectCapabilities
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BugToolPageSelectCapabilities));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewItems = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.SelectButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.DescriptionValue = new System.Windows.Forms.Label();
            this.SizeLabel = new System.Windows.Forms.Label();
            this.SizeValue = new System.Windows.Forms.Label();
            this.TotalSizeLabel = new System.Windows.Forms.Label();
            this.TotalSizeValue = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewItems)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewItems, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.SelectButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.ClearButton, 2, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // dataGridViewItems
            // 
            this.dataGridViewItems.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewItems.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnCheck,
            this.columnItem,
            this.columnImage});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridViewItems, 3);
            resources.ApplyResources(this.dataGridViewItems, "dataGridViewItems");
            this.dataGridViewItems.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridViewItems.Name = "dataGridViewItems";
            this.dataGridViewItems.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewItems_CellClick);
            this.dataGridViewItems.SelectionChanged += new System.EventHandler(this.dataGridViewItems_SelectionChanged);
            this.dataGridViewItems.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridViewItems_SortCompare);
            // 
            // columnCheck
            // 
            this.columnCheck.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnCheck, "columnCheck");
            this.columnCheck.Name = "columnCheck";
            this.columnCheck.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.columnCheck.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // columnItem
            // 
            resources.ApplyResources(this.columnItem, "columnItem");
            this.columnItem.Name = "columnItem";
            // 
            // columnImage
            // 
            resources.ApplyResources(this.columnImage, "columnImage");
            this.columnImage.Name = "columnImage";
            this.columnImage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // SelectButton
            // 
            resources.ApplyResources(this.SelectButton, "SelectButton");
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // ClearButton
            // 
            resources.ApplyResources(this.ClearButton, "ClearButton");
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.DescriptionLabel, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.DescriptionValue, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.SizeLabel, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.SizeValue, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.TotalSizeLabel, 0, 4);
            this.tableLayoutPanel4.Controls.Add(this.TotalSizeValue, 1, 4);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // DescriptionLabel
            // 
            resources.ApplyResources(this.DescriptionLabel, "DescriptionLabel");
            this.DescriptionLabel.Name = "DescriptionLabel";
            // 
            // DescriptionValue
            // 
            resources.ApplyResources(this.DescriptionValue, "DescriptionValue");
            this.tableLayoutPanel4.SetColumnSpan(this.DescriptionValue, 2);
            this.DescriptionValue.Name = "DescriptionValue";
            // 
            // SizeLabel
            // 
            resources.ApplyResources(this.SizeLabel, "SizeLabel");
            this.SizeLabel.Name = "SizeLabel";
            // 
            // SizeValue
            // 
            resources.ApplyResources(this.SizeValue, "SizeValue");
            this.tableLayoutPanel4.SetColumnSpan(this.SizeValue, 2);
            this.SizeValue.Name = "SizeValue";
            // 
            // TotalSizeLabel
            // 
            resources.ApplyResources(this.TotalSizeLabel, "TotalSizeLabel");
            this.TotalSizeLabel.Name = "TotalSizeLabel";
            // 
            // TotalSizeValue
            // 
            resources.ApplyResources(this.TotalSizeValue, "TotalSizeValue");
            this.TotalSizeValue.Name = "TotalSizeValue";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 1, 1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel3.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // BugToolPageSelectCapabilities
            // 
            this.Controls.Add(this.tableLayoutPanel3);
            this.DoubleBuffered = true;
            this.Name = "BugToolPageSelectCapabilities";
            resources.ApplyResources(this, "$this");
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewItems)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.Label SizeLabel;
        private System.Windows.Forms.Label DescriptionValue;
        private System.Windows.Forms.Label SizeValue;
        private System.Windows.Forms.Label TotalSizeLabel;
        private System.Windows.Forms.Label TotalSizeValue;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewItems;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnItem;
        private System.Windows.Forms.DataGridViewImageColumn columnImage;
        private System.Windows.Forms.Label label1;
    }
}
