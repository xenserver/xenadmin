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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThinProvisioningParametersControl));
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
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.incremental_allocation_units, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.initialAllocationNumericUpDown, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.initial_allocation_units, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.allocationQuantumNumericUpDown, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // incremental_allocation_units
            // 
            this.incremental_allocation_units.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.incremental_allocation_units, "incremental_allocation_units");
            this.incremental_allocation_units.FormattingEnabled = true;
            this.incremental_allocation_units.Items.AddRange(new object[] {
            resources.GetString("incremental_allocation_units.Items"),
            resources.GetString("incremental_allocation_units.Items1")});
            this.incremental_allocation_units.Name = "incremental_allocation_units";
            this.incremental_allocation_units.SelectedIndexChanged += new System.EventHandler(this.incremental_allocation_units_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // initialAllocationNumericUpDown
            // 
            resources.ApplyResources(this.initialAllocationNumericUpDown, "initialAllocationNumericUpDown");
            this.initialAllocationNumericUpDown.DecimalPlaces = 1;
            this.initialAllocationNumericUpDown.Name = "initialAllocationNumericUpDown";
            this.initialAllocationNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // initial_allocation_units
            // 
            this.initial_allocation_units.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.initial_allocation_units, "initial_allocation_units");
            this.initial_allocation_units.FormattingEnabled = true;
            this.initial_allocation_units.Items.AddRange(new object[] {
            resources.GetString("initial_allocation_units.Items"),
            resources.GetString("initial_allocation_units.Items1")});
            this.initial_allocation_units.Name = "initial_allocation_units";
            this.initial_allocation_units.SelectedIndexChanged += new System.EventHandler(this.initial_allocation_units_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // allocationQuantumNumericUpDown
            // 
            resources.ApplyResources(this.allocationQuantumNumericUpDown, "allocationQuantumNumericUpDown");
            this.allocationQuantumNumericUpDown.DecimalPlaces = 1;
            this.allocationQuantumNumericUpDown.Name = "allocationQuantumNumericUpDown";
            this.allocationQuantumNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // ThinProvisioningParametersControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ThinProvisioningParametersControl";
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
