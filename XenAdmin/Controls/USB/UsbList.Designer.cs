namespace XenAdmin.Controls
{
    partial class UsbList
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
            this.UsbGridView = new System.Windows.Forms.DataGridView();
            this.buttonUsage = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAttach = new System.Windows.Forms.Button();
            this.buttonDetach = new System.Windows.Forms.Button();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LocationColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UsageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VMColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AttachedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.UsbGridView)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UsbGridView
            // 
            this.UsbGridView.AllowUserToAddRows = false;
            this.UsbGridView.AllowUserToDeleteRows = false;
            this.UsbGridView.AllowUserToResizeColumns = false;
            this.UsbGridView.AllowUserToResizeRows = false;
            this.UsbGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UsbGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.UsbGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.UsbGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.UsbGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.UsbGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.UsbGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.UsbGridView.GridColor = System.Drawing.SystemColors.Window;
            this.UsbGridView.Location = new System.Drawing.Point(0, 0);
            this.UsbGridView.Margin = new System.Windows.Forms.Padding(0);
            this.UsbGridView.MultiSelect = false;
            this.UsbGridView.Name = "UsbGridView";
            this.UsbGridView.RowHeadersVisible = false;
            this.UsbGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.UsbGridView.ShowCellErrors = false;
            this.UsbGridView.ShowEditingIcon = false;
            this.UsbGridView.ShowRowErrors = false;
            this.UsbGridView.Size = new System.Drawing.Size(650, 182);
            this.UsbGridView.StandardTab = true;
            this.UsbGridView.TabIndex = 0;
            this.UsbGridView.SelectionChanged += new System.EventHandler(this.UsbGridView_SelectionChanged);
            // 
            // buttonUsage
            // 
            this.buttonUsage.BackColor = System.Drawing.Color.Transparent;
            this.buttonUsage.Enabled = false;
            this.buttonUsage.Location = new System.Drawing.Point(0, 5);
            this.buttonUsage.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.buttonUsage.Name = "buttonUsage";
            this.buttonUsage.Size = new System.Drawing.Size(102, 23);
            this.buttonUsage.TabIndex = 1;
            this.buttonUsage.Text = "&Usage";
            this.buttonUsage.UseVisualStyleBackColor = true;
            this.buttonUsage.Click += new System.EventHandler(this.buttonUsage_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonUsage);
            this.flowLayoutPanel1.Controls.Add(this.buttonAttach);
            this.flowLayoutPanel1.Controls.Add(this.buttonDetach);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 182);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(650, 35);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // buttonAttach
            // 
            this.buttonAttach.BackColor = System.Drawing.Color.Transparent;
            this.buttonAttach.Enabled = false;
            this.buttonAttach.Location = new System.Drawing.Point(105, 5);
            this.buttonAttach.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.buttonAttach.Name = "buttonAttach";
            this.buttonAttach.Size = new System.Drawing.Size(102, 23);
            this.buttonAttach.TabIndex = 2;
            this.buttonAttach.Text = "&Attach";
            this.buttonAttach.UseVisualStyleBackColor = true;
            this.buttonAttach.Click += new System.EventHandler(this.buttonAttach_Click);
            // 
            // buttonDetach
            // 
            this.buttonDetach.BackColor = System.Drawing.Color.Transparent;
            this.buttonDetach.Enabled = false;
            this.buttonDetach.Location = new System.Drawing.Point(210, 5);
            this.buttonDetach.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.buttonDetach.Name = "buttonDetach";
            this.buttonDetach.Size = new System.Drawing.Size(102, 23);
            this.buttonDetach.TabIndex = 3;
            this.buttonDetach.Text = "&Detach";
            this.buttonDetach.UseVisualStyleBackColor = true;
            this.buttonDetach.Click += new System.EventHandler(this.buttonDetach_Click);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // LocationColumn
            // 
            this.LocationColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LocationColumn.FillWeight = 15F;
            this.LocationColumn.HeaderText = "Location";
            this.LocationColumn.Name = "LocationColumn";
            this.LocationColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // DescriptionColumn
            // 
            this.DescriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DescriptionColumn.FillWeight = 40F;
            this.DescriptionColumn.HeaderText = "Description";
            this.DescriptionColumn.Name = "DescriptionColumn";
            this.DescriptionColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // UsageColumn
            // 
            this.UsageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.UsageColumn.FillWeight = 22F;
            this.UsageColumn.HeaderText = "Allow pass through";
            this.UsageColumn.Name = "UsageColumn";
            this.UsageColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // VMColumn
            // 
            this.VMColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.VMColumn.FillWeight = 23F;
            this.VMColumn.HeaderText = "Virtual Machine";
            this.VMColumn.Name = "VMColumn";
            this.VMColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // AttachedColumn
            // 
            this.AttachedColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.AttachedColumn.FillWeight = 23F;
            this.AttachedColumn.HeaderText = "Attached";
            this.AttachedColumn.Name = "AttachedColumn";
            this.AttachedColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // UsbList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.UsbGridView);
            this.Name = "UsbList";
            this.Size = new System.Drawing.Size(650, 217);
            ((System.ComponentModel.ISupportInitialize)(this.UsbGridView)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView UsbGridView;
        private System.Windows.Forms.Button buttonUsage;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn LocationColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UsageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VMColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn AttachedColumn;
        private System.Windows.Forms.Button buttonAttach;
        private System.Windows.Forms.Button buttonDetach;
    }
}
