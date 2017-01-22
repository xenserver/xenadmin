namespace XenAdmin.Dialogs.OptionsPages
{
    partial class ConfirmationOptionsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfirmationOptionsPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelBlurb = new System.Windows.Forms.Label();
            this.checkBoxDontConfirmDismissAlerts = new System.Windows.Forms.CheckBox();
            this.checkBoxDontConfirmDismissUpdates = new System.Windows.Forms.CheckBox();
            this.checkBoxDontConfirmDismissEvents = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.Controls.Add(this.labelBlurb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxDontConfirmDismissAlerts, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxDontConfirmDismissUpdates, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxDontConfirmDismissEvents, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelBlurb
            // 
            resources.ApplyResources(this.labelBlurb, "labelBlurb");
            this.labelBlurb.Name = "labelBlurb";
            // 
            // checkBoxDontConfirmDismissAlerts
            // 
            resources.ApplyResources(this.checkBoxDontConfirmDismissAlerts, "checkBoxDontConfirmDismissAlerts");
            this.checkBoxDontConfirmDismissAlerts.Name = "checkBoxDontConfirmDismissAlerts";
            this.checkBoxDontConfirmDismissAlerts.UseVisualStyleBackColor = true;
            // 
            // checkBoxDontConfirmDismissUpdates
            // 
            resources.ApplyResources(this.checkBoxDontConfirmDismissUpdates, "checkBoxDontConfirmDismissUpdates");
            this.checkBoxDontConfirmDismissUpdates.Name = "checkBoxDontConfirmDismissUpdates";
            this.checkBoxDontConfirmDismissUpdates.UseVisualStyleBackColor = true;
            // 
            // checkBoxDontConfirmDismissEvents
            // 
            resources.ApplyResources(this.checkBoxDontConfirmDismissEvents, "checkBoxDontConfirmDismissEvents");
            this.checkBoxDontConfirmDismissEvents.Name = "checkBoxDontConfirmDismissEvents";
            this.checkBoxDontConfirmDismissEvents.UseVisualStyleBackColor = true;
            // 
            // ConfirmationOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ConfirmationOptionsPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelBlurb;
        private System.Windows.Forms.CheckBox checkBoxDontConfirmDismissAlerts;
        private System.Windows.Forms.CheckBox checkBoxDontConfirmDismissEvents;
        private System.Windows.Forms.CheckBox checkBoxDontConfirmDismissUpdates;
    }
}
