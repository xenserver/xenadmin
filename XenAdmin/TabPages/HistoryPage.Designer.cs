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
            this.toolStripTop = new System.Windows.Forms.ToolStrip();
            this.toolStripSplitButtonDismiss = new System.Windows.Forms.ToolStripSplitButton();
            this.tsmiDismissAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDismissSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.customHistoryContainer1 = new XenAdmin.Controls.CustomHistoryContainer();
            this.gradientPanel1 = new XenAdmin.Controls.GradientPanel.GradientPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStripTop.SuspendLayout();
            this.gradientPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripTop
            // 
            resources.ApplyResources(this.toolStripTop, "toolStripTop");
            this.toolStripTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButtonDismiss});
            this.toolStripTop.Name = "toolStripTop";
            // 
            // toolStripSplitButtonDismiss
            // 
            this.toolStripSplitButtonDismiss.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButtonDismiss.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDismissAll,
            this.tsmiDismissSelected});
            resources.ApplyResources(this.toolStripSplitButtonDismiss, "toolStripSplitButtonDismiss");
            this.toolStripSplitButtonDismiss.Name = "toolStripSplitButtonDismiss";
            this.toolStripSplitButtonDismiss.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripSplitButtonDismiss_DropDownItemClicked);
            // 
            // tsmiDismissAll
            // 
            this.tsmiDismissAll.Name = "tsmiDismissAll";
            resources.ApplyResources(this.tsmiDismissAll, "tsmiDismissAll");
            this.tsmiDismissAll.Click += new System.EventHandler(this.tsmiDismissAll_Click);
            // 
            // tsmiDismissSelected
            // 
            this.tsmiDismissSelected.Name = "tsmiDismissSelected";
            resources.ApplyResources(this.tsmiDismissSelected, "tsmiDismissSelected");
            this.tsmiDismissSelected.Click += new System.EventHandler(this.tsmiDismissSelected_Click);
            // 
            // customHistoryContainer1
            // 
            resources.ApplyResources(this.customHistoryContainer1, "customHistoryContainer1");
            this.customHistoryContainer1.BackColor = System.Drawing.Color.Transparent;
            this.customHistoryContainer1.Name = "customHistoryContainer1";
            // 
            // gradientPanel1
            // 
            resources.ApplyResources(this.gradientPanel1, "gradientPanel1");
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
            // HistoryPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.toolStripTop);
            this.Controls.Add(this.customHistoryContainer1);
            this.Controls.Add(this.gradientPanel1);
            this.DoubleBuffered = true;
            this.Name = "HistoryPage";
            this.toolStripTop.ResumeLayout(false);
            this.toolStripTop.PerformLayout();
            this.gradientPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.CustomHistoryContainer customHistoryContainer1;
        private System.Windows.Forms.ToolTip toolTip1;
        private XenAdmin.Controls.GradientPanel.GradientPanel gradientPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip toolStripTop;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonDismiss;
        private System.Windows.Forms.ToolStripMenuItem tsmiDismissAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiDismissSelected;
    }
}
