namespace XenAdmin.Controls.XenSearch
{
    partial class Searcher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Searcher));
            XenAdmin.XenSearch.DummyQuery dummyQuery1 = new XenAdmin.XenSearch.DummyQuery();
            this.GroupsLabel = new System.Windows.Forms.Label();
            this.FiltersLabel = new System.Windows.Forms.Label();
            this.SearchLabel = new System.Windows.Forms.Label();
            this.searchFor = new XenAdmin.Controls.XenSearch.SearchFor();
            this.GroupingControl = new XenAdmin.Controls.XenSearch.GroupingControl();
            this.QueryElement = new XenAdmin.Controls.XenSearch.QueryElement();
            this.SuspendLayout();
            // 
            // GroupsLabel
            // 
            resources.ApplyResources(this.GroupsLabel, "GroupsLabel");
            this.GroupsLabel.Name = "GroupsLabel";
            // 
            // FiltersLabel
            // 
            resources.ApplyResources(this.FiltersLabel, "FiltersLabel");
            this.FiltersLabel.Name = "FiltersLabel";
            // 
            // SearchLabel
            // 
            resources.ApplyResources(this.SearchLabel, "SearchLabel");
            this.SearchLabel.Name = "SearchLabel";
            // 
            // searchFor
            // 
            resources.ApplyResources(this.searchFor, "searchFor");
            this.searchFor.Name = "searchFor";
            // 
            // GroupingControl
            // 
            resources.ApplyResources(this.GroupingControl, "GroupingControl");
            this.GroupingControl.BackColor = System.Drawing.Color.Transparent;
            this.GroupingControl.Name = "GroupingControl";
            this.GroupingControl.Searcher = null;
            // 
            // QueryElement
            // 
            resources.ApplyResources(this.QueryElement, "QueryElement");
            this.QueryElement.BackColor = System.Drawing.Color.Transparent;
            this.QueryElement.Name = "QueryElement";
            this.QueryElement.QueryFilter = dummyQuery1;
            this.QueryElement.Searcher = null;
            this.QueryElement.Resize += new System.EventHandler(this.QueryElement_Resize);
            // 
            // Searcher
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.searchFor);
            this.Controls.Add(this.SearchLabel);
            this.Controls.Add(this.FiltersLabel);
            this.Controls.Add(this.GroupsLabel);
            this.Controls.Add(this.GroupingControl);
            this.Controls.Add(this.QueryElement);
            this.Name = "Searcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupingControl GroupingControl;
        private System.Windows.Forms.Label GroupsLabel;
        private System.Windows.Forms.Label FiltersLabel;
        internal QueryElement QueryElement;
        private System.Windows.Forms.Label SearchLabel;
        private SearchFor searchFor;
    }
}