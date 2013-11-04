namespace XenAdmin.Wizards.BugToolWizardFiles
{
    partial class GenericSelectHostsPage
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
            ConnectionsManager.XenConnections.CollectionChanged -= XenConnections_CollectionChanged;
            DeregisterEvents();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenericSelectHostsPage));
            this.HostListTreeView = new XenAdmin.Controls.CustomTreeView();
            this.SelectNoneButton = new System.Windows.Forms.Button();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.connectbutton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // HostListTreeView
            // 
            resources.ApplyResources(this.HostListTreeView, "HostListTreeView");
            this.tableLayoutPanel1.SetColumnSpan(this.HostListTreeView, 3);
            this.HostListTreeView.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.HostListTreeView.Name = "HostListTreeView";
            this.HostListTreeView.NodeIndent = 19;
            this.HostListTreeView.RootAlwaysExpanded = false;
            this.HostListTreeView.ShowCheckboxes = true;
            this.HostListTreeView.ShowDescription = true;
            this.HostListTreeView.ShowImages = false;
            this.HostListTreeView.ShowRootLines = true;
            // 
            // SelectNoneButton
            // 
            resources.ApplyResources(this.SelectNoneButton, "SelectNoneButton");
            this.SelectNoneButton.Name = "SelectNoneButton";
            this.SelectNoneButton.UseVisualStyleBackColor = true;
            this.SelectNoneButton.Click += new System.EventHandler(this.SelectNoneButton_Click);
            // 
            // SelectAllButton
            // 
            resources.ApplyResources(this.SelectAllButton, "SelectAllButton");
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // connectbutton
            // 
            resources.ApplyResources(this.connectbutton, "connectbutton");
            this.connectbutton.Image = global::XenAdmin.Properties.Resources._000_AddApplicationServer_h32bit_16;
            this.connectbutton.Name = "connectbutton";
            this.connectbutton.UseVisualStyleBackColor = true;
            this.connectbutton.Click += new System.EventHandler(this.connectbutton_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.autoHeightLabel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.HostListTreeView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.connectbutton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.SelectAllButton, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.SelectNoneButton, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.tableLayoutPanel1.SetColumnSpan(this.autoHeightLabel1, 3);
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // GenericSelectHostsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "GenericSelectHostsPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SelectNoneButton;
        private System.Windows.Forms.Button SelectAllButton;
        private System.Windows.Forms.Button connectbutton;
        private XenAdmin.Controls.CustomTreeView HostListTreeView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
    }
}
