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
            this.vCPUWarningLabel = new System.Windows.Forms.Label();
            this.initialVCPUWarningLabel = new System.Windows.Forms.Label();
            this.comboBoxInitialVCPUs = new System.Windows.Forms.ComboBox();
            this.labelInitialVCPUs = new System.Windows.Forms.Label();
            this.labelInvalidVCPUWarning = new System.Windows.Forms.Label();
            this.comboBoxTopology = new XenAdmin.Controls.CPUTopologyComboBox();
            this.labelTopology = new System.Windows.Forms.Label();
            this.comboBoxVCPUs = new System.Windows.Forms.ComboBox();
            this.labelDynMin = new System.Windows.Forms.Label();
            this.labelDynMax = new System.Windows.Forms.Label();
            this.labelStatMax = new System.Windows.Forms.Label();
            this.labelDynMinInfo = new System.Windows.Forms.Label();
            this.labelDynMaxInfo = new System.Windows.Forms.Label();
            this.labelStatMaxInfo = new System.Windows.Forms.Label();
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
            this.tableLayoutPanel1.SetColumnSpan(this.label5, 4);
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
            resources.ApplyResources(this.spinnerDynMin, "spinnerDynMin");
            this.tableLayoutPanel1.SetColumnSpan(this.spinnerDynMin, 2);
            this.spinnerDynMin.Increment = 0.1D;
            this.spinnerDynMin.Name = "spinnerDynMin";
            this.spinnerDynMin.SpinnerValueChanged += new System.EventHandler(this.memory_ValueChanged);
            // 
            // spinnerDynMax
            // 
            resources.ApplyResources(this.spinnerDynMax, "spinnerDynMax");
            this.tableLayoutPanel1.SetColumnSpan(this.spinnerDynMax, 2);
            this.spinnerDynMax.Increment = 0.1D;
            this.spinnerDynMax.Name = "spinnerDynMax";
            this.spinnerDynMax.SpinnerValueChanged += new System.EventHandler(this.memory_ValueChanged);
            // 
            // spinnerStatMax
            // 
            resources.ApplyResources(this.spinnerStatMax, "spinnerStatMax");
            this.tableLayoutPanel1.SetColumnSpan(this.spinnerStatMax, 2);
            this.spinnerStatMax.Increment = 0.1D;
            this.spinnerStatMax.Name = "spinnerStatMax";
            this.spinnerStatMax.SpinnerValueChanged += new System.EventHandler(this.memory_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.vCPUWarningLabel, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.initialVCPUWarningLabel, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxInitialVCPUs, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelInitialVCPUs, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelInvalidVCPUWarning, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxTopology, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.spinnerStatMax, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.labelVCPUs, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.spinnerDynMax, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelTopology, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxVCPUs, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.spinnerDynMin, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelDynMin, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelDynMax, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelStatMax, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.labelDynMinInfo, 3, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelDynMaxInfo, 3, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelStatMaxInfo, 3, 8);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // vCPUWarningLabel
            // 
            resources.ApplyResources(this.vCPUWarningLabel, "vCPUWarningLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.vCPUWarningLabel, 2);
            this.vCPUWarningLabel.ForeColor = System.Drawing.Color.Red;
            this.vCPUWarningLabel.Name = "vCPUWarningLabel";
            // 
            // initialVCPUWarningLabel
            // 
            resources.ApplyResources(this.initialVCPUWarningLabel, "initialVCPUWarningLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.initialVCPUWarningLabel, 2);
            this.initialVCPUWarningLabel.ForeColor = System.Drawing.Color.Red;
            this.initialVCPUWarningLabel.Name = "initialVCPUWarningLabel";
            // 
            // comboBoxInitialVCPUs
            // 
            this.comboBoxInitialVCPUs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInitialVCPUs.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxInitialVCPUs, "comboBoxInitialVCPUs");
            this.comboBoxInitialVCPUs.Name = "comboBoxInitialVCPUs";
            this.comboBoxInitialVCPUs.SelectedIndexChanged += new System.EventHandler(this.comboBoxInitialVCPUs_SelectedIndexChanged);
            // 
            // labelInitialVCPUs
            // 
            resources.ApplyResources(this.labelInitialVCPUs, "labelInitialVCPUs");
            this.labelInitialVCPUs.Name = "labelInitialVCPUs";
            // 
            // labelInvalidVCPUWarning
            // 
            resources.ApplyResources(this.labelInvalidVCPUWarning, "labelInvalidVCPUWarning");
            this.tableLayoutPanel1.SetColumnSpan(this.labelInvalidVCPUWarning, 3);
            this.labelInvalidVCPUWarning.ForeColor = System.Drawing.Color.Red;
            this.labelInvalidVCPUWarning.Name = "labelInvalidVCPUWarning";
            // 
            // comboBoxTopology
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.comboBoxTopology, 3);
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
            // labelDynMin
            // 
            resources.ApplyResources(this.labelDynMin, "labelDynMin");
            this.labelDynMin.Name = "labelDynMin";
            // 
            // labelDynMax
            // 
            resources.ApplyResources(this.labelDynMax, "labelDynMax");
            this.labelDynMax.Name = "labelDynMax";
            // 
            // labelStatMax
            // 
            resources.ApplyResources(this.labelStatMax, "labelStatMax");
            this.labelStatMax.Name = "labelStatMax";
            // 
            // labelDynMinInfo
            // 
            resources.ApplyResources(this.labelDynMinInfo, "labelDynMinInfo");
            this.labelDynMinInfo.Name = "labelDynMinInfo";
            // 
            // labelDynMaxInfo
            // 
            resources.ApplyResources(this.labelDynMaxInfo, "labelDynMaxInfo");
            this.labelDynMaxInfo.Name = "labelDynMaxInfo";
            // 
            // labelStatMaxInfo
            // 
            resources.ApplyResources(this.labelStatMaxInfo, "labelStatMaxInfo");
            this.labelStatMaxInfo.Name = "labelStatMaxInfo";
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
        private System.Windows.Forms.ComboBox comboBoxVCPUs;
        private System.Windows.Forms.ComboBox comboBoxInitialVCPUs;
        private System.Windows.Forms.Label labelInitialVCPUs;
        private System.Windows.Forms.Label labelInvalidVCPUWarning;
        private System.Windows.Forms.Label labelDynMin;
        private System.Windows.Forms.Label labelDynMax;
        private System.Windows.Forms.Label labelStatMax;
        private System.Windows.Forms.Label labelDynMinInfo;
        private System.Windows.Forms.Label labelDynMaxInfo;
        private System.Windows.Forms.Label labelStatMaxInfo;
        private System.Windows.Forms.Label vCPUWarningLabel;
        private System.Windows.Forms.Label initialVCPUWarningLabel;
    }
}
