namespace XenAdmin.TabPages
{
    partial class VMStoragePage
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
            // Deregister listeners.
            VM = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VMStoragePage));
            this.AddButton = new System.Windows.Forms.Button();
            this.EditButton = new System.Windows.Forms.Button();
            this.AttachButton = new System.Windows.Forms.Button();
            this.RightButtonSeparator = new System.Windows.Forms.Panel();
            this.LeftButtonSeparator = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridViewStorage = new System.Windows.Forms.DataGridView();
            this.ColumnDevicePosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSRVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnReadOnly = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPriority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnActive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDevicePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.DeactivateButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.DeactivateButton = new System.Windows.Forms.Button();
            this.MoveButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.MoveButton = new System.Windows.Forms.Button();
            this.DeleteButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.DeleteDriveButton = new System.Windows.Forms.Button();
            this.DetachButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.DetachDriveButton = new System.Windows.Forms.Button();
            this.multipleDvdIsoList1 = new XenAdmin.Controls.MultipleDvdIsoList();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gradientPanel1 = new XenAdmin.Controls.GradientPanel.GradientPanel();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStorage)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.DeactivateButtonContainer.SuspendLayout();
            this.MoveButtonContainer.SuspendLayout();
            this.DeleteButtonContainer.SuspendLayout();
            this.DetachButtonContainer.SuspendLayout();
            this.gradientPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AddButton
            // 
            resources.ApplyResources(this.AddButton, "AddButton");
            this.AddButton.Name = "AddButton";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // EditButton
            // 
            resources.ApplyResources(this.EditButton, "EditButton");
            this.EditButton.Name = "EditButton";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // AttachButton
            // 
            resources.ApplyResources(this.AttachButton, "AttachButton");
            this.AttachButton.Name = "AttachButton";
            this.AttachButton.UseVisualStyleBackColor = true;
            this.AttachButton.Click += new System.EventHandler(this.AttachButton_Click);
            // 
            // RightButtonSeparator
            // 
            this.RightButtonSeparator.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            resources.ApplyResources(this.RightButtonSeparator, "RightButtonSeparator");
            this.RightButtonSeparator.Name = "RightButtonSeparator";
            // 
            // LeftButtonSeparator
            // 
            this.LeftButtonSeparator.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            resources.ApplyResources(this.LeftButtonSeparator, "LeftButtonSeparator");
            this.LeftButtonSeparator.Name = "LeftButtonSeparator";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.multipleDvdIsoList1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.dataGridViewStorage);
            this.panel2.Controls.Add(this.flowLayoutPanel1);
            this.panel2.MaximumSize = new System.Drawing.Size(900, 400);
            this.panel2.Name = "panel2";
            // 
            // dataGridViewStorage
            // 
            this.dataGridViewStorage.AllowUserToAddRows = false;
            this.dataGridViewStorage.AllowUserToDeleteRows = false;
            this.dataGridViewStorage.AllowUserToResizeRows = false;
            this.dataGridViewStorage.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewStorage.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewStorage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewStorage.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnDevicePosition,
            this.ColumnName,
            this.ColumnDesc,
            this.ColumnSR,
            this.ColumnSRVolume,
            this.ColumnSize,
            this.ColumnReadOnly,
            this.ColumnPriority,
            this.ColumnActive,
            this.ColumnDevicePath});
            resources.ApplyResources(this.dataGridViewStorage, "dataGridViewStorage");
            this.dataGridViewStorage.Name = "dataGridViewStorage";
            this.dataGridViewStorage.ReadOnly = true;
            this.dataGridViewStorage.RowHeadersVisible = false;
            this.dataGridViewStorage.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewStorage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TheHeaderListBox_DataGridView_MouseUp);
            this.dataGridViewStorage.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.TheHeaderListBox_CellMouseDoubleClick);
            this.dataGridViewStorage.SelectionChanged += new System.EventHandler(this.TheHeaderListBox_SelectedIndexChanged);
            // 
            // ColumnDevicePosition
            // 
            this.ColumnDevicePosition.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnDevicePosition, "ColumnDevicePosition");
            this.ColumnDevicePosition.Name = "ColumnDevicePosition";
            this.ColumnDevicePosition.ReadOnly = true;
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnName, "ColumnName");
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            // 
            // ColumnDesc
            // 
            this.ColumnDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnDesc, "ColumnDesc");
            this.ColumnDesc.Name = "ColumnDesc";
            this.ColumnDesc.ReadOnly = true;
            // 
            // ColumnSR
            // 
            this.ColumnSR.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnSR, "ColumnSR");
            this.ColumnSR.Name = "ColumnSR";
            this.ColumnSR.ReadOnly = true;
            // 
            // ColumnSRVolume
            // 
            this.ColumnSRVolume.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnSRVolume, "ColumnSRVolume");
            this.ColumnSRVolume.Name = "ColumnSRVolume";
            this.ColumnSRVolume.ReadOnly = true;
            // 
            // ColumnSize
            // 
            this.ColumnSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnSize, "ColumnSize");
            this.ColumnSize.Name = "ColumnSize";
            this.ColumnSize.ReadOnly = true;
            // 
            // ColumnReadOnly
            // 
            this.ColumnReadOnly.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnReadOnly, "ColumnReadOnly");
            this.ColumnReadOnly.Name = "ColumnReadOnly";
            this.ColumnReadOnly.ReadOnly = true;
            // 
            // ColumnPriority
            // 
            this.ColumnPriority.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnPriority, "ColumnPriority");
            this.ColumnPriority.Name = "ColumnPriority";
            this.ColumnPriority.ReadOnly = true;
            // 
            // ColumnActive
            // 
            this.ColumnActive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnActive, "ColumnActive");
            this.ColumnActive.Name = "ColumnActive";
            this.ColumnActive.ReadOnly = true;
            // 
            // ColumnDevicePath
            // 
            this.ColumnDevicePath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnDevicePath, "ColumnDevicePath");
            this.ColumnDevicePath.Name = "ColumnDevicePath";
            this.ColumnDevicePath.ReadOnly = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.AddButton);
            this.flowLayoutPanel1.Controls.Add(this.AttachButton);
            this.flowLayoutPanel1.Controls.Add(this.RightButtonSeparator);
            this.flowLayoutPanel1.Controls.Add(this.DeactivateButtonContainer);
            this.flowLayoutPanel1.Controls.Add(this.MoveButtonContainer);
            this.flowLayoutPanel1.Controls.Add(this.EditButton);
            this.flowLayoutPanel1.Controls.Add(this.LeftButtonSeparator);
            this.flowLayoutPanel1.Controls.Add(this.DeleteButtonContainer);
            this.flowLayoutPanel1.Controls.Add(this.DetachButtonContainer);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.MaximumSize = new System.Drawing.Size(900, 35);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // DeactivateButtonContainer
            // 
            this.DeactivateButtonContainer.Controls.Add(this.DeactivateButton);
            resources.ApplyResources(this.DeactivateButtonContainer, "DeactivateButtonContainer");
            this.DeactivateButtonContainer.Name = "DeactivateButtonContainer";
            // 
            // DeactivateButton
            // 
            resources.ApplyResources(this.DeactivateButton, "DeactivateButton");
            this.DeactivateButton.Name = "DeactivateButton";
            this.DeactivateButton.UseVisualStyleBackColor = true;
            this.DeactivateButton.Click += new System.EventHandler(this.DeactivateButton_Click);
            // 
            // MoveButtonContainer
            // 
            this.MoveButtonContainer.Controls.Add(this.MoveButton);
            resources.ApplyResources(this.MoveButtonContainer, "MoveButtonContainer");
            this.MoveButtonContainer.Name = "MoveButtonContainer";
            // 
            // MoveButton
            // 
            resources.ApplyResources(this.MoveButton, "MoveButton");
            this.MoveButton.Name = "MoveButton";
            this.MoveButton.UseVisualStyleBackColor = true;
            this.MoveButton.Click += new System.EventHandler(this.MoveButton_Click);
            // 
            // DeleteButtonContainer
            // 
            this.DeleteButtonContainer.Controls.Add(this.DeleteDriveButton);
            resources.ApplyResources(this.DeleteButtonContainer, "DeleteButtonContainer");
            this.DeleteButtonContainer.Name = "DeleteButtonContainer";
            // 
            // DeleteDriveButton
            // 
            resources.ApplyResources(this.DeleteDriveButton, "DeleteDriveButton");
            this.DeleteDriveButton.Name = "DeleteDriveButton";
            this.DeleteDriveButton.UseVisualStyleBackColor = true;
            this.DeleteDriveButton.Click += new System.EventHandler(this.DeleteDriveButton_Click);
            // 
            // DetachButtonContainer
            // 
            this.DetachButtonContainer.Controls.Add(this.DetachDriveButton);
            resources.ApplyResources(this.DetachButtonContainer, "DetachButtonContainer");
            this.DetachButtonContainer.Name = "DetachButtonContainer";
            // 
            // DetachDriveButton
            // 
            resources.ApplyResources(this.DetachDriveButton, "DetachDriveButton");
            this.DetachDriveButton.Name = "DetachDriveButton";
            this.DetachDriveButton.UseVisualStyleBackColor = true;
            this.DetachDriveButton.Click += new System.EventHandler(this.DetachButton_Click);
            // 
            // multipleDvdIsoList1
            // 
            resources.ApplyResources(this.multipleDvdIsoList1, "multipleDvdIsoList1");
            this.multipleDvdIsoList1.MaximumSize = new System.Drawing.Size(900, 36);
            this.multipleDvdIsoList1.Name = "multipleDvdIsoList1";
            this.multipleDvdIsoList1.VM = null;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
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
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
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
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn6, "dataGridViewTextBoxColumn6");
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn7, "dataGridViewTextBoxColumn7");
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.dataGridViewTextBoxColumn8, "dataGridViewTextBoxColumn8");
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn9, "dataGridViewTextBoxColumn9");
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn10, "dataGridViewTextBoxColumn10");
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.Controls.Add(this.TitleLabel);
            resources.ApplyResources(this.gradientPanel1, "gradientPanel1");
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Scheme = XenAdmin.Controls.GradientPanel.GradientPanel.Schemes.Tab;
            // 
            // TitleLabel
            // 
            resources.ApplyResources(this.TitleLabel, "TitleLabel");
            this.TitleLabel.AutoEllipsis = true;
            this.TitleLabel.ForeColor = System.Drawing.Color.White;
            this.TitleLabel.Name = "TitleLabel";
            // 
            // VMStoragePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gradientPanel1);
            this.DoubleBuffered = true;
            this.Name = "VMStoragePage";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStorage)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.DeactivateButtonContainer.ResumeLayout(false);
            this.MoveButtonContainer.ResumeLayout(false);
            this.DeleteButtonContainer.ResumeLayout(false);
            this.DetachButtonContainer.ResumeLayout(false);
            this.gradientPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button AttachButton;
        public System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button EditButton;
        public System.Windows.Forms.Button DetachDriveButton;
        public System.Windows.Forms.Button DeleteDriveButton;
        private System.Windows.Forms.Label TitleLabel;
        private XenAdmin.Controls.ToolTipContainer DetachButtonContainer;
        private XenAdmin.Controls.ToolTipContainer DeleteButtonContainer;
        private System.Windows.Forms.Panel RightButtonSeparator;
        private System.Windows.Forms.Panel LeftButtonSeparator;
        private XenAdmin.Controls.ToolTipContainer DeactivateButtonContainer;
        private XenAdmin.Controls.GradientPanel.GradientPanel gradientPanel1;
        public System.Windows.Forms.Button DeactivateButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private XenAdmin.Controls.MultipleDvdIsoList multipleDvdIsoList1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGridViewStorage;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDevicePosition;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDesc;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSR;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSRVolume;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSize;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnReadOnly;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPriority;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnActive;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDevicePath;
        private XenAdmin.Controls.ToolTipContainer MoveButtonContainer;
        private System.Windows.Forms.Button MoveButton;
    }
}
