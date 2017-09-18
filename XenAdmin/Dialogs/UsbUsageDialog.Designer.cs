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
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.checkBoxAllow = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlert)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxAlert
            // 
            this.pictureBoxAlert.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.pictureBoxAlert.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.pictureBoxAlert.InitialImage = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.pictureBoxAlert.Location = new System.Drawing.Point(9, 80);
            this.pictureBoxAlert.Name = "pictureBoxAlert";
            this.pictureBoxAlert.Size = new System.Drawing.Size(22, 22);
            this.pictureBoxAlert.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxAlert.TabIndex = 1;
            this.pictureBoxAlert.TabStop = false;
            // 
            // buttonOK
            // 
            this.buttonOK.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.buttonOK.Location = new System.Drawing.Point(213, 147);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.buttonCancel.Location = new System.Drawing.Point(294, 147);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label1.Location = new System.Drawing.Point(37, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(332, 45);
            this.label1.TabIndex = 5;
            this.label1.Text = "Plugging in untrustworthy USB devices to your computer may put your computer at r" +
    "isk. USB devices with modifiable behaviour should only be assigned to trustworth" +
    "y guest VMs.";
            // 
            // autoHeightLabel1
            // 
            this.autoHeightLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autoHeightLabel1.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.autoHeightLabel1.Location = new System.Drawing.Point(9, 9);
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            this.autoHeightLabel1.Size = new System.Drawing.Size(360, 31);
            this.autoHeightLabel1.TabIndex = 6;
            this.autoHeightLabel1.Text = "Allow device to be passed through to VM or Used as Removable SR";
            // 
            // checkBoxAllow
            // 
            this.checkBoxAllow.AutoSize = true;
            this.checkBoxAllow.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkBoxAllow.Location = new System.Drawing.Point(40, 43);
            this.checkBoxAllow.Name = "checkBoxAllow";
            this.checkBoxAllow.Size = new System.Drawing.Size(128, 19);
            this.checkBoxAllow.TabIndex = 7;
            this.checkBoxAllow.Text = "Allow pass through";
            this.checkBoxAllow.UseVisualStyleBackColor = true;
            // 
            // UsbUsageDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 182);
            this.Controls.Add(this.checkBoxAllow);
            this.Controls.Add(this.autoHeightLabel1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.pictureBoxAlert);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "UsbUsageDialog";
            this.Text = "Configure USB devices usage";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlert)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxAlert;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private Controls.Common.AutoHeightLabel label1;
        private Controls.Common.AutoHeightLabel autoHeightLabel1;
        private System.Windows.Forms.CheckBox checkBoxAllow;

    }
}