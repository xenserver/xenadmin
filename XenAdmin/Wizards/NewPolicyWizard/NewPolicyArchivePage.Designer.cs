namespace XenAdmin.Wizards.NewPolicyWizard
{
    partial class NewPolicyArchivePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewPolicyArchivePage));
            this.radioButtonArchiveWeekly = new System.Windows.Forms.RadioButton();
            this.radioButtonArchiveDaily = new System.Windows.Forms.RadioButton();
            this.radioButtonArchiveASAP = new System.Windows.Forms.RadioButton();
            this.radioButtonDoNotArchive = new System.Windows.Forms.RadioButton();
            this.m_tlpDestination = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.pictureBoxTest = new System.Windows.Forms.PictureBox();
            this.checkBoxCredentials = new System.Windows.Forms.CheckBox();
            this.m_tlpCredentials = new System.Windows.Forms.TableLayoutPanel();
            this.labelUser = new System.Windows.Forms.Label();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.m_labelError = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.toolTipContainer1 = new XenAdmin.Controls.ToolTipContainer();
            this.labelDivider = new System.Windows.Forms.Label();
            this.labelRecurDaily = new System.Windows.Forms.Label();
            this.dateTimePickerDaily = new XenAdmin.Controls.DateTimeMinutes15();
            this.daysWeekCheckboxes1 = new XenAdmin.Wizards.NewPolicyWizard.DaysWeekCheckboxes();
            this.labelDays = new System.Windows.Forms.Label();
            this.labelRecurWeekly = new System.Windows.Forms.Label();
            this.dateTimePickerWeekly = new XenAdmin.Controls.DateTimeMinutes15();
            this.localServerTime1 = new XenAdmin.Wizards.NewPolicyWizard.LocalServerTime();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.sectionLabelSchedule = new XenAdmin.Controls.SectionHeaderLabel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.m_tlpRecur = new System.Windows.Forms.TableLayoutPanel();
            this.sectionLabelDest = new XenAdmin.Controls.SectionHeaderLabel();
            this.m_tlpDestination.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTest)).BeginInit();
            this.m_tlpCredentials.SuspendLayout();
            this.toolTipContainer1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.m_tlpRecur.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonArchiveWeekly
            // 
            resources.ApplyResources(this.radioButtonArchiveWeekly, "radioButtonArchiveWeekly");
            this.radioButtonArchiveWeekly.Name = "radioButtonArchiveWeekly";
            this.radioButtonArchiveWeekly.UseVisualStyleBackColor = true;
            this.radioButtonArchiveWeekly.CheckedChanged += new System.EventHandler(this.radioButtonArchiveWeekly_CheckedChanged);
            // 
            // radioButtonArchiveDaily
            // 
            resources.ApplyResources(this.radioButtonArchiveDaily, "radioButtonArchiveDaily");
            this.radioButtonArchiveDaily.Name = "radioButtonArchiveDaily";
            this.radioButtonArchiveDaily.TabStop = true;
            this.radioButtonArchiveDaily.UseVisualStyleBackColor = true;
            this.radioButtonArchiveDaily.CheckedChanged += new System.EventHandler(this.radioButtonArchiveDaily_CheckedChanged);
            // 
            // radioButtonArchiveASAP
            // 
            resources.ApplyResources(this.radioButtonArchiveASAP, "radioButtonArchiveASAP");
            this.radioButtonArchiveASAP.Name = "radioButtonArchiveASAP";
            this.radioButtonArchiveASAP.UseVisualStyleBackColor = true;
            this.radioButtonArchiveASAP.CheckedChanged += new System.EventHandler(this.radioButtonArchiveASAP_CheckedChanged);
            // 
            // radioButtonDoNotArchive
            // 
            resources.ApplyResources(this.radioButtonDoNotArchive, "radioButtonDoNotArchive");
            this.radioButtonDoNotArchive.Name = "radioButtonDoNotArchive";
            this.radioButtonDoNotArchive.UseVisualStyleBackColor = true;
            this.radioButtonDoNotArchive.CheckedChanged += new System.EventHandler(this.radioButtonDoNotArchive_CheckedChanged);
            // 
            // m_tlpDestination
            // 
            resources.ApplyResources(this.m_tlpDestination, "m_tlpDestination");
            this.m_tlpDestination.Controls.Add(this.label2, 0, 0);
            this.m_tlpDestination.Controls.Add(this.textBoxPath, 1, 0);
            this.m_tlpDestination.Controls.Add(this.buttonTest, 2, 0);
            this.m_tlpDestination.Controls.Add(this.pictureBoxTest, 3, 0);
            this.m_tlpDestination.Controls.Add(this.checkBoxCredentials, 0, 2);
            this.m_tlpDestination.Controls.Add(this.m_tlpCredentials, 0, 3);
            this.m_tlpDestination.Controls.Add(this.m_labelError, 1, 1);
            this.m_tlpDestination.Name = "m_tlpDestination";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxPath
            // 
            resources.ApplyResources(this.textBoxPath, "textBoxPath");
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.TextChanged += new System.EventHandler(this.textBoxPath_TextChanged);
            // 
            // buttonTest
            // 
            resources.ApplyResources(this.buttonTest, "buttonTest");
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // pictureBoxTest
            // 
            resources.ApplyResources(this.pictureBoxTest, "pictureBoxTest");
            this.pictureBoxTest.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureBoxTest.Image = global::XenAdmin.Properties.Resources.ajax_loader;
            this.pictureBoxTest.Name = "pictureBoxTest";
            this.pictureBoxTest.TabStop = false;
            // 
            // checkBoxCredentials
            // 
            resources.ApplyResources(this.checkBoxCredentials, "checkBoxCredentials");
            this.m_tlpDestination.SetColumnSpan(this.checkBoxCredentials, 4);
            this.checkBoxCredentials.Name = "checkBoxCredentials";
            this.checkBoxCredentials.UseVisualStyleBackColor = true;
            this.checkBoxCredentials.CheckedChanged += new System.EventHandler(this.checkBoxCredentials_CheckedChanged);
            // 
            // m_tlpCredentials
            // 
            resources.ApplyResources(this.m_tlpCredentials, "m_tlpCredentials");
            this.m_tlpDestination.SetColumnSpan(this.m_tlpCredentials, 2);
            this.m_tlpCredentials.Controls.Add(this.labelUser, 1, 0);
            this.m_tlpCredentials.Controls.Add(this.textBoxUser, 2, 0);
            this.m_tlpCredentials.Controls.Add(this.labelPassword, 3, 0);
            this.m_tlpCredentials.Controls.Add(this.textBoxPassword, 4, 0);
            this.m_tlpCredentials.Name = "m_tlpCredentials";
            // 
            // labelUser
            // 
            resources.ApplyResources(this.labelUser, "labelUser");
            this.labelUser.Name = "labelUser";
            // 
            // textBoxUser
            // 
            resources.ApplyResources(this.textBoxUser, "textBoxUser");
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.TextChanged += new System.EventHandler(this.textBoxUser_TextChanged);
            // 
            // labelPassword
            // 
            resources.ApplyResources(this.labelPassword, "labelPassword");
            this.labelPassword.Name = "labelPassword";
            // 
            // textBoxPassword
            // 
            resources.ApplyResources(this.textBoxPassword, "textBoxPassword");
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
            // 
            // m_labelError
            // 
            resources.ApplyResources(this.m_labelError, "m_labelError");
            this.m_labelError.ForeColor = System.Drawing.Color.Red;
            this.m_labelError.Name = "m_labelError";
            // 
            // toolTipContainer1
            // 
            resources.ApplyResources(this.toolTipContainer1, "toolTipContainer1");
            this.toolTipContainer1.Controls.Add(this.radioButtonArchiveDaily);
            this.toolTipContainer1.Name = "toolTipContainer1";
            this.toolTipContainer1.TabStop = true;
            // 
            // labelDivider
            // 
            this.labelDivider.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.labelDivider, "labelDivider");
            this.labelDivider.Name = "labelDivider";
            this.tableLayoutPanel5.SetRowSpan(this.labelDivider, 4);
            // 
            // labelRecurDaily
            // 
            resources.ApplyResources(this.labelRecurDaily, "labelRecurDaily");
            this.labelRecurDaily.Name = "labelRecurDaily";
            // 
            // dateTimePickerDaily
            // 
            resources.ApplyResources(this.dateTimePickerDaily, "dateTimePickerDaily");
            this.dateTimePickerDaily.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerDaily.Name = "dateTimePickerDaily";
            this.dateTimePickerDaily.ShowUpDown = true;
            this.dateTimePickerDaily.Value = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            // 
            // daysWeekCheckboxes1
            // 
            resources.ApplyResources(this.daysWeekCheckboxes1, "daysWeekCheckboxes1");
            this.m_tlpRecur.SetColumnSpan(this.daysWeekCheckboxes1, 2);
            this.daysWeekCheckboxes1.Days = "";
            this.daysWeekCheckboxes1.Name = "daysWeekCheckboxes1";
            // 
            // labelDays
            // 
            resources.ApplyResources(this.labelDays, "labelDays");
            this.m_tlpRecur.SetColumnSpan(this.labelDays, 2);
            this.labelDays.Name = "labelDays";
            // 
            // labelRecurWeekly
            // 
            resources.ApplyResources(this.labelRecurWeekly, "labelRecurWeekly");
            this.labelRecurWeekly.Name = "labelRecurWeekly";
            // 
            // dateTimePickerWeekly
            // 
            resources.ApplyResources(this.dateTimePickerWeekly, "dateTimePickerWeekly");
            this.dateTimePickerWeekly.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerWeekly.Name = "dateTimePickerWeekly";
            this.dateTimePickerWeekly.ShowUpDown = true;
            this.dateTimePickerWeekly.Value = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            // 
            // localServerTime1
            // 
            resources.ApplyResources(this.localServerTime1, "localServerTime1");
            this.localServerTime1.Name = "localServerTime1";
            this.localServerTime1.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.autoHeightLabel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.sectionLabelSchedule, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.localServerTime1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.sectionLabelDest, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.m_tlpDestination, 0, 5);
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
            this.sectionLabelSchedule.LineColor = System.Drawing.SystemColors.Window;
            this.sectionLabelSchedule.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionLabelSchedule.Name = "sectionLabelSchedule";
            this.sectionLabelSchedule.TabStop = false;
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.m_tlpRecur, 1, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.radioButtonDoNotArchive, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.radioButtonArchiveASAP, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.toolTipContainer1, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.radioButtonArchiveWeekly, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.labelDivider, 1, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // m_tlpRecur
            // 
            resources.ApplyResources(this.m_tlpRecur, "m_tlpRecur");
            this.m_tlpRecur.Controls.Add(this.labelRecurDaily, 0, 0);
            this.m_tlpRecur.Controls.Add(this.dateTimePickerDaily, 1, 0);
            this.m_tlpRecur.Controls.Add(this.labelRecurWeekly, 0, 1);
            this.m_tlpRecur.Controls.Add(this.dateTimePickerWeekly, 1, 1);
            this.m_tlpRecur.Controls.Add(this.labelDays, 0, 2);
            this.m_tlpRecur.Controls.Add(this.daysWeekCheckboxes1, 0, 3);
            this.m_tlpRecur.Name = "m_tlpRecur";
            // 
            // sectionLabelDest
            // 
            resources.ApplyResources(this.sectionLabelDest, "sectionLabelDest");
            this.sectionLabelDest.LineColor = System.Drawing.SystemColors.Window;
            this.sectionLabelDest.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionLabelDest.Name = "sectionLabelDest";
            this.sectionLabelDest.TabStop = false;
            // 
            // NewPolicyArchivePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "NewPolicyArchivePage";
            this.m_tlpDestination.ResumeLayout(false);
            this.m_tlpDestination.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTest)).EndInit();
            this.m_tlpCredentials.ResumeLayout(false);
            this.m_tlpCredentials.PerformLayout();
            this.toolTipContainer1.ResumeLayout(false);
            this.toolTipContainer1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.m_tlpRecur.ResumeLayout(false);
            this.m_tlpRecur.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonArchiveWeekly;
        private System.Windows.Forms.RadioButton radioButtonArchiveDaily;
        private System.Windows.Forms.RadioButton radioButtonArchiveASAP;
        private System.Windows.Forms.RadioButton radioButtonDoNotArchive;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label labelUser;
        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.CheckBox checkBoxCredentials;
        private System.Windows.Forms.Label labelRecurDaily;
        private XenAdmin.Controls.DateTimeMinutes15 dateTimePickerDaily;
        private DaysWeekCheckboxes daysWeekCheckboxes1;
        private System.Windows.Forms.Label labelDays;
        private System.Windows.Forms.Label labelRecurWeekly;
        private XenAdmin.Controls.DateTimeMinutes15 dateTimePickerWeekly;
        private System.Windows.Forms.PictureBox pictureBoxTest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel m_tlpCredentials;
        private System.Windows.Forms.Label labelDivider;
        private LocalServerTime localServerTime1;
        private XenAdmin.Controls.ToolTipContainer toolTipContainer1;
        private System.Windows.Forms.TableLayoutPanel m_tlpDestination;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
        private XenAdmin.Controls.SectionHeaderLabel sectionLabelSchedule;
        private XenAdmin.Controls.SectionHeaderLabel sectionLabelDest;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel m_tlpRecur;
        private XenAdmin.Controls.Common.AutoHeightLabel m_labelError;
        private System.Windows.Forms.TextBox textBoxPath;
    }
}
