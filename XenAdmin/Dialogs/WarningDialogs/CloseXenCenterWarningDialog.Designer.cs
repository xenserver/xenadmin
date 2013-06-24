namespace XenAdmin.Dialogs.WarningDialogs
{
    partial class CloseXenCenterWarningDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloseXenCenterWarningDialog));
            this.customHistoryContainer1 = new XenAdmin.Controls.CustomHistoryContainer();
            this.DontExitButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // customHistoryContainer1
            // 
            resources.ApplyResources(this.customHistoryContainer1, "customHistoryContainer1");
            this.customHistoryContainer1.BackColor = System.Drawing.SystemColors.Window;
            this.customHistoryContainer1.MaximumSize = new System.Drawing.Size(1024, 575);
            this.customHistoryContainer1.MinimumSize = new System.Drawing.Size(775, 10);
            this.customHistoryContainer1.Name = "customHistoryContainer1";
            // 
            // DontExitButton
            // 
            resources.ApplyResources(this.DontExitButton, "DontExitButton");
            this.DontExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.DontExitButton.Name = "DontExitButton";
            this.DontExitButton.UseVisualStyleBackColor = true;
            // 
            // ExitButton
            // 
            resources.ApplyResources(this.ExitButton, "ExitButton");
            this.ExitButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // CloseXenCenterWarningDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DontExitButton);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.customHistoryContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "CloseXenCenterWarningDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.CustomHistoryContainer customHistoryContainer1;
        private System.Windows.Forms.Button DontExitButton;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}