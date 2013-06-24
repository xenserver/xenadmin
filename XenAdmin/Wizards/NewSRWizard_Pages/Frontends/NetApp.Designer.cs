namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class NetApp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetApp));
            this.nudFlexvols = new System.Windows.Forms.NumericUpDown();
            this.labelFlexvols = new System.Windows.Forms.Label();
            this.toolTipContainerDedup = new XenAdmin.Controls.ToolTipContainer();
            this.checkBoxDedup = new System.Windows.Forms.CheckBox();
            this.checkBoxThin = new System.Windows.Forms.CheckBox();
            this.checkBoxHttps = new System.Windows.Forms.CheckBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.helplinkFlexvols = new System.Windows.Forms.LinkLabel();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colAggregate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRaidType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAsis = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.listBoxSRs = new XenAdmin.Controls.SRListBox();
            this.radioButtonNew = new System.Windows.Forms.RadioButton();
            this.radioButtonReattach = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.nudFlexvols)).BeginInit();
            this.toolTipContainerDedup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // nudFlexvols
            // 
            resources.ApplyResources(this.nudFlexvols, "nudFlexvols");
            this.nudFlexvols.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudFlexvols.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFlexvols.Name = "nudFlexvols";
            this.nudFlexvols.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // labelFlexvols
            // 
            resources.ApplyResources(this.labelFlexvols, "labelFlexvols");
            this.labelFlexvols.Name = "labelFlexvols";
            // 
            // toolTipContainerDedup
            // 
            resources.ApplyResources(this.toolTipContainerDedup, "toolTipContainerDedup");
            this.toolTipContainerDedup.Controls.Add(this.checkBoxDedup);
            this.toolTipContainerDedup.Name = "toolTipContainerDedup";
            // 
            // checkBoxDedup
            // 
            resources.ApplyResources(this.checkBoxDedup, "checkBoxDedup");
            this.checkBoxDedup.Name = "checkBoxDedup";
            this.checkBoxDedup.UseVisualStyleBackColor = true;
            // 
            // checkBoxThin
            // 
            resources.ApplyResources(this.checkBoxThin, "checkBoxThin");
            this.checkBoxThin.Name = "checkBoxThin";
            this.checkBoxThin.UseVisualStyleBackColor = true;
            this.checkBoxThin.CheckedChanged += new System.EventHandler(this.checkBoxThin_CheckedChanged);
            // 
            // checkBoxHttps
            // 
            resources.ApplyResources(this.checkBoxHttps, "checkBoxHttps");
            this.checkBoxHttps.Name = "checkBoxHttps";
            this.checkBoxHttps.UseVisualStyleBackColor = true;
            this.checkBoxHttps.CheckedChanged += new System.EventHandler(this.checkBoxHttps_CheckedChanged);
            // 
            // labelPort
            // 
            resources.ApplyResources(this.labelPort, "labelPort");
            this.labelPort.Name = "labelPort";
            // 
            // helplinkFlexvols
            // 
            resources.ApplyResources(this.helplinkFlexvols, "helplinkFlexvols");
            this.helplinkFlexvols.Name = "helplinkFlexvols";
            this.helplinkFlexvols.TabStop = true;
            this.helplinkFlexvols.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helplinkFlexvols_LinkClicked);
            // 
            // textBoxPort
            // 
            resources.ApplyResources(this.textBoxPort, "textBoxPort");
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxPort_KeyDown);
            this.textBoxPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPort_KeyPress);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAggregate,
            this.colSize,
            this.colDisks,
            this.colRaidType,
            this.colAsis});
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // colAggregate
            // 
            this.colAggregate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colAggregate, "colAggregate");
            this.colAggregate.Name = "colAggregate";
            this.colAggregate.ReadOnly = true;
            // 
            // colSize
            // 
            this.colSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colSize, "colSize");
            this.colSize.Name = "colSize";
            this.colSize.ReadOnly = true;
            // 
            // colDisks
            // 
            this.colDisks.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colDisks, "colDisks");
            this.colDisks.Name = "colDisks";
            this.colDisks.ReadOnly = true;
            // 
            // colRaidType
            // 
            this.colRaidType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colRaidType, "colRaidType");
            this.colRaidType.Name = "colRaidType";
            this.colRaidType.ReadOnly = true;
            // 
            // colAsis
            // 
            this.colAsis.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colAsis, "colAsis");
            this.colAsis.Name = "colAsis";
            this.colAsis.ReadOnly = true;
            // 
            // listBoxSRs
            // 
            resources.ApplyResources(this.listBoxSRs, "listBoxSRs");
            this.listBoxSRs.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxSRs.FormattingEnabled = true;
            this.listBoxSRs.Name = "listBoxSRs";
            this.listBoxSRs.Sorted = true;
            this.listBoxSRs.SelectedIndexChanged += new System.EventHandler(this.listBoxSRs_SelectedIndexChanged);
            // 
            // radioButtonNew
            // 
            resources.ApplyResources(this.radioButtonNew, "radioButtonNew");
            this.radioButtonNew.Name = "radioButtonNew";
            this.radioButtonNew.TabStop = true;
            this.radioButtonNew.UseVisualStyleBackColor = true;
            this.radioButtonNew.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonReattach
            // 
            resources.ApplyResources(this.radioButtonReattach, "radioButtonReattach");
            this.radioButtonReattach.Name = "radioButtonReattach";
            this.radioButtonReattach.TabStop = true;
            this.radioButtonReattach.UseVisualStyleBackColor = true;
            this.radioButtonReattach.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.nudFlexvols);
            this.panel1.Controls.Add(this.labelFlexvols);
            this.panel1.Controls.Add(this.checkBoxThin);
            this.panel1.Controls.Add(this.toolTipContainerDedup);
            this.panel1.Controls.Add(this.checkBoxHttps);
            this.panel1.Controls.Add(this.textBoxPort);
            this.panel1.Controls.Add(this.labelPort);
            this.panel1.Controls.Add(this.helplinkFlexvols);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.radioButtonReattach, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.listBoxSRs, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonNew, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // NetApp
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NetApp";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this.nudFlexvols)).EndInit();
            this.toolTipContainerDedup.ResumeLayout(false);
            this.toolTipContainerDedup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudFlexvols;
        private System.Windows.Forms.Label labelFlexvols;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerDedup;
        private System.Windows.Forms.CheckBox checkBoxDedup;
        private System.Windows.Forms.CheckBox checkBoxThin;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.CheckBox checkBoxHttps;
        private System.Windows.Forms.LinkLabel helplinkFlexvols;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.DataGridView dataGridView1;
        private XenAdmin.Controls.SRListBox listBoxSRs;
        private System.Windows.Forms.RadioButton radioButtonNew;
        private System.Windows.Forms.RadioButton radioButtonReattach;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAggregate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisks;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRaidType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAsis;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
