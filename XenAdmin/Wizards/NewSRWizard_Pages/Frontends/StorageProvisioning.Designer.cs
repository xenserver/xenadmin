namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class StorageProvisioning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StorageProvisioning));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonThickProvisioning = new System.Windows.Forms.RadioButton();
            this.radioButtonThinProvisioning = new System.Windows.Forms.RadioButton();
            this.thinProvisioningAllocationsControl = new XenAdmin.Controls.ThinProvisioningParametersControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonThickProvisioning, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonThinProvisioning, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.thinProvisioningAllocationsControl, 1, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 5);
            this.label1.Name = "label1";
            // 
            // radioButtonThickProvisioning
            // 
            resources.ApplyResources(this.radioButtonThickProvisioning, "radioButtonThickProvisioning");
            this.radioButtonThickProvisioning.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonThickProvisioning, 5);
            this.radioButtonThickProvisioning.Name = "radioButtonThickProvisioning";
            this.radioButtonThickProvisioning.TabStop = true;
            this.radioButtonThickProvisioning.UseVisualStyleBackColor = true;
            // 
            // radioButtonThinProvisioning
            // 
            resources.ApplyResources(this.radioButtonThinProvisioning, "radioButtonThinProvisioning");
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonThinProvisioning, 5);
            this.radioButtonThinProvisioning.Name = "radioButtonThinProvisioning";
            this.radioButtonThinProvisioning.UseVisualStyleBackColor = true;
            // 
            // thinProvisioningAllocationsControl
            // 
            resources.ApplyResources(this.thinProvisioningAllocationsControl, "thinProvisioningAllocationsControl");
            this.tableLayoutPanel1.SetColumnSpan(this.thinProvisioningAllocationsControl, 3);
            this.thinProvisioningAllocationsControl.Name = "thinProvisioningAllocationsControl";
            this.thinProvisioningAllocationsControl.Enter += new System.EventHandler(this.thinProvisioningAllocationsControl_Enter);
            // 
            // StorageProvisioning
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "StorageProvisioning";
            resources.ApplyResources(this, "$this");
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonThickProvisioning;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonThinProvisioning;
        private Controls.ThinProvisioningParametersControl thinProvisioningAllocationsControl;
    }
}
