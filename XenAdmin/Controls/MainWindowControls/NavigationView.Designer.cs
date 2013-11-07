namespace XenAdmin.Controls.MainWindowControls
{
    partial class NavigationView
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
            if (disposing)
            {
                treeViewUpdateManager.Update -= treeViewUpdateManager_Update;

                if (treeViewUpdateManager != null)
                    treeViewUpdateManager.Dispose();

                if (selectionManager != null)
                    selectionManager.Dispose();

                if (components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NavigationView));
            this.TreeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.searchTextBox = new XenAdmin.Controls.SearchTextBox();
            this.treeView = new XenAdmin.Controls.FlickerFreeTreeView();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TreeContextMenu
            // 
            this.TreeContextMenu.Name = "TreeContextMenu";
            resources.ApplyResources(this.TreeContextMenu, "TreeContextMenu");
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.treeView);
            this.panel1.Name = "panel1";
            // 
            // searchTextBox
            // 
            resources.ApplyResources(this.searchTextBox, "searchTextBox");
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // treeView
            // 
            this.treeView.AllowDrop = true;
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.HideSelection = false;
            this.treeView.HScrollPos = 0;
            this.treeView.Name = "treeView";
            this.treeView.SelectionMode = XenAdmin.Controls.TreeViewSelectionMode.MultiSelect;
            this.treeView.ShowLines = false;
            this.treeView.ShowNodeToolTips = true;
            this.treeView.NodeMouseDoubleClick += new System.EventHandler<XenAdmin.Controls.VirtualTreeNodeMouseClickEventArgs>(this.treeView_NodeMouseDoubleClick);
            this.treeView.DragLeave += new System.EventHandler(this.treeView_DragLeave);
            this.treeView.AfterLabelEdit += new System.EventHandler<XenAdmin.Controls.VirtualNodeLabelEditEventArgs>(this.treeView_AfterLabelEdit);
            this.treeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView_DragEnter);
            this.treeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyUp);
            this.treeView.NodeMouseClick += new System.EventHandler<XenAdmin.Controls.VirtualTreeNodeMouseClickEventArgs>(this.treeView_NodeMouseClick);
            this.treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
            this.treeView.BeforeSelect += new System.EventHandler<XenAdmin.Controls.VirtualTreeViewCancelEventArgs>(this.treeView_BeforeSelect);
            this.treeView.ItemDrag += new System.EventHandler<XenAdmin.Controls.VirtualTreeViewItemDragEventArgs>(this.treeView_ItemDrag);
            this.treeView.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
            this.treeView.SelectionsChanged += new System.EventHandler(this.treeView_SelectionsChanged);
            // 
            // NavigationView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.searchTextBox);
            this.Controls.Add(this.panel1);
            this.Name = "NavigationView";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SearchTextBox searchTextBox;
        private FlickerFreeTreeView treeView;
        private System.Windows.Forms.ContextMenuStrip TreeContextMenu;
        private System.Windows.Forms.Panel panel1;
    }
}
