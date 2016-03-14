using XenAdmin.Controls.DataGridViewEx;

namespace XenAdmin.Wizards.PatchingWizard
{
    partial class PatchingWizard_SelectPatchPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatchingWizard_SelectPatchPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.automaticOptionLabel = new System.Windows.Forms.Label();
            this.AutomaticRadioButton = new System.Windows.Forms.RadioButton();
            this.RestoreDismUpdatesButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.selectFromDiskRadioButton = new System.Windows.Forms.RadioButton();
            this.downloadUpdateRadioButton = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.RefreshListButton = new System.Windows.Forms.Button();
            this.dataGridViewPatches = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnUpdate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.webPageColumn = new System.Windows.Forms.DataGridViewLinkColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPatches)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.automaticOptionLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.AutomaticRadioButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.RestoreDismUpdatesButton, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.fileNameTextBox, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.BrowseButton, 2, 7);
            this.tableLayoutPanel1.Controls.Add(this.selectFromDiskRadioButton, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.downloadUpdateRadioButton, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewPatches, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.RefreshListButton, 0, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // automaticOptionLabel
            // 
            resources.ApplyResources(this.automaticOptionLabel, "automaticOptionLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.automaticOptionLabel, 3);
            this.automaticOptionLabel.Name = "automaticOptionLabel";
            // 
            // AutomaticRadioButton
            // 
            resources.ApplyResources(this.AutomaticRadioButton, "AutomaticRadioButton");
            this.tableLayoutPanel1.SetColumnSpan(this.AutomaticRadioButton, 3);
            this.AutomaticRadioButton.Name = "AutomaticRadioButton";
            this.AutomaticRadioButton.UseVisualStyleBackColor = true;
            this.AutomaticRadioButton.CheckedChanged += new System.EventHandler(this.AutomaticRadioButton_CheckedChanged);
            // 
            // RestoreDismUpdatesButton
            // 
            resources.ApplyResources(this.RestoreDismUpdatesButton, "RestoreDismUpdatesButton");
            this.RestoreDismUpdatesButton.Name = "RestoreDismUpdatesButton";
            this.RestoreDismUpdatesButton.UseVisualStyleBackColor = true;
            this.RestoreDismUpdatesButton.Click += new System.EventHandler(this.RestoreDismUpdatesButton_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // fileNameTextBox
            // 
            resources.ApplyResources(this.fileNameTextBox, "fileNameTextBox");
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.TextChanged += new System.EventHandler(this.fileNameTextBox_TextChanged);
            this.fileNameTextBox.Enter += new System.EventHandler(this.fileNameTextBox_Enter);
            // 
            // BrowseButton
            // 
            resources.ApplyResources(this.BrowseButton, "BrowseButton");
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // selectFromDiskRadioButton
            // 
            resources.ApplyResources(this.selectFromDiskRadioButton, "selectFromDiskRadioButton");
            this.tableLayoutPanel1.SetColumnSpan(this.selectFromDiskRadioButton, 4);
            this.selectFromDiskRadioButton.Name = "selectFromDiskRadioButton";
            this.selectFromDiskRadioButton.UseVisualStyleBackColor = true;
            this.selectFromDiskRadioButton.CheckedChanged += new System.EventHandler(this.selectFromDiskRadioButton_CheckedChanged);
            // 
            // downloadUpdateRadioButton
            // 
            resources.ApplyResources(this.downloadUpdateRadioButton, "downloadUpdateRadioButton");
            this.downloadUpdateRadioButton.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.downloadUpdateRadioButton, 3);
            this.downloadUpdateRadioButton.Name = "downloadUpdateRadioButton";
            this.downloadUpdateRadioButton.TabStop = true;
            this.downloadUpdateRadioButton.UseVisualStyleBackColor = true;
            this.downloadUpdateRadioButton.CheckedChanged += new System.EventHandler(this.downloadUpdateRadioButton_CheckedChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.tableLayoutPanel1.SetColumnSpan(this.label3, 3);
            this.label3.Name = "label3";
            // 
            // RefreshListButton
            // 
            resources.ApplyResources(this.RefreshListButton, "RefreshListButton");
            this.RefreshListButton.Name = "RefreshListButton";
            this.RefreshListButton.UseVisualStyleBackColor = true;
            this.RefreshListButton.Click += new System.EventHandler(this.RefreshListButton_Click);
            // 
            // dataGridViewPatches
            // 
            this.dataGridViewPatches.AllowUserToResizeColumns = false;
            this.dataGridViewPatches.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewPatches.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewPatches.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dataGridViewPatches.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPatches.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnUpdate,
            this.ColumnDescription,
            this.ColumnDate,
            this.webPageColumn});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridViewPatches, 3);
            resources.ApplyResources(this.dataGridViewPatches, "dataGridViewPatches");
            this.dataGridViewPatches.Name = "dataGridViewPatches";
            this.dataGridViewPatches.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewPatches.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewPatches.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewPatches.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewPatches.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPatches_CellContentClick);
            this.dataGridViewPatches.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewPatches_CellMouseClick);
            this.dataGridViewPatches.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewPatches_ColumnHeaderMouseClick);
            this.dataGridViewPatches.SelectionChanged += new System.EventHandler(this.dataGridViewPatches_SelectionChanged);
            this.dataGridViewPatches.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridViewPatches_SortCompare);
            this.dataGridViewPatches.Enter += new System.EventHandler(this.dataGridViewPatches_Enter);
            // 
            // ColumnUpdate
            // 
            this.ColumnUpdate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnUpdate.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColumnUpdate.FillWeight = 76.67365F;
            resources.ApplyResources(this.ColumnUpdate, "ColumnUpdate");
            this.ColumnUpdate.Name = "ColumnUpdate";
            this.ColumnUpdate.ReadOnly = true;
            // 
            // ColumnDescription
            // 
            this.ColumnDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnDescription.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnDescription.FillWeight = 172.4619F;
            resources.ApplyResources(this.ColumnDescription, "ColumnDescription");
            this.ColumnDescription.Name = "ColumnDescription";
            this.ColumnDescription.ReadOnly = true;
            // 
            // ColumnDate
            // 
            this.ColumnDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnDate.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColumnDate.FillWeight = 80F;
            resources.ApplyResources(this.ColumnDate, "ColumnDate");
            this.ColumnDate.Name = "ColumnDate";
            this.ColumnDate.ReadOnly = true;
            // 
            // webPageColumn
            // 
            this.webPageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.webPageColumn.FillWeight = 60F;
            resources.ApplyResources(this.webPageColumn, "webPageColumn");
            this.webPageColumn.Name = "webPageColumn";
            this.webPageColumn.ReadOnly = true;
            this.webPageColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // PatchingWizard_SelectPatchPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PatchingWizard_SelectPatchPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPatches)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BrowseButton;
        private DataGridViewEx dataGridViewPatches;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.RadioButton selectFromDiskRadioButton;
        private System.Windows.Forms.Button RefreshListButton;
        private System.Windows.Forms.RadioButton downloadUpdateRadioButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button RestoreDismUpdatesButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnUpdate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDate;
        private System.Windows.Forms.DataGridViewLinkColumn webPageColumn;
        private System.Windows.Forms.Label automaticOptionLabel;
        private System.Windows.Forms.RadioButton AutomaticRadioButton;
    }
}
