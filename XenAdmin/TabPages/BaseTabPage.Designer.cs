namespace XenAdmin.TabPages
{
    partial class BaseTabPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseTabPage));
            this.pageContainerPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanelBanner = new System.Windows.Forms.TableLayoutPanel();
            this.deprecationBanner1 = new XenAdmin.Controls.DeprecationBanner();
            this.gradientPanel1 = new XenAdmin.Controls.GradientPanel.GradientPanel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanelBanner.SuspendLayout();
            this.gradientPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            this.pageContainerPanel.Name = "pageContainerPanel";
            // 
            // tableLayoutPanelBanner
            // 
            resources.ApplyResources(this.tableLayoutPanelBanner, "tableLayoutPanelBanner");
            this.tableLayoutPanelBanner.Controls.Add(this.deprecationBanner1, 1, 0);
            this.tableLayoutPanelBanner.Name = "tableLayoutPanelBanner";
            // 
            // deprecationBanner1
            // 
            resources.ApplyResources(this.deprecationBanner1, "deprecationBanner1");
            this.deprecationBanner1.BackColor = System.Drawing.Color.LightCoral;
            this.deprecationBanner1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deprecationBanner1.Name = "deprecationBanner1";
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.Controls.Add(this.titleLabel);
            resources.ApplyResources(this.gradientPanel1, "gradientPanel1");
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Scheme = XenAdmin.Controls.GradientPanel.GradientPanel.Schemes.Tab;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoEllipsis = true;
            this.titleLabel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.titleLabel, "titleLabel");
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.UseMnemonic = false;
            // 
            // BaseTabPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.pageContainerPanel);
            this.Controls.Add(this.tableLayoutPanelBanner);
            this.Controls.Add(this.gradientPanel1);
            this.Name = "BaseTabPage";
            this.tableLayoutPanelBanner.ResumeLayout(false);
            this.tableLayoutPanelBanner.PerformLayout();
            this.gradientPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.GradientPanel.GradientPanel gradientPanel1;
        private System.Windows.Forms.Label titleLabel;
        protected System.Windows.Forms.Panel pageContainerPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBanner;
        private XenAdmin.Controls.DeprecationBanner deprecationBanner1;

    }
}
