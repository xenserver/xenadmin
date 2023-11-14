namespace XenAdmin.Controls
{
    partial class MultipleDvdIsoList
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
            // deregister all event handlers
            DeregisterEvents();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultipleDvdIsoList));
            this.labelSingleDvd = new System.Windows.Forms.Label();
            this.newCDLabel = new System.Windows.Forms.Label();
            this.comboBoxDrive = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cdChanger1 = new XenAdmin.Controls.CDChanger();
            this.linkLabelEject = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelSingleDvd
            // 
            resources.ApplyResources(this.labelSingleDvd, "labelSingleDvd");
            this.labelSingleDvd.Name = "labelSingleDvd";
            // 
            // newCDLabel
            // 
            resources.ApplyResources(this.newCDLabel, "newCDLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.newCDLabel, 4);
            this.newCDLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.newCDLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.newCDLabel.Name = "newCDLabel";
            this.newCDLabel.Click += new System.EventHandler(this.newCDLabel_Click);
            // 
            // comboBoxDrive
            // 
            this.comboBoxDrive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDrive.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxDrive, "comboBoxDrive");
            this.comboBoxDrive.Name = "comboBoxDrive";
            this.comboBoxDrive.SelectedIndexChanged += new System.EventHandler(this.comboBoxDrive_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelSingleDvd, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxDrive, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cdChanger1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.linkLabelEject, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.newCDLabel, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // cdChanger1
            // 
            resources.ApplyResources(this.cdChanger1, "cdChanger1");
            this.cdChanger1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cdChanger1.DropDownHeight = 500;
            this.cdChanger1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cdChanger1.FormattingEnabled = true;
            this.cdChanger1.Name = "cdChanger1";
            // 
            // linkLabelEject
            // 
            resources.ApplyResources(this.linkLabelEject, "linkLabelEject");
            this.linkLabelEject.Name = "linkLabelEject";
            this.linkLabelEject.TabStop = true;
            this.linkLabelEject.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEject_LinkClicked);
            // 
            // MultipleDvdIsoList
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MultipleDvdIsoList";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CDChanger cdChanger1;
        private System.Windows.Forms.Label labelSingleDvd;
        private System.Windows.Forms.ComboBox comboBoxDrive;
		private System.Windows.Forms.Label newCDLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.LinkLabel linkLabelEject;
    }
}
