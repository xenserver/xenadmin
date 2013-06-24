namespace XenAdmin.TabPages
{
    partial class HistoryPage
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
            ConnectionsManager.XenConnections.CollectionChanged -= History_CollectionChanged;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryPage));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.gradientPanel1 = new XenAdmin.Controls.GradientPanel.GradientPanel();
            this.errorsCheckBox = new System.Windows.Forms.CheckBox();
            this.alertsCheckBox = new System.Windows.Forms.CheckBox();
            this.informationCheckBox = new System.Windows.Forms.CheckBox();
            this.actionsCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.customHistoryContainer1 = new XenAdmin.Controls.CustomHistoryContainer();
            this.gradientPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.toolTip1.SetToolTip(this.button1, resources.GetString("button1.ToolTip"));
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // gradientPanel1
            // 
            resources.ApplyResources(this.gradientPanel1, "gradientPanel1");
            this.gradientPanel1.Controls.Add(this.button1);
            this.gradientPanel1.Controls.Add(this.errorsCheckBox);
            this.gradientPanel1.Controls.Add(this.alertsCheckBox);
            this.gradientPanel1.Controls.Add(this.informationCheckBox);
            this.gradientPanel1.Controls.Add(this.actionsCheckBox);
            this.gradientPanel1.Controls.Add(this.label1);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Scheme = XenAdmin.Controls.GradientPanel.GradientPanel.Schemes.Tab;
            // 
            // errorsCheckBox
            // 
            resources.ApplyResources(this.errorsCheckBox, "errorsCheckBox");
            this.errorsCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.errorsCheckBox.Checked = true;
            this.errorsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.errorsCheckBox.ForeColor = System.Drawing.Color.White;
            this.errorsCheckBox.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.errorsCheckBox.Name = "errorsCheckBox";
            this.errorsCheckBox.UseVisualStyleBackColor = false;
            this.errorsCheckBox.CheckedChanged += new System.EventHandler(this.errorsCheckBox_CheckedChanged);
            // 
            // alertsCheckBox
            // 
            resources.ApplyResources(this.alertsCheckBox, "alertsCheckBox");
            this.alertsCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.alertsCheckBox.Checked = true;
            this.alertsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alertsCheckBox.ForeColor = System.Drawing.Color.White;
            this.alertsCheckBox.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.alertsCheckBox.Name = "alertsCheckBox";
            this.alertsCheckBox.UseVisualStyleBackColor = false;
            this.alertsCheckBox.CheckedChanged += new System.EventHandler(this.alertsCheckBox_CheckedChanged);
            // 
            // informationCheckBox
            // 
            resources.ApplyResources(this.informationCheckBox, "informationCheckBox");
            this.informationCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.informationCheckBox.Checked = true;
            this.informationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.informationCheckBox.ForeColor = System.Drawing.Color.White;
            this.informationCheckBox.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.informationCheckBox.Name = "informationCheckBox";
            this.informationCheckBox.UseVisualStyleBackColor = false;
            this.informationCheckBox.CheckedChanged += new System.EventHandler(this.informationCheckBox_CheckedChanged);
            // 
            // actionsCheckBox
            // 
            resources.ApplyResources(this.actionsCheckBox, "actionsCheckBox");
            this.actionsCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.actionsCheckBox.Checked = true;
            this.actionsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.actionsCheckBox.ForeColor = System.Drawing.Color.White;
            this.actionsCheckBox.Image = global::XenAdmin.Properties.Resources.commands_16;
            this.actionsCheckBox.Name = "actionsCheckBox";
            this.actionsCheckBox.UseVisualStyleBackColor = false;
            this.actionsCheckBox.CheckedChanged += new System.EventHandler(this.actionsCheckBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // customHistoryContainer1
            // 
            resources.ApplyResources(this.customHistoryContainer1, "customHistoryContainer1");
            this.customHistoryContainer1.BackColor = System.Drawing.Color.Transparent;
            this.customHistoryContainer1.Name = "customHistoryContainer1";
            // 
            // HistoryPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.gradientPanel1);
            this.Controls.Add(this.customHistoryContainer1);
            this.DoubleBuffered = true;
            this.Name = "HistoryPage";
            this.gradientPanel1.ResumeLayout(false);
            this.gradientPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox informationCheckBox;
        private System.Windows.Forms.CheckBox actionsCheckBox;
        private System.Windows.Forms.CheckBox alertsCheckBox;
        private System.Windows.Forms.CheckBox errorsCheckBox;
        private XenAdmin.Controls.CustomHistoryContainer customHistoryContainer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip toolTip1;
        private XenAdmin.Controls.GradientPanel.GradientPanel gradientPanel1;
        private System.Windows.Forms.Label label1;
    }
}
