namespace XenAdmin.Dialogs.OptionsPages
{
    partial class ConnectionOptionsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionOptionsPage));
            this.ConnectionTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.TimeoutGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.ConnectionTimeoutLabel = new System.Windows.Forms.Label();
            this.ConnectionTimeoutNud = new System.Windows.Forms.NumericUpDown();
            this.SecondsLabel = new System.Windows.Forms.Label();
            this.TimeOutLabel = new System.Windows.Forms.Label();
            this.ProxyGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BypassForServersCheckbox = new System.Windows.Forms.CheckBox();
            this.ProxyPasswordTextBox = new System.Windows.Forms.TextBox();
            this.ProxyUsernameLabel = new System.Windows.Forms.Label();
            this.ProxyUsernameTextBox = new System.Windows.Forms.TextBox();
            this.ProxyPasswordLabel = new System.Windows.Forms.Label();
            this.AuthenticationCheckBox = new System.Windows.Forms.CheckBox();
            this.ProxyAddressLabel = new System.Windows.Forms.Label();
            this.UseProxyRadioButton = new System.Windows.Forms.RadioButton();
            this.DirectConnectionRadioButton = new System.Windows.Forms.RadioButton();
            this.UseIERadioButton = new System.Windows.Forms.RadioButton();
            this.ProxyAddressTextBox = new System.Windows.Forms.TextBox();
            this.ProxyPortTextBox = new System.Windows.Forms.TextBox();
            this.ProxyPortLabel = new System.Windows.Forms.Label();
            this.ConnectionTableLayoutPanel.SuspendLayout();
            this.TimeoutGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConnectionTimeoutNud)).BeginInit();
            this.ProxyGroupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectionTableLayoutPanel
            // 
            resources.ApplyResources(this.ConnectionTableLayoutPanel, "ConnectionTableLayoutPanel");
            this.ConnectionTableLayoutPanel.Controls.Add(this.TimeoutGroupBox, 0, 2);
            this.ConnectionTableLayoutPanel.Controls.Add(this.ProxyGroupBox, 0, 1);
            this.ConnectionTableLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.ConnectionTableLayoutPanel.Name = "ConnectionTableLayoutPanel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // TimeoutGroupBox
            // 
            resources.ApplyResources(this.TimeoutGroupBox, "TimeoutGroupBox");
            this.TimeoutGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.TimeoutGroupBox.Controls.Add(this.TimeOutLabel);
            this.TimeoutGroupBox.Name = "TimeoutGroupBox";
            this.TimeoutGroupBox.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.ConnectionTimeoutLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ConnectionTimeoutNud, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.SecondsLabel, 2, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // ConnectionTimeoutLabel
            // 
            resources.ApplyResources(this.ConnectionTimeoutLabel, "ConnectionTimeoutLabel");
            this.ConnectionTimeoutLabel.Name = "ConnectionTimeoutLabel";
            // 
            // ConnectionTimeoutNud
            // 
            resources.ApplyResources(this.ConnectionTimeoutNud, "ConnectionTimeoutNud");
            this.ConnectionTimeoutNud.Maximum = new decimal(new int[] {
            2147483,
            0,
            0,
            0});
            this.ConnectionTimeoutNud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ConnectionTimeoutNud.Name = "ConnectionTimeoutNud";
            this.ConnectionTimeoutNud.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // SecondsLabel
            // 
            resources.ApplyResources(this.SecondsLabel, "SecondsLabel");
            this.SecondsLabel.Name = "SecondsLabel";
            // 
            // TimeOutLabel
            // 
            resources.ApplyResources(this.TimeOutLabel, "TimeOutLabel");
            this.TimeOutLabel.Name = "TimeOutLabel";
            // 
            // ProxyGroupBox
            // 
            resources.ApplyResources(this.ProxyGroupBox, "ProxyGroupBox");
            this.ProxyGroupBox.Controls.Add(this.tableLayoutPanel1);
            this.ProxyGroupBox.Name = "ProxyGroupBox";
            this.ProxyGroupBox.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.BypassForServersCheckbox, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.ProxyPasswordTextBox, 6, 6);
            this.tableLayoutPanel1.Controls.Add(this.ProxyUsernameLabel, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.ProxyUsernameTextBox, 4, 6);
            this.tableLayoutPanel1.Controls.Add(this.ProxyPasswordLabel, 5, 6);
            this.tableLayoutPanel1.Controls.Add(this.AuthenticationCheckBox, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.ProxyAddressLabel, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.UseProxyRadioButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.DirectConnectionRadioButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.UseIERadioButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ProxyAddressTextBox, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.ProxyPortTextBox, 5, 3);
            this.tableLayoutPanel1.Controls.Add(this.ProxyPortLabel, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // BypassForServersCheckbox
            // 
            resources.ApplyResources(this.BypassForServersCheckbox, "BypassForServersCheckbox");
            this.tableLayoutPanel1.SetColumnSpan(this.BypassForServersCheckbox, 6);
            this.BypassForServersCheckbox.Name = "BypassForServersCheckbox";
            this.BypassForServersCheckbox.UseVisualStyleBackColor = true;
            this.BypassForServersCheckbox.CheckedChanged += new System.EventHandler(this.BypassForServersCheckbox_CheckedChanged);
            // 
            // ProxyPasswordTextBox
            // 
            resources.ApplyResources(this.ProxyPasswordTextBox, "ProxyPasswordTextBox");
            this.ProxyPasswordTextBox.Name = "ProxyPasswordTextBox";
            this.ProxyPasswordTextBox.UseSystemPasswordChar = true;
            this.ProxyPasswordTextBox.TextChanged += new System.EventHandler(this.ProxyPasswordTextBox_TextChanged);
            // 
            // ProxyUsernameLabel
            // 
            resources.ApplyResources(this.ProxyUsernameLabel, "ProxyUsernameLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.ProxyUsernameLabel, 2);
            this.ProxyUsernameLabel.Name = "ProxyUsernameLabel";
            // 
            // ProxyUsernameTextBox
            // 
            resources.ApplyResources(this.ProxyUsernameTextBox, "ProxyUsernameTextBox");
            this.ProxyUsernameTextBox.Name = "ProxyUsernameTextBox";
            this.ProxyUsernameTextBox.TextChanged += new System.EventHandler(this.ProxyUsernameTextBox_TextChanged);
            // 
            // ProxyPasswordLabel
            // 
            resources.ApplyResources(this.ProxyPasswordLabel, "ProxyPasswordLabel");
            this.ProxyPasswordLabel.Name = "ProxyPasswordLabel";
            // 
            // AuthenticationCheckBox
            // 
            resources.ApplyResources(this.AuthenticationCheckBox, "AuthenticationCheckBox");
            this.tableLayoutPanel1.SetColumnSpan(this.AuthenticationCheckBox, 6);
            this.AuthenticationCheckBox.Name = "AuthenticationCheckBox";
            this.AuthenticationCheckBox.UseVisualStyleBackColor = true;
            this.AuthenticationCheckBox.CheckedChanged += new System.EventHandler(this.AuthenticationCheckBox_CheckedChanged);
            // 
            // ProxyAddressLabel
            // 
            resources.ApplyResources(this.ProxyAddressLabel, "ProxyAddressLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.ProxyAddressLabel, 2);
            this.ProxyAddressLabel.Name = "ProxyAddressLabel";
            // 
            // UseProxyRadioButton
            // 
            resources.ApplyResources(this.UseProxyRadioButton, "UseProxyRadioButton");
            this.tableLayoutPanel1.SetColumnSpan(this.UseProxyRadioButton, 6);
            this.UseProxyRadioButton.Name = "UseProxyRadioButton";
            this.UseProxyRadioButton.TabStop = true;
            this.UseProxyRadioButton.UseVisualStyleBackColor = true;
            this.UseProxyRadioButton.CheckedChanged += new System.EventHandler(this.UseProxyRadioButton_CheckedChanged);
            // 
            // DirectConnectionRadioButton
            // 
            resources.ApplyResources(this.DirectConnectionRadioButton, "DirectConnectionRadioButton");
            this.tableLayoutPanel1.SetColumnSpan(this.DirectConnectionRadioButton, 6);
            this.DirectConnectionRadioButton.Name = "DirectConnectionRadioButton";
            this.DirectConnectionRadioButton.TabStop = true;
            this.DirectConnectionRadioButton.UseVisualStyleBackColor = true;
            // 
            // UseIERadioButton
            // 
            resources.ApplyResources(this.UseIERadioButton, "UseIERadioButton");
            this.tableLayoutPanel1.SetColumnSpan(this.UseIERadioButton, 6);
            this.UseIERadioButton.Name = "UseIERadioButton";
            this.UseIERadioButton.TabStop = true;
            this.UseIERadioButton.UseVisualStyleBackColor = true;
            // 
            // ProxyAddressTextBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.ProxyAddressTextBox, 2);
            resources.ApplyResources(this.ProxyAddressTextBox, "ProxyAddressTextBox");
            this.ProxyAddressTextBox.Name = "ProxyAddressTextBox";
            this.ProxyAddressTextBox.TextChanged += new System.EventHandler(this.ProxyAddressTextBox_TextChanged);
            // 
            // ProxyPortTextBox
            // 
            resources.ApplyResources(this.ProxyPortTextBox, "ProxyPortTextBox");
            this.ProxyPortTextBox.Name = "ProxyPortTextBox";
            this.ProxyPortTextBox.TextChanged += new System.EventHandler(this.ProxyPortTextBox_TextChanged);
            // 
            // ProxyPortLabel
            // 
            resources.ApplyResources(this.ProxyPortLabel, "ProxyPortLabel");
            this.ProxyPortLabel.Name = "ProxyPortLabel";
            // 
            // ConnectionOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.ConnectionTableLayoutPanel);
            this.Name = "ConnectionOptionsPage";
            this.ConnectionTableLayoutPanel.ResumeLayout(false);
            this.ConnectionTableLayoutPanel.PerformLayout();
            this.TimeoutGroupBox.ResumeLayout(false);
            this.TimeoutGroupBox.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConnectionTimeoutNud)).EndInit();
            this.ProxyGroupBox.ResumeLayout(false);
            this.ProxyGroupBox.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel ConnectionTableLayoutPanel;
        private XenAdmin.Controls.DecentGroupBox TimeoutGroupBox;
        private System.Windows.Forms.NumericUpDown ConnectionTimeoutNud;
        private System.Windows.Forms.Label TimeOutLabel;
        private System.Windows.Forms.Label ConnectionTimeoutLabel;
        private System.Windows.Forms.Label SecondsLabel;
        private XenAdmin.Controls.DecentGroupBox ProxyGroupBox;
        private System.Windows.Forms.RadioButton DirectConnectionRadioButton;
        private System.Windows.Forms.TextBox ProxyAddressTextBox;
        private System.Windows.Forms.RadioButton UseProxyRadioButton;
        private System.Windows.Forms.TextBox ProxyPortTextBox;
        private System.Windows.Forms.Label ProxyAddressLabel;
        private System.Windows.Forms.RadioButton UseIERadioButton;
        private System.Windows.Forms.Label ProxyPortLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox AuthenticationCheckBox;
        private System.Windows.Forms.CheckBox BypassForServersCheckbox;
        private System.Windows.Forms.TextBox ProxyPasswordTextBox;
        private System.Windows.Forms.Label ProxyUsernameLabel;
        private System.Windows.Forms.TextBox ProxyUsernameTextBox;
        private System.Windows.Forms.Label ProxyPasswordLabel;
    }
}
