namespace XenAdmin.Dialogs
{
    partial class NetworkingPropertiesPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkingPropertiesPage));
            this.NetworkComboBox = new System.Windows.Forms.ComboBox();
            this.Network2Label = new System.Windows.Forms.Label();
            this.PurposeTextBox = new System.Windows.Forms.TextBox();
            this.PurposeLabel = new System.Windows.Forms.Label();
            this.InUseWarningText = new System.Windows.Forms.Label();
            this.haEnabledRubric = new System.Windows.Forms.Label();
            this.tableLayoutPanelBody = new System.Windows.Forms.TableLayoutPanel();
            this.IpAddressSettingsLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanelStaticSettings = new System.Windows.Forms.TableLayoutPanel();
            this.AlternateDNS2TextBox = new System.Windows.Forms.TextBox();
            this.AlternateDNS2Label = new System.Windows.Forms.Label();
            this.IPAddressLabel = new System.Windows.Forms.Label();
            this.RangeEndLabel = new System.Windows.Forms.Label();
            this.IPAddressTextBox = new System.Windows.Forms.TextBox();
            this.AlternateDNS1TextBox = new System.Windows.Forms.TextBox();
            this.PreferredDNSLabel = new System.Windows.Forms.Label();
            this.AlternateDNS1Label = new System.Windows.Forms.Label();
            this.PreferredDNSTextBox = new System.Windows.Forms.TextBox();
            this.SubnetMastLabel = new System.Windows.Forms.Label();
            this.GatewayLabel = new System.Windows.Forms.Label();
            this.GatewayTextBox = new System.Windows.Forms.TextBox();
            this.SubnetTextBox = new System.Windows.Forms.TextBox();
            this.panelHAEnabledWarning = new System.Windows.Forms.Panel();
            this.haEnabledWarningIcon = new System.Windows.Forms.PictureBox();
            this.panelInUseWarning = new System.Windows.Forms.Panel();
            this.InUseWarningIcon = new System.Windows.Forms.PictureBox();
            this.FixedIPRadioButton = new System.Windows.Forms.RadioButton();
            this.DHCPIPRadioButton = new System.Windows.Forms.RadioButton();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.tableLayoutPanelBody.SuspendLayout();
            this.tableLayoutPanelStaticSettings.SuspendLayout();
            this.panelHAEnabledWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.haEnabledWarningIcon)).BeginInit();
            this.panelInUseWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InUseWarningIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // NetworkComboBox
            // 
            this.NetworkComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.NetworkComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NetworkComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.NetworkComboBox, "NetworkComboBox");
            this.NetworkComboBox.Name = "NetworkComboBox";
            this.NetworkComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.NetworkComboBox_DrawItem);
            this.NetworkComboBox.SelectedIndexChanged += new System.EventHandler(this.NetworkComboBox_SelectedIndexChanged);
            // 
            // Network2Label
            // 
            resources.ApplyResources(this.Network2Label, "Network2Label");
            this.tableLayoutPanelBody.SetColumnSpan(this.Network2Label, 2);
            this.Network2Label.Name = "Network2Label";
            // 
            // PurposeTextBox
            // 
            resources.ApplyResources(this.PurposeTextBox, "PurposeTextBox");
            this.PurposeTextBox.Name = "PurposeTextBox";
            this.PurposeTextBox.TextChanged += new System.EventHandler(this.PurposeTextBox_TextChanged);
            // 
            // PurposeLabel
            // 
            resources.ApplyResources(this.PurposeLabel, "PurposeLabel");
            this.tableLayoutPanelBody.SetColumnSpan(this.PurposeLabel, 2);
            this.PurposeLabel.Name = "PurposeLabel";
            // 
            // InUseWarningText
            // 
            resources.ApplyResources(this.InUseWarningText, "InUseWarningText");
            this.InUseWarningText.Name = "InUseWarningText";
            // 
            // haEnabledRubric
            // 
            resources.ApplyResources(this.haEnabledRubric, "haEnabledRubric");
            this.haEnabledRubric.Name = "haEnabledRubric";
            // 
            // tableLayoutPanelBody
            // 
            this.tableLayoutPanelBody.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.tableLayoutPanelBody, "tableLayoutPanelBody");
            this.tableLayoutPanelBody.Controls.Add(this.IpAddressSettingsLabel, 0, 4);
            this.tableLayoutPanelBody.Controls.Add(this.PurposeLabel, 0, 1);
            this.tableLayoutPanelBody.Controls.Add(this.PurposeTextBox, 2, 1);
            this.tableLayoutPanelBody.Controls.Add(this.tableLayoutPanelStaticSettings, 1, 7);
            this.tableLayoutPanelBody.Controls.Add(this.panelHAEnabledWarning, 0, 0);
            this.tableLayoutPanelBody.Controls.Add(this.NetworkComboBox, 2, 2);
            this.tableLayoutPanelBody.Controls.Add(this.Network2Label, 0, 2);
            this.tableLayoutPanelBody.Controls.Add(this.panelInUseWarning, 0, 3);
            this.tableLayoutPanelBody.Controls.Add(this.FixedIPRadioButton, 1, 6);
            this.tableLayoutPanelBody.Controls.Add(this.DHCPIPRadioButton, 1, 5);
            this.tableLayoutPanelBody.Controls.Add(this.DeleteButton, 2, 8);
            this.tableLayoutPanelBody.Name = "tableLayoutPanelBody";
            // 
            // IpAddressSettingsLabel
            // 
            resources.ApplyResources(this.IpAddressSettingsLabel, "IpAddressSettingsLabel");
            this.tableLayoutPanelBody.SetColumnSpan(this.IpAddressSettingsLabel, 3);
            this.IpAddressSettingsLabel.Name = "IpAddressSettingsLabel";
            // 
            // tableLayoutPanelStaticSettings
            // 
            resources.ApplyResources(this.tableLayoutPanelStaticSettings, "tableLayoutPanelStaticSettings");
            this.tableLayoutPanelBody.SetColumnSpan(this.tableLayoutPanelStaticSettings, 2);
            this.tableLayoutPanelStaticSettings.Controls.Add(this.AlternateDNS2TextBox, 1, 5);
            this.tableLayoutPanelStaticSettings.Controls.Add(this.AlternateDNS2Label, 0, 5);
            this.tableLayoutPanelStaticSettings.Controls.Add(this.IPAddressLabel, 0, 0);
            this.tableLayoutPanelStaticSettings.Controls.Add(this.RangeEndLabel, 2, 0);
            this.tableLayoutPanelStaticSettings.Controls.Add(this.IPAddressTextBox, 1, 0);
            this.tableLayoutPanelStaticSettings.Controls.Add(this.AlternateDNS1TextBox, 1, 4);
            this.tableLayoutPanelStaticSettings.Controls.Add(this.PreferredDNSLabel, 0, 3);
            this.tableLayoutPanelStaticSettings.Controls.Add(this.AlternateDNS1Label, 0, 4);
            this.tableLayoutPanelStaticSettings.Controls.Add(this.PreferredDNSTextBox, 1, 3);
            this.tableLayoutPanelStaticSettings.Controls.Add(this.SubnetMastLabel, 0, 1);
            this.tableLayoutPanelStaticSettings.Controls.Add(this.GatewayLabel, 0, 2);
            this.tableLayoutPanelStaticSettings.Controls.Add(this.GatewayTextBox, 1, 2);
            this.tableLayoutPanelStaticSettings.Controls.Add(this.SubnetTextBox, 1, 1);
            this.tableLayoutPanelStaticSettings.Name = "tableLayoutPanelStaticSettings";
            this.tableLayoutPanelStaticSettings.TabStop = true;
            // 
            // AlternateDNS2TextBox
            // 
            resources.ApplyResources(this.AlternateDNS2TextBox, "AlternateDNS2TextBox");
            this.AlternateDNS2TextBox.Name = "AlternateDNS2TextBox";
            // 
            // AlternateDNS2Label
            // 
            resources.ApplyResources(this.AlternateDNS2Label, "AlternateDNS2Label");
            this.AlternateDNS2Label.Name = "AlternateDNS2Label";
            // 
            // IPAddressLabel
            // 
            resources.ApplyResources(this.IPAddressLabel, "IPAddressLabel");
            this.IPAddressLabel.Name = "IPAddressLabel";
            // 
            // RangeEndLabel
            // 
            resources.ApplyResources(this.RangeEndLabel, "RangeEndLabel");
            this.RangeEndLabel.Name = "RangeEndLabel";
            // 
            // IPAddressTextBox
            // 
            resources.ApplyResources(this.IPAddressTextBox, "IPAddressTextBox");
            this.IPAddressTextBox.Name = "IPAddressTextBox";
            this.IPAddressTextBox.TextChanged += new System.EventHandler(this.IPAddressTextBox_TextChanged);
            // 
            // AlternateDNS1TextBox
            // 
            resources.ApplyResources(this.AlternateDNS1TextBox, "AlternateDNS1TextBox");
            this.AlternateDNS1TextBox.Name = "AlternateDNS1TextBox";
            this.AlternateDNS1TextBox.TextChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // PreferredDNSLabel
            // 
            resources.ApplyResources(this.PreferredDNSLabel, "PreferredDNSLabel");
            this.PreferredDNSLabel.Name = "PreferredDNSLabel";
            // 
            // AlternateDNS1Label
            // 
            resources.ApplyResources(this.AlternateDNS1Label, "AlternateDNS1Label");
            this.AlternateDNS1Label.Name = "AlternateDNS1Label";
            // 
            // PreferredDNSTextBox
            // 
            resources.ApplyResources(this.PreferredDNSTextBox, "PreferredDNSTextBox");
            this.PreferredDNSTextBox.Name = "PreferredDNSTextBox";
            this.PreferredDNSTextBox.TextChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // SubnetMastLabel
            // 
            resources.ApplyResources(this.SubnetMastLabel, "SubnetMastLabel");
            this.SubnetMastLabel.Name = "SubnetMastLabel";
            // 
            // GatewayLabel
            // 
            resources.ApplyResources(this.GatewayLabel, "GatewayLabel");
            this.GatewayLabel.Name = "GatewayLabel";
            // 
            // GatewayTextBox
            // 
            resources.ApplyResources(this.GatewayTextBox, "GatewayTextBox");
            this.GatewayTextBox.Name = "GatewayTextBox";
            this.GatewayTextBox.TextChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // SubnetTextBox
            // 
            resources.ApplyResources(this.SubnetTextBox, "SubnetTextBox");
            this.SubnetTextBox.Name = "SubnetTextBox";
            this.SubnetTextBox.TextChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // panelHAEnabledWarning
            // 
            this.tableLayoutPanelBody.SetColumnSpan(this.panelHAEnabledWarning, 3);
            this.panelHAEnabledWarning.Controls.Add(this.haEnabledRubric);
            this.panelHAEnabledWarning.Controls.Add(this.haEnabledWarningIcon);
            resources.ApplyResources(this.panelHAEnabledWarning, "panelHAEnabledWarning");
            this.panelHAEnabledWarning.Name = "panelHAEnabledWarning";
            // 
            // haEnabledWarningIcon
            // 
            resources.ApplyResources(this.haEnabledWarningIcon, "haEnabledWarningIcon");
            this.haEnabledWarningIcon.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.haEnabledWarningIcon.Name = "haEnabledWarningIcon";
            this.haEnabledWarningIcon.TabStop = false;
            // 
            // panelInUseWarning
            // 
            this.tableLayoutPanelBody.SetColumnSpan(this.panelInUseWarning, 3);
            this.panelInUseWarning.Controls.Add(this.InUseWarningText);
            this.panelInUseWarning.Controls.Add(this.InUseWarningIcon);
            resources.ApplyResources(this.panelInUseWarning, "panelInUseWarning");
            this.panelInUseWarning.Name = "panelInUseWarning";
            // 
            // InUseWarningIcon
            // 
            resources.ApplyResources(this.InUseWarningIcon, "InUseWarningIcon");
            this.InUseWarningIcon.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.InUseWarningIcon.Name = "InUseWarningIcon";
            this.InUseWarningIcon.TabStop = false;
            // 
            // FixedIPRadioButton
            // 
            resources.ApplyResources(this.FixedIPRadioButton, "FixedIPRadioButton");
            this.FixedIPRadioButton.Checked = true;
            this.tableLayoutPanelBody.SetColumnSpan(this.FixedIPRadioButton, 2);
            this.FixedIPRadioButton.Name = "FixedIPRadioButton";
            this.FixedIPRadioButton.TabStop = true;
            this.FixedIPRadioButton.UseVisualStyleBackColor = true;
            this.FixedIPRadioButton.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // DHCPIPRadioButton
            // 
            resources.ApplyResources(this.DHCPIPRadioButton, "DHCPIPRadioButton");
            this.tableLayoutPanelBody.SetColumnSpan(this.DHCPIPRadioButton, 2);
            this.DHCPIPRadioButton.Name = "DHCPIPRadioButton";
            this.DHCPIPRadioButton.TabStop = true;
            this.DHCPIPRadioButton.UseVisualStyleBackColor = true;
            this.DHCPIPRadioButton.CheckedChanged += new System.EventHandler(this.SomethingChanged);
            // 
            // DeleteButton
            // 
            resources.ApplyResources(this.DeleteButton, "DeleteButton");
            this.DeleteButton.Image = global::XenAdmin.Properties.Resources._000_RemoveIPAddress_h32bit_16;
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // NetworkingPropertiesPage
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanelBody);
            this.DoubleBuffered = true;
            resources.ApplyResources(this, "$this");
            this.Name = "NetworkingPropertiesPage";
            this.tableLayoutPanelBody.ResumeLayout(false);
            this.tableLayoutPanelBody.PerformLayout();
            this.tableLayoutPanelStaticSettings.ResumeLayout(false);
            this.tableLayoutPanelStaticSettings.PerformLayout();
            this.panelHAEnabledWarning.ResumeLayout(false);
            this.panelHAEnabledWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.haEnabledWarningIcon)).EndInit();
            this.panelInUseWarning.ResumeLayout(false);
            this.panelInUseWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InUseWarningIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label AlternateDNS1Label;
        private System.Windows.Forms.Label PreferredDNSLabel;
        private System.Windows.Forms.Label RangeEndLabel;
        private System.Windows.Forms.Label IPAddressLabel;
        private System.Windows.Forms.Label GatewayLabel;
        private System.Windows.Forms.Label SubnetMastLabel;
        internal System.Windows.Forms.ComboBox NetworkComboBox;
        internal System.Windows.Forms.TextBox AlternateDNS1TextBox;
        internal System.Windows.Forms.TextBox PreferredDNSTextBox;
        internal System.Windows.Forms.TextBox GatewayTextBox;
        internal System.Windows.Forms.TextBox SubnetTextBox;
        internal System.Windows.Forms.RadioButton FixedIPRadioButton;
        internal System.Windows.Forms.RadioButton DHCPIPRadioButton;
        internal System.Windows.Forms.TextBox IPAddressTextBox;
        private System.Windows.Forms.Label Network2Label;
        internal System.Windows.Forms.TextBox PurposeTextBox;
        private System.Windows.Forms.Label PurposeLabel;
        private System.Windows.Forms.Label InUseWarningText;
        private System.Windows.Forms.PictureBox InUseWarningIcon;
        private System.Windows.Forms.PictureBox haEnabledWarningIcon;
        private System.Windows.Forms.Label haEnabledRubric;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelStaticSettings;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBody;
        private System.Windows.Forms.Panel panelHAEnabledWarning;
        private System.Windows.Forms.Panel panelInUseWarning;
        private System.Windows.Forms.Label IpAddressSettingsLabel;
        private System.Windows.Forms.Label AlternateDNS2Label;
        internal System.Windows.Forms.TextBox AlternateDNS2TextBox;
        private System.Windows.Forms.Button DeleteButton;
    }
}
