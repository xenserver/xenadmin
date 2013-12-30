namespace XenAdmin.SettingsPanels
{
    partial class VMHAEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VMHAEditPage));
            this.labelScanning = new System.Windows.Forms.Label();
            this.labelRestartPriority = new System.Windows.Forms.Label();
            this.groupBoxStartupOptions = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.nudOrder = new System.Windows.Forms.NumericUpDown();
            this.labelStartOrder = new System.Windows.Forms.Label();
            this.labelStartDelay = new System.Windows.Forms.Label();
            this.nudStartDelay = new System.Windows.Forms.NumericUpDown();
            this.labelStartDelayUnits = new System.Windows.Forms.Label();
            this.labelProtectionLevel = new System.Windows.Forms.Label();
            this.m_comboBoxProtectionLevel = new System.Windows.Forms.ComboBox();
            this.m_tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.m_tlpScanning = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBoxHA = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.m_tlpPriority = new System.Windows.Forms.TableLayoutPanel();
            this.comboLabel = new System.Windows.Forms.Label();
            this.m_tlpHaConfig = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.m_labelHaStatus = new System.Windows.Forms.Label();
            this.m_linkLabel = new System.Windows.Forms.LinkLabel();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.haNtolIndicator = new XenAdmin.Controls.HaNtolIndicatorSimple();
            this.groupBoxStartupOptions.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartDelay)).BeginInit();
            this.m_tlpMain.SuspendLayout();
            this.m_tlpScanning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBoxHA.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.m_tlpPriority.SuspendLayout();
            this.m_tlpHaConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // labelScanning
            // 
            resources.ApplyResources(this.labelScanning, "labelScanning");
            this.labelScanning.Name = "labelScanning";
            // 
            // labelRestartPriority
            // 
            resources.ApplyResources(this.labelRestartPriority, "labelRestartPriority");
            this.labelRestartPriority.Name = "labelRestartPriority";
            // 
            // groupBoxStartupOptions
            // 
            resources.ApplyResources(this.groupBoxStartupOptions, "groupBoxStartupOptions");
            this.groupBoxStartupOptions.Controls.Add(this.tableLayoutPanel2);
            this.groupBoxStartupOptions.Name = "groupBoxStartupOptions";
            this.groupBoxStartupOptions.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.nudOrder, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelStartOrder, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelStartDelay, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.autoHeightLabel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.nudStartDelay, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.labelStartDelayUnits, 2, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // nudOrder
            // 
            resources.ApplyResources(this.nudOrder, "nudOrder");
            this.nudOrder.Name = "nudOrder";
            // 
            // labelStartOrder
            // 
            resources.ApplyResources(this.labelStartOrder, "labelStartOrder");
            this.labelStartOrder.Name = "labelStartOrder";
            // 
            // labelStartDelay
            // 
            resources.ApplyResources(this.labelStartDelay, "labelStartDelay");
            this.labelStartDelay.Name = "labelStartDelay";
            // 
            // nudStartDelay
            // 
            resources.ApplyResources(this.nudStartDelay, "nudStartDelay");
            this.nudStartDelay.Name = "nudStartDelay";
            // 
            // labelStartDelayUnits
            // 
            resources.ApplyResources(this.labelStartDelayUnits, "labelStartDelayUnits");
            this.labelStartDelayUnits.Name = "labelStartDelayUnits";
            // 
            // labelProtectionLevel
            // 
            resources.ApplyResources(this.labelProtectionLevel, "labelProtectionLevel");
            this.labelProtectionLevel.Name = "labelProtectionLevel";
            // 
            // m_comboBoxProtectionLevel
            // 
            resources.ApplyResources(this.m_comboBoxProtectionLevel, "m_comboBoxProtectionLevel");
            this.m_comboBoxProtectionLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboBoxProtectionLevel.FormattingEnabled = true;
            this.m_comboBoxProtectionLevel.Name = "m_comboBoxProtectionLevel";
            this.m_comboBoxProtectionLevel.SelectedIndexChanged += new System.EventHandler(this.comboBoxRestartPriority_SelectedIndexChanged);
            // 
            // m_tlpMain
            // 
            resources.ApplyResources(this.m_tlpMain, "m_tlpMain");
            this.m_tlpMain.Controls.Add(this.groupBoxStartupOptions, 0, 0);
            this.m_tlpMain.Controls.Add(this.m_tlpScanning, 0, 1);
            this.m_tlpMain.Controls.Add(this.groupBoxHA, 0, 2);
            this.m_tlpMain.Name = "m_tlpMain";
            // 
            // m_tlpScanning
            // 
            resources.ApplyResources(this.m_tlpScanning, "m_tlpScanning");
            this.m_tlpScanning.Controls.Add(this.labelScanning, 1, 0);
            this.m_tlpScanning.Controls.Add(this.pictureBox1, 2, 0);
            this.m_tlpScanning.Name = "m_tlpScanning";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources.ajax_loader;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // groupBoxHA
            // 
            resources.ApplyResources(this.groupBoxHA, "groupBoxHA");
            this.groupBoxHA.Controls.Add(this.tableLayoutPanel3);
            this.groupBoxHA.Name = "groupBoxHA";
            this.groupBoxHA.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.m_tlpPriority, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.m_tlpHaConfig, 0, 1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // m_tlpPriority
            // 
            resources.ApplyResources(this.m_tlpPriority, "m_tlpPriority");
            this.m_tlpPriority.Controls.Add(this.m_comboBoxProtectionLevel, 1, 0);
            this.m_tlpPriority.Controls.Add(this.labelProtectionLevel, 0, 0);
            this.m_tlpPriority.Controls.Add(this.comboLabel, 2, 0);
            this.m_tlpPriority.Name = "m_tlpPriority";
            // 
            // comboLabel
            // 
            resources.ApplyResources(this.comboLabel, "comboLabel");
            this.comboLabel.Name = "comboLabel";
            // 
            // m_tlpHaConfig
            // 
            resources.ApplyResources(this.m_tlpHaConfig, "m_tlpHaConfig");
            this.m_tlpHaConfig.Controls.Add(this.pictureBox2, 0, 0);
            this.m_tlpHaConfig.Controls.Add(this.m_labelHaStatus, 1, 0);
            this.m_tlpHaConfig.Controls.Add(this.haNtolIndicator, 1, 1);
            this.m_tlpHaConfig.Controls.Add(this.m_linkLabel, 1, 2);
            this.m_tlpHaConfig.Name = "m_tlpHaConfig";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // m_labelHaStatus
            // 
            resources.ApplyResources(this.m_labelHaStatus, "m_labelHaStatus");
            this.m_labelHaStatus.Name = "m_labelHaStatus";
            // 
            // m_linkLabel
            // 
            resources.ApplyResources(this.m_linkLabel, "m_linkLabel");
            this.m_linkLabel.Name = "m_linkLabel";
            this.m_linkLabel.TabStop = true;
            this.m_linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.m_linkLabel_LinkClicked);
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.tableLayoutPanel2.SetColumnSpan(this.autoHeightLabel1, 3);
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // haNtolIndicator
            // 
            resources.ApplyResources(this.haNtolIndicator, "haNtolIndicator");
            this.haNtolIndicator.Name = "haNtolIndicator";
            this.haNtolIndicator.TabStop = false;
            // 
            // VMHAEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_tlpMain);
            this.Name = "VMHAEditPage";
            this.groupBoxStartupOptions.ResumeLayout(false);
            this.groupBoxStartupOptions.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartDelay)).EndInit();
            this.m_tlpMain.ResumeLayout(false);
            this.m_tlpMain.PerformLayout();
            this.m_tlpScanning.ResumeLayout(false);
            this.m_tlpScanning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBoxHA.ResumeLayout(false);
            this.groupBoxHA.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.m_tlpPriority.ResumeLayout(false);
            this.m_tlpPriority.PerformLayout();
            this.m_tlpHaConfig.ResumeLayout(false);
            this.m_tlpHaConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.ComboBox m_comboBoxProtectionLevel;
        private System.Windows.Forms.Label labelScanning;
        private System.Windows.Forms.Label labelRestartPriority;
        private System.Windows.Forms.Label labelProtectionLevel;
        private System.Windows.Forms.GroupBox groupBoxStartupOptions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label labelStartDelayUnits;
        private System.Windows.Forms.NumericUpDown nudStartDelay;
        private System.Windows.Forms.NumericUpDown nudOrder;
		private System.Windows.Forms.Label labelStartOrder;
        private System.Windows.Forms.Label labelStartDelay;
		private System.Windows.Forms.TableLayoutPanel m_tlpMain;
		private System.Windows.Forms.TableLayoutPanel m_tlpScanning;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.GroupBox groupBoxHA;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.TableLayoutPanel m_tlpPriority;
		private System.Windows.Forms.Label comboLabel;
        private System.Windows.Forms.TableLayoutPanel m_tlpHaConfig;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.LinkLabel m_linkLabel;
		private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
		private System.Windows.Forms.Label m_labelHaStatus;
		private XenAdmin.Controls.HaNtolIndicatorSimple haNtolIndicator;


    }
}
