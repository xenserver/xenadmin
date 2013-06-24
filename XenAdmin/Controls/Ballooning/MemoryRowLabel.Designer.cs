using XenAPI;

namespace XenAdmin.Controls.Ballooning
{
    partial class MemoryRowLabel
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
            UnsubscribeEvents();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MemoryRowLabel));
            this.linkShowAll = new System.Windows.Forms.LinkLabel();
            this.linkHide = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // linkShowAll
            // 
            resources.ApplyResources(this.linkShowAll, "linkShowAll");
            this.linkShowAll.Name = "linkShowAll";
            this.linkShowAll.TabStop = true;
            this.linkShowAll.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkShowAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkShowAll_LinkClicked);
            // 
            // linkHide
            // 
            resources.ApplyResources(this.linkHide, "linkHide");
            this.linkHide.Name = "linkHide";
            this.linkHide.TabStop = true;
            this.linkHide.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkHide.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkHide_LinkClicked);
            // 
            // MemoryRowLabel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.linkHide);
            this.Controls.Add(this.linkShowAll);
            this.DoubleBuffered = true;
            this.Name = "MemoryRowLabel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkShowAll;
        private System.Windows.Forms.LinkLabel linkHide;

    }
}
