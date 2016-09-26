namespace XenAdmin.Controls
{
    partial class AffinityPicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AffinityPicker));
            this.DynamicRadioButton = new XenAdmin.Controls.Common.AutoHeightRadioButton();
            this.StaticRadioButton = new System.Windows.Forms.RadioButton();
            this.labelInstructions = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelWlbWarning = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ServersGridView = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReasonColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelWlbWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServersGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // DynamicRadioButton
            // 
            resources.ApplyResources(this.DynamicRadioButton, "DynamicRadioButton");
            this.DynamicRadioButton.ForeColor = System.Drawing.SystemColors.WindowText;
            this.DynamicRadioButton.Name = "DynamicRadioButton";
            this.DynamicRadioButton.UseVisualStyleBackColor = true;
            // 
            // StaticRadioButton
            // 
            resources.ApplyResources(this.StaticRadioButton, "StaticRadioButton");
            this.StaticRadioButton.Checked = true;
            this.StaticRadioButton.ForeColor = System.Drawing.SystemColors.WindowText;
            this.StaticRadioButton.Name = "StaticRadioButton";
            this.StaticRadioButton.TabStop = true;
            this.StaticRadioButton.UseVisualStyleBackColor = true;
            this.StaticRadioButton.CheckedChanged += new System.EventHandler(this.StaticRadioButton_CheckedChanged);
            // 
            // labelInstructions
            // 
            resources.ApplyResources(this.labelInstructions, "labelInstructions");
            this.labelInstructions.Name = "labelInstructions";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelInstructions, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelWlbWarning, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.DynamicRadioButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.StaticRadioButton, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.ServersGridView, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanelWlbWarning
            // 
            resources.ApplyResources(this.tableLayoutPanelWlbWarning, "tableLayoutPanelWlbWarning");
            this.tableLayoutPanelWlbWarning.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanelWlbWarning.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanelWlbWarning.Name = "tableLayoutPanelWlbWarning";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ServersGridView
            // 
            this.ServersGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.ServersGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.ServersGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ServersGridView.ColumnHeadersVisible = false;
            this.ServersGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImageColumn,
            this.NameColumn,
            this.ReasonColumn});
            resources.ApplyResources(this.ServersGridView, "ServersGridView");
            this.ServersGridView.Name = "ServersGridView";
            this.ServersGridView.VisibleChanged += new System.EventHandler(this.ServersGridView_VisibleChanged);
            // 
            // ImageColumn
            // 
            this.ImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ImageColumn, "ImageColumn");
            this.ImageColumn.Name = "ImageColumn";
            // 
            // NameColumn
            // 
            this.NameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.NameColumn, "NameColumn");
            this.NameColumn.Name = "NameColumn";
            // 
            // ReasonColumn
            // 
            this.ReasonColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ReasonColumn, "ReasonColumn");
            this.ReasonColumn.Name = "ReasonColumn";
            // 
            // AffinityPicker
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "AffinityPicker";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelWlbWarning.ResumeLayout(false);
            this.tableLayoutPanelWlbWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServersGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.Common.AutoHeightRadioButton DynamicRadioButton;
        private System.Windows.Forms.RadioButton StaticRadioButton;
        private System.Windows.Forms.Label labelInstructions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx ServersGridView;
        private System.Windows.Forms.DataGridViewImageColumn ImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReasonColumn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelWlbWarning;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
