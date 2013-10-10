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
            this.scanTargetHostButton = new System.Windows.Forms.Button();
            this.IscsiUseChapCheckBox = new System.Windows.Forms.CheckBox();
            this.comboBoxIscsiIqns = new System.Windows.Forms.ComboBox();
            this.comboBoxIscsiLuns = new System.Windows.Forms.ComboBox();
            this.errorLabelAtHostname = new System.Windows.Forms.Label();
            this.lunInUseLabel = new System.Windows.Forms.Label();
            this.targetLunLabel = new System.Windows.Forms.Label();
            this.IScsiChapSecretLabel = new System.Windows.Forms.Label();
            this.IScsiChapSecretTextBox = new System.Windows.Forms.TextBox();
            this.labelCHAPuser = new System.Windows.Forms.Label();
            this.IScsiChapUserTextBox = new System.Windows.Forms.TextBox();
            this.labelIscsiTargetHost = new System.Windows.Forms.Label();
            this.labelIscsiIQN = new System.Windows.Forms.Label();
            this.textBoxIscsiHost = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.placeHolderLabel2 = new System.Windows.Forms.Label();
            this.placeholderLabel = new System.Windows.Forms.Label();
            this.errorLabelAtCHAPPassword = new System.Windows.Forms.Label();
            this.errorIconAtCHAPPassword = new System.Windows.Forms.PictureBox();
            this.errorIconAtHostOrIP = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.spinnerIconAtScanTargetHostButton = new XenAdmin.Controls.SpinnerIcon();
            this.iSCSITargetGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.errorLabelAtTargetLUN = new System.Windows.Forms.Label();
            this.errorIconAtTargetLUN = new System.Windows.Forms.PictureBox();
            this.spinnerIconAtTargetIqn = new XenAdmin.Controls.SpinnerIcon();
            this.spinnerIconAtTargetLun = new XenAdmin.Controls.SpinnerIcon();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtCHAPPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtHostOrIP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtScanTargetHostButton)).BeginInit();
            this.iSCSITargetGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtTargetLUN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtTargetIqn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtTargetLun)).BeginInit();
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
            this.toolTipContainerIQNscan.Name = "toolTipContainerIQNscan";
            // 
            // scanTargetHostButton
            // 
            resources.ApplyResources(this.scanTargetHostButton, "scanTargetHostButton");
            this.tableLayoutPanel1.SetColumnSpan(this.scanTargetHostButton, 2);
            this.scanTargetHostButton.Name = "scanTargetHostButton";
            this.scanTargetHostButton.Click += new System.EventHandler(this.scanTargetHostButton_Click);
            // 
            // IscsiUseChapCheckBox
            // 
            resources.ApplyResources(this.IscsiUseChapCheckBox, "IscsiUseChapCheckBox");
            this.tableLayoutPanel1.SetColumnSpan(this.IscsiUseChapCheckBox, 2);
            this.IscsiUseChapCheckBox.Name = "IscsiUseChapCheckBox";
            this.IscsiUseChapCheckBox.UseVisualStyleBackColor = true;
            this.IscsiUseChapCheckBox.CheckedChanged += new System.EventHandler(this.IscsiUseChapCheckBox_CheckedChanged);
            // 
            // comboBoxIscsiIqns
            // 
            resources.ApplyResources(this.comboBoxIscsiIqns, "comboBoxIscsiIqns");
            this.tableLayoutPanel2.SetColumnSpan(this.comboBoxIscsiIqns, 2);
            this.comboBoxIscsiIqns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIscsiIqns.FormattingEnabled = true;
            this.comboBoxIscsiIqns.Name = "comboBoxIscsiIqns";
            this.comboBoxIscsiIqns.SelectedIndexChanged += new System.EventHandler(this.IScsiTargetIqnComboBox_SelectedIndexChanged);
            // 
            // comboBoxIscsiLuns
            // 
            resources.ApplyResources(this.comboBoxIscsiLuns, "comboBoxIscsiLuns");
            this.tableLayoutPanel2.SetColumnSpan(this.comboBoxIscsiLuns, 2);
            this.comboBoxIscsiLuns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIscsiLuns.FormattingEnabled = true;
            this.comboBoxIscsiLuns.Name = "comboBoxIscsiLuns";
            this.comboBoxIscsiLuns.SelectedIndexChanged += new System.EventHandler(this.comboBoxIscsiLuns_SelectedIndexChanged);
            // 
            // errorLabelAtHostname
            // 
            resources.ApplyResources(this.errorLabelAtHostname, "errorLabelAtHostname");
            this.tableLayoutPanel1.SetColumnSpan(this.errorLabelAtHostname, 3);
            this.errorLabelAtHostname.ForeColor = System.Drawing.Color.Red;
            this.errorLabelAtHostname.Name = "errorLabelAtHostname";
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
            // IScsiChapSecretLabel
            // 
            resources.ApplyResources(this.IScsiChapSecretLabel, "IScsiChapSecretLabel");
            this.IScsiChapSecretLabel.BackColor = System.Drawing.Color.Transparent;
            this.IScsiChapSecretLabel.Name = "IScsiChapSecretLabel";
            // 
            // IScsiChapSecretTextBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.IScsiChapSecretTextBox, 2);
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
            this.tableLayoutPanel1.SetColumnSpan(this.IScsiChapUserTextBox, 2);
            resources.ApplyResources(this.IScsiChapUserTextBox, "IScsiChapUserTextBox");
            this.IScsiChapUserTextBox.Name = "IScsiChapUserTextBox";
            this.IScsiChapUserTextBox.TextChanged += new System.EventHandler(this.ChapSettings_Changed);
            // 
            // labelIscsiTargetHost
            // 
            resources.ApplyResources(this.labelIscsiTargetHost, "labelIscsiTargetHost");
            this.labelIscsiTargetHost.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.SetColumnSpan(this.labelIscsiTargetHost, 2);
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
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxIscsiHost, 2);
            resources.ApplyResources(this.textBoxIscsiHost, "textBoxIscsiHost");
            this.textBoxIscsiHost.Name = "textBoxIscsiHost";
            this.textBoxIscsiHost.TextChanged += new System.EventHandler(this.textBoxIscsiHost_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.placeHolderLabel2, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.placeholderLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.errorLabelAtCHAPPassword, 3, 7);
            this.tableLayoutPanel1.Controls.Add(this.errorIconAtCHAPPassword, 2, 7);
            this.tableLayoutPanel1.Controls.Add(this.errorIconAtHostOrIP, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.scanTargetHostButton, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.labelIscsiTargetHost, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxIscsiHost, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelColon, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxIscsiPort, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.IScsiChapUserTextBox, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.IScsiChapSecretTextBox, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelCHAPuser, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.IScsiChapSecretLabel, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.IscsiUseChapCheckBox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.errorLabelAtHostname, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.spinnerIconAtScanTargetHostButton, 2, 8);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // placeHolderLabel2
            // 
            resources.ApplyResources(this.placeHolderLabel2, "placeHolderLabel2");
            this.placeHolderLabel2.ForeColor = System.Drawing.Color.Red;
            this.placeHolderLabel2.Name = "placeHolderLabel2";
            // 
            // placeholderLabel
            // 
            resources.ApplyResources(this.placeholderLabel, "placeholderLabel");
            this.placeholderLabel.ForeColor = System.Drawing.Color.Red;
            this.placeholderLabel.Name = "placeholderLabel";
            // 
            // errorLabelAtCHAPPassword
            // 
            resources.ApplyResources(this.errorLabelAtCHAPPassword, "errorLabelAtCHAPPassword");
            this.tableLayoutPanel1.SetColumnSpan(this.errorLabelAtCHAPPassword, 3);
            this.errorLabelAtCHAPPassword.ForeColor = System.Drawing.Color.Red;
            this.errorLabelAtCHAPPassword.Name = "errorLabelAtCHAPPassword";
            // 
            // errorIconAtCHAPPassword
            // 
            resources.ApplyResources(this.errorIconAtCHAPPassword, "errorIconAtCHAPPassword");
            this.errorIconAtCHAPPassword.ErrorImage = null;
            this.errorIconAtCHAPPassword.InitialImage = null;
            this.errorIconAtCHAPPassword.Name = "errorIconAtCHAPPassword";
            this.errorIconAtCHAPPassword.TabStop = false;
            // 
            // errorIconAtHostOrIP
            // 
            resources.ApplyResources(this.errorIconAtHostOrIP, "errorIconAtHostOrIP");
            this.errorIconAtHostOrIP.ErrorImage = null;
            this.errorIconAtHostOrIP.InitialImage = null;
            this.errorIconAtHostOrIP.Name = "errorIconAtHostOrIP";
            this.errorIconAtHostOrIP.TabStop = false;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.tableLayoutPanel1.SetColumnSpan(this.label11, 6);
            this.label11.Name = "label11";
            // 
            // spinnerIconAtScanTargetHostButton
            // 
            resources.ApplyResources(this.spinnerIconAtScanTargetHostButton, "spinnerIconAtScanTargetHostButton");
            this.spinnerIconAtScanTargetHostButton.Name = "spinnerIconAtScanTargetHostButton";
            this.spinnerIconAtScanTargetHostButton.SucceededImage = global::XenAdmin.Properties.Resources._000_Tick_h32bit_16;
            this.spinnerIconAtScanTargetHostButton.TabStop = false;
            // 
            // iSCSITargetGroupBox
            // 
            resources.ApplyResources(this.iSCSITargetGroupBox, "iSCSITargetGroupBox");
            this.iSCSITargetGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.iSCSITargetGroupBox.Name = "iSCSITargetGroupBox";
            this.iSCSITargetGroupBox.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.labelIscsiIQN, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.comboBoxIscsiIqns, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.comboBoxIscsiLuns, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.targetLunLabel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.errorLabelAtTargetLUN, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.errorIconAtTargetLUN, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.spinnerIconAtTargetIqn, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.spinnerIconAtTargetLun, 3, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // errorLabelAtTargetLUN
            // 
            resources.ApplyResources(this.errorLabelAtTargetLUN, "errorLabelAtTargetLUN");
            this.tableLayoutPanel2.SetColumnSpan(this.errorLabelAtTargetLUN, 2);
            this.errorLabelAtTargetLUN.ForeColor = System.Drawing.Color.Red;
            this.errorLabelAtTargetLUN.Name = "errorLabelAtTargetLUN";
            // 
            // errorIconAtTargetLUN
            // 
            resources.ApplyResources(this.errorIconAtTargetLUN, "errorIconAtTargetLUN");
            this.errorIconAtTargetLUN.ErrorImage = null;
            this.errorIconAtTargetLUN.InitialImage = null;
            this.errorIconAtTargetLUN.Name = "errorIconAtTargetLUN";
            this.errorIconAtTargetLUN.TabStop = false;
            // 
            // spinnerIconAtTargetIqn
            // 
            resources.ApplyResources(this.spinnerIconAtTargetIqn, "spinnerIconAtTargetIqn");
            this.spinnerIconAtTargetIqn.Name = "spinnerIconAtTargetIqn";
            this.spinnerIconAtTargetIqn.SucceededImage = global::XenAdmin.Properties.Resources._000_Tick_h32bit_16;
            this.spinnerIconAtTargetIqn.TabStop = false;
            // 
            // spinnerIconAtTargetLun
            // 
            resources.ApplyResources(this.spinnerIconAtTargetLun, "spinnerIconAtTargetLun");
            this.spinnerIconAtTargetLun.Name = "spinnerIconAtTargetLun";
            this.spinnerIconAtTargetLun.SucceededImage = global::XenAdmin.Properties.Resources._000_Tick_h32bit_16;
            this.spinnerIconAtTargetLun.TabStop = false;
            // 
            // LVMoISCSI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.iSCSITargetGroupBox);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolTipContainerIQNscan);
            this.Name = "LVMoISCSI";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtCHAPPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtHostOrIP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtScanTargetHostButton)).EndInit();
            this.iSCSITargetGroupBox.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtTargetLUN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtTargetIqn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtTargetLun)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxIscsiPort;
        private System.Windows.Forms.Label labelColon;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerIQNscan;
        private System.Windows.Forms.Button scanTargetHostButton;
        private System.Windows.Forms.CheckBox IscsiUseChapCheckBox;
        private System.Windows.Forms.ComboBox comboBoxIscsiIqns;
        private System.Windows.Forms.ComboBox comboBoxIscsiLuns;
        private System.Windows.Forms.Label errorLabelAtHostname;
        private System.Windows.Forms.Label lunInUseLabel;
        private System.Windows.Forms.Label targetLunLabel;
        private System.Windows.Forms.Label IScsiChapSecretLabel;
        private System.Windows.Forms.TextBox IScsiChapSecretTextBox;
        private System.Windows.Forms.Label labelCHAPuser;
        private System.Windows.Forms.TextBox IScsiChapUserTextBox;
        private System.Windows.Forms.Label labelIscsiTargetHost;
        private System.Windows.Forms.Label labelIscsiIQN;
        private System.Windows.Forms.TextBox textBoxIscsiHost;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.DecentGroupBox iSCSITargetGroupBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox errorIconAtTargetLUN;
        private System.Windows.Forms.PictureBox errorIconAtHostOrIP;
        private System.Windows.Forms.PictureBox errorIconAtCHAPPassword;
        private System.Windows.Forms.Label errorLabelAtCHAPPassword;
        private System.Windows.Forms.Label errorLabelAtTargetLUN;
        private XenAdmin.Controls.SpinnerIcon spinnerIconAtTargetIqn;
        private XenAdmin.Controls.SpinnerIcon spinnerIconAtTargetLun;
        private XenAdmin.Controls.SpinnerIcon spinnerIconAtScanTargetHostButton;
        private System.Windows.Forms.Label placeholderLabel;
        private System.Windows.Forms.Label placeHolderLabel2;
    }
}
