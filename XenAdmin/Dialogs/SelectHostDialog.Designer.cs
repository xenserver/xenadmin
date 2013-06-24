namespace XenAdmin.Dialogs
{
    partial class SelectHostDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectHostDialog));
            this.poolHostPicker1 = new XenAdmin.Controls.PoolHostPicker();
            this.connectbutton = new System.Windows.Forms.Button();
            this.cancelbutton = new System.Windows.Forms.Button();
            this.okbutton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // poolHostPicker1
            // 
            resources.ApplyResources(this.poolHostPicker1, "poolHostPicker1");
            this.poolHostPicker1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.poolHostPicker1.FormattingEnabled = true;
            this.poolHostPicker1.Name = "poolHostPicker1";
            this.poolHostPicker1.NodeIndent = 19;
            this.poolHostPicker1.ShowCheckboxes = false;
            this.poolHostPicker1.ShowDescription = true;
            this.poolHostPicker1.ShowImages = true;
            // 
            // connectbutton
            // 
            resources.ApplyResources(this.connectbutton, "connectbutton");
            this.connectbutton.Image = global::XenAdmin.Properties.Resources._000_AddApplicationServer_h32bit_16;
            this.connectbutton.Name = "connectbutton";
            this.connectbutton.UseVisualStyleBackColor = true;
            this.connectbutton.Click += new System.EventHandler(this.button3_Click);
            // 
            // cancelbutton
            // 
            resources.ApplyResources(this.cancelbutton, "cancelbutton");
            this.cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelbutton.Name = "cancelbutton";
            this.cancelbutton.UseVisualStyleBackColor = true;
            this.cancelbutton.Click += new System.EventHandler(this.cancelbutton_Click);
            // 
            // okbutton
            // 
            resources.ApplyResources(this.okbutton, "okbutton");
            this.okbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okbutton.Name = "okbutton";
            this.okbutton.UseVisualStyleBackColor = true;
            this.okbutton.Click += new System.EventHandler(this.okbutton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources.licensekey_32;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // SelectHostDialog
            // 
            this.AcceptButton = this.okbutton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelbutton;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okbutton);
            this.Controls.Add(this.connectbutton);
            this.Controls.Add(this.cancelbutton);
            this.Controls.Add(this.poolHostPicker1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "SelectHostDialog";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.PoolHostPicker poolHostPicker1;
        private System.Windows.Forms.Button connectbutton;
        private System.Windows.Forms.Button cancelbutton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Button okbutton;
    }
}