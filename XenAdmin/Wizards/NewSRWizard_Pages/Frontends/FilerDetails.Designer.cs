namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class FilerDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilerDetails));
            this.checkBoxNetappUseChap = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.labelHostAddress = new System.Windows.Forms.Label();
            this.textBoxNetappHostAddress = new System.Windows.Forms.TextBox();
            this.labelInvalidHost = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.textBoxNetappUsername = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxNetappPassword = new System.Windows.Forms.TextBox();
            this.labelNetappChapUser = new System.Windows.Forms.Label();
            this.textBoxNetappChapUser = new System.Windows.Forms.TextBox();
            this.labelNetappChapSecret = new System.Windows.Forms.Label();
            this.textBoxNetappChapSecret = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxNetappUseChap
            // 
            resources.ApplyResources(this.checkBoxNetappUseChap, "checkBoxNetappUseChap");
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxNetappUseChap, 2);
            this.checkBoxNetappUseChap.ForeColor = System.Drawing.SystemColors.InfoText;
            this.checkBoxNetappUseChap.Name = "checkBoxNetappUseChap";
            this.checkBoxNetappUseChap.UseVisualStyleBackColor = true;
            this.checkBoxNetappUseChap.CheckedChanged += new System.EventHandler(this.UseChapCheckBox_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelHostAddress, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxNetappHostAddress, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelInvalidHost, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelUsername, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxNetappUsername, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelPassword, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.textBoxNetappPassword, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxNetappUseChap, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelNetappChapUser, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.textBoxNetappChapUser, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelNetappChapSecret, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.textBoxNetappChapSecret, 1, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // labelHostAddress
            // 
            resources.ApplyResources(this.labelHostAddress, "labelHostAddress");
            this.labelHostAddress.BackColor = System.Drawing.Color.Transparent;
            this.labelHostAddress.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelHostAddress.Name = "labelHostAddress";
            // 
            // textBoxNetappHostAddress
            // 
            resources.ApplyResources(this.textBoxNetappHostAddress, "textBoxNetappHostAddress");
            this.textBoxNetappHostAddress.Name = "textBoxNetappHostAddress";
            this.textBoxNetappHostAddress.TextChanged += new System.EventHandler(this.textBoxHostAddress_TextChanged);
            // 
            // labelInvalidHost
            // 
            resources.ApplyResources(this.labelInvalidHost, "labelInvalidHost");
            this.labelInvalidHost.ForeColor = System.Drawing.Color.Red;
            this.labelInvalidHost.Name = "labelInvalidHost";
            // 
            // labelUsername
            // 
            resources.ApplyResources(this.labelUsername, "labelUsername");
            this.labelUsername.BackColor = System.Drawing.Color.Transparent;
            this.labelUsername.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelUsername.Name = "labelUsername";
            // 
            // textBoxNetappUsername
            // 
            this.textBoxNetappUsername.AllowDrop = true;
            resources.ApplyResources(this.textBoxNetappUsername, "textBoxNetappUsername");
            this.textBoxNetappUsername.Name = "textBoxNetappUsername";
            this.textBoxNetappUsername.TextChanged += new System.EventHandler(this.textBoxUsername_TextChanged);
            // 
            // labelPassword
            // 
            resources.ApplyResources(this.labelPassword, "labelPassword");
            this.labelPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelPassword.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelPassword.Name = "labelPassword";
            // 
            // textBoxNetappPassword
            // 
            resources.ApplyResources(this.textBoxNetappPassword, "textBoxNetappPassword");
            this.textBoxNetappPassword.Name = "textBoxNetappPassword";
            this.textBoxNetappPassword.UseSystemPasswordChar = true;
            // 
            // labelNetappChapUser
            // 
            resources.ApplyResources(this.labelNetappChapUser, "labelNetappChapUser");
            this.labelNetappChapUser.BackColor = System.Drawing.Color.Transparent;
            this.labelNetappChapUser.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelNetappChapUser.Name = "labelNetappChapUser";
            // 
            // textBoxNetappChapUser
            // 
            this.textBoxNetappChapUser.AllowDrop = true;
            resources.ApplyResources(this.textBoxNetappChapUser, "textBoxNetappChapUser");
            this.textBoxNetappChapUser.Name = "textBoxNetappChapUser";
            this.textBoxNetappChapUser.TextChanged += new System.EventHandler(this.textBoxNetappChapUser_TextChanged);
            // 
            // labelNetappChapSecret
            // 
            resources.ApplyResources(this.labelNetappChapSecret, "labelNetappChapSecret");
            this.labelNetappChapSecret.BackColor = System.Drawing.Color.Transparent;
            this.labelNetappChapSecret.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelNetappChapSecret.Name = "labelNetappChapSecret";
            // 
            // textBoxNetappChapSecret
            // 
            resources.ApplyResources(this.textBoxNetappChapSecret, "textBoxNetappChapSecret");
            this.textBoxNetappChapSecret.Name = "textBoxNetappChapSecret";
            this.textBoxNetappChapSecret.UseSystemPasswordChar = true;
            // 
            // FilerDetails
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FilerDetails";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxNetappUseChap;
        private System.Windows.Forms.Label labelNetappChapSecret;
        private System.Windows.Forms.TextBox textBoxNetappChapSecret;
        private System.Windows.Forms.Label labelNetappChapUser;
        private System.Windows.Forms.TextBox textBoxNetappChapUser;
        private System.Windows.Forms.Label labelInvalidHost;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxNetappPassword;
        private System.Windows.Forms.TextBox textBoxNetappUsername;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Label labelHostAddress;
        private System.Windows.Forms.TextBox textBoxNetappHostAddress;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
    }
}
