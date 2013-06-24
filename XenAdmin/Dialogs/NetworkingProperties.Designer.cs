namespace XenAdmin.Dialogs
{
    partial class NetworkingProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkingProperties));
            this.BlurbLabel = new System.Windows.Forms.Label();
            this.AddButton = new System.Windows.Forms.Button();
            this.linkLabelTellMeMore = new System.Windows.Forms.LinkLabel();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.blueBorder.SuspendLayout();
            this.SuspendLayout();
            // 
            // ContentPanel
            // 
            resources.ApplyResources(this.ContentPanel, "ContentPanel");
            // 
            // verticalTabs
            // 
            this.verticalTabs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.verticalTabs, "verticalTabs");
            this.verticalTabs.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.verticalTabs_DrawItem);
            this.verticalTabs.MouseClick += new System.Windows.Forms.MouseEventHandler(this.verticalTabs_MouseClick);
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
            this.okButton.Click += new System.EventHandler(this.AcceptBtn_Click);
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
            resources.ApplyResources(this.splitContainer.Panel1, "splitContainer.Panel1");
            this.splitContainer.Panel1.Resize += new System.EventHandler(this.splitContainer_Panel1_Resize);
            // 
            // splitContainer.Panel2
            // 
            resources.ApplyResources(this.splitContainer.Panel2, "splitContainer.Panel2");
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
            this.AddButton.Image = global::XenAdmin.Properties.Resources._000_AddIPAddress_h32bit_16;
            this.AddButton.Name = "AddButton";
            this.AddButton.UseVisualStyleBackColor = false;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // linkLabelTellMeMore
            // 
            resources.ApplyResources(this.linkLabelTellMeMore, "linkLabelTellMeMore");
            this.linkLabelTellMeMore.Name = "linkLabelTellMeMore";
            this.linkLabelTellMeMore.TabStop = true;
            this.linkLabelTellMeMore.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelTellMeMore_LinkClicked);
            // 
            // NetworkingProperties
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.linkLabelTellMeMore);
            this.Controls.Add(this.BlurbLabel);
            this.Name = "NetworkingProperties";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.NetworkingProperties_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.NetworkingProperties_HelpRequested);
            this.Controls.SetChildIndex(this.okButton, 0);
            this.Controls.SetChildIndex(this.cancelButton, 0);
            this.Controls.SetChildIndex(this.splitContainer, 0);
            this.Controls.SetChildIndex(this.BlurbLabel, 0);
            this.Controls.SetChildIndex(this.linkLabelTellMeMore, 0);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.blueBorder.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label BlurbLabel;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.LinkLabel linkLabelTellMeMore;
    }
}
