using XenAdmin.Commands;

namespace XenAdmin.Dialogs.HealthCheck
{
    partial class HealthCheckOverviewDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HealthCheckOverviewDialog));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.poolsDataGridView = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.PoolNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.poolNameLabel = new System.Windows.Forms.Label();
            this.poolDetailsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.healthCheckStatusPanel = new System.Windows.Forms.TableLayoutPanel();
            this.failedUploadDateLabel = new System.Windows.Forms.Label();
            this.failedUploadLabel = new System.Windows.Forms.Label();
            this.lastUploadDateLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.issuesLabel = new System.Windows.Forms.Label();
            this.refreshLinkLabel = new System.Windows.Forms.LinkLabel();
            this.disableLinkLabel = new System.Windows.Forms.LinkLabel();
            this.uploadRequestLinkLabel = new System.Windows.Forms.LinkLabel();
            this.scheduleLabel = new System.Windows.Forms.Label();
            this.editLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lastUploadLabel = new System.Windows.Forms.Label();
            this.ReportAnalysisLinkLabel = new System.Windows.Forms.LinkLabel();
            this.previousUploadPanel = new System.Windows.Forms.TableLayoutPanel();
            this.viewReportLinkLabel = new System.Windows.Forms.LinkLabel();
            this.previousUploadDateLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.notEnrolledPanel = new System.Windows.Forms.TableLayoutPanel();
            this.enrollNowLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.PolicyStatementLinkLabel = new System.Windows.Forms.LinkLabel();
            this.rubricLabel = new System.Windows.Forms.Label();
            this.showAgainCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.poolsDataGridView)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.poolDetailsPanel.SuspendLayout();
            this.healthCheckStatusPanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.previousUploadPanel.SuspendLayout();
            this.notEnrolledPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.poolsDataGridView);
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel2);
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            // 
            // poolsDataGridView
            // 
            this.poolsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.poolsDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.poolsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.poolsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.poolsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PoolNameColumn,
            this.StatusColumn});
            resources.ApplyResources(this.poolsDataGridView, "poolsDataGridView");
            this.poolsDataGridView.Name = "poolsDataGridView";
            this.poolsDataGridView.ReadOnly = true;
            this.poolsDataGridView.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // PoolNameColumn
            // 
            this.PoolNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.PoolNameColumn, "PoolNameColumn");
            this.PoolNameColumn.Name = "PoolNameColumn";
            this.PoolNameColumn.ReadOnly = true;
            // 
            // StatusColumn
            // 
            this.StatusColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.StatusColumn, "StatusColumn");
            this.StatusColumn.Name = "StatusColumn";
            this.StatusColumn.ReadOnly = true;
            this.StatusColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.poolNameLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.poolDetailsPanel, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // poolNameLabel
            // 
            this.poolNameLabel.AutoEllipsis = true;
            resources.ApplyResources(this.poolNameLabel, "poolNameLabel");
            this.poolNameLabel.Name = "poolNameLabel";
            // 
            // poolDetailsPanel
            // 
            resources.ApplyResources(this.poolDetailsPanel, "poolDetailsPanel");
            this.tableLayoutPanel2.SetColumnSpan(this.poolDetailsPanel, 2);
            this.poolDetailsPanel.Controls.Add(this.healthCheckStatusPanel, 0, 0);
            this.poolDetailsPanel.Controls.Add(this.notEnrolledPanel, 0, 1);
            this.poolDetailsPanel.Name = "poolDetailsPanel";
            // 
            // healthCheckStatusPanel
            // 
            resources.ApplyResources(this.healthCheckStatusPanel, "healthCheckStatusPanel");
            this.healthCheckStatusPanel.Controls.Add(this.failedUploadDateLabel, 1, 5);
            this.healthCheckStatusPanel.Controls.Add(this.failedUploadLabel, 0, 5);
            this.healthCheckStatusPanel.Controls.Add(this.lastUploadDateLabel, 1, 1);
            this.healthCheckStatusPanel.Controls.Add(this.flowLayoutPanel1, 0, 3);
            this.healthCheckStatusPanel.Controls.Add(this.disableLinkLabel, 0, 10);
            this.healthCheckStatusPanel.Controls.Add(this.uploadRequestLinkLabel, 0, 11);
            this.healthCheckStatusPanel.Controls.Add(this.scheduleLabel, 0, 8);
            this.healthCheckStatusPanel.Controls.Add(this.editLinkLabel, 0, 9);
            this.healthCheckStatusPanel.Controls.Add(this.label4, 0, 7);
            this.healthCheckStatusPanel.Controls.Add(this.label1, 0, 0);
            this.healthCheckStatusPanel.Controls.Add(this.lastUploadLabel, 0, 1);
            this.healthCheckStatusPanel.Controls.Add(this.ReportAnalysisLinkLabel, 0, 4);
            this.healthCheckStatusPanel.Controls.Add(this.previousUploadPanel, 0, 6);
            this.healthCheckStatusPanel.Name = "healthCheckStatusPanel";
            // 
            // failedUploadDateLabel
            // 
            resources.ApplyResources(this.failedUploadDateLabel, "failedUploadDateLabel");
            this.failedUploadDateLabel.Name = "failedUploadDateLabel";
            // 
            // failedUploadLabel
            // 
            resources.ApplyResources(this.failedUploadLabel, "failedUploadLabel");
            this.failedUploadLabel.Name = "failedUploadLabel";
            // 
            // lastUploadDateLabel
            // 
            resources.ApplyResources(this.lastUploadDateLabel, "lastUploadDateLabel");
            this.lastUploadDateLabel.Name = "lastUploadDateLabel";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.healthCheckStatusPanel.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.issuesLabel);
            this.flowLayoutPanel1.Controls.Add(this.refreshLinkLabel);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // issuesLabel
            // 
            resources.ApplyResources(this.issuesLabel, "issuesLabel");
            this.issuesLabel.Name = "issuesLabel";
            // 
            // refreshLinkLabel
            // 
            resources.ApplyResources(this.refreshLinkLabel, "refreshLinkLabel");
            this.refreshLinkLabel.Name = "refreshLinkLabel";
            this.refreshLinkLabel.TabStop = true;
            this.refreshLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.refreshLinkLabel_LinkClicked);
            // 
            // disableLinkLabel
            // 
            resources.ApplyResources(this.disableLinkLabel, "disableLinkLabel");
            this.healthCheckStatusPanel.SetColumnSpan(this.disableLinkLabel, 2);
            this.disableLinkLabel.Name = "disableLinkLabel";
            this.disableLinkLabel.TabStop = true;
            this.disableLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.disableLinkLabel_LinkClicked);
            // 
            // uploadRequestLinkLabel
            // 
            resources.ApplyResources(this.uploadRequestLinkLabel, "uploadRequestLinkLabel");
            this.healthCheckStatusPanel.SetColumnSpan(this.uploadRequestLinkLabel, 2);
            this.uploadRequestLinkLabel.Name = "uploadRequestLinkLabel";
            this.uploadRequestLinkLabel.TabStop = true;
            this.uploadRequestLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uploadRequestLinkLabel_LinkClicked);
            // 
            // scheduleLabel
            // 
            resources.ApplyResources(this.scheduleLabel, "scheduleLabel");
            this.healthCheckStatusPanel.SetColumnSpan(this.scheduleLabel, 2);
            this.scheduleLabel.Name = "scheduleLabel";
            // 
            // editLinkLabel
            // 
            resources.ApplyResources(this.editLinkLabel, "editLinkLabel");
            this.healthCheckStatusPanel.SetColumnSpan(this.editLinkLabel, 2);
            this.editLinkLabel.Name = "editLinkLabel";
            this.editLinkLabel.TabStop = true;
            this.editLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.editlinkLabel_LinkClicked);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.healthCheckStatusPanel.SetColumnSpan(this.label4, 2);
            this.label4.Name = "label4";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.healthCheckStatusPanel.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // lastUploadLabel
            // 
            resources.ApplyResources(this.lastUploadLabel, "lastUploadLabel");
            this.lastUploadLabel.Name = "lastUploadLabel";
            // 
            // ReportAnalysisLinkLabel
            // 
            resources.ApplyResources(this.ReportAnalysisLinkLabel, "ReportAnalysisLinkLabel");
            this.healthCheckStatusPanel.SetColumnSpan(this.ReportAnalysisLinkLabel, 2);
            this.ReportAnalysisLinkLabel.Name = "ReportAnalysisLinkLabel";
            this.ReportAnalysisLinkLabel.TabStop = true;
            this.ReportAnalysisLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ReportAnalysisLinkLabel_LinkClicked);
            // 
            // previousUploadPanel
            // 
            resources.ApplyResources(this.previousUploadPanel, "previousUploadPanel");
            this.healthCheckStatusPanel.SetColumnSpan(this.previousUploadPanel, 2);
            this.previousUploadPanel.Controls.Add(this.viewReportLinkLabel, 0, 1);
            this.previousUploadPanel.Controls.Add(this.previousUploadDateLabel, 1, 0);
            this.previousUploadPanel.Controls.Add(this.label3, 0, 0);
            this.previousUploadPanel.Name = "previousUploadPanel";
            // 
            // viewReportLinkLabel
            // 
            resources.ApplyResources(this.viewReportLinkLabel, "viewReportLinkLabel");
            this.previousUploadPanel.SetColumnSpan(this.viewReportLinkLabel, 2);
            this.viewReportLinkLabel.Name = "viewReportLinkLabel";
            this.viewReportLinkLabel.TabStop = true;
            this.viewReportLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.viewReportLinkLabel_LinkClicked);
            // 
            // previousUploadDateLabel
            // 
            resources.ApplyResources(this.previousUploadDateLabel, "previousUploadDateLabel");
            this.previousUploadDateLabel.Name = "previousUploadDateLabel";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // notEnrolledPanel
            // 
            resources.ApplyResources(this.notEnrolledPanel, "notEnrolledPanel");
            this.poolDetailsPanel.SetColumnSpan(this.notEnrolledPanel, 2);
            this.notEnrolledPanel.Controls.Add(this.enrollNowLinkLabel, 0, 1);
            this.notEnrolledPanel.Controls.Add(this.label6, 0, 0);
            this.notEnrolledPanel.Name = "notEnrolledPanel";
            // 
            // enrollNowLinkLabel
            // 
            resources.ApplyResources(this.enrollNowLinkLabel, "enrollNowLinkLabel");
            this.enrollNowLinkLabel.Name = "enrollNowLinkLabel";
            this.enrollNowLinkLabel.TabStop = true;
            this.enrollNowLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.enrollNowLinkLabel_LinkClicked);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.PolicyStatementLinkLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.rubricLabel, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // PolicyStatementLinkLabel
            // 
            resources.ApplyResources(this.PolicyStatementLinkLabel, "PolicyStatementLinkLabel");
            this.PolicyStatementLinkLabel.Name = "PolicyStatementLinkLabel";
            this.PolicyStatementLinkLabel.TabStop = true;
            this.PolicyStatementLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PolicyStatementLinkLabel_LinkClicked);
            // 
            // rubricLabel
            // 
            resources.ApplyResources(this.rubricLabel, "rubricLabel");
            this.rubricLabel.Name = "rubricLabel";
            // 
            // showAgainCheckBox
            // 
            resources.ApplyResources(this.showAgainCheckBox, "showAgainCheckBox");
            this.showAgainCheckBox.Name = "showAgainCheckBox";
            this.showAgainCheckBox.UseVisualStyleBackColor = true;
            this.showAgainCheckBox.CheckedChanged += new System.EventHandler(this.showAgainCheckBox_CheckedChanged);
            // 
            // HealthCheckOverviewDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.showAgainCheckBox);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "HealthCheckOverviewDialog";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HealthCheckOOverviewDialog_FormClosed);
            this.Load += new System.EventHandler(this.HealthCheckOverviewDialog_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.poolsDataGridView)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.poolDetailsPanel.ResumeLayout(false);
            this.poolDetailsPanel.PerformLayout();
            this.healthCheckStatusPanel.ResumeLayout(false);
            this.healthCheckStatusPanel.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.previousUploadPanel.ResumeLayout(false);
            this.previousUploadPanel.PerformLayout();
            this.notEnrolledPanel.ResumeLayout(false);
            this.notEnrolledPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx poolsDataGridView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label poolNameLabel;
        private System.Windows.Forms.Label rubricLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn PoolNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusColumn;
        private System.Windows.Forms.TableLayoutPanel healthCheckStatusPanel;
        private System.Windows.Forms.Label issuesLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lastUploadLabel;
        private System.Windows.Forms.LinkLabel ReportAnalysisLinkLabel;
        private System.Windows.Forms.TableLayoutPanel previousUploadPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label previousUploadDateLabel;
        private System.Windows.Forms.LinkLabel editLinkLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label scheduleLabel;
        private System.Windows.Forms.TableLayoutPanel notEnrolledPanel;
        private System.Windows.Forms.LinkLabel enrollNowLinkLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel poolDetailsPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.LinkLabel PolicyStatementLinkLabel;
        private System.Windows.Forms.LinkLabel uploadRequestLinkLabel;
        private System.Windows.Forms.CheckBox showAgainCheckBox;
        private System.Windows.Forms.LinkLabel disableLinkLabel;
        private System.Windows.Forms.LinkLabel viewReportLinkLabel;
        private System.Windows.Forms.LinkLabel refreshLinkLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lastUploadDateLabel;
        private System.Windows.Forms.Label failedUploadDateLabel;
        private System.Windows.Forms.Label failedUploadLabel;
    }
}

