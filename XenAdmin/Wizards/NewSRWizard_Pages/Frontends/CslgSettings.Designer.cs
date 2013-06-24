namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class CslgSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CslgSettings));
            this.labelStoragePool = new System.Windows.Forms.Label();
            this.labelRaidType = new System.Windows.Forms.Label();
            this.labelProvisioningType = new System.Windows.Forms.Label();
            this.labelProvisioningOptions = new System.Windows.Forms.Label();
            this.labelProtocol = new System.Windows.Forms.Label();
            this.comboBoxStoragePool = new System.Windows.Forms.ComboBox();
            this.comboBoxRaidType = new System.Windows.Forms.ComboBox();
            this.comboBoxProvisioningType = new System.Windows.Forms.ComboBox();
            this.comboBoxProvisionOptions = new System.Windows.Forms.ComboBox();
            this.comboBoxProtocol = new System.Windows.Forms.ComboBox();
            this.checkBoxShowAll = new System.Windows.Forms.CheckBox();
            this.checkBoxUseChap = new System.Windows.Forms.CheckBox();
            this.simpleProgressBar1 = new XenAdmin.Controls.SimpleProgressBar();
            this.textBoxChapUser = new System.Windows.Forms.TextBox();
            this.labelChapUser = new System.Windows.Forms.Label();
            this.textBoxChapSecret = new System.Windows.Forms.TextBox();
            this.labelTopDivider = new System.Windows.Forms.Label();
            this.labelChapSecret = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.labelProgress = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelStoragePool
            // 
            resources.ApplyResources(this.labelStoragePool, "labelStoragePool");
            this.labelStoragePool.Name = "labelStoragePool";
            // 
            // labelRaidType
            // 
            resources.ApplyResources(this.labelRaidType, "labelRaidType");
            this.labelRaidType.Name = "labelRaidType";
            // 
            // labelProvisioningType
            // 
            resources.ApplyResources(this.labelProvisioningType, "labelProvisioningType");
            this.labelProvisioningType.Name = "labelProvisioningType";
            // 
            // labelProvisioningOptions
            // 
            resources.ApplyResources(this.labelProvisioningOptions, "labelProvisioningOptions");
            this.labelProvisioningOptions.Name = "labelProvisioningOptions";
            // 
            // labelProtocol
            // 
            resources.ApplyResources(this.labelProtocol, "labelProtocol");
            this.labelProtocol.Name = "labelProtocol";
            // 
            // comboBoxStoragePool
            // 
            resources.ApplyResources(this.comboBoxStoragePool, "comboBoxStoragePool");
            this.comboBoxStoragePool.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBoxStoragePool.DropDownHeight = 206;
            this.comboBoxStoragePool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStoragePool.DropDownWidth = 400;
            this.comboBoxStoragePool.FormattingEnabled = true;
            this.comboBoxStoragePool.Name = "comboBoxStoragePool";
            this.comboBoxStoragePool.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBoxStoragePool_DrawItem);
            this.comboBoxStoragePool.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.comboBoxStoragePool_MeasureItem);
            this.comboBoxStoragePool.SelectedIndexChanged += new System.EventHandler(this.comboBoxStoragePools_SelectedIndexChanged);
            this.comboBoxStoragePool.SizeChanged += new System.EventHandler(this.comboBoxStoragePool_SizeChanged);
            // 
            // comboBoxRaidType
            // 
            resources.ApplyResources(this.comboBoxRaidType, "comboBoxRaidType");
            this.comboBoxRaidType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxRaidType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRaidType.FormattingEnabled = true;
            this.comboBoxRaidType.Name = "comboBoxRaidType";
            this.comboBoxRaidType.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
            // 
            // comboBoxProvisioningType
            // 
            resources.ApplyResources(this.comboBoxProvisioningType, "comboBoxProvisioningType");
            this.comboBoxProvisioningType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxProvisioningType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProvisioningType.FormattingEnabled = true;
            this.comboBoxProvisioningType.Name = "comboBoxProvisioningType";
            this.comboBoxProvisioningType.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
            // 
            // comboBoxProvisionOptions
            // 
            resources.ApplyResources(this.comboBoxProvisionOptions, "comboBoxProvisionOptions");
            this.comboBoxProvisionOptions.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxProvisionOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProvisionOptions.FormattingEnabled = true;
            this.comboBoxProvisionOptions.Name = "comboBoxProvisionOptions";
            this.comboBoxProvisionOptions.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
            // 
            // comboBoxProtocol
            // 
            resources.ApplyResources(this.comboBoxProtocol, "comboBoxProtocol");
            this.comboBoxProtocol.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProtocol.FormattingEnabled = true;
            this.comboBoxProtocol.Name = "comboBoxProtocol";
            this.comboBoxProtocol.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
            // 
            // checkBoxShowAll
            // 
            resources.ApplyResources(this.checkBoxShowAll, "checkBoxShowAll");
            this.checkBoxShowAll.Name = "checkBoxShowAll";
            this.checkBoxShowAll.UseVisualStyleBackColor = true;
            this.checkBoxShowAll.CheckedChanged += new System.EventHandler(this.checkBoxShowAll_CheckedChanged);
            // 
            // checkBoxUseChap
            // 
            resources.ApplyResources(this.checkBoxUseChap, "checkBoxUseChap");
            this.checkBoxUseChap.ForeColor = System.Drawing.SystemColors.InfoText;
            this.checkBoxUseChap.Name = "checkBoxUseChap";
            this.checkBoxUseChap.UseVisualStyleBackColor = true;
            this.checkBoxUseChap.CheckedChanged += new System.EventHandler(this.checkBoxUseChap_CheckedChanged);
            // 
            // simpleProgressBar1
            // 
            this.simpleProgressBar1.Color = XenAdmin.Core.Drawing.SimpleProgressBarColor.Blue;
            resources.ApplyResources(this.simpleProgressBar1, "simpleProgressBar1");
            this.simpleProgressBar1.Name = "simpleProgressBar1";
            this.simpleProgressBar1.Progress = 0.7;
            this.simpleProgressBar1.TabStop = false;
            // 
            // textBoxChapUser
            // 
            this.textBoxChapUser.AllowDrop = true;
            resources.ApplyResources(this.textBoxChapUser, "textBoxChapUser");
            this.textBoxChapUser.Name = "textBoxChapUser";
            // 
            // labelChapUser
            // 
            resources.ApplyResources(this.labelChapUser, "labelChapUser");
            this.labelChapUser.BackColor = System.Drawing.Color.Transparent;
            this.labelChapUser.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelChapUser.Name = "labelChapUser";
            // 
            // textBoxChapSecret
            // 
            resources.ApplyResources(this.textBoxChapSecret, "textBoxChapSecret");
            this.textBoxChapSecret.Name = "textBoxChapSecret";
            this.textBoxChapSecret.UseSystemPasswordChar = true;
            // 
            // labelTopDivider
            // 
            this.labelTopDivider.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLayoutPanel1.SetColumnSpan(this.labelTopDivider, 3);
            resources.ApplyResources(this.labelTopDivider, "labelTopDivider");
            this.labelTopDivider.Name = "labelTopDivider";
            // 
            // labelChapSecret
            // 
            resources.ApplyResources(this.labelChapSecret, "labelChapSecret");
            this.labelChapSecret.BackColor = System.Drawing.Color.Transparent;
            this.labelChapSecret.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelChapSecret.Name = "labelChapSecret";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.autoHeightLabel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelStoragePool, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxStoragePool, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxShowAll, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelProgress, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.simpleProgressBar1, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelTopDivider, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelRaidType, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxRaidType, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelProvisioningType, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxProvisioningType, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelProvisioningOptions, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxProvisionOptions, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelProtocol, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxProtocol, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxUseChap, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.labelChapUser, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.textBoxChapUser, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.labelChapSecret, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.textBoxChapSecret, 1, 11);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.tableLayoutPanel1.SetColumnSpan(this.autoHeightLabel1, 3);
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // labelProgress
            // 
            resources.ApplyResources(this.labelProgress, "labelProgress");
            this.labelProgress.Name = "labelProgress";
            // 
            // CslgSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CslgSettings";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelStoragePool;
        private System.Windows.Forms.Label labelRaidType;
        private System.Windows.Forms.Label labelProvisioningType;
        private System.Windows.Forms.Label labelProvisioningOptions;
        private System.Windows.Forms.Label labelProtocol;
        private System.Windows.Forms.ComboBox comboBoxStoragePool;
        private System.Windows.Forms.ComboBox comboBoxRaidType;
        private System.Windows.Forms.ComboBox comboBoxProvisioningType;
        private System.Windows.Forms.ComboBox comboBoxProvisionOptions;
        private System.Windows.Forms.ComboBox comboBoxProtocol;
        private System.Windows.Forms.CheckBox checkBoxShowAll;
        private System.Windows.Forms.CheckBox checkBoxUseChap;
        private XenAdmin.Controls.SimpleProgressBar simpleProgressBar1;
        private System.Windows.Forms.TextBox textBoxChapUser;
        private System.Windows.Forms.Label labelChapUser;
        private System.Windows.Forms.TextBox textBoxChapSecret;
        private System.Windows.Forms.Label labelTopDivider;
        private System.Windows.Forms.Label labelChapSecret;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
        private System.Windows.Forms.Label labelProgress;

    }
}
