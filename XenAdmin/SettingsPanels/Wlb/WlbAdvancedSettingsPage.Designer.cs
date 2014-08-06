namespace XenAdmin.SettingsPanels
{
    partial class WlbAdvancedSettingsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbAdvancedSettingsPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelOptAgr = new System.Windows.Forms.Label();
            this.labelHistData = new System.Windows.Forms.Label();
            this.labelRepSub = new System.Windows.Forms.Label();
            this.labelVmMigInt = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelRecSev = new System.Windows.Forms.Label();
            this.panelVmMigInt = new System.Windows.Forms.FlowLayoutPanel();
            this.numericUpDownRelocationInterval = new System.Windows.Forms.NumericUpDown();
            this.labelRelocationUnit = new System.Windows.Forms.Label();
            this.labelRelocationDefault = new System.Windows.Forms.Label();
            this.labelRecCnt = new System.Windows.Forms.Label();
            this.panelRecCnt = new System.Windows.Forms.FlowLayoutPanel();
            this.numericUpDownPollInterval = new System.Windows.Forms.NumericUpDown();
            this.labelRecommendationIntervalUnit = new System.Windows.Forms.Label();
            this.labelRecommendationIntervalDefault = new System.Windows.Forms.Label();
            this.panelRecSev = new System.Windows.Forms.FlowLayoutPanel();
            this.comboBoxOptimizationSeverity = new System.Windows.Forms.ComboBox();
            this.labelRecommendationSeverityDefault = new System.Windows.Forms.Label();
            this.panelOptAgr = new System.Windows.Forms.FlowLayoutPanel();
            this.comboBoxAutoBalanceAggressiveness = new System.Windows.Forms.ComboBox();
            this.labelAutoBalanceAggressivenessDefault = new System.Windows.Forms.Label();
            this.panelHistData = new System.Windows.Forms.FlowLayoutPanel();
            this.numericUpDownGroomingPeriod = new System.Windows.Forms.NumericUpDown();
            this.labelGroomingUnits = new System.Windows.Forms.Label();
            this.labelGroomingDefault = new System.Windows.Forms.Label();
            this.panelRepSub = new System.Windows.Forms.FlowLayoutPanel();
            this.labelSMTPServer = new System.Windows.Forms.Label();
            this.textBoxSMTPServer = new System.Windows.Forms.TextBox();
            this.LabelSmtpPort = new System.Windows.Forms.Label();
            this.TextBoxSMTPServerPort = new System.Windows.Forms.TextBox();
            this.sectionHeaderLabelRepSub = new XenAdmin.Controls.SectionHeaderLabel();
            this.sectionHeaderLabelHistData = new XenAdmin.Controls.SectionHeaderLabel();
            this.sectionHeaderLabelOptAgr = new XenAdmin.Controls.SectionHeaderLabel();
            this.sectionHeaderLabelRecSev = new XenAdmin.Controls.SectionHeaderLabel();
            this.sectionHeaderLabelVmMigInt = new XenAdmin.Controls.SectionHeaderLabel();
            this.sectionHeaderLabelRecCnt = new XenAdmin.Controls.SectionHeaderLabel();
            this.labelAuditTrail = new System.Windows.Forms.Label();
            this.sectionHeaderLabelAuditTrail = new XenAdmin.Controls.SectionHeaderLabel();
            this.comboBoxPoolAuditTrailLevel = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.auditTrailPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.poolAuditTrailNote = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelVmMigInt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRelocationInterval)).BeginInit();
            this.panelRecCnt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPollInterval)).BeginInit();
            this.panelRecSev.SuspendLayout();
            this.panelOptAgr.SuspendLayout();
            this.panelHistData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGroomingPeriod)).BeginInit();
            this.panelRepSub.SuspendLayout();
            this.auditTrailPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelAuditTrail, 0, 23);
            this.tableLayoutPanel1.Controls.Add(this.sectionHeaderLabelAuditTrail, 0, 22);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 21);
            this.tableLayoutPanel1.Controls.Add(this.sectionHeaderLabelRepSub, 0, 16);
            this.tableLayoutPanel1.Controls.Add(this.sectionHeaderLabelHistData, 0, 13);
            this.tableLayoutPanel1.Controls.Add(this.sectionHeaderLabelOptAgr, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.labelOptAgr, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.labelHistData, 0, 14);
            this.tableLayoutPanel1.Controls.Add(this.sectionHeaderLabelRecSev, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelRepSub, 0, 17);
            this.tableLayoutPanel1.Controls.Add(this.sectionHeaderLabelVmMigInt, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelVmMigInt, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelRecSev, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.panelVmMigInt, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.sectionHeaderLabelRecCnt, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelRecCnt, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.panelRecCnt, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.panelRecSev, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.panelOptAgr, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.panelHistData, 0, 15);
            this.tableLayoutPanel1.Controls.Add(this.panelRepSub, 0, 18);
            this.tableLayoutPanel1.Controls.Add(this.auditTrailPanel, 0, 24);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 20);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelAuditTrail
            // 
            resources.ApplyResources(this.labelAuditTrail, "labelAuditTrail");
            this.labelAuditTrail.BackColor = System.Drawing.Color.Transparent;
            this.labelAuditTrail.Name = "labelAuditTrail";
            // 
            // sectionHeaderLabelAuditTrail
            // 
            resources.ApplyResources(this.sectionHeaderLabelAuditTrail, "sectionHeaderLabelAuditTrail");
            this.sectionHeaderLabelAuditTrail.FocusControl = this.comboBoxPoolAuditTrailLevel;
            this.sectionHeaderLabelAuditTrail.LineColor = System.Drawing.SystemColors.ActiveBorder;
            this.sectionHeaderLabelAuditTrail.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionHeaderLabelAuditTrail.Name = "sectionHeaderLabelAuditTrail";
            // 
            // comboBoxPoolAuditTrailLevel
            // 
            this.comboBoxPoolAuditTrailLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPoolAuditTrailLevel.FormattingEnabled = true;
            this.comboBoxPoolAuditTrailLevel.Items.AddRange(new object[] {
            resources.GetString("comboBoxPoolAuditTrailLevel.Items"),
            resources.GetString("comboBoxPoolAuditTrailLevel.Items1"),
            resources.GetString("comboBoxPoolAuditTrailLevel.Items2")});
            resources.ApplyResources(this.comboBoxPoolAuditTrailLevel, "comboBoxPoolAuditTrailLevel");
            this.comboBoxPoolAuditTrailLevel.Name = "comboBoxPoolAuditTrailLevel";
            this.comboBoxPoolAuditTrailLevel.SelectedIndexChanged += new System.EventHandler(this.comboBoxPoolAuditTrailLevel_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // labelOptAgr
            // 
            resources.ApplyResources(this.labelOptAgr, "labelOptAgr");
            this.labelOptAgr.Name = "labelOptAgr";
            // 
            // labelHistData
            // 
            resources.ApplyResources(this.labelHistData, "labelHistData");
            this.labelHistData.BackColor = System.Drawing.Color.Transparent;
            this.labelHistData.MaximumSize = new System.Drawing.Size(533, 45);
            this.labelHistData.Name = "labelHistData";
            // 
            // labelRepSub
            // 
            resources.ApplyResources(this.labelRepSub, "labelRepSub");
            this.labelRepSub.BackColor = System.Drawing.Color.Transparent;
            this.labelRepSub.Name = "labelRepSub";
            // 
            // labelVmMigInt
            // 
            resources.ApplyResources(this.labelVmMigInt, "labelVmMigInt");
            this.labelVmMigInt.BackColor = System.Drawing.Color.Transparent;
            this.labelVmMigInt.Name = "labelVmMigInt";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // labelRecSev
            // 
            resources.ApplyResources(this.labelRecSev, "labelRecSev");
            this.labelRecSev.Name = "labelRecSev";
            // 
            // panelVmMigInt
            // 
            resources.ApplyResources(this.panelVmMigInt, "panelVmMigInt");
            this.panelVmMigInt.Controls.Add(this.numericUpDownRelocationInterval);
            this.panelVmMigInt.Controls.Add(this.labelRelocationUnit);
            this.panelVmMigInt.Controls.Add(this.labelRelocationDefault);
            this.panelVmMigInt.Name = "panelVmMigInt";
            // 
            // numericUpDownRelocationInterval
            // 
            resources.ApplyResources(this.numericUpDownRelocationInterval, "numericUpDownRelocationInterval");
            this.numericUpDownRelocationInterval.Maximum = new decimal(new int[] {
            260,
            0,
            0,
            0});
            this.numericUpDownRelocationInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRelocationInterval.Name = "numericUpDownRelocationInterval";
            this.numericUpDownRelocationInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRelocationInterval.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            this.numericUpDownRelocationInterval.KeyUp += new System.Windows.Forms.KeyEventHandler(this.numericUpDownRelocationInterval_KeyUp);
            // 
            // labelRelocationUnit
            // 
            resources.ApplyResources(this.labelRelocationUnit, "labelRelocationUnit");
            this.labelRelocationUnit.Name = "labelRelocationUnit";
            // 
            // labelRelocationDefault
            // 
            resources.ApplyResources(this.labelRelocationDefault, "labelRelocationDefault");
            this.labelRelocationDefault.ForeColor = System.Drawing.Color.Gray;
            this.labelRelocationDefault.Name = "labelRelocationDefault";
            // 
            // labelRecCnt
            // 
            resources.ApplyResources(this.labelRecCnt, "labelRecCnt");
            this.labelRecCnt.Name = "labelRecCnt";
            // 
            // panelRecCnt
            // 
            resources.ApplyResources(this.panelRecCnt, "panelRecCnt");
            this.panelRecCnt.Controls.Add(this.numericUpDownPollInterval);
            this.panelRecCnt.Controls.Add(this.labelRecommendationIntervalUnit);
            this.panelRecCnt.Controls.Add(this.labelRecommendationIntervalDefault);
            this.panelRecCnt.Name = "panelRecCnt";
            // 
            // numericUpDownPollInterval
            // 
            resources.ApplyResources(this.numericUpDownPollInterval, "numericUpDownPollInterval");
            this.numericUpDownPollInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownPollInterval.Name = "numericUpDownPollInterval";
            this.numericUpDownPollInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownPollInterval.ValueChanged += new System.EventHandler(this.numericUpDownPollInterval_ValueChanged);
            // 
            // labelRecommendationIntervalUnit
            // 
            resources.ApplyResources(this.labelRecommendationIntervalUnit, "labelRecommendationIntervalUnit");
            this.labelRecommendationIntervalUnit.Name = "labelRecommendationIntervalUnit";
            // 
            // labelRecommendationIntervalDefault
            // 
            resources.ApplyResources(this.labelRecommendationIntervalDefault, "labelRecommendationIntervalDefault");
            this.labelRecommendationIntervalDefault.ForeColor = System.Drawing.Color.Gray;
            this.labelRecommendationIntervalDefault.Name = "labelRecommendationIntervalDefault";
            // 
            // panelRecSev
            // 
            resources.ApplyResources(this.panelRecSev, "panelRecSev");
            this.panelRecSev.Controls.Add(this.comboBoxOptimizationSeverity);
            this.panelRecSev.Controls.Add(this.labelRecommendationSeverityDefault);
            this.panelRecSev.Name = "panelRecSev";
            // 
            // comboBoxOptimizationSeverity
            // 
            this.comboBoxOptimizationSeverity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOptimizationSeverity.FormattingEnabled = true;
            this.comboBoxOptimizationSeverity.Items.AddRange(new object[] {
            resources.GetString("comboBoxOptimizationSeverity.Items"),
            resources.GetString("comboBoxOptimizationSeverity.Items1"),
            resources.GetString("comboBoxOptimizationSeverity.Items2"),
            resources.GetString("comboBoxOptimizationSeverity.Items3")});
            resources.ApplyResources(this.comboBoxOptimizationSeverity, "comboBoxOptimizationSeverity");
            this.comboBoxOptimizationSeverity.Name = "comboBoxOptimizationSeverity";
            this.comboBoxOptimizationSeverity.SelectedIndexChanged += new System.EventHandler(this.comboBoxOptimizationSeverity_SelectedIndexChanged);
            // 
            // labelRecommendationSeverityDefault
            // 
            resources.ApplyResources(this.labelRecommendationSeverityDefault, "labelRecommendationSeverityDefault");
            this.labelRecommendationSeverityDefault.ForeColor = System.Drawing.Color.Gray;
            this.labelRecommendationSeverityDefault.Name = "labelRecommendationSeverityDefault";
            // 
            // panelOptAgr
            // 
            resources.ApplyResources(this.panelOptAgr, "panelOptAgr");
            this.panelOptAgr.Controls.Add(this.comboBoxAutoBalanceAggressiveness);
            this.panelOptAgr.Controls.Add(this.labelAutoBalanceAggressivenessDefault);
            this.panelOptAgr.Name = "panelOptAgr";
            // 
            // comboBoxAutoBalanceAggressiveness
            // 
            this.comboBoxAutoBalanceAggressiveness.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAutoBalanceAggressiveness.FormattingEnabled = true;
            this.comboBoxAutoBalanceAggressiveness.Items.AddRange(new object[] {
            resources.GetString("comboBoxAutoBalanceAggressiveness.Items"),
            resources.GetString("comboBoxAutoBalanceAggressiveness.Items1"),
            resources.GetString("comboBoxAutoBalanceAggressiveness.Items2")});
            resources.ApplyResources(this.comboBoxAutoBalanceAggressiveness, "comboBoxAutoBalanceAggressiveness");
            this.comboBoxAutoBalanceAggressiveness.Name = "comboBoxAutoBalanceAggressiveness";
            this.comboBoxAutoBalanceAggressiveness.SelectedIndexChanged += new System.EventHandler(this.comboBoxAutoBalanceAggressiveness_SelectedIndexChanged);
            // 
            // labelAutoBalanceAggressivenessDefault
            // 
            resources.ApplyResources(this.labelAutoBalanceAggressivenessDefault, "labelAutoBalanceAggressivenessDefault");
            this.labelAutoBalanceAggressivenessDefault.ForeColor = System.Drawing.Color.Gray;
            this.labelAutoBalanceAggressivenessDefault.Name = "labelAutoBalanceAggressivenessDefault";
            // 
            // panelHistData
            // 
            resources.ApplyResources(this.panelHistData, "panelHistData");
            this.panelHistData.Controls.Add(this.numericUpDownGroomingPeriod);
            this.panelHistData.Controls.Add(this.labelGroomingUnits);
            this.panelHistData.Controls.Add(this.labelGroomingDefault);
            this.panelHistData.Name = "panelHistData";
            // 
            // numericUpDownGroomingPeriod
            // 
            resources.ApplyResources(this.numericUpDownGroomingPeriod, "numericUpDownGroomingPeriod");
            this.numericUpDownGroomingPeriod.Maximum = new decimal(new int[] {
            260,
            0,
            0,
            0});
            this.numericUpDownGroomingPeriod.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownGroomingPeriod.Name = "numericUpDownGroomingPeriod";
            this.numericUpDownGroomingPeriod.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownGroomingPeriod.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // labelGroomingUnits
            // 
            resources.ApplyResources(this.labelGroomingUnits, "labelGroomingUnits");
            this.labelGroomingUnits.Name = "labelGroomingUnits";
            // 
            // labelGroomingDefault
            // 
            resources.ApplyResources(this.labelGroomingDefault, "labelGroomingDefault");
            this.labelGroomingDefault.ForeColor = System.Drawing.Color.Gray;
            this.labelGroomingDefault.Name = "labelGroomingDefault";
            // 
            // panelRepSub
            // 
            resources.ApplyResources(this.panelRepSub, "panelRepSub");
            this.panelRepSub.Controls.Add(this.labelSMTPServer);
            this.panelRepSub.Controls.Add(this.textBoxSMTPServer);
            this.panelRepSub.Controls.Add(this.LabelSmtpPort);
            this.panelRepSub.Controls.Add(this.TextBoxSMTPServerPort);
            this.panelRepSub.Name = "panelRepSub";
            // 
            // labelSMTPServer
            // 
            resources.ApplyResources(this.labelSMTPServer, "labelSMTPServer");
            this.labelSMTPServer.Name = "labelSMTPServer";
            // 
            // textBoxSMTPServer
            // 
            resources.ApplyResources(this.textBoxSMTPServer, "textBoxSMTPServer");
            this.textBoxSMTPServer.Name = "textBoxSMTPServer";
            this.textBoxSMTPServer.TextChanged += new System.EventHandler(this.textBoxSMTPServer_TextChanged);
            // 
            // LabelSmtpPort
            // 
            resources.ApplyResources(this.LabelSmtpPort, "LabelSmtpPort");
            this.LabelSmtpPort.Name = "LabelSmtpPort";
            // 
            // TextBoxSMTPServerPort
            // 
            resources.ApplyResources(this.TextBoxSMTPServerPort, "TextBoxSMTPServerPort");
            this.TextBoxSMTPServerPort.Name = "TextBoxSMTPServerPort";
            // 
            // sectionHeaderLabelRepSub
            // 
            resources.ApplyResources(this.sectionHeaderLabelRepSub, "sectionHeaderLabelRepSub");
            this.sectionHeaderLabelRepSub.FocusControl = this.textBoxSMTPServer;
            this.sectionHeaderLabelRepSub.LineColor = System.Drawing.SystemColors.ActiveBorder;
            this.sectionHeaderLabelRepSub.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionHeaderLabelRepSub.MinimumSize = new System.Drawing.Size(0, 14);
            this.sectionHeaderLabelRepSub.Name = "sectionHeaderLabelRepSub";
            // 
            // sectionHeaderLabelHistData
            // 
            resources.ApplyResources(this.sectionHeaderLabelHistData, "sectionHeaderLabelHistData");
            this.sectionHeaderLabelHistData.FocusControl = this.numericUpDownGroomingPeriod;
            this.sectionHeaderLabelHistData.LineColor = System.Drawing.SystemColors.ActiveBorder;
            this.sectionHeaderLabelHistData.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionHeaderLabelHistData.MinimumSize = new System.Drawing.Size(0, 14);
            this.sectionHeaderLabelHistData.Name = "sectionHeaderLabelHistData";
            // 
            // sectionHeaderLabelOptAgr
            // 
            resources.ApplyResources(this.sectionHeaderLabelOptAgr, "sectionHeaderLabelOptAgr");
            this.sectionHeaderLabelOptAgr.FocusControl = this.comboBoxAutoBalanceAggressiveness;
            this.sectionHeaderLabelOptAgr.LineColor = System.Drawing.SystemColors.ActiveBorder;
            this.sectionHeaderLabelOptAgr.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionHeaderLabelOptAgr.MinimumSize = new System.Drawing.Size(0, 14);
            this.sectionHeaderLabelOptAgr.Name = "sectionHeaderLabelOptAgr";
            // 
            // sectionHeaderLabelRecSev
            // 
            resources.ApplyResources(this.sectionHeaderLabelRecSev, "sectionHeaderLabelRecSev");
            this.sectionHeaderLabelRecSev.FocusControl = this.comboBoxOptimizationSeverity;
            this.sectionHeaderLabelRecSev.LineColor = System.Drawing.SystemColors.ActiveBorder;
            this.sectionHeaderLabelRecSev.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionHeaderLabelRecSev.MinimumSize = new System.Drawing.Size(0, 14);
            this.sectionHeaderLabelRecSev.Name = "sectionHeaderLabelRecSev";
            // 
            // sectionHeaderLabelVmMigInt
            // 
            resources.ApplyResources(this.sectionHeaderLabelVmMigInt, "sectionHeaderLabelVmMigInt");
            this.sectionHeaderLabelVmMigInt.FocusControl = this.numericUpDownRelocationInterval;
            this.sectionHeaderLabelVmMigInt.LineColor = System.Drawing.SystemColors.ActiveBorder;
            this.sectionHeaderLabelVmMigInt.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionHeaderLabelVmMigInt.MinimumSize = new System.Drawing.Size(0, 14);
            this.sectionHeaderLabelVmMigInt.Name = "sectionHeaderLabelVmMigInt";
            // 
            // sectionHeaderLabelRecCnt
            // 
            resources.ApplyResources(this.sectionHeaderLabelRecCnt, "sectionHeaderLabelRecCnt");
            this.sectionHeaderLabelRecCnt.FocusControl = this.numericUpDownPollInterval;
            this.sectionHeaderLabelRecCnt.LineColor = System.Drawing.SystemColors.ActiveBorder;
            this.sectionHeaderLabelRecCnt.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionHeaderLabelRecCnt.MinimumSize = new System.Drawing.Size(0, 14);
            this.sectionHeaderLabelRecCnt.Name = "sectionHeaderLabelRecCnt";
            // 
            // auditTrailPanel
            // 
            this.auditTrailPanel.Controls.Add(this.comboBoxPoolAuditTrailLevel);
            this.auditTrailPanel.Controls.Add(this.poolAuditTrailNote);
            resources.ApplyResources(this.auditTrailPanel, "auditTrailPanel");
            this.auditTrailPanel.Name = "auditTrailPanel";
            // 
            // poolAuditTrailNote
            // 
            resources.ApplyResources(this.poolAuditTrailNote, "poolAuditTrailNote");
            this.poolAuditTrailNote.ForeColor = System.Drawing.Color.Gray;
            this.poolAuditTrailNote.Name = "poolAuditTrailNote";
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // WlbAdvancedSettingsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(560, 560);
            this.Name = "WlbAdvancedSettingsPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panelVmMigInt.ResumeLayout(false);
            this.panelVmMigInt.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRelocationInterval)).EndInit();
            this.panelRecCnt.ResumeLayout(false);
            this.panelRecCnt.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPollInterval)).EndInit();
            this.panelRecSev.ResumeLayout(false);
            this.panelRecSev.PerformLayout();
            this.panelOptAgr.ResumeLayout(false);
            this.panelOptAgr.PerformLayout();
            this.panelHistData.ResumeLayout(false);
            this.panelHistData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGroomingPeriod)).EndInit();
            this.panelRepSub.ResumeLayout(false);
            this.panelRepSub.PerformLayout();
            this.auditTrailPanel.ResumeLayout(false);
            this.auditTrailPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelHistData;
        private System.Windows.Forms.Label labelGroomingUnits;
        private System.Windows.Forms.Label labelGroomingDefault;
        private System.Windows.Forms.NumericUpDown numericUpDownGroomingPeriod;
        private System.Windows.Forms.Label labelRepSub;
        private System.Windows.Forms.Label labelSMTPServer;
        private System.Windows.Forms.TextBox textBoxSMTPServer;
        private System.Windows.Forms.ComboBox comboBoxOptimizationSeverity;
        private System.Windows.Forms.ComboBox comboBoxAutoBalanceAggressiveness;
        private System.Windows.Forms.Label labelRecSev;
        private System.Windows.Forms.Label labelRecommendationIntervalUnit;
        private System.Windows.Forms.NumericUpDown numericUpDownPollInterval;
        private System.Windows.Forms.Label labelRecCnt;
        private System.Windows.Forms.Label labelRecommendationSeverityDefault;
        private System.Windows.Forms.Label labelRecommendationIntervalDefault;
        private System.Windows.Forms.Label labelOptAgr;
        private System.Windows.Forms.Label labelAutoBalanceAggressivenessDefault;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LabelSmtpPort;
        private System.Windows.Forms.TextBox TextBoxSMTPServerPort;
        private System.Windows.Forms.FlowLayoutPanel panelVmMigInt;
        private System.Windows.Forms.Label labelVmMigInt;
        private System.Windows.Forms.NumericUpDown numericUpDownRelocationInterval;
        private System.Windows.Forms.Label labelRelocationUnit;
        private System.Windows.Forms.Label labelRelocationDefault;
        private XenAdmin.Controls.SectionHeaderLabel sectionHeaderLabelVmMigInt;
        private XenAdmin.Controls.SectionHeaderLabel sectionHeaderLabelRecCnt;
        private XenAdmin.Controls.SectionHeaderLabel sectionHeaderLabelRecSev;
        private System.Windows.Forms.FlowLayoutPanel panelRecCnt;
        private System.Windows.Forms.FlowLayoutPanel panelRecSev;
        private XenAdmin.Controls.SectionHeaderLabel sectionHeaderLabelOptAgr;
        private System.Windows.Forms.FlowLayoutPanel panelOptAgr;
        private XenAdmin.Controls.SectionHeaderLabel sectionHeaderLabelRepSub;
        private XenAdmin.Controls.SectionHeaderLabel sectionHeaderLabelHistData;
        private System.Windows.Forms.FlowLayoutPanel panelHistData;
        private System.Windows.Forms.FlowLayoutPanel panelRepSub;
        private System.Windows.Forms.Label labelAuditTrail;
        private Controls.SectionHeaderLabel sectionHeaderLabelAuditTrail;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel auditTrailPanel;
        private System.Windows.Forms.ComboBox comboBoxPoolAuditTrailLevel;
        private System.Windows.Forms.Label poolAuditTrailNote;
        private System.Windows.Forms.Label label3;
    }
}
