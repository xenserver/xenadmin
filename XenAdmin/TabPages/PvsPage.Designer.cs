namespace XenAdmin.TabPages
{
    partial class PvsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PvsPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewVms = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnVM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCachingEnabled = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPvsSite = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ConfigureButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.enableButton = new XenAdmin.Commands.CommandButton();
            this.disableButton = new XenAdmin.Commands.CommandButton();
            this.pageContainerPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewVms, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // dataGridViewVms
            // 
            this.dataGridViewVms.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewVms.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewVms.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewVms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewVms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnVM,
            this.ColumnCachingEnabled,
            this.ColumnPvsSite,
            this.ColumnStatus});
            resources.ApplyResources(this.dataGridViewVms, "dataGridViewVms");
            this.dataGridViewVms.MultiSelect = true;
            this.dataGridViewVms.Name = "dataGridViewVms";
            this.dataGridViewVms.ReadOnly = true;
            // 
            // columnVM
            // 
            this.columnVM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.columnVM.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.columnVM, "columnVM");
            this.columnVM.Name = "columnVM";
            this.columnVM.ReadOnly = true;
            // 
            // ColumnCachingEnabled
            // 
            this.ColumnCachingEnabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            resources.ApplyResources(this.ColumnCachingEnabled, "ColumnCachingEnabled");
            this.ColumnCachingEnabled.Name = "ColumnCachingEnabled";
            this.ColumnCachingEnabled.ReadOnly = true;
            // 
            // ColumnPvsSite
            // 
            resources.ApplyResources(this.ColumnPvsSite, "ColumnPvsSite");
            this.ColumnPvsSite.Name = "ColumnPvsSite";
            this.ColumnPvsSite.ReadOnly = true;
            // 
            // ColumnStatus
            // 
            this.ColumnStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnStatus.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnStatus.FillWeight = 30F;
            resources.ApplyResources(this.ColumnStatus, "ColumnStatus");
            this.ColumnStatus.Name = "ColumnStatus";
            this.ColumnStatus.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.ConfigureButton);
            this.panel1.Name = "panel1";
            // 
            // ConfigureButton
            // 
            resources.ApplyResources(this.ConfigureButton, "ConfigureButton");
            this.ConfigureButton.Name = "ConfigureButton";
            this.ConfigureButton.UseVisualStyleBackColor = true;
            this.ConfigureButton.Click += new System.EventHandler(this.ConfigureButton_Click);
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.enableButton);
            this.panel2.Controls.Add(this.disableButton);
            this.panel2.Name = "panel2";
            // 
            // enableButton
            // 
            resources.ApplyResources(this.enableButton, "enableButton");
            this.enableButton.Name = "enableButton";
            this.enableButton.UseVisualStyleBackColor = true;
            // 
            // disableButton
            // 
            resources.ApplyResources(this.disableButton, "disableButton");
            this.disableButton.Name = "disableButton";
            this.disableButton.UseVisualStyleBackColor = true;
            // 
            // PvsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Name = "PvsPage";
            this.pageContainerPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private Controls.DataGridViewEx.DataGridViewEx dataGridViewVms;
        private XenAdmin.Commands.CommandButton disableButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private XenAdmin.Commands.CommandButton enableButton;
        public System.Windows.Forms.Button ConfigureButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnVM;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCachingEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPvsSite;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnStatus;
    }
}
