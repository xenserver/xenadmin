using XenAPI;

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
            this.quiesceCheckBox = new System.Windows.Forms.CheckBox();
            this.radioButtonDiskAndMemory = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.checkpointInfoPictureBox = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelWarning = new System.Windows.Forms.Label();
            this.pictureBoxWarning = new System.Windows.Forms.PictureBox();
            this.pictureBoxVSS = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.checkpointInfoPictureBox)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVSS)).BeginInit();
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
            // quiesceCheckBox
            // 
            resources.ApplyResources(this.quiesceCheckBox, "quiesceCheckBox");
            this.quiesceCheckBox.Name = "quiesceCheckBox";
            this.quiesceCheckBox.UseVisualStyleBackColor = true;
            this.quiesceCheckBox.CheckedChanged += new System.EventHandler(this.quiesceCheckBox_CheckedChanged);
            // 
            // radioButtonDiskAndMemory
            // 
            resources.ApplyResources(this.radioButtonDiskAndMemory, "radioButtonDiskAndMemory");
            this.radioButtonDiskAndMemory.Name = "radioButtonDiskAndMemory";
            this.radioButtonDiskAndMemory.UseVisualStyleBackColor = true;
            this.radioButtonDiskAndMemory.CheckedChanged += new System.EventHandler(this.radioButtonDiskAndMemory_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // checkpointInfoPictureBox
            // 
            this.checkpointInfoPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.checkpointInfoPictureBox, "checkpointInfoPictureBox");
            this.checkpointInfoPictureBox.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.checkpointInfoPictureBox.Name = "checkpointInfoPictureBox";
            this.checkpointInfoPictureBox.TabStop = false;
            this.checkpointInfoPictureBox.Click += new System.EventHandler(this.checkpointInfoPictureBox_Click);
            this.checkpointInfoPictureBox.MouseLeave += new System.EventHandler(this.checkpointInfoPictureBox_MouseLeave);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelWarning, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxWarning, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.labelWarning.Name = "labelWarning";
            // 
            // pictureBoxWarning
            // 
            this.pictureBoxWarning.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.pictureBoxWarning, "pictureBoxWarning");
            this.pictureBoxWarning.Name = "pictureBoxWarning";
            this.pictureBoxWarning.TabStop = false;
            // 
            // pictureBoxVSS
            // 
            this.pictureBoxVSS.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.pictureBoxVSS, "pictureBoxVSS");
            this.pictureBoxVSS.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBoxVSS.Name = "pictureBoxVSS";
            this.pictureBoxVSS.TabStop = false;
            this.pictureBoxVSS.Click += new System.EventHandler(this.pictureBoxVSS_Click);
            this.pictureBoxVSS.MouseLeave += new System.EventHandler(this.pictureBoxVSS_MouseLeave);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // NewPolicySnapshotTypePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBoxVSS);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.checkpointInfoPictureBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.radioButtonDiskAndMemory);
            this.Controls.Add(this.radioButtonDiskOnly);
            this.Controls.Add(this.quiesceCheckBox);
            this.Controls.Add(this.label1);
            this.Name = "NewPolicySnapshotTypePage";
            ((System.ComponentModel.ISupportInitialize)(this.checkpointInfoPictureBox)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVSS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Label label1;
        protected System.Windows.Forms.RadioButton radioButtonDiskOnly;
        protected System.Windows.Forms.RadioButton radioButtonDiskAndMemory;
        protected System.Windows.Forms.CheckBox quiesceCheckBox;
        protected System.Windows.Forms.Label label2;
        protected System.Windows.Forms.PictureBox checkpointInfoPictureBox;
        protected System.Windows.Forms.ToolTip toolTip;
        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        protected System.Windows.Forms.PictureBox pictureBoxWarning;
        protected System.Windows.Forms.Label labelWarning;
        protected System.Windows.Forms.PictureBox pictureBoxVSS;
        protected System.Windows.Forms.Label label3;
    }
}
