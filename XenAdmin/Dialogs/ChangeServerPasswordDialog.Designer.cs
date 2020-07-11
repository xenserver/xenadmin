namespace XenAdmin.Dialogs
{
    partial class ChangeServerPasswordDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeServerPasswordDialog));
            this.OldPasswordLabel = new System.Windows.Forms.Label();
            this.oldPassBox = new System.Windows.Forms.TextBox();
            this.NewPasswordLabel = new System.Windows.Forms.Label();
            this.newPassBox = new System.Windows.Forms.TextBox();
            this.confirmBox = new System.Windows.Forms.TextBox();
            this.RetypeNewPasswordLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.currentPasswordError = new XenAdmin.Controls.Common.PasswordFailure();
            this.newPasswordError = new XenAdmin.Controls.Common.PasswordFailure();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.noteLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // OldPasswordLabel
            // 
            resources.ApplyResources(this.OldPasswordLabel, "OldPasswordLabel");
            this.OldPasswordLabel.Name = "OldPasswordLabel";
            // 
            // oldPassBox
            // 
            resources.ApplyResources(this.oldPassBox, "oldPassBox");
            this.tableLayoutPanel1.SetColumnSpan(this.oldPassBox, 2);
            this.oldPassBox.Name = "oldPassBox";
            this.oldPassBox.UseSystemPasswordChar = true;
            this.oldPassBox.TextChanged += new System.EventHandler(this.oldPassBox_TextChanged);
            // 
            // NewPasswordLabel
            // 
            resources.ApplyResources(this.NewPasswordLabel, "NewPasswordLabel");
            this.NewPasswordLabel.Name = "NewPasswordLabel";
            // 
            // newPassBox
            // 
            resources.ApplyResources(this.newPassBox, "newPassBox");
            this.tableLayoutPanel1.SetColumnSpan(this.newPassBox, 2);
            this.newPassBox.Name = "newPassBox";
            this.newPassBox.UseSystemPasswordChar = true;
            this.newPassBox.TextChanged += new System.EventHandler(this.newPassBox_TextChanged);
            // 
            // confirmBox
            // 
            resources.ApplyResources(this.confirmBox, "confirmBox");
            this.tableLayoutPanel1.SetColumnSpan(this.confirmBox, 2);
            this.confirmBox.Name = "confirmBox";
            this.confirmBox.UseSystemPasswordChar = true;
            this.confirmBox.TextChanged += new System.EventHandler(this.confirmBox_TextChanged);
            // 
            // RetypeNewPasswordLabel
            // 
            resources.ApplyResources(this.RetypeNewPasswordLabel, "RetypeNewPasswordLabel");
            this.RetypeNewPasswordLabel.Name = "RetypeNewPasswordLabel";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.OldPasswordLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.oldPassBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.NewPasswordLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.newPassBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.RetypeNewPasswordLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.confirmBox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.okButton, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 2, 7);
            this.tableLayoutPanel1.Controls.Add(this.currentPasswordError, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.newPasswordError, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // currentPasswordError
            // 
            resources.ApplyResources(this.currentPasswordError, "currentPasswordError");
            this.tableLayoutPanel1.SetColumnSpan(this.currentPasswordError, 2);
            this.currentPasswordError.Name = "currentPasswordError";
            // 
            // newPasswordError
            // 
            resources.ApplyResources(this.newPasswordError, "newPasswordError");
            this.tableLayoutPanel1.SetColumnSpan(this.newPasswordError, 2);
            this.newPasswordError.Name = "newPasswordError";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 3);
            this.tableLayoutPanel2.Controls.Add(this.noteLabel, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // noteLabel
            // 
            resources.ApplyResources(this.noteLabel, "noteLabel");
            this.noteLabel.Name = "noteLabel";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel3, 3);
            this.tableLayoutPanel3.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.pictureBox2, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // ChangeServerPasswordDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ChangeServerPasswordDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label OldPasswordLabel;
        private System.Windows.Forms.TextBox oldPassBox;
        private System.Windows.Forms.Label NewPasswordLabel;
        private System.Windows.Forms.TextBox newPassBox;
        private System.Windows.Forms.TextBox confirmBox;
        private System.Windows.Forms.Label RetypeNewPasswordLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.Common.PasswordFailure currentPasswordError;
        private XenAdmin.Controls.Common.PasswordFailure newPasswordError;
        private System.Windows.Forms.Label noteLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}