namespace XenAdmin.SettingsPanels
{
    partial class GpuEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GpuEditPage));
            this.labelRubric = new System.Windows.Forms.Label();
            this.warningsTable = new System.Windows.Forms.TableLayoutPanel();
            this.imgRDP = new System.Windows.Forms.PictureBox();
            this.labelRDP = new System.Windows.Forms.Label();
            this.imgNeedDriver = new System.Windows.Forms.PictureBox();
            this.labelNeedDriver = new System.Windows.Forms.Label();
            this.imgNeedGpu = new System.Windows.Forms.PictureBox();
            this.labelNeedGpu = new System.Windows.Forms.Label();
            this.imgStopVM = new System.Windows.Forms.PictureBox();
            this.labelStopVM = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelGpuType = new System.Windows.Forms.Label();
            this.comboBoxGpus = new XenAdmin.Controls.VgpuComboBox();
            this.warningsTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgRDP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgNeedDriver)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgNeedGpu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStopVM)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelRubric
            // 
            resources.ApplyResources(this.labelRubric, "labelRubric");
            this.labelRubric.Name = "labelRubric";
            // 
            // warningsTable
            // 
            resources.ApplyResources(this.warningsTable, "warningsTable");
            this.warningsTable.Controls.Add(this.imgRDP, 0, 0);
            this.warningsTable.Controls.Add(this.labelRDP, 1, 0);
            this.warningsTable.Controls.Add(this.imgNeedDriver, 0, 1);
            this.warningsTable.Controls.Add(this.labelNeedDriver, 1, 1);
            this.warningsTable.Controls.Add(this.imgNeedGpu, 0, 2);
            this.warningsTable.Controls.Add(this.labelNeedGpu, 1, 2);
            this.warningsTable.Controls.Add(this.imgStopVM, 0, 3);
            this.warningsTable.Controls.Add(this.labelStopVM, 1, 3);
            this.warningsTable.Name = "warningsTable";
            this.warningsTable.SizeChanged += new System.EventHandler(this.warningsTable_SizeChanged);
            // 
            // imgRDP
            // 
            this.imgRDP.ErrorImage = null;
            this.imgRDP.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.imgRDP, "imgRDP");
            this.imgRDP.InitialImage = null;
            this.imgRDP.Name = "imgRDP";
            this.imgRDP.TabStop = false;
            // 
            // labelRDP
            // 
            resources.ApplyResources(this.labelRDP, "labelRDP");
            this.labelRDP.MaximumSize = new System.Drawing.Size(586, 999);
            this.labelRDP.Name = "labelRDP";
            // 
            // imgNeedDriver
            // 
            this.imgNeedDriver.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.imgNeedDriver, "imgNeedDriver");
            this.imgNeedDriver.InitialImage = null;
            this.imgNeedDriver.Name = "imgNeedDriver";
            this.imgNeedDriver.TabStop = false;
            // 
            // labelNeedDriver
            // 
            resources.ApplyResources(this.labelNeedDriver, "labelNeedDriver");
            this.labelNeedDriver.Name = "labelNeedDriver";
            // 
            // imgNeedGpu
            // 
            this.imgNeedGpu.ErrorImage = null;
            this.imgNeedGpu.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.imgNeedGpu, "imgNeedGpu");
            this.imgNeedGpu.InitialImage = null;
            this.imgNeedGpu.Name = "imgNeedGpu";
            this.imgNeedGpu.TabStop = false;
            // 
            // labelNeedGpu
            // 
            resources.ApplyResources(this.labelNeedGpu, "labelNeedGpu");
            this.labelNeedGpu.Name = "labelNeedGpu";
            // 
            // imgStopVM
            // 
            this.imgStopVM.ErrorImage = null;
            this.imgStopVM.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.imgStopVM, "imgStopVM");
            this.imgStopVM.InitialImage = null;
            this.imgStopVM.Name = "imgStopVM";
            this.imgStopVM.TabStop = false;
            // 
            // labelStopVM
            // 
            resources.ApplyResources(this.labelStopVM, "labelStopVM");
            this.labelStopVM.MaximumSize = new System.Drawing.Size(584, 999);
            this.labelStopVM.Name = "labelStopVM";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelGpuType, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxGpus, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelGpuType
            // 
            resources.ApplyResources(this.labelGpuType, "labelGpuType");
            this.labelGpuType.Name = "labelGpuType";
            // 
            // comboBoxGpus
            // 
            resources.ApplyResources(this.comboBoxGpus, "comboBoxGpus");
            this.comboBoxGpus.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBoxGpus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGpus.FormattingEnabled = true;
            this.comboBoxGpus.Name = "comboBoxGpus";
            this.comboBoxGpus.SelectedIndexChanged += new System.EventHandler(this.comboBoxGpus_SelectedIndexChanged);
            // 
            // GpuEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.labelRubric);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.warningsTable);
            this.Name = "GpuEditPage";
            this.warningsTable.ResumeLayout(false);
            this.warningsTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgRDP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgNeedDriver)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgNeedGpu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStopVM)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelRubric;
        private XenAdmin.Controls.VgpuComboBox comboBoxGpus;
        private System.Windows.Forms.TableLayoutPanel warningsTable;
        private System.Windows.Forms.PictureBox imgRDP;
        private System.Windows.Forms.Label labelRDP;
        private System.Windows.Forms.PictureBox imgStopVM;
        private System.Windows.Forms.Label labelStopVM;
        private System.Windows.Forms.PictureBox imgNeedGpu;
        private System.Windows.Forms.Label labelNeedGpu;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelGpuType;
        private System.Windows.Forms.PictureBox imgNeedDriver;
        private System.Windows.Forms.Label labelNeedDriver;
    }
}
