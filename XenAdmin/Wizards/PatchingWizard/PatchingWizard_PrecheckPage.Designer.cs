using XenAdmin.Controls.DataGridViewEx;

namespace XenAdmin.Wizards.PatchingWizard
{
    partial class PatchingWizard_PrecheckPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatchingWizard_PrecheckPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelPrechecksFirstLine = new System.Windows.Forms.Label();
            this.dataGridView1 = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnState = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSolution = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonReCheckProblems = new System.Windows.Forms.Button();
            this.buttonResolveAll = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panelErrorsFound = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkBoxViewPrecheckFailuresOnly = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panelErrorsFound.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelPrechecksFirstLine
            // 
            resources.ApplyResources(this.labelPrechecksFirstLine, "labelPrechecksFirstLine");
            this.labelPrechecksFirstLine.AutoEllipsis = true;
            this.labelPrechecksFirstLine.Name = "labelPrechecksFirstLine";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToResizeColumns = false;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ColumnHeadersVisible = false;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnState,
            this.ColumnDescription,
            this.ColumnSolution});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.HideSelection = true;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridView1.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseMove);
            this.dataGridView1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dataGridView1_KeyPress);
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // ColumnState
            // 
            this.ColumnState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.NullValue = "System.Drawing.Bitmap";
            this.ColumnState.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.ColumnState, "ColumnState");
            this.ColumnState.Name = "ColumnState";
            this.ColumnState.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnState.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ColumnDescription
            // 
            this.ColumnDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnDescription.FillWeight = 137.8531F;
            resources.ApplyResources(this.ColumnDescription, "ColumnDescription");
            this.ColumnDescription.Name = "ColumnDescription";
            // 
            // ColumnSolution
            // 
            this.ColumnSolution.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnSolution.FillWeight = 40F;
            resources.ApplyResources(this.ColumnSolution, "ColumnSolution");
            this.ColumnSolution.Name = "ColumnSolution";
            // 
            // buttonReCheckProblems
            // 
            resources.ApplyResources(this.buttonReCheckProblems, "buttonReCheckProblems");
            this.buttonReCheckProblems.Name = "buttonReCheckProblems";
            this.buttonReCheckProblems.UseVisualStyleBackColor = true;
            this.buttonReCheckProblems.Click += new System.EventHandler(this.buttonReCheckProblems_Click);
            // 
            // buttonResolveAll
            // 
            resources.ApplyResources(this.buttonResolveAll, "buttonResolveAll");
            this.buttonResolveAll.Name = "buttonResolveAll";
            this.buttonResolveAll.UseVisualStyleBackColor = true;
            this.buttonResolveAll.Click += new System.EventHandler(this.buttonResolveAll_Click);
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // panelErrorsFound
            // 
            resources.ApplyResources(this.panelErrorsFound, "panelErrorsFound");
            this.panelErrorsFound.Controls.Add(this.label1);
            this.panelErrorsFound.Controls.Add(this.pictureBox1);
            this.panelErrorsFound.Name = "panelErrorsFound";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.AutoEllipsis = true;
            this.label1.Name = "label1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Abort_h32bit_16;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // checkBoxViewPrecheckFailuresOnly
            // 
            resources.ApplyResources(this.checkBoxViewPrecheckFailuresOnly, "checkBoxViewPrecheckFailuresOnly");
            this.checkBoxViewPrecheckFailuresOnly.Name = "checkBoxViewPrecheckFailuresOnly";
            this.checkBoxViewPrecheckFailuresOnly.UseVisualStyleBackColor = true;
            this.checkBoxViewPrecheckFailuresOnly.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // PatchingWizard_PrecheckPage
            // 
            this.Controls.Add(this.checkBoxViewPrecheckFailuresOnly);
            this.Controls.Add(this.panelErrorsFound);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.buttonResolveAll);
            this.Controls.Add(this.buttonReCheckProblems);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.labelPrechecksFirstLine);
            resources.ApplyResources(this, "$this");
            this.Name = "PatchingWizard_PrecheckPage";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panelErrorsFound.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Label labelPrechecksFirstLine;
        private DataGridViewEx dataGridView1;
        private System.Windows.Forms.Button buttonReCheckProblems;
        private System.Windows.Forms.Button buttonResolveAll;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Panel panelErrorsFound;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox checkBoxViewPrecheckFailuresOnly;
        private System.Windows.Forms.DataGridViewImageColumn ColumnState;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSolution;

    }
}
