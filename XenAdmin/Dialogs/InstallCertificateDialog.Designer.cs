namespace XenAdmin.Dialogs
{
    partial class InstallCertificateDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallCertificateDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelKeyBlurb = new System.Windows.Forms.Label();
            this.labelKey = new System.Windows.Forms.Label();
            this.textBoxKey = new System.Windows.Forms.TextBox();
            this.buttonBrowseKey = new System.Windows.Forms.Button();
            this.labelKeyError = new System.Windows.Forms.Label();
            this.labelCertificate = new System.Windows.Forms.Label();
            this.textBoxCertificate = new System.Windows.Forms.TextBox();
            this.buttonBrowseCertificate = new System.Windows.Forms.Button();
            this.labelChain = new System.Windows.Forms.Label();
            this.dataGridViewCertificates = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnCertificate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonAddCertificate = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.labelChainError = new System.Windows.Forms.Label();
            this.buttonInstall = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tlpActionProgress = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelActionProgress = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelCertificateError = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCertificates)).BeginInit();
            this.tlpActionProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelKeyBlurb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelKey, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxKey, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonBrowseKey, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelKeyError, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelCertificate, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxCertificate, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonBrowseCertificate, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelChain, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewCertificates, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.buttonAddCertificate, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.buttonRemove, 2, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelChainError, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.buttonInstall, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 2, 10);
            this.tableLayoutPanel1.Controls.Add(this.tlpActionProgress, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.labelCertificateError, 1, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelKeyBlurb
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.labelKeyBlurb, 3);
            resources.ApplyResources(this.labelKeyBlurb, "labelKeyBlurb");
            this.labelKeyBlurb.Name = "labelKeyBlurb";
            // 
            // labelKey
            // 
            resources.ApplyResources(this.labelKey, "labelKey");
            this.labelKey.Name = "labelKey";
            // 
            // textBoxKey
            // 
            resources.ApplyResources(this.textBoxKey, "textBoxKey");
            this.textBoxKey.Name = "textBoxKey";
            this.textBoxKey.TextChanged += new System.EventHandler(this.textBoxKey_TextChanged);
            // 
            // buttonBrowseKey
            // 
            resources.ApplyResources(this.buttonBrowseKey, "buttonBrowseKey");
            this.buttonBrowseKey.Name = "buttonBrowseKey";
            this.buttonBrowseKey.UseVisualStyleBackColor = true;
            this.buttonBrowseKey.Click += new System.EventHandler(this.buttonBrowseKey_Click);
            // 
            // labelKeyError
            // 
            this.labelKeyError.AutoEllipsis = true;
            resources.ApplyResources(this.labelKeyError, "labelKeyError");
            this.labelKeyError.ForeColor = System.Drawing.Color.Red;
            this.labelKeyError.Name = "labelKeyError";
            // 
            // labelCertificate
            // 
            resources.ApplyResources(this.labelCertificate, "labelCertificate");
            this.labelCertificate.Name = "labelCertificate";
            // 
            // textBoxCertificate
            // 
            resources.ApplyResources(this.textBoxCertificate, "textBoxCertificate");
            this.textBoxCertificate.Name = "textBoxCertificate";
            this.textBoxCertificate.TextChanged += new System.EventHandler(this.textBoxCertificate_TextChanged);
            // 
            // buttonBrowseCertificate
            // 
            resources.ApplyResources(this.buttonBrowseCertificate, "buttonBrowseCertificate");
            this.buttonBrowseCertificate.Name = "buttonBrowseCertificate";
            this.buttonBrowseCertificate.UseVisualStyleBackColor = true;
            this.buttonBrowseCertificate.Click += new System.EventHandler(this.buttonBrowseCertificate_Click);
            // 
            // labelChain
            // 
            resources.ApplyResources(this.labelChain, "labelChain");
            this.tableLayoutPanel1.SetColumnSpan(this.labelChain, 3);
            this.labelChain.Name = "labelChain";
            // 
            // dataGridViewCertificates
            // 
            this.dataGridViewCertificates.AllowUserToResizeColumns = false;
            this.dataGridViewCertificates.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewCertificates.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewCertificates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewCertificates.ColumnHeadersVisible = false;
            this.dataGridViewCertificates.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnCertificate});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridViewCertificates, 2);
            resources.ApplyResources(this.dataGridViewCertificates, "dataGridViewCertificates");
            this.dataGridViewCertificates.Name = "dataGridViewCertificates";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewCertificates.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.tableLayoutPanel1.SetRowSpan(this.dataGridViewCertificates, 3);
            this.dataGridViewCertificates.SelectionChanged += new System.EventHandler(this.dataGridViewCertificates_SelectionChanged);
            // 
            // columnCertificate
            // 
            resources.ApplyResources(this.columnCertificate, "columnCertificate");
            this.columnCertificate.Name = "columnCertificate";
            // 
            // buttonAddCertificate
            // 
            resources.ApplyResources(this.buttonAddCertificate, "buttonAddCertificate");
            this.buttonAddCertificate.Name = "buttonAddCertificate";
            this.buttonAddCertificate.UseVisualStyleBackColor = true;
            this.buttonAddCertificate.Click += new System.EventHandler(this.buttonAddCertificate_Click);
            // 
            // buttonRemove
            // 
            resources.ApplyResources(this.buttonRemove, "buttonRemove");
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // labelChainError
            // 
            this.labelChainError.AutoEllipsis = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelChainError, 2);
            resources.ApplyResources(this.labelChainError, "labelChainError");
            this.labelChainError.ForeColor = System.Drawing.Color.Red;
            this.labelChainError.Name = "labelChainError";
            // 
            // buttonInstall
            // 
            resources.ApplyResources(this.buttonInstall, "buttonInstall");
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.UseVisualStyleBackColor = true;
            this.buttonInstall.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tlpActionProgress
            // 
            resources.ApplyResources(this.tlpActionProgress, "tlpActionProgress");
            this.tableLayoutPanel1.SetColumnSpan(this.tlpActionProgress, 3);
            this.tlpActionProgress.Controls.Add(this.pictureBox1, 0, 0);
            this.tlpActionProgress.Controls.Add(this.labelActionProgress, 1, 0);
            this.tlpActionProgress.Name = "tlpActionProgress";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // labelActionProgress
            // 
            resources.ApplyResources(this.labelActionProgress, "labelActionProgress");
            this.labelActionProgress.Name = "labelActionProgress";
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.tableLayoutPanel1.SetColumnSpan(this.progressBar1, 3);
            this.progressBar1.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.progressBar1.Name = "progressBar1";
            // 
            // labelCertificateError
            // 
            resources.ApplyResources(this.labelCertificateError, "labelCertificateError");
            this.labelCertificateError.ForeColor = System.Drawing.Color.Red;
            this.labelCertificateError.Name = "labelCertificateError";
            // 
            // InstallCertificateDialog
            // 
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "InstallCertificateDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCertificates)).EndInit();
            this.tlpActionProgress.ResumeLayout(false);
            this.tlpActionProgress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelKeyBlurb;
        private System.Windows.Forms.Label labelKey;
        private System.Windows.Forms.TextBox textBoxKey;
        private System.Windows.Forms.Button buttonBrowseKey;
        private System.Windows.Forms.Label labelChain;
        private System.Windows.Forms.Label labelKeyError;
        private System.Windows.Forms.Button buttonAddCertificate;
        private System.Windows.Forms.Label labelChainError;
        private System.Windows.Forms.Button buttonInstall;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TableLayoutPanel tlpActionProgress;
        private System.Windows.Forms.Label labelActionProgress;
        private System.Windows.Forms.Button buttonRemove;
        private Controls.DataGridViewEx.DataGridViewEx dataGridViewCertificates;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnCertificate;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelCertificate;
        private System.Windows.Forms.TextBox textBoxCertificate;
        private System.Windows.Forms.Button buttonBrowseCertificate;
        private System.Windows.Forms.Label labelCertificateError;
    }
}