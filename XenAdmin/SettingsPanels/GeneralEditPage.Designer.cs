namespace XenAdmin.SettingsPanels
{
    partial class GeneralEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralEditPage));
            this.lblName = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblIQN = new System.Windows.Forms.Label();
            this.labelIqnHint = new System.Windows.Forms.Label();
            this.labelTags = new System.Windows.Forms.Label();
            this.lblFolder = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtIQN = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblDescrReadOnly = new System.Windows.Forms.Label();
            this.txtDescrReadOnly = new System.Windows.Forms.Label();
            this.labelTitle = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.folderPanel = new XenAdmin.Controls.BlueBorderPanel();
            this.tagsPanel = new XenAdmin.Controls.BlueBorderPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // lblIQN
            // 
            resources.ApplyResources(this.lblIQN, "lblIQN");
            this.lblIQN.Name = "lblIQN";
            // 
            // labelIqnHint
            // 
            this.labelIqnHint.AutoEllipsis = true;
            resources.ApplyResources(this.labelIqnHint, "labelIqnHint");
            this.labelIqnHint.Name = "labelIqnHint";
            // 
            // labelTags
            // 
            resources.ApplyResources(this.labelTags, "labelTags");
            this.labelTags.Name = "labelTags";
            // 
            // lblFolder
            // 
            resources.ApplyResources(this.lblFolder, "lblFolder");
            this.lblFolder.Name = "lblFolder";
            // 
            // txtName
            // 
            resources.ApplyResources(this.txtName, "txtName");
            this.txtName.Name = "txtName";
            this.txtName.TextChanged += new System.EventHandler(this.AnyTextChanged);
            // 
            // txtDescription
            // 
            resources.ApplyResources(this.txtDescription, "txtDescription");
            this.txtDescription.MinimumSize = new System.Drawing.Size(4, 50);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.TextChanged += new System.EventHandler(this.AnyTextChanged);
            // 
            // txtIQN
            // 
            resources.ApplyResources(this.txtIQN, "txtIQN");
            this.txtIQN.Name = "txtIQN";
            this.txtIQN.TextChanged += new System.EventHandler(this.AnyTextChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelTitle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblDescription, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtDescription, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblDescrReadOnly, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtDescrReadOnly, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblFolder, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.folderPanel, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelTags, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.tagsPanel, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.lblIQN, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.txtIQN, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelIqnHint, 1, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // lblDescrReadOnly
            // 
            resources.ApplyResources(this.lblDescrReadOnly, "lblDescrReadOnly");
            this.lblDescrReadOnly.Name = "lblDescrReadOnly";
            // 
            // txtDescrReadOnly
            // 
            this.txtDescrReadOnly.AutoEllipsis = true;
            resources.ApplyResources(this.txtDescrReadOnly, "txtDescrReadOnly");
            this.txtDescrReadOnly.MinimumSize = new System.Drawing.Size(0, 50);
            this.txtDescrReadOnly.Name = "txtDescrReadOnly";
            // 
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            this.tableLayoutPanel1.SetColumnSpan(this.labelTitle, 2);
            this.labelTitle.Name = "labelTitle";
            // 
            // folderPanel
            // 
            this.folderPanel.BackColor = System.Drawing.SystemColors.Window;
            this.folderPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(158)))), ((int)(((byte)(189)))));
            this.folderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.folderPanel, "folderPanel");
            this.folderPanel.Name = "folderPanel";
            this.folderPanel.TabStop = true;
            // 
            // tagsPanel
            // 
            this.tagsPanel.BackColor = System.Drawing.SystemColors.Window;
            this.tagsPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(158)))), ((int)(((byte)(189)))));
            this.tagsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.tagsPanel, "tagsPanel");
            this.tagsPanel.Name = "tagsPanel";
            this.tagsPanel.TabStop = true;
            // 
            // GeneralEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "GeneralEditPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblIQN;
        private System.Windows.Forms.Label labelIqnHint;
        private System.Windows.Forms.Label labelTags;
        private System.Windows.Forms.Label lblFolder;
        public System.Windows.Forms.TextBox txtName;
        public System.Windows.Forms.TextBox txtDescription;
        public System.Windows.Forms.TextBox txtIQN;
        private XenAdmin.Controls.BlueBorderPanel tagsPanel;
        private XenAdmin.Controls.BlueBorderPanel folderPanel;
        private XenAdmin.Controls.Common.AutoHeightLabel labelTitle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblDescrReadOnly;
        private System.Windows.Forms.Label txtDescrReadOnly;

    }
}
