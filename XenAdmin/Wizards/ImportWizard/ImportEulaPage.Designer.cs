namespace XenAdmin.Wizards.ImportWizard
{
    partial class ImportEulaPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportEulaPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_tabControlEULA = new System.Windows.Forms.TabControl();
            this.m_labelIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_checkBoxAccept = new System.Windows.Forms.CheckBox();
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.m_tabControlEULA, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_labelIntro, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_checkBoxAccept, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.m_ctrlError, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // m_tabControlEULA
            // 
            resources.ApplyResources(this.m_tabControlEULA, "m_tabControlEULA");
            this.m_tabControlEULA.Name = "m_tabControlEULA";
            this.m_tabControlEULA.SelectedIndex = 0;
            // 
            // m_labelIntro
            // 
            resources.ApplyResources(this.m_labelIntro, "m_labelIntro");
            this.m_labelIntro.Name = "m_labelIntro";
            // 
            // m_checkBoxAccept
            // 
            resources.ApplyResources(this.m_checkBoxAccept, "m_checkBoxAccept");
            this.m_checkBoxAccept.Name = "m_checkBoxAccept";
            this.m_checkBoxAccept.UseVisualStyleBackColor = true;
            this.m_checkBoxAccept.CheckedChanged += new System.EventHandler(this.m_checkBoxAccept_CheckedChanged);
            // 
            // m_ctrlError
            // 
            resources.ApplyResources(this.m_ctrlError, "m_ctrlError");
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // ImportEulaPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ImportEulaPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabControl m_tabControlEULA;
        private System.Windows.Forms.CheckBox m_checkBoxAccept;
		private XenAdmin.Controls.Common.AutoHeightLabel m_labelIntro;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlError;
    }
}
