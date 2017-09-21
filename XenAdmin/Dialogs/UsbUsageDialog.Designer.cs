namespace XenAdmin.Dialogs
{
    partial class UsbUsageDialog
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
            this.pictureBoxAlert = new System.Windows.Forms.PictureBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.labelNote = new XenAdmin.Controls.Common.AutoHeightLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlert)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxAlert
            // 
            this.pictureBoxAlert.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.pictureBoxAlert.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.pictureBoxAlert.InitialImage = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.pictureBoxAlert.Location = new System.Drawing.Point(8, 52);
            this.pictureBoxAlert.Name = "pictureBoxAlert";
            this.pictureBoxAlert.Size = new System.Drawing.Size(22, 22);
            this.pictureBoxAlert.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxAlert.TabIndex = 1;
            this.pictureBoxAlert.TabStop = false;
            // 
            // buttonOK
            // 
            this.buttonOK.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.buttonOK.Location = new System.Drawing.Point(118, 112);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(170, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "Yes, enable passthrough";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.buttonCancel.Location = new System.Drawing.Point(294, 112);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "&No";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label1.Location = new System.Drawing.Point(32, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(332, 45);
            this.label1.TabIndex = 5;
            this.label1.Text = "Plugging in untrustworthy USB devices to your computer may put your computer at r" +
    "isk. USB devices with modifiable behaviour should only be assigned to trustworth" +
    "y guest VMs.";
            // 
            // labelNote
            // 
            this.labelNote.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelNote.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.labelNote.Location = new System.Drawing.Point(9, 9);
            this.labelNote.Name = "labelNote";
            this.labelNote.Size = new System.Drawing.Size(360, 30);
            this.labelNote.TabIndex = 6;
            this.labelNote.Text = "Are you sure that you want to enable passthrough on this device and allow to be p" +
    "assed through to VMs?";
            // 
            // UsbUsageDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 147);
            this.Controls.Add(this.labelNote);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.pictureBoxAlert);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "UsbUsageDialog";
            this.Text = "Configure USB devices usage";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlert)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxAlert;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private Controls.Common.AutoHeightLabel label1;
        private Controls.Common.AutoHeightLabel labelNote;

    }
}