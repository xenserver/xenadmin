namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class CIFS_ISO
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CIFS_ISO));
            this.checkBoxUseDifferentUsername = new System.Windows.Forms.CheckBox();
            this.groupBoxLogin = new XenAdmin.Controls.DecentGroupBox();
            this.labelCifsPassword = new System.Windows.Forms.Label();
            this.labelCifsUsername = new System.Windows.Forms.Label();
            this.textBoxCifsPassword = new System.Windows.Forms.TextBox();
            this.textBoxCifsUsername = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.labelCifsShareName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.passwordFailure1 = new XenAdmin.Controls.Common.PasswordFailure();
            this.comboBoxCifsSharename = new System.Windows.Forms.ComboBox();
            this.groupBoxLogin.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxUseDifferentUsername
            // 
            resources.ApplyResources(this.checkBoxUseDifferentUsername, "checkBoxUseDifferentUsername");
            this.checkBoxUseDifferentUsername.Name = "checkBoxUseDifferentUsername";
            this.checkBoxUseDifferentUsername.UseVisualStyleBackColor = true;
            this.checkBoxUseDifferentUsername.CheckedChanged += new System.EventHandler(this.checkBoxUseDifferentUsername_CheckedChanged);
            // 
            // groupBoxLogin
            // 
            resources.ApplyResources(this.groupBoxLogin, "groupBoxLogin");
            this.groupBoxLogin.Controls.Add(this.labelCifsPassword);
            this.groupBoxLogin.Controls.Add(this.labelCifsUsername);
            this.groupBoxLogin.Controls.Add(this.textBoxCifsPassword);
            this.groupBoxLogin.Controls.Add(this.textBoxCifsUsername);
            this.groupBoxLogin.Name = "groupBoxLogin";
            this.groupBoxLogin.TabStop = false;
            // 
            // labelCifsPassword
            // 
            resources.ApplyResources(this.labelCifsPassword, "labelCifsPassword");
            this.labelCifsPassword.Name = "labelCifsPassword";
            // 
            // labelCifsUsername
            // 
            resources.ApplyResources(this.labelCifsUsername, "labelCifsUsername");
            this.labelCifsUsername.Name = "labelCifsUsername";
            // 
            // textBoxCifsPassword
            // 
            resources.ApplyResources(this.textBoxCifsPassword, "textBoxCifsPassword");
            this.textBoxCifsPassword.Name = "textBoxCifsPassword";
            this.textBoxCifsPassword.UseSystemPasswordChar = true;
            // 
            // textBoxCifsUsername
            // 
            resources.ApplyResources(this.textBoxCifsUsername, "textBoxCifsUsername");
            this.textBoxCifsUsername.Name = "textBoxCifsUsername";
            this.textBoxCifsUsername.TextChanged += new System.EventHandler(this.textBoxCifsUsername_TextChanged);
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label22.Name = "label22";
            // 
            // labelCifsShareName
            // 
            resources.ApplyResources(this.labelCifsShareName, "labelCifsShareName");
            this.labelCifsShareName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelCifsShareName.Name = "labelCifsShareName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.passwordFailure1, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelCifsShareName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label22, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxCifsSharename, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // passwordFailure1
            // 
            resources.ApplyResources(this.passwordFailure1, "passwordFailure1");
            this.passwordFailure1.Name = "passwordFailure1";
            this.passwordFailure1.TabStop = false;
            // 
            // comboBoxCifsSharename
            // 
            this.comboBoxCifsSharename.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxCifsSharename, "comboBoxCifsSharename");
            this.comboBoxCifsSharename.Name = "comboBoxCifsSharename";
            this.comboBoxCifsSharename.SelectedIndexChanged += new System.EventHandler(this.textBoxCifsSharename_TextChanged);
            this.comboBoxCifsSharename.TextChanged += new System.EventHandler(this.textBoxCifsSharename_TextChanged);
            // 
            // CIFS_ISO
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.checkBoxUseDifferentUsername);
            this.Controls.Add(this.groupBoxLogin);
            this.Name = "CIFS_ISO";
            this.groupBoxLogin.ResumeLayout(false);
            this.groupBoxLogin.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxUseDifferentUsername;
        private XenAdmin.Controls.DecentGroupBox groupBoxLogin;
        private System.Windows.Forms.Label labelCifsPassword;
        private System.Windows.Forms.Label labelCifsUsername;
        private System.Windows.Forms.TextBox textBoxCifsPassword;
        private System.Windows.Forms.TextBox textBoxCifsUsername;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label labelCifsShareName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox comboBoxCifsSharename;
        private Controls.Common.PasswordFailure passwordFailure1;
    }
}
