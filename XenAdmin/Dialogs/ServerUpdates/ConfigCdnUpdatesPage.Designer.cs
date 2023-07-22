namespace XenAdmin.Dialogs.ServerUpdates
{
    partial class ConfigCdnUpdatesPage
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigCdnUpdatesPage));
            this.tableLayoutPanelConfig = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxProxy = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxProxyUrl = new System.Windows.Forms.TextBox();
            this.labelProxyUrl = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.textBoxProxyUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxProxyPassword = new System.Windows.Forms.TextBox();
            this.groupBoxSchedule = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxPeriodicSync = new System.Windows.Forms.CheckBox();
            this.radioButtonDaily = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonWeekly = new System.Windows.Forms.RadioButton();
            this.comboBoxWeekday = new System.Windows.Forms.ComboBox();
            this.groupBoxRepo = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.labelRepo = new System.Windows.Forms.Label();
            this.labelTopBlurb = new System.Windows.Forms.Label();
            this.comboBoxRepo = new System.Windows.Forms.ComboBox();
            this.labelPool = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonDiscard = new System.Windows.Forms.Button();
            this.labelNoConnections = new System.Windows.Forms.Label();
            this.dataGridViewExPools = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanelConfig.SuspendLayout();
            this.groupBoxProxy.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBoxSchedule.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBoxRepo.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewExPools)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanelConfig
            // 
            resources.ApplyResources(this.tableLayoutPanelConfig, "tableLayoutPanelConfig");
            this.tableLayoutPanelConfig.Controls.Add(this.groupBoxProxy, 0, 3);
            this.tableLayoutPanelConfig.Controls.Add(this.groupBoxSchedule, 0, 2);
            this.tableLayoutPanelConfig.Controls.Add(this.groupBoxRepo, 0, 1);
            this.tableLayoutPanelConfig.Controls.Add(this.labelPool, 0, 0);
            this.tableLayoutPanelConfig.Name = "tableLayoutPanelConfig";
            this.tableLayoutPanel5.SetRowSpan(this.tableLayoutPanelConfig, 2);
            // 
            // groupBoxProxy
            // 
            this.groupBoxProxy.Controls.Add(this.tableLayoutPanel2);
            resources.ApplyResources(this.groupBoxProxy, "groupBoxProxy");
            this.groupBoxProxy.Name = "groupBoxProxy";
            this.groupBoxProxy.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.textBoxProxyUrl, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelProxyUrl, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelUsername, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBoxProxyUsername, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelPassword, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.textBoxProxyPassword, 1, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // textBoxProxyUrl
            // 
            resources.ApplyResources(this.textBoxProxyUrl, "textBoxProxyUrl");
            this.textBoxProxyUrl.Name = "textBoxProxyUrl";
            // 
            // labelProxyUrl
            // 
            resources.ApplyResources(this.labelProxyUrl, "labelProxyUrl");
            this.labelProxyUrl.Name = "labelProxyUrl";
            // 
            // labelUsername
            // 
            resources.ApplyResources(this.labelUsername, "labelUsername");
            this.labelUsername.Name = "labelUsername";
            // 
            // textBoxProxyUsername
            // 
            resources.ApplyResources(this.textBoxProxyUsername, "textBoxProxyUsername");
            this.textBoxProxyUsername.Name = "textBoxProxyUsername";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.tableLayoutPanel2.SetColumnSpan(this.label2, 2);
            this.label2.Name = "label2";
            // 
            // labelPassword
            // 
            resources.ApplyResources(this.labelPassword, "labelPassword");
            this.labelPassword.Name = "labelPassword";
            // 
            // textBoxProxyPassword
            // 
            resources.ApplyResources(this.textBoxProxyPassword, "textBoxProxyPassword");
            this.textBoxProxyPassword.Name = "textBoxProxyPassword";
            this.textBoxProxyPassword.UseSystemPasswordChar = true;
            // 
            // groupBoxSchedule
            // 
            this.groupBoxSchedule.Controls.Add(this.tableLayoutPanel3);
            resources.ApplyResources(this.groupBoxSchedule, "groupBoxSchedule");
            this.groupBoxSchedule.Name = "groupBoxSchedule";
            this.groupBoxSchedule.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.checkBoxPeriodicSync, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.radioButtonDaily, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.radioButtonWeekly, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.comboBoxWeekday, 1, 2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // checkBoxPeriodicSync
            // 
            resources.ApplyResources(this.checkBoxPeriodicSync, "checkBoxPeriodicSync");
            this.tableLayoutPanel3.SetColumnSpan(this.checkBoxPeriodicSync, 2);
            this.checkBoxPeriodicSync.Name = "checkBoxPeriodicSync";
            this.checkBoxPeriodicSync.UseVisualStyleBackColor = true;
            // 
            // radioButtonDaily
            // 
            resources.ApplyResources(this.radioButtonDaily, "radioButtonDaily");
            this.radioButtonDaily.Name = "radioButtonDaily";
            this.radioButtonDaily.TabStop = true;
            this.radioButtonDaily.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel3.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // radioButtonWeekly
            // 
            resources.ApplyResources(this.radioButtonWeekly, "radioButtonWeekly");
            this.radioButtonWeekly.Name = "radioButtonWeekly";
            this.radioButtonWeekly.TabStop = true;
            this.radioButtonWeekly.UseVisualStyleBackColor = true;
            // 
            // comboBoxWeekday
            // 
            resources.ApplyResources(this.comboBoxWeekday, "comboBoxWeekday");
            this.comboBoxWeekday.FormattingEnabled = true;
            this.comboBoxWeekday.Items.AddRange(new object[] {
            resources.GetString("comboBoxWeekday.Items"),
            resources.GetString("comboBoxWeekday.Items1"),
            resources.GetString("comboBoxWeekday.Items2"),
            resources.GetString("comboBoxWeekday.Items3"),
            resources.GetString("comboBoxWeekday.Items4"),
            resources.GetString("comboBoxWeekday.Items5"),
            resources.GetString("comboBoxWeekday.Items6")});
            this.comboBoxWeekday.Name = "comboBoxWeekday";
            // 
            // groupBoxRepo
            // 
            this.groupBoxRepo.Controls.Add(this.tableLayoutPanel4);
            resources.ApplyResources(this.groupBoxRepo, "groupBoxRepo");
            this.groupBoxRepo.Name = "groupBoxRepo";
            this.groupBoxRepo.TabStop = false;
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.labelRepo, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.labelTopBlurb, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.comboBoxRepo, 1, 1);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // labelRepo
            // 
            resources.ApplyResources(this.labelRepo, "labelRepo");
            this.labelRepo.Name = "labelRepo";
            // 
            // labelTopBlurb
            // 
            resources.ApplyResources(this.labelTopBlurb, "labelTopBlurb");
            this.tableLayoutPanel4.SetColumnSpan(this.labelTopBlurb, 2);
            this.labelTopBlurb.Name = "labelTopBlurb";
            // 
            // comboBoxRepo
            // 
            resources.ApplyResources(this.comboBoxRepo, "comboBoxRepo");
            this.comboBoxRepo.FormattingEnabled = true;
            this.comboBoxRepo.Name = "comboBoxRepo";
            // 
            // labelPool
            // 
            resources.ApplyResources(this.labelPool, "labelPool");
            this.labelPool.Name = "labelPool";
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Error;
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.dataGridViewExPools, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanelConfig, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonApply);
            this.flowLayoutPanel1.Controls.Add(this.buttonDiscard);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // buttonApply
            // 
            resources.ApplyResources(this.buttonApply, "buttonApply");
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonDiscard
            // 
            resources.ApplyResources(this.buttonDiscard, "buttonDiscard");
            this.buttonDiscard.Name = "buttonDiscard";
            this.buttonDiscard.UseVisualStyleBackColor = true;
            this.buttonDiscard.Click += new System.EventHandler(this.buttonDiscard_Click);
            // 
            // labelNoConnections
            // 
            this.labelNoConnections.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.labelNoConnections, "labelNoConnections");
            this.labelNoConnections.Name = "labelNoConnections";
            // 
            // dataGridViewExPools
            // 
            this.dataGridViewExPools.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewExPools.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewExPools.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewExPools.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnCheck,
            this.ColumnName});
            resources.ApplyResources(this.dataGridViewExPools, "dataGridViewExPools");
            this.dataGridViewExPools.Name = "dataGridViewExPools";
            this.dataGridViewExPools.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewExPools_CellContentClick);
            this.dataGridViewExPools.SelectionChanged += new System.EventHandler(this.dataGridViewExPools_SelectionChanged);
            // 
            // ColumnCheck
            // 
            this.ColumnCheck.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnCheck, "ColumnCheck");
            this.ColumnCheck.Name = "ColumnCheck";
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnName, "ColumnName");
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ConfigCdnUpdatesPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel5);
            this.Controls.Add(this.labelNoConnections);
            this.Name = "ConfigCdnUpdatesPage";
            this.tableLayoutPanelConfig.ResumeLayout(false);
            this.tableLayoutPanelConfig.PerformLayout();
            this.groupBoxProxy.ResumeLayout(false);
            this.groupBoxProxy.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBoxSchedule.ResumeLayout(false);
            this.groupBoxSchedule.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupBoxRepo.ResumeLayout(false);
            this.groupBoxRepo.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewExPools)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelConfig;
        private System.Windows.Forms.Label labelTopBlurb;
        private System.Windows.Forms.Label labelRepo;
        private System.Windows.Forms.ComboBox comboBoxRepo;
        private System.Windows.Forms.Label labelProxyUrl;
        private System.Windows.Forms.TextBox textBoxProxyUrl;
        private System.Windows.Forms.GroupBox groupBoxProxy;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxProxyUsername;
        private System.Windows.Forms.TextBox textBoxProxyPassword;
        private System.Windows.Forms.GroupBox groupBoxSchedule;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.CheckBox checkBoxPeriodicSync;
        private System.Windows.Forms.RadioButton radioButtonDaily;
        private System.Windows.Forms.RadioButton radioButtonWeekly;
        private System.Windows.Forms.ComboBox comboBoxWeekday;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBoxRepo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private Controls.DataGridViewEx.DataGridViewEx dataGridViewExPools;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonDiscard;
        private System.Windows.Forms.Label labelPool;
        private System.Windows.Forms.Label labelNoConnections;
    }
}
