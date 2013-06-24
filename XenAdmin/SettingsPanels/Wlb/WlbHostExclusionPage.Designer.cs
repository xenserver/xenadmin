namespace XenAdmin.SettingsPanels
{
    partial class WlbHostExclusionPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbHostExclusionPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.listViewExExcludedHosts = new XenAdmin.Controls.ListViewEx();
            this.columnHeaderExclude = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderHostName = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.listViewExExcludedHosts, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // listViewExExcludedHosts
            // 
            resources.ApplyResources(this.listViewExExcludedHosts, "listViewExExcludedHosts");
            this.listViewExExcludedHosts.CheckBoxes = true;
            this.listViewExExcludedHosts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderExclude,
            this.columnHeaderHostName});
            this.listViewExExcludedHosts.Name = "listViewExExcludedHosts";
            this.listViewExExcludedHosts.UseCompatibleStateImageBehavior = false;
            this.listViewExExcludedHosts.View = System.Windows.Forms.View.Details;
            this.listViewExExcludedHosts.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.listViewExExcludedHosts_ColumnWidthChanged);
            this.listViewExExcludedHosts.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listViewExExcludedHosts_ItemCheck);
            // 
            // columnHeaderExclude
            // 
            resources.ApplyResources(this.columnHeaderExclude, "columnHeaderExclude");
            // 
            // columnHeaderHostName
            // 
            resources.ApplyResources(this.columnHeaderHostName, "columnHeaderHostName");
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // WlbHostExclusionPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "WlbHostExclusionPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private XenAdmin.Controls.ListViewEx listViewExExcludedHosts;
        private XenAdmin.Controls.ListViewColumnSorter listViewExExcludedHostsSorter;
        private System.Windows.Forms.ColumnHeader columnHeaderExclude;
        private System.Windows.Forms.ColumnHeader columnHeaderHostName;
    }
}
