namespace XenAdmin.Controls
{
    partial class UpsellPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpsellPage));
            this.LearnMoreButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.OKButton = new System.Windows.Forms.Button();
            this.Blurb = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LearnMoreButton
            // 
            resources.ApplyResources(this.LearnMoreButton, "LearnMoreButton");
            this.LearnMoreButton.Name = "LearnMoreButton";
            this.LearnMoreButton.UseVisualStyleBackColor = true;
            this.LearnMoreButton.Click += new System.EventHandler(this.LearnMoreButton_Clicked);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.Blurb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LearnMoreButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.OKButton, 2, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // OKButton
            // 
            resources.ApplyResources(this.OKButton, "OKButton");
            this.OKButton.Name = "OKButton";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // Blurb
            // 
            resources.ApplyResources(this.Blurb, "Blurb");
            this.tableLayoutPanel1.SetColumnSpan(this.Blurb, 3);
            this.Blurb.Name = "Blurb";
            // 
            // UpsellPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "UpsellPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LearnMoreButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        internal System.Windows.Forms.Button OKButton;
        private XenAdmin.Controls.Common.AutoHeightLabel Blurb;
    }
}
