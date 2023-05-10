namespace XenAdmin.SettingsPanels
{
    partial class LivePatchingEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LivePatchingEditPage));
            this.radioButtonDisable = new System.Windows.Forms.RadioButton();
            this.radioButtonEnable = new System.Windows.Forms.RadioButton();
            this.labelRubric = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonDisable
            // 
            resources.ApplyResources(this.radioButtonDisable, "radioButtonDisable");
            this.radioButtonDisable.Name = "radioButtonDisable";
            this.radioButtonDisable.TabStop = true;
            this.radioButtonDisable.UseVisualStyleBackColor = true;
            // 
            // radioButtonEnable
            // 
            resources.ApplyResources(this.radioButtonEnable, "radioButtonEnable");
            this.radioButtonEnable.Name = "radioButtonEnable";
            this.radioButtonEnable.TabStop = true;
            this.radioButtonEnable.UseVisualStyleBackColor = true;
            // 
            // labelRubric
            // 
            resources.ApplyResources(this.labelRubric, "labelRubric");
            this.labelRubric.BackColor = System.Drawing.Color.Transparent;
            this.labelRubric.Name = "labelRubric";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelRubric, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // LivePatchingEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.radioButtonDisable);
            this.Controls.Add(this.radioButtonEnable);
            this.Name = "LivePatchingEditPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonDisable;
        private System.Windows.Forms.RadioButton radioButtonEnable;
        private System.Windows.Forms.Label labelRubric;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

    }
}
