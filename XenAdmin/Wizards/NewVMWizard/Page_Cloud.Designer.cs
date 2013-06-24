using XenAdmin.Controls.DataGridViewEx;
namespace XenAdmin.Wizards.NewVMWizard
{
    partial class Page_Cloud
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_Cloud));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridViewInstanceType = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxKernel = new System.Windows.Forms.ComboBox();
            this.comboBoxRAMdisk = new System.Windows.Forms.ComboBox();
            this.comboBoxKeyPair = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labelSecurityGroup = new System.Windows.Forms.Label();
            this.checkedListBoxSecurityGroups = new System.Windows.Forms.CheckedListBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInstanceType)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // dataGridViewInstanceType
            // 
            this.dataGridViewInstanceType.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewInstanceType.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewInstanceType.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewInstanceType.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.Column3,
            this.Column4});
            resources.ApplyResources(this.dataGridViewInstanceType, "dataGridViewInstanceType");
            this.dataGridViewInstanceType.Name = "dataGridViewInstanceType";
            // 
            // dataGridViewTextBoxColumn1
            // 
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // Column3
            // 
            resources.ApplyResources(this.Column3, "Column3");
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            resources.ApplyResources(this.Column4, "Column4");
            this.Column4.Name = "Column4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // comboBoxKernel
            // 
            this.comboBoxKernel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKernel.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxKernel, "comboBoxKernel");
            this.comboBoxKernel.Name = "comboBoxKernel";
            // 
            // comboBoxRAMdisk
            // 
            this.comboBoxRAMdisk.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRAMdisk.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxRAMdisk, "comboBoxRAMdisk");
            this.comboBoxRAMdisk.Name = "comboBoxRAMdisk";
            // 
            // comboBoxKeyPair
            // 
            this.comboBoxKeyPair.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKeyPair.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxKeyPair, "comboBoxKeyPair");
            this.comboBoxKeyPair.Name = "comboBoxKeyPair";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // labelSecurityGroup
            // 
            resources.ApplyResources(this.labelSecurityGroup, "labelSecurityGroup");
            this.labelSecurityGroup.Name = "labelSecurityGroup";
            // 
            // checkedListBoxSecurityGroups
            // 
            resources.ApplyResources(this.checkedListBoxSecurityGroups, "checkedListBoxSecurityGroups");
            this.checkedListBoxSecurityGroups.FormattingEnabled = true;
            this.checkedListBoxSecurityGroups.Name = "checkedListBoxSecurityGroups";
            // 
            // Page_Cloud
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.checkedListBoxSecurityGroups);
            this.Controls.Add(this.labelSecurityGroup);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBoxKeyPair);
            this.Controls.Add(this.comboBoxRAMdisk);
            this.Controls.Add(this.comboBoxKernel);
            this.Controls.Add(this.dataGridViewInstanceType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Page_Cloud";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInstanceType)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DataGridViewEx dataGridViewInstanceType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxKernel;
        private System.Windows.Forms.ComboBox comboBoxRAMdisk;
        private System.Windows.Forms.ComboBox comboBoxKeyPair;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelSecurityGroup;
        private System.Windows.Forms.CheckedListBox checkedListBoxSecurityGroups;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
    }
}
