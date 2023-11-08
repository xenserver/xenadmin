namespace XenAdmin.Wizards.ExportWizard
{
	partial class ExportFinishPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportFinishPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_labelIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_dataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_checkBoxVerify = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanelWarning = new System.Windows.Forms.TableLayoutPanel();
            this.warningPicture = new System.Windows.Forms.PictureBox();
            this.warningLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).BeginInit();
            this.tableLayoutPanelWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.m_labelIntro, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_dataGridView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_checkBoxVerify, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelWarning, 1, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // m_labelIntro
            // 
            resources.ApplyResources(this.m_labelIntro, "m_labelIntro");
            this.tableLayoutPanel1.SetColumnSpan(this.m_labelIntro, 2);
            this.m_labelIntro.Name = "m_labelIntro";
            // 
            // m_dataGridView
            // 
            this.m_dataGridView.AllowUserToAddRows = false;
            this.m_dataGridView.AllowUserToDeleteRows = false;
            this.m_dataGridView.AllowUserToResizeColumns = false;
            this.m_dataGridView.AllowUserToResizeRows = false;
            this.m_dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.m_dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.m_dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.m_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dataGridView.ColumnHeadersVisible = false;
            this.m_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.tableLayoutPanel1.SetColumnSpan(this.m_dataGridView, 2);
            resources.ApplyResources(this.m_dataGridView, "m_dataGridView");
            this.m_dataGridView.MultiSelect = false;
            this.m_dataGridView.Name = "m_dataGridView";
            this.m_dataGridView.ReadOnly = true;
            this.m_dataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            this.m_dataGridView.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.m_dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // Column1
            // 
            resources.ApplyResources(this.Column1, "Column1");
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.Column2, "Column2");
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // m_checkBoxVerify
            // 
            resources.ApplyResources(this.m_checkBoxVerify, "m_checkBoxVerify");
            this.m_checkBoxVerify.Name = "m_checkBoxVerify";
            this.m_checkBoxVerify.UseVisualStyleBackColor = true;
            this.m_checkBoxVerify.CheckStateChanged += new System.EventHandler(this.m_checkBoxVerify_CheckStateChanged);
            // 
            // tableLayoutPanelWarning
            // 
            resources.ApplyResources(this.tableLayoutPanelWarning, "tableLayoutPanelWarning");
            this.tableLayoutPanelWarning.Controls.Add(this.warningPicture, 0, 0);
            this.tableLayoutPanelWarning.Controls.Add(this.warningLabel, 1, 0);
            this.tableLayoutPanelWarning.Name = "tableLayoutPanelWarning";
            // 
            // warningPicture
            // 
            resources.ApplyResources(this.warningPicture, "warningPicture");
            this.warningPicture.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.warningPicture.Name = "warningPicture";
            this.warningPicture.TabStop = false;
            // 
            // warningLabel
            // 
            resources.ApplyResources(this.warningLabel, "warningLabel");
            this.warningLabel.Name = "warningLabel";
            // 
            // ExportFinishPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ExportFinishPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).EndInit();
            this.tableLayoutPanelWarning.ResumeLayout(false);
            this.tableLayoutPanelWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningPicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private XenAdmin.Controls.Common.AutoHeightLabel m_labelIntro;
		private System.Windows.Forms.DataGridView m_dataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.PictureBox warningPicture;
        private System.Windows.Forms.Label warningLabel;
        private System.Windows.Forms.CheckBox m_checkBoxVerify;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelWarning;
    }
}
