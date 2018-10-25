namespace XenAdmin.Controls.Ballooning
{
    partial class VMMemoryControlsNoEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VMMemoryControlsNoEdit));
            this.iconBoxDynMin = new System.Windows.Forms.PictureBox();
            this.iconBoxDynMax = new System.Windows.Forms.PictureBox();
            this.labelDynMin = new System.Windows.Forms.Label();
            this.labelDynMax = new System.Windows.Forms.Label();
            this.labelStatMax = new System.Windows.Forms.Label();
            this.valueDynMin = new System.Windows.Forms.Label();
            this.valueDynMax = new System.Windows.Forms.Label();
            this.valueStatMax = new System.Windows.Forms.Label();
            this.editButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.vmShinyBar = new XenAdmin.Controls.Ballooning.VMShinyBar();
            ((System.ComponentModel.ISupportInitialize)(this.iconBoxDynMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconBoxDynMax)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // iconBoxDynMin
            // 
            resources.ApplyResources(this.iconBoxDynMin, "iconBoxDynMin");
            this.iconBoxDynMin.BackColor = System.Drawing.Color.Transparent;
            this.iconBoxDynMin.Image = global::XenAdmin.Properties.Resources.memory_dynmin_slider_noedit_small;
            this.iconBoxDynMin.Name = "iconBoxDynMin";
            this.iconBoxDynMin.TabStop = false;
            // 
            // iconBoxDynMax
            // 
            resources.ApplyResources(this.iconBoxDynMax, "iconBoxDynMax");
            this.iconBoxDynMax.BackColor = System.Drawing.Color.Transparent;
            this.iconBoxDynMax.Image = global::XenAdmin.Properties.Resources.memory_dynmax_slider_noedit_small;
            this.iconBoxDynMax.Name = "iconBoxDynMax";
            this.iconBoxDynMax.TabStop = false;
            // 
            // labelDynMin
            // 
            resources.ApplyResources(this.labelDynMin, "labelDynMin");
            this.labelDynMin.BackColor = System.Drawing.Color.Transparent;
            this.labelDynMin.Name = "labelDynMin";
            // 
            // labelDynMax
            // 
            resources.ApplyResources(this.labelDynMax, "labelDynMax");
            this.labelDynMax.BackColor = System.Drawing.Color.Transparent;
            this.labelDynMax.Name = "labelDynMax";
            // 
            // labelStatMax
            // 
            resources.ApplyResources(this.labelStatMax, "labelStatMax");
            this.labelStatMax.BackColor = System.Drawing.Color.Transparent;
            this.labelStatMax.Name = "labelStatMax";
            // 
            // valueDynMin
            // 
            resources.ApplyResources(this.valueDynMin, "valueDynMin");
            this.valueDynMin.BackColor = System.Drawing.Color.Transparent;
            this.valueDynMin.Name = "valueDynMin";
            // 
            // valueDynMax
            // 
            resources.ApplyResources(this.valueDynMax, "valueDynMax");
            this.valueDynMax.BackColor = System.Drawing.Color.Transparent;
            this.valueDynMax.Name = "valueDynMax";
            // 
            // valueStatMax
            // 
            resources.ApplyResources(this.valueStatMax, "valueStatMax");
            this.valueStatMax.BackColor = System.Drawing.Color.Transparent;
            this.valueStatMax.Name = "valueStatMax";
            // 
            // editButton
            // 
            resources.ApplyResources(this.editButton, "editButton");
            this.editButton.Name = "editButton";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.vmShinyBar, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.editButton, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.iconBoxDynMax, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.iconBoxDynMin, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelDynMax, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelDynMin, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelStatMax, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.valueDynMax, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.valueDynMin, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.valueStatMax, 2, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // vmShinyBar
            // 
            resources.ApplyResources(this.vmShinyBar, "vmShinyBar");
            this.vmShinyBar.Increment = 0D;
            this.vmShinyBar.Name = "vmShinyBar";
            this.tableLayoutPanel1.SetRowSpan(this.vmShinyBar, 2);
            // 
            // VMMemoryControlsNoEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "VMMemoryControlsNoEdit";
            ((System.ComponentModel.ISupportInitialize)(this.iconBoxDynMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconBoxDynMax)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private VMShinyBar vmShinyBar;
        private System.Windows.Forms.PictureBox iconBoxDynMin;
        private System.Windows.Forms.PictureBox iconBoxDynMax;
        private System.Windows.Forms.Label labelDynMin;
        private System.Windows.Forms.Label labelDynMax;
        private System.Windows.Forms.Label labelStatMax;
        private System.Windows.Forms.Label valueDynMin;
        private System.Windows.Forms.Label valueDynMax;
        private System.Windows.Forms.Label valueStatMax;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
