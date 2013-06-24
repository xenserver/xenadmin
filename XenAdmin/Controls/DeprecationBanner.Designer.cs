namespace XenAdmin.Controls
{
    partial class DeprecationBanner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeprecationBanner));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.helperLink = new System.Windows.Forms.LinkLabel();
            this.message = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.Controls.Add(this.helperLink, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.message, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // helperLink
            // 
            resources.ApplyResources(this.helperLink, "helperLink");
            this.helperLink.Name = "helperLink";
            this.helperLink.TabStop = true;
            // 
            // message
            // 
            this.message.AutoEllipsis = true;
            resources.ApplyResources(this.message, "message");
            this.message.Name = "message";
            // 
            // DeprecationBanner
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Orange;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DeprecationBanner";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.LinkLabel helperLink;
        private XenAdmin.Controls.Common.AutoHeightLabel message;
    }
}
