namespace XenAdmin.Wizards.NewVMWizard
{
    partial class Page_Networking
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_Networking));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.AddButton = new System.Windows.Forms.Button();
            this.PropertiesButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.BoxTitle = new System.Windows.Forms.Label();
            this.NetworksGridView = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.MacColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NetworkColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelDefaultTemplateInfo = new System.Windows.Forms.Panel();
            this.labelDefaultTemplateInfo = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolTipContainerAddButton = new XenAdmin.Controls.ToolTipContainer();
            ((System.ComponentModel.ISupportInitialize)(this.NetworksGridView)).BeginInit();
            this.panelDefaultTemplateInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolTipContainerAddButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // AddButton
            // 
            resources.ApplyResources(this.AddButton, "AddButton");
            this.AddButton.Name = "AddButton";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // PropertiesButton
            // 
            resources.ApplyResources(this.PropertiesButton, "PropertiesButton");
            this.PropertiesButton.Name = "PropertiesButton";
            this.PropertiesButton.UseVisualStyleBackColor = true;
            this.PropertiesButton.Click += new System.EventHandler(this.PropertiesButton_Click);
            // 
            // DeleteButton
            // 
            resources.ApplyResources(this.DeleteButton, "DeleteButton");
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // BoxTitle
            // 
            this.BoxTitle.AutoEllipsis = true;
            resources.ApplyResources(this.BoxTitle, "BoxTitle");
            this.BoxTitle.Name = "BoxTitle";
            // 
            // NetworksGridView
            // 
            resources.ApplyResources(this.NetworksGridView, "NetworksGridView");
            this.NetworksGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.NetworksGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.NetworksGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.NetworksGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImageColumn,
            this.MacColumn,
            this.NetworkColumn});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.NetworksGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.NetworksGridView.Name = "NetworksGridView";
            // 
            // ImageColumn
            // 
            this.ImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ImageColumn, "ImageColumn");
            this.ImageColumn.Name = "ImageColumn";
            this.ImageColumn.ReadOnly = true;
            // 
            // MacColumn
            // 
            this.MacColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.MacColumn, "MacColumn");
            this.MacColumn.Name = "MacColumn";
            this.MacColumn.ReadOnly = true;
            // 
            // NetworkColumn
            // 
            this.NetworkColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.NetworkColumn, "NetworkColumn");
            this.NetworkColumn.Name = "NetworkColumn";
            this.NetworkColumn.ReadOnly = true;
            // 
            // panelDefaultTemplateInfo
            // 
            resources.ApplyResources(this.panelDefaultTemplateInfo, "panelDefaultTemplateInfo");
            this.panelDefaultTemplateInfo.Controls.Add(this.labelDefaultTemplateInfo);
            this.panelDefaultTemplateInfo.Controls.Add(this.pictureBox1);
            this.panelDefaultTemplateInfo.Name = "panelDefaultTemplateInfo";
            // 
            // labelDefaultTemplateInfo
            // 
            resources.ApplyResources(this.labelDefaultTemplateInfo, "labelDefaultTemplateInfo");
            this.labelDefaultTemplateInfo.Name = "labelDefaultTemplateInfo";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // toolTipContainerAddButton
            // 
            resources.ApplyResources(this.toolTipContainerAddButton, "toolTipContainerAddButton");
            this.toolTipContainerAddButton.Controls.Add(this.AddButton);
            this.toolTipContainerAddButton.Name = "toolTipContainerAddButton";
            // 
            // Page_Networking
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.toolTipContainerAddButton);
            this.Controls.Add(this.panelDefaultTemplateInfo);
            this.Controls.Add(this.NetworksGridView);
            this.Controls.Add(this.BoxTitle);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.PropertiesButton);
            this.Controls.Add(this.label1);
            this.Name = "Page_Networking";
            ((System.ComponentModel.ISupportInitialize)(this.NetworksGridView)).EndInit();
            this.panelDefaultTemplateInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolTipContainerAddButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button PropertiesButton;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Label BoxTitle;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx NetworksGridView;
        private System.Windows.Forms.DataGridViewImageColumn ImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MacColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NetworkColumn;
        private System.Windows.Forms.Panel panelDefaultTemplateInfo;
        private System.Windows.Forms.Label labelDefaultTemplateInfo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerAddButton;
    }
}
