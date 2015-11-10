using XenAdmin.Commands;

namespace XenAdmin.Dialogs
{
    partial class UpgradeVmDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;





        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpgradeVmDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.CancelButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.searchTextBox1 = new XenAdmin.Controls.SearchTextBox();
            this.tableLayoutPanelWarning = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.vmsDataGridView = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.CheckedColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.VMNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActionCoulmn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rubricLabel = new System.Windows.Forms.Label();
            this.ClearAllButton = new System.Windows.Forms.Button();
            this.UpgradeButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vmsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // CancelButton
            // 
            resources.ApplyResources(this.CancelButton, "CancelButton");
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.searchTextBox1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelWarning, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.SelectAllButton, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.CancelButton, 4, 7);
            this.tableLayoutPanel1.Controls.Add(this.vmsDataGridView, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.rubricLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ClearAllButton, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.UpgradeButton, 3, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // searchTextBox1
            // 
            resources.ApplyResources(this.searchTextBox1, "searchTextBox1");
            this.tableLayoutPanel1.SetColumnSpan(this.searchTextBox1, 4);
            this.searchTextBox1.Name = "searchTextBox1";
            this.searchTextBox1.TextChanged += new System.EventHandler(this.searchTextBox1_TextChanged);
            // 
            // tableLayoutPanelWarning
            // 
            resources.ApplyResources(this.tableLayoutPanelWarning, "tableLayoutPanelWarning");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelWarning, 5);
            this.tableLayoutPanelWarning.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanelWarning.Controls.Add(this.pictureBox2, 0, 0);
            this.tableLayoutPanelWarning.Name = "tableLayoutPanelWarning";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Name = "label2";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // SelectAllButton
            // 
            resources.ApplyResources(this.SelectAllButton, "SelectAllButton");
            this.SelectAllButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // vmsDataGridView
            // 
            this.vmsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.vmsDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.vmsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.vmsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.vmsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CheckedColumn,
            this.VMNameColumn,
            this.StatusColumn,
            this.ActionCoulmn});
            this.tableLayoutPanel1.SetColumnSpan(this.vmsDataGridView, 5);
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.vmsDataGridView.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.vmsDataGridView, "vmsDataGridView");
            this.vmsDataGridView.HideSelection = true;
            this.vmsDataGridView.Name = "vmsDataGridView";
            this.vmsDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.vmsDataGridView_CellContentClick);
            // 
            // CheckedColumn
            // 
            resources.ApplyResources(this.CheckedColumn, "CheckedColumn");
            this.CheckedColumn.Name = "CheckedColumn";
            // 
            // VMNameColumn
            // 
            this.VMNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.VMNameColumn, "VMNameColumn");
            this.VMNameColumn.Name = "VMNameColumn";
            this.VMNameColumn.ReadOnly = true;
            // 
            // StatusColumn
            // 
            this.StatusColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.StatusColumn, "StatusColumn");
            this.StatusColumn.Name = "StatusColumn";
            this.StatusColumn.ReadOnly = true;
            this.StatusColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ActionCoulmn
            // 
            resources.ApplyResources(this.ActionCoulmn, "ActionCoulmn");
            this.ActionCoulmn.Name = "ActionCoulmn";
            this.ActionCoulmn.ReadOnly = true;
            // 
            // rubricLabel
            // 
            resources.ApplyResources(this.rubricLabel, "rubricLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.rubricLabel, 4);
            this.rubricLabel.Name = "rubricLabel";
            // 
            // ClearAllButton
            // 
            resources.ApplyResources(this.ClearAllButton, "ClearAllButton");
            this.ClearAllButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ClearAllButton.Name = "ClearAllButton";
            this.ClearAllButton.UseVisualStyleBackColor = true;
            this.ClearAllButton.Click += new System.EventHandler(this.ClearAllButton_Click);
            // 
            // UpgradeButton
            // 
            resources.ApplyResources(this.UpgradeButton, "UpgradeButton");
            this.UpgradeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.UpgradeButton.Name = "UpgradeButton";
            this.UpgradeButton.UseVisualStyleBackColor = true;
            this.UpgradeButton.Click += new System.EventHandler(this.UpgradeButton_Click);
            // 
            // UpgradeVmDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "UpgradeVmDialog";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.UpgradeVmDialog_FormClosed);
            this.Load += new System.EventHandler(this.UpgradeVmDialog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelWarning.ResumeLayout(false);
            this.tableLayoutPanelWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vmsDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label rubricLabel;
        private System.Windows.Forms.Button SelectAllButton;
        private Controls.DataGridViewEx.DataGridViewEx vmsDataGridView;
        private System.Windows.Forms.Button ClearAllButton;
        private System.Windows.Forms.Button UpgradeButton;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CheckedColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VMNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActionCoulmn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelWarning;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox2;
        protected Controls.SearchTextBox searchTextBox1;
    }
}

