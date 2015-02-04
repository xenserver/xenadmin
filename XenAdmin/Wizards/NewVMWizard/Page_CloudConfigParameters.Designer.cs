namespace XenAdmin.Wizards.NewVMWizard
{
    partial class Page_CloudConfigParameters
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_CloudConfigParameters));
            this.ConfigDriveTemplateTextBox = new System.Windows.Forms.TextBox();
            this.ConfigDriveTemplateLabel = new System.Windows.Forms.Label();
            this.topLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.IncludeConfigDriveCheckBox = new System.Windows.Forms.CheckBox();
            this.reloadDefaults = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConfigDriveTemplateTextBox
            // 
            this.ConfigDriveTemplateTextBox.AcceptsReturn = true;
            resources.ApplyResources(this.ConfigDriveTemplateTextBox, "ConfigDriveTemplateTextBox");
            this.ConfigDriveTemplateTextBox.Name = "ConfigDriveTemplateTextBox";
            this.tableLayoutPanel1.SetRowSpan(this.ConfigDriveTemplateTextBox, 2);
            // 
            // ConfigDriveTemplateLabel
            // 
            resources.ApplyResources(this.ConfigDriveTemplateLabel, "ConfigDriveTemplateLabel");
            this.ConfigDriveTemplateLabel.Name = "ConfigDriveTemplateLabel";
            // 
            // topLabel
            // 
            resources.ApplyResources(this.topLabel, "topLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.topLabel, 2);
            this.topLabel.Name = "topLabel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.topLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ConfigDriveTemplateTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.ConfigDriveTemplateLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.IncludeConfigDriveCheckBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.reloadDefaults, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // IncludeConfigDriveCheckBox
            // 
            resources.ApplyResources(this.IncludeConfigDriveCheckBox, "IncludeConfigDriveCheckBox");
            this.IncludeConfigDriveCheckBox.Checked = true;
            this.IncludeConfigDriveCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel1.SetColumnSpan(this.IncludeConfigDriveCheckBox, 2);
            this.IncludeConfigDriveCheckBox.Name = "IncludeConfigDriveCheckBox";
            this.IncludeConfigDriveCheckBox.UseVisualStyleBackColor = true;
            this.IncludeConfigDriveCheckBox.CheckedChanged += new System.EventHandler(this.IncludeConfigDriveCheckBox_CheckedChanged);
            // 
            // reloadDefaults
            // 
            resources.ApplyResources(this.reloadDefaults, "reloadDefaults");
            this.reloadDefaults.Name = "reloadDefaults";
            this.reloadDefaults.UseVisualStyleBackColor = true;
            this.reloadDefaults.Click += new System.EventHandler(this.reloadDefaults_Click);
            // 
            // Page_CloudConfigParameters
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Page_CloudConfigParameters";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox ConfigDriveTemplateTextBox;
        private System.Windows.Forms.Label ConfigDriveTemplateLabel;
        private System.Windows.Forms.Label topLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox IncludeConfigDriveCheckBox;
        private System.Windows.Forms.Button reloadDefaults;
    }
}
