namespace XenAdmin.Wizards.HAWizard_Pages
{
    partial class HAFinishPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HAFinishPage));
            this.label1 = new System.Windows.Forms.Label();
            this.StartVMGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.labelSummary = new System.Windows.Forms.Label();
            this.labelNoVmsProtected = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelNoHaGuaranteed = new System.Windows.Forms.Label();
            this.StartVMGroupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // StartVMGroupBox
            // 
            resources.ApplyResources(this.StartVMGroupBox, "StartVMGroupBox");
            this.tableLayoutPanel1.SetColumnSpan(this.StartVMGroupBox, 2);
            this.StartVMGroupBox.Controls.Add(this.labelSummary);
            this.StartVMGroupBox.Name = "StartVMGroupBox";
            this.StartVMGroupBox.TabStop = false;
            // 
            // labelSummary
            // 
            resources.ApplyResources(this.labelSummary, "labelSummary");
            this.labelSummary.MinimumSize = new System.Drawing.Size(400, 75);
            this.labelSummary.Name = "labelSummary";
            // 
            // labelNoVmsProtected
            // 
            resources.ApplyResources(this.labelNoVmsProtected, "labelNoVmsProtected");
            this.labelNoVmsProtected.ForeColor = System.Drawing.Color.Red;
            this.labelNoVmsProtected.Name = "labelNoVmsProtected";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.StartVMGroupBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelNoVmsProtected, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelNoHaGuaranteed, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.tableLayoutPanel1.SetRowSpan(this.pictureBox1, 2);
            this.pictureBox1.TabStop = false;
            // 
            // labelNoHaGuaranteed
            // 
            resources.ApplyResources(this.labelNoHaGuaranteed, "labelNoHaGuaranteed");
            this.labelNoHaGuaranteed.ForeColor = System.Drawing.Color.Red;
            this.labelNoHaGuaranteed.Name = "labelNoHaGuaranteed";
            // 
            // HAFinishPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "HAFinishPage";
            this.StartVMGroupBox.ResumeLayout(false);
            this.StartVMGroupBox.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private XenAdmin.Controls.DecentGroupBox StartVMGroupBox;
        private System.Windows.Forms.Label labelSummary;
        private System.Windows.Forms.Label labelNoVmsProtected;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelNoHaGuaranteed;
    }
}
