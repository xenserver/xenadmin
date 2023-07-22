namespace XenAdmin.Dialogs.OptionsPages
{
    partial class UpdatesOptionsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdatesOptionsPage));
            this.UpdatesTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._checkBoxClientUpdates = new System.Windows.Forms.CheckBox();
            this.labelClientUpdates = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.labelInfoCdn = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkLabelCongifUpdates = new System.Windows.Forms.LinkLabel();
            this.UpdatesTableLayoutPanel.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // UpdatesTableLayoutPanel
            // 
            resources.ApplyResources(this.UpdatesTableLayoutPanel, "UpdatesTableLayoutPanel");
            this.UpdatesTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.UpdatesTableLayoutPanel.Controls.Add(this._checkBoxClientUpdates, 0, 1);
            this.UpdatesTableLayoutPanel.Controls.Add(this.labelClientUpdates, 0, 0);
            this.UpdatesTableLayoutPanel.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.UpdatesTableLayoutPanel.Name = "UpdatesTableLayoutPanel";
            // 
            // _checkBoxClientUpdates
            // 
            resources.ApplyResources(this._checkBoxClientUpdates, "_checkBoxClientUpdates");
            this._checkBoxClientUpdates.Name = "_checkBoxClientUpdates";
            this._checkBoxClientUpdates.UseVisualStyleBackColor = true;
            // 
            // labelClientUpdates
            // 
            resources.ApplyResources(this.labelClientUpdates, "labelClientUpdates");
            this.labelClientUpdates.Name = "labelClientUpdates";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.labelInfoCdn, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.linkLabelCongifUpdates, 2, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // labelInfoCdn
            // 
            resources.ApplyResources(this.labelInfoCdn, "labelInfoCdn");
            this.labelInfoCdn.Name = "labelInfoCdn";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // linkLabelCongifUpdates
            // 
            resources.ApplyResources(this.linkLabelCongifUpdates, "linkLabelCongifUpdates");
            this.linkLabelCongifUpdates.Name = "linkLabelCongifUpdates";
            this.linkLabelCongifUpdates.TabStop = true;
            this.linkLabelCongifUpdates.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCongifUpdates_LinkClicked);
            // 
            // UpdatesOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.UpdatesTableLayoutPanel);
            this.Name = "UpdatesOptionsPage";
            this.UpdatesTableLayoutPanel.ResumeLayout(false);
            this.UpdatesTableLayoutPanel.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel UpdatesTableLayoutPanel;
        private System.Windows.Forms.Label labelInfoCdn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox _checkBoxClientUpdates;
        private System.Windows.Forms.Label labelClientUpdates;
        private System.Windows.Forms.LinkLabel linkLabelCongifUpdates;
    }
}
