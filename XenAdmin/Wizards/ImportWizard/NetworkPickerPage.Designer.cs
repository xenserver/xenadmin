namespace XenAdmin.Wizards.ImportWizard
{
	partial class NetworkPickerPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkPickerPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_srLabel = new System.Windows.Forms.Label();
            this.m_networkGridView = new System.Windows.Forms.DataGridView();
            this.NameNetworkColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MACAddressNetworkColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NetworkNetworkColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.m_buttonAddNetwork = new System.Windows.Forms.Button();
            this.m_buttonDeleteNetwork = new System.Windows.Forms.Button();
            this.m_invalidMacLabel = new XenAdmin.Controls.Common.PasswordFailure();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_networkGridView)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_srLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_networkGridView, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // m_srLabel
            // 
            resources.ApplyResources(this.m_srLabel, "m_srLabel");
            this.m_srLabel.Name = "m_srLabel";
            // 
            // m_networkGridView
            // 
            this.m_networkGridView.AllowUserToAddRows = false;
            this.m_networkGridView.AllowUserToDeleteRows = false;
            this.m_networkGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.m_networkGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.m_networkGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameNetworkColumn,
            this.MACAddressNetworkColumn,
            this.NetworkNetworkColumn});
            this.m_networkGridView.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.m_networkGridView, "m_networkGridView");
            this.m_networkGridView.Name = "m_networkGridView";
            this.m_networkGridView.RowHeadersVisible = false;
            this.m_networkGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_networkGridView.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            this.m_networkGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_networkGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.m_networkGridView_CellBeginEdit);
            this.m_networkGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_networkGridView_CellEndEdit);
            this.m_networkGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_networkGridView_CellClick);
            this.m_networkGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.m_networkGridView_CurrentCellDirtyStateChanged);
            this.m_networkGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.m_networkGridView_DataError);
            this.m_networkGridView.SelectionChanged += new System.EventHandler(this.m_networkGridView_SelectionChanged);
            // 
            // NameNetworkColumn
            // 
            resources.ApplyResources(this.NameNetworkColumn, "NameNetworkColumn");
            this.NameNetworkColumn.Name = "NameNetworkColumn";
            // 
            // MACAddressNetworkColumn
            // 
            resources.ApplyResources(this.MACAddressNetworkColumn, "MACAddressNetworkColumn");
            this.MACAddressNetworkColumn.Name = "MACAddressNetworkColumn";
            // 
            // NetworkNetworkColumn
            // 
            this.NetworkNetworkColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NetworkNetworkColumn.DropDownWidth = 300;
            this.NetworkNetworkColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.NetworkNetworkColumn, "NetworkNetworkColumn");
            this.NetworkNetworkColumn.Name = "NetworkNetworkColumn";
            this.NetworkNetworkColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.NetworkNetworkColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.m_buttonAddNetwork, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.m_buttonDeleteNetwork, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.m_invalidMacLabel, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // m_buttonAddNetwork
            // 
            resources.ApplyResources(this.m_buttonAddNetwork, "m_buttonAddNetwork");
            this.m_buttonAddNetwork.Name = "m_buttonAddNetwork";
            this.m_buttonAddNetwork.UseVisualStyleBackColor = true;
            this.m_buttonAddNetwork.Click += new System.EventHandler(this.m_buttonAddNetwork_Click);
            // 
            // m_buttonDeleteNetwork
            // 
            resources.ApplyResources(this.m_buttonDeleteNetwork, "m_buttonDeleteNetwork");
            this.m_buttonDeleteNetwork.Name = "m_buttonDeleteNetwork";
            this.m_buttonDeleteNetwork.UseVisualStyleBackColor = true;
            this.m_buttonDeleteNetwork.Click += new System.EventHandler(this.m_buttonDeleteNetwork_Click);
            // 
            // m_invalidMacLabel
            // 
            resources.ApplyResources(this.m_invalidMacLabel, "m_invalidMacLabel");
            this.m_invalidMacLabel.ForeColor = System.Drawing.Color.Red;
            this.m_invalidMacLabel.Name = "m_invalidMacLabel";
            // 
            // NetworkPickerPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NetworkPickerPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_networkGridView)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.Common.AutoHeightLabel label2;
        private System.Windows.Forms.Label m_srLabel;
        private System.Windows.Forms.DataGridView m_networkGridView;
		private System.Windows.Forms.Button m_buttonAddNetwork;
        private XenAdmin.Controls.Common.PasswordFailure m_invalidMacLabel;
		private System.Windows.Forms.Button m_buttonDeleteNetwork;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameNetworkColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MACAddressNetworkColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn NetworkNetworkColumn;

	}
}
