namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    partial class NetWChinDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetWChinDetails));
            this.cbxAutomatic = new System.Windows.Forms.CheckBox();
            this.lblNicHelp = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboInterfaces = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelMTUWarning = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelMTUWarning = new System.Windows.Forms.Label();
            this.labelMTU = new System.Windows.Forms.Label();
            this.numericUpDownMTU = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelMTUWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMTU)).BeginInit();
            this.SuspendLayout();
            // 
            // cbxAutomatic
            // 
            resources.ApplyResources(this.cbxAutomatic, "cbxAutomatic");
            this.tableLayoutPanel1.SetColumnSpan(this.cbxAutomatic, 2);
            this.cbxAutomatic.Name = "cbxAutomatic";
            this.cbxAutomatic.UseVisualStyleBackColor = true;
            // 
            // lblNicHelp
            // 
            resources.ApplyResources(this.lblNicHelp, "lblNicHelp");
            this.tableLayoutPanel1.SetColumnSpan(this.lblNicHelp, 2);
            this.lblNicHelp.Name = "lblNicHelp";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // comboInterfaces
            // 
            resources.ApplyResources(this.comboInterfaces, "comboInterfaces");
            this.comboInterfaces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInterfaces.FormattingEnabled = true;
            this.comboInterfaces.Name = "comboInterfaces";
            this.comboInterfaces.Sorted = true;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.lblNicHelp, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboInterfaces, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbxAutomatic, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelMTUWarning, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelMTU, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownMTU, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanelMTUWarning
            // 
            resources.ApplyResources(this.tableLayoutPanelMTUWarning, "tableLayoutPanelMTUWarning");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelMTUWarning, 2);
            this.tableLayoutPanelMTUWarning.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanelMTUWarning.Controls.Add(this.labelMTUWarning, 1, 0);
            this.tableLayoutPanelMTUWarning.MinimumSize = new System.Drawing.Size(0, 40);
            this.tableLayoutPanelMTUWarning.Name = "tableLayoutPanelMTUWarning";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // labelMTUWarning
            // 
            resources.ApplyResources(this.labelMTUWarning, "labelMTUWarning");
            this.labelMTUWarning.Name = "labelMTUWarning";
            // 
            // labelMTU
            // 
            resources.ApplyResources(this.labelMTU, "labelMTU");
            this.labelMTU.Name = "labelMTU";
            // 
            // numericUpDownMTU
            // 
            resources.ApplyResources(this.numericUpDownMTU, "numericUpDownMTU");
            this.numericUpDownMTU.Name = "numericUpDownMTU";
            this.numericUpDownMTU.ValueChanged += new System.EventHandler(this.numericUpDownMTU_ValueChanged);
            // 
            // NetWChinDetails
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NetWChinDetails";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelMTUWarning.ResumeLayout(false);
            this.tableLayoutPanelMTUWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMTU)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cbxAutomatic;
        private System.Windows.Forms.Label lblNicHelp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboInterfaces;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelMTUWarning;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMTUWarning;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelMTU;
        private System.Windows.Forms.NumericUpDown numericUpDownMTU;
    }
}
