namespace XenAdmin.Dialogs
{
    partial class AttachUsbDialog
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
            this.buttonAttach = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.treeUsbList = new XenAdmin.Controls.CustomTreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxAlert = new System.Windows.Forms.PictureBox();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlert)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAttach
            // 
            this.buttonAttach.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.buttonAttach.Location = new System.Drawing.Point(230, 239);
            this.buttonAttach.Name = "buttonAttach";
            this.buttonAttach.Size = new System.Drawing.Size(75, 23);
            this.buttonAttach.TabIndex = 0;
            this.buttonAttach.Text = "&Attach";
            this.buttonAttach.UseVisualStyleBackColor = true;
            this.buttonAttach.Click += new System.EventHandler(this.buttonAttach_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.buttonCancel.Location = new System.Drawing.Point(311, 239);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // treeUsbList
            // 
            this.treeUsbList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.treeUsbList.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.treeUsbList.IntegralHeight = false;
            this.treeUsbList.ItemHeight = 17;
            this.treeUsbList.Location = new System.Drawing.Point(16, 35);
            this.treeUsbList.Name = "treeUsbList";
            this.treeUsbList.NodeIndent = 19;
            this.treeUsbList.RootAlwaysExpanded = false;
            this.treeUsbList.ShowCheckboxes = false;
            this.treeUsbList.ShowDescription = true;
            this.treeUsbList.ShowImages = false;
            this.treeUsbList.ShowRootLines = true;
            this.treeUsbList.Size = new System.Drawing.Size(370, 141);
            this.treeUsbList.TabIndex = 3;
            this.treeUsbList.SelectedIndexChanged += new System.EventHandler(this.treeUsbList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(275, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select a device to pass through from the list below.";
            // 
            // pictureBoxAlert
            // 
            this.pictureBoxAlert.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.pictureBoxAlert.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.pictureBoxAlert.InitialImage = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.pictureBoxAlert.Location = new System.Drawing.Point(15, 180);
            this.pictureBoxAlert.Name = "pictureBoxAlert";
            this.pictureBoxAlert.Size = new System.Drawing.Size(22, 22);
            this.pictureBoxAlert.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxAlert.TabIndex = 6;
            this.pictureBoxAlert.TabStop = false;
            // 
            // autoHeightLabel1
            // 
            this.autoHeightLabel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.autoHeightLabel1.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.autoHeightLabel1.Location = new System.Drawing.Point(39, 183);
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            this.autoHeightLabel1.Size = new System.Drawing.Size(332, 45);
            this.autoHeightLabel1.TabIndex = 7;
            this.autoHeightLabel1.Text = "Plugging in untrustworthy USB devices to your computer may put your computer at r" +
    "isk. USB devices with modifiable behaviour should only be assigned to trustworth" +
    "y guest VMs.";
            // 
            // AttachUsbDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 274);
            this.Controls.Add(this.pictureBoxAlert);
            this.Controls.Add(this.autoHeightLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonAttach);
            this.Controls.Add(this.treeUsbList);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "AttachUsbDialog";
            this.Text = "Attach USB";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlert)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.CustomTreeView treeUsbList;
        private System.Windows.Forms.Button buttonAttach;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxAlert;
        private Controls.Common.AutoHeightLabel autoHeightLabel1;
    }
}
