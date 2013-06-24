namespace XenAdmin.Wizards.ExportWizard
{
    partial class ExportEulaPage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportEulaPage));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.m_buttonAdd = new System.Windows.Forms.Button();
			this.m_buttonRemove = new System.Windows.Forms.Button();
			this.m_buttonView = new System.Windows.Forms.Button();
			this.m_listViewEulaFiles = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.m_labelEulaIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.m_toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.m_listViewEulaFiles, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.m_labelEulaIntro, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// tableLayoutPanel2
			// 
			resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
			this.tableLayoutPanel2.Controls.Add(this.m_buttonAdd, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.m_buttonRemove, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.m_buttonView, 2, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			// 
			// m_buttonAdd
			// 
			resources.ApplyResources(this.m_buttonAdd, "m_buttonAdd");
			this.m_buttonAdd.Name = "m_buttonAdd";
			this.m_buttonAdd.UseVisualStyleBackColor = true;
			this.m_buttonAdd.Click += new System.EventHandler(this.m_buttonAdd_Click);
			// 
			// m_buttonRemove
			// 
			resources.ApplyResources(this.m_buttonRemove, "m_buttonRemove");
			this.m_buttonRemove.Name = "m_buttonRemove";
			this.m_buttonRemove.UseVisualStyleBackColor = true;
			this.m_buttonRemove.Click += new System.EventHandler(this.m_buttonRemove_Click);
			// 
			// m_buttonView
			// 
			resources.ApplyResources(this.m_buttonView, "m_buttonView");
			this.m_buttonView.Name = "m_buttonView";
			this.m_buttonView.UseVisualStyleBackColor = true;
			this.m_buttonView.Click += new System.EventHandler(this.m_buttonView_Click);
			// 
			// m_listViewEulaFiles
			// 
			this.m_listViewEulaFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			resources.ApplyResources(this.m_listViewEulaFiles, "m_listViewEulaFiles");
			this.m_listViewEulaFiles.FullRowSelect = true;
			this.m_listViewEulaFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.m_listViewEulaFiles.HideSelection = false;
			this.m_listViewEulaFiles.Name = "m_listViewEulaFiles";
			this.m_listViewEulaFiles.UseCompatibleStateImageBehavior = false;
			this.m_listViewEulaFiles.View = System.Windows.Forms.View.Details;
			this.m_listViewEulaFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_listViewEulaFiles_MouseDoubleClick);
			this.m_listViewEulaFiles.SelectedIndexChanged += new System.EventHandler(this.m_listViewEulaFiles_SelectedIndexChanged);
			// 
			// m_labelEulaIntro
			// 
			resources.ApplyResources(this.m_labelEulaIntro, "m_labelEulaIntro");
			this.m_labelEulaIntro.Name = "m_labelEulaIntro";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// m_toolTip
			// 
			this.m_toolTip.ShowAlways = true;
			// 
			// ExportEulaPage
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ExportEulaPage";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button m_buttonAdd;
		private System.Windows.Forms.Button m_buttonRemove;
		private XenAdmin.Controls.Common.AutoHeightLabel m_labelEulaIntro;
		private System.Windows.Forms.Button m_buttonView;
		private System.Windows.Forms.ListView m_listViewEulaFiles;
		private System.Windows.Forms.ToolTip m_toolTip;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Label label1;

    }
}
