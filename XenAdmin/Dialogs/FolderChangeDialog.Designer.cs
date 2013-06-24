using XenAdmin.Controls;
using System;
namespace XenAdmin.Dialogs
{
    partial class FolderChangeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderChangeDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.treeView = new XenAdmin.Controls.FolderChangeDialogTreeView();
            this.radioButtonNone = new System.Windows.Forms.RadioButton();
            this.radioButtonChoose = new System.Windows.Forms.RadioButton();
            this.newButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // treeView
            // 
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.HideSelection = false;
            this.treeView.Name = "treeView";
            this.treeView.ShowLines = false;
            this.treeView.NodeMouseDoubleClick += new EventHandler<VirtualTreeNodeMouseClickEventArgs>(this.treeView_NodeMouseDoubleClick);
            this.treeView.AfterSelect += new EventHandler<VirtualTreeViewEventArgs>(this.treeView_AfterSelect);
            this.treeView.NodeMouseClick += new EventHandler<VirtualTreeNodeMouseClickEventArgs>(this.treeView_NodeMouseClick);
            // 
            // radioButtonNone
            // 
            resources.ApplyResources(this.radioButtonNone, "radioButtonNone");
            this.radioButtonNone.Name = "radioButtonNone";
            this.radioButtonNone.TabStop = true;
            this.radioButtonNone.UseVisualStyleBackColor = true;
            this.radioButtonNone.Click += new System.EventHandler(this.radioButtonNone_Click);
            // 
            // radioButtonChoose
            // 
            resources.ApplyResources(this.radioButtonChoose, "radioButtonChoose");
            this.radioButtonChoose.Name = "radioButtonChoose";
            this.radioButtonChoose.UseVisualStyleBackColor = true;
            this.radioButtonChoose.Click += new System.EventHandler(this.radioButtonChoose_Click);
            // 
            // newButton
            // 
            resources.ApplyResources(this.newButton, "newButton");
            this.newButton.Name = "newButton";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // FolderChangeDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.newButton);
            this.Controls.Add(this.radioButtonChoose);
            this.Controls.Add(this.radioButtonNone);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "FolderChangeDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private XenAdmin.Controls.FolderChangeDialogTreeView treeView;
        private System.Windows.Forms.RadioButton radioButtonNone;
        private System.Windows.Forms.RadioButton radioButtonChoose;
        private System.Windows.Forms.Button newButton;
    }
}