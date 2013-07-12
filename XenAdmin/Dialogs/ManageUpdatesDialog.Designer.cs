namespace XenAdmin.Dialogs
{
    partial class ManageUpdatesDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageUpdatesDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewUpdates = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.downloadAndInstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelUpdates = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.helpButton = new XenAdmin.Controls.HelpButton();
            this.panelProgress = new XenAdmin.Controls.FlickerFreePanel();
            this.labelProgress = new System.Windows.Forms.Label();
            this.pictureBoxProgress = new System.Windows.Forms.PictureBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.downloadAndInstallButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.informationLabelIcon = new System.Windows.Forms.PictureBox();
            this.informationLabel = new System.Windows.Forms.LinkLabel();
            this.ColumnExpander = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnAppliesTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnReleaseDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnWebPage = new System.Windows.Forms.DataGridViewLinkColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUpdates)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.helpButton)).BeginInit();
            this.panelProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.informationLabelIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewUpdates
            // 
            resources.ApplyResources(this.dataGridViewUpdates, "dataGridViewUpdates");
            this.dataGridViewUpdates.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewUpdates.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewUpdates.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewUpdates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewUpdates.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnExpander,
            this.ColumnMessage,
            this.ColumnAppliesTo,
            this.ColumnReleaseDate,
            this.ColumnWebPage});
            this.tableLayoutPanel.SetColumnSpan(this.dataGridViewUpdates, 3);
            this.dataGridViewUpdates.ContextMenuStrip = this.contextMenuStrip;
            this.dataGridViewUpdates.Name = "dataGridViewUpdates";
            this.dataGridViewUpdates.ReadOnly = true;
            this.dataGridViewUpdates.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridViewUpdates_SortCompare);
            this.dataGridViewUpdates.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewUpdates_CellDoubleClick);
            this.dataGridViewUpdates.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewUpdates_CellClick);
            this.dataGridViewUpdates.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewUpdates_KeyDown);
            this.dataGridViewUpdates.SelectionChanged += new System.EventHandler(this.dataGridViewUpdates_SelectionChanged);
            this.dataGridViewUpdates.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewUpdates_CellContentClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.separatorToolStripMenuItem,
            this.downloadAndInstallToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            resources.ApplyResources(this.refreshToolStripMenuItem, "refreshToolStripMenuItem");
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // separatorToolStripMenuItem
            // 
            this.separatorToolStripMenuItem.Name = "separatorToolStripMenuItem";
            resources.ApplyResources(this.separatorToolStripMenuItem, "separatorToolStripMenuItem");
            // 
            // downloadAndInstallToolStripMenuItem
            // 
            this.downloadAndInstallToolStripMenuItem.Name = "downloadAndInstallToolStripMenuItem";
            resources.ApplyResources(this.downloadAndInstallToolStripMenuItem, "downloadAndInstallToolStripMenuItem");
            this.downloadAndInstallToolStripMenuItem.Click += new System.EventHandler(this.downloadAndInstallToolStripMenuItem_Click);
            // 
            // closeButton
            // 
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // tableLayoutPanel
            // 
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.labelUpdates, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.pictureBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.helpButton, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.dataGridViewUpdates, 1, 1);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // labelUpdates
            // 
            resources.ApplyResources(this.labelUpdates, "labelUpdates");
            this.labelUpdates.Name = "labelUpdates";
            // 
            // pictureBox
            // 
            resources.ApplyResources(this.pictureBox, "pictureBox");
            this.pictureBox.Image = global::XenAdmin.Properties.Resources._015_Download_h32bit_32;
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.TabStop = false;
            // 
            // helpButton
            // 
            resources.ApplyResources(this.helpButton, "helpButton");
            this.helpButton.Name = "helpButton";
            this.helpButton.TabStop = false;
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // panelProgress
            // 
            resources.ApplyResources(this.panelProgress, "panelProgress");
            this.panelProgress.BackColor = System.Drawing.SystemColors.Window;
            this.panelProgress.BorderColor = System.Drawing.Color.Black;
            this.panelProgress.BorderWidth = 1;
            this.panelProgress.Controls.Add(this.labelProgress);
            this.panelProgress.Controls.Add(this.pictureBoxProgress);
            this.panelProgress.Name = "panelProgress";
            // 
            // labelProgress
            // 
            resources.ApplyResources(this.labelProgress, "labelProgress");
            this.labelProgress.Name = "labelProgress";
            // 
            // pictureBoxProgress
            // 
            resources.ApplyResources(this.pictureBoxProgress, "pictureBoxProgress");
            this.pictureBoxProgress.Name = "pictureBoxProgress";
            this.pictureBoxProgress.TabStop = false;
            // 
            // refreshButton
            // 
            resources.ApplyResources(this.refreshButton, "refreshButton");
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // downloadAndInstallButton
            // 
            resources.ApplyResources(this.downloadAndInstallButton, "downloadAndInstallButton");
            this.downloadAndInstallButton.Name = "downloadAndInstallButton";
            this.downloadAndInstallButton.UseVisualStyleBackColor = true;
            this.downloadAndInstallButton.Click += new System.EventHandler(this.downloadAndInstallButton_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.informationLabelIcon, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.informationLabel, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // informationLabelIcon
            // 
            resources.ApplyResources(this.informationLabelIcon, "informationLabelIcon");
            this.informationLabelIcon.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.informationLabelIcon.InitialImage = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.informationLabelIcon.Name = "informationLabelIcon";
            this.informationLabelIcon.TabStop = false;
            // 
            // informationLabel
            // 
            resources.ApplyResources(this.informationLabel, "informationLabel");
            this.informationLabel.Name = "informationLabel";
            this.informationLabel.TabStop = true;
            // 
            // ColumnExpander
            // 
            this.ColumnExpander.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle1.NullValue")));
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.ColumnExpander.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.ColumnExpander, "ColumnExpander");
            this.ColumnExpander.Name = "ColumnExpander";
            this.ColumnExpander.ReadOnly = true;
            // 
            // ColumnMessage
            // 
            this.ColumnMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnMessage.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnMessage.FillWeight = 40F;
            resources.ApplyResources(this.ColumnMessage, "ColumnMessage");
            this.ColumnMessage.Name = "ColumnMessage";
            this.ColumnMessage.ReadOnly = true;
            // 
            // ColumnAppliesTo
            // 
            this.ColumnAppliesTo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnAppliesTo.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColumnAppliesTo.FillWeight = 20F;
            resources.ApplyResources(this.ColumnAppliesTo, "ColumnAppliesTo");
            this.ColumnAppliesTo.Name = "ColumnAppliesTo";
            this.ColumnAppliesTo.ReadOnly = true;
            this.ColumnAppliesTo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnReleaseDate
            // 
            this.ColumnReleaseDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnReleaseDate.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColumnReleaseDate.FillWeight = 20F;
            resources.ApplyResources(this.ColumnReleaseDate, "ColumnReleaseDate");
            this.ColumnReleaseDate.Name = "ColumnReleaseDate";
            this.ColumnReleaseDate.ReadOnly = true;
            // 
            // ColumnWebPage
            // 
            this.ColumnWebPage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ColumnWebPage.FillWeight = 20F;
            resources.ApplyResources(this.ColumnWebPage, "ColumnWebPage");
            this.ColumnWebPage.Name = "ColumnWebPage";
            this.ColumnWebPage.ReadOnly = true;
            this.ColumnWebPage.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnWebPage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ManageUpdatesDialog
            // 
            this.AcceptButton = this.closeButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.closeButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panelProgress);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.downloadAndInstallButton);
            this.Controls.Add(this.refreshButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "ManageUpdatesDialog";
            this.Shown += new System.EventHandler(this.ManageUpdatesDialog_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ManageUpdatesDialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUpdates)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.helpButton)).EndInit();
            this.panelProgress.ResumeLayout(false);
            this.panelProgress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.informationLabelIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewUpdates;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label labelUpdates;
        private XenAdmin.Controls.FlickerFreePanel panelProgress;
        private System.Windows.Forms.PictureBox pictureBoxProgress;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem downloadAndInstallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator separatorToolStripMenuItem;
        private System.Windows.Forms.Button downloadAndInstallButton;
        private XenAdmin.Controls.HelpButton helpButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox informationLabelIcon;
        private System.Windows.Forms.LinkLabel informationLabel;
        private System.Windows.Forms.DataGridViewImageColumn ColumnExpander;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnAppliesTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnReleaseDate;
        private System.Windows.Forms.DataGridViewLinkColumn ColumnWebPage;
    }
}