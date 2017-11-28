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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonGfs2 = new System.Windows.Forms.RadioButton();
            this.labelGFS2 = new System.Windows.Forms.Label();
            this.radioButtonLvm = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.labelWarning = new System.Windows.Forms.Label();
            this.pictureBoxInfo = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonGfs2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelGFS2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonLvm, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelWarning, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxInfo, 0, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(540, 336);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(340, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select the provisioning method for the new Storage Repository.";
            // 
            // radioButtonGfs2
            // 
            this.radioButtonGfs2.AutoSize = true;
            this.radioButtonGfs2.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonGfs2, 2);
            this.radioButtonGfs2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioButtonGfs2.Location = new System.Drawing.Point(3, 34);
            this.radioButtonGfs2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.radioButtonGfs2.Name = "radioButtonGfs2";
            this.radioButtonGfs2.Size = new System.Drawing.Size(155, 19);
            this.radioButtonGfs2.TabIndex = 1;
            this.radioButtonGfs2.TabStop = true;
            this.radioButtonGfs2.Text = "Thin provisioning (GFS2)";
            this.radioButtonGfs2.UseVisualStyleBackColor = true;
            // 
            // labelGFS2
            // 
            this.labelGFS2.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelGFS2, 2);
            this.labelGFS2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelGFS2.Location = new System.Drawing.Point(20, 59);
            this.labelGFS2.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.labelGFS2.Name = "labelGFS2";
            this.labelGFS2.Size = new System.Drawing.Size(511, 15);
            this.labelGFS2.TabIndex = 2;
            this.labelGFS2.Text = "The SR will be formatted with the GFS2 cluster file system for hosting thinly pro" +
    "visioned images.";
            // 
            // radioButtonLvm
            // 
            this.radioButtonLvm.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonLvm, 2);
            this.radioButtonLvm.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioButtonLvm.Location = new System.Drawing.Point(3, 83);
            this.radioButtonLvm.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.radioButtonLvm.Name = "radioButtonLvm";
            this.radioButtonLvm.Size = new System.Drawing.Size(147, 19);
            this.radioButtonLvm.TabIndex = 3;
            this.radioButtonLvm.Text = "Full provisioning (LVM)";
            this.radioButtonLvm.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label3, 2);
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.Location = new System.Drawing.Point(20, 108);
            this.label3.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(400, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "This SR will be configured to host fully provisioned virtual disks using LVM.";
            // 
            // labelWarning
            // 
            this.labelWarning.AutoSize = true;
            this.labelWarning.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelWarning.Location = new System.Drawing.Point(25, 141);
            this.labelWarning.Margin = new System.Windows.Forms.Padding(0, 15, 3, 0);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new System.Drawing.Size(0, 15);
            this.labelWarning.TabIndex = 6;
            // 
            // pictureBoxInfo
            // 
            this.pictureBoxInfo.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBoxInfo.Location = new System.Drawing.Point(3, 141);
            this.pictureBoxInfo.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.pictureBoxInfo.Name = "pictureBoxInfo";
            this.pictureBoxInfo.Size = new System.Drawing.Size(19, 20);
            this.pictureBoxInfo.TabIndex = 5;
            this.pictureBoxInfo.TabStop = false;
            // 
            // ChooseSrProvisioningPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ChooseSrProvisioningPage";
            this.Size = new System.Drawing.Size(540, 336);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).EndInit();
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
    }
}
