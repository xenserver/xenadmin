namespace XenAdmin.Controls.GPU
{
    partial class GpuRow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GpuRow));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel = new XenAdmin.Controls.PanelWithBorder();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.pGPULabel = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.editPlacementPolicyButton = new System.Windows.Forms.Button();
            this.dataGridViewEx1 = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.TextColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gpuShinyBar1 = new XenAdmin.Controls.GPU.GpuShinyBar();
            this.panel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            // panel
            // 
            resources.ApplyResources(this.panel, "panel");
            this.panel.BackColor = System.Drawing.Color.Transparent;
            this.panel.Controls.Add(this.tableLayoutPanel1);
            this.panel.Name = "panel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.autoHeightLabel1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.pGPULabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.editPlacementPolicyButton, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewEx1, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.gpuShinyBar1, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.autoHeightLabel1.AutoEllipsis = true;
            this.autoHeightLabel1.BackColor = System.Drawing.Color.Transparent;
            this.autoHeightLabel1.MinimumSize = new System.Drawing.Size(0, 16);
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // pGPULabel
            // 
            resources.ApplyResources(this.pGPULabel, "pGPULabel");
            this.pGPULabel.AutoEllipsis = true;
            this.pGPULabel.BackColor = System.Drawing.Color.Transparent;
            this.pGPULabel.MinimumSize = new System.Drawing.Size(0, 16);
            this.pGPULabel.Name = "pGPULabel";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_GetMemoryInfo_h32bit_16;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // editPlacementPolicyButton
            // 
            this.editPlacementPolicyButton.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.editPlacementPolicyButton, "editPlacementPolicyButton");
            this.editPlacementPolicyButton.Name = "editPlacementPolicyButton";
            this.editPlacementPolicyButton.UseVisualStyleBackColor = false;
            // 
            // dataGridViewEx1
            // 
            this.dataGridViewEx1.AllowUserToResizeColumns = false;
            this.dataGridViewEx1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewEx1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewEx1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewEx1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridViewEx1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewEx1.ColumnHeadersVisible = false;
            this.dataGridViewEx1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImageColumn,
            this.TextColumn});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewEx1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewEx1.HideSelection = true;
            resources.ApplyResources(this.dataGridViewEx1, "dataGridViewEx1");
            this.dataGridViewEx1.Name = "dataGridViewEx1";
            this.dataGridViewEx1.ReadOnly = true;
            // 
            // ImageColumn
            // 
            this.ImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ImageColumn.Frozen = true;
            resources.ApplyResources(this.ImageColumn, "ImageColumn");
            this.ImageColumn.Name = "ImageColumn";
            this.ImageColumn.ReadOnly = true;
            // 
            // TextColumn
            // 
            this.TextColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TextColumn.FillWeight = 190F;
            this.TextColumn.Frozen = true;
            resources.ApplyResources(this.TextColumn, "TextColumn");
            this.TextColumn.Name = "TextColumn";
            this.TextColumn.ReadOnly = true;
            this.TextColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // gpuShinyBar1
            // 
            resources.ApplyResources(this.gpuShinyBar1, "gpuShinyBar1");
            this.gpuShinyBar1.Name = "gpuShinyBar1";
            // 
            // GpuRow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panel);
            this.Name = "GpuRow";
            this.panel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PanelWithBorder panel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private XenAdmin.Controls.Common.AutoHeightLabel pGPULabel;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button editPlacementPolicyButton;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewEx1;
        private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
        private GpuShinyBar gpuShinyBar1;
        private System.Windows.Forms.DataGridViewImageColumn ImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TextColumn;
    }
}
