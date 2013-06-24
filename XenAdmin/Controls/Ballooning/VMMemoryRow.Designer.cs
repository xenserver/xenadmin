namespace XenAdmin.Controls.Ballooning
{
    partial class VMMemoryRow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VMMemoryRow));
            this.panelControls = new XenAdmin.Controls.PanelWithBorder();
            this.vmMemoryControls = new XenAdmin.Controls.Ballooning.VMMemoryControlsNoEdit();
            this.panelLabel = new XenAdmin.Controls.PanelWithBorder();
            this.memoryRowLabel = new XenAdmin.Controls.Ballooning.MemoryRowLabel();
            this.panelControls.SuspendLayout();
            this.panelLabel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControls
            // 
            resources.ApplyResources(this.panelControls, "panelControls");
            this.panelControls.BackColor = System.Drawing.Color.Transparent;
            this.panelControls.Controls.Add(this.vmMemoryControls);
            this.panelControls.Name = "panelControls";
            // 
            // vmMemoryControls
            // 
            resources.ApplyResources(this.vmMemoryControls, "vmMemoryControls");
            this.vmMemoryControls.BackColor = System.Drawing.Color.Transparent;
            this.vmMemoryControls.Name = "vmMemoryControls";
            // 
            // panelLabel
            // 
            resources.ApplyResources(this.panelLabel, "panelLabel");
            this.panelLabel.BackColor = System.Drawing.Color.Transparent;
            this.panelLabel.Controls.Add(this.memoryRowLabel);
            this.panelLabel.Name = "panelLabel";
            // 
            // memoryRowLabel
            // 
            resources.ApplyResources(this.memoryRowLabel, "memoryRowLabel");
            this.memoryRowLabel.BackColor = System.Drawing.Color.Gainsboro;
            this.memoryRowLabel.MinimumSize = new System.Drawing.Size(0, 26);
            this.memoryRowLabel.Name = "memoryRowLabel";
            this.memoryRowLabel.SizeChanged += new System.EventHandler(this.memoryRowLabel_SizeChanged);
            // 
            // VMMemoryRow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.panelLabel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(700, 0);
            this.Name = "VMMemoryRow";
            this.panelControls.ResumeLayout(false);
            this.panelLabel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private VMMemoryControlsNoEdit vmMemoryControls;
        private MemoryRowLabel memoryRowLabel;
        private PanelWithBorder panelLabel;
        private PanelWithBorder panelControls;
    }
}
