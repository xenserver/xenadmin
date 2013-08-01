namespace XenAdmin.TabPages
{
    partial class SearchPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchPage));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SearchButton = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.OutputPanel = new XenAdmin.Controls.XenSearch.SearchOutput();
            this.Searcher = new XenAdmin.Controls.XenSearch.Searcher();
            this.searchOptionsMenuStrip = new XenAdmin.Controls.NonReopeningContextMenuStrip(this.components);
            this.editSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.applySavedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSavedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exportSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pageContainerPanel.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.searchOptionsMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.tableLayoutPanel);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.panel4, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.Searcher, 0, 1);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.Controls.Add(this.SearchButton);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // SearchButton
            // 
            this.SearchButton.Image = global::XenAdmin.Properties.Resources.expanded_triangle;
            resources.ApplyResources(this.SearchButton, "SearchButton");
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Window;
            this.panel4.Controls.Add(this.OutputPanel);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // OutputPanel
            // 
            resources.ApplyResources(this.OutputPanel, "OutputPanel");
            this.OutputPanel.Name = "OutputPanel";
            // 
            // Searcher
            // 
            resources.ApplyResources(this.Searcher, "Searcher");
            this.Searcher.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Searcher.MaxHeight = 400;
            this.Searcher.Name = "Searcher";
            // 
            // searchOptionsMenuStrip
            // 
            this.searchOptionsMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editSearchToolStripMenuItem,
            this.resetSearchToolStripMenuItem,
            this.toolStripSeparator1,
            this.applySavedToolStripMenuItem,
            this.deleteSavedToolStripMenuItem,
            this.toolStripSeparator2,
            this.exportSearchToolStripMenuItem,
            this.importSearchToolStripMenuItem});
            this.searchOptionsMenuStrip.Name = "searchOptionsMenuStrip";
            resources.ApplyResources(this.searchOptionsMenuStrip, "searchOptionsMenuStrip");
            this.searchOptionsMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.searchOptionsMenuStrip_Opening);
            // 
            // editSearchToolStripMenuItem
            // 
            this.editSearchToolStripMenuItem.Name = "editSearchToolStripMenuItem";
            resources.ApplyResources(this.editSearchToolStripMenuItem, "editSearchToolStripMenuItem");
            this.editSearchToolStripMenuItem.Click += new System.EventHandler(this.editSearchToolStripMenuItem_Click);
            // 
            // resetSearchToolStripMenuItem
            // 
            this.resetSearchToolStripMenuItem.Name = "resetSearchToolStripMenuItem";
            resources.ApplyResources(this.resetSearchToolStripMenuItem, "resetSearchToolStripMenuItem");
            this.resetSearchToolStripMenuItem.Click += new System.EventHandler(this.resetSearchToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // applySavedToolStripMenuItem
            // 
            this.applySavedToolStripMenuItem.Name = "applySavedToolStripMenuItem";
            resources.ApplyResources(this.applySavedToolStripMenuItem, "applySavedToolStripMenuItem");
            // 
            // deleteSavedToolStripMenuItem
            // 
            this.deleteSavedToolStripMenuItem.Name = "deleteSavedToolStripMenuItem";
            resources.ApplyResources(this.deleteSavedToolStripMenuItem, "deleteSavedToolStripMenuItem");
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // exportSearchToolStripMenuItem
            // 
            this.exportSearchToolStripMenuItem.Name = "exportSearchToolStripMenuItem";
            resources.ApplyResources(this.exportSearchToolStripMenuItem, "exportSearchToolStripMenuItem");
            this.exportSearchToolStripMenuItem.Click += new System.EventHandler(this.exportSearchToolStripMenuItem_Click);
            // 
            // importSearchToolStripMenuItem
            // 
            this.importSearchToolStripMenuItem.Name = "importSearchToolStripMenuItem";
            resources.ApplyResources(this.importSearchToolStripMenuItem, "importSearchToolStripMenuItem");
            this.importSearchToolStripMenuItem.Click += new System.EventHandler(this.importSearchToolStripMenuItem_Click);
            // 
            // SearchPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.DoubleBuffered = true;
            this.Name = "SearchPage";
            this.Controls.SetChildIndex(this.pageContainerPanel, 0);
            this.pageContainerPanel.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.searchOptionsMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem resetSearchToolStripMenuItem;
        internal XenAdmin.Controls.NonReopeningContextMenuStrip searchOptionsMenuStrip;
        internal System.Windows.Forms.ToolStripMenuItem editSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private XenAdmin.Controls.XenSearch.SearchOutput OutputPanel;
        public XenAdmin.Controls.XenSearch.Searcher Searcher;
        private System.Windows.Forms.ToolStripMenuItem applySavedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSavedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Panel panel4;
    }
}
