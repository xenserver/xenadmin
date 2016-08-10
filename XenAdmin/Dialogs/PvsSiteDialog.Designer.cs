namespace XenAdmin.Dialogs
{
    partial class PvsSiteDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PvsSiteDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.closeButton = new System.Windows.Forms.Button();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.gridView = new XenAdmin.Controls.DataGridViewEx.CollapsingPvsSiteServerDataGridView(this.components);
            this.expansionColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.siteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ipAddressesColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.firstPortColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastPortColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bottomPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.closeButton);
            resources.ApplyResources(this.bottomPanel, "bottomPanel");
            this.bottomPanel.Name = "bottomPanel";
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.gridView);
            resources.ApplyResources(this.mainPanel, "mainPanel");
            this.mainPanel.Name = "mainPanel";
            // 
            // gridView
            // 
            this.gridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.expansionColumn,
            this.siteColumn,
            this.ipAddressesColumn,
            this.firstPortColumn,
            this.lastPortColumn});
            resources.ApplyResources(this.gridView, "gridView");
            this.gridView.Name = "gridView";
            this.gridView.ReadOnly = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridView.Updating = false;
            // 
            // expansionColumn
            // 
            this.expansionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.expansionColumn.FillWeight = 16.66667F;
            resources.ApplyResources(this.expansionColumn, "expansionColumn");
            this.expansionColumn.Name = "expansionColumn";
            this.expansionColumn.ReadOnly = true;
            this.expansionColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // siteColumn
            // 
            this.siteColumn.FillWeight = 108.4123F;
            resources.ApplyResources(this.siteColumn, "siteColumn");
            this.siteColumn.Name = "siteColumn";
            this.siteColumn.ReadOnly = true;
            this.siteColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ipAddressesColumn
            // 
            this.ipAddressesColumn.FillWeight = 172.9413F;
            resources.ApplyResources(this.ipAddressesColumn, "ipAddressesColumn");
            this.ipAddressesColumn.Name = "ipAddressesColumn";
            this.ipAddressesColumn.ReadOnly = true;
            this.ipAddressesColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // firstPortColumn
            // 
            this.firstPortColumn.FillWeight = 100.9122F;
            resources.ApplyResources(this.firstPortColumn, "firstPortColumn");
            this.firstPortColumn.Name = "firstPortColumn";
            this.firstPortColumn.ReadOnly = true;
            this.firstPortColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // lastPortColumn
            // 
            this.lastPortColumn.FillWeight = 101.0676F;
            resources.ApplyResources(this.lastPortColumn, "lastPortColumn");
            this.lastPortColumn.Name = "lastPortColumn";
            this.lastPortColumn.ReadOnly = true;
            this.lastPortColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // PvsSiteDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.closeButton;
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.bottomPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.HelpButton = false;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "PvsSiteDialog";
            this.ShowInTaskbar = true;
            this.bottomPanel.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Panel mainPanel;
        private Controls.DataGridViewEx.CollapsingPvsSiteServerDataGridView gridView;
        private System.Windows.Forms.DataGridViewImageColumn expansionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn siteColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ipAddressesColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn firstPortColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastPortColumn;
    }
}