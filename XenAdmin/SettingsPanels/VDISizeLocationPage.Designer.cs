namespace XenAdmin.SettingsPanels
{
    partial class VDISizeLocationPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VDISizeLocationPage));
            this.SizeLabel = new System.Windows.Forms.Label();
            this.locationLabel = new System.Windows.Forms.Label();
            this.sizeNUD = new System.Windows.Forms.NumericUpDown();
            this.sizeValue = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.sizeValueROLabel = new System.Windows.Forms.Label();
            this.labelLocationValueRO = new System.Windows.Forms.Label();
            this.pictureBoxError = new System.Windows.Forms.PictureBox();
            this.labelError = new System.Windows.Forms.Label();
            this.initial_allocation_label = new System.Windows.Forms.Label();
            this.incremental_allocation_label = new System.Windows.Forms.Label();
            this.initial_alloc_value = new System.Windows.Forms.Label();
            this.incr_alloc_value = new System.Windows.Forms.Label();
            this.panelShutDownHint = new System.Windows.Forms.Panel();
            this.labelShutDownWarning = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.sizeNUD)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).BeginInit();
            this.panelShutDownHint.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // SizeLabel
            // 
            resources.ApplyResources(this.SizeLabel, "SizeLabel");
            this.SizeLabel.Name = "SizeLabel";
            // 
            // locationLabel
            // 
            resources.ApplyResources(this.locationLabel, "locationLabel");
            this.locationLabel.Name = "locationLabel";
            // 
            // sizeNUD
            // 
            this.sizeNUD.DecimalPlaces = 3;
            resources.ApplyResources(this.sizeNUD, "sizeNUD");
            this.sizeNUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.sizeNUD.Maximum = new decimal(new int[] {
            -1,
            1,
            0,
            0});
            this.sizeNUD.Name = "sizeNUD";
            this.sizeNUD.ValueChanged += new System.EventHandler(this.sizeNUD_ValueChanged);
            // 
            // sizeValue
            // 
            resources.ApplyResources(this.sizeValue, "sizeValue");
            this.sizeValue.Name = "sizeValue";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.sizeValue, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.locationLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.SizeLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.sizeNUD, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.sizeValueROLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelLocationValueRO, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxError, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelError, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.initial_allocation_label, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.incremental_allocation_label, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.initial_alloc_value, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.incr_alloc_value, 2, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // sizeValueROLabel
            // 
            resources.ApplyResources(this.sizeValueROLabel, "sizeValueROLabel");
            this.sizeValueROLabel.Name = "sizeValueROLabel";
            // 
            // labelLocationValueRO
            // 
            resources.ApplyResources(this.labelLocationValueRO, "labelLocationValueRO");
            this.tableLayoutPanel1.SetColumnSpan(this.labelLocationValueRO, 5);
            this.labelLocationValueRO.Name = "labelLocationValueRO";
            // 
            // pictureBoxError
            // 
            resources.ApplyResources(this.pictureBoxError, "pictureBoxError");
            this.pictureBoxError.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.pictureBoxError.Name = "pictureBoxError";
            this.pictureBoxError.TabStop = false;
            // 
            // labelError
            // 
            resources.ApplyResources(this.labelError, "labelError");
            this.labelError.ForeColor = System.Drawing.Color.Red;
            this.labelError.Name = "labelError";
            // 
            // initial_allocation_label
            // 
            resources.ApplyResources(this.initial_allocation_label, "initial_allocation_label");
            this.initial_allocation_label.Name = "initial_allocation_label";
            // 
            // incremental_allocation_label
            // 
            resources.ApplyResources(this.incremental_allocation_label, "incremental_allocation_label");
            this.incremental_allocation_label.Name = "incremental_allocation_label";
            // 
            // initial_alloc_value
            // 
            resources.ApplyResources(this.initial_alloc_value, "initial_alloc_value");
            this.initial_alloc_value.Name = "initial_alloc_value";
            // 
            // incr_alloc_value
            // 
            resources.ApplyResources(this.incr_alloc_value, "incr_alloc_value");
            this.incr_alloc_value.Name = "incr_alloc_value";
            // 
            // panelShutDownHint
            // 
            this.panelShutDownHint.Controls.Add(this.labelShutDownWarning);
            this.panelShutDownHint.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.panelShutDownHint, "panelShutDownHint");
            this.panelShutDownHint.Name = "panelShutDownHint";
            // 
            // labelShutDownWarning
            // 
            resources.ApplyResources(this.labelShutDownWarning, "labelShutDownWarning");
            this.labelShutDownWarning.Name = "labelShutDownWarning";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // VDISizeLocationPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panelShutDownHint);
            this.Controls.Add(this.tableLayoutPanel2);
            this.DoubleBuffered = true;
            this.Name = "VDISizeLocationPage";
            ((System.ComponentModel.ISupportInitialize)(this.sizeNUD)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).EndInit();
            this.panelShutDownHint.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label SizeLabel;
        private System.Windows.Forms.Label locationLabel;
        private System.Windows.Forms.NumericUpDown sizeNUD;
        private System.Windows.Forms.Label sizeValue;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label sizeValueROLabel;
        private System.Windows.Forms.Panel panelShutDownHint;
        private System.Windows.Forms.Label labelShutDownWarning;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelLocationValueRO;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox pictureBoxError;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.Label initial_allocation_label;
        private System.Windows.Forms.Label incremental_allocation_label;
        private System.Windows.Forms.Label initial_alloc_value;
        private System.Windows.Forms.Label incr_alloc_value;

    }
}
