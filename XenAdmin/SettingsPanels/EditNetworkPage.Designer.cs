namespace XenAdmin.SettingsPanels
{
    partial class EditNetworkPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditNetworkPage));
            this.HostNicLabel = new System.Windows.Forms.Label();
            this.HostVLanLabel = new System.Windows.Forms.Label();
            this.autoCheckBox = new System.Windows.Forms.CheckBox();
            this.HostPNICList = new System.Windows.Forms.ComboBox();
            this.numUpDownVLAN = new System.Windows.Forms.NumericUpDown();
            this.nicHelpLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelVLAN0Info = new System.Windows.Forms.Label();
            this.panelLACPWarning = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.labelBlurb = new System.Windows.Forms.Label();
            this.panelDisruptionWarning = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.numericUpDownMTU = new System.Windows.Forms.NumericUpDown();
            this.labelMTU = new System.Windows.Forms.Label();
            this.warningText = new System.Windows.Forms.Label();
            this.labelCannotConfigureMTU = new System.Windows.Forms.Label();
            this.groupBoxBondMode = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelBondMode = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonLacpTcpudpPorts = new System.Windows.Forms.RadioButton();
            this.radioButtonBalanceSlb = new System.Windows.Forms.RadioButton();
            this.radioButtonActiveBackup = new System.Windows.Forms.RadioButton();
            this.radioButtonLacpSrcMac = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownVLAN)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelLACPWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panelDisruptionWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMTU)).BeginInit();
            this.groupBoxBondMode.SuspendLayout();
            this.tableLayoutPanelBondMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // HostNicLabel
            // 
            resources.ApplyResources(this.HostNicLabel, "HostNicLabel");
            this.HostNicLabel.Name = "HostNicLabel";
            // 
            // HostVLanLabel
            // 
            resources.ApplyResources(this.HostVLanLabel, "HostVLanLabel");
            this.HostVLanLabel.Name = "HostVLanLabel";
            // 
            // autoCheckBox
            // 
            resources.ApplyResources(this.autoCheckBox, "autoCheckBox");
            this.tableLayoutPanel1.SetColumnSpan(this.autoCheckBox, 4);
            this.autoCheckBox.Name = "autoCheckBox";
            // 
            // HostPNICList
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.HostPNICList, 3);
            this.HostPNICList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.HostPNICList, "HostPNICList");
            this.HostPNICList.FormattingEnabled = true;
            this.HostPNICList.Name = "HostPNICList";
            this.HostPNICList.Sorted = true;
            this.HostPNICList.SelectedIndexChanged += new System.EventHandler(this.HostPNICList_SelectedIndexChanged);
            // 
            // numUpDownVLAN
            // 
            resources.ApplyResources(this.numUpDownVLAN, "numUpDownVLAN");
            this.numUpDownVLAN.Maximum = new decimal(new int[] {
            4094,
            0,
            0,
            0});
            this.numUpDownVLAN.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numUpDownVLAN.Name = "numUpDownVLAN";
            this.numUpDownVLAN.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numUpDownVLAN.ValueChanged += new System.EventHandler(this.numUpDownVLAN_ValueChanged);
            // 
            // nicHelpLabel
            // 
            resources.ApplyResources(this.nicHelpLabel, "nicHelpLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.nicHelpLabel, 3);
            this.nicHelpLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nicHelpLabel.Name = "nicHelpLabel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelVLAN0Info, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.panelLACPWarning, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelBlurb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.numUpDownVLAN, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.nicHelpLabel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.HostVLanLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.HostNicLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.HostPNICList, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panelDisruptionWarning, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownMTU, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelMTU, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.autoCheckBox, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.warningText, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.labelCannotConfigureMTU, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxBondMode, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelVLAN0Info
            // 
            resources.ApplyResources(this.labelVLAN0Info, "labelVLAN0Info");
            this.tableLayoutPanel1.SetColumnSpan(this.labelVLAN0Info, 2);
            this.labelVLAN0Info.Name = "labelVLAN0Info";
            // 
            // panelLACPWarning
            // 
            resources.ApplyResources(this.panelLACPWarning, "panelLACPWarning");
            this.tableLayoutPanel1.SetColumnSpan(this.panelLACPWarning, 4);
            this.panelLACPWarning.Controls.Add(this.label2);
            this.panelLACPWarning.Controls.Add(this.pictureBox2);
            this.panelLACPWarning.Name = "panelLACPWarning";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // labelBlurb
            // 
            resources.ApplyResources(this.labelBlurb, "labelBlurb");
            this.tableLayoutPanel1.SetColumnSpan(this.labelBlurb, 4);
            this.labelBlurb.Name = "labelBlurb";
            // 
            // panelDisruptionWarning
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panelDisruptionWarning, 4);
            this.panelDisruptionWarning.Controls.Add(this.label1);
            this.panelDisruptionWarning.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.panelDisruptionWarning, "panelDisruptionWarning");
            this.panelDisruptionWarning.Name = "panelDisruptionWarning";
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
            // numericUpDownMTU
            // 
            resources.ApplyResources(this.numericUpDownMTU, "numericUpDownMTU");
            this.numericUpDownMTU.Name = "numericUpDownMTU";
            this.numericUpDownMTU.ValueChanged += new System.EventHandler(this.numericUpDownMTU_ValueChanged);
            // 
            // labelMTU
            // 
            resources.ApplyResources(this.labelMTU, "labelMTU");
            this.labelMTU.Name = "labelMTU";
            // 
            // warningText
            // 
            resources.ApplyResources(this.warningText, "warningText");
            this.tableLayoutPanel1.SetColumnSpan(this.warningText, 4);
            this.warningText.Name = "warningText";
            // 
            // labelCannotConfigureMTU
            // 
            resources.ApplyResources(this.labelCannotConfigureMTU, "labelCannotConfigureMTU");
            this.tableLayoutPanel1.SetColumnSpan(this.labelCannotConfigureMTU, 4);
            this.labelCannotConfigureMTU.Name = "labelCannotConfigureMTU";
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
            this.tableLayoutPanelBondMode.Controls.Add(this.radioButtonBalanceSlb, 0, 0);
            this.tableLayoutPanelBondMode.Controls.Add(this.radioButtonActiveBackup, 0, 1);
            this.tableLayoutPanelBondMode.Controls.Add(this.radioButtonLacpSrcMac, 0, 3);
            this.tableLayoutPanelBondMode.Name = "tableLayoutPanelBondMode";
            // 
            // radioButtonLacpTcpudpPorts
            // 
            resources.ApplyResources(this.radioButtonLacpTcpudpPorts, "radioButtonLacpTcpudpPorts");
            this.radioButtonLacpTcpudpPorts.Name = "radioButtonLacpTcpudpPorts";
            this.radioButtonLacpTcpudpPorts.UseVisualStyleBackColor = true;
            this.radioButtonLacpTcpudpPorts.CheckedChanged += new System.EventHandler(this.BondMode_CheckedChanged);
            // 
            // radioButtonBalanceSlb
            // 
            resources.ApplyResources(this.radioButtonBalanceSlb, "radioButtonBalanceSlb");
            this.radioButtonBalanceSlb.Name = "radioButtonBalanceSlb";
            this.radioButtonBalanceSlb.TabStop = true;
            this.radioButtonBalanceSlb.UseVisualStyleBackColor = true;
            this.radioButtonBalanceSlb.CheckedChanged += new System.EventHandler(this.BondMode_CheckedChanged);
            // 
            // radioButtonActiveBackup
            // 
            resources.ApplyResources(this.radioButtonActiveBackup, "radioButtonActiveBackup");
            this.radioButtonActiveBackup.Name = "radioButtonActiveBackup";
            this.radioButtonActiveBackup.TabStop = true;
            this.radioButtonActiveBackup.UseVisualStyleBackColor = true;
            this.radioButtonActiveBackup.CheckedChanged += new System.EventHandler(this.BondMode_CheckedChanged);
            // 
            // radioButtonLacpSrcMac
            // 
            resources.ApplyResources(this.radioButtonLacpSrcMac, "radioButtonLacpSrcMac");
            this.radioButtonLacpSrcMac.Name = "radioButtonLacpSrcMac";
            this.radioButtonLacpSrcMac.UseVisualStyleBackColor = true;
            this.radioButtonLacpSrcMac.CheckedChanged += new System.EventHandler(this.BondMode_CheckedChanged);
            // 
            // EditNetworkPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "EditNetworkPage";
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownVLAN)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panelLACPWarning.ResumeLayout(false);
            this.panelLACPWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panelDisruptionWarning.ResumeLayout(false);
            this.panelDisruptionWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMTU)).EndInit();
            this.groupBoxBondMode.ResumeLayout(false);
            this.groupBoxBondMode.PerformLayout();
            this.tableLayoutPanelBondMode.ResumeLayout(false);
            this.tableLayoutPanelBondMode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label HostNicLabel;
        private System.Windows.Forms.Label HostVLanLabel;
        private System.Windows.Forms.CheckBox autoCheckBox;
        private System.Windows.Forms.ComboBox HostPNICList;
        private System.Windows.Forms.NumericUpDown numUpDownVLAN;
        private System.Windows.Forms.Label nicHelpLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelBlurb;
        private System.Windows.Forms.Label labelCannotConfigureMTU;
        private System.Windows.Forms.Label warningText;
        private System.Windows.Forms.NumericUpDown numericUpDownMTU;
        private System.Windows.Forms.Label labelMTU;
        private System.Windows.Forms.Panel panelDisruptionWarning;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBondMode;
        private System.Windows.Forms.RadioButton radioButtonBalanceSlb;
        private System.Windows.Forms.RadioButton radioButtonActiveBackup;
        private System.Windows.Forms.RadioButton radioButtonLacpSrcMac;
        private System.Windows.Forms.RadioButton radioButtonLacpTcpudpPorts;
        private System.Windows.Forms.GroupBox groupBoxBondMode;
        private System.Windows.Forms.Panel panelLACPWarning;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label labelVLAN0Info;

    }
}
