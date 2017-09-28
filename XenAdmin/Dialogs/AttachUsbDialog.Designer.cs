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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttachUsbDialog));
            this.buttonAttach = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.treeUsbList = new XenAdmin.Controls.CustomTreeView();
            this.labelNote = new System.Windows.Forms.Label();
            this.pictureBoxAlert = new System.Windows.Forms.PictureBox();
            this.labelWarningLine1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.labelWarningLine3 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.labelWarningLine2 = new XenAdmin.Controls.Common.AutoHeightLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlert)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAttach
            // 
            this.buttonAttach.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.buttonAttach, "buttonAttach");
            this.buttonAttach.Name = "buttonAttach";
            this.buttonAttach.UseVisualStyleBackColor = true;
            this.buttonAttach.Click += new System.EventHandler(this.buttonAttach_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // treeUsbList
            // 
            resources.ApplyResources(this.treeUsbList, "treeUsbList");
            this.treeUsbList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.treeUsbList.Name = "treeUsbList";
            this.treeUsbList.NodeIndent = 19;
            this.treeUsbList.RootAlwaysExpanded = false;
            this.treeUsbList.ShowCheckboxes = false;
            this.treeUsbList.ShowDescription = false;
            this.treeUsbList.ShowImages = false;
            this.treeUsbList.ShowRootLines = true;
            this.treeUsbList.SelectedIndexChanged += new System.EventHandler(this.treeUsbList_SelectedIndexChanged);
            // 
            // labelNote
            // 
            resources.ApplyResources(this.labelNote, "labelNote");
            this.labelNote.Name = "labelNote";
            // 
            // pictureBoxAlert
            // 
            resources.ApplyResources(this.pictureBoxAlert, "pictureBoxAlert");
            this.pictureBoxAlert.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxAlert.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.pictureBoxAlert.InitialImage = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.pictureBoxAlert.Name = "pictureBoxAlert";
            this.pictureBoxAlert.TabStop = false;
            // 
            // labelWarningLine1
            // 
            resources.ApplyResources(this.labelWarningLine1, "labelWarningLine1");
            this.labelWarningLine1.Name = "labelWarningLine1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelNote, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.treeUsbList, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.buttonCancel);
            this.flowLayoutPanel1.Controls.Add(this.buttonAttach);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.pictureBoxAlert, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.labelWarningLine3, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.labelWarningLine1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelWarningLine2, 0, 2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // labelWarningLine3
            // 
            resources.ApplyResources(this.labelWarningLine3, "labelWarningLine3");
            this.labelWarningLine3.Name = "labelWarningLine3";
            // 
            // labelWarningLine2
            // 
            resources.ApplyResources(this.labelWarningLine2, "labelWarningLine2");
            this.labelWarningLine2.Name = "labelWarningLine2";
            // 
            // AttachUsbDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "AttachUsbDialog";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlert)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.CustomTreeView treeUsbList;
        private System.Windows.Forms.Button buttonAttach;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelNote;
        private System.Windows.Forms.PictureBox pictureBoxAlert;
        private Controls.Common.AutoHeightLabel labelWarningLine1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private Controls.Common.AutoHeightLabel labelWarningLine3;
        private Controls.Common.AutoHeightLabel labelWarningLine2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    }
}
