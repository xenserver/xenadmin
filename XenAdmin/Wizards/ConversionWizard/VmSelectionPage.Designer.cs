namespace XenAdmin.Wizards.ConversionWizard
{
    partial class VmSelectionPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VmSelectionPage));
            this.tableLayoutPanelVmSelection = new System.Windows.Forms.TableLayoutPanel();
            this.labelDesc = new System.Windows.Forms.Label();
            this.dataGridViewVms = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnChecked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnVm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnOs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDiskSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnRemarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanelButtons = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.tableLayoutPanelVersion = new System.Windows.Forms.TableLayoutPanel();
            this.supportedOSLinkLabel = new System.Windows.Forms.LinkLabel();
            this.showOnlySupportedGuestCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanelError = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxError = new System.Windows.Forms.PictureBox();
            this.labelError = new System.Windows.Forms.Label();
            this._backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanelVmSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).BeginInit();
            this.tableLayoutPanelButtons.SuspendLayout();
            this.tableLayoutPanelVersion.SuspendLayout();
            this.tableLayoutPanelError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanelVmSelection
            // 
            resources.ApplyResources(this.tableLayoutPanelVmSelection, "tableLayoutPanelVmSelection");
            this.tableLayoutPanelVmSelection.Controls.Add(this.labelDesc, 0, 0);
            this.tableLayoutPanelVmSelection.Controls.Add(this.dataGridViewVms, 0, 1);
            this.tableLayoutPanelVmSelection.Controls.Add(this.tableLayoutPanelButtons, 0, 2);
            this.tableLayoutPanelVmSelection.Controls.Add(this.tableLayoutPanelVersion, 0, 3);
            this.tableLayoutPanelVmSelection.Controls.Add(this.tableLayoutPanelError, 0, 4);
            this.tableLayoutPanelVmSelection.Name = "tableLayoutPanelVmSelection";
            // 
            // labelDesc
            // 
            resources.ApplyResources(this.labelDesc, "labelDesc");
            this.labelDesc.Name = "labelDesc";
            // 
            // dataGridViewVms
            // 
            this.dataGridViewVms.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewVms.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewVms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewVms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnChecked,
            this.columnVm,
            this.columnOs,
            this.columnDiskSize,
            this.columnRemarks});
            resources.ApplyResources(this.dataGridViewVms, "dataGridViewVms");
            this.dataGridViewVms.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dataGridViewVms.MultiSelect = true;
            this.dataGridViewVms.Name = "dataGridViewVms";
            this.dataGridViewVms.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewVms_CellValueChanged);
            this.dataGridViewVms.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridViewVms_CurrentCellDirtyStateChanged);
            // 
            // columnChecked
            // 
            this.columnChecked.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnChecked, "columnChecked");
            this.columnChecked.Name = "columnChecked";
            // 
            // columnVm
            // 
            this.columnVm.FillWeight = 70F;
            resources.ApplyResources(this.columnVm, "columnVm");
            this.columnVm.Name = "columnVm";
            this.columnVm.ReadOnly = true;
            // 
            // columnOs
            // 
            this.columnOs.FillWeight = 80F;
            resources.ApplyResources(this.columnOs, "columnOs");
            this.columnOs.Name = "columnOs";
            this.columnOs.ReadOnly = true;
            // 
            // columnDiskSize
            // 
            this.columnDiskSize.FillWeight = 25F;
            resources.ApplyResources(this.columnDiskSize, "columnDiskSize");
            this.columnDiskSize.Name = "columnDiskSize";
            this.columnDiskSize.ReadOnly = true;
            // 
            // columnRemarks
            // 
            this.columnRemarks.FillWeight = 30F;
            resources.ApplyResources(this.columnRemarks, "columnRemarks");
            this.columnRemarks.Name = "columnRemarks";
            this.columnRemarks.ReadOnly = true;
            // 
            // tableLayoutPanelButtons
            // 
            resources.ApplyResources(this.tableLayoutPanelButtons, "tableLayoutPanelButtons");
            this.tableLayoutPanelButtons.Controls.Add(this.buttonSelectAll, 0, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.buttonClearAll, 1, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.buttonRefresh, 3, 0);
            this.tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
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
            // buttonRefresh
            // 
            resources.ApplyResources(this.buttonRefresh, "buttonRefresh");
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // tableLayoutPanelVersion
            // 
            resources.ApplyResources(this.tableLayoutPanelVersion, "tableLayoutPanelVersion");
            this.tableLayoutPanelVersion.Controls.Add(this.supportedOSLinkLabel, 1, 0);
            this.tableLayoutPanelVersion.Controls.Add(this.showOnlySupportedGuestCheckBox, 0, 0);
            this.tableLayoutPanelVersion.Name = "tableLayoutPanelVersion";
            // 
            // supportedOSLinkLabel
            // 
            resources.ApplyResources(this.supportedOSLinkLabel, "supportedOSLinkLabel");
            this.supportedOSLinkLabel.Name = "supportedOSLinkLabel";
            this.supportedOSLinkLabel.TabStop = true;
            this.supportedOSLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.supportedOSLinkLabel_LinkClicked);
            // 
            // showOnlySupportedGuestCheckBox
            // 
            resources.ApplyResources(this.showOnlySupportedGuestCheckBox, "showOnlySupportedGuestCheckBox");
            this.showOnlySupportedGuestCheckBox.Checked = true;
            this.showOnlySupportedGuestCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showOnlySupportedGuestCheckBox.Name = "showOnlySupportedGuestCheckBox";
            this.showOnlySupportedGuestCheckBox.UseVisualStyleBackColor = true;
            this.showOnlySupportedGuestCheckBox.CheckedChanged += new System.EventHandler(this.showOnlySupportedGuestCheckBox_CheckedChanged);
            // 
            // tableLayoutPanelError
            // 
            resources.ApplyResources(this.tableLayoutPanelError, "tableLayoutPanelError");
            this.tableLayoutPanelError.Controls.Add(this.pictureBoxError, 0, 0);
            this.tableLayoutPanelError.Controls.Add(this.labelError, 1, 0);
            this.tableLayoutPanelError.Name = "tableLayoutPanelError";
            // 
            // pictureBoxError
            // 
            resources.ApplyResources(this.pictureBoxError, "pictureBoxError");
            this.pictureBoxError.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.pictureBoxError.Name = "pictureBoxError";
            this.pictureBoxError.TabStop = false;
            // 
            // labelError
            // 
            resources.ApplyResources(this.labelError, "labelError");
            this.labelError.Name = "labelError";
            // 
            // _backgroundWorker
            // 
            this._backgroundWorker.WorkerSupportsCancellation = true;
            this._backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._backgroundWorker_DoWork);
            this._backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._backgroundWorker_RunWorkerCompleted);
            // 
            // VmSelectionPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanelVmSelection);
            this.Name = "VmSelectionPage";
            this.tableLayoutPanelVmSelection.ResumeLayout(false);
            this.tableLayoutPanelVmSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).EndInit();
            this.tableLayoutPanelButtons.ResumeLayout(false);
            this.tableLayoutPanelVersion.ResumeLayout(false);
            this.tableLayoutPanelVersion.PerformLayout();
            this.tableLayoutPanelError.ResumeLayout(false);
            this.tableLayoutPanelError.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelVmSelection;
        private System.Windows.Forms.Label labelDesc;
        private Controls.DataGridViewEx.DataGridViewEx dataGridViewVms;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnChecked;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnVm;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnOs;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDiskSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnRemarks;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelButtons;
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.Button buttonClearAll;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelVersion;
        private System.Windows.Forms.CheckBox showOnlySupportedGuestCheckBox;
        private System.Windows.Forms.LinkLabel supportedOSLinkLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelError;
        private System.Windows.Forms.PictureBox pictureBoxError;
        private System.Windows.Forms.Label labelError;
        private System.ComponentModel.BackgroundWorker _backgroundWorker;
    }
}