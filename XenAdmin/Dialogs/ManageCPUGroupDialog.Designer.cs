using XenAdmin.Commands;

namespace XenAdmin.Dialogs
{
    partial class ManageCPUGroupDialog
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEdit = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelCPUGroupInPool = new System.Windows.Forms.Label();
            this.dataGridViewCPUGroups = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CPUCoresColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.labelCPUGroupName = new System.Windows.Forms.Label();
            this.listViewCPUGroup = new XenAdmin.Controls.DoubleBufferedListView();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCPUGroups)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(1);
            this.panel1.Size = new System.Drawing.Size(721, 30);
            this.panel1.TabIndex = 67;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNew,
            this.toolStripButtonDelete,
            this.toolStripButtonEdit});
            this.toolStrip1.Location = new System.Drawing.Point(1, 1);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(719, 28);
            this.toolStrip1.TabIndex = 65;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonNew
            // 
            this.toolStripButtonNew.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripButtonNew.Image = global::XenAdmin.Properties.Resources._000_NewVirtualAppliance_h32bit_16;
            this.toolStripButtonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNew.Name = "toolStripButtonNew";
            this.toolStripButtonNew.Size = new System.Drawing.Size(121, 25);
            this.toolStripButtonNew.Text = "&New CPU group...";
            this.toolStripButtonNew.Click += new System.EventHandler(this.toolStripButtonNew_Click);
            // 
            // toolStripButtonDelete
            // 
            this.toolStripButtonDelete.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripButtonDelete.Image = global::XenAdmin.Properties.Resources._000_DeleteVirtualAppliance_h32bit_16;
            this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Size = new System.Drawing.Size(60, 25);
            this.toolStripButtonDelete.Text = "&Delete";
            // 
            // toolStripButtonEdit
            // 
            this.toolStripButtonEdit.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripButtonEdit.Image = global::XenAdmin.Properties.Resources.edit_16;
            this.toolStripButtonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEdit.Name = "toolStripButtonEdit";
            this.toolStripButtonEdit.Size = new System.Drawing.Size(80, 25);
            this.toolStripButtonEdit.Text = "&Properties";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Controls.Add(this.labelCPUGroupInPool, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewCPUGroups, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 36);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(702, 353);
            this.tableLayoutPanel1.TabIndex = 68;
            // 
            // labelCPUGroupInPool
            // 
            this.labelCPUGroupInPool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCPUGroupInPool.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelCPUGroupInPool, 2);
            this.labelCPUGroupInPool.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelCPUGroupInPool.Location = new System.Drawing.Point(0, 0);
            this.labelCPUGroupInPool.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelCPUGroupInPool.Name = "labelCPUGroupInPool";
            this.labelCPUGroupInPool.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.labelCPUGroupInPool.Size = new System.Drawing.Size(699, 19);
            this.labelCPUGroupInPool.TabIndex = 0;
            this.labelCPUGroupInPool.Text = "CPU Groups defined in pool {0}:";
            // 
            // dataGridViewCPUGroups
            // 
            this.dataGridViewCPUGroups.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridViewCPUGroups.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewCPUGroups.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewCPUGroups.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridViewCPUGroups.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.DescriptionColumn,
            this.CPUCoresColumn});
            this.dataGridViewCPUGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCPUGroups.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dataGridViewCPUGroups.Location = new System.Drawing.Point(3, 22);
            this.dataGridViewCPUGroups.Name = "dataGridViewCPUGroups";
            this.dataGridViewCPUGroups.ReadOnly = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewCPUGroups.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewCPUGroups.Size = new System.Drawing.Size(496, 328);
            this.dataGridViewCPUGroups.TabIndex = 1;
            // 
            // NameColumn
            // 
            this.NameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.NameColumn.HeaderText = "Name";
            this.NameColumn.MinimumWidth = 30;
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            this.NameColumn.Width = 200;
            // 
            // DescriptionColumn
            // 
            this.DescriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DescriptionColumn.HeaderText = "Description";
            this.DescriptionColumn.MinimumWidth = 30;
            this.DescriptionColumn.Name = "DescriptionColumn";
            this.DescriptionColumn.ReadOnly = true;
            // 
            // CPUCoresColumn
            // 
            this.CPUCoresColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CPUCoresColumn.HeaderText = "#CPU Cores";
            this.CPUCoresColumn.MinimumWidth = 30;
            this.CPUCoresColumn.Name = "CPUCoresColumn";
            this.CPUCoresColumn.ReadOnly = true;
            this.CPUCoresColumn.Width = 95;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.labelCPUGroupName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.listViewCPUGroup, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(505, 22);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(194, 328);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // labelCPUGroupName
            // 
            this.labelCPUGroupName.AutoEllipsis = true;
            this.labelCPUGroupName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCPUGroupName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelCPUGroupName.Location = new System.Drawing.Point(4, 1);
            this.labelCPUGroupName.Name = "labelCPUGroupName";
            this.labelCPUGroupName.Padding = new System.Windows.Forms.Padding(0, 2, 0, 4);
            this.labelCPUGroupName.Size = new System.Drawing.Size(186, 21);
            this.labelCPUGroupName.TabIndex = 0;
            // 
            // listViewCPUGroup
            // 
            this.listViewCPUGroup.BackColor = System.Drawing.SystemColors.Control;
            this.listViewCPUGroup.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel2.SetColumnSpan(this.listViewCPUGroup, 4);
            this.listViewCPUGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewCPUGroup.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.listViewCPUGroup.FullRowSelect = true;
            this.listViewCPUGroup.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewCPUGroup.Location = new System.Drawing.Point(7, 29);
            this.listViewCPUGroup.Margin = new System.Windows.Forms.Padding(6);
            this.listViewCPUGroup.MultiSelect = false;
            this.listViewCPUGroup.Name = "listViewCPUGroup";
            this.listViewCPUGroup.ShowItemToolTips = true;
            this.listViewCPUGroup.Size = new System.Drawing.Size(180, 292);
            this.listViewCPUGroup.TabIndex = 1;
            this.listViewCPUGroup.UseCompatibleStateImageBehavior = false;
            this.listViewCPUGroup.View = System.Windows.Forms.View.SmallIcon;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.buttonCancel.Location = new System.Drawing.Point(639, 395);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 69;
            this.buttonCancel.Text = "Close";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // ManageCPUGroupDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 428);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "ManageCPUGroupDialog";
            this.Text = "Manage CPU Groups";
            this.panel1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCPUGroups)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonNew;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripButton toolStripButtonEdit;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelCPUGroupInPool;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewCPUGroups;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label labelCPUGroupName;
        private XenAdmin.Controls.DoubleBufferedListView listViewCPUGroup;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn CPUCoresColumn;
    }
}