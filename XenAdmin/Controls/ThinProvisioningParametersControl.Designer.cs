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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.SR_incremental_allocation_limits_info_label = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.incremental_allocation_units = new System.Windows.Forms.ComboBox();
            this.initial_allocation_label = new System.Windows.Forms.Label();
            this.initialAllocationNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.initial_allocation_units = new System.Windows.Forms.ComboBox();
            this.incremental_allocation_label = new System.Windows.Forms.Label();
            this.allocationQuantumNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.initialAllocationNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.allocationQuantumNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.incremental_allocation_units, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.initial_allocation_label, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.initialAllocationNumericUpDown, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.initial_allocation_units, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.incremental_allocation_label, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.allocationQuantumNumericUpDown, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 4);
            this.tableLayoutPanel2.Controls.Add(this.SR_incremental_allocation_limits_info_label, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox2, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // SR_incremental_allocation_limits_info_label
            // 
            resources.ApplyResources(this.SR_incremental_allocation_limits_info_label, "SR_incremental_allocation_limits_info_label");
            this.SR_incremental_allocation_limits_info_label.Name = "SR_incremental_allocation_limits_info_label";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
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
            // initial_allocation_label
            // 
            resources.ApplyResources(this.initial_allocation_label, "initial_allocation_label");
            this.initial_allocation_label.Name = "initial_allocation_label";
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
            // incremental_allocation_label
            // 
            resources.ApplyResources(this.incremental_allocation_label, "incremental_allocation_label");
            this.incremental_allocation_label.Name = "incremental_allocation_label";
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
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.initialAllocationNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.allocationQuantumNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label initial_allocation_label;
        private System.Windows.Forms.NumericUpDown initialAllocationNumericUpDown;
        private System.Windows.Forms.ComboBox initial_allocation_units;
        private System.Windows.Forms.Label incremental_allocation_label;
        private System.Windows.Forms.NumericUpDown allocationQuantumNumericUpDown;
        private System.Windows.Forms.ComboBox incremental_allocation_units;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label SR_incremental_allocation_limits_info_label;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}
