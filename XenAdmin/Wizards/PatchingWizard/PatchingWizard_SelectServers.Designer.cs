using System.Drawing;
using System.Windows.Forms;
using XenAdmin;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard
{
    partial class PatchingWizard_SelectServers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatchingWizard_SelectServers));
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.dataGridViewHosts = new XenAdmin.Wizards.PatchingWizard.PatchingWizard_SelectServers.PatchingHostsDataGridView();
            this.ColumnPoolCheckBox = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnExpander = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnPoolIconHostCheck = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHosts)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonSelectAll
            // 
            resources.ApplyResources(this.buttonSelectAll, "buttonSelectAll");
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
            // 
            // buttonClearAll
            // 
            resources.ApplyResources(this.buttonClearAll, "buttonClearAll");
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.UseVisualStyleBackColor = true;
            this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
            // 
            // dataGridViewHosts
            // 
            resources.ApplyResources(this.dataGridViewHosts, "dataGridViewHosts");
            this.dataGridViewHosts.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewHosts.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewHosts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewHosts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnPoolCheckBox,
            this.ColumnExpander,
            this.ColumnPoolIconHostCheck,
            this.ColumnName,
            this.ColumnVersion});
            this.dataGridViewHosts.Name = "dataGridViewHosts";
            this.dataGridViewHosts.Updating = false;
            // 
            // ColumnPoolCheckBox
            // 
            this.ColumnPoolCheckBox.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnPoolCheckBox.FillWeight = 48.73096F;
            resources.ApplyResources(this.ColumnPoolCheckBox, "ColumnPoolCheckBox");
            this.ColumnPoolCheckBox.Name = "ColumnPoolCheckBox";
            // 
            // ColumnExpander
            // 
            this.ColumnExpander.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnExpander.FillWeight = 110.2538F;
            resources.ApplyResources(this.ColumnExpander, "ColumnExpander");
            this.ColumnExpander.Name = "ColumnExpander";
            // 
            // ColumnPoolIconHostCheck
            // 
            this.ColumnPoolIconHostCheck.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnPoolIconHostCheck.FillWeight = 110.2538F;
            resources.ApplyResources(this.ColumnPoolIconHostCheck, "ColumnPoolIconHostCheck");
            this.ColumnPoolIconHostCheck.Name = "ColumnPoolIconHostCheck";
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnName.FillWeight = 110.2538F;
            resources.ApplyResources(this.ColumnName, "ColumnName");
            this.ColumnName.Name = "ColumnName";
            // 
            // ColumnVersion
            // 
            this.ColumnVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnVersion.FillWeight = 110.2538F;
            resources.ApplyResources(this.ColumnVersion, "ColumnVersion");
            this.ColumnVersion.Name = "ColumnVersion";
            // 
            // PatchingWizard_SelectServers
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.dataGridViewHosts);
            this.Controls.Add(this.buttonClearAll);
            this.Controls.Add(this.buttonSelectAll);
            this.Controls.Add(this.label1);
            this.Name = "PatchingWizard_SelectServers";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHosts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Label label1;
        private Button buttonSelectAll;
        private Button buttonClearAll;
        private XenAdmin.Wizards.PatchingWizard.PatchingWizard_SelectServers.PatchingHostsDataGridView dataGridViewHosts;
        private DataGridViewCheckBoxColumn ColumnPoolCheckBox;
        private DataGridViewImageColumn ColumnExpander;
        private DataGridViewImageColumn ColumnPoolIconHostCheck;
        private DataGridViewTextBoxColumn ColumnName;
        private DataGridViewTextBoxColumn ColumnVersion;
    }
}
