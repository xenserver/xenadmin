namespace XenAdmin.Wizards.PatchingWizard
{
    partial class PatchingWizard_UploadPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatchingWizard_UploadPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.labelProgress = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.flickerFreeListBox1 = new XenAdmin.Controls.FlickerFreeListBox();
            this.diskSpaceErrorLinkLabel = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelProgress, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.flickerFreeListBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.diskSpaceErrorLinkLabel, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // labelProgress
            // 
            resources.ApplyResources(this.labelProgress, "labelProgress");
            this.labelProgress.Name = "labelProgress";
            // 
            // progressBar1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.progressBar1, 2);
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // flickerFreeListBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.flickerFreeListBox1, 2);
            resources.ApplyResources(this.flickerFreeListBox1, "flickerFreeListBox1");
            this.flickerFreeListBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.flickerFreeListBox1.FormattingEnabled = true;
            this.flickerFreeListBox1.Name = "flickerFreeListBox1";
            this.flickerFreeListBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.flickerFreeListBox1_DrawItem);
            // 
            // diskSpaceErrorLinkLabel
            // 
            resources.ApplyResources(this.diskSpaceErrorLinkLabel, "diskSpaceErrorLinkLabel");
            this.diskSpaceErrorLinkLabel.Name = "diskSpaceErrorLinkLabel";
            this.diskSpaceErrorLinkLabel.TabStop = true;
            this.diskSpaceErrorLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.diskspaceErrorLinkLabel_LinkClicked);
            // 
            // PatchingWizard_UploadPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PatchingWizard_UploadPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.ProgressBar progressBar1;
        private Controls.FlickerFreeListBox flickerFreeListBox1;
        private System.Windows.Forms.LinkLabel diskSpaceErrorLinkLabel;
    }
}
