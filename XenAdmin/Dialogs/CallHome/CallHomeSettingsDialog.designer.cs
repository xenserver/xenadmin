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
            this.callHomeAuthenticationPanel1 = new XenAdmin.Controls.CallHomeAuthenticationPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericBox)).BeginInit();
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
            this.tableLayoutPanel1.Controls.Add(this.callHomeAuthenticationPanel1, 2, 17);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
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
            this.newAuthenticationRadioButton.CheckedChanged += new System.EventHandler(this.newAuthenticationRadioButton_CheckedChanged);
            // 
            // callHomeAuthenticationPanel1
            // 
            resources.ApplyResources(this.callHomeAuthenticationPanel1, "callHomeAuthenticationPanel1");
            this.callHomeAuthenticationPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.SetColumnSpan(this.callHomeAuthenticationPanel1, 3);
            this.callHomeAuthenticationPanel1.Name = "callHomeAuthenticationPanel1";
            this.callHomeAuthenticationPanel1.Pool = null;
            this.callHomeAuthenticationPanel1.AuthenticationChanged += new System.EventHandler(this.callHomeAuthenticationPanel1_AuthenticationChanged);
            // 
            // CallHomeSettingsDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CallHomeSettingsDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericBox)).EndInit();
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
        private System.Windows.Forms.RadioButton existingAuthenticationRadioButton;
        private System.Windows.Forms.RadioButton newAuthenticationRadioButton;
        private Controls.CallHomeAuthenticationPanel callHomeAuthenticationPanel1;
    }
}