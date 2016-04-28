namespace XenAdmin.Controls
{
    partial class SrProvisioningMethod
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SrProvisioningMethod));
            this.groupBoxProvisioningMethod = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelBondMode = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonLvm = new System.Windows.Forms.RadioButton();
            this.radioButtonGfs2 = new System.Windows.Forms.RadioButton();
            this.groupBoxProvisioningMethod.SuspendLayout();
            this.tableLayoutPanelBondMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxProvisioningMethod
            // 
            resources.ApplyResources(this.groupBoxProvisioningMethod, "groupBoxProvisioningMethod");
            this.groupBoxProvisioningMethod.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxProvisioningMethod.Controls.Add(this.tableLayoutPanelBondMode);
            this.groupBoxProvisioningMethod.Name = "groupBoxProvisioningMethod";
            this.groupBoxProvisioningMethod.TabStop = false;
            // 
            // tableLayoutPanelBondMode
            // 
            resources.ApplyResources(this.tableLayoutPanelBondMode, "tableLayoutPanelBondMode");
            this.tableLayoutPanelBondMode.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanelBondMode.Controls.Add(this.radioButtonLvm, 0, 0);
            this.tableLayoutPanelBondMode.Controls.Add(this.radioButtonGfs2, 1, 0);
            this.tableLayoutPanelBondMode.Name = "tableLayoutPanelBondMode";
            // 
            // radioButtonLvm
            // 
            resources.ApplyResources(this.radioButtonLvm, "radioButtonLvm");
            this.radioButtonLvm.Checked = true;
            this.radioButtonLvm.Name = "radioButtonLvm";
            this.radioButtonLvm.TabStop = true;
            this.radioButtonLvm.UseVisualStyleBackColor = true;
            // 
            // radioButtonGfs2
            // 
            resources.ApplyResources(this.radioButtonGfs2, "radioButtonGfs2");
            this.radioButtonGfs2.Name = "radioButtonGfs2";
            this.radioButtonGfs2.UseVisualStyleBackColor = true;
            // 
            // SrProvisioningMethod
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.groupBoxProvisioningMethod);
            this.Name = "SrProvisioningMethod";
            this.groupBoxProvisioningMethod.ResumeLayout(false);
            this.groupBoxProvisioningMethod.PerformLayout();
            this.tableLayoutPanelBondMode.ResumeLayout(false);
            this.tableLayoutPanelBondMode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxProvisioningMethod;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBondMode;
        private System.Windows.Forms.RadioButton radioButtonLvm;
        private System.Windows.Forms.RadioButton radioButtonGfs2;
    }
}
