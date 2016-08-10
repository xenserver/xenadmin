namespace XenAdmin.Dialogs
{
    partial class ControlDomainMemoryDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlDomainMemoryDialog));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.CloseButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.hostRebootWarningLabel = new System.Windows.Forms.Label();
            this.hostRebootWarningImage = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.maintenanceWarningImage = new System.Windows.Forms.PictureBox();
            this.maintenanceWarningLabel = new System.Windows.Forms.Label();
            this.memorySpinner = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.maintenanceModeLinkLabel = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hostRebootWarningImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maintenanceWarningImage)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.hostRebootWarningLabel, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.hostRebootWarningImage, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.maintenanceWarningImage, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.maintenanceWarningLabel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.memorySpinner, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.maintenanceModeLinkLabel, 1, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.CloseButton);
            this.panel1.Controls.Add(this.OkButton);
            this.panel1.Name = "panel1";
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // OkButton
            // 
            resources.ApplyResources(this.OkButton, "OkButton");
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OkButton.Name = "OkButton";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // hostRebootWarningLabel
            // 
            resources.ApplyResources(this.hostRebootWarningLabel, "hostRebootWarningLabel");
            this.hostRebootWarningLabel.Name = "hostRebootWarningLabel";
            // 
            // hostRebootWarningImage
            // 
            resources.ApplyResources(this.hostRebootWarningImage, "hostRebootWarningImage");
            this.hostRebootWarningImage.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.hostRebootWarningImage.Name = "hostRebootWarningImage";
            this.hostRebootWarningImage.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // maintenanceWarningImage
            // 
            resources.ApplyResources(this.maintenanceWarningImage, "maintenanceWarningImage");
            this.maintenanceWarningImage.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.maintenanceWarningImage.Name = "maintenanceWarningImage";
            this.maintenanceWarningImage.TabStop = false;
            // 
            // maintenanceWarningLabel
            // 
            resources.ApplyResources(this.maintenanceWarningLabel, "maintenanceWarningLabel");
            this.maintenanceWarningLabel.Name = "maintenanceWarningLabel";
            // 
            // memorySpinner
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.memorySpinner, 2);
            resources.ApplyResources(this.memorySpinner, "memorySpinner");
            this.memorySpinner.Increment = 0.1D;
            this.memorySpinner.Name = "memorySpinner";
            // 
            // maintenanceModeLinkLabel
            // 
            resources.ApplyResources(this.maintenanceModeLinkLabel, "maintenanceModeLinkLabel");
            this.maintenanceModeLinkLabel.Name = "maintenanceModeLinkLabel";
            this.maintenanceModeLinkLabel.TabStop = true;
            this.maintenanceModeLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.maintenanceModeLinkLabel_LinkClicked);
            // 
            // ControlDomainMemoryDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ControlDomainMemoryDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlDomainMemoryDialog_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.hostRebootWarningImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maintenanceWarningImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label hostRebootWarningLabel;
        private System.Windows.Forms.PictureBox hostRebootWarningImage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox maintenanceWarningImage;
        private System.Windows.Forms.Label maintenanceWarningLabel;
        private Controls.Ballooning.MemorySpinner memorySpinner;
        private System.Windows.Forms.FlowLayoutPanel panel1;
        public System.Windows.Forms.Button CloseButton;
        public System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.LinkLabel maintenanceModeLinkLabel;
    }
}