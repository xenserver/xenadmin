namespace XenAdmin.Dialogs
{
    partial class UpsellDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpsellDialog));
            this.upsellPage1 = new XenAdmin.Controls.UpsellPage();
            this.SuspendLayout();
            // 
            // upsellPage1
            // 
            resources.ApplyResources(this.upsellPage1, "upsellPage1");
            this.upsellPage1.Image = ((System.Drawing.Image)(resources.GetObject("upsellPage1.Image")));
            this.upsellPage1.Name = "upsellPage1";
            // 
            // UpsellDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.upsellPage1);
            this.HelpButton = false;
            this.Name = "UpsellDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.UpsellPage upsellPage1;

    }
}