using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Wizards.NewPolicyWizard;
using XenAPI;

namespace XenAdmin.Dialogs.VMProtection_Recovery
{
    partial class VMProtectionPoliciesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VMProtectionPoliciesDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.NameColum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EnabledColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnVMs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriptionColum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnNextArchive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLastResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNew = new System.Windows.Forms.Button();
            this.buttonEnable = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonRunNow = new System.Windows.Forms.Button();
            this.labelPolicyTitle = new System.Windows.Forms.Label();
            this.buttonProperties = new System.Windows.Forms.Button();
            this.policyHistory1 = new XenAdmin.Dialogs.VMProtectionRecovery.PolicyHistory();
            this.label2 = new System.Windows.Forms.Label();
            this.localServerTime1 = new XenAdmin.Wizards.NewPolicyWizard.LocalServerTime();
            this.chevronButton1 = new XenAdmin.Controls.ChevronButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.deprecationBanner = new XenAdmin.Controls.DeprecationBanner();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColum,
            this.EnabledColumn,
            this.ColumnVMs,
            this.DescriptionColum,
            this.ColumnNextArchive,
            this.ColumnLastResult});
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.MultiSelect = true;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.tableLayoutPanel3.SetRowSpan(this.dataGridView1, 6);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // NameColum
            // 
            this.NameColum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.NameColum, "NameColum");
            this.NameColum.Name = "NameColum";
            this.NameColum.ReadOnly = true;
            // 
            // EnabledColumn
            // 
            this.EnabledColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.EnabledColumn, "EnabledColumn");
            this.EnabledColumn.Name = "EnabledColumn";
            this.EnabledColumn.ReadOnly = true;
            this.EnabledColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnVMs
            // 
            this.ColumnVMs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnVMs.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.ColumnVMs, "ColumnVMs");
            this.ColumnVMs.Name = "ColumnVMs";
            this.ColumnVMs.ReadOnly = true;
            // 
            // DescriptionColum
            // 
            this.DescriptionColum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.DescriptionColum, "DescriptionColum");
            this.DescriptionColum.Name = "DescriptionColum";
            this.DescriptionColum.ReadOnly = true;
            // 
            // ColumnNextArchive
            // 
            this.ColumnNextArchive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnNextArchive, "ColumnNextArchive");
            this.ColumnNextArchive.Name = "ColumnNextArchive";
            this.ColumnNextArchive.ReadOnly = true;
            // 
            // ColumnLastResult
            // 
            this.ColumnLastResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnLastResult, "ColumnLastResult");
            this.ColumnLastResult.Name = "ColumnLastResult";
            this.ColumnLastResult.ReadOnly = true;
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonNew
            // 
            resources.ApplyResources(this.buttonNew, "buttonNew");
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.UseVisualStyleBackColor = true;
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // buttonEnable
            // 
            resources.ApplyResources(this.buttonEnable, "buttonEnable");
            this.buttonEnable.Name = "buttonEnable";
            this.buttonEnable.UseVisualStyleBackColor = true;
            this.buttonEnable.Click += new System.EventHandler(this.buttonEnable_Click);
            // 
            // buttonDelete
            // 
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonRunNow
            // 
            resources.ApplyResources(this.buttonRunNow, "buttonRunNow");
            this.buttonRunNow.Name = "buttonRunNow";
            this.buttonRunNow.UseVisualStyleBackColor = true;
            this.buttonRunNow.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelPolicyTitle
            // 
            resources.ApplyResources(this.labelPolicyTitle, "labelPolicyTitle");
            this.labelPolicyTitle.AutoEllipsis = true;
            this.labelPolicyTitle.Name = "labelPolicyTitle";
            // 
            // buttonProperties
            // 
            resources.ApplyResources(this.buttonProperties, "buttonProperties");
            this.buttonProperties.Name = "buttonProperties";
            this.buttonProperties.UseVisualStyleBackColor = true;
            this.buttonProperties.Click += new System.EventHandler(this.buttonProperties_Click);
            // 
            // policyHistory1
            // 
            resources.ApplyResources(this.policyHistory1, "policyHistory1");
            this.policyHistory1.Name = "policyHistory1";
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
            // 
            // chevronButton1
            // 
            resources.ApplyResources(this.chevronButton1, "chevronButton1");
            this.chevronButton1.Cursor = System.Windows.Forms.Cursors.Default;
            this.chevronButton1.Image = global::XenAdmin.Properties.Resources.PDChevronDown;
            this.chevronButton1.Name = "chevronButton1";
            this.chevronButton1.ButtonClick += new System.EventHandler(this.chevronButton1_ButtonClick);
            this.chevronButton1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chevronButton1_KeyDown);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.deprecationBanner, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // deprecationBanner
            // 
            resources.ApplyResources(this.deprecationBanner, "deprecationBanner");
            this.deprecationBanner.BackColor = System.Drawing.Color.LightCoral;
            this.deprecationBanner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deprecationBanner.Name = "deprecationBanner";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.buttonProperties, 1, 5);
            this.tableLayoutPanel3.Controls.Add(this.labelPolicyTitle, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.dataGridView1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.localServerTime1, 0, 7);
            this.tableLayoutPanel3.Controls.Add(this.buttonEnable, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.buttonRunNow, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.buttonDelete, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.buttonNew, 1, 1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.chevronButton1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.buttonCancel, 1, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.policyHistory1, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel4, 0, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // VMProtectionPoliciesDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanel5);
            this.Name = "VMProtectionPoliciesDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.VMProtectionPoliciesDialog_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.VMProtectionPoliciesDialog_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected DataGridViewEx dataGridView1;
        protected System.Windows.Forms.Button buttonCancel;
        protected System.Windows.Forms.Button buttonNew;
        protected System.Windows.Forms.Button buttonEnable;
        protected System.Windows.Forms.Button buttonDelete;
        protected System.Windows.Forms.Button buttonRunNow;
        protected System.Windows.Forms.Label labelPolicyTitle;
        protected System.Windows.Forms.Button buttonProperties;
        protected XenAdmin.Dialogs.VMProtectionRecovery.PolicyHistory policyHistory1;
        protected System.Windows.Forms.Label label2;
        protected LocalServerTime localServerTime1;
        protected XenAdmin.Controls.ChevronButton chevronButton1;
        protected System.Windows.Forms.DataGridViewTextBoxColumn NameColum;
        protected System.Windows.Forms.DataGridViewTextBoxColumn EnabledColumn;
        protected System.Windows.Forms.DataGridViewTextBoxColumn ColumnVMs;
        protected System.Windows.Forms.DataGridViewTextBoxColumn DescriptionColum;
        protected System.Windows.Forms.DataGridViewTextBoxColumn ColumnNextArchive;
        protected System.Windows.Forms.DataGridViewTextBoxColumn ColumnLastResult;
        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        protected XenAdmin.Controls.DeprecationBanner deprecationBanner;
    }
}