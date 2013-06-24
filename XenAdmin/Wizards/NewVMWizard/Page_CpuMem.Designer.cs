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
            this.VcpuSpinner = new System.Windows.Forms.NumericUpDown();
            this.labelVCPUs = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ErrorLabel = new System.Windows.Forms.Label();
            this.ErrorPanel = new System.Windows.Forms.Panel();
            this.spinnerDynMin = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.spinnerDynMax = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.spinnerStatMax = new XenAdmin.Controls.Ballooning.MemorySpinner();
            ((System.ComponentModel.ISupportInitialize)(this.VcpuSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.ErrorPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // VcpuSpinner
            // 
            resources.ApplyResources(this.VcpuSpinner, "VcpuSpinner");
            this.VcpuSpinner.Name = "VcpuSpinner";
            this.VcpuSpinner.ValueChanged += new System.EventHandler(this.vCPU_ValueChanged);
            this.VcpuSpinner.Leave += new System.EventHandler(this.VcpuSpinner_Leave);
            // 
            // labelVCPUs
            // 
            resources.ApplyResources(this.labelVCPUs, "labelVCPUs");
            this.labelVCPUs.Name = "labelVCPUs";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
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
            this.spinnerDynMin.Name = "spinnerDynMin";
            this.spinnerDynMin.SpinnerValueChanged += new System.EventHandler(this.memory_ValueChanged);
            // 
            // spinnerDynMax
            // 
            resources.ApplyResources(this.spinnerDynMax, "spinnerDynMax");
            this.spinnerDynMax.Name = "spinnerDynMax";
            this.spinnerDynMax.SpinnerValueChanged += new System.EventHandler(this.memory_ValueChanged);
            // 
            // spinnerStatMax
            // 
            resources.ApplyResources(this.spinnerStatMax, "spinnerStatMax");
            this.spinnerStatMax.Name = "spinnerStatMax";
            this.spinnerStatMax.SpinnerValueChanged += new System.EventHandler(this.memory_ValueChanged);
            // 
            // Page_CpuMem
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.spinnerStatMax);
            this.Controls.Add(this.spinnerDynMax);
            this.Controls.Add(this.spinnerDynMin);
            this.Controls.Add(this.labelVCPUs);
            this.Controls.Add(this.VcpuSpinner);
            this.Controls.Add(this.ErrorPanel);
            this.Controls.Add(this.label5);
            this.Name = "Page_CpuMem";
            ((System.ComponentModel.ISupportInitialize)(this.VcpuSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ErrorPanel.ResumeLayout(false);
            this.ErrorPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown VcpuSpinner;
        private System.Windows.Forms.Label labelVCPUs;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label ErrorLabel;
        private System.Windows.Forms.Panel ErrorPanel;
        private XenAdmin.Controls.Ballooning.MemorySpinner spinnerDynMin;
        private XenAdmin.Controls.Ballooning.MemorySpinner spinnerDynMax;
        private XenAdmin.Controls.Ballooning.MemorySpinner spinnerStatMax;
    }
}
