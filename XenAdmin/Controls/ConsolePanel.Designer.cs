namespace XenAdmin.Controls
{
    partial class ConsolePanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsolePanel));
            this.errorLabel = new System.Windows.Forms.Label();
            this.RbacWarningPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lableRbacWarning = new System.Windows.Forms.Label();
            this.RbacWarningPanel.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // errorLabel
            // 
            resources.ApplyResources(this.errorLabel, "errorLabel");
            this.errorLabel.Name = "errorLabel";
            // 
            // RbacWarningPanel
            // 
            this.RbacWarningPanel.BackColor = System.Drawing.Color.Transparent;
            this.RbacWarningPanel.Controls.Add(this.tableLayoutPanel3);
            resources.ApplyResources(this.RbacWarningPanel, "RbacWarningPanel");
            this.RbacWarningPanel.Name = "RbacWarningPanel";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.pictureBox2, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lableRbacWarning, 1, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // lableRbacWarning
            // 
            resources.ApplyResources(this.lableRbacWarning, "lableRbacWarning");
            this.lableRbacWarning.Name = "lableRbacWarning";
            // 
            // ConsolePanel
            // 
            this.Controls.Add(this.RbacWarningPanel);
            this.Name = "ConsolePanel";
            resources.ApplyResources(this, "$this");
            this.RbacWarningPanel.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }
        
        #endregion


        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.Label lableRbacWarning;
        private System.Windows.Forms.Panel RbacWarningPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.PictureBox pictureBox2;

    }
}