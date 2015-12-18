namespace XenAdmin.Controls
{
    partial class BondDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BondDetails));
            this.cbxAutomatic = new System.Windows.Forms.CheckBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ColumnCheckBox = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnNic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnMac = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLinkStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSpeed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDuplex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnVendor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDevice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPci = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.numericUpDownMTU = new System.Windows.Forms.NumericUpDown();
            this.labelMTU = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.infoMtuPanel = new System.Windows.Forms.Panel();
            this.infoMtuMessage = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panelLACPWarning = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBoxBondMode = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelBondMode = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonLacpTcpudpPorts = new System.Windows.Forms.RadioButton();
            this.radioButtonLacpSrcMac = new System.Windows.Forms.RadioButton();
            this.radioButtonBalanceSlb = new System.Windows.Forms.RadioButton();
            this.radioButtonActiveBackup = new System.Windows.Forms.RadioButton();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMTU)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.infoMtuPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panelLACPWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBoxBondMode.SuspendLayout();
            this.tableLayoutPanelBondMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxAutomatic
            // 
            resources.ApplyResources(this.cbxAutomatic, "cbxAutomatic");
            this.tableLayoutPanel1.SetColumnSpan(this.cbxAutomatic, 3);
            this.cbxAutomatic.Name = "cbxAutomatic";
            this.cbxAutomatic.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnCheckBox,
            this.ColumnNic,
            this.ColumnMac,
            this.ColumnLinkStatus,
            this.ColumnSpeed,
            this.ColumnDuplex,
            this.ColumnVendor,
            this.ColumnDevice,
            this.ColumnPci});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridView1, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView1_CurrentCellDirtyStateChanged);
            // 
            // ColumnCheckBox
            // 
            this.ColumnCheckBox.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnCheckBox, "ColumnCheckBox");
            this.ColumnCheckBox.Name = "ColumnCheckBox";
            // 
            // ColumnNic
            // 
            resources.ApplyResources(this.ColumnNic, "ColumnNic");
            this.ColumnNic.Name = "ColumnNic";
            this.ColumnNic.ReadOnly = true;
            // 
            // ColumnMac
            // 
            resources.ApplyResources(this.ColumnMac, "ColumnMac");
            this.ColumnMac.Name = "ColumnMac";
            this.ColumnMac.ReadOnly = true;
            // 
            // ColumnLinkStatus
            // 
            resources.ApplyResources(this.ColumnLinkStatus, "ColumnLinkStatus");
            this.ColumnLinkStatus.Name = "ColumnLinkStatus";
            this.ColumnLinkStatus.ReadOnly = true;
            // 
            // ColumnSpeed
            // 
            resources.ApplyResources(this.ColumnSpeed, "ColumnSpeed");
            this.ColumnSpeed.Name = "ColumnSpeed";
            this.ColumnSpeed.ReadOnly = true;
            // 
            // ColumnDuplex
            // 
            resources.ApplyResources(this.ColumnDuplex, "ColumnDuplex");
            this.ColumnDuplex.Name = "ColumnDuplex";
            this.ColumnDuplex.ReadOnly = true;
            // 
            // ColumnVendor
            // 
            resources.ApplyResources(this.ColumnVendor, "ColumnVendor");
            this.ColumnVendor.Name = "ColumnVendor";
            this.ColumnVendor.ReadOnly = true;
            // 
            // ColumnDevice
            // 
            resources.ApplyResources(this.ColumnDevice, "ColumnDevice");
            this.ColumnDevice.Name = "ColumnDevice";
            this.ColumnDevice.ReadOnly = true;
            // 
            // ColumnPci
            // 
            this.ColumnPci.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnPci, "ColumnPci");
            this.ColumnPci.Name = "ColumnPci";
            this.ColumnPci.ReadOnly = true;
            // 
            // numericUpDownMTU
            // 
            resources.ApplyResources(this.numericUpDownMTU, "numericUpDownMTU");
            this.numericUpDownMTU.Name = "numericUpDownMTU";
            // 
            // labelMTU
            // 
            resources.ApplyResources(this.labelMTU, "labelMTU");
            this.labelMTU.Name = "labelMTU";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.infoMtuPanel, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.panelLACPWarning, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownMTU, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbxAutomatic, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelMTU, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxBondMode, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // infoMtuPanel
            // 
            resources.ApplyResources(this.infoMtuPanel, "infoMtuPanel");
            this.infoMtuPanel.Controls.Add(this.infoMtuMessage);
            this.infoMtuPanel.Controls.Add(this.pictureBox2);
            this.infoMtuPanel.Name = "infoMtuPanel";
            // 
            // infoMtuMessage
            // 
            resources.ApplyResources(this.infoMtuMessage, "infoMtuMessage");
            this.infoMtuMessage.Name = "infoMtuMessage";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // panelLACPWarning
            // 
            resources.ApplyResources(this.panelLACPWarning, "panelLACPWarning");
            this.tableLayoutPanel1.SetColumnSpan(this.panelLACPWarning, 3);
            this.panelLACPWarning.Controls.Add(this.label1);
            this.panelLACPWarning.Controls.Add(this.pictureBox1);
            this.panelLACPWarning.Name = "panelLACPWarning";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // groupBoxBondMode
            // 
            resources.ApplyResources(this.groupBoxBondMode, "groupBoxBondMode");
            this.tableLayoutPanel1.SetColumnSpan(this.groupBoxBondMode, 3);
            this.groupBoxBondMode.Controls.Add(this.tableLayoutPanelBondMode);
            this.groupBoxBondMode.Name = "groupBoxBondMode";
            this.groupBoxBondMode.TabStop = false;
            // 
            // tableLayoutPanelBondMode
            // 
            resources.ApplyResources(this.tableLayoutPanelBondMode, "tableLayoutPanelBondMode");
            this.tableLayoutPanelBondMode.Controls.Add(this.radioButtonLacpTcpudpPorts, 0, 2);
            this.tableLayoutPanelBondMode.Controls.Add(this.radioButtonLacpSrcMac, 0, 3);
            this.tableLayoutPanelBondMode.Controls.Add(this.radioButtonBalanceSlb, 0, 0);
            this.tableLayoutPanelBondMode.Controls.Add(this.radioButtonActiveBackup, 0, 1);
            this.tableLayoutPanelBondMode.Name = "tableLayoutPanelBondMode";
            // 
            // radioButtonLacpTcpudpPorts
            // 
            resources.ApplyResources(this.radioButtonLacpTcpudpPorts, "radioButtonLacpTcpudpPorts");
            this.radioButtonLacpTcpudpPorts.Name = "radioButtonLacpTcpudpPorts";
            this.radioButtonLacpTcpudpPorts.UseVisualStyleBackColor = true;
            this.radioButtonLacpTcpudpPorts.CheckedChanged += new System.EventHandler(this.BondMode_CheckedChanged);
            // 
            // radioButtonLacpSrcMac
            // 
            resources.ApplyResources(this.radioButtonLacpSrcMac, "radioButtonLacpSrcMac");
            this.radioButtonLacpSrcMac.Name = "radioButtonLacpSrcMac";
            this.radioButtonLacpSrcMac.UseVisualStyleBackColor = true;
            this.radioButtonLacpSrcMac.CheckedChanged += new System.EventHandler(this.BondMode_CheckedChanged);
            // 
            // radioButtonBalanceSlb
            // 
            resources.ApplyResources(this.radioButtonBalanceSlb, "radioButtonBalanceSlb");
            this.radioButtonBalanceSlb.Checked = true;
            this.radioButtonBalanceSlb.Name = "radioButtonBalanceSlb";
            this.radioButtonBalanceSlb.TabStop = true;
            this.radioButtonBalanceSlb.UseVisualStyleBackColor = true;
            this.radioButtonBalanceSlb.CheckedChanged += new System.EventHandler(this.BondMode_CheckedChanged);
            // 
            // radioButtonActiveBackup
            // 
            resources.ApplyResources(this.radioButtonActiveBackup, "radioButtonActiveBackup");
            this.radioButtonActiveBackup.Name = "radioButtonActiveBackup";
            this.radioButtonActiveBackup.UseVisualStyleBackColor = true;
            this.radioButtonActiveBackup.CheckedChanged += new System.EventHandler(this.BondMode_CheckedChanged);
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
            resources.ApplyResources(this.dataGridViewTextBoxColumn5, "dataGridViewTextBoxColumn5");
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            resources.ApplyResources(this.dataGridViewTextBoxColumn6, "dataGridViewTextBoxColumn6");
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            resources.ApplyResources(this.dataGridViewTextBoxColumn7, "dataGridViewTextBoxColumn7");
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn8, "dataGridViewTextBoxColumn8");
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            // 
            // BondDetails
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "BondDetails";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMTU)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.infoMtuPanel.ResumeLayout(false);
            this.infoMtuPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panelLACPWarning.ResumeLayout(false);
            this.panelLACPWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBoxBondMode.ResumeLayout(false);
            this.groupBoxBondMode.PerformLayout();
            this.tableLayoutPanelBondMode.ResumeLayout(false);
            this.tableLayoutPanelBondMode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox cbxAutomatic;
		private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.NumericUpDown numericUpDownMTU;
        private System.Windows.Forms.Label labelMTU;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnCheckBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNic;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnMac;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLinkStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSpeed;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDuplex;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnVendor;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDevice;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPci;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBondMode;
        private System.Windows.Forms.RadioButton radioButtonBalanceSlb;
        private System.Windows.Forms.RadioButton radioButtonActiveBackup;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.RadioButton radioButtonLacpTcpudpPorts;
        private System.Windows.Forms.RadioButton radioButtonLacpSrcMac;
        private System.Windows.Forms.GroupBox groupBoxBondMode;
        private System.Windows.Forms.Panel panelLACPWarning;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel infoMtuPanel;
        private System.Windows.Forms.Label infoMtuMessage;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}
