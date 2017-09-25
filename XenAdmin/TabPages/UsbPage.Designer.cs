namespace XenAdmin.TabPages
{
    partial class UsbPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UsbPage));
            this.tableLayoutPanelBase = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewUsbList = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnPassthrough = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnVirtualMachine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonPassthrough = new System.Windows.Forms.Button();
            this.pageContainerPanel.SuspendLayout();
            this.tableLayoutPanelBase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUsbList)).BeginInit();
            this.flowLayoutPanelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.tableLayoutPanelBase);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // tableLayoutPanelBase
            // 
            resources.ApplyResources(this.tableLayoutPanelBase, "tableLayoutPanelBase");
            this.tableLayoutPanelBase.Controls.Add(this.dataGridViewUsbList, 0, 0);
            this.tableLayoutPanelBase.Controls.Add(this.flowLayoutPanelButtons, 0, 1);
            this.tableLayoutPanelBase.Name = "tableLayoutPanelBase";
            // 
            // dataGridViewUsbList
            // 
            this.dataGridViewUsbList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewUsbList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewUsbList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewUsbList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUsbList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnLocation,
            this.columnDescription,
            this.columnPassthrough,
            this.columnVirtualMachine});
            resources.ApplyResources(this.dataGridViewUsbList, "dataGridViewUsbList");
            this.dataGridViewUsbList.Name = "dataGridViewUsbList";
            this.dataGridViewUsbList.ReadOnly = true;
            this.dataGridViewUsbList.SelectionChanged += new System.EventHandler(this.dataGridViewUsbList_SelectionChanged);
            // 
            // columnLocation
            // 
            this.columnLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnLocation.FillWeight = 17F;
            resources.ApplyResources(this.columnLocation, "columnLocation");
            this.columnLocation.Name = "columnLocation";
            this.columnLocation.ReadOnly = true;
            // 
            // columnDescription
            // 
            this.columnDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnDescription.FillWeight = 45F;
            resources.ApplyResources(this.columnDescription, "columnDescription");
            this.columnDescription.Name = "columnDescription";
            this.columnDescription.ReadOnly = true;
            // 
            // columnPassthrough
            // 
            this.columnPassthrough.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnPassthrough.FillWeight = 15F;
            resources.ApplyResources(this.columnPassthrough, "columnPassthrough");
            this.columnPassthrough.Name = "columnPassthrough";
            this.columnPassthrough.ReadOnly = true;
            // 
            // columnVirtualMachine
            // 
            this.columnVirtualMachine.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnVirtualMachine.FillWeight = 24F;
            resources.ApplyResources(this.columnVirtualMachine, "columnVirtualMachine");
            this.columnVirtualMachine.Name = "columnVirtualMachine";
            this.columnVirtualMachine.ReadOnly = true;
            // 
            // flowLayoutPanelButtons
            // 
            resources.ApplyResources(this.flowLayoutPanelButtons, "flowLayoutPanelButtons");
            this.flowLayoutPanelButtons.Controls.Add(this.buttonPassthrough);
            this.flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            // 
            // buttonPassthrough
            // 
            resources.ApplyResources(this.buttonPassthrough, "buttonPassthrough");
            this.buttonPassthrough.Name = "buttonPassthrough";
            this.buttonPassthrough.UseVisualStyleBackColor = true;
            this.buttonPassthrough.Click += new System.EventHandler(this.buttonPassthrough_Click);
            // 
            // UsbPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Name = "UsbPage";
            this.pageContainerPanel.ResumeLayout(false);
            this.pageContainerPanel.PerformLayout();
            this.tableLayoutPanelBase.ResumeLayout(false);
            this.tableLayoutPanelBase.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUsbList)).EndInit();
            this.flowLayoutPanelButtons.ResumeLayout(false);
            this.flowLayoutPanelButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBase;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewUsbList;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;
        private System.Windows.Forms.Button buttonPassthrough;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnPassthrough;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnVirtualMachine;

    }
}
