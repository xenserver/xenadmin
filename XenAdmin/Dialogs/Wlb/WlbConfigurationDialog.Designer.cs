namespace XenAdmin.Dialogs.Wlb
{
    partial class WlbConfigurationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbConfigurationDialog));
            this.wlbOptimizationModePage = new XenAdmin.SettingsPanels.WlbOptimizationModePage();
            this.wlbAutomationPage = new XenAdmin.SettingsPanels.WlbAutomationPage();
            this.wlbThresholdsPage = new XenAdmin.SettingsPanels.WlbThresholdsPage();
            this.wlbMetricWeightingPage = new XenAdmin.SettingsPanels.WlbMetricWeightingPage();
            this.wlbHostExclusionPage = new XenAdmin.SettingsPanels.WlbHostExclusionPage();
            this.wlbAdvancedSettingsPage = new XenAdmin.SettingsPanels.WlbAdvancedSettingsPage();
            this.ContentPanel.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.blueBorder.SuspendLayout();
            this.SuspendLayout();
            // 
            // ContentPanel
            // 
            resources.ApplyResources(this.ContentPanel, "ContentPanel");
            this.ContentPanel.Controls.Add(this.wlbOptimizationModePage);
            this.ContentPanel.Controls.Add(this.wlbAutomationPage);
            this.ContentPanel.Controls.Add(this.wlbThresholdsPage);
            this.ContentPanel.Controls.Add(this.wlbMetricWeightingPage);
            this.ContentPanel.Controls.Add(this.wlbHostExclusionPage);
            this.ContentPanel.Controls.Add(this.wlbAdvancedSettingsPage);
            // 
            // verticalTabs
            // 
            this.verticalTabs.Items.AddRange(new object[] {
            this.wlbOptimizationModePage,
            this.wlbAutomationPage,
            this.wlbThresholdsPage,
            this.wlbMetricWeightingPage,
            this.wlbHostExclusionPage,
            this.wlbAdvancedSettingsPage});
            resources.ApplyResources(this.verticalTabs, "verticalTabs");
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // splitContainer
            // 
            resources.ApplyResources(this.splitContainer, "splitContainer");
            // 
            // splitContainer.Panel1
            // 
            resources.ApplyResources(this.splitContainer.Panel1, "splitContainer.Panel1");
            // 
            // splitContainer.Panel2
            // 
            resources.ApplyResources(this.splitContainer.Panel2, "splitContainer.Panel2");
            // 
            // blueBorder
            // 
            resources.ApplyResources(this.blueBorder, "blueBorder");
            // 
            // wlbOptimizationModePage
            // 
            this.wlbOptimizationModePage.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.wlbOptimizationModePage, "wlbOptimizationModePage");
            this.wlbOptimizationModePage.Name = "wlbOptimizationModePage";
            // 
            // wlbAutomationPage
            // 
            this.wlbAutomationPage.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.wlbAutomationPage, "wlbAutomationPage");
            this.wlbAutomationPage.Name = "wlbAutomationPage";
            // 
            // wlbThresholdsPage
            // 
            this.wlbThresholdsPage.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.wlbThresholdsPage, "wlbThresholdsPage");
            this.wlbThresholdsPage.Name = "wlbThresholdsPage";
            // 
            // wlbMetricWeightingPage
            // 
            this.wlbMetricWeightingPage.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.wlbMetricWeightingPage, "wlbMetricWeightingPage");
            this.wlbMetricWeightingPage.Name = "wlbMetricWeightingPage";
            // 
            // wlbHostExclusionPage
            // 
            this.wlbHostExclusionPage.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.wlbHostExclusionPage, "wlbHostExclusionPage");
            this.wlbHostExclusionPage.Name = "wlbHostExclusionPage";
            // 
            // wlbAdvancedSettingsPage
            // 
            this.wlbAdvancedSettingsPage.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.wlbAdvancedSettingsPage, "wlbAdvancedSettingsPage");
            this.wlbAdvancedSettingsPage.MinimumSize = new System.Drawing.Size(560, 560);
            this.wlbAdvancedSettingsPage.Name = "wlbAdvancedSettingsPage";
            // 
            // WlbConfigurationDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "WlbConfigurationDialog";
            this.SizeChanged += new System.EventHandler(this.WlbConfigurationDialog_SizeChanged);
            this.Controls.SetChildIndex(this.cancelButton, 0);
            this.Controls.SetChildIndex(this.okButton, 0);
            this.Controls.SetChildIndex(this.splitContainer, 0);
            this.ContentPanel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.blueBorder.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.SettingsPanels.WlbOptimizationModePage wlbOptimizationModePage;
        private XenAdmin.SettingsPanels.WlbAutomationPage wlbAutomationPage;
        //private XenAdmin.SettingsPanels.WlbPowerManagementPage wlbPowerManagementPage;
        private XenAdmin.SettingsPanels.WlbThresholdsPage wlbThresholdsPage;
        private XenAdmin.SettingsPanels.WlbMetricWeightingPage wlbMetricWeightingPage;
        private XenAdmin.SettingsPanels.WlbHostExclusionPage wlbHostExclusionPage;
        private XenAdmin.SettingsPanels.WlbAdvancedSettingsPage wlbAdvancedSettingsPage;
    }
}
