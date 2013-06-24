using XenAdmin.CustomFields;
using XenAdmin.XenSearch;
using System;
using XenAPI;
using XenAdmin.Core;
namespace XenAdmin.Dialogs
{
    partial class CustomFieldsDialog
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
            CustomFieldsManager.CustomFieldsChanged -= CustomFields_CustomFieldsChanged;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomFieldsDialog));
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.CustomFieldsLabel = new System.Windows.Forms.Label();
            this.lbCustomFields = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // CustomFieldsLabel
            // 
            resources.ApplyResources(this.CustomFieldsLabel, "CustomFieldsLabel");
            this.CustomFieldsLabel.BackColor = System.Drawing.Color.Transparent;
            this.CustomFieldsLabel.Name = "CustomFieldsLabel";
            // 
            // lbCustomFields
            // 
            resources.ApplyResources(this.lbCustomFields, "lbCustomFields");
            this.lbCustomFields.Name = "lbCustomFields";
            this.lbCustomFields.SelectedIndexChanged += new System.EventHandler(this.lbCustomFields_SelectedIndexChanged);
            // 
            // CustomFieldsDialog
            // 
            this.AcceptButton = this.btnClose;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnClose;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.CustomFieldsLabel);
            this.Controls.Add(this.lbCustomFields);
            this.Name = "CustomFieldsDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAdd;
        internal System.Windows.Forms.Label CustomFieldsLabel;
        internal System.Windows.Forms.ListBox lbCustomFields;
    }
}