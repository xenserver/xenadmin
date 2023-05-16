namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    partial class RollingUpgradeExtrasPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RollingUpgradeExtrasPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxInstallSuppPack = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.applyUpdatesLabel = new System.Windows.Forms.Label();
            this.applyUpdatesCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxInstallSuppPack, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.fileNameTextBox, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.BrowseButton, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.applyUpdatesLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.applyUpdatesCheckBox, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // BrowseButton
            // 
            resources.ApplyResources(this.BrowseButton, "BrowseButton");
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // fileNameTextBox
            // 
            resources.ApplyResources(this.fileNameTextBox, "fileNameTextBox");
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.TextChanged += new System.EventHandler(this.fileNameTextBox_TextChanged);
            this.fileNameTextBox.Enter += new System.EventHandler(this.fileNameTextBox_Enter);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.tableLayoutPanel1.SetColumnSpan(this.label4, 3);
            this.label4.Name = "label4";
            // 
            // checkBoxInstallSuppPack
            // 
            resources.ApplyResources(this.checkBoxInstallSuppPack, "checkBoxInstallSuppPack");
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxInstallSuppPack, 3);
            this.checkBoxInstallSuppPack.Name = "checkBoxInstallSuppPack";
            this.checkBoxInstallSuppPack.UseVisualStyleBackColor = true;
            this.checkBoxInstallSuppPack.CheckedChanged += new System.EventHandler(this.checkBoxInstallSuppPack_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 3);
            this.label2.Name = "label2";
            // 
            // applyUpdatesLabel
            // 
            resources.ApplyResources(this.applyUpdatesLabel, "applyUpdatesLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.applyUpdatesLabel, 3);
            this.applyUpdatesLabel.Name = "applyUpdatesLabel";
            // 
            // applyUpdatesCheckBox
            // 
            resources.ApplyResources(this.applyUpdatesCheckBox, "applyUpdatesCheckBox");
            this.tableLayoutPanel1.SetColumnSpan(this.applyUpdatesCheckBox, 3);
            this.applyUpdatesCheckBox.Name = "applyUpdatesCheckBox";
            this.applyUpdatesCheckBox.UseVisualStyleBackColor = true;
            // 
            // RollingUpgradeExtrasPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RollingUpgradeExtrasPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBoxInstallSuppPack;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label applyUpdatesLabel;
        private System.Windows.Forms.CheckBox applyUpdatesCheckBox;
    }
}
