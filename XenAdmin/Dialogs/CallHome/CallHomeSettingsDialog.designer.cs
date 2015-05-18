namespace XenAdmin.Dialogs.CallHome
{
    partial class CallHomeSettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CallHomeSettingsDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.authenticationRubricLabel = new System.Windows.Forms.Label();
            this.authenticationLabel = new System.Windows.Forms.Label();
            this.timeOfDayComboBox = new System.Windows.Forms.ComboBox();
            this.timeOfDayLabel = new System.Windows.Forms.Label();
            this.dayOfweekLabel = new System.Windows.Forms.Label();
            this.weeksLabel = new System.Windows.Forms.Label();
            this.frequencyLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.policyStatementLabel = new System.Windows.Forms.Label();
            this.PolicyStatementLinkLabel = new System.Windows.Forms.LinkLabel();
            this.scheduleLabel = new System.Windows.Forms.Label();
            this.rubricLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.enrollmentCheckBox = new System.Windows.Forms.CheckBox();
            this.frequencyNumericBox = new System.Windows.Forms.NumericUpDown();
            this.dayOfWeekComboBox = new System.Windows.Forms.ComboBox();
            this.existingAuthenticationRadioButton = new System.Windows.Forms.RadioButton();
            this.newAuthenticationRadioButton = new System.Windows.Forms.RadioButton();
            this.authenticationStatusTable = new System.Windows.Forms.TableLayoutPanel();
            this.spinnerIcon = new XenAdmin.Controls.SpinnerIcon();
            this.authenticateButton = new System.Windows.Forms.Button();
            this.statusPictureBox = new System.Windows.Forms.PictureBox();
            this.statusLabel = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericBox)).BeginInit();
            this.authenticationStatusTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusPictureBox)).BeginInit();
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
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 2, 15);
            this.tableLayoutPanel1.Controls.Add(this.passwordTextBox, 3, 14);
            this.tableLayoutPanel1.Controls.Add(this.usernameTextBox, 3, 13);
            this.tableLayoutPanel1.Controls.Add(this.passwordLabel, 2, 14);
            this.tableLayoutPanel1.Controls.Add(this.usernameLabel, 2, 13);
            this.tableLayoutPanel1.Controls.Add(this.authenticationRubricLabel, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.authenticationLabel, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.timeOfDayComboBox, 3, 8);
            this.tableLayoutPanel1.Controls.Add(this.timeOfDayLabel, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.dayOfweekLabel, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.weeksLabel, 4, 6);
            this.tableLayoutPanel1.Controls.Add(this.frequencyLabel, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.scheduleLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.rubricLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 18);
            this.tableLayoutPanel1.Controls.Add(this.enrollmentCheckBox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.frequencyNumericBox, 3, 6);
            this.tableLayoutPanel1.Controls.Add(this.dayOfWeekComboBox, 3, 7);
            this.tableLayoutPanel1.Controls.Add(this.existingAuthenticationRadioButton, 1, 11);
            this.tableLayoutPanel1.Controls.Add(this.newAuthenticationRadioButton, 1, 12);
            this.tableLayoutPanel1.Controls.Add(this.authenticationStatusTable, 2, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // flowLayoutPanel3
            // 
            resources.ApplyResources(this.flowLayoutPanel3, "flowLayoutPanel3");
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel3, 3);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            // 
            // passwordTextBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.passwordTextBox, 2);
            resources.ApplyResources(this.passwordTextBox, "passwordTextBox");
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.UseSystemPasswordChar = true;
            this.passwordTextBox.TextChanged += new System.EventHandler(this.credentials_TextChanged);
            // 
            // usernameTextBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.usernameTextBox, 2);
            resources.ApplyResources(this.usernameTextBox, "usernameTextBox");
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.TextChanged += new System.EventHandler(this.credentials_TextChanged);
            // 
            // passwordLabel
            // 
            resources.ApplyResources(this.passwordLabel, "passwordLabel");
            this.passwordLabel.Name = "passwordLabel";
            // 
            // usernameLabel
            // 
            resources.ApplyResources(this.usernameLabel, "usernameLabel");
            this.usernameLabel.Name = "usernameLabel";
            // 
            // authenticationRubricLabel
            // 
            resources.ApplyResources(this.authenticationRubricLabel, "authenticationRubricLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.authenticationRubricLabel, 4);
            this.authenticationRubricLabel.Name = "authenticationRubricLabel";
            // 
            // authenticationLabel
            // 
            resources.ApplyResources(this.authenticationLabel, "authenticationLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.authenticationLabel, 5);
            this.authenticationLabel.Name = "authenticationLabel";
            // 
            // timeOfDayComboBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.timeOfDayComboBox, 2);
            this.timeOfDayComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.timeOfDayComboBox, "timeOfDayComboBox");
            this.timeOfDayComboBox.FormattingEnabled = true;
            this.timeOfDayComboBox.Name = "timeOfDayComboBox";
            // 
            // timeOfDayLabel
            // 
            resources.ApplyResources(this.timeOfDayLabel, "timeOfDayLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.timeOfDayLabel, 2);
            this.timeOfDayLabel.Name = "timeOfDayLabel";
            // 
            // dayOfweekLabel
            // 
            resources.ApplyResources(this.dayOfweekLabel, "dayOfweekLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.dayOfweekLabel, 2);
            this.dayOfweekLabel.Name = "dayOfweekLabel";
            // 
            // weeksLabel
            // 
            resources.ApplyResources(this.weeksLabel, "weeksLabel");
            this.weeksLabel.Name = "weeksLabel";
            // 
            // frequencyLabel
            // 
            resources.ApplyResources(this.frequencyLabel, "frequencyLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.frequencyLabel, 2);
            this.frequencyLabel.Name = "frequencyLabel";
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel2, 5);
            this.flowLayoutPanel2.Controls.Add(this.policyStatementLabel);
            this.flowLayoutPanel2.Controls.Add(this.PolicyStatementLinkLabel);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // policyStatementLabel
            // 
            resources.ApplyResources(this.policyStatementLabel, "policyStatementLabel");
            this.policyStatementLabel.Name = "policyStatementLabel";
            // 
            // PolicyStatementLinkLabel
            // 
            resources.ApplyResources(this.PolicyStatementLinkLabel, "PolicyStatementLinkLabel");
            this.PolicyStatementLinkLabel.Name = "PolicyStatementLinkLabel";
            this.PolicyStatementLinkLabel.TabStop = true;
            // 
            // scheduleLabel
            // 
            resources.ApplyResources(this.scheduleLabel, "scheduleLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.scheduleLabel, 5);
            this.scheduleLabel.Name = "scheduleLabel";
            // 
            // rubricLabel
            // 
            resources.ApplyResources(this.rubricLabel, "rubricLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.rubricLabel, 5);
            this.rubricLabel.Name = "rubricLabel";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 5);
            this.flowLayoutPanel1.Controls.Add(this.cancelButton);
            this.flowLayoutPanel1.Controls.Add(this.okButton);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // enrollmentCheckBox
            // 
            resources.ApplyResources(this.enrollmentCheckBox, "enrollmentCheckBox");
            this.tableLayoutPanel1.SetColumnSpan(this.enrollmentCheckBox, 5);
            this.enrollmentCheckBox.Name = "enrollmentCheckBox";
            this.enrollmentCheckBox.UseVisualStyleBackColor = true;
            this.enrollmentCheckBox.CheckedChanged += new System.EventHandler(this.enrollmentCheckBox_CheckedChanged);
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
            // dayOfWeekComboBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.dayOfWeekComboBox, 2);
            this.dayOfWeekComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.dayOfWeekComboBox, "dayOfWeekComboBox");
            this.dayOfWeekComboBox.FormattingEnabled = true;
            this.dayOfWeekComboBox.Name = "dayOfWeekComboBox";
            // 
            // existingAuthenticationRadioButton
            // 
            resources.ApplyResources(this.existingAuthenticationRadioButton, "existingAuthenticationRadioButton");
            this.tableLayoutPanel1.SetColumnSpan(this.existingAuthenticationRadioButton, 4);
            this.existingAuthenticationRadioButton.Name = "existingAuthenticationRadioButton";
            this.existingAuthenticationRadioButton.TabStop = true;
            this.existingAuthenticationRadioButton.UseVisualStyleBackColor = true;
            // 
            // newAuthenticationRadioButton
            // 
            resources.ApplyResources(this.newAuthenticationRadioButton, "newAuthenticationRadioButton");
            this.tableLayoutPanel1.SetColumnSpan(this.newAuthenticationRadioButton, 4);
            this.newAuthenticationRadioButton.Name = "newAuthenticationRadioButton";
            this.newAuthenticationRadioButton.TabStop = true;
            this.newAuthenticationRadioButton.UseVisualStyleBackColor = true;
            // 
            // authenticationStatusTable
            // 
            resources.ApplyResources(this.authenticationStatusTable, "authenticationStatusTable");
            this.tableLayoutPanel1.SetColumnSpan(this.authenticationStatusTable, 3);
            this.authenticationStatusTable.Controls.Add(this.spinnerIcon, 0, 0);
            this.authenticationStatusTable.Controls.Add(this.authenticateButton, 0, 0);
            this.authenticationStatusTable.Controls.Add(this.statusPictureBox, 2, 0);
            this.authenticationStatusTable.Controls.Add(this.statusLabel, 3, 0);
            this.authenticationStatusTable.Name = "authenticationStatusTable";
            // 
            // spinnerIcon
            // 
            resources.ApplyResources(this.spinnerIcon, "spinnerIcon");
            this.spinnerIcon.Name = "spinnerIcon";
            this.spinnerIcon.SucceededImage = global::XenAdmin.Properties.Resources._000_Tick_h32bit_16;
            this.spinnerIcon.TabStop = false;
            // 
            // authenticateButton
            // 
            resources.ApplyResources(this.authenticateButton, "authenticateButton");
            this.authenticateButton.Name = "authenticateButton";
            this.authenticateButton.UseVisualStyleBackColor = true;
            this.authenticateButton.Click += new System.EventHandler(this.authenticateButton_Click);
            // 
            // statusPictureBox
            // 
            resources.ApplyResources(this.statusPictureBox, "statusPictureBox");
            this.statusPictureBox.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.statusPictureBox.Name = "statusPictureBox";
            this.statusPictureBox.TabStop = false;
            // 
            // statusLabel
            // 
            resources.ApplyResources(this.statusLabel, "statusLabel");
            this.statusLabel.ForeColor = System.Drawing.Color.Red;
            this.statusLabel.Name = "statusLabel";
            // 
            // CallHomeEnrollmentDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CallHomeEnrollmentDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericBox)).EndInit();
            this.authenticationStatusTable.ResumeLayout(false);
            this.authenticationStatusTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label rubricLabel;
        private System.Windows.Forms.Label scheduleLabel;
        private System.Windows.Forms.CheckBox enrollmentCheckBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label policyStatementLabel;
        private System.Windows.Forms.LinkLabel PolicyStatementLinkLabel;
        private System.Windows.Forms.Label frequencyLabel;
        private System.Windows.Forms.Label weeksLabel;
        private System.Windows.Forms.NumericUpDown frequencyNumericBox;
        private System.Windows.Forms.Label authenticationLabel;
        private System.Windows.Forms.ComboBox timeOfDayComboBox;
        private System.Windows.Forms.Label timeOfDayLabel;
        private System.Windows.Forms.Label dayOfweekLabel;
        private System.Windows.Forms.ComboBox dayOfWeekComboBox;
        private System.Windows.Forms.Label authenticationRubricLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.RadioButton existingAuthenticationRadioButton;
        private System.Windows.Forms.RadioButton newAuthenticationRadioButton;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button authenticateButton;
        private System.Windows.Forms.TableLayoutPanel authenticationStatusTable;
        private System.Windows.Forms.PictureBox statusPictureBox;
        private Controls.Common.AutoHeightLabel statusLabel;
        private Controls.SpinnerIcon spinnerIcon;
    }
}