namespace XenAdmin.Dialogs
{
    partial class RevertDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RevertDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.questionLabel = new System.Windows.Forms.Label();
            this.takeSnapshotBeforeCheckBox = new System.Windows.Forms.CheckBox();
            this.yesButton = new System.Windows.Forms.Button();
            this.noButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // questionLabel
            // 
            this.questionLabel.AutoEllipsis = true;
            resources.ApplyResources(this.questionLabel, "questionLabel");
            this.questionLabel.Name = "questionLabel";
            // 
            // takeSnapshotBeforeCheckBox
            // 
            resources.ApplyResources(this.takeSnapshotBeforeCheckBox, "takeSnapshotBeforeCheckBox");
            this.takeSnapshotBeforeCheckBox.Checked = true;
            this.takeSnapshotBeforeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.takeSnapshotBeforeCheckBox.Name = "takeSnapshotBeforeCheckBox";
            this.takeSnapshotBeforeCheckBox.UseVisualStyleBackColor = true;
            // 
            // yesButton
            // 
            this.yesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            resources.ApplyResources(this.yesButton, "yesButton");
            this.yesButton.Name = "yesButton";
            this.yesButton.UseVisualStyleBackColor = true;
            // 
            // noButton
            // 
            this.noButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.noButton, "noButton");
            this.noButton.Name = "noButton";
            this.noButton.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // RevertDialog
            // 
            this.AcceptButton = this.yesButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.noButton;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.noButton);
            this.Controls.Add(this.yesButton);
            this.Controls.Add(this.takeSnapshotBeforeCheckBox);
            this.Controls.Add(this.questionLabel);
            this.Controls.Add(this.label1);
            this.Name = "RevertDialog";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label questionLabel;
        private System.Windows.Forms.CheckBox takeSnapshotBeforeCheckBox;
        private System.Windows.Forms.Button yesButton;
        private System.Windows.Forms.Button noButton;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}