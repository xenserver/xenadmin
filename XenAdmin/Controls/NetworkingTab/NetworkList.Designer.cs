namespace XenAdmin.Controls.NetworkingTab
{
    partial class NetworkList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkList));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.AddNetworkButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.EditNetworkButton = new System.Windows.Forms.Button();
            this.RemoveButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.RemoveNetworkButton = new System.Windows.Forms.Button();
            this.toolTipContainerActivateToggle = new XenAdmin.Controls.ToolTipContainer();
            this.buttonActivateToggle = new System.Windows.Forms.Button();
            this.NetworksGridView = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NicColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VlanColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AutoColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LinkStatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NetworkMacColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VifMacColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeviceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LimitColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NetworkColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IpColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActiveColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MtuColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowLayoutPanel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.EditButtonContainer.SuspendLayout();
            this.RemoveButtonContainer.SuspendLayout();
            this.toolTipContainerActivateToggle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NetworksGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.AddNetworkButton);
            this.flowLayoutPanel1.Controls.Add(this.EditButtonContainer);
            this.flowLayoutPanel1.Controls.Add(this.RemoveButtonContainer);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.toolTipContainerActivateToggle);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // AddNetworkButton
            // 
            resources.ApplyResources(this.AddNetworkButton, "AddNetworkButton");
            this.AddNetworkButton.Name = "AddNetworkButton";
            this.AddNetworkButton.UseVisualStyleBackColor = true;
            this.AddNetworkButton.Click += new System.EventHandler(this.AddNetworkButton_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.propertiesToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::XenAdmin.Properties.Resources.copy_16;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            resources.ApplyResources(this.addToolStripMenuItem, "addToolStripMenuItem");
            this.addToolStripMenuItem.Click += new System.EventHandler(this.AddMenuItemHandler);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            resources.ApplyResources(this.removeToolStripMenuItem, "removeToolStripMenuItem");
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.RemoveMenuItemHandler);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            resources.ApplyResources(this.propertiesToolStripMenuItem, "propertiesToolStripMenuItem");
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.EditMenuItemHandler);
            // 
            // EditButtonContainer
            // 
            this.EditButtonContainer.Controls.Add(this.EditNetworkButton);
            resources.ApplyResources(this.EditButtonContainer, "EditButtonContainer");
            this.EditButtonContainer.Name = "EditButtonContainer";
            // 
            // EditNetworkButton
            // 
            resources.ApplyResources(this.EditNetworkButton, "EditNetworkButton");
            this.EditNetworkButton.Name = "EditNetworkButton";
            this.EditNetworkButton.UseVisualStyleBackColor = true;
            this.EditNetworkButton.Click += new System.EventHandler(this.EditNetworkButton_Click);
            // 
            // RemoveButtonContainer
            // 
            resources.ApplyResources(this.RemoveButtonContainer, "RemoveButtonContainer");
            this.RemoveButtonContainer.Controls.Add(this.RemoveNetworkButton);
            this.RemoveButtonContainer.Name = "RemoveButtonContainer";
            // 
            // RemoveNetworkButton
            // 
            resources.ApplyResources(this.RemoveNetworkButton, "RemoveNetworkButton");
            this.RemoveNetworkButton.Name = "RemoveNetworkButton";
            this.RemoveNetworkButton.UseVisualStyleBackColor = true;
            this.RemoveNetworkButton.Click += new System.EventHandler(this.RemoveNetworkButton_Click);
            // 
            // toolTipContainerActivateToggle
            // 
            this.toolTipContainerActivateToggle.Controls.Add(this.buttonActivateToggle);
            resources.ApplyResources(this.toolTipContainerActivateToggle, "toolTipContainerActivateToggle");
            this.toolTipContainerActivateToggle.Name = "toolTipContainerActivateToggle";
            // 
            // buttonActivateToggle
            // 
            resources.ApplyResources(this.buttonActivateToggle, "buttonActivateToggle");
            this.buttonActivateToggle.Name = "buttonActivateToggle";
            this.buttonActivateToggle.UseVisualStyleBackColor = true;
            this.buttonActivateToggle.Click += new System.EventHandler(this.buttonActivateToggle_Click);
            // 
            // NetworksGridView
            // 
            this.NetworksGridView.AdjustColorsForClassic = false;
            resources.ApplyResources(this.NetworksGridView, "NetworksGridView");
            this.NetworksGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.NetworksGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.NetworksGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.NetworksGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.NetworksGridView.ContextMenuStrip = this.contextMenuStrip1;
            this.NetworksGridView.Name = "NetworksGridView";
            this.NetworksGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.NetworksGridView_CellValueChanged);
            this.NetworksGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.NetworksGridView_SortCompare);
            this.NetworksGridView.SelectionChanged += new System.EventHandler(this.NetworksGridView_SelectionChanged);
            // 
            // ImageColumn
            // 
            this.ImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ImageColumn, "ImageColumn");
            this.ImageColumn.Name = "ImageColumn";
            // 
            // NameColumn
            // 
            this.NameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.NameColumn, "NameColumn");
            this.NameColumn.Name = "NameColumn";
            // 
            // DescriptionColumn
            // 
            this.DescriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.DescriptionColumn, "DescriptionColumn");
            this.DescriptionColumn.Name = "DescriptionColumn";
            this.DescriptionColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // NicColumn
            // 
            this.NicColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.NicColumn, "NicColumn");
            this.NicColumn.Name = "NicColumn";
            // 
            // VlanColumn
            // 
            this.VlanColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.VlanColumn, "VlanColumn");
            this.VlanColumn.Name = "VlanColumn";
            // 
            // AutoColumn
            // 
            this.AutoColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.AutoColumn, "AutoColumn");
            this.AutoColumn.Name = "AutoColumn";
            // 
            // LinkStatusColumn
            // 
            this.LinkStatusColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.LinkStatusColumn, "LinkStatusColumn");
            this.LinkStatusColumn.Name = "LinkStatusColumn";
            // 
            // NetworkMacColumn
            // 
            this.NetworkMacColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.NetworkMacColumn, "NetworkMacColumn");
            this.NetworkMacColumn.Name = "NetworkMacColumn";
            // 
            // VifMacColumn
            // 
            this.VifMacColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.VifMacColumn, "VifMacColumn");
            this.VifMacColumn.Name = "VifMacColumn";
            // 
            // DeviceColumn
            // 
            this.DeviceColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.DeviceColumn, "DeviceColumn");
            this.DeviceColumn.Name = "DeviceColumn";
            // 
            // LimitColumn
            // 
            this.LimitColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.LimitColumn, "LimitColumn");
            this.LimitColumn.Name = "LimitColumn";
            // 
            // NetworkColumn
            // 
            this.NetworkColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.NetworkColumn, "NetworkColumn");
            this.NetworkColumn.Name = "NetworkColumn";
            this.NetworkColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // IpColumn
            // 
            this.IpColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.IpColumn, "IpColumn");
            this.IpColumn.Name = "IpColumn";
            this.IpColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ActiveColumn
            // 
            this.ActiveColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ActiveColumn, "ActiveColumn");
            this.ActiveColumn.Name = "ActiveColumn";
            // 
            // MtuColumn
            // 
            this.MtuColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.MtuColumn, "MtuColumn");
            this.MtuColumn.Name = "MtuColumn";
            // 
            // NetworkList
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.NetworksGridView);
            this.Name = "NetworkList";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.EditButtonContainer.ResumeLayout(false);
            this.RemoveButtonContainer.ResumeLayout(false);
            this.toolTipContainerActivateToggle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NetworksGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ToolTipContainer RemoveButtonContainer;
        private System.Windows.Forms.Button RemoveNetworkButton;
        private ToolTipContainer EditButtonContainer;
        private System.Windows.Forms.Button EditNetworkButton;
        private System.Windows.Forms.Button AddNetworkButton;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx NetworksGridView;
        private System.Windows.Forms.DataGridViewImageColumn ImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NicColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VlanColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn AutoColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LinkStatusColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NetworkMacColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VifMacColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeviceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LimitColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NetworkColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn IpColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActiveColumn;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private ToolTipContainer toolTipContainerActivateToggle;
        private System.Windows.Forms.Button buttonActivateToggle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn MtuColumn;

    }
}
