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
            this.buttonEditSearch = new System.Windows.Forms.Button();
            this.ddButtonSavedSearches = new XenAdmin.Controls.DropDownButton();
            this.contextMenuStripSearches = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.OutputPanel = new XenAdmin.Controls.XenSearch.SearchOutput();
            this.Searcher = new XenAdmin.Controls.XenSearch.Searcher();
            this.buttonNewSearch = new System.Windows.Forms.Button();
            this.pageContainerPanel.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
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
            this.panel2.Controls.Add(this.buttonNewSearch);
            this.panel2.Controls.Add(this.buttonEditSearch);
            this.panel2.Controls.Add(this.ddButtonSavedSearches);
            this.panel2.Controls.Add(this.buttonExport);
            this.panel2.Controls.Add(this.buttonImport);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // buttonEditSearch
            // 
            resources.ApplyResources(this.buttonEditSearch, "buttonEditSearch");
            this.buttonEditSearch.Name = "buttonEditSearch";
            this.buttonEditSearch.UseVisualStyleBackColor = true;
            this.buttonEditSearch.Click += new System.EventHandler(this.buttonEditSearch_Click);
            // 
            // ddButtonSavedSearches
            // 
            this.ddButtonSavedSearches.ContextMenuStrip = this.contextMenuStripSearches;
            resources.ApplyResources(this.ddButtonSavedSearches, "ddButtonSavedSearches");
            this.ddButtonSavedSearches.Name = "ddButtonSavedSearches";
            this.ddButtonSavedSearches.UseVisualStyleBackColor = true;
            // 
            // contextMenuStripSearches
            // 
            this.contextMenuStripSearches.Name = "contextMenuStripSearches";
            resources.ApplyResources(this.contextMenuStripSearches, "contextMenuStripSearches");
            this.contextMenuStripSearches.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripSearches_Opening);
            // 
            // buttonExport
            // 
            resources.ApplyResources(this.buttonExport, "buttonExport");
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonImport
            // 
            resources.ApplyResources(this.buttonImport, "buttonImport");
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
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
            // buttonNewSearch
            // 
            resources.ApplyResources(this.buttonNewSearch, "buttonNewSearch");
            this.buttonNewSearch.Name = "buttonNewSearch";
            this.buttonNewSearch.UseVisualStyleBackColor = true;
            this.buttonNewSearch.Click += new System.EventHandler(this.buttonNewSearch_Click);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private XenAdmin.Controls.XenSearch.SearchOutput OutputPanel;
        private XenAdmin.Controls.XenSearch.Searcher Searcher;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private XenAdmin.Controls.DropDownButton ddButtonSavedSearches;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonEditSearch;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripSearches;
        private System.Windows.Forms.Button buttonNewSearch;
    }
}
