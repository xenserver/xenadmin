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
            this.UpdatesBlurb = new System.Windows.Forms.Label();
            this.AllowXenCenterUpdatesCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBoxClientId = new System.Windows.Forms.GroupBox();
            this.clientIdControl1 = new XenAdmin.Controls.ClientIdControl();
            this.UpdatesTableLayoutPanel.SuspendLayout();
            this.groupBoxClientId.SuspendLayout();
            this.SuspendLayout();
            // 
            // UpdatesTableLayoutPanel
            // 
            resources.ApplyResources(this.UpdatesTableLayoutPanel, "UpdatesTableLayoutPanel");
            this.UpdatesTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.UpdatesTableLayoutPanel.Controls.Add(this.UpdatesBlurb, 0, 0);
            this.UpdatesTableLayoutPanel.Controls.Add(this.AllowXenCenterUpdatesCheckBox, 0, 1);
            this.UpdatesTableLayoutPanel.Controls.Add(this.groupBoxClientId, 0, 2);
            this.UpdatesTableLayoutPanel.Name = "UpdatesTableLayoutPanel";
            // 
            // UpdatesBlurb
            // 
            resources.ApplyResources(this.UpdatesBlurb, "UpdatesBlurb");
            this.UpdatesBlurb.Name = "UpdatesBlurb";
            // 
            // AllowXenCenterUpdatesCheckBox
            // 
            resources.ApplyResources(this.AllowXenCenterUpdatesCheckBox, "AllowXenCenterUpdatesCheckBox");
            this.AllowXenCenterUpdatesCheckBox.Name = "AllowXenCenterUpdatesCheckBox";
            this.AllowXenCenterUpdatesCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBoxClientId
            // 
            resources.ApplyResources(this.groupBoxClientId, "groupBoxClientId");
            this.groupBoxClientId.Controls.Add(this.clientIdControl1);
            this.groupBoxClientId.Name = "groupBoxClientId";
            this.groupBoxClientId.TabStop = false;
            // 
            // clientIdControl1
            // 
            resources.ApplyResources(this.clientIdControl1, "clientIdControl1");
            this.clientIdControl1.Name = "clientIdControl1";
            // 
            // UpdatesOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.UpdatesTableLayoutPanel);
            this.Name = "UpdatesOptionsPage";
            this.UpdatesTableLayoutPanel.ResumeLayout(false);
            this.UpdatesTableLayoutPanel.PerformLayout();
            this.groupBoxClientId.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel UpdatesTableLayoutPanel;
        private System.Windows.Forms.Label UpdatesBlurb;
        private System.Windows.Forms.CheckBox AllowXenCenterUpdatesCheckBox;
        private System.Windows.Forms.GroupBox groupBoxClientId;
        private Controls.ClientIdControl clientIdControl1;
    }
}
