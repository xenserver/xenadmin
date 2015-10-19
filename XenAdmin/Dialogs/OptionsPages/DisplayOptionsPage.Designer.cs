namespace XenAdmin.Dialogs.OptionsPages
{
    partial class GraphsOptionsPage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphsOptionsPage));
			this.GraphsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.GraphTypeGroupBox = new XenAdmin.Controls.DecentGroupBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.GraphAreasRadioButton = new System.Windows.Forms.RadioButton();
			this.GraphLinesRadioButton = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.GraphsTableLayoutPanel.SuspendLayout();
			this.GraphTypeGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// GraphsTableLayoutPanel
			// 
			resources.ApplyResources(this.GraphsTableLayoutPanel, "GraphsTableLayoutPanel");
			this.GraphsTableLayoutPanel.Controls.Add(this.GraphTypeGroupBox, 0, 1);
			this.GraphsTableLayoutPanel.Controls.Add(this.label5, 0, 0);
			this.GraphsTableLayoutPanel.Name = "GraphsTableLayoutPanel";
			// 
			// GraphTypeGroupBox
			// 
			resources.ApplyResources(this.GraphTypeGroupBox, "GraphTypeGroupBox");
			this.GraphTypeGroupBox.Controls.Add(this.pictureBox2);
			this.GraphTypeGroupBox.Controls.Add(this.pictureBox1);
			this.GraphTypeGroupBox.Controls.Add(this.GraphAreasRadioButton);
			this.GraphTypeGroupBox.Controls.Add(this.GraphLinesRadioButton);
			this.GraphTypeGroupBox.Name = "GraphTypeGroupBox";
			this.GraphTypeGroupBox.TabStop = false;
			// 
			// pictureBox2
			// 
			resources.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Image = global::XenAdmin.Properties.Resources.graphline;
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = global::XenAdmin.Properties.Resources.grapharea;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// GraphAreasRadioButton
			// 
			resources.ApplyResources(this.GraphAreasRadioButton, "GraphAreasRadioButton");
			this.GraphAreasRadioButton.Name = "GraphAreasRadioButton";
			this.GraphAreasRadioButton.UseVisualStyleBackColor = true;
			// 
			// GraphLinesRadioButton
			// 
			resources.ApplyResources(this.GraphLinesRadioButton, "GraphLinesRadioButton");
			this.GraphLinesRadioButton.Checked = true;
			this.GraphLinesRadioButton.Name = "GraphLinesRadioButton";
			this.GraphLinesRadioButton.TabStop = true;
			this.GraphLinesRadioButton.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// GraphsOptionsPage
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.GraphsTableLayoutPanel);
			this.Name = "GraphsOptionsPage";
			this.GraphsTableLayoutPanel.ResumeLayout(false);
			this.GraphsTableLayoutPanel.PerformLayout();
			this.GraphTypeGroupBox.ResumeLayout(false);
			this.GraphTypeGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel GraphsTableLayoutPanel;
        private XenAdmin.Controls.DecentGroupBox GraphTypeGroupBox;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton GraphAreasRadioButton;
        private System.Windows.Forms.RadioButton GraphLinesRadioButton;
		private System.Windows.Forms.Label label5;
    }
}
