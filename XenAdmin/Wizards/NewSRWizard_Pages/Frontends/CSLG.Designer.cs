namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class CSLG
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSLG));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelSystem = new System.Windows.Forms.Label();
            this.labelStorageSystem = new System.Windows.Forms.Label();
            this.comboBoxStorageSystem = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelStorageLinkPropertiesLinkBlurb = new System.Windows.Forms.Label();
            this.linkLabelGotoStorageLinkProperties = new System.Windows.Forms.LinkLabel();
            this.labelAdapter = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelSystem, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelAdapter, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelStorageSystem, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxStorageSystem, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelSystem
            // 
            resources.ApplyResources(this.labelSystem, "labelSystem");
            this.tableLayoutPanel1.SetColumnSpan(this.labelSystem, 2);
            this.labelSystem.Name = "labelSystem";
            // 
            // labelStorageSystem
            // 
            resources.ApplyResources(this.labelStorageSystem, "labelStorageSystem");
            this.labelStorageSystem.Name = "labelStorageSystem";
            // 
            // comboBoxStorageSystem
            // 
            resources.ApplyResources(this.comboBoxStorageSystem, "comboBoxStorageSystem");
            this.comboBoxStorageSystem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxStorageSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStorageSystem.FormattingEnabled = true;
            this.comboBoxStorageSystem.Name = "comboBoxStorageSystem";
            this.comboBoxStorageSystem.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBoxStorageSystem_DrawItem);
            this.comboBoxStorageSystem.SelectedIndexChanged += new System.EventHandler(this.comboBoxStorageSystem_SelectedIndexChanged);
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.labelStorageLinkPropertiesLinkBlurb);
            this.flowLayoutPanel1.Controls.Add(this.linkLabelGotoStorageLinkProperties);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // labelStorageLinkPropertiesLinkBlurb
            // 
            resources.ApplyResources(this.labelStorageLinkPropertiesLinkBlurb, "labelStorageLinkPropertiesLinkBlurb");
            this.labelStorageLinkPropertiesLinkBlurb.Name = "labelStorageLinkPropertiesLinkBlurb";
            // 
            // linkLabelGotoStorageLinkProperties
            // 
            resources.ApplyResources(this.linkLabelGotoStorageLinkProperties, "linkLabelGotoStorageLinkProperties");
            this.linkLabelGotoStorageLinkProperties.Name = "linkLabelGotoStorageLinkProperties";
            this.linkLabelGotoStorageLinkProperties.TabStop = true;
            this.linkLabelGotoStorageLinkProperties.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGotoStorageLinkProperties_LinkClicked);
            // 
            // labelAdapter
            // 
            resources.ApplyResources(this.labelAdapter, "labelAdapter");
            this.tableLayoutPanel1.SetColumnSpan(this.labelAdapter, 2);
            this.labelAdapter.Name = "labelAdapter";
            // 
            // CSLG
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CSLG";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelStorageSystem;
        private System.Windows.Forms.ComboBox comboBoxStorageSystem;
        private System.Windows.Forms.LinkLabel linkLabelGotoStorageLinkProperties;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labelStorageLinkPropertiesLinkBlurb;
        private System.Windows.Forms.Label labelSystem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelAdapter;
    }
}
