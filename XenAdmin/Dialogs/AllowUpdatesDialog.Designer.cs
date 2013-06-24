namespace XenAdmin.Dialogs
{
    partial class AllowUpdatesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AllowUpdatesDialog));
            this.QuestionPictureBox = new System.Windows.Forms.PictureBox();
            this.BlurbLabel = new System.Windows.Forms.Label();
            this.NoButton = new System.Windows.Forms.Button();
            this.YesButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.QuestionPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // QuestionPictureBox
            // 
            resources.ApplyResources(this.QuestionPictureBox, "QuestionPictureBox");
            this.QuestionPictureBox.Name = "QuestionPictureBox";
            this.QuestionPictureBox.TabStop = false;
            // 
            // BlurbLabel
            // 
            resources.ApplyResources(this.BlurbLabel, "BlurbLabel");
            this.BlurbLabel.Name = "BlurbLabel";
            // 
            // NoButton
            // 
            resources.ApplyResources(this.NoButton, "NoButton");
            this.NoButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.NoButton.Name = "NoButton";
            this.NoButton.UseVisualStyleBackColor = true;
            // 
            // YesButton
            // 
            resources.ApplyResources(this.YesButton, "YesButton");
            this.YesButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.YesButton.Name = "YesButton";
            this.YesButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // AllowUpdatesDialog
            // 
            this.AcceptButton = this.YesButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.NoButton;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.YesButton);
            this.Controls.Add(this.NoButton);
            this.Controls.Add(this.BlurbLabel);
            this.Controls.Add(this.QuestionPictureBox);
            this.Name = "AllowUpdatesDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AllowUpdatesDialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.QuestionPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox QuestionPictureBox;
        private System.Windows.Forms.Label BlurbLabel;
        private System.Windows.Forms.Button NoButton;
        private System.Windows.Forms.Button YesButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}