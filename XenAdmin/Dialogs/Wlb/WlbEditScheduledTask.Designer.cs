namespace XenAdmin.Dialogs.Wlb
{
    partial class WlbEditScheduledTask
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbEditScheduledTask));
            this.panelEdit = new System.Windows.Forms.Panel();
            this.labelTimeDelimiter = new System.Windows.Forms.Label();
            this.comboBoxHour = new System.Windows.Forms.ComboBox();
            this.checkBoxEnable = new System.Windows.Forms.CheckBox();
            this.labelMode = new System.Windows.Forms.Label();
            this.labelAt = new System.Windows.Forms.Label();
            this.labelChangeTo = new System.Windows.Forms.Label();
            this.comboOptMode = new System.Windows.Forms.ComboBox();
            this.comboDayOfWeek = new System.Windows.Forms.ComboBox();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panelEdit.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelEdit
            // 
            this.panelEdit.Controls.Add(this.labelTimeDelimiter);
            this.panelEdit.Controls.Add(this.comboBoxHour);
            this.panelEdit.Controls.Add(this.checkBoxEnable);
            this.panelEdit.Controls.Add(this.labelMode);
            this.panelEdit.Controls.Add(this.labelAt);
            this.panelEdit.Controls.Add(this.labelChangeTo);
            this.panelEdit.Controls.Add(this.comboOptMode);
            this.panelEdit.Controls.Add(this.comboDayOfWeek);
            resources.ApplyResources(this.panelEdit, "panelEdit");
            this.panelEdit.Name = "panelEdit";
            // 
            // labelTimeDelimiter
            // 
            resources.ApplyResources(this.labelTimeDelimiter, "labelTimeDelimiter");
            this.labelTimeDelimiter.BackColor = System.Drawing.SystemColors.Control;
            this.labelTimeDelimiter.Name = "labelTimeDelimiter";
            // 
            // comboBoxHour
            // 
            this.comboBoxHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxHour, "comboBoxHour");
            this.comboBoxHour.Name = "comboBoxHour";
            // 
            // checkBoxEnable
            // 
            resources.ApplyResources(this.checkBoxEnable, "checkBoxEnable");
            this.checkBoxEnable.Checked = true;
            this.checkBoxEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEnable.Name = "checkBoxEnable";
            this.checkBoxEnable.UseVisualStyleBackColor = true;
            // 
            // labelMode
            // 
            resources.ApplyResources(this.labelMode, "labelMode");
            this.labelMode.Name = "labelMode";
            // 
            // labelAt
            // 
            resources.ApplyResources(this.labelAt, "labelAt");
            this.labelAt.Name = "labelAt";
            // 
            // labelChangeTo
            // 
            resources.ApplyResources(this.labelChangeTo, "labelChangeTo");
            this.labelChangeTo.Name = "labelChangeTo";
            // 
            // comboOptMode
            // 
            this.comboOptMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboOptMode, "comboOptMode");
            this.comboOptMode.FormattingEnabled = true;
            this.comboOptMode.Name = "comboOptMode";
            // 
            // comboDayOfWeek
            // 
            this.comboDayOfWeek.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboDayOfWeek, "comboDayOfWeek");
            this.comboDayOfWeek.FormattingEnabled = true;
            this.comboDayOfWeek.Name = "comboDayOfWeek";
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.buttonOK);
            this.panelButtons.Controls.Add(this.button1);
            resources.ApplyResources(this.panelButtons, "panelButtons");
            this.panelButtons.Name = "panelButtons";
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // WlbEditScheduledTask
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panelEdit);
            this.Controls.Add(this.panelButtons);
            this.Name = "WlbEditScheduledTask";
            this.panelEdit.ResumeLayout(false);
            this.panelEdit.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelEdit;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.Label labelAt;
        private System.Windows.Forms.Label labelChangeTo;
        private System.Windows.Forms.ComboBox comboOptMode;
        private System.Windows.Forms.ComboBox comboDayOfWeek;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBoxEnable;
        private System.Windows.Forms.ComboBox comboBoxHour;
        private System.Windows.Forms.Label labelTimeDelimiter;


    }
}
