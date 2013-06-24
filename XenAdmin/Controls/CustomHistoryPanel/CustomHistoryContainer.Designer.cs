using System.Windows.Forms;

namespace XenAdmin.Controls
{
    partial class CustomHistoryContainer
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

            CustomHistoryPanel.Rows.CollectionChanged -= Rows_CollectionChanged;
            // Destroy any remaining timers
            foreach (Timer timer in refreshTimers.ToArray())
            {
                timer.Dispose();
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
            this.CustomHistoryPanel = new XenAdmin.Controls.CustomHistoryPanel();
            this.SuspendLayout();
            // 
            // CustomHistoryPanel
            // 
            this.CustomHistoryPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CustomHistoryPanel.Location = new System.Drawing.Point(0, 0);
            this.CustomHistoryPanel.Name = "CustomHistoryPanel";
            this.CustomHistoryPanel.Size = new System.Drawing.Size(200, 1);
            this.CustomHistoryPanel.TabIndex = 0;
            // 
            // CustomHistoryContainer
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.CustomHistoryPanel);
            this.ResumeLayout(false);

        }

        #endregion

        public CustomHistoryPanel CustomHistoryPanel;

    }
}
