namespace XenAdmin.Controls
{
    partial class HaNtolIndicator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HaNtolIndicator));
            this.groupBoxControls = new System.Windows.Forms.GroupBox();
            this.tableServerFailureLimit = new System.Windows.Forms.TableLayoutPanel();
            this.tableStatus = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxStatus = new System.Windows.Forms.PictureBox();
            this.labelStatus = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.labelMax = new System.Windows.Forms.Label();
            this.spinner = new System.Windows.Forms.PictureBox();
            this.numericUpDownCapacity = new System.Windows.Forms.NumericUpDown();
            this.labelNumberOfServers = new System.Windows.Forms.Label();
            this.groupBoxControls.SuspendLayout();
            this.tableServerFailureLimit.SuspendLayout();
            this.tableStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCapacity)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxControls
            // 
            resources.ApplyResources(this.groupBoxControls, "groupBoxControls");
            this.groupBoxControls.Controls.Add(this.tableServerFailureLimit);
            this.groupBoxControls.Name = "groupBoxControls";
            this.groupBoxControls.TabStop = false;
            // 
            // tableServerFailureLimit
            // 
            resources.ApplyResources(this.tableServerFailureLimit, "tableServerFailureLimit");
            this.tableServerFailureLimit.Controls.Add(this.tableStatus, 0, 2);
            this.tableServerFailureLimit.Controls.Add(this.label1, 0, 0);
            this.tableServerFailureLimit.Controls.Add(this.labelMax, 2, 1);
            this.tableServerFailureLimit.Controls.Add(this.spinner, 3, 1);
            this.tableServerFailureLimit.Controls.Add(this.numericUpDownCapacity, 1, 1);
            this.tableServerFailureLimit.Controls.Add(this.labelNumberOfServers, 0, 1);
            this.tableServerFailureLimit.Name = "tableServerFailureLimit";
            this.tableServerFailureLimit.Resize += new System.EventHandler(this.tableServerFailureLimit_Resize);
            // 
            // tableStatus
            // 
            resources.ApplyResources(this.tableStatus, "tableStatus");
            this.tableServerFailureLimit.SetColumnSpan(this.tableStatus, 4);
            this.tableStatus.Controls.Add(this.pictureBoxStatus, 0, 0);
            this.tableStatus.Controls.Add(this.labelStatus, 1, 0);
            this.tableStatus.Name = "tableStatus";
            // 
            // pictureBoxStatus
            // 
            resources.ApplyResources(this.pictureBoxStatus, "pictureBoxStatus");
            this.pictureBoxStatus.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.pictureBoxStatus.Name = "pictureBoxStatus";
            this.pictureBoxStatus.TabStop = false;
            // 
            // labelStatus
            // 
            resources.ApplyResources(this.labelStatus, "labelStatus");
            this.labelStatus.ForeColor = System.Drawing.Color.Red;
            this.labelStatus.Name = "labelStatus";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableServerFailureLimit.SetColumnSpan(this.label1, 4);
            this.label1.Name = "label1";
            // 
            // labelMax
            // 
            resources.ApplyResources(this.labelMax, "labelMax");
            this.labelMax.Name = "labelMax";
            // 
            // spinner
            // 
            resources.ApplyResources(this.spinner, "spinner");
            this.spinner.Name = "spinner";
            this.spinner.TabStop = false;
            // 
            // numericUpDownCapacity
            // 
            resources.ApplyResources(this.numericUpDownCapacity, "numericUpDownCapacity");
            this.numericUpDownCapacity.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericUpDownCapacity.Name = "numericUpDownCapacity";
            this.numericUpDownCapacity.ValueChanged += new System.EventHandler(this.numericUpDownCapacity_ValueChanged);
            // 
            // labelNumberOfServers
            // 
            resources.ApplyResources(this.labelNumberOfServers, "labelNumberOfServers");
            this.labelNumberOfServers.Name = "labelNumberOfServers";
            // 
            // HaNtolIndicator
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBoxControls);
            this.Name = "HaNtolIndicator";
            this.groupBoxControls.ResumeLayout(false);
            this.groupBoxControls.PerformLayout();
            this.tableServerFailureLimit.ResumeLayout(false);
            this.tableServerFailureLimit.PerformLayout();
            this.tableStatus.ResumeLayout(false);
            this.tableStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCapacity)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxControls;
        private System.Windows.Forms.TableLayoutPanel tableServerFailureLimit;
        private System.Windows.Forms.TableLayoutPanel tableStatus;
        private System.Windows.Forms.PictureBox pictureBoxStatus;
        private XenAdmin.Controls.Common.AutoHeightLabel labelStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelMax;
        private System.Windows.Forms.PictureBox spinner;
        private System.Windows.Forms.NumericUpDown numericUpDownCapacity;
        private System.Windows.Forms.Label labelNumberOfServers;
    }
}
