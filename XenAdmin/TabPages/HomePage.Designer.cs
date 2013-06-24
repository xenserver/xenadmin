namespace XenAdmin.TabPages
{
    partial class HomePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomePage));
            this.line2Learn = new System.Windows.Forms.Label();
            this.line1Learn = new System.Windows.Forms.Label();
            this.titleLearn = new System.Windows.Forms.Label();
            this.line1Add = new System.Windows.Forms.Label();
            this.titleAdd = new System.Windows.Forms.Label();
            this.line1Upgrade = new System.Windows.Forms.Label();
            this.titleUpgrade = new System.Windows.Forms.Label();
            this.line2Try = new System.Windows.Forms.Label();
            this.line1Try = new System.Windows.Forms.Label();
            this.titleTry = new System.Windows.Forms.Label();
            this.heading = new System.Windows.Forms.Label();
            this.subHeading = new System.Windows.Forms.Label();
            this.panelLearn = new XenAdmin.Controls.TransparentPanel();
            this.panelAdd = new XenAdmin.Controls.TransparentPanel();
            this.panelGet = new XenAdmin.Controls.TransparentPanel();
            this.panelTry = new XenAdmin.Controls.TransparentPanel();
            this.titleCommunity = new System.Windows.Forms.Label();
            this.labelNetwork = new System.Windows.Forms.Label();
            this.iconCommunity = new System.Windows.Forms.PictureBox();
            this.iconLearn = new System.Windows.Forms.PictureBox();
            this.iconAdd = new System.Windows.Forms.PictureBox();
            this.iconGet = new System.Windows.Forms.PictureBox();
            this.iconTry = new System.Windows.Forms.PictureBox();
            this.horizDiv = new System.Windows.Forms.PictureBox();
            this.vertDiv3 = new System.Windows.Forms.PictureBox();
            this.vertDiv2 = new System.Windows.Forms.PictureBox();
            this.vertDiv1 = new System.Windows.Forms.PictureBox();
            this.bulletNetwork = new System.Windows.Forms.PictureBox();
            this.bulletSupport = new System.Windows.Forms.PictureBox();
            this.bulletPartners = new System.Windows.Forms.PictureBox();
            this.labelSupport = new System.Windows.Forms.Label();
            this.labelPartners = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.iconCommunity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconLearn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconGet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconTry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.horizDiv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertDiv3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertDiv2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertDiv1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bulletNetwork)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bulletSupport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bulletPartners)).BeginInit();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // line2Learn
            // 
            resources.ApplyResources(this.line2Learn, "line2Learn");
            this.line2Learn.Name = "line2Learn";
            // 
            // line1Learn
            // 
            resources.ApplyResources(this.line1Learn, "line1Learn");
            this.line1Learn.Name = "line1Learn";
            // 
            // titleLearn
            // 
            resources.ApplyResources(this.titleLearn, "titleLearn");
            this.titleLearn.Name = "titleLearn";
            // 
            // line1Add
            // 
            resources.ApplyResources(this.line1Add, "line1Add");
            this.line1Add.Name = "line1Add";
            this.line1Add.Click += new System.EventHandler(this.panelAdd_Click);
            // 
            // titleAdd
            // 
            resources.ApplyResources(this.titleAdd, "titleAdd");
            this.titleAdd.Name = "titleAdd";
            this.titleAdd.Click += new System.EventHandler(this.panelAdd_Click);
            // 
            // line1Upgrade
            // 
            resources.ApplyResources(this.line1Upgrade, "line1Upgrade");
            this.line1Upgrade.Name = "line1Upgrade";
            this.line1Upgrade.Click += new System.EventHandler(this.panelGet_Click);
            // 
            // titleUpgrade
            // 
            resources.ApplyResources(this.titleUpgrade, "titleUpgrade");
            this.titleUpgrade.Name = "titleUpgrade";
            this.titleUpgrade.Click += new System.EventHandler(this.panelGet_Click);
            // 
            // line2Try
            // 
            resources.ApplyResources(this.line2Try, "line2Try");
            this.line2Try.Name = "line2Try";
            this.line2Try.Click += new System.EventHandler(this.panelTry_Click);
            // 
            // line1Try
            // 
            resources.ApplyResources(this.line1Try, "line1Try");
            this.line1Try.Name = "line1Try";
            this.line1Try.Click += new System.EventHandler(this.panelTry_Click);
            // 
            // titleTry
            // 
            this.titleTry.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            resources.ApplyResources(this.titleTry, "titleTry");
            this.titleTry.Name = "titleTry";
            this.titleTry.Click += new System.EventHandler(this.panelTry_Click);
            // 
            // heading
            // 
            resources.ApplyResources(this.heading, "heading");
            this.heading.Name = "heading";
            // 
            // subHeading
            // 
            this.subHeading.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            resources.ApplyResources(this.subHeading, "subHeading");
            this.subHeading.Name = "subHeading";
            // 
            // panelLearn
            // 
            this.panelLearn.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.panelLearn, "panelLearn");
            this.panelLearn.Name = "panelLearn";
            this.panelLearn.MouseLeave += new System.EventHandler(this.panelLearn_MouseLeave);
            this.panelLearn.Click += new System.EventHandler(this.panelLearn_Click);
            this.panelLearn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelLearn_MouseDown);
            this.panelLearn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelLearn_MouseUp);
            this.panelLearn.MouseEnter += new System.EventHandler(this.panelLearn_MouseEnter);
            // 
            // panelAdd
            // 
            this.panelAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.panelAdd, "panelAdd");
            this.panelAdd.Name = "panelAdd";
            this.panelAdd.MouseLeave += new System.EventHandler(this.panelAdd_MouseLeave);
            this.panelAdd.Click += new System.EventHandler(this.panelAdd_Click);
            this.panelAdd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelAdd_MouseDown);
            this.panelAdd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelAdd_MouseUp);
            this.panelAdd.MouseEnter += new System.EventHandler(this.panelAdd_MouseEnter);
            // 
            // panelGet
            // 
            this.panelGet.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.panelGet, "panelGet");
            this.panelGet.Name = "panelGet";
            this.panelGet.MouseLeave += new System.EventHandler(this.panelGet_MouseLeave);
            this.panelGet.Click += new System.EventHandler(this.panelGet_Click);
            this.panelGet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelGet_MouseDown);
            this.panelGet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelGet_MouseUp);
            this.panelGet.MouseEnter += new System.EventHandler(this.panelGet_MouseEnter);
            // 
            // panelTry
            // 
            this.panelTry.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.panelTry, "panelTry");
            this.panelTry.Name = "panelTry";
            this.panelTry.MouseLeave += new System.EventHandler(this.panelTry_MouseLeave);
            this.panelTry.Click += new System.EventHandler(this.panelTry_Click);
            this.panelTry.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTry_MouseDown);
            this.panelTry.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTry_MouseUp);
            this.panelTry.MouseEnter += new System.EventHandler(this.panelTry_MouseEnter);
            // 
            // titleCommunity
            // 
            resources.ApplyResources(this.titleCommunity, "titleCommunity");
            this.titleCommunity.Name = "titleCommunity";
            // 
            // labelNetwork
            // 
            resources.ApplyResources(this.labelNetwork, "labelNetwork");
            this.labelNetwork.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelNetwork.Name = "labelNetwork";
            this.labelNetwork.MouseLeave += new System.EventHandler(this.labelNetwork_MouseLeave);
            this.labelNetwork.Click += new System.EventHandler(this.labelNetwork_Click);
            this.labelNetwork.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labelNetwork_MouseDown);
            this.labelNetwork.MouseUp += new System.Windows.Forms.MouseEventHandler(this.labelNetwork_MouseUp);
            this.labelNetwork.MouseEnter += new System.EventHandler(this.labelNetwork_MouseEnter);
            // 
            // iconCommunity
            // 
            this.iconCommunity.ErrorImage = null;
            this.iconCommunity.Image = global::XenAdmin.Properties.Resources.homepage_community_icon;
            resources.ApplyResources(this.iconCommunity, "iconCommunity");
            this.iconCommunity.InitialImage = null;
            this.iconCommunity.Name = "iconCommunity";
            this.iconCommunity.TabStop = false;
            // 
            // iconLearn
            // 
            this.iconLearn.ErrorImage = null;
            this.iconLearn.Image = global::XenAdmin.Properties.Resources.homepage_learn_icon;
            resources.ApplyResources(this.iconLearn, "iconLearn");
            this.iconLearn.InitialImage = null;
            this.iconLearn.Name = "iconLearn";
            this.iconLearn.TabStop = false;
            // 
            // iconAdd
            // 
            this.iconAdd.ErrorImage = null;
            this.iconAdd.Image = global::XenAdmin.Properties.Resources.homepage_add_icon;
            resources.ApplyResources(this.iconAdd, "iconAdd");
            this.iconAdd.InitialImage = null;
            this.iconAdd.Name = "iconAdd";
            this.iconAdd.TabStop = false;
            this.iconAdd.Click += new System.EventHandler(this.panelAdd_Click);
            // 
            // iconGet
            // 
            this.iconGet.ErrorImage = null;
            this.iconGet.Image = global::XenAdmin.Properties.Resources.homepage_get_icon;
            resources.ApplyResources(this.iconGet, "iconGet");
            this.iconGet.InitialImage = null;
            this.iconGet.Name = "iconGet";
            this.iconGet.TabStop = false;
            this.iconGet.Click += new System.EventHandler(this.panelGet_Click);
            // 
            // iconTry
            // 
            this.iconTry.ErrorImage = null;
            this.iconTry.Image = global::XenAdmin.Properties.Resources.homepage_try_icon;
            resources.ApplyResources(this.iconTry, "iconTry");
            this.iconTry.InitialImage = null;
            this.iconTry.Name = "iconTry";
            this.iconTry.TabStop = false;
            this.iconTry.Click += new System.EventHandler(this.panelTry_Click);
            // 
            // horizDiv
            // 
            this.horizDiv.ErrorImage = null;
            this.horizDiv.Image = global::XenAdmin.Properties.Resources.homepage_hor_div;
            resources.ApplyResources(this.horizDiv, "horizDiv");
            this.horizDiv.InitialImage = null;
            this.horizDiv.Name = "horizDiv";
            this.horizDiv.TabStop = false;
            // 
            // vertDiv3
            // 
            this.vertDiv3.ErrorImage = null;
            this.vertDiv3.Image = global::XenAdmin.Properties.Resources.homepage_vert_div;
            resources.ApplyResources(this.vertDiv3, "vertDiv3");
            this.vertDiv3.InitialImage = null;
            this.vertDiv3.Name = "vertDiv3";
            this.vertDiv3.TabStop = false;
            // 
            // vertDiv2
            // 
            this.vertDiv2.ErrorImage = null;
            this.vertDiv2.Image = global::XenAdmin.Properties.Resources.homepage_vert_div;
            resources.ApplyResources(this.vertDiv2, "vertDiv2");
            this.vertDiv2.InitialImage = null;
            this.vertDiv2.Name = "vertDiv2";
            this.vertDiv2.TabStop = false;
            // 
            // vertDiv1
            // 
            this.vertDiv1.ErrorImage = null;
            this.vertDiv1.Image = global::XenAdmin.Properties.Resources.homepage_vert_div;
            resources.ApplyResources(this.vertDiv1, "vertDiv1");
            this.vertDiv1.InitialImage = null;
            this.vertDiv1.Name = "vertDiv1";
            this.vertDiv1.TabStop = false;
            // 
            // bulletNetwork
            // 
            this.bulletNetwork.ErrorImage = null;
            this.bulletNetwork.Image = global::XenAdmin.Properties.Resources.homepage_bullet;
            this.bulletNetwork.InitialImage = null;
            resources.ApplyResources(this.bulletNetwork, "bulletNetwork");
            this.bulletNetwork.Name = "bulletNetwork";
            this.bulletNetwork.TabStop = false;
            // 
            // bulletSupport
            // 
            this.bulletSupport.ErrorImage = null;
            this.bulletSupport.Image = global::XenAdmin.Properties.Resources.homepage_bullet;
            resources.ApplyResources(this.bulletSupport, "bulletSupport");
            this.bulletSupport.InitialImage = null;
            this.bulletSupport.Name = "bulletSupport";
            this.bulletSupport.TabStop = false;
            // 
            // bulletPartners
            // 
            this.bulletPartners.ErrorImage = null;
            this.bulletPartners.Image = global::XenAdmin.Properties.Resources.homepage_bullet;
            resources.ApplyResources(this.bulletPartners, "bulletPartners");
            this.bulletPartners.InitialImage = null;
            this.bulletPartners.Name = "bulletPartners";
            this.bulletPartners.TabStop = false;
            // 
            // labelSupport
            // 
            resources.ApplyResources(this.labelSupport, "labelSupport");
            this.labelSupport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelSupport.Name = "labelSupport";
            this.labelSupport.MouseLeave += new System.EventHandler(this.labelSupport_MouseLeave);
            this.labelSupport.Click += new System.EventHandler(this.labelSupport_Click);
            this.labelSupport.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labelSupport_MouseDown);
            this.labelSupport.MouseUp += new System.Windows.Forms.MouseEventHandler(this.labelSupport_MouseUp);
            this.labelSupport.MouseEnter += new System.EventHandler(this.labelSupport_MouseEnter);
            // 
            // labelPartners
            // 
            resources.ApplyResources(this.labelPartners, "labelPartners");
            this.labelPartners.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelPartners.Name = "labelPartners";
            this.labelPartners.MouseLeave += new System.EventHandler(this.labelPartners_MouseLeave);
            this.labelPartners.Click += new System.EventHandler(this.labelPartners_Click);
            this.labelPartners.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labelPartners_MouseDown);
            this.labelPartners.MouseUp += new System.Windows.Forms.MouseEventHandler(this.labelPartners_MouseUp);
            this.labelPartners.MouseEnter += new System.EventHandler(this.labelPartners_MouseEnter);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.labelPartners);
            this.mainPanel.Controls.Add(this.labelSupport);
            this.mainPanel.Controls.Add(this.bulletPartners);
            this.mainPanel.Controls.Add(this.bulletSupport);
            this.mainPanel.Controls.Add(this.bulletNetwork);
            this.mainPanel.Controls.Add(this.labelNetwork);
            this.mainPanel.Controls.Add(this.iconCommunity);
            this.mainPanel.Controls.Add(this.titleCommunity);
            this.mainPanel.Controls.Add(this.panelTry);
            this.mainPanel.Controls.Add(this.panelGet);
            this.mainPanel.Controls.Add(this.panelAdd);
            this.mainPanel.Controls.Add(this.panelLearn);
            this.mainPanel.Controls.Add(this.iconLearn);
            this.mainPanel.Controls.Add(this.line2Learn);
            this.mainPanel.Controls.Add(this.line1Learn);
            this.mainPanel.Controls.Add(this.titleLearn);
            this.mainPanel.Controls.Add(this.iconAdd);
            this.mainPanel.Controls.Add(this.titleAdd);
            this.mainPanel.Controls.Add(this.line1Add);
            this.mainPanel.Controls.Add(this.iconGet);
            this.mainPanel.Controls.Add(this.titleUpgrade);
            this.mainPanel.Controls.Add(this.line1Upgrade);
            this.mainPanel.Controls.Add(this.iconTry);
            this.mainPanel.Controls.Add(this.titleTry);
            this.mainPanel.Controls.Add(this.line1Try);
            this.mainPanel.Controls.Add(this.line2Try);
            this.mainPanel.Controls.Add(this.subHeading);
            this.mainPanel.Controls.Add(this.heading);
            this.mainPanel.Controls.Add(this.horizDiv);
            this.mainPanel.Controls.Add(this.vertDiv3);
            this.mainPanel.Controls.Add(this.vertDiv2);
            this.mainPanel.Controls.Add(this.vertDiv1);
            resources.ApplyResources(this.mainPanel, "mainPanel");
            this.mainPanel.MinimumSize = new System.Drawing.Size(707, 442);
            this.mainPanel.Name = "mainPanel";
            // 
            // HomePage
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mainPanel);
            resources.ApplyResources(this, "$this");
            this.Name = "HomePage";
            this.SizeChanged += new System.EventHandler(this.HomePage_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.iconCommunity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconLearn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconGet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconTry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.horizDiv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertDiv3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertDiv2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertDiv1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bulletNetwork)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bulletSupport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bulletPartners)).EndInit();
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox vertDiv1;
        private System.Windows.Forms.PictureBox vertDiv2;
        private System.Windows.Forms.PictureBox vertDiv3;
        private System.Windows.Forms.PictureBox horizDiv;
        private System.Windows.Forms.PictureBox iconLearn;
        private System.Windows.Forms.PictureBox iconAdd;
        private System.Windows.Forms.PictureBox iconGet;
        private System.Windows.Forms.PictureBox iconTry;
        private System.Windows.Forms.Label heading;
        private System.Windows.Forms.Label subHeading;
        private System.Windows.Forms.Label titleLearn;
        private System.Windows.Forms.Label titleAdd;
        private System.Windows.Forms.Label titleUpgrade;
        private System.Windows.Forms.Label titleTry;
        private System.Windows.Forms.Label line1Learn;
        private System.Windows.Forms.Label line1Add;
        private System.Windows.Forms.Label line1Upgrade;
        private System.Windows.Forms.Label line1Try;
        private System.Windows.Forms.Label line2Learn;
        private System.Windows.Forms.Label line2Try;
        private XenAdmin.Controls.TransparentPanel panelLearn;
        private XenAdmin.Controls.TransparentPanel panelAdd;
        private XenAdmin.Controls.TransparentPanel panelGet;
        private XenAdmin.Controls.TransparentPanel panelTry;
        private System.Windows.Forms.Label titleCommunity;
        private System.Windows.Forms.PictureBox iconCommunity;
        private System.Windows.Forms.Label labelNetwork;
        private System.Windows.Forms.PictureBox bulletNetwork;
        private System.Windows.Forms.PictureBox bulletSupport;
        private System.Windows.Forms.PictureBox bulletPartners;
        private System.Windows.Forms.Label labelSupport;
        private System.Windows.Forms.Label labelPartners;
        private System.Windows.Forms.Panel mainPanel;
    }
}
