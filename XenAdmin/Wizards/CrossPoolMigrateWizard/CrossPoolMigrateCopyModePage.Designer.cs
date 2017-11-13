namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    partial class CrossPoolMigrateCopyModePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrossPoolMigrateCopyModePage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelRubric = new System.Windows.Forms.Label();
            this.intraPoolRadioButton = new System.Windows.Forms.RadioButton();
            this.crossPoolRadioButton = new System.Windows.Forms.RadioButton();
            this.intraPoolDescriptionLabel = new System.Windows.Forms.Label();
            this.crossPoolDescriptionLabel = new System.Windows.Forms.Label();
            this.warningsTable = new System.Windows.Forms.TableLayoutPanel();
            this.imgStopVM = new System.Windows.Forms.PictureBox();
            this.labelWarning = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.warningsTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgStopVM)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.warningsTable, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelRubric, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.intraPoolRadioButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.crossPoolRadioButton, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.intraPoolDescriptionLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.crossPoolDescriptionLabel, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelRubric
            // 
            resources.ApplyResources(this.labelRubric, "labelRubric");
            this.labelRubric.Name = "labelRubric";
            // 
            // intraPoolRadioButton
            // 
            resources.ApplyResources(this.intraPoolRadioButton, "intraPoolRadioButton");
            this.intraPoolRadioButton.Name = "intraPoolRadioButton";
            this.intraPoolRadioButton.TabStop = true;
            this.intraPoolRadioButton.UseVisualStyleBackColor = true;
            // 
            // crossPoolRadioButton
            // 
            resources.ApplyResources(this.crossPoolRadioButton, "crossPoolRadioButton");
            this.crossPoolRadioButton.Name = "crossPoolRadioButton";
            this.crossPoolRadioButton.TabStop = true;
            this.crossPoolRadioButton.UseVisualStyleBackColor = true;
            // 
            // intraPoolDescriptionLabel
            // 
            this.intraPoolDescriptionLabel.AutoEllipsis = true;
            resources.ApplyResources(this.intraPoolDescriptionLabel, "intraPoolDescriptionLabel");
            this.intraPoolDescriptionLabel.Name = "intraPoolDescriptionLabel";
            // 
            // crossPoolDescriptionLabel
            // 
            this.crossPoolDescriptionLabel.AutoEllipsis = true;
            resources.ApplyResources(this.crossPoolDescriptionLabel, "crossPoolDescriptionLabel");
            this.crossPoolDescriptionLabel.Name = "crossPoolDescriptionLabel";
            // 
            // warningsTable
            // 
            resources.ApplyResources(this.warningsTable, "warningsTable");
            this.warningsTable.Controls.Add(this.imgStopVM, 0, 0);
            this.warningsTable.Controls.Add(this.labelWarning, 1, 0);
            this.warningsTable.Name = "warningsTable";
            // 
            // imgStopVM
            // 
            resources.ApplyResources(this.imgStopVM, "imgStopVM");
            this.imgStopVM.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.imgStopVM.Name = "imgStopVM";
            this.imgStopVM.TabStop = false;
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.labelWarning.Name = "labelWarning";
            // 
            // CrossPoolMigrateCopyModePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CrossPoolMigrateCopyModePage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.warningsTable.ResumeLayout(false);
            this.warningsTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgStopVM)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelRubric;
        private System.Windows.Forms.RadioButton intraPoolRadioButton;
        private System.Windows.Forms.RadioButton crossPoolRadioButton;
        private System.Windows.Forms.Label intraPoolDescriptionLabel;
        private System.Windows.Forms.Label crossPoolDescriptionLabel;
        private System.Windows.Forms.TableLayoutPanel warningsTable;
        private System.Windows.Forms.PictureBox imgStopVM;
        private System.Windows.Forms.Label labelWarning;



    }
}

