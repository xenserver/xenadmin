namespace XenAdmin.Wizards.ImportWizard
{
    partial class GlobalSelectHost
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GlobalSelectHost));
            this.m_poolHostPicker = new XenAdmin.Controls.PoolHostPicker();
            this.m_buttonAddNewServer = new System.Windows.Forms.Button();
            this.label1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_poolHostPicker
            // 
            resources.ApplyResources(this.m_poolHostPicker, "m_poolHostPicker");
            this.m_poolHostPicker.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.m_poolHostPicker.FormattingEnabled = true;
            this.m_poolHostPicker.Name = "m_poolHostPicker";
            this.m_poolHostPicker.NodeIndent = 19;
            this.m_poolHostPicker.RootAlwaysExpanded = false;
            this.m_poolHostPicker.ShowCheckboxes = false;
            this.m_poolHostPicker.ShowDescription = true;
            this.m_poolHostPicker.ShowImages = true;
            this.m_poolHostPicker.ShowRootLines = true;
            // 
            // m_buttonAddNewServer
            // 
            resources.ApplyResources(this.m_buttonAddNewServer, "m_buttonAddNewServer");
            this.m_buttonAddNewServer.Image = global::XenAdmin.Properties.Resources._000_AddApplicationServer_h32bit_16;
            this.m_buttonAddNewServer.Name = "m_buttonAddNewServer";
            this.m_buttonAddNewServer.UseVisualStyleBackColor = true;
            this.m_buttonAddNewServer.Click += new System.EventHandler(this.m_buttonAddNewServer_Click);
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.m_poolHostPicker, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_buttonAddNewServer, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // GlobalSelectHost
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "GlobalSelectHost";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.PoolHostPicker m_poolHostPicker;
        private System.Windows.Forms.Button m_buttonAddNewServer;
        private XenAdmin.Controls.Common.AutoHeightLabel label1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

    }
}
