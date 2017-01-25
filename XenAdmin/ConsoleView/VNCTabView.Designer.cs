namespace XenAdmin.ConsoleView
{
    partial class VNCTabView
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
            Program.AssertOnEventThread();

            UnregisterEventListeners();

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing && vncScreen != null && !vncScreen.IsDisposed)
            {
                vncScreen.GpuStatusChanged -= ShowGpuWarningIfRequired;
                vncScreen.Dispose();
            }

            if (this.fullscreenForm != null)
            {
                fullscreenForm.Hide();
                fullscreenForm.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VNCTabView));
            this.sendCAD = new System.Windows.Forms.Button();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.scaleCheckBox = new System.Windows.Forms.CheckBox();
            this.fullscreenButton = new System.Windows.Forms.Button();
            this.dockButton = new System.Windows.Forms.Button();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.LifeCycleMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.powerStateLabel = new System.Windows.Forms.Label();
            this.dedicatedGpuWarning = new System.Windows.Forms.Label();
            this.gradientPanel1 = new XenAdmin.Controls.GradientPanel.GradientPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.HostLabel = new System.Windows.Forms.Label();
            this.buttonSSH = new System.Windows.Forms.Button();
            this.toggleConsoleButton = new System.Windows.Forms.Button();
            this.multipleDvdIsoList1 = new XenAdmin.Controls.MultipleDvdIsoList();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.gradientPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // sendCAD
            // 
            resources.ApplyResources(this.sendCAD, "sendCAD");
            this.sendCAD.Name = "sendCAD";
            this.sendCAD.UseVisualStyleBackColor = true;
            this.sendCAD.Click += new System.EventHandler(this.sendCAD_Click);
            // 
            // contentPanel
            // 
            resources.ApplyResources(this.contentPanel, "contentPanel");
            this.contentPanel.Name = "contentPanel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.sendCAD, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.scaleCheckBox, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.fullscreenButton, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.dockButton, 3, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // scaleCheckBox
            // 
            resources.ApplyResources(this.scaleCheckBox, "scaleCheckBox");
            this.scaleCheckBox.Name = "scaleCheckBox";
            this.scaleCheckBox.UseVisualStyleBackColor = true;
            this.scaleCheckBox.CheckedChanged += new System.EventHandler(this.scaleCheckBox_CheckedChanged);
            // 
            // fullscreenButton
            // 
            resources.ApplyResources(this.fullscreenButton, "fullscreenButton");
            this.fullscreenButton.Name = "fullscreenButton";
            this.fullscreenButton.UseVisualStyleBackColor = true;
            this.fullscreenButton.Click += new System.EventHandler(this.fullscreenButton_Click);
            // 
            // dockButton
            // 
            resources.ApplyResources(this.dockButton, "dockButton");
            this.dockButton.Image = global::XenAdmin.Properties.Resources.detach_24;
            this.dockButton.Name = "dockButton";
            this.dockButton.UseVisualStyleBackColor = true;
            this.dockButton.Click += new System.EventHandler(this.dockButton_Click);
            // 
            // tip
            // 
            this.tip.ShowAlways = true;
            // 
            // LifeCycleMenuStrip
            // 
            this.LifeCycleMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.LifeCycleMenuStrip.Name = "LifeCycleMenuStrip";
            resources.ApplyResources(this.LifeCycleMenuStrip, "LifeCycleMenuStrip");
            this.LifeCycleMenuStrip.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.LifeCycleMenuStrip_Closing);
            this.LifeCycleMenuStrip.Opened += new System.EventHandler(this.LifeCycleMenuStrip_Opened);
            // 
            // powerStateLabel
            // 
            this.powerStateLabel.AutoEllipsis = true;
            this.powerStateLabel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.powerStateLabel, "powerStateLabel");
            this.powerStateLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.powerStateLabel.Name = "powerStateLabel";
            this.powerStateLabel.Click += new System.EventHandler(this.powerStateLabel_Click);
            // 
            // dedicatedGpuWarning
            // 
            this.dedicatedGpuWarning.AutoEllipsis = true;
            this.dedicatedGpuWarning.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.dedicatedGpuWarning, "dedicatedGpuWarning");
            this.dedicatedGpuWarning.ForeColor = System.Drawing.SystemColors.ControlText;
            this.dedicatedGpuWarning.Name = "dedicatedGpuWarning";
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.Controls.Add(this.tableLayoutPanel2);
            resources.ApplyResources(this.gradientPanel1, "gradientPanel1");
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Scheme = XenAdmin.Controls.GradientPanel.GradientPanel.Schemes.Tab;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.HostLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonSSH, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.toggleConsoleButton, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.multipleDvdIsoList1, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox1, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // HostLabel
            // 
            this.HostLabel.AutoEllipsis = true;
            resources.ApplyResources(this.HostLabel, "HostLabel");
            this.HostLabel.ForeColor = System.Drawing.Color.White;
            this.HostLabel.Name = "HostLabel";
            // 
            // buttonSSH
            // 
            resources.ApplyResources(this.buttonSSH, "buttonSSH");
            this.buttonSSH.Name = "buttonSSH";
            this.buttonSSH.UseVisualStyleBackColor = true;
            this.buttonSSH.Click += new System.EventHandler(this.buttonSSH_Click);
            // 
            // toggleConsoleButton
            // 
            resources.ApplyResources(this.toggleConsoleButton, "toggleConsoleButton");
            this.toggleConsoleButton.Name = "toggleConsoleButton";
            this.tip.SetToolTip(this.toggleConsoleButton, resources.GetString("toggleConsoleButton.ToolTip"));
            this.toggleConsoleButton.UseVisualStyleBackColor = true;
            this.toggleConsoleButton.Click += new System.EventHandler(this.toggleConsoleButton_Click);
            // 
            // multipleDvdIsoList1
            // 
            resources.ApplyResources(this.multipleDvdIsoList1, "multipleDvdIsoList1");
            this.multipleDvdIsoList1.LabelNewCdForeColor = System.Drawing.SystemColors.HotTrack;
            this.multipleDvdIsoList1.LabelSingleDvdForeColor = System.Drawing.SystemColors.ControlText;
            this.multipleDvdIsoList1.LinkLabelLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.multipleDvdIsoList1.Name = "multipleDvdIsoList1";
            this.multipleDvdIsoList1.VM = null;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._001_LifeCycle_h32bit_24;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.LifeCycleButton_MouseClick);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseEnter += new System.EventHandler(this.pictureBox1_MouseEnter);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // VNCTabView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.dedicatedGpuWarning);
            this.Controls.Add(this.powerStateLabel);
            this.Controls.Add(this.gradientPanel1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "VNCTabView";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.gradientPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button dockButton;
        private System.Windows.Forms.Button sendCAD;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.CheckBox scaleCheckBox;
        private System.Windows.Forms.Button fullscreenButton;
        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ContextMenuStrip LifeCycleMenuStrip;
        private System.Windows.Forms.PictureBox pictureBox1;
        private XenAdmin.Controls.GradientPanel.GradientPanel gradientPanel1;
        private System.Windows.Forms.Label HostLabel;
        private System.Windows.Forms.Button toggleConsoleButton;
        private XenAdmin.Controls.MultipleDvdIsoList multipleDvdIsoList1;
        private System.Windows.Forms.Label powerStateLabel;
        private System.Windows.Forms.Label dedicatedGpuWarning;
        private System.Windows.Forms.Button buttonSSH;
    }
}
