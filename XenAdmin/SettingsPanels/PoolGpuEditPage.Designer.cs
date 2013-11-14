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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PoolGpuEditPage));
            this.labelRubric = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonMixture = new System.Windows.Forms.RadioButton();
            this.radioButtonDepth = new System.Windows.Forms.RadioButton();
            this.radioButtonBreadth = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelRubric
            // 
            resources.ApplyResources(this.labelRubric, "labelRubric");
            this.labelRubric.Name = "labelRubric";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.radioButtonMixture, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelRubric, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonDepth, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonBreadth, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // radioButtonMixture
            // 
            resources.ApplyResources(this.radioButtonMixture, "radioButtonMixture");
            this.radioButtonMixture.Name = "radioButtonMixture";
            this.radioButtonMixture.TabStop = true;
            this.radioButtonMixture.UseVisualStyleBackColor = true;
            // 
            // radioButtonDepth
            // 
            resources.ApplyResources(this.radioButtonDepth, "radioButtonDepth");
            this.radioButtonDepth.Name = "radioButtonDepth";
            this.radioButtonDepth.TabStop = true;
            this.radioButtonDepth.UseVisualStyleBackColor = true;
            // 
            // radioButtonBreadth
            // 
            resources.ApplyResources(this.radioButtonBreadth, "radioButtonBreadth");
            this.radioButtonBreadth.Name = "radioButtonBreadth";
            this.radioButtonBreadth.TabStop = true;
            this.radioButtonBreadth.UseVisualStyleBackColor = true;
            // 
            // PoolGpuEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PoolGpuEditPage";
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
