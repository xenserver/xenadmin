using System.Windows.Forms;

namespace XenAdmin.Wizards.PatchingWizard
{
    partial class PatchingWizard_ModePage
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatchingWizard_ModePage));
            this.ManualRadioButton = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.AutomaticRadioButton = new System.Windows.Forms.RadioButton();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.removeUpdateFileCheckBox = new System.Windows.Forms.CheckBox();
            this.automaticRadioButtonTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ManualRadioButton
            // 
            resources.ApplyResources(this.ManualRadioButton, "ManualRadioButton");
            this.ManualRadioButton.Name = "ManualRadioButton";
            this.ManualRadioButton.TabStop = true;
            this.ManualRadioButton.UseVisualStyleBackColor = true;
            this.ManualRadioButton.CheckedChanged += new System.EventHandler(this.ManualRadioButton_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxLog
            // 
            resources.ApplyResources(this.textBoxLog, "textBoxLog");
            this.textBoxLog.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.AutomaticRadioButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.autoHeightLabel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ManualRadioButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.removeUpdateFileCheckBox, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.textBoxLog, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // AutomaticRadioButton
            // 
            resources.ApplyResources(this.AutomaticRadioButton, "AutomaticRadioButton");
            this.AutomaticRadioButton.Name = "AutomaticRadioButton";
            this.AutomaticRadioButton.TabStop = true;
            this.AutomaticRadioButton.UseVisualStyleBackColor = true;
            this.AutomaticRadioButton.CheckedChanged += new System.EventHandler(this.AutomaticRadioButton_CheckedChanged);
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // removeUpdateFileCheckBox
            // 
            resources.ApplyResources(this.removeUpdateFileCheckBox, "removeUpdateFileCheckBox");
            this.removeUpdateFileCheckBox.Checked = true;
            this.removeUpdateFileCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.removeUpdateFileCheckBox.Name = "removeUpdateFileCheckBox";
            this.removeUpdateFileCheckBox.UseVisualStyleBackColor = true;
            // 
            // PatchingWizard_ModePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PatchingWizard_ModePage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton ManualRadioButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
        private System.Windows.Forms.CheckBox removeUpdateFileCheckBox;
        private RadioButton AutomaticRadioButton;
        private ToolTip automaticRadioButtonTooltip;
    }
}
