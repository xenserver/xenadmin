namespace XenAdmin.Controls
{
    partial class GpuPlacementPolicyPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GpuPlacementPolicyPanel));
            this.panelWithBorder = new XenAdmin.Controls.PanelWithBorder();
            this.containerPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.placementPolicyLabel = new System.Windows.Forms.Label();
            this.editPlacementPolicyButton = new System.Windows.Forms.Button();
            this.panelWithBorder.SuspendLayout();
            this.containerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelWithBorder
            // 
            resources.ApplyResources(this.panelWithBorder, "panelWithBorder");
            this.panelWithBorder.BackColor = System.Drawing.Color.Transparent;
            this.panelWithBorder.Controls.Add(this.containerPanel);
            this.panelWithBorder.Name = "panelWithBorder";
            // 
            // containerPanel
            // 
            resources.ApplyResources(this.containerPanel, "containerPanel");
            this.containerPanel.Controls.Add(this.placementPolicyLabel);
            this.containerPanel.Controls.Add(this.editPlacementPolicyButton);
            this.containerPanel.Name = "containerPanel";
            // 
            // placementPolicyLabel
            // 
            resources.ApplyResources(this.placementPolicyLabel, "placementPolicyLabel");
            this.placementPolicyLabel.AutoEllipsis = true;
            this.placementPolicyLabel.Name = "placementPolicyLabel";
            // 
            // editPlacementPolicyButton
            // 
            resources.ApplyResources(this.editPlacementPolicyButton, "editPlacementPolicyButton");
            this.editPlacementPolicyButton.Name = "editPlacementPolicyButton";
            this.editPlacementPolicyButton.UseVisualStyleBackColor = true;
            this.editPlacementPolicyButton.Click += new System.EventHandler(this.editPlacementPolicyButton_Click);
            // 
            // GpuPlacementPolicyPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panelWithBorder);
            this.DoubleBuffered = true;
            this.Name = "GpuPlacementPolicyPanel";
            this.VisibleChanged += new System.EventHandler(this.GpuPlacementPolicyPanel_VisibleChanged);
            this.panelWithBorder.ResumeLayout(false);
            this.containerPanel.ResumeLayout(false);
            this.containerPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PanelWithBorder panelWithBorder;
        private System.Windows.Forms.Button editPlacementPolicyButton;
        private System.Windows.Forms.Label placementPolicyLabel;
        private System.Windows.Forms.FlowLayoutPanel containerPanel;

    }
}
