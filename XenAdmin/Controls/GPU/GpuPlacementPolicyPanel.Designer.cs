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
            this.panel = new XenAdmin.Controls.PanelWithBorder();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.placementPolicyLabel = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.editPlacementPolicyButton = new System.Windows.Forms.Button();
            this.panel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            resources.ApplyResources(this.panel, "panel");
            this.panel.BackColor = System.Drawing.Color.Transparent;
            this.panel.Controls.Add(this.tableLayoutPanel1);
            this.panel.Name = "panel";
            this.panel.VisibleChanged += new System.EventHandler(this.GpuPlacementPolicyPanel_VisibleChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.placementPolicyLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.editPlacementPolicyButton, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // placementPolicyLabel
            // 
            resources.ApplyResources(this.placementPolicyLabel, "placementPolicyLabel");
            this.placementPolicyLabel.AutoEllipsis = true;
            this.placementPolicyLabel.BackColor = System.Drawing.Color.Transparent;
            this.placementPolicyLabel.MinimumSize = new System.Drawing.Size(0, 16);
            this.placementPolicyLabel.Name = "placementPolicyLabel";
            // 
            // editPlacementPolicyButton
            // 
            resources.ApplyResources(this.editPlacementPolicyButton, "editPlacementPolicyButton");
            this.editPlacementPolicyButton.BackColor = System.Drawing.Color.Transparent;
            this.editPlacementPolicyButton.Name = "editPlacementPolicyButton";
            this.editPlacementPolicyButton.UseVisualStyleBackColor = false;
            this.editPlacementPolicyButton.Click += new System.EventHandler(this.editPlacementPolicyButton_Click);
            // 
            // GpuPlacementPolicyPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(519, 32);
            this.Name = "GpuPlacementPolicyPanel";
            this.panel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PanelWithBorder panel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button editPlacementPolicyButton;
        private XenAdmin.Controls.Common.AutoHeightLabel placementPolicyLabel;
    }
}
