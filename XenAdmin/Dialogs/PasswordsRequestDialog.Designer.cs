namespace XenAdmin.Dialogs
{
    partial class PasswordsRequestDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasswordsRequestDialog));
            this.OKThisTimeButton = new System.Windows.Forms.Button();
            this.OKAlwaysButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.ApplicationLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.OKAlwaysTooltipContainer = new XenAdmin.Controls.ToolTipContainer();
            this.OKAlwaysTooltipContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // OKThisTimeButton
            // 
            resources.ApplyResources(this.OKThisTimeButton, "OKThisTimeButton");
            this.OKThisTimeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKThisTimeButton.Name = "OKThisTimeButton";
            this.OKThisTimeButton.UseVisualStyleBackColor = true;
            // 
            // OKAlwaysButton
            // 
            this.OKAlwaysButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            resources.ApplyResources(this.OKAlwaysButton, "OKAlwaysButton");
            this.OKAlwaysButton.Name = "OKAlwaysButton";
            this.OKAlwaysButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // CancelBtn
            // 
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // ApplicationLabel
            // 
            resources.ApplyResources(this.ApplicationLabel, "ApplicationLabel");
            this.ApplicationLabel.Name = "ApplicationLabel";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // OKAlwaysTooltipContainer
            // 
            resources.ApplyResources(this.OKAlwaysTooltipContainer, "OKAlwaysTooltipContainer");
            this.OKAlwaysTooltipContainer.Controls.Add(this.OKAlwaysButton);
            this.OKAlwaysTooltipContainer.Name = "OKAlwaysTooltipContainer";
            // 
            // PasswordsRequestDialog
            // 
            this.AcceptButton = this.OKThisTimeButton;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.CancelBtn;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.OKAlwaysTooltipContainer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ApplicationLabel);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OKThisTimeButton);
            this.HelpButton = false;
            this.Name = "PasswordsRequestDialog";
            this.Load += new System.EventHandler(this.PasswordsRequestDialog_Load);
            this.OKAlwaysTooltipContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OKThisTimeButton;
        private System.Windows.Forms.Button OKAlwaysButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Label ApplicationLabel;
        private System.Windows.Forms.Label label3;
        private XenAdmin.Controls.ToolTipContainer OKAlwaysTooltipContainer;
    }
}
