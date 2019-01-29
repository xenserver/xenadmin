using XenAdmin.Wizards.PatchingWizard;

namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    partial class RollingUpgradeWizardUpgradeModePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RollingUpgradeWizardUpgradeModePage));
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonManual = new System.Windows.Forms.RadioButton();
            this.radioButtonAutomatic = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.comboBoxUpgradeMethod = new System.Windows.Forms.ComboBox();
            this.watermarkTextBox1 = new System.Windows.Forms.TextBox();
            this.labelUser = new System.Windows.Forms.Label();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // radioButtonManual
            // 
            resources.ApplyResources(this.radioButtonManual, "radioButtonManual");
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonManual, 2);
            this.radioButtonManual.Name = "radioButtonManual";
            this.radioButtonManual.UseVisualStyleBackColor = true;
            this.radioButtonManual.CheckedChanged += new System.EventHandler(this.radioButtonManual_CheckedChanged);
            // 
            // radioButtonAutomatic
            // 
            resources.ApplyResources(this.radioButtonAutomatic, "radioButtonAutomatic");
            this.radioButtonAutomatic.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonAutomatic, 2);
            this.radioButtonAutomatic.Name = "radioButtonAutomatic";
            this.radioButtonAutomatic.TabStop = true;
            this.radioButtonAutomatic.UseVisualStyleBackColor = true;
            this.radioButtonAutomatic.CheckedChanged += new System.EventHandler(this.radioButtonAutomatic_CheckedChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonAutomatic, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonManual, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.comboBoxUpgradeMethod, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.watermarkTextBox1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelUser, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.textBoxUser, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelPassword, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBoxPassword, 1, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // comboBoxUpgradeMethod
            // 
            this.comboBoxUpgradeMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUpgradeMethod.FormattingEnabled = true;
            this.comboBoxUpgradeMethod.Items.AddRange(new object[] {
            resources.GetString("comboBoxUpgradeMethod.Items"),
            resources.GetString("comboBoxUpgradeMethod.Items1"),
            resources.GetString("comboBoxUpgradeMethod.Items2")});
            resources.ApplyResources(this.comboBoxUpgradeMethod, "comboBoxUpgradeMethod");
            this.comboBoxUpgradeMethod.Name = "comboBoxUpgradeMethod";
            this.comboBoxUpgradeMethod.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // watermarkTextBox1
            // 
            resources.ApplyResources(this.watermarkTextBox1, "watermarkTextBox1");
            this.watermarkTextBox1.Name = "watermarkTextBox1";
            this.watermarkTextBox1.TextChanged += new System.EventHandler(this.watermarkTextBox1_TextChanged);
            // 
            // labelUser
            // 
            resources.ApplyResources(this.labelUser, "labelUser");
            this.labelUser.Name = "labelUser";
            // 
            // textBoxUser
            // 
            resources.ApplyResources(this.textBoxUser, "textBoxUser");
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.TextChanged += new System.EventHandler(this.textBoxUser_TextChanged);
            // 
            // labelPassword
            // 
            resources.ApplyResources(this.labelPassword, "labelPassword");
            this.labelPassword.Name = "labelPassword";
            // 
            // textBoxPassword
            // 
            resources.ApplyResources(this.textBoxPassword, "textBoxPassword");
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
            // 
            // RollingUpgradeWizardUpgradeModePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RollingUpgradeWizardUpgradeModePage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonManual;
        private System.Windows.Forms.RadioButton radioButtonAutomatic;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ComboBox comboBoxUpgradeMethod;
        private System.Windows.Forms.TextBox watermarkTextBox1;
        private System.Windows.Forms.Label labelUser;
        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxPassword;
    }
}
