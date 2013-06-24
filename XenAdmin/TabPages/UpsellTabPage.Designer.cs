namespace XenAdmin.TabPages
{
    partial class UpsellTabPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpsellTabPage));
            this.TitleLabel = new System.Windows.Forms.Label();
            this.upsellPage1 = new XenAdmin.Controls.UpsellPage();
            this.pageContainerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.upsellPage1);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // TitleLabel
            // 
            resources.ApplyResources(this.TitleLabel, "TitleLabel");
            this.TitleLabel.AutoEllipsis = true;
            this.TitleLabel.ForeColor = System.Drawing.Color.White;
            this.TitleLabel.Name = "TitleLabel";
            // 
            // upsellPage1
            // 
            resources.ApplyResources(this.upsellPage1, "upsellPage1");
            this.upsellPage1.Image = ((System.Drawing.Image)(resources.GetObject("upsellPage1.Image")));
            this.upsellPage1.Name = "upsellPage1";
            // 
            // UpsellTabPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "UpsellTabPage";
            this.pageContainerPanel.ResumeLayout(false);
            this.pageContainerPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.UpsellPage upsellPage1;
        private System.Windows.Forms.Label TitleLabel;
    }
}
