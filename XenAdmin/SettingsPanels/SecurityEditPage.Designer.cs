namespace XenAdmin.SettingsPanels
{
    partial class SecurityEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SecurityEditPage));
            this.labelRubricPool = new System.Windows.Forms.Label();
            this.radioButtonTLS = new System.Windows.Forms.RadioButton();
            this.radioButtonSSL = new System.Windows.Forms.RadioButton();
            this.pictureBoxDisruption = new System.Windows.Forms.PictureBox();
            this.labelDisruption = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDisruption)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelRubricPool
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.labelRubricPool, 2);
            resources.ApplyResources(this.labelRubricPool, "labelRubricPool");
            this.labelRubricPool.Name = "labelRubricPool";
            // 
            // radioButtonTLS
            // 
            resources.ApplyResources(this.radioButtonTLS, "radioButtonTLS");
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonTLS, 2);
            this.radioButtonTLS.Name = "radioButtonTLS";
            this.radioButtonTLS.TabStop = true;
            this.radioButtonTLS.UseVisualStyleBackColor = true;
            this.radioButtonTLS.CheckedChanged += new System.EventHandler(this.radioButtonTLS_CheckedChanged);
            // 
            // radioButtonSSL
            // 
            resources.ApplyResources(this.radioButtonSSL, "radioButtonSSL");
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonSSL, 2);
            this.radioButtonSSL.Name = "radioButtonSSL";
            this.radioButtonSSL.TabStop = true;
            this.radioButtonSSL.UseVisualStyleBackColor = true;
            // 
            // pictureBoxDisruption
            // 
            this.pictureBoxDisruption.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.pictureBoxDisruption, "pictureBoxDisruption");
            this.pictureBoxDisruption.Name = "pictureBoxDisruption";
            this.pictureBoxDisruption.TabStop = false;
            // 
            // labelDisruption
            // 
            resources.ApplyResources(this.labelDisruption, "labelDisruption");
            this.labelDisruption.Name = "labelDisruption";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelRubricPool, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelDisruption, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonTLS, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxDisruption, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonSSL, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // SecurityEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SecurityEditPage";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDisruption)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelRubricPool;
        private System.Windows.Forms.RadioButton radioButtonTLS;
        private System.Windows.Forms.RadioButton radioButtonSSL;
        private System.Windows.Forms.PictureBox pictureBoxDisruption;
        private System.Windows.Forms.Label labelDisruption;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
