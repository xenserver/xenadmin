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
            this.tableLayoutPanelButtons = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonDisabled = new System.Windows.Forms.RadioButton();
            this.radioButtonWakeonLAN = new System.Windows.Forms.RadioButton();
            this.radioButtonILO = new System.Windows.Forms.RadioButton();
            this.radioButtonDRAC = new System.Windows.Forms.RadioButton();
            this.radioButtonCustom = new System.Windows.Forms.RadioButton();
            this.textBoxCustom = new System.Windows.Forms.TextBox();
            this.groupBoxCredentials = new System.Windows.Forms.GroupBox();
            this.dataGridViewCustom = new System.Windows.Forms.DataGridView();
            this.ColumnKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanelCredentials = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxInterface = new System.Windows.Forms.TextBox();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.labelServer = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelPool = new System.Windows.Forms.Label();
            this.tableLayoutPanelHosts = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSelect = new System.Windows.Forms.Button();
            this.dataGridViewHosts = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnHost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnPowerOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.spinnerIcon1 = new XenAdmin.Controls.SpinnerIcon();
            this.groupBoxMode.SuspendLayout();
            this.tableLayoutPanelButtons.SuspendLayout();
            this.groupBoxCredentials.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCustom)).BeginInit();
            this.tableLayoutPanelCredentials.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelHosts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHosts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIcon1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxMode
            // 
            resources.ApplyResources(this.groupBoxMode, "groupBoxMode");
            this.groupBoxMode.Controls.Add(this.tableLayoutPanelButtons);
            this.groupBoxMode.Name = "groupBoxMode";
            this.groupBoxMode.TabStop = false;
            // 
            // tableLayoutPanelButtons
            // 
            resources.ApplyResources(this.tableLayoutPanelButtons, "tableLayoutPanelButtons");
            this.tableLayoutPanelButtons.Controls.Add(this.radioButtonDisabled, 0, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.radioButtonWakeonLAN, 0, 1);
            this.tableLayoutPanelButtons.Controls.Add(this.radioButtonILO, 0, 2);
            this.tableLayoutPanelButtons.Controls.Add(this.radioButtonDRAC, 0, 3);
            this.tableLayoutPanelButtons.Controls.Add(this.radioButtonCustom, 0, 4);
            this.tableLayoutPanelButtons.Controls.Add(this.textBoxCustom, 1, 4);
            this.tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            // 
            // radioButtonDisabled
            // 
            resources.ApplyResources(this.radioButtonDisabled, "radioButtonDisabled");
            this.radioButtonDisabled.Name = "radioButtonDisabled";
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
            this.textBoxCustom.TextChanged += new System.EventHandler(this.textBoxCustom_TextChanged);
            // 
            // groupBoxCredentials
            // 
            resources.ApplyResources(this.groupBoxCredentials, "groupBoxCredentials");
            this.groupBoxCredentials.Controls.Add(this.dataGridViewCustom);
            this.groupBoxCredentials.Controls.Add(this.tableLayoutPanelCredentials);
            this.groupBoxCredentials.Name = "groupBoxCredentials";
            this.groupBoxCredentials.TabStop = false;
            // 
            // dataGridViewCustom
            // 
            this.dataGridViewCustom.AllowUserToResizeRows = false;
            this.dataGridViewCustom.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewCustom.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewCustom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridViewCustom.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewCustom.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnKey,
            this.ColumnValue});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewCustom.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.dataGridViewCustom, "dataGridViewCustom");
            this.dataGridViewCustom.MultiSelect = false;
            this.dataGridViewCustom.Name = "dataGridViewCustom";
            this.dataGridViewCustom.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewCustom.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewCustom_CellEndEdit);
            this.dataGridViewCustom.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridViewCustom_CurrentCellDirtyStateChanged);
            this.dataGridViewCustom.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridViewCustom_RowsAdded);
            this.dataGridViewCustom.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridViewCustom_RowsRemoved);
            this.dataGridViewCustom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewCustom_KeyDown);
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
            this.textBoxInterface.TextChanged += new System.EventHandler(this.textBoxInterface_TextChanged);
            // 
            // textBoxUser
            // 
            resources.ApplyResources(this.textBoxUser, "textBoxUser");
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.TextChanged += new System.EventHandler(this.textBoxUser_TextChanged);
            // 
            // textBoxPassword
            // 
            resources.ApplyResources(this.textBoxPassword, "textBoxPassword");
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
            // 
            // labelServer
            // 
            resources.ApplyResources(this.labelServer, "labelServer");
            this.labelServer.Name = "labelServer";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelServer, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelPool, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelHosts, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxMode, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxCredentials, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelPool
            // 
            resources.ApplyResources(this.labelPool, "labelPool");
            this.labelPool.Name = "labelPool";
            // 
            // tableLayoutPanelHosts
            // 
            resources.ApplyResources(this.tableLayoutPanelHosts, "tableLayoutPanelHosts");
            this.tableLayoutPanelHosts.Controls.Add(this.buttonSelect, 1, 0);
            this.tableLayoutPanelHosts.Controls.Add(this.dataGridViewHosts, 0, 0);
            this.tableLayoutPanelHosts.Name = "tableLayoutPanelHosts";
            // 
            // buttonSelect
            // 
            resources.ApplyResources(this.buttonSelect, "buttonSelect");
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.UseVisualStyleBackColor = true;
            this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
            // 
            // dataGridViewHosts
            // 
            this.dataGridViewHosts.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewHosts.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewHosts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewHosts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnHost,
            this.columnPowerOn});
            resources.ApplyResources(this.dataGridViewHosts, "dataGridViewHosts");
            this.dataGridViewHosts.MultiSelect = true;
            this.dataGridViewHosts.Name = "dataGridViewHosts";
            this.dataGridViewHosts.SelectionChanged += new System.EventHandler(this.dataGridViewHosts_SelectionChanged);
            // 
            // columnHost
            // 
            resources.ApplyResources(this.columnHost, "columnHost");
            this.columnHost.Name = "columnHost";
            // 
            // columnPowerOn
            // 
            resources.ApplyResources(this.columnPowerOn, "columnPowerOn");
            this.columnPowerOn.Name = "columnPowerOn";
            // 
            // bgWorker
            // 
            this.bgWorker.WorkerSupportsCancellation = true;
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
            // 
            // spinnerIcon1
            // 
            resources.ApplyResources(this.spinnerIcon1, "spinnerIcon1");
            this.spinnerIcon1.Name = "spinnerIcon1";
            this.spinnerIcon1.TabStop = false;
            // 
            // HostPowerONEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.spinnerIcon1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "HostPowerONEditPage";
            this.groupBoxMode.ResumeLayout(false);
            this.groupBoxMode.PerformLayout();
            this.tableLayoutPanelButtons.ResumeLayout(false);
            this.tableLayoutPanelButtons.PerformLayout();
            this.groupBoxCredentials.ResumeLayout(false);
            this.groupBoxCredentials.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCustom)).EndInit();
            this.tableLayoutPanelCredentials.ResumeLayout(false);
            this.tableLayoutPanelCredentials.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelHosts.ResumeLayout(false);
            this.tableLayoutPanelHosts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHosts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIcon1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonWakeonLAN;
        private System.Windows.Forms.RadioButton radioButtonILO;
        private System.Windows.Forms.RadioButton radioButtonDisabled;
        private System.Windows.Forms.TextBox textBoxCustom;
        private System.Windows.Forms.RadioButton radioButtonCustom;
        private System.Windows.Forms.RadioButton radioButtonDRAC;
        private System.Windows.Forms.GroupBox groupBoxCredentials;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCredentials;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxInterface;
        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnValue;
        private System.Windows.Forms.DataGridView dataGridViewCustom;
        private System.Windows.Forms.GroupBox groupBoxMode;
        private System.Windows.Forms.Label labelServer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelButtons;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Controls.SpinnerIcon spinnerIcon1;
        private System.ComponentModel.BackgroundWorker bgWorker;
        private System.Windows.Forms.Label labelPool;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelHosts;
        private System.Windows.Forms.Button buttonSelect;
        private Controls.DataGridViewEx.DataGridViewEx dataGridViewHosts;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnHost;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnPowerOn;
    }
}
