using XenAdmin.Wizards.PatchingWizard;

namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    partial class RollingUpgradeWizardInstallMethodPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RollingUpgradeWizardInstallMethodPage));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.comboBoxUpgradeMethod = new System.Windows.Forms.ComboBox();
            this.watermarkTextBox1 = new System.Windows.Forms.TextBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelUser = new System.Windows.Forms.Label();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelErrors = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.labelSelectDiskFile = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.checkBoxInstallSuppPack = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.groupBox2, 3);
            this.groupBox2.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.comboBoxUpgradeMethod, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.watermarkTextBox1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonTest, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelUser, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxUser, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelPassword, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxPassword, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // comboBoxUpgradeMethod
            // 
            this.comboBoxUpgradeMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUpgradeMethod.FormattingEnabled = true;
            this.comboBoxUpgradeMethod.Items.AddRange(new object[] {
            resources.GetString("comboBoxUpgradeMethod.Items"),
            resources.GetString("comboBoxUpgradeMethod.Items1"),
            resources.GetString("comboBoxUpgradeMethod.Items2")});
            resources.ApplyResources(this.comboBoxUpgradeMethod, "comboBoxUpgradeMethod");
            this.comboBoxUpgradeMethod.Name = "comboBoxUpgradeMethod";
            this.comboBoxUpgradeMethod.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // watermarkTextBox1
            // 
            resources.ApplyResources(this.watermarkTextBox1, "watermarkTextBox1");
            this.watermarkTextBox1.Name = "watermarkTextBox1";
            this.watermarkTextBox1.TextChanged += new System.EventHandler(this.watermarkTextBox1_TextChanged);
            // 
            // buttonTest
            // 
            resources.ApplyResources(this.buttonTest, "buttonTest");
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // labelUser
            // 
            resources.ApplyResources(this.labelUser, "labelUser");
            this.labelUser.Name = "labelUser";
            // 
            // textBoxUser
            // 
            resources.ApplyResources(this.textBoxUser, "textBoxUser");
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.TextChanged += new System.EventHandler(this.textBoxUser_TextChanged);
            // 
            // labelPassword
            // 
            resources.ApplyResources(this.labelPassword, "labelPassword");
            this.labelPassword.Name = "labelPassword";
            // 
            // textBoxPassword
            // 
            resources.ApplyResources(this.textBoxPassword, "textBoxPassword");
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel2.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // labelErrors
            // 
            resources.ApplyResources(this.labelErrors, "labelErrors");
            this.tableLayoutPanel2.SetColumnSpan(this.labelErrors, 2);
            this.labelErrors.Name = "labelErrors";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_Abort_h32bit_16;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // labelSelectDiskFile
            // 
            resources.ApplyResources(this.labelSelectDiskFile, "labelSelectDiskFile");
            this.tableLayoutPanel2.SetColumnSpan(this.labelSelectDiskFile, 2);
            this.labelSelectDiskFile.Name = "labelSelectDiskFile";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelSelectDiskFile, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox2, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelErrors, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.checkBoxInstallSuppPack, 0, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel3, 2);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.BrowseButton, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.fileNameTextBox, 1, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // BrowseButton
            // 
            resources.ApplyResources(this.BrowseButton, "BrowseButton");
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // fileNameTextBox
            // 
            resources.ApplyResources(this.fileNameTextBox, "fileNameTextBox");
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.TextChanged += new System.EventHandler(this.fileNameTextBox_TextChanged);
            this.fileNameTextBox.Enter += new System.EventHandler(this.fileNameTextBox_Enter);
            // 
            // checkBoxInstallSuppPack
            // 
            resources.ApplyResources(this.checkBoxInstallSuppPack, "checkBoxInstallSuppPack");
            this.tableLayoutPanel2.SetColumnSpan(this.checkBoxInstallSuppPack, 3);
            this.checkBoxInstallSuppPack.Name = "checkBoxInstallSuppPack";
            this.checkBoxInstallSuppPack.UseVisualStyleBackColor = true;
            this.checkBoxInstallSuppPack.CheckedChanged += new System.EventHandler(this.checkBoxInstallSuppPack_CheckedChanged);
            // 
            // RollingUpgradeWizardInstallMethodPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "RollingUpgradeWizardInstallMethodPage";
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxUpgradeMethod;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Label labelUser;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.TextBox watermarkTextBox1;
        private System.Windows.Forms.Label labelErrors;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label labelSelectDiskFile;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.CheckBox checkBoxInstallSuppPack;


    }
}
