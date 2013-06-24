namespace XenAdmin.Controls.Ballooning
{
    partial class MemorySpinner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MemorySpinner));
            this.MB = new System.Windows.Forms.Label();
            this.Spinner = new System.Windows.Forms.NumericUpDown();
            this.NameLabel = new System.Windows.Forms.Label();
            this.iconBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Spinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconBox)).BeginInit();
            this.SuspendLayout();
            // 
            // MB
            // 
            resources.ApplyResources(this.MB, "MB");
            this.MB.Name = "MB";
            // 
            // Spinner
            // 
            resources.ApplyResources(this.Spinner, "Spinner");
            this.Spinner.Increment = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.Spinner.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.Spinner.Name = "Spinner";
            this.Spinner.ValueChanged += new System.EventHandler(this.Spinner_ValueChanged);
            this.Spinner.Leave += new System.EventHandler(this.Spinner_Leave);
            // 
            // NameLabel
            // 
            resources.ApplyResources(this.NameLabel, "NameLabel");
            this.NameLabel.Name = "NameLabel";
            // 
            // iconBox
            // 
            resources.ApplyResources(this.iconBox, "iconBox");
            this.iconBox.Name = "iconBox";
            this.iconBox.TabStop = false;
            // 
            // MemorySpinner
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.MB);
            this.Controls.Add(this.Spinner);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.iconBox);
            this.DoubleBuffered = true;
            this.Name = "MemorySpinner";
            ((System.ComponentModel.ISupportInitialize)(this.Spinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label MB;
        private System.Windows.Forms.NumericUpDown Spinner;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.PictureBox iconBox;
    }
}
