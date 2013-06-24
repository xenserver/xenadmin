namespace XenAdmin.Controls
{
    partial class HaNtolIndicatorSimple
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HaNtolIndicatorSimple));
            this.labelMax = new System.Windows.Forms.Label();
            this.spinner = new System.Windows.Forms.PictureBox();
            this.labelNumberOfServers = new System.Windows.Forms.Label();
            this.pictureBoxStatus = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_tlpCalFailure = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.m_tlpMaxCapacity = new System.Windows.Forms.TableLayoutPanel();
            this.m_tlpTolerance = new System.Windows.Forms.TableLayoutPanel();
            this.labelStatus = new XenAdmin.Controls.Common.AutoHeightLabel();
            ((System.ComponentModel.ISupportInitialize)(this.spinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.m_tlpCalFailure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.m_tlpMaxCapacity.SuspendLayout();
            this.m_tlpTolerance.SuspendLayout();
            this.SuspendLayout();
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
            // labelNumberOfServers
            // 
            resources.ApplyResources(this.labelNumberOfServers, "labelNumberOfServers");
            this.labelNumberOfServers.Name = "labelNumberOfServers";
            // 
            // pictureBoxStatus
            // 
            this.pictureBoxStatus.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.pictureBoxStatus, "pictureBoxStatus");
            this.pictureBoxStatus.Name = "pictureBoxStatus";
            this.pictureBoxStatus.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.m_tlpCalFailure, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_tlpMaxCapacity, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_tlpTolerance, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // m_tlpCalFailure
            // 
            resources.ApplyResources(this.m_tlpCalFailure, "m_tlpCalFailure");
            this.m_tlpCalFailure.Controls.Add(this.label3, 1, 0);
            this.m_tlpCalFailure.Controls.Add(this.pictureBox2, 0, 0);
            this.m_tlpCalFailure.Name = "m_tlpCalFailure";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // m_tlpMaxCapacity
            // 
            resources.ApplyResources(this.m_tlpMaxCapacity, "m_tlpMaxCapacity");
            this.m_tlpMaxCapacity.Controls.Add(this.spinner, 1, 0);
            this.m_tlpMaxCapacity.Controls.Add(this.labelMax, 0, 0);
            this.m_tlpMaxCapacity.Name = "m_tlpMaxCapacity";
            // 
            // m_tlpTolerance
            // 
            resources.ApplyResources(this.m_tlpTolerance, "m_tlpTolerance");
            this.m_tlpTolerance.Controls.Add(this.pictureBoxStatus, 1, 0);
            this.m_tlpTolerance.Controls.Add(this.labelStatus, 2, 0);
            this.m_tlpTolerance.Controls.Add(this.labelNumberOfServers, 0, 0);
            this.m_tlpTolerance.Name = "m_tlpTolerance";
            // 
            // labelStatus
            // 
            resources.ApplyResources(this.labelStatus, "labelStatus");
            this.labelStatus.Name = "labelStatus";
            // 
            // HaNtolIndicatorSimple
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "HaNtolIndicatorSimple";
            ((System.ComponentModel.ISupportInitialize)(this.spinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.m_tlpCalFailure.ResumeLayout(false);
            this.m_tlpCalFailure.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.m_tlpMaxCapacity.ResumeLayout(false);
            this.m_tlpMaxCapacity.PerformLayout();
            this.m_tlpTolerance.ResumeLayout(false);
            this.m_tlpTolerance.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.PictureBox spinner;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.PictureBox pictureBoxStatus;
        private System.Windows.Forms.Label labelNumberOfServers;
		private System.Windows.Forms.Label labelMax;
        private XenAdmin.Controls.Common.AutoHeightLabel labelStatus;
		private System.Windows.Forms.TableLayoutPanel m_tlpCalFailure;
		private System.Windows.Forms.TableLayoutPanel m_tlpMaxCapacity;
		private System.Windows.Forms.TableLayoutPanel m_tlpTolerance;
    }
}
