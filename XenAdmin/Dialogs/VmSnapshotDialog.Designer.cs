namespace XenAdmin.Dialogs
{
    partial class VmSnapshotDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VmSnapshotDialog));
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.memoryRadioButton = new System.Windows.Forms.RadioButton();
            this.CheckpointInfoPictureBox = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.quiesceCheckBox = new System.Windows.Forms.CheckBox();
            this.pictureBoxQuiesceInfo = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.diskRadioButton = new System.Windows.Forms.RadioButton();
            this.pictureBoxSnapshotsInfo = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CheckpointInfoPictureBox)).BeginInit();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQuiesceInfo)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSnapshotsInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // labelName
            // 
            resources.ApplyResources(this.labelName, "labelName");
            this.labelName.Name = "labelName";
            // 
            // textBoxName
            // 
            resources.ApplyResources(this.textBoxName, "textBoxName");
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxDescription, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // groupBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.flowLayoutPanel3);
            this.groupBox1.Controls.Add(this.flowLayoutPanel2);
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // flowLayoutPanel3
            // 
            resources.ApplyResources(this.flowLayoutPanel3, "flowLayoutPanel3");
            this.flowLayoutPanel3.Controls.Add(this.memoryRadioButton);
            this.flowLayoutPanel3.Controls.Add(this.CheckpointInfoPictureBox);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            // 
            // memoryRadioButton
            // 
            resources.ApplyResources(this.memoryRadioButton, "memoryRadioButton");
            this.memoryRadioButton.Name = "memoryRadioButton";
            this.memoryRadioButton.TabStop = true;
            this.memoryRadioButton.UseVisualStyleBackColor = true;
            this.memoryRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // CheckpointInfoPictureBox
            // 
            this.CheckpointInfoPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.CheckpointInfoPictureBox, "CheckpointInfoPictureBox");
            this.CheckpointInfoPictureBox.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.CheckpointInfoPictureBox.Name = "CheckpointInfoPictureBox";
            this.CheckpointInfoPictureBox.TabStop = false;
            this.CheckpointInfoPictureBox.MouseLeave += new System.EventHandler(this.CheckpointInfoPictureBox_MouseLeave);
            this.CheckpointInfoPictureBox.Click += new System.EventHandler(this.CheckpointInfoPictureBox_Click);
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Controls.Add(this.quiesceCheckBox);
            this.flowLayoutPanel2.Controls.Add(this.pictureBoxQuiesceInfo);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // quiesceCheckBox
            // 
            resources.ApplyResources(this.quiesceCheckBox, "quiesceCheckBox");
            this.quiesceCheckBox.Name = "quiesceCheckBox";
            this.quiesceCheckBox.UseVisualStyleBackColor = true;
            this.quiesceCheckBox.CheckedChanged += new System.EventHandler(this.quiesceCheckBox_CheckedChanged);
            // 
            // pictureBoxQuiesceInfo
            // 
            this.pictureBoxQuiesceInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.pictureBoxQuiesceInfo, "pictureBoxQuiesceInfo");
            this.pictureBoxQuiesceInfo.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBoxQuiesceInfo.Name = "pictureBoxQuiesceInfo";
            this.pictureBoxQuiesceInfo.TabStop = false;
            this.pictureBoxQuiesceInfo.MouseLeave += new System.EventHandler(this.pictureBoxQuiesceInfo_MouseLeave);
            this.pictureBoxQuiesceInfo.Click += new System.EventHandler(this.pictureBoxQuiesceInfo_Click);
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.diskRadioButton);
            this.flowLayoutPanel1.Controls.Add(this.pictureBoxSnapshotsInfo);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // diskRadioButton
            // 
            resources.ApplyResources(this.diskRadioButton, "diskRadioButton");
            this.diskRadioButton.Checked = true;
            this.diskRadioButton.Name = "diskRadioButton";
            this.diskRadioButton.TabStop = true;
            this.diskRadioButton.UseVisualStyleBackColor = true;
            this.diskRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // pictureBoxSnapshotsInfo
            // 
            this.pictureBoxSnapshotsInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.pictureBoxSnapshotsInfo, "pictureBoxSnapshotsInfo");
            this.pictureBoxSnapshotsInfo.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBoxSnapshotsInfo.Name = "pictureBoxSnapshotsInfo";
            this.pictureBoxSnapshotsInfo.TabStop = false;
            this.pictureBoxSnapshotsInfo.MouseLeave += new System.EventHandler(this.pictureBoxSnapshotsInfo_MouseLeave);
            this.pictureBoxSnapshotsInfo.Click += new System.EventHandler(this.pictureBoxSnapshotsInfo_Click);
            // 
            // VmSnapshotDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "VmSnapshotDialog";
            this.Load += new System.EventHandler(this.VmSnapshotDialog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CheckpointInfoPictureBox)).EndInit();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQuiesceInfo)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSnapshotsInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.RadioButton memoryRadioButton;
        private System.Windows.Forms.PictureBox CheckpointInfoPictureBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.CheckBox quiesceCheckBox;
        private System.Windows.Forms.PictureBox pictureBoxQuiesceInfo;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.RadioButton diskRadioButton;
        private System.Windows.Forms.PictureBox pictureBoxSnapshotsInfo;
    }
}