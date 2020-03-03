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
            this.lunInUseLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.iSCSITargetGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.labelTargetIqn = new System.Windows.Forms.Label();
            this.comboBoxIscsiIqns = new System.Windows.Forms.ComboBox();
            this.comboBoxIscsiLuns = new System.Windows.Forms.ComboBox();
            this.labelTargetLun = new System.Windows.Forms.Label();
            this.spinnerIconAtTargetIqn = new XenAdmin.Controls.SpinnerIcon();
            this.spinnerIconAtTargetLun = new XenAdmin.Controls.SpinnerIcon();
            this.errorIconAtTargetLUN = new System.Windows.Forms.PictureBox();
            this.errorLabelAtTargetLUN = new System.Windows.Forms.Label();
            this.buttonScanTargetHost = new System.Windows.Forms.Button();
            this.labelIscsiTargetHost = new System.Windows.Forms.Label();
            this.textBoxIscsiHost = new System.Windows.Forms.TextBox();
            this.labelColon = new System.Windows.Forms.Label();
            this.textBoxIscsiPort = new System.Windows.Forms.TextBox();
            this.textBoxChapUser = new System.Windows.Forms.TextBox();
            this.textBoxChapPassword = new System.Windows.Forms.TextBox();
            this.labelCHAPuser = new System.Windows.Forms.Label();
            this.IScsiChapSecretLabel = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.spinnerIconAtScanTargetHostButton = new XenAdmin.Controls.SpinnerIcon();
            this.tableLayoutPanelError = new System.Windows.Forms.TableLayoutPanel();
            this.errorIconBottom = new System.Windows.Forms.PictureBox();
            this.errorLabelBottom = new System.Windows.Forms.Label();
            this.errorIconAtHostOrIP = new System.Windows.Forms.PictureBox();
            this.errorLabelAtHostOrIP = new System.Windows.Forms.Label();
            this.errorIconAtCHAPPassword = new System.Windows.Forms.PictureBox();
            this.errorLabelAtCHAPPassword = new System.Windows.Forms.Label();
            this.checkBoxUseChap = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.iSCSITargetGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtTargetIqn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtTargetLun)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtTargetLUN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtScanTargetHostButton)).BeginInit();
            this.tableLayoutPanelError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtHostOrIP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtCHAPPassword)).BeginInit();
            this.SuspendLayout();
            // 
            // lunInUseLabel
            // 
            resources.ApplyResources(this.lunInUseLabel, "lunInUseLabel");
            this.lunInUseLabel.Name = "lunInUseLabel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelIscsiTargetHost, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxIscsiHost, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelColon, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxIscsiPort, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.errorIconAtHostOrIP, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.errorLabelAtHostOrIP, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxUseChap, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelCHAPuser, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.textBoxChapUser, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.IScsiChapSecretLabel, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.textBoxChapPassword, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.errorIconAtCHAPPassword, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.errorLabelAtCHAPPassword, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.buttonScanTargetHost, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.spinnerIconAtScanTargetHostButton, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.iSCSITargetGroupBox, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelError, 0, 9);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // iSCSITargetGroupBox
            // 
            resources.ApplyResources(this.iSCSITargetGroupBox, "iSCSITargetGroupBox");
            this.tableLayoutPanel1.SetColumnSpan(this.iSCSITargetGroupBox, 5);
            this.iSCSITargetGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.iSCSITargetGroupBox.Name = "iSCSITargetGroupBox";
            this.iSCSITargetGroupBox.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.labelTargetIqn, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.comboBoxIscsiIqns, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.spinnerIconAtTargetIqn, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelTargetLun, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.comboBoxIscsiLuns, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.spinnerIconAtTargetLun, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.errorIconAtTargetLUN, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.errorLabelAtTargetLUN, 2, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // labelTargetIqn
            // 
            resources.ApplyResources(this.labelTargetIqn, "labelTargetIqn");
            this.labelTargetIqn.BackColor = System.Drawing.Color.Transparent;
            this.labelTargetIqn.Name = "labelTargetIqn";
            // 
            // comboBoxIscsiIqns
            // 
            resources.ApplyResources(this.comboBoxIscsiIqns, "comboBoxIscsiIqns");
            this.tableLayoutPanel2.SetColumnSpan(this.comboBoxIscsiIqns, 2);
            this.comboBoxIscsiIqns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIscsiIqns.FormattingEnabled = true;
            this.comboBoxIscsiIqns.Name = "comboBoxIscsiIqns";
            this.comboBoxIscsiIqns.SelectedIndexChanged += new System.EventHandler(this.comboBoxIscsiIqns_SelectedIndexChanged);
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
            // labelTargetLun
            // 
            resources.ApplyResources(this.labelTargetLun, "labelTargetLun");
            this.labelTargetLun.Name = "labelTargetLun";
            // 
            // spinnerIconAtTargetIqn
            // 
            resources.ApplyResources(this.spinnerIconAtTargetIqn, "spinnerIconAtTargetIqn");
            this.spinnerIconAtTargetIqn.Name = "spinnerIconAtTargetIqn";
            this.spinnerIconAtTargetIqn.TabStop = false;
            // 
            // spinnerIconAtTargetLun
            // 
            resources.ApplyResources(this.spinnerIconAtTargetLun, "spinnerIconAtTargetLun");
            this.spinnerIconAtTargetLun.Name = "spinnerIconAtTargetLun";
            this.spinnerIconAtTargetLun.TabStop = false;
            // 
            // errorIconAtTargetLUN
            // 
            this.errorIconAtTargetLUN.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            resources.ApplyResources(this.errorIconAtTargetLUN, "errorIconAtTargetLUN");
            this.errorIconAtTargetLUN.Name = "errorIconAtTargetLUN";
            this.errorIconAtTargetLUN.TabStop = false;
            // 
            // errorLabelAtTargetLUN
            // 
            resources.ApplyResources(this.errorLabelAtTargetLUN, "errorLabelAtTargetLUN");
            this.errorLabelAtTargetLUN.ForeColor = System.Drawing.Color.Red;
            this.errorLabelAtTargetLUN.Name = "errorLabelAtTargetLUN";
            // 
            // buttonScanTargetHost
            // 
            resources.ApplyResources(this.buttonScanTargetHost, "buttonScanTargetHost");
            this.buttonScanTargetHost.Name = "buttonScanTargetHost";
            this.buttonScanTargetHost.Click += new System.EventHandler(this.buttonScanTargetHost_Click);
            // 
            // labelIscsiTargetHost
            // 
            resources.ApplyResources(this.labelIscsiTargetHost, "labelIscsiTargetHost");
            this.labelIscsiTargetHost.BackColor = System.Drawing.Color.Transparent;
            this.labelIscsiTargetHost.Name = "labelIscsiTargetHost";
            // 
            // textBoxIscsiHost
            // 
            resources.ApplyResources(this.textBoxIscsiHost, "textBoxIscsiHost");
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxIscsiHost, 2);
            this.textBoxIscsiHost.Name = "textBoxIscsiHost";
            this.textBoxIscsiHost.TextChanged += new System.EventHandler(this.textBoxIscsiHost_TextChanged);
            // 
            // labelColon
            // 
            resources.ApplyResources(this.labelColon, "labelColon");
            this.labelColon.Name = "labelColon";
            // 
            // textBoxIscsiPort
            // 
            resources.ApplyResources(this.textBoxIscsiPort, "textBoxIscsiPort");
            this.textBoxIscsiPort.Name = "textBoxIscsiPort";
            this.textBoxIscsiPort.TextChanged += new System.EventHandler(this.textBoxIscsiHost_TextChanged);
            // 
            // textBoxChapUser
            // 
            this.textBoxChapUser.AllowDrop = true;
            resources.ApplyResources(this.textBoxChapUser, "textBoxChapUser");
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxChapUser, 2);
            this.textBoxChapUser.Name = "textBoxChapUser";
            this.textBoxChapUser.TextChanged += new System.EventHandler(this.textBoxChapUser_TextChanged);
            // 
            // textBoxChapPassword
            // 
            resources.ApplyResources(this.textBoxChapPassword, "textBoxChapPassword");
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxChapPassword, 2);
            this.textBoxChapPassword.Name = "textBoxChapPassword";
            this.textBoxChapPassword.UseSystemPasswordChar = true;
            this.textBoxChapPassword.TextChanged += new System.EventHandler(this.textBoxChapPassword_TextChanged);
            // 
            // labelCHAPuser
            // 
            resources.ApplyResources(this.labelCHAPuser, "labelCHAPuser");
            this.labelCHAPuser.BackColor = System.Drawing.Color.Transparent;
            this.labelCHAPuser.Name = "labelCHAPuser";
            // 
            // IScsiChapSecretLabel
            // 
            resources.ApplyResources(this.IScsiChapSecretLabel, "IScsiChapSecretLabel");
            this.IScsiChapSecretLabel.BackColor = System.Drawing.Color.Transparent;
            this.IScsiChapSecretLabel.Name = "IScsiChapSecretLabel";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.tableLayoutPanel1.SetColumnSpan(this.label11, 5);
            this.label11.Name = "label11";
            // 
            // spinnerIconAtScanTargetHostButton
            // 
            resources.ApplyResources(this.spinnerIconAtScanTargetHostButton, "spinnerIconAtScanTargetHostButton");
            this.spinnerIconAtScanTargetHostButton.Name = "spinnerIconAtScanTargetHostButton";
            this.spinnerIconAtScanTargetHostButton.TabStop = false;
            // 
            // tableLayoutPanelError
            // 
            resources.ApplyResources(this.tableLayoutPanelError, "tableLayoutPanelError");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelError, 5);
            this.tableLayoutPanelError.Controls.Add(this.errorIconBottom, 0, 0);
            this.tableLayoutPanelError.Controls.Add(this.errorLabelBottom, 1, 0);
            this.tableLayoutPanelError.Name = "tableLayoutPanelError";
            // 
            // errorIconBottom
            // 
            this.errorIconBottom.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            resources.ApplyResources(this.errorIconBottom, "errorIconBottom");
            this.errorIconBottom.Name = "errorIconBottom";
            this.errorIconBottom.TabStop = false;
            // 
            // errorLabelBottom
            // 
            resources.ApplyResources(this.errorLabelBottom, "errorLabelBottom");
            this.errorLabelBottom.ForeColor = System.Drawing.Color.Red;
            this.errorLabelBottom.Name = "errorLabelBottom";
            // 
            // errorIconAtHostOrIP
            // 
            this.errorIconAtHostOrIP.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            resources.ApplyResources(this.errorIconAtHostOrIP, "errorIconAtHostOrIP");
            this.errorIconAtHostOrIP.Name = "errorIconAtHostOrIP";
            this.errorIconAtHostOrIP.TabStop = false;
            // 
            // errorLabelAtHostOrIP
            // 
            resources.ApplyResources(this.errorLabelAtHostOrIP, "errorLabelAtHostOrIP");
            this.tableLayoutPanel1.SetColumnSpan(this.errorLabelAtHostOrIP, 3);
            this.errorLabelAtHostOrIP.ForeColor = System.Drawing.Color.Red;
            this.errorLabelAtHostOrIP.Name = "errorLabelAtHostOrIP";
            // 
            // errorIconAtCHAPPassword
            // 
            this.errorIconAtCHAPPassword.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            resources.ApplyResources(this.errorIconAtCHAPPassword, "errorIconAtCHAPPassword");
            this.errorIconAtCHAPPassword.Name = "errorIconAtCHAPPassword";
            this.errorIconAtCHAPPassword.TabStop = false;
            // 
            // errorLabelAtCHAPPassword
            // 
            resources.ApplyResources(this.errorLabelAtCHAPPassword, "errorLabelAtCHAPPassword");
            this.tableLayoutPanel1.SetColumnSpan(this.errorLabelAtCHAPPassword, 3);
            this.errorLabelAtCHAPPassword.ForeColor = System.Drawing.Color.Red;
            this.errorLabelAtCHAPPassword.Name = "errorLabelAtCHAPPassword";
            // 
            // checkBoxUseChap
            // 
            resources.ApplyResources(this.checkBoxUseChap, "checkBoxUseChap");
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxUseChap, 5);
            this.checkBoxUseChap.Name = "checkBoxUseChap";
            this.checkBoxUseChap.UseVisualStyleBackColor = true;
            this.checkBoxUseChap.CheckedChanged += new System.EventHandler(this.checkBoxUseChap_CheckedChanged);
            // 
            // LVMoISCSI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LVMoISCSI";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.iSCSITargetGroupBox.ResumeLayout(false);
            this.iSCSITargetGroupBox.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtTargetIqn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtTargetLun)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtTargetLUN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtScanTargetHostButton)).EndInit();
            this.tableLayoutPanelError.ResumeLayout(false);
            this.tableLayoutPanelError.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtHostOrIP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtCHAPPassword)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxIscsiPort;
        private System.Windows.Forms.Label labelColon;
        private System.Windows.Forms.Button buttonScanTargetHost;
        private System.Windows.Forms.CheckBox checkBoxUseChap;
        private System.Windows.Forms.ComboBox comboBoxIscsiIqns;
        private System.Windows.Forms.ComboBox comboBoxIscsiLuns;
        private System.Windows.Forms.Label lunInUseLabel;
        private System.Windows.Forms.Label labelTargetLun;
        private System.Windows.Forms.Label IScsiChapSecretLabel;
        private System.Windows.Forms.TextBox textBoxChapPassword;
        private System.Windows.Forms.Label labelCHAPuser;
        private System.Windows.Forms.TextBox textBoxChapUser;
        private System.Windows.Forms.Label labelIscsiTargetHost;
        private System.Windows.Forms.Label labelTargetIqn;
        private System.Windows.Forms.TextBox textBoxIscsiHost;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.DecentGroupBox iSCSITargetGroupBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private XenAdmin.Controls.SpinnerIcon spinnerIconAtTargetIqn;
        private XenAdmin.Controls.SpinnerIcon spinnerIconAtTargetLun;
        private XenAdmin.Controls.SpinnerIcon spinnerIconAtScanTargetHostButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelError;
        private System.Windows.Forms.PictureBox errorIconBottom;
        private System.Windows.Forms.Label errorLabelBottom;
        private System.Windows.Forms.PictureBox errorIconAtTargetLUN;
        private System.Windows.Forms.Label errorLabelAtTargetLUN;
        private System.Windows.Forms.PictureBox errorIconAtHostOrIP;
        private System.Windows.Forms.Label errorLabelAtHostOrIP;
        private System.Windows.Forms.PictureBox errorIconAtCHAPPassword;
        private System.Windows.Forms.Label errorLabelAtCHAPPassword;
    }
}
