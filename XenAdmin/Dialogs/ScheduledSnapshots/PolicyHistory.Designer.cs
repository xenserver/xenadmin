using XenAdmin.Controls.DataGridViewEx;

namespace XenAdmin.Dialogs.ScheduledSnapshots
{
    partial class PolicyHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PolicyHistory));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelShow = new System.Windows.Forms.Label();
            this.comboBoxTimeSpan = new System.Windows.Forms.ComboBox();
            this.labelHistory = new System.Windows.Forms.Label();
            this.panelLoading = new System.Windows.Forms.Panel();
            this.labelLoading = new System.Windows.Forms.Label();
            this.pictureBoxSpinner = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panelHistory = new System.Windows.Forms.Panel();
            this.dataGridViewRunHistory = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnExpand = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelLoading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpinner)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.panelHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRunHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // labelShow
            // 
            resources.ApplyResources(this.labelShow, "labelShow");
            this.labelShow.Name = "labelShow";
            // 
            // comboBoxTimeSpan
            // 
            resources.ApplyResources(this.comboBoxTimeSpan, "comboBoxTimeSpan");
            this.comboBoxTimeSpan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTimeSpan.FormattingEnabled = true;
            this.comboBoxTimeSpan.Items.AddRange(new object[] {
            resources.GetString("comboBoxTimeSpan.Items"),
            resources.GetString("comboBoxTimeSpan.Items1"),
            resources.GetString("comboBoxTimeSpan.Items2")});
            this.comboBoxTimeSpan.Name = "comboBoxTimeSpan";
            this.comboBoxTimeSpan.SelectedIndexChanged += new System.EventHandler(this.comboBoxTimeSpan_SelectedIndexChanged);
            // 
            // labelHistory
            // 
            resources.ApplyResources(this.labelHistory, "labelHistory");
            this.labelHistory.Name = "labelHistory";
            // 
            // panelLoading
            // 
            resources.ApplyResources(this.panelLoading, "panelLoading");
            this.panelLoading.BackColor = System.Drawing.SystemColors.Window;
            this.panelLoading.Controls.Add(this.pictureBoxSpinner);
            this.panelLoading.Controls.Add(this.labelLoading);
            this.panelLoading.Name = "panelLoading";
            // 
            // labelLoading
            // 
            resources.ApplyResources(this.labelLoading, "labelLoading");
            this.labelLoading.Name = "labelLoading";
            // 
            // pictureBoxSpinner
            // 
            resources.ApplyResources(this.pictureBoxSpinner, "pictureBoxSpinner");
            this.pictureBoxSpinner.Image = global::XenAdmin.Properties.Resources.ajax_loader;
            this.pictureBoxSpinner.Name = "pictureBoxSpinner";
            this.pictureBoxSpinner.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.labelHistory, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelShow, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.comboBoxTimeSpan, 2, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // panelHistory
            // 
            this.panelHistory.Controls.Add(this.panelLoading);
            this.panelHistory.Controls.Add(this.dataGridViewRunHistory);
            this.panelHistory.Controls.Add(this.tableLayoutPanel2);
            resources.ApplyResources(this.panelHistory, "panelHistory");
            this.panelHistory.Name = "panelHistory";
            // 
            // dataGridViewRunHistory
            // 
            this.dataGridViewRunHistory.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewRunHistory.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewRunHistory.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewRunHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewRunHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnExpand,
            this.ColumnResult,
            this.ColumnDateTime,
            this.ColumnDescription});
            resources.ApplyResources(this.dataGridViewRunHistory, "dataGridViewRunHistory");
            this.dataGridViewRunHistory.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridViewRunHistory.Name = "dataGridViewRunHistory";
            this.dataGridViewRunHistory.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewRunHistory.RowsDefaultCellStyle = dataGridViewCellStyle3;
            // 
            // ColumnExpand
            // 
            this.ColumnExpand.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.NullValue = "System.Drawing.Bitmap";
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(5);
            this.ColumnExpand.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.ColumnExpand, "ColumnExpand");
            this.ColumnExpand.Name = "ColumnExpand";
            this.ColumnExpand.ReadOnly = true;
            // 
            // ColumnResult
            // 
            this.ColumnResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnResult.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.ColumnResult, "ColumnResult");
            this.ColumnResult.Name = "ColumnResult";
            this.ColumnResult.ReadOnly = true;
            // 
            // ColumnDateTime
            // 
            this.ColumnDateTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnDateTime, "ColumnDateTime");
            this.ColumnDateTime.Name = "ColumnDateTime";
            this.ColumnDateTime.ReadOnly = true;
            // 
            // ColumnDescription
            // 
            this.ColumnDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnDescription, "ColumnDescription");
            this.ColumnDescription.Name = "ColumnDescription";
            this.ColumnDescription.ReadOnly = true;
            // 
            // PolicyHistory
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panelHistory);
            this.Name = "PolicyHistory";
            this.panelLoading.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpinner)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panelHistory.ResumeLayout(false);
            this.panelHistory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRunHistory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridViewEx dataGridViewRunHistory;
        private System.Windows.Forms.Label labelShow;
        private System.Windows.Forms.ComboBox comboBoxTimeSpan;
        private System.Windows.Forms.Label labelHistory;
        private System.Windows.Forms.Panel panelLoading;
        private System.Windows.Forms.PictureBox pictureBoxSpinner;
        private System.Windows.Forms.Label labelLoading;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridViewImageColumn ColumnExpand;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDescription;
        private System.Windows.Forms.Panel panelHistory;
    }
}
