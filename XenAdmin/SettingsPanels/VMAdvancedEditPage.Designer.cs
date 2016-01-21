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
            this.ManualOptimizationRadioButton = new System.Windows.Forms.RadioButton();
            this.labelWarning = new System.Windows.Forms.Label();
            this.ManualOptimizationGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.ShadowMultiplierTextBox = new System.Windows.Forms.TextBox();
            this.labelShadowMultiplier = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.iconWarning = new System.Windows.Forms.PictureBox();
            this.ManualOptimizationGroupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconWarning)).BeginInit();
            this.SuspendLayout();
            // 
            // GeneralOptimizationRadioButton
            // 
            resources.ApplyResources(this.GeneralOptimizationRadioButton, "GeneralOptimizationRadioButton");
            this.GeneralOptimizationRadioButton.Checked = true;
            this.GeneralOptimizationRadioButton.Name = "GeneralOptimizationRadioButton";
            this.GeneralOptimizationRadioButton.TabStop = true;
            this.GeneralOptimizationRadioButton.UseVisualStyleBackColor = true;
            this.GeneralOptimizationRadioButton.CheckedChanged += new System.EventHandler(this.GeneralOptimizationRadioButton_CheckedChanged);
            // 
            // CPSOptimizationRadioButton
            // 
            resources.ApplyResources(this.CPSOptimizationRadioButton, "CPSOptimizationRadioButton");
            this.CPSOptimizationRadioButton.Name = "CPSOptimizationRadioButton";
            this.CPSOptimizationRadioButton.UseVisualStyleBackColor = true;
            this.CPSOptimizationRadioButton.CheckedChanged += new System.EventHandler(this.CPSOptimizationRadioButton_CheckedChanged);
            this.CPSOptimizationRadioButton.Visible = !XenAdmin.Core.Registry.CPSOptimizationHidden;
            // 
            // ManualOptimizationRadioButton
            // 
            resources.ApplyResources(this.ManualOptimizationRadioButton, "ManualOptimizationRadioButton");
            this.ManualOptimizationRadioButton.Name = "ManualOptimizationRadioButton";
            this.ManualOptimizationRadioButton.UseVisualStyleBackColor = true;
            this.ManualOptimizationRadioButton.CheckedChanged += new System.EventHandler(this.ManualOptimizationRadioButton_CheckedChanged);
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.labelWarning.Name = "labelWarning";
            // 
            // ManualOptimizationGroupBox
            // 
            resources.ApplyResources(this.ManualOptimizationGroupBox, "ManualOptimizationGroupBox");
            this.ManualOptimizationGroupBox.Controls.Add(this.ShadowMultiplierTextBox);
            this.ManualOptimizationGroupBox.Controls.Add(this.labelShadowMultiplier);
            this.ManualOptimizationGroupBox.Name = "ManualOptimizationGroupBox";
            this.ManualOptimizationGroupBox.TabStop = false;
            // 
            // ShadowMultiplierTextBox
            // 
            resources.ApplyResources(this.ShadowMultiplierTextBox, "ShadowMultiplierTextBox");
            this.ShadowMultiplierTextBox.Name = "ShadowMultiplierTextBox";
            // 
            // labelShadowMultiplier
            // 
            resources.ApplyResources(this.labelShadowMultiplier, "labelShadowMultiplier");
            this.labelShadowMultiplier.Name = "labelShadowMultiplier";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // iconWarning
            // 
            resources.ApplyResources(this.iconWarning, "iconWarning");
            this.iconWarning.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.iconWarning.Name = "iconWarning";
            this.iconWarning.TabStop = false;
            // 
            // VMAdvancedEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.labelWarning);
            this.Controls.Add(this.iconWarning);
            this.Controls.Add(this.ManualOptimizationRadioButton);
            this.Controls.Add(this.CPSOptimizationRadioButton);
            this.Controls.Add(this.GeneralOptimizationRadioButton);
            this.Controls.Add(this.ManualOptimizationGroupBox);
            this.DoubleBuffered = true;
            this.Name = "VMAdvancedEditPage";
            this.ManualOptimizationGroupBox.ResumeLayout(false);
            this.ManualOptimizationGroupBox.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconWarning)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton GeneralOptimizationRadioButton;
        private System.Windows.Forms.RadioButton CPSOptimizationRadioButton;
        private System.Windows.Forms.RadioButton ManualOptimizationRadioButton;
        private XenAdmin.Controls.DecentGroupBox ManualOptimizationGroupBox;
        private System.Windows.Forms.TextBox ShadowMultiplierTextBox;
        private System.Windows.Forms.Label labelShadowMultiplier;
        private System.Windows.Forms.PictureBox iconWarning;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
    }
}
