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
            this._tlpWarning = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.labelWarning = new System.Windows.Forms.Label();
            this.m_textBoxApplianceName = new System.Windows.Forms.TextBox();
            this.m_textBoxFolderName = new System.Windows.Forms.TextBox();
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.groupBoxFormat = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this._tlpInfoOvf = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.labelOvf = new System.Windows.Forms.Label();
            this.radioButtonXva = new System.Windows.Forms.RadioButton();
            this._tlpInfoXva = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelXva = new System.Windows.Forms.Label();
            this.radioButtonOvf = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1.SuspendLayout();
            this._tlpWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBoxFormat.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this._tlpInfoOvf.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this._tlpInfoXva.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.tableLayoutPanel1.Controls.Add(this._tlpWarning, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.m_labelIntro, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_labelApplianceName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_textBoxApplianceName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_labelSelect, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_textBoxFolderName, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_buttonBrowse, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_ctrlError, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxFormat, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // _tlpWarning
            // 
            resources.ApplyResources(this._tlpWarning, "_tlpWarning");
            this.tableLayoutPanel1.SetColumnSpan(this._tlpWarning, 3);
            this._tlpWarning.Controls.Add(this.pictureBox2, 0, 0);
            this._tlpWarning.Controls.Add(this.labelWarning, 1, 0);
            this._tlpWarning.Name = "_tlpWarning";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.labelWarning.Name = "labelWarning";
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
            // m_ctrlError
            // 
            resources.ApplyResources(this.m_ctrlError, "m_ctrlError");
            this.tableLayoutPanel1.SetColumnSpan(this.m_ctrlError, 3);
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // groupBoxFormat
            // 
            resources.ApplyResources(this.groupBoxFormat, "groupBoxFormat");
            this.tableLayoutPanel1.SetColumnSpan(this.groupBoxFormat, 3);
            this.groupBoxFormat.Controls.Add(this.tableLayoutPanel2);
            this.groupBoxFormat.Name = "groupBoxFormat";
            this.groupBoxFormat.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this._tlpInfoOvf, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.radioButtonXva, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this._tlpInfoXva, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.radioButtonOvf, 0, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // _tlpInfoOvf
            // 
            resources.ApplyResources(this._tlpInfoOvf, "_tlpInfoOvf");
            this._tlpInfoOvf.Controls.Add(this.pictureBox3, 0, 0);
            this._tlpInfoOvf.Controls.Add(this.labelOvf, 1, 0);
            this._tlpInfoOvf.Name = "_tlpInfoOvf";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.pictureBox3, "pictureBox3");
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.TabStop = false;
            // 
            // labelOvf
            // 
            resources.ApplyResources(this.labelOvf, "labelOvf");
            this.labelOvf.Name = "labelOvf";
            // 
            // radioButtonXva
            // 
            resources.ApplyResources(this.radioButtonXva, "radioButtonXva");
            this.radioButtonXva.Name = "radioButtonXva";
            this.radioButtonXva.TabStop = true;
            this.radioButtonXva.UseVisualStyleBackColor = true;
            this.radioButtonXva.CheckedChanged += new System.EventHandler(this.radioButtonXva_CheckedChanged);
            // 
            // _tlpInfoXva
            // 
            resources.ApplyResources(this._tlpInfoXva, "_tlpInfoXva");
            this._tlpInfoXva.Controls.Add(this.pictureBox1, 0, 0);
            this._tlpInfoXva.Controls.Add(this.labelXva, 1, 0);
            this._tlpInfoXva.Name = "_tlpInfoXva";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // labelXva
            // 
            resources.ApplyResources(this.labelXva, "labelXva");
            this.labelXva.Name = "labelXva";
            // 
            // radioButtonOvf
            // 
            resources.ApplyResources(this.radioButtonOvf, "radioButtonOvf");
            this.radioButtonOvf.Name = "radioButtonOvf";
            this.radioButtonOvf.TabStop = true;
            this.radioButtonOvf.UseVisualStyleBackColor = true;
            this.radioButtonOvf.CheckedChanged += new System.EventHandler(this.radioButtonOvf_CheckedChanged);
            // 
            // ExportAppliancePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ExportAppliancePage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this._tlpWarning.ResumeLayout(false);
            this._tlpWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBoxFormat.ResumeLayout(false);
            this.groupBoxFormat.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this._tlpInfoOvf.ResumeLayout(false);
            this._tlpInfoOvf.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this._tlpInfoXva.ResumeLayout(false);
            this._tlpInfoXva.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private XenAdmin.Controls.Common.AutoHeightLabel m_labelIntro;
		private System.Windows.Forms.Label m_labelSelect;
		private System.Windows.Forms.Label m_labelApplianceName;
		private System.Windows.Forms.Button m_buttonBrowse;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlError;
		private System.Windows.Forms.TextBox m_textBoxApplianceName;
		private System.Windows.Forms.TextBox m_textBoxFolderName;
        private System.Windows.Forms.TableLayoutPanel _tlpInfoXva;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel _tlpWarning;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.RadioButton radioButtonXva;
        private System.Windows.Forms.RadioButton radioButtonOvf;
        private System.Windows.Forms.GroupBox groupBoxFormat;
        private System.Windows.Forms.Label labelXva;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel _tlpInfoOvf;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label labelOvf;
    }
}
