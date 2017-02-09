namespace XenAdmin.Controls
{
    partial class SrPicker
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
            UnregisterHandlers();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SrPicker));
			this.srListBox = new XenAdmin.Controls.CustomTreeView();
			this.SrHint = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// srListBox
			// 
			resources.ApplyResources(this.srListBox, "srListBox");
			this.srListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.srListBox.FormattingEnabled = true;
			this.srListBox.Name = "srListBox";
			this.srListBox.NodeIndent = 19;
			this.srListBox.RootAlwaysExpanded = false;
			this.srListBox.ShowCheckboxes = true;
			this.srListBox.ShowDescription = true;
			this.srListBox.ShowImages = false;
			this.srListBox.ShowRootLines = true;
			// 
			// SrHint
			// 
			resources.ApplyResources(this.SrHint, "SrHint");
			this.SrHint.ForeColor = System.Drawing.SystemColors.WindowText;
			this.SrHint.Name = "SrHint";
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.SrHint, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.srListBox, 0, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// SrPicker
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.tableLayoutPanel1);
			this.DoubleBuffered = true;
			this.Name = "SrPicker";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        public XenAdmin.Controls.CustomTreeView srListBox;
        public System.Windows.Forms.Label SrHint;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
