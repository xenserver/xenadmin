namespace XenAdmin.Wizards.BallooningWizard_Pages
{
    partial class MemorySettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MemorySettings));
            this.memoryControlsBasic = new XenAdmin.Controls.Ballooning.VMMemoryControlsBasic();
            this.memoryControlsAdvanced = new XenAdmin.Controls.Ballooning.VMMemoryControlsAdvanced();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // memoryControlsBasic
            // 
            resources.ApplyResources(this.memoryControlsBasic, "memoryControlsBasic");
            this.memoryControlsBasic.Name = "memoryControlsBasic";
            this.memoryControlsBasic.InstallTools += new System.EventHandler(this.memoryControlsBasic_InstallTools);
            // 
            // memoryControlsAdvanced
            // 
            resources.ApplyResources(this.memoryControlsAdvanced, "memoryControlsAdvanced");
            this.memoryControlsAdvanced.Name = "memoryControlsAdvanced";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // MemorySettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.memoryControlsBasic);
            this.Controls.Add(this.memoryControlsAdvanced);
            this.Name = "MemorySettings";
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.Ballooning.VMMemoryControlsBasic memoryControlsBasic;
        private XenAdmin.Controls.Ballooning.VMMemoryControlsAdvanced memoryControlsAdvanced;
        private System.Windows.Forms.Label label1;


    }
}
