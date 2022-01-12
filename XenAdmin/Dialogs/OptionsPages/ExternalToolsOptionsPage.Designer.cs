
namespace XenAdmin.Dialogs.OptionsPages
{
    partial class ExternalToolsOptionsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExternalToolsOptionsPage));
            this.externalToolsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.sshConsoleGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.sshConsoleLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.sshConsoleInfoLabel = new System.Windows.Forms.Label();
            this.radioButtonPutty = new System.Windows.Forms.RadioButton();
            this.radioButtonOpenSsh = new System.Windows.Forms.RadioButton();
            this.buttonBrowseSsh = new System.Windows.Forms.Button();
            this.textBoxOpenSsh = new System.Windows.Forms.TextBox();
            this.textBoxPutty = new System.Windows.Forms.TextBox();
            this.buttonBrowsePutty = new System.Windows.Forms.Button();
            this.tooltipValidation = new System.Windows.Forms.ToolTip(this.components);
            this.externalToolsLayoutPanel.SuspendLayout();
            this.sshConsoleGroupBox.SuspendLayout();
            this.sshConsoleLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // externalToolsLayoutPanel
            // 
            resources.ApplyResources(this.externalToolsLayoutPanel, "externalToolsLayoutPanel");
            this.externalToolsLayoutPanel.Controls.Add(this.sshConsoleGroupBox, 0, 1);
            this.externalToolsLayoutPanel.Name = "externalToolsLayoutPanel";
            // 
            // sshConsoleGroupBox
            // 
            resources.ApplyResources(this.sshConsoleGroupBox, "sshConsoleGroupBox");
            this.sshConsoleGroupBox.Controls.Add(this.sshConsoleLayoutPanel);
            this.sshConsoleGroupBox.Name = "sshConsoleGroupBox";
            this.sshConsoleGroupBox.TabStop = false;
            // 
            // sshConsoleLayoutPanel
            // 
            resources.ApplyResources(this.sshConsoleLayoutPanel, "sshConsoleLayoutPanel");
            this.sshConsoleLayoutPanel.Controls.Add(this.sshConsoleInfoLabel, 0, 0);
            this.sshConsoleLayoutPanel.Controls.Add(this.radioButtonPutty, 0, 1);
            this.sshConsoleLayoutPanel.Controls.Add(this.radioButtonOpenSsh, 0, 3);
            this.sshConsoleLayoutPanel.Controls.Add(this.buttonBrowseSsh, 1, 4);
            this.sshConsoleLayoutPanel.Controls.Add(this.textBoxOpenSsh, 0, 4);
            this.sshConsoleLayoutPanel.Controls.Add(this.textBoxPutty, 0, 2);
            this.sshConsoleLayoutPanel.Controls.Add(this.buttonBrowsePutty, 1, 2);
            this.sshConsoleLayoutPanel.Name = "sshConsoleLayoutPanel";
            // 
            // sshConsoleInfoLabel
            // 
            resources.ApplyResources(this.sshConsoleInfoLabel, "sshConsoleInfoLabel");
            this.sshConsoleLayoutPanel.SetColumnSpan(this.sshConsoleInfoLabel, 2);
            this.sshConsoleInfoLabel.Name = "sshConsoleInfoLabel";
            // 
            // radioButtonPutty
            // 
            resources.ApplyResources(this.radioButtonPutty, "radioButtonPutty");
            this.radioButtonPutty.Name = "radioButtonPutty";
            this.radioButtonPutty.TabStop = true;
            this.radioButtonPutty.UseVisualStyleBackColor = true;
            // 
            // radioButtonOpenSsh
            // 
            resources.ApplyResources(this.radioButtonOpenSsh, "radioButtonOpenSsh");
            this.radioButtonOpenSsh.Name = "radioButtonOpenSsh";
            this.radioButtonOpenSsh.TabStop = true;
            this.radioButtonOpenSsh.UseVisualStyleBackColor = true;
            // 
            // buttonBrowseSsh
            // 
            resources.ApplyResources(this.buttonBrowseSsh, "buttonBrowseSsh");
            this.buttonBrowseSsh.Name = "buttonBrowseSsh";
            this.buttonBrowseSsh.UseVisualStyleBackColor = true;
            this.buttonBrowseSsh.Click += new System.EventHandler(this.buttonBrowseSsh_Click);
            // 
            // textBoxOpenSsh
            // 
            resources.ApplyResources(this.textBoxOpenSsh, "textBoxOpenSsh");
            this.textBoxOpenSsh.Name = "textBoxOpenSsh";
            this.textBoxOpenSsh.TextChanged += new System.EventHandler(this.textBoxOpenSsh_TextChanged);
            // 
            // textBoxPutty
            // 
            resources.ApplyResources(this.textBoxPutty, "textBoxPutty");
            this.textBoxPutty.Name = "textBoxPutty";
            this.textBoxPutty.TextChanged += new System.EventHandler(this.textBoxPutty_TextChanged);
            // 
            // buttonBrowsePutty
            // 
            resources.ApplyResources(this.buttonBrowsePutty, "buttonBrowsePutty");
            this.buttonBrowsePutty.Name = "buttonBrowsePutty";
            this.buttonBrowsePutty.UseVisualStyleBackColor = true;
            this.buttonBrowsePutty.Click += new System.EventHandler(this.buttonBrowsePutty_Click);
            // 
            // tooltipValidation
            // 
            this.tooltipValidation.IsBalloon = true;
            this.tooltipValidation.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            // 
            // ExternalToolsOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.externalToolsLayoutPanel);
            this.Name = "ExternalToolsOptionsPage";
            this.externalToolsLayoutPanel.ResumeLayout(false);
            this.sshConsoleGroupBox.ResumeLayout(false);
            this.sshConsoleLayoutPanel.ResumeLayout(false);
            this.sshConsoleLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel externalToolsLayoutPanel;
        private Controls.DecentGroupBox sshConsoleGroupBox;
        protected System.Windows.Forms.TableLayoutPanel sshConsoleLayoutPanel;
        private System.Windows.Forms.Label sshConsoleInfoLabel;
        private System.Windows.Forms.RadioButton radioButtonPutty;
        private System.Windows.Forms.RadioButton radioButtonOpenSsh;
        private System.Windows.Forms.Button buttonBrowseSsh;
        private System.Windows.Forms.TextBox textBoxOpenSsh;
        private System.Windows.Forms.TextBox textBoxPutty;
        private System.Windows.Forms.Button buttonBrowsePutty;
        private System.Windows.Forms.ToolTip tooltipValidation;
    }
}
