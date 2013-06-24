namespace XenAdmin.Wizards.GenericPages
{
    partial class SelectVMStorageWithMultipleVirtualDisksPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectVMStorageWithMultipleVirtualDisksPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_labelIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_radioAllOnSameSr = new System.Windows.Forms.RadioButton();
            this.m_radioSpecifySr = new System.Windows.Forms.RadioButton();
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.m_comboBoxSr = new XenAdmin.Controls.EnableableComboBox();
            this.m_dataGridView = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.m_colVmDisk = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_colStorage = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.m_labelIntro, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_radioAllOnSameSr, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_radioSpecifySr, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.m_ctrlError, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.m_comboBoxSr, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_dataGridView, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // m_labelIntro
            // 
            resources.ApplyResources(this.m_labelIntro, "m_labelIntro");
            this.m_labelIntro.Name = "m_labelIntro";
            // 
            // m_radioAllOnSameSr
            // 
            resources.ApplyResources(this.m_radioAllOnSameSr, "m_radioAllOnSameSr");
            this.m_radioAllOnSameSr.Name = "m_radioAllOnSameSr";
            this.m_radioAllOnSameSr.TabStop = true;
            this.m_radioAllOnSameSr.UseVisualStyleBackColor = true;
            this.m_radioAllOnSameSr.CheckedChanged += new System.EventHandler(this.m_radioAllOnSameSr_CheckedChanged);
            // 
            // m_radioSpecifySr
            // 
            resources.ApplyResources(this.m_radioSpecifySr, "m_radioSpecifySr");
            this.m_radioSpecifySr.Name = "m_radioSpecifySr";
            this.m_radioSpecifySr.TabStop = true;
            this.m_radioSpecifySr.UseVisualStyleBackColor = true;
            this.m_radioSpecifySr.CheckedChanged += new System.EventHandler(this.m_radioSpecifySr_CheckedChanged);
            // 
            // m_ctrlError
            // 
            resources.ApplyResources(this.m_ctrlError, "m_ctrlError");
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // m_comboBoxSr
            // 
            resources.ApplyResources(this.m_comboBoxSr, "m_comboBoxSr");
            this.m_comboBoxSr.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.m_comboBoxSr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboBoxSr.FormattingEnabled = true;
            this.m_comboBoxSr.Name = "m_comboBoxSr";
            this.m_comboBoxSr.SelectedIndexChanged += new System.EventHandler(this.m_comboBoxSr_SelectedIndexChanged);
            // 
            // m_dataGridView
            // 
            this.m_dataGridView.AllowUserToAddRows = false;
            this.m_dataGridView.AllowUserToDeleteRows = false;
            this.m_dataGridView.AllowUserToResizeColumns = false;
            this.m_dataGridView.AllowUserToResizeRows = false;
            this.m_dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.m_dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.m_dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.m_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.m_colVmDisk,
            this.m_colStorage});
            resources.ApplyResources(this.m_dataGridView, "m_dataGridView");
            this.m_dataGridView.Name = "m_dataGridView";
            this.m_dataGridView.RowHeadersVisible = false;
            this.m_dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridView_CellValueChanged);
            this.m_dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.m_dataGridView_CurrentCellDirtyStateChanged);
            this.m_dataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridView_CellEnter);
            // 
            // m_colVmDisk
            // 
            resources.ApplyResources(this.m_colVmDisk, "m_colVmDisk");
            this.m_colVmDisk.Name = "m_colVmDisk";
            this.m_colVmDisk.ReadOnly = true;
            // 
            // m_colStorage
            // 
            this.m_colStorage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.m_colStorage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.m_colStorage, "m_colStorage");
            this.m_colStorage.Name = "m_colStorage";
            // 
            // SelectVMStorageWithMultipleVirtualDisksPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SelectVMStorageWithMultipleVirtualDisksPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private XenAdmin.Controls.Common.AutoHeightLabel m_labelIntro;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlError;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx m_dataGridView;
		private System.Windows.Forms.RadioButton m_radioAllOnSameSr;
        private XenAdmin.Controls.EnableableComboBox m_comboBoxSr;
        private System.Windows.Forms.RadioButton m_radioSpecifySr;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_colVmDisk;
        private System.Windows.Forms.DataGridViewComboBoxColumn m_colStorage;
    }
}
