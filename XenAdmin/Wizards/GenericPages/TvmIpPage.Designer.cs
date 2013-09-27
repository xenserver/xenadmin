namespace XenAdmin.Wizards.GenericPages
{
	partial class TvmIpPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TvmIpPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_radioAutomatic = new System.Windows.Forms.RadioButton();
            this.m_radioManual = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.m_textBoxIP = new System.Windows.Forms.TextBox();
            this.m_textBoxMask = new System.Windows.Forms.TextBox();
            this.m_textBoxGateway = new System.Windows.Forms.TextBox();
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.m_comboBoxNetwork = new XenAdmin.Controls.NetworkComboBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.m_radioAutomatic, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_radioManual, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.m_textBoxIP, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_textBoxMask, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.m_textBoxGateway, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.m_ctrlError, 3, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // m_radioAutomatic
            // 
            resources.ApplyResources(this.m_radioAutomatic, "m_radioAutomatic");
            this.tableLayoutPanel1.SetColumnSpan(this.m_radioAutomatic, 3);
            this.m_radioAutomatic.Name = "m_radioAutomatic";
            this.m_radioAutomatic.TabStop = true;
            this.m_radioAutomatic.UseVisualStyleBackColor = true;
            this.m_radioAutomatic.CheckedChanged += new System.EventHandler(this.m_radioAutomatic_CheckedChanged);
            // 
            // m_radioManual
            // 
            resources.ApplyResources(this.m_radioManual, "m_radioManual");
            this.tableLayoutPanel1.SetColumnSpan(this.m_radioManual, 3);
            this.m_radioManual.Name = "m_radioManual";
            this.m_radioManual.TabStop = true;
            this.m_radioManual.UseVisualStyleBackColor = true;
            this.m_radioManual.CheckedChanged += new System.EventHandler(this.m_radioManual_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // m_textBoxIP
            // 
            resources.ApplyResources(this.m_textBoxIP, "m_textBoxIP");
            this.m_textBoxIP.Name = "m_textBoxIP";
            this.m_textBoxIP.TextChanged += new System.EventHandler(this.m_textBoxIP_TextChanged);
            // 
            // m_textBoxMask
            // 
            resources.ApplyResources(this.m_textBoxMask, "m_textBoxMask");
            this.m_textBoxMask.Name = "m_textBoxMask";
            this.m_textBoxMask.TextChanged += new System.EventHandler(this.m_textBoxMask_TextChanged);
            // 
            // m_textBoxGateway
            // 
            resources.ApplyResources(this.m_textBoxGateway, "m_textBoxGateway");
            this.m_textBoxGateway.Name = "m_textBoxGateway";
            this.m_textBoxGateway.TextChanged += new System.EventHandler(this.m_textBoxGateway_TextChanged);
            // 
            // m_ctrlError
            // 
            resources.ApplyResources(this.m_ctrlError, "m_ctrlError");
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.tableLayoutPanel2.SetColumnSpan(this.autoHeightLabel1, 2);
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // m_comboBoxNetwork
            // 
            resources.ApplyResources(this.m_comboBoxNetwork, "m_comboBoxNetwork");
            this.m_comboBoxNetwork.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboBoxNetwork.FormattingEnabled = true;
            this.m_comboBoxNetwork.Name = "m_comboBoxNetwork";
            this.m_comboBoxNetwork.SelectedIndexChanged += new System.EventHandler(this.m_comboBoxNetwork_SelectedIndexChanged);
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.autoHeightLabel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.m_comboBoxNetwork, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.tableLayoutPanel2.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // TvmIpPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "TvmIpPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
		private System.Windows.Forms.RadioButton m_radioAutomatic;
		private System.Windows.Forms.RadioButton m_radioManual;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox m_textBoxIP;
		private System.Windows.Forms.TextBox m_textBoxMask;
		private System.Windows.Forms.TextBox m_textBoxGateway;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlError;
		private System.Windows.Forms.Label label4;
        private XenAdmin.Controls.NetworkComboBox m_comboBoxNetwork;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
