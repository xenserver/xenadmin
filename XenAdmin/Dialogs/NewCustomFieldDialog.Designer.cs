namespace XenAdmin.Dialogs
{
    partial class NewCustomFieldDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewCustomFieldDialog));
            this.btnCancel = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.labelType = new System.Windows.Forms.Label();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.labelDuplicate = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelBlurb = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // textBoxName
            // 
            resources.ApplyResources(this.textBoxName, "textBoxName");
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxName, 2);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
            // 
            // labelName
            // 
            resources.ApplyResources(this.labelName, "labelName");
            this.labelName.Name = "labelName";
            // 
            // labelType
            // 
            resources.ApplyResources(this.labelType, "labelType");
            this.labelType.Name = "labelType";
            // 
            // comboBoxType
            // 
            resources.ApplyResources(this.comboBoxType, "comboBoxType");
            this.tableLayoutPanel1.SetColumnSpan(this.comboBoxType, 2);
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Items.AddRange(new object[] {
            resources.GetString("comboBoxType.Items"),
            resources.GetString("comboBoxType.Items1")});
            this.comboBoxType.Name = "comboBoxType";
            // 
            // labelDuplicate
            // 
            resources.ApplyResources(this.labelDuplicate, "labelDuplicate");
            this.tableLayoutPanel1.SetColumnSpan(this.labelDuplicate, 2);
            this.labelDuplicate.ForeColor = System.Drawing.Color.Red;
            this.labelDuplicate.Name = "labelDuplicate";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelBlurb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxType, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelDuplicate, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelType, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.okButton, 1, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelBlurb
            // 
            resources.ApplyResources(this.labelBlurb, "labelBlurb");
            this.tableLayoutPanel1.SetColumnSpan(this.labelBlurb, 3);
            this.labelBlurb.Name = "labelBlurb";
            // 
            // NewCustomFieldDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NewCustomFieldDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Label labelDuplicate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelBlurb;
    }
}