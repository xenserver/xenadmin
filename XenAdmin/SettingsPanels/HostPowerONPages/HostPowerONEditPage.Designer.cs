using XenAdmin.Controls.DataGridViewEx;
using System.Windows.Forms;
namespace XenAdmin.SettingsPanels
{
    partial class HostPowerONEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HostPowerONEditPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBoxMode = new System.Windows.Forms.GroupBox();
            this.radioButtonDisabled = new System.Windows.Forms.RadioButton();
            this.radioButtonWakeonLAN = new System.Windows.Forms.RadioButton();
            this.radioButtonILO = new System.Windows.Forms.RadioButton();
            this.radioButtonDRAC = new System.Windows.Forms.RadioButton();
            this.radioButtonCustom = new System.Windows.Forms.RadioButton();
            this.textBoxCustom = new System.Windows.Forms.TextBox();
            this.groupBoxCredentials = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelCredentials = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxInterface = new System.Windows.Forms.TextBox();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new XenAdmin.SettingsPanels.HostPowerONEditPage.DataGridViewKey();
            this.ColumnKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBoxMode.SuspendLayout();
            this.groupBoxCredentials.SuspendLayout();
            this.tableLayoutPanelCredentials.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxMode
            // 
            this.groupBoxMode.Controls.Add(this.radioButtonDisabled);
            this.groupBoxMode.Controls.Add(this.radioButtonWakeonLAN);
            this.groupBoxMode.Controls.Add(this.radioButtonILO);
            this.groupBoxMode.Controls.Add(this.radioButtonDRAC);
            this.groupBoxMode.Controls.Add(this.radioButtonCustom);
            this.groupBoxMode.Controls.Add(this.textBoxCustom);
            resources.ApplyResources(this.groupBoxMode, "groupBoxMode");
            this.groupBoxMode.Name = "groupBoxMode";
            this.groupBoxMode.TabStop = false;
            // 
            // radioButtonDisabled
            // 
            resources.ApplyResources(this.radioButtonDisabled, "radioButtonDisabled");
            this.radioButtonDisabled.Checked = true;
            this.radioButtonDisabled.Name = "radioButtonDisabled";
            this.radioButtonDisabled.TabStop = true;
            this.radioButtonDisabled.UseVisualStyleBackColor = true;
            this.radioButtonDisabled.CheckedChanged += new System.EventHandler(this.radioButtonDisabled_CheckedChanged);
            // 
            // radioButtonWakeonLAN
            // 
            resources.ApplyResources(this.radioButtonWakeonLAN, "radioButtonWakeonLAN");
            this.radioButtonWakeonLAN.Name = "radioButtonWakeonLAN";
            this.radioButtonWakeonLAN.UseVisualStyleBackColor = true;
            this.radioButtonWakeonLAN.CheckedChanged += new System.EventHandler(this.radioButtonWakeonLAN_CheckedChanged);
            // 
            // radioButtonILO
            // 
            resources.ApplyResources(this.radioButtonILO, "radioButtonILO");
            this.radioButtonILO.Name = "radioButtonILO";
            this.radioButtonILO.UseVisualStyleBackColor = true;
            this.radioButtonILO.CheckedChanged += new System.EventHandler(this.radioButtonILO_CheckedChanged);
            // 
            // radioButtonDRAC
            // 
            resources.ApplyResources(this.radioButtonDRAC, "radioButtonDRAC");
            this.radioButtonDRAC.Name = "radioButtonDRAC";
            this.radioButtonDRAC.UseVisualStyleBackColor = true;
            this.radioButtonDRAC.CheckedChanged += new System.EventHandler(this.radioButtonDRAC_CheckedChanged);
            // 
            // radioButtonCustom
            // 
            resources.ApplyResources(this.radioButtonCustom, "radioButtonCustom");
            this.radioButtonCustom.Name = "radioButtonCustom";
            this.radioButtonCustom.UseVisualStyleBackColor = true;
            this.radioButtonCustom.CheckedChanged += new System.EventHandler(this.radioButtonCustom_CheckedChanged);
            // 
            // textBoxCustom
            // 
            resources.ApplyResources(this.textBoxCustom, "textBoxCustom");
            this.textBoxCustom.Name = "textBoxCustom";
            this.textBoxCustom.Click += new System.EventHandler(this.textBoxCustom_Click);
            // 
            // groupBoxCredentials
            // 
            this.groupBoxCredentials.Controls.Add(this.tableLayoutPanelCredentials);
            resources.ApplyResources(this.groupBoxCredentials, "groupBoxCredentials");
            this.groupBoxCredentials.Name = "groupBoxCredentials";
            this.groupBoxCredentials.TabStop = false;
            // 
            // tableLayoutPanelCredentials
            // 
            resources.ApplyResources(this.tableLayoutPanelCredentials, "tableLayoutPanelCredentials");
            this.tableLayoutPanelCredentials.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanelCredentials.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanelCredentials.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanelCredentials.Controls.Add(this.textBoxInterface, 1, 0);
            this.tableLayoutPanelCredentials.Controls.Add(this.textBoxUser, 1, 1);
            this.tableLayoutPanelCredentials.Controls.Add(this.textBoxPassword, 1, 2);
            this.tableLayoutPanelCredentials.Name = "tableLayoutPanelCredentials";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // textBoxInterface
            // 
            resources.ApplyResources(this.textBoxInterface, "textBoxInterface");
            this.textBoxInterface.Name = "textBoxInterface";
            // 
            // textBoxUser
            // 
            resources.ApplyResources(this.textBoxUser, "textBoxUser");
            this.textBoxUser.Name = "textBoxUser";
            // 
            // textBoxPassword
            // 
            resources.ApplyResources(this.textBoxPassword, "textBoxPassword");
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnKey,
            this.ColumnValue});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            // 
            // ColumnKey
            // 
            resources.ApplyResources(this.ColumnKey, "ColumnKey");
            this.ColumnKey.Name = "ColumnKey";
            // 
            // ColumnValue
            // 
            resources.ApplyResources(this.ColumnValue, "ColumnValue");
            this.ColumnValue.Name = "ColumnValue";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // HostPowerONEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBoxMode);
            this.Controls.Add(this.groupBoxCredentials);
            this.Name = "HostPowerONEditPage";
            this.groupBoxMode.ResumeLayout(false);
            this.groupBoxMode.PerformLayout();
            this.groupBoxCredentials.ResumeLayout(false);
            this.tableLayoutPanelCredentials.ResumeLayout(false);
            this.tableLayoutPanelCredentials.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.RadioButton radioButtonWakeonLAN;
        protected System.Windows.Forms.RadioButton radioButtonILO;
        protected System.Windows.Forms.RadioButton radioButtonDisabled;
        protected System.Windows.Forms.TextBox textBoxCustom;
        protected System.Windows.Forms.RadioButton radioButtonCustom;
        protected System.Windows.Forms.RadioButton radioButtonDRAC;
        protected System.Windows.Forms.GroupBox groupBoxCredentials;
        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanelCredentials;
        protected System.Windows.Forms.Label label1;
        protected System.Windows.Forms.Label label2;
        protected System.Windows.Forms.Label label3;
        protected System.Windows.Forms.TextBox textBoxInterface;
        protected System.Windows.Forms.TextBox textBoxUser;
        protected System.Windows.Forms.TextBox textBoxPassword;
        protected System.Windows.Forms.DataGridViewTextBoxColumn ColumnKey;
        protected System.Windows.Forms.DataGridViewTextBoxColumn ColumnValue;
        protected DataGridViewKey dataGridView1;
        protected GroupBox groupBoxMode;
        protected Label label4;
    }
}
