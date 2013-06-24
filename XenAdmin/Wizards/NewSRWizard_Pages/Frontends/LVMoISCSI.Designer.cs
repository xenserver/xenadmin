namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class LVMoISCSI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LVMoISCSI));
            this.textBoxIscsiPort = new System.Windows.Forms.TextBox();
            this.labelColon = new System.Windows.Forms.Label();
            this.toolTipContainerIQNscan = new XenAdmin.Controls.ToolTipContainer();
            this.buttonIscsiPopulateIQNs = new System.Windows.Forms.Button();
            this.IscsiUseChapCheckBox = new System.Windows.Forms.CheckBox();
            this.comboBoxIscsiIqns = new System.Windows.Forms.ComboBox();
            this.comboBoxIscsiLuns = new System.Windows.Forms.ComboBox();
            this.labelIscsiInvalidHost = new System.Windows.Forms.Label();
            this.lunInUseLabel = new System.Windows.Forms.Label();
            this.targetLunLabel = new System.Windows.Forms.Label();
            this.buttonIscsiPopulateLUNs = new System.Windows.Forms.Button();
            this.groupBoxChap = new XenAdmin.Controls.DecentGroupBox();
            this.IScsiChapSecretLabel = new System.Windows.Forms.Label();
            this.IScsiChapSecretTextBox = new System.Windows.Forms.TextBox();
            this.labelCHAPuser = new System.Windows.Forms.Label();
            this.IScsiChapUserTextBox = new System.Windows.Forms.TextBox();
            this.labelIscsiTargetHost = new System.Windows.Forms.Label();
            this.labelIscsiIQN = new System.Windows.Forms.Label();
            this.textBoxIscsiHost = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolTipContainerIQNscan.SuspendLayout();
            this.groupBoxChap.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxIscsiPort
            // 
            resources.ApplyResources(this.textBoxIscsiPort, "textBoxIscsiPort");
            this.textBoxIscsiPort.Name = "textBoxIscsiPort";
            this.textBoxIscsiPort.TextChanged += new System.EventHandler(this.textBoxIscsiHost_TextChanged);
            // 
            // labelColon
            // 
            resources.ApplyResources(this.labelColon, "labelColon");
            this.labelColon.Name = "labelColon";
            // 
            // toolTipContainerIQNscan
            // 
            resources.ApplyResources(this.toolTipContainerIQNscan, "toolTipContainerIQNscan");
            this.toolTipContainerIQNscan.Controls.Add(this.buttonIscsiPopulateIQNs);
            this.toolTipContainerIQNscan.Name = "toolTipContainerIQNscan";
            // 
            // buttonIscsiPopulateIQNs
            // 
            resources.ApplyResources(this.buttonIscsiPopulateIQNs, "buttonIscsiPopulateIQNs");
            this.buttonIscsiPopulateIQNs.Name = "buttonIscsiPopulateIQNs";
            this.buttonIscsiPopulateIQNs.UseVisualStyleBackColor = true;
            this.buttonIscsiPopulateIQNs.Click += new System.EventHandler(this.buttonIscsiPopulateIQNs_Click);
            // 
            // IscsiUseChapCheckBox
            // 
            resources.ApplyResources(this.IscsiUseChapCheckBox, "IscsiUseChapCheckBox");
            this.IscsiUseChapCheckBox.Name = "IscsiUseChapCheckBox";
            this.IscsiUseChapCheckBox.UseVisualStyleBackColor = true;
            this.IscsiUseChapCheckBox.CheckedChanged += new System.EventHandler(this.IscsiUseChapCheckBox_CheckedChanged);
            // 
            // comboBoxIscsiIqns
            // 
            resources.ApplyResources(this.comboBoxIscsiIqns, "comboBoxIscsiIqns");
            this.comboBoxIscsiIqns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIscsiIqns.FormattingEnabled = true;
            this.comboBoxIscsiIqns.Name = "comboBoxIscsiIqns";
            this.comboBoxIscsiIqns.SelectedIndexChanged += new System.EventHandler(this.IScsiTargetIqnComboBox_SelectedIndexChanged);
            // 
            // comboBoxIscsiLuns
            // 
            resources.ApplyResources(this.comboBoxIscsiLuns, "comboBoxIscsiLuns");
            this.comboBoxIscsiLuns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIscsiLuns.FormattingEnabled = true;
            this.comboBoxIscsiLuns.Name = "comboBoxIscsiLuns";
            this.comboBoxIscsiLuns.SelectedIndexChanged += new System.EventHandler(this.comboBoxIscsiLuns_SelectedIndexChanged);
            // 
            // labelIscsiInvalidHost
            // 
            resources.ApplyResources(this.labelIscsiInvalidHost, "labelIscsiInvalidHost");
            this.labelIscsiInvalidHost.ForeColor = System.Drawing.Color.Red;
            this.labelIscsiInvalidHost.Name = "labelIscsiInvalidHost";
            // 
            // lunInUseLabel
            // 
            resources.ApplyResources(this.lunInUseLabel, "lunInUseLabel");
            this.lunInUseLabel.Name = "lunInUseLabel";
            // 
            // targetLunLabel
            // 
            resources.ApplyResources(this.targetLunLabel, "targetLunLabel");
            this.targetLunLabel.Name = "targetLunLabel";
            // 
            // buttonIscsiPopulateLUNs
            // 
            resources.ApplyResources(this.buttonIscsiPopulateLUNs, "buttonIscsiPopulateLUNs");
            this.buttonIscsiPopulateLUNs.Name = "buttonIscsiPopulateLUNs";
            this.buttonIscsiPopulateLUNs.UseVisualStyleBackColor = true;
            this.buttonIscsiPopulateLUNs.Click += new System.EventHandler(this.buttonIscsiPopulateLUNs_Click);
            // 
            // groupBoxChap
            // 
            resources.ApplyResources(this.groupBoxChap, "groupBoxChap");
            this.groupBoxChap.Controls.Add(this.IScsiChapSecretLabel);
            this.groupBoxChap.Controls.Add(this.IScsiChapSecretTextBox);
            this.groupBoxChap.Controls.Add(this.labelCHAPuser);
            this.groupBoxChap.Controls.Add(this.IScsiChapUserTextBox);
            this.groupBoxChap.Name = "groupBoxChap";
            this.groupBoxChap.TabStop = false;
            // 
            // IScsiChapSecretLabel
            // 
            resources.ApplyResources(this.IScsiChapSecretLabel, "IScsiChapSecretLabel");
            this.IScsiChapSecretLabel.BackColor = System.Drawing.Color.Transparent;
            this.IScsiChapSecretLabel.Name = "IScsiChapSecretLabel";
            // 
            // IScsiChapSecretTextBox
            // 
            resources.ApplyResources(this.IScsiChapSecretTextBox, "IScsiChapSecretTextBox");
            this.IScsiChapSecretTextBox.Name = "IScsiChapSecretTextBox";
            this.IScsiChapSecretTextBox.UseSystemPasswordChar = true;
            this.IScsiChapSecretTextBox.TextChanged += new System.EventHandler(this.ChapSettings_Changed);
            // 
            // labelCHAPuser
            // 
            resources.ApplyResources(this.labelCHAPuser, "labelCHAPuser");
            this.labelCHAPuser.BackColor = System.Drawing.Color.Transparent;
            this.labelCHAPuser.Name = "labelCHAPuser";
            // 
            // IScsiChapUserTextBox
            // 
            this.IScsiChapUserTextBox.AllowDrop = true;
            resources.ApplyResources(this.IScsiChapUserTextBox, "IScsiChapUserTextBox");
            this.IScsiChapUserTextBox.Name = "IScsiChapUserTextBox";
            this.IScsiChapUserTextBox.TextChanged += new System.EventHandler(this.ChapSettings_Changed);
            // 
            // labelIscsiTargetHost
            // 
            resources.ApplyResources(this.labelIscsiTargetHost, "labelIscsiTargetHost");
            this.labelIscsiTargetHost.BackColor = System.Drawing.Color.Transparent;
            this.labelIscsiTargetHost.Name = "labelIscsiTargetHost";
            // 
            // labelIscsiIQN
            // 
            resources.ApplyResources(this.labelIscsiIQN, "labelIscsiIQN");
            this.labelIscsiIQN.BackColor = System.Drawing.Color.Transparent;
            this.labelIscsiIQN.Name = "labelIscsiIQN";
            // 
            // textBoxIscsiHost
            // 
            resources.ApplyResources(this.textBoxIscsiHost, "textBoxIscsiHost");
            this.textBoxIscsiHost.Name = "textBoxIscsiHost";
            this.textBoxIscsiHost.TextChanged += new System.EventHandler(this.textBoxIscsiHost_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 4);
            this.label1.Name = "label1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelIscsiTargetHost, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxIscsiHost, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelColon, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxIscsiPort, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelIscsiInvalidHost, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // LVMoISCSI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.IscsiUseChapCheckBox);
            this.Controls.Add(this.groupBoxChap);
            this.Controls.Add(this.labelIscsiIQN);
            this.Controls.Add(this.comboBoxIscsiIqns);
            this.Controls.Add(this.toolTipContainerIQNscan);
            this.Controls.Add(this.targetLunLabel);
            this.Controls.Add(this.comboBoxIscsiLuns);
            this.Controls.Add(this.buttonIscsiPopulateLUNs);
            this.Controls.Add(this.lunInUseLabel);
            this.Name = "LVMoISCSI";
            this.toolTipContainerIQNscan.ResumeLayout(false);
            this.groupBoxChap.ResumeLayout(false);
            this.groupBoxChap.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxIscsiPort;
        private System.Windows.Forms.Label labelColon;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerIQNscan;
        private System.Windows.Forms.Button buttonIscsiPopulateIQNs;
        private System.Windows.Forms.CheckBox IscsiUseChapCheckBox;
        private System.Windows.Forms.ComboBox comboBoxIscsiIqns;
        private System.Windows.Forms.ComboBox comboBoxIscsiLuns;
        private System.Windows.Forms.Label labelIscsiInvalidHost;
        private System.Windows.Forms.Label lunInUseLabel;
        private System.Windows.Forms.Label targetLunLabel;
        private System.Windows.Forms.Button buttonIscsiPopulateLUNs;
        private XenAdmin.Controls.DecentGroupBox groupBoxChap;
        private System.Windows.Forms.Label IScsiChapSecretLabel;
        private System.Windows.Forms.TextBox IScsiChapSecretTextBox;
        private System.Windows.Forms.Label labelCHAPuser;
        private System.Windows.Forms.TextBox IScsiChapUserTextBox;
        private System.Windows.Forms.Label labelIscsiTargetHost;
        private System.Windows.Forms.Label labelIscsiIQN;
        private System.Windows.Forms.TextBox textBoxIscsiHost;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
