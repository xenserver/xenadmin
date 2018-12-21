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
            this.memorySpinner = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.hostRebootWarningLabel = new System.Windows.Forms.Label();
            this.hostRebootWarningImage = new System.Windows.Forms.PictureBox();
            this.tplRebootWarning = new System.Windows.Forms.TableLayoutPanel();
            this.maintenanceModeLinkLabel = new System.Windows.Forms.LinkLabel();
            this.maintenanceWarningLabel = new System.Windows.Forms.Label();
            this.maintenanceWarningImage = new System.Windows.Forms.PictureBox();
            this.tplMaintenanceWarning = new System.Windows.Forms.TableLayoutPanel();
            this.labelMemory = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.OkButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hostRebootWarningImage)).BeginInit();
            this.tplRebootWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maintenanceWarningImage)).BeginInit();
            this.tplMaintenanceWarning.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tplMaintenanceWarning, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tplRebootWarning, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.memorySpinner, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelMemory, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // memorySpinner
            // 
            resources.ApplyResources(this.memorySpinner, "memorySpinner");
            this.memorySpinner.Increment = 0.1D;
            this.memorySpinner.Name = "memorySpinner";
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
            // tplRebootWarning
            // 
            resources.ApplyResources(this.tplRebootWarning, "tplRebootWarning");
            this.tableLayoutPanel1.SetColumnSpan(this.tplRebootWarning, 2);
            this.tplRebootWarning.Controls.Add(this.hostRebootWarningImage, 0, 0);
            this.tplRebootWarning.Controls.Add(this.hostRebootWarningLabel, 1, 0);
            this.tplRebootWarning.Name = "tplRebootWarning";
            // 
            // maintenanceModeLinkLabel
            // 
            resources.ApplyResources(this.maintenanceModeLinkLabel, "maintenanceModeLinkLabel");
            this.maintenanceModeLinkLabel.Name = "maintenanceModeLinkLabel";
            this.maintenanceModeLinkLabel.TabStop = true;
            this.maintenanceModeLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.maintenanceModeLinkLabel_LinkClicked);
            // 
            // maintenanceWarningLabel
            // 
            resources.ApplyResources(this.maintenanceWarningLabel, "maintenanceWarningLabel");
            this.maintenanceWarningLabel.Name = "maintenanceWarningLabel";
            // 
            // maintenanceWarningImage
            // 
            resources.ApplyResources(this.maintenanceWarningImage, "maintenanceWarningImage");
            this.maintenanceWarningImage.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.maintenanceWarningImage.Name = "maintenanceWarningImage";
            this.maintenanceWarningImage.TabStop = false;
            // 
            // tplMaintenanceWarning
            // 
            resources.ApplyResources(this.tplMaintenanceWarning, "tplMaintenanceWarning");
            this.tableLayoutPanel1.SetColumnSpan(this.tplMaintenanceWarning, 2);
            this.tplMaintenanceWarning.Controls.Add(this.maintenanceWarningImage, 0, 0);
            this.tplMaintenanceWarning.Controls.Add(this.maintenanceWarningLabel, 1, 0);
            this.tplMaintenanceWarning.Controls.Add(this.maintenanceModeLinkLabel, 1, 1);
            this.tplMaintenanceWarning.Name = "tplMaintenanceWarning";
            // 
            // labelMemory
            // 
            resources.ApplyResources(this.labelMemory, "labelMemory");
            this.labelMemory.Name = "labelMemory";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // OkButton
            // 
            resources.ApplyResources(this.OkButton, "OkButton");
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OkButton.Name = "OkButton";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.CloseButton);
            this.panel1.Controls.Add(this.OkButton);
            this.panel1.Name = "panel1";
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
            ((System.ComponentModel.ISupportInitialize)(this.hostRebootWarningImage)).EndInit();
            this.tplRebootWarning.ResumeLayout(false);
            this.tplRebootWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maintenanceWarningImage)).EndInit();
            this.tplMaintenanceWarning.ResumeLayout(false);
            this.tplMaintenanceWarning.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Controls.Ballooning.MemorySpinner memorySpinner;
        private System.Windows.Forms.FlowLayoutPanel panel1;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tplMaintenanceWarning;
        private System.Windows.Forms.PictureBox maintenanceWarningImage;
        private System.Windows.Forms.Label maintenanceWarningLabel;
        private System.Windows.Forms.LinkLabel maintenanceModeLinkLabel;
        private System.Windows.Forms.TableLayoutPanel tplRebootWarning;
        private System.Windows.Forms.PictureBox hostRebootWarningImage;
        private System.Windows.Forms.Label hostRebootWarningLabel;
        private System.Windows.Forms.Label labelMemory;
    }
}