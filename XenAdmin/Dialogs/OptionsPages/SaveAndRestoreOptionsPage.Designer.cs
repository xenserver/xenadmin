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
            this.mainPasswordGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.mainPasswordLabel = new System.Windows.Forms.Label();
            this.changeMainPasswordButton = new System.Windows.Forms.Button();
            this.requireMainPasswordCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.mainPasswordGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.saveStateCheckBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.saveStateLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.mainPasswordGroupBox, 0, 2);
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
            // mainPasswordGroupBox
            // 
            resources.ApplyResources(this.mainPasswordGroupBox, "mainPasswordGroupBox");
            this.mainPasswordGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.mainPasswordGroupBox.Name = "mainPasswordGroupBox";
            this.mainPasswordGroupBox.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.mainPasswordLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.changeMainPasswordButton, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.requireMainPasswordCheckBox, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // mainPasswordLabel
            // 
            resources.ApplyResources(this.mainPasswordLabel, "mainPasswordLabel");
            this.tableLayoutPanel2.SetColumnSpan(this.mainPasswordLabel, 2);
            this.mainPasswordLabel.Name = "mainPasswordLabel";
            // 
            // changeMainPasswordButton
            // 
            resources.ApplyResources(this.changeMainPasswordButton, "changeMainPasswordButton");
            this.changeMainPasswordButton.Name = "changeMainPasswordButton";
            this.changeMainPasswordButton.UseVisualStyleBackColor = true;
            this.changeMainPasswordButton.Click += new System.EventHandler(this.changeMainPasswordButton_Click);
            // 
            // requireMainPasswordCheckBox
            // 
            this.requireMainPasswordCheckBox.AutoCheck = false;
            resources.ApplyResources(this.requireMainPasswordCheckBox, "requireMainPasswordCheckBox");
            this.requireMainPasswordCheckBox.Name = "requireMainPasswordCheckBox";
            this.requireMainPasswordCheckBox.UseVisualStyleBackColor = true;
            this.requireMainPasswordCheckBox.Click += new System.EventHandler(this.requireMainPasswordCheckBox_Click);
            // 
            // SaveAndRestoreOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SaveAndRestoreOptionsPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.mainPasswordGroupBox.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox saveStateCheckBox;
        private System.Windows.Forms.Label saveStateLabel;
        private XenAdmin.Controls.DecentGroupBox mainPasswordGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label mainPasswordLabel;
        private System.Windows.Forms.Button changeMainPasswordButton;
        private System.Windows.Forms.CheckBox requireMainPasswordCheckBox;
    }
}
