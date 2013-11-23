namespace XenAdmin.Dialogs.WarningDialogs
{
    partial class CloseXenCenterWarningDialog
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
            if (disposing)
            {
                ConnectionsManager.History.CollectionChanged -= History_CollectionChanged;

                if (components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloseXenCenterWarningDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DontExitButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewActions = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnExpander = new System.Windows.Forms.DataGridViewImageColumn();
            this.columnStatus = new System.Windows.Forms.DataGridViewImageColumn();
            this.columnMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewActions)).BeginInit();
            this.SuspendLayout();
            // 
            // DontExitButton
            // 
            resources.ApplyResources(this.DontExitButton, "DontExitButton");
            this.DontExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.DontExitButton.Name = "DontExitButton";
            this.DontExitButton.UseVisualStyleBackColor = true;
            // 
            // ExitButton
            // 
            resources.ApplyResources(this.ExitButton, "ExitButton");
            this.ExitButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Name = "label2";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.ExitButton, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.DontExitButton, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewActions, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // dataGridViewActions
            // 
            this.dataGridViewActions.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewActions.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewActions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewActions.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewActions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewActions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnExpander,
            this.columnStatus,
            this.columnMessage,
            this.columnLocation,
            this.columnDate});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridViewActions, 2);
            resources.ApplyResources(this.dataGridViewActions, "dataGridViewActions");
            this.dataGridViewActions.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridViewActions.Name = "dataGridViewActions";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewActions.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewActions.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.datagridViewActions_ColumnHeaderMouseClick);
            this.dataGridViewActions.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            // 
            // columnExpander
            // 
            this.columnExpander.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle1.NullValue")));
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.columnExpander.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.columnExpander, "columnExpander");
            this.columnExpander.Name = "columnExpander";
            // 
            // columnStatus
            // 
            this.columnStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle2.NullValue")));
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.columnStatus.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.columnStatus, "columnStatus");
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // columnMessage
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.columnMessage.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.columnMessage, "columnMessage");
            this.columnMessage.Name = "columnMessage";
            // 
            // columnLocation
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.columnLocation.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.columnLocation, "columnLocation");
            this.columnLocation.Name = "columnLocation";
            // 
            // columnDate
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.columnDate.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.columnDate, "columnDate");
            this.columnDate.Name = "columnDate";
            // 
            // CloseXenCenterWarningDialog
            // 
            this.AcceptButton = this.ExitButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.DontExitButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "CloseXenCenterWarningDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewActions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button DontExitButton;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewActions;
        private System.Windows.Forms.DataGridViewImageColumn columnExpander;
        private System.Windows.Forms.DataGridViewImageColumn columnStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDate;
    }
}