namespace XenAdmin.Dialogs.OptionsPages
{
    partial class UpdatesOptionsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdatesOptionsPage));
            this.UpdatesTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.AllowXenServerUpdatesCheckBox = new System.Windows.Forms.CheckBox();
            this.AllowXenCenterUpdatesCheckBox = new System.Windows.Forms.CheckBox();
            this.UpdatesBlurb = new System.Windows.Forms.Label();
            this.UpdatesTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // UpdatesTableLayoutPanel
            // 
            resources.ApplyResources(this.UpdatesTableLayoutPanel, "UpdatesTableLayoutPanel");
            this.UpdatesTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.UpdatesTableLayoutPanel.Controls.Add(this.UpdatesBlurb, 0, 0);
            this.UpdatesTableLayoutPanel.Controls.Add(this.AllowXenCenterUpdatesCheckBox, 0, 1);
            this.UpdatesTableLayoutPanel.Controls.Add(this.AllowXenServerUpdatesCheckBox, 0, 2);
            this.UpdatesTableLayoutPanel.Name = "UpdatesTableLayoutPanel";
            // 
            // AllowXenServerUpdatesCheckBox
            // 
            resources.ApplyResources(this.AllowXenServerUpdatesCheckBox, "AllowXenServerUpdatesCheckBox");
            this.AllowXenServerUpdatesCheckBox.Name = "AllowXenServerUpdatesCheckBox";
            this.AllowXenServerUpdatesCheckBox.UseVisualStyleBackColor = true;
            // 
            // AllowXenCenterUpdatesCheckBox
            // 
            resources.ApplyResources(this.AllowXenCenterUpdatesCheckBox, "AllowXenCenterUpdatesCheckBox");
            this.AllowXenCenterUpdatesCheckBox.Name = "AllowXenCenterUpdatesCheckBox";
            this.AllowXenCenterUpdatesCheckBox.UseVisualStyleBackColor = true;
            // 
            // UpdatesBlurb
            // 
            resources.ApplyResources(this.UpdatesBlurb, "UpdatesBlurb");
            this.UpdatesBlurb.Name = "UpdatesBlurb";
            // 
            // UpdatesOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.UpdatesTableLayoutPanel);
            this.Name = "UpdatesOptionsPage";
            this.UpdatesTableLayoutPanel.ResumeLayout(false);
            this.UpdatesTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel UpdatesTableLayoutPanel;
        private System.Windows.Forms.Label UpdatesBlurb;
        private System.Windows.Forms.CheckBox AllowXenServerUpdatesCheckBox;
        private System.Windows.Forms.CheckBox AllowXenCenterUpdatesCheckBox;
    }
}
