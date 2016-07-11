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
            this.panel1 = new System.Windows.Forms.Panel();
            this.cdChanger1 = new XenAdmin.Controls.CDChanger();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
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
            this.tableLayoutPanel1.Controls.Add(this.newCDLabel, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 2, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.cdChanger1);
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Name = "panel1";
            // 
            // cdChanger1
            // 
            this.cdChanger1.connection = null;
            this.cdChanger1.DisplayISO = false;
            this.cdChanger1.DisplayPhysical = false;
            resources.ApplyResources(this.cdChanger1, "cdChanger1");
            this.cdChanger1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cdChanger1.Drive = null;
            this.cdChanger1.DropDownHeight = 500;
            this.cdChanger1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cdChanger1.Empty = true;
            this.cdChanger1.FormattingEnabled = true;
            this.cdChanger1.Name = "cdChanger1";
            this.cdChanger1.SelectedCD = null;
            this.cdChanger1.TheVM = null;
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // MultipleDvdIsoList
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MultipleDvdIsoList";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private CDChanger cdChanger1;
        private System.Windows.Forms.Label labelSingleDvd;
        private System.Windows.Forms.ComboBox comboBoxDrive;
		private System.Windows.Forms.Label newCDLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}
