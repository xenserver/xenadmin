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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.warningIcon = new System.Windows.Forms.PictureBox();
            this.warningText = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.information = new System.Windows.Forms.Label();
            this.helperLink = new System.Windows.Forms.LinkLabel();
            this.informationLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.informationImage = new System.Windows.Forms.PictureBox();
            this.informationMessage = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningIcon)).BeginInit();
            this.informationLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.informationImage)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.titleLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.information, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.helperLink, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.informationLayoutPanel, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(235, 295);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 183F));
            this.tableLayoutPanel2.Controls.Add(this.warningIcon, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.warningText, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 72);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(229, 41);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // warningIcon
            // 
            this.warningIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.warningIcon.Location = new System.Drawing.Point(3, 3);
            this.warningIcon.Name = "warningIcon";
            this.warningIcon.Size = new System.Drawing.Size(40, 35);
            this.warningIcon.TabIndex = 0;
            this.warningIcon.TabStop = false;
            // 
            // warningText
            // 
            this.warningText.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.warningText.AutoEllipsis = true;
            this.warningText.AutoSize = true;
            this.warningText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.warningText.Location = new System.Drawing.Point(49, 14);
            this.warningText.Name = "warningText";
            this.warningText.Size = new System.Drawing.Size(0, 13);
            this.warningText.TabIndex = 1;
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.titleLabel.AutoEllipsis = true;
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(8, 13);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(8, 8, 3, 8);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(0, 13);
            this.titleLabel.TabIndex = 4;
            // 
            // information
            // 
            this.information.AutoSize = true;
            this.information.Location = new System.Drawing.Point(8, 48);
            this.information.Margin = new System.Windows.Forms.Padding(8);
            this.information.Name = "information";
            this.information.Size = new System.Drawing.Size(0, 13);
            this.information.TabIndex = 5;
            // 
            // helperLink
            // 
            this.helperLink.AutoEllipsis = true;
            this.helperLink.AutoSize = true;
            this.helperLink.Location = new System.Drawing.Point(8, 270);
            this.helperLink.Margin = new System.Windows.Forms.Padding(8, 0, 0, 8);
            this.helperLink.Name = "helperLink";
            this.helperLink.Size = new System.Drawing.Size(0, 13);
            this.helperLink.TabIndex = 3;
            // 
            // informationLayoutPanel
            // 
            this.informationLayoutPanel.ColumnCount = 2;
            this.informationLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.informationLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.informationLayoutPanel.Controls.Add(this.informationImage, 0, 0);
            this.informationLayoutPanel.Controls.Add(this.informationMessage, 1, 0);
            this.informationLayoutPanel.Location = new System.Drawing.Point(3, 119);
            this.informationLayoutPanel.Name = "informationLayoutPanel";
            this.informationLayoutPanel.RowCount = 1;
            this.informationLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.informationLayoutPanel.Size = new System.Drawing.Size(229, 40);
            this.informationLayoutPanel.TabIndex = 6;
            // 
            // informationImage
            // 
            this.informationImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.informationImage.Location = new System.Drawing.Point(3, 3);
            this.informationImage.Name = "informationImage";
            this.informationImage.Size = new System.Drawing.Size(33, 34);
            this.informationImage.TabIndex = 0;
            this.informationImage.TabStop = false;
            // 
            // informationMessage
            // 
            this.informationMessage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.informationMessage.AutoEllipsis = true;
            this.informationMessage.AutoSize = true;
            this.informationMessage.Location = new System.Drawing.Point(42, 13);
            this.informationMessage.Name = "informationMessage";
            this.informationMessage.Size = new System.Drawing.Size(0, 13);
            this.informationMessage.TabIndex = 1;
            // 
            // SummaryPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SummaryPanel";
            this.Size = new System.Drawing.Size(235, 295);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningIcon)).EndInit();
            this.informationLayoutPanel.ResumeLayout(false);
            this.informationLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.informationImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox warningIcon;
        private System.Windows.Forms.Label warningText;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.LinkLabel helperLink;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label information;
        private System.Windows.Forms.TableLayoutPanel informationLayoutPanel;
        private System.Windows.Forms.PictureBox informationImage;
        private System.Windows.Forms.Label informationMessage;
    }
}
