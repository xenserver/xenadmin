using XenAdmin.Controls;
namespace XenAdmin.Wizards.NewPolicyWizard
{
    partial class NewPolicySnapshotFrequencyPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewPolicySnapshotFrequencyPage));
            this.radioButtonHourly = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.dateTimePickerDaily = new XenAdmin.Controls.DateTimeMinutes15();
            this.comboBoxMin = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.daysWeekCheckboxes = new XenAdmin.Wizards.NewPolicyWizard.DaysWeekCheckboxes();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePickerWeekly = new XenAdmin.Controls.DateTimeMinutes15();
            this.radioButtonWeekly = new System.Windows.Forms.RadioButton();
            this.radioButtonDaily = new System.Windows.Forms.RadioButton();
            this.numericUpDownRetention = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.localServerTime1 = new XenAdmin.Wizards.NewPolicyWizard.LocalServerTime();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panelRecurrence = new System.Windows.Forms.Panel();
            this.panelWeekly = new System.Windows.Forms.TableLayoutPanel();
            this.panelHourly = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.sectionLabelSchedule = new XenAdmin.Controls.SectionHeaderLabel();
            this.sectionLabelNumber = new XenAdmin.Controls.SectionHeaderLabel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.autoHeightLabel2 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.panelDaily = new System.Windows.Forms.TableLayoutPanel();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRetention)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panelRecurrence.SuspendLayout();
            this.panelWeekly.SuspendLayout();
            this.panelHourly.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panelDaily.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonHourly
            // 
            resources.ApplyResources(this.radioButtonHourly, "radioButtonHourly");
            this.radioButtonHourly.Name = "radioButtonHourly";
            this.radioButtonHourly.UseVisualStyleBackColor = true;
            this.radioButtonHourly.CheckedChanged += new System.EventHandler(this.radioButtonHourly_CheckedChanged);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // dateTimePickerDaily
            // 
            resources.ApplyResources(this.dateTimePickerDaily, "dateTimePickerDaily");
            this.dateTimePickerDaily.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerDaily.Name = "dateTimePickerDaily";
            this.dateTimePickerDaily.ShowUpDown = true;
            this.dateTimePickerDaily.Value = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            // 
            // comboBoxMin
            // 
            this.comboBoxMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMin.FormattingEnabled = true;
            this.comboBoxMin.Items.AddRange(new object[] {
            resources.GetString("comboBoxMin.Items"),
            resources.GetString("comboBoxMin.Items1"),
            resources.GetString("comboBoxMin.Items2"),
            resources.GetString("comboBoxMin.Items3")});
            resources.ApplyResources(this.comboBoxMin, "comboBoxMin");
            this.comboBoxMin.Name = "comboBoxMin";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // daysWeekCheckboxes
            // 
            resources.ApplyResources(this.daysWeekCheckboxes, "daysWeekCheckboxes");
            this.panelWeekly.SetColumnSpan(this.daysWeekCheckboxes, 2);
            this.daysWeekCheckboxes.Days = "";
            this.daysWeekCheckboxes.MaximumSize = new System.Drawing.Size(400, 80);
            this.daysWeekCheckboxes.Name = "daysWeekCheckboxes";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.panelWeekly.SetColumnSpan(this.label5, 2);
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // dateTimePickerWeekly
            // 
            resources.ApplyResources(this.dateTimePickerWeekly, "dateTimePickerWeekly");
            this.dateTimePickerWeekly.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerWeekly.Name = "dateTimePickerWeekly";
            this.dateTimePickerWeekly.ShowUpDown = true;
            this.dateTimePickerWeekly.Value = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            // 
            // radioButtonWeekly
            // 
            resources.ApplyResources(this.radioButtonWeekly, "radioButtonWeekly");
            this.radioButtonWeekly.Name = "radioButtonWeekly";
            this.radioButtonWeekly.UseVisualStyleBackColor = true;
            this.radioButtonWeekly.CheckedChanged += new System.EventHandler(this.radioButtonWeekly_CheckedChanged);
            // 
            // radioButtonDaily
            // 
            resources.ApplyResources(this.radioButtonDaily, "radioButtonDaily");
            this.radioButtonDaily.Name = "radioButtonDaily";
            this.radioButtonDaily.UseVisualStyleBackColor = true;
            this.radioButtonDaily.CheckedChanged += new System.EventHandler(this.radioButtonDaily_CheckedChanged);
            // 
            // numericUpDownRetention
            // 
            resources.ApplyResources(this.numericUpDownRetention, "numericUpDownRetention");
            this.numericUpDownRetention.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownRetention.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRetention.Name = "numericUpDownRetention";
            this.numericUpDownRetention.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // localServerTime1
            // 
            resources.ApplyResources(this.localServerTime1, "localServerTime1");
            this.localServerTime1.Name = "localServerTime1";
            this.localServerTime1.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelRecurrence, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.radioButtonHourly, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.radioButtonDaily, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.radioButtonWeekly, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.label9, 1, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // panelRecurrence
            // 
            this.panelRecurrence.Controls.Add(this.panelWeekly);
            this.panelRecurrence.Controls.Add(this.panelHourly);
            this.panelRecurrence.Controls.Add(this.panelDaily);
            resources.ApplyResources(this.panelRecurrence, "panelRecurrence");
            this.panelRecurrence.Name = "panelRecurrence";
            // 
            // panelWeekly
            // 
            resources.ApplyResources(this.panelWeekly, "panelWeekly");
            this.panelWeekly.Controls.Add(this.label4, 0, 0);
            this.panelWeekly.Controls.Add(this.dateTimePickerWeekly, 1, 0);
            this.panelWeekly.Controls.Add(this.label5, 0, 1);
            this.panelWeekly.Controls.Add(this.daysWeekCheckboxes, 0, 2);
            this.panelWeekly.Name = "panelWeekly";
            // 
            // panelHourly
            // 
            resources.ApplyResources(this.panelHourly, "panelHourly");
            this.panelHourly.Controls.Add(this.label6, 0, 0);
            this.panelHourly.Controls.Add(this.comboBoxMin, 1, 0);
            this.panelHourly.Controls.Add(this.label7, 2, 0);
            this.panelHourly.MinimumSize = new System.Drawing.Size(100, 40);
            this.panelHourly.Name = "panelHourly";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.autoHeightLabel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.localServerTime1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.sectionLabelSchedule, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.sectionLabelNumber, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 5);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // sectionLabelSchedule
            // 
            resources.ApplyResources(this.sectionLabelSchedule, "sectionLabelSchedule");
            this.sectionLabelSchedule.FocusControl = null;
            this.sectionLabelSchedule.LineColor = System.Drawing.SystemColors.Window;
            this.sectionLabelSchedule.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionLabelSchedule.MinimumSize = new System.Drawing.Size(0, 14);
            this.sectionLabelSchedule.Name = "sectionLabelSchedule";
            this.sectionLabelSchedule.TabStop = false;
            // 
            // sectionLabelNumber
            // 
            resources.ApplyResources(this.sectionLabelNumber, "sectionLabelNumber");
            this.sectionLabelNumber.FocusControl = null;
            this.sectionLabelNumber.LineColor = System.Drawing.SystemColors.Window;
            this.sectionLabelNumber.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionLabelNumber.MinimumSize = new System.Drawing.Size(0, 14);
            this.sectionLabelNumber.Name = "sectionLabelNumber";
            this.sectionLabelNumber.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.autoHeightLabel2, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.numericUpDownRetention, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // autoHeightLabel2
            // 
            resources.ApplyResources(this.autoHeightLabel2, "autoHeightLabel2");
            this.tableLayoutPanel3.SetColumnSpan(this.autoHeightLabel2, 2);
            this.autoHeightLabel2.Name = "autoHeightLabel2";
            // 
            // panelDaily
            // 
            resources.ApplyResources(this.panelDaily, "panelDaily");
            this.panelDaily.Controls.Add(this.dateTimePickerDaily, 1, 0);
            this.panelDaily.Controls.Add(this.label8, 0, 0);
            this.panelDaily.MinimumSize = new System.Drawing.Size(10, 40);
            this.panelDaily.Name = "panelDaily";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Name = "label9";
            this.tableLayoutPanel4.SetRowSpan(this.label9, 3);
            // 
            // NewPolicySnapshotFrequencyPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "NewPolicySnapshotFrequencyPage";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRetention)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.panelRecurrence.ResumeLayout(false);
            this.panelWeekly.ResumeLayout(false);
            this.panelWeekly.PerformLayout();
            this.panelHourly.ResumeLayout(false);
            this.panelHourly.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panelDaily.ResumeLayout(false);
            this.panelDaily.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonHourly;
        private System.Windows.Forms.RadioButton radioButtonWeekly;
        private System.Windows.Forms.RadioButton radioButtonDaily;
        private System.Windows.Forms.NumericUpDown numericUpDownRetention;
        private System.Windows.Forms.Label label2;
        private DateTimeMinutes15 dateTimePickerDaily;
        private DateTimeMinutes15 dateTimePickerWeekly;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxMin;
        private DaysWeekCheckboxes daysWeekCheckboxes;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panelRecurrence;
        private LocalServerTime localServerTime1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
        private SectionHeaderLabel sectionLabelSchedule;
        private SectionHeaderLabel sectionLabelNumber;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel panelWeekly;
        private System.Windows.Forms.TableLayoutPanel panelHourly;
        private System.Windows.Forms.TableLayoutPanel panelDaily;
        private System.Windows.Forms.Label label9;
    }
}
