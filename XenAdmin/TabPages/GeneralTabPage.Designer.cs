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
                if (licenseStatus != null)
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
            this.buttonViewConsole = new System.Windows.Forms.Button();
            this.buttonViewLog = new System.Windows.Forms.Button();
            this.linkLabelExpand = new System.Windows.Forms.LinkLabel();
            this.linkLabelCollapse = new System.Windows.Forms.LinkLabel();
            this.panel2 = new XenAdmin.Controls.PanelNoFocusScroll();
            this.panelReadCaching = new System.Windows.Forms.Panel();
            this.pdSectionReadCaching = new XenAdmin.Controls.PDSection();
            this.panelDockerInfo = new System.Windows.Forms.Panel();
            this.pdSectionDockerInfo = new XenAdmin.Controls.PDSection();
            this.panelDockerVersion = new System.Windows.Forms.Panel();
            this.pdSectionDockerVersion = new XenAdmin.Controls.PDSection();
            this.panelStorageLinkSystemCapabilities = new System.Windows.Forms.Panel();
            this.pdSectionStorageLinkSystemCapabilities = new XenAdmin.Controls.PDSection();
            this.panelMultipathBoot = new System.Windows.Forms.Panel();
            this.pdSectionMultipathBoot = new XenAdmin.Controls.PDSection();
            this.panelStorageLink = new System.Windows.Forms.Panel();
            this.pdStorageLink = new XenAdmin.Controls.PDSection();
            this.panelMemoryAndVCPUs = new System.Windows.Forms.Panel();
            this.pdSectionVCPUs = new XenAdmin.Controls.PDSection();
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
            this.panelMemory = new System.Windows.Forms.Panel();
            this.pdSectionMemory = new XenAdmin.Controls.PDSection();
            this.panelManagementInterfaces = new System.Windows.Forms.Panel();
            this.pdSectionManagementInterfaces = new XenAdmin.Controls.PDSection();
            this.panelUpdates = new System.Windows.Forms.Panel();
            this.pdSectionUpdates = new XenAdmin.Controls.PDSection();
            this.panelVersion = new System.Windows.Forms.Panel();
            this.pdSectionVersion = new XenAdmin.Controls.PDSection();
            this.panelLicense = new System.Windows.Forms.Panel();
            this.pdSectionLicense = new XenAdmin.Controls.PDSection();
            this.panelCustomFields = new System.Windows.Forms.Panel();
            this.pdSectionCustomFields = new XenAdmin.Controls.PDSection();
            this.panelCertificate = new System.Windows.Forms.Panel();
            this.pdSectionCertificate = new XenAdmin.Controls.PDSection();
            this.panelGeneral = new System.Windows.Forms.Panel();
            this.pdSectionGeneral = new XenAdmin.Controls.PDSection();
            this.tableLayoutPanelButtons = new System.Windows.Forms.TableLayoutPanel();
            this.pageContainerPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelReadCaching.SuspendLayout();
            this.panelDockerInfo.SuspendLayout();
            this.panelDockerVersion.SuspendLayout();
            this.panelStorageLinkSystemCapabilities.SuspendLayout();
            this.panelMultipathBoot.SuspendLayout();
            this.panelStorageLink.SuspendLayout();
            this.panelMemoryAndVCPUs.SuspendLayout();
            this.panelMultipathing.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.panelHighAvailability.SuspendLayout();
            this.panelBootOptions.SuspendLayout();
            this.panelCPU.SuspendLayout();
            this.panelMemory.SuspendLayout();
            this.panelManagementInterfaces.SuspendLayout();
            this.panelUpdates.SuspendLayout();
            this.panelVersion.SuspendLayout();
            this.panelLicense.SuspendLayout();
            this.panelCustomFields.SuspendLayout();
            this.panelCertificate.SuspendLayout();
            this.panelGeneral.SuspendLayout();
            this.tableLayoutPanelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.panel2);
            this.pageContainerPanel.Controls.Add(this.tableLayoutPanelButtons);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // buttonProperties
            // 
            resources.ApplyResources(this.buttonProperties, "buttonProperties");
            this.buttonProperties.Name = "buttonProperties";
            this.buttonProperties.UseVisualStyleBackColor = true;
            this.buttonProperties.Click += new System.EventHandler(this.buttonProperties_Click);
            // 
            // buttonViewConsole
            // 
            resources.ApplyResources(this.buttonViewConsole, "buttonViewConsole");
            this.buttonViewConsole.Name = "buttonViewConsole";
            this.buttonViewConsole.UseVisualStyleBackColor = true;
            this.buttonViewConsole.Click += new System.EventHandler(this.buttonViewConsole_Click);
            // 
            // buttonViewLog
            // 
            resources.ApplyResources(this.buttonViewLog, "buttonViewLog");
            this.buttonViewLog.Name = "buttonViewLog";
            this.buttonViewLog.UseVisualStyleBackColor = true;
            this.buttonViewLog.Click += new System.EventHandler(this.buttonViewLog_Click);
            // 
            // linkLabelExpand
            // 
            resources.ApplyResources(this.linkLabelExpand, "linkLabelExpand");
            this.linkLabelExpand.Name = "linkLabelExpand";
            this.linkLabelExpand.TabStop = true;
            this.linkLabelExpand.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelExpand_LinkClicked);
            // 
            // linkLabelCollapse
            // 
            resources.ApplyResources(this.linkLabelCollapse, "linkLabelCollapse");
            this.linkLabelCollapse.Name = "linkLabelCollapse";
            this.linkLabelCollapse.TabStop = true;
            this.linkLabelCollapse.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCollapse_LinkClicked);
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.panelReadCaching);
            this.panel2.Controls.Add(this.panelDockerInfo);
            this.panel2.Controls.Add(this.panelDockerVersion);
            this.panel2.Controls.Add(this.panelStorageLinkSystemCapabilities);
            this.panel2.Controls.Add(this.panelMultipathBoot);
            this.panel2.Controls.Add(this.panelStorageLink);
            this.panel2.Controls.Add(this.panelMemoryAndVCPUs);
            this.panel2.Controls.Add(this.panelMultipathing);
            this.panel2.Controls.Add(this.panelStatus);
            this.panel2.Controls.Add(this.panelHighAvailability);
            this.panel2.Controls.Add(this.panelBootOptions);
            this.panel2.Controls.Add(this.panelCPU);
            this.panel2.Controls.Add(this.panelMemory);
            this.panel2.Controls.Add(this.panelManagementInterfaces);
            this.panel2.Controls.Add(this.panelUpdates);
            this.panel2.Controls.Add(this.panelVersion);
            this.panel2.Controls.Add(this.panelLicense);
            this.panel2.Controls.Add(this.panelCustomFields);
            this.panel2.Controls.Add(this.panelCertificate);
            this.panel2.Controls.Add(this.panelGeneral);
            this.panel2.Name = "panel2";
            // 
            // panelReadCaching
            // 
            resources.ApplyResources(this.panelReadCaching, "panelReadCaching");
            this.panelReadCaching.Controls.Add(this.pdSectionReadCaching);
            this.panelReadCaching.Name = "panelReadCaching";
            // 
            // pdSectionReadCaching
            // 
            this.pdSectionReadCaching.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionReadCaching, "pdSectionReadCaching");
            this.pdSectionReadCaching.Name = "pdSectionReadCaching";
            this.pdSectionReadCaching.ShowCellToolTips = false;
            // 
            // panelDockerInfo
            // 
            resources.ApplyResources(this.panelDockerInfo, "panelDockerInfo");
            this.panelDockerInfo.Controls.Add(this.pdSectionDockerInfo);
            this.panelDockerInfo.Name = "panelDockerInfo";
            // 
            // pdSectionDockerInfo
            // 
            this.pdSectionDockerInfo.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionDockerInfo, "pdSectionDockerInfo");
            this.pdSectionDockerInfo.Name = "pdSectionDockerInfo";
            this.pdSectionDockerInfo.ShowCellToolTips = false;
            // 
            // panelDockerVersion
            // 
            resources.ApplyResources(this.panelDockerVersion, "panelDockerVersion");
            this.panelDockerVersion.Controls.Add(this.pdSectionDockerVersion);
            this.panelDockerVersion.Name = "panelDockerVersion";
            // 
            // pdSectionDockerVersion
            // 
            this.pdSectionDockerVersion.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionDockerVersion, "pdSectionDockerVersion");
            this.pdSectionDockerVersion.Name = "pdSectionDockerVersion";
            this.pdSectionDockerVersion.ShowCellToolTips = false;
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
            this.pdStorageLink.Name = "pdStorageLink";
            this.pdStorageLink.ShowCellToolTips = false;
            // 
            // panelMemoryAndVCPUs
            // 
            resources.ApplyResources(this.panelMemoryAndVCPUs, "panelMemoryAndVCPUs");
            this.panelMemoryAndVCPUs.Controls.Add(this.pdSectionVCPUs);
            this.panelMemoryAndVCPUs.Name = "panelMemoryAndVCPUs";
            // 
            // pdSectionVCPUs
            // 
            this.pdSectionVCPUs.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionVCPUs, "pdSectionVCPUs");
            this.pdSectionVCPUs.Name = "pdSectionVCPUs";
            this.pdSectionVCPUs.ShowCellToolTips = false;
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
            this.pdSectionMultipathing.Name = "pdSectionMultipathing";
            this.pdSectionMultipathing.ShowCellToolTips = false;
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
            this.pdSectionStatus.Name = "pdSectionStatus";
            this.pdSectionStatus.ShowCellToolTips = false;
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
            this.pdSectionHighAvailability.Name = "pdSectionHighAvailability";
            this.pdSectionHighAvailability.ShowCellToolTips = false;
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
            this.pdSectionBootOptions.Name = "pdSectionBootOptions";
            this.pdSectionBootOptions.ShowCellToolTips = false;
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
            this.pdSectionCPU.Name = "pdSectionCPU";
            this.pdSectionCPU.ShowCellToolTips = false;
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
            this.pdSectionMemory.Name = "pdSectionMemory";
            this.pdSectionMemory.ShowCellToolTips = false;
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
            this.pdSectionManagementInterfaces.Name = "pdSectionManagementInterfaces";
            this.pdSectionManagementInterfaces.ShowCellToolTips = false;
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
            this.pdSectionUpdates.Name = "pdSectionUpdates";
            this.pdSectionUpdates.ShowCellToolTips = false;
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
            this.pdSectionVersion.Name = "pdSectionVersion";
            this.pdSectionVersion.ShowCellToolTips = false;
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
            this.pdSectionLicense.Name = "pdSectionLicense";
            this.pdSectionLicense.ShowCellToolTips = false;
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
            this.pdSectionCustomFields.Name = "pdSectionCustomFields";
            this.pdSectionCustomFields.ShowCellToolTips = true;
            // 
            // panelCertificate
            // 
            resources.ApplyResources(this.panelCertificate, "panelCertificate");
            this.panelCertificate.Controls.Add(this.pdSectionCertificate);
            this.panelCertificate.Name = "panelCertificate";
            // 
            // pdSectionCertificate
            // 
            this.pdSectionCertificate.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionCertificate, "pdSectionCertificate");
            this.pdSectionCertificate.Name = "pdSectionCertificate";
            this.pdSectionCertificate.ShowCellToolTips = false;
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
            this.pdSectionGeneral.Name = "pdSectionGeneral";
            this.pdSectionGeneral.ShowCellToolTips = false;
            // 
            // tableLayoutPanelButtons
            // 
            resources.ApplyResources(this.tableLayoutPanelButtons, "tableLayoutPanelButtons");
            this.tableLayoutPanelButtons.Controls.Add(this.buttonViewLog, 2, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.buttonViewConsole, 1, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.buttonProperties, 0, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.linkLabelCollapse, 5, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.linkLabelExpand, 4, 0);
            this.tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            // 
            // GeneralTabPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.DoubleBuffered = true;
            this.Name = "GeneralTabPage";
            this.pageContainerPanel.ResumeLayout(false);
            this.pageContainerPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelReadCaching.ResumeLayout(false);
            this.panelDockerInfo.ResumeLayout(false);
            this.panelDockerVersion.ResumeLayout(false);
            this.panelStorageLinkSystemCapabilities.ResumeLayout(false);
            this.panelMultipathBoot.ResumeLayout(false);
            this.panelStorageLink.ResumeLayout(false);
            this.panelMemoryAndVCPUs.ResumeLayout(false);
            this.panelMultipathing.ResumeLayout(false);
            this.panelStatus.ResumeLayout(false);
            this.panelHighAvailability.ResumeLayout(false);
            this.panelBootOptions.ResumeLayout(false);
            this.panelCPU.ResumeLayout(false);
            this.panelMemory.ResumeLayout(false);
            this.panelManagementInterfaces.ResumeLayout(false);
            this.panelUpdates.ResumeLayout(false);
            this.panelVersion.ResumeLayout(false);
            this.panelLicense.ResumeLayout(false);
            this.panelCustomFields.ResumeLayout(false);
            this.panelCertificate.ResumeLayout(false);
            this.panelGeneral.ResumeLayout(false);
            this.tableLayoutPanelButtons.ResumeLayout(false);
            this.tableLayoutPanelButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonProperties;
        private XenAdmin.Controls.PanelNoFocusScroll panel2;
        private System.Windows.Forms.Panel panelGeneral;
        private XenAdmin.Controls.PDSection pdSectionGeneral;
        private System.Windows.Forms.Panel panelMemoryAndVCPUs;
        private XenAdmin.Controls.PDSection pdSectionVCPUs;
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
        private System.Windows.Forms.Panel panelStorageLink;
        private XenAdmin.Controls.PDSection pdStorageLink;
        private System.Windows.Forms.Panel panelMultipathBoot;
        private XenAdmin.Controls.PDSection pdSectionMultipathBoot;
        private System.Windows.Forms.Panel panelStorageLinkSystemCapabilities;
        private XenAdmin.Controls.PDSection pdSectionStorageLinkSystemCapabilities;
        private System.Windows.Forms.Panel panelDockerInfo;
        private System.Windows.Forms.Panel panelDockerVersion;
        private Controls.PDSection pdSectionDockerVersion;
        private Controls.PDSection pdSectionDockerInfo;
        private System.Windows.Forms.Panel panelReadCaching;
        private Controls.PDSection pdSectionReadCaching;
        private System.Windows.Forms.Button buttonViewConsole;
        private System.Windows.Forms.Button buttonViewLog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelButtons;
        private System.Windows.Forms.Panel panelCertificate;
        private Controls.PDSection pdSectionCertificate;
    }
}
