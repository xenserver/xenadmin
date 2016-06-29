namespace XenAdmin.Wizards.ImportWizard
{
	partial class StoragePickerPage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoragePickerPage));
			this.m_srPicker = new XenAdmin.Controls.SrPicker();
			this.SuspendLayout();
			// 
			// m_srPicker
			// 
			this.m_srPicker.Connection = null;
			resources.ApplyResources(this.m_srPicker, "m_srPicker");
			this.m_srPicker.Name = "m_srPicker";
			this.m_srPicker.ItemSelectionNull += new System.Action(this.m_srPicker_ItemSelectionNull);
            this.m_srPicker.ItemSelectionNotNull += new System.Action(this.m_srPicker_ItemSelectionNotNull);
			// 
			// StoragePickerPage
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.m_srPicker);
			this.Name = "StoragePickerPage";
			this.ResumeLayout(false);

		}

		#endregion

        private XenAdmin.Controls.SrPicker m_srPicker;
	}
}
