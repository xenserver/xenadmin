namespace XenAdmin.Dialogs
{
    partial class ConfirmVMDeleteDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfirmVMDeleteDialog));
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.AlertLabel = new System.Windows.Forms.Label();
            this.listView = new XenAdmin.Dialogs.ConfirmVMDeleteDialog.ListViewDeleteDialog();
            this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderVM = new System.Windows.Forms.ColumnHeader();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnYes
            // 
            resources.ApplyResources(this.btnYes, "btnYes");
            this.btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnYes.Name = "btnYes";
            this.btnYes.UseVisualStyleBackColor = true;
            // 
            // btnNo
            // 
            resources.ApplyResources(this.btnNo, "btnNo");
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNo.Name = "btnNo";
            this.btnNo.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // AlertLabel
            // 
            resources.ApplyResources(this.AlertLabel, "AlertLabel");
            this.AlertLabel.AutoEllipsis = true;
            this.AlertLabel.Name = "AlertLabel";
            this.AlertLabel.UseMnemonic = false;
            // 
            // listView
            // 
            resources.ApplyResources(this.listView, "listView");
            this.listView.CheckBoxes = true;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderVM});
            this.listView.FullRowSelect = true;
            this.listView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listView.Groups"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listView.Groups1")))});
            this.listView.Name = "listView";
            this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView_ItemChecked);
            this.listView.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.listView_ColumnWidthChanged);
            this.listView.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.listView_ColumnWidthChanging);
            // 
            // columnHeaderName
            // 
            resources.ApplyResources(this.columnHeaderName, "columnHeaderName");
            // 
            // columnHeaderVM
            // 
            resources.ApplyResources(this.columnHeaderVM, "columnHeaderVM");
            // 
            // buttonSelectAll
            // 
            resources.ApplyResources(this.buttonSelectAll, "buttonSelectAll");
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
            // 
            // buttonClear
            // 
            resources.ApplyResources(this.buttonClear, "buttonClear");
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ConfirmVMDeleteDialog
            // 
            this.AcceptButton = this.btnYes;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnNo;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonSelectAll);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.AlertLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.Name = "ConfirmVMDeleteDialog";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label AlertLabel;
        private ListViewDeleteDialog listView;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderVM;
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Label label1;
    }
}
