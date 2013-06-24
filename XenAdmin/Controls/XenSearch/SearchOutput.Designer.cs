namespace XenAdmin.Controls.XenSearch
{
    partial class SearchOutput
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchOutput));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.folderNavigator = new XenAdmin.Controls.XenSearch.FolderNavigator();
            this.queryPanel = new XenAdmin.Controls.XenSearch.QueryPanel();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.folderNavigator, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.queryPanel, 0, 1);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // folderNavigator
            // 
            this.folderNavigator.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.folderNavigator, "folderNavigator");
            this.folderNavigator.Name = "folderNavigator";
            // 
            // queryPanel
            // 
            this.queryPanel.AllowDrop = true;
            resources.ApplyResources(this.queryPanel, "queryPanel");
            this.queryPanel.HasLeftExpanders = false;
            this.queryPanel.MinimumSize = new System.Drawing.Size(1, 1);
            this.queryPanel.Name = "queryPanel";
            this.queryPanel.Sorting = new XenAdmin.XenSearch.Sort[0];
            // 
            // SearchOutput
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "SearchOutput";
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private FolderNavigator folderNavigator;
        private QueryPanel queryPanel;



    }
}