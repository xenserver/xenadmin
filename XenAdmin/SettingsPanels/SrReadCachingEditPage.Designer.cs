namespace XenAdmin.SettingsPanels
{
    partial class SrReadCachingEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SrReadCachingEditPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelTitle = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.checkBoxEnableReadCaching = new System.Windows.Forms.CheckBox();
            this.tableLayoutInfo = new System.Windows.Forms.TableLayoutPanel();
            this.linkLabelTellMeMore = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelChangeInfo = new System.Windows.Forms.Label();
            this.pictureBoxInfo2 = new System.Windows.Forms.PictureBox();
            this.pictureBoxInfo1 = new System.Windows.Forms.PictureBox();
            this.labelVdiInfo = new System.Windows.Forms.Label();
            this.labelMemoryInfo = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelTitle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxEnableReadCaching, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutInfo, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            this.tableLayoutPanel1.SetColumnSpan(this.labelTitle, 2);
            this.labelTitle.Name = "labelTitle";
            // 
            // checkBoxEnableReadCaching
            // 
            resources.ApplyResources(this.checkBoxEnableReadCaching, "checkBoxEnableReadCaching");
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxEnableReadCaching, 2);
            this.checkBoxEnableReadCaching.Name = "checkBoxEnableReadCaching";
            this.checkBoxEnableReadCaching.UseVisualStyleBackColor = true;
            // 
            // tableLayoutInfo
            // 
            resources.ApplyResources(this.tableLayoutInfo, "tableLayoutInfo");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutInfo, 2);
            this.tableLayoutInfo.Controls.Add(this.linkLabelTellMeMore, 0, 3);
            this.tableLayoutInfo.Controls.Add(this.pictureBox1, 0, 2);
            this.tableLayoutInfo.Controls.Add(this.labelChangeInfo, 0, 2);
            this.tableLayoutInfo.Controls.Add(this.pictureBoxInfo2, 0, 1);
            this.tableLayoutInfo.Controls.Add(this.pictureBoxInfo1, 0, 0);
            this.tableLayoutInfo.Controls.Add(this.labelVdiInfo, 1, 0);
            this.tableLayoutInfo.Controls.Add(this.labelMemoryInfo, 1, 1);
            this.tableLayoutInfo.Name = "tableLayoutInfo";
            // 
            // linkLabelTellMeMore
            // 
            resources.ApplyResources(this.linkLabelTellMeMore, "linkLabelTellMeMore");
            this.tableLayoutInfo.SetColumnSpan(this.linkLabelTellMeMore, 2);
            this.linkLabelTellMeMore.Name = "linkLabelTellMeMore";
            this.linkLabelTellMeMore.TabStop = true;
            this.linkLabelTellMeMore.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelTellMeMore_LinkClicked);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // labelChangeInfo
            // 
            resources.ApplyResources(this.labelChangeInfo, "labelChangeInfo");
            this.labelChangeInfo.Name = "labelChangeInfo";
            // 
            // pictureBoxInfo2
            // 
            resources.ApplyResources(this.pictureBoxInfo2, "pictureBoxInfo2");
            this.pictureBoxInfo2.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBoxInfo2.Name = "pictureBoxInfo2";
            this.pictureBoxInfo2.TabStop = false;
            // 
            // pictureBoxInfo1
            // 
            resources.ApplyResources(this.pictureBoxInfo1, "pictureBoxInfo1");
            this.pictureBoxInfo1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBoxInfo1.Name = "pictureBoxInfo1";
            this.pictureBoxInfo1.TabStop = false;
            // 
            // labelVdiInfo
            // 
            resources.ApplyResources(this.labelVdiInfo, "labelVdiInfo");
            this.labelVdiInfo.Name = "labelVdiInfo";
            // 
            // labelMemoryInfo
            // 
            resources.ApplyResources(this.labelMemoryInfo, "labelMemoryInfo");
            this.labelMemoryInfo.Name = "labelMemoryInfo";
            // 
            // SrReadCachingEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SrReadCachingEditPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutInfo.ResumeLayout(false);
            this.tableLayoutInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Controls.Common.AutoHeightLabel labelTitle;
        private System.Windows.Forms.CheckBox checkBoxEnableReadCaching;
        private System.Windows.Forms.TableLayoutPanel tableLayoutInfo;
        private System.Windows.Forms.PictureBox pictureBoxInfo2;
        private System.Windows.Forms.PictureBox pictureBoxInfo1;
        private System.Windows.Forms.Label labelVdiInfo;
        private System.Windows.Forms.Label labelMemoryInfo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelChangeInfo;
        private System.Windows.Forms.LinkLabel linkLabelTellMeMore;
    }
}
