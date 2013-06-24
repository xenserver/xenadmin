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
            this.SSLLabel = new System.Windows.Forms.Label();
            this.SSLTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.CertificateChangedCheckBox = new System.Windows.Forms.CheckBox();
            this.CertificateFoundCheckBox = new System.Windows.Forms.CheckBox();
            this.SecurityTableLayoutPanel.SuspendLayout();
            this.SSLTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SecurityTableLayoutPanel
            // 
            resources.ApplyResources(this.SecurityTableLayoutPanel, "SecurityTableLayoutPanel");
            this.SecurityTableLayoutPanel.Controls.Add(this.SSLTableLayoutPanel, 0, 1);
            this.SecurityTableLayoutPanel.Controls.Add(this.SSLLabel, 0, 0);
            this.SecurityTableLayoutPanel.Name = "SecurityTableLayoutPanel";
            // 
            // SSLLabel
            // 
            resources.ApplyResources(this.SSLLabel, "SSLLabel");
            this.SSLLabel.Name = "SSLLabel";
            // 
            // SSLTableLayoutPanel
            // 
            resources.ApplyResources(this.SSLTableLayoutPanel, "SSLTableLayoutPanel");
            this.SSLTableLayoutPanel.Controls.Add(this.CertificateChangedCheckBox, 0, 1);
            this.SSLTableLayoutPanel.Controls.Add(this.CertificateFoundCheckBox, 0, 0);
            this.SSLTableLayoutPanel.Name = "SSLTableLayoutPanel";
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
            // SecurityOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.SecurityTableLayoutPanel);
            this.Name = "SecurityOptionsPage";
            this.SecurityTableLayoutPanel.ResumeLayout(false);
            this.SecurityTableLayoutPanel.PerformLayout();
            this.SSLTableLayoutPanel.ResumeLayout(false);
            this.SSLTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel SecurityTableLayoutPanel;
        private System.Windows.Forms.Label SSLLabel;
        private System.Windows.Forms.TableLayoutPanel SSLTableLayoutPanel;
        private System.Windows.Forms.CheckBox CertificateChangedCheckBox;
        private System.Windows.Forms.CheckBox CertificateFoundCheckBox;
    }
}
