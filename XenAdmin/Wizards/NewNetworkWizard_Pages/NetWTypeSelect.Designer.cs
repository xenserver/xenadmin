namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    partial class NetWTypeSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetWTypeSelect));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblNetTypeSel = new System.Windows.Forms.Label();
            this.rbtnExternalNetwork = new System.Windows.Forms.RadioButton();
            this.labelExternalNetwork = new System.Windows.Forms.Label();
            this.rbtnInternalNetwork = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.rbtnBondedNetwork = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.rbtnCHIN = new System.Windows.Forms.RadioButton();
            this.labelCHIN = new System.Windows.Forms.Label();
            this.rbtnSriov = new System.Windows.Forms.RadioButton();
            this.labelSriov = new System.Windows.Forms.Label();
            this.warningsTable = new System.Windows.Forms.TableLayoutPanel();
            this.iconWarningChinOption = new System.Windows.Forms.PictureBox();
            this.labelWarningChinOption = new System.Windows.Forms.Label();
            this.warningTableSriov = new System.Windows.Forms.TableLayoutPanel();
            this.iconWarningSriovOption = new System.Windows.Forms.PictureBox();
            this.labelWarningSriovOption = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.warningsTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconWarningChinOption)).BeginInit();
            this.warningTableSriov.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconWarningSriovOption)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.ShowAlways = true;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.lblNetTypeSel);
            this.flowLayoutPanel1.Controls.Add(this.rbtnExternalNetwork);
            this.flowLayoutPanel1.Controls.Add(this.labelExternalNetwork);
            this.flowLayoutPanel1.Controls.Add(this.rbtnInternalNetwork);
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.rbtnBondedNetwork);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.rbtnCHIN);
            this.flowLayoutPanel1.Controls.Add(this.labelCHIN);
            this.flowLayoutPanel1.Controls.Add(this.rbtnSriov);
            this.flowLayoutPanel1.Controls.Add(this.labelSriov);
            this.flowLayoutPanel1.Controls.Add(this.warningsTable);
            this.flowLayoutPanel1.Controls.Add(this.warningTableSriov);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // lblNetTypeSel
            // 
            resources.ApplyResources(this.lblNetTypeSel, "lblNetTypeSel");
            this.lblNetTypeSel.Name = "lblNetTypeSel";
            // 
            // rbtnExternalNetwork
            // 
            resources.ApplyResources(this.rbtnExternalNetwork, "rbtnExternalNetwork");
            this.rbtnExternalNetwork.Checked = true;
            this.rbtnExternalNetwork.Name = "rbtnExternalNetwork";
            this.rbtnExternalNetwork.TabStop = true;
            this.rbtnExternalNetwork.UseVisualStyleBackColor = true;
            // 
            // labelExternalNetwork
            // 
            resources.ApplyResources(this.labelExternalNetwork, "labelExternalNetwork");
            this.labelExternalNetwork.Name = "labelExternalNetwork";
            // 
            // rbtnInternalNetwork
            // 
            resources.ApplyResources(this.rbtnInternalNetwork, "rbtnInternalNetwork");
            this.rbtnInternalNetwork.Name = "rbtnInternalNetwork";
            this.rbtnInternalNetwork.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // rbtnBondedNetwork
            // 
            resources.ApplyResources(this.rbtnBondedNetwork, "rbtnBondedNetwork");
            this.rbtnBondedNetwork.Name = "rbtnBondedNetwork";
            this.rbtnBondedNetwork.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // rbtnCHIN
            // 
            resources.ApplyResources(this.rbtnCHIN, "rbtnCHIN");
            this.rbtnCHIN.Name = "rbtnCHIN";
            this.rbtnCHIN.UseVisualStyleBackColor = true;
            // 
            // labelCHIN
            // 
            resources.ApplyResources(this.labelCHIN, "labelCHIN");
            this.labelCHIN.Name = "labelCHIN";
            // 
            // rbtnSriov
            // 
            resources.ApplyResources(this.rbtnSriov, "rbtnSriov");
            this.rbtnSriov.Name = "rbtnSriov";
            this.rbtnSriov.UseVisualStyleBackColor = true;
            // 
            // labelSriov
            // 
            resources.ApplyResources(this.labelSriov, "labelSriov");
            this.labelSriov.Name = "labelSriov";
            // 
            // warningsTable
            // 
            resources.ApplyResources(this.warningsTable, "warningsTable");
            this.warningsTable.Controls.Add(this.iconWarningChinOption, 0, 1);
            this.warningsTable.Controls.Add(this.labelWarningChinOption, 1, 1);
            this.warningsTable.Name = "warningsTable";
            // 
            // iconWarningChinOption
            // 
            resources.ApplyResources(this.iconWarningChinOption, "iconWarningChinOption");
            this.iconWarningChinOption.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.iconWarningChinOption.Name = "iconWarningChinOption";
            this.iconWarningChinOption.TabStop = false;
            // 
            // labelWarningChinOption
            // 
            resources.ApplyResources(this.labelWarningChinOption, "labelWarningChinOption");
            this.labelWarningChinOption.Name = "labelWarningChinOption";
            // 
            // warningTableSriov
            // 
            resources.ApplyResources(this.warningTableSriov, "warningTableSriov");
            this.warningTableSriov.Controls.Add(this.iconWarningSriovOption, 0, 1);
            this.warningTableSriov.Controls.Add(this.labelWarningSriovOption, 1, 1);
            this.warningTableSriov.Name = "warningTableSriov";
            // 
            // iconWarningSriovOption
            // 
            resources.ApplyResources(this.iconWarningSriovOption, "iconWarningSriovOption");
            this.iconWarningSriovOption.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.iconWarningSriovOption.Name = "iconWarningSriovOption";
            this.iconWarningSriovOption.TabStop = false;
            // 
            // labelWarningSriovOption
            // 
            resources.ApplyResources(this.labelWarningSriovOption, "labelWarningSriovOption");
            this.labelWarningSriovOption.Name = "labelWarningSriovOption";
            // 
            // NetWTypeSelect
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "NetWTypeSelect";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.warningsTable.ResumeLayout(false);
            this.warningsTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconWarningChinOption)).EndInit();
            this.warningTableSriov.ResumeLayout(false);
            this.warningTableSriov.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconWarningSriovOption)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNetTypeSel;
        private System.Windows.Forms.RadioButton rbtnBondedNetwork;
        private System.Windows.Forms.RadioButton rbtnExternalNetwork;
        private System.Windows.Forms.RadioButton rbtnInternalNetwork;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelExternalNetwork;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelCHIN;
        private System.Windows.Forms.RadioButton rbtnCHIN;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TableLayoutPanel warningsTable;
        private System.Windows.Forms.PictureBox iconWarningChinOption;
        private System.Windows.Forms.Label labelWarningChinOption;
        private System.Windows.Forms.RadioButton rbtnSriov;
        private System.Windows.Forms.Label labelSriov;
        private System.Windows.Forms.TableLayoutPanel warningTableSriov;
        private System.Windows.Forms.PictureBox iconWarningSriovOption;
        private System.Windows.Forms.Label labelWarningSriovOption;
    }
}
