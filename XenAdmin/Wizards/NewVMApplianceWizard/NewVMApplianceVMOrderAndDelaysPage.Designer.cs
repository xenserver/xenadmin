namespace XenAdmin.Wizards.NewVMApplianceWizard
{
    partial class NewVMApplianceVMOrderAndDelaysPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewVMApplianceVMOrderAndDelaysPage));
            this.labelwizard = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView1 = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.startupSequenceGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.labelStartOrder = new System.Windows.Forms.Label();
            this.labelStartDelayUnits = new System.Windows.Forms.Label();
            this.nudStartDelay = new System.Windows.Forms.NumericUpDown();
            this.labelStartDelay = new System.Windows.Forms.Label();
            this.nudOrder = new System.Windows.Forms.NumericUpDown();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnStartDelay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.startupSequenceGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOrder)).BeginInit();
            this.SuspendLayout();
            // 
            // labelwizard
            // 
            resources.ApplyResources(this.labelwizard, "labelwizard");
            this.labelwizard.Name = "labelwizard";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelwizard, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.startupSequenceGroupBox, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.ColumnDescription,
            this.ColumnOrder,
            this.ColumnStartDelay});
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.MultiSelect = true;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // startupSequenceGroupBox
            // 
            resources.ApplyResources(this.startupSequenceGroupBox, "startupSequenceGroupBox");
            this.startupSequenceGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.startupSequenceGroupBox.Name = "startupSequenceGroupBox";
            this.startupSequenceGroupBox.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.labelStartOrder, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelStartDelayUnits, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.nudStartDelay, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelStartDelay, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.nudOrder, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // labelStartOrder
            // 
            resources.ApplyResources(this.labelStartOrder, "labelStartOrder");
            this.labelStartOrder.Name = "labelStartOrder";
            // 
            // labelStartDelayUnits
            // 
            resources.ApplyResources(this.labelStartDelayUnits, "labelStartDelayUnits");
            this.labelStartDelayUnits.Name = "labelStartDelayUnits";
            // 
            // nudStartDelay
            // 
            resources.ApplyResources(this.nudStartDelay, "nudStartDelay");
            this.nudStartDelay.Name = "nudStartDelay";
            this.nudStartDelay.ValueChanged += new System.EventHandler(this.nudStartDelay_ValueChanged);
            // 
            // labelStartDelay
            // 
            resources.ApplyResources(this.labelStartDelay, "labelStartDelay");
            this.labelStartDelay.Name = "labelStartDelay";
            // 
            // nudOrder
            // 
            resources.ApplyResources(this.nudOrder, "nudOrder");
            this.nudOrder.Name = "nudOrder";
            this.nudOrder.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnName, "ColumnName");
            this.ColumnName.Name = "ColumnName";
            // 
            // ColumnDescription
            // 
            this.ColumnDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnDescription, "ColumnDescription");
            this.ColumnDescription.Name = "ColumnDescription";
            // 
            // ColumnOrder
            // 
            this.ColumnOrder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnOrder, "ColumnOrder");
            this.ColumnOrder.Name = "ColumnOrder";
            // 
            // ColumnStartDelay
            // 
            this.ColumnStartDelay.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnStartDelay, "ColumnStartDelay");
            this.ColumnStartDelay.Name = "ColumnStartDelay";
            // 
            // NewVMApplianceVMOrderAndDelaysPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NewVMApplianceVMOrderAndDelaysPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.startupSequenceGroupBox.ResumeLayout(false);
            this.startupSequenceGroupBox.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOrder)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelwizard;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelStartOrder;
        private System.Windows.Forms.Label labelStartDelay;
        private System.Windows.Forms.NumericUpDown nudOrder;
        private System.Windows.Forms.NumericUpDown nudStartDelay;
        private System.Windows.Forms.Label labelStartDelayUnits;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridView1;
        private System.Windows.Forms.GroupBox startupSequenceGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnStartDelay;
    }
}
