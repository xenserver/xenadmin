namespace XenAdmin.Controls.ConsoleTab
{
    partial class ConnectionBar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionBar));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.labelConnection = new System.Windows.Forms.ToolStripLabel();
            this.buttonPin = new System.Windows.Forms.ToolStripButton();
            this.buttonExitFullScreen = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonPin,
            this.labelConnection,
            this.buttonExitFullScreen});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Resize += new System.EventHandler(this.toolStrip1_Resize);
            // 
            // labelConnection
            // 
            resources.ApplyResources(this.labelConnection, "labelConnection");
            this.labelConnection.BackColor = System.Drawing.Color.Transparent;
            this.labelConnection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.labelConnection.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelConnection.Margin = new System.Windows.Forms.Padding(2, 1, 2, 2);
            this.labelConnection.Name = "labelConnection";
            // 
            // buttonPin
            // 
            this.buttonPin.CheckOnClick = true;
            this.buttonPin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPin.Image = global::XenAdmin.Properties.Resources._001_Pin_h32bit_16;
            resources.ApplyResources(this.buttonPin, "buttonPin");
            this.buttonPin.Margin = new System.Windows.Forms.Padding(2, 1, 2, 2);
            this.buttonPin.Name = "buttonPin";
            this.buttonPin.CheckedChanged += new System.EventHandler(this.buttonPin_CheckedChanged);
            // 
            // buttonExitFullScreen
            // 
            this.buttonExitFullScreen.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonExitFullScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonExitFullScreen.Image = global::XenAdmin.Properties.Resources._001_WindowView_h32bit_16;
            resources.ApplyResources(this.buttonExitFullScreen, "buttonExitFullScreen");
            this.buttonExitFullScreen.Margin = new System.Windows.Forms.Padding(2, 1, 1, 2);
            this.buttonExitFullScreen.Name = "buttonExitFullScreen";
            this.buttonExitFullScreen.Click += new System.EventHandler(this.buttonExitFullScreen_Click);
            // 
            // ConnectionBar
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.toolStrip1);
            this.MinimumSize = new System.Drawing.Size(298, 24);
            this.Name = "ConnectionBar";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonPin;
        private System.Windows.Forms.ToolStripLabel labelConnection;
        private System.Windows.Forms.ToolStripButton buttonExitFullScreen;
    }
}
