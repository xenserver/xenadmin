namespace XenAdmin.Controls
{
    partial class ThinProvisioningParametersControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.incremental_allocation_units = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.initialAllocationNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.initial_allocation_units = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.allocationQuantumNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.initialAllocationNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.allocationQuantumNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.incremental_allocation_units, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.initialAllocationNumericUpDown, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.initial_allocation_units, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.allocationQuantumNumericUpDown, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(302, 58);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // incremental_allocation_units
            // 
            this.incremental_allocation_units.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.incremental_allocation_units.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.incremental_allocation_units.FormattingEnabled = true;
            this.incremental_allocation_units.Items.AddRange(new object[] {
            "GB",
            "MB"});
            this.incremental_allocation_units.Location = new System.Drawing.Point(250, 32);
            this.incremental_allocation_units.Name = "incremental_allocation_units";
            this.incremental_allocation_units.Size = new System.Drawing.Size(49, 23);
            this.incremental_allocation_units.TabIndex = 24;
            this.incremental_allocation_units.SelectedIndexChanged += new System.EventHandler(this.incremental_allocation_units_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 15);
            this.label1.TabIndex = 11;
            this.label1.Text = "&Initial allocation:";
            // 
            // initialAllocationNumericUpDown
            // 
            this.initialAllocationNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.initialAllocationNumericUpDown.DecimalPlaces = 1;
            this.initialAllocationNumericUpDown.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.initialAllocationNumericUpDown.Location = new System.Drawing.Point(139, 3);
            this.initialAllocationNumericUpDown.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.initialAllocationNumericUpDown.Name = "initialAllocationNumericUpDown";
            this.initialAllocationNumericUpDown.Size = new System.Drawing.Size(105, 23);
            this.initialAllocationNumericUpDown.TabIndex = 12;
            this.initialAllocationNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.initialAllocationNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // initial_allocation_units
            // 
            this.initial_allocation_units.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.initial_allocation_units.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.initial_allocation_units.FormattingEnabled = true;
            this.initial_allocation_units.Items.AddRange(new object[] {
            "GB",
            "MB"});
            this.initial_allocation_units.Location = new System.Drawing.Point(250, 3);
            this.initial_allocation_units.Name = "initial_allocation_units";
            this.initial_allocation_units.Size = new System.Drawing.Size(49, 23);
            this.initial_allocation_units.TabIndex = 23;
            this.initial_allocation_units.SelectedIndexChanged += new System.EventHandler(this.initial_allocation_units_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(3, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "In&cremental allocations:";
            // 
            // allocationQuantumNumericUpDown
            // 
            this.allocationQuantumNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.allocationQuantumNumericUpDown.DecimalPlaces = 1;
            this.allocationQuantumNumericUpDown.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.allocationQuantumNumericUpDown.Location = new System.Drawing.Point(139, 32);
            this.allocationQuantumNumericUpDown.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.allocationQuantumNumericUpDown.Name = "allocationQuantumNumericUpDown";
            this.allocationQuantumNumericUpDown.Size = new System.Drawing.Size(105, 23);
            this.allocationQuantumNumericUpDown.TabIndex = 14;
            this.allocationQuantumNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.allocationQuantumNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // ThinProvisioningParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ThinProvisioningParameters";
            this.Size = new System.Drawing.Size(308, 64);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.initialAllocationNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.allocationQuantumNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown initialAllocationNumericUpDown;
        private System.Windows.Forms.ComboBox initial_allocation_units;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown allocationQuantumNumericUpDown;
        private System.Windows.Forms.ComboBox incremental_allocation_units;
    }
}
