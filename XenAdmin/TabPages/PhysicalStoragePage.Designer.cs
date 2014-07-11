namespace XenAdmin.TabPages
{
    partial class PhysicalStoragePage
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
            Host = null;
            Connection = null;

            if (disposing)
            {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhysicalStoragePage));
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.listViewSrs = new XenAdmin.Controls.ListViewEx();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelNetworkheadings = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.newSRButton = new XenAdmin.Commands.CommandButton();
            this.trimButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.trimButton = new XenAdmin.Commands.CommandButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonProperties = new System.Windows.Forms.Button();
            this.pageContainerPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.trimButtonContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.panel1);
            this.pageContainerPanel.Controls.Add(this.labelNetworkheadings);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // listViewSrs
            // 
            this.listViewSrs.AllowColumnReorder = true;
            this.listViewSrs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.listViewSrs.ContextMenuStrip = this.contextMenuStrip;
            resources.ApplyResources(this.listViewSrs, "listViewSrs");
            this.listViewSrs.FullRowSelect = true;
            this.listViewSrs.HideSelection = false;
            this.listViewSrs.Name = "listViewSrs";
            this.listViewSrs.UseCompatibleStateImageBehavior = false;
            this.listViewSrs.View = System.Windows.Forms.View.Details;
            this.listViewSrs.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewSrs_ColumnClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // columnHeader6
            // 
            resources.ApplyResources(this.columnHeader6, "columnHeader6");
            // 
            // columnHeader7
            // 
            resources.ApplyResources(this.columnHeader7, "columnHeader7");
            // 
            // labelNetworkheadings
            // 
            resources.ApplyResources(this.labelNetworkheadings, "labelNetworkheadings");
            this.labelNetworkheadings.Name = "labelNetworkheadings";
            // 
            // TitleLabel
            // 
            resources.ApplyResources(this.TitleLabel, "TitleLabel");
            this.TitleLabel.ForeColor = System.Drawing.Color.White;
            this.TitleLabel.Name = "TitleLabel";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.listViewSrs);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Name = "panel1";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.newSRButton);
            this.flowLayoutPanel1.Controls.Add(this.trimButtonContainer);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.buttonProperties);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // newSRButton
            // 
            this.newSRButton.Command = new XenAdmin.Commands.NewSRCommand();
            resources.ApplyResources(this.newSRButton, "newSRButton");
            this.newSRButton.Name = "newSRButton";
            this.newSRButton.UseVisualStyleBackColor = true;
            // 
            // trimButtonContainer
            // 
            this.trimButtonContainer.Controls.Add(this.trimButton);
            resources.ApplyResources(this.trimButtonContainer, "trimButtonContainer");
            this.trimButtonContainer.Name = "trimButtonContainer";
            // 
            // trimButton
            // 
            this.trimButton.Command = new XenAdmin.Commands.TrimSRCommand();
            resources.ApplyResources(this.trimButton, "trimButton");
            this.trimButton.Name = "trimButton";
            this.trimButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonProperties
            // 
            resources.ApplyResources(this.buttonProperties, "buttonProperties");
            this.buttonProperties.Name = "buttonProperties";
            this.buttonProperties.UseVisualStyleBackColor = true;
            this.buttonProperties.Click += new System.EventHandler(this.buttonProperties_Click);
            // 
            // PhysicalStoragePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "PhysicalStoragePage";
            this.Controls.SetChildIndex(this.pageContainerPanel, 0);
            this.pageContainerPanel.ResumeLayout(false);
            this.pageContainerPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.trimButtonContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TitleLabel;
        private XenAdmin.Controls.ListViewEx listViewSrs;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Label labelNetworkheadings;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonProperties;
        private System.Windows.Forms.GroupBox groupBox1;
        private XenAdmin.Commands.CommandButton newSRButton;
        private XenAdmin.Commands.CommandButton trimButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Controls.ToolTipContainer trimButtonContainer;
    }
}
