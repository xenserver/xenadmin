namespace XenAdmin.Dialogs
{
    partial class RebootHostsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RebootHostsDialog));
            this.rebootButton = new System.Windows.Forms.Button();
            this.noRebootButton = new System.Windows.Forms.Button();
            this.mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.hostsLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.mainLabel = new System.Windows.Forms.Label();
            this.buttonsLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.mainLayoutPanel.SuspendLayout();
            this.buttonsLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // rebootButton
            // 
            resources.ApplyResources(this.rebootButton, "rebootButton");
            this.rebootButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.rebootButton.Name = "rebootButton";
            this.rebootButton.UseVisualStyleBackColor = true;
            // 
            // noRebootButton
            // 
            resources.ApplyResources(this.noRebootButton, "noRebootButton");
            this.noRebootButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.noRebootButton.Name = "noRebootButton";
            this.noRebootButton.UseVisualStyleBackColor = true;
            // 
            // mainLayoutPanel
            // 
            resources.ApplyResources(this.mainLayoutPanel, "mainLayoutPanel");
            this.mainLayoutPanel.Controls.Add(this.hostsLayoutPanel, 0, 1);
            this.mainLayoutPanel.Controls.Add(this.progressBar, 0, 2);
            this.mainLayoutPanel.Controls.Add(this.textBoxLog, 0, 3);
            this.mainLayoutPanel.Controls.Add(this.mainLabel, 0, 0);
            this.mainLayoutPanel.Controls.Add(this.buttonsLayoutPanel, 0, 4);
            this.mainLayoutPanel.Name = "mainLayoutPanel";
            // 
            // hostsLayoutPanel
            // 
            resources.ApplyResources(this.hostsLayoutPanel, "hostsLayoutPanel");
            this.hostsLayoutPanel.Name = "hostsLayoutPanel";
            // 
            // progressBar
            // 
            resources.ApplyResources(this.progressBar, "progressBar");
            this.progressBar.Name = "progressBar";
            // 
            // textBoxLog
            // 
            resources.ApplyResources(this.textBoxLog, "textBoxLog");
            this.textBoxLog.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            // 
            // mainLabel
            // 
            resources.ApplyResources(this.mainLabel, "mainLabel");
            this.mainLabel.Name = "mainLabel";
            // 
            // buttonsLayoutPanel
            // 
            resources.ApplyResources(this.buttonsLayoutPanel, "buttonsLayoutPanel");
            this.buttonsLayoutPanel.Controls.Add(this.rebootButton);
            this.buttonsLayoutPanel.Controls.Add(this.noRebootButton);
            this.buttonsLayoutPanel.Controls.Add(this.cancelButton);
            this.buttonsLayoutPanel.Name = "buttonsLayoutPanel";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // RebootHostsDialog
            // 
            this.AcceptButton = this.rebootButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.noRebootButton;
            this.Controls.Add(this.mainLayoutPanel);
            this.Name = "RebootHostsDialog";
            this.mainLayoutPanel.ResumeLayout(false);
            this.mainLayoutPanel.PerformLayout();
            this.buttonsLayoutPanel.ResumeLayout(false);
            this.buttonsLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button rebootButton;
        private System.Windows.Forms.Button noRebootButton;
        private System.Windows.Forms.TableLayoutPanel mainLayoutPanel;
        private System.Windows.Forms.Label mainLabel;
        private System.Windows.Forms.FlowLayoutPanel buttonsLayoutPanel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.FlowLayoutPanel hostsLayoutPanel;
    }
}