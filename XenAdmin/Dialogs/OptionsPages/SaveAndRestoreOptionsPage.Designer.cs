namespace XenAdmin.Dialogs.OptionsPages
{
    partial class SaveAndRestoreOptionsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveAndRestoreOptionsPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.saveStateCheckBox = new System.Windows.Forms.CheckBox();
            this.saveStateLabel = new System.Windows.Forms.Label();
            this.masterPasswordGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.masterPasswordLabel = new System.Windows.Forms.Label();
            this.changeMasterPasswordButton = new System.Windows.Forms.Button();
            this.requireMasterPasswordCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.masterPasswordGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.saveStateCheckBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.saveStateLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.masterPasswordGroupBox, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // saveStateCheckBox
            // 
            this.saveStateCheckBox.AutoCheck = false;
            resources.ApplyResources(this.saveStateCheckBox, "saveStateCheckBox");
            this.saveStateCheckBox.Name = "saveStateCheckBox";
            this.saveStateCheckBox.UseVisualStyleBackColor = true;
            this.saveStateCheckBox.Click += new System.EventHandler(this.saveStateCheckBox_Click);
            // 
            // saveStateLabel
            // 
            resources.ApplyResources(this.saveStateLabel, "saveStateLabel");
            this.saveStateLabel.Name = "saveStateLabel";
            // 
            // masterPasswordGroupBox
            // 
            resources.ApplyResources(this.masterPasswordGroupBox, "masterPasswordGroupBox");
            this.masterPasswordGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.masterPasswordGroupBox.Name = "masterPasswordGroupBox";
            this.masterPasswordGroupBox.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.masterPasswordLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.changeMasterPasswordButton, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.requireMasterPasswordCheckBox, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // masterPasswordLabel
            // 
            resources.ApplyResources(this.masterPasswordLabel, "masterPasswordLabel");
            this.tableLayoutPanel2.SetColumnSpan(this.masterPasswordLabel, 2);
            this.masterPasswordLabel.Name = "masterPasswordLabel";
            // 
            // changeMasterPasswordButton
            // 
            resources.ApplyResources(this.changeMasterPasswordButton, "changeMasterPasswordButton");
            this.changeMasterPasswordButton.Name = "changeMasterPasswordButton";
            this.changeMasterPasswordButton.UseVisualStyleBackColor = true;
            this.changeMasterPasswordButton.Click += new System.EventHandler(this.changeMasterPasswordButton_Click);
            // 
            // requireMasterPasswordCheckBox
            // 
            this.requireMasterPasswordCheckBox.AutoCheck = false;
            resources.ApplyResources(this.requireMasterPasswordCheckBox, "requireMasterPasswordCheckBox");
            this.requireMasterPasswordCheckBox.Name = "requireMasterPasswordCheckBox";
            this.requireMasterPasswordCheckBox.UseVisualStyleBackColor = true;
            this.requireMasterPasswordCheckBox.Click += new System.EventHandler(this.requireMasterPasswordCheckBox_Click);
            // 
            // SaveAndRestoreOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SaveAndRestoreOptionsPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.masterPasswordGroupBox.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox saveStateCheckBox;
        private System.Windows.Forms.Label saveStateLabel;
        private XenAdmin.Controls.DecentGroupBox masterPasswordGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label masterPasswordLabel;
        private System.Windows.Forms.Button changeMasterPasswordButton;
        private System.Windows.Forms.CheckBox requireMasterPasswordCheckBox;
    }
}
