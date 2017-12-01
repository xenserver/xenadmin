namespace XenAdmin.Wizards.NewSRWizard_Pages
{
    partial class ChooseSrProvisioningPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseSrProvisioningPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonGfs2 = new System.Windows.Forms.RadioButton();
            this.labelGFS2 = new System.Windows.Forms.Label();
            this.radioButtonLvm = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.labelWarning = new System.Windows.Forms.Label();
            this.pictureBoxInfo = new System.Windows.Forms.PictureBox();
            this.flowLayoutInfo = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).BeginInit();
            this.flowLayoutInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonGfs2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelGFS2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonLvm, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutInfo, 0, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // radioButtonGfs2
            // 
            resources.ApplyResources(this.radioButtonGfs2, "radioButtonGfs2");
            this.radioButtonGfs2.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonGfs2, 2);
            this.radioButtonGfs2.Name = "radioButtonGfs2";
            this.radioButtonGfs2.TabStop = true;
            this.radioButtonGfs2.UseVisualStyleBackColor = true;
            // 
            // labelGFS2
            // 
            resources.ApplyResources(this.labelGFS2, "labelGFS2");
            this.tableLayoutPanel1.SetColumnSpan(this.labelGFS2, 2);
            this.labelGFS2.Name = "labelGFS2";
            // 
            // radioButtonLvm
            // 
            resources.ApplyResources(this.radioButtonLvm, "radioButtonLvm");
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonLvm, 2);
            this.radioButtonLvm.Name = "radioButtonLvm";
            this.radioButtonLvm.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.tableLayoutPanel1.SetColumnSpan(this.label3, 2);
            this.label3.Name = "label3";
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.labelWarning.Name = "labelWarning";
            // 
            // pictureBoxInfo
            // 
            this.pictureBoxInfo.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.pictureBoxInfo, "pictureBoxInfo");
            this.pictureBoxInfo.Name = "pictureBoxInfo";
            this.pictureBoxInfo.TabStop = false;
            // 
            // flowLayoutInfo
            // 
            resources.ApplyResources(this.flowLayoutInfo, "flowLayoutInfo");
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutInfo, 2);
            this.flowLayoutInfo.Controls.Add(this.pictureBoxInfo);
            this.flowLayoutInfo.Controls.Add(this.labelWarning);
            this.flowLayoutInfo.Name = "flowLayoutInfo";
            // 
            // ChooseSrProvisioningPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ChooseSrProvisioningPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).EndInit();
            this.flowLayoutInfo.ResumeLayout(false);
            this.flowLayoutInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonGfs2;
        private System.Windows.Forms.Label labelGFS2;
        private System.Windows.Forms.RadioButton radioButtonLvm;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBoxInfo;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutInfo;
    }
}
