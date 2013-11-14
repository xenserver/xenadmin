namespace XenAdmin.Controls.GPU
{
    partial class GpuConfiguration
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GpuConfiguration));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.dataGridViewEx1 = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.rubricLabel = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.CheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VGPUsPerGPUColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxResolutionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxDisplaysColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VideoRAMColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx1)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "000_Abort_h32bit_16.png");
            this.imageList1.Images.SetKeyName(1, "000_Tick_h32bit_16.png");
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.okButton, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewEx1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.rubricLabel, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // dataGridViewEx1
            // 
            this.dataGridViewEx1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewEx1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewEx1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGridViewEx1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewEx1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CheckBoxColumn,
            this.NameColumn,
            this.VGPUsPerGPUColumn,
            this.MaxResolutionColumn,
            this.MaxDisplaysColumn,
            this.VideoRAMColumn});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridViewEx1, 3);
            resources.ApplyResources(this.dataGridViewEx1, "dataGridViewEx1");
            this.dataGridViewEx1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystrokeOrF2;
            this.dataGridViewEx1.Name = "dataGridViewEx1";
            this.dataGridViewEx1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewEx1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewEx1.SelectionChanged += new System.EventHandler(this.dataGridViewEx1_SelectionChanged);
            // 
            // rubricLabel
            // 
            resources.ApplyResources(this.rubricLabel, "rubricLabel");
            this.rubricLabel.AutoEllipsis = true;
            this.rubricLabel.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.SetColumnSpan(this.rubricLabel, 3);
            this.rubricLabel.MinimumSize = new System.Drawing.Size(0, 16);
            this.rubricLabel.Name = "rubricLabel";
            // 
            // CheckBoxColumn
            // 
            this.CheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            resources.ApplyResources(this.CheckBoxColumn, "CheckBoxColumn");
            this.CheckBoxColumn.Name = "CheckBoxColumn";
            this.CheckBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // NameColumn
            // 
            resources.ApplyResources(this.NameColumn, "NameColumn");
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // VGPUsPerGPUColumn
            // 
            this.VGPUsPerGPUColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.VGPUsPerGPUColumn.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.VGPUsPerGPUColumn, "VGPUsPerGPUColumn");
            this.VGPUsPerGPUColumn.Name = "VGPUsPerGPUColumn";
            this.VGPUsPerGPUColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.VGPUsPerGPUColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // MaxResolutionColumn
            // 
            this.MaxResolutionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.MaxResolutionColumn.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.MaxResolutionColumn, "MaxResolutionColumn");
            this.MaxResolutionColumn.Name = "MaxResolutionColumn";
            this.MaxResolutionColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.MaxResolutionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // MaxDisplaysColumn
            // 
            this.MaxDisplaysColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.MaxDisplaysColumn.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.MaxDisplaysColumn, "MaxDisplaysColumn");
            this.MaxDisplaysColumn.Name = "MaxDisplaysColumn";
            this.MaxDisplaysColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.MaxDisplaysColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // VideoRAMColumn
            // 
            this.VideoRAMColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.VideoRAMColumn.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.VideoRAMColumn, "VideoRAMColumn");
            this.VideoRAMColumn.Name = "VideoRAMColumn";
            this.VideoRAMColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.VideoRAMColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // GpuConfiguration
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "GpuConfiguration";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ImageList imageList1;
        private XenAdmin.Controls.Common.AutoHeightLabel rubricLabel;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewEx1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VGPUsPerGPUColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxResolutionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxDisplaysColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VideoRAMColumn;
    }
}
