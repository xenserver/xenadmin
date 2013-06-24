namespace XenAdmin.SettingsPanels
{
    partial class StorageLinkEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StorageLinkEditPage));
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.labelUsername = new System.Windows.Forms.Label();
            this.labelHostAddress = new System.Windows.Forms.Label();
            this.textBoxHostAddress = new System.Windows.Forms.TextBox();
            this.testConnectionButton = new System.Windows.Forms.Button();
            this.passFailLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.passFailPictureBox = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.allServersCheckBox = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.passFailPictureBox)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPassword
            // 
            resources.ApplyResources(this.labelPassword, "labelPassword");
            this.labelPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelPassword.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelPassword.Name = "labelPassword";
            // 
            // textBoxPassword
            // 
            resources.ApplyResources(this.textBoxPassword, "textBoxPassword");
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.AllowDrop = true;
            resources.ApplyResources(this.textBoxUsername, "textBoxUsername");
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.TextChanged += new System.EventHandler(this.textBoxUsername_TextChanged);
            // 
            // labelUsername
            // 
            resources.ApplyResources(this.labelUsername, "labelUsername");
            this.labelUsername.BackColor = System.Drawing.Color.Transparent;
            this.labelUsername.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelUsername.Name = "labelUsername";
            // 
            // labelHostAddress
            // 
            resources.ApplyResources(this.labelHostAddress, "labelHostAddress");
            this.labelHostAddress.BackColor = System.Drawing.Color.Transparent;
            this.labelHostAddress.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelHostAddress.Name = "labelHostAddress";
            // 
            // textBoxHostAddress
            // 
            resources.ApplyResources(this.textBoxHostAddress, "textBoxHostAddress");
            this.textBoxHostAddress.Name = "textBoxHostAddress";
            this.textBoxHostAddress.TextChanged += new System.EventHandler(this.textBoxHostAddress_TextChanged);
            // 
            // testConnectionButton
            // 
            resources.ApplyResources(this.testConnectionButton, "testConnectionButton");
            this.testConnectionButton.Name = "testConnectionButton";
            this.testConnectionButton.UseVisualStyleBackColor = true;
            this.testConnectionButton.Click += new System.EventHandler(this.testConnectionButton_Click);
            // 
            // passFailLabel
            // 
            resources.ApplyResources(this.passFailLabel, "passFailLabel");
            this.passFailLabel.Name = "passFailLabel";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.testConnectionButton);
            this.flowLayoutPanel1.Controls.Add(this.passFailPictureBox);
            this.flowLayoutPanel1.Controls.Add(this.passFailLabel);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // passFailPictureBox
            // 
            resources.ApplyResources(this.passFailPictureBox, "passFailPictureBox");
            this.passFailPictureBox.Name = "passFailPictureBox";
            this.passFailPictureBox.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // allServersCheckBox
            // 
            resources.ApplyResources(this.allServersCheckBox, "allServersCheckBox");
            this.allServersCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.allServersCheckBox.Checked = true;
            this.allServersCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.allServersCheckBox.Name = "allServersCheckBox";
            this.allServersCheckBox.UseVisualStyleBackColor = false;
            this.allServersCheckBox.CheckedChanged += new System.EventHandler(this.allServersCheckBox_CheckedChanged);
            // 
            // StorageLinkEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.allServersCheckBox);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.labelUsername);
            this.Controls.Add(this.labelHostAddress);
            this.Controls.Add(this.textBoxHostAddress);
            this.DoubleBuffered = true;
            this.Name = "StorageLinkEditPage";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.passFailPictureBox)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Label labelHostAddress;
        private System.Windows.Forms.TextBox textBoxHostAddress;
        private System.Windows.Forms.Button testConnectionButton;
        private System.Windows.Forms.Label passFailLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.PictureBox passFailPictureBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox allServersCheckBox;
    }
}
