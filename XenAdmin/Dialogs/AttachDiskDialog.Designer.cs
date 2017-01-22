namespace XenAdmin.Dialogs
{
    partial class AttachDiskDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttachDiskDialog));
            this.DiskListTreeView = new XenAdmin.Controls.CustomTreeView();
            this.ReadOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.OkBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.readonlyCheckboxToolTipContainer = new XenAdmin.Controls.ToolTipContainer();
            this.readonlyCheckboxToolTipContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // DiskListTreeView
            // 
            resources.ApplyResources(this.DiskListTreeView, "DiskListTreeView");
            this.DiskListTreeView.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.DiskListTreeView.FormattingEnabled = true;
            this.DiskListTreeView.Name = "DiskListTreeView";
            this.DiskListTreeView.NodeIndent = 19;
            this.DiskListTreeView.RootAlwaysExpanded = false;
            this.DiskListTreeView.ShowCheckboxes = true;
            this.DiskListTreeView.ShowDescription = true;
            this.DiskListTreeView.ShowImages = false;
            this.DiskListTreeView.ShowRootLines = true;
            // 
            // ReadOnlyCheckBox
            // 
            resources.ApplyResources(this.ReadOnlyCheckBox, "ReadOnlyCheckBox");
            this.ReadOnlyCheckBox.Name = "ReadOnlyCheckBox";
            this.ReadOnlyCheckBox.UseVisualStyleBackColor = true;
            this.ReadOnlyCheckBox.CheckedChanged += new System.EventHandler(this.ReadOnlyCheckBox_CheckedChanged);
            // 
            // OkBtn
            // 
            resources.ApplyResources(this.OkBtn, "OkBtn");
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // CancelBtn
            // 
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // readonlyCheckboxToolTipContainer
            // 
            resources.ApplyResources(this.readonlyCheckboxToolTipContainer, "readonlyCheckboxToolTipContainer");
            this.readonlyCheckboxToolTipContainer.Controls.Add(this.ReadOnlyCheckBox);
            this.readonlyCheckboxToolTipContainer.Name = "readonlyCheckboxToolTipContainer";
            // 
            // AttachDiskDialog
            // 
            this.AcceptButton = this.OkBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.readonlyCheckboxToolTipContainer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OkBtn);
            this.Controls.Add(this.DiskListTreeView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "AttachDiskDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AttachDiskDialog_FormClosing);
            this.readonlyCheckboxToolTipContainer.ResumeLayout(false);
            this.readonlyCheckboxToolTipContainer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.CustomTreeView DiskListTreeView;
        private System.Windows.Forms.CheckBox ReadOnlyCheckBox;
        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Label label1;
        private XenAdmin.Controls.ToolTipContainer readonlyCheckboxToolTipContainer;
    }
}