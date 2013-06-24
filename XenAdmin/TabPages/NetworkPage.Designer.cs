namespace XenAdmin.TabPages
{
    partial class NetworkPage
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
            // Deregister listeners.
            XenObject = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.button1 = new System.Windows.Forms.Button();
            this.labelNetworkheadings = new System.Windows.Forms.Label();
            this.labelInterfaces = new System.Windows.Forms.Label();
            this.panelManagementInterfaces = new System.Windows.Forms.Panel();
            this.toolTipContainerConfigureButton = new XenAdmin.Controls.ToolTipContainer();
            this.dataGridViewEx1 = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnServer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnIcon = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnInterface = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnNetwork = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnNIC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnIpSetup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gatewayColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dnsColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.networkList1 = new XenAdmin.Controls.NetworkingTab.NetworkList();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pageContainerPanel.SuspendLayout();
            this.panelManagementInterfaces.SuspendLayout();
            this.toolTipContainerConfigureButton.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            this.pageContainerPanel.Controls.Add(this.tableLayoutPanel1);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Image = global::XenAdmin.Properties.Resources._000_ConfigureIPAddresses_h32bit_16;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonManagementInterfaces_Click);
            // 
            // labelNetworkheadings
            // 
            resources.ApplyResources(this.labelNetworkheadings, "labelNetworkheadings");
            this.labelNetworkheadings.Name = "labelNetworkheadings";
            // 
            // labelInterfaces
            // 
            resources.ApplyResources(this.labelInterfaces, "labelInterfaces");
            this.labelInterfaces.Name = "labelInterfaces";
            // 
            // panelManagementInterfaces
            // 
            this.panelManagementInterfaces.Controls.Add(this.toolTipContainerConfigureButton);
            this.panelManagementInterfaces.Controls.Add(this.dataGridViewEx1);
            this.panelManagementInterfaces.Controls.Add(this.labelInterfaces);
            resources.ApplyResources(this.panelManagementInterfaces, "panelManagementInterfaces");
            this.panelManagementInterfaces.MaximumSize = new System.Drawing.Size(900, 350);
            this.panelManagementInterfaces.Name = "panelManagementInterfaces";
            // 
            // toolTipContainerConfigureButton
            // 
            resources.ApplyResources(this.toolTipContainerConfigureButton, "toolTipContainerConfigureButton");
            this.toolTipContainerConfigureButton.Controls.Add(this.button1);
            this.toolTipContainerConfigureButton.Name = "toolTipContainerConfigureButton";
            // 
            // dataGridViewEx1
            // 
            resources.ApplyResources(this.dataGridViewEx1, "dataGridViewEx1");
            this.dataGridViewEx1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewEx1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewEx1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewEx1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewEx1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnServer,
            this.ColumnIcon,
            this.ColumnInterface,
            this.ColumnNetwork,
            this.ColumnNIC,
            this.ColumnIpSetup,
            this.ColumnIP,
            this.Column1,
            this.gatewayColumn,
            this.dnsColumn});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewEx1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewEx1.HideSelection = true;
            this.dataGridViewEx1.Name = "dataGridViewEx1";
            this.dataGridViewEx1.ReadOnly = true;
            // 
            // ColumnServer
            // 
            this.ColumnServer.FillWeight = 35F;
            resources.ApplyResources(this.ColumnServer, "ColumnServer");
            this.ColumnServer.Name = "ColumnServer";
            this.ColumnServer.ReadOnly = true;
            this.ColumnServer.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnIcon
            // 
            this.ColumnIcon.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnIcon, "ColumnIcon");
            this.ColumnIcon.Name = "ColumnIcon";
            this.ColumnIcon.ReadOnly = true;
            this.ColumnIcon.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnInterface
            // 
            this.ColumnInterface.FillWeight = 33F;
            resources.ApplyResources(this.ColumnInterface, "ColumnInterface");
            this.ColumnInterface.Name = "ColumnInterface";
            this.ColumnInterface.ReadOnly = true;
            this.ColumnInterface.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnNetwork
            // 
            this.ColumnNetwork.FillWeight = 33F;
            resources.ApplyResources(this.ColumnNetwork, "ColumnNetwork");
            this.ColumnNetwork.Name = "ColumnNetwork";
            this.ColumnNetwork.ReadOnly = true;
            this.ColumnNetwork.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnNIC
            // 
            resources.ApplyResources(this.ColumnNIC, "ColumnNIC");
            this.ColumnNIC.Name = "ColumnNIC";
            this.ColumnNIC.ReadOnly = true;
            // 
            // ColumnIpSetup
            // 
            resources.ApplyResources(this.ColumnIpSetup, "ColumnIpSetup");
            this.ColumnIpSetup.Name = "ColumnIpSetup";
            this.ColumnIpSetup.ReadOnly = true;
            // 
            // ColumnIP
            // 
            this.ColumnIP.FillWeight = 65F;
            resources.ApplyResources(this.ColumnIP, "ColumnIP");
            this.ColumnIP.Name = "ColumnIP";
            this.ColumnIP.ReadOnly = true;
            this.ColumnIP.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Column1
            // 
            resources.ApplyResources(this.Column1, "Column1");
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // gatewayColumn
            // 
            resources.ApplyResources(this.gatewayColumn, "gatewayColumn");
            this.gatewayColumn.Name = "gatewayColumn";
            this.gatewayColumn.ReadOnly = true;
            // 
            // dnsColumn
            // 
            this.dnsColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dnsColumn, "dnsColumn");
            this.dnsColumn.Name = "dnsColumn";
            this.dnsColumn.ReadOnly = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // networkList1
            // 
            resources.ApplyResources(this.networkList1, "networkList1");
            this.networkList1.BackColor = System.Drawing.Color.Transparent;
            this.networkList1.MaximumSize = new System.Drawing.Size(900, 400);
            this.networkList1.Name = "networkList1";
            this.networkList1.XenObject = null;
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoEllipsis = true;
            resources.ApplyResources(this.TitleLabel, "TitleLabel");
            this.TitleLabel.ForeColor = System.Drawing.Color.White;
            this.TitleLabel.Name = "TitleLabel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelManagementInterfaces, 0, 1);
            this.tableLayoutPanel1.MaximumSize = new System.Drawing.Size(1000, 900);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.networkList1);
            this.panel2.Controls.Add(this.labelNetworkheadings);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // NetworkPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "NetworkPage";
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.pageContainerPanel, 0);
            this.pageContainerPanel.ResumeLayout(false);
            this.panelManagementInterfaces.ResumeLayout(false);
            this.panelManagementInterfaces.PerformLayout();
            this.toolTipContainerConfigureButton.ResumeLayout(false);
            this.toolTipContainerConfigureButton.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelNetworkheadings;
        private System.Windows.Forms.Label labelInterfaces;
        private XenAdmin.Controls.NetworkingTab.NetworkList networkList1;
        private System.Windows.Forms.Panel panelManagementInterfaces;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewEx1;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerConfigureButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnServer;
        private System.Windows.Forms.DataGridViewImageColumn ColumnIcon;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnInterface;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNetwork;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNIC;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnIpSetup;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnIP;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn gatewayColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dnsColumn;
    }
}
