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
            this.labelNetworkheadings = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.columnVirtAlloc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnUsage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnShared = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewSr = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.newSRButton = new XenAdmin.Commands.CommandButton();
            this.trimButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.trimButton = new XenAdmin.Commands.CommandButton();
            this.buttonProperties = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pageContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSr)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.trimButtonContainer.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
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
            // columnVirtAlloc
            // 
            this.columnVirtAlloc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnVirtAlloc, "columnVirtAlloc");
            this.columnVirtAlloc.Name = "columnVirtAlloc";
            // 
            // columnSize
            // 
            this.columnSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnSize, "columnSize");
            this.columnSize.Name = "columnSize";
            // 
            // columnUsage
            // 
            this.columnUsage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnUsage, "columnUsage");
            this.columnUsage.Name = "columnUsage";
            // 
            // columnShared
            // 
            this.columnShared.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnShared, "columnShared");
            this.columnShared.Name = "columnShared";
            // 
            // columnType
            // 
            this.columnType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnType, "columnType");
            this.columnType.Name = "columnType";
            // 
            // columnDescription
            // 
            this.columnDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.columnDescription, "columnDescription");
            this.columnDescription.Name = "columnDescription";
            // 
            // columnName
            // 
            this.columnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.columnName, "columnName");
            this.columnName.Name = "columnName";
            // 
            // columnImage
            // 
            this.columnImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnImage, "columnImage");
            this.columnImage.Name = "columnImage";
            this.columnImage.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dataGridViewSr
            // 
            resources.ApplyResources(this.dataGridViewSr, "dataGridViewSr");
            this.dataGridViewSr.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewSr.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewSr.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewSr.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnImage,
            this.columnName,
            this.columnDescription,
            this.columnType,
            this.columnShared,
            this.columnUsage,
            this.columnSize,
            this.columnVirtAlloc});
            this.dataGridViewSr.MultiSelect = true;
            this.dataGridViewSr.Name = "dataGridViewSr";
            this.dataGridViewSr.SelectionChanged += new System.EventHandler(this.dataGridViewSrs_SelectedIndexChanged);
            this.dataGridViewSr.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridViewSr_SortCompare);
            this.dataGridViewSr.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridViewSr_MouseUp);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.newSRButton);
            this.flowLayoutPanel1.Controls.Add(this.trimButtonContainer);
            this.flowLayoutPanel1.Controls.Add(this.buttonProperties);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // newSRButton
            // 
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
            resources.ApplyResources(this.trimButton, "trimButton");
            this.trimButton.Name = "trimButton";
            this.trimButton.UseVisualStyleBackColor = true;
            // 
            // buttonProperties
            // 
            resources.ApplyResources(this.buttonProperties, "buttonProperties");
            this.buttonProperties.Name = "buttonProperties";
            this.buttonProperties.UseVisualStyleBackColor = true;
            this.buttonProperties.Click += new System.EventHandler(this.buttonProperties_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelNetworkheadings, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewSr, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSr)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.trimButtonContainer.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label labelNetworkheadings;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Commands.CommandButton newSRButton;
        private Commands.CommandButton trimButton;
        private Controls.ToolTipContainer trimButtonContainer;

        private Controls.DataGridViewEx.DataGridViewEx dataGridViewSr;
        private System.Windows.Forms.Button buttonProperties;
        private System.Windows.Forms.DataGridViewImageColumn columnImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnShared;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnUsage;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnVirtAlloc;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
