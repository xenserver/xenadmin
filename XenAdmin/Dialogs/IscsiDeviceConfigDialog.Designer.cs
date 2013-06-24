namespace XenAdmin.Dialogs
{
    partial class IscsiDeviceConfigDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IscsiDeviceConfigDialog));
            this.panelButtons = new System.Windows.Forms.Panel();
            this.toolTipContainerOkButton = new XenAdmin.Controls.ToolTipContainer();
            this.buttonOk = new System.Windows.Forms.Button();
            this.Cancelbutton = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBoxIscsiPort = new System.Windows.Forms.TextBox();
            this.labelColon = new System.Windows.Forms.Label();
            this.lunInUseLabel = new System.Windows.Forms.Label();
            this.toolTipContainerIQNscan = new XenAdmin.Controls.ToolTipContainer();
            this.buttonIscsiPopulateIQNs = new System.Windows.Forms.Button();
            this.textBoxIscsiHost = new System.Windows.Forms.TextBox();
            this.IscsiUseChapCheckBox = new System.Windows.Forms.CheckBox();
            this.comboBoxIscsiIqns = new System.Windows.Forms.ComboBox();
            this.labelIscsiIQN = new System.Windows.Forms.Label();
            this.comboBoxIscsiLuns = new System.Windows.Forms.ComboBox();
            this.labelIscsiTargetHost = new System.Windows.Forms.Label();
            this.labelIscsiInvalidHost = new System.Windows.Forms.Label();
            this.groupBoxChap = new XenAdmin.Controls.DecentGroupBox();
            this.IScsiChapSecretLabel = new System.Windows.Forms.Label();
            this.IScsiChapSecretTextBox = new System.Windows.Forms.TextBox();
            this.labelCHAPuser = new System.Windows.Forms.Label();
            this.IScsiChapUserTextBox = new System.Windows.Forms.TextBox();
            this.buttonIscsiPopulateLUNs = new System.Windows.Forms.Button();
            this.targetLunLabel = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.panelButtons.SuspendLayout();
            this.toolTipContainerOkButton.SuspendLayout();
            this.panel3.SuspendLayout();
            this.toolTipContainerIQNscan.SuspendLayout();
            this.groupBoxChap.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelButtons
            // 
            resources.ApplyResources(this.panelButtons, "panelButtons");
            this.panelButtons.Controls.Add(this.toolTipContainerOkButton);
            this.panelButtons.Controls.Add(this.Cancelbutton);
            this.panelButtons.Name = "panelButtons";
            // 
            // toolTipContainerOkButton
            // 
            resources.ApplyResources(this.toolTipContainerOkButton, "toolTipContainerOkButton");
            this.toolTipContainerOkButton.Controls.Add(this.buttonOk);
            this.toolTipContainerOkButton.Name = "toolTipContainerOkButton";
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // Cancelbutton
            // 
            resources.ApplyResources(this.Cancelbutton, "Cancelbutton");
            this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancelbutton.Name = "Cancelbutton";
            this.Cancelbutton.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.textBoxIscsiPort);
            this.panel3.Controls.Add(this.labelColon);
            this.panel3.Controls.Add(this.lunInUseLabel);
            this.panel3.Controls.Add(this.toolTipContainerIQNscan);
            this.panel3.Controls.Add(this.textBoxIscsiHost);
            this.panel3.Controls.Add(this.IscsiUseChapCheckBox);
            this.panel3.Controls.Add(this.comboBoxIscsiIqns);
            this.panel3.Controls.Add(this.labelIscsiIQN);
            this.panel3.Controls.Add(this.comboBoxIscsiLuns);
            this.panel3.Controls.Add(this.labelIscsiTargetHost);
            this.panel3.Controls.Add(this.labelIscsiInvalidHost);
            this.panel3.Controls.Add(this.groupBoxChap);
            this.panel3.Controls.Add(this.buttonIscsiPopulateLUNs);
            this.panel3.Controls.Add(this.targetLunLabel);
            this.panel3.Controls.Add(this.label11);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            this.panel3.TabStop = true;
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
            // lunInUseLabel
            // 
            resources.ApplyResources(this.lunInUseLabel, "lunInUseLabel");
            this.lunInUseLabel.Name = "lunInUseLabel";
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
            // textBoxIscsiHost
            // 
            resources.ApplyResources(this.textBoxIscsiHost, "textBoxIscsiHost");
            this.textBoxIscsiHost.Name = "textBoxIscsiHost";
            this.textBoxIscsiHost.TextChanged += new System.EventHandler(this.textBoxIscsiHost_TextChanged);
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
            this.comboBoxIscsiIqns.SelectedIndexChanged += new System.EventHandler(this.comboBoxIscsiIqns_SelectedIndexChanged);
            // 
            // labelIscsiIQN
            // 
            resources.ApplyResources(this.labelIscsiIQN, "labelIscsiIQN");
            this.labelIscsiIQN.BackColor = System.Drawing.Color.Transparent;
            this.labelIscsiIQN.Name = "labelIscsiIQN";
            // 
            // comboBoxIscsiLuns
            // 
            resources.ApplyResources(this.comboBoxIscsiLuns, "comboBoxIscsiLuns");
            this.comboBoxIscsiLuns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIscsiLuns.FormattingEnabled = true;
            this.comboBoxIscsiLuns.Name = "comboBoxIscsiLuns";
            this.comboBoxIscsiLuns.SelectedIndexChanged += new System.EventHandler(this.comboBoxIscsiLuns_SelectedIndexChanged);
            // 
            // labelIscsiTargetHost
            // 
            resources.ApplyResources(this.labelIscsiTargetHost, "labelIscsiTargetHost");
            this.labelIscsiTargetHost.BackColor = System.Drawing.Color.Transparent;
            this.labelIscsiTargetHost.Name = "labelIscsiTargetHost";
            // 
            // labelIscsiInvalidHost
            // 
            resources.ApplyResources(this.labelIscsiInvalidHost, "labelIscsiInvalidHost");
            this.labelIscsiInvalidHost.ForeColor = System.Drawing.Color.Red;
            this.labelIscsiInvalidHost.Name = "labelIscsiInvalidHost";
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
            // buttonIscsiPopulateLUNs
            // 
            resources.ApplyResources(this.buttonIscsiPopulateLUNs, "buttonIscsiPopulateLUNs");
            this.buttonIscsiPopulateLUNs.Name = "buttonIscsiPopulateLUNs";
            this.buttonIscsiPopulateLUNs.UseVisualStyleBackColor = true;
            this.buttonIscsiPopulateLUNs.Click += new System.EventHandler(this.buttonIscsiPopulateLUNs_Click);
            // 
            // targetLunLabel
            // 
            resources.ApplyResources(this.targetLunLabel, "targetLunLabel");
            this.targetLunLabel.Name = "targetLunLabel";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // IscsiDeviceConfigDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.Cancelbutton;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panelButtons);
            this.Name = "IscsiDeviceConfigDialog";
            this.panelButtons.ResumeLayout(false);
            this.toolTipContainerOkButton.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.toolTipContainerIQNscan.ResumeLayout(false);
            this.groupBoxChap.ResumeLayout(false);
            this.groupBoxChap.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelButtons;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerOkButton;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button Cancelbutton;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxIscsiPort;
        private System.Windows.Forms.Label labelColon;
        private System.Windows.Forms.Label lunInUseLabel;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerIQNscan;
        internal System.Windows.Forms.Button buttonIscsiPopulateIQNs;
        public System.Windows.Forms.TextBox textBoxIscsiHost;
        public System.Windows.Forms.CheckBox IscsiUseChapCheckBox;
        public System.Windows.Forms.ComboBox comboBoxIscsiIqns;
        private System.Windows.Forms.Label labelIscsiIQN;
        public System.Windows.Forms.ComboBox comboBoxIscsiLuns;
        private System.Windows.Forms.Label labelIscsiTargetHost;
        private System.Windows.Forms.Label labelIscsiInvalidHost;
        private XenAdmin.Controls.DecentGroupBox groupBoxChap;
        private System.Windows.Forms.Label IScsiChapSecretLabel;
        public System.Windows.Forms.TextBox IScsiChapSecretTextBox;
        private System.Windows.Forms.Label labelCHAPuser;
        public System.Windows.Forms.TextBox IScsiChapUserTextBox;
        internal System.Windows.Forms.Button buttonIscsiPopulateLUNs;
        private System.Windows.Forms.Label targetLunLabel;

    }
}

