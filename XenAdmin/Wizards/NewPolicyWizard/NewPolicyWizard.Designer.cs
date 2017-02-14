using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.NewPolicyWizard
{
    partial class NewPolicyWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewPolicyWizard));
            this.SuspendLayout();
            // 
            // NewPolicyWizard
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.HelpButton = true;
            this.Name = "NewPolicyWizard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    }
}