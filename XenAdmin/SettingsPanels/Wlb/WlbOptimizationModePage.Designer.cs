namespace XenAdmin.SettingsPanels
{
    partial class WlbOptimizationModePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbOptimizationModePage));
            this.labelOptModeBlurb = new System.Windows.Forms.Label();
            this.radioButtonFixedMode = new System.Windows.Forms.RadioButton();
            this.radioButtonAutomatedMode = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.decentGroupBoxAutomatedMode = new XenAdmin.Controls.DecentGroupBox();
            this.wlbOptModeScheduler1 = new XenAdmin.Controls.Wlb.WlbOptModeScheduler();
            this.decentGroupBoxFixedMode = new XenAdmin.Controls.DecentGroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButtonMaxDensity = new System.Windows.Forms.RadioButton();
            this.radioButtonMaxPerformance = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.decentGroupBoxAutomatedMode.SuspendLayout();
            this.decentGroupBoxFixedMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelOptModeBlurb
            // 
            resources.ApplyResources(this.labelOptModeBlurb, "labelOptModeBlurb");
            this.labelOptModeBlurb.Name = "labelOptModeBlurb";
            // 
            // radioButtonFixedMode
            // 
            resources.ApplyResources(this.radioButtonFixedMode, "radioButtonFixedMode");
            this.radioButtonFixedMode.Checked = true;
            this.radioButtonFixedMode.Name = "radioButtonFixedMode";
            this.radioButtonFixedMode.TabStop = true;
            this.radioButtonFixedMode.UseVisualStyleBackColor = true;
            this.radioButtonFixedMode.CheckedChanged += new System.EventHandler(this.radioButtonMode_CheckedChanged);
            // 
            // radioButtonAutomatedMode
            // 
            resources.ApplyResources(this.radioButtonAutomatedMode, "radioButtonAutomatedMode");
            this.radioButtonAutomatedMode.Name = "radioButtonAutomatedMode";
            this.radioButtonAutomatedMode.UseVisualStyleBackColor = true;
            this.radioButtonAutomatedMode.CheckedChanged += new System.EventHandler(this.radioButtonMode_CheckedChanged);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.labelOptModeBlurb);
            this.panel1.Controls.Add(this.radioButtonFixedMode);
            this.panel1.Controls.Add(this.radioButtonAutomatedMode);
            this.panel1.Controls.Add(this.decentGroupBoxAutomatedMode);
            this.panel1.Controls.Add(this.decentGroupBoxFixedMode);
            this.panel1.Name = "panel1";
            // 
            // decentGroupBoxAutomatedMode
            // 
            resources.ApplyResources(this.decentGroupBoxAutomatedMode, "decentGroupBoxAutomatedMode");
            this.decentGroupBoxAutomatedMode.Controls.Add(this.wlbOptModeScheduler1);
            this.decentGroupBoxAutomatedMode.Name = "decentGroupBoxAutomatedMode";
            this.decentGroupBoxAutomatedMode.TabStop = false;
            // 
            // wlbOptModeScheduler1
            // 
            resources.ApplyResources(this.wlbOptModeScheduler1, "wlbOptModeScheduler1");
            this.wlbOptModeScheduler1.Name = "wlbOptModeScheduler1";
            this.wlbOptModeScheduler1.ScheduledTasks = null;
            // 
            // decentGroupBoxFixedMode
            // 
            resources.ApplyResources(this.decentGroupBoxFixedMode, "decentGroupBoxFixedMode");
            this.decentGroupBoxFixedMode.Controls.Add(this.label3);
            this.decentGroupBoxFixedMode.Controls.Add(this.label2);
            this.decentGroupBoxFixedMode.Controls.Add(this.radioButtonMaxDensity);
            this.decentGroupBoxFixedMode.Controls.Add(this.radioButtonMaxPerformance);
            this.decentGroupBoxFixedMode.Name = "decentGroupBoxFixedMode";
            this.decentGroupBoxFixedMode.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // radioButtonMaxDensity
            // 
            resources.ApplyResources(this.radioButtonMaxDensity, "radioButtonMaxDensity");
            this.radioButtonMaxDensity.Name = "radioButtonMaxDensity";
            this.radioButtonMaxDensity.UseVisualStyleBackColor = true;
            // 
            // radioButtonMaxPerformance
            // 
            resources.ApplyResources(this.radioButtonMaxPerformance, "radioButtonMaxPerformance");
            this.radioButtonMaxPerformance.Checked = true;
            this.radioButtonMaxPerformance.Name = "radioButtonMaxPerformance";
            this.radioButtonMaxPerformance.TabStop = true;
            this.radioButtonMaxPerformance.UseVisualStyleBackColor = true;
            this.radioButtonMaxPerformance.CheckedChanged += new System.EventHandler(this.radioButtonOptMode_CheckedChanged);
            // 
            // WlbOptimizationModePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.panel1);
            this.Name = "WlbOptimizationModePage";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.decentGroupBoxAutomatedMode.ResumeLayout(false);
            this.decentGroupBoxFixedMode.ResumeLayout(false);
            this.decentGroupBoxFixedMode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelOptModeBlurb;
        private System.Windows.Forms.RadioButton radioButtonFixedMode;
        private System.Windows.Forms.RadioButton radioButtonAutomatedMode;
        private System.Windows.Forms.Panel panel1;
        private XenAdmin.Controls.DecentGroupBox decentGroupBoxFixedMode;
        private XenAdmin.Controls.DecentGroupBox decentGroupBoxAutomatedMode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioButtonMaxDensity;
        private System.Windows.Forms.RadioButton radioButtonMaxPerformance;
        private XenAdmin.Controls.Wlb.WlbOptModeScheduler wlbOptModeScheduler1;
    }
}
