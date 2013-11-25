namespace XenAdmin.Dialogs
{
    partial class NewPoolDialog
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
                ConnectionsManager.XenConnections.CollectionChanged -= XenConnections_CollectionChanged;

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewPoolDialog));
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.tableLayoutPanelFill = new System.Windows.Forms.TableLayoutPanel();
            this.labelNameBlurb = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.labelDescriptionOptional = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.comboBoxServers = new System.Windows.Forms.ComboBox();
            this.labelMaster = new System.Windows.Forms.Label();
            this.labelSlaveListBlurb = new System.Windows.Forms.Label();
            this.customTreeViewServers = new XenAdmin.Controls.CustomTreeView();
            this.flowLayoutPanelServerListButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAddNewServer = new System.Windows.Forms.Button();
            this.flowLayoutPanelDialogButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.toolTipContainerCreate = new XenAdmin.Controls.ToolTipContainer();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.tableLayoutPanelFill.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanelServerListButtons.SuspendLayout();
            this.flowLayoutPanelDialogButtons.SuspendLayout();
            this.toolTipContainerCreate.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            // 
            // labelDescription
            // 
            resources.ApplyResources(this.labelDescription, "labelDescription");
            this.labelDescription.Name = "labelDescription";
            // 
            // tableLayoutPanelFill
            // 
            resources.ApplyResources(this.tableLayoutPanelFill, "tableLayoutPanelFill");
            this.tableLayoutPanelFill.Controls.Add(this.labelNameBlurb, 0, 0);
            this.tableLayoutPanelFill.Controls.Add(this.labelName, 0, 1);
            this.tableLayoutPanelFill.Controls.Add(this.textBoxName, 1, 1);
            this.tableLayoutPanelFill.Controls.Add(this.labelDescription, 0, 3);
            this.tableLayoutPanelFill.Controls.Add(this.textBoxDescription, 1, 3);
            this.tableLayoutPanelFill.Controls.Add(this.labelDescriptionOptional, 2, 3);
            this.tableLayoutPanelFill.Name = "tableLayoutPanelFill";
            // 
            // labelNameBlurb
            // 
            resources.ApplyResources(this.labelNameBlurb, "labelNameBlurb");
            this.tableLayoutPanelFill.SetColumnSpan(this.labelNameBlurb, 3);
            this.labelNameBlurb.Name = "labelNameBlurb";
            // 
            // textBoxName
            // 
            resources.ApplyResources(this.textBoxName, "textBoxName");
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // labelName
            // 
            resources.ApplyResources(this.labelName, "labelName");
            this.labelName.Name = "labelName";
            // 
            // labelDescriptionOptional
            // 
            resources.ApplyResources(this.labelDescriptionOptional, "labelDescriptionOptional");
            this.labelDescriptionOptional.Name = "labelDescriptionOptional";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.comboBoxServers, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelMaster, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelSlaveListBlurb, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.customTreeViewServers, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanelServerListButtons, 0, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // comboBoxServers
            // 
            this.comboBoxServers.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxServers.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            resources.ApplyResources(this.comboBoxServers, "comboBoxServers");
            this.comboBoxServers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxServers.FormattingEnabled = true;
            this.comboBoxServers.Items.AddRange(new object[] {
            resources.GetString("comboBoxServers.Items"),
            resources.GetString("comboBoxServers.Items1"),
            resources.GetString("comboBoxServers.Items2"),
            resources.GetString("comboBoxServers.Items3")});
            this.comboBoxServers.Name = "comboBoxServers";
            this.comboBoxServers.Sorted = true;
            this.comboBoxServers.SelectionChangeCommitted += new System.EventHandler(this.comboBoxServers_SelectionChangeCommitted);
            this.comboBoxServers.SelectedIndexChanged += new System.EventHandler(this.comboBoxServers_SelectedIndexChanged);
            // 
            // labelMaster
            // 
            resources.ApplyResources(this.labelMaster, "labelMaster");
            this.labelMaster.Name = "labelMaster";
            // 
            // labelSlaveListBlurb
            // 
            resources.ApplyResources(this.labelSlaveListBlurb, "labelSlaveListBlurb");
            this.tableLayoutPanel1.SetColumnSpan(this.labelSlaveListBlurb, 2);
            this.labelSlaveListBlurb.Name = "labelSlaveListBlurb";
            // 
            // customTreeViewServers
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.customTreeViewServers, 2);
            resources.ApplyResources(this.customTreeViewServers, "customTreeViewServers");
            this.customTreeViewServers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.customTreeViewServers.FormattingEnabled = true;
            this.customTreeViewServers.Name = "customTreeViewServers";
            this.customTreeViewServers.NodeIndent = 19;
            this.customTreeViewServers.RootAlwaysExpanded = false;
            this.customTreeViewServers.ShowCheckboxes = true;
            this.customTreeViewServers.ShowDescription = true;
            this.customTreeViewServers.ShowImages = false;
            this.customTreeViewServers.ShowRootLines = true;
            // 
            // flowLayoutPanelServerListButtons
            // 
            resources.ApplyResources(this.flowLayoutPanelServerListButtons, "flowLayoutPanelServerListButtons");
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanelServerListButtons, 2);
            this.flowLayoutPanelServerListButtons.Controls.Add(this.buttonAddNewServer);
            this.flowLayoutPanelServerListButtons.Name = "flowLayoutPanelServerListButtons";
            // 
            // buttonAddNewServer
            // 
            resources.ApplyResources(this.buttonAddNewServer, "buttonAddNewServer");
            this.buttonAddNewServer.Image = global::XenAdmin.Properties.Resources._000_AddApplicationServer_h32bit_16;
            this.buttonAddNewServer.Name = "buttonAddNewServer";
            this.buttonAddNewServer.UseVisualStyleBackColor = true;
            this.buttonAddNewServer.Click += new System.EventHandler(this.buttonAddNewServer_Click);
            // 
            // flowLayoutPanelDialogButtons
            // 
            resources.ApplyResources(this.flowLayoutPanelDialogButtons, "flowLayoutPanelDialogButtons");
            this.flowLayoutPanelDialogButtons.Controls.Add(this.buttonCancel);
            this.flowLayoutPanelDialogButtons.Controls.Add(this.toolTipContainerCreate);
            this.flowLayoutPanelDialogButtons.Name = "flowLayoutPanelDialogButtons";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // toolTipContainerCreate
            // 
            this.toolTipContainerCreate.Controls.Add(this.buttonCreate);
            resources.ApplyResources(this.toolTipContainerCreate, "toolTipContainerCreate");
            this.toolTipContainerCreate.Name = "toolTipContainerCreate";
            // 
            // buttonCreate
            // 
            resources.ApplyResources(this.buttonCreate, "buttonCreate");
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // NewPoolDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.flowLayoutPanelDialogButtons);
            this.Controls.Add(this.tableLayoutPanelFill);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "NewPoolDialog";
            this.tableLayoutPanelFill.ResumeLayout(false);
            this.tableLayoutPanelFill.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanelServerListButtons.ResumeLayout(false);
            this.flowLayoutPanelServerListButtons.PerformLayout();
            this.flowLayoutPanelDialogButtons.ResumeLayout(false);
            this.toolTipContainerCreate.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelFill;
        public System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelDescriptionOptional;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public XenAdmin.Controls.CustomTreeView customTreeViewServers;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelServerListButtons;
        public System.Windows.Forms.Button buttonAddNewServer;
        public System.Windows.Forms.ComboBox comboBoxServers;
        private System.Windows.Forms.Label labelMaster;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelDialogButtons;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Label labelSlaveListBlurb;
        private System.Windows.Forms.Label labelNameBlurb;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerCreate;
    }
}