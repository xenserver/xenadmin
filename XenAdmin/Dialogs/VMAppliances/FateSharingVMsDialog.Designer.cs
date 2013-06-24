namespace XenAdmin.Dialogs.VMAppliances
{
	partial class FateSharingVMsDialog
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FateSharingVMsDialog));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
			this.m_buttonYes = new System.Windows.Forms.Button();
			this.m_buttonNo = new System.Windows.Forms.Button();
			this.autoHeightLabel2 = new XenAdmin.Controls.Common.AutoHeightLabel();
			this.m_listBoxVMs = new System.Windows.Forms.ListBox();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.autoHeightLabel1, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.m_buttonYes, 2, 3);
			this.tableLayoutPanel1.Controls.Add(this.m_buttonNo, 3, 3);
			this.tableLayoutPanel1.Controls.Add(this.autoHeightLabel2, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.m_listBoxVMs, 1, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// autoHeightLabel1
			// 
			resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
			this.tableLayoutPanel1.SetColumnSpan(this.autoHeightLabel1, 3);
			this.autoHeightLabel1.Name = "autoHeightLabel1";
			// 
			// m_buttonYes
			// 
			this.m_buttonYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
			resources.ApplyResources(this.m_buttonYes, "m_buttonYes");
			this.m_buttonYes.Name = "m_buttonYes";
			this.m_buttonYes.UseVisualStyleBackColor = true;
			// 
			// m_buttonNo
			// 
			this.m_buttonNo.DialogResult = System.Windows.Forms.DialogResult.No;
			resources.ApplyResources(this.m_buttonNo, "m_buttonNo");
			this.m_buttonNo.Name = "m_buttonNo";
			this.m_buttonNo.UseVisualStyleBackColor = true;
			// 
			// autoHeightLabel2
			// 
			resources.ApplyResources(this.autoHeightLabel2, "autoHeightLabel2");
			this.tableLayoutPanel1.SetColumnSpan(this.autoHeightLabel2, 3);
			this.autoHeightLabel2.Name = "autoHeightLabel2";
			// 
			// m_listBoxVMs
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.m_listBoxVMs, 3);
			resources.ApplyResources(this.m_listBoxVMs, "m_listBoxVMs");
			this.m_listBoxVMs.FormattingEnabled = true;
			this.m_listBoxVMs.Name = "m_listBoxVMs";
			this.m_listBoxVMs.SelectionMode = System.Windows.Forms.SelectionMode.None;
			// 
			// FateSharingVMsDialog
			// 
			this.AcceptButton = this.m_buttonYes;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.m_buttonNo;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "FateSharingVMsDialog";
			this.Load += new System.EventHandler(this.FateSharingVMsDialog_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
		private System.Windows.Forms.Button m_buttonYes;
		private System.Windows.Forms.Button m_buttonNo;
		private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel2;
		private System.Windows.Forms.ListBox m_listBoxVMs;
	}
}

