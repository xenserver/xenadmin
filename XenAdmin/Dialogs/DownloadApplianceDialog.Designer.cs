namespace XenAdmin.Dialogs
{
	partial class DownloadApplianceDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadApplianceDialog));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.m_buttonCancel = new System.Windows.Forms.Button();
            this.m_buttonDownload = new System.Windows.Forms.Button();
            this.m_tlpError = new System.Windows.Forms.TableLayoutPanel();
            this.m_pictureBoxError = new System.Windows.Forms.PictureBox();
            this.autoHeightLabel3 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_labelError = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_tlpProgress = new System.Windows.Forms.TableLayoutPanel();
            this.m_progressBar = new System.Windows.Forms.ProgressBar();
            this.autoHeightLabel2 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.m_textBoxWorkspace = new System.Windows.Forms.TextBox();
            this.m_buttonBrowse = new System.Windows.Forms.Button();
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.m_tlpError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxError)).BeginInit();
            this.m_tlpProgress.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.autoHeightLabel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.m_tlpError, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.m_tlpProgress, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.m_buttonCancel, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.m_buttonDownload, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // m_buttonCancel
            // 
            resources.ApplyResources(this.m_buttonCancel, "m_buttonCancel");
            this.m_buttonCancel.Name = "m_buttonCancel";
            this.m_buttonCancel.UseVisualStyleBackColor = true;
            this.m_buttonCancel.Click += new System.EventHandler(this.m_buttonCancel_Click);
            // 
            // m_buttonDownload
            // 
            resources.ApplyResources(this.m_buttonDownload, "m_buttonDownload");
            this.m_buttonDownload.Name = "m_buttonDownload";
            this.m_buttonDownload.UseVisualStyleBackColor = true;
            this.m_buttonDownload.Click += new System.EventHandler(this.m_buttonDownload_Click);
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
            // m_tlpProgress
            // 
            resources.ApplyResources(this.m_tlpProgress, "m_tlpProgress");
            this.m_tlpProgress.Controls.Add(this.m_progressBar, 0, 2);
            this.m_tlpProgress.Controls.Add(this.autoHeightLabel2, 0, 0);
            this.m_tlpProgress.Name = "m_tlpProgress";
            // 
            // m_progressBar
            // 
            resources.ApplyResources(this.m_progressBar, "m_progressBar");
            this.m_progressBar.MarqueeAnimationSpeed = 20;
            this.m_progressBar.Name = "m_progressBar";
            this.m_progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // autoHeightLabel2
            // 
            resources.ApplyResources(this.autoHeightLabel2, "autoHeightLabel2");
            this.autoHeightLabel2.Name = "autoHeightLabel2";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.m_textBoxWorkspace, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.m_buttonBrowse, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.m_ctrlError, 1, 1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // m_textBoxWorkspace
            // 
            resources.ApplyResources(this.m_textBoxWorkspace, "m_textBoxWorkspace");
            this.m_textBoxWorkspace.Name = "m_textBoxWorkspace";
            this.m_textBoxWorkspace.TextChanged += new System.EventHandler(this.m_textBoxWorkspace_TextChanged);
            // 
            // m_buttonBrowse
            // 
            resources.ApplyResources(this.m_buttonBrowse, "m_buttonBrowse");
            this.m_buttonBrowse.Name = "m_buttonBrowse";
            this.m_buttonBrowse.UseVisualStyleBackColor = true;
            this.m_buttonBrowse.Click += new System.EventHandler(this.m_buttonBrowse_Click);
            // 
            // m_ctrlError
            // 
            resources.ApplyResources(this.m_ctrlError, "m_ctrlError");
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // DownloadApplianceDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.HelpButton = false;
            this.Name = "DownloadApplianceDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.m_tlpError.ResumeLayout(false);
            this.m_tlpError.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxError)).EndInit();
            this.m_tlpProgress.ResumeLayout(false);
            this.m_tlpProgress.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox m_textBoxWorkspace;
		private System.Windows.Forms.Button m_buttonCancel;
		private System.Windows.Forms.Button m_buttonDownload;
		private System.Windows.Forms.Button m_buttonBrowse;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlError;
		private System.Windows.Forms.TableLayoutPanel m_tlpProgress;
		private System.Windows.Forms.ProgressBar m_progressBar;
		private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel2;
		private System.Windows.Forms.TableLayoutPanel m_tlpError;
		private System.Windows.Forms.PictureBox m_pictureBoxError;
		private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel3;
		private XenAdmin.Controls.Common.AutoHeightLabel m_labelError;
	}
}

