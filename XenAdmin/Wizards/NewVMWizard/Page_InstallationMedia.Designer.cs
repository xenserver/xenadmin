namespace XenAdmin.Wizards.NewVMWizard
{
    partial class Page_InstallationMedia
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_InstallationMedia));
            this.label1 = new System.Windows.Forms.Label();
            this.CdRadioButton = new System.Windows.Forms.RadioButton();
            this.UrlRadioButton = new System.Windows.Forms.RadioButton();
            this.PvBootBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.PvBootTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CdDropDownBox = new XenAdmin.Controls.ISODropDownBox();
            this.UrlTextBox = new System.Windows.Forms.TextBox();
            this.panelInstallationMethod = new System.Windows.Forms.TableLayoutPanel();
            this.linkLabelAttachNewIsoStore = new System.Windows.Forms.LinkLabel();
            this.comboBoxToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.PvBootBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panelInstallationMethod.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.panelInstallationMethod.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // CdRadioButton
            // 
            resources.ApplyResources(this.CdRadioButton, "CdRadioButton");
            this.CdRadioButton.Name = "CdRadioButton";
            this.CdRadioButton.TabStop = true;
            this.CdRadioButton.UseVisualStyleBackColor = true;
            this.CdRadioButton.CheckedChanged += new System.EventHandler(this.PhysicalRadioButton_CheckedChanged);
            // 
            // UrlRadioButton
            // 
            resources.ApplyResources(this.UrlRadioButton, "UrlRadioButton");
            this.UrlRadioButton.Name = "UrlRadioButton";
            this.UrlRadioButton.TabStop = true;
            this.UrlRadioButton.UseVisualStyleBackColor = true;
            this.UrlRadioButton.CheckedChanged += new System.EventHandler(this.UrlRadioButton_CheckedChanged);
            // 
            // PvBootBox
            // 
            resources.ApplyResources(this.PvBootBox, "PvBootBox");
            this.PvBootBox.Controls.Add(this.tableLayoutPanel2);
            this.PvBootBox.Name = "PvBootBox";
            this.PvBootBox.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.PvBootTextBox, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // PvBootTextBox
            // 
            resources.ApplyResources(this.PvBootTextBox, "PvBootTextBox");
            this.PvBootTextBox.Name = "PvBootTextBox";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // CdDropDownBox
            // 
            resources.ApplyResources(this.CdDropDownBox, "CdDropDownBox");
            this.CdDropDownBox.connection = null;
            this.CdDropDownBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CdDropDownBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CdDropDownBox.Empty = false;
            this.CdDropDownBox.FormattingEnabled = true;
            this.CdDropDownBox.DisplayISO = false;
            this.CdDropDownBox.Name = "CdDropDownBox";
            this.CdDropDownBox.DisplayPhysical = false;
            this.CdDropDownBox.SelectedCD = null;
            // 
            // UrlTextBox
            // 
            resources.ApplyResources(this.UrlTextBox, "UrlTextBox");
            this.UrlTextBox.Name = "UrlTextBox";
            // 
            // panelInstallationMethod
            // 
            resources.ApplyResources(this.panelInstallationMethod, "panelInstallationMethod");
            this.panelInstallationMethod.Controls.Add(this.CdRadioButton, 0, 1);
            this.panelInstallationMethod.Controls.Add(this.UrlRadioButton, 0, 3);
            this.panelInstallationMethod.Controls.Add(this.label1, 0, 0);
            this.panelInstallationMethod.Controls.Add(this.CdDropDownBox, 0, 2);
            this.panelInstallationMethod.Controls.Add(this.UrlTextBox, 0, 4);
            this.panelInstallationMethod.Controls.Add(this.linkLabelAttachNewIsoStore, 1, 2);
            this.panelInstallationMethod.Name = "panelInstallationMethod";
            // 
            // linkLabelAttachNewIsoStore
            // 
            resources.ApplyResources(this.linkLabelAttachNewIsoStore, "linkLabelAttachNewIsoStore");
            this.linkLabelAttachNewIsoStore.Name = "linkLabelAttachNewIsoStore";
            this.linkLabelAttachNewIsoStore.TabStop = true;
            this.linkLabelAttachNewIsoStore.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // Page_InstallationMedia
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panelInstallationMethod);
            this.Controls.Add(this.PvBootBox);
            this.Name = "Page_InstallationMedia";
            this.PvBootBox.ResumeLayout(false);
            this.PvBootBox.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panelInstallationMethod.ResumeLayout(false);
            this.panelInstallationMethod.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton CdRadioButton;
        private System.Windows.Forms.RadioButton UrlRadioButton;
        private System.Windows.Forms.GroupBox PvBootBox;
        private System.Windows.Forms.TextBox PvBootTextBox;
        private System.Windows.Forms.Label label2;
        private XenAdmin.Controls.ISODropDownBox CdDropDownBox;
        private System.Windows.Forms.TextBox UrlTextBox;
        private System.Windows.Forms.TableLayoutPanel panelInstallationMethod;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.LinkLabel linkLabelAttachNewIsoStore;
        private System.Windows.Forms.ToolTip comboBoxToolTip;
    }
}
