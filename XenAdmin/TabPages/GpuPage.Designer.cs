namespace XenAdmin.TabPages
{
	partial class GpuPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GpuPage));
            this.gpuPlacementPolicyPanel1 = new XenAdmin.Controls.GpuPlacementPolicyPanel();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            this.pageContainerPanel.SizeChanged += new System.EventHandler(this.pageContainerPanel_SizeChanged);
            // 
            // gpuPlacementPolicyPanel1
            // 
            resources.ApplyResources(this.gpuPlacementPolicyPanel1, "gpuPlacementPolicyPanel1");
            this.gpuPlacementPolicyPanel1.BackColor = System.Drawing.Color.Transparent;
            this.gpuPlacementPolicyPanel1.MinimumSize = new System.Drawing.Size(393, 35);
            this.gpuPlacementPolicyPanel1.Name = "gpuPlacementPolicyPanel1";
            // 
            // GpuPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.gpuPlacementPolicyPanel1);
            this.Name = "GpuPage";
            this.VisibleChanged += new System.EventHandler(this.GpuPage_VisibleChanged);
            this.Controls.SetChildIndex(this.gpuPlacementPolicyPanel1, 0);
            this.Controls.SetChildIndex(this.pageContainerPanel, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private XenAdmin.Controls.GpuPlacementPolicyPanel gpuPlacementPolicyPanel1;
	}
}
