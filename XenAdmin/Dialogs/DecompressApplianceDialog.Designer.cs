namespace XenAdmin.Dialogs
{
	partial class DecompressApplianceDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DecompressApplianceDialog));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.m_tlpButtons = new System.Windows.Forms.TableLayoutPanel();
			this.m_buttonDecompress = new System.Windows.Forms.Button();
			this.m_buttonCancel = new System.Windows.Forms.Button();
			this.m_tlpNotice = new System.Windows.Forms.TableLayoutPanel();
			this.m_labelNotice = new XenAdmin.Controls.Common.AutoHeightLabel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.m_tlpProgress = new System.Windows.Forms.TableLayoutPanel();
			this.m_progressBar = new System.Windows.Forms.ProgressBar();
			this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
			this.m_tlpError = new System.Windows.Forms.TableLayoutPanel();
			this.m_pictureBoxError = new System.Windows.Forms.PictureBox();
			this.autoHeightLabel3 = new XenAdmin.Controls.Common.AutoHeightLabel();
			this.m_labelError = new XenAdmin.Controls.Common.AutoHeightLabel();
			this.tableLayoutPanel1.SuspendLayout();
			this.m_tlpButtons.SuspendLayout();
			this.m_tlpNotice.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.m_tlpProgress.SuspendLayout();
			this.m_tlpError.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxError)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.m_tlpButtons, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.m_tlpNotice, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.m_tlpProgress, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.m_tlpError, 0, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// m_tlpButtons
			// 
			resources.ApplyResources(this.m_tlpButtons, "m_tlpButtons");
			this.m_tlpButtons.Controls.Add(this.m_buttonDecompress, 1, 0);
			this.m_tlpButtons.Controls.Add(this.m_buttonCancel, 2, 0);
			this.m_tlpButtons.Name = "m_tlpButtons";
			// 
			// m_buttonDecompress
			// 
			resources.ApplyResources(this.m_buttonDecompress, "m_buttonDecompress");
			this.m_buttonDecompress.Name = "m_buttonDecompress";
			this.m_buttonDecompress.UseVisualStyleBackColor = true;
			this.m_buttonDecompress.Click += new System.EventHandler(this.m_buttonDecompress_Click);
			// 
			// m_buttonCancel
			// 
			resources.ApplyResources(this.m_buttonCancel, "m_buttonCancel");
			this.m_buttonCancel.Name = "m_buttonCancel";
			this.m_buttonCancel.UseVisualStyleBackColor = true;
			this.m_buttonCancel.Click += new System.EventHandler(this.m_buttonCancel_Click);
			// 
			// m_tlpNotice
			// 
			resources.ApplyResources(this.m_tlpNotice, "m_tlpNotice");
			this.m_tlpNotice.Controls.Add(this.m_labelNotice, 1, 0);
			this.m_tlpNotice.Controls.Add(this.pictureBox1, 0, 0);
			this.m_tlpNotice.Name = "m_tlpNotice";
			// 
			// m_labelNotice
			// 
			resources.ApplyResources(this.m_labelNotice, "m_labelNotice");
			this.m_labelNotice.Name = "m_labelNotice";
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// m_tlpProgress
			// 
			resources.ApplyResources(this.m_tlpProgress, "m_tlpProgress");
			this.m_tlpProgress.Controls.Add(this.m_progressBar, 0, 2);
			this.m_tlpProgress.Controls.Add(this.autoHeightLabel1, 0, 0);
			this.m_tlpProgress.Name = "m_tlpProgress";
			// 
			// m_progressBar
			// 
			resources.ApplyResources(this.m_progressBar, "m_progressBar");
			this.m_progressBar.MarqueeAnimationSpeed = 20;
			this.m_progressBar.Name = "m_progressBar";
			this.m_progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			// 
			// autoHeightLabel1
			// 
			resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
			this.autoHeightLabel1.Name = "autoHeightLabel1";
			// 
			// m_tlpError
			// 
			resources.ApplyResources(this.m_tlpError, "m_tlpError");
			this.m_tlpError.Controls.Add(this.m_pictureBoxError, 0, 0);
			this.m_tlpError.Controls.Add(this.autoHeightLabel3, 1, 0);
			this.m_tlpError.Controls.Add(this.m_labelError, 1, 2);
			this.m_tlpError.Name = "m_tlpError";
			// 
			// m_pictureBoxError
			// 
			resources.ApplyResources(this.m_pictureBoxError, "m_pictureBoxError");
			this.m_pictureBoxError.Name = "m_pictureBoxError";
			this.m_tlpError.SetRowSpan(this.m_pictureBoxError, 3);
			this.m_pictureBoxError.TabStop = false;
			// 
			// autoHeightLabel3
			// 
			resources.ApplyResources(this.autoHeightLabel3, "autoHeightLabel3");
			this.autoHeightLabel3.Name = "autoHeightLabel3";
			// 
			// m_labelError
			// 
			resources.ApplyResources(this.m_labelError, "m_labelError");
			this.m_labelError.Name = "m_labelError";
			// 
			// DecompressApplianceDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ControlBox = false;
			this.Controls.Add(this.tableLayoutPanel1);
			this.HelpButton = false;
			this.Name = "DecompressApplianceDialog";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.m_tlpButtons.ResumeLayout(false);
			this.m_tlpNotice.ResumeLayout(false);
			this.m_tlpNotice.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.m_tlpProgress.ResumeLayout(false);
			this.m_tlpProgress.PerformLayout();
			this.m_tlpError.ResumeLayout(false);
			this.m_tlpError.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxError)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private XenAdmin.Controls.Common.AutoHeightLabel m_labelNotice;
		private System.Windows.Forms.ProgressBar m_progressBar;
		private System.Windows.Forms.TableLayoutPanel m_tlpNotice;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TableLayoutPanel m_tlpProgress;
		private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
		private System.Windows.Forms.TableLayoutPanel m_tlpError;
		private System.Windows.Forms.PictureBox m_pictureBoxError;
		private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel3;
		private System.Windows.Forms.TableLayoutPanel m_tlpButtons;
		private System.Windows.Forms.Button m_buttonDecompress;
		private System.Windows.Forms.Button m_buttonCancel;
		private XenAdmin.Controls.Common.AutoHeightLabel m_labelError;
	}
}

