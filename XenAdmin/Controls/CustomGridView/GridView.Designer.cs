namespace XenAdmin.Controls.CustomGridView
{
    partial class GridView
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
            DisposeBuffer();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MoreInfoToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // GridView
            // 
            this.AllowDrop = true;
            this.AutoScroll = true;
            this.MinimumSize = new System.Drawing.Size(1, 1);
            this.Padding = new System.Windows.Forms.Padding(5);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip MoreInfoToolTip;
    }
}
