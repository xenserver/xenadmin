namespace XenAdmin.Dialogs.Wlb
{
    partial class WlbReportCustomFilter
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
            System.Windows.Forms.Label labelNoFilters;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbReportCustomFilter));
            this.comboFilterType = new System.Windows.Forms.ComboBox();
            this.comboBoxTags = new System.Windows.Forms.ComboBox();
            this.labelFilter = new System.Windows.Forms.Label();
            this.labelHost = new System.Windows.Forms.Label();
            this.labelTags = new System.Windows.Forms.Label();
            this.panelFilterType = new System.Windows.Forms.Panel();
            this.panelTags = new System.Windows.Forms.Panel();
            this.panelListView = new System.Windows.Forms.Panel();
            this.listViewFilterItem = new XenAdmin.Controls.ListViewEx();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.checkBoxCheckAll = new System.Windows.Forms.CheckBox();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            labelNoFilters = new System.Windows.Forms.Label();
            this.panelFilterType.SuspendLayout();
            this.panelTags.SuspendLayout();
            this.panelListView.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelNoFilters
            // 
            resources.ApplyResources(labelNoFilters, "labelNoFilters");
            labelNoFilters.BackColor = System.Drawing.SystemColors.Window;
            labelNoFilters.Name = "labelNoFilters";
            // 
            // comboFilterType
            // 
            resources.ApplyResources(this.comboFilterType, "comboFilterType");
            this.comboFilterType.FormattingEnabled = true;
            this.comboFilterType.Items.AddRange(new object[] {
            resources.GetString("comboFilterType.Items"),
            resources.GetString("comboFilterType.Items1"),
            resources.GetString("comboFilterType.Items2")});
            this.comboFilterType.Name = "comboFilterType";
            this.comboFilterType.SelectedIndexChanged += new System.EventHandler(this.comboFilterType_SelectedIndexChanged);
            // 
            // comboBoxTags
            // 
            this.comboBoxTags.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxTags, "comboBoxTags");
            this.comboBoxTags.FormattingEnabled = true;
            this.comboBoxTags.Name = "comboBoxTags";
            // 
            // labelFilter
            // 
            resources.ApplyResources(this.labelFilter, "labelFilter");
            this.labelFilter.Name = "labelFilter";
            // 
            // labelHost
            // 
            resources.ApplyResources(this.labelHost, "labelHost");
            this.labelHost.Name = "labelHost";
            // 
            // labelTags
            // 
            resources.ApplyResources(this.labelTags, "labelTags");
            this.labelTags.Name = "labelTags";
            // 
            // panelFilterType
            // 
            resources.ApplyResources(this.panelFilterType, "panelFilterType");
            this.panelFilterType.Controls.Add(this.comboFilterType);
            this.panelFilterType.Controls.Add(this.labelFilter);
            this.panelFilterType.Name = "panelFilterType";
            // 
            // panelTags
            // 
            resources.ApplyResources(this.panelTags, "panelTags");
            this.panelTags.Controls.Add(this.labelHost);
            this.panelTags.Controls.Add(this.labelTags);
            this.panelTags.Controls.Add(this.comboBoxTags);
            this.panelTags.Name = "panelTags";
            // 
            // panelListView
            // 
            this.panelListView.Controls.Add(this.listViewFilterItem);
            this.panelListView.Controls.Add(labelNoFilters);
            this.panelListView.Controls.Add(this.checkBoxCheckAll);
            resources.ApplyResources(this.panelListView, "panelListView");
            this.panelListView.Name = "panelListView";
            // 
            // listViewFilterItem
            // 
            this.listViewFilterItem.CheckBoxes = true;
            this.listViewFilterItem.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            resources.ApplyResources(this.listViewFilterItem, "listViewFilterItem");
            this.listViewFilterItem.Name = "listViewFilterItem";
            this.listViewFilterItem.OwnerDraw = true;
            this.listViewFilterItem.UseCompatibleStateImageBehavior = false;
            this.listViewFilterItem.View = System.Windows.Forms.View.Details;
            this.listViewFilterItem.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listViewFilterItem_DrawColumnHeader);
            this.listViewFilterItem.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listViewFilterItem_DrawItem);
            this.listViewFilterItem.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listViewFilterItem_DrawSubItem);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // checkBoxCheckAll
            // 
            resources.ApplyResources(this.checkBoxCheckAll, "checkBoxCheckAll");
            this.checkBoxCheckAll.Name = "checkBoxCheckAll";
            this.checkBoxCheckAll.UseVisualStyleBackColor = true;
            this.checkBoxCheckAll.CheckedChanged += new System.EventHandler(this.checkBoxCheckAll_CheckedChanged);
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.buttonOK);
            this.panelButtons.Controls.Add(this.buttonCancel);
            resources.ApplyResources(this.panelButtons, "panelButtons");
            this.panelButtons.Name = "panelButtons";
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.panelFilterType);
            this.flowLayoutPanel1.Controls.Add(this.panelTags);
            this.flowLayoutPanel1.Controls.Add(this.panelListView);
            this.flowLayoutPanel1.Controls.Add(this.panelButtons);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // WlbReportCustomFilter
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.flowLayoutPanel1);
            this.HelpButton = false;
            this.Name = "WlbReportCustomFilter";
            this.panelFilterType.ResumeLayout(false);
            this.panelTags.ResumeLayout(false);
            this.panelTags.PerformLayout();
            this.panelListView.ResumeLayout(false);
            this.panelListView.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panelFilterType;
        private System.Windows.Forms.Label labelFilter;
        private System.Windows.Forms.Panel panelTags;
        private System.Windows.Forms.Label labelTags;
        private System.Windows.Forms.ComboBox comboBoxTags;
        private System.Windows.Forms.Panel panelListView;
        private XenAdmin.Controls.ListViewEx listViewFilterItem;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.CheckBox checkBoxCheckAll;
        private System.Windows.Forms.Label labelHost;
        public System.Windows.Forms.ComboBox comboFilterType;
    }
}
