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
            this.tableVtpmWarning = new System.Windows.Forms.TableLayoutPanel();
            this.labelTpm = new System.Windows.Forms.Label();
            this.imgTpm = new System.Windows.Forms.PictureBox();
            this.labelUnsupported = new System.Windows.Forms.Label();
            this.imgUnsupported = new System.Windows.Forms.PictureBox();
            this.groupBoxDevSecurity = new System.Windows.Forms.GroupBox();
            this.checkBoxVtpm = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableBootModeWarning = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxBootMode.SuspendLayout();
            this.tableLayoutPanelBootMode.SuspendLayout();
            this.tableVtpmWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgTpm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgUnsupported)).BeginInit();
            this.groupBoxDevSecurity.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableBootModeWarning.SuspendLayout();
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
            this.radioButtonBIOSBoot.CheckedChanged += new System.EventHandler(this.radioButtonBIOSBoot_CheckedChanged);
            // 
            // radioButtonUEFIBoot
            // 
            resources.ApplyResources(this.radioButtonUEFIBoot, "radioButtonUEFIBoot");
            this.radioButtonUEFIBoot.Name = "radioButtonUEFIBoot";
            this.radioButtonUEFIBoot.UseVisualStyleBackColor = true;
            // 
            // tableVtpmWarning
            // 
            resources.ApplyResources(this.tableVtpmWarning, "tableVtpmWarning");
            this.tableVtpmWarning.Controls.Add(this.labelTpm, 1, 0);
            this.tableVtpmWarning.Controls.Add(this.imgTpm, 0, 0);
            this.tableVtpmWarning.Name = "tableVtpmWarning";
            // 
            // labelTpm
            // 
            resources.ApplyResources(this.labelTpm, "labelTpm");
            this.labelTpm.Name = "labelTpm";
            // 
            // imgTpm
            // 
            this.imgTpm.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.imgTpm, "imgTpm");
            this.imgTpm.Name = "imgTpm";
            this.imgTpm.TabStop = false;
            // 
            // labelUnsupported
            // 
            resources.ApplyResources(this.labelUnsupported, "labelUnsupported");
            this.labelUnsupported.Name = "labelUnsupported";
            // 
            // imgUnsupported
            // 
            resources.ApplyResources(this.imgUnsupported, "imgUnsupported");
            this.imgUnsupported.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.imgUnsupported.Name = "imgUnsupported";
            this.imgUnsupported.TabStop = false;
            // 
            // groupBoxDevSecurity
            // 
            this.groupBoxDevSecurity.Controls.Add(this.checkBoxVtpm);
            resources.ApplyResources(this.groupBoxDevSecurity, "groupBoxDevSecurity");
            this.groupBoxDevSecurity.Name = "groupBoxDevSecurity";
            this.groupBoxDevSecurity.TabStop = false;
            // 
            // checkBoxVtpm
            // 
            resources.ApplyResources(this.checkBoxVtpm, "checkBoxVtpm");
            this.checkBoxVtpm.Name = "checkBoxVtpm";
            this.checkBoxVtpm.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.groupBoxBootMode, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxDevSecurity, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableVtpmWarning, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tableBootModeWarning, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableBootModeWarning
            // 
            resources.ApplyResources(this.tableBootModeWarning, "tableBootModeWarning");
            this.tableBootModeWarning.Controls.Add(this.imgUnsupported, 0, 0);
            this.tableBootModeWarning.Controls.Add(this.labelUnsupported, 1, 0);
            this.tableBootModeWarning.Name = "tableBootModeWarning";
            // 
            // BootModesControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "BootModesControl";
            this.groupBoxBootMode.ResumeLayout(false);
            this.tableLayoutPanelBootMode.ResumeLayout(false);
            this.tableLayoutPanelBootMode.PerformLayout();
            this.tableVtpmWarning.ResumeLayout(false);
            this.tableVtpmWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgTpm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgUnsupported)).EndInit();
            this.groupBoxDevSecurity.ResumeLayout(false);
            this.groupBoxDevSecurity.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableBootModeWarning.ResumeLayout(false);
            this.tableBootModeWarning.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxBootMode;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBootMode;
        private System.Windows.Forms.RadioButton radioButtonUEFISecureBoot;
        private System.Windows.Forms.RadioButton radioButtonBIOSBoot;
        private System.Windows.Forms.RadioButton radioButtonUEFIBoot;
        private System.Windows.Forms.TableLayoutPanel tableVtpmWarning;
        private System.Windows.Forms.PictureBox imgUnsupported;
        private System.Windows.Forms.Label labelUnsupported;
        private System.Windows.Forms.GroupBox groupBoxDevSecurity;
        private System.Windows.Forms.CheckBox checkBoxVtpm;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelTpm;
        private System.Windows.Forms.PictureBox imgTpm;
        private System.Windows.Forms.TableLayoutPanel tableBootModeWarning;
    }
}
