namespace XenAdmin.Dialogs
{
    partial class EvacuateHostDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EvacuateHostDialog));
            this.vmListLabel = new System.Windows.Forms.Label();
            this.vmsListBox = new XenAdmin.Controls.FlickerFreeListBox();
            this.CloseButton = new System.Windows.Forms.Button();
            this.EvacuateButton = new System.Windows.Forms.Button();
            this.labelBlurb = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.NewMasterComboBox = new System.Windows.Forms.ComboBox();
            this.NewMasterLabel = new System.Windows.Forms.Label();
            this.labelMasterBlurb = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lableWLBEnabled = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ActionStatusLabel
            // 
            resources.ApplyResources(this.ActionStatusLabel, "ActionStatusLabel");
            // 
            // ProgressSeparator
            // 
            resources.ApplyResources(this.ProgressSeparator, "ProgressSeparator");
            // 
            // ActionProgressBar
            // 
            resources.ApplyResources(this.ActionProgressBar, "ActionProgressBar");
            // 
            // vmListLabel
            // 
            resources.ApplyResources(this.vmListLabel, "vmListLabel");
            this.vmListLabel.Name = "vmListLabel";
            // 
            // vmsListBox
            // 
            resources.ApplyResources(this.vmsListBox, "vmsListBox");
            this.vmsListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.vmsListBox.Name = "vmsListBox";
            this.vmsListBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.vmsListBox.Sorted = true;
            this.vmsListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.vmsListBox_DrawItem);
            this.vmsListBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.vmsListBox_MouseClick);
            this.vmsListBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.vmsListBox_MouseMove);
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // EvacuateButton
            // 
            resources.ApplyResources(this.EvacuateButton, "EvacuateButton");
            this.EvacuateButton.Name = "EvacuateButton";
            this.EvacuateButton.UseVisualStyleBackColor = true;
            this.EvacuateButton.Click += new System.EventHandler(this.RepairButton_Click);
            // 
            // labelBlurb
            // 
            resources.ApplyResources(this.labelBlurb, "labelBlurb");
            this.labelBlurb.Name = "labelBlurb";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_ServerMaintenance_h32bit_32;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // NewMasterComboBox
            // 
            resources.ApplyResources(this.NewMasterComboBox, "NewMasterComboBox");
            this.NewMasterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NewMasterComboBox.FormattingEnabled = true;
            this.NewMasterComboBox.Name = "NewMasterComboBox";
            // 
            // NewMasterLabel
            // 
            resources.ApplyResources(this.NewMasterLabel, "NewMasterLabel");
            this.NewMasterLabel.Name = "NewMasterLabel";
            // 
            // labelMasterBlurb
            // 
            resources.ApplyResources(this.labelMasterBlurb, "labelMasterBlurb");
            this.labelMasterBlurb.Name = "labelMasterBlurb";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.lableWLBEnabled);
            this.panel2.Controls.Add(this.vmsListBox);
            this.panel2.Controls.Add(this.vmListLabel);
            this.panel2.Name = "panel2";
            // 
            // lableWLBEnabled
            // 
            resources.ApplyResources(this.lableWLBEnabled, "lableWLBEnabled");
            this.lableWLBEnabled.Name = "lableWLBEnabled";
            // 
            // EvacuateHostDialog
            // 
            this.AcceptButton = this.EvacuateButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.CloseButton;
            this.Controls.Add(this.NewMasterComboBox);
            this.Controls.Add(this.NewMasterLabel);
            this.Controls.Add(this.labelBlurb);
            this.Controls.Add(this.labelMasterBlurb);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.EvacuateButton);
            this.Name = "EvacuateHostDialog";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EvacuateHostDialog_FormClosed);
            this.Controls.SetChildIndex(this.ActionProgressBar, 0);
            this.Controls.SetChildIndex(this.ProgressSeparator, 0);
            this.Controls.SetChildIndex(this.ActionStatusLabel, 0);
            this.Controls.SetChildIndex(this.EvacuateButton, 0);
            this.Controls.SetChildIndex(this.CloseButton, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            this.Controls.SetChildIndex(this.pictureBox1, 0);
            this.Controls.SetChildIndex(this.labelMasterBlurb, 0);
            this.Controls.SetChildIndex(this.labelBlurb, 0);
            this.Controls.SetChildIndex(this.NewMasterLabel, 0);
            this.Controls.SetChildIndex(this.NewMasterComboBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button EvacuateButton;
        private XenAdmin.Controls.FlickerFreeListBox vmsListBox;
        private System.Windows.Forms.Label vmListLabel;
        private System.Windows.Forms.Label labelBlurb;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox NewMasterComboBox;
        private System.Windows.Forms.Label NewMasterLabel;
        private System.Windows.Forms.Label labelMasterBlurb;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lableWLBEnabled;
    }
}