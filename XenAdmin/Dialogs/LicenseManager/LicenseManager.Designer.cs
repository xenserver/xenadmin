namespace XenAdmin.Dialogs
{
    partial class LicenseManager
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
                checkableDataGridView.SelectionChanged -= checkableDataGridView_SelectionChanged;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseManager));
            this.checkableDataGridView = new XenAdmin.Controls.LicenseCheckableDataGridView();
            this.checkBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.poolColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.productVersionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.statusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.downloadLicenseServerLink = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.assignLicenceButton = new System.Windows.Forms.Button();
            this.releaseLicenseButton = new System.Windows.Forms.Button();
            this.activateFreeXenServerButton = new XenAdmin.Controls.DropDownButton();
            this.summaryPanel = new XenAdmin.Controls.SummaryPanel.SummaryPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.freeXenServerContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.requestActivationKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applyActivationKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.checkableDataGridView)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.freeXenServerContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkableDataGridView
            // 
            this.checkableDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.checkableDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.checkableDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.checkableDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.checkBoxColumn,
            this.poolColumn,
            this.productVersionColumn,
            this.statusImageColumn,
            this.statusColumn});
            resources.ApplyResources(this.checkableDataGridView, "checkableDataGridView");
            this.checkableDataGridView.Name = "checkableDataGridView";
            // 
            // checkBoxColumn
            // 
            this.checkBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.checkBoxColumn, "checkBoxColumn");
            this.checkBoxColumn.Name = "checkBoxColumn";
            // 
            // poolColumn
            // 
            this.poolColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.poolColumn, "poolColumn");
            this.poolColumn.Name = "poolColumn";
            this.poolColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // productVersionColumn
            // 
            this.productVersionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.productVersionColumn, "productVersionColumn");
            this.productVersionColumn.Name = "productVersionColumn";
            this.productVersionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // statusImageColumn
            // 
            this.statusImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.statusImageColumn, "statusImageColumn");
            this.statusImageColumn.Name = "statusImageColumn";
            // 
            // statusColumn
            // 
            this.statusColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.statusColumn, "statusColumn");
            this.statusColumn.Name = "statusColumn";
            this.statusColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.statusColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.downloadLicenseServerLink, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkableDataGridView, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.summaryPanel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // downloadLicenseServerLink
            // 
            resources.ApplyResources(this.downloadLicenseServerLink, "downloadLicenseServerLink");
            this.downloadLicenseServerLink.Name = "downloadLicenseServerLink";
            this.downloadLicenseServerLink.TabStop = true;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.assignLicenceButton, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.releaseLicenseButton, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.activateFreeXenServerButton, 2, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // assignLicenceButton
            // 
            resources.ApplyResources(this.assignLicenceButton, "assignLicenceButton");
            this.assignLicenceButton.Name = "assignLicenceButton";
            this.assignLicenceButton.UseVisualStyleBackColor = true;
            // 
            // releaseLicenseButton
            // 
            resources.ApplyResources(this.releaseLicenseButton, "releaseLicenseButton");
            this.releaseLicenseButton.Name = "releaseLicenseButton";
            this.releaseLicenseButton.UseVisualStyleBackColor = true;
            // 
            // activateFreeXenServerButton
            // 
            resources.ApplyResources(this.activateFreeXenServerButton, "activateFreeXenServerButton");
            this.activateFreeXenServerButton.Name = "activateFreeXenServerButton";
            this.activateFreeXenServerButton.UseVisualStyleBackColor = true;
            // 
            // summaryPanel
            // 
            this.summaryPanel.BackColor = System.Drawing.SystemColors.Window;
            this.summaryPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.summaryPanel, "summaryPanel");
            this.summaryPanel.Name = "summaryPanel";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // freeXenServerContextMenuStrip
            // 
            this.freeXenServerContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.requestActivationKeyToolStripMenuItem,
            this.applyActivationKeyToolStripMenuItem});
            this.freeXenServerContextMenuStrip.Name = "freeXenServerContextMenuStrip";
            resources.ApplyResources(this.freeXenServerContextMenuStrip, "freeXenServerContextMenuStrip");
            // 
            // requestActivationKeyToolStripMenuItem
            // 
            this.requestActivationKeyToolStripMenuItem.Name = "requestActivationKeyToolStripMenuItem";
            resources.ApplyResources(this.requestActivationKeyToolStripMenuItem, "requestActivationKeyToolStripMenuItem");
            // 
            // applyActivationKeyToolStripMenuItem
            // 
            this.applyActivationKeyToolStripMenuItem.Name = "applyActivationKeyToolStripMenuItem";
            resources.ApplyResources(this.applyActivationKeyToolStripMenuItem, "applyActivationKeyToolStripMenuItem");
            // 
            // LicenseManager
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LicenseManager";
            ((System.ComponentModel.ISupportInitialize)(this.checkableDataGridView)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.freeXenServerContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.LicenseCheckableDataGridView checkableDataGridView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button assignLicenceButton;
        private System.Windows.Forms.Button releaseLicenseButton;
        private XenAdmin.Controls.SummaryPanel.SummaryPanel summaryPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.LinkLabel downloadLicenseServerLink;
        private XenAdmin.Controls.DropDownButton activateFreeXenServerButton;
        private System.Windows.Forms.ContextMenuStrip freeXenServerContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem requestActivationKeyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applyActivationKeyToolStripMenuItem;
        private System.Windows.Forms.DataGridViewCheckBoxColumn checkBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn poolColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn productVersionColumn;
        private System.Windows.Forms.DataGridViewImageColumn statusImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusColumn;
    }
}

