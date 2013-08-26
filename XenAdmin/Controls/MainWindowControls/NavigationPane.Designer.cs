namespace XenAdmin.Controls.MainWindowControls
{
    partial class NavigationPane
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
            this.splitContainer1 = new XenAdmin.Controls.Common.SmoothSplitContainer();
            this.toolStripBig = new XenAdmin.Controls.MainWindowControls.NavigationToolStripBig();
            this.buttonInfraBig = new XenAdmin.Controls.MainWindowControls.NavigationButtonBig();
            this.buttonObjectsBig = new XenAdmin.Controls.MainWindowControls.NavigationButtonBig();
            this.buttonTagsBig = new XenAdmin.Controls.MainWindowControls.NavigationButtonBig();
            this.buttonSearchesBig = new XenAdmin.Controls.MainWindowControls.NavigationDropDownButtonBig();
            this.buttonNotifyBig = new XenAdmin.Controls.MainWindowControls.NavigationButtonBig();
            this.toolStripSmall = new XenAdmin.Controls.MainWindowControls.NavigationToolStripSmall();
            this.buttonNotifySmall = new XenAdmin.Controls.MainWindowControls.NavigationButtonSmall();
            this.buttonSearchesSmall = new XenAdmin.Controls.MainWindowControls.NavigationDropDownButtonSmall();
            this.buttonTagsSmall = new XenAdmin.Controls.MainWindowControls.NavigationButtonSmall();
            this.buttonObjectsSmall = new XenAdmin.Controls.MainWindowControls.NavigationButtonSmall();
            this.buttonInfraSmall = new XenAdmin.Controls.MainWindowControls.NavigationButtonSmall();
            this.navigationView = new XenAdmin.Controls.MainWindowControls.NavigationView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStripBig.SuspendLayout();
            this.toolStripSmall.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.navigationView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.toolStripBig);
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(215, 401);
            this.splitContainer1.SplitterDistance = 229;
            this.splitContainer1.TabIndex = 4;
            // 
            // toolStripBig
            // 
            this.toolStripBig.AutoSize = false;
            this.toolStripBig.ClickThrough = true;
            this.toolStripBig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripBig.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripBig.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonInfraBig,
            this.buttonObjectsBig,
            this.buttonTagsBig,
            this.buttonSearchesBig,
            this.buttonNotifyBig});
            this.toolStripBig.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStripBig.Location = new System.Drawing.Point(0, 0);
            this.toolStripBig.MaximumSize = new System.Drawing.Size(0, 157);
            this.toolStripBig.MinimumSize = new System.Drawing.Size(215, 0);
            this.toolStripBig.Name = "toolStripBig";
            this.toolStripBig.Size = new System.Drawing.Size(215, 157);
            this.toolStripBig.TabIndex = 0;
            this.toolStripBig.Text = "toolStrip1";
            this.toolStripBig.LayoutCompleted += new System.EventHandler(this.toolStripBig_LayoutCompleted);
            // 
            // buttonInfraBig
            // 
            this.buttonInfraBig.CheckOnClick = true;
            this.buttonInfraBig.Image = global::XenAdmin.Properties.Resources.tempInfra24;
            this.buttonInfraBig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonInfraBig.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonInfraBig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonInfraBig.Name = "buttonInfraBig";
            this.buttonInfraBig.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonInfraBig.PairedItem = null;
            this.buttonInfraBig.Size = new System.Drawing.Size(213, 28);
            this.buttonInfraBig.Text = "Infrastructure";
            this.buttonInfraBig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonObjectsBig
            // 
            this.buttonObjectsBig.CheckOnClick = true;
            this.buttonObjectsBig.Image = global::XenAdmin.Properties.Resources.tempObj24;
            this.buttonObjectsBig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonObjectsBig.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonObjectsBig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonObjectsBig.Name = "buttonObjectsBig";
            this.buttonObjectsBig.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonObjectsBig.PairedItem = null;
            this.buttonObjectsBig.Size = new System.Drawing.Size(213, 28);
            this.buttonObjectsBig.Text = "Objects";
            this.buttonObjectsBig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonTagsBig
            // 
            this.buttonTagsBig.CheckOnClick = true;
            this.buttonTagsBig.Image = global::XenAdmin.Properties.Resources.tempTags24;
            this.buttonTagsBig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonTagsBig.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonTagsBig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonTagsBig.Name = "buttonTagsBig";
            this.buttonTagsBig.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonTagsBig.PairedItem = null;
            this.buttonTagsBig.Size = new System.Drawing.Size(213, 28);
            this.buttonTagsBig.Text = "Tags, Folders && Custom Fields";
            this.buttonTagsBig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonSearchesBig
            // 
            this.buttonSearchesBig.Image = global::XenAdmin.Properties.Resources.tempSearch24;
            this.buttonSearchesBig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSearchesBig.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonSearchesBig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSearchesBig.Name = "buttonSearchesBig";
            this.buttonSearchesBig.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonSearchesBig.PairedItem = null;
            this.buttonSearchesBig.Size = new System.Drawing.Size(213, 28);
            this.buttonSearchesBig.Text = "Saved Searches";
            this.buttonSearchesBig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonNotifyBig
            // 
            this.buttonNotifyBig.CheckOnClick = true;
            this.buttonNotifyBig.Image = global::XenAdmin.Properties.Resources.tempNotif24;
            this.buttonNotifyBig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonNotifyBig.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonNotifyBig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNotifyBig.Name = "buttonNotifyBig";
            this.buttonNotifyBig.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonNotifyBig.PairedItem = null;
            this.buttonNotifyBig.Size = new System.Drawing.Size(213, 28);
            this.buttonNotifyBig.Text = "Notifications";
            this.buttonNotifyBig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripSmall
            // 
            this.toolStripSmall.CanOverflow = false;
            this.toolStripSmall.ClickThrough = true;
            this.toolStripSmall.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStripSmall.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripSmall.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonNotifySmall,
            this.buttonSearchesSmall,
            this.buttonTagsSmall,
            this.buttonObjectsSmall,
            this.buttonInfraSmall});
            this.toolStripSmall.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStripSmall.Location = new System.Drawing.Point(0, 401);
            this.toolStripSmall.Name = "toolStripSmall";
            this.toolStripSmall.Size = new System.Drawing.Size(215, 25);
            this.toolStripSmall.TabIndex = 1;
            this.toolStripSmall.Text = "toolStrip1";
            // 
            // buttonNotifySmall
            // 
            this.buttonNotifySmall.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonNotifySmall.CheckOnClick = true;
            this.buttonNotifySmall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNotifySmall.Image = global::XenAdmin.Properties.Resources.tempNotif16;
            this.buttonNotifySmall.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonNotifySmall.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNotifySmall.Name = "buttonNotifySmall";
            this.buttonNotifySmall.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonNotifySmall.PairedItem = null;
            this.buttonNotifySmall.Size = new System.Drawing.Size(23, 22);
            this.buttonNotifySmall.Text = "toolStripButton1";
            // 
            // buttonSearchesSmall
            // 
            this.buttonSearchesSmall.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonSearchesSmall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSearchesSmall.Image = global::XenAdmin.Properties.Resources.tempSearch16;
            this.buttonSearchesSmall.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonSearchesSmall.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSearchesSmall.Name = "buttonSearchesSmall";
            this.buttonSearchesSmall.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonSearchesSmall.PairedItem = null;
            this.buttonSearchesSmall.Size = new System.Drawing.Size(29, 22);
            this.buttonSearchesSmall.Text = "toolStripButton1";
            // 
            // buttonTagsSmall
            // 
            this.buttonTagsSmall.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonTagsSmall.CheckOnClick = true;
            this.buttonTagsSmall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonTagsSmall.Image = global::XenAdmin.Properties.Resources.tempTags16;
            this.buttonTagsSmall.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonTagsSmall.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonTagsSmall.Name = "buttonTagsSmall";
            this.buttonTagsSmall.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonTagsSmall.PairedItem = null;
            this.buttonTagsSmall.Size = new System.Drawing.Size(23, 22);
            this.buttonTagsSmall.Text = "toolStripButton1";
            // 
            // buttonObjectsSmall
            // 
            this.buttonObjectsSmall.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonObjectsSmall.CheckOnClick = true;
            this.buttonObjectsSmall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonObjectsSmall.Image = global::XenAdmin.Properties.Resources.tempObj16;
            this.buttonObjectsSmall.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonObjectsSmall.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonObjectsSmall.Name = "buttonObjectsSmall";
            this.buttonObjectsSmall.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonObjectsSmall.PairedItem = null;
            this.buttonObjectsSmall.Size = new System.Drawing.Size(23, 22);
            this.buttonObjectsSmall.Text = "toolStripButton1";
            // 
            // buttonInfraSmall
            // 
            this.buttonInfraSmall.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonInfraSmall.CheckOnClick = true;
            this.buttonInfraSmall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonInfraSmall.Image = global::XenAdmin.Properties.Resources.tempInfra16;
            this.buttonInfraSmall.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonInfraSmall.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonInfraSmall.Name = "buttonInfraSmall";
            this.buttonInfraSmall.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonInfraSmall.PairedItem = null;
            this.buttonInfraSmall.Size = new System.Drawing.Size(23, 22);
            this.buttonInfraSmall.Text = "toolStripButton1";
            // 
            // navigationView
            // 
            this.navigationView.CurrentSearch = null;
            this.navigationView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigationView.Location = new System.Drawing.Point(0, 0);
            this.navigationView.Name = "navigationView";
            this.navigationView.Size = new System.Drawing.Size(213, 227);
            this.navigationView.TabIndex = 3;
            this.navigationView.TreeViewRefreshResumed += new System.Action(this.navigationView_TreeViewRefreshResumed);
            this.navigationView.TreeViewRefreshSuspended += new System.Action(this.navigationView_TreeViewRefreshSuspended);
            this.navigationView.TreeViewSelectionChanged += new System.Action(this.navigationView_TreeViewSelectionChanged);
            this.navigationView.TreeNodeClicked += new System.Action(this.navigationView_TreeNodeClicked);
            this.navigationView.TreeNodeRightClicked += new System.Action(this.navigationView_TreeNodeRightClicked);
            this.navigationView.TreeNodeBeforeSelected += new System.Action(this.navigationView_TreeNodeBeforeSelected);
            this.navigationView.TreeViewRefreshed += new System.Action(this.navigationView_TreeViewRefreshed);
            // 
            // NavigationPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStripSmall);
            this.Name = "NavigationPane";
            this.Size = new System.Drawing.Size(215, 426);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.toolStripBig.ResumeLayout(false);
            this.toolStripBig.PerformLayout();
            this.toolStripSmall.ResumeLayout(false);
            this.toolStripSmall.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.MainWindowControls.NavigationToolStripSmall toolStripSmall;
        private XenAdmin.Controls.MainWindowControls.NavigationButtonSmall buttonInfraSmall;
        private XenAdmin.Controls.MainWindowControls.NavigationButtonSmall buttonObjectsSmall;
        private XenAdmin.Controls.MainWindowControls.NavigationButtonSmall buttonTagsSmall;
        private XenAdmin.Controls.MainWindowControls.NavigationDropDownButtonSmall buttonSearchesSmall;
        private XenAdmin.Controls.MainWindowControls.NavigationButtonSmall buttonNotifySmall;
        private XenAdmin.Controls.MainWindowControls.NavigationToolStripBig toolStripBig;
        private XenAdmin.Controls.MainWindowControls.NavigationButtonBig buttonInfraBig;
        private XenAdmin.Controls.MainWindowControls.NavigationButtonBig buttonObjectsBig;
        private XenAdmin.Controls.MainWindowControls.NavigationButtonBig buttonTagsBig;
        private XenAdmin.Controls.MainWindowControls.NavigationDropDownButtonBig buttonSearchesBig;
        private XenAdmin.Controls.MainWindowControls.NavigationButtonBig buttonNotifyBig;
        private XenAdmin.Controls.Common.SmoothSplitContainer splitContainer1;
        private NavigationView navigationView;
    }
}
