namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class CifsFrontend
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CifsFrontend));
            this.CifsScanButton = new System.Windows.Forms.Button();
            this.listBoxCifsSRs = new XenAdmin.Controls.SRListBox();
            this.radioButtonCifsReattach = new System.Windows.Forms.RadioButton();
            this.radioButtonCifsNew = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.labelAdvancedOptions = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CifsServerPathTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CifsScanButton
            // 
            resources.ApplyResources(this.CifsScanButton, "CifsScanButton");
            this.CifsScanButton.Name = "CifsScanButton";
            this.CifsScanButton.UseVisualStyleBackColor = true;
            this.CifsScanButton.Click += new System.EventHandler(this.buttonCifsScan_Click);
            // 
            // listBoxCifsSRs
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.listBoxCifsSRs, 3);
            resources.ApplyResources(this.listBoxCifsSRs, "listBoxCifsSRs");
            this.listBoxCifsSRs.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxCifsSRs.Name = "listBoxCifsSRs";
            this.listBoxCifsSRs.Sorted = true;
            this.listBoxCifsSRs.SelectedIndexChanged += new System.EventHandler(this.listBoxCifsSRs_SelectedIndexChanged);
            // 
            // radioButtonCifsReattach
            // 
            resources.ApplyResources(this.radioButtonCifsReattach, "radioButtonCifsReattach");
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonCifsReattach, 3);
            this.radioButtonCifsReattach.Name = "radioButtonCifsReattach";
            this.radioButtonCifsReattach.UseVisualStyleBackColor = true;
            this.radioButtonCifsReattach.CheckedChanged += new System.EventHandler(this.radioButtonCifsReattach_CheckedChanged);
            // 
            // radioButtonCifsNew
            // 
            resources.ApplyResources(this.radioButtonCifsNew, "radioButtonCifsNew");
            this.radioButtonCifsNew.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonCifsNew, 3);
            this.radioButtonCifsNew.Name = "radioButtonCifsNew";
            this.radioButtonCifsNew.TabStop = true;
            this.radioButtonCifsNew.UseVisualStyleBackColor = true;
            this.radioButtonCifsNew.CheckedChanged += new System.EventHandler(this.radioButtonCifsNew_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label2.Name = "label2";
            // 
            // labelAdvancedOptions
            // 
            resources.ApplyResources(this.labelAdvancedOptions, "labelAdvancedOptions");
            this.labelAdvancedOptions.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelAdvancedOptions.Name = "labelAdvancedOptions";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label3.Name = "label3";
            // 
            // CifsServerPathTextBox
            // 
            resources.ApplyResources(this.CifsServerPathTextBox, "CifsServerPathTextBox");
            this.CifsServerPathTextBox.Name = "CifsServerPathTextBox";
            this.CifsServerPathTextBox.TextChanged += new System.EventHandler(this.AnyCifsParameters_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.passwordTextBox, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.CifsServerPathTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.CifsScanButton, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelAdvancedOptions, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.userNameTextBox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonCifsNew, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonCifsReattach, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.listBoxCifsSRs, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // passwordTextBox
            // 
            resources.ApplyResources(this.passwordTextBox, "passwordTextBox");
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.UseSystemPasswordChar = true;
            this.passwordTextBox.TextChanged += new System.EventHandler(this.AnyCifsParameters_TextChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label4.Name = "label4";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // userNameTextBox
            // 
            resources.ApplyResources(this.userNameTextBox, "userNameTextBox");
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.TextChanged += new System.EventHandler(this.AnyCifsParameters_TextChanged);
            // 
            // CifsFrontend
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CifsFrontend";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CifsScanButton;
        private System.Windows.Forms.RadioButton radioButtonCifsReattach;
        private XenAdmin.Controls.SRListBox listBoxCifsSRs;
        private System.Windows.Forms.RadioButton radioButtonCifsNew;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelAdvancedOptions;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox CifsServerPathTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox userNameTextBox;
    }
}
