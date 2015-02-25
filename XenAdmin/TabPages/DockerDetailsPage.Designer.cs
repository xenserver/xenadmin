namespace XenAdmin.TabPages
{
    partial class DockerDetailsPage
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockerDetailsPage));
            this.RefreshButton = new System.Windows.Forms.Button();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.RefreshTime = new System.Windows.Forms.Label();
            this.TreePanel = new System.Windows.Forms.Panel();
            this.DetailtreeView = new System.Windows.Forms.TreeView();
            this.RefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.pageContainerPanel.SuspendLayout();
            this.ButtonPanel.SuspendLayout();
            this.TreePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.TreePanel);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // RefreshButton
            // 
            resources.ApplyResources(this.RefreshButton, "RefreshButton");
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.Controls.Add(this.RefreshTime);
            this.ButtonPanel.Controls.Add(this.RefreshButton);
            resources.ApplyResources(this.ButtonPanel, "ButtonPanel");
            this.ButtonPanel.Name = "ButtonPanel";
            // 
            // RefreshTime
            // 
            resources.ApplyResources(this.RefreshTime, "RefreshTime");
            this.RefreshTime.Name = "RefreshTime";
            // 
            // TreePanel
            // 
            resources.ApplyResources(this.TreePanel, "TreePanel");
            this.TreePanel.Controls.Add(this.DetailtreeView);
            this.TreePanel.Controls.Add(this.ButtonPanel);
            this.TreePanel.Name = "TreePanel";
            // 
            // DetailtreeView
            // 
            resources.ApplyResources(this.DetailtreeView, "DetailtreeView");
            this.DetailtreeView.Name = "DetailtreeView";
            // 
            // RefreshTimer
            // 
            this.RefreshTimer.Tick += new System.EventHandler(this.RefreshTimer_Tick);
            // 
            // DockerDetailsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Name = "DockerDetailsPage";
            this.pageContainerPanel.ResumeLayout(false);
            this.pageContainerPanel.PerformLayout();
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            this.TreePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.Panel TreePanel;
        private System.Windows.Forms.Panel ButtonPanel;
        private System.Windows.Forms.Label RefreshTime;
        private System.Windows.Forms.TreeView DetailtreeView;
        private System.Windows.Forms.Timer RefreshTimer;

    }
}
