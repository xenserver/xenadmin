namespace XenAdmin.Dialogs.RestoreSession
{
    partial class SetMainPasswordDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetMainPasswordDialog));
            this.noteLabel = new System.Windows.Forms.Label();
            this.reEnterMainLabel = new System.Windows.Forms.Label();
            this.mainLabel = new System.Windows.Forms.Label();
            this.reEnterMainTextBox = new System.Windows.Forms.TextBox();
            this.mainTextBox = new System.Windows.Forms.TextBox();
            this.mainBlurbLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.newPasswordError = new XenAdmin.Controls.Common.PasswordFailure();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // noteLabel
            // 
            resources.ApplyResources(this.noteLabel, "noteLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.noteLabel, 2);
            this.noteLabel.Name = "noteLabel";
            // 
            // reEnterMainLabel
            // 
            resources.ApplyResources(this.reEnterMainLabel, "reEnterMainLabel");
            this.reEnterMainLabel.Name = "reEnterMainLabel";
            // 
            // mainLabel
            // 
            resources.ApplyResources(this.mainLabel, "mainLabel");
            this.mainLabel.Name = "mainLabel";
            // 
            // reEnterMainTextBox
            // 
            resources.ApplyResources(this.reEnterMainTextBox, "reEnterMainTextBox");
            this.reEnterMainTextBox.Name = "reEnterMainTextBox";
            this.reEnterMainTextBox.UseSystemPasswordChar = true;
            this.reEnterMainTextBox.TextChanged += new System.EventHandler(this.reEnterMainTextBox_TextChanged);
            // 
            // mainTextBox
            // 
            resources.ApplyResources(this.mainTextBox, "mainTextBox");
            this.mainTextBox.Name = "mainTextBox";
            this.mainTextBox.UseSystemPasswordChar = true;
            this.mainTextBox.TextChanged += new System.EventHandler(this.mainTextBox_TextChanged);
            // 
            // mainBlurbLabel
            // 
            resources.ApplyResources(this.mainBlurbLabel, "mainBlurbLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.mainBlurbLabel, 2);
            this.mainBlurbLabel.Name = "mainBlurbLabel";
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
            // newPasswordError
            // 
            resources.ApplyResources(this.newPasswordError, "newPasswordError");
            this.newPasswordError.Name = "newPasswordError";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.mainLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.newPasswordError, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.mainTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.noteLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.reEnterMainTextBox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.reEnterMainLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.mainBlurbLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.cancelButton);
            this.flowLayoutPanel1.Controls.Add(this.okButton);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // SetMainPasswordDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SetMainPasswordDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label noteLabel;
        private System.Windows.Forms.Label reEnterMainLabel;
        private System.Windows.Forms.Label mainLabel;
        private System.Windows.Forms.TextBox reEnterMainTextBox;
        private System.Windows.Forms.TextBox mainTextBox;
        private System.Windows.Forms.Label mainBlurbLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private XenAdmin.Controls.Common.PasswordFailure newPasswordError;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}