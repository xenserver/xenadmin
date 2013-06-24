using XenAdmin.Commands;

namespace XenAdmin.Dialogs.VMAppliances
{
    partial class VMAppliancesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VMAppliancesDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewVMAppliances = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonStart = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShutdown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonImport = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelVMAppliancesInPool = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.listViewVMs = new XenAdmin.Controls.DoubleBufferedListView();
            this.labelVMApplianceName = new System.Windows.Forms.Label();
            this.NameColum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VMsColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVMAppliances)).BeginInit();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewVMAppliances
            // 
            this.dataGridViewVMAppliances.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridViewVMAppliances.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewVMAppliances.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewVMAppliances.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridViewVMAppliances.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColum,
            this.DescriptionColumn,
            this.VMsColumn});
            resources.ApplyResources(this.dataGridViewVMAppliances, "dataGridViewVMAppliances");
            this.dataGridViewVMAppliances.Name = "dataGridViewVMAppliances";
            this.dataGridViewVMAppliances.ReadOnly = true;
            this.dataGridViewVMAppliances.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.toolStrip1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNew,
            this.toolStripButtonDelete,
            this.toolStripButtonEdit,
            this.toolStripSeparator1,
            this.toolStripButtonStart,
            this.toolStripButtonShutdown,
            this.toolStripSeparator2,
            this.toolStripButtonImport,
            this.toolStripButtonExport});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripButtonNew
            // 
            resources.ApplyResources(this.toolStripButtonNew, "toolStripButtonNew");
            this.toolStripButtonNew.Image = global::XenAdmin.Properties.Resources._000_NewVirtualAppliance_h32bit_16;
            this.toolStripButtonNew.Name = "toolStripButtonNew";
            this.toolStripButtonNew.Click += new System.EventHandler(this.toolStripButtonNew_Click);
            // 
            // toolStripButtonDelete
            // 
            resources.ApplyResources(this.toolStripButtonDelete, "toolStripButtonDelete");
            this.toolStripButtonDelete.Image = global::XenAdmin.Properties.Resources._000_DeleteVirtualAppliance_h32bit_16;
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // toolStripButtonEdit
            // 
            resources.ApplyResources(this.toolStripButtonEdit, "toolStripButtonEdit");
            this.toolStripButtonEdit.Image = global::XenAdmin.Properties.Resources.edit_16;
            this.toolStripButtonEdit.Name = "toolStripButtonEdit";
            this.toolStripButtonEdit.Click += new System.EventHandler(this.toolStripButtonEdit_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonStart
            // 
            resources.ApplyResources(this.toolStripButtonStart, "toolStripButtonStart");
            this.toolStripButtonStart.Image = global::XenAdmin.Properties.Resources._001_PowerOn_h32bit_16;
            this.toolStripButtonStart.Name = "toolStripButtonStart";
            this.toolStripButtonStart.Click += new System.EventHandler(this.toolStripMenuItemStart_Click);
            // 
            // toolStripButtonShutdown
            // 
            resources.ApplyResources(this.toolStripButtonShutdown, "toolStripButtonShutdown");
            this.toolStripButtonShutdown.Image = global::XenAdmin.Properties.Resources._001_ShutDown_h32bit_16;
            this.toolStripButtonShutdown.Name = "toolStripButtonShutdown";
            this.toolStripButtonShutdown.Click += new System.EventHandler(this.toolStripButtonShutdown_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripButtonImport
            // 
            resources.ApplyResources(this.toolStripButtonImport, "toolStripButtonImport");
            this.toolStripButtonImport.Image = global::XenAdmin.Properties.Resources._000_ImportVirtualAppliance_h32bit_16;
            this.toolStripButtonImport.Name = "toolStripButtonImport";
            this.toolStripButtonImport.Click += new System.EventHandler(this.toolStripButtonImport_Click);
            // 
            // toolStripButtonExport
            // 
            resources.ApplyResources(this.toolStripButtonExport, "toolStripButtonExport");
            this.toolStripButtonExport.Image = global::XenAdmin.Properties.Resources._000_ExportVirtualAppliance_h32bit_16;
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Click += new System.EventHandler(this.toolStripButtonExport_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelVMAppliancesInPool, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewVMAppliances, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelVMAppliancesInPool
            // 
            resources.ApplyResources(this.labelVMAppliancesInPool, "labelVMAppliancesInPool");
            this.tableLayoutPanel1.SetColumnSpan(this.labelVMAppliancesInPool, 2);
            this.labelVMAppliancesInPool.Name = "labelVMAppliancesInPool";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.listViewVMs, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelVMApplianceName, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // listViewVMs
            // 
            this.listViewVMs.BackColor = System.Drawing.SystemColors.Control;
            this.listViewVMs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel2.SetColumnSpan(this.listViewVMs, 4);
            this.listViewVMs.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.listViewVMs, "listViewVMs");
            this.listViewVMs.FullRowSelect = true;
            this.listViewVMs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewVMs.MultiSelect = false;
            this.listViewVMs.Name = "listViewVMs";
            this.listViewVMs.ShowItemToolTips = true;
            this.listViewVMs.UseCompatibleStateImageBehavior = false;
            this.listViewVMs.View = System.Windows.Forms.View.SmallIcon;
            // 
            // labelVMApplianceName
            // 
            this.labelVMApplianceName.AutoEllipsis = true;
            resources.ApplyResources(this.labelVMApplianceName, "labelVMApplianceName");
            this.labelVMApplianceName.Name = "labelVMApplianceName";
            // 
            // NameColum
            // 
            this.NameColum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.NameColum, "NameColum");
            this.NameColum.Name = "NameColum";
            this.NameColum.ReadOnly = true;
            // 
            // DescriptionColumn
            // 
            this.DescriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.DescriptionColumn, "DescriptionColumn");
            this.DescriptionColumn.Name = "DescriptionColumn";
            this.DescriptionColumn.ReadOnly = true;
            this.DescriptionColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // VMsColumn
            // 
            this.VMsColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.VMsColumn.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.VMsColumn, "VMsColumn");
            this.VMsColumn.Name = "VMsColumn";
            this.VMsColumn.ReadOnly = true;
            // 
            // VMAppliancesDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "VMAppliancesDialog";
            this.Load += new System.EventHandler(this.VMAppliancesDialog_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.VMAppliancesDialog_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVMAppliances)).EndInit();
            this.panel1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewVMAppliances;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripButton toolStripButtonEdit;
        private System.Windows.Forms.ToolStripButton toolStripButtonShutdown;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label labelVMApplianceName;
        private System.Windows.Forms.Label labelVMAppliancesInPool;
        private System.Windows.Forms.ToolStripButton toolStripButtonExport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonStart;
        private XenAdmin.Controls.DoubleBufferedListView listViewVMs;
        private System.Windows.Forms.ToolStripButton toolStripButtonImport;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColum;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VMsColumn;
    }
}

