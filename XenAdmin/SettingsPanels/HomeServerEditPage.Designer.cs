namespace XenAdmin.SettingsPanels
{
    partial class HomeServerEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeServerEditPage));
            this.picker = new XenAdmin.Controls.AffinityPicker();
            this.SuspendLayout();
            // 
            // picker
            // 
            resources.ApplyResources(this.picker, "picker");
            this.picker.Name = "picker";
            // 
            // HomeServerEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.picker);
            this.Name = "HomeServerEditPage";
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.AffinityPicker picker;
    }
}