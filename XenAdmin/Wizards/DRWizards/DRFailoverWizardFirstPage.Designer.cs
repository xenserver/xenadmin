namespace XenAdmin.Wizards.DRWizards
{
    partial class DRFailoverWizardFirstPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DRFailoverWizardFirstPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelInformation1 = new System.Windows.Forms.Label();
            this.labelInformation2 = new System.Windows.Forms.Label();
            this.labelInformation3 = new System.Windows.Forms.Label();
            this.panelWarning = new System.Windows.Forms.TableLayoutPanel();
            this.labelInformation4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelInformation1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelInformation2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelInformation3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panelWarning, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelInformation1
            // 
            resources.ApplyResources(this.labelInformation1, "labelInformation1");
            this.labelInformation1.Name = "labelInformation1";
            // 
            // labelInformation2
            // 
            resources.ApplyResources(this.labelInformation2, "labelInformation2");
            this.labelInformation2.Name = "labelInformation2";
            // 
            // labelInformation3
            // 
            resources.ApplyResources(this.labelInformation3, "labelInformation3");
            this.labelInformation3.Name = "labelInformation3";
            // 
            // panelWarning
            // 
            resources.ApplyResources(this.panelWarning, "panelWarning");
            this.panelWarning.Controls.Add(this.labelInformation4, 1, 1);
            this.panelWarning.Controls.Add(this.pictureBox1, 0, 0);
            this.panelWarning.Controls.Add(this.label6, 1, 0);
            this.panelWarning.Name = "panelWarning";
            // 
            // labelInformation4
            // 
            resources.ApplyResources(this.labelInformation4, "labelInformation4");
            this.labelInformation4.Name = "labelInformation4";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.panelWarning.SetRowSpan(this.pictureBox1, 2);
            this.pictureBox1.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // DRFailoverWizardFirstPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DRFailoverWizardFirstPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panelWarning.ResumeLayout(false);
            this.panelWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelInformation1;
        private System.Windows.Forms.Label labelInformation2;
        private System.Windows.Forms.Label labelInformation3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelInformation4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel panelWarning;
    }
}
