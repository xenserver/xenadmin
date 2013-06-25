namespace XenAdmin.Wizards.DRWizards
{
    partial class DRFailoverWizardStoragePage
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DRFailoverWizardStoragePage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelText = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.dataGridViewSRs = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnTick = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnMetadata = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FindSrsButton = new System.Windows.Forms.Button();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.FindSrsOptionsMenuStrip = new XenAdmin.Controls.NonReopeningContextMenuStrip(this.components);
            this.iscsiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSRs)).BeginInit();
            this.FindSrsOptionsMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelText
            // 
            resources.ApplyResources(this.labelText, "labelText");
            this.tableLayoutPanel1.SetColumnSpan(this.labelText, 2);
            this.labelText.Name = "labelText";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.buttonClearAll, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewSRs, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.FindSrsButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelText, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonSelectAll, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // buttonClearAll
            // 
            resources.ApplyResources(this.buttonClearAll, "buttonClearAll");
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.UseVisualStyleBackColor = true;
            this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
            // 
            // dataGridViewSRs
            // 
            this.dataGridViewSRs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewSRs.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewSRs.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewSRs.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridViewSRs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewSRs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnTick,
            this.columnName,
            this.columnDesc,
            this.columnType,
            this.columnMetadata});
            resources.ApplyResources(this.dataGridViewSRs, "dataGridViewSRs");
            this.dataGridViewSRs.Name = "dataGridViewSRs";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewSRs.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridViewSRs.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.tableLayoutPanel1.SetRowSpan(this.dataGridViewSRs, 2);
            this.dataGridViewSRs.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewSRs_CellValueChanged);
            this.dataGridViewSRs.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewSRs_CellClick);
            // 
            // columnTick
            // 
            resources.ApplyResources(this.columnTick, "columnTick");
            this.columnTick.Name = "columnTick";
            // 
            // columnName
            // 
            this.columnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.columnName, "columnName");
            this.columnName.Name = "columnName";
            this.columnName.ReadOnly = true;
            // 
            // columnDesc
            // 
            this.columnDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.columnDesc, "columnDesc");
            this.columnDesc.Name = "columnDesc";
            this.columnDesc.ReadOnly = true;
            // 
            // columnType
            // 
            this.columnType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.columnType, "columnType");
            this.columnType.Name = "columnType";
            this.columnType.ReadOnly = true;
            // 
            // columnMetadata
            // 
            this.columnMetadata.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.columnMetadata, "columnMetadata");
            this.columnMetadata.Name = "columnMetadata";
            this.columnMetadata.ReadOnly = true;
            // 
            // FindSrsButton
            // 
            resources.ApplyResources(this.FindSrsButton, "FindSrsButton");
            this.FindSrsButton.Image = global::XenAdmin.Properties.Resources.expanded_triangle;
            this.FindSrsButton.Name = "FindSrsButton";
            this.FindSrsButton.UseVisualStyleBackColor = true;
            this.FindSrsButton.Click += new System.EventHandler(this.FindSrsButton_Click);
            // 
            // buttonSelectAll
            // 
            resources.ApplyResources(this.buttonSelectAll, "buttonSelectAll");
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
            // 
            // FindSrsOptionsMenuStrip
            // 
            this.FindSrsOptionsMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iscsiToolStripMenuItem,
            this.fcToolStripMenuItem});
            this.FindSrsOptionsMenuStrip.Name = "searchOptionsMenuStrip";
            resources.ApplyResources(this.FindSrsOptionsMenuStrip, "FindSrsOptionsMenuStrip");
            // 
            // iscsiToolStripMenuItem
            // 
            this.iscsiToolStripMenuItem.Name = "iscsiToolStripMenuItem";
            resources.ApplyResources(this.iscsiToolStripMenuItem, "iscsiToolStripMenuItem");
            this.iscsiToolStripMenuItem.Click += new System.EventHandler(this.iscsiToolStripMenuItem_Click);
            // 
            // fcToolStripMenuItem
            // 
            this.fcToolStripMenuItem.Name = "fcToolStripMenuItem";
            resources.ApplyResources(this.fcToolStripMenuItem, "fcToolStripMenuItem");
            this.fcToolStripMenuItem.Click += new System.EventHandler(this.fcToolStripMenuItem_Click);
            // 
            // DRFailoverWizardStoragePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DRFailoverWizardStoragePage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSRs)).EndInit();
            this.FindSrsOptionsMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelText;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.NonReopeningContextMenuStrip FindSrsOptionsMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem iscsiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fcToolStripMenuItem;
        private System.Windows.Forms.Button FindSrsButton;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewSRs;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnTick;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnMetadata;
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.Button buttonClearAll;
    }
}
