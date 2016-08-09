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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button2 = new System.Windows.Forms.Button();
            this.dataGridViewVms = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnVM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnCurrentlyCached = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPrepopulation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridViewSites = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnSite = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnConfiguration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSRs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConfigureButton = new System.Windows.Forms.Button();
            this.ViewPvsSitesButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pageContainerPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSites)).BeginInit();
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
            this.tableLayoutPanel1.Controls.Add(this.button2, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewVms, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewSites, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ConfigureButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.ViewPvsSitesButton, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.tableLayoutPanel1.SetColumnSpan(this.button2, 2);
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // dataGridViewVms
            // 
            this.dataGridViewVms.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewVms.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewVms.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewVms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewVms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnVM,
            this.columnCurrentlyCached,
            this.ColumnSR,
            this.ColumnPrepopulation});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridViewVms, 2);
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
            // columnCurrentlyCached
            // 
            this.columnCurrentlyCached.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.columnCurrentlyCached.DefaultCellStyle = dataGridViewCellStyle2;
            this.columnCurrentlyCached.FillWeight = 30F;
            resources.ApplyResources(this.columnCurrentlyCached, "columnCurrentlyCached");
            this.columnCurrentlyCached.Name = "columnCurrentlyCached";
            this.columnCurrentlyCached.ReadOnly = true;
            this.columnCurrentlyCached.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnSR
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnSR.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColumnSR.FillWeight = 30F;
            resources.ApplyResources(this.ColumnSR, "ColumnSR");
            this.ColumnSR.Name = "ColumnSR";
            this.ColumnSR.ReadOnly = true;
            // 
            // ColumnPrepopulation
            // 
            this.ColumnPrepopulation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnPrepopulation.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColumnPrepopulation.FillWeight = 40F;
            resources.ApplyResources(this.ColumnPrepopulation, "ColumnPrepopulation");
            this.ColumnPrepopulation.Name = "ColumnPrepopulation";
            this.ColumnPrepopulation.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Name = "label2";
            // 
            // dataGridViewSites
            // 
            this.dataGridViewSites.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewSites.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewSites.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewSites.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewSites.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnSite,
            this.ColumnConfiguration,
            this.ColumnSRs});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridViewSites, 2);
            resources.ApplyResources(this.dataGridViewSites, "dataGridViewSites");
            this.dataGridViewSites.MultiSelect = true;
            this.dataGridViewSites.Name = "dataGridViewSites";
            this.dataGridViewSites.ReadOnly = true;
            // 
            // ColumnSite
            // 
            this.ColumnSite.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnSite.DefaultCellStyle = dataGridViewCellStyle5;
            this.ColumnSite.FillWeight = 20F;
            resources.ApplyResources(this.ColumnSite, "ColumnSite");
            this.ColumnSite.Name = "ColumnSite";
            this.ColumnSite.ReadOnly = true;
            // 
            // ColumnConfiguration
            // 
            this.ColumnConfiguration.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnConfiguration.DefaultCellStyle = dataGridViewCellStyle6;
            this.ColumnConfiguration.FillWeight = 20F;
            resources.ApplyResources(this.ColumnConfiguration, "ColumnConfiguration");
            this.ColumnConfiguration.Name = "ColumnConfiguration";
            this.ColumnConfiguration.ReadOnly = true;
            this.ColumnConfiguration.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnSRs
            // 
            this.ColumnSRs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnSRs.DefaultCellStyle = dataGridViewCellStyle7;
            resources.ApplyResources(this.ColumnSRs, "ColumnSRs");
            this.ColumnSRs.Name = "ColumnSRs";
            this.ColumnSRs.ReadOnly = true;
            // 
            // ConfigureButton
            // 
            resources.ApplyResources(this.ConfigureButton, "ConfigureButton");
            this.ConfigureButton.Name = "ConfigureButton";
            this.ConfigureButton.UseVisualStyleBackColor = true;
            // 
            // ViewPvsSitesButton
            // 
            resources.ApplyResources(this.ViewPvsSitesButton, "ViewPvsSitesButton");
            this.ViewPvsSitesButton.Name = "ViewPvsSitesButton";
            this.ViewPvsSitesButton.UseVisualStyleBackColor = true;
            this.ViewPvsSitesButton.Click += new System.EventHandler(this.ViewPvsSitesButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSites)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Controls.DataGridViewEx.DataGridViewEx dataGridViewSites;
        public System.Windows.Forms.Button ConfigureButton;
        public System.Windows.Forms.Button ViewPvsSitesButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button button2;
        private Controls.DataGridViewEx.DataGridViewEx dataGridViewVms;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnVM;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnCurrentlyCached;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSR;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPrepopulation;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSite;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnConfiguration;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSRs;
    }
}
