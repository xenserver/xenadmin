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
            this.initialAllocationNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.labelInitialAllocation = new System.Windows.Forms.Label();
            this.radioButtonThinProvisioning = new System.Windows.Forms.RadioButton();
            this.radioButtonThickProvisioning = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.labelAllocationQuantum = new System.Windows.Forms.Label();
            this.allocationQuantumNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.labelPercent1 = new System.Windows.Forms.Label();
            this.labelPercent2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.initialAllocationNumericUpDown)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.allocationQuantumNumericUpDown)).BeginInit();
            this.SuspendLayout();
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
            // labelInitialAllocation
            // 
            resources.ApplyResources(this.labelInitialAllocation, "labelInitialAllocation");
            this.labelInitialAllocation.Name = "labelInitialAllocation";
            // 
            // radioButtonThinProvisioning
            // 
            resources.ApplyResources(this.radioButtonThinProvisioning, "radioButtonThinProvisioning");
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonThinProvisioning, 2);
            this.radioButtonThinProvisioning.Name = "radioButtonThinProvisioning";
            this.radioButtonThinProvisioning.UseVisualStyleBackColor = true;
            this.radioButtonThinProvisioning.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonThickProvisioning
            // 
            resources.ApplyResources(this.radioButtonThickProvisioning, "radioButtonThickProvisioning");
            this.radioButtonThickProvisioning.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonThickProvisioning, 2);
            this.radioButtonThickProvisioning.Name = "radioButtonThickProvisioning";
            this.radioButtonThickProvisioning.TabStop = true;
            this.radioButtonThickProvisioning.UseVisualStyleBackColor = true;
            this.radioButtonThickProvisioning.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
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
            this.tableLayoutPanel1.Controls.Add(this.labelPercent1, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelPercent2, 3, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 4);
            this.label1.Name = "label1";
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
            // labelPercent1
            // 
            resources.ApplyResources(this.labelPercent1, "labelPercent1");
            this.labelPercent1.Name = "labelPercent1";
            // 
            // labelPercent2
            // 
            resources.ApplyResources(this.labelPercent2, "labelPercent2");
            this.labelPercent2.Name = "labelPercent2";
            // 
            // StorageProvisioning
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "StorageProvisioning";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this.initialAllocationNumericUpDown)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
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
        private System.Windows.Forms.Label labelPercent1;
        private System.Windows.Forms.Label labelPercent2;
        private System.Windows.Forms.Label label1;
    }
}
