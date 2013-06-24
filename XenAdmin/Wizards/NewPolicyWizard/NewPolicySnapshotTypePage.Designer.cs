namespace XenAdmin.Wizards.NewPolicyWizard
{
    partial class NewPolicySnapshotTypePage
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewPolicySnapshotTypePage));
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonDiskOnly = new System.Windows.Forms.RadioButton();
            this.radioButtonDiskAndMemory = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkpointInfoPictureBox = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxWarning = new System.Windows.Forms.PictureBox();
            this.labelWarning = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.checkpointInfoPictureBox)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarning)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // radioButtonDiskOnly
            // 
            resources.ApplyResources(this.radioButtonDiskOnly, "radioButtonDiskOnly");
            this.radioButtonDiskOnly.Checked = true;
            this.radioButtonDiskOnly.Name = "radioButtonDiskOnly";
            this.radioButtonDiskOnly.TabStop = true;
            this.radioButtonDiskOnly.UseVisualStyleBackColor = true;
            // 
            // radioButtonDiskAndMemory
            // 
            resources.ApplyResources(this.radioButtonDiskAndMemory, "radioButtonDiskAndMemory");
            this.radioButtonDiskAndMemory.Name = "radioButtonDiskAndMemory";
            this.radioButtonDiskAndMemory.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.tableLayoutPanel1.SetColumnSpan(this.label3, 2);
            this.label3.Name = "label3";
            // 
            // checkpointInfoPictureBox
            // 
            this.checkpointInfoPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.checkpointInfoPictureBox, "checkpointInfoPictureBox");
            this.checkpointInfoPictureBox.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.checkpointInfoPictureBox.Name = "checkpointInfoPictureBox";
            this.checkpointInfoPictureBox.TabStop = false;
            this.checkpointInfoPictureBox.MouseLeave += new System.EventHandler(this.checkpointInfoPictureBox_MouseLeave);
            this.checkpointInfoPictureBox.Click += new System.EventHandler(this.checkpointInfoPictureBox_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxWarning, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelWarning, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // pictureBoxWarning
            // 
            this.pictureBoxWarning.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.pictureBoxWarning, "pictureBoxWarning");
            this.pictureBoxWarning.Name = "pictureBoxWarning";
            this.pictureBoxWarning.TabStop = false;
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.labelWarning.Name = "labelWarning";
            // 
            // NewPolicySnapshotTypePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.checkpointInfoPictureBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.radioButtonDiskAndMemory);
            this.Controls.Add(this.radioButtonDiskOnly);
            this.Controls.Add(this.label1);
            this.Name = "NewPolicySnapshotTypePage";
            ((System.ComponentModel.ISupportInitialize)(this.checkpointInfoPictureBox)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarning)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonDiskOnly;
        private System.Windows.Forms.RadioButton radioButtonDiskAndMemory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox checkpointInfoPictureBox;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBoxWarning;
        private System.Windows.Forms.Label labelWarning;
    }
}
