namespace XenAdmin.Dialogs
{
    partial class EditVmHaPrioritiesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditVmHaPrioritiesDialog));
            this.assignPriorities = new XenAdmin.Wizards.HAWizard_Pages.AssignPriorities();
            this.labelBlurb = new System.Windows.Forms.Label();
            this.pictureBoxWarningIcon = new System.Windows.Forms.PictureBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.m_panelBottom = new System.Windows.Forms.Panel();
            this.m_panelTop = new System.Windows.Forms.Panel();
            this.m_panelTopLeft = new System.Windows.Forms.Panel();
            this.m_panelMiddle = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarningIcon)).BeginInit();
            this.m_panelBottom.SuspendLayout();
            this.m_panelTop.SuspendLayout();
            this.m_panelTopLeft.SuspendLayout();
            this.m_panelMiddle.SuspendLayout();
            this.SuspendLayout();
            // 
            // assignPriorities
            // 
            resources.ApplyResources(this.assignPriorities, "assignPriorities");
            this.assignPriorities.Name = "assignPriorities";
            // 
            // labelBlurb
            // 
            this.labelBlurb.AutoEllipsis = true;
            resources.ApplyResources(this.labelBlurb, "labelBlurb");
            this.labelBlurb.Name = "labelBlurb";
            // 
            // pictureBoxWarningIcon
            // 
            resources.ApplyResources(this.pictureBoxWarningIcon, "pictureBoxWarningIcon");
            this.pictureBoxWarningIcon.Name = "pictureBoxWarningIcon";
            this.pictureBoxWarningIcon.TabStop = false;
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // m_panelBottom
            // 
            this.m_panelBottom.Controls.Add(this.buttonOk);
            this.m_panelBottom.Controls.Add(this.buttonCancel);
            resources.ApplyResources(this.m_panelBottom, "m_panelBottom");
            this.m_panelBottom.Name = "m_panelBottom";
            // 
            // m_panelTop
            // 
            this.m_panelTop.Controls.Add(this.labelBlurb);
            this.m_panelTop.Controls.Add(this.m_panelTopLeft);
            resources.ApplyResources(this.m_panelTop, "m_panelTop");
            this.m_panelTop.Name = "m_panelTop";
            // 
            // m_panelTopLeft
            // 
            this.m_panelTopLeft.Controls.Add(this.pictureBoxWarningIcon);
            resources.ApplyResources(this.m_panelTopLeft, "m_panelTopLeft");
            this.m_panelTopLeft.Name = "m_panelTopLeft";
            // 
            // m_panelMiddle
            // 
            this.m_panelMiddle.Controls.Add(this.assignPriorities);
            resources.ApplyResources(this.m_panelMiddle, "m_panelMiddle");
            this.m_panelMiddle.Name = "m_panelMiddle";
            // 
            // EditVmHaPrioritiesDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.m_panelMiddle);
            this.Controls.Add(this.m_panelTop);
            this.Controls.Add(this.m_panelBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "EditVmHaPrioritiesDialog";
            this.ShowInTaskbar = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarningIcon)).EndInit();
            this.m_panelBottom.ResumeLayout(false);
            this.m_panelTop.ResumeLayout(false);
            this.m_panelTopLeft.ResumeLayout(false);
            this.m_panelTopLeft.PerformLayout();
            this.m_panelMiddle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Wizards.HAWizard_Pages.AssignPriorities assignPriorities;
        private System.Windows.Forms.Label labelBlurb;
        private System.Windows.Forms.PictureBox pictureBoxWarningIcon;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Panel m_panelBottom;
        private System.Windows.Forms.Panel m_panelTop;
        private System.Windows.Forms.Panel m_panelTopLeft;
        private System.Windows.Forms.Panel m_panelMiddle;
    }
}