namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class EqualLogic
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EqualLogic));
            this.ThinProvisioningCheckBox = new System.Windows.Forms.CheckBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colPool = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFreeSpace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVolumes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMembers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.listBoxSRs = new XenAdmin.Controls.SRListBox();
            this.radioButtonNew = new System.Windows.Forms.RadioButton();
            this.radioButtonReattach = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ThinProvisioningCheckBox
            // 
            resources.ApplyResources(this.ThinProvisioningCheckBox, "ThinProvisioningCheckBox");
            this.ThinProvisioningCheckBox.Name = "ThinProvisioningCheckBox";
            this.ThinProvisioningCheckBox.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPool,
            this.colSize,
            this.colFreeSpace,
            this.colVolumes,
            this.colMembers});
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // colPool
            // 
            this.colPool.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colPool, "colPool");
            this.colPool.Name = "colPool";
            this.colPool.ReadOnly = true;
            // 
            // colSize
            // 
            this.colSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colSize, "colSize");
            this.colSize.Name = "colSize";
            this.colSize.ReadOnly = true;
            // 
            // colFreeSpace
            // 
            this.colFreeSpace.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colFreeSpace, "colFreeSpace");
            this.colFreeSpace.Name = "colFreeSpace";
            this.colFreeSpace.ReadOnly = true;
            // 
            // colVolumes
            // 
            this.colVolumes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colVolumes, "colVolumes");
            this.colVolumes.Name = "colVolumes";
            this.colVolumes.ReadOnly = true;
            // 
            // colMembers
            // 
            this.colMembers.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colMembers, "colMembers");
            this.colMembers.Name = "colMembers";
            this.colMembers.ReadOnly = true;
            // 
            // listBoxSRs
            // 
            resources.ApplyResources(this.listBoxSRs, "listBoxSRs");
            this.listBoxSRs.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxSRs.FormattingEnabled = true;
            this.listBoxSRs.Name = "listBoxSRs";
            this.listBoxSRs.Sorted = true;
            this.listBoxSRs.SelectedIndexChanged += new System.EventHandler(this.listBoxSRs_SelectedIndexChanged);
            // 
            // radioButtonNew
            // 
            resources.ApplyResources(this.radioButtonNew, "radioButtonNew");
            this.radioButtonNew.Name = "radioButtonNew";
            this.radioButtonNew.TabStop = true;
            this.radioButtonNew.UseVisualStyleBackColor = true;
            this.radioButtonNew.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonReattach
            // 
            resources.ApplyResources(this.radioButtonReattach, "radioButtonReattach");
            this.radioButtonReattach.Name = "radioButtonReattach";
            this.radioButtonReattach.TabStop = true;
            this.radioButtonReattach.UseVisualStyleBackColor = true;
            this.radioButtonReattach.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.radioButtonReattach, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.listBoxSRs, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonNew, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.ThinProvisioningCheckBox, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // EqualLogic
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "EqualLogic";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox ThinProvisioningCheckBox;
        private System.Windows.Forms.DataGridView dataGridView1;
        private XenAdmin.Controls.SRListBox listBoxSRs;
        private System.Windows.Forms.RadioButton radioButtonNew;
        private System.Windows.Forms.RadioButton radioButtonReattach;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPool;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFreeSpace;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVolumes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMembers;
    }
}
