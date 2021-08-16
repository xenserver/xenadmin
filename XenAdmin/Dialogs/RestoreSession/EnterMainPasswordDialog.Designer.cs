namespace XenAdmin.Dialogs.RestoreSession
{
    partial class EnterMainPasswordDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnterMainPasswordDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.mainTextBox = new System.Windows.Forms.TextBox();
            this.mainBlurbLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.passwordError = new XenAdmin.Controls.Common.PasswordFailure();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // mainTextBox
            // 
            resources.ApplyResources(this.mainTextBox, "mainTextBox");
            this.tableLayoutPanel1.SetColumnSpan(this.mainTextBox, 3);
            this.mainTextBox.Name = "mainTextBox";
            this.mainTextBox.UseSystemPasswordChar = true;
            this.mainTextBox.TextChanged += new System.EventHandler(this.mainTextBox_TextChanged);
            // 
            // mainBlurbLabel
            // 
            resources.ApplyResources(this.mainBlurbLabel, "mainBlurbLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.mainBlurbLabel, 3);
            this.mainBlurbLabel.Name = "mainBlurbLabel";
            this.mainBlurbLabel.UseMnemonic = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // passwordError
            // 
            resources.ApplyResources(this.passwordError, "passwordError");
            this.tableLayoutPanel1.SetColumnSpan(this.passwordError, 3);
            this.passwordError.Name = "passwordError";
            this.passwordError.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.mainBlurbLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.mainTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.passwordError, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.okButton, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.TabStop = true;
            // 
            // EnterMainPasswordDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "EnterMainPasswordDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox mainTextBox;
        private System.Windows.Forms.Label mainBlurbLabel;
        private System.Windows.Forms.Label label1;
        private XenAdmin.Controls.Common.PasswordFailure passwordError;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}