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
            this.unitsTotal = new System.Windows.Forms.Label();
            this.unitsUsed = new System.Windows.Forms.Label();
            this.unitsTotDynMax = new System.Windows.Forms.Label();
            this.labelOvercommit = new System.Windows.Forms.Label();
            this.labelControlDomain = new System.Windows.Forms.Label();
            this.valueControlDomain = new System.Windows.Forms.LinkLabel();
            this.hostShinyBar = new XenAdmin.Controls.Ballooning.HostShinyBar();
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
            // unitsTotal
            // 
            resources.ApplyResources(this.unitsTotal, "unitsTotal");
            this.unitsTotal.Name = "unitsTotal";
            // 
            // unitsUsed
            // 
            resources.ApplyResources(this.unitsUsed, "unitsUsed");
            this.unitsUsed.Name = "unitsUsed";
            // 
            // unitsTotDynMax
            // 
            resources.ApplyResources(this.unitsTotDynMax, "unitsTotDynMax");
            this.unitsTotDynMax.Name = "unitsTotDynMax";
            // 
            // labelOvercommit
            // 
            resources.ApplyResources(this.labelOvercommit, "labelOvercommit");
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
            this.valueControlDomain.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.valueControlDomain_LinkClicked);
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
            this.Controls.Add(this.valueControlDomain);
            this.Controls.Add(this.labelControlDomain);
            this.Controls.Add(this.labelOvercommit);
            this.Controls.Add(this.unitsTotDynMax);
            this.Controls.Add(this.unitsUsed);
            this.Controls.Add(this.unitsTotal);
            this.Controls.Add(this.valueTotDynMax);
            this.Controls.Add(this.valueAvail);
            this.Controls.Add(this.valueUsed);
            this.Controls.Add(this.valueTotal);
            this.Controls.Add(this.labelTotDynMax);
            this.Controls.Add(this.labelAvail);
            this.Controls.Add(this.labelUsed);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.hostShinyBar);
            this.DoubleBuffered = true;
            this.Name = "HostMemoryControls";
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
        private System.Windows.Forms.Label unitsTotal;
        private System.Windows.Forms.Label unitsUsed;
        private System.Windows.Forms.Label unitsTotDynMax;
        private System.Windows.Forms.Label labelOvercommit;
        private System.Windows.Forms.Label labelControlDomain;
        private System.Windows.Forms.LinkLabel valueControlDomain;
    }
}
