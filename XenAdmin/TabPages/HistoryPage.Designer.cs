namespace XenAdmin.TabPages
{
    partial class HistoryPage
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
            ConnectionsManager.XenConnections.CollectionChanged -= History_CollectionChanged;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryPage));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.gradientPanel1 = new XenAdmin.Controls.GradientPanel.GradientPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.customHistoryContainer1 = new XenAdmin.Controls.CustomHistoryContainer();
            this.gradientPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.toolTip1.SetToolTip(this.button1, resources.GetString("button1.ToolTip"));
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // gradientPanel1
            // 
            resources.ApplyResources(this.gradientPanel1, "gradientPanel1");
            this.gradientPanel1.Controls.Add(this.button1);
            this.gradientPanel1.Controls.Add(this.label1);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Scheme = XenAdmin.Controls.GradientPanel.GradientPanel.Schemes.Tab;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // customHistoryContainer1
            // 
            resources.ApplyResources(this.customHistoryContainer1, "customHistoryContainer1");
            this.customHistoryContainer1.BackColor = System.Drawing.Color.Transparent;
            this.customHistoryContainer1.Name = "customHistoryContainer1";
            // 
            // HistoryPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.gradientPanel1);
            this.Controls.Add(this.customHistoryContainer1);
            this.DoubleBuffered = true;
            this.Name = "HistoryPage";
            this.gradientPanel1.ResumeLayout(false);
            this.gradientPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.CustomHistoryContainer customHistoryContainer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip toolTip1;
        private XenAdmin.Controls.GradientPanel.GradientPanel gradientPanel1;
        private System.Windows.Forms.Label label1;
    }
}
