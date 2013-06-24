namespace XenAdmin.Dialogs
{
    partial class BallooningDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BallooningDialog));
            this.memoryControlsBasic = new XenAdmin.Controls.Ballooning.VMMemoryControlsBasic();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.rubric = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // memoryControlsBasic
            // 
            resources.ApplyResources(this.memoryControlsBasic, "memoryControlsBasic");
            this.memoryControlsBasic.Name = "memoryControlsBasic";
            this.memoryControlsBasic.InstallTools += new System.EventHandler(this.memoryControls_InstallTools);
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // rubric
            // 
            resources.ApplyResources(this.rubric, "rubric");
            this.rubric.Name = "rubric";
            // 
            // BallooningDialog
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.rubric);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.memoryControlsBasic);
            this.Name = "BallooningDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.Ballooning.VMMemoryControlsBasic memoryControlsBasic;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label rubric;
    }
}
