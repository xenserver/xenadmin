namespace XenAdmin.Controls.SummaryPanel
{
    partial class SummaryPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SummaryPanel));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.information = new System.Windows.Forms.LinkLabel();
            this.supportWarningTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.supportWarningImage = new System.Windows.Forms.PictureBox();
            this.supportWarningLabel = new System.Windows.Forms.Label();
            this.licenseWarningTableLayoutPabel = new System.Windows.Forms.TableLayoutPanel();
            this.licenseWarningImage = new System.Windows.Forms.PictureBox();
            this.licenseWarningLabel = new System.Windows.Forms.Label();
            this.informationLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.informationImage = new System.Windows.Forms.PictureBox();
            this.informationLabel = new System.Windows.Forms.Label();
            this.licenseHelperLinkLabel = new System.Windows.Forms.LinkLabel();
            this.helperLinksFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.supportHelperLinkLabel = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.supportWarningTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.supportWarningImage)).BeginInit();
            this.licenseWarningTableLayoutPabel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.licenseWarningImage)).BeginInit();
            this.informationLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.informationImage)).BeginInit();
            this.helperLinksFlowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.titleLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.information, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.supportWarningTableLayoutPanel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.licenseWarningTableLayoutPabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.informationLayoutPanel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.helperLinksFlowLayoutPanel, 0, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // titleLabel
            // 
            resources.ApplyResources(this.titleLabel, "titleLabel");
            this.titleLabel.AutoEllipsis = true;
            this.titleLabel.Name = "titleLabel";
            // 
            // information
            // 
            resources.ApplyResources(this.information, "information");
            this.information.Name = "information";
            // 
            // supportWarningTableLayoutPanel
            // 
            resources.ApplyResources(this.supportWarningTableLayoutPanel, "supportWarningTableLayoutPanel");
            this.supportWarningTableLayoutPanel.Controls.Add(this.supportWarningImage, 0, 0);
            this.supportWarningTableLayoutPanel.Controls.Add(this.supportWarningLabel, 1, 0);
            this.supportWarningTableLayoutPanel.Name = "supportWarningTableLayoutPanel";
            // 
            // supportWarningImage
            // 
            resources.ApplyResources(this.supportWarningImage, "supportWarningImage");
            this.supportWarningImage.Name = "supportWarningImage";
            this.supportWarningImage.TabStop = false;
            // 
            // supportWarningLabel
            // 
            resources.ApplyResources(this.supportWarningLabel, "supportWarningLabel");
            this.supportWarningLabel.AutoEllipsis = true;
            this.supportWarningLabel.Name = "supportWarningLabel";
            // 
            // licenseWarningTableLayoutPabel
            // 
            resources.ApplyResources(this.licenseWarningTableLayoutPabel, "licenseWarningTableLayoutPabel");
            this.licenseWarningTableLayoutPabel.Controls.Add(this.licenseWarningImage, 0, 0);
            this.licenseWarningTableLayoutPabel.Controls.Add(this.licenseWarningLabel, 1, 0);
            this.licenseWarningTableLayoutPabel.Name = "licenseWarningTableLayoutPabel";
            // 
            // licenseWarningImage
            // 
            resources.ApplyResources(this.licenseWarningImage, "licenseWarningImage");
            this.licenseWarningImage.Name = "licenseWarningImage";
            this.licenseWarningImage.TabStop = false;
            // 
            // licenseWarningLabel
            // 
            resources.ApplyResources(this.licenseWarningLabel, "licenseWarningLabel");
            this.licenseWarningLabel.AutoEllipsis = true;
            this.licenseWarningLabel.Name = "licenseWarningLabel";
            // 
            // informationLayoutPanel
            // 
            resources.ApplyResources(this.informationLayoutPanel, "informationLayoutPanel");
            this.informationLayoutPanel.Controls.Add(this.informationImage, 0, 0);
            this.informationLayoutPanel.Controls.Add(this.informationLabel, 1, 0);
            this.informationLayoutPanel.Name = "informationLayoutPanel";
            // 
            // informationImage
            // 
            resources.ApplyResources(this.informationImage, "informationImage");
            this.informationImage.Name = "informationImage";
            this.informationImage.TabStop = false;
            // 
            // informationLabel
            // 
            resources.ApplyResources(this.informationLabel, "informationLabel");
            this.informationLabel.AutoEllipsis = true;
            this.informationLabel.Name = "informationLabel";
            // 
            // licenseHelperLinkLabel
            // 
            this.licenseHelperLinkLabel.AutoEllipsis = true;
            resources.ApplyResources(this.licenseHelperLinkLabel, "licenseHelperLinkLabel");
            this.licenseHelperLinkLabel.Name = "licenseHelperLinkLabel";
            // 
            // helperLinksFlowLayoutPanel
            // 
            this.helperLinksFlowLayoutPanel.Controls.Add(this.licenseHelperLinkLabel);
            this.helperLinksFlowLayoutPanel.Controls.Add(this.supportHelperLinkLabel);
            resources.ApplyResources(this.helperLinksFlowLayoutPanel, "helperLinksFlowLayoutPanel");
            this.helperLinksFlowLayoutPanel.Name = "helperLinksFlowLayoutPanel";
            // 
            // supportHelperLinkLabel
            // 
            this.supportHelperLinkLabel.AutoEllipsis = true;
            resources.ApplyResources(this.supportHelperLinkLabel, "supportHelperLinkLabel");
            this.supportHelperLinkLabel.Name = "supportHelperLinkLabel";
            // 
            // SummaryPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SummaryPanel";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.supportWarningTableLayoutPanel.ResumeLayout(false);
            this.supportWarningTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.supportWarningImage)).EndInit();
            this.licenseWarningTableLayoutPabel.ResumeLayout(false);
            this.licenseWarningTableLayoutPabel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.licenseWarningImage)).EndInit();
            this.informationLayoutPanel.ResumeLayout(false);
            this.informationLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.informationImage)).EndInit();
            this.helperLinksFlowLayoutPanel.ResumeLayout(false);
            this.helperLinksFlowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox licenseWarningImage;
        private System.Windows.Forms.Label licenseWarningLabel;
        private System.Windows.Forms.TableLayoutPanel licenseWarningTableLayoutPabel;
        private System.Windows.Forms.LinkLabel licenseHelperLinkLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.LinkLabel information;
        private System.Windows.Forms.TableLayoutPanel informationLayoutPanel;
        private System.Windows.Forms.PictureBox informationImage;
        private System.Windows.Forms.Label informationLabel;
        private System.Windows.Forms.TableLayoutPanel supportWarningTableLayoutPanel;
        private System.Windows.Forms.PictureBox supportWarningImage;
        private System.Windows.Forms.Label supportWarningLabel;
        private System.Windows.Forms.FlowLayoutPanel helperLinksFlowLayoutPanel;
        private System.Windows.Forms.LinkLabel supportHelperLinkLabel;
    }
}
