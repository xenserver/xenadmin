namespace XenAdmin.Dialogs
{
    partial class DateFilterDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DateFilterDialog));
            this.ComboStartDate = new System.Windows.Forms.ComboBox();
            this.LabelEndDate = new System.Windows.Forms.Label();
            this.DatePickerEndTime = new System.Windows.Forms.DateTimePicker();
            this.DatePickerStartDate = new System.Windows.Forms.DateTimePicker();
            this.LabelStartDate = new System.Windows.Forms.Label();
            this.DatePickerStartTime = new System.Windows.Forms.DateTimePicker();
            this.DatePickerEndDate = new System.Windows.Forms.DateTimePicker();
            this.ComboEndDate = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonApplyFilter = new System.Windows.Forms.Button();
            this.tableLayoutPanelWarning = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ComboStartDate
            // 
            this.ComboStartDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.ComboStartDate, "ComboStartDate");
            this.ComboStartDate.FormattingEnabled = true;
            this.ComboStartDate.Items.AddRange(new object[] {
            resources.GetString("ComboStartDate.Items"),
            resources.GetString("ComboStartDate.Items1"),
            resources.GetString("ComboStartDate.Items2"),
            resources.GetString("ComboStartDate.Items3"),
            resources.GetString("ComboStartDate.Items4"),
            resources.GetString("ComboStartDate.Items5")});
            this.ComboStartDate.Name = "ComboStartDate";
            this.ComboStartDate.SelectedIndexChanged += new System.EventHandler(this.startDateCombo_SelectedIndexChanged);
            // 
            // LabelEndDate
            // 
            resources.ApplyResources(this.LabelEndDate, "LabelEndDate");
            this.LabelEndDate.Name = "LabelEndDate";
            // 
            // DatePickerEndTime
            // 
            resources.ApplyResources(this.DatePickerEndTime, "DatePickerEndTime");
            this.DatePickerEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.DatePickerEndTime.Name = "DatePickerEndTime";
            this.DatePickerEndTime.ShowUpDown = true;
            this.DatePickerEndTime.ValueChanged += new System.EventHandler(this.DatePickersChanged);
            // 
            // DatePickerStartDate
            // 
            resources.ApplyResources(this.DatePickerStartDate, "DatePickerStartDate");
            this.DatePickerStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DatePickerStartDate.Name = "DatePickerStartDate";
            this.DatePickerStartDate.ValueChanged += new System.EventHandler(this.DatePickersChanged);
            // 
            // LabelStartDate
            // 
            resources.ApplyResources(this.LabelStartDate, "LabelStartDate");
            this.LabelStartDate.Name = "LabelStartDate";
            // 
            // DatePickerStartTime
            // 
            resources.ApplyResources(this.DatePickerStartTime, "DatePickerStartTime");
            this.DatePickerStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.DatePickerStartTime.Name = "DatePickerStartTime";
            this.DatePickerStartTime.ShowUpDown = true;
            this.DatePickerStartTime.ValueChanged += new System.EventHandler(this.DatePickersChanged);
            // 
            // DatePickerEndDate
            // 
            resources.ApplyResources(this.DatePickerEndDate, "DatePickerEndDate");
            this.DatePickerEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DatePickerEndDate.Name = "DatePickerEndDate";
            this.DatePickerEndDate.ValueChanged += new System.EventHandler(this.DatePickersChanged);
            // 
            // ComboEndDate
            // 
            this.ComboEndDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.ComboEndDate, "ComboEndDate");
            this.ComboEndDate.FormattingEnabled = true;
            this.ComboEndDate.Items.AddRange(new object[] {
            resources.GetString("ComboEndDate.Items"),
            resources.GetString("ComboEndDate.Items1"),
            resources.GetString("ComboEndDate.Items2"),
            resources.GetString("ComboEndDate.Items3"),
            resources.GetString("ComboEndDate.Items4"),
            resources.GetString("ComboEndDate.Items5")});
            this.ComboEndDate.Name = "ComboEndDate";
            this.ComboEndDate.SelectedIndexChanged += new System.EventHandler(this.endDateCombo_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonApplyFilter
            // 
            resources.ApplyResources(this.buttonApplyFilter, "buttonApplyFilter");
            this.buttonApplyFilter.Name = "buttonApplyFilter";
            this.buttonApplyFilter.UseVisualStyleBackColor = true;
            this.buttonApplyFilter.Click += new System.EventHandler(this.buttonApplyFilter_Click);
            // 
            // tableLayoutPanelWarning
            // 
            resources.ApplyResources(this.tableLayoutPanelWarning, "tableLayoutPanelWarning");
            this.tableLayoutPanelWarning.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanelWarning.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanelWarning.Name = "tableLayoutPanelWarning";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Name = "label2";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // DateFilterDialog
            // 
            this.AcceptButton = this.buttonApplyFilter;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanelWarning);
            this.Controls.Add(this.buttonApplyFilter);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.ComboStartDate);
            this.Controls.Add(this.LabelEndDate);
            this.Controls.Add(this.DatePickerEndTime);
            this.Controls.Add(this.DatePickerStartDate);
            this.Controls.Add(this.LabelStartDate);
            this.Controls.Add(this.DatePickerStartTime);
            this.Controls.Add(this.DatePickerEndDate);
            this.Controls.Add(this.ComboEndDate);
            this.HelpButton = false;
            this.Name = "DateFilterDialog";
            this.Shown += new System.EventHandler(this.RefreshPickersAndCombo);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelWarning.ResumeLayout(false);
            this.tableLayoutPanelWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ComboStartDate;
        private System.Windows.Forms.Label LabelEndDate;
        private System.Windows.Forms.DateTimePicker DatePickerEndTime;
        private System.Windows.Forms.DateTimePicker DatePickerStartDate;
        private System.Windows.Forms.Label LabelStartDate;
        private System.Windows.Forms.DateTimePicker DatePickerStartTime;
        private System.Windows.Forms.DateTimePicker DatePickerEndDate;
        private System.Windows.Forms.ComboBox ComboEndDate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonApplyFilter;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelWarning;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}