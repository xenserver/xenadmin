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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle28 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle29 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle30 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button2 = new System.Windows.Forms.Button();
            this.dataGridViewVms = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnVM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnCurrentlyCached = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPrepopulation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridViewFarms = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnFarm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnConfiguration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSRs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConfigureButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pageContainerPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFarms)).BeginInit();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.tableLayoutPanel1);
            this.pageContainerPanel.Location = new System.Drawing.Point(0, 78);
            this.pageContainerPanel.Size = new System.Drawing.Size(739, 407);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.button2, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewVms, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewFarms, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ConfigureButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.button1, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(719, 387);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.button2, 2);
            this.button2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button2.Location = new System.Drawing.Point(0, 361);
            this.button2.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(199, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Configure VMs for PVS read-caching...";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // dataGridViewVms
            // 
            this.dataGridViewVms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.dataGridViewVms.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dataGridViewVms.Location = new System.Drawing.Point(0, 255);
            this.dataGridViewVms.Margin = new System.Windows.Forms.Padding(0, 0, 1, 5);
            this.dataGridViewVms.MultiSelect = true;
            this.dataGridViewVms.Name = "dataGridViewVms";
            this.dataGridViewVms.ReadOnly = true;
            this.dataGridViewVms.Size = new System.Drawing.Size(718, 98);
            this.dataGridViewVms.TabIndex = 10;
            // 
            // columnVM
            // 
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle25.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.columnVM.DefaultCellStyle = dataGridViewCellStyle25;
            this.columnVM.FillWeight = 30F;
            this.columnVM.HeaderText = "Virtual machine";
            this.columnVM.MinimumWidth = 80;
            this.columnVM.Name = "columnVM";
            this.columnVM.ReadOnly = true;
            // 
            // columnCurrentlyCached
            // 
            dataGridViewCellStyle26.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.columnCurrentlyCached.DefaultCellStyle = dataGridViewCellStyle26;
            this.columnCurrentlyCached.FillWeight = 30F;
            this.columnCurrentlyCached.HeaderText = "Currently cached";
            this.columnCurrentlyCached.MinimumWidth = 95;
            this.columnCurrentlyCached.Name = "columnCurrentlyCached";
            this.columnCurrentlyCached.ReadOnly = true;
            this.columnCurrentlyCached.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnSR
            // 
            dataGridViewCellStyle27.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnSR.DefaultCellStyle = dataGridViewCellStyle27;
            this.ColumnSR.FillWeight = 30F;
            this.ColumnSR.HeaderText = "SR used for cache";
            this.ColumnSR.MinimumWidth = 80;
            this.ColumnSR.Name = "ColumnSR";
            this.ColumnSR.ReadOnly = true;
            // 
            // ColumnPrepopulation
            // 
            this.ColumnPrepopulation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnPrepopulation.HeaderText = "Marked for prepopulation";
            this.ColumnPrepopulation.Name = "ColumnPrepopulation";
            this.ColumnPrepopulation.ReadOnly = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 232);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.label2.Size = new System.Drawing.Size(213, 23);
            this.label2.TabIndex = 9;
            this.label2.Text = "VMs with PVS Caching enabled";
            // 
            // dataGridViewFarms
            // 
            this.dataGridViewFarms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewFarms.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewFarms.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewFarms.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewFarms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewFarms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnFarm,
            this.ColumnConfiguration,
            this.ColumnSRs});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridViewFarms, 2);
            this.dataGridViewFarms.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dataGridViewFarms.Location = new System.Drawing.Point(0, 23);
            this.dataGridViewFarms.Margin = new System.Windows.Forms.Padding(0, 0, 1, 5);
            this.dataGridViewFarms.MultiSelect = true;
            this.dataGridViewFarms.Name = "dataGridViewFarms";
            this.dataGridViewFarms.ReadOnly = true;
            this.dataGridViewFarms.Size = new System.Drawing.Size(718, 165);
            this.dataGridViewFarms.TabIndex = 5;
            // 
            // ColumnFarm
            // 
            dataGridViewCellStyle28.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle28.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnFarm.DefaultCellStyle = dataGridViewCellStyle28;
            this.ColumnFarm.FillWeight = 20F;
            this.ColumnFarm.HeaderText = "PVS Farm";
            this.ColumnFarm.MinimumWidth = 80;
            this.ColumnFarm.Name = "ColumnFarm";
            this.ColumnFarm.ReadOnly = true;
            // 
            // ColumnConfiguration
            // 
            dataGridViewCellStyle29.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnConfiguration.DefaultCellStyle = dataGridViewCellStyle29;
            this.ColumnConfiguration.FillWeight = 20F;
            this.ColumnConfiguration.HeaderText = "Cache configuration";
            this.ColumnConfiguration.MinimumWidth = 95;
            this.ColumnConfiguration.Name = "ColumnConfiguration";
            this.ColumnConfiguration.ReadOnly = true;
            this.ColumnConfiguration.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnSRs
            // 
            this.ColumnSRs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle30.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnSRs.DefaultCellStyle = dataGridViewCellStyle30;
            this.ColumnSRs.FillWeight = 60F;
            this.ColumnSRs.HeaderText = "Cache SRs";
            this.ColumnSRs.MinimumWidth = 80;
            this.ColumnSRs.Name = "ColumnSRs";
            this.ColumnSRs.ReadOnly = true;
            // 
            // ConfigureButton
            // 
            this.ConfigureButton.AutoSize = true;
            this.ConfigureButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ConfigureButton.Location = new System.Drawing.Point(0, 196);
            this.ConfigureButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.ConfigureButton.Name = "ConfigureButton";
            this.ConfigureButton.Size = new System.Drawing.Size(128, 23);
            this.ConfigureButton.TabIndex = 6;
            this.ConfigureButton.Text = "&Configure PVS cache...";
            this.ConfigureButton.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(131, 196);
            this.button1.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "&View PVS Farms...";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.label1.Size = new System.Drawing.Size(171, 23);
            this.label1.TabIndex = 8;
            this.label1.Text = "PVS Cache configuration";
            // 
            // PvsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "PvsPage";
            this.Size = new System.Drawing.Size(739, 485);
            this.pageContainerPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFarms)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Controls.DataGridViewEx.DataGridViewEx dataGridViewFarms;
        public System.Windows.Forms.Button ConfigureButton;
        public System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button button2;
        private Controls.DataGridViewEx.DataGridViewEx dataGridViewVms;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnVM;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnCurrentlyCached;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSR;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPrepopulation;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFarm;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnConfiguration;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSRs;
    }
}
