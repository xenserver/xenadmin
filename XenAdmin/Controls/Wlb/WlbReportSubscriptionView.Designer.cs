namespace XenAdmin.Controls.Wlb
{
    partial class WlbReportSubscriptionView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbReportSubscriptionView));
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnChange = new System.Windows.Forms.Button();
            this.labelSubscription = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.tableLayoutPanelSubscriptionDetails = new System.Windows.Forms.TableLayoutPanel();
            this.pdSectionParameters = new XenAdmin.Controls.PDSection();
            this.pdSectionGeneral = new XenAdmin.Controls.PDSection();
            this.pdSectionDelivery = new XenAdmin.Controls.PDSection();
            this.pdSectionSchedule = new XenAdmin.Controls.PDSection();
            this.pdSectionHistory = new XenAdmin.Controls.PDSection();
            this.panelTopControls = new System.Windows.Forms.Panel();
            this.flowLayoutPanelTopButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanelLowerButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanelSubscriptionDetails.SuspendLayout();
            this.panelTopControls.SuspendLayout();
            this.flowLayoutPanelTopButtons.SuspendLayout();
            this.flowLayoutPanelLowerButtons.SuspendLayout();
            this.panelCenter.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnChange
            // 
            resources.ApplyResources(this.btnChange, "btnChange");
            this.btnChange.Name = "btnChange";
            this.btnChange.UseVisualStyleBackColor = true;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // labelSubscription
            // 
            resources.ApplyResources(this.labelSubscription, "labelSubscription");
            this.labelSubscription.Name = "labelSubscription";
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tableLayoutPanelSubscriptionDetails
            // 
            resources.ApplyResources(this.tableLayoutPanelSubscriptionDetails, "tableLayoutPanelSubscriptionDetails");
            this.tableLayoutPanelSubscriptionDetails.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanelSubscriptionDetails.Controls.Add(this.pdSectionParameters, 0, 1);
            this.tableLayoutPanelSubscriptionDetails.Controls.Add(this.pdSectionGeneral, 0, 0);
            this.tableLayoutPanelSubscriptionDetails.Controls.Add(this.pdSectionDelivery, 0, 2);
            this.tableLayoutPanelSubscriptionDetails.Controls.Add(this.pdSectionSchedule, 0, 3);
            this.tableLayoutPanelSubscriptionDetails.Controls.Add(this.pdSectionHistory, 0, 4);
            this.tableLayoutPanelSubscriptionDetails.Name = "tableLayoutPanelSubscriptionDetails";
            // 
            // pdSectionParameters
            // 
            this.pdSectionParameters.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionParameters, "pdSectionParameters");
            this.pdSectionParameters.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionParameters.Name = "pdSectionParameters";
            this.pdSectionParameters.ShowCellToolTips = false;
            // 
            // pdSectionGeneral
            // 
            this.pdSectionGeneral.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionGeneral, "pdSectionGeneral");
            this.pdSectionGeneral.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionGeneral.Name = "pdSectionGeneral";
            this.pdSectionGeneral.ShowCellToolTips = false;
            // 
            // pdSectionDelivery
            // 
            this.pdSectionDelivery.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionDelivery, "pdSectionDelivery");
            this.pdSectionDelivery.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionDelivery.Name = "pdSectionDelivery";
            this.pdSectionDelivery.ShowCellToolTips = false;
            // 
            // pdSectionSchedule
            // 
            this.pdSectionSchedule.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionSchedule, "pdSectionSchedule");
            this.pdSectionSchedule.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionSchedule.Name = "pdSectionSchedule";
            this.pdSectionSchedule.ShowCellToolTips = false;
            // 
            // pdSectionHistory
            // 
            this.pdSectionHistory.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.pdSectionHistory, "pdSectionHistory");
            this.pdSectionHistory.MinimumSize = new System.Drawing.Size(0, 34);
            this.pdSectionHistory.Name = "pdSectionHistory";
            this.pdSectionHistory.ShowCellToolTips = false;
            // 
            // panelTopControls
            // 
            this.panelTopControls.Controls.Add(this.flowLayoutPanelTopButtons);
            this.panelTopControls.Controls.Add(this.flowLayoutPanel1);
            this.panelTopControls.Controls.Add(this.labelSubscription);
            resources.ApplyResources(this.panelTopControls, "panelTopControls");
            this.panelTopControls.Name = "panelTopControls";
            // 
            // flowLayoutPanelTopButtons
            // 
            resources.ApplyResources(this.flowLayoutPanelTopButtons, "flowLayoutPanelTopButtons");
            this.flowLayoutPanelTopButtons.Controls.Add(this.btnDelete);
            this.flowLayoutPanelTopButtons.Controls.Add(this.btnChange);
            this.flowLayoutPanelTopButtons.Name = "flowLayoutPanelTopButtons";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // flowLayoutPanelLowerButtons
            // 
            resources.ApplyResources(this.flowLayoutPanelLowerButtons, "flowLayoutPanelLowerButtons");
            this.flowLayoutPanelLowerButtons.Controls.Add(this.btnClose);
            this.flowLayoutPanelLowerButtons.Name = "flowLayoutPanelLowerButtons";
            // 
            // panelCenter
            // 
            resources.ApplyResources(this.panelCenter, "panelCenter");
            this.panelCenter.Controls.Add(this.tableLayoutPanelSubscriptionDetails);
            this.panelCenter.Name = "panelCenter";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.flowLayoutPanelLowerButtons);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // WlbReportSubscriptionView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.panelTopControls);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(671, 278);
            this.Name = "WlbReportSubscriptionView";
            this.Load += new System.EventHandler(this.ReportSubscriptionView_Load);
            this.Resize += new System.EventHandler(this.WlbReportSubscriptionView_Resize);
            this.tableLayoutPanelSubscriptionDetails.ResumeLayout(false);
            this.panelTopControls.ResumeLayout(false);
            this.panelTopControls.PerformLayout();
            this.flowLayoutPanelTopButtons.ResumeLayout(false);
            this.flowLayoutPanelLowerButtons.ResumeLayout(false);
            this.panelCenter.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button btnDelete;
        internal System.Windows.Forms.Button btnChange;
        private System.Windows.Forms.Label labelSubscription;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSubscriptionDetails;
        private PDSection pdSectionGeneral;
        private PDSection pdSectionParameters;
        private PDSection pdSectionDelivery;
        private PDSection pdSectionSchedule;
        private PDSection pdSectionHistory;
        private System.Windows.Forms.Panel panelTopControls;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelLowerButtons;
        private System.Windows.Forms.Panel panelCenter;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelTopButtons;
        private System.Windows.Forms.Panel panel1;
    }
}
