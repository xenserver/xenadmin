namespace XenAdmin.Controls
{
    partial class TreeSearchBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeSearchBox));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelViews = new System.Windows.Forms.Label();
            this.comboButtonViews = new XenAdmin.Controls.XenSearch.DropDownComboButton();
            this.searchTextBox = new XenAdmin.Controls.SearchTextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelViews, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboButtonViews, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelViews
            // 
            resources.ApplyResources(this.labelViews, "labelViews");
            this.labelViews.Name = "labelViews";
            // 
            // comboButtonViews
            // 
            resources.ApplyResources(this.comboButtonViews, "comboButtonViews");
            this.comboButtonViews.Name = "comboButtonViews";
            this.comboButtonViews.SelectedItem = null;
            this.comboButtonViews.UseVisualStyleBackColor = true;
            this.comboButtonViews.SelectedItemChanged += new System.EventHandler(this.comboButtonViews_SelectedItemChanged);
            // 
            // searchTextBox
            // 
            resources.ApplyResources(this.searchTextBox, "searchTextBox");
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // TreeSearchBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.searchTextBox);
            this.Name = "TreeSearchBox";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SearchTextBox searchTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelViews;
        private XenAdmin.Controls.XenSearch.DropDownComboButton comboButtonViews;
    }
}
