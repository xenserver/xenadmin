namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    partial class NetWDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetWDetails));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelExternal = new System.Windows.Forms.Label();
            this.labelInternal = new System.Windows.Forms.Label();
            this.labelNIC = new System.Windows.Forms.Label();
            this.comboBoxNICList = new System.Windows.Forms.ComboBox();
            this.labelVLAN = new System.Windows.Forms.Label();
            this.numericUpDownVLAN = new System.Windows.Forms.NumericUpDown();
            this.infoVlanPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxVlan = new System.Windows.Forms.PictureBox();
            this.labelVlanMessage = new System.Windows.Forms.Label();
            this.labelMTU = new System.Windows.Forms.Label();
            this.numericUpDownMTU = new System.Windows.Forms.NumericUpDown();
            this.infoMtuPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxMtu = new System.Windows.Forms.PictureBox();
            this.labelMtuMessage = new System.Windows.Forms.Label();
            this.checkBoxSriov = new System.Windows.Forms.CheckBox();
            this.checkBoxAutomatic = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVLAN)).BeginInit();
            this.infoVlanPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVlan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMTU)).BeginInit();
            this.infoMtuPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMtu)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelExternal, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelInternal, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelNIC, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxNICList, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelVLAN, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownVLAN, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.infoVlanPanel, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelMTU, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownMTU, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.infoMtuPanel, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxSriov, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxAutomatic, 0, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelExternal
            // 
            resources.ApplyResources(this.labelExternal, "labelExternal");
            this.tableLayoutPanel1.SetColumnSpan(this.labelExternal, 4);
            this.labelExternal.Name = "labelExternal";
            // 
            // labelInternal
            // 
            resources.ApplyResources(this.labelInternal, "labelInternal");
            this.tableLayoutPanel1.SetColumnSpan(this.labelInternal, 4);
            this.labelInternal.Name = "labelInternal";
            // 
            // labelNIC
            // 
            resources.ApplyResources(this.labelNIC, "labelNIC");
            this.labelNIC.Name = "labelNIC";
            // 
            // comboBoxNICList
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.comboBoxNICList, 2);
            this.comboBoxNICList.DisplayMember = "Server";
            this.comboBoxNICList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNICList.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxNICList, "comboBoxNICList");
            this.comboBoxNICList.Name = "comboBoxNICList";
            this.comboBoxNICList.Sorted = true;
            this.comboBoxNICList.SelectedIndexChanged += new System.EventHandler(this.cmbHostNicList_SelectedIndexChanged);
            // 
            // labelVLAN
            // 
            resources.ApplyResources(this.labelVLAN, "labelVLAN");
            this.labelVLAN.Name = "labelVLAN";
            // 
            // numericUpDownVLAN
            // 
            resources.ApplyResources(this.numericUpDownVLAN, "numericUpDownVLAN");
            this.numericUpDownVLAN.Name = "numericUpDownVLAN";
            this.numericUpDownVLAN.ValueChanged += new System.EventHandler(this.numericUpDownVLAN_ValueChanged);
            this.numericUpDownVLAN.Leave += new System.EventHandler(this.numericUpDownVLAN_Leave);
            // 
            // infoVlanPanel
            // 
            resources.ApplyResources(this.infoVlanPanel, "infoVlanPanel");
            this.tableLayoutPanel1.SetColumnSpan(this.infoVlanPanel, 2);
            this.infoVlanPanel.Controls.Add(this.pictureBoxVlan, 0, 0);
            this.infoVlanPanel.Controls.Add(this.labelVlanMessage, 1, 0);
            this.infoVlanPanel.Name = "infoVlanPanel";
            // 
            // pictureBoxVlan
            // 
            resources.ApplyResources(this.pictureBoxVlan, "pictureBoxVlan");
            this.pictureBoxVlan.Name = "pictureBoxVlan";
            this.pictureBoxVlan.TabStop = false;
            // 
            // labelVlanMessage
            // 
            resources.ApplyResources(this.labelVlanMessage, "labelVlanMessage");
            this.labelVlanMessage.Name = "labelVlanMessage";
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
            this.numericUpDownMTU.Leave += new System.EventHandler(this.numericUpDownMTU_Leave);
            // 
            // infoMtuPanel
            // 
            resources.ApplyResources(this.infoMtuPanel, "infoMtuPanel");
            this.tableLayoutPanel1.SetColumnSpan(this.infoMtuPanel, 2);
            this.infoMtuPanel.Controls.Add(this.pictureBoxMtu, 0, 0);
            this.infoMtuPanel.Controls.Add(this.labelMtuMessage, 1, 0);
            this.infoMtuPanel.Name = "infoMtuPanel";
            // 
            // pictureBoxMtu
            // 
            resources.ApplyResources(this.pictureBoxMtu, "pictureBoxMtu");
            this.pictureBoxMtu.Name = "pictureBoxMtu";
            this.pictureBoxMtu.TabStop = false;
            // 
            // labelMtuMessage
            // 
            resources.ApplyResources(this.labelMtuMessage, "labelMtuMessage");
            this.labelMtuMessage.Name = "labelMtuMessage";
            // 
            // checkBoxSriov
            // 
            resources.ApplyResources(this.checkBoxSriov, "checkBoxSriov");
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxSriov, 4);
            this.checkBoxSriov.Name = "checkBoxSriov";
            this.checkBoxSriov.UseVisualStyleBackColor = true;
            this.checkBoxSriov.CheckedChanged += new System.EventHandler(this.checkBoxSriov_CheckedChanged);
            // 
            // checkBoxAutomatic
            // 
            resources.ApplyResources(this.checkBoxAutomatic, "checkBoxAutomatic");
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxAutomatic, 4);
            this.checkBoxAutomatic.Name = "checkBoxAutomatic";
            this.checkBoxAutomatic.UseVisualStyleBackColor = true;
            // 
            // NetWDetails
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NetWDetails";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVLAN)).EndInit();
            this.infoVlanPanel.ResumeLayout(false);
            this.infoVlanPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVlan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMTU)).EndInit();
            this.infoMtuPanel.ResumeLayout(false);
            this.infoMtuPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMtu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelNIC;
        private System.Windows.Forms.ComboBox comboBoxNICList;
        private System.Windows.Forms.Label labelVLAN;
        private System.Windows.Forms.Label labelInternal;
        private System.Windows.Forms.CheckBox checkBoxAutomatic;
        private System.Windows.Forms.Label labelMTU;
        private System.Windows.Forms.Label labelMtuMessage;
        private System.Windows.Forms.PictureBox pictureBoxMtu;
        private System.Windows.Forms.CheckBox checkBoxSriov;
        private System.Windows.Forms.Label labelExternal;
        private System.Windows.Forms.TableLayoutPanel infoMtuPanel;
        private System.Windows.Forms.TableLayoutPanel infoVlanPanel;
        private System.Windows.Forms.PictureBox pictureBoxVlan;
        private System.Windows.Forms.Label labelVlanMessage;
        private System.Windows.Forms.NumericUpDown numericUpDownVLAN;
        private System.Windows.Forms.NumericUpDown numericUpDownMTU;
    }
}
