namespace XenAdmin.TabPages
{
    partial class GeneralTabPage
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
                licenseStatus.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralTabPage));
            this.buttonProperties = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.linkLabelExpand = new System.Windows.Forms.LinkLabel();
            this.linkLabelCollapse = new System.Windows.Forms.LinkLabel();
            this.panel2 = new XenAdmin.Controls.PanelNoFocusScroll();
            this.panelStorageLinkSystemCapabilities = new System.Windows.Forms.Panel();
            this.pdSectionStorageLinkSystemCapabilities = new XenAdmin.Controls.PDSection();
            this.panelMultipathBoot = new System.Windows.Forms.Panel();
            this.pdSectionMultipathBoot = new XenAdmin.Controls.PDSection();
            this.panelStorageLink = new System.Windows.Forms.Panel();
            this.pdStorageLink = new XenAdmin.Controls.PDSection();
            this.panelUpdates = new System.Windows.Forms.Panel();
            this.pdSectionUpdates = new XenAdmin.Controls.PDSection();
            this.panelMemoryAndVCPUs = new System.Windows.Forms.Panel();
            this.pdSectionMemoryAndVCPUs = new XenAdmin.Controls.PDSection();
            this.panelMultipathing = new System.Windows.Forms.Panel();
            this.pdSectionMultipathing = new XenAdmin.Controls.PDSection();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.pdSectionStatus = new XenAdmin.Controls.PDSection();
            this.panelHighAvailability = new System.Windows.Forms.Panel();
            this.pdSectionHighAvailability = new XenAdmin.Controls.PDSection();
            this.panelBootOptions = new System.Windows.Forms.Panel();
            this.pdSectionBootOptions = new XenAdmin.Controls.PDSection();
            this.panelCPU = new System.Windows.Forms.Panel();
            this.pdSectionCPU = new XenAdmin.Controls.PDSection();
            this.panelLicense = new System.Windows.Forms.Panel();
            this.pdSectionLicense = new XenAdmin.Controls.PDSection();
            this.panelVersion = new System.Windows.Forms.Panel();
            this.pdSectionVersion = new XenAdmin.Controls.PDSection();
            this.panelMemory = new System.Windows.Forms.Panel();
            this.pdSectionMemory = new XenAdmin.Controls.PDSection();
            this.panelManagementInterfaces = new System.Windows.Forms.Panel();
            this.pdSectionManagementInterfaces = new XenAdmin.Controls.PDSection();
            this.panelCustomFields = new System.Windows.Forms.Panel();
            this.pdSectionCustomFields = new XenAdmin.Controls.PDSection();
            this.panelGeneral = new System.Windows.Forms.Panel();
            this.pdSectionGeneral = new XenAdmin.Controls.PDSection();
            this.pageContainerPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelStorageLinkSystemCapabilities.SuspendLayout();
            this.panelMultipathBoot.SuspendLayout();
            this.panelStorageLink.SuspendLayout();
            this.panelUpdates.SuspendLayout();
            this.panelMemoryAndVCPUs.SuspendLayout();
            this.panelMultipathing.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.panelHighAvailability.SuspendLayout();
            this.panelBootOptions.SuspendLayout();
            this.panelCPU.SuspendLayout();
            this.panelLicense.SuspendLayout();
            this.panelVersion.SuspendLayout();
            this.panelMemory.SuspendLayout();
            this.panelManagementInterfaces.SuspendLayout();
            this.panelCustomFields.SuspendLayout();
            this.panelGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.panel2);
            this.pageContainerPanel.Controls.Add(this.panel1);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // buttonProperties
            // 
            resources.ApplyResources(this.buttonProperties, "buttonProperties");
            this.buttonProperties.Name = "buttonProperties";
            this.buttonProperties.UseVisualStyleBackColor = true;
            this.buttonProperties.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.buttonProperties);
            this.panel3.Controls.Add(this.linkLabelExpand);
            this.panel3.Controls.Add(this.linkLabelCollapse);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.MaximumSize = new System.Drawing.Size(900, 28);
            this.panel3.Name = "panel3";
            // 
            // linkLabelExpand
            // 
            resources.ApplyResources(this.linkLabelExpand, "linkLabelExpand");
            this.linkLabelExpand.MinimumSize = new System.Drawing.Size(65, 28);
            this.linkLabelExpand.Name = "linkLabelExpand";
            this.linkLabelExpand.TabStop = true;
            this.linkLabelExpand.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelExpand_LinkClicked);
            // 
            // linkLabelCollapse
            // 
            resources.ApplyResources(this.linkLabelCollapse, "linkLabelCollapse");
            this.linkLabelCollapse.MinimumSize = new System.Drawing.Size(76, 28);
            this.linkLabelCollapse.Name = "linkLabelCollapse";
            this.linkLabelCollapse.TabStop = true;
            this.linkLabelCollapse.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCollapse_LinkClicked);
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.panelStorageLinkSystemCapabilities);
            this.panel2.Controls.Add(this.panelMultipathBoot);
            this.panel2.Controls.Add(this.panelStorageLink);
            this.panel2.Controls.Add(this.panelUpdates);
            this.panel2.Controls.Add(this.panelMemoryAndVCPUs);
            this.panel2.Controls.Add(this.panelMultipathing);
            this.panel2.Controls.Add(this.panelStatus);
            this.panel2.Controls.Add(this.panelHighAvailability);
            this.panel2.Controls.Add(this.panelBootOptions);
            this.panel2.Controls.Add(this.panelCPU);
            this.panel2.Controls.Add(this.panelLicense);
            this.panel2.Controls.Add(this.panelVersion);
            this.panel2.Controls.Add(this.panelMemory);
            this.panel2.Controls.Add(this.panelManagementInterfaces);
            this.panel2.Controls.Add(this.panelCustomFields);
            this.panel2.Controls.Add(this.panelGeneral);
            this.panel2.Name = "panel2";
            // 
            // panelStorageLinkSystemCapabilities
            // 
            resources.ApplyResources(this.panelStorageLinkSystemCapabilities, "panelStorageLinkSystemCapabilities");
            this.panelStorageLinkSystemCapabilities.Controls.Add(this.pdSectionStorageLinkSystemCapabilities);
            this.panelStorageLinkSystemCapabilities.Name = "panelStorageLinkSystemCapabilities";
            // 
            // pdSectionStorageLinkSystemCapabilities
            // 
            this.pdSectionStorageLinkSystemCapabilities.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionStorageLinkSystemCapabilities, "pdSectionStorageLinkSystemCapabilities");
            this.pdSectionStorageLinkSystemCapabilities.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionStorageLinkSystemCapabilities.Name = "pdSectionStorageLinkSystemCapabilities";
            this.pdSectionStorageLinkSystemCapabilities.ShowCellToolTips = false;
            // 
            // panelMultipathBoot
            // 
            resources.ApplyResources(this.panelMultipathBoot, "panelMultipathBoot");
            this.panelMultipathBoot.Controls.Add(this.pdSectionMultipathBoot);
            this.panelMultipathBoot.Name = "panelMultipathBoot";
            // 
            // pdSectionMultipathBoot
            // 
            this.pdSectionMultipathBoot.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionMultipathBoot, "pdSectionMultipathBoot");
            this.pdSectionMultipathBoot.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionMultipathBoot.Name = "pdSectionMultipathBoot";
            this.pdSectionMultipathBoot.ShowCellToolTips = false;
            // 
            // panelStorageLink
            // 
            resources.ApplyResources(this.panelStorageLink, "panelStorageLink");
            this.panelStorageLink.Controls.Add(this.pdStorageLink);
            this.panelStorageLink.Name = "panelStorageLink";
            // 
            // pdStorageLink
            // 
            this.pdStorageLink.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdStorageLink, "pdStorageLink");
            this.pdStorageLink.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdStorageLink.Name = "pdStorageLink";
            this.pdStorageLink.ShowCellToolTips = false;
            this.pdStorageLink.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // panelUpdates
            // 
            resources.ApplyResources(this.panelUpdates, "panelUpdates");
            this.panelUpdates.Controls.Add(this.pdSectionUpdates);
            this.panelUpdates.Name = "panelUpdates";
            // 
            // pdSectionUpdates
            // 
            this.pdSectionUpdates.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionUpdates, "pdSectionUpdates");
            this.pdSectionUpdates.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionUpdates.Name = "pdSectionUpdates";
            this.pdSectionUpdates.ShowCellToolTips = false;
            this.pdSectionUpdates.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // panelMemoryAndVCPUs
            // 
            resources.ApplyResources(this.panelMemoryAndVCPUs, "panelMemoryAndVCPUs");
            this.panelMemoryAndVCPUs.Controls.Add(this.pdSectionMemoryAndVCPUs);
            this.panelMemoryAndVCPUs.Name = "panelMemoryAndVCPUs";
            // 
            // pdSectionMemoryAndVCPUs
            // 
            this.pdSectionMemoryAndVCPUs.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionMemoryAndVCPUs, "pdSectionMemoryAndVCPUs");
            this.pdSectionMemoryAndVCPUs.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionMemoryAndVCPUs.Name = "pdSectionMemoryAndVCPUs";
            this.pdSectionMemoryAndVCPUs.ShowCellToolTips = false;
            this.pdSectionMemoryAndVCPUs.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // panelMultipathing
            // 
            resources.ApplyResources(this.panelMultipathing, "panelMultipathing");
            this.panelMultipathing.Controls.Add(this.pdSectionMultipathing);
            this.panelMultipathing.Name = "panelMultipathing";
            // 
            // pdSectionMultipathing
            // 
            this.pdSectionMultipathing.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionMultipathing, "pdSectionMultipathing");
            this.pdSectionMultipathing.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionMultipathing.Name = "pdSectionMultipathing";
            this.pdSectionMultipathing.ShowCellToolTips = false;
            this.pdSectionMultipathing.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // panelStatus
            // 
            resources.ApplyResources(this.panelStatus, "panelStatus");
            this.panelStatus.Controls.Add(this.pdSectionStatus);
            this.panelStatus.Name = "panelStatus";
            // 
            // pdSectionStatus
            // 
            this.pdSectionStatus.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionStatus, "pdSectionStatus");
            this.pdSectionStatus.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionStatus.Name = "pdSectionStatus";
            this.pdSectionStatus.ShowCellToolTips = false;
            this.pdSectionStatus.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // panelHighAvailability
            // 
            resources.ApplyResources(this.panelHighAvailability, "panelHighAvailability");
            this.panelHighAvailability.Controls.Add(this.pdSectionHighAvailability);
            this.panelHighAvailability.Name = "panelHighAvailability";
            // 
            // pdSectionHighAvailability
            // 
            this.pdSectionHighAvailability.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionHighAvailability, "pdSectionHighAvailability");
            this.pdSectionHighAvailability.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionHighAvailability.Name = "pdSectionHighAvailability";
            this.pdSectionHighAvailability.ShowCellToolTips = false;
            this.pdSectionHighAvailability.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // panelBootOptions
            // 
            resources.ApplyResources(this.panelBootOptions, "panelBootOptions");
            this.panelBootOptions.Controls.Add(this.pdSectionBootOptions);
            this.panelBootOptions.Name = "panelBootOptions";
            // 
            // pdSectionBootOptions
            // 
            this.pdSectionBootOptions.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionBootOptions, "pdSectionBootOptions");
            this.pdSectionBootOptions.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionBootOptions.Name = "pdSectionBootOptions";
            this.pdSectionBootOptions.ShowCellToolTips = false;
            this.pdSectionBootOptions.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // panelCPU
            // 
            resources.ApplyResources(this.panelCPU, "panelCPU");
            this.panelCPU.Controls.Add(this.pdSectionCPU);
            this.panelCPU.Name = "panelCPU";
            // 
            // pdSectionCPU
            // 
            this.pdSectionCPU.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionCPU, "pdSectionCPU");
            this.pdSectionCPU.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionCPU.Name = "pdSectionCPU";
            this.pdSectionCPU.ShowCellToolTips = false;
            this.pdSectionCPU.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // panelLicense
            // 
            resources.ApplyResources(this.panelLicense, "panelLicense");
            this.panelLicense.Controls.Add(this.pdSectionLicense);
            this.panelLicense.Name = "panelLicense";
            // 
            // pdSectionLicense
            // 
            this.pdSectionLicense.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionLicense, "pdSectionLicense");
            this.pdSectionLicense.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionLicense.Name = "pdSectionLicense";
            this.pdSectionLicense.ShowCellToolTips = false;
            this.pdSectionLicense.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // panelVersion
            // 
            resources.ApplyResources(this.panelVersion, "panelVersion");
            this.panelVersion.Controls.Add(this.pdSectionVersion);
            this.panelVersion.Name = "panelVersion";
            // 
            // pdSectionVersion
            // 
            this.pdSectionVersion.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionVersion, "pdSectionVersion");
            this.pdSectionVersion.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionVersion.Name = "pdSectionVersion";
            this.pdSectionVersion.ShowCellToolTips = false;
            this.pdSectionVersion.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // panelMemory
            // 
            resources.ApplyResources(this.panelMemory, "panelMemory");
            this.panelMemory.Controls.Add(this.pdSectionMemory);
            this.panelMemory.Name = "panelMemory";
            // 
            // pdSectionMemory
            // 
            this.pdSectionMemory.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionMemory, "pdSectionMemory");
            this.pdSectionMemory.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionMemory.Name = "pdSectionMemory";
            this.pdSectionMemory.ShowCellToolTips = false;
            this.pdSectionMemory.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // panelManagementInterfaces
            // 
            resources.ApplyResources(this.panelManagementInterfaces, "panelManagementInterfaces");
            this.panelManagementInterfaces.Controls.Add(this.pdSectionManagementInterfaces);
            this.panelManagementInterfaces.Name = "panelManagementInterfaces";
            // 
            // pdSectionManagementInterfaces
            // 
            this.pdSectionManagementInterfaces.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionManagementInterfaces, "pdSectionManagementInterfaces");
            this.pdSectionManagementInterfaces.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionManagementInterfaces.Name = "pdSectionManagementInterfaces";
            this.pdSectionManagementInterfaces.ShowCellToolTips = false;
            this.pdSectionManagementInterfaces.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // panelCustomFields
            // 
            resources.ApplyResources(this.panelCustomFields, "panelCustomFields");
            this.panelCustomFields.Controls.Add(this.pdSectionCustomFields);
            this.panelCustomFields.Name = "panelCustomFields";
            // 
            // pdSectionCustomFields
            // 
            this.pdSectionCustomFields.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionCustomFields, "pdSectionCustomFields");
            this.pdSectionCustomFields.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionCustomFields.Name = "pdSectionCustomFields";
            this.pdSectionCustomFields.ShowCellToolTips = true;
            this.pdSectionCustomFields.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // panelGeneral
            // 
            resources.ApplyResources(this.panelGeneral, "panelGeneral");
            this.panelGeneral.Controls.Add(this.pdSectionGeneral);
            this.panelGeneral.Name = "panelGeneral";
            // 
            // pdSectionGeneral
            // 
            this.pdSectionGeneral.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionGeneral, "pdSectionGeneral");
            this.pdSectionGeneral.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionGeneral.Name = "pdSectionGeneral";
            this.pdSectionGeneral.ShowCellToolTips = false;
            this.pdSectionGeneral.ExpandedChanged += new System.Action<XenAdmin.Controls.PDSection>(this.s_ExpandedEventHandler);
            // 
            // GeneralTabPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.DoubleBuffered = true;
            this.Name = "GeneralTabPage";
            this.pageContainerPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelStorageLinkSystemCapabilities.ResumeLayout(false);
            this.panelMultipathBoot.ResumeLayout(false);
            this.panelStorageLink.ResumeLayout(false);
            this.panelUpdates.ResumeLayout(false);
            this.panelMemoryAndVCPUs.ResumeLayout(false);
            this.panelMultipathing.ResumeLayout(false);
            this.panelStatus.ResumeLayout(false);
            this.panelHighAvailability.ResumeLayout(false);
            this.panelBootOptions.ResumeLayout(false);
            this.panelCPU.ResumeLayout(false);
            this.panelLicense.ResumeLayout(false);
            this.panelVersion.ResumeLayout(false);
            this.panelMemory.ResumeLayout(false);
            this.panelManagementInterfaces.ResumeLayout(false);
            this.panelCustomFields.ResumeLayout(false);
            this.panelGeneral.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonProperties;
        private System.Windows.Forms.Panel panel1;
        private XenAdmin.Controls.PanelNoFocusScroll panel2;
        private System.Windows.Forms.Panel panelGeneral;
        private XenAdmin.Controls.PDSection pdSectionGeneral;
        private System.Windows.Forms.Panel panelMemoryAndVCPUs;
        private XenAdmin.Controls.PDSection pdSectionMemoryAndVCPUs;
        private System.Windows.Forms.Panel panelBootOptions;
        private XenAdmin.Controls.PDSection pdSectionBootOptions;
        private System.Windows.Forms.Panel panelMultipathing;
        private XenAdmin.Controls.PDSection pdSectionMultipathing;
        private System.Windows.Forms.Panel panelStatus;
        private XenAdmin.Controls.PDSection pdSectionStatus;
        private System.Windows.Forms.Panel panelHighAvailability;
        private XenAdmin.Controls.PDSection pdSectionHighAvailability;
        private System.Windows.Forms.Panel panelCustomFields;
        private XenAdmin.Controls.PDSection pdSectionCustomFields;
        private System.Windows.Forms.Panel panelManagementInterfaces;
        private XenAdmin.Controls.PDSection pdSectionManagementInterfaces;
        private System.Windows.Forms.Panel panelCPU;
        private XenAdmin.Controls.PDSection pdSectionCPU;
        private System.Windows.Forms.Panel panelVersion;
        private XenAdmin.Controls.PDSection pdSectionVersion;
        private System.Windows.Forms.Panel panelLicense;
        private XenAdmin.Controls.PDSection pdSectionLicense;
        private System.Windows.Forms.Panel panelMemory;
        private XenAdmin.Controls.PDSection pdSectionMemory;
        private System.Windows.Forms.Panel panelUpdates;
        private XenAdmin.Controls.PDSection pdSectionUpdates;
        private System.Windows.Forms.LinkLabel linkLabelExpand;
        private System.Windows.Forms.LinkLabel linkLabelCollapse;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panelStorageLink;
        private XenAdmin.Controls.PDSection pdStorageLink;
        private System.Windows.Forms.Panel panelMultipathBoot;
        private XenAdmin.Controls.PDSection pdSectionMultipathBoot;
        private System.Windows.Forms.Panel panelStorageLinkSystemCapabilities;
        private XenAdmin.Controls.PDSection pdSectionStorageLinkSystemCapabilities;
    }
}
