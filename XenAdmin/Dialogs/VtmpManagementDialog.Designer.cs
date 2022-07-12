namespace XenAdmin.Dialogs
{
    partial class VtpmManagementDialog
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
            if (disposing)
            {
                UnregisterEvents();
                if (components != null)
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VtpmManagementDialog));
            this.BlurbLabel = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipContainerAdd = new XenAdmin.Controls.ToolTipContainer();
            this.labelNoVtpms = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.blueBorder.SuspendLayout();
            this.toolTipContainerAdd.SuspendLayout();
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
            this.verticalTabs.MouseClick += new System.Windows.Forms.MouseEventHandler(this.verticalTabs_MouseClick);
            this.verticalTabs.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.verticalTabs_DrawItem);
            this.verticalTabs.MouseMove += new System.Windows.Forms.MouseEventHandler(this.verticalTabs_MouseMove);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.splitContainer, "splitContainer");
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer.Panel1.Controls.Add(this.toolTipContainerAdd);
            this.splitContainer.Panel1.Resize += new System.EventHandler(this.splitContainer_Panel1_Resize);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.labelNoVtpms);
            // 
            // blueBorder
            // 
            resources.ApplyResources(this.blueBorder, "blueBorder");
            // 
            // BlurbLabel
            // 
            resources.ApplyResources(this.BlurbLabel, "BlurbLabel");
            this.BlurbLabel.Name = "BlurbLabel";
            this.BlurbLabel.UseMnemonic = false;
            // 
            // addButton
            // 
            this.addButton.BackColor = System.Drawing.SystemColors.Window;
            this.addButton.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.FlatAppearance.BorderSize = 0;
            this.addButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.addButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.addButton.Image = global::XenAdmin.Properties.Resources.sl_add_storage_system_small_16;
            this.addButton.Name = "addButton";
            this.addButton.UseVisualStyleBackColor = false;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // toolTipContainerAdd
            // 
            this.toolTipContainerAdd.Controls.Add(this.addButton);
            this.toolTipContainerAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.toolTipContainerAdd, "toolTipContainerAdd");
            this.toolTipContainerAdd.Name = "toolTipContainerAdd";
            // 
            // labelNoVtpms
            // 
            this.labelNoVtpms.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.labelNoVtpms, "labelNoVtpms");
            this.labelNoVtpms.Name = "labelNoVtpms";
            // 
            // VtpmManagementDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.BlurbLabel);
            this.Name = "VtpmManagementDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.VtpmManagementDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.VtpmManagementDialog_HelpRequested);
            this.Controls.SetChildIndex(this.okButton, 0);
            this.Controls.SetChildIndex(this.cancelButton, 0);
            this.Controls.SetChildIndex(this.splitContainer, 0);
            this.Controls.SetChildIndex(this.BlurbLabel, 0);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.blueBorder.ResumeLayout(false);
            this.toolTipContainerAdd.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label BlurbLabel;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ToolTip toolTip;
        private Controls.ToolTipContainer toolTipContainerAdd;
        private System.Windows.Forms.Label labelNoVtpms;
    }
}
