namespace XenAdmin.Dialogs
{
    partial class PvsCacheConfigurationPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PvsCacheConfigurationPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cacheStorageInUseInfoLabel = new System.Windows.Forms.Label();
            this.memoryOnlyInfoLabel = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.hostsPanel = new XenAdmin.Controls.FlickerFreePanel();
            this.viewPvsServersButton = new System.Windows.Forms.Button();
            this.pvsConfigInfoIcon = new System.Windows.Forms.PictureBox();
            this.pvsConfigInfoLabel = new System.Windows.Forms.Label();
            this.memoryOnlyInfoIcon = new System.Windows.Forms.PictureBox();
            this.cacheStorageInUseInfoIcon = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pvsConfigInfoIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryOnlyInfoIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cacheStorageInUseInfoIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.cacheStorageInUseInfoLabel, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.memoryOnlyInfoLabel, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.deleteButton, 4, 6);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.hostsPanel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.viewPvsServersButton, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.pvsConfigInfoIcon, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.pvsConfigInfoLabel, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.memoryOnlyInfoIcon, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.cacheStorageInUseInfoIcon, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // cacheStorageInUseInfoLabel
            // 
            resources.ApplyResources(this.cacheStorageInUseInfoLabel, "cacheStorageInUseInfoLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.cacheStorageInUseInfoLabel, 3);
            this.cacheStorageInUseInfoLabel.Name = "cacheStorageInUseInfoLabel";
            // 
            // memoryOnlyInfoLabel
            // 
            resources.ApplyResources(this.memoryOnlyInfoLabel, "memoryOnlyInfoLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.memoryOnlyInfoLabel, 3);
            this.memoryOnlyInfoLabel.Name = "memoryOnlyInfoLabel";
            // 
            // deleteButton
            // 
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.Image = global::XenAdmin.Properties.Resources._000_RemoveSite_h32bit_16;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 4);
            this.label1.Name = "label1";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.tableLayoutPanel1.SetColumnSpan(this.label7, 2);
            this.label7.Name = "label7";
            // 
            // textBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBox1, 2);
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.TextChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // hostsPanel
            // 
            resources.ApplyResources(this.hostsPanel, "hostsPanel");
            this.hostsPanel.BorderColor = System.Drawing.Color.Black;
            this.hostsPanel.BorderWidth = 1;
            this.tableLayoutPanel1.SetColumnSpan(this.hostsPanel, 4);
            this.hostsPanel.Name = "hostsPanel";
            // 
            // viewPvsServersButton
            // 
            resources.ApplyResources(this.viewPvsServersButton, "viewPvsServersButton");
            this.viewPvsServersButton.Name = "viewPvsServersButton";
            this.viewPvsServersButton.UseVisualStyleBackColor = true;
            this.viewPvsServersButton.Click += new System.EventHandler(this.viewServersButton_Click);
            // 
            // pvsConfigInfoIcon
            // 
            resources.ApplyResources(this.pvsConfigInfoIcon, "pvsConfigInfoIcon");
            this.pvsConfigInfoIcon.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.pvsConfigInfoIcon.Name = "pvsConfigInfoIcon";
            this.pvsConfigInfoIcon.TabStop = false;
            // 
            // pvsConfigInfoLabel
            // 
            resources.ApplyResources(this.pvsConfigInfoLabel, "pvsConfigInfoLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.pvsConfigInfoLabel, 3);
            this.pvsConfigInfoLabel.Name = "pvsConfigInfoLabel";
            // 
            // memoryOnlyInfoIcon
            // 
            resources.ApplyResources(this.memoryOnlyInfoIcon, "memoryOnlyInfoIcon");
            this.memoryOnlyInfoIcon.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.memoryOnlyInfoIcon.Name = "memoryOnlyInfoIcon";
            this.memoryOnlyInfoIcon.TabStop = false;
            // 
            // cacheStorageInUseInfoIcon
            // 
            resources.ApplyResources(this.cacheStorageInUseInfoIcon, "cacheStorageInUseInfoIcon");
            this.cacheStorageInUseInfoIcon.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.cacheStorageInUseInfoIcon.Name = "cacheStorageInUseInfoIcon";
            this.cacheStorageInUseInfoIcon.TabStop = false;
            // 
            // PvsCacheConfigurationPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "PvsCacheConfigurationPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pvsConfigInfoIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryOnlyInfoIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cacheStorageInUseInfoIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox1;
        private Controls.FlickerFreePanel hostsPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button deleteButton;
        public System.Windows.Forms.Button viewPvsServersButton;
        private System.Windows.Forms.Label pvsConfigInfoLabel;
        private System.Windows.Forms.PictureBox pvsConfigInfoIcon;
        private System.Windows.Forms.Label memoryOnlyInfoLabel;
        private System.Windows.Forms.PictureBox memoryOnlyInfoIcon;
        private System.Windows.Forms.Label cacheStorageInUseInfoLabel;
        private System.Windows.Forms.PictureBox cacheStorageInUseInfoIcon;
    }
}
