namespace XenAdmin.Wizards.ImportWizard
{
    partial class ImageVMConfigPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageVMConfigPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.m_textBoxVMName = new System.Windows.Forms.TextBox();
            this.lblCPUs = new System.Windows.Forms.Label();
            this.m_upDownCpuCount = new System.Windows.Forms.NumericUpDown();
            this.lblMemory = new System.Windows.Forms.Label();
            this.m_upDownMemory = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.m_groupBoxAddSpace = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.m_upDownAddSpace = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_upDownCpuCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_upDownMemory)).BeginInit();
            this.m_groupBoxAddSpace.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_upDownAddSpace)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.lblIntro, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_textBoxVMName, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblCPUs, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.m_upDownCpuCount, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblMemory, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.m_upDownMemory, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label4, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.m_groupBoxAddSpace, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.m_ctrlError, 0, 8);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // lblIntro
            // 
            resources.ApplyResources(this.lblIntro, "lblIntro");
            this.tableLayoutPanel1.SetColumnSpan(this.lblIntro, 4);
            this.lblIntro.Name = "lblIntro";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // m_textBoxVMName
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.m_textBoxVMName, 3);
            resources.ApplyResources(this.m_textBoxVMName, "m_textBoxVMName");
            this.m_textBoxVMName.Name = "m_textBoxVMName";
            this.m_textBoxVMName.TextChanged += new System.EventHandler(this.m_textBoxVMName_TextChanged);
            // 
            // lblCPUs
            // 
            resources.ApplyResources(this.lblCPUs, "lblCPUs");
            this.lblCPUs.Name = "lblCPUs";
            // 
            // m_upDownCpuCount
            // 
            resources.ApplyResources(this.m_upDownCpuCount, "m_upDownCpuCount");
            this.m_upDownCpuCount.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.m_upDownCpuCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_upDownCpuCount.Name = "m_upDownCpuCount";
            this.m_upDownCpuCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_upDownCpuCount.ValueChanged += new System.EventHandler(this.m_upDownCpuCount_ValueChanged);
            // 
            // lblMemory
            // 
            resources.ApplyResources(this.lblMemory, "lblMemory");
            this.lblMemory.Name = "lblMemory";
            // 
            // m_upDownMemory
            // 
            this.m_upDownMemory.Increment = new decimal(new int[] {
            256,
            0,
            0,
            0});
            resources.ApplyResources(this.m_upDownMemory, "m_upDownMemory");
            this.m_upDownMemory.Maximum = new decimal(new int[] {
            16128,
            0,
            0,
            0});
            this.m_upDownMemory.Minimum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.m_upDownMemory.Name = "m_upDownMemory";
            this.m_upDownMemory.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.m_upDownMemory.ValueChanged += new System.EventHandler(this.m_upDownMemory_ValueChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // m_groupBoxAddSpace
            // 
            resources.ApplyResources(this.m_groupBoxAddSpace, "m_groupBoxAddSpace");
            this.tableLayoutPanel1.SetColumnSpan(this.m_groupBoxAddSpace, 4);
            this.m_groupBoxAddSpace.Controls.Add(this.tableLayoutPanel2);
            this.m_groupBoxAddSpace.Name = "m_groupBoxAddSpace";
            this.m_groupBoxAddSpace.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.label1, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.m_upDownAddSpace, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.label5, 3, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // m_upDownAddSpace
            // 
            this.m_upDownAddSpace.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            resources.ApplyResources(this.m_upDownAddSpace, "m_upDownAddSpace");
            this.m_upDownAddSpace.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.m_upDownAddSpace.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.m_upDownAddSpace.Name = "m_upDownAddSpace";
            this.m_upDownAddSpace.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.m_upDownAddSpace.ValueChanged += new System.EventHandler(this.m_upDownAddSpace_ValueChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // m_ctrlError
            // 
            resources.ApplyResources(this.m_ctrlError, "m_ctrlError");
            this.tableLayoutPanel1.SetColumnSpan(this.m_ctrlError, 4);
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // ImageVMConfigPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ImageVMConfigPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_upDownCpuCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_upDownMemory)).EndInit();
            this.m_groupBoxAddSpace.ResumeLayout(false);
            this.m_groupBoxAddSpace.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_upDownAddSpace)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private XenAdmin.Controls.Common.AutoHeightLabel lblIntro;
		private System.Windows.Forms.Label lblCPUs;
		private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown m_upDownCpuCount;
        private System.Windows.Forms.NumericUpDown m_upDownMemory;
        private System.Windows.Forms.NumericUpDown m_upDownAddSpace;
        private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox m_textBoxVMName;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlError;
		private System.Windows.Forms.GroupBox m_groupBoxAddSpace;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}
