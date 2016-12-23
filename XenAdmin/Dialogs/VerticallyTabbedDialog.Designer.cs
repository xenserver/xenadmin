namespace XenAdmin.Dialogs
{
    partial class VerticallyTabbedDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerticallyTabbedDialog));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.verticalTabs = new XenAdmin.Controls.VerticalTabs();
            this.blueBorder = new XenAdmin.Controls.BlueBorderPanel();
            this.TopPanel = new XenAdmin.Controls.GradientPanel.GradientPanel();
            this.TabImage = new System.Windows.Forms.PictureBox();
            this.TabTitle = new System.Windows.Forms.Label();
            this.ContentPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.blueBorder.SuspendLayout();
            this.TopPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TabImage)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            resources.ApplyResources(this.splitContainer, "splitContainer");
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.verticalTabs);
            resources.ApplyResources(this.splitContainer.Panel1, "splitContainer.Panel1");
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.blueBorder);
            resources.ApplyResources(this.splitContainer.Panel2, "splitContainer.Panel2");
            // 
            // verticalTabs
            // 
            resources.ApplyResources(this.verticalTabs, "verticalTabs");
            this.verticalTabs.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.verticalTabs.FormattingEnabled = true;
            this.verticalTabs.Name = "verticalTabs";
            this.verticalTabs.SelectedIndexChanged += new System.EventHandler(this.verticalTabs_SelectedIndexChanged);
            // 
            // blueBorder
            // 
            this.blueBorder.BackColor = System.Drawing.SystemColors.Window;
            this.blueBorder.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(158)))), ((int)(((byte)(189)))));
            this.blueBorder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.blueBorder.Controls.Add(this.TopPanel);
            this.blueBorder.Controls.Add(this.ContentPanel);
            resources.ApplyResources(this.blueBorder, "blueBorder");
            this.blueBorder.Name = "blueBorder";
            // 
            // TopPanel
            // 
            resources.ApplyResources(this.TopPanel, "TopPanel");
            this.TopPanel.Controls.Add(this.TabImage);
            this.TopPanel.Controls.Add(this.TabTitle);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Scheme = XenAdmin.Controls.GradientPanel.GradientPanel.Schemes.Tab;
            // 
            // TabImage
            // 
            this.TabImage.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.TabImage, "TabImage");
            this.TabImage.Name = "TabImage";
            this.TabImage.TabStop = false;
            // 
            // TabTitle
            // 
            resources.ApplyResources(this.TabTitle, "TabTitle");
            this.TabTitle.AutoEllipsis = true;
            this.TabTitle.BackColor = System.Drawing.Color.Transparent;
            this.TabTitle.ForeColor = System.Drawing.Color.White;
            this.TabTitle.Name = "TabTitle";
            this.TabTitle.UseMnemonic = false;
            // 
            // ContentPanel
            // 
            resources.ApplyResources(this.ContentPanel, "ContentPanel");
            this.ContentPanel.Name = "ContentPanel";
            this.ContentPanel.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.ContentPanel_ControlAdded);
            this.ContentPanel.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.ContentPanel_ControlRemoved);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // VerticallyTabbedDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "VerticallyTabbedDialog";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.blueBorder.ResumeLayout(false);
            this.TopPanel.ResumeLayout(false);
            this.TopPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TabImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox TabImage;
        private System.Windows.Forms.Label TabTitle;
        private XenAdmin.Controls.GradientPanel.GradientPanel TopPanel;
        protected System.Windows.Forms.Panel ContentPanel;
        protected XenAdmin.Controls.VerticalTabs verticalTabs;
        protected System.Windows.Forms.Button cancelButton;
        public System.Windows.Forms.Button okButton;
        protected System.Windows.Forms.SplitContainer splitContainer;
        protected XenAdmin.Controls.BlueBorderPanel blueBorder;
    }
}
