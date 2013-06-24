namespace XenAdmin
{
    partial class HistoryWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryWindow));
            this.SuspendLayout();
            // 
            // HistoryWindow
            // 
            resources.ApplyResources(this, "$this");
            this.DoubleBuffered = true;
            this.Name = "HistoryWindow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HistoryWindow_FormClosed);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.HistoryWindow_HelpRequested);
            this.Load += new System.EventHandler(this.HistoryWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }
}