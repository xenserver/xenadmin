namespace XenAdmin.Controls.ConsoleTab
{
    partial class FullScreenForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FullScreenForm));
            this.connectionBar1 = new XenAdmin.Controls.ConsoleTab.ConnectionBar();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // connectionBar1
            // 
            resources.ApplyResources(this.connectionBar1, "connectionBar1");
            this.connectionBar1.BackColor = System.Drawing.Color.Transparent;
            this.connectionBar1.Name = "connectionBar1";
            this.connectionBar1.TabStop = false;
            // 
            // contentPanel
            // 
            resources.ApplyResources(this.contentPanel, "contentPanel");
            this.contentPanel.BackColor = System.Drawing.SystemColors.Control;
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.TabStop = true;
            // 
            // FullScreenForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.connectionBar1);
            this.Controls.Add(this.contentPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FullScreenForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        private ConnectionBar connectionBar1;
        private System.Windows.Forms.Panel contentPanel;
    }
}

