namespace XenAdmin.Dialogs.OptionsPages
{
    partial class DisplayOptionsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayOptionsPage));
            this.GraphsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SearchGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.showWholePoolOptionForSearchRadioButton = new System.Windows.Forms.RadioButton();
            this.showHostOnlyOptionForSearchRadioButton = new System.Windows.Forms.RadioButton();
            this.GraphTypeGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.GraphAreasRadioButton = new System.Windows.Forms.RadioButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.GraphLinesRadioButton = new System.Windows.Forms.RadioButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.GraphsTableLayoutPanel.SuspendLayout();
            this.SearchGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.GraphTypeGroupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // GraphsTableLayoutPanel
            // 
            resources.ApplyResources(this.GraphsTableLayoutPanel, "GraphsTableLayoutPanel");
            this.GraphsTableLayoutPanel.Controls.Add(this.SearchGroupBox, 0, 3);
            this.GraphsTableLayoutPanel.Controls.Add(this.GraphTypeGroupBox, 0, 1);
            this.GraphsTableLayoutPanel.Name = "GraphsTableLayoutPanel";
            // 
            // SearchGroupBox
            // 
            resources.ApplyResources(this.SearchGroupBox, "SearchGroupBox");
            this.SearchGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.SearchGroupBox.Name = "SearchGroupBox";
            this.SearchGroupBox.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.showWholePoolOptionForSearchRadioButton, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.showHostOnlyOptionForSearchRadioButton, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // showWholePoolOptionForSearchRadioButton
            // 
            resources.ApplyResources(this.showWholePoolOptionForSearchRadioButton, "showWholePoolOptionForSearchRadioButton");
            this.showWholePoolOptionForSearchRadioButton.Name = "showWholePoolOptionForSearchRadioButton";
            this.showWholePoolOptionForSearchRadioButton.TabStop = true;
            this.showWholePoolOptionForSearchRadioButton.UseVisualStyleBackColor = true;
            // 
            // showHostOnlyOptionForSearchRadioButton
            // 
            resources.ApplyResources(this.showHostOnlyOptionForSearchRadioButton, "showHostOnlyOptionForSearchRadioButton");
            this.showHostOnlyOptionForSearchRadioButton.Name = "showHostOnlyOptionForSearchRadioButton";
            this.showHostOnlyOptionForSearchRadioButton.TabStop = true;
            this.showHostOnlyOptionForSearchRadioButton.UseVisualStyleBackColor = true;
            // 
            // GraphTypeGroupBox
            // 
            resources.ApplyResources(this.GraphTypeGroupBox, "GraphTypeGroupBox");
            this.GraphTypeGroupBox.Controls.Add(this.tableLayoutPanel1);
            this.GraphTypeGroupBox.Name = "GraphTypeGroupBox";
            this.GraphTypeGroupBox.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.GraphAreasRadioButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.GraphLinesRadioButton, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // GraphAreasRadioButton
            // 
            resources.ApplyResources(this.GraphAreasRadioButton, "GraphAreasRadioButton");
            this.GraphAreasRadioButton.Name = "GraphAreasRadioButton";
            this.GraphAreasRadioButton.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources.graphline;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // GraphLinesRadioButton
            // 
            resources.ApplyResources(this.GraphLinesRadioButton, "GraphLinesRadioButton");
            this.GraphLinesRadioButton.Checked = true;
            this.GraphLinesRadioButton.Name = "GraphLinesRadioButton";
            this.GraphLinesRadioButton.TabStop = true;
            this.GraphLinesRadioButton.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources.grapharea;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // DisplayOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.GraphsTableLayoutPanel);
            this.Name = "DisplayOptionsPage";
            this.GraphsTableLayoutPanel.ResumeLayout(false);
            this.GraphsTableLayoutPanel.PerformLayout();
            this.SearchGroupBox.ResumeLayout(false);
            this.SearchGroupBox.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.GraphTypeGroupBox.ResumeLayout(false);
            this.GraphTypeGroupBox.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel GraphsTableLayoutPanel;
        private XenAdmin.Controls.DecentGroupBox GraphTypeGroupBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton GraphAreasRadioButton;
        private System.Windows.Forms.RadioButton GraphLinesRadioButton;
        private System.Windows.Forms.Label label5;
        private Controls.DecentGroupBox SearchGroupBox;
        private System.Windows.Forms.RadioButton showWholePoolOptionForSearchRadioButton;
        private System.Windows.Forms.RadioButton showHostOnlyOptionForSearchRadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}
