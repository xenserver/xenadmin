namespace XenAdmin.SettingsPanels
{
    partial class USBEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(USBEditPage));
            this.tableLayoutPanelBase = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewUsbList = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnAttached = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAttach = new System.Windows.Forms.Button();
            this.buttonDetach = new System.Windows.Forms.Button();
            this.tableLayoutPanelBase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUsbList)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelBase
            // 
            resources.ApplyResources(this.tableLayoutPanelBase, "tableLayoutPanelBase");
            this.tableLayoutPanelBase.Controls.Add(this.dataGridViewUsbList, 0, 0);
            this.tableLayoutPanelBase.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanelBase.Name = "tableLayoutPanelBase";
            // 
            // dataGridViewUsbList
            // 
            this.dataGridViewUsbList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewUsbList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewUsbList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUsbList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnLocation,
            this.columnDescription,
            this.columnAttached});
            resources.ApplyResources(this.dataGridViewUsbList, "dataGridViewUsbList");
            this.dataGridViewUsbList.Name = "dataGridViewUsbList";
            this.dataGridViewUsbList.SelectionChanged += new System.EventHandler(this.dataGridViewUsbList_SelectionChanged);
            // 
            // columnLocation
            // 
            this.columnLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.columnLocation, "columnLocation");
            this.columnLocation.Name = "columnLocation";
            // 
            // columnDescription
            // 
            this.columnDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.columnDescription, "columnDescription");
            this.columnDescription.Name = "columnDescription";
            // 
            // columnAttached
            // 
            this.columnAttached.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.columnAttached, "columnAttached");
            this.columnAttached.Name = "columnAttached";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonAttach);
            this.flowLayoutPanel1.Controls.Add(this.buttonDetach);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // buttonAttach
            // 
            resources.ApplyResources(this.buttonAttach, "buttonAttach");
            this.buttonAttach.Name = "buttonAttach";
            this.buttonAttach.UseVisualStyleBackColor = true;
            this.buttonAttach.Click += new System.EventHandler(this.buttonAttach_Click);
            // 
            // buttonDetach
            // 
            resources.ApplyResources(this.buttonDetach, "buttonDetach");
            this.buttonDetach.Name = "buttonDetach";
            this.buttonDetach.UseVisualStyleBackColor = true;
            this.buttonDetach.Click += new System.EventHandler(this.buttonDetach_Click);
            // 
            // USBEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanelBase);
            this.Name = "USBEditPage";
            this.tableLayoutPanelBase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUsbList)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBase;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewUsbList;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonAttach;
        private System.Windows.Forms.Button buttonDetach;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnAttached;
    }
}
