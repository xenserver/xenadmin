namespace XenAdmin.Wizards.DRWizards
{
    partial class DRFailoverWizardWelcomePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DRFailoverWizardWelcomePage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.labelDryrunDescription = new System.Windows.Forms.Label();
            this.labelFailobackDescription = new System.Windows.Forms.Label();
            this.radioButtonFailback = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonFailover = new System.Windows.Forms.RadioButton();
            this.labelFailoverDescription = new System.Windows.Forms.Label();
            this.radioButtonDryrun = new System.Windows.Forms.RadioButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.pictureBox3, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelDryrunDescription, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelFailobackDescription, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonFailback, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonFailover, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelFailoverDescription, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonDryrun, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::XenAdmin.Properties.Resources._000_TestFailover_h32bit_32;
            resources.ApplyResources(this.pictureBox3, "pictureBox3");
            this.pictureBox3.Name = "pictureBox3";
            this.tableLayoutPanel1.SetRowSpan(this.pictureBox3, 2);
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_Failback_h32bit_32;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.tableLayoutPanel1.SetRowSpan(this.pictureBox2, 2);
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // labelDryrunDescription
            // 
            resources.ApplyResources(this.labelDryrunDescription, "labelDryrunDescription");
            this.labelDryrunDescription.Name = "labelDryrunDescription";
            // 
            // labelFailobackDescription
            // 
            resources.ApplyResources(this.labelFailobackDescription, "labelFailobackDescription");
            this.labelFailobackDescription.Name = "labelFailobackDescription";
            // 
            // radioButtonFailback
            // 
            resources.ApplyResources(this.radioButtonFailback, "radioButtonFailback");
            this.radioButtonFailback.Name = "radioButtonFailback";
            this.radioButtonFailback.TabStop = true;
            this.radioButtonFailback.UseVisualStyleBackColor = true;
            this.radioButtonFailback.CheckedChanged += new System.EventHandler(this.radioButtonFailback_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // radioButtonFailover
            // 
            resources.ApplyResources(this.radioButtonFailover, "radioButtonFailover");
            this.radioButtonFailover.Name = "radioButtonFailover";
            this.radioButtonFailover.TabStop = true;
            this.radioButtonFailover.UseVisualStyleBackColor = true;
            this.radioButtonFailover.CheckedChanged += new System.EventHandler(this.radioButtonFailover_CheckedChanged);
            // 
            // labelFailoverDescription
            // 
            resources.ApplyResources(this.labelFailoverDescription, "labelFailoverDescription");
            this.labelFailoverDescription.Name = "labelFailoverDescription";
            // 
            // radioButtonDryrun
            // 
            resources.ApplyResources(this.radioButtonDryrun, "radioButtonDryrun");
            this.radioButtonDryrun.Name = "radioButtonDryrun";
            this.radioButtonDryrun.TabStop = true;
            this.radioButtonDryrun.UseVisualStyleBackColor = true;
            this.radioButtonDryrun.CheckedChanged += new System.EventHandler(this.radioButtonDryrun_CheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Failover_h32bit_32;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.tableLayoutPanel1.SetRowSpan(this.pictureBox1, 2);
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // DRFailoverWizardWelcomePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DRFailoverWizardWelcomePage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonFailover;
        private System.Windows.Forms.Label labelFailoverDescription;
        private System.Windows.Forms.Label labelDryrunDescription;
        private System.Windows.Forms.Label labelFailobackDescription;
        private System.Windows.Forms.RadioButton radioButtonFailback;
        private System.Windows.Forms.RadioButton radioButtonDryrun;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}
