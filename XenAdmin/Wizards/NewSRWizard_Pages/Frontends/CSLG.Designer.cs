namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class CSLG
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSLG));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelAdapter = new System.Windows.Forms.Label();
            this.labelStorageSystem = new System.Windows.Forms.Label();
            this.comboBoxStorageSystem = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelAdapter, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelStorageSystem, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxStorageSystem, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelAdapter
            // 
            resources.ApplyResources(this.labelAdapter, "labelAdapter");
            this.tableLayoutPanel1.SetColumnSpan(this.labelAdapter, 2);
            this.labelAdapter.Name = "labelAdapter";
            // 
            // labelStorageSystem
            // 
            resources.ApplyResources(this.labelStorageSystem, "labelStorageSystem");
            this.labelStorageSystem.Name = "labelStorageSystem";
            // 
            // comboBoxStorageSystem
            // 
            resources.ApplyResources(this.comboBoxStorageSystem, "comboBoxStorageSystem");
            this.comboBoxStorageSystem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxStorageSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStorageSystem.FormattingEnabled = true;
            this.comboBoxStorageSystem.Name = "comboBoxStorageSystem";
            this.comboBoxStorageSystem.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBoxStorageSystem_DrawItem);
            this.comboBoxStorageSystem.SelectedIndexChanged += new System.EventHandler(this.comboBoxStorageSystem_SelectedIndexChanged);
            // 
            // CSLG
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CSLG";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelStorageSystem;
        private System.Windows.Forms.ComboBox comboBoxStorageSystem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelAdapter;
    }
}
