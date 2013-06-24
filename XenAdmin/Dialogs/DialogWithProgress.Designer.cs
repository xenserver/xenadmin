namespace XenAdmin.Dialogs
{
    partial class DialogWithProgress
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogWithProgress));
            this.ProgressSeparator = new System.Windows.Forms.Label();
            this.ActionStatusLabel = new System.Windows.Forms.Label();
            this.ActionProgressBar = new System.Windows.Forms.ProgressBar();
            this.ExceptionToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // ProgressSeparator
            // 
            resources.ApplyResources(this.ProgressSeparator, "ProgressSeparator");
            this.ProgressSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ProgressSeparator.Name = "ProgressSeparator";
            // 
            // ActionStatusLabel
            // 
            resources.ApplyResources(this.ActionStatusLabel, "ActionStatusLabel");
            this.ActionStatusLabel.AutoEllipsis = true;
            this.ActionStatusLabel.Name = "ActionStatusLabel";
            // 
            // ActionProgressBar
            // 
            resources.ApplyResources(this.ActionProgressBar, "ActionProgressBar");
            this.ActionProgressBar.Name = "ActionProgressBar";
            this.ActionProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // DialogWithProgress
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.ProgressSeparator);
            this.Controls.Add(this.ActionStatusLabel);
            this.Controls.Add(this.ActionProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "DialogWithProgress";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip ExceptionToolTip;
        protected System.Windows.Forms.Label ActionStatusLabel;
        protected System.Windows.Forms.Label ProgressSeparator;
        protected System.Windows.Forms.ProgressBar ActionProgressBar;
    }
}