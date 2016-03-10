namespace XenAdmin.Controls.XenSearch
{
    partial class SearchFor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchFor));
            this.searchForComboButton = new XenAdmin.Controls.XenSearch.DropDownComboButton();
            this.SuspendLayout();
            // 
            // searchForComboButton
            // 
            resources.ApplyResources(this.searchForComboButton, "searchForComboButton");
            this.searchForComboButton.Name = "searchForComboButton";
            this.searchForComboButton.SelectedItem = null;
            this.searchForComboButton.UseVisualStyleBackColor = true;
            this.searchForComboButton.SelectedItemChanged += new System.EventHandler(this.searchForComboButton_selChanged);
            this.searchForComboButton.ItemSelected += new System.EventHandler(this.searchForComboButton_itemSelected);
            this.searchForComboButton.BeforePopup += new System.EventHandler(this.searchForComboButton_BeforePopup);
            // 
            // SearchFor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.searchForComboButton);
            this.Name = "SearchFor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal DropDownComboButton searchForComboButton;
    }
}