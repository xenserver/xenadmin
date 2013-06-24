namespace XenAdmin.Wizards.DRWizards
{
    partial class DRFailoverWizardAppliancesPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DRFailoverWizardAppliancesPage));
            this.labelPageDescription = new System.Windows.Forms.Label();
            this.ApplianceTreeView = new XenAdmin.Controls.CustomTreeView();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.radioButtonPaused = new System.Windows.Forms.RadioButton();
            this.radioButtonStart = new System.Windows.Forms.RadioButton();
            this.radioButtonDoNotStart = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPageDescription
            // 
            resources.ApplyResources(this.labelPageDescription, "labelPageDescription");
            this.tableLayoutPanel1.SetColumnSpan(this.labelPageDescription, 2);
            this.labelPageDescription.Name = "labelPageDescription";
            // 
            // ApplianceTreeView
            // 
            resources.ApplyResources(this.ApplianceTreeView, "ApplianceTreeView");
            this.ApplianceTreeView.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ApplianceTreeView.FormattingEnabled = true;
            this.ApplianceTreeView.Name = "ApplianceTreeView";
            this.ApplianceTreeView.NodeIndent = 19;
            this.ApplianceTreeView.RootAlwaysExpanded = true;
            this.tableLayoutPanel1.SetRowSpan(this.ApplianceTreeView, 2);
            this.ApplianceTreeView.ShowCheckboxes = true;
            this.ApplianceTreeView.ShowDescription = true;
            this.ApplianceTreeView.ShowImages = true;
            this.ApplianceTreeView.ShowRootLines = false;
            this.ApplianceTreeView.ItemCheckChanged += new System.EventHandler<System.EventArgs>(this.ApplianceTreeView_ItemCheckChanged);
            // 
            // buttonClearAll
            // 
            resources.ApplyResources(this.buttonClearAll, "buttonClearAll");
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.UseVisualStyleBackColor = true;
            this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
            // 
            // buttonSelectAll
            // 
            resources.ApplyResources(this.buttonSelectAll, "buttonSelectAll");
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
            // 
            // radioButtonPaused
            // 
            resources.ApplyResources(this.radioButtonPaused, "radioButtonPaused");
            this.radioButtonPaused.Name = "radioButtonPaused";
            this.radioButtonPaused.UseVisualStyleBackColor = true;
            // 
            // radioButtonStart
            // 
            resources.ApplyResources(this.radioButtonStart, "radioButtonStart");
            this.radioButtonStart.Checked = true;
            this.radioButtonStart.Name = "radioButtonStart";
            this.radioButtonStart.TabStop = true;
            this.radioButtonStart.UseVisualStyleBackColor = true;
            // 
            // radioButtonDoNotStart
            // 
            resources.ApplyResources(this.radioButtonDoNotStart, "radioButtonDoNotStart");
            this.radioButtonDoNotStart.Name = "radioButtonDoNotStart";
            this.radioButtonDoNotStart.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.radioButtonStart, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.radioButtonDoNotStart, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.radioButtonPaused, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ApplianceTreeView, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonClearAll, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonSelectAll, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelPageDescription, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // DRFailoverWizardAppliancesPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DRFailoverWizardAppliancesPage";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelPageDescription;
        private XenAdmin.Controls.CustomTreeView ApplianceTreeView;
        private System.Windows.Forms.Button buttonClearAll;
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.RadioButton radioButtonPaused;
        private System.Windows.Forms.RadioButton radioButtonStart;
        private System.Windows.Forms.RadioButton radioButtonDoNotStart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;

    }
}
