namespace XenAdmin.Wizards.ExportWizard
{
    partial class ExportSelectVMsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportSelectVMsPage));
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.m_labelIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_buttonSelectAll = new System.Windows.Forms.Button();
            this.m_buttonClearAll = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.m_tlpWarning = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_dataGridView = new System.Windows.Forms.DataGridView();
            this.columnTick = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnVM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnVapp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.m_labelCounter = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.m_searchTextBox = new XenAdmin.Controls.SearchTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2.SuspendLayout();
            this.m_tlpWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_ctrlError
            // 
            resources.ApplyResources(this.m_ctrlError, "m_ctrlError");
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // m_labelIntro
            // 
            resources.ApplyResources(this.m_labelIntro, "m_labelIntro");
            this.m_labelIntro.Name = "m_labelIntro";
            // 
            // m_buttonSelectAll
            // 
            resources.ApplyResources(this.m_buttonSelectAll, "m_buttonSelectAll");
            this.m_buttonSelectAll.Name = "m_buttonSelectAll";
            this.m_buttonSelectAll.UseVisualStyleBackColor = true;
            this.m_buttonSelectAll.Click += new System.EventHandler(this.m_buttonSelectAll_Click);
            // 
            // m_buttonClearAll
            // 
            resources.ApplyResources(this.m_buttonClearAll, "m_buttonClearAll");
            this.m_buttonClearAll.Name = "m_buttonClearAll";
            this.m_buttonClearAll.UseVisualStyleBackColor = true;
            this.m_buttonClearAll.Click += new System.EventHandler(this.m_buttonClearAll_Click);
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.m_ctrlError, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.m_labelIntro, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.m_tlpWarning, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.m_dataGridView, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // m_tlpWarning
            // 
            resources.ApplyResources(this.m_tlpWarning, "m_tlpWarning");
            this.m_tlpWarning.Controls.Add(this.pictureBox1, 0, 0);
            this.m_tlpWarning.Controls.Add(this.autoHeightLabel1, 1, 0);
            this.m_tlpWarning.Name = "m_tlpWarning";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // m_dataGridView
            // 
            this.m_dataGridView.AllowUserToAddRows = false;
            this.m_dataGridView.AllowUserToDeleteRows = false;
            this.m_dataGridView.AllowUserToOrderColumns = true;
            this.m_dataGridView.AllowUserToResizeRows = false;
            this.m_dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.m_dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.m_dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.m_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnTick,
            this.columnVM,
            this.columnDesc,
            this.columnSize,
            this.columnVapp});
            resources.ApplyResources(this.m_dataGridView, "m_dataGridView");
            this.m_dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.m_dataGridView.MultiSelect = false;
            this.m_dataGridView.Name = "m_dataGridView";
            this.m_dataGridView.RowHeadersVisible = false;
            this.m_dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridView_CellValueChanged);
            this.m_dataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.m_dataGridView_SortCompare);
            this.m_dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.m_dataGridView_CurrentCellDirtyStateChanged);
            this.m_dataGridView.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.m_dataGridView_RowStateChanged);
            // 
            // columnTick
            // 
            this.columnTick.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnTick, "columnTick");
            this.columnTick.Name = "columnTick";
            // 
            // columnVM
            // 
            resources.ApplyResources(this.columnVM, "columnVM");
            this.columnVM.Name = "columnVM";
            this.columnVM.ReadOnly = true;
            // 
            // columnDesc
            // 
            resources.ApplyResources(this.columnDesc, "columnDesc");
            this.columnDesc.Name = "columnDesc";
            this.columnDesc.ReadOnly = true;
            // 
            // columnSize
            // 
            resources.ApplyResources(this.columnSize, "columnSize");
            this.columnSize.Name = "columnSize";
            this.columnSize.ReadOnly = true;
            // 
            // columnVapp
            // 
            resources.ApplyResources(this.columnVapp, "columnVapp");
            this.columnVapp.Name = "columnVapp";
            this.columnVapp.ReadOnly = true;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.m_buttonSelectAll, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.m_buttonClearAll, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.m_labelCounter, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // m_labelCounter
            // 
            resources.ApplyResources(this.m_labelCounter, "m_labelCounter");
            this.m_labelCounter.Name = "m_labelCounter";
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.m_searchTextBox, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // m_searchTextBox
            // 
            resources.ApplyResources(this.m_searchTextBox, "m_searchTextBox");
            this.m_searchTextBox.Name = "m_searchTextBox";
            this.m_searchTextBox.TextChanged += new System.EventHandler(this.m_searchTextBox_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // ExportSelectVMsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "ExportSelectVMsPage";
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.m_tlpWarning.ResumeLayout(false);
            this.m_tlpWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private XenAdmin.Controls.Common.AutoHeightLabel m_labelIntro;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlError;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Button m_buttonSelectAll;
		private System.Windows.Forms.Button m_buttonClearAll;
		private System.Windows.Forms.TableLayoutPanel m_tlpWarning;
		private System.Windows.Forms.PictureBox pictureBox1;
		private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
		private System.Windows.Forms.DataGridView m_dataGridView;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
		private XenAdmin.Controls.SearchTextBox m_searchTextBox;
		private System.Windows.Forms.Label m_labelCounter;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DataGridViewCheckBoxColumn columnTick;
		private System.Windows.Forms.DataGridViewTextBoxColumn columnVM;
		private System.Windows.Forms.DataGridViewTextBoxColumn columnDesc;
		private System.Windows.Forms.DataGridViewTextBoxColumn columnSize;
		private System.Windows.Forms.DataGridViewTextBoxColumn columnVapp;
    }
}
