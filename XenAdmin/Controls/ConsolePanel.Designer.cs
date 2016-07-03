namespace XenAdmin.Controls
{
    partial class ConsolePanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsolePanel));
            this.tableLayoutPanelRbac = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lableRbacWarning = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanelError = new System.Windows.Forms.TableLayoutPanel();
            this.errorLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanelRbac.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanelError.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelRbac
            // 
            resources.ApplyResources(this.tableLayoutPanelRbac, "tableLayoutPanelRbac");
            this.tableLayoutPanelRbac.Controls.Add(this.pictureBox2, 0, 0);
            this.tableLayoutPanelRbac.Controls.Add(this.lableRbacWarning, 1, 0);
            this.tableLayoutPanelRbac.Name = "tableLayoutPanelRbac";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // lableRbacWarning
            // 
            resources.ApplyResources(this.lableRbacWarning, "lableRbacWarning");
            this.lableRbacWarning.Name = "lableRbacWarning";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // tableLayoutPanelError
            // 
            resources.ApplyResources(this.tableLayoutPanelError, "tableLayoutPanelError");
            this.tableLayoutPanelError.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanelError.Controls.Add(this.errorLabel, 1, 0);
            this.tableLayoutPanelError.Name = "tableLayoutPanelError";
            // 
            // errorLabel
            // 
            resources.ApplyResources(this.errorLabel, "errorLabel");
            this.errorLabel.Name = "errorLabel";
            // 
            // ConsolePanel
            // 
            this.Controls.Add(this.tableLayoutPanelError);
            this.Controls.Add(this.tableLayoutPanelRbac);
            this.Name = "ConsolePanel";
            resources.ApplyResources(this, "$this");
            this.tableLayoutPanelRbac.ResumeLayout(false);
            this.tableLayoutPanelRbac.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanelError.ResumeLayout(false);
            this.tableLayoutPanelError.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        #endregion

        private System.Windows.Forms.Label lableRbacWarning;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRbac;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelError;
        private System.Windows.Forms.Label errorLabel;

    }
}