namespace XenAdmin.Dialogs
{
    partial class VtpmManagementPage
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
            if (disposing)
            {
                UnregisterEvents();

                if (components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VtpmManagementPage));
            this.tableLayoutPanelBody = new System.Windows.Forms.TableLayoutPanel();
            this.buttonReset = new System.Windows.Forms.Button();
            this.groupBoxProperties = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelProtectedKey = new System.Windows.Forms.Label();
            this.labelUniqueValue = new System.Windows.Forms.Label();
            this.labelProtectedValue = new System.Windows.Forms.Label();
            this.labelUniqueKey = new System.Windows.Forms.Label();
            this.toolTipContainerRemove = new XenAdmin.Controls.ToolTipContainer();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.labelProtectedInfo = new System.Windows.Forms.Label();
            this.labelUniqueInfo = new System.Windows.Forms.Label();
            this.tableLayoutPanelBody.SuspendLayout();
            this.groupBoxProperties.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolTipContainerRemove.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelBody
            // 
            this.tableLayoutPanelBody.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.tableLayoutPanelBody, "tableLayoutPanelBody");
            this.tableLayoutPanelBody.Controls.Add(this.buttonReset, 0, 2);
            this.tableLayoutPanelBody.Controls.Add(this.groupBoxProperties, 0, 0);
            this.tableLayoutPanelBody.Controls.Add(this.toolTipContainerRemove, 1, 2);
            this.tableLayoutPanelBody.Name = "tableLayoutPanelBody";
            // 
            // buttonReset
            // 
            resources.ApplyResources(this.buttonReset, "buttonReset");
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // groupBoxProperties
            // 
            this.tableLayoutPanelBody.SetColumnSpan(this.groupBoxProperties, 2);
            this.groupBoxProperties.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.groupBoxProperties, "groupBoxProperties");
            this.groupBoxProperties.Name = "groupBoxProperties";
            this.groupBoxProperties.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelProtectedKey, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelProtectedValue, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelProtectedInfo, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelUniqueKey, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelUniqueValue, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelUniqueInfo, 1, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelProtectedKey
            // 
            resources.ApplyResources(this.labelProtectedKey, "labelProtectedKey");
            this.labelProtectedKey.Name = "labelProtectedKey";
            // 
            // labelUniqueValue
            // 
            resources.ApplyResources(this.labelUniqueValue, "labelUniqueValue");
            this.labelUniqueValue.Name = "labelUniqueValue";
            // 
            // labelProtectedValue
            // 
            resources.ApplyResources(this.labelProtectedValue, "labelProtectedValue");
            this.labelProtectedValue.Name = "labelProtectedValue";
            // 
            // labelUniqueKey
            // 
            resources.ApplyResources(this.labelUniqueKey, "labelUniqueKey");
            this.labelUniqueKey.Name = "labelUniqueKey";
            // 
            // toolTipContainerRemove
            // 
            this.toolTipContainerRemove.Controls.Add(this.buttonRemove);
            resources.ApplyResources(this.toolTipContainerRemove, "toolTipContainerRemove");
            this.toolTipContainerRemove.Name = "toolTipContainerRemove";
            // 
            // buttonRemove
            // 
            resources.ApplyResources(this.buttonRemove, "buttonRemove");
            this.buttonRemove.Image = global::XenAdmin.Properties.Resources._000_Abort_h32bit_16;
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Tag = "";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // labelProtectedInfo
            // 
            resources.ApplyResources(this.labelProtectedInfo, "labelProtectedInfo");
            this.labelProtectedInfo.Name = "labelProtectedInfo";
            // 
            // labelUniqueInfo
            // 
            resources.ApplyResources(this.labelUniqueInfo, "labelUniqueInfo");
            this.labelUniqueInfo.Name = "labelUniqueInfo";
            // 
            // VtpmManagementPage
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanelBody);
            this.DoubleBuffered = true;
            resources.ApplyResources(this, "$this");
            this.Name = "VtpmManagementPage";
            this.tableLayoutPanelBody.ResumeLayout(false);
            this.groupBoxProperties.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolTipContainerRemove.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBody;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Label labelProtectedKey;
        private System.Windows.Forms.GroupBox groupBoxProperties;
        private System.Windows.Forms.Label labelUniqueKey;
        private System.Windows.Forms.Label labelUniqueValue;
        private System.Windows.Forms.Label labelProtectedValue;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Controls.ToolTipContainer toolTipContainerRemove;
        private System.Windows.Forms.Label labelProtectedInfo;
        private System.Windows.Forms.Label labelUniqueInfo;
    }
}
