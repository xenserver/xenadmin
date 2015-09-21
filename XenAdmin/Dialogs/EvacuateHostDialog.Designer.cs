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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.vmListLabel = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.EvacuateButton = new System.Windows.Forms.Button();
            this.labelBlurb = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.NewMasterComboBox = new System.Windows.Forms.ComboBox();
            this.NewMasterLabel = new System.Windows.Forms.Label();
            this.labelMasterBlurb = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridViewVms = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.columnVm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lableWLBEnabled = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).BeginInit();
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
            this.panel2.Controls.Add(this.dataGridViewVms);
            this.panel2.Controls.Add(this.lableWLBEnabled);
            this.panel2.Controls.Add(this.vmListLabel);
            this.panel2.Name = "panel2";
            // 
            // dataGridViewVms
            // 
            this.dataGridViewVms.AllowUserToResizeColumns = false;
            resources.ApplyResources(this.dataGridViewVms, "dataGridViewVms");
            this.dataGridViewVms.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewVms.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewVms.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewVms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewVms.ColumnHeadersVisible = false;
            this.dataGridViewVms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnImage,
            this.columnVm,
            this.columnAction});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewVms.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewVms.HideSelection = true;
            this.dataGridViewVms.Name = "dataGridViewVms";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewVms.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewVms.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewVms.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewVms_CellMouseClick);
            this.dataGridViewVms.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewVms_CellMouseMove);
            // 
            // columnImage
            // 
            this.columnImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = "System.Drawing.Bitmap";
            this.columnImage.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.columnImage, "columnImage");
            this.columnImage.Name = "columnImage";
            this.columnImage.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.columnImage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // columnVm
            // 
            this.columnVm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnVm.FillWeight = 137.8531F;
            resources.ApplyResources(this.columnVm, "columnVm");
            this.columnVm.Name = "columnVm";
            // 
            // columnAction
            // 
            this.columnAction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.columnAction.DefaultCellStyle = dataGridViewCellStyle3;
            this.columnAction.FillWeight = 40F;
            resources.ApplyResources(this.columnAction, "columnAction");
            this.columnAction.Name = "columnAction";
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button EvacuateButton;
        private System.Windows.Forms.Label vmListLabel;
        private System.Windows.Forms.Label labelBlurb;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox NewMasterComboBox;
        private System.Windows.Forms.Label NewMasterLabel;
        private System.Windows.Forms.Label labelMasterBlurb;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lableWLBEnabled;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewVms;
        private System.Windows.Forms.DataGridViewImageColumn columnImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnVm;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnAction;
    }
}