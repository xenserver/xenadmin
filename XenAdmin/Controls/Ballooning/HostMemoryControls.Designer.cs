namespace XenAdmin.Controls.Ballooning
{
    partial class HostMemoryControls
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HostMemoryControls));
            this.labelTotal = new System.Windows.Forms.Label();
            this.labelUsed = new System.Windows.Forms.Label();
            this.labelAvail = new System.Windows.Forms.Label();
            this.labelTotDynMax = new System.Windows.Forms.Label();
            this.valueTotal = new System.Windows.Forms.Label();
            this.valueUsed = new System.Windows.Forms.Label();
            this.valueAvail = new System.Windows.Forms.Label();
            this.valueTotDynMax = new System.Windows.Forms.Label();
            this.labelOvercommit = new System.Windows.Forms.Label();
            this.labelControlDomain = new System.Windows.Forms.Label();
            this.valueControlDomain = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.hostShinyBar = new XenAdmin.Controls.Ballooning.HostShinyBar();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTotal
            // 
            resources.ApplyResources(this.labelTotal, "labelTotal");
            this.labelTotal.Name = "labelTotal";
            // 
            // labelUsed
            // 
            resources.ApplyResources(this.labelUsed, "labelUsed");
            this.labelUsed.Name = "labelUsed";
            // 
            // labelAvail
            // 
            resources.ApplyResources(this.labelAvail, "labelAvail");
            this.labelAvail.Name = "labelAvail";
            // 
            // labelTotDynMax
            // 
            resources.ApplyResources(this.labelTotDynMax, "labelTotDynMax");
            this.labelTotDynMax.Name = "labelTotDynMax";
            // 
            // valueTotal
            // 
            resources.ApplyResources(this.valueTotal, "valueTotal");
            this.valueTotal.Name = "valueTotal";
            // 
            // valueUsed
            // 
            resources.ApplyResources(this.valueUsed, "valueUsed");
            this.valueUsed.Name = "valueUsed";
            // 
            // valueAvail
            // 
            resources.ApplyResources(this.valueAvail, "valueAvail");
            this.valueAvail.Name = "valueAvail";
            // 
            // valueTotDynMax
            // 
            resources.ApplyResources(this.valueTotDynMax, "valueTotDynMax");
            this.valueTotDynMax.Name = "valueTotDynMax";
            // 
            // labelOvercommit
            // 
            resources.ApplyResources(this.labelOvercommit, "labelOvercommit");
            this.tableLayoutPanel2.SetColumnSpan(this.labelOvercommit, 2);
            this.labelOvercommit.Name = "labelOvercommit";
            // 
            // labelControlDomain
            // 
            resources.ApplyResources(this.labelControlDomain, "labelControlDomain");
            this.labelControlDomain.Name = "labelControlDomain";
            // 
            // valueControlDomain
            // 
            resources.ApplyResources(this.valueControlDomain, "valueControlDomain");
            this.valueControlDomain.DisabledLinkColor = System.Drawing.SystemColors.ControlText;
            this.valueControlDomain.Name = "valueControlDomain";
            this.valueControlDomain.TabStop = true;
            this.valueControlDomain.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.valueControlDomain_LinkClicked);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.hostShinyBar, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.labelTotal, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.valueControlDomain, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelOvercommit, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.valueTotDynMax, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.valueAvail, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.valueUsed, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.valueTotal, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelUsed, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelControlDomain, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelAvail, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.labelTotDynMax, 0, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // hostShinyBar
            // 
            resources.ApplyResources(this.hostShinyBar, "hostShinyBar");
            this.hostShinyBar.Name = "hostShinyBar";
            // 
            // HostMemoryControls
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "HostMemoryControls";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private HostShinyBar hostShinyBar;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.Label labelUsed;
        private System.Windows.Forms.Label labelAvail;
        private System.Windows.Forms.Label labelTotDynMax;
        private System.Windows.Forms.Label valueTotal;
        private System.Windows.Forms.Label valueUsed;
        private System.Windows.Forms.Label valueAvail;
        private System.Windows.Forms.Label valueTotDynMax;
        private System.Windows.Forms.Label labelOvercommit;
        private System.Windows.Forms.Label labelControlDomain;
        private System.Windows.Forms.LinkLabel valueControlDomain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}
