namespace XenAdmin.Dialogs
{
    partial class LoadSessionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadSessionDialog));
            this.EnterPassLabel = new System.Windows.Forms.Label();
            this.passLabel = new System.Windows.Forms.Label();
            this.passBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.passwordFailure1 = new XenAdmin.Controls.Common.PasswordFailure();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // EnterPassLabel
            // 
            resources.ApplyResources(this.EnterPassLabel, "EnterPassLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.EnterPassLabel, 4);
            this.EnterPassLabel.Name = "EnterPassLabel";
            this.EnterPassLabel.UseMnemonic = false;
            // 
            // passLabel
            // 
            resources.ApplyResources(this.passLabel, "passLabel");
            this.passLabel.Name = "passLabel";
            // 
            // passBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.passBox, 3);
            resources.ApplyResources(this.passBox, "passBox");
            this.passBox.Name = "passBox";
            this.passBox.UseSystemPasswordChar = true;
            this.passBox.TextChanged += new System.EventHandler(this.passBox_TextChanged);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // passwordFailure1
            // 
            resources.ApplyResources(this.passwordFailure1, "passwordFailure1");
            this.tableLayoutPanel1.SetColumnSpan(this.passwordFailure1, 3);
            this.passwordFailure1.Name = "passwordFailure1";
            this.passwordFailure1.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.EnterPassLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.passLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.passBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.passwordFailure1, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.okButton, 2, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // LoadSessionDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LoadSessionDialog";
            this.Load += new System.EventHandler(this.LoadSessionDialog_Load);
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.LoadSessionDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.LoadSessionDialog_HelpRequested);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label EnterPassLabel;
        private System.Windows.Forms.Label passLabel;
        private System.Windows.Forms.TextBox passBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private XenAdmin.Controls.Common.PasswordFailure passwordFailure1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}