namespace XenAdmin.Dialogs
{
    partial class AddServerDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddServerDialog));
            this.AddButton = new System.Windows.Forms.Button();
            this.CancelButton2 = new System.Windows.Forms.Button();
            this.ServerNameLabel = new System.Windows.Forms.Label();
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.ServerNameComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelCreds = new System.Windows.Forms.TableLayoutPanel();
            this.labelError = new System.Windows.Forms.Label();
            this.pictureBoxError = new System.Windows.Forms.PictureBox();
            this.labelInstructions = new System.Windows.Forms.Label();
            this.tableLayoutPanelType = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanelCreds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).BeginInit();
            this.tableLayoutPanelType.SuspendLayout();
            this.SuspendLayout();
            // 
            // AddButton
            // 
            resources.ApplyResources(this.AddButton, "AddButton");
            this.AddButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.AddButton.Name = "AddButton";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // CancelButton2
            // 
            this.CancelButton2.CausesValidation = false;
            this.CancelButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.CancelButton2, "CancelButton2");
            this.CancelButton2.Name = "CancelButton2";
            this.CancelButton2.UseVisualStyleBackColor = true;
            this.CancelButton2.Click += new System.EventHandler(this.CancelButton2_Click);
            this.CancelButton2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CancelButton2_KeyDown);
            // 
            // ServerNameLabel
            // 
            resources.ApplyResources(this.ServerNameLabel, "ServerNameLabel");
            this.ServerNameLabel.Name = "ServerNameLabel";
            // 
            // UsernameLabel
            // 
            resources.ApplyResources(this.UsernameLabel, "UsernameLabel");
            this.UsernameLabel.Name = "UsernameLabel";
            // 
            // PasswordLabel
            // 
            resources.ApplyResources(this.PasswordLabel, "PasswordLabel");
            this.PasswordLabel.Name = "PasswordLabel";
            // 
            // UsernameTextBox
            // 
            resources.ApplyResources(this.UsernameTextBox, "UsernameTextBox");
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.TextChanged += new System.EventHandler(this.TextFields_TextChanged);
            // 
            // PasswordTextBox
            // 
            resources.ApplyResources(this.PasswordTextBox, "PasswordTextBox");
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.UseSystemPasswordChar = true;
            this.PasswordTextBox.TextChanged += new System.EventHandler(this.TextFields_TextChanged);
            // 
            // ServerNameComboBox
            // 
            resources.ApplyResources(this.ServerNameComboBox, "ServerNameComboBox");
            this.ServerNameComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ServerNameComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.tableLayoutPanelType.SetColumnSpan(this.ServerNameComboBox, 2);
            this.ServerNameComboBox.Name = "ServerNameComboBox";
            this.ServerNameComboBox.TextChanged += new System.EventHandler(this.TextFields_TextChanged);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.tableLayoutPanelType.SetColumnSpan(this.groupBox1, 3);
            this.groupBox1.Controls.Add(this.tableLayoutPanelCreds);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanelCreds
            // 
            resources.ApplyResources(this.tableLayoutPanelCreds, "tableLayoutPanelCreds");
            this.tableLayoutPanelCreds.Controls.Add(this.UsernameLabel, 0, 0);
            this.tableLayoutPanelCreds.Controls.Add(this.PasswordLabel, 0, 1);
            this.tableLayoutPanelCreds.Controls.Add(this.PasswordTextBox, 1, 1);
            this.tableLayoutPanelCreds.Controls.Add(this.UsernameTextBox, 1, 0);
            this.tableLayoutPanelCreds.Controls.Add(this.labelError, 1, 2);
            this.tableLayoutPanelCreds.Controls.Add(this.pictureBoxError, 0, 2);
            this.tableLayoutPanelCreds.Name = "tableLayoutPanelCreds";
            // 
            // labelError
            // 
            resources.ApplyResources(this.labelError, "labelError");
            this.labelError.Name = "labelError";
            this.labelError.TextChanged += new System.EventHandler(this.labelError_TextChanged);
            // 
            // pictureBoxError
            // 
            resources.ApplyResources(this.pictureBoxError, "pictureBoxError");
            this.pictureBoxError.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.pictureBoxError.Name = "pictureBoxError";
            this.pictureBoxError.TabStop = false;
            // 
            // labelInstructions
            // 
            resources.ApplyResources(this.labelInstructions, "labelInstructions");
            this.tableLayoutPanelType.SetColumnSpan(this.labelInstructions, 3);
            this.labelInstructions.Name = "labelInstructions";
            // 
            // tableLayoutPanelType
            // 
            resources.ApplyResources(this.tableLayoutPanelType, "tableLayoutPanelType");
            this.tableLayoutPanelType.Controls.Add(this.ServerNameLabel, 0, 2);
            this.tableLayoutPanelType.Controls.Add(this.ServerNameComboBox, 1, 2);
            this.tableLayoutPanelType.Controls.Add(this.groupBox1, 0, 3);
            this.tableLayoutPanelType.Controls.Add(this.labelInstructions, 0, 0);
            this.tableLayoutPanelType.Controls.Add(this.CancelButton2, 2, 4);
            this.tableLayoutPanelType.Controls.Add(this.AddButton, 1, 4);
            this.tableLayoutPanelType.Name = "tableLayoutPanelType";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // AddServerDialog
            // 
            this.AcceptButton = this.AddButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.CancelButton2;
            this.Controls.Add(this.tableLayoutPanelType);
            this.Name = "AddServerDialog";
            this.Load += new System.EventHandler(this.AddServerDialog_Load);
            this.Shown += new System.EventHandler(this.AddServerDialog_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanelCreds.ResumeLayout(false);
            this.tableLayoutPanelCreds.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).EndInit();
            this.tableLayoutPanelType.ResumeLayout(false);
            this.tableLayoutPanelType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.Label PasswordLabel;
        private System.Windows.Forms.Button CancelButton2;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCreds;
        private System.Windows.Forms.PictureBox pictureBoxError;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label ServerNameLabel;
        private System.Windows.Forms.TextBox UsernameTextBox;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.ComboBox ServerNameComboBox;
        private System.Windows.Forms.Label labelInstructions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelType;
    }
}
