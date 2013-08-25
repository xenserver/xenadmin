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
            this.searchTextBox = new XenAdmin.Controls.SearchTextBox();
            this.treeView = new XenAdmin.Controls.FlickerFreeTreeView();
            this.TreeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // searchTextBox
            // 
            this.searchTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchTextBox.Location = new System.Drawing.Point(0, 0);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(210, 23);
            this.searchTextBox.TabIndex = 0;
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // treeView
            // 
            this.treeView.AllowDrop = true;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.HideSelection = false;
            this.treeView.HScrollPos = 0;
            this.treeView.Location = new System.Drawing.Point(0, 23);
            this.treeView.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectionMode = XenAdmin.Controls.TreeViewSelectionMode.MultiSelect;
            this.treeView.ShowLines = false;
            this.treeView.ShowNodeToolTips = true;
            this.treeView.Size = new System.Drawing.Size(210, 265);
            this.treeView.TabIndex = 1;
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
            // TreeContextMenu
            // 
            this.TreeContextMenu.Name = "TreeContextMenu";
            this.TreeContextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // NavigationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.searchTextBox);
            this.Name = "NavigationView";
            this.Size = new System.Drawing.Size(210, 288);
            this.ResumeLayout(false);

        }

        #endregion

        private SearchTextBox searchTextBox;
        private FlickerFreeTreeView treeView;
        private System.Windows.Forms.ContextMenuStrip TreeContextMenu;
    }
}
