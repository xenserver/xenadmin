using System.Drawing;

namespace XenAdmin.Wizards
{
    partial class BootModesControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BootModesControl));
            this.groupBoxBootMode = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelBootMode = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonUEFISecureBoot = new System.Windows.Forms.RadioButton();
            this.radioButtonBIOSBoot = new System.Windows.Forms.RadioButton();
            this.radioButtonUEFIBoot = new System.Windows.Forms.RadioButton();
            this.groupBoxBootMode.SuspendLayout();
            this.tableLayoutPanelBootMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxBootMode
            // 
            this.groupBoxBootMode.Controls.Add(this.tableLayoutPanelBootMode);
            resources.ApplyResources(this.groupBoxBootMode, "groupBoxBootMode");
            this.groupBoxBootMode.Name = "groupBoxBootMode";
            this.groupBoxBootMode.TabStop = false;
            // 
            // tableLayoutPanelBootMode
            // 
            this.tableLayoutPanelBootMode.Controls.Add(this.radioButtonUEFISecureBoot, 0, 2);
            this.tableLayoutPanelBootMode.Controls.Add(this.radioButtonBIOSBoot, 0, 0);
            this.tableLayoutPanelBootMode.Controls.Add(this.radioButtonUEFIBoot, 0, 1);
            resources.ApplyResources(this.tableLayoutPanelBootMode, "tableLayoutPanelBootMode");
            this.tableLayoutPanelBootMode.Name = "tableLayoutPanelBootMode";
            // 
            // radioButtonUEFISecureBoot
            // 
            resources.ApplyResources(this.radioButtonUEFISecureBoot, "radioButtonUEFISecureBoot");
            this.radioButtonUEFISecureBoot.Name = "radioButtonUEFISecureBoot";
            this.radioButtonUEFISecureBoot.UseVisualStyleBackColor = true;
            // 
            // radioButtonBIOSBoot
            // 
            resources.ApplyResources(this.radioButtonBIOSBoot, "radioButtonBIOSBoot");
            this.radioButtonBIOSBoot.Name = "radioButtonBIOSBoot";
            this.radioButtonBIOSBoot.UseVisualStyleBackColor = true;
            // 
            // radioButtonUEFIBoot
            // 
            resources.ApplyResources(this.radioButtonUEFIBoot, "radioButtonUEFIBoot");
            this.radioButtonUEFIBoot.Name = "radioButtonUEFIBoot";
            this.radioButtonUEFIBoot.UseVisualStyleBackColor = true;
            // 
            // BootModesControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.groupBoxBootMode);
            this.DoubleBuffered = true;
            this.Name = "BootModesControl";
            this.groupBoxBootMode.ResumeLayout(false);
            this.tableLayoutPanelBootMode.ResumeLayout(false);
            this.tableLayoutPanelBootMode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxBootMode;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBootMode;
        private System.Windows.Forms.RadioButton radioButtonUEFISecureBoot;
        private System.Windows.Forms.RadioButton radioButtonBIOSBoot;
        private System.Windows.Forms.RadioButton radioButtonUEFIBoot;
    }
}
