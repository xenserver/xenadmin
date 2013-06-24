namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class VHDoNFS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VHDoNFS));
            this.toolTipContainerNfs1 = new XenAdmin.Controls.ToolTipContainer();
            this.NfsScanButton = new System.Windows.Forms.Button();
            this.toolTipContainerNfs2 = new XenAdmin.Controls.ToolTipContainer();
            this.panelNfsReattach = new System.Windows.Forms.Panel();
            this.listBoxNfsSRs = new XenAdmin.Controls.SRListBox();
            this.radioButtonNfsReattach = new System.Windows.Forms.RadioButton();
            this.radioButtonNfsNew = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.serverOptionsTextBox = new System.Windows.Forms.TextBox();
            this.labelAdvancedOptions = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NfsServerPathTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTipContainerNfs1.SuspendLayout();
            this.toolTipContainerNfs2.SuspendLayout();
            this.panelNfsReattach.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTipContainerNfs1
            // 
            resources.ApplyResources(this.toolTipContainerNfs1, "toolTipContainerNfs1");
            this.toolTipContainerNfs1.Controls.Add(this.NfsScanButton);
            this.toolTipContainerNfs1.Name = "toolTipContainerNfs1";
            // 
            // NfsScanButton
            // 
            resources.ApplyResources(this.NfsScanButton, "NfsScanButton");
            this.NfsScanButton.Name = "NfsScanButton";
            this.NfsScanButton.UseVisualStyleBackColor = true;
            this.NfsScanButton.Click += new System.EventHandler(this.buttonNfsScan_Click);
            // 
            // toolTipContainerNfs2
            // 
            resources.ApplyResources(this.toolTipContainerNfs2, "toolTipContainerNfs2");
            this.toolTipContainerNfs2.Controls.Add(this.panelNfsReattach);
            this.toolTipContainerNfs2.Name = "toolTipContainerNfs2";
            // 
            // panelNfsReattach
            // 
            this.panelNfsReattach.Controls.Add(this.listBoxNfsSRs);
            this.panelNfsReattach.Controls.Add(this.radioButtonNfsReattach);
            resources.ApplyResources(this.panelNfsReattach, "panelNfsReattach");
            this.panelNfsReattach.Name = "panelNfsReattach";
            // 
            // listBoxNfsSRs
            // 
            resources.ApplyResources(this.listBoxNfsSRs, "listBoxNfsSRs");
            this.listBoxNfsSRs.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxNfsSRs.Name = "listBoxNfsSRs";
            this.listBoxNfsSRs.Sorted = true;
            this.listBoxNfsSRs.SelectedIndexChanged += new System.EventHandler(this.listBoxNfsSRs_SelectedIndexChanged);
            // 
            // radioButtonNfsReattach
            // 
            resources.ApplyResources(this.radioButtonNfsReattach, "radioButtonNfsReattach");
            this.radioButtonNfsReattach.Name = "radioButtonNfsReattach";
            this.radioButtonNfsReattach.UseVisualStyleBackColor = true;
            this.radioButtonNfsReattach.CheckedChanged += new System.EventHandler(this.radioButtonNfsReattach_CheckedChanged);
            // 
            // radioButtonNfsNew
            // 
            resources.ApplyResources(this.radioButtonNfsNew, "radioButtonNfsNew");
            this.radioButtonNfsNew.Checked = true;
            this.radioButtonNfsNew.Name = "radioButtonNfsNew";
            this.radioButtonNfsNew.TabStop = true;
            this.radioButtonNfsNew.UseVisualStyleBackColor = true;
            this.radioButtonNfsNew.CheckedChanged += new System.EventHandler(this.radioButtonNfsNew_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label2.Name = "label2";
            // 
            // serverOptionsTextBox
            // 
            resources.ApplyResources(this.serverOptionsTextBox, "serverOptionsTextBox");
            this.serverOptionsTextBox.Name = "serverOptionsTextBox";
            // 
            // labelAdvancedOptions
            // 
            resources.ApplyResources(this.labelAdvancedOptions, "labelAdvancedOptions");
            this.labelAdvancedOptions.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelAdvancedOptions.Name = "labelAdvancedOptions";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.ForeColor = System.Drawing.SystemColors.InfoText;
            this.label10.Name = "label10";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label3.Name = "label3";
            // 
            // NfsServerPathTextBox
            // 
            resources.ApplyResources(this.NfsServerPathTextBox, "NfsServerPathTextBox");
            this.NfsServerPathTextBox.Name = "NfsServerPathTextBox";
            this.NfsServerPathTextBox.TextChanged += new System.EventHandler(this.NfsServerPathTextBox_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.toolTipContainerNfs1, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelAdvancedOptions, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.serverOptionsTextBox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.NfsServerPathTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // VHDoNFS
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolTipContainerNfs2);
            this.Controls.Add(this.radioButtonNfsNew);
            this.Controls.Add(this.label10);
            this.Name = "VHDoNFS";
            this.toolTipContainerNfs1.ResumeLayout(false);
            this.toolTipContainerNfs2.ResumeLayout(false);
            this.panelNfsReattach.ResumeLayout(false);
            this.panelNfsReattach.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.ToolTipContainer toolTipContainerNfs1;
        private System.Windows.Forms.Button NfsScanButton;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerNfs2;
        private System.Windows.Forms.Panel panelNfsReattach;
        private System.Windows.Forms.RadioButton radioButtonNfsReattach;
        private XenAdmin.Controls.SRListBox listBoxNfsSRs;
        private System.Windows.Forms.RadioButton radioButtonNfsNew;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox serverOptionsTextBox;
        private System.Windows.Forms.Label labelAdvancedOptions;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox NfsServerPathTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
    }
}
