namespace XenAdmin.Dialogs.Network
{
    partial class CertificateChangedDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CertificateChangedDialog));
            this.Cancelbutton = new System.Windows.Forms.Button();
            this.Okbutton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BlurbLabel = new System.Windows.Forms.Label();
            this.AlwaysIgnoreCheckBox = new System.Windows.Forms.CheckBox();
            this.ViewCertificateButton = new System.Windows.Forms.Button();
            this.Blurb2Label = new System.Windows.Forms.Label();
            this.Blurb3Label = new System.Windows.Forms.Label();
            this.labelTrusted = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Cancelbutton
            // 
            resources.ApplyResources(this.Cancelbutton, "Cancelbutton");
            this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancelbutton.Name = "Cancelbutton";
            this.Cancelbutton.UseVisualStyleBackColor = true;
            // 
            // Okbutton
            // 
            resources.ApplyResources(this.Okbutton, "Okbutton");
            this.Okbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Okbutton.Name = "Okbutton";
            this.Okbutton.UseVisualStyleBackColor = true;
            this.Okbutton.Click += new System.EventHandler(this.Okbutton_Click);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // BlurbLabel
            // 
            resources.ApplyResources(this.BlurbLabel, "BlurbLabel");
            this.BlurbLabel.Name = "BlurbLabel";
            // 
            // AlwaysIgnoreCheckBox
            // 
            resources.ApplyResources(this.AlwaysIgnoreCheckBox, "AlwaysIgnoreCheckBox");
            this.AlwaysIgnoreCheckBox.Name = "AlwaysIgnoreCheckBox";
            this.AlwaysIgnoreCheckBox.UseVisualStyleBackColor = true;
            // 
            // ViewCertificateButton
            // 
            resources.ApplyResources(this.ViewCertificateButton, "ViewCertificateButton");
            this.ViewCertificateButton.Name = "ViewCertificateButton";
            this.ViewCertificateButton.UseVisualStyleBackColor = true;
            this.ViewCertificateButton.Click += new System.EventHandler(this.ViewCertificateButton_Click);
            // 
            // Blurb2Label
            // 
            resources.ApplyResources(this.Blurb2Label, "Blurb2Label");
            this.Blurb2Label.Name = "Blurb2Label";
            // 
            // Blurb3Label
            // 
            resources.ApplyResources(this.Blurb3Label, "Blurb3Label");
            this.Blurb3Label.Name = "Blurb3Label";
            // 
            // labelTrusted
            // 
            resources.ApplyResources(this.labelTrusted, "labelTrusted");
            this.labelTrusted.Name = "labelTrusted";
            // 
            // CertificateChangedDialog
            // 
            this.AcceptButton = this.Okbutton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.Cancelbutton;
            this.Controls.Add(this.labelTrusted);
            this.Controls.Add(this.Blurb3Label);
            this.Controls.Add(this.Blurb2Label);
            this.Controls.Add(this.ViewCertificateButton);
            this.Controls.Add(this.AlwaysIgnoreCheckBox);
            this.Controls.Add(this.BlurbLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Okbutton);
            this.Controls.Add(this.Cancelbutton);
            this.Name = "CertificateChangedDialog";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancelbutton;
        private System.Windows.Forms.Button Okbutton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label BlurbLabel;
        private System.Windows.Forms.CheckBox AlwaysIgnoreCheckBox;
        private System.Windows.Forms.Button ViewCertificateButton;
        private System.Windows.Forms.Label Blurb2Label;
        private System.Windows.Forms.Label Blurb3Label;
        private System.Windows.Forms.Label labelTrusted;
    }
}