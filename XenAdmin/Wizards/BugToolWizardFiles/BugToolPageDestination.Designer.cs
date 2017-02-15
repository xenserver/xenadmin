namespace XenAdmin.Wizards.BugToolWizardFiles
{
    partial class BugToolPageDestination
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BugToolPageDestination));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.m_textBoxName = new System.Windows.Forms.TextBox();
            this.labelFileLocation = new System.Windows.Forms.Label();
            this.m_textBoxLocation = new System.Windows.Forms.TextBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.uploadCheckBox = new System.Windows.Forms.CheckBox();
            this.caseNumberLabel = new System.Windows.Forms.Label();
            this.caseNumberTextBox = new System.Windows.Forms.TextBox();
            this.optionalLabel = new System.Windows.Forms.Label();
            this.enterCredentialsLinkLabel = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.usernameLabel, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.passwordLabel, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.passwordTextBox, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.usernameTextBox, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_textBoxName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelFileLocation, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_textBoxLocation, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.BrowseButton, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_ctrlError, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.uploadCheckBox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.caseNumberLabel, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.caseNumberTextBox, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.optionalLabel, 2, 8);
            this.tableLayoutPanel1.Controls.Add(this.enterCredentialsLinkLabel, 0, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // usernameLabel
            // 
            resources.ApplyResources(this.usernameLabel, "usernameLabel");
            this.usernameLabel.Name = "usernameLabel";
            // 
            // passwordLabel
            // 
            resources.ApplyResources(this.passwordLabel, "passwordLabel");
            this.passwordLabel.Name = "passwordLabel";
            // 
            // passwordTextBox
            // 
            resources.ApplyResources(this.passwordTextBox, "passwordTextBox");
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.UseSystemPasswordChar = true;
            this.passwordTextBox.TextChanged += new System.EventHandler(this.credentials_TextChanged);
            // 
            // usernameTextBox
            // 
            resources.ApplyResources(this.usernameTextBox, "usernameTextBox");
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.TextChanged += new System.EventHandler(this.credentials_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 4);
            this.label2.Name = "label2";
            // 
            // labelName
            // 
            resources.ApplyResources(this.labelName, "labelName");
            this.labelName.Name = "labelName";
            // 
            // m_textBoxName
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.m_textBoxName, 2);
            resources.ApplyResources(this.m_textBoxName, "m_textBoxName");
            this.m_textBoxName.Name = "m_textBoxName";
            this.m_textBoxName.TextChanged += new System.EventHandler(this.m_textBoxName_TextChanged);
            // 
            // labelFileLocation
            // 
            resources.ApplyResources(this.labelFileLocation, "labelFileLocation");
            this.labelFileLocation.Name = "labelFileLocation";
            // 
            // m_textBoxLocation
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.m_textBoxLocation, 2);
            resources.ApplyResources(this.m_textBoxLocation, "m_textBoxLocation");
            this.m_textBoxLocation.Name = "m_textBoxLocation";
            this.m_textBoxLocation.TextChanged += new System.EventHandler(this.m_textBoxLocation_TextChanged);
            // 
            // BrowseButton
            // 
            resources.ApplyResources(this.BrowseButton, "BrowseButton");
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // m_ctrlError
            // 
            resources.ApplyResources(this.m_ctrlError, "m_ctrlError");
            this.tableLayoutPanel1.SetColumnSpan(this.m_ctrlError, 4);
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // uploadCheckBox
            // 
            resources.ApplyResources(this.uploadCheckBox, "uploadCheckBox");
            this.tableLayoutPanel1.SetColumnSpan(this.uploadCheckBox, 4);
            this.uploadCheckBox.Name = "uploadCheckBox";
            this.uploadCheckBox.UseVisualStyleBackColor = true;
            this.uploadCheckBox.CheckedChanged += new System.EventHandler(this.uploadCheckBox_CheckedChanged);
            // 
            // caseNumberLabel
            // 
            resources.ApplyResources(this.caseNumberLabel, "caseNumberLabel");
            this.caseNumberLabel.Name = "caseNumberLabel";
            // 
            // caseNumberTextBox
            // 
            resources.ApplyResources(this.caseNumberTextBox, "caseNumberTextBox");
            this.caseNumberTextBox.Name = "caseNumberTextBox";
            this.caseNumberTextBox.TextChanged += new System.EventHandler(this.caseNumberLabelTextBox_TextChanged);
            // 
            // optionalLabel
            // 
            resources.ApplyResources(this.optionalLabel, "optionalLabel");
            this.optionalLabel.Name = "optionalLabel";
            // 
            // enterCredentialsLinkLabel
            // 
            resources.ApplyResources(this.enterCredentialsLinkLabel, "enterCredentialsLinkLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.enterCredentialsLinkLabel, 4);
            this.enterCredentialsLinkLabel.Name = "enterCredentialsLinkLabel";
            this.enterCredentialsLinkLabel.TabStop = true;
            this.enterCredentialsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.enterCredentialsLinkLabel_LinkClicked);
            // 
            // BugToolPageDestination
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "BugToolPageDestination";
            resources.ApplyResources(this, "$this");
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox m_textBoxName;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlError;
        private System.Windows.Forms.CheckBox uploadCheckBox;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label labelFileLocation;
        private System.Windows.Forms.TextBox m_textBoxLocation;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Label caseNumberLabel;
        private System.Windows.Forms.TextBox caseNumberTextBox;
        private System.Windows.Forms.Label optionalLabel;
        private System.Windows.Forms.LinkLabel enterCredentialsLinkLabel;
    }
}
