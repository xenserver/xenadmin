namespace XenAdmin.Wizards.NewVMWizard
{
    partial class Page_CpuMem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_CpuMem));
            this.labelVCPUs = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ErrorLabel = new System.Windows.Forms.Label();
            this.ErrorPanel = new System.Windows.Forms.Panel();
            this.spinnerDynMin = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.spinnerDynMax = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.spinnerStatMax = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.comboBoxInitialVCPUs = new System.Windows.Forms.ComboBox();
            this.labelInitialVCPUs = new System.Windows.Forms.Label();
            this.labelInvalidVCPUWarning = new System.Windows.Forms.Label();
            this.comboBoxTopology = new XenAdmin.Controls.CPUTopologyComboBox();
            this.labelTopology = new System.Windows.Forms.Label();
            this.comboBoxVCPUs = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.ErrorPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelVCPUs
            // 
            resources.ApplyResources(this.labelVCPUs, "labelVCPUs");
            this.labelVCPUs.Name = "labelVCPUs";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.tableLayoutPanel1.SetColumnSpan(this.label5, 2);
            this.label5.Name = "label5";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // ErrorLabel
            // 
            resources.ApplyResources(this.ErrorLabel, "ErrorLabel");
            this.ErrorLabel.Name = "ErrorLabel";
            // 
            // ErrorPanel
            // 
            this.ErrorPanel.Controls.Add(this.ErrorLabel);
            this.ErrorPanel.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.ErrorPanel, "ErrorPanel");
            this.ErrorPanel.Name = "ErrorPanel";
            // 
            // spinnerDynMin
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.spinnerDynMin, 2);
            this.spinnerDynMin.Increment = 0.1D;
            resources.ApplyResources(this.spinnerDynMin, "spinnerDynMin");
            this.spinnerDynMin.Name = "spinnerDynMin";
            this.spinnerDynMin.SpinnerValueChanged += new System.EventHandler(this.memory_ValueChanged);
            // 
            // spinnerDynMax
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.spinnerDynMax, 2);
            this.spinnerDynMax.Increment = 0.1D;
            resources.ApplyResources(this.spinnerDynMax, "spinnerDynMax");
            this.spinnerDynMax.Name = "spinnerDynMax";
            this.spinnerDynMax.SpinnerValueChanged += new System.EventHandler(this.memory_ValueChanged);
            // 
            // spinnerStatMax
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.spinnerStatMax, 2);
            this.spinnerStatMax.Increment = 0.1D;
            resources.ApplyResources(this.spinnerStatMax, "spinnerStatMax");
            this.spinnerStatMax.Name = "spinnerStatMax";
            this.spinnerStatMax.SpinnerValueChanged += new System.EventHandler(this.memory_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.comboBoxInitialVCPUs, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelInitialVCPUs, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelInvalidVCPUWarning, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxTopology, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.spinnerStatMax, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelVCPUs, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.spinnerDynMax, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.spinnerDynMin, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelTopology, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxVCPUs, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // comboBoxInitialVCPUs
            // 
            this.comboBoxInitialVCPUs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInitialVCPUs.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxInitialVCPUs, "comboBoxInitialVCPUs");
            this.comboBoxInitialVCPUs.Name = "comboBoxInitialVCPUs";
            // 
            // labelInitialVCPUs
            // 
            resources.ApplyResources(this.labelInitialVCPUs, "labelInitialVCPUs");
            this.labelInitialVCPUs.Name = "labelInitialVCPUs";
            // 
            // labelInvalidVCPUWarning
            // 
            resources.ApplyResources(this.labelInvalidVCPUWarning, "labelInvalidVCPUWarning");
            this.labelInvalidVCPUWarning.ForeColor = System.Drawing.Color.Red;
            this.labelInvalidVCPUWarning.Name = "labelInvalidVCPUWarning";
            // 
            // comboBoxTopology
            // 
            this.comboBoxTopology.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxTopology, "comboBoxTopology");
            this.comboBoxTopology.FormattingEnabled = true;
            this.comboBoxTopology.Name = "comboBoxTopology";
            this.comboBoxTopology.SelectedIndexChanged += new System.EventHandler(this.comboBoxTopology_SelectedIndexChanged);
            // 
            // labelTopology
            // 
            resources.ApplyResources(this.labelTopology, "labelTopology");
            this.labelTopology.Name = "labelTopology";
            // 
            // comboBoxVCPUs
            // 
            this.comboBoxVCPUs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVCPUs.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxVCPUs, "comboBoxVCPUs");
            this.comboBoxVCPUs.Name = "comboBoxVCPUs";
            this.comboBoxVCPUs.SelectedIndexChanged += new System.EventHandler(this.vCPU_ValueChanged);
            // 
            // Page_CpuMem
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.ErrorPanel);
            this.Name = "Page_CpuMem";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ErrorPanel.ResumeLayout(false);
            this.ErrorPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelVCPUs;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label ErrorLabel;
        private System.Windows.Forms.Panel ErrorPanel;
        private XenAdmin.Controls.Ballooning.MemorySpinner spinnerDynMin;
        private XenAdmin.Controls.Ballooning.MemorySpinner spinnerDynMax;
        private XenAdmin.Controls.Ballooning.MemorySpinner spinnerStatMax;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelTopology;
        private Controls.CPUTopologyComboBox comboBoxTopology;
        private System.Windows.Forms.Label labelInvalidVCPUWarning;
        private System.Windows.Forms.ComboBox comboBoxVCPUs;
        private System.Windows.Forms.ComboBox comboBoxInitialVCPUs;
        private System.Windows.Forms.Label labelInitialVCPUs;
    }
}
