namespace XenAdmin.Wizards.NewSRWizard_Pages
{
    partial class ChooseSrTypePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseSrTypePage));
            this.radioButtonNfs = new System.Windows.Forms.RadioButton();
            this.radioButtonIscsi = new System.Windows.Forms.RadioButton();
            this.radioButtonNfsIso = new System.Windows.Forms.RadioButton();
            this.radioButtonCifsIso = new System.Windows.Forms.RadioButton();
            this.radioButtonFibreChannel = new System.Windows.Forms.RadioButton();
            this.radioButtonCslg = new System.Windows.Forms.RadioButton();
            this.labelISOlibrary = new System.Windows.Forms.Label();
            this.labelVirtualDiskStorage = new System.Windows.Forms.Label();
            this.upsellPage1 = new XenAdmin.Controls.UpsellPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.SRBlurb = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.deprecationBanner = new XenAdmin.Controls.DeprecationBanner();
            this.selectedStoreTypeLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonFcoe = new System.Windows.Forms.RadioButton();
            this.radioButtonCifs = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonNfs
            // 
            resources.ApplyResources(this.radioButtonNfs, "radioButtonNfs");
            this.radioButtonNfs.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonNfs.ForeColor = System.Drawing.SystemColors.WindowText;
            this.radioButtonNfs.Name = "radioButtonNfs";
            this.radioButtonNfs.UseVisualStyleBackColor = false;
            this.radioButtonNfs.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonIscsi
            // 
            resources.ApplyResources(this.radioButtonIscsi, "radioButtonIscsi");
            this.radioButtonIscsi.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonIscsi.ForeColor = System.Drawing.SystemColors.WindowText;
            this.radioButtonIscsi.Name = "radioButtonIscsi";
            this.radioButtonIscsi.UseVisualStyleBackColor = false;
            this.radioButtonIscsi.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonNfsIso
            // 
            resources.ApplyResources(this.radioButtonNfsIso, "radioButtonNfsIso");
            this.radioButtonNfsIso.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonNfsIso.ForeColor = System.Drawing.SystemColors.WindowText;
            this.radioButtonNfsIso.Name = "radioButtonNfsIso";
            this.radioButtonNfsIso.UseVisualStyleBackColor = false;
            this.radioButtonNfsIso.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonCifsIso
            // 
            resources.ApplyResources(this.radioButtonCifsIso, "radioButtonCifsIso");
            this.radioButtonCifsIso.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonCifsIso.ForeColor = System.Drawing.SystemColors.WindowText;
            this.radioButtonCifsIso.Name = "radioButtonCifsIso";
            this.radioButtonCifsIso.UseVisualStyleBackColor = false;
            this.radioButtonCifsIso.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonFibreChannel
            // 
            resources.ApplyResources(this.radioButtonFibreChannel, "radioButtonFibreChannel");
            this.radioButtonFibreChannel.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonFibreChannel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.radioButtonFibreChannel.Name = "radioButtonFibreChannel";
            this.radioButtonFibreChannel.UseVisualStyleBackColor = false;
            this.radioButtonFibreChannel.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonCslg
            // 
            resources.ApplyResources(this.radioButtonCslg, "radioButtonCslg");
            this.radioButtonCslg.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonCslg.ForeColor = System.Drawing.SystemColors.WindowText;
            this.radioButtonCslg.Name = "radioButtonCslg";
            this.radioButtonCslg.UseVisualStyleBackColor = false;
            this.radioButtonCslg.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // labelISOlibrary
            // 
            resources.ApplyResources(this.labelISOlibrary, "labelISOlibrary");
            this.labelISOlibrary.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelISOlibrary.Name = "labelISOlibrary";
            // 
            // labelVirtualDiskStorage
            // 
            resources.ApplyResources(this.labelVirtualDiskStorage, "labelVirtualDiskStorage");
            this.labelVirtualDiskStorage.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelVirtualDiskStorage.Name = "labelVirtualDiskStorage";
            // 
            // upsellPage1
            // 
            resources.ApplyResources(this.upsellPage1, "upsellPage1");
            this.upsellPage1.Image = ((System.Drawing.Image)(resources.GetObject("upsellPage1.Image")));
            this.upsellPage1.Name = "upsellPage1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.upsellPage1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.SRBlurb, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.deprecationBanner, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.selectedStoreTypeLabel, 0, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // SRBlurb
            // 
            this.SRBlurb.AutoEllipsis = true;
            resources.ApplyResources(this.SRBlurb, "SRBlurb");
            this.SRBlurb.Name = "SRBlurb";
            // 
            // deprecationBanner
            // 
            resources.ApplyResources(this.deprecationBanner, "deprecationBanner");
            this.deprecationBanner.BackColor = System.Drawing.Color.LightCoral;
            this.deprecationBanner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deprecationBanner.Name = "deprecationBanner";
            // 
            // selectedStoreTypeLabel
            // 
            resources.ApplyResources(this.selectedStoreTypeLabel, "selectedStoreTypeLabel");
            this.selectedStoreTypeLabel.Name = "selectedStoreTypeLabel";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.radioButtonFcoe, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.radioButtonCifs, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.radioButtonCslg, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this.labelVirtualDiskStorage, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.radioButtonNfs, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.radioButtonFibreChannel, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.radioButtonIscsi, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.labelISOlibrary, 0, 7);
            this.tableLayoutPanel3.Controls.Add(this.radioButtonCifsIso, 0, 8);
            this.tableLayoutPanel3.Controls.Add(this.radioButtonNfsIso, 0, 9);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // radioButtonFcoe
            // 
            resources.ApplyResources(this.radioButtonFcoe, "radioButtonFcoe");
            this.radioButtonFcoe.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonFcoe.ForeColor = System.Drawing.SystemColors.WindowText;
            this.radioButtonFcoe.Name = "radioButtonFcoe";
            this.radioButtonFcoe.UseVisualStyleBackColor = false;
            this.radioButtonFcoe.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonCifs
            // 
            resources.ApplyResources(this.radioButtonCifs, "radioButtonCifs");
            this.radioButtonCifs.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonCifs.ForeColor = System.Drawing.SystemColors.WindowText;
            this.radioButtonCifs.Name = "radioButtonCifs";
            this.radioButtonCifs.UseVisualStyleBackColor = false;
            this.radioButtonCifs.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // ChooseSrTypePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ChooseSrTypePage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonNfs;
        private System.Windows.Forms.RadioButton radioButtonIscsi;
        private System.Windows.Forms.RadioButton radioButtonNfsIso;
        private System.Windows.Forms.RadioButton radioButtonCifsIso;
        private System.Windows.Forms.RadioButton radioButtonFibreChannel;
        private System.Windows.Forms.RadioButton radioButtonCslg;
        private System.Windows.Forms.Label labelISOlibrary;
        private System.Windows.Forms.Label labelVirtualDiskStorage;
        private XenAdmin.Controls.UpsellPage upsellPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private XenAdmin.Controls.Common.AutoHeightLabel SRBlurb;
        private XenAdmin.Controls.DeprecationBanner deprecationBanner;
        private System.Windows.Forms.Label selectedStoreTypeLabel;
        private System.Windows.Forms.RadioButton radioButtonCifs;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.RadioButton radioButtonFcoe;
    }
}
