using XenAdmin.Controls;

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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pvsSiteList = new XenAdmin.Controls.LongStringComboBox();
            this.rubricLabel = new System.Windows.Forms.Label();
            this.pvsSiteLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cancel = new System.Windows.Forms.Button();
            this.enableButton = new System.Windows.Forms.Button();
            this.readonlyCheckboxToolTipContainer = new XenAdmin.Controls.ToolTipContainer();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.pvsSiteList, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.rubricLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pvsSiteLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // pvsSiteList
            // 
            this.pvsSiteList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.pvsSiteList, "pvsSiteList");
            this.pvsSiteList.FormattingEnabled = true;
            this.pvsSiteList.Name = "pvsSiteList";
            // 
            // rubricLabel
            // 
            resources.ApplyResources(this.rubricLabel, "rubricLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.rubricLabel, 2);
            this.rubricLabel.Name = "rubricLabel";
            // 
            // pvsSiteLabel
            // 
            resources.ApplyResources(this.pvsSiteLabel, "pvsSiteLabel");
            this.pvsSiteLabel.Name = "pvsSiteLabel";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.cancel);
            this.flowLayoutPanel1.Controls.Add(this.enableButton);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // cancel
            // 
            resources.ApplyResources(this.cancel, "cancel");
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Name = "cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // enableButton
            // 
            this.enableButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            resources.ApplyResources(this.enableButton, "enableButton");
            this.enableButton.Name = "enableButton";
            this.enableButton.UseVisualStyleBackColor = true;
            this.enableButton.Click += new System.EventHandler(this.enableButton_Click);
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
            this.Name = "EnablePvsReadCachingDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.ToolTipContainer readonlyCheckboxToolTipContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label rubricLabel;
        private LongStringComboBox pvsSiteList;
        private System.Windows.Forms.Label pvsSiteLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button enableButton;
    }
}