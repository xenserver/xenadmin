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
            this.memorySpinnerDynMax = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.vmShinyBar = new XenAdmin.Controls.Ballooning.VMShinyBar();
            this.pictureBoxDynMin = new System.Windows.Forms.PictureBox();
            this.pictureBoxDynMax = new System.Windows.Forms.PictureBox();
            this.labelDynMin = new System.Windows.Forms.Label();
            this.labelDynMax = new System.Windows.Forms.Label();
            this.radioFixed = new System.Windows.Forms.RadioButton();
            this.radioDynamic = new System.Windows.Forms.RadioButton();
            this.groupBoxOn = new System.Windows.Forms.GroupBox();
            this.iconDMCUnavailable = new System.Windows.Forms.PictureBox();
            this.labelDMCUnavailable = new System.Windows.Forms.Label();
            this.linkInstallTools = new System.Windows.Forms.LinkLabel();
            this.memorySpinnerFixed = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.spinnerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDynMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDynMax)).BeginInit();
            this.groupBoxOn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconDMCUnavailable)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // spinnerPanel
            // 
            resources.ApplyResources(this.spinnerPanel, "spinnerPanel");
            this.spinnerPanel.Controls.Add(this.memorySpinnerDynMin, 2, 1);
            this.spinnerPanel.Controls.Add(this.memorySpinnerDynMax, 2, 2);
            this.spinnerPanel.Controls.Add(this.vmShinyBar, 0, 0);
            this.spinnerPanel.Controls.Add(this.pictureBoxDynMin, 0, 1);
            this.spinnerPanel.Controls.Add(this.pictureBoxDynMax, 0, 2);
            this.spinnerPanel.Controls.Add(this.labelDynMin, 1, 1);
            this.spinnerPanel.Controls.Add(this.labelDynMax, 1, 2);
            this.spinnerPanel.Name = "spinnerPanel";
            // 
            // memorySpinnerDynMin
            // 
            resources.ApplyResources(this.memorySpinnerDynMin, "memorySpinnerDynMin");
            this.memorySpinnerDynMin.Increment = 0.1D;
            this.memorySpinnerDynMin.Name = "memorySpinnerDynMin";
            this.memorySpinnerDynMin.SpinnerValueChanged += new System.EventHandler(this.DynamicSpinners_ValueChanged);
            // 
            // memorySpinnerDynMax
            // 
            resources.ApplyResources(this.memorySpinnerDynMax, "memorySpinnerDynMax");
            this.memorySpinnerDynMax.Increment = 0.1D;
            this.memorySpinnerDynMax.Name = "memorySpinnerDynMax";
            this.memorySpinnerDynMax.SpinnerValueChanged += new System.EventHandler(this.DynamicSpinners_ValueChanged);
            // 
            // vmShinyBar
            // 
            this.spinnerPanel.SetColumnSpan(this.vmShinyBar, 3);
            this.vmShinyBar.Increment = 0D;
            resources.ApplyResources(this.vmShinyBar, "vmShinyBar");
            this.vmShinyBar.Name = "vmShinyBar";
            this.vmShinyBar.TabStop = false;
            this.vmShinyBar.SliderDragged += new System.EventHandler(this.vmShinyBar_SliderDragged);
            // 
            // pictureBoxDynMin
            // 
            resources.ApplyResources(this.pictureBoxDynMin, "pictureBoxDynMin");
            this.pictureBoxDynMin.Image = global::XenAdmin.Properties.Resources.memory_dynmin_slider_small;
            this.pictureBoxDynMin.Name = "pictureBoxDynMin";
            this.pictureBoxDynMin.TabStop = false;
            // 
            // pictureBoxDynMax
            // 
            resources.ApplyResources(this.pictureBoxDynMax, "pictureBoxDynMax");
            this.pictureBoxDynMax.Image = global::XenAdmin.Properties.Resources.memory_dynmax_slider_small;
            this.pictureBoxDynMax.Name = "pictureBoxDynMax";
            this.pictureBoxDynMax.TabStop = false;
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
            // radioFixed
            // 
            resources.ApplyResources(this.radioFixed, "radioFixed");
            this.radioFixed.Name = "radioFixed";
            this.radioFixed.TabStop = true;
            this.radioFixed.UseVisualStyleBackColor = true;
            // 
            // radioDynamic
            // 
            resources.ApplyResources(this.radioDynamic, "radioDynamic");
            this.tableLayoutPanel1.SetColumnSpan(this.radioDynamic, 2);
            this.radioDynamic.Name = "radioDynamic";
            this.radioDynamic.TabStop = true;
            this.radioDynamic.UseVisualStyleBackColor = true;
            // 
            // groupBoxOn
            // 
            resources.ApplyResources(this.groupBoxOn, "groupBoxOn");
            this.tableLayoutPanel1.SetColumnSpan(this.groupBoxOn, 2);
            this.groupBoxOn.Controls.Add(this.spinnerPanel);
            this.groupBoxOn.Name = "groupBoxOn";
            this.groupBoxOn.TabStop = false;
            // 
            // iconDMCUnavailable
            // 
            resources.ApplyResources(this.iconDMCUnavailable, "iconDMCUnavailable");
            this.iconDMCUnavailable.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
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
            resources.ApplyResources(this.memorySpinnerFixed, "memorySpinnerFixed");
            this.memorySpinnerFixed.Increment = 0.1D;
            this.memorySpinnerFixed.Name = "memorySpinnerFixed";
            this.memorySpinnerFixed.SpinnerValueChanged += new System.EventHandler(this.FixedSpinner_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.memorySpinnerFixed, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxOn, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.radioFixed, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioDynamic, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelDMCUnavailable, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.iconDMCUnavailable, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.linkInstallTools, 1, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // VMMemoryControlsBasic
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "VMMemoryControlsBasic";
            this.spinnerPanel.ResumeLayout(false);
            this.spinnerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDynMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDynMax)).EndInit();
            this.groupBoxOn.ResumeLayout(false);
            this.groupBoxOn.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconDMCUnavailable)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VMShinyBar vmShinyBar;
        private System.Windows.Forms.TableLayoutPanel spinnerPanel;
        private MemorySpinner memorySpinnerDynMin;
        private System.Windows.Forms.RadioButton radioFixed;
        private System.Windows.Forms.RadioButton radioDynamic;
        private System.Windows.Forms.GroupBox groupBoxOn;
        private System.Windows.Forms.PictureBox iconDMCUnavailable;
        private System.Windows.Forms.Label labelDMCUnavailable;
        private System.Windows.Forms.LinkLabel linkInstallTools;
        private MemorySpinner memorySpinnerFixed;
        private MemorySpinner memorySpinnerDynMax;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBoxDynMin;
        private System.Windows.Forms.PictureBox pictureBoxDynMax;
        private System.Windows.Forms.Label labelDynMin;
        private System.Windows.Forms.Label labelDynMax;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}
