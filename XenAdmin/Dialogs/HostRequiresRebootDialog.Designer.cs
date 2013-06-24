namespace XenAdmin.Dialogs
{
    partial class HostRequiresRebootDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HostRequiresRebootDialog));
            this.CloseButton = new System.Windows.Forms.Button();
            this.RebootButton = new System.Windows.Forms.Button();
            this.BlurbLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // RebootButton
            // 
            resources.ApplyResources(this.RebootButton, "RebootButton");
            this.RebootButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.RebootButton.Name = "RebootButton";
            this.RebootButton.UseVisualStyleBackColor = true;
            // 
            // BlurbLabel
            // 
            resources.ApplyResources(this.BlurbLabel, "BlurbLabel");
            this.BlurbLabel.Name = "BlurbLabel";
            // 
            // HostRequiresRebootDialog
            // 
            this.AcceptButton = this.RebootButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.CloseButton;
            this.Controls.Add(this.BlurbLabel);
            this.Controls.Add(this.RebootButton);
            this.Controls.Add(this.CloseButton);
            this.HelpButton = false;
            this.Name = "HostRequiresRebootDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button RebootButton;
        private System.Windows.Forms.Label BlurbLabel;
    }
}