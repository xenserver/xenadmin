namespace XenAdmin.Wizards.ImportWizard
{
    partial class ImportBootOptionPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportBootOptionPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.bootModesControl1 = new XenAdmin.Wizards.BootModesControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.lblIntro, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.bootModesControl1, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // lblIntro
            // 
            resources.ApplyResources(this.lblIntro, "lblIntro");
            this.lblIntro.Name = "lblIntro";
            // 
            // bootModesControl1
            // 
            this.bootModesControl1.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.bootModesControl1, "bootModesControl1");
            this.bootModesControl1.Name = "bootModesControl1";
            // 
            // ImportBootOptionPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ImportBootOptionPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Controls.Common.AutoHeightLabel lblIntro;
        private BootModesControl bootModesControl1;
    }
}
