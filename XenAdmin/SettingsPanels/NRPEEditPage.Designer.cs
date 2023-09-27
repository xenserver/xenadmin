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
            this.NRPETableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.EnableNRPECheckBox = new System.Windows.Forms.CheckBox();
            this.DescLabelPool = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.descLabelHost = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.GeneralConfigureGroupBox = new System.Windows.Forms.GroupBox();
            this.GeneralConfigTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.AllowHostsLabel = new System.Windows.Forms.Label();
            this.AllowHostsTextBox = new System.Windows.Forms.TextBox();
            this.DebugLogCheckBox = new System.Windows.Forms.CheckBox();
            this.SslDebugLogCheckBox = new System.Windows.Forms.CheckBox();
            this.CheckDataGridView = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.CheckColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WarningThresholdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CriticalThresholdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NRPETableLayoutPanel.SuspendLayout();
            this.GeneralConfigureGroupBox.SuspendLayout();
            this.GeneralConfigTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CheckDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // NRPETableLayoutPanel
            // 
            resources.ApplyResources(this.NRPETableLayoutPanel, "NRPETableLayoutPanel");
            this.NRPETableLayoutPanel.Controls.Add(this.EnableNRPECheckBox, 0, 2);
            this.NRPETableLayoutPanel.Controls.Add(this.DescLabelPool, 0, 0);
            this.NRPETableLayoutPanel.Controls.Add(this.descLabelHost, 0, 1);
            this.NRPETableLayoutPanel.Controls.Add(this.GeneralConfigureGroupBox, 0, 3);
            this.NRPETableLayoutPanel.Controls.Add(this.CheckDataGridView, 0, 4);
            this.NRPETableLayoutPanel.Name = "NRPETableLayoutPanel";
            // 
            // EnableNRPECheckBox
            // 
            resources.ApplyResources(this.EnableNRPECheckBox, "EnableNRPECheckBox");
            this.EnableNRPECheckBox.Name = "EnableNRPECheckBox";
            this.EnableNRPECheckBox.UseVisualStyleBackColor = true;
            this.EnableNRPECheckBox.CheckedChanged += new System.EventHandler(this.EnableNRPECheckBox_CheckedChanged);
            // 
            // DescLabelPool
            // 
            resources.ApplyResources(this.DescLabelPool, "DescLabelPool");
            this.DescLabelPool.Name = "DescLabelPool";
            // 
            // descLabelHost
            // 
            resources.ApplyResources(this.descLabelHost, "descLabelHost");
            this.descLabelHost.Name = "descLabelHost";
            // 
            // GeneralConfigureGroupBox
            // 
            this.GeneralConfigureGroupBox.Controls.Add(this.GeneralConfigTableLayoutPanel);
            resources.ApplyResources(this.GeneralConfigureGroupBox, "GeneralConfigureGroupBox");
            this.GeneralConfigureGroupBox.Name = "GeneralConfigureGroupBox";
            this.GeneralConfigureGroupBox.TabStop = false;
            // 
            // GeneralConfigTableLayoutPanel
            // 
            resources.ApplyResources(this.GeneralConfigTableLayoutPanel, "GeneralConfigTableLayoutPanel");
            this.GeneralConfigTableLayoutPanel.Controls.Add(this.AllowHostsLabel, 0, 0);
            this.GeneralConfigTableLayoutPanel.Controls.Add(this.AllowHostsTextBox, 1, 0);
            this.GeneralConfigTableLayoutPanel.Controls.Add(this.DebugLogCheckBox, 0, 1);
            this.GeneralConfigTableLayoutPanel.Controls.Add(this.SslDebugLogCheckBox, 0, 2);
            this.GeneralConfigTableLayoutPanel.Name = "GeneralConfigTableLayoutPanel";
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
            this.CheckDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.CheckDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.CheckDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.CheckDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CheckColumn,
            this.WarningThresholdColumn,
            this.CriticalThresholdColumn});
            resources.ApplyResources(this.CheckDataGridView, "CheckDataGridView");
            this.CheckDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.CheckDataGridView.Name = "CheckDataGridView";
            this.CheckDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.CheckDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.CheckDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.CheckDataGridView_BeginEdit);
            this.CheckDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.CheckDataGridView_EndEdit);
            // 
            // CheckColumn
            // 
            this.CheckColumn.FillWeight = 40F;
            resources.ApplyResources(this.CheckColumn, "CheckColumn");
            this.CheckColumn.Name = "CheckColumn";
            this.CheckColumn.ReadOnly = true;
            // 
            // WarningThresholdColumn
            // 
            this.WarningThresholdColumn.FillWeight = 30F;
            resources.ApplyResources(this.WarningThresholdColumn, "WarningThresholdColumn");
            this.WarningThresholdColumn.Name = "WarningThresholdColumn";
            // 
            // CriticalThresholdColumn
            // 
            this.CriticalThresholdColumn.FillWeight = 30F;
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
            this.GeneralConfigureGroupBox.ResumeLayout(false);
            this.GeneralConfigureGroupBox.PerformLayout();
            this.GeneralConfigTableLayoutPanel.ResumeLayout(false);
            this.GeneralConfigTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CheckDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel NRPETableLayoutPanel;
        private XenAdmin.Controls.Common.AutoHeightLabel DescLabelPool;
        private System.Windows.Forms.GroupBox GeneralConfigureGroupBox;
        private System.Windows.Forms.TableLayoutPanel GeneralConfigTableLayoutPanel;
        private System.Windows.Forms.CheckBox EnableNRPECheckBox;
        private System.Windows.Forms.Label AllowHostsLabel;
        private System.Windows.Forms.TextBox AllowHostsTextBox;
        private System.Windows.Forms.CheckBox DebugLogCheckBox;
        private System.Windows.Forms.CheckBox SslDebugLogCheckBox;
        private Controls.DataGridViewEx.DataGridViewEx CheckDataGridView;
        private Controls.Common.AutoHeightLabel descLabelHost;
        private System.Windows.Forms.DataGridViewTextBoxColumn CheckColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn WarningThresholdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn CriticalThresholdColumn;
    }
}