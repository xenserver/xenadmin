namespace XenAdmin.Dialogs
{
    partial class PvsCacheConfigurationDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PvsCacheConfigurationDialog));
            this.BlurbLabel = new System.Windows.Forms.Label();
            this.AddButton = new System.Windows.Forms.Button();
            this.addSiteButton = new System.Windows.Forms.Button();
            this.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.blueBorder.SuspendLayout();
            this.SuspendLayout();
            // 
            // ContentPanel
            // 
            resources.ApplyResources(this.ContentPanel, "ContentPanel");
            this.ContentPanel.Controls.Add(this.addSiteButton);
            // 
            // verticalTabs
            // 
            this.verticalTabs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.verticalTabs, "verticalTabs");
            this.verticalTabs.MouseClick += new System.Windows.Forms.MouseEventHandler(this.verticalTabs_MouseClick);
            this.verticalTabs.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.verticalTabs_DrawItem);
            this.verticalTabs.MouseMove += new System.Windows.Forms.MouseEventHandler(this.verticalTabs_MouseMove);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.splitContainer, "splitContainer");
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer.Panel1.Controls.Add(this.AddButton);
            this.splitContainer.Panel1.Resize += new System.EventHandler(this.splitContainer_Panel1_Resize);
            // 
            // blueBorder
            // 
            this.blueBorder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.blueBorder, "blueBorder");
            // 
            // BlurbLabel
            // 
            resources.ApplyResources(this.BlurbLabel, "BlurbLabel");
            this.BlurbLabel.Name = "BlurbLabel";
            this.BlurbLabel.UseMnemonic = false;
            // 
            // AddButton
            // 
            resources.ApplyResources(this.AddButton, "AddButton");
            this.AddButton.BackColor = System.Drawing.SystemColors.Window;
            this.AddButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AddButton.FlatAppearance.BorderSize = 0;
            this.AddButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.AddButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.AddButton.Image = global::XenAdmin.Properties.Resources._000_AddSite_h32bit_16;
            this.AddButton.Name = "AddButton";
            this.AddButton.UseVisualStyleBackColor = false;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // addSiteButton
            // 
            resources.ApplyResources(this.addSiteButton, "addSiteButton");
            this.addSiteButton.BackColor = System.Drawing.Color.Transparent;
            this.addSiteButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.addSiteButton.FlatAppearance.BorderSize = 0;
            this.addSiteButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.addSiteButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.addSiteButton.Image = global::XenAdmin.Properties.Resources._000_AddSite_h32bit_16;
            this.addSiteButton.Name = "addSiteButton";
            this.addSiteButton.UseVisualStyleBackColor = false;
            this.addSiteButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // PvsCacheConfigurationDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.BlurbLabel);
            this.Name = "PvsCacheConfigurationDialog";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PvsCacheConfigurationDialog_FormClosed);
            this.Controls.SetChildIndex(this.okButton, 0);
            this.Controls.SetChildIndex(this.cancelButton, 0);
            this.Controls.SetChildIndex(this.splitContainer, 0);
            this.Controls.SetChildIndex(this.BlurbLabel, 0);
            this.ContentPanel.ResumeLayout(false);
            this.ContentPanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.blueBorder.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label BlurbLabel;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button addSiteButton;
    }
}
