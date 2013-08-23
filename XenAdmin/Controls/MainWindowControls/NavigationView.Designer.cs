namespace XenAdmin.Controls.MainWindowControls
{
    partial class NavigationView
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
            this.searchTextBox = new XenAdmin.Controls.SearchTextBox();
            this.SuspendLayout();
            // 
            // searchTextBox
            // 
            this.searchTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchTextBox.Location = new System.Drawing.Point(0, 0);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(210, 23);
            this.searchTextBox.TabIndex = 3;
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // NavigationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.searchTextBox);
            this.Name = "NavigationView";
            this.Size = new System.Drawing.Size(210, 288);
            this.ResumeLayout(false);

        }

        #endregion

        private SearchTextBox searchTextBox;
    }
}
