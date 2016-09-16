namespace XenAdmin.Dialogs
{
    partial class EnablePvsReadCachingDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnablePvsReadCachingDialog));
            this.enableButton = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.rubricLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pvsSiteList = new System.Windows.Forms.ComboBox();
            this.pvsSiteLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.readonlyCheckboxToolTipContainer = new XenAdmin.Controls.ToolTipContainer();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // enableButton
            // 
            resources.ApplyResources(this.enableButton, "enableButton");
            this.enableButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.enableButton.Name = "enableButton";
            this.enableButton.UseVisualStyleBackColor = true;
            this.enableButton.Click += new System.EventHandler(this.enableButton_Click);
            // 
            // cancel
            // 
            resources.ApplyResources(this.cancel, "cancel");
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Name = "cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.rubricLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // rubricLabel
            // 
            resources.ApplyResources(this.rubricLabel, "rubricLabel");
            this.rubricLabel.Name = "rubricLabel";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.pvsSiteList);
            this.panel1.Controls.Add(this.pvsSiteLabel);
            this.panel1.Name = "panel1";
            // 
            // pvsSiteList
            // 
            this.pvsSiteList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.pvsSiteList, "pvsSiteList");
            this.pvsSiteList.FormattingEnabled = true;
            this.pvsSiteList.Name = "pvsSiteList";
            // 
            // pvsSiteLabel
            // 
            resources.ApplyResources(this.pvsSiteLabel, "pvsSiteLabel");
            this.pvsSiteLabel.Name = "pvsSiteLabel";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.cancel);
            this.flowLayoutPanel1.Controls.Add(this.enableButton);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // readonlyCheckboxToolTipContainer
            // 
            resources.ApplyResources(this.readonlyCheckboxToolTipContainer, "readonlyCheckboxToolTipContainer");
            this.readonlyCheckboxToolTipContainer.Name = "readonlyCheckboxToolTipContainer";
            // 
            // EnablePvsReadCachingDialog
            // 
            this.AcceptButton = this.enableButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.readonlyCheckboxToolTipContainer);
            this.HelpButton = false;
            this.Name = "EnablePvsReadCachingDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.ToolTipContainer readonlyCheckboxToolTipContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label rubricLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox pvsSiteList;
        private System.Windows.Forms.Label pvsSiteLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button enableButton;
    }
}