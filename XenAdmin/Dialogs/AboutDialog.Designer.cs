namespace XenAdmin.Dialogs
{
    partial class AboutDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            this.OkButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LicenseDetailsTextBox = new System.Windows.Forms.TextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.licenseDetailsLabel = new System.Windows.Forms.Label();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.showAgainCheckBox = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // OkButton
            // 
            resources.ApplyResources(this.OkButton, "OkButton");
            this.OkButton.BackColor = System.Drawing.SystemColors.Control;
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Name = "OkButton";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.Controls.Add(this.LicenseDetailsTextBox, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.OkButton, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.linkLabel1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.licenseDetailsLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.VersionLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.showAgainCheckBox, 0, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // LicenseDetailsTextBox
            // 
            this.LicenseDetailsTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.tableLayoutPanel1.SetColumnSpan(this.LicenseDetailsTextBox, 2);
            this.LicenseDetailsTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.LicenseDetailsTextBox, "LicenseDetailsTextBox");
            this.LicenseDetailsTextBox.Name = "LicenseDetailsTextBox";
            this.LicenseDetailsTextBox.ReadOnly = true;
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Name = "label2";
            // 
            // licenseDetailsLabel
            // 
            resources.ApplyResources(this.licenseDetailsLabel, "licenseDetailsLabel");
            this.licenseDetailsLabel.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.SetColumnSpan(this.licenseDetailsLabel, 2);
            this.licenseDetailsLabel.Name = "licenseDetailsLabel";
            // 
            // VersionLabel
            // 
            resources.ApplyResources(this.VersionLabel, "VersionLabel");
            this.VersionLabel.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.SetColumnSpan(this.VersionLabel, 2);
            this.VersionLabel.Name = "VersionLabel";
            // 
            // showAgainCheckBox
            // 
            resources.ApplyResources(this.showAgainCheckBox, "showAgainCheckBox");
            this.showAgainCheckBox.Name = "showAgainCheckBox";
            this.showAgainCheckBox.UseVisualStyleBackColor = true;
            this.showAgainCheckBox.CheckedChanged += new System.EventHandler(this.showAgainCheckBox_CheckedChanged);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources.about_box_graphic_423x79;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // AboutDialog
            // 
            this.AcceptButton = this.OkButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.OkButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.pictureBox1);
            this.HelpButton = false;
            this.Name = "AboutDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label licenseDetailsLabel;
        private System.Windows.Forms.TextBox LicenseDetailsTextBox;
        private System.Windows.Forms.CheckBox showAgainCheckBox;
    }
}
