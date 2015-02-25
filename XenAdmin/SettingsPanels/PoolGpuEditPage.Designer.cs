namespace XenAdmin.SettingsPanels
{
    partial class PoolGpuEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PoolGpuEditPage));
            this.labelRubric = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonMixture = new System.Windows.Forms.RadioButton();
            this.radioButtonDepth = new System.Windows.Forms.RadioButton();
            this.radioButtonBreadth = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxPlacementPolicy = new System.Windows.Forms.GroupBox();
            this.groupBoxIntedratedGpu = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonDisable = new System.Windows.Forms.RadioButton();
            this.autoHeightLabel3 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.labelCurrentState = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.radioButtonEnable = new System.Windows.Forms.RadioButton();
            this.optionsLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.groupBoxPlacementPolicy.SuspendLayout();
            this.groupBoxIntedratedGpu.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.optionsLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelRubric
            // 
            resources.ApplyResources(this.labelRubric, "labelRubric");
            this.labelRubric.Name = "labelRubric";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelRubric, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonMixture, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonDepth, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonBreadth, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // radioButtonMixture
            // 
            resources.ApplyResources(this.radioButtonMixture, "radioButtonMixture");
            this.radioButtonMixture.Name = "radioButtonMixture";
            this.radioButtonMixture.TabStop = true;
            this.radioButtonMixture.UseVisualStyleBackColor = true;
            // 
            // radioButtonDepth
            // 
            resources.ApplyResources(this.radioButtonDepth, "radioButtonDepth");
            this.radioButtonDepth.Name = "radioButtonDepth";
            this.radioButtonDepth.TabStop = true;
            this.radioButtonDepth.UseVisualStyleBackColor = true;
            // 
            // radioButtonBreadth
            // 
            resources.ApplyResources(this.radioButtonBreadth, "radioButtonBreadth");
            this.radioButtonBreadth.Name = "radioButtonBreadth";
            this.radioButtonBreadth.TabStop = true;
            this.radioButtonBreadth.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelMain
            // 
            resources.ApplyResources(this.tableLayoutPanelMain, "tableLayoutPanelMain");
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxPlacementPolicy, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxIntedratedGpu, 0, 1);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            // 
            // groupBoxPlacementPolicy
            // 
            resources.ApplyResources(this.groupBoxPlacementPolicy, "groupBoxPlacementPolicy");
            this.groupBoxPlacementPolicy.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxPlacementPolicy.Name = "groupBoxPlacementPolicy";
            this.groupBoxPlacementPolicy.TabStop = false;
            // 
            // groupBoxIntedratedGpu
            // 
            resources.ApplyResources(this.groupBoxIntedratedGpu, "groupBoxIntedratedGpu");
            this.groupBoxIntedratedGpu.Controls.Add(this.tableLayoutPanel2);
            this.groupBoxIntedratedGpu.Name = "groupBoxIntedratedGpu";
            this.groupBoxIntedratedGpu.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.optionsLayoutPanel, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.autoHeightLabel3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelCurrentState, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.autoHeightLabel1, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // radioButtonDisable
            // 
            resources.ApplyResources(this.radioButtonDisable, "radioButtonDisable");
            this.radioButtonDisable.Name = "radioButtonDisable";
            this.radioButtonDisable.TabStop = true;
            this.radioButtonDisable.UseVisualStyleBackColor = true;
            // 
            // autoHeightLabel3
            // 
            resources.ApplyResources(this.autoHeightLabel3, "autoHeightLabel3");
            this.autoHeightLabel3.Name = "autoHeightLabel3";
            // 
            // labelCurrentState
            // 
            resources.ApplyResources(this.labelCurrentState, "labelCurrentState");
            this.labelCurrentState.Name = "labelCurrentState";
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // radioButtonEnable
            // 
            resources.ApplyResources(this.radioButtonEnable, "radioButtonEnable");
            this.radioButtonEnable.Name = "radioButtonEnable";
            this.radioButtonEnable.TabStop = true;
            this.radioButtonEnable.UseVisualStyleBackColor = true;
            // 
            // optionsLayoutPanel
            // 
            resources.ApplyResources(this.optionsLayoutPanel, "optionsLayoutPanel");
            this.optionsLayoutPanel.Controls.Add(this.radioButtonEnable);
            this.optionsLayoutPanel.Controls.Add(this.radioButtonDisable);
            this.optionsLayoutPanel.Name = "optionsLayoutPanel";
            // 
            // PoolGpuEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Name = "PoolGpuEditPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.groupBoxPlacementPolicy.ResumeLayout(false);
            this.groupBoxPlacementPolicy.PerformLayout();
            this.groupBoxIntedratedGpu.ResumeLayout(false);
            this.groupBoxIntedratedGpu.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.optionsLayoutPanel.ResumeLayout(false);
            this.optionsLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.Common.AutoHeightLabel labelRubric;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RadioButton radioButtonDepth;
        private System.Windows.Forms.RadioButton radioButtonBreadth;
        private System.Windows.Forms.RadioButton radioButtonMixture;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.GroupBox groupBoxPlacementPolicy;
        private System.Windows.Forms.GroupBox groupBoxIntedratedGpu;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.RadioButton radioButtonDisable;
        private Controls.Common.AutoHeightLabel autoHeightLabel3;
        private Controls.Common.AutoHeightLabel labelCurrentState;
        private Controls.Common.AutoHeightLabel autoHeightLabel1;
        private System.Windows.Forms.RadioButton radioButtonEnable;
        private System.Windows.Forms.FlowLayoutPanel optionsLayoutPanel;
    }
}
