namespace XenAdmin.SettingsPanels
{
    partial class PerfmonAlertOptionsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerfmonAlertOptionsPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.SendEmailNoteLabel = new System.Windows.Forms.Label();
            this.EmailNotificationCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new XenAdmin.Controls.DecentGroupBox();
            this.MailLanguageComboBox = new System.Windows.Forms.ComboBox();
            this.MailLanguageLabel = new System.Windows.Forms.Label();
            this.SmtpPortLabel = new System.Windows.Forms.Label();
            this.SmtpServerPortTextBox = new System.Windows.Forms.TextBox();
            this.EmailAddressTextBox = new System.Windows.Forms.TextBox();
            this.EmailAddressLabel = new System.Windows.Forms.Label();
            this.SmtpServerAddrTextBox = new System.Windows.Forms.TextBox();
            this.SmtpServerLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.SendEmailNoteLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.EmailNotificationCheckBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // SendEmailNoteLabel
            // 
            resources.ApplyResources(this.SendEmailNoteLabel, "SendEmailNoteLabel");
            this.SendEmailNoteLabel.Name = "SendEmailNoteLabel";
            // 
            // EmailNotificationCheckBox
            // 
            resources.ApplyResources(this.EmailNotificationCheckBox, "EmailNotificationCheckBox");
            this.EmailNotificationCheckBox.Name = "EmailNotificationCheckBox";
            this.EmailNotificationCheckBox.UseVisualStyleBackColor = false;
            this.EmailNotificationCheckBox.CheckedChanged += new System.EventHandler(this.EmailNotificationCheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.MailLanguageComboBox);
            this.groupBox1.Controls.Add(this.MailLanguageLabel);
            this.groupBox1.Controls.Add(this.SmtpPortLabel);
            this.groupBox1.Controls.Add(this.SmtpServerPortTextBox);
            this.groupBox1.Controls.Add(this.EmailAddressTextBox);
            this.groupBox1.Controls.Add(this.EmailAddressLabel);
            this.groupBox1.Controls.Add(this.SmtpServerAddrTextBox);
            this.groupBox1.Controls.Add(this.SmtpServerLabel);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // MailLanguageComboBox
            // 
            resources.ApplyResources(this.MailLanguageComboBox, "MailLanguageComboBox");
            this.MailLanguageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MailLanguageComboBox.FormattingEnabled = true;
            this.MailLanguageComboBox.Name = "MailLanguageComboBox";
            // 
            // MailLanguageLabel
            // 
            resources.ApplyResources(this.MailLanguageLabel, "MailLanguageLabel");
            this.MailLanguageLabel.Name = "MailLanguageLabel";
            // 
            // SmtpPortLabel
            // 
            resources.ApplyResources(this.SmtpPortLabel, "SmtpPortLabel");
            this.SmtpPortLabel.Name = "SmtpPortLabel";
            // 
            // SmtpServerPortTextBox
            // 
            resources.ApplyResources(this.SmtpServerPortTextBox, "SmtpServerPortTextBox");
            this.SmtpServerPortTextBox.Name = "SmtpServerPortTextBox";
            // 
            // EmailAddressTextBox
            // 
            resources.ApplyResources(this.EmailAddressTextBox, "EmailAddressTextBox");
            this.EmailAddressTextBox.Name = "EmailAddressTextBox";
            // 
            // EmailAddressLabel
            // 
            resources.ApplyResources(this.EmailAddressLabel, "EmailAddressLabel");
            this.EmailAddressLabel.Name = "EmailAddressLabel";
            // 
            // SmtpServerAddrTextBox
            // 
            resources.ApplyResources(this.SmtpServerAddrTextBox, "SmtpServerAddrTextBox");
            this.SmtpServerAddrTextBox.Name = "SmtpServerAddrTextBox";
            // 
            // SmtpServerLabel
            // 
            resources.ApplyResources(this.SmtpServerLabel, "SmtpServerLabel");
            this.SmtpServerLabel.Name = "SmtpServerLabel";
            // 
            // PerfmonAlertOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PerfmonAlertOptionsPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label EmailAddressLabel;
        private System.Windows.Forms.Label SmtpServerLabel;
        private System.Windows.Forms.TextBox SmtpServerPortTextBox;
        private System.Windows.Forms.Label SmtpPortLabel;
        private System.Windows.Forms.TextBox SmtpServerAddrTextBox;
        private System.Windows.Forms.TextBox EmailAddressTextBox;
        private XenAdmin.Controls.DecentGroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label SendEmailNoteLabel;
        private System.Windows.Forms.CheckBox EmailNotificationCheckBox;
        private System.Windows.Forms.Label MailLanguageLabel;
        private System.Windows.Forms.ComboBox MailLanguageComboBox;
    }
}
