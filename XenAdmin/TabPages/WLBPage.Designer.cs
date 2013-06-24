

namespace XenAdmin.TabPages
{
    partial class WlbPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbPage));
            this.panelConfiguration = new System.Windows.Forms.Panel();
            this.pdSectionConfiguration = new XenAdmin.Controls.PDSection();
            this.panelOptimize = new System.Windows.Forms.Panel();
            this.wlbOptimizePool = new XenAdmin.Controls.Wlb.WlbOptimizePool();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelStatus = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxWarningTriangle = new System.Windows.Forms.PictureBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.flowLayoutPanelLeftButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonConfigure = new System.Windows.Forms.Button();
            this.buttonEnableDisableWlb = new System.Windows.Forms.Button();
            this.flowLayoutPanelRightButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonReports = new System.Windows.Forms.Button();
            this.pageContainerPanel.SuspendLayout();
            this.panelConfiguration.SuspendLayout();
            this.panelOptimize.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tableLayoutPanelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarningTriangle)).BeginInit();
            this.panelButtons.SuspendLayout();
            this.flowLayoutPanelLeftButtons.SuspendLayout();
            this.flowLayoutPanelRightButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            this.pageContainerPanel.BackColor = System.Drawing.Color.Transparent;
            this.pageContainerPanel.Controls.Add(this.tableLayoutPanelMain);
            // 
            // panelConfiguration
            // 
            resources.ApplyResources(this.panelConfiguration, "panelConfiguration");
            this.panelConfiguration.BackColor = System.Drawing.Color.Transparent;
            this.panelConfiguration.Controls.Add(this.pdSectionConfiguration);
            this.panelConfiguration.Name = "panelConfiguration";
            // 
            // pdSectionConfiguration
            // 
            this.pdSectionConfiguration.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionConfiguration, "pdSectionConfiguration");
            this.pdSectionConfiguration.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionConfiguration.Name = "pdSectionConfiguration";
            this.pdSectionConfiguration.ShowCellToolTips = false;
            // 
            // panelOptimize
            // 
            resources.ApplyResources(this.panelOptimize, "panelOptimize");
            this.panelOptimize.BackColor = System.Drawing.Color.Transparent;
            this.panelOptimize.Controls.Add(this.wlbOptimizePool);
            this.panelOptimize.Name = "panelOptimize";
            // 
            // wlbOptimizePool
            // 
            this.wlbOptimizePool.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.wlbOptimizePool, "wlbOptimizePool");
            this.wlbOptimizePool.Name = "wlbOptimizePool";
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.tableLayoutPanelMain, "tableLayoutPanelMain");
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelStatus, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.panelButtons, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.panelConfiguration, 0, 2);
            this.tableLayoutPanelMain.Controls.Add(this.panelOptimize, 0, 3);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            // 
            // tableLayoutPanelStatus
            // 
            resources.ApplyResources(this.tableLayoutPanelStatus, "tableLayoutPanelStatus");
            this.tableLayoutPanelStatus.Controls.Add(this.pictureBoxWarningTriangle, 0, 0);
            this.tableLayoutPanelStatus.Controls.Add(this.labelStatus, 1, 0);
            this.tableLayoutPanelStatus.Name = "tableLayoutPanelStatus";
            // 
            // pictureBoxWarningTriangle
            // 
            resources.ApplyResources(this.pictureBoxWarningTriangle, "pictureBoxWarningTriangle");
            this.pictureBoxWarningTriangle.Name = "pictureBoxWarningTriangle";
            this.pictureBoxWarningTriangle.TabStop = false;
            // 
            // labelStatus
            // 
            resources.ApplyResources(this.labelStatus, "labelStatus");
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.UseMnemonic = false;
            // 
            // panelButtons
            // 
            this.panelButtons.BackColor = System.Drawing.Color.Transparent;
            this.panelButtons.Controls.Add(this.flowLayoutPanelLeftButtons);
            this.panelButtons.Controls.Add(this.flowLayoutPanelRightButtons);
            resources.ApplyResources(this.panelButtons, "panelButtons");
            this.panelButtons.Name = "panelButtons";
            // 
            // flowLayoutPanelLeftButtons
            // 
            this.flowLayoutPanelLeftButtons.Controls.Add(this.buttonConnect);
            this.flowLayoutPanelLeftButtons.Controls.Add(this.buttonConfigure);
            this.flowLayoutPanelLeftButtons.Controls.Add(this.buttonEnableDisableWlb);
            resources.ApplyResources(this.flowLayoutPanelLeftButtons, "flowLayoutPanelLeftButtons");
            this.flowLayoutPanelLeftButtons.Name = "flowLayoutPanelLeftButtons";
            // 
            // buttonConnect
            // 
            resources.ApplyResources(this.buttonConnect, "buttonConnect");
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonConfigure
            // 
            resources.ApplyResources(this.buttonConfigure, "buttonConfigure");
            this.buttonConfigure.Name = "buttonConfigure";
            this.buttonConfigure.UseVisualStyleBackColor = true;
            this.buttonConfigure.Click += new System.EventHandler(this.buttonConfigure_Click);
            // 
            // buttonEnableDisableWlb
            // 
            resources.ApplyResources(this.buttonEnableDisableWlb, "buttonEnableDisableWlb");
            this.buttonEnableDisableWlb.Name = "buttonEnableDisableWlb";
            this.buttonEnableDisableWlb.UseVisualStyleBackColor = true;
            this.buttonEnableDisableWlb.Click += new System.EventHandler(this.buttonEnableDisableWlb_Click);
            // 
            // flowLayoutPanelRightButtons
            // 
            resources.ApplyResources(this.flowLayoutPanelRightButtons, "flowLayoutPanelRightButtons");
            this.flowLayoutPanelRightButtons.Controls.Add(this.buttonReports);
            this.flowLayoutPanelRightButtons.Name = "flowLayoutPanelRightButtons";
            // 
            // buttonReports
            // 
            resources.ApplyResources(this.buttonReports, "buttonReports");
            this.buttonReports.Name = "buttonReports";
            this.buttonReports.UseVisualStyleBackColor = true;
            this.buttonReports.Click += new System.EventHandler(this.buttonReports_Click);
            // 
            // WlbPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "WlbPage";
            this.pageContainerPanel.ResumeLayout(false);
            this.panelConfiguration.ResumeLayout(false);
            this.panelOptimize.ResumeLayout(false);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.tableLayoutPanelStatus.ResumeLayout(false);
            this.tableLayoutPanelStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarningTriangle)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.flowLayoutPanelLeftButtons.ResumeLayout(false);
            this.flowLayoutPanelLeftButtons.PerformLayout();
            this.flowLayoutPanelRightButtons.ResumeLayout(false);
            this.flowLayoutPanelRightButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.PDSection pdSectionConfiguration;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelStatus;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Panel panelConfiguration;
        private System.Windows.Forms.Panel panelOptimize;
        private System.Windows.Forms.Button buttonEnableDisableWlb;
        private System.Windows.Forms.Button buttonConfigure;
        private XenAdmin.Controls.Wlb.WlbOptimizePool wlbOptimizePool;
        private System.Windows.Forms.PictureBox pictureBoxWarningTriangle;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelLeftButtons;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelRightButtons;
        private System.Windows.Forms.Button buttonReports;
        private System.Windows.Forms.Panel panelButtons;
    }
}
