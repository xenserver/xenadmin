namespace XenAdmin.Wizards.ExportWizard
{
    partial class ExportOptionsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportOptionsPage));
            this.m_checkBoxEncrypt = new System.Windows.Forms.CheckBox();
            this.m_checkBoxManifest = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_checkBoxCompressFiles = new System.Windows.Forms.CheckBox();
            this.m_checkBoxCreateOVA = new System.Windows.Forms.CheckBox();
            this.sectionHeaderLabel3 = new XenAdmin.Controls.SectionHeaderLabel();
            this.m_tableLayoutPanelEncryption = new System.Windows.Forms.TableLayoutPanel();
            this.m_labelPwd = new System.Windows.Forms.Label();
            this.m_textBoxPwd = new System.Windows.Forms.TextBox();
            this.m_textBoxReEnterPwd = new System.Windows.Forms.TextBox();
            this.m_labelReEnterPwd = new System.Windows.Forms.Label();
            this.m_pictureBoxTick = new System.Windows.Forms.PictureBox();
            this.m_labelStrength = new System.Windows.Forms.Label();
            this.sectionHeaderLabel2 = new XenAdmin.Controls.SectionHeaderLabel();
            this.sectionHeaderLabel1 = new XenAdmin.Controls.SectionHeaderLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.m_tableLayoutPanelManifest = new System.Windows.Forms.TableLayoutPanel();
            this.m_labelCertificate = new System.Windows.Forms.Label();
            this.m_labelPrivateKeyPwd = new System.Windows.Forms.Label();
            this.m_checkBoxSign = new System.Windows.Forms.CheckBox();
            this.m_buttonBrowseCert = new System.Windows.Forms.Button();
            this.m_textBoxPrivateKeyPwd = new System.Windows.Forms.TextBox();
            this.m_textBoxCertificate = new System.Windows.Forms.TextBox();
            this.m_ctrlErrorCert = new XenAdmin.Controls.Common.PasswordFailure();
            this.m_buttonValidate = new System.Windows.Forms.Button();
            this.m_pictureBoxTickValidate = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.m_tableLayoutPanelEncryption.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxTick)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.m_tableLayoutPanelManifest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxTickValidate)).BeginInit();
            this.SuspendLayout();
            // 
            // m_checkBoxEncrypt
            // 
            resources.ApplyResources(this.m_checkBoxEncrypt, "m_checkBoxEncrypt");
            this.m_tableLayoutPanelEncryption.SetColumnSpan(this.m_checkBoxEncrypt, 4);
            this.m_checkBoxEncrypt.Name = "m_checkBoxEncrypt";
            this.m_checkBoxEncrypt.UseVisualStyleBackColor = true;
            this.m_checkBoxEncrypt.CheckedChanged += new System.EventHandler(this.m_checkBoxEncrypt_CheckedChanged);
            // 
            // m_checkBoxManifest
            // 
            resources.ApplyResources(this.m_checkBoxManifest, "m_checkBoxManifest");
            this.m_checkBoxManifest.Name = "m_checkBoxManifest";
            this.m_checkBoxManifest.UseVisualStyleBackColor = true;
            this.m_checkBoxManifest.CheckedChanged += new System.EventHandler(this.m_checkBoxManifest_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.m_checkBoxCompressFiles, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.m_checkBoxCreateOVA, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.sectionHeaderLabel3, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.m_tableLayoutPanelEncryption, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.sectionHeaderLabel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.sectionHeaderLabel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // m_checkBoxCompressFiles
            // 
            resources.ApplyResources(this.m_checkBoxCompressFiles, "m_checkBoxCompressFiles");
            this.m_checkBoxCompressFiles.Name = "m_checkBoxCompressFiles";
            this.m_checkBoxCompressFiles.UseVisualStyleBackColor = true;
            this.m_checkBoxCompressFiles.CheckedChanged += new System.EventHandler(this.m_checkBoxCompressFiles_CheckedChanged);
            // 
            // m_checkBoxCreateOVA
            // 
            resources.ApplyResources(this.m_checkBoxCreateOVA, "m_checkBoxCreateOVA");
            this.m_checkBoxCreateOVA.Name = "m_checkBoxCreateOVA";
            this.m_checkBoxCreateOVA.UseVisualStyleBackColor = true;
            this.m_checkBoxCreateOVA.CheckedChanged += new System.EventHandler(this.m_checkBoxCreateOVA_CheckedChanged);
            // 
            // sectionHeaderLabel3
            // 
            resources.ApplyResources(this.sectionHeaderLabel3, "sectionHeaderLabel3");
            this.sectionHeaderLabel3.LineColor = System.Drawing.SystemColors.Window;
            this.sectionHeaderLabel3.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionHeaderLabel3.MinimumSize = new System.Drawing.Size(0, 14);
            this.sectionHeaderLabel3.Name = "sectionHeaderLabel3";
            // 
            // m_tableLayoutPanelEncryption
            // 
            resources.ApplyResources(this.m_tableLayoutPanelEncryption, "m_tableLayoutPanelEncryption");
            this.m_tableLayoutPanelEncryption.Controls.Add(this.m_labelPwd, 1, 1);
            this.m_tableLayoutPanelEncryption.Controls.Add(this.m_textBoxPwd, 2, 1);
            this.m_tableLayoutPanelEncryption.Controls.Add(this.m_textBoxReEnterPwd, 2, 2);
            this.m_tableLayoutPanelEncryption.Controls.Add(this.m_labelReEnterPwd, 1, 2);
            this.m_tableLayoutPanelEncryption.Controls.Add(this.m_pictureBoxTick, 3, 2);
            this.m_tableLayoutPanelEncryption.Controls.Add(this.m_labelStrength, 3, 1);
            this.m_tableLayoutPanelEncryption.Controls.Add(this.m_checkBoxEncrypt, 0, 0);
            this.m_tableLayoutPanelEncryption.Name = "m_tableLayoutPanelEncryption";
            // 
            // m_labelPwd
            // 
            resources.ApplyResources(this.m_labelPwd, "m_labelPwd");
            this.m_labelPwd.Name = "m_labelPwd";
            // 
            // m_textBoxPwd
            // 
            resources.ApplyResources(this.m_textBoxPwd, "m_textBoxPwd");
            this.m_textBoxPwd.Name = "m_textBoxPwd";
            this.m_textBoxPwd.UseSystemPasswordChar = true;
            this.m_textBoxPwd.TextChanged += new System.EventHandler(this.m_textBoxPwd_TextChanged);
            // 
            // m_textBoxReEnterPwd
            // 
            resources.ApplyResources(this.m_textBoxReEnterPwd, "m_textBoxReEnterPwd");
            this.m_textBoxReEnterPwd.Name = "m_textBoxReEnterPwd";
            this.m_textBoxReEnterPwd.UseSystemPasswordChar = true;
            this.m_textBoxReEnterPwd.TextChanged += new System.EventHandler(this.m_textBoxReEnterPwd_TextChanged);
            // 
            // m_labelReEnterPwd
            // 
            resources.ApplyResources(this.m_labelReEnterPwd, "m_labelReEnterPwd");
            this.m_labelReEnterPwd.Name = "m_labelReEnterPwd";
            // 
            // m_pictureBoxTick
            // 
            resources.ApplyResources(this.m_pictureBoxTick, "m_pictureBoxTick");
            this.m_pictureBoxTick.Image = global::XenAdmin.Properties.Resources._000_Tick_h32bit_16;
            this.m_pictureBoxTick.Name = "m_pictureBoxTick";
            this.m_pictureBoxTick.TabStop = false;
            // 
            // m_labelStrength
            // 
            resources.ApplyResources(this.m_labelStrength, "m_labelStrength");
            this.m_labelStrength.Name = "m_labelStrength";
            // 
            // sectionHeaderLabel2
            // 
            resources.ApplyResources(this.sectionHeaderLabel2, "sectionHeaderLabel2");
            this.sectionHeaderLabel2.LineColor = System.Drawing.SystemColors.Window;
            this.sectionHeaderLabel2.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionHeaderLabel2.MinimumSize = new System.Drawing.Size(0, 14);
            this.sectionHeaderLabel2.Name = "sectionHeaderLabel2";
            // 
            // sectionHeaderLabel1
            // 
            resources.ApplyResources(this.sectionHeaderLabel1, "sectionHeaderLabel1");
            this.sectionHeaderLabel1.LineColor = System.Drawing.SystemColors.Window;
            this.sectionHeaderLabel1.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionHeaderLabel1.MinimumSize = new System.Drawing.Size(0, 14);
            this.sectionHeaderLabel1.Name = "sectionHeaderLabel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.m_checkBoxManifest, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.m_tableLayoutPanelManifest, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // m_tableLayoutPanelManifest
            // 
            resources.ApplyResources(this.m_tableLayoutPanelManifest, "m_tableLayoutPanelManifest");
            this.m_tableLayoutPanelManifest.Controls.Add(this.m_labelCertificate, 2, 1);
            this.m_tableLayoutPanelManifest.Controls.Add(this.m_labelPrivateKeyPwd, 2, 2);
            this.m_tableLayoutPanelManifest.Controls.Add(this.m_checkBoxSign, 1, 0);
            this.m_tableLayoutPanelManifest.Controls.Add(this.m_buttonBrowseCert, 5, 1);
            this.m_tableLayoutPanelManifest.Controls.Add(this.m_textBoxPrivateKeyPwd, 3, 2);
            this.m_tableLayoutPanelManifest.Controls.Add(this.m_textBoxCertificate, 3, 1);
            this.m_tableLayoutPanelManifest.Controls.Add(this.m_ctrlErrorCert, 3, 3);
            this.m_tableLayoutPanelManifest.Controls.Add(this.m_buttonValidate, 5, 2);
            this.m_tableLayoutPanelManifest.Controls.Add(this.m_pictureBoxTickValidate, 6, 2);
            this.m_tableLayoutPanelManifest.Name = "m_tableLayoutPanelManifest";
            // 
            // m_labelCertificate
            // 
            resources.ApplyResources(this.m_labelCertificate, "m_labelCertificate");
            this.m_labelCertificate.Name = "m_labelCertificate";
            // 
            // m_labelPrivateKeyPwd
            // 
            resources.ApplyResources(this.m_labelPrivateKeyPwd, "m_labelPrivateKeyPwd");
            this.m_labelPrivateKeyPwd.Name = "m_labelPrivateKeyPwd";
            // 
            // m_checkBoxSign
            // 
            resources.ApplyResources(this.m_checkBoxSign, "m_checkBoxSign");
            this.m_tableLayoutPanelManifest.SetColumnSpan(this.m_checkBoxSign, 2);
            this.m_checkBoxSign.Name = "m_checkBoxSign";
            this.m_checkBoxSign.UseVisualStyleBackColor = true;
            this.m_checkBoxSign.CheckedChanged += new System.EventHandler(this.m_checkBoxSign_CheckedChanged);
            // 
            // m_buttonBrowseCert
            // 
            resources.ApplyResources(this.m_buttonBrowseCert, "m_buttonBrowseCert");
            this.m_buttonBrowseCert.Name = "m_buttonBrowseCert";
            this.m_buttonBrowseCert.UseVisualStyleBackColor = true;
            this.m_buttonBrowseCert.Click += new System.EventHandler(this.m_buttonBrowseCert_Click);
            // 
            // m_textBoxPrivateKeyPwd
            // 
            this.m_tableLayoutPanelManifest.SetColumnSpan(this.m_textBoxPrivateKeyPwd, 2);
            resources.ApplyResources(this.m_textBoxPrivateKeyPwd, "m_textBoxPrivateKeyPwd");
            this.m_textBoxPrivateKeyPwd.Name = "m_textBoxPrivateKeyPwd";
            this.m_textBoxPrivateKeyPwd.UseSystemPasswordChar = true;
            this.m_textBoxPrivateKeyPwd.TextChanged += new System.EventHandler(this.m_textBoxPrivateKeyPwd_TextChanged);
            // 
            // m_textBoxCertificate
            // 
            this.m_tableLayoutPanelManifest.SetColumnSpan(this.m_textBoxCertificate, 2);
            resources.ApplyResources(this.m_textBoxCertificate, "m_textBoxCertificate");
            this.m_textBoxCertificate.Name = "m_textBoxCertificate";
            this.m_textBoxCertificate.TextChanged += new System.EventHandler(this.m_textBoxCertificate_TextChanged);
            // 
            // m_ctrlErrorCert
            // 
            resources.ApplyResources(this.m_ctrlErrorCert, "m_ctrlErrorCert");
            this.m_tableLayoutPanelManifest.SetColumnSpan(this.m_ctrlErrorCert, 2);
            this.m_ctrlErrorCert.Name = "m_ctrlErrorCert";
            // 
            // m_buttonValidate
            // 
            resources.ApplyResources(this.m_buttonValidate, "m_buttonValidate");
            this.m_buttonValidate.Name = "m_buttonValidate";
            this.m_buttonValidate.UseVisualStyleBackColor = true;
            this.m_buttonValidate.Click += new System.EventHandler(this.m_buttonValidate_Click);
            // 
            // m_pictureBoxTickValidate
            // 
            resources.ApplyResources(this.m_pictureBoxTickValidate, "m_pictureBoxTickValidate");
            this.m_pictureBoxTickValidate.Image = global::XenAdmin.Properties.Resources._000_Tick_h32bit_16;
            this.m_pictureBoxTickValidate.Name = "m_pictureBoxTickValidate";
            this.m_pictureBoxTickValidate.TabStop = false;
            // 
            // ExportOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ExportOptionsPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.m_tableLayoutPanelEncryption.ResumeLayout(false);
            this.m_tableLayoutPanelEncryption.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxTick)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.m_tableLayoutPanelManifest.ResumeLayout(false);
            this.m_tableLayoutPanelManifest.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxTickValidate)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.CheckBox m_checkBoxEncrypt;
        private System.Windows.Forms.CheckBox m_checkBoxManifest;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label m_labelPrivateKeyPwd;
        private System.Windows.Forms.Label m_labelCertificate;
        private System.Windows.Forms.Label m_labelReEnterPwd;
		private System.Windows.Forms.Label m_labelPwd;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelManifest;
		private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanelEncryption;
		private System.Windows.Forms.CheckBox m_checkBoxSign;
		private System.Windows.Forms.Button m_buttonBrowseCert;
		private System.Windows.Forms.TextBox m_textBoxPwd;
		private System.Windows.Forms.TextBox m_textBoxReEnterPwd;
		private System.Windows.Forms.Label m_labelStrength;
		private System.Windows.Forms.TextBox m_textBoxPrivateKeyPwd;
		private System.Windows.Forms.TextBox m_textBoxCertificate;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlErrorCert;
		private System.Windows.Forms.PictureBox m_pictureBoxTick;
		private System.Windows.Forms.Button m_buttonValidate;
		private System.Windows.Forms.PictureBox m_pictureBoxTickValidate;
		private XenAdmin.Controls.SectionHeaderLabel sectionHeaderLabel1;
		private XenAdmin.Controls.SectionHeaderLabel sectionHeaderLabel2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private XenAdmin.Controls.SectionHeaderLabel sectionHeaderLabel3;
		private System.Windows.Forms.CheckBox m_checkBoxCreateOVA;
		private System.Windows.Forms.CheckBox m_checkBoxCompressFiles;
    }
}
