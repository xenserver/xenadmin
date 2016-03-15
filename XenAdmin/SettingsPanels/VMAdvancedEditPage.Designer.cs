namespace XenAdmin.SettingsPanels
{
    partial class VMAdvancedEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VMAdvancedEditPage));
            this.GeneralOptimizationRadioButton = new System.Windows.Forms.RadioButton();
            this.CPSOptimizationRadioButton = new System.Windows.Forms.RadioButton();
            this.labelWarning = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.iconWarning = new System.Windows.Forms.PictureBox();
            this.ManualOptimizationRadioButton = new System.Windows.Forms.RadioButton();
            this.ShadowMultiplierTextBox = new System.Windows.Forms.TextBox();
            this.labelShadowMultiplier = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconWarning)).BeginInit();
            this.SuspendLayout();
            // 
            // GeneralOptimizationRadioButton
            // 
            resources.ApplyResources(this.GeneralOptimizationRadioButton, "GeneralOptimizationRadioButton");
            this.GeneralOptimizationRadioButton.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.GeneralOptimizationRadioButton, 3);
            this.GeneralOptimizationRadioButton.Name = "GeneralOptimizationRadioButton";
            this.GeneralOptimizationRadioButton.TabStop = true;
            this.GeneralOptimizationRadioButton.UseVisualStyleBackColor = true;
            this.GeneralOptimizationRadioButton.CheckedChanged += new System.EventHandler(this.GeneralOptimizationRadioButton_CheckedChanged);
            // 
            // CPSOptimizationRadioButton
            // 
            resources.ApplyResources(this.CPSOptimizationRadioButton, "CPSOptimizationRadioButton");
            this.tableLayoutPanel1.SetColumnSpan(this.CPSOptimizationRadioButton, 3);
            this.CPSOptimizationRadioButton.Name = "CPSOptimizationRadioButton";
            this.CPSOptimizationRadioButton.UseVisualStyleBackColor = true;
            this.CPSOptimizationRadioButton.CheckedChanged += new System.EventHandler(this.CPSOptimizationRadioButton_CheckedChanged);
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.tableLayoutPanel1.SetColumnSpan(this.labelWarning, 2);
            this.labelWarning.Name = "labelWarning";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.GeneralOptimizationRadioButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.CPSOptimizationRadioButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.iconWarning, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelWarning, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.ManualOptimizationRadioButton, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.ShadowMultiplierTextBox, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelShadowMultiplier, 1, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // iconWarning
            // 
            this.iconWarning.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.iconWarning, "iconWarning");
            this.iconWarning.Name = "iconWarning";
            this.iconWarning.TabStop = false;
            // 
            // ManualOptimizationRadioButton
            // 
            resources.ApplyResources(this.ManualOptimizationRadioButton, "ManualOptimizationRadioButton");
            this.tableLayoutPanel1.SetColumnSpan(this.ManualOptimizationRadioButton, 3);
            this.ManualOptimizationRadioButton.Name = "ManualOptimizationRadioButton";
            this.ManualOptimizationRadioButton.UseVisualStyleBackColor = true;
            // 
            // ShadowMultiplierTextBox
            // 
            resources.ApplyResources(this.ShadowMultiplierTextBox, "ShadowMultiplierTextBox");
            this.ShadowMultiplierTextBox.Name = "ShadowMultiplierTextBox";
            this.ShadowMultiplierTextBox.TextChanged += new System.EventHandler(this.ShadowMultiplierTextBox_TextChanged);
            // 
            // labelShadowMultiplier
            // 
            resources.ApplyResources(this.labelShadowMultiplier, "labelShadowMultiplier");
            this.labelShadowMultiplier.Name = "labelShadowMultiplier";
            // 
            // VMAdvancedEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "VMAdvancedEditPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconWarning)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton GeneralOptimizationRadioButton;
        private System.Windows.Forms.RadioButton CPSOptimizationRadioButton;
        private System.Windows.Forms.RadioButton ManualOptimizationRadioButton;
        private System.Windows.Forms.TextBox ShadowMultiplierTextBox;
        private System.Windows.Forms.Label labelShadowMultiplier;
        private System.Windows.Forms.PictureBox iconWarning;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
    }
}
