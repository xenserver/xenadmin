namespace XenAdmin.Controls.Wlb
{
    partial class WlbReportView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbReportView));
            this.labelHostCombo = new System.Windows.Forms.Label();
            this.hostComboBox = new System.Windows.Forms.ComboBox();
            this.btnRunReport = new System.Windows.Forms.Button();
            this.btnLaterReport = new System.Windows.Forms.Button();
            this.labelEndDate = new System.Windows.Forms.Label();
            this.EndDatePicker = new System.Windows.Forms.DateTimePicker();
            this.labelStartDate = new System.Windows.Forms.Label();
            this.StartDatePicker = new System.Windows.Forms.DateTimePicker();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.btnSubscribe = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.comboBoxView = new System.Windows.Forms.ComboBox();
            this.labelShow = new System.Windows.Forms.Label();
            this.panelHosts = new System.Windows.Forms.Panel();
            this.panelShow = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblExported = new System.Windows.Forms.Label();
            this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.panelUsers = new System.Windows.Forms.Panel();
            this.userComboBox = new XenAdmin.Controls.LongStringComboBox();
            this.labelUsers = new System.Windows.Forms.Label();
            this.panelObjects = new System.Windows.Forms.Panel();
            this.labelObjects = new System.Windows.Forms.Label();
            this.objectComboBox = new System.Windows.Forms.ComboBox();
            this.panelHosts.SuspendLayout();
            this.panelShow.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanelButtons.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.panelUsers.SuspendLayout();
            this.panelObjects.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelHostCombo
            // 
            resources.ApplyResources(this.labelHostCombo, "labelHostCombo");
            this.labelHostCombo.Name = "labelHostCombo";
            // 
            // hostComboBox
            // 
            this.hostComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.hostComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hostComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.hostComboBox, "hostComboBox");
            this.hostComboBox.Name = "hostComboBox";
            this.hostComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
            this.hostComboBox.Leave += new System.EventHandler(this.comboBox_DropDownClosed);
            this.hostComboBox.DropDownClosed += new System.EventHandler(this.comboBox_DropDownClosed);
            // 
            // btnRunReport
            // 
            resources.ApplyResources(this.btnRunReport, "btnRunReport");
            this.btnRunReport.Name = "btnRunReport";
            this.btnRunReport.UseVisualStyleBackColor = true;
            this.btnRunReport.Click += new System.EventHandler(this.btnRunReport_Click);
            // 
            // btnLaterReport
            // 
            resources.ApplyResources(this.btnLaterReport, "btnLaterReport");
            this.btnLaterReport.Name = "btnLaterReport";
            this.btnLaterReport.UseVisualStyleBackColor = true;
            this.btnLaterReport.Click += new System.EventHandler(this.btnLaterReport_Click);
            // 
            // labelEndDate
            // 
            resources.ApplyResources(this.labelEndDate, "labelEndDate");
            this.labelEndDate.Name = "labelEndDate";
            // 
            // EndDatePicker
            // 
            resources.ApplyResources(this.EndDatePicker, "EndDatePicker");
            this.EndDatePicker.Name = "EndDatePicker";
            this.EndDatePicker.ValueChanged += new System.EventHandler(this.comboBox_SelectionChanged);
            // 
            // labelStartDate
            // 
            resources.ApplyResources(this.labelStartDate, "labelStartDate");
            this.labelStartDate.Name = "labelStartDate";
            // 
            // StartDatePicker
            // 
            resources.ApplyResources(this.StartDatePicker, "StartDatePicker");
            this.StartDatePicker.Name = "StartDatePicker";
            this.StartDatePicker.ValueChanged += new System.EventHandler(this.comboBox_SelectionChanged);
            // 
            // reportViewer1
            // 
            resources.ApplyResources(this.reportViewer1, "reportViewer1");
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ShowRefreshButton = false;
            this.reportViewer1.ReportExport += new Microsoft.Reporting.WinForms.ExportEventHandler(this.reportViewer1_ReportExport);
            this.reportViewer1.Back += new Microsoft.Reporting.WinForms.BackEventHandler(this.reportViewer1_Back);
            this.reportViewer1.Drillthrough += new Microsoft.Reporting.WinForms.DrillthroughEventHandler(this.reportViewer1_Drillthrough);
            // 
            // btnSubscribe
            // 
            resources.ApplyResources(this.btnSubscribe, "btnSubscribe");
            this.btnSubscribe.Name = "btnSubscribe";
            this.btnSubscribe.UseVisualStyleBackColor = true;
            this.btnSubscribe.Click += new System.EventHandler(this.btnSubscribe_Click);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // comboBoxView
            // 
            this.comboBoxView.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxView.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxView, "comboBoxView");
            this.comboBoxView.Name = "comboBoxView";
            this.comboBoxView.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
            this.comboBoxView.SelectedIndexChanged += new System.EventHandler(this.comboBoxView_SelectedIndexChanged);
            this.comboBoxView.DropDownClosed += new System.EventHandler(this.comboBox_DropDownClosed);
            // 
            // labelShow
            // 
            resources.ApplyResources(this.labelShow, "labelShow");
            this.labelShow.Name = "labelShow";
            // 
            // panelHosts
            // 
            resources.ApplyResources(this.panelHosts, "panelHosts");
            this.panelHosts.Controls.Add(this.hostComboBox);
            this.panelHosts.Controls.Add(this.labelHostCombo);
            this.panelHosts.Name = "panelHosts";
            // 
            // panelShow
            // 
            resources.ApplyResources(this.panelShow, "panelShow");
            this.panelShow.Controls.Add(this.labelShow);
            this.panelShow.Controls.Add(this.comboBoxView);
            this.panelShow.Name = "panelShow";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.panelHosts);
            this.flowLayoutPanel1.Controls.Add(this.panelShow);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblExported
            // 
            resources.ApplyResources(this.lblExported, "lblExported");
            this.lblExported.BackColor = System.Drawing.Color.Khaki;
            this.lblExported.Name = "lblExported";
            // 
            // flowLayoutPanelButtons
            // 
            resources.ApplyResources(this.flowLayoutPanelButtons, "flowLayoutPanelButtons");
            this.flowLayoutPanelButtons.Controls.Add(this.btnClose);
            this.flowLayoutPanelButtons.Controls.Add(this.btnSubscribe);
            this.flowLayoutPanelButtons.Controls.Add(this.btnRunReport);
            this.flowLayoutPanelButtons.Controls.Add(this.btnLaterReport);
            this.flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Controls.Add(this.panelUsers);
            this.flowLayoutPanel2.Controls.Add(this.panelObjects);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // panelUsers
            // 
            resources.ApplyResources(this.panelUsers, "panelUsers");
            this.panelUsers.Controls.Add(this.userComboBox);
            this.panelUsers.Controls.Add(this.labelUsers);
            this.panelUsers.Name = "panelUsers";
            // 
            // userComboBox
            // 
            this.userComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.userComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.userComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.userComboBox, "userComboBox");
            this.userComboBox.Name = "userComboBox";
            this.userComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
            this.userComboBox.DropDownClosed += new System.EventHandler(this.comboBox_DropDownClosed);
            this.userComboBox.TextChanged += new System.EventHandler(this.comboBox_SelectionChanged);
            this.userComboBox.Leave += new System.EventHandler(this.comboBox_DropDownClosed);
            // 
            // labelUsers
            // 
            resources.ApplyResources(this.labelUsers, "labelUsers");
            this.labelUsers.Name = "labelUsers";
            // 
            // panelObjects
            // 
            resources.ApplyResources(this.panelObjects, "panelObjects");
            this.panelObjects.Controls.Add(this.labelObjects);
            this.panelObjects.Controls.Add(this.objectComboBox);
            this.panelObjects.Name = "panelObjects";
            // 
            // labelObjects
            // 
            resources.ApplyResources(this.labelObjects, "labelObjects");
            this.labelObjects.Name = "labelObjects";
            // 
            // objectComboBox
            // 
            this.objectComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.objectComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.objectComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.objectComboBox, "objectComboBox");
            this.objectComboBox.Name = "objectComboBox";
            this.objectComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
            this.objectComboBox.DropDownClosed += new System.EventHandler(this.comboBox_DropDownClosed);
            this.objectComboBox.TextChanged += new System.EventHandler(this.comboBox_SelectionChanged);
            this.objectComboBox.Leave += new System.EventHandler(this.comboBox_DropDownClosed);
            // 
            // WlbReportView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanelButtons);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.lblExported);
            this.Controls.Add(this.labelEndDate);
            this.Controls.Add(this.EndDatePicker);
            this.Controls.Add(this.labelStartDate);
            this.Controls.Add(this.StartDatePicker);
            this.Controls.Add(this.reportViewer1);
            this.Name = "WlbReportView";
            this.Load += new System.EventHandler(this.ReportView_Load);
            this.panelHosts.ResumeLayout(false);
            this.panelHosts.PerformLayout();
            this.panelShow.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanelButtons.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.panelUsers.ResumeLayout(false);
            this.panelUsers.PerformLayout();
            this.panelObjects.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label labelHostCombo;
        public System.Windows.Forms.ComboBox hostComboBox;
        public System.Windows.Forms.Button btnRunReport;
        public System.Windows.Forms.Button btnLaterReport;
        private System.Windows.Forms.Label labelEndDate;
        private System.Windows.Forms.DateTimePicker EndDatePicker;
        private System.Windows.Forms.Label labelStartDate;
        private System.Windows.Forms.DateTimePicker StartDatePicker;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        public System.Windows.Forms.Button btnSubscribe;
        public System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ComboBox comboBoxView;
        private System.Windows.Forms.Label labelShow;
        private System.Windows.Forms.Panel panelHosts;
        private System.Windows.Forms.Panel panelShow;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblExported;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Panel panelUsers;
        public XenAdmin.Controls.LongStringComboBox userComboBox;
        public System.Windows.Forms.Label labelUsers;
        private System.Windows.Forms.Panel panelObjects;
        private System.Windows.Forms.Label labelObjects;
        private System.Windows.Forms.ComboBox objectComboBox;

    }
}
