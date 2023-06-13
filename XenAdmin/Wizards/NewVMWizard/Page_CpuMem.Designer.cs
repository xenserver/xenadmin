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
            this.warningsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cpuWarningPictureBox = new System.Windows.Forms.PictureBox();
            this.cpuWarningLabel = new System.Windows.Forms.Label();
            this.memoryWarningLabel = new System.Windows.Forms.Label();
            this.memoryPictureBox = new System.Windows.Forms.PictureBox();
            this.labelStatMaxInfo = new System.Windows.Forms.Label();
            this.labelDynMaxInfo = new System.Windows.Forms.Label();
            this.labelDynMinInfo = new System.Windows.Forms.Label();
            this.labelStatMax = new System.Windows.Forms.Label();
            this.labelDynMax = new System.Windows.Forms.Label();
            this.labelDynMin = new System.Windows.Forms.Label();
            this.spinnerDynMin = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.comboBoxVCPUs = new System.Windows.Forms.ComboBox();
            this.labelTopology = new System.Windows.Forms.Label();
            this.spinnerDynMax = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.labelVCPUs = new System.Windows.Forms.Label();
            this.spinnerStatMax = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxTopology = new XenAdmin.Controls.CPUTopologyComboBox();
            this.labelInitialVCPUs = new System.Windows.Forms.Label();
            this.comboBoxInitialVCPUs = new System.Windows.Forms.ComboBox();
            this.initialVCPUWarningLabel = new System.Windows.Forms.Label();
            this.vCPUWarningLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxTopology = new System.Windows.Forms.PictureBox();
            this.labelTopologyWarning = new System.Windows.Forms.Label();
            this.warningsTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cpuWarningPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryPictureBox)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTopology)).BeginInit();
            this.SuspendLayout();
            // 
            // warningsTableLayoutPanel
            // 
            resources.ApplyResources(this.warningsTableLayoutPanel, "warningsTableLayoutPanel");
            this.tableLayoutPanel1.SetColumnSpan(this.warningsTableLayoutPanel, 4);
            this.warningsTableLayoutPanel.Controls.Add(this.pictureBoxTopology, 0, 2);
            this.warningsTableLayoutPanel.Controls.Add(this.cpuWarningPictureBox, 0, 0);
            this.warningsTableLayoutPanel.Controls.Add(this.cpuWarningLabel, 1, 0);
            this.warningsTableLayoutPanel.Controls.Add(this.memoryWarningLabel, 1, 4);
            this.warningsTableLayoutPanel.Controls.Add(this.memoryPictureBox, 0, 4);
            this.warningsTableLayoutPanel.Controls.Add(this.labelTopologyWarning, 1, 2);
            this.warningsTableLayoutPanel.Name = "warningsTableLayoutPanel";
            // 
            // cpuWarningPictureBox
            // 
            this.cpuWarningPictureBox.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.cpuWarningPictureBox, "cpuWarningPictureBox");
            this.cpuWarningPictureBox.Name = "cpuWarningPictureBox";
            this.cpuWarningPictureBox.TabStop = false;
            // 
            // cpuWarningLabel
            // 
            resources.ApplyResources(this.cpuWarningLabel, "cpuWarningLabel");
            this.cpuWarningLabel.Name = "cpuWarningLabel";
            // 
            // memoryWarningLabel
            // 
            resources.ApplyResources(this.memoryWarningLabel, "memoryWarningLabel");
            this.memoryWarningLabel.Name = "memoryWarningLabel";
            // 
            // memoryPictureBox
            // 
            this.memoryPictureBox.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.memoryPictureBox, "memoryPictureBox");
            this.memoryPictureBox.Name = "memoryPictureBox";
            this.memoryPictureBox.TabStop = false;
            // 
            // labelStatMaxInfo
            // 
            resources.ApplyResources(this.labelStatMaxInfo, "labelStatMaxInfo");
            this.labelStatMaxInfo.Name = "labelStatMaxInfo";
            // 
            // labelDynMaxInfo
            // 
            resources.ApplyResources(this.labelDynMaxInfo, "labelDynMaxInfo");
            this.labelDynMaxInfo.Name = "labelDynMaxInfo";
            // 
            // labelDynMinInfo
            // 
            resources.ApplyResources(this.labelDynMinInfo, "labelDynMinInfo");
            this.labelDynMinInfo.Name = "labelDynMinInfo";
            // 
            // labelStatMax
            // 
            resources.ApplyResources(this.labelStatMax, "labelStatMax");
            this.labelStatMax.Name = "labelStatMax";
            // 
            // labelDynMax
            // 
            resources.ApplyResources(this.labelDynMax, "labelDynMax");
            this.labelDynMax.Name = "labelDynMax";
            // 
            // labelDynMin
            // 
            resources.ApplyResources(this.labelDynMin, "labelDynMin");
            this.labelDynMin.Name = "labelDynMin";
            // 
            // spinnerDynMin
            // 
            resources.ApplyResources(this.spinnerDynMin, "spinnerDynMin");
            this.tableLayoutPanel1.SetColumnSpan(this.spinnerDynMin, 2);
            this.spinnerDynMin.Increment = 0.1D;
            this.spinnerDynMin.Name = "spinnerDynMin";
            this.spinnerDynMin.SpinnerValueChanged += new System.EventHandler(this.memory_ValueChanged);
            // 
            // comboBoxVCPUs
            // 
            this.comboBoxVCPUs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVCPUs.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxVCPUs, "comboBoxVCPUs");
            this.comboBoxVCPUs.Name = "comboBoxVCPUs";
            this.comboBoxVCPUs.SelectedIndexChanged += new System.EventHandler(this.vCPU_ValueChanged);
            // 
            // labelTopology
            // 
            resources.ApplyResources(this.labelTopology, "labelTopology");
            this.labelTopology.Name = "labelTopology";
            // 
            // spinnerDynMax
            // 
            resources.ApplyResources(this.spinnerDynMax, "spinnerDynMax");
            this.tableLayoutPanel1.SetColumnSpan(this.spinnerDynMax, 2);
            this.spinnerDynMax.Increment = 0.1D;
            this.spinnerDynMax.Name = "spinnerDynMax";
            this.spinnerDynMax.SpinnerValueChanged += new System.EventHandler(this.memory_ValueChanged);
            // 
            // labelVCPUs
            // 
            resources.ApplyResources(this.labelVCPUs, "labelVCPUs");
            this.labelVCPUs.Name = "labelVCPUs";
            // 
            // spinnerStatMax
            // 
            resources.ApplyResources(this.spinnerStatMax, "spinnerStatMax");
            this.tableLayoutPanel1.SetColumnSpan(this.spinnerStatMax, 2);
            this.spinnerStatMax.Increment = 0.1D;
            this.spinnerStatMax.Name = "spinnerStatMax";
            this.spinnerStatMax.SpinnerValueChanged += new System.EventHandler(this.memory_ValueChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.tableLayoutPanel1.SetColumnSpan(this.label5, 4);
            this.label5.Name = "label5";
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
            // labelInitialVCPUs
            // 
            resources.ApplyResources(this.labelInitialVCPUs, "labelInitialVCPUs");
            this.labelInitialVCPUs.Name = "labelInitialVCPUs";
            // 
            // comboBoxInitialVCPUs
            // 
            this.comboBoxInitialVCPUs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInitialVCPUs.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxInitialVCPUs, "comboBoxInitialVCPUs");
            this.comboBoxInitialVCPUs.Name = "comboBoxInitialVCPUs";
            this.comboBoxInitialVCPUs.SelectedIndexChanged += new System.EventHandler(this.comboBoxInitialVCPUs_SelectedIndexChanged);
            // 
            // initialVCPUWarningLabel
            // 
            resources.ApplyResources(this.initialVCPUWarningLabel, "initialVCPUWarningLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.initialVCPUWarningLabel, 2);
            this.initialVCPUWarningLabel.ForeColor = System.Drawing.Color.Red;
            this.initialVCPUWarningLabel.Name = "initialVCPUWarningLabel";
            // 
            // vCPUWarningLabel
            // 
            resources.ApplyResources(this.vCPUWarningLabel, "vCPUWarningLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.vCPUWarningLabel, 2);
            this.vCPUWarningLabel.ForeColor = System.Drawing.Color.Red;
            this.vCPUWarningLabel.Name = "vCPUWarningLabel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.vCPUWarningLabel, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.initialVCPUWarningLabel, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxInitialVCPUs, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelInitialVCPUs, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxTopology, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.spinnerStatMax, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelVCPUs, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.spinnerDynMax, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelTopology, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxVCPUs, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.spinnerDynMin, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelDynMin, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelDynMax, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelStatMax, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelDynMinInfo, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelDynMaxInfo, 3, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelStatMaxInfo, 3, 7);
            this.tableLayoutPanel1.Controls.Add(this.warningsTableLayoutPanel, 0, 8);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // pictureBoxTopology
            // 
            this.pictureBoxTopology.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.pictureBoxTopology, "pictureBoxTopology");
            this.pictureBoxTopology.Name = "pictureBoxTopology";
            this.pictureBoxTopology.TabStop = false;
            // 
            // labelTopologyWarning
            // 
            resources.ApplyResources(this.labelTopologyWarning, "labelTopologyWarning");
            this.labelTopologyWarning.Name = "labelTopologyWarning";
            // 
            // Page_CpuMem
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Page_CpuMem";
            this.warningsTableLayoutPanel.ResumeLayout(false);
            this.warningsTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cpuWarningPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryPictureBox)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTopology)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel warningsTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label vCPUWarningLabel;
        private System.Windows.Forms.Label initialVCPUWarningLabel;
        private System.Windows.Forms.ComboBox comboBoxInitialVCPUs;
        private System.Windows.Forms.Label labelInitialVCPUs;
        private Controls.CPUTopologyComboBox comboBoxTopology;
        private System.Windows.Forms.Label label5;
        private Controls.Ballooning.MemorySpinner spinnerStatMax;
        private System.Windows.Forms.Label labelVCPUs;
        private Controls.Ballooning.MemorySpinner spinnerDynMax;
        private System.Windows.Forms.Label labelTopology;
        private System.Windows.Forms.ComboBox comboBoxVCPUs;
        private Controls.Ballooning.MemorySpinner spinnerDynMin;
        private System.Windows.Forms.Label labelDynMin;
        private System.Windows.Forms.Label labelDynMax;
        private System.Windows.Forms.Label labelStatMax;
        private System.Windows.Forms.Label labelDynMinInfo;
        private System.Windows.Forms.Label labelDynMaxInfo;
        private System.Windows.Forms.Label labelStatMaxInfo;
        private System.Windows.Forms.PictureBox cpuWarningPictureBox;
        private System.Windows.Forms.Label cpuWarningLabel;
        private System.Windows.Forms.Label memoryWarningLabel;
        private System.Windows.Forms.PictureBox memoryPictureBox;
        private System.Windows.Forms.PictureBox pictureBoxTopology;
        private System.Windows.Forms.Label labelTopologyWarning;
    }
}
