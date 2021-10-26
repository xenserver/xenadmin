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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.warningImage = new System.Windows.Forms.PictureBox();
            this.warningLabel = new System.Windows.Forms.Label();
            this.informationLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.informationImage = new System.Windows.Forms.PictureBox();
            this.informationLabel = new System.Windows.Forms.Label();
            this.helperLink = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningImage)).BeginInit();
            this.informationLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.informationImage)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.titleLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.information, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.informationLayoutPanel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.helperLink, 0, 4);
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
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.warningImage, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.warningLabel, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // warningImage
            // 
            resources.ApplyResources(this.warningImage, "warningImage");
            this.warningImage.Name = "warningImage";
            this.warningImage.TabStop = false;
            // 
            // warningLabel
            // 
            resources.ApplyResources(this.warningLabel, "warningLabel");
            this.warningLabel.AutoEllipsis = true;
            this.warningLabel.Name = "warningLabel";
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
            // helperLink
            // 
            this.helperLink.AutoEllipsis = true;
            resources.ApplyResources(this.helperLink, "helperLink");
            this.helperLink.Name = "helperLink";
            // 
            // SummaryPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SummaryPanel";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningImage)).EndInit();
            this.informationLayoutPanel.ResumeLayout(false);
            this.informationLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.informationImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox warningImage;
        private System.Windows.Forms.Label warningLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.LinkLabel helperLink;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.LinkLabel information;
        private System.Windows.Forms.TableLayoutPanel informationLayoutPanel;
        private System.Windows.Forms.PictureBox informationImage;
        private System.Windows.Forms.Label informationLabel;
    }
}
