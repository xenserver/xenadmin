namespace XenAdmin.SettingsPanels
{
    partial class SecurityEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SecurityEditPage));
            this.labelRubric = new System.Windows.Forms.Label();
            this.radioButtonTLS = new System.Windows.Forms.RadioButton();
            this.radioButtonSSL = new System.Windows.Forms.RadioButton();
            this.pictureBoxDisruption = new System.Windows.Forms.PictureBox();
            this.labelDisruption = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDisruption)).BeginInit();
            this.SuspendLayout();
            // 
            // labelRubric
            // 
            resources.ApplyResources(this.labelRubric, "labelRubric");
            this.labelRubric.Name = "labelRubric";
            // 
            // radioButtonTLS
            // 
            resources.ApplyResources(this.radioButtonTLS, "radioButtonTLS");
            this.radioButtonTLS.Name = "radioButtonTLS";
            this.radioButtonTLS.TabStop = true;
            this.radioButtonTLS.UseVisualStyleBackColor = true;
            this.radioButtonTLS.CheckedChanged += new System.EventHandler(this.radioButtonTLS_CheckedChanged);
            // 
            // radioButtonSSL
            // 
            resources.ApplyResources(this.radioButtonSSL, "radioButtonSSL");
            this.radioButtonSSL.Name = "radioButtonSSL";
            this.radioButtonSSL.TabStop = true;
            this.radioButtonSSL.UseVisualStyleBackColor = true;
            // 
            // pictureBoxDisruption
            // 
            this.pictureBoxDisruption.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.pictureBoxDisruption, "pictureBoxDisruption");
            this.pictureBoxDisruption.Name = "pictureBoxDisruption";
            this.pictureBoxDisruption.TabStop = false;
            // 
            // labelDisruption
            // 
            resources.ApplyResources(this.labelDisruption, "labelDisruption");
            this.labelDisruption.Name = "labelDisruption";
            // 
            // SecurityEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.labelDisruption);
            this.Controls.Add(this.pictureBoxDisruption);
            this.Controls.Add(this.radioButtonSSL);
            this.Controls.Add(this.radioButtonTLS);
            this.Controls.Add(this.labelRubric);
            this.Name = "SecurityEditPage";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDisruption)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelRubric;
        private System.Windows.Forms.RadioButton radioButtonTLS;
        private System.Windows.Forms.RadioButton radioButtonSSL;
        private System.Windows.Forms.PictureBox pictureBoxDisruption;
        private System.Windows.Forms.Label labelDisruption;
    }
}
