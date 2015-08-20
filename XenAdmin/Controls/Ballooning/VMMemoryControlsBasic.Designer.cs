namespace XenAdmin.Controls.Ballooning
{
    partial class VMMemoryControlsBasic
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VMMemoryControlsBasic));
            this.spinnerPanel = new System.Windows.Forms.TableLayoutPanel();
            this.memorySpinnerDynMin = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.vmShinyBar = new XenAdmin.Controls.Ballooning.VMShinyBar();
            this.radioOff = new System.Windows.Forms.RadioButton();
            this.radioOn = new System.Windows.Forms.RadioButton();
            this.groupBoxOn = new System.Windows.Forms.GroupBox();
            this.iconDMCUnavailable = new System.Windows.Forms.PictureBox();
            this.labelDMCUnavailable = new System.Windows.Forms.Label();
            this.linkInstallTools = new System.Windows.Forms.LinkLabel();
            this.memorySpinnerFixed = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.memorySpinnerDynMax = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.spinnerPanel.SuspendLayout();
            this.groupBoxOn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconDMCUnavailable)).BeginInit();
            this.SuspendLayout();
            // 
            // spinnerPanel
            // 
            resources.ApplyResources(this.spinnerPanel, "spinnerPanel");
            this.spinnerPanel.Controls.Add(this.memorySpinnerDynMin, 0, 0);
            this.spinnerPanel.Controls.Add(this.memorySpinnerDynMax, 0, 1);
            this.spinnerPanel.Name = "spinnerPanel";
            // 
            // memorySpinnerDynMin
            // 
            this.memorySpinnerDynMin.Increment = 1D;
            resources.ApplyResources(this.memorySpinnerDynMin, "memorySpinnerDynMin");
            this.memorySpinnerDynMin.Name = "memorySpinnerDynMin";
            this.memorySpinnerDynMin.Units = "GB";
            this.memorySpinnerDynMin.SpinnerValueChanged += new System.EventHandler(this.DynamicSpinners_ValueChanged);
            // 
            // memorySpinnerDynMax
            // 
            this.memorySpinnerDynMax.Increment = 1D;
            resources.ApplyResources(this.memorySpinnerDynMax, "memorySpinnerDynMax");
            this.memorySpinnerDynMax.Name = "memorySpinnerDynMax";
            this.memorySpinnerDynMax.Units = "GB";
            this.memorySpinnerDynMax.SpinnerValueChanged += new System.EventHandler(this.DynamicSpinners_ValueChanged);
            // 
            // vmShinyBar
            // 
            resources.ApplyResources(this.vmShinyBar, "vmShinyBar");
            this.vmShinyBar.Name = "vmShinyBar";
            this.vmShinyBar.SliderDragged += new System.EventHandler(this.vmShinyBar_SliderDragged);
            // 
            // radioOff
            // 
            resources.ApplyResources(this.radioOff, "radioOff");
            this.radioOff.Name = "radioOff";
            this.radioOff.TabStop = true;
            this.radioOff.UseVisualStyleBackColor = true;
            // 
            // radioOn
            // 
            resources.ApplyResources(this.radioOn, "radioOn");
            this.radioOn.Name = "radioOn";
            this.radioOn.TabStop = true;
            this.radioOn.UseVisualStyleBackColor = true;
            // 
            // groupBoxOn
            // 
            this.groupBoxOn.Controls.Add(this.vmShinyBar);
            this.groupBoxOn.Controls.Add(this.spinnerPanel);
            resources.ApplyResources(this.groupBoxOn, "groupBoxOn");
            this.groupBoxOn.Name = "groupBoxOn";
            this.groupBoxOn.TabStop = false;
            // 
            // iconDMCUnavailable
            // 
            resources.ApplyResources(this.iconDMCUnavailable, "iconDMCUnavailable");
            this.iconDMCUnavailable.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.iconDMCUnavailable.Name = "iconDMCUnavailable";
            this.iconDMCUnavailable.TabStop = false;
            // 
            // labelDMCUnavailable
            // 
            resources.ApplyResources(this.labelDMCUnavailable, "labelDMCUnavailable");
            this.labelDMCUnavailable.Name = "labelDMCUnavailable";
            // 
            // linkInstallTools
            // 
            resources.ApplyResources(this.linkInstallTools, "linkInstallTools");
            this.linkInstallTools.Name = "linkInstallTools";
            this.linkInstallTools.TabStop = true;
            this.linkInstallTools.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkInstallTools.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.InstallTools_LinkClicked);
            // 
            // memorySpinnerFixed
            // 
            this.memorySpinnerFixed.Increment = 1D;
            resources.ApplyResources(this.memorySpinnerFixed, "memorySpinnerFixed");
            this.memorySpinnerFixed.Name = "memorySpinnerFixed";
            this.memorySpinnerFixed.Units = "GB";
            this.memorySpinnerFixed.SpinnerValueChanged += new System.EventHandler(this.FixedSpinner_ValueChanged);
            // 
            // VMMemoryControlsBasic
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.memorySpinnerFixed);
            this.Controls.Add(this.linkInstallTools);
            this.Controls.Add(this.labelDMCUnavailable);
            this.Controls.Add(this.iconDMCUnavailable);
            this.Controls.Add(this.groupBoxOn);
            this.Controls.Add(this.radioOn);
            this.Controls.Add(this.radioOff);
            this.DoubleBuffered = true;
            this.Name = "VMMemoryControlsBasic";
            this.spinnerPanel.ResumeLayout(false);
            this.groupBoxOn.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.iconDMCUnavailable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VMShinyBar vmShinyBar;
        private System.Windows.Forms.TableLayoutPanel spinnerPanel;
        private MemorySpinner memorySpinnerDynMin;
        private System.Windows.Forms.RadioButton radioOff;
        private System.Windows.Forms.RadioButton radioOn;
        private System.Windows.Forms.GroupBox groupBoxOn;
        private System.Windows.Forms.PictureBox iconDMCUnavailable;
        private System.Windows.Forms.Label labelDMCUnavailable;
        private System.Windows.Forms.LinkLabel linkInstallTools;
        private MemorySpinner memorySpinnerFixed;
        private MemorySpinner memorySpinnerDynMax;
    }
}
