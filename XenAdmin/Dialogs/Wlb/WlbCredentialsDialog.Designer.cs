namespace XenAdmin.Dialogs.Wlb
{
    partial class WlbCredentialsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbCredentialsDialog));
            this.textboxWLBPort = new System.Windows.Forms.TextBox();
            this.LabelWLBServerPort = new System.Windows.Forms.Label();
            this.checkboxUseCurrentXSCredentials = new System.Windows.Forms.CheckBox();
            this.textboxWlbUrl = new System.Windows.Forms.TextBox();
            this.textboxXSPassword = new System.Windows.Forms.TextBox();
            this.textboxXSUserName = new System.Windows.Forms.TextBox();
            this.LabelXenServerPassword = new System.Windows.Forms.Label();
            this.LabelXenServerUsername = new System.Windows.Forms.Label();
            this.LabelXenServerCredsBlurb = new System.Windows.Forms.Label();
            this.textboxWlbPassword = new System.Windows.Forms.TextBox();
            this.textboxWlbUserName = new System.Windows.Forms.TextBox();
            this.LabelWLBPassword = new System.Windows.Forms.Label();
            this.LabelWLBUsername = new System.Windows.Forms.Label();
            this.LabelWLBServerCredsBlurb = new System.Windows.Forms.Label();
            this.LabelWLBServerName = new System.Windows.Forms.Label();
            this.LabelWLBServerNameBlurb = new System.Windows.Forms.Label();
            this.decentGroupBoxWLBServerAddress = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelDefaultPortBlurb = new System.Windows.Forms.Label();
            this.decentGroupBoxWLBCredentials = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.decentGroupBoxXSCredentials = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.decentGroupBoxWLBServerAddress.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.decentGroupBoxWLBCredentials.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.decentGroupBoxXSCredentials.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // textboxWLBPort
            // 
            resources.ApplyResources(this.textboxWLBPort, "textboxWLBPort");
            this.textboxWLBPort.Name = "textboxWLBPort";
            this.textboxWLBPort.TextChanged += new System.EventHandler(this.textboxWLBPort_TextChanged);
            // 
            // LabelWLBServerPort
            // 
            resources.ApplyResources(this.LabelWLBServerPort, "LabelWLBServerPort");
            this.LabelWLBServerPort.Name = "LabelWLBServerPort";
            // 
            // checkboxUseCurrentXSCredentials
            // 
            resources.ApplyResources(this.checkboxUseCurrentXSCredentials, "checkboxUseCurrentXSCredentials");
            this.tableLayoutPanel3.SetColumnSpan(this.checkboxUseCurrentXSCredentials, 2);
            this.checkboxUseCurrentXSCredentials.Name = "checkboxUseCurrentXSCredentials";
            this.checkboxUseCurrentXSCredentials.UseVisualStyleBackColor = true;
            this.checkboxUseCurrentXSCredentials.CheckedChanged += new System.EventHandler(this.checkboxUseCurrentXSCredentials_CheckedChanged);
            // 
            // textboxWlbUrl
            // 
            resources.ApplyResources(this.textboxWlbUrl, "textboxWlbUrl");
            this.tableLayoutPanel1.SetColumnSpan(this.textboxWlbUrl, 2);
            this.textboxWlbUrl.Name = "textboxWlbUrl";
            this.textboxWlbUrl.TextChanged += new System.EventHandler(this.textboxWlbUrl_TextChanged);
            // 
            // textboxXSPassword
            // 
            resources.ApplyResources(this.textboxXSPassword, "textboxXSPassword");
            this.textboxXSPassword.Name = "textboxXSPassword";
            this.textboxXSPassword.TextChanged += new System.EventHandler(this.textboxXSPassword_TextChanged);
            // 
            // textboxXSUserName
            // 
            resources.ApplyResources(this.textboxXSUserName, "textboxXSUserName");
            this.textboxXSUserName.Name = "textboxXSUserName";
            this.textboxXSUserName.TextChanged += new System.EventHandler(this.textboxXSUserName_TextChanged);
            // 
            // LabelXenServerPassword
            // 
            resources.ApplyResources(this.LabelXenServerPassword, "LabelXenServerPassword");
            this.LabelXenServerPassword.Name = "LabelXenServerPassword";
            // 
            // LabelXenServerUsername
            // 
            resources.ApplyResources(this.LabelXenServerUsername, "LabelXenServerUsername");
            this.LabelXenServerUsername.Name = "LabelXenServerUsername";
            // 
            // LabelXenServerCredsBlurb
            // 
            resources.ApplyResources(this.LabelXenServerCredsBlurb, "LabelXenServerCredsBlurb");
            this.tableLayoutPanel3.SetColumnSpan(this.LabelXenServerCredsBlurb, 2);
            this.LabelXenServerCredsBlurb.Name = "LabelXenServerCredsBlurb";
            // 
            // textboxWlbPassword
            // 
            resources.ApplyResources(this.textboxWlbPassword, "textboxWlbPassword");
            this.textboxWlbPassword.Name = "textboxWlbPassword";
            this.textboxWlbPassword.TextChanged += new System.EventHandler(this.textboxWlbPassword_TextChanged);
            // 
            // textboxWlbUserName
            // 
            resources.ApplyResources(this.textboxWlbUserName, "textboxWlbUserName");
            this.textboxWlbUserName.Name = "textboxWlbUserName";
            this.textboxWlbUserName.TextChanged += new System.EventHandler(this.textboxWlbUserName_TextChanged);
            // 
            // LabelWLBPassword
            // 
            resources.ApplyResources(this.LabelWLBPassword, "LabelWLBPassword");
            this.LabelWLBPassword.Name = "LabelWLBPassword";
            // 
            // LabelWLBUsername
            // 
            resources.ApplyResources(this.LabelWLBUsername, "LabelWLBUsername");
            this.LabelWLBUsername.Name = "LabelWLBUsername";
            // 
            // LabelWLBServerCredsBlurb
            // 
            resources.ApplyResources(this.LabelWLBServerCredsBlurb, "LabelWLBServerCredsBlurb");
            this.tableLayoutPanel2.SetColumnSpan(this.LabelWLBServerCredsBlurb, 2);
            this.LabelWLBServerCredsBlurb.Name = "LabelWLBServerCredsBlurb";
            // 
            // LabelWLBServerName
            // 
            resources.ApplyResources(this.LabelWLBServerName, "LabelWLBServerName");
            this.LabelWLBServerName.Name = "LabelWLBServerName";
            // 
            // LabelWLBServerNameBlurb
            // 
            resources.ApplyResources(this.LabelWLBServerNameBlurb, "LabelWLBServerNameBlurb");
            this.tableLayoutPanel1.SetColumnSpan(this.LabelWLBServerNameBlurb, 3);
            this.LabelWLBServerNameBlurb.Name = "LabelWLBServerNameBlurb";
            // 
            // decentGroupBoxWLBServerAddress
            // 
            resources.ApplyResources(this.decentGroupBoxWLBServerAddress, "decentGroupBoxWLBServerAddress");
            this.decentGroupBoxWLBServerAddress.Controls.Add(this.tableLayoutPanel1);
            this.decentGroupBoxWLBServerAddress.Name = "decentGroupBoxWLBServerAddress";
            this.decentGroupBoxWLBServerAddress.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.LabelWLBServerNameBlurb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LabelWLBServerPort, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textboxWLBPort, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelDefaultPortBlurb, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.textboxWlbUrl, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.LabelWLBServerName, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelDefaultPortBlurb
            // 
            resources.ApplyResources(this.labelDefaultPortBlurb, "labelDefaultPortBlurb");
            this.labelDefaultPortBlurb.Name = "labelDefaultPortBlurb";
            // 
            // decentGroupBoxWLBCredentials
            // 
            resources.ApplyResources(this.decentGroupBoxWLBCredentials, "decentGroupBoxWLBCredentials");
            this.decentGroupBoxWLBCredentials.Controls.Add(this.tableLayoutPanel2);
            this.decentGroupBoxWLBCredentials.Name = "decentGroupBoxWLBCredentials";
            this.decentGroupBoxWLBCredentials.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.LabelWLBServerCredsBlurb, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.LabelWLBPassword, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.LabelWLBUsername, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.textboxWlbPassword, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.textboxWlbUserName, 1, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // decentGroupBoxXSCredentials
            // 
            resources.ApplyResources(this.decentGroupBoxXSCredentials, "decentGroupBoxXSCredentials");
            this.decentGroupBoxXSCredentials.Controls.Add(this.tableLayoutPanel3);
            this.decentGroupBoxXSCredentials.Name = "decentGroupBoxXSCredentials";
            this.decentGroupBoxXSCredentials.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.LabelXenServerCredsBlurb, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.LabelXenServerPassword, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.LabelXenServerUsername, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.textboxXSUserName, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.checkboxUseCurrentXSCredentials, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.textboxXSPassword, 1, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // WlbCredentialsDialog
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.decentGroupBoxXSCredentials);
            this.Controls.Add(this.decentGroupBoxWLBCredentials);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.decentGroupBoxWLBServerAddress);
            this.Name = "WlbCredentialsDialog";
            this.decentGroupBoxWLBServerAddress.ResumeLayout(false);
            this.decentGroupBoxWLBServerAddress.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.decentGroupBoxWLBCredentials.ResumeLayout(false);
            this.decentGroupBoxWLBCredentials.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.decentGroupBoxXSCredentials.ResumeLayout(false);
            this.decentGroupBoxXSCredentials.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textboxWLBPort;
        private System.Windows.Forms.Label LabelWLBServerPort;
        private System.Windows.Forms.CheckBox checkboxUseCurrentXSCredentials;
        private System.Windows.Forms.TextBox textboxWlbUrl;
        private System.Windows.Forms.TextBox textboxXSPassword;
        private System.Windows.Forms.TextBox textboxXSUserName;
        private System.Windows.Forms.Label LabelXenServerPassword;
        private System.Windows.Forms.Label LabelXenServerUsername;
        private System.Windows.Forms.Label LabelXenServerCredsBlurb;
        private System.Windows.Forms.TextBox textboxWlbPassword;
        private System.Windows.Forms.TextBox textboxWlbUserName;
        private System.Windows.Forms.Label LabelWLBPassword;
        private System.Windows.Forms.Label LabelWLBUsername;
        private System.Windows.Forms.Label LabelWLBServerCredsBlurb;
        private System.Windows.Forms.Label LabelWLBServerName;
        private System.Windows.Forms.Label LabelWLBServerNameBlurb;
        private XenAdmin.Controls.DecentGroupBox decentGroupBoxWLBServerAddress;
        private XenAdmin.Controls.DecentGroupBox decentGroupBoxWLBCredentials;
        private XenAdmin.Controls.DecentGroupBox decentGroupBoxXSCredentials;
        private System.Windows.Forms.Label labelDefaultPortBlurb;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    }
}
