using System.Windows.Forms;

namespace XenAdmin.Wizards.NewVMWizard
{
    partial class NewVMWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewVMWizard));
            this.SuspendLayout();
            // 
            // NewVMWizard
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "NewVMWizard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


    }
}
