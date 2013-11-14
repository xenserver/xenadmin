namespace XenAdmin.SettingsPanels
{
    partial class PoolGpuEditPage
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
            this.labelRubric = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonDepth = new System.Windows.Forms.RadioButton();
            this.radioButtonBreadth = new System.Windows.Forms.RadioButton();
            this.radioButtonMixture = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelRubric
            // 
            this.labelRubric.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRubric.AutoSize = true;
            this.labelRubric.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelRubric.Location = new System.Drawing.Point(3, 0);
            this.labelRubric.Name = "labelRubric";
            this.labelRubric.Padding = new System.Windows.Forms.Padding(0, 3, 0, 10);
            this.labelRubric.Size = new System.Drawing.Size(412, 39);
            this.labelRubric.TabIndex = 2;
            this.labelRubric.Text = "Set a placement policy for assigning VMs to GPUs to achieve either maximum densit" +
                "y or maximum performance.";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.radioButtonMixture, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelRubric, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonDepth, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonBreadth, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(418, 289);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // radioButtonDepth
            // 
            this.radioButtonDepth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonDepth.AutoSize = true;
            this.radioButtonDepth.Location = new System.Drawing.Point(3, 42);
            this.radioButtonDepth.Name = "radioButtonDepth";
            this.radioButtonDepth.Size = new System.Drawing.Size(412, 17);
            this.radioButtonDepth.TabIndex = 3;
            this.radioButtonDepth.TabStop = true;
            this.radioButtonDepth.Text = "Maximum density: put as many VMs as possible on the same GPU";
            this.radioButtonDepth.UseVisualStyleBackColor = true;
            // 
            // radioButtonBreadth
            // 
            this.radioButtonBreadth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonBreadth.AutoSize = true;
            this.radioButtonBreadth.Location = new System.Drawing.Point(3, 65);
            this.radioButtonBreadth.Name = "radioButtonBreadth";
            this.radioButtonBreadth.Size = new System.Drawing.Size(412, 17);
            this.radioButtonBreadth.TabIndex = 4;
            this.radioButtonBreadth.TabStop = true;
            this.radioButtonBreadth.Text = "Maximum performance: put VMs on as many GPUs as possible";
            this.radioButtonBreadth.UseVisualStyleBackColor = true;
            // 
            // radioButtonMixture
            // 
            this.radioButtonMixture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonMixture.AutoSize = true;
            this.radioButtonMixture.Enabled = false;
            this.radioButtonMixture.Location = new System.Drawing.Point(3, 88);
            this.radioButtonMixture.Name = "radioButtonMixture";
            this.radioButtonMixture.Size = new System.Drawing.Size(412, 17);
            this.radioButtonMixture.TabIndex = 5;
            this.radioButtonMixture.TabStop = true;
            this.radioButtonMixture.Text = "Unknown or mixture";
            this.radioButtonMixture.UseVisualStyleBackColor = true;
            // 
            // PoolGpuEditPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PoolGpuEditPage";
            this.Size = new System.Drawing.Size(418, 289);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelRubric;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RadioButton radioButtonDepth;
        private System.Windows.Forms.RadioButton radioButtonBreadth;
        private System.Windows.Forms.RadioButton radioButtonMixture;
    }
}
