﻿namespace XenAdmin.Dialogs
{
    partial class HealthCheckPolicyStatementDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HealthCheckPolicyStatementDialog));
            this.OKButton = new System.Windows.Forms.Button();
            this.policyStatementTextBox = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            resources.ApplyResources(this.OKButton, "OKButton");
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Name = "OKButton";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // policyStatementTextBox
            // 
            this.policyStatementTextBox.AutoEllipsis = true;
            resources.ApplyResources(this.policyStatementTextBox, "policyStatementTextBox");
            this.policyStatementTextBox.Name = "policyStatementTextBox";
            this.policyStatementTextBox.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.policyStatementTextBox_LinkClicked);
            // 
            // HealthCheckPolicyStatementDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.policyStatementTextBox);
            this.Controls.Add(this.OKButton);
            this.HelpButton = false;
            this.Name = "HealthCheckPolicyStatementDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.LinkLabel policyStatementTextBox;
    }
}