namespace XenAdmin.Dialogs.OptionsPages
{
    partial class PluginOptionsPage
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginOptionsPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.m_gridFeatures = new System.Windows.Forms.DataGridView();
            this.col1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.labelIntro = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.refreshButton = new System.Windows.Forms.Button();
            this.m_tlpScanning = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labelNoPlugins = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.m_gridPlugins = new System.Windows.Forms.DataGridView();
            this.colEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVendor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.m_gridFeatures)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.m_tlpScanning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_gridPlugins)).BeginInit();
            this.SuspendLayout();
            // 
            // m_gridFeatures
            // 
            this.m_gridFeatures.AllowUserToAddRows = false;
            this.m_gridFeatures.AllowUserToDeleteRows = false;
            this.m_gridFeatures.AllowUserToResizeRows = false;
            this.m_gridFeatures.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.m_gridFeatures.BackgroundColor = System.Drawing.SystemColors.Window;
            this.m_gridFeatures.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_gridFeatures.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.m_gridFeatures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_gridFeatures.ColumnHeadersVisible = false;
            this.m_gridFeatures.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col1,
            this.col2});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.m_gridFeatures.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.m_gridFeatures, "m_gridFeatures");
            this.m_gridFeatures.Name = "m_gridFeatures";
            this.m_gridFeatures.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.m_gridFeatures.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.m_gridFeatures.RowHeadersVisible = false;
            this.m_gridFeatures.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_gridFeatures.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.m_gridFeatures_RowStateChanged);
            this.m_gridFeatures.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_gridFeatures_CellContentClick);
            // 
            // col1
            // 
            resources.ApplyResources(this.col1, "col1");
            this.col1.Name = "col1";
            this.col1.ReadOnly = true;
            // 
            // col2
            // 
            resources.ApplyResources(this.col2, "col2");
            this.col2.Name = "col2";
            this.col2.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // labelIntro
            // 
            resources.ApplyResources(this.labelIntro, "labelIntro");
            this.labelIntro.Name = "labelIntro";
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.labelIntro, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.refreshButton, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.m_tlpScanning, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelNoPlugins, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.m_gridFeatures, 0, 7);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.m_gridPlugins, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.linkLabel1, 0, 8);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // refreshButton
            // 
            resources.ApplyResources(this.refreshButton, "refreshButton");
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // m_tlpScanning
            // 
            resources.ApplyResources(this.m_tlpScanning, "m_tlpScanning");
            this.m_tlpScanning.Controls.Add(this.pictureBox1, 0, 0);
            this.m_tlpScanning.Controls.Add(this.label4, 1, 0);
            this.m_tlpScanning.Name = "m_tlpScanning";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources.ajax_loader;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // labelNoPlugins
            // 
            resources.ApplyResources(this.labelNoPlugins, "labelNoPlugins");
            this.labelNoPlugins.Name = "labelNoPlugins";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // m_gridPlugins
            // 
            this.m_gridPlugins.AllowUserToAddRows = false;
            this.m_gridPlugins.AllowUserToDeleteRows = false;
            this.m_gridPlugins.AllowUserToResizeRows = false;
            this.m_gridPlugins.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.m_gridPlugins.BackgroundColor = System.Drawing.SystemColors.Window;
            this.m_gridPlugins.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_gridPlugins.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.m_gridPlugins.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.m_gridPlugins.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEnabled,
            this.colName,
            this.colVendor,
            this.colDesc});
            resources.ApplyResources(this.m_gridPlugins, "m_gridPlugins");
            this.m_gridPlugins.MultiSelect = false;
            this.m_gridPlugins.Name = "m_gridPlugins";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.m_gridPlugins.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.m_gridPlugins.RowHeadersVisible = false;
            this.m_gridPlugins.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.m_gridPlugins.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_gridPlugins.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.m_gridPlugins_CellFormatting);
            this.m_gridPlugins.SelectionChanged += new System.EventHandler(this.m_gridPlugins_SelectionChanged);
            this.m_gridPlugins.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_gridPlugins_CellClick);
            // 
            // colEnabled
            // 
            resources.ApplyResources(this.colEnabled, "colEnabled");
            this.colEnabled.Name = "colEnabled";
            this.colEnabled.ReadOnly = true;
            this.colEnabled.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // colName
            // 
            resources.ApplyResources(this.colName, "colName");
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colVendor
            // 
            resources.ApplyResources(this.colVendor, "colVendor");
            this.colVendor.Name = "colVendor";
            this.colVendor.ReadOnly = true;
            // 
            // colDesc
            // 
            this.colDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colDesc, "colDesc");
            this.colDesc.Name = "colDesc";
            this.colDesc.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            resources.ApplyResources(this.dataGridViewTextBoxColumn2, "dataGridViewTextBoxColumn2");
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn3, "dataGridViewTextBoxColumn3");
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            resources.ApplyResources(this.dataGridViewTextBoxColumn4, "dataGridViewTextBoxColumn4");
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn5, "dataGridViewTextBoxColumn5");
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // PluginOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "PluginOptionsPage";
            ((System.ComponentModel.ISupportInitialize)(this.m_gridFeatures)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.m_tlpScanning.ResumeLayout(false);
            this.m_tlpScanning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_gridPlugins)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelIntro;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.DataGridView m_gridPlugins;
        private System.Windows.Forms.DataGridView m_gridFeatures;
        private System.Windows.Forms.DataGridViewTextBoxColumn col1;
        private System.Windows.Forms.DataGridViewTextBoxColumn col2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVendor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel m_tlpScanning;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelNoPlugins;
    }
}