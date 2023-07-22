namespace XenAdmin.Dialogs.ServerUpdates
{
    partial class ConfigLcmUpdatesPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigLcmUpdatesPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelServerUpdates = new System.Windows.Forms.Label();
            this.clientIdControl1 = new XenAdmin.Controls.ClientIdControl();
            this._checkBoxServerVersions = new System.Windows.Forms.CheckBox();
            this._checkBoxServerUpdates = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelServerUpdates, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.clientIdControl1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._checkBoxServerVersions, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._checkBoxServerUpdates, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelServerUpdates
            // 
            resources.ApplyResources(this.labelServerUpdates, "labelServerUpdates");
            this.labelServerUpdates.Name = "labelServerUpdates";
            // 
            // clientIdControl1
            // 
            resources.ApplyResources(this.clientIdControl1, "clientIdControl1");
            this.clientIdControl1.Name = "clientIdControl1";
            // 
            // _checkBoxServerVersions
            // 
            resources.ApplyResources(this._checkBoxServerVersions, "_checkBoxServerVersions");
            this._checkBoxServerVersions.Name = "_checkBoxServerVersions";
            this._checkBoxServerVersions.UseVisualStyleBackColor = true;
            // 
            // _checkBoxServerUpdates
            // 
            resources.ApplyResources(this._checkBoxServerUpdates, "_checkBoxServerUpdates");
            this._checkBoxServerUpdates.Name = "_checkBoxServerUpdates";
            this._checkBoxServerUpdates.UseVisualStyleBackColor = true;
            // 
            // ConfigLcmUpdatesPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ConfigLcmUpdatesPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelServerUpdates;
        private Controls.ClientIdControl clientIdControl1;
        private System.Windows.Forms.CheckBox _checkBoxServerVersions;
        private System.Windows.Forms.CheckBox _checkBoxServerUpdates;
    }
}

