namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    partial class NetWDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetWDetails));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.infoMtuPanel = new System.Windows.Forms.Panel();
            this.infoMtuMessage = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.labelNIC = new System.Windows.Forms.Label();
            this.labelVLAN = new System.Windows.Forms.Label();
            this.lblNicHelp = new System.Windows.Forms.Label();
            this.numericUpDownVLAN = new System.Windows.Forms.NumericUpDown();
            this.comboBoxNICList = new System.Windows.Forms.ComboBox();
            this.checkBoxAutomatic = new System.Windows.Forms.CheckBox();
            this.labelMTU = new System.Windows.Forms.Label();
            this.numericUpDownMTU = new System.Windows.Forms.NumericUpDown();
            this.panelVLANInfo = new System.Windows.Forms.Panel();
            this.labelVlanError = new System.Windows.Forms.Label();
            this.labelVLAN0Info = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.infoMtuPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVLAN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMTU)).BeginInit();
            this.panelVLANInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.infoMtuPanel, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelNIC, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelVLAN, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblNicHelp, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownVLAN, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxNICList, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxAutomatic, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelMTU, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownMTU, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.panelVLANInfo, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // infoMtuPanel
            // 
            resources.ApplyResources(this.infoMtuPanel, "infoMtuPanel");
            this.tableLayoutPanel1.SetColumnSpan(this.infoMtuPanel, 2);
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
            // labelNIC
            // 
            resources.ApplyResources(this.labelNIC, "labelNIC");
            this.labelNIC.Name = "labelNIC";
            // 
            // labelVLAN
            // 
            resources.ApplyResources(this.labelVLAN, "labelVLAN");
            this.labelVLAN.Name = "labelVLAN";
            // 
            // lblNicHelp
            // 
            resources.ApplyResources(this.lblNicHelp, "lblNicHelp");
            this.tableLayoutPanel1.SetColumnSpan(this.lblNicHelp, 4);
            this.lblNicHelp.Name = "lblNicHelp";
            // 
            // numericUpDownVLAN
            // 
            resources.ApplyResources(this.numericUpDownVLAN, "numericUpDownVLAN");
            this.numericUpDownVLAN.Maximum = new decimal(new int[] {
            4094,
            0,
            0,
            0});
            this.numericUpDownVLAN.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownVLAN.Name = "numericUpDownVLAN";
            this.numericUpDownVLAN.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownVLAN.ValueChanged += new System.EventHandler(this.nudVLAN_ValueChanged);
            // 
            // comboBoxNICList
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.comboBoxNICList, 2);
            this.comboBoxNICList.DisplayMember = "Server";
            this.comboBoxNICList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNICList.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxNICList, "comboBoxNICList");
            this.comboBoxNICList.Name = "comboBoxNICList";
            this.comboBoxNICList.Sorted = true;
            this.comboBoxNICList.SelectedIndexChanged += new System.EventHandler(this.cmbHostNicList_SelectedIndexChanged);
            // 
            // checkBoxAutomatic
            // 
            resources.ApplyResources(this.checkBoxAutomatic, "checkBoxAutomatic");
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxAutomatic, 4);
            this.checkBoxAutomatic.Name = "checkBoxAutomatic";
            this.checkBoxAutomatic.UseVisualStyleBackColor = true;
            // 
            // labelMTU
            // 
            resources.ApplyResources(this.labelMTU, "labelMTU");
            this.labelMTU.Name = "labelMTU";
            // 
            // numericUpDownMTU
            // 
            resources.ApplyResources(this.numericUpDownMTU, "numericUpDownMTU");
            this.numericUpDownMTU.Name = "numericUpDownMTU";
            // 
            // panelVLANInfo
            // 
            resources.ApplyResources(this.panelVLANInfo, "panelVLANInfo");
            this.tableLayoutPanel1.SetColumnSpan(this.panelVLANInfo, 2);
            this.panelVLANInfo.Controls.Add(this.labelVlanError);
            this.panelVLANInfo.Controls.Add(this.labelVLAN0Info);
            this.panelVLANInfo.Name = "panelVLANInfo";
            // 
            // labelVlanError
            // 
            resources.ApplyResources(this.labelVlanError, "labelVlanError");
            this.labelVlanError.ForeColor = System.Drawing.Color.Red;
            this.labelVlanError.Name = "labelVlanError";
            // 
            // labelVLAN0Info
            // 
            resources.ApplyResources(this.labelVLAN0Info, "labelVLAN0Info");
            this.labelVLAN0Info.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelVLAN0Info.Name = "labelVLAN0Info";
            // 
            // NetWDetails
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panel1);
            this.Name = "NetWDetails";
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.infoMtuPanel.ResumeLayout(false);
            this.infoMtuPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVLAN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMTU)).EndInit();
            this.panelVLANInfo.ResumeLayout(false);
            this.panelVLANInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelNIC;
        private System.Windows.Forms.ComboBox comboBoxNICList;
        private System.Windows.Forms.NumericUpDown numericUpDownVLAN;
        private System.Windows.Forms.Label labelVLAN;
        private System.Windows.Forms.Label lblNicHelp;
        private System.Windows.Forms.CheckBox checkBoxAutomatic;
        private System.Windows.Forms.Label labelVlanError;
        private System.Windows.Forms.Label labelMTU;
        private System.Windows.Forms.NumericUpDown numericUpDownMTU;
        private System.Windows.Forms.Panel panelVLANInfo;
        private System.Windows.Forms.Label labelVLAN0Info;
        private System.Windows.Forms.Panel infoMtuPanel;
        private System.Windows.Forms.Label infoMtuMessage;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}
