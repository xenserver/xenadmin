namespace XenAdmin.Wizards.ExportWizard
{
	partial class ExportAppliancePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportAppliancePage));
            this.m_labelIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_labelApplianceName = new System.Windows.Forms.Label();
            this.m_labelSelect = new System.Windows.Forms.Label();
            this.m_buttonBrowse = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_textBoxApplianceName = new System.Windows.Forms.TextBox();
            this.m_textBoxFolderName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_comboBoxFormat = new System.Windows.Forms.ComboBox();
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_labelIntro
            // 
            resources.ApplyResources(this.m_labelIntro, "m_labelIntro");
            this.tableLayoutPanel1.SetColumnSpan(this.m_labelIntro, 3);
            this.m_labelIntro.Name = "m_labelIntro";
            // 
            // m_labelApplianceName
            // 
            resources.ApplyResources(this.m_labelApplianceName, "m_labelApplianceName");
            this.m_labelApplianceName.Name = "m_labelApplianceName";
            // 
            // m_labelSelect
            // 
            resources.ApplyResources(this.m_labelSelect, "m_labelSelect");
            this.m_labelSelect.Name = "m_labelSelect";
            // 
            // m_buttonBrowse
            // 
            resources.ApplyResources(this.m_buttonBrowse, "m_buttonBrowse");
            this.m_buttonBrowse.Name = "m_buttonBrowse";
            this.m_buttonBrowse.UseVisualStyleBackColor = true;
            this.m_buttonBrowse.Click += new System.EventHandler(this.m_buttonBrowse_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.m_labelIntro, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_labelApplianceName, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_textBoxApplianceName, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_labelSelect, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.m_textBoxFolderName, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.m_buttonBrowse, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.m_comboBoxFormat, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.m_ctrlError, 1, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // m_textBoxApplianceName
            // 
            resources.ApplyResources(this.m_textBoxApplianceName, "m_textBoxApplianceName");
            this.m_textBoxApplianceName.Name = "m_textBoxApplianceName";
            this.m_textBoxApplianceName.TextChanged += new System.EventHandler(this.m_textBoxApplianceName_TextChanged);
            // 
            // m_textBoxFolderName
            // 
            resources.ApplyResources(this.m_textBoxFolderName, "m_textBoxFolderName");
            this.m_textBoxFolderName.Name = "m_textBoxFolderName";
            this.m_textBoxFolderName.TextChanged += new System.EventHandler(this.m_textBoxFolderName_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // m_comboBoxFormat
            // 
            resources.ApplyResources(this.m_comboBoxFormat, "m_comboBoxFormat");
            this.m_comboBoxFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboBoxFormat.FormattingEnabled = true;
            this.m_comboBoxFormat.Name = "m_comboBoxFormat";
            this.m_comboBoxFormat.SelectedIndexChanged += new System.EventHandler(this.m_comboBoxFormat_SelectedIndexChanged);
            // 
            // m_ctrlError
            // 
            resources.ApplyResources(this.m_ctrlError, "m_ctrlError");
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // ExportAppliancePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ExportAppliancePage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private XenAdmin.Controls.Common.AutoHeightLabel m_labelIntro;
		private System.Windows.Forms.Label m_labelSelect;
		private System.Windows.Forms.Label m_labelApplianceName;
		private System.Windows.Forms.Button m_buttonBrowse;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlError;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox m_comboBoxFormat;
		private System.Windows.Forms.TextBox m_textBoxApplianceName;
		private System.Windows.Forms.TextBox m_textBoxFolderName;
    }
}
