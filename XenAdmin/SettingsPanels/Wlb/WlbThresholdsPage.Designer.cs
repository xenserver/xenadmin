namespace XenAdmin.SettingsPanels
{
    partial class WlbThresholdsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbThresholdsPage));
            this.labelBlurb = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.decentGroupBox1 = new XenAdmin.Controls.DecentGroupBox();
            this.label1DiskWrite = new System.Windows.Forms.Label();
            this.labelDiskRead = new System.Windows.Forms.Label();
            this.labelNetworkWrite = new System.Windows.Forms.Label();
            this.labelNetworkRead = new System.Windows.Forms.Label();
            this.labelFreeMemory = new System.Windows.Forms.Label();
            this.labelCPU = new System.Windows.Forms.Label();
            this.updownDiskWriteCriticalPoint = new System.Windows.Forms.NumericUpDown();
            this.labelNetworkReadUnits = new System.Windows.Forms.Label();
            this.updownDiskReadCriticalPoint = new System.Windows.Forms.NumericUpDown();
            this.labelFreeMemoryUnits = new System.Windows.Forms.Label();
            this.updownNetworkWriteCriticalPoint = new System.Windows.Forms.NumericUpDown();
            this.labelCPUUnits = new System.Windows.Forms.Label();
            this.updownNetworkReadCriticalPoint = new System.Windows.Forms.NumericUpDown();
            this.labelNetworkWriteUnits = new System.Windows.Forms.Label();
            this.updownMemoryCriticalPoint = new System.Windows.Forms.NumericUpDown();
            this.labelDiskReadUnits = new System.Windows.Forms.Label();
            this.updownCPUCriticalPoint = new System.Windows.Forms.NumericUpDown();
            this.labelDiskWriteUnits = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.decentGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updownDiskWriteCriticalPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownDiskReadCriticalPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownNetworkWriteCriticalPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownNetworkReadCriticalPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownMemoryCriticalPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownCPUCriticalPoint)).BeginInit();
            this.SuspendLayout();
            // 
            // labelBlurb
            // 
            resources.ApplyResources(this.labelBlurb, "labelBlurb");
            this.labelBlurb.Name = "labelBlurb";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.decentGroupBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelBlurb, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // decentGroupBox1
            // 
            resources.ApplyResources(this.decentGroupBox1, "decentGroupBox1");
            this.decentGroupBox1.Controls.Add(this.label1DiskWrite);
            this.decentGroupBox1.Controls.Add(this.labelDiskRead);
            this.decentGroupBox1.Controls.Add(this.labelNetworkWrite);
            this.decentGroupBox1.Controls.Add(this.labelNetworkRead);
            this.decentGroupBox1.Controls.Add(this.labelFreeMemory);
            this.decentGroupBox1.Controls.Add(this.labelCPU);
            this.decentGroupBox1.Controls.Add(this.updownDiskWriteCriticalPoint);
            this.decentGroupBox1.Controls.Add(this.labelNetworkReadUnits);
            this.decentGroupBox1.Controls.Add(this.updownDiskReadCriticalPoint);
            this.decentGroupBox1.Controls.Add(this.labelFreeMemoryUnits);
            this.decentGroupBox1.Controls.Add(this.updownNetworkWriteCriticalPoint);
            this.decentGroupBox1.Controls.Add(this.labelCPUUnits);
            this.decentGroupBox1.Controls.Add(this.updownNetworkReadCriticalPoint);
            this.decentGroupBox1.Controls.Add(this.labelNetworkWriteUnits);
            this.decentGroupBox1.Controls.Add(this.updownMemoryCriticalPoint);
            this.decentGroupBox1.Controls.Add(this.labelDiskReadUnits);
            this.decentGroupBox1.Controls.Add(this.updownCPUCriticalPoint);
            this.decentGroupBox1.Controls.Add(this.labelDiskWriteUnits);
            this.decentGroupBox1.Name = "decentGroupBox1";
            this.decentGroupBox1.TabStop = false;
            // 
            // label1DiskWrite
            // 
            resources.ApplyResources(this.label1DiskWrite, "label1DiskWrite");
            this.label1DiskWrite.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1DiskWrite.Name = "label1DiskWrite";
            // 
            // labelDiskRead
            // 
            resources.ApplyResources(this.labelDiskRead, "labelDiskRead");
            this.labelDiskRead.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelDiskRead.Name = "labelDiskRead";
            // 
            // labelNetworkWrite
            // 
            resources.ApplyResources(this.labelNetworkWrite, "labelNetworkWrite");
            this.labelNetworkWrite.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelNetworkWrite.Name = "labelNetworkWrite";
            // 
            // labelNetworkRead
            // 
            resources.ApplyResources(this.labelNetworkRead, "labelNetworkRead");
            this.labelNetworkRead.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelNetworkRead.Name = "labelNetworkRead";
            // 
            // labelFreeMemory
            // 
            resources.ApplyResources(this.labelFreeMemory, "labelFreeMemory");
            this.labelFreeMemory.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelFreeMemory.Name = "labelFreeMemory";
            // 
            // labelCPU
            // 
            resources.ApplyResources(this.labelCPU, "labelCPU");
            this.labelCPU.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelCPU.Name = "labelCPU";
            // 
            // updownDiskWriteCriticalPoint
            // 
            this.updownDiskWriteCriticalPoint.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.updownDiskWriteCriticalPoint, "updownDiskWriteCriticalPoint");
            this.updownDiskWriteCriticalPoint.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.updownDiskWriteCriticalPoint.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownDiskWriteCriticalPoint.Name = "updownDiskWriteCriticalPoint";
            this.updownDiskWriteCriticalPoint.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownDiskWriteCriticalPoint.ValueChanged += new System.EventHandler(this.updown_ValueChanged);
            this.updownDiskWriteCriticalPoint.KeyUp += new System.Windows.Forms.KeyEventHandler(this.updown_KeyUp);
            // 
            // labelNetworkReadUnits
            // 
            resources.ApplyResources(this.labelNetworkReadUnits, "labelNetworkReadUnits");
            this.labelNetworkReadUnits.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelNetworkReadUnits.Name = "labelNetworkReadUnits";
            // 
            // updownDiskReadCriticalPoint
            // 
            this.updownDiskReadCriticalPoint.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.updownDiskReadCriticalPoint, "updownDiskReadCriticalPoint");
            this.updownDiskReadCriticalPoint.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.updownDiskReadCriticalPoint.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownDiskReadCriticalPoint.Name = "updownDiskReadCriticalPoint";
            this.updownDiskReadCriticalPoint.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownDiskReadCriticalPoint.ValueChanged += new System.EventHandler(this.updown_ValueChanged);
            this.updownDiskReadCriticalPoint.KeyUp += new System.Windows.Forms.KeyEventHandler(this.updown_KeyUp);
            // 
            // labelFreeMemoryUnits
            // 
            resources.ApplyResources(this.labelFreeMemoryUnits, "labelFreeMemoryUnits");
            this.labelFreeMemoryUnits.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelFreeMemoryUnits.Name = "labelFreeMemoryUnits";
            // 
            // updownNetworkWriteCriticalPoint
            // 
            this.updownNetworkWriteCriticalPoint.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.updownNetworkWriteCriticalPoint, "updownNetworkWriteCriticalPoint");
            this.updownNetworkWriteCriticalPoint.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.updownNetworkWriteCriticalPoint.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownNetworkWriteCriticalPoint.Name = "updownNetworkWriteCriticalPoint";
            this.updownNetworkWriteCriticalPoint.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownNetworkWriteCriticalPoint.ValueChanged += new System.EventHandler(this.updown_ValueChanged);
            this.updownNetworkWriteCriticalPoint.KeyUp += new System.Windows.Forms.KeyEventHandler(this.updown_KeyUp);
            // 
            // labelCPUUnits
            // 
            resources.ApplyResources(this.labelCPUUnits, "labelCPUUnits");
            this.labelCPUUnits.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelCPUUnits.Name = "labelCPUUnits";
            // 
            // updownNetworkReadCriticalPoint
            // 
            this.updownNetworkReadCriticalPoint.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.updownNetworkReadCriticalPoint, "updownNetworkReadCriticalPoint");
            this.updownNetworkReadCriticalPoint.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.updownNetworkReadCriticalPoint.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownNetworkReadCriticalPoint.Name = "updownNetworkReadCriticalPoint";
            this.updownNetworkReadCriticalPoint.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownNetworkReadCriticalPoint.ValueChanged += new System.EventHandler(this.updown_ValueChanged);
            this.updownNetworkReadCriticalPoint.KeyUp += new System.Windows.Forms.KeyEventHandler(this.updown_KeyUp);
            // 
            // labelNetworkWriteUnits
            // 
            resources.ApplyResources(this.labelNetworkWriteUnits, "labelNetworkWriteUnits");
            this.labelNetworkWriteUnits.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelNetworkWriteUnits.Name = "labelNetworkWriteUnits";
            // 
            // updownMemoryCriticalPoint
            // 
            this.updownMemoryCriticalPoint.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            resources.ApplyResources(this.updownMemoryCriticalPoint, "updownMemoryCriticalPoint");
            this.updownMemoryCriticalPoint.Maximum = new decimal(new int[] {
            32000,
            0,
            0,
            0});
            this.updownMemoryCriticalPoint.Name = "updownMemoryCriticalPoint";
            this.updownMemoryCriticalPoint.ValueChanged += new System.EventHandler(this.updown_ValueChanged);
            this.updownMemoryCriticalPoint.KeyUp += new System.Windows.Forms.KeyEventHandler(this.updown_KeyUp);
            // 
            // labelDiskReadUnits
            // 
            resources.ApplyResources(this.labelDiskReadUnits, "labelDiskReadUnits");
            this.labelDiskReadUnits.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelDiskReadUnits.Name = "labelDiskReadUnits";
            // 
            // updownCPUCriticalPoint
            // 
            resources.ApplyResources(this.updownCPUCriticalPoint, "updownCPUCriticalPoint");
            this.updownCPUCriticalPoint.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownCPUCriticalPoint.Name = "updownCPUCriticalPoint";
            this.updownCPUCriticalPoint.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownCPUCriticalPoint.ValueChanged += new System.EventHandler(this.updown_ValueChanged);
            this.updownCPUCriticalPoint.KeyUp += new System.Windows.Forms.KeyEventHandler(this.updown_KeyUp);
            // 
            // labelDiskWriteUnits
            // 
            resources.ApplyResources(this.labelDiskWriteUnits, "labelDiskWriteUnits");
            this.labelDiskWriteUnits.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelDiskWriteUnits.Name = "labelDiskWriteUnits";
            // 
            // WlbThresholdsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "WlbThresholdsPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.decentGroupBox1.ResumeLayout(false);
            this.decentGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updownDiskWriteCriticalPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownDiskReadCriticalPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownNetworkWriteCriticalPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownNetworkReadCriticalPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownMemoryCriticalPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownCPUCriticalPoint)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelBlurb;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.DecentGroupBox decentGroupBox1;
        private System.Windows.Forms.Label label1DiskWrite;
        private System.Windows.Forms.Label labelDiskRead;
        private System.Windows.Forms.Label labelNetworkWrite;
        private System.Windows.Forms.Label labelNetworkRead;
        private System.Windows.Forms.Label labelFreeMemory;
        private System.Windows.Forms.Label labelCPU;
        private System.Windows.Forms.NumericUpDown updownDiskWriteCriticalPoint;
        private System.Windows.Forms.Label labelNetworkReadUnits;
        private System.Windows.Forms.NumericUpDown updownDiskReadCriticalPoint;
        private System.Windows.Forms.Label labelFreeMemoryUnits;
        private System.Windows.Forms.NumericUpDown updownNetworkWriteCriticalPoint;
        private System.Windows.Forms.Label labelCPUUnits;
        private System.Windows.Forms.NumericUpDown updownNetworkReadCriticalPoint;
        private System.Windows.Forms.Label labelNetworkWriteUnits;
        private System.Windows.Forms.NumericUpDown updownMemoryCriticalPoint;
        private System.Windows.Forms.Label labelDiskReadUnits;
        private System.Windows.Forms.NumericUpDown updownCPUCriticalPoint;
        private System.Windows.Forms.Label labelDiskWriteUnits;
    }
}
