namespace XenAdmin.Dialogs.RestoreSession
{
    partial class EnterMasterPasswordDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnterMasterPasswordDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.masterTextBox = new System.Windows.Forms.TextBox();
            this.masterBlurbLabel = new System.Windows.Forms.Label();
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
            // masterTextBox
            // 
            resources.ApplyResources(this.masterTextBox, "masterTextBox");
            this.tableLayoutPanel1.SetColumnSpan(this.masterTextBox, 3);
            this.masterTextBox.Name = "masterTextBox";
            this.masterTextBox.UseSystemPasswordChar = true;
            this.masterTextBox.TextChanged += new System.EventHandler(this.masterTextBox_TextChanged);
            // 
            // masterBlurbLabel
            // 
            resources.ApplyResources(this.masterBlurbLabel, "masterBlurbLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.masterBlurbLabel, 3);
            this.masterBlurbLabel.Name = "masterBlurbLabel";
            this.masterBlurbLabel.UseMnemonic = false;
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
            this.tableLayoutPanel1.Controls.Add(this.masterBlurbLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.masterTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.passwordError, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.okButton, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.TabStop = true;
            // 
            // EnterMasterPasswordDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "EnterMasterPasswordDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox masterTextBox;
        private System.Windows.Forms.Label masterBlurbLabel;
        private System.Windows.Forms.Label label1;
        private XenAdmin.Controls.Common.PasswordFailure passwordError;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}