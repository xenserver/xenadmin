namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class VHDoNFSNew
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VHDoNFSNew));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.nfsVersionLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ServerTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelAdvancedOptions = new System.Windows.Forms.Label();
            this.serverOptionsTextBox = new System.Windows.Forms.TextBox();
            this.radioButtonNfsNew = new System.Windows.Forms.RadioButton();
            this.radioButtonNfsReattach = new System.Windows.Forms.RadioButton();
            this.listBoxNfsSRs = new XenAdmin.Controls.SRListBox();
            this.serverScanButton = new System.Windows.Forms.Button();
            this.nfsVersionSelectorTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.nfsVersion3RadioButton = new System.Windows.Forms.RadioButton();
            this.nfsVersion4RadioButton = new System.Windows.Forms.RadioButton();
            this.labelShareName = new System.Windows.Forms.Label();
            this.NfsSharePathTextBox = new System.Windows.Forms.TextBox();
            this.NfsScanButton = new System.Windows.Forms.Button();
            this.sharePathExampleLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.nfsVersionSelectorTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.nfsVersionLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ServerTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelAdvancedOptions, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.serverOptionsTextBox, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonNfsNew, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonNfsReattach, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.listBoxNfsSRs, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.serverScanButton, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.nfsVersionSelectorTableLayoutPanel, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelShareName, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.NfsSharePathTextBox, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.NfsScanButton, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.sharePathExampleLabel, 1, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // nfsVersionLabel
            // 
            resources.ApplyResources(this.nfsVersionLabel, "nfsVersionLabel");
            this.nfsVersionLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nfsVersionLabel.Name = "nfsVersionLabel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label3.Name = "label3";
            // 
            // ServerTextBox
            // 
            resources.ApplyResources(this.ServerTextBox, "ServerTextBox");
            this.ServerTextBox.Name = "ServerTextBox";
            this.ServerTextBox.TextChanged += new System.EventHandler(this.ServerTextBox_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label2.Name = "label2";
            // 
            // labelAdvancedOptions
            // 
            resources.ApplyResources(this.labelAdvancedOptions, "labelAdvancedOptions");
            this.labelAdvancedOptions.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelAdvancedOptions.Name = "labelAdvancedOptions";
            // 
            // serverOptionsTextBox
            // 
            resources.ApplyResources(this.serverOptionsTextBox, "serverOptionsTextBox");
            this.serverOptionsTextBox.Name = "serverOptionsTextBox";
            // 
            // radioButtonNfsNew
            // 
            resources.ApplyResources(this.radioButtonNfsNew, "radioButtonNfsNew");
            this.radioButtonNfsNew.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonNfsNew, 2);
            this.radioButtonNfsNew.Name = "radioButtonNfsNew";
            this.radioButtonNfsNew.TabStop = true;
            this.radioButtonNfsNew.UseVisualStyleBackColor = true;
            this.radioButtonNfsNew.CheckedChanged += new System.EventHandler(this.radioButtonNfsNew_CheckedChanged);
            // 
            // radioButtonNfsReattach
            // 
            resources.ApplyResources(this.radioButtonNfsReattach, "radioButtonNfsReattach");
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonNfsReattach, 3);
            this.radioButtonNfsReattach.Name = "radioButtonNfsReattach";
            this.radioButtonNfsReattach.UseVisualStyleBackColor = true;
            this.radioButtonNfsReattach.CheckedChanged += new System.EventHandler(this.radioButtonNfsReattach_CheckedChanged);
            // 
            // listBoxNfsSRs
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.listBoxNfsSRs, 3);
            resources.ApplyResources(this.listBoxNfsSRs, "listBoxNfsSRs");
            this.listBoxNfsSRs.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxNfsSRs.Name = "listBoxNfsSRs";
            this.listBoxNfsSRs.Sorted = true;
            this.listBoxNfsSRs.SelectedIndexChanged += new System.EventHandler(this.listBoxNfsSRs_SelectedIndexChanged);
            // 
            // serverScanButton
            // 
            resources.ApplyResources(this.serverScanButton, "serverScanButton");
            this.serverScanButton.Name = "serverScanButton";
            this.serverScanButton.UseVisualStyleBackColor = true;
            this.serverScanButton.Click += new System.EventHandler(this.serverScanButton_Click);
            // 
            // nfsVersionSelectorTableLayoutPanel
            // 
            resources.ApplyResources(this.nfsVersionSelectorTableLayoutPanel, "nfsVersionSelectorTableLayoutPanel");
            this.tableLayoutPanel1.SetColumnSpan(this.nfsVersionSelectorTableLayoutPanel, 2);
            this.nfsVersionSelectorTableLayoutPanel.Controls.Add(this.nfsVersion3RadioButton, 0, 0);
            this.nfsVersionSelectorTableLayoutPanel.Controls.Add(this.nfsVersion4RadioButton, 0, 1);
            this.nfsVersionSelectorTableLayoutPanel.Name = "nfsVersionSelectorTableLayoutPanel";
            // 
            // nfsVersion3RadioButton
            // 
            resources.ApplyResources(this.nfsVersion3RadioButton, "nfsVersion3RadioButton");
            this.nfsVersion3RadioButton.Checked = true;
            this.nfsVersion3RadioButton.Name = "nfsVersion3RadioButton";
            this.nfsVersion3RadioButton.TabStop = true;
            this.nfsVersion3RadioButton.UseVisualStyleBackColor = true;
            this.nfsVersion3RadioButton.CheckedChanged += new System.EventHandler(this.nfsVersion3RadioButton_CheckedChanged);
            // 
            // nfsVersion4RadioButton
            // 
            resources.ApplyResources(this.nfsVersion4RadioButton, "nfsVersion4RadioButton");
            this.nfsVersion4RadioButton.Name = "nfsVersion4RadioButton";
            this.nfsVersion4RadioButton.UseVisualStyleBackColor = true;
            this.nfsVersion4RadioButton.CheckedChanged += new System.EventHandler(this.nfsVersion4RadioButton_CheckedChanged);
            // 
            // labelShareName
            // 
            resources.ApplyResources(this.labelShareName, "labelShareName");
            this.labelShareName.Name = "labelShareName";
            // 
            // NfsSharePathTextBox
            // 
            resources.ApplyResources(this.NfsSharePathTextBox, "NfsSharePathTextBox");
            this.NfsSharePathTextBox.Name = "NfsSharePathTextBox";
            this.NfsSharePathTextBox.TextChanged += new System.EventHandler(this.NfsServerPathTextBox_TextChanged);
            // 
            // NfsScanButton
            // 
            resources.ApplyResources(this.NfsScanButton, "NfsScanButton");
            this.NfsScanButton.Name = "NfsScanButton";
            this.NfsScanButton.UseVisualStyleBackColor = true;
            this.NfsScanButton.Click += new System.EventHandler(this.buttonNfsScan_Click);
            // 
            // sharePathExampleLabel
            // 
            resources.ApplyResources(this.sharePathExampleLabel, "sharePathExampleLabel");
            this.sharePathExampleLabel.Name = "sharePathExampleLabel";
            // 
            // VHDoNFSNew
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "VHDoNFSNew";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.nfsVersionSelectorTableLayoutPanel.ResumeLayout(false);
            this.nfsVersionSelectorTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonNfsReattach;
        private XenAdmin.Controls.SRListBox listBoxNfsSRs;
       private System.Windows.Forms.RadioButton radioButtonNfsNew;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox serverOptionsTextBox;
        private System.Windows.Forms.Label labelAdvancedOptions;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ServerTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label nfsVersionLabel;
        private System.Windows.Forms.TableLayoutPanel nfsVersionSelectorTableLayoutPanel;
        private System.Windows.Forms.RadioButton nfsVersion3RadioButton;
        private System.Windows.Forms.RadioButton nfsVersion4RadioButton;
        private System.Windows.Forms.Button serverScanButton;
        private System.Windows.Forms.Label labelShareName;
        private System.Windows.Forms.TextBox NfsSharePathTextBox;
        private System.Windows.Forms.Button NfsScanButton;
        private System.Windows.Forms.Label sharePathExampleLabel;
    }
}
