namespace XenAdmin.SettingsPanels
{
    partial class NetworkOptionsEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkOptionsEditPage));
            this.radioButtonDisable = new System.Windows.Forms.RadioButton();
            this.radioButtonEnable = new System.Windows.Forms.RadioButton();
            this.labelRubric = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.iconWarningIGMPSnoopingOption = new System.Windows.Forms.PictureBox();
            this.labelWarningIGMPSnoopingOption = new System.Windows.Forms.Label();
            this.warningsTable = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.iconWarningIGMPSnoopingOption)).BeginInit();
            this.warningsTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonDisable
            // 
            resources.ApplyResources(this.radioButtonDisable, "radioButtonDisable");
            this.warningsTable.SetColumnSpan(this.radioButtonDisable, 2);
            this.radioButtonDisable.Name = "radioButtonDisable";
            this.radioButtonDisable.TabStop = true;
            this.radioButtonDisable.UseVisualStyleBackColor = true;
            // 
            // radioButtonEnable
            // 
            resources.ApplyResources(this.radioButtonEnable, "radioButtonEnable");
            this.warningsTable.SetColumnSpan(this.radioButtonEnable, 2);
            this.radioButtonEnable.Name = "radioButtonEnable";
            this.radioButtonEnable.TabStop = true;
            this.radioButtonEnable.UseVisualStyleBackColor = true;
            // 
            // labelRubric
            // 
            resources.ApplyResources(this.labelRubric, "labelRubric");
            this.labelRubric.BackColor = System.Drawing.Color.Transparent;
            this.warningsTable.SetColumnSpan(this.labelRubric, 2);
            this.labelRubric.Name = "labelRubric";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // iconWarningIGMPSnoopingOption
            // 
            resources.ApplyResources(this.iconWarningIGMPSnoopingOption, "iconWarningIGMPSnoopingOption");
            this.iconWarningIGMPSnoopingOption.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.iconWarningIGMPSnoopingOption.Name = "iconWarningIGMPSnoopingOption";
            this.iconWarningIGMPSnoopingOption.TabStop = false;
            // 
            // labelWarningIGMPSnoopingOption
            // 
            resources.ApplyResources(this.labelWarningIGMPSnoopingOption, "labelWarningIGMPSnoopingOption");
            this.labelWarningIGMPSnoopingOption.Name = "labelWarningIGMPSnoopingOption";
            // 
            // warningsTable
            // 
            this.warningsTable.AllowDrop = true;
            resources.ApplyResources(this.warningsTable, "warningsTable");
            this.warningsTable.Controls.Add(this.labelRubric, 0, 0);
            this.warningsTable.Controls.Add(this.iconWarningIGMPSnoopingOption, 0, 3);
            this.warningsTable.Controls.Add(this.labelWarningIGMPSnoopingOption, 1, 3);
            this.warningsTable.Controls.Add(this.radioButtonEnable, 0, 1);
            this.warningsTable.Controls.Add(this.radioButtonDisable, 0, 2);
            this.warningsTable.Name = "warningsTable";
            // 
            // NetworkOptionsEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.warningsTable);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NetworkOptionsEditPage";
            ((System.ComponentModel.ISupportInitialize)(this.iconWarningIGMPSnoopingOption)).EndInit();
            this.warningsTable.ResumeLayout(false);
            this.warningsTable.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonDisable;
        private System.Windows.Forms.RadioButton radioButtonEnable;
        private System.Windows.Forms.Label labelRubric;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox iconWarningIGMPSnoopingOption;
        private System.Windows.Forms.Label labelWarningIGMPSnoopingOption;
        private System.Windows.Forms.TableLayoutPanel warningsTable;
    }
}
