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
            this.topologyWarningLabel = new System.Windows.Forms.Label();
            this.topologyPictureBox = new System.Windows.Forms.PictureBox();
            this.comboBoxInitialVCPUs = new System.Windows.Forms.ComboBox();
            this.labelInitialVCPUs = new System.Windows.Forms.Label();
            this.comboBoxTopology = new XenAdmin.Controls.CPUTopologyComboBox();
            this.labelTopology = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.transparentTrackBar1 = new XenAdmin.Controls.TransparentTrackBar();
            this.lblVCPUs = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxVCPUs = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanelInfo = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelInfo = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.warningsTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cpuWarningPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topologyPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanelInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.tableLayoutPanel1.SetColumnSpan(this.lblPriority, 3);
            this.lblPriority.Name = "lblPriority";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.warningsTableLayoutPanel, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxInitialVCPUs, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelInitialVCPUs, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxTopology, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelTopology, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.lblPriority, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.lblVCPUs, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxVCPUs, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelInfo, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // warningsTableLayoutPanel
            // 
            resources.ApplyResources(this.warningsTableLayoutPanel, "warningsTableLayoutPanel");
            this.tableLayoutPanel1.SetColumnSpan(this.warningsTableLayoutPanel, 3);
            this.warningsTableLayoutPanel.Controls.Add(this.cpuWarningPictureBox, 0, 0);
            this.warningsTableLayoutPanel.Controls.Add(this.cpuWarningLabel, 1, 0);
            this.warningsTableLayoutPanel.Controls.Add(this.topologyWarningLabel, 0, 2);
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
            // topologyWarningLabel
            // 
            resources.ApplyResources(this.topologyWarningLabel, "topologyWarningLabel");
            this.topologyWarningLabel.Name = "topologyWarningLabel";
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
            this.tableLayoutPanel1.SetColumnSpan(this.comboBoxTopology, 2);
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
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 3);
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
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
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
            // tableLayoutPanelInfo
            // 
            resources.ApplyResources(this.tableLayoutPanelInfo, "tableLayoutPanelInfo");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelInfo, 3);
            this.tableLayoutPanelInfo.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanelInfo.Controls.Add(this.labelInfo, 1, 0);
            this.tableLayoutPanelInfo.Name = "tableLayoutPanelInfo";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // labelInfo
            // 
            resources.ApplyResources(this.labelInfo, "labelInfo");
            this.labelInfo.Name = "labelInfo";
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
            ((System.ComponentModel.ISupportInitialize)(this.topologyPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanelInfo.ResumeLayout(false);
            this.tableLayoutPanelInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private XenAdmin.Controls.TransparentTrackBar transparentTrackBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTopology;
        private XenAdmin.Controls.CPUTopologyComboBox comboBoxTopology;
        private System.Windows.Forms.ComboBox comboBoxVCPUs;
        private System.Windows.Forms.ComboBox comboBoxInitialVCPUs;
        private System.Windows.Forms.Label labelInitialVCPUs;
        private System.Windows.Forms.TableLayoutPanel warningsTableLayoutPanel;
        private System.Windows.Forms.PictureBox cpuWarningPictureBox;
        private System.Windows.Forms.Label cpuWarningLabel;
        private System.Windows.Forms.Label topologyWarningLabel;
        private System.Windows.Forms.PictureBox topologyPictureBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelInfo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelInfo;
    }
}
