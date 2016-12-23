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
            this.FindSrsButton = new System.Windows.Forms.Button();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.spinnerIcon1 = new XenAdmin.Controls.SpinnerIcon();
            this.dataGridViewSRs = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnTick = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnMetadata = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FindSrsOptionsMenuStrip = new XenAdmin.Controls.NonReopeningContextMenuStrip(this.components);
            this.iscsiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._worker = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIcon1)).BeginInit();
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
            this.tableLayoutPanel1.Controls.Add(this.FindSrsButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelText, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonSelectAll, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonClearAll, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
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
            // buttonClearAll
            // 
            resources.ApplyResources(this.buttonClearAll, "buttonClearAll");
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.UseVisualStyleBackColor = true;
            this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.spinnerIcon1);
            this.panel1.Controls.Add(this.dataGridViewSRs);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.tableLayoutPanel1.SetRowSpan(this.panel1, 2);
            // 
            // spinnerIcon1
            // 
            resources.ApplyResources(this.spinnerIcon1, "spinnerIcon1");
            this.spinnerIcon1.BackColor = System.Drawing.SystemColors.Window;
            this.spinnerIcon1.Name = "spinnerIcon1";
            this.spinnerIcon1.TabStop = false;
            // 
            // dataGridViewSRs
            // 
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
            this.dataGridViewSRs.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridViewSRs.Name = "dataGridViewSRs";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewSRs.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridViewSRs.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewSRs.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewSRs_CellClick);
            this.dataGridViewSRs.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewSRs_CellValueChanged);
            // 
            // columnTick
            // 
            this.columnTick.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            resources.ApplyResources(this.columnTick, "columnTick");
            this.columnTick.Name = "columnTick";
            // 
            // columnName
            // 
            resources.ApplyResources(this.columnName, "columnName");
            this.columnName.Name = "columnName";
            this.columnName.ReadOnly = true;
            // 
            // columnDesc
            // 
            resources.ApplyResources(this.columnDesc, "columnDesc");
            this.columnDesc.Name = "columnDesc";
            this.columnDesc.ReadOnly = true;
            // 
            // columnType
            // 
            resources.ApplyResources(this.columnType, "columnType");
            this.columnType.Name = "columnType";
            this.columnType.ReadOnly = true;
            // 
            // columnMetadata
            // 
            this.columnMetadata.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnMetadata, "columnMetadata");
            this.columnMetadata.Name = "columnMetadata";
            this.columnMetadata.ReadOnly = true;
            // 
            // FindSrsOptionsMenuStrip
            // 
            this.FindSrsOptionsMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
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
            // _worker
            // 
            this._worker.WorkerReportsProgress = true;
            this._worker.WorkerSupportsCancellation = true;
            this._worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._worker_DoWork);
            this._worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this._worker_ProgressChanged);
            this._worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._worker_RunWorkerCompleted);
            // 
            // DRFailoverWizardStoragePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DRFailoverWizardStoragePage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIcon1)).EndInit();
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
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.Button buttonClearAll;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnTick;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnMetadata;
        private System.ComponentModel.BackgroundWorker _worker;
        private System.Windows.Forms.Panel panel1;
        private Controls.SpinnerIcon spinnerIcon1;
    }
}
