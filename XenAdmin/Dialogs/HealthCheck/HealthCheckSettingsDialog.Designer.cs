namespace XenAdmin.Dialogs.HealthCheck
{
    partial class HealthCheckSettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HealthCheckSettingsDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.decentGroupBox2 = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.frequencyLabel = new System.Windows.Forms.Label();
            this.frequencyNumericBox = new System.Windows.Forms.NumericUpDown();
            this.weeksLabel = new System.Windows.Forms.Label();
            this.dayOfweekLabel = new System.Windows.Forms.Label();
            this.timeOfDayLabel = new System.Windows.Forms.Label();
            this.timeOfDayComboBox = new System.Windows.Forms.ComboBox();
            this.dayOfWeekComboBox = new System.Windows.Forms.ComboBox();
            this.decentGroupBox1 = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxMyCitrixPassword = new System.Windows.Forms.TextBox();
            this.textBoxMyCitrixUsername = new System.Windows.Forms.TextBox();
            this.existingAuthenticationRadioButton = new System.Windows.Forms.RadioButton();
            this.newAuthenticationRadioButton = new System.Windows.Forms.RadioButton();
            this.authRubricLinkLabel = new System.Windows.Forms.LinkLabel();
            this.authRubricTextLabel = new System.Windows.Forms.Label();
            this.rubricLabel = new System.Windows.Forms.Label();
            this.PolicyStatementLinkLabel = new System.Windows.Forms.LinkLabel();
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.enrollmentCheckBox = new System.Windows.Forms.CheckBox();
            this.decentGroupBoxXSCredentials = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.errorLabel = new System.Windows.Forms.Label();
            this.testCredentialsStatusImage = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textboxXSPassword = new System.Windows.Forms.TextBox();
            this.textboxXSUserName = new System.Windows.Forms.TextBox();
            this.currentXsCredentialsRadioButton = new System.Windows.Forms.RadioButton();
            this.newXsCredentialsRadioButton = new System.Windows.Forms.RadioButton();
            this.testCredentialsButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.decentGroupBox2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericBox)).BeginInit();
            this.decentGroupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.decentGroupBoxXSCredentials.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testCredentialsStatusImage)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.decentGroupBox2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.decentGroupBox1, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.rubricLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.PolicyStatementLinkLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_ctrlError, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.enrollmentCheckBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.decentGroupBoxXSCredentials, 0, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // decentGroupBox2
            // 
            resources.ApplyResources(this.decentGroupBox2, "decentGroupBox2");
            this.tableLayoutPanel1.SetColumnSpan(this.decentGroupBox2, 2);
            this.decentGroupBox2.Controls.Add(this.tableLayoutPanel4);
            this.decentGroupBox2.Name = "decentGroupBox2";
            this.decentGroupBox2.TabStop = false;
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.frequencyLabel, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.frequencyNumericBox, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.weeksLabel, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.dayOfweekLabel, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.timeOfDayLabel, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.timeOfDayComboBox, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.dayOfWeekComboBox, 1, 2);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // frequencyLabel
            // 
            resources.ApplyResources(this.frequencyLabel, "frequencyLabel");
            this.frequencyLabel.Name = "frequencyLabel";
            // 
            // frequencyNumericBox
            // 
            resources.ApplyResources(this.frequencyNumericBox, "frequencyNumericBox");
            this.frequencyNumericBox.Maximum = new decimal(new int[] {
            52,
            0,
            0,
            0});
            this.frequencyNumericBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.frequencyNumericBox.Name = "frequencyNumericBox";
            this.frequencyNumericBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // weeksLabel
            // 
            resources.ApplyResources(this.weeksLabel, "weeksLabel");
            this.weeksLabel.Name = "weeksLabel";
            // 
            // dayOfweekLabel
            // 
            resources.ApplyResources(this.dayOfweekLabel, "dayOfweekLabel");
            this.dayOfweekLabel.Name = "dayOfweekLabel";
            // 
            // timeOfDayLabel
            // 
            resources.ApplyResources(this.timeOfDayLabel, "timeOfDayLabel");
            this.timeOfDayLabel.Name = "timeOfDayLabel";
            // 
            // timeOfDayComboBox
            // 
            this.tableLayoutPanel4.SetColumnSpan(this.timeOfDayComboBox, 2);
            this.timeOfDayComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.timeOfDayComboBox, "timeOfDayComboBox");
            this.timeOfDayComboBox.FormattingEnabled = true;
            this.timeOfDayComboBox.Name = "timeOfDayComboBox";
            // 
            // dayOfWeekComboBox
            // 
            this.tableLayoutPanel4.SetColumnSpan(this.dayOfWeekComboBox, 2);
            this.dayOfWeekComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.dayOfWeekComboBox, "dayOfWeekComboBox");
            this.dayOfWeekComboBox.FormattingEnabled = true;
            this.dayOfWeekComboBox.Name = "dayOfWeekComboBox";
            // 
            // decentGroupBox1
            // 
            resources.ApplyResources(this.decentGroupBox1, "decentGroupBox1");
            this.tableLayoutPanel1.SetColumnSpan(this.decentGroupBox1, 2);
            this.decentGroupBox1.Controls.Add(this.tableLayoutPanel2);
            this.decentGroupBox1.Name = "decentGroupBox1";
            this.decentGroupBox1.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMyCitrixPassword, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMyCitrixUsername, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.existingAuthenticationRadioButton, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.newAuthenticationRadioButton, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.authRubricLinkLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.authRubricTextLabel, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxMyCitrixPassword
            // 
            resources.ApplyResources(this.textBoxMyCitrixPassword, "textBoxMyCitrixPassword");
            this.textBoxMyCitrixPassword.Name = "textBoxMyCitrixPassword";
            this.textBoxMyCitrixPassword.UseSystemPasswordChar = true;
            this.textBoxMyCitrixPassword.TextChanged += new System.EventHandler(this.credentials_TextChanged);
            // 
            // textBoxMyCitrixUsername
            // 
            resources.ApplyResources(this.textBoxMyCitrixUsername, "textBoxMyCitrixUsername");
            this.textBoxMyCitrixUsername.Name = "textBoxMyCitrixUsername";
            this.textBoxMyCitrixUsername.TextChanged += new System.EventHandler(this.credentials_TextChanged);
            // 
            // existingAuthenticationRadioButton
            // 
            resources.ApplyResources(this.existingAuthenticationRadioButton, "existingAuthenticationRadioButton");
            this.tableLayoutPanel2.SetColumnSpan(this.existingAuthenticationRadioButton, 2);
            this.existingAuthenticationRadioButton.Name = "existingAuthenticationRadioButton";
            this.existingAuthenticationRadioButton.TabStop = true;
            this.existingAuthenticationRadioButton.UseVisualStyleBackColor = true;
            this.existingAuthenticationRadioButton.CheckedChanged += new System.EventHandler(this.existingAuthenticationRadioButton_CheckedChanged);
            // 
            // newAuthenticationRadioButton
            // 
            resources.ApplyResources(this.newAuthenticationRadioButton, "newAuthenticationRadioButton");
            this.tableLayoutPanel2.SetColumnSpan(this.newAuthenticationRadioButton, 2);
            this.newAuthenticationRadioButton.Name = "newAuthenticationRadioButton";
            this.newAuthenticationRadioButton.TabStop = true;
            this.newAuthenticationRadioButton.UseVisualStyleBackColor = true;
            this.newAuthenticationRadioButton.CheckedChanged += new System.EventHandler(this.newAuthenticationRadioButton_CheckedChanged);
            // 
            // authRubricLinkLabel
            // 
            resources.ApplyResources(this.authRubricLinkLabel, "authRubricLinkLabel");
            this.tableLayoutPanel2.SetColumnSpan(this.authRubricLinkLabel, 2);
            this.authRubricLinkLabel.Name = "authRubricLinkLabel";
            this.authRubricLinkLabel.TabStop = true;
            this.authRubricLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.authRubricLinkLabel_LinkClicked);
            // 
            // authRubricTextLabel
            // 
            resources.ApplyResources(this.authRubricTextLabel, "authRubricTextLabel");
            this.tableLayoutPanel2.SetColumnSpan(this.authRubricTextLabel, 2);
            this.authRubricTextLabel.Name = "authRubricTextLabel";
            // 
            // rubricLabel
            // 
            resources.ApplyResources(this.rubricLabel, "rubricLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.rubricLabel, 2);
            this.rubricLabel.Name = "rubricLabel";
            // 
            // PolicyStatementLinkLabel
            // 
            resources.ApplyResources(this.PolicyStatementLinkLabel, "PolicyStatementLinkLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.PolicyStatementLinkLabel, 2);
            this.PolicyStatementLinkLabel.Name = "PolicyStatementLinkLabel";
            this.PolicyStatementLinkLabel.TabStop = true;
            this.PolicyStatementLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PolicyStatementLinkLabel_LinkClicked);
            // 
            // m_ctrlError
            // 
            resources.ApplyResources(this.m_ctrlError, "m_ctrlError");
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.cancelButton);
            this.flowLayoutPanel1.Controls.Add(this.okButton);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // enrollmentCheckBox
            // 
            resources.ApplyResources(this.enrollmentCheckBox, "enrollmentCheckBox");
            this.tableLayoutPanel1.SetColumnSpan(this.enrollmentCheckBox, 2);
            this.enrollmentCheckBox.Name = "enrollmentCheckBox";
            this.enrollmentCheckBox.UseVisualStyleBackColor = true;
            this.enrollmentCheckBox.CheckedChanged += new System.EventHandler(this.enrollmentCheckBox_CheckedChanged);
            // 
            // decentGroupBoxXSCredentials
            // 
            resources.ApplyResources(this.decentGroupBoxXSCredentials, "decentGroupBoxXSCredentials");
            this.tableLayoutPanel1.SetColumnSpan(this.decentGroupBoxXSCredentials, 2);
            this.decentGroupBoxXSCredentials.Controls.Add(this.tableLayoutPanel3);
            this.decentGroupBoxXSCredentials.Name = "decentGroupBoxXSCredentials";
            this.decentGroupBoxXSCredentials.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.errorLabel, 3, 5);
            this.tableLayoutPanel3.Controls.Add(this.testCredentialsStatusImage, 2, 5);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.textboxXSPassword, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.textboxXSUserName, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.currentXsCredentialsRadioButton, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.newXsCredentialsRadioButton, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.testCredentialsButton, 1, 5);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // errorLabel
            // 
            this.errorLabel.AutoEllipsis = true;
            resources.ApplyResources(this.errorLabel, "errorLabel");
            this.errorLabel.ForeColor = System.Drawing.Color.Red;
            this.errorLabel.Name = "errorLabel";
            // 
            // testCredentialsStatusImage
            // 
            resources.ApplyResources(this.testCredentialsStatusImage, "testCredentialsStatusImage");
            this.testCredentialsStatusImage.Name = "testCredentialsStatusImage";
            this.testCredentialsStatusImage.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.tableLayoutPanel3.SetColumnSpan(this.label3, 4);
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // textboxXSPassword
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.textboxXSPassword, 3);
            resources.ApplyResources(this.textboxXSPassword, "textboxXSPassword");
            this.textboxXSPassword.Name = "textboxXSPassword";
            this.textboxXSPassword.UseSystemPasswordChar = true;
            this.textboxXSPassword.TextChanged += new System.EventHandler(this.xsCredentials_TextChanged);
            // 
            // textboxXSUserName
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.textboxXSUserName, 3);
            resources.ApplyResources(this.textboxXSUserName, "textboxXSUserName");
            this.textboxXSUserName.Name = "textboxXSUserName";
            this.textboxXSUserName.TextChanged += new System.EventHandler(this.xsCredentials_TextChanged);
            // 
            // currentXsCredentialsRadioButton
            // 
            resources.ApplyResources(this.currentXsCredentialsRadioButton, "currentXsCredentialsRadioButton");
            this.currentXsCredentialsRadioButton.Checked = true;
            this.tableLayoutPanel3.SetColumnSpan(this.currentXsCredentialsRadioButton, 4);
            this.currentXsCredentialsRadioButton.Name = "currentXsCredentialsRadioButton";
            this.currentXsCredentialsRadioButton.TabStop = true;
            this.currentXsCredentialsRadioButton.UseVisualStyleBackColor = true;
            // 
            // newXsCredentialsRadioButton
            // 
            resources.ApplyResources(this.newXsCredentialsRadioButton, "newXsCredentialsRadioButton");
            this.tableLayoutPanel3.SetColumnSpan(this.newXsCredentialsRadioButton, 4);
            this.newXsCredentialsRadioButton.Name = "newXsCredentialsRadioButton";
            this.newXsCredentialsRadioButton.TabStop = true;
            this.newXsCredentialsRadioButton.UseVisualStyleBackColor = true;
            this.newXsCredentialsRadioButton.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // testCredentialsButton
            // 
            resources.ApplyResources(this.testCredentialsButton, "testCredentialsButton");
            this.testCredentialsButton.Name = "testCredentialsButton";
            this.testCredentialsButton.UseVisualStyleBackColor = true;
            this.testCredentialsButton.Click += new System.EventHandler(this.testCredentialsButton_Click);
            // 
            // HealthCheckSettingsDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "HealthCheckSettingsDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.decentGroupBox2.ResumeLayout(false);
            this.decentGroupBox2.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericBox)).EndInit();
            this.decentGroupBox1.ResumeLayout(false);
            this.decentGroupBox1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.decentGroupBoxXSCredentials.ResumeLayout(false);
            this.decentGroupBoxXSCredentials.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testCredentialsStatusImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Controls.DecentGroupBox decentGroupBox2;
        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label frequencyLabel;
        private System.Windows.Forms.NumericUpDown frequencyNumericBox;
        private System.Windows.Forms.Label weeksLabel;
        private System.Windows.Forms.Label dayOfweekLabel;
        private System.Windows.Forms.Label timeOfDayLabel;
        private System.Windows.Forms.ComboBox timeOfDayComboBox;
        private System.Windows.Forms.ComboBox dayOfWeekComboBox;
        private Controls.DecentGroupBox decentGroupBox1;
        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        protected System.Windows.Forms.Label label1;
        protected System.Windows.Forms.TextBox textBoxMyCitrixUsername;
        private System.Windows.Forms.RadioButton existingAuthenticationRadioButton;
        private System.Windows.Forms.RadioButton newAuthenticationRadioButton;
        private System.Windows.Forms.LinkLabel PolicyStatementLinkLabel;
        private System.Windows.Forms.Label rubricLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckBox enrollmentCheckBox;
        private Controls.DecentGroupBox decentGroupBoxXSCredentials;
        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label3;
        protected System.Windows.Forms.Label label4;
        protected System.Windows.Forms.Label label5;
        protected System.Windows.Forms.TextBox textboxXSPassword;
        protected System.Windows.Forms.TextBox textboxXSUserName;
        private System.Windows.Forms.RadioButton currentXsCredentialsRadioButton;
        private System.Windows.Forms.RadioButton newXsCredentialsRadioButton;
        private Controls.Common.PasswordFailure m_ctrlError;
        private System.Windows.Forms.Button testCredentialsButton;
        private System.Windows.Forms.PictureBox testCredentialsStatusImage;
        private System.Windows.Forms.Label errorLabel;
        protected System.Windows.Forms.Label label2;
        protected System.Windows.Forms.TextBox textBoxMyCitrixPassword;
        private System.Windows.Forms.LinkLabel authRubricLinkLabel;
        private System.Windows.Forms.Label authRubricTextLabel;
    }
}
