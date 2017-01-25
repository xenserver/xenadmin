namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class CslgLocation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CslgLocation));
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxTarget = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboBoxStorageSystemBoston = new System.Windows.Forms.ComboBox();
            this.buttonDiscoverBostonSS = new System.Windows.Forms.Button();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.textBoxTarget, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.autoHeightLabel1, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // textBoxTarget
            // 
            resources.ApplyResources(this.textBoxTarget, "textBoxTarget");
            this.textBoxTarget.Name = "textBoxTarget";
            this.textBoxTarget.TextChanged += new System.EventHandler(this.textBoxTarget_TextChanged);
            // 
            // groupBox1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.tableLayoutPanel3);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.textBoxUsername, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label7, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.textBoxPassword, 1, 1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // textBoxUsername
            // 
            resources.ApplyResources(this.textBoxUsername, "textBoxUsername");
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.TextChanged += new System.EventHandler(this.textBoxTarget_TextChanged);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // textBoxPassword
            // 
            resources.ApplyResources(this.textBoxPassword, "textBoxPassword");
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxTarget_TextChanged);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.comboBoxStorageSystemBoston);
            this.panel1.Controls.Add(this.buttonDiscoverBostonSS);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // comboBoxStorageSystemBoston
            // 
            resources.ApplyResources(this.comboBoxStorageSystemBoston, "comboBoxStorageSystemBoston");
            this.comboBoxStorageSystemBoston.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxStorageSystemBoston.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStorageSystemBoston.FormattingEnabled = true;
            this.comboBoxStorageSystemBoston.Name = "comboBoxStorageSystemBoston";
            this.comboBoxStorageSystemBoston.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBoxStorageSystemBoston_DrawItem);
            this.comboBoxStorageSystemBoston.SelectedIndexChanged += new System.EventHandler(this.comboBoxStorageSystemBoston_SelectedIndexChanged);
            // 
            // buttonDiscoverBostonSS
            // 
            resources.ApplyResources(this.buttonDiscoverBostonSS, "buttonDiscoverBostonSS");
            this.buttonDiscoverBostonSS.Name = "buttonDiscoverBostonSS";
            this.buttonDiscoverBostonSS.UseVisualStyleBackColor = true;
            this.buttonDiscoverBostonSS.Click += new System.EventHandler(this.buttonDiscoverBostonSS_Click);
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.tableLayoutPanel2.SetColumnSpan(this.autoHeightLabel1, 2);
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // CslgLocation
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "CslgLocation";
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxTarget;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboBoxStorageSystemBoston;
        private System.Windows.Forms.Button buttonDiscoverBostonSS;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
    }
}
