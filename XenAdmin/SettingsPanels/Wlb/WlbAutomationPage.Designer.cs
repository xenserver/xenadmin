namespace XenAdmin.SettingsPanels
{
    partial class WlbAutomationPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbAutomationPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.decentGroupBoxAutomationSettings = new XenAdmin.Controls.DecentGroupBox();
            this.checkBoxUseAutomation = new System.Windows.Forms.CheckBox();
            this.checkBoxEnablePowerManagement = new System.Windows.Forms.CheckBox();
            this.decentGroupBoxPowerManagementHosts = new XenAdmin.Controls.DecentGroupBox();
            this.labelNoHosts = new System.Windows.Forms.Label();
            this.listViewExPowerManagementHosts = new XenAdmin.Controls.ListViewEx();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.labelWhichHostsBlurb = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.decentGroupBoxAutomationSettings.SuspendLayout();
            this.decentGroupBoxPowerManagementHosts.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.decentGroupBoxAutomationSettings, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.decentGroupBoxPowerManagementHosts, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // decentGroupBoxAutomationSettings
            // 
            resources.ApplyResources(this.decentGroupBoxAutomationSettings, "decentGroupBoxAutomationSettings");
            this.decentGroupBoxAutomationSettings.Controls.Add(this.checkBoxUseAutomation);
            this.decentGroupBoxAutomationSettings.Controls.Add(this.checkBoxEnablePowerManagement);
            this.decentGroupBoxAutomationSettings.Name = "decentGroupBoxAutomationSettings";
            this.decentGroupBoxAutomationSettings.TabStop = false;
            // 
            // checkBoxUseAutomation
            // 
            resources.ApplyResources(this.checkBoxUseAutomation, "checkBoxUseAutomation");
            this.checkBoxUseAutomation.Name = "checkBoxUseAutomation";
            this.checkBoxUseAutomation.UseVisualStyleBackColor = true;
            this.checkBoxUseAutomation.CheckedChanged += new System.EventHandler(this.checkBoxUseAutomation_CheckedChanged);
            // 
            // checkBoxEnablePowerManagement
            // 
            resources.ApplyResources(this.checkBoxEnablePowerManagement, "checkBoxEnablePowerManagement");
            this.checkBoxEnablePowerManagement.Name = "checkBoxEnablePowerManagement";
            this.checkBoxEnablePowerManagement.UseVisualStyleBackColor = true;
            this.checkBoxEnablePowerManagement.CheckedChanged += new System.EventHandler(this.checkBoxEnablePowerManagement_CheckedChanged);
            // 
            // decentGroupBoxPowerManagementHosts
            // 
            resources.ApplyResources(this.decentGroupBoxPowerManagementHosts, "decentGroupBoxPowerManagementHosts");
            this.decentGroupBoxPowerManagementHosts.Controls.Add(this.labelNoHosts);
            this.decentGroupBoxPowerManagementHosts.Controls.Add(this.listViewExPowerManagementHosts);
            this.decentGroupBoxPowerManagementHosts.Controls.Add(this.labelWhichHostsBlurb);
            this.decentGroupBoxPowerManagementHosts.Name = "decentGroupBoxPowerManagementHosts";
            this.decentGroupBoxPowerManagementHosts.TabStop = false;
            // 
            // labelNoHosts
            // 
            resources.ApplyResources(this.labelNoHosts, "labelNoHosts");
            this.labelNoHosts.BackColor = System.Drawing.SystemColors.Window;
            this.labelNoHosts.Name = "labelNoHosts";
            // 
            // listViewExPowerManagementHosts
            // 
            resources.ApplyResources(this.listViewExPowerManagementHosts, "listViewExPowerManagementHosts");
            this.listViewExPowerManagementHosts.CheckBoxes = true;
            this.listViewExPowerManagementHosts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader1,
            this.columnHeader3});
            this.listViewExPowerManagementHosts.Name = "listViewExPowerManagementHosts";
            this.listViewExPowerManagementHosts.OwnerDraw = true;
            this.listViewExPowerManagementHosts.UseCompatibleStateImageBehavior = false;
            this.listViewExPowerManagementHosts.View = System.Windows.Forms.View.Details;
            this.listViewExPowerManagementHosts.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listViewExPowerManagementHosts_DrawColumnHeader);
            this.listViewExPowerManagementHosts.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listViewExPowerManagementHosts_DrawItem);
            this.listViewExPowerManagementHosts.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.listViewExPowerManagementHosts_ColumnWidthChanged);
            this.listViewExPowerManagementHosts.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listViewExPowerManagementHosts_ItemCheck);
            this.listViewExPowerManagementHosts.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listViewExPowerManagementHosts_DrawSubItem);
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // labelWhichHostsBlurb
            // 
            resources.ApplyResources(this.labelWhichHostsBlurb, "labelWhichHostsBlurb");
            this.labelWhichHostsBlurb.Name = "labelWhichHostsBlurb";
            // 
            // WlbAutomationPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "WlbAutomationPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.decentGroupBoxAutomationSettings.ResumeLayout(false);
            this.decentGroupBoxAutomationSettings.PerformLayout();
            this.decentGroupBoxPowerManagementHosts.ResumeLayout(false);
            this.decentGroupBoxPowerManagementHosts.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.DecentGroupBox decentGroupBoxAutomationSettings;
        private System.Windows.Forms.CheckBox checkBoxEnablePowerManagement;
        private XenAdmin.Controls.DecentGroupBox decentGroupBoxPowerManagementHosts;
        private System.Windows.Forms.Label labelNoHosts;
        private XenAdmin.Controls.ListViewEx listViewExPowerManagementHosts;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.CheckBox checkBoxUseAutomation;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelWhichHostsBlurb;
    }
}
