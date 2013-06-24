namespace XenAdmin.Dialogs
{
    partial class ResolvingSubjectsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResolvingSubjectsDialog));
            this.tableLayoutPanelAddUsers = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxNames = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.buttonGrantAccess = new System.Windows.Forms.Button();
            this.entryListView = new System.Windows.Forms.ListView();
            this.ColumnName = new System.Windows.Forms.ColumnHeader();
            this.ColumnResolveStatus = new System.Windows.Forms.ColumnHeader();
            this.ColumnGrantAccess = new System.Windows.Forms.ColumnHeader();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.LabelStatus = new System.Windows.Forms.Label();
            this.labelTopBlurb = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxUserNames = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanelAddUsers.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelAddUsers
            // 
            resources.ApplyResources(this.tableLayoutPanelAddUsers, "tableLayoutPanelAddUsers");
            this.tableLayoutPanelAddUsers.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanelAddUsers.Name = "tableLayoutPanelAddUsers";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanelAddUsers.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // textBoxNames
            // 
            resources.ApplyResources(this.textBoxNames, "textBoxNames");
            this.textBoxNames.Name = "textBoxNames";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.ButtonCancel);
            this.flowLayoutPanel1.Controls.Add(this.buttonGrantAccess);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.ButtonCancel, "ButtonCancel");
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonGrantAccess
            // 
            resources.ApplyResources(this.buttonGrantAccess, "buttonGrantAccess");
            this.buttonGrantAccess.Name = "buttonGrantAccess";
            this.buttonGrantAccess.UseVisualStyleBackColor = true;
            this.buttonGrantAccess.Click += new System.EventHandler(this.buttonGrantAccess_Click);
            // 
            // entryListView
            // 
            this.entryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnName,
            this.ColumnResolveStatus,
            this.ColumnGrantAccess});
            this.tableLayoutPanel1.SetColumnSpan(this.entryListView, 2);
            resources.ApplyResources(this.entryListView, "entryListView");
            this.entryListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.entryListView.Name = "entryListView";
            this.entryListView.UseCompatibleStateImageBehavior = false;
            this.entryListView.View = System.Windows.Forms.View.Details;
            // 
            // ColumnName
            // 
            resources.ApplyResources(this.ColumnName, "ColumnName");
            // 
            // ColumnResolveStatus
            // 
            resources.ApplyResources(this.ColumnResolveStatus, "ColumnResolveStatus");
            // 
            // ColumnGrantAccess
            // 
            resources.ApplyResources(this.ColumnGrantAccess, "ColumnGrantAccess");
            // 
            // progressBar1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.progressBar1, 2);
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // LabelStatus
            // 
            resources.ApplyResources(this.LabelStatus, "LabelStatus");
            this.tableLayoutPanel1.SetColumnSpan(this.LabelStatus, 2);
            this.LabelStatus.Name = "LabelStatus";
            // 
            // labelTopBlurb
            // 
            resources.ApplyResources(this.labelTopBlurb, "labelTopBlurb");
            this.tableLayoutPanel1.SetColumnSpan(this.labelTopBlurb, 2);
            this.labelTopBlurb.MaximumSize = new System.Drawing.Size(631, 80);
            this.labelTopBlurb.Name = "labelTopBlurb";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.textBoxUserNames, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelTopBlurb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LabelStatus, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.entryListView, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // textBoxUserNames
            // 
            resources.ApplyResources(this.textBoxUserNames, "textBoxUserNames");
            this.textBoxUserNames.Name = "textBoxUserNames";
            this.textBoxUserNames.TextChanged += new System.EventHandler(this.textBoxUserNames_TextChanged);
            this.textBoxUserNames.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxUserNames_KeyUp);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // ResolvingSubjectsDialog
            // 
            this.AcceptButton = this.buttonGrantAccess;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.ButtonCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ResolvingSubjectsDialog";
            this.tableLayoutPanelAddUsers.ResumeLayout(false);
            this.tableLayoutPanelAddUsers.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelAddUsers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxNames;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ListView entryListView;
        private System.Windows.Forms.ColumnHeader ColumnName;
        private System.Windows.Forms.ColumnHeader ColumnResolveStatus;
        private System.Windows.Forms.ColumnHeader ColumnGrantAccess;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LabelStatus;
        private System.Windows.Forms.Label labelTopBlurb;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBoxUserNames;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonGrantAccess;
        private System.Windows.Forms.Button ButtonCancel;
    }
}