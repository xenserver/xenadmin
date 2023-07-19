
namespace XenAdmin.Controls
{
    partial class ClientIdControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientIdControl));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textBoxClientIdFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabelClientIdUrl = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanelInfo = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxInfo = new System.Windows.Forms.PictureBox();
            this.labelInfo = new System.Windows.Forms.Label();
            this.tooltipValidation = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.buttonBrowse, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxClientIdFile, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.linkLabelClientIdUrl, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelInfo, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // buttonBrowse
            // 
            resources.ApplyResources(this.buttonBrowse, "buttonBrowse");
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // textBoxClientIdFile
            // 
            resources.ApplyResources(this.textBoxClientIdFile, "textBoxClientIdFile");
            this.textBoxClientIdFile.Name = "textBoxClientIdFile";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // linkLabelClientIdUrl
            // 
            resources.ApplyResources(this.linkLabelClientIdUrl, "linkLabelClientIdUrl");
            this.tableLayoutPanel1.SetColumnSpan(this.linkLabelClientIdUrl, 3);
            this.linkLabelClientIdUrl.Name = "linkLabelClientIdUrl";
            this.linkLabelClientIdUrl.TabStop = true;
            this.linkLabelClientIdUrl.Click += new System.EventHandler(this.linkLabelClientIdUrl_Click);
            // 
            // tableLayoutPanelInfo
            // 
            resources.ApplyResources(this.tableLayoutPanelInfo, "tableLayoutPanelInfo");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelInfo, 3);
            this.tableLayoutPanelInfo.Controls.Add(this.pictureBoxInfo, 0, 0);
            this.tableLayoutPanelInfo.Controls.Add(this.labelInfo, 1, 0);
            this.tableLayoutPanelInfo.Name = "tableLayoutPanelInfo";
            // 
            // pictureBoxInfo
            // 
            this.pictureBoxInfo.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.pictureBoxInfo, "pictureBoxInfo");
            this.pictureBoxInfo.Name = "pictureBoxInfo";
            this.pictureBoxInfo.TabStop = false;
            // 
            // labelInfo
            // 
            resources.ApplyResources(this.labelInfo, "labelInfo");
            this.labelInfo.Name = "labelInfo";
            // 
            // tooltipValidation
            // 
            this.tooltipValidation.IsBalloon = true;
            this.tooltipValidation.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            // 
            // ClientIdControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ClientIdControl";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelInfo.ResumeLayout(false);
            this.tableLayoutPanelInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.TextBox textBoxClientIdFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabelClientIdUrl;
        private System.Windows.Forms.ToolTip tooltipValidation;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelInfo;
        private System.Windows.Forms.PictureBox pictureBoxInfo;
        private System.Windows.Forms.Label labelInfo;
    }
}
