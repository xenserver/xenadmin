using System.Windows.Forms;

namespace XenAdmin.TabPages
{
    partial class TagCloudPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagCloudPage));
            this.panel2 = new System.Windows.Forms.Panel();
            this.gradientPanel1 = new XenAdmin.Controls.GradientPanel.GradientPanel();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel = new FlowLayoutPanel();
            this.gradientPanel1.SuspendLayout();
            this.flowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel2.Name = "panel2";
            // 
            // gradientPanel1
            // 
            resources.ApplyResources(this.gradientPanel1, "gradientPanel1");
            this.gradientPanel1.Controls.Add(this.TitleLabel);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Scheme = XenAdmin.Controls.GradientPanel.GradientPanel.Schemes.Tab;
            // 
            // TitleLabel
            // 
            resources.ApplyResources(this.TitleLabel, "TitleLabel");
            this.TitleLabel.ForeColor = System.Drawing.Color.White;
            this.TitleLabel.Name = "TitleLabel";
            // 
            // flowLayoutPanel
            // 
            resources.ApplyResources(this.flowLayoutPanel, "flowLayoutPanel");
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Dock = DockStyle.Fill;
            this.flowLayoutPanel.Padding=new Padding(20,50,20,20);
            this.flowLayoutPanel.WrapContents = true;
            this.flowLayoutPanel.AutoScroll = true;
 
            // 
            // TagCloudPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.gradientPanel1);
            this.Controls.Add(this.flowLayoutPanel);
            this.Controls.Add(this.panel2);
            this.DoubleBuffered = true;
            this.Name = "TagCloudPage";
            this.gradientPanel1.ResumeLayout(false);
            this.flowLayoutPanel.ResumeLayout(false);
            this.flowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label TitleLabel;
        private FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Panel panel2;
        private XenAdmin.Controls.GradientPanel.GradientPanel gradientPanel1;

    }
}
