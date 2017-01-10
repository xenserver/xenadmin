namespace XenAdmin.Dialogs
{
	partial class DRConfigureDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DRConfigureDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.m_buttonOK = new System.Windows.Forms.Button();
            this.m_buttonCancel = new System.Windows.Forms.Button();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.autoHeightLabel2 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_tableLpWarning = new System.Windows.Forms.TableLayoutPanel();
            this.m_labelDisable2 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.pictureBoxWarning = new System.Windows.Forms.PictureBox();
            this.m_labelDisable1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_tableLpInfo = new System.Windows.Forms.TableLayoutPanel();
            this.m_labelEnable = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.pictureBoxInfo = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.spinnerIcon1 = new XenAdmin.Controls.SpinnerIcon();
            this.m_dataGridView = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnTick = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnSpace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._worker = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.m_tableLpWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarning)).BeginInit();
            this.m_tableLpInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIcon1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.autoHeightLabel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.autoHeightLabel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_tableLpWarning, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.m_tableLpInfo, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.m_buttonOK, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.m_buttonCancel, 3, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // m_buttonOK
            // 
            this.m_buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.m_buttonOK, "m_buttonOK");
            this.m_buttonOK.Name = "m_buttonOK";
            this.m_buttonOK.UseVisualStyleBackColor = true;
            // 
            // m_buttonCancel
            // 
            this.m_buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_buttonCancel, "m_buttonCancel");
            this.m_buttonCancel.Name = "m_buttonCancel";
            this.m_buttonCancel.UseVisualStyleBackColor = true;
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // autoHeightLabel2
            // 
            resources.ApplyResources(this.autoHeightLabel2, "autoHeightLabel2");
            this.autoHeightLabel2.Name = "autoHeightLabel2";
            // 
            // m_tableLpWarning
            // 
            resources.ApplyResources(this.m_tableLpWarning, "m_tableLpWarning");
            this.m_tableLpWarning.Controls.Add(this.m_labelDisable2, 1, 1);
            this.m_tableLpWarning.Controls.Add(this.pictureBoxWarning, 0, 0);
            this.m_tableLpWarning.Controls.Add(this.m_labelDisable1, 1, 0);
            this.m_tableLpWarning.Name = "m_tableLpWarning";
            // 
            // m_labelDisable2
            // 
            resources.ApplyResources(this.m_labelDisable2, "m_labelDisable2");
            this.m_labelDisable2.Name = "m_labelDisable2";
            // 
            // pictureBoxWarning
            // 
            resources.ApplyResources(this.pictureBoxWarning, "pictureBoxWarning");
            this.pictureBoxWarning.Name = "pictureBoxWarning";
            this.m_tableLpWarning.SetRowSpan(this.pictureBoxWarning, 2);
            this.pictureBoxWarning.TabStop = false;
            // 
            // m_labelDisable1
            // 
            resources.ApplyResources(this.m_labelDisable1, "m_labelDisable1");
            this.m_labelDisable1.Name = "m_labelDisable1";
            // 
            // m_tableLpInfo
            // 
            resources.ApplyResources(this.m_tableLpInfo, "m_tableLpInfo");
            this.m_tableLpInfo.Controls.Add(this.m_labelEnable, 9, 0);
            this.m_tableLpInfo.Controls.Add(this.pictureBoxInfo, 0, 0);
            this.m_tableLpInfo.Name = "m_tableLpInfo";
            // 
            // m_labelEnable
            // 
            resources.ApplyResources(this.m_labelEnable, "m_labelEnable");
            this.m_labelEnable.Name = "m_labelEnable";
            // 
            // pictureBoxInfo
            // 
            resources.ApplyResources(this.pictureBoxInfo, "pictureBoxInfo");
            this.pictureBoxInfo.Name = "pictureBoxInfo";
            this.pictureBoxInfo.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.spinnerIcon1);
            this.panel1.Controls.Add(this.m_dataGridView);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // spinnerIcon1
            // 
            resources.ApplyResources(this.spinnerIcon1, "spinnerIcon1");
            this.spinnerIcon1.BackColor = System.Drawing.SystemColors.Window;
            this.spinnerIcon1.Name = "spinnerIcon1";
            this.spinnerIcon1.TabStop = false;
            // 
            // m_dataGridView
            // 
            this.m_dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.m_dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.m_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.m_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnTick,
            this.columnName,
            this.columnDesc,
            this.columnType,
            this.columnSpace});
            resources.ApplyResources(this.m_dataGridView, "m_dataGridView");
            this.m_dataGridView.Name = "m_dataGridView";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.m_dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.m_dataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.m_dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridView_CellClick);
            this.m_dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridView_CellValueChanged);
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
            // columnSpace
            // 
            this.columnSpace.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnSpace, "columnSpace");
            this.columnSpace.Name = "columnSpace";
            this.columnSpace.ReadOnly = true;
            // 
            // _worker
            // 
            this._worker.WorkerReportsProgress = true;
            this._worker.WorkerSupportsCancellation = true;
            this._worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._worker_DoWork);
            this._worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this._worker_ProgressChanged);
            this._worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._worker_RunWorkerCompleted);
            // 
            // DRConfigureDialog
            // 
            this.AcceptButton = this.m_buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_buttonCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "DRConfigureDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DRConfigureDialog_FormClosing);
            this.Load += new System.EventHandler(this.DRConfigureDialog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.m_tableLpWarning.ResumeLayout(false);
            this.m_tableLpWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarning)).EndInit();
            this.m_tableLpInfo.ResumeLayout(false);
            this.m_tableLpInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIcon1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Button m_buttonOK;
		private System.Windows.Forms.Button m_buttonCancel;
		private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
		private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel2;
		private XenAdmin.Controls.DataGridViewEx.DataGridViewEx m_dataGridView;
		private System.Windows.Forms.TableLayoutPanel m_tableLpWarning;
		private System.Windows.Forms.PictureBox pictureBoxWarning;
        private XenAdmin.Controls.Common.AutoHeightLabel m_labelDisable1;
        private System.Windows.Forms.TableLayoutPanel m_tableLpInfo;
        private XenAdmin.Controls.Common.AutoHeightLabel m_labelEnable;
        private System.Windows.Forms.PictureBox pictureBoxInfo;
        private System.ComponentModel.BackgroundWorker _worker;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnTick;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnSpace;
        private Controls.Common.AutoHeightLabel m_labelDisable2;
        private System.Windows.Forms.Panel panel1;
        private Controls.SpinnerIcon spinnerIcon1;
	}
}

