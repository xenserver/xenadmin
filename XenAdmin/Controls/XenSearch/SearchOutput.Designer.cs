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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchOutput));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.contextMenuStripColumns = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.folderNavigator = new XenAdmin.Controls.XenSearch.FolderNavigator();
            this.queryPanel = new XenAdmin.Controls.XenSearch.QueryPanel();
            this.buttonColumns = new XenAdmin.Controls.DropDownButton();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.folderNavigator, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.queryPanel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.buttonColumns, 0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // contextMenuStripColumns
            // 
            this.contextMenuStripColumns.Name = "contextMenuStripColumns";
            resources.ApplyResources(this.contextMenuStripColumns, "contextMenuStripColumns");
            this.contextMenuStripColumns.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripColumns_Opening);
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
            // buttonColumns
            // 
            resources.ApplyResources(this.buttonColumns, "buttonColumns");
            this.buttonColumns.ContextMenuStrip = this.contextMenuStripColumns;
            this.buttonColumns.Name = "buttonColumns";
            this.buttonColumns.UseVisualStyleBackColor = true;
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
        private XenAdmin.Controls.DropDownButton buttonColumns;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripColumns;
    }
}