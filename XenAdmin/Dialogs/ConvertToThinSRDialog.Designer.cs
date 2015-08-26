namespace XenAdmin.Dialogs
{
    partial class ConvertToThinSRDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConvertToThinSRDialog));
            this.GbLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelWarning = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.CloseButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.labelAllocationQuantum = new System.Windows.Forms.Label();
            this.labelInitialAllocation = new System.Windows.Forms.Label();
            this.initialAllocationNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.allocationQuantumNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.queuedBackgroundWorker1 = new XenCenterLib.QueuedBackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.initialAllocationNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.allocationQuantumNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // GbLabel
            // 
            resources.ApplyResources(this.GbLabel, "GbLabel");
            this.GbLabel.Name = "GbLabel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelWarning, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelAllocationQuantum, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelInitialAllocation, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.initialAllocationNumericUpDown, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.allocationQuantumNumericUpDown, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label7, 2, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.tableLayoutPanel1.SetColumnSpan(this.labelWarning, 4);
            this.labelWarning.Name = "labelWarning";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 5);
            this.panel1.Controls.Add(this.CloseButton);
            this.panel1.Controls.Add(this.OkButton);
            this.panel1.Name = "panel1";
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // OkButton
            // 
            resources.ApplyResources(this.OkButton, "OkButton");
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OkButton.Name = "OkButton";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.tableLayoutPanel1.SetColumnSpan(this.panel2, 2);
            this.panel2.Name = "panel2";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.tableLayoutPanel1.SetColumnSpan(this.label6, 5);
            this.label6.Name = "label6";
            // 
            // labelAllocationQuantum
            // 
            resources.ApplyResources(this.labelAllocationQuantum, "labelAllocationQuantum");
            this.tableLayoutPanel1.SetColumnSpan(this.labelAllocationQuantum, 2);
            this.labelAllocationQuantum.Name = "labelAllocationQuantum";
            // 
            // labelInitialAllocation
            // 
            resources.ApplyResources(this.labelInitialAllocation, "labelInitialAllocation");
            this.tableLayoutPanel1.SetColumnSpan(this.labelInitialAllocation, 2);
            this.labelInitialAllocation.Name = "labelInitialAllocation";
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
            this.initialAllocationNumericUpDown.ValueChanged += new System.EventHandler(this.initialAllocationNumericUpDown_ValueChanged);
            this.initialAllocationNumericUpDown.Enter += new System.EventHandler(this.initialAllocationNumericUpDown_Enter);
            this.initialAllocationNumericUpDown.Leave += new System.EventHandler(this.initialAllocationNumericUpDown_Leave);
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
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // ConvertToThinSRDialog
            // 
            this.AcceptButton = this.OkButton;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.CloseButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.GbLabel);
            this.Name = "ConvertToThinSRDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.initialAllocationNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.allocationQuantumNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label GbLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label6;
        private XenCenterLib.QueuedBackgroundWorker queuedBackgroundWorker1;
        private System.Windows.Forms.Label labelAllocationQuantum;
        private System.Windows.Forms.Label labelInitialAllocation;
        private System.Windows.Forms.NumericUpDown initialAllocationNumericUpDown;
        private System.Windows.Forms.NumericUpDown allocationQuantumNumericUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.FlowLayoutPanel panel1;
        public System.Windows.Forms.Button CloseButton;
        public System.Windows.Forms.Button OkButton;

    }
}
