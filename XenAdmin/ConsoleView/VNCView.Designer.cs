namespace XenAdmin.ConsoleView
{
    partial class VNCView
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
            Program.AssertOnEventThread();

            UnregisterEventListeners();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            if (this.undockedForm != null)
            {
                undockedForm.Hide();
                undockedForm.Dispose();
            }

            if (disposing && this.vncTabView != null)
            {
                this.vncTabView.Dispose();
            }
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VNCView));
            this.findConsoleButton = new System.Windows.Forms.Button();
            this.reattachConsoleButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // findConsoleButton
            // 
            resources.ApplyResources(this.findConsoleButton, "findConsoleButton");
            this.findConsoleButton.Name = "findConsoleButton";
            this.findConsoleButton.UseVisualStyleBackColor = true;
            this.findConsoleButton.Click += new System.EventHandler(this.findConsoleButton_Click);
            // 
            // reattachConsoleButton
            // 
            resources.ApplyResources(this.reattachConsoleButton, "reattachConsoleButton");
            this.reattachConsoleButton.Name = "reattachConsoleButton";
            this.reattachConsoleButton.UseVisualStyleBackColor = true;
            this.reattachConsoleButton.Click += new System.EventHandler(this.reattachConsoleButton_Click);
            // 
            // VNCView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.reattachConsoleButton);
            this.Controls.Add(this.findConsoleButton);
            this.Name = "VNCView";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button findConsoleButton;
        private System.Windows.Forms.Button reattachConsoleButton;
    }
}
