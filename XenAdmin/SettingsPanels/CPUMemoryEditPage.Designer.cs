namespace XenAdmin.SettingsPanels
{
    partial class CpuMemoryEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CpuMemoryEditPage));
            this.lblSliderHighest = new System.Windows.Forms.Label();
            this.lblSliderNormal = new System.Windows.Forms.Label();
            this.lblSliderLowest = new System.Windows.Forms.Label();
            this.lblPriority = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.warningsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cpuWarningPictureBox = new System.Windows.Forms.PictureBox();
            this.cpuWarningLabel = new System.Windows.Forms.Label();
            this.memoryWarningLabel = new System.Windows.Forms.Label();
            this.topologyWarningLabel = new System.Windows.Forms.Label();
            this.memoryPictureBox = new System.Windows.Forms.PictureBox();
            this.topologyPictureBox = new System.Windows.Forms.PictureBox();
            this.comboBoxInitialVCPUs = new System.Windows.Forms.ComboBox();
            this.labelInitialVCPUs = new System.Windows.Forms.Label();
            this.comboBoxTopology = new XenAdmin.Controls.CPUTopologyComboBox();
            this.labelTopology = new System.Windows.Forms.Label();
            this.MemWarningLabel = new System.Windows.Forms.Label();
            this.memorySpinner = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.panel1 = new System.Windows.Forms.Panel();
            this.transparentTrackBar1 = new XenAdmin.Controls.TransparentTrackBar();
            this.lblVCPUs = new System.Windows.Forms.Label();
            this.lblMemory = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxVCPUs = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.warningsTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cpuWarningPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topologyPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSliderHighest
            // 
            resources.ApplyResources(this.lblSliderHighest, "lblSliderHighest");
            this.lblSliderHighest.Name = "lblSliderHighest";
            // 
            // lblSliderNormal
            // 
            resources.ApplyResources(this.lblSliderNormal, "lblSliderNormal");
            this.lblSliderNormal.Name = "lblSliderNormal";
            // 
            // lblSliderLowest
            // 
            resources.ApplyResources(this.lblSliderLowest, "lblSliderLowest");
            this.lblSliderLowest.Name = "lblSliderLowest";
            // 
            // lblPriority
            // 
            resources.ApplyResources(this.lblPriority, "lblPriority");
            this.tableLayoutPanel1.SetColumnSpan(this.lblPriority, 4);
            this.lblPriority.Name = "lblPriority";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.warningsTableLayoutPanel, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxInitialVCPUs, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelInitialVCPUs, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxTopology, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelTopology, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.MemWarningLabel, 3, 10);
            this.tableLayoutPanel1.Controls.Add(this.memorySpinner, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.lblPriority, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.lblVCPUs, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblMemory, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxVCPUs, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // warningsTableLayoutPanel
            // 
            resources.ApplyResources(this.warningsTableLayoutPanel, "warningsTableLayoutPanel");
            this.tableLayoutPanel1.SetColumnSpan(this.warningsTableLayoutPanel, 4);
            this.warningsTableLayoutPanel.Controls.Add(this.cpuWarningPictureBox, 0, 0);
            this.warningsTableLayoutPanel.Controls.Add(this.cpuWarningLabel, 1, 0);
            this.warningsTableLayoutPanel.Controls.Add(this.memoryWarningLabel, 1, 1);
            this.warningsTableLayoutPanel.Controls.Add(this.topologyWarningLabel, 0, 2);
            this.warningsTableLayoutPanel.Controls.Add(this.memoryPictureBox, 0, 1);
            this.warningsTableLayoutPanel.Controls.Add(this.topologyPictureBox, 0, 2);
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
            // topologyWarningLabel
            // 
            resources.ApplyResources(this.topologyWarningLabel, "topologyWarningLabel");
            this.topologyWarningLabel.Name = "topologyWarningLabel";
            // 
            // memoryPictureBox
            // 
            this.memoryPictureBox.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.memoryPictureBox, "memoryPictureBox");
            this.memoryPictureBox.Name = "memoryPictureBox";
            this.memoryPictureBox.TabStop = false;
            // 
            // topologyPictureBox
            // 
            this.topologyPictureBox.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.topologyPictureBox, "topologyPictureBox");
            this.topologyPictureBox.Name = "topologyPictureBox";
            this.topologyPictureBox.TabStop = false;
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
            // MemWarningLabel
            // 
            resources.ApplyResources(this.MemWarningLabel, "MemWarningLabel");
            this.MemWarningLabel.ForeColor = System.Drawing.Color.Red;
            this.MemWarningLabel.Name = "MemWarningLabel";
            this.tableLayoutPanel1.SetRowSpan(this.MemWarningLabel, 2);
            // 
            // memorySpinner
            // 
            resources.ApplyResources(this.memorySpinner, "memorySpinner");
            this.tableLayoutPanel1.SetColumnSpan(this.memorySpinner, 2);
            this.memorySpinner.Increment = 0.1D;
            this.memorySpinner.Name = "memorySpinner";
            this.memorySpinner.SpinnerValueChanged += new System.EventHandler(this.memorySpinner_SpinnerValueChanged);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 4);
            this.panel1.Controls.Add(this.lblSliderHighest);
            this.panel1.Controls.Add(this.lblSliderNormal);
            this.panel1.Controls.Add(this.lblSliderLowest);
            this.panel1.Controls.Add(this.transparentTrackBar1);
            this.panel1.Name = "panel1";
            // 
            // transparentTrackBar1
            // 
            resources.ApplyResources(this.transparentTrackBar1, "transparentTrackBar1");
            this.transparentTrackBar1.BackColor = System.Drawing.Color.Transparent;
            this.transparentTrackBar1.Name = "transparentTrackBar1";
            this.transparentTrackBar1.TabStop = false;
            // 
            // lblVCPUs
            // 
            resources.ApplyResources(this.lblVCPUs, "lblVCPUs");
            this.lblVCPUs.Name = "lblVCPUs";
            // 
            // lblMemory
            // 
            resources.ApplyResources(this.lblMemory, "lblMemory");
            this.lblMemory.Name = "lblMemory";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 4);
            this.label1.Name = "label1";
            // 
            // comboBoxVCPUs
            // 
            this.comboBoxVCPUs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVCPUs.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxVCPUs, "comboBoxVCPUs");
            this.comboBoxVCPUs.Name = "comboBoxVCPUs";
            this.comboBoxVCPUs.SelectedIndexChanged += new System.EventHandler(this.comboBoxVCPUs_SelectedIndexChanged);
            // 
            // CpuMemoryEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "CpuMemoryEditPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.warningsTableLayoutPanel.ResumeLayout(false);
            this.warningsTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cpuWarningPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.topologyPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblSliderHighest;
        private System.Windows.Forms.Label lblSliderNormal;
        private System.Windows.Forms.Label lblSliderLowest;
        private System.Windows.Forms.Label lblPriority;
        private System.Windows.Forms.Label lblVCPUs;
        private System.Windows.Forms.Label lblMemory;
        private XenAdmin.Controls.TransparentTrackBar transparentTrackBar1;
        private System.Windows.Forms.Label MemWarningLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTopology;
        private XenAdmin.Controls.CPUTopologyComboBox comboBoxTopology;
        private System.Windows.Forms.ComboBox comboBoxVCPUs;
        private System.Windows.Forms.ComboBox comboBoxInitialVCPUs;
        private System.Windows.Forms.Label labelInitialVCPUs;
        private System.Windows.Forms.TableLayoutPanel warningsTableLayoutPanel;
        private System.Windows.Forms.PictureBox cpuWarningPictureBox;
        private System.Windows.Forms.Label cpuWarningLabel;
        private System.Windows.Forms.Label memoryWarningLabel;
        private System.Windows.Forms.PictureBox memoryPictureBox;
        private System.Windows.Forms.Label topologyWarningLabel;
        private System.Windows.Forms.PictureBox topologyPictureBox;
        private Controls.Ballooning.MemorySpinner memorySpinner;
    }
}
