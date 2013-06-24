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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetWTypeSelect));
            this.lblNetTypeSel = new System.Windows.Forms.Label();
            this.rbtnInternalNetwork = new System.Windows.Forms.RadioButton();
            this.rbtnBondedNetwork = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelCHIN = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTipContainerCHIN = new XenAdmin.Controls.ToolTipContainer();
            this.panelCHIN = new System.Windows.Forms.Panel();
            this.rbtnCHIN = new System.Windows.Forms.RadioButton();
            this.toolTipContainerExternal = new XenAdmin.Controls.ToolTipContainer();
            this.panelExternal = new System.Windows.Forms.Panel();
            this.rbtnExternalNetwork = new System.Windows.Forms.RadioButton();
            this.toolTipContainerCHIN.SuspendLayout();
            this.panelCHIN.SuspendLayout();
            this.toolTipContainerExternal.SuspendLayout();
            this.panelExternal.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblNetTypeSel
            // 
            resources.ApplyResources(this.lblNetTypeSel, "lblNetTypeSel");
            this.lblNetTypeSel.Name = "lblNetTypeSel";
            // 
            // rbtnInternalNetwork
            // 
            resources.ApplyResources(this.rbtnInternalNetwork, "rbtnInternalNetwork");
            this.rbtnInternalNetwork.Name = "rbtnInternalNetwork";
            this.rbtnInternalNetwork.UseVisualStyleBackColor = true;
            // 
            // rbtnBondedNetwork
            // 
            resources.ApplyResources(this.rbtnBondedNetwork, "rbtnBondedNetwork");
            this.rbtnBondedNetwork.Name = "rbtnBondedNetwork";
            this.rbtnBondedNetwork.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // labelCHIN
            // 
            resources.ApplyResources(this.labelCHIN, "labelCHIN");
            this.labelCHIN.Name = "labelCHIN";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // toolTipContainerCHIN
            // 
            resources.ApplyResources(this.toolTipContainerCHIN, "toolTipContainerCHIN");
            this.toolTipContainerCHIN.Controls.Add(this.panelCHIN);
            this.toolTipContainerCHIN.Name = "toolTipContainerCHIN";
            // 
            // panelCHIN
            // 
            this.panelCHIN.Controls.Add(this.rbtnCHIN);
            this.panelCHIN.Controls.Add(this.labelCHIN);
            resources.ApplyResources(this.panelCHIN, "panelCHIN");
            this.panelCHIN.Name = "panelCHIN";
            // 
            // rbtnCHIN
            // 
            resources.ApplyResources(this.rbtnCHIN, "rbtnCHIN");
            this.rbtnCHIN.Name = "rbtnCHIN";
            this.rbtnCHIN.UseVisualStyleBackColor = true;
            // 
            // toolTipContainerExternal
            // 
            resources.ApplyResources(this.toolTipContainerExternal, "toolTipContainerExternal");
            this.toolTipContainerExternal.Controls.Add(this.panelExternal);
            this.toolTipContainerExternal.Name = "toolTipContainerExternal";
            // 
            // panelExternal
            // 
            this.panelExternal.Controls.Add(this.rbtnExternalNetwork);
            this.panelExternal.Controls.Add(this.label2);
            resources.ApplyResources(this.panelExternal, "panelExternal");
            this.panelExternal.Name = "panelExternal";
            // 
            // rbtnExternalNetwork
            // 
            resources.ApplyResources(this.rbtnExternalNetwork, "rbtnExternalNetwork");
            this.rbtnExternalNetwork.Checked = true;
            this.rbtnExternalNetwork.Name = "rbtnExternalNetwork";
            this.rbtnExternalNetwork.TabStop = true;
            this.rbtnExternalNetwork.UseVisualStyleBackColor = true;
            // 
            // NetWTypeSelect
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.toolTipContainerCHIN);
            this.Controls.Add(this.toolTipContainerExternal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbtnBondedNetwork);
            this.Controls.Add(this.lblNetTypeSel);
            this.Controls.Add(this.rbtnInternalNetwork);
            this.Name = "NetWTypeSelect";
            this.toolTipContainerCHIN.ResumeLayout(false);
            this.panelCHIN.ResumeLayout(false);
            this.panelCHIN.PerformLayout();
            this.toolTipContainerExternal.ResumeLayout(false);
            this.panelExternal.ResumeLayout(false);
            this.panelExternal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNetTypeSel;
        private System.Windows.Forms.RadioButton rbtnBondedNetwork;
        private System.Windows.Forms.RadioButton rbtnExternalNetwork;
        private System.Windows.Forms.RadioButton rbtnInternalNetwork;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerExternal;
        private System.Windows.Forms.Label labelCHIN;
        private System.Windows.Forms.RadioButton rbtnCHIN;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerCHIN;
        private System.Windows.Forms.Panel panelExternal;
        private System.Windows.Forms.Panel panelCHIN;

    }
}
