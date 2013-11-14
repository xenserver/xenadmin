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
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            this.pageContainerPanel.SizeChanged += new System.EventHandler(this.pageContainerPanel_SizeChanged);

            // 
            // GpuPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Name = "GpuPage";
            this.VisibleChanged += new System.EventHandler(this.GpuPage_VisibleChanged);
            this.Controls.SetChildIndex(this.pageContainerPanel, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

	}
}
