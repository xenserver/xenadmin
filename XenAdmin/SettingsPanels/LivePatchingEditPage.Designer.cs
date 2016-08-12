namespace XenAdmin.SettingsPanels
{
    partial class LivePatchingEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LivePatchingEditPage));
            this.labelLivePatching = new System.Windows.Forms.Label();
            this.checkLivePatchingAllowed = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labelLivePatching
            // 
            resources.ApplyResources(this.labelLivePatching, "labelLivePatching");
            this.labelLivePatching.Name = "labelLivePatching";
            // 
            // checkLivePatching
            // 
            resources.ApplyResources(this.checkLivePatchingAllowed, "checkLivePatching");
            this.checkLivePatchingAllowed.Checked = true;
            this.checkLivePatchingAllowed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkLivePatchingAllowed.Name = "checkLivePatching";
            this.checkLivePatchingAllowed.UseVisualStyleBackColor = true;
            // 
            // LivePatchingEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.checkLivePatchingAllowed);
            this.Controls.Add(this.labelLivePatching);
            this.Name = "LivePatchingEditPage";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelLivePatching;
        private System.Windows.Forms.CheckBox checkLivePatchingAllowed;
    }
}
