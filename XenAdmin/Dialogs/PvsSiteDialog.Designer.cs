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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PvsSiteDialog));
            this.closeButton = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.gridView = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ipAddressesColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.firstPortColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastPortColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.bottomPanel.SuspendLayout();
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
            this.ipAddressesColumn,
            this.firstPortColumn,
            this.lastPortColumn});
            resources.ApplyResources(this.gridView, "gridView");
            this.gridView.Name = "gridView";
            this.gridView.ReadOnly = true;
            // 
            // ipAddressesColumn
            // 
            this.ipAddressesColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ipAddressesColumn.FillWeight = 105.6863F;
            resources.ApplyResources(this.ipAddressesColumn, "ipAddressesColumn");
            this.ipAddressesColumn.Name = "ipAddressesColumn";
            this.ipAddressesColumn.ReadOnly = true;
            // 
            // firstPortColumn
            // 
            this.firstPortColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.firstPortColumn.FillWeight = 173.6275F;
            resources.ApplyResources(this.firstPortColumn, "firstPortColumn");
            this.firstPortColumn.Name = "firstPortColumn";
            this.firstPortColumn.ReadOnly = true;
            // 
            // lastPortColumn
            // 
            this.lastPortColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.lastPortColumn.FillWeight = 173.6275F;
            resources.ApplyResources(this.lastPortColumn, "lastPortColumn");
            this.lastPortColumn.Name = "lastPortColumn";
            this.lastPortColumn.ReadOnly = true;
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.closeButton);
            resources.ApplyResources(this.bottomPanel, "bottomPanel");
            this.bottomPanel.Name = "bottomPanel";
            // 
            // PvsSiteDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.closeButton;
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.bottomPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "PvsSiteDialog";
            this.ShowInTaskbar = true;
            this.mainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Panel mainPanel;
        private Controls.DataGridViewEx.DataGridViewEx gridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ipAddressesColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn firstPortColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastPortColumn;
    }
}