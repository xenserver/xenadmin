namespace XenAdmin.Wizards.NewVMWizard
{
    partial class Page_HomeServer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_HomeServer));
            this.affinityPicker1 = new XenAdmin.Controls.AffinityPicker();
            this.SuspendLayout();
            // 
            // affinityPicker1
            // 
            resources.ApplyResources(this.affinityPicker1, "affinityPicker1");
            this.affinityPicker1.Name = "affinityPicker1";
            this.affinityPicker1.SelectedAffinityChanged += new System.EventHandler(this.affinityPicker1_SelectedAffinityChanged);
            // 
            // Page_HomeServer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.affinityPicker1);
            this.Name = "Page_HomeServer";
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.AffinityPicker affinityPicker1;
    }
}
