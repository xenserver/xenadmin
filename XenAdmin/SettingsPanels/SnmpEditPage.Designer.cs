using System.Windows.Forms;
using DataGridView = System.Windows.Forms.DataGridView;

namespace XenAdmin.SettingsPanels
{
    partial class SnmpEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SnmpEditPage));
            this.SnmpTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.DescLabel = new System.Windows.Forms.Label();
            this.SnmpServiceStatusPanel = new System.Windows.Forms.TableLayoutPanel();
            this.EnableSnmpCheckBox = new System.Windows.Forms.CheckBox();
            this.ServiceStatusPicture = new System.Windows.Forms.PictureBox();
            this.ServiceStatusLabel = new System.Windows.Forms.Label();
            this.DebugLogCheckBox = new System.Windows.Forms.CheckBox();
            this.SupportV2cCheckBox = new System.Windows.Forms.CheckBox();
            this.SnmpV2cGroupBox = new System.Windows.Forms.GroupBox();
            this.SnmpV2cPanel = new System.Windows.Forms.TableLayoutPanel();
            this.CommunityLabel = new System.Windows.Forms.Label();
            this.CommunityTextBox = new System.Windows.Forms.TextBox();
            this.SupportV3CheckBox = new System.Windows.Forms.CheckBox();
            this.SnmpV3GroupBox = new System.Windows.Forms.GroupBox();
            this.SnmpV3Panel = new System.Windows.Forms.TableLayoutPanel();
            this.UserNameLabel = new System.Windows.Forms.Label();
            this.UserNameTextBox = new System.Windows.Forms.TextBox();
            this.AuthenticationPasswordLabel = new System.Windows.Forms.Label();
            this.AuthenticationPasswordLabelTextBox = new System.Windows.Forms.TextBox();
            this.AuthenticationProtocolLabel = new System.Windows.Forms.Label();
            this.AuthenticationProtocolComboBox = new System.Windows.Forms.ComboBox();
            this.PrivacyPasswordLabel = new System.Windows.Forms.Label();
            this.PrivacyPasswordTextBox = new System.Windows.Forms.TextBox();
            this.PrivacyProtocolLabel = new System.Windows.Forms.Label();
            this.PrivacyProtocolComboBox = new System.Windows.Forms.ComboBox();
            this.RetrieveSnmpPanel = new System.Windows.Forms.TableLayoutPanel();
            this.RetrieveSnmpPicture = new System.Windows.Forms.PictureBox();
            this.RetrieveSnmpLabel = new System.Windows.Forms.Label();
            this.GeneralConfigureGroupBox = new System.Windows.Forms.GroupBox();
            this.GeneralConfigTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SnmpTableLayoutPanel.SuspendLayout();
            this.SnmpServiceStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ServiceStatusPicture)).BeginInit();
            this.SnmpV2cGroupBox.SuspendLayout();
            this.SnmpV2cPanel.SuspendLayout();
            this.SnmpV3GroupBox.SuspendLayout();
            this.SnmpV3Panel.SuspendLayout();
            this.RetrieveSnmpPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RetrieveSnmpPicture)).BeginInit();
            this.GeneralConfigureGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // SnmpTableLayoutPanel
            // 
            resources.ApplyResources(this.SnmpTableLayoutPanel, "SnmpTableLayoutPanel");
            this.SnmpTableLayoutPanel.Controls.Add(this.DescLabel, 0, 0);
            this.SnmpTableLayoutPanel.Controls.Add(this.SnmpServiceStatusPanel, 0, 1);
            this.SnmpTableLayoutPanel.Controls.Add(this.DebugLogCheckBox, 0, 2);
            this.SnmpTableLayoutPanel.Controls.Add(this.SupportV2cCheckBox, 0, 3);
            this.SnmpTableLayoutPanel.Controls.Add(this.SnmpV2cGroupBox, 0, 4);
            this.SnmpTableLayoutPanel.Controls.Add(this.SupportV3CheckBox, 0, 5);
            this.SnmpTableLayoutPanel.Controls.Add(this.SnmpV3GroupBox, 0, 6);
            this.SnmpTableLayoutPanel.Controls.Add(this.RetrieveSnmpPanel, 0, 7);
            this.SnmpTableLayoutPanel.Name = "SnmpTableLayoutPanel";
            // 
            // DescLabel
            // 
            resources.ApplyResources(this.DescLabel, "DescLabel");
            this.DescLabel.Name = "DescLabel";
            // 
            // SnmpServiceStatusPanel
            // 
            resources.ApplyResources(this.SnmpServiceStatusPanel, "SnmpServiceStatusPanel");
            this.SnmpServiceStatusPanel.Controls.Add(this.EnableSnmpCheckBox, 0, 0);
            this.SnmpServiceStatusPanel.Controls.Add(this.ServiceStatusPicture, 1, 0);
            this.SnmpServiceStatusPanel.Controls.Add(this.ServiceStatusLabel, 2, 0);
            this.SnmpServiceStatusPanel.Name = "SnmpServiceStatusPanel";
            // 
            // EnableSnmpCheckBox
            // 
            resources.ApplyResources(this.EnableSnmpCheckBox, "EnableSnmpCheckBox");
            this.EnableSnmpCheckBox.Name = "EnableSnmpCheckBox";
            this.EnableSnmpCheckBox.UseVisualStyleBackColor = true;
            this.EnableSnmpCheckBox.CheckedChanged += new System.EventHandler(this.EnableSNMPCheckBox_CheckedChanged);
            // 
            // ServiceStatusPicture
            // 
            resources.ApplyResources(this.ServiceStatusPicture, "ServiceStatusPicture");
            this.ServiceStatusPicture.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.ServiceStatusPicture.Name = "ServiceStatusPicture";
            this.ServiceStatusPicture.TabStop = false;
            // 
            // ServiceStatusLabel
            // 
            resources.ApplyResources(this.ServiceStatusLabel, "ServiceStatusLabel");
            this.ServiceStatusLabel.Name = "ServiceStatusLabel";
            // 
            // DebugLogCheckBox
            // 
            resources.ApplyResources(this.DebugLogCheckBox, "DebugLogCheckBox");
            this.SnmpTableLayoutPanel.SetColumnSpan(this.DebugLogCheckBox, 2);
            this.DebugLogCheckBox.Name = "DebugLogCheckBox";
            this.DebugLogCheckBox.UseVisualStyleBackColor = true;
            // 
            // SupportV2cCheckBox
            // 
            resources.ApplyResources(this.SupportV2cCheckBox, "SupportV2cCheckBox");
            this.SupportV2cCheckBox.Checked = true;
            this.SupportV2cCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SnmpTableLayoutPanel.SetColumnSpan(this.SupportV2cCheckBox, 2);
            this.SupportV2cCheckBox.Name = "SupportV2cCheckBox";
            this.SupportV2cCheckBox.UseVisualStyleBackColor = true;
            this.SupportV2cCheckBox.CheckedChanged += new System.EventHandler(this.SupportV2CheckBox_CheckedChanged);
            // 
            // SnmpV2cGroupBox
            // 
            resources.ApplyResources(this.SnmpV2cGroupBox, "SnmpV2cGroupBox");
            this.SnmpV2cGroupBox.Controls.Add(this.SnmpV2cPanel);
            this.SnmpV2cGroupBox.Name = "SnmpV2cGroupBox";
            this.SnmpV2cGroupBox.TabStop = false;
            // 
            // SnmpV2cPanel
            // 
            resources.ApplyResources(this.SnmpV2cPanel, "SnmpV2cPanel");
            this.SnmpV2cPanel.Controls.Add(this.CommunityLabel, 0, 0);
            this.SnmpV2cPanel.Controls.Add(this.CommunityTextBox, 1, 0);
            this.SnmpV2cPanel.Name = "SnmpV2cPanel";
            // 
            // CommunityLabel
            // 
            resources.ApplyResources(this.CommunityLabel, "CommunityLabel");
            this.CommunityLabel.Name = "CommunityLabel";
            // 
            // CommunityTextBox
            // 
            resources.ApplyResources(this.CommunityTextBox, "CommunityTextBox");
            this.CommunityTextBox.Name = "CommunityTextBox";
            // 
            // SupportV3CheckBox
            // 
            resources.ApplyResources(this.SupportV3CheckBox, "SupportV3CheckBox");
            this.SnmpTableLayoutPanel.SetColumnSpan(this.SupportV3CheckBox, 2);
            this.SupportV3CheckBox.Name = "SupportV3CheckBox";
            this.SupportV3CheckBox.UseVisualStyleBackColor = true;
            this.SupportV3CheckBox.CheckedChanged += new System.EventHandler(this.SupportV3CheckBox_CheckedChanged);
            // 
            // SnmpV3GroupBox
            // 
            resources.ApplyResources(this.SnmpV3GroupBox, "SnmpV3GroupBox");
            this.SnmpV3GroupBox.Controls.Add(this.SnmpV3Panel);
            this.SnmpV3GroupBox.Name = "SnmpV3GroupBox";
            this.SnmpV3GroupBox.TabStop = false;
            // 
            // SnmpV3Panel
            // 
            resources.ApplyResources(this.SnmpV3Panel, "SnmpV3Panel");
            this.SnmpV3Panel.Controls.Add(this.UserNameLabel, 0, 2);
            this.SnmpV3Panel.Controls.Add(this.UserNameTextBox, 1, 2);
            this.SnmpV3Panel.Controls.Add(this.AuthenticationPasswordLabel, 0, 3);
            this.SnmpV3Panel.Controls.Add(this.AuthenticationPasswordLabelTextBox, 1, 3);
            this.SnmpV3Panel.Controls.Add(this.AuthenticationProtocolLabel, 0, 4);
            this.SnmpV3Panel.Controls.Add(this.AuthenticationProtocolComboBox, 1, 4);
            this.SnmpV3Panel.Controls.Add(this.PrivacyPasswordLabel, 0, 5);
            this.SnmpV3Panel.Controls.Add(this.PrivacyPasswordTextBox, 1, 5);
            this.SnmpV3Panel.Controls.Add(this.PrivacyProtocolLabel, 0, 6);
            this.SnmpV3Panel.Controls.Add(this.PrivacyProtocolComboBox, 1, 6);
            this.SnmpV3Panel.Name = "SnmpV3Panel";
            // 
            // UserNameLabel
            // 
            resources.ApplyResources(this.UserNameLabel, "UserNameLabel");
            this.UserNameLabel.Name = "UserNameLabel";
            // 
            // UserNameTextBox
            // 
            resources.ApplyResources(this.UserNameTextBox, "UserNameTextBox");
            this.UserNameTextBox.Name = "UserNameTextBox";
            this.UserNameTextBox.TextChanged += new System.EventHandler(this.V3Block_Changed);
            // 
            // AuthenticationPasswordLabel
            // 
            resources.ApplyResources(this.AuthenticationPasswordLabel, "AuthenticationPasswordLabel");
            this.AuthenticationPasswordLabel.Name = "AuthenticationPasswordLabel";
            // 
            // AuthenticationPasswordLabelTextBox
            // 
            resources.ApplyResources(this.AuthenticationPasswordLabelTextBox, "AuthenticationPasswordLabelTextBox");
            this.AuthenticationPasswordLabelTextBox.Name = "AuthenticationPasswordLabelTextBox";
            this.AuthenticationPasswordLabelTextBox.TextChanged += new System.EventHandler(this.EncryptTextBox_TextChanged);
            // 
            // AuthenticationProtocolLabel
            // 
            resources.ApplyResources(this.AuthenticationProtocolLabel, "AuthenticationProtocolLabel");
            this.AuthenticationProtocolLabel.Name = "AuthenticationProtocolLabel";
            // 
            // AuthenticationProtocolComboBox
            // 
            resources.ApplyResources(this.AuthenticationProtocolComboBox, "AuthenticationProtocolComboBox");
            this.AuthenticationProtocolComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AuthenticationProtocolComboBox.FormattingEnabled = true;
            this.AuthenticationProtocolComboBox.Items.AddRange(new object[] {
            resources.GetString("AuthenticationProtocolComboBox.Items"),
            resources.GetString("AuthenticationProtocolComboBox.Items1")});
            this.AuthenticationProtocolComboBox.Name = "AuthenticationProtocolComboBox";
            this.AuthenticationProtocolComboBox.SelectedIndex = 0;
            this.AuthenticationProtocolComboBox.SelectedIndexChanged += new System.EventHandler(this.V3Block_Changed);
            // 
            // PrivacyPasswordLabel
            // 
            resources.ApplyResources(this.PrivacyPasswordLabel, "PrivacyPasswordLabel");
            this.PrivacyPasswordLabel.Name = "PrivacyPasswordLabel";
            // 
            // PrivacyPasswordTextBox
            // 
            resources.ApplyResources(this.PrivacyPasswordTextBox, "PrivacyPasswordTextBox");
            this.PrivacyPasswordTextBox.Name = "PrivacyPasswordTextBox";
            this.PrivacyPasswordTextBox.TextChanged += new System.EventHandler(this.EncryptTextBox_TextChanged);
            // 
            // PrivacyProtocolLabel
            // 
            resources.ApplyResources(this.PrivacyProtocolLabel, "PrivacyProtocolLabel");
            this.PrivacyProtocolLabel.Name = "PrivacyProtocolLabel";
            // 
            // PrivacyProtocolComboBox
            // 
            resources.ApplyResources(this.PrivacyProtocolComboBox, "PrivacyProtocolComboBox");
            this.PrivacyProtocolComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PrivacyProtocolComboBox.FormattingEnabled = true;
            this.PrivacyProtocolComboBox.Items.AddRange(new object[] {
            resources.GetString("PrivacyProtocolComboBox.Items"),
            resources.GetString("PrivacyProtocolComboBox.Items1")});
            this.PrivacyProtocolComboBox.Name = "PrivacyProtocolComboBox";
            this.PrivacyProtocolComboBox.SelectedIndex = 0;
            this.PrivacyProtocolComboBox.SelectedIndexChanged += new System.EventHandler(this.V3Block_Changed);
            // 
            // RetrieveSnmpPanel
            // 
            resources.ApplyResources(this.RetrieveSnmpPanel, "RetrieveSnmpPanel");
            this.RetrieveSnmpPanel.Controls.Add(this.RetrieveSnmpPicture, 0, 0);
            this.RetrieveSnmpPanel.Controls.Add(this.RetrieveSnmpLabel, 1, 0);
            this.RetrieveSnmpPanel.Name = "RetrieveSnmpPanel";
            // 
            // RetrieveSnmpPicture
            // 
            resources.ApplyResources(this.RetrieveSnmpPicture, "RetrieveSnmpPicture");
            this.RetrieveSnmpPicture.Name = "RetrieveSnmpPicture";
            this.RetrieveSnmpPicture.TabStop = false;
            // 
            // RetrieveSnmpLabel
            // 
            resources.ApplyResources(this.RetrieveSnmpLabel, "RetrieveSnmpLabel");
            this.RetrieveSnmpLabel.Name = "RetrieveSnmpLabel";
            // 
            // GeneralConfigureGroupBox
            // 
            resources.ApplyResources(this.GeneralConfigureGroupBox, "GeneralConfigureGroupBox");
            this.GeneralConfigureGroupBox.Controls.Add(this.GeneralConfigTableLayoutPanel);
            this.GeneralConfigureGroupBox.Name = "GeneralConfigureGroupBox";
            this.GeneralConfigureGroupBox.TabStop = false;
            // 
            // GeneralConfigTableLayoutPanel
            // 
            resources.ApplyResources(this.GeneralConfigTableLayoutPanel, "GeneralConfigTableLayoutPanel");
            this.GeneralConfigTableLayoutPanel.Name = "GeneralConfigTableLayoutPanel";
            // 
            // SnmpEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.SnmpTableLayoutPanel);
            this.Name = "SnmpEditPage";
            this.SnmpTableLayoutPanel.ResumeLayout(false);
            this.SnmpTableLayoutPanel.PerformLayout();
            this.SnmpServiceStatusPanel.ResumeLayout(false);
            this.SnmpServiceStatusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ServiceStatusPicture)).EndInit();
            this.SnmpV2cGroupBox.ResumeLayout(false);
            this.SnmpV2cGroupBox.PerformLayout();
            this.SnmpV2cPanel.ResumeLayout(false);
            this.SnmpV2cPanel.PerformLayout();
            this.SnmpV3GroupBox.ResumeLayout(false);
            this.SnmpV3GroupBox.PerformLayout();
            this.SnmpV3Panel.ResumeLayout(false);
            this.SnmpV3Panel.PerformLayout();
            this.RetrieveSnmpPanel.ResumeLayout(false);
            this.RetrieveSnmpPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RetrieveSnmpPicture)).EndInit();
            this.GeneralConfigureGroupBox.ResumeLayout(false);
            this.GeneralConfigureGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private TableLayoutPanel SnmpTableLayoutPanel;

        private Label DescLabel;

        private GroupBox GeneralConfigureGroupBox;
        private TableLayoutPanel GeneralConfigTableLayoutPanel;

        private CheckBox EnableSnmpCheckBox;
        private CheckBox DebugLogCheckBox;
        private GroupBox SnmpV2cGroupBox;
        private TableLayoutPanel SnmpV2cPanel;
        private GroupBox SnmpV3GroupBox;
        private TableLayoutPanel SnmpV3Panel;
        private CheckBox SupportV3CheckBox;
        private TextBox PrivacyPasswordTextBox;
        private TextBox UserNameTextBox;
        private Label PrivacyPasswordLabel;
        private Label UserNameLabel;
        private TextBox AuthenticationPasswordLabelTextBox;
        private Label AuthenticationPasswordLabel;
        private Label AuthenticationProtocolLabel;
        private ComboBox AuthenticationProtocolComboBox;
        private TableLayoutPanel SnmpServiceStatusPanel;
        private PictureBox ServiceStatusPicture;
        private Label ServiceStatusLabel;
        private CheckBox SupportV2cCheckBox;
        private Label CommunityLabel;
        private TextBox CommunityTextBox;
        private Label PrivacyProtocolLabel;
        private ComboBox PrivacyProtocolComboBox;
        private TableLayoutPanel RetrieveSnmpPanel;
        private PictureBox RetrieveSnmpPicture;
        private Label RetrieveSnmpLabel;
    }
}