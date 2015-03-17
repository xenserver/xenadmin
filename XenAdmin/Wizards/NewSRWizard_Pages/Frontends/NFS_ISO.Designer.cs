namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class NFS_ISO
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NFS_ISO));
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.nfsVersionTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.nfsVersion3RadioButton = new System.Windows.Forms.RadioButton();
            this.nfsVersion4RadioButton = new System.Windows.Forms.RadioButton();
            this.NfsServerPathComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.passwordFailure1 = new XenAdmin.Controls.Common.PasswordFailure();
            this.nfsVersionLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.nfsVersionTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label3.Name = "label3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.nfsVersionTableLayoutPanel, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.NfsServerPathComboBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.passwordFailure1, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.nfsVersionLabel, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // nfsVersionTableLayoutPanel
            // 
            resources.ApplyResources(this.nfsVersionTableLayoutPanel, "nfsVersionTableLayoutPanel");
            this.nfsVersionTableLayoutPanel.Controls.Add(this.nfsVersion3RadioButton, 0, 0);
            this.nfsVersionTableLayoutPanel.Controls.Add(this.nfsVersion4RadioButton, 0, 1);
            this.nfsVersionTableLayoutPanel.Name = "nfsVersionTableLayoutPanel";
            // 
            // nfsVersion3RadioButton
            // 
            resources.ApplyResources(this.nfsVersion3RadioButton, "nfsVersion3RadioButton");
            this.nfsVersion3RadioButton.Checked = true;
            this.nfsVersion3RadioButton.Name = "nfsVersion3RadioButton";
            this.nfsVersion3RadioButton.TabStop = true;
            this.nfsVersion3RadioButton.UseVisualStyleBackColor = true;
            // 
            // nfsVersion4RadioButton
            // 
            resources.ApplyResources(this.nfsVersion4RadioButton, "nfsVersion4RadioButton");
            this.nfsVersion4RadioButton.Name = "nfsVersion4RadioButton";
            this.nfsVersion4RadioButton.UseVisualStyleBackColor = true;
            // 
            // NfsServerPathComboBox
            // 
            resources.ApplyResources(this.NfsServerPathComboBox, "NfsServerPathComboBox");
            this.NfsServerPathComboBox.FormattingEnabled = true;
            this.NfsServerPathComboBox.Name = "NfsServerPathComboBox";
            this.NfsServerPathComboBox.SelectedIndexChanged += new System.EventHandler(this.NfsServerPathTextBox_TextChanged);
            this.NfsServerPathComboBox.TextChanged += new System.EventHandler(this.NfsServerPathTextBox_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // passwordFailure1
            // 
            resources.ApplyResources(this.passwordFailure1, "passwordFailure1");
            this.passwordFailure1.Name = "passwordFailure1";
            this.passwordFailure1.TabStop = false;
            // 
            // nfsVersionLabel
            // 
            resources.ApplyResources(this.nfsVersionLabel, "nfsVersionLabel");
            this.nfsVersionLabel.Name = "nfsVersionLabel";
            // 
            // NFS_ISO
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NFS_ISO";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.nfsVersionTableLayoutPanel.ResumeLayout(false);
            this.nfsVersionTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox NfsServerPathComboBox;
        private XenAdmin.Controls.Common.PasswordFailure passwordFailure1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label nfsVersionLabel;
        private System.Windows.Forms.TableLayoutPanel nfsVersionTableLayoutPanel;
        private System.Windows.Forms.RadioButton nfsVersion3RadioButton;
        private System.Windows.Forms.RadioButton nfsVersion4RadioButton;
    }
}
