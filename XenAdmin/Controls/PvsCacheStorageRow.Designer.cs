namespace XenAdmin.Controls
{
    partial class PvsCacheStorageRow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PvsCacheStorageRow));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelUnits = new System.Windows.Forms.Label();
            this.numericUpDownCacheSize = new System.Windows.Forms.NumericUpDown();
            this.comboBoxCacheSr = new XenAdmin.Controls.EnableableComboBox();
            this.labelHostName = new System.Windows.Forms.Label();
            this.labelCacheStorage = new System.Windows.Forms.Label();
            this.labelCacheSize = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCacheSize)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelUnits, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownCacheSize, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxCacheSr, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelHostName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelCacheStorage, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelCacheSize, 3, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelUnits
            // 
            resources.ApplyResources(this.labelUnits, "labelUnits");
            this.labelUnits.Name = "labelUnits";
            // 
            // numericUpDownCacheSize
            // 
            resources.ApplyResources(this.numericUpDownCacheSize, "numericUpDownCacheSize");
            this.numericUpDownCacheSize.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownCacheSize.Name = "numericUpDownCacheSize";
            this.numericUpDownCacheSize.ValueChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // comboBoxCacheSr
            // 
            resources.ApplyResources(this.comboBoxCacheSr, "comboBoxCacheSr");
            this.comboBoxCacheSr.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxCacheSr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCacheSr.FormattingEnabled = true;
            this.comboBoxCacheSr.Name = "comboBoxCacheSr";
            this.comboBoxCacheSr.SelectedIndexChanged += new System.EventHandler(this.comboBoxCacheSr_SelectedIndexChanged);
            // 
            // labelHostName
            // 
            this.labelHostName.AutoEllipsis = true;
            resources.ApplyResources(this.labelHostName, "labelHostName");
            this.labelHostName.Name = "labelHostName";
            // 
            // labelCacheStorage
            // 
            resources.ApplyResources(this.labelCacheStorage, "labelCacheStorage");
            this.labelCacheStorage.Name = "labelCacheStorage";
            // 
            // labelCacheSize
            // 
            resources.ApplyResources(this.labelCacheSize, "labelCacheSize");
            this.labelCacheSize.Name = "labelCacheSize";
            // 
            // PvsCacheStorageRow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PvsCacheStorageRow";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCacheSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private EnableableComboBox comboBoxCacheSr;
        private System.Windows.Forms.Label labelHostName;
        private System.Windows.Forms.Label labelCacheStorage;
        private System.Windows.Forms.Label labelCacheSize;
        private System.Windows.Forms.Label labelUnits;
        private System.Windows.Forms.NumericUpDown numericUpDownCacheSize;
    }
}
