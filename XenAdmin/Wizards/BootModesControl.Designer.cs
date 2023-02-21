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
            this.imgBios = new System.Windows.Forms.PictureBox();
            this.imgUefi = new System.Windows.Forms.PictureBox();
            this.imgSecureUefi = new System.Windows.Forms.PictureBox();
            this.labelBios = new System.Windows.Forms.Label();
            this.labelUefi = new System.Windows.Forms.Label();
            this.labelSecureUefi = new System.Windows.Forms.Label();
            this.warningsTable = new System.Windows.Forms.TableLayoutPanel();
            this.imgExperimental = new System.Windows.Forms.PictureBox();
            this.labelExperimental = new System.Windows.Forms.Label();
            this.groupBoxBootMode.SuspendLayout();
            this.tableLayoutPanelBootMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgBios)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgUefi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgSecureUefi)).BeginInit();
            this.warningsTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgExperimental)).BeginInit();
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
            resources.ApplyResources(this.tableLayoutPanelBootMode, "tableLayoutPanelBootMode");
            this.tableLayoutPanelBootMode.Controls.Add(this.radioButtonUEFISecureBoot, 0, 2);
            this.tableLayoutPanelBootMode.Controls.Add(this.radioButtonBIOSBoot, 0, 0);
            this.tableLayoutPanelBootMode.Controls.Add(this.radioButtonUEFIBoot, 0, 1);
            this.tableLayoutPanelBootMode.Controls.Add(this.imgBios, 1, 0);
            this.tableLayoutPanelBootMode.Controls.Add(this.imgUefi, 1, 1);
            this.tableLayoutPanelBootMode.Controls.Add(this.imgSecureUefi, 1, 2);
            this.tableLayoutPanelBootMode.Controls.Add(this.labelBios, 2, 0);
            this.tableLayoutPanelBootMode.Controls.Add(this.labelUefi, 2, 1);
            this.tableLayoutPanelBootMode.Controls.Add(this.labelSecureUefi, 2, 2);
            this.tableLayoutPanelBootMode.Name = "tableLayoutPanelBootMode";
            // 
            // radioButtonUEFISecureBoot
            // 
            resources.ApplyResources(this.radioButtonUEFISecureBoot, "radioButtonUEFISecureBoot");
            this.radioButtonUEFISecureBoot.Name = "radioButtonUEFISecureBoot";
            this.radioButtonUEFISecureBoot.UseVisualStyleBackColor = true;
            this.radioButtonUEFISecureBoot.CheckedChanged += new System.EventHandler(this.radioButtonUEFISecureBoot_CheckedChanged);
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
            // imgBios
            // 
            this.imgBios.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.imgBios, "imgBios");
            this.imgBios.Name = "imgBios";
            this.imgBios.TabStop = false;
            // 
            // imgUefi
            // 
            this.imgUefi.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.imgUefi, "imgUefi");
            this.imgUefi.Name = "imgUefi";
            this.imgUefi.TabStop = false;
            // 
            // imgSecureUefi
            // 
            this.imgSecureUefi.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.imgSecureUefi, "imgSecureUefi");
            this.imgSecureUefi.Name = "imgSecureUefi";
            this.imgSecureUefi.TabStop = false;
            // 
            // labelBios
            // 
            resources.ApplyResources(this.labelBios, "labelBios");
            this.labelBios.Name = "labelBios";
            // 
            // labelUefi
            // 
            resources.ApplyResources(this.labelUefi, "labelUefi");
            this.labelUefi.Name = "labelUefi";
            // 
            // labelSecureUefi
            // 
            resources.ApplyResources(this.labelSecureUefi, "labelSecureUefi");
            this.labelSecureUefi.Name = "labelSecureUefi";
            // 
            // warningsTable
            // 
            resources.ApplyResources(this.warningsTable, "warningsTable");
            this.warningsTable.Controls.Add(this.imgExperimental, 0, 0);
            this.warningsTable.Controls.Add(this.labelExperimental, 1, 0);
            this.warningsTable.Name = "warningsTable";
            // 
            // imgExperimental
            // 
            this.imgExperimental.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.imgExperimental, "imgExperimental");
            this.imgExperimental.Name = "imgExperimental";
            this.imgExperimental.TabStop = false;
            // 
            // labelExperimental
            // 
            resources.ApplyResources(this.labelExperimental, "labelExperimental");
            this.labelExperimental.Name = "labelExperimental";
            // 
            // BootModesControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.warningsTable);
            this.Controls.Add(this.groupBoxBootMode);
            this.DoubleBuffered = true;
            this.Name = "BootModesControl";
            this.groupBoxBootMode.ResumeLayout(false);
            this.tableLayoutPanelBootMode.ResumeLayout(false);
            this.tableLayoutPanelBootMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgBios)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgUefi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgSecureUefi)).EndInit();
            this.warningsTable.ResumeLayout(false);
            this.warningsTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgExperimental)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxBootMode;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBootMode;
        private System.Windows.Forms.RadioButton radioButtonUEFISecureBoot;
        private System.Windows.Forms.RadioButton radioButtonBIOSBoot;
        private System.Windows.Forms.RadioButton radioButtonUEFIBoot;
        private System.Windows.Forms.TableLayoutPanel warningsTable;
        private System.Windows.Forms.PictureBox imgExperimental;
        private System.Windows.Forms.Label labelExperimental;
        private System.Windows.Forms.PictureBox imgBios;
        private System.Windows.Forms.PictureBox imgUefi;
        private System.Windows.Forms.PictureBox imgSecureUefi;
        private System.Windows.Forms.Label labelBios;
        private System.Windows.Forms.Label labelUefi;
        private System.Windows.Forms.Label labelSecureUefi;
    }
}
