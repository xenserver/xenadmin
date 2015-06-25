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
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.autoHeightLabel2 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.m_textBoxName = new System.Windows.Forms.TextBox();
            this.labelFileLocation = new System.Windows.Forms.Label();
            this.m_textBoxLocation = new System.Windows.Forms.TextBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_textBoxName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelFileLocation, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_textBoxLocation, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.BrowseButton, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_ctrlError, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.checkBox1, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 3);
            this.label2.Name = "label2";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 3);
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.autoHeightLabel2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.usernameLabel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.passwordLabel, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.passwordTextBox, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.usernameTextBox, 1, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // autoHeightLabel2
            // 
            resources.ApplyResources(this.autoHeightLabel2, "autoHeightLabel2");
            this.tableLayoutPanel2.SetColumnSpan(this.autoHeightLabel2, 3);
            this.autoHeightLabel2.Name = "autoHeightLabel2";
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
            // labelName
            // 
            resources.ApplyResources(this.labelName, "labelName");
            this.labelName.Name = "labelName";
            // 
            // m_textBoxName
            // 
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
            this.tableLayoutPanel1.SetColumnSpan(this.m_ctrlError, 3);
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.tableLayoutPanel1.SetColumnSpan(this.checkBox1, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // BugToolPageDestination
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "BugToolPageDestination";
            resources.ApplyResources(this, "$this");
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox m_textBoxName;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlError;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox usernameTextBox;
        private Controls.Common.AutoHeightLabel autoHeightLabel2;
        private System.Windows.Forms.Label labelFileLocation;
        private System.Windows.Forms.TextBox m_textBoxLocation;
        private System.Windows.Forms.Button BrowseButton;
    }
}
