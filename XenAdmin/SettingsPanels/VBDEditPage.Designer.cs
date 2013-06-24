namespace XenAdmin.SettingsPanels
{
    partial class VBDEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VBDEditPage));
            this.modeLabel = new System.Windows.Forms.Label();
            this.deviceLabel = new System.Windows.Forms.Label();
            this.devicePositionComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelDevicePositionMsg = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTipContainer1 = new XenAdmin.Controls.ToolTipContainer();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.PriorityToolTipContainer = new XenAdmin.Controls.ToolTipContainer();
            this.DiskPriorityPanel = new System.Windows.Forms.Panel();
            this.diskAccessPriorityTrackBar = new XenAdmin.Controls.TransparentTrackBar();
            this.lblHighest = new System.Windows.Forms.Label();
            this.lblLowest = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolTipContainer1.SuspendLayout();
            this.PriorityToolTipContainer.SuspendLayout();
            this.DiskPriorityPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // modeLabel
            // 
            resources.ApplyResources(this.modeLabel, "modeLabel");
            this.modeLabel.Name = "modeLabel";
            // 
            // deviceLabel
            // 
            resources.ApplyResources(this.deviceLabel, "deviceLabel");
            this.deviceLabel.Name = "deviceLabel";
            // 
            // devicePositionComboBox
            // 
            resources.ApplyResources(this.devicePositionComboBox, "devicePositionComboBox");
            this.devicePositionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.devicePositionComboBox.FormattingEnabled = true;
            this.devicePositionComboBox.Name = "devicePositionComboBox";
            this.devicePositionComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBoxDevicePosition_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // labelDevicePositionMsg
            // 
            resources.ApplyResources(this.labelDevicePositionMsg, "labelDevicePositionMsg");
            this.labelDevicePositionMsg.AutoEllipsis = true;
            this.labelDevicePositionMsg.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labelDevicePositionMsg.MaximumSize = new System.Drawing.Size(320, 32000);
            this.labelDevicePositionMsg.Name = "labelDevicePositionMsg";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // toolTipContainer1
            // 
            resources.ApplyResources(this.toolTipContainer1, "toolTipContainer1");
            this.toolTipContainer1.Controls.Add(this.modeComboBox);
            this.toolTipContainer1.Name = "toolTipContainer1";
            // 
            // modeComboBox
            // 
            resources.ApplyResources(this.modeComboBox, "modeComboBox");
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Name = "modeComboBox";
            // 
            // PriorityToolTipContainer
            // 
            resources.ApplyResources(this.PriorityToolTipContainer, "PriorityToolTipContainer");
            this.PriorityToolTipContainer.Controls.Add(this.DiskPriorityPanel);
            this.PriorityToolTipContainer.Name = "PriorityToolTipContainer";
            // 
            // DiskPriorityPanel
            // 
            this.DiskPriorityPanel.Controls.Add(this.diskAccessPriorityTrackBar);
            this.DiskPriorityPanel.Controls.Add(this.lblHighest);
            this.DiskPriorityPanel.Controls.Add(this.lblLowest);
            resources.ApplyResources(this.DiskPriorityPanel, "DiskPriorityPanel");
            this.DiskPriorityPanel.Name = "DiskPriorityPanel";
            // 
            // diskAccessPriorityTrackBar
            // 
            resources.ApplyResources(this.diskAccessPriorityTrackBar, "diskAccessPriorityTrackBar");
            this.diskAccessPriorityTrackBar.BackColor = System.Drawing.Color.Transparent;
            this.diskAccessPriorityTrackBar.Name = "diskAccessPriorityTrackBar";
            // 
            // lblHighest
            // 
            resources.ApplyResources(this.lblHighest, "lblHighest");
            this.lblHighest.Name = "lblHighest";
            // 
            // lblLowest
            // 
            resources.ApplyResources(this.lblLowest, "lblLowest");
            this.lblLowest.BackColor = System.Drawing.Color.Transparent;
            this.lblLowest.Name = "lblLowest";
            // 
            // VBDEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.toolTipContainer1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.PriorityToolTipContainer);
            this.Controls.Add(this.labelDevicePositionMsg);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.devicePositionComboBox);
            this.Controls.Add(this.deviceLabel);
            this.Controls.Add(this.modeLabel);
            this.DoubleBuffered = true;
            this.Name = "VBDEditPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolTipContainer1.ResumeLayout(false);
            this.PriorityToolTipContainer.ResumeLayout(false);
            this.DiskPriorityPanel.ResumeLayout(false);
            this.DiskPriorityPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.Label deviceLabel;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.ComboBox devicePositionComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelDevicePositionMsg;
        private System.Windows.Forms.Label lblLowest;
        private System.Windows.Forms.Label lblHighest;
        private XenAdmin.Controls.TransparentTrackBar diskAccessPriorityTrackBar;
        private XenAdmin.Controls.ToolTipContainer PriorityToolTipContainer;
        private System.Windows.Forms.Panel DiskPriorityPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private XenAdmin.Controls.ToolTipContainer toolTipContainer1;

    }
}
