using System;
namespace XenAdmin.Dialogs
{
    partial class ActionProgressDialog
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
            if (action != null)
            {
                action.Completed -= action_Completed;
                action.Changed -= action_Changed;
            }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActionProgressDialog));
            this.labelStatus = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.icon = new System.Windows.Forms.PictureBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelBottom = new System.Windows.Forms.Label();
            this.labelException = new System.Windows.Forms.Label();
            this.labelSubActionStatus = new System.Windows.Forms.Label();
            this.labelTop = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).BeginInit();
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            resources.ApplyResources(this.labelStatus, "labelStatus");
            this.labelStatus.Name = "labelStatus";
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelStatus, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.icon, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonClose, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelBottom, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelException, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelSubActionStatus, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelTop, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // icon
            // 
            resources.ApplyResources(this.icon, "icon");
            this.icon.Name = "icon";
            this.tableLayoutPanel1.SetRowSpan(this.icon, 6);
            this.icon.TabStop = false;
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.VisibleChanged += new System.EventHandler(this.buttonClose_VisibleChanged);
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelBottom
            // 
            resources.ApplyResources(this.labelBottom, "labelBottom");
            this.labelBottom.Name = "labelBottom";
            // 
            // labelException
            // 
            resources.ApplyResources(this.labelException, "labelException");
            this.labelException.Name = "labelException";
            // 
            // labelSubActionStatus
            // 
            resources.ApplyResources(this.labelSubActionStatus, "labelSubActionStatus");
            this.labelSubActionStatus.Name = "labelSubActionStatus";
            // 
            // labelTop
            // 
            resources.ApplyResources(this.labelTop, "labelTop");
            this.labelTop.Name = "labelTop";
            // 
            // ActionProgressDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.HelpButton = false;
            this.MinimizeBox = true;
            this.Name = "ActionProgressDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelTop;
        private System.Windows.Forms.Label labelException;
        private System.Windows.Forms.Label labelBottom;
        private System.Windows.Forms.PictureBox icon;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelSubActionStatus;
    }
}