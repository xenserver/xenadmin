namespace XenAdmin.Dialogs.OptionsPages
{
    partial class SecurityOptionsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SecurityOptionsPage));
            this.SecurityTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.CertificateChangedCheckBox = new System.Windows.Forms.CheckBox();
            this.CertificateFoundCheckBox = new System.Windows.Forms.CheckBox();
            this.SSLLabel = new System.Windows.Forms.Label();
            this.labelReminder = new System.Windows.Forms.Label();
            this.checkBoxReminder = new System.Windows.Forms.CheckBox();
            this.SecurityTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SecurityTableLayoutPanel
            // 
            resources.ApplyResources(this.SecurityTableLayoutPanel, "SecurityTableLayoutPanel");
            this.SecurityTableLayoutPanel.Controls.Add(this.SSLLabel, 0, 0);
            this.SecurityTableLayoutPanel.Controls.Add(this.CertificateFoundCheckBox, 0, 1);
            this.SecurityTableLayoutPanel.Controls.Add(this.CertificateChangedCheckBox, 0, 2);
            this.SecurityTableLayoutPanel.Controls.Add(this.labelReminder, 0, 3);
            this.SecurityTableLayoutPanel.Controls.Add(this.checkBoxReminder, 0, 4);
            this.SecurityTableLayoutPanel.Name = "SecurityTableLayoutPanel";
            // 
            // CertificateChangedCheckBox
            // 
            resources.ApplyResources(this.CertificateChangedCheckBox, "CertificateChangedCheckBox");
            this.CertificateChangedCheckBox.Name = "CertificateChangedCheckBox";
            this.CertificateChangedCheckBox.UseVisualStyleBackColor = true;
            // 
            // CertificateFoundCheckBox
            // 
            resources.ApplyResources(this.CertificateFoundCheckBox, "CertificateFoundCheckBox");
            this.CertificateFoundCheckBox.Name = "CertificateFoundCheckBox";
            this.CertificateFoundCheckBox.UseVisualStyleBackColor = true;
            // 
            // SSLLabel
            // 
            resources.ApplyResources(this.SSLLabel, "SSLLabel");
            this.SSLLabel.Name = "SSLLabel";
            // 
            // labelReminder
            // 
            resources.ApplyResources(this.labelReminder, "labelReminder");
            this.labelReminder.Name = "labelReminder";
            // 
            // checkBoxReminder
            // 
            resources.ApplyResources(this.checkBoxReminder, "checkBoxReminder");
            this.checkBoxReminder.Name = "checkBoxReminder";
            this.checkBoxReminder.UseVisualStyleBackColor = true;
            // 
            // SecurityOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.SecurityTableLayoutPanel);
            this.Name = "SecurityOptionsPage";
            this.SecurityTableLayoutPanel.ResumeLayout(false);
            this.SecurityTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel SecurityTableLayoutPanel;
        private System.Windows.Forms.Label SSLLabel;
        private System.Windows.Forms.CheckBox CertificateChangedCheckBox;
        private System.Windows.Forms.CheckBox CertificateFoundCheckBox;
        private System.Windows.Forms.Label labelReminder;
        private System.Windows.Forms.CheckBox checkBoxReminder;
    }
}
