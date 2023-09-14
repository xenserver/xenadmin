using System.Windows.Forms;
using DataGridView = System.Windows.Forms.DataGridView;

namespace XenAdmin.SettingsPanels
{
    partial class NRPEEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NRPEEditPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.NRPETableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.PoolTipsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.PoolTipsPicture = new System.Windows.Forms.PictureBox();
            this.PoolTipsLabel = new System.Windows.Forms.Label();
            this.DescLabel = new System.Windows.Forms.Label();
            this.GeneralConfigureGroupBox = new System.Windows.Forms.GroupBox();
            this.GeneralConfigTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.EnableNRPECheckBox = new System.Windows.Forms.CheckBox();
            this.AllowHostsLabel = new System.Windows.Forms.Label();
            this.AllowHostsTextBox = new System.Windows.Forms.TextBox();
            this.DebugLogCheckBox = new System.Windows.Forms.CheckBox();
            this.SslDebugLogCheckBox = new System.Windows.Forms.CheckBox();
            this.CheckDataGridView = new System.Windows.Forms.DataGridView();
            this.CheckColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WarningThresholdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CriticalThresholdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NRPETableLayoutPanel.SuspendLayout();
            this.PoolTipsTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PoolTipsPicture)).BeginInit();
            this.GeneralConfigureGroupBox.SuspendLayout();
            this.GeneralConfigTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CheckDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // NRPETableLayoutPanel
            // 
            resources.ApplyResources(this.NRPETableLayoutPanel, "NRPETableLayoutPanel");
            this.NRPETableLayoutPanel.Controls.Add(this.PoolTipsTableLayoutPanel, 0, 4);
            this.NRPETableLayoutPanel.Controls.Add(this.DescLabel, 0, 0);
            this.NRPETableLayoutPanel.Controls.Add(this.GeneralConfigureGroupBox, 0, 1);
            this.NRPETableLayoutPanel.Controls.Add(this.CheckDataGridView, 0, 2);
            this.NRPETableLayoutPanel.Name = "NRPETableLayoutPanel";
            // 
            // PoolTipsTableLayoutPanel
            // 
            resources.ApplyResources(this.PoolTipsTableLayoutPanel, "PoolTipsTableLayoutPanel");
            this.PoolTipsTableLayoutPanel.Controls.Add(this.PoolTipsPicture, 0, 0);
            this.PoolTipsTableLayoutPanel.Controls.Add(this.PoolTipsLabel, 1, 0);
            this.PoolTipsTableLayoutPanel.Name = "PoolTipsTableLayoutPanel";
            // 
            // PoolTipsPicture
            // 
            resources.ApplyResources(this.PoolTipsPicture, "PoolTipsPicture");
            this.PoolTipsPicture.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.PoolTipsPicture.Name = "PoolTipsPicture";
            this.PoolTipsPicture.TabStop = false;
            // 
            // PoolTipsLabel
            // 
            resources.ApplyResources(this.PoolTipsLabel, "PoolTipsLabel");
            this.PoolTipsLabel.Name = "PoolTipsLabel";
            // 
            // DescLabel
            // 
            resources.ApplyResources(this.DescLabel, "DescLabel");
            this.DescLabel.Name = "DescLabel";
            // 
            // GeneralConfigureGroupBox
            // 
            resources.ApplyResources(this.GeneralConfigureGroupBox, "GeneralConfigureGroupBox");
            this.GeneralConfigureGroupBox.Controls.Add(this.GeneralConfigTableLayoutPanel);
            this.GeneralConfigureGroupBox.Name = "GeneralConfigureGroupBox";
            this.GeneralConfigureGroupBox.TabStop = false;
            // 
            // GeneralConfigTableLayoutPanel
            // 
            resources.ApplyResources(this.GeneralConfigTableLayoutPanel, "GeneralConfigTableLayoutPanel");
            this.GeneralConfigTableLayoutPanel.Controls.Add(this.EnableNRPECheckBox, 0, 1);
            this.GeneralConfigTableLayoutPanel.Controls.Add(this.AllowHostsLabel, 0, 2);
            this.GeneralConfigTableLayoutPanel.Controls.Add(this.AllowHostsTextBox, 1, 2);
            this.GeneralConfigTableLayoutPanel.Controls.Add(this.DebugLogCheckBox, 0, 3);
            this.GeneralConfigTableLayoutPanel.Controls.Add(this.SslDebugLogCheckBox, 0, 4);
            this.GeneralConfigTableLayoutPanel.Name = "GeneralConfigTableLayoutPanel";
            // 
            // EnableNRPECheckBox
            // 
            resources.ApplyResources(this.EnableNRPECheckBox, "EnableNRPECheckBox");
            this.GeneralConfigTableLayoutPanel.SetColumnSpan(this.EnableNRPECheckBox, 2);
            this.EnableNRPECheckBox.Name = "EnableNRPECheckBox";
            this.EnableNRPECheckBox.UseVisualStyleBackColor = true;
            this.EnableNRPECheckBox.CheckedChanged += new System.EventHandler(this.EnableNRPECheckBox_CheckedChanged);
            // 
            // AllowHostsLabel
            // 
            resources.ApplyResources(this.AllowHostsLabel, "AllowHostsLabel");
            this.AllowHostsLabel.Name = "AllowHostsLabel";
            // 
            // AllowHostsTextBox
            // 
            resources.ApplyResources(this.AllowHostsTextBox, "AllowHostsTextBox");
            this.AllowHostsTextBox.Name = "AllowHostsTextBox";
            // 
            // DebugLogCheckBox
            // 
            resources.ApplyResources(this.DebugLogCheckBox, "DebugLogCheckBox");
            this.GeneralConfigTableLayoutPanel.SetColumnSpan(this.DebugLogCheckBox, 2);
            this.DebugLogCheckBox.Name = "DebugLogCheckBox";
            this.DebugLogCheckBox.UseVisualStyleBackColor = true;
            // 
            // SslDebugLogCheckBox
            // 
            resources.ApplyResources(this.SslDebugLogCheckBox, "SslDebugLogCheckBox");
            this.GeneralConfigTableLayoutPanel.SetColumnSpan(this.SslDebugLogCheckBox, 2);
            this.SslDebugLogCheckBox.Name = "SslDebugLogCheckBox";
            this.SslDebugLogCheckBox.UseVisualStyleBackColor = true;
            // 
            // CheckDataGridView
            // 
            this.CheckDataGridView.AllowUserToAddRows = false;
            this.CheckDataGridView.AllowUserToDeleteRows = false;
            this.CheckDataGridView.AllowUserToResizeRows = false;
            resources.ApplyResources(this.CheckDataGridView, "CheckDataGridView");
            this.CheckDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.CheckDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.CheckDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.CheckDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CheckDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CheckColumn,
            this.WarningThresholdColumn,
            this.CriticalThresholdColumn});
            this.CheckDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.CheckDataGridView.EnableHeadersVisualStyles = false;
            this.CheckDataGridView.MultiSelect = false;
            this.CheckDataGridView.Name = "CheckDataGridView";
            this.CheckDataGridView.RowHeadersVisible = false;
            this.CheckDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.CheckDataGridView_BeginEdit);
            this.CheckDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.CheckDataGridView_EndEdit);
            // 
            // CheckColumn
            // 
            this.CheckColumn.FillWeight = 103.4483F;
            resources.ApplyResources(this.CheckColumn, "CheckColumn");
            this.CheckColumn.Name = "CheckColumn";
            this.CheckColumn.ReadOnly = true;
            // 
            // WarningThresholdColumn
            // 
            dataGridViewCellStyle1.Format = "N2";
            dataGridViewCellStyle1.NullValue = null;
            this.WarningThresholdColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.WarningThresholdColumn.FillWeight = 98.27586F;
            resources.ApplyResources(this.WarningThresholdColumn, "WarningThresholdColumn");
            this.WarningThresholdColumn.Name = "WarningThresholdColumn";
            // 
            // CriticalThresholdColumn
            // 
            this.CriticalThresholdColumn.FillWeight = 98.27586F;
            resources.ApplyResources(this.CriticalThresholdColumn, "CriticalThresholdColumn");
            this.CriticalThresholdColumn.Name = "CriticalThresholdColumn";
            // 
            // NRPEEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.NRPETableLayoutPanel);
            this.Name = "NRPEEditPage";
            this.NRPETableLayoutPanel.ResumeLayout(false);
            this.NRPETableLayoutPanel.PerformLayout();
            this.PoolTipsTableLayoutPanel.ResumeLayout(false);
            this.PoolTipsTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PoolTipsPicture)).EndInit();
            this.GeneralConfigureGroupBox.ResumeLayout(false);
            this.GeneralConfigTableLayoutPanel.ResumeLayout(false);
            this.GeneralConfigTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CheckDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TableLayoutPanel NRPETableLayoutPanel;

        private Label DescLabel;

        private GroupBox GeneralConfigureGroupBox;
        private TableLayoutPanel GeneralConfigTableLayoutPanel;

        private CheckBox EnableNRPECheckBox;
        private Label AllowHostsLabel;
        private TextBox AllowHostsTextBox;
        private CheckBox DebugLogCheckBox;
        private CheckBox SslDebugLogCheckBox;

        private DataGridView CheckDataGridView;
        private DataGridViewTextBoxColumn CheckColumn;
        private DataGridViewTextBoxColumn WarningThresholdColumn;
        private DataGridViewTextBoxColumn CriticalThresholdColumn;

        private TableLayoutPanel PoolTipsTableLayoutPanel;
        private PictureBox PoolTipsPicture;
        private Label PoolTipsLabel;
    }
}