namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class StorageProvisioning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StorageProvisioning));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.initialAllocationNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.radioButtonThickProvisioning = new System.Windows.Forms.RadioButton();
            this.labelInitialAllocation = new System.Windows.Forms.Label();
            this.radioButtonThinProvisioning = new System.Windows.Forms.RadioButton();
            this.labelAllocationQuantum = new System.Windows.Forms.Label();
            this.allocationQuantumNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.initial_allocation_units = new System.Windows.Forms.ComboBox();
            this.incremental_allocation_units = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.initialAllocationNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.allocationQuantumNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.initialAllocationNumericUpDown, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonThickProvisioning, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelInitialAllocation, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonThinProvisioning, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelAllocationQuantum, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.allocationQuantumNumericUpDown, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.initial_allocation_units, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.incremental_allocation_units, 3, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 5);
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
            // radioButtonThickProvisioning
            // 
            resources.ApplyResources(this.radioButtonThickProvisioning, "radioButtonThickProvisioning");
            this.radioButtonThickProvisioning.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonThickProvisioning, 5);
            this.radioButtonThickProvisioning.Name = "radioButtonThickProvisioning";
            this.radioButtonThickProvisioning.TabStop = true;
            this.radioButtonThickProvisioning.UseVisualStyleBackColor = true;
            this.radioButtonThickProvisioning.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // labelInitialAllocation
            // 
            resources.ApplyResources(this.labelInitialAllocation, "labelInitialAllocation");
            this.labelInitialAllocation.Name = "labelInitialAllocation";
            // 
            // radioButtonThinProvisioning
            // 
            resources.ApplyResources(this.radioButtonThinProvisioning, "radioButtonThinProvisioning");
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonThinProvisioning, 5);
            this.radioButtonThinProvisioning.Name = "radioButtonThinProvisioning";
            this.radioButtonThinProvisioning.UseVisualStyleBackColor = true;
            this.radioButtonThinProvisioning.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // labelAllocationQuantum
            // 
            resources.ApplyResources(this.labelAllocationQuantum, "labelAllocationQuantum");
            this.labelAllocationQuantum.Name = "labelAllocationQuantum";
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
            // StorageProvisioning
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "StorageProvisioning";
            resources.ApplyResources(this, "$this");
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.initialAllocationNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.allocationQuantumNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown initialAllocationNumericUpDown;
        private System.Windows.Forms.Label labelInitialAllocation;
        private System.Windows.Forms.RadioButton radioButtonThinProvisioning;
        private System.Windows.Forms.RadioButton radioButtonThickProvisioning;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelAllocationQuantum;
        private System.Windows.Forms.NumericUpDown allocationQuantumNumericUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox initial_allocation_units;
        private System.Windows.Forms.ComboBox incremental_allocation_units;
    }
}
