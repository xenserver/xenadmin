namespace XenAdmin.Dialogs
{
    partial class ConnectionRefusedDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionRefusedDialog));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.linkLabel = new System.Windows.Forms.LinkLabel();
            this.buttonOk = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxIcon, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelMessage, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.linkLabel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonOk, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // pictureBoxIcon
            // 
            resources.ApplyResources(this.pictureBoxIcon, "pictureBoxIcon");
            this.pictureBoxIcon.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_32;
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.TabStop = false;
            // 
            // labelMessage
            // 
            resources.ApplyResources(this.labelMessage, "labelMessage");
            this.tableLayoutPanel1.SetColumnSpan(this.labelMessage, 2);
            this.labelMessage.MaximumSize = new System.Drawing.Size(409, 9999);
            this.labelMessage.Name = "labelMessage";
            // 
            // linkLabel
            // 
            resources.ApplyResources(this.linkLabel, "linkLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.linkLabel, 2);
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.TabStop = true;
            this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // ConnectionRefusedDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonOk;
            this.Controls.Add(this.tableLayoutPanel1);
            this.HelpButton = false;
            this.Name = "ConnectionRefusedDialog";
            this.ShowIcon = false;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.LinkLabel linkLabel;
        private System.Windows.Forms.Button buttonOk;
    }
}