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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelSr = new System.Windows.Forms.Label();
            this.labelNtol = new System.Windows.Forms.Label();
            this.labelRestart = new System.Windows.Forms.Label();
            this.labelBestEffort = new System.Windows.Forms.Label();
            this.labelDoNotRestart = new System.Windows.Forms.Label();
            this.labelNoVmsProtected = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelNoHaGuaranteed = new System.Windows.Forms.Label();
            this.StartVMGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
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
            this.StartVMGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.StartVMGroupBox.Name = "StartVMGroupBox";
            this.StartVMGroupBox.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.labelSr, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelNtol, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelRestart, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelBestEffort, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.labelDoNotRestart, 1, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // labelSr
            // 
            resources.ApplyResources(this.labelSr, "labelSr");
            this.labelSr.Name = "labelSr";
            // 
            // labelNtol
            // 
            resources.ApplyResources(this.labelNtol, "labelNtol");
            this.labelNtol.Name = "labelNtol";
            // 
            // labelRestart
            // 
            resources.ApplyResources(this.labelRestart, "labelRestart");
            this.labelRestart.Name = "labelRestart";
            // 
            // labelBestEffort
            // 
            resources.ApplyResources(this.labelBestEffort, "labelBestEffort");
            this.labelBestEffort.Name = "labelBestEffort";
            // 
            // labelDoNotRestart
            // 
            resources.ApplyResources(this.labelDoNotRestart, "labelDoNotRestart");
            this.labelDoNotRestart.Name = "labelDoNotRestart";
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
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private XenAdmin.Controls.DecentGroupBox StartVMGroupBox;
        private System.Windows.Forms.Label labelNoVmsProtected;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelNoHaGuaranteed;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelSr;
        private System.Windows.Forms.Label labelNtol;
        private System.Windows.Forms.Label labelRestart;
        private System.Windows.Forms.Label labelBestEffort;
        private System.Windows.Forms.Label labelDoNotRestart;
    }
}
