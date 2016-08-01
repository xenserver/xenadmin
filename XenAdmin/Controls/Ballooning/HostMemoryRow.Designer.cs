namespace XenAdmin.Controls.Ballooning
{
    partial class HostMemoryRow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HostMemoryRow));
            this.panel = new XenAdmin.Controls.PanelWithBorder();
            this.hostMemoryControls = new XenAdmin.Controls.Ballooning.HostMemoryControls();
            this.memoryRowLabel = new XenAdmin.Controls.Ballooning.MemoryRowLabel();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            resources.ApplyResources(this.panel, "panel");
            this.panel.BackColor = System.Drawing.Color.Transparent;
            this.panel.Controls.Add(this.hostMemoryControls);
            this.panel.Controls.Add(this.memoryRowLabel);
            this.panel.Name = "panel";
            // 
            // hostMemoryControls
            // 
            resources.ApplyResources(this.hostMemoryControls, "hostMemoryControls");
            this.hostMemoryControls.BackColor = System.Drawing.Color.Transparent;
            this.hostMemoryControls.Name = "hostMemoryControls";
            // 
            // memoryRowLabel
            // 
            resources.ApplyResources(this.memoryRowLabel, "memoryRowLabel");
            this.memoryRowLabel.BackColor = System.Drawing.Color.Transparent;
            this.memoryRowLabel.Name = "memoryRowLabel";
            // 
            // HostMemoryRow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel);
            this.DoubleBuffered = true;
            this.Name = "HostMemoryRow";
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private HostMemoryControls hostMemoryControls;
        private MemoryRowLabel memoryRowLabel;
        private PanelWithBorder panel;
    }
}
