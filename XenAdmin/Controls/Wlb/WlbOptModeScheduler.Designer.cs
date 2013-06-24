namespace XenAdmin.Controls.Wlb
{
    partial class WlbOptModeScheduler
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbOptModeScheduler));
            XenAdmin.Controls.Wlb.TriggerPoints triggerPoints1 = new XenAdmin.Controls.Wlb.TriggerPoints();
            this.panelScheduleList = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lvTaskList = new System.Windows.Forms.ListView();
            this.columnHeaderHidden = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderMode = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderDay = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderTime = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderEnabled = new System.Windows.Forms.ColumnHeader();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.lableScheduledTaskBlurb = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonAddNew = new System.Windows.Forms.Button();
            this.weekView1 = new XenAdmin.Controls.Wlb.WeekView();
            this.panelScheduleList.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelScheduleList
            // 
            this.panelScheduleList.Controls.Add(this.panel1);
            this.panelScheduleList.Controls.Add(this.flowLayoutPanel2);
            this.panelScheduleList.Controls.Add(this.flowLayoutPanel1);
            resources.ApplyResources(this.panelScheduleList, "panelScheduleList");
            this.panelScheduleList.Name = "panelScheduleList";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.weekView1);
            this.panel1.Controls.Add(this.lvTaskList);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // lvTaskList
            // 
            resources.ApplyResources(this.lvTaskList, "lvTaskList");
            this.lvTaskList.CheckBoxes = true;
            this.lvTaskList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderHidden,
            this.columnHeaderMode,
            this.columnHeaderDay,
            this.columnHeaderTime,
            this.columnHeaderEnabled});
            this.lvTaskList.FullRowSelect = true;
            this.lvTaskList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvTaskList.HideSelection = false;
            this.lvTaskList.MultiSelect = false;
            this.lvTaskList.Name = "lvTaskList";
            this.lvTaskList.OwnerDraw = true;
            this.lvTaskList.ShowItemToolTips = true;
            this.lvTaskList.UseCompatibleStateImageBehavior = false;
            this.lvTaskList.View = System.Windows.Forms.View.Details;
            this.lvTaskList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvTaskList_MouseDoubleClick);
            this.lvTaskList.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.lvTaskList_DrawColumnHeader);
            this.lvTaskList.Resize += new System.EventHandler(this.lvTaskList_Resize);
            this.lvTaskList.EnabledChanged += new System.EventHandler(this.lvTaskList_EnabledChanged);
            this.lvTaskList.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lvTaskList_DrawItem);
            this.lvTaskList.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.lvTaskList_ColumnWidthChanged);
            this.lvTaskList.SelectedIndexChanged += new System.EventHandler(this.lvTaskList_SelectedIndexChanged);
            this.lvTaskList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lvTaskList_MouseDown);
            this.lvTaskList.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.lvTaskList_DrawSubItem);
            // 
            // columnHeaderHidden
            // 
            this.columnHeaderHidden.Text = global::XenAdmin.Messages.SOLUTION_UNKNOWN;
            resources.ApplyResources(this.columnHeaderHidden, "columnHeaderHidden");
            // 
            // columnHeaderMode
            // 
            resources.ApplyResources(this.columnHeaderMode, "columnHeaderMode");
            // 
            // columnHeaderDay
            // 
            resources.ApplyResources(this.columnHeaderDay, "columnHeaderDay");
            // 
            // columnHeaderTime
            // 
            resources.ApplyResources(this.columnHeaderTime, "columnHeaderTime");
            // 
            // columnHeaderEnabled
            // 
            resources.ApplyResources(this.columnHeaderEnabled, "columnHeaderEnabled");
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Controls.Add(this.lableScheduledTaskBlurb);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // lableScheduledTaskBlurb
            // 
            resources.ApplyResources(this.lableScheduledTaskBlurb, "lableScheduledTaskBlurb");
            this.lableScheduledTaskBlurb.Name = "lableScheduledTaskBlurb";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonDelete);
            this.flowLayoutPanel1.Controls.Add(this.buttonEdit);
            this.flowLayoutPanel1.Controls.Add(this.buttonAddNew);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // buttonDelete
            // 
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonEdit
            // 
            resources.ApplyResources(this.buttonEdit, "buttonEdit");
            this.buttonEdit.BackColor = System.Drawing.Color.Transparent;
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.UseVisualStyleBackColor = false;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonAddNew
            // 
            resources.ApplyResources(this.buttonAddNew, "buttonAddNew");
            this.buttonAddNew.BackColor = System.Drawing.Color.Transparent;
            this.buttonAddNew.Name = "buttonAddNew";
            this.buttonAddNew.UseVisualStyleBackColor = false;
            this.buttonAddNew.Click += new System.EventHandler(this.buttonAddNew_Click);
            // 
            // weekView1
            // 
            this.weekView1.BarHeight = 22;
            this.weekView1.BarPadding = new System.Windows.Forms.Padding(3, 2, 3, 1);
            this.weekView1.CurrentTimeMarkColor = System.Drawing.Color.Red;
            this.weekView1.DayLabelColor = System.Drawing.Color.DarkGray;
            this.weekView1.DayLabelPadding = new System.Windows.Forms.Padding(0, 3, 3, 3);
            resources.ApplyResources(this.weekView1, "weekView1");
            this.weekView1.GridColor = System.Drawing.SystemColors.ActiveBorder;
            this.weekView1.HightlightColor = System.Drawing.Color.Yellow;
            this.weekView1.HourLabelColor = System.Drawing.Color.DarkGray;
            this.weekView1.HourLabelFont = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.weekView1.HourLabelInterval = 6;
            this.weekView1.HourLabelPadding = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.weekView1.HourLineColor = System.Drawing.Color.DarkGray;
            this.weekView1.HourLineInterval = 6;
            this.weekView1.LargeTickHeight = 5;
            this.weekView1.MinimumSize = new System.Drawing.Size(350, 45);
            this.weekView1.Name = "weekView1";
            this.weekView1.SelectedItemHighlightType = XenAdmin.Controls.Wlb.WeekView.HighlightType.Box;
            this.weekView1.ShowCurrentTimeMark = true;
            this.weekView1.SmalltickHeight = 3;
            triggerPoints1.Selected = null;
            this.weekView1.TriggerPoints = triggerPoints1;
            this.weekView1.OnTriggerPointDoubleClick += new System.Windows.Forms.MouseEventHandler(this.weekView1_OnTriggerPointDoubleClick);
            this.weekView1.OnTriggerPointClick += new System.Windows.Forms.MouseEventHandler(this.weekView1_OnTriggerPointClick);
            // 
            // WlbOptModeScheduler
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panelScheduleList);
            this.Name = "WlbOptModeScheduler";
            this.panelScheduleList.ResumeLayout(false);
            this.panelScheduleList.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelScheduleList;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonAddNew;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label lableScheduledTaskBlurb;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView lvTaskList;
        private WeekView weekView1;
        private System.Windows.Forms.ColumnHeader columnHeaderHidden;
        private System.Windows.Forms.ColumnHeader columnHeaderMode;
        private System.Windows.Forms.ColumnHeader columnHeaderDay;
        private System.Windows.Forms.ColumnHeader columnHeaderTime;
        private System.Windows.Forms.ColumnHeader columnHeaderEnabled;
    }
}
