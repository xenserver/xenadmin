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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.vmListLabel = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.EvacuateButton = new System.Windows.Forms.Button();
            this.labelBlurb = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.NewCoordinatorComboBox = new System.Windows.Forms.ComboBox();
            this.NewCoordinatorLabel = new System.Windows.Forms.Label();
            this.labelCoordinatorBlurb = new System.Windows.Forms.Label();
            this.dataGridViewVms = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.columnVm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lableWLBEnabled = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelWlb = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanelNewCoordinator = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelPSr = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.labelWarning = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanelSpinner = new System.Windows.Forms.TableLayoutPanel();
            this.spinnerIcon1 = new XenAdmin.Controls.SpinnerIcon();
            this.labelSpinner = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanelStatus = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxStatus = new System.Windows.Forms.PictureBox();
            this.labelProgress = new System.Windows.Forms.Label();
            this.buttonCheckAgain = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelWlb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tableLayoutPanelNewCoordinator.SuspendLayout();
            this.tableLayoutPanelPSr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanelSpinner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIcon1)).BeginInit();
            this.tableLayoutPanelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // vmListLabel
            // 
            resources.ApplyResources(this.vmListLabel, "vmListLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.vmListLabel, 4);
            this.vmListLabel.Name = "vmListLabel";
            // 
            // CloseButton
            // 
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // EvacuateButton
            // 
            resources.ApplyResources(this.EvacuateButton, "EvacuateButton");
            this.EvacuateButton.Name = "EvacuateButton";
            this.EvacuateButton.UseVisualStyleBackColor = true;
            this.EvacuateButton.Click += new System.EventHandler(this.EvacuateButton_Click);
            // 
            // labelBlurb
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.labelBlurb, 3);
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
            // NewCoordinatorComboBox
            // 
            resources.ApplyResources(this.NewCoordinatorComboBox, "NewCoordinatorComboBox");
            this.NewCoordinatorComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.NewCoordinatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NewCoordinatorComboBox.FormattingEnabled = true;
            this.NewCoordinatorComboBox.Name = "NewCoordinatorComboBox";
            this.NewCoordinatorComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.NewCoordinatorComboBox_DrawItem);
            // 
            // NewCoordinatorLabel
            // 
            resources.ApplyResources(this.NewCoordinatorLabel, "NewCoordinatorLabel");
            this.NewCoordinatorLabel.Name = "NewCoordinatorLabel";
            // 
            // labelCoordinatorBlurb
            // 
            this.tableLayoutPanelNewCoordinator.SetColumnSpan(this.labelCoordinatorBlurb, 2);
            resources.ApplyResources(this.labelCoordinatorBlurb, "labelCoordinatorBlurb");
            this.labelCoordinatorBlurb.Name = "labelCoordinatorBlurb";
            // 
            // dataGridViewVms
            // 
            this.dataGridViewVms.AllowUserToResizeColumns = false;
            this.dataGridViewVms.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewVms.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewVms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewVms.ColumnHeadersVisible = false;
            this.dataGridViewVms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnImage,
            this.columnVm,
            this.columnAction});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewVms.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.dataGridViewVms, "dataGridViewVms");
            this.dataGridViewVms.HideSelection = true;
            this.dataGridViewVms.Name = "dataGridViewVms";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewVms.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewVms.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewVms.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewVms_CellMouseClick);
            this.dataGridViewVms.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewVms_CellMouseMove);
            // 
            // columnImage
            // 
            this.columnImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = "System.Drawing.Bitmap";
            this.columnImage.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.columnImage, "columnImage");
            this.columnImage.Name = "columnImage";
            this.columnImage.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.columnImage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // columnVm
            // 
            this.columnVm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnVm.FillWeight = 40F;
            resources.ApplyResources(this.columnVm, "columnVm");
            this.columnVm.Name = "columnVm";
            // 
            // columnAction
            // 
            this.columnAction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.columnAction.DefaultCellStyle = dataGridViewCellStyle2;
            this.columnAction.FillWeight = 60F;
            resources.ApplyResources(this.columnAction, "columnAction");
            this.columnAction.Name = "columnAction";
            // 
            // lableWLBEnabled
            // 
            resources.ApplyResources(this.lableWLBEnabled, "lableWLBEnabled");
            this.lableWLBEnabled.Name = "lableWLBEnabled";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelBlurb, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelWlb, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelNewCoordinator, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.vmListLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.EvacuateButton, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.CloseButton, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelStatus, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.buttonCheckAgain, 0, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanelWlb
            // 
            resources.ApplyResources(this.tableLayoutPanelWlb, "tableLayoutPanelWlb");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelWlb, 4);
            this.tableLayoutPanelWlb.Controls.Add(this.pictureBox2, 0, 0);
            this.tableLayoutPanelWlb.Controls.Add(this.lableWLBEnabled, 1, 0);
            this.tableLayoutPanelWlb.Name = "tableLayoutPanelWlb";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // tableLayoutPanelNewCoordinator
            // 
            resources.ApplyResources(this.tableLayoutPanelNewCoordinator, "tableLayoutPanelNewCoordinator");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelNewCoordinator, 4);
            this.tableLayoutPanelNewCoordinator.Controls.Add(this.labelCoordinatorBlurb, 0, 0);
            this.tableLayoutPanelNewCoordinator.Controls.Add(this.NewCoordinatorLabel, 0, 1);
            this.tableLayoutPanelNewCoordinator.Controls.Add(this.NewCoordinatorComboBox, 1, 1);
            this.tableLayoutPanelNewCoordinator.Controls.Add(this.tableLayoutPanelPSr, 0, 2);
            this.tableLayoutPanelNewCoordinator.Name = "tableLayoutPanelNewCoordinator";
            // 
            // tableLayoutPanelPSr
            // 
            resources.ApplyResources(this.tableLayoutPanelPSr, "tableLayoutPanelPSr");
            this.tableLayoutPanelNewCoordinator.SetColumnSpan(this.tableLayoutPanelPSr, 2);
            this.tableLayoutPanelPSr.Controls.Add(this.pictureBox3, 0, 0);
            this.tableLayoutPanelPSr.Controls.Add(this.labelWarning, 1, 0);
            this.tableLayoutPanelPSr.Name = "tableLayoutPanelPSr";
            // 
            // pictureBox3
            // 
            resources.ApplyResources(this.pictureBox3, "pictureBox3");
            this.pictureBox3.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.TabStop = false;
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.labelWarning.Name = "labelWarning";
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 4);
            this.panel1.Controls.Add(this.tableLayoutPanelSpinner);
            this.panel1.Controls.Add(this.dataGridViewVms);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanelSpinner
            // 
            resources.ApplyResources(this.tableLayoutPanelSpinner, "tableLayoutPanelSpinner");
            this.tableLayoutPanelSpinner.BackColor = System.Drawing.SystemColors.Window;
            this.tableLayoutPanelSpinner.Controls.Add(this.spinnerIcon1, 0, 0);
            this.tableLayoutPanelSpinner.Controls.Add(this.labelSpinner, 1, 0);
            this.tableLayoutPanelSpinner.Name = "tableLayoutPanelSpinner";
            // 
            // spinnerIcon1
            // 
            resources.ApplyResources(this.spinnerIcon1, "spinnerIcon1");
            this.spinnerIcon1.Name = "spinnerIcon1";
            this.spinnerIcon1.TabStop = false;
            // 
            // labelSpinner
            // 
            resources.ApplyResources(this.labelSpinner, "labelSpinner");
            this.labelSpinner.Name = "labelSpinner";
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.tableLayoutPanel1.SetColumnSpan(this.progressBar1, 4);
            this.progressBar1.Name = "progressBar1";
            // 
            // tableLayoutPanelStatus
            // 
            resources.ApplyResources(this.tableLayoutPanelStatus, "tableLayoutPanelStatus");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelStatus, 4);
            this.tableLayoutPanelStatus.Controls.Add(this.pictureBoxStatus, 0, 0);
            this.tableLayoutPanelStatus.Controls.Add(this.labelProgress, 1, 0);
            this.tableLayoutPanelStatus.Name = "tableLayoutPanelStatus";
            // 
            // pictureBoxStatus
            // 
            resources.ApplyResources(this.pictureBoxStatus, "pictureBoxStatus");
            this.pictureBoxStatus.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.pictureBoxStatus.Name = "pictureBoxStatus";
            this.pictureBoxStatus.TabStop = false;
            // 
            // labelProgress
            // 
            resources.ApplyResources(this.labelProgress, "labelProgress");
            this.labelProgress.Name = "labelProgress";
            // 
            // buttonCheckAgain
            // 
            resources.ApplyResources(this.buttonCheckAgain, "buttonCheckAgain");
            this.tableLayoutPanel1.SetColumnSpan(this.buttonCheckAgain, 2);
            this.buttonCheckAgain.Name = "buttonCheckAgain";
            this.buttonCheckAgain.UseVisualStyleBackColor = true;
            this.buttonCheckAgain.Click += new System.EventHandler(this.buttonCheckAgain_Click);
            // 
            // EvacuateHostDialog
            // 
            this.AcceptButton = this.EvacuateButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.CloseButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "EvacuateHostDialog";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EvacuateHostDialog_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelWlb.ResumeLayout(false);
            this.tableLayoutPanelWlb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tableLayoutPanelNewCoordinator.ResumeLayout(false);
            this.tableLayoutPanelNewCoordinator.PerformLayout();
            this.tableLayoutPanelPSr.ResumeLayout(false);
            this.tableLayoutPanelPSr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanelSpinner.ResumeLayout(false);
            this.tableLayoutPanelSpinner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIcon1)).EndInit();
            this.tableLayoutPanelStatus.ResumeLayout(false);
            this.tableLayoutPanelStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button EvacuateButton;
        private System.Windows.Forms.Label vmListLabel;
        private System.Windows.Forms.Label labelBlurb;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox NewCoordinatorComboBox;
        private System.Windows.Forms.Label NewCoordinatorLabel;
        private System.Windows.Forms.Label labelCoordinatorBlurb;
        private System.Windows.Forms.Label lableWLBEnabled;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewVms;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelNewCoordinator;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelWlb;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button buttonCheckAgain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelPSr;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpinner;
        private Controls.SpinnerIcon spinnerIcon1;
        private System.Windows.Forms.Label labelSpinner;
        private System.Windows.Forms.DataGridViewImageColumn columnImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnVm;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnAction;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelStatus;
        private System.Windows.Forms.PictureBox pictureBoxStatus;
        private System.Windows.Forms.Label labelProgress;
    }
}