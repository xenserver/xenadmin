namespace XenAdmin.Dialogs
{
    partial class AcceptEuaDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AcceptEuaDialog));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.infoLabel = new System.Windows.Forms.Label();
            this.warningTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.warningPictureBox = new System.Windows.Forms.PictureBox();
            this.warningLabel = new System.Windows.Forms.Label();
            this.euaPanel = new System.Windows.Forms.Panel();
            this.euaTextBox = new System.Windows.Forms.RichTextBox();
            this.buttonsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.declineButton = new System.Windows.Forms.Button();
            this.acceptButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.warningTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningPictureBox)).BeginInit();
            this.euaPanel.SuspendLayout();
            this.buttonsFlowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel.Controls.Add(this.infoLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.warningTableLayoutPanel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.euaPanel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonsFlowLayoutPanel, 0, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // infoLabel
            // 
            resources.ApplyResources(this.infoLabel, "infoLabel");
            this.infoLabel.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel.SetColumnSpan(this.infoLabel, 2);
            this.infoLabel.Name = "infoLabel";
            // 
            // warningTableLayoutPanel
            // 
            resources.ApplyResources(this.warningTableLayoutPanel, "warningTableLayoutPanel");
            this.warningTableLayoutPanel.Controls.Add(this.warningPictureBox, 0, 0);
            this.warningTableLayoutPanel.Controls.Add(this.warningLabel, 1, 0);
            this.warningTableLayoutPanel.Name = "warningTableLayoutPanel";
            // 
            // warningPictureBox
            // 
            resources.ApplyResources(this.warningPictureBox, "warningPictureBox");
            this.warningPictureBox.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            this.warningPictureBox.Name = "warningPictureBox";
            this.warningPictureBox.TabStop = false;
            // 
            // warningLabel
            // 
            resources.ApplyResources(this.warningLabel, "warningLabel");
            this.warningLabel.Name = "warningLabel";
            // 
            // euaPanel
            // 
            this.euaPanel.Controls.Add(this.euaTextBox);
            resources.ApplyResources(this.euaPanel, "euaPanel");
            this.euaPanel.Name = "euaPanel";
            // 
            // euaTextBox
            // 
            this.euaTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.euaTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.euaTextBox, "euaTextBox");
            this.euaTextBox.Name = "euaTextBox";
            this.euaTextBox.ReadOnly = true;
            // 
            // buttonsFlowLayoutPanel
            // 
            this.buttonsFlowLayoutPanel.Controls.Add(this.declineButton);
            this.buttonsFlowLayoutPanel.Controls.Add(this.acceptButton);
            resources.ApplyResources(this.buttonsFlowLayoutPanel, "buttonsFlowLayoutPanel");
            this.buttonsFlowLayoutPanel.Name = "buttonsFlowLayoutPanel";
            // 
            // declineButton
            // 
            resources.ApplyResources(this.declineButton, "declineButton");
            this.declineButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.declineButton.Name = "declineButton";
            this.declineButton.UseVisualStyleBackColor = true;
            // 
            // acceptButton
            // 
            resources.ApplyResources(this.acceptButton, "acceptButton");
            this.acceptButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.UseVisualStyleBackColor = true;
            // 
            // AcceptEuaDialog
            // 
            this.AcceptButton = this.declineButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.declineButton;
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.HelpButton = false;
            this.Name = "AcceptEuaDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.warningTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.warningPictureBox)).EndInit();
            this.euaPanel.ResumeLayout(false);
            this.buttonsFlowLayoutPanel.ResumeLayout(false);
            this.buttonsFlowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.TableLayoutPanel warningTableLayoutPanel;
        private System.Windows.Forms.PictureBox warningPictureBox;
        private System.Windows.Forms.Label warningLabel;
        private System.Windows.Forms.Panel euaPanel;
        private System.Windows.Forms.RichTextBox euaTextBox;
        private System.Windows.Forms.FlowLayoutPanel buttonsFlowLayoutPanel;
        private System.Windows.Forms.Button declineButton;
        private System.Windows.Forms.Button acceptButton;
    }
}
