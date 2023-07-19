namespace XenAdmin.Dialogs.OptionsPages
{
    partial class UpdatesOptionsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdatesOptionsPage));
            this.UpdatesTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxServer = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelServerUpdates = new System.Windows.Forms.Label();
            this.clientIdControl1 = new XenAdmin.Controls.ClientIdControl();
            this._checkBoxServerVersions = new System.Windows.Forms.CheckBox();
            this._checkBoxServerUpdates = new System.Windows.Forms.CheckBox();
            this.groupBoxClient = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.labelClientUpdates = new System.Windows.Forms.Label();
            this._checkBoxClientUpdates = new System.Windows.Forms.CheckBox();
            this.UpdatesTableLayoutPanel.SuspendLayout();
            this.groupBoxServer.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxClient.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // UpdatesTableLayoutPanel
            // 
            resources.ApplyResources(this.UpdatesTableLayoutPanel, "UpdatesTableLayoutPanel");
            this.UpdatesTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.UpdatesTableLayoutPanel.Controls.Add(this.groupBoxServer, 0, 1);
            this.UpdatesTableLayoutPanel.Controls.Add(this.groupBoxClient, 0, 0);
            this.UpdatesTableLayoutPanel.Name = "UpdatesTableLayoutPanel";
            // 
            // groupBoxServer
            // 
            this.groupBoxServer.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.groupBoxServer, "groupBoxServer");
            this.groupBoxServer.Name = "groupBoxServer";
            this.groupBoxServer.TabStop = false;
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
            // groupBoxClient
            // 
            this.groupBoxClient.Controls.Add(this.tableLayoutPanel2);
            resources.ApplyResources(this.groupBoxClient, "groupBoxClient");
            this.groupBoxClient.Name = "groupBoxClient";
            this.groupBoxClient.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.labelClientUpdates, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this._checkBoxClientUpdates, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // labelClientUpdates
            // 
            resources.ApplyResources(this.labelClientUpdates, "labelClientUpdates");
            this.labelClientUpdates.Name = "labelClientUpdates";
            // 
            // _checkBoxClientUpdates
            // 
            resources.ApplyResources(this._checkBoxClientUpdates, "_checkBoxClientUpdates");
            this._checkBoxClientUpdates.Name = "_checkBoxClientUpdates";
            this._checkBoxClientUpdates.UseVisualStyleBackColor = true;
            // 
            // UpdatesOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.UpdatesTableLayoutPanel);
            this.Name = "UpdatesOptionsPage";
            this.UpdatesTableLayoutPanel.ResumeLayout(false);
            this.groupBoxServer.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBoxClient.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel UpdatesTableLayoutPanel;
        private System.Windows.Forms.Label labelClientUpdates;
        private System.Windows.Forms.CheckBox _checkBoxClientUpdates;
        private System.Windows.Forms.GroupBox groupBoxServer;
        private Controls.ClientIdControl clientIdControl1;
        private System.Windows.Forms.CheckBox _checkBoxServerUpdates;
        private System.Windows.Forms.CheckBox _checkBoxServerVersions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelServerUpdates;
        private System.Windows.Forms.GroupBox groupBoxClient;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}
