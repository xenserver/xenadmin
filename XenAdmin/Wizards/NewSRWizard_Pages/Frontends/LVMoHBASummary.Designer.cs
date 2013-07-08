namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class LVMoHBASummary
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewSummary = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnArrow = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnDetails = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSummary)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewSummary
            // 
            this.dataGridViewSummary.AllowUserToResizeColumns = false;
            this.dataGridViewSummary.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewSummary.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewSummary.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSummary.ColumnHeadersVisible = false;
            this.dataGridViewSummary.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnArrow,
            this.ColumnDetails});
            this.dataGridViewSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewSummary.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewSummary.Name = "dataGridViewSummary";
            this.dataGridViewSummary.ReadOnly = true;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            this.dataGridViewSummary.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewSummary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridViewSummary.Size = new System.Drawing.Size(711, 455);
            this.dataGridViewSummary.TabIndex = 5;
            this.dataGridViewSummary.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewSummary_CellClick);
            // 
            // ColumnArrow
            // 
            this.ColumnArrow.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.NullValue = "System.Drawing.Bitmap";
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnArrow.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColumnArrow.FillWeight = 31.26904F;
            this.ColumnArrow.Frozen = true;
            this.ColumnArrow.HeaderText = "";
            this.ColumnArrow.MinimumWidth = 16;
            this.ColumnArrow.Name = "ColumnArrow";
            this.ColumnArrow.ReadOnly = true;
            this.ColumnArrow.Width = 16;
            // 
            // ColumnDetails
            // 
            this.ColumnDetails.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnDetails.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnDetails.FillWeight = 172.4619F;
            this.ColumnDetails.HeaderText = "Details";
            this.ColumnDetails.MinimumWidth = 90;
            this.ColumnDetails.Name = "ColumnDetails";
            this.ColumnDetails.ReadOnly = true;
            // 
            // LVMoHBASummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewSummary);
            this.DoubleBuffered = true;
            this.Name = "LVMoHBASummary";
            this.Size = new System.Drawing.Size(711, 455);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSummary)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewSummary;
        private System.Windows.Forms.DataGridViewImageColumn ColumnArrow;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDetails;

    }
}
