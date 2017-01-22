/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.Wlb;
using XenAdmin.Wlb;
using XenAPI;


namespace XenAdmin.Controls.Wlb
{
    public partial class WlbOptModeScheduler : UserControl
    {
        #region Color Constants

        private Color COLOR_MAX_PERFORMANCE = Color.White; // Color.FromArgb(30, 49, 191);
        private Color COLOR_MAX_DENSITY = Color.White; // Color.FromArgb(30, 191, 30);
        private Color COLOR_MAX_PERFORMANCE_SUBITEM_TEXT = Color.Black; //Color.White;
        private Color COLOR_MAX_DENSITY_SUBITEM_TEXT = Color.DarkGreen; //Color.White;
        private Color COLOR_HIGHLIGHTED_MAX_PERFORMANCE_SUBITEM_TEXT = Color.White; //Color.White;
        private Color COLOR_HIGHLIGHTED_MAX_DENSITY_SUBITEM_TEXT = Color.LightGreen; //Color.White;
        private Color COLOR_NEW_DAY = Color.LightBlue; // Color.Yellow;
        private Color COLOR_DISABLED_TASK = Color.Gray;
        private Color COLOR_DUPLICATE_TASK = Color.Red;

        #endregion
        
        #region Private Fields

        private WlbScheduledTasks _scheduledTasks;
        private int _newTaskId = 0;
        private WlbPoolPerformanceMode _baseMode = WlbPoolPerformanceMode.MaximizePerformance;
        private Pool _pool = null;
        private bool _hasChanged = false;

        private bool _initializing = false;

        // Listview minimum column widths
        private int[] minimumColumnWidths = { 0, 60, 60, 60, 60 };


        #endregion Private Fields

        #region Public CTor
        public WlbOptModeScheduler()
        {
            InitializeComponent();
        }
        #endregion Public CTor

        #region Public Properties
        public Pool Pool
        {
            set
            {
                _pool = value;
                InitializeControls();
            }
        }

        public WlbPoolPerformanceMode BaseMode
        {
            set
            {
                _baseMode = value;
                InitializeControls();
            }
        }

        public WlbScheduledTasks ScheduledTasks
        {
            get { return _scheduledTasks; }
            set
            {
                if (null != value)
                {
                    _scheduledTasks = new WlbScheduledTasks();
                    //Clone the tasks so local edits don't affect the base collection
                    foreach (string key in value.TaskList.Keys)
                    {
                        _scheduledTasks.TaskList.Add(key, value.TaskList[key]);
                    }
                    InitializeControls();
                }
            }
        }

        public bool HasChanged
        {
            get { return _hasChanged; }
        }

        #endregion Public Properties

        #region Public Methods

        public void RefreshScheduleList()
        {
            PopulateListView();
        }

        #endregion
        
        #region Private Methods
        private void SetUpListView()
        {
            lvTaskList.Columns[0].Width = 0;  // sorting column
            if (lvTaskList.Columns.Count > 1)
            {
                for (int iCol = 1; iCol < lvTaskList.Columns.Count; iCol++)
                {
                    lvTaskList.Columns[iCol].Width = lvTaskList.Width / (lvTaskList.Columns.Count - 1);
                }
            } 
            lvTaskList.ListViewItemSorter = new ListViewItemComparer(0);
        }

        private void InitializeControls()
        {
            typeof(WeekView).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, weekView1, new object[] { true });

            if (null != _pool && null != _scheduledTasks)
            {
                _initializing = true;
                buttonEdit.Enabled = false;
                buttonDelete.Enabled = false;

                SetUpListView();
                PopulateListView();
                //PopulateWeekView();
                _initializing = false;
            }
        }

        private void PopulateListView()
        {
            weekView1.ClearTriggerPoints();
            lvTaskList.Items.Clear();

            //foreach (int key in _scheduledTasks.SortedTaskList.Keys)
            foreach (int key in _scheduledTasks.VirtualTaskList.Keys)
            {
                //WlbScheduledTask task = _scheduledTasks.SortedTaskList[key];
                WlbScheduledTask task = _scheduledTasks.VirtualTaskList[key];
                WlbScheduledTask parentTask = _scheduledTasks.TaskList[_scheduledTasks.VirtualTaskList[key].TaskId.ToString()];

                DateTime localExecuteTime;
                WlbScheduledTask.WlbTaskDaysOfWeek localDaysOfWeek;
                WlbScheduledTask.GetLocalTaskTimes((task.DaysOfWeek), task.ExecuteTime, out localDaysOfWeek, out localExecuteTime);

                ListViewItem item = new ListViewItem();
                item.Text = key.ToString();
                item.SubItems.Add(GetTaskOptMode(task) == WlbPoolPerformanceMode.MaximizeDensity ? Messages.WLB_OPT_MODE_MAXIMIZEDENSITY : Messages.WLB_OPT_MODE_MAXIMIZEPERFORMANCE);
                item.SubItems.Add(GetTaskDayOfWeek(localDaysOfWeek, parentTask.DaysOfWeek));
                item.SubItems.Add(GetTaskExecuteTime(localExecuteTime));
                item.SubItems.Add(task.Enabled ? Messages.YES : Messages.NO);
                item.Tag = task;
                lvTaskList.Items.Add(item);
            //}

            //foreach (WlbScheduledTask task in _scheduledTasks.VirtualTaskList.Values)
            //{
                if (task.Enabled)
                {
                    //DateTime localExecuteTime;
                    //WlbScheduledTask.WlbTaskDaysOfWeek localDaysOfWeek;
                    WlbScheduledTask.GetLocalTaskTimes((task.DaysOfWeek), task.ExecuteTime, out localDaysOfWeek, out localExecuteTime);
                    string toolTipText = string.Format("Change to {0} mode at {1} on {2}", WlbScheduledTask.GetTaskOptMode(task), WlbScheduledTask.GetTaskExecuteTime(localExecuteTime), WlbScheduledTask.DaysOfWeekL10N(localDaysOfWeek));
                    
                    TriggerPoint triggerPoint = new TriggerPoint();
                    triggerPoint.Day = WlbScheduledTask.ConvertFromWlbTaskDayOfWeek(localDaysOfWeek);
                    triggerPoint.Hour = localExecuteTime.Hour;
                    triggerPoint.Color = GetTaskOptMode(task) == WlbPoolPerformanceMode.MaximizePerformance ? Color.Blue : Color.Green;
                    triggerPoint.ToolTip = toolTipText;
                    triggerPoint.Tag = task.TaskId;
                    weekView1.AddTriggerPoint(triggerPoint);
                }

            }

            lvTaskList.Sort();
            EnableButtons();
       }

        //private void AddTaskToList(WlbScheduledTask task)
        //{
        //    foreach (WlbScheduledTask.WlbTaskDaysOfWeek dayValue in Enum.GetValues(typeof(WlbScheduledTask.WlbTaskDaysOfWeek)))
        //    {
        //        if (dayValue != WlbScheduledTask.WlbTaskDaysOfWeek.None && 
        //            dayValue != WlbScheduledTask.WlbTaskDaysOfWeek.All &&
        //            dayValue != WlbScheduledTask.WlbTaskDaysOfWeek.Weekdays &&
        //            dayValue != WlbScheduledTask.WlbTaskDaysOfWeek.Weekends &&
        //            ((task.DaysOfWeek & dayValue) == dayValue))
        //        {
        //            DateTime localExecuteTime;
        //            WlbScheduledTask.WlbTaskDaysOfWeek localDaysOfWeek;
        //            WlbScheduledTask.GetLocalTaskTimes((task.DaysOfWeek & dayValue), task.ExecuteTime, out localDaysOfWeek, out localExecuteTime);

        //            int sortValue = (int)localDaysOfWeek * 10000 + (int)localExecuteTime.TimeOfDay.TotalMinutes;

        //            ListViewItem item = new ListViewItem();
        //            item.Text = sortValue.ToString();
        //            item.SubItems.Add(GetTaskOptMode(task) == WlbPoolPerformanceMode.MaximizeDensity ? Messages.WLB_OPT_MODE_MAXIMIZEDENSITY : Messages.WLB_OPT_MODE_MAXIMIZEPERFORMANCE);
        //            item.SubItems.Add(GetTaskDayOfWeek(localDaysOfWeek));
        //            item.SubItems.Add(GetTaskExecuteTime(localExecuteTime));
        //            item.SubItems.Add(task.Enabled ? Messages.YES : Messages.NO);
        //            item.Tag = task;
        //            lvTaskList.Items.Add(item);
        //        }
        //    }
        //}

        private void FixColumnWidths()
        {
            //Make sure the last column always spans the width of the control
            int otherColumnWidths = 0;
            if (lvTaskList.Columns.Count > 0)
            {
                // skip sorting column (index 0)
                for (int i = 1; i < (lvTaskList.Columns.Count - 1); i++)
                {
                    otherColumnWidths += lvTaskList.Columns[i].Width;
                }
                if (otherColumnWidths < lvTaskList.Width)
                {
                    lvTaskList.ColumnWidthChanged -= new System.Windows.Forms.ColumnWidthChangedEventHandler(lvTaskList_ColumnWidthChanged);
                    lvTaskList.Columns[lvTaskList.Columns.Count - 1].Width = lvTaskList.Width - otherColumnWidths;
                    lvTaskList.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(lvTaskList_ColumnWidthChanged);
                }
            }
        }

        private void AddTask()
        {
            WlbEditScheduledTask addTask = new WlbEditScheduledTask(_newTaskId--, WlbScheduledTask.WlbTaskActionType.SetOptimizationMode);
            DialogResult dr = addTask.ShowDialog();
            if (DialogResult.OK == dr)
            {
                WlbScheduledTask newTask = addTask.Task;
                newTask.Owner = _pool.Connection.Username;
                newTask.LastTouchedBy = _pool.Connection.Username;
                newTask.AddTaskParameter("PoolUUID", _pool.uuid);

                WlbScheduledTask checkTask = CheckForDuplicateTask(newTask);
                if (null != checkTask)
                {
                    using (var dlg = new ThreeButtonDialog(
                       new ThreeButtonDialog.Details(
                           SystemIcons.Warning,
                           Messages.WLB_TASK_SCHEDULE_CONFLICT_BLURB,
                           Messages.WLB_TASK_SCHEDULE_CONFLICT_TITLE)))
                    {
                        dlg.ShowDialog(this);
                    }
                    SelectTask(checkTask.TaskId);
                }
                else
                {
                    _scheduledTasks.TaskList.Add(newTask.TaskId.ToString(), newTask);
                    PopulateListView();
                    _hasChanged = true;
                }
            }
        }

        private void EditTask(WlbScheduledTask task)
        {
            WlbScheduledTask editTask = task.Clone();
            WlbEditScheduledTask taskEditor = new WlbEditScheduledTask(editTask);
            DialogResult dr = taskEditor.ShowDialog();
            if (DialogResult.OK == dr)
            {
                WlbScheduledTask checkTask = CheckForDuplicateTask(editTask);
                if (null != checkTask)
                {
                    using (var dlg = new ThreeButtonDialog(
                       new ThreeButtonDialog.Details(
                           SystemIcons.Warning,
                           Messages.WLB_TASK_SCHEDULE_CONFLICT_BLURB,
                           Messages.WLB_TASK_SCHEDULE_CONFLICT_TITLE)))
                    {
                        dlg.ShowDialog(this);
                    }
                    SelectTask(checkTask.TaskId);
                }
                else
                {
                    editTask.LastTouchedBy = _pool.Connection.Username;
                    editTask.LastTouched = DateTime.UtcNow;
                    _scheduledTasks.TaskList[editTask.TaskId.ToString()] = editTask;
                    PopulateListView();
                    _hasChanged = true;
                }
            }
        }

        private WlbScheduledTask CheckForDuplicateTask(WlbScheduledTask newTask)
        {
            WlbScheduledTask checkTask;
            foreach (string key in _scheduledTasks.TaskList.Keys)
            {
                checkTask = _scheduledTasks.TaskList[key];
                if (checkTask.Enabled && !checkTask.DeleteTask)
                {
                    if ((checkTask.ActionType == newTask.ActionType) &&
                        ((checkTask.DaysOfWeek & newTask.DaysOfWeek) == checkTask.DaysOfWeek ||
                        (checkTask.DaysOfWeek & newTask.DaysOfWeek) == newTask.DaysOfWeek) &&
                        (string.Compare(HelpersGUI.DateTimeToString(checkTask.ExecuteTime, "t", false), HelpersGUI.DateTimeToString(newTask.ExecuteTime, "t", false)) == 0) &&
                        (checkTask.TaskId != newTask.TaskId))
                    {
                        return checkTask; // taskExists = true;
                    }
                }
            }
            return null;
        }

       private void DeleteTask(WlbScheduledTask task)
        {
            // if this is a new, unsaved task, simply delete it from the collection
            if (task.TaskId <= 0)
            {
                _scheduledTasks.TaskList.Remove(task.TaskId.ToString());
            }
            // otherwise, leave it in the collection but mark it for deletion
            else
            {
                task.DeleteTask = true;
            }
            PopulateListView();
            _hasChanged = true;
        }

       private void EnableTask(WlbScheduledTask task)
       {
           //If we are trying to enable the task, check to see if it is a duplicate before allowing it
           if (!task.Enabled)
           {
               //We need to pretend the task is enabled to check for duplicates
               WlbScheduledTask enabledTask = task.Clone();
               enabledTask.Enabled = true;
               WlbScheduledTask checkTask = CheckForDuplicateTask(enabledTask);
               //if it's a duplicate task, display warning and return
               if (null != checkTask)
               {
                   using (var dlg = new ThreeButtonDialog(
                       new ThreeButtonDialog.Details(
                           SystemIcons.Warning,
                           Messages.WLB_TASK_SCHEDULE_CONFLICT_BLURB,
                           Messages.WLB_TASK_SCHEDULE_CONFLICT_TITLE)))
                   {
                       dlg.ShowDialog(this);
                   }
                   SelectTask(checkTask.TaskId);
                   return;
               }
           }           
           task.Enabled = !task.Enabled;
           PopulateListView();
           _hasChanged = true;         

       }

        private void EnableButtons()
        {
            // If an item is selected, enable the edit and delete buttons
            buttonEdit.Enabled = (lvTaskList.SelectedItems != null && lvTaskList.SelectedItems.Count > 0);
            buttonDelete.Enabled = (lvTaskList.SelectedItems != null && lvTaskList.SelectedItems.Count > 0);
        }

        private void HighlightTrigger()
        {
            weekView1.TriggerPoints.ClearSelected();

            if (lvTaskList.SelectedItems.Count > 0)
            {
                int selectedTaskId = ((WlbScheduledTask)lvTaskList.SelectedItems[0].Tag).TaskId;
                TriggerPoints triggerPoints =  weekView1.TriggerPoints.FindByTag(selectedTaskId);
                if (null != triggerPoints)
                {
                    foreach (TriggerPoint triggerPoint in triggerPoints.List.Values)
                    {
                        triggerPoint.IsSelected = true;
                    }
                }
                weekView1.Refresh();
            }
        }

        private void SelectTask(int taskId)
        {
            foreach (ListViewItem listViewItem in lvTaskList.Items)
            {
                if (((WlbScheduledTask)listViewItem.Tag).TaskId == taskId)
                {
                    listViewItem.Selected = true;
                }
                else
                {
                    listViewItem.Selected = false;
                }
            }
        }

        private void EnableTask_Click(object sender, EventArgs e)
        {
            EnableTask(TaskFromItem(lvTaskList.SelectedItems[0]));
        }

        #endregion Private Methods

        #region Private Static Methods
        private static WlbPoolPerformanceMode GetTaskOptMode(WlbScheduledTask task)
        {
            WlbPoolPerformanceMode mode = WlbPoolPerformanceMode.MaximizePerformance;

            if (task.TaskParameters["OptMode"] == "0")
            {
                mode = WlbPoolPerformanceMode.MaximizePerformance;
            }
            else
            {
                mode = WlbPoolPerformanceMode.MaximizeDensity;
            }
            return mode;
        }

        /// <summary>
        /// Returns the number of bit set in an integer
        /// </summary>
        public static uint Bitcount(int n)
        {
            uint count = 0;
            while (n != 0)
            {
                count++;
                n &= (n - 1);
            }
            return count;
        }


        private static string GetTaskDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek taskDaysOfWeek)
        {
            return WlbScheduledTask.DaysOfWeekL10N(taskDaysOfWeek);
        }

        private static string GetTaskDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek taskDaysOfWeek,
            WlbScheduledTask.WlbTaskDaysOfWeek taskDaysofWeekSortedList)
        {
            string returnStr = "";
            returnStr += WlbScheduledTask.DaysOfWeekL10N(taskDaysOfWeek);

            //count the bits set in days of week.
            //this workaround had to be made to determine whether the original task was set for
            //weekends/weekdays/alldays
            uint bitCount = Bitcount((int)taskDaysofWeekSortedList);
            if (bitCount == 2)
            {
                returnStr += " (" + Messages.ResourceManager.GetString("WLB_DAY_WEEKENDS") + ")";
            }
            else if (bitCount == 5)
            {
                returnStr += " (" + Messages.ResourceManager.GetString("WLB_DAY_WEEKDAYS") + ")";
            }
            else if (bitCount == 7)
            {
                returnStr += " (" + Messages.ResourceManager.GetString("WLB_DAY_ALL") + ")";
            }
            
            return returnStr;
        }

        private static string GetTaskExecuteTime(DateTime TaskExecuteTime)
        {
            return HelpersGUI.DateTimeToString(TaskExecuteTime, Messages.DATEFORMAT_HM, true);
        }


        #endregion Private Static Methods

        #region ListView Custom Rendering Event Handlers
 
        private void lvTaskList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // Draw default column header
            e.DrawDefault = true;
        }

        private WlbScheduledTask TaskFromItem(ListViewItem item)
        {
            return _scheduledTasks.TaskList[((WlbScheduledTask)item.Tag).TaskId.ToString()];
        }

        private void lvTaskList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            //Get the task associated with the list view item
            WlbScheduledTask task = TaskFromItem(e.Item);

            //Set the row color based on the Opt Mode
            Color color = GetTaskOptMode(task) == WlbPoolPerformanceMode.MaximizePerformance ? COLOR_MAX_PERFORMANCE : COLOR_MAX_DENSITY;

            if (!task.Enabled)
            {
                color = COLOR_DISABLED_TASK;
            }

            // Fill the full width of the control, not just to the end of the column headers.
            Rectangle rect = e.Bounds;
            int i = 0;
            foreach (ColumnHeader col in lvTaskList.Columns)
            {
                i += col.Width;
            }
            // Fill from the right of the rightmost item to the end of the control
            rect.Width = Math.Max(i, lvTaskList.ClientSize.Width);
            rect.X = i;

            if (e.Item.Selected)
            {
                using (SolidBrush brush = new SolidBrush(SystemColors.Highlight))
                    e.Graphics.FillRectangle(brush, rect);
            }
            else
            {
                Rectangle topRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 2);
                Rectangle botRect = new Rectangle(rect.X, rect.Y + topRect.Height, rect.Width, rect.Height / 2);
                if (e.ItemIndex == 0 || (e.ItemIndex > 0 && lvTaskList.Items[e.ItemIndex - 1].SubItems[2].Text != e.Item.SubItems[2].Text))
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(topRect, COLOR_NEW_DAY, ControlPaint.LightLight(color), 90f)) // SystemBrushes.Highlight;
                    {
                        e.Graphics.FillRectangle(brush, topRect);
                    }
                }
                else
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(topRect, color, ControlPaint.LightLight(color), 90f)) // SystemBrushes.Highlight;
                    {
                        e.Graphics.FillRectangle(brush, topRect);
                    }
                }
                using (LinearGradientBrush brush = new LinearGradientBrush(botRect, ControlPaint.LightLight(color), color, 90f)) // SystemBrushes.Highlight;
                {
                    e.Graphics.FillRectangle(brush, botRect);
                }
            }
        }

        private void lvTaskList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            //Get the task associated with the list view item
            WlbScheduledTask task = TaskFromItem(e.Item);

            //Set the row color based on the Opt Mode
            Color color = GetTaskOptMode(task) == WlbPoolPerformanceMode.MaximizePerformance ? COLOR_MAX_PERFORMANCE : COLOR_MAX_DENSITY;
            Color subItemTextColor = GetTaskOptMode(task) == WlbPoolPerformanceMode.MaximizePerformance ? COLOR_MAX_PERFORMANCE_SUBITEM_TEXT : COLOR_MAX_DENSITY_SUBITEM_TEXT;

            if (!task.Enabled)
            {
                subItemTextColor = COLOR_DISABLED_TASK;
            }
            
            //Get the base rectangle
            Rectangle rect = e.Bounds;

            if (e.Item.Selected)
            {
                using (SolidBrush brush = new SolidBrush(SystemColors.Highlight))
                    e.Graphics.FillRectangle(brush, rect);
                subItemTextColor = GetTaskOptMode(task) == WlbPoolPerformanceMode.MaximizePerformance ? COLOR_HIGHLIGHTED_MAX_PERFORMANCE_SUBITEM_TEXT : COLOR_HIGHLIGHTED_MAX_DENSITY_SUBITEM_TEXT;
            }
            else
            {
                Rectangle topRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 2);
                Rectangle botRect = new Rectangle(rect.X, rect.Y + topRect.Height, rect.Width, rect.Height / 2);
                if (e.ItemIndex ==0 || (e.ItemIndex> 0 && lvTaskList.Items[e.ItemIndex - 1].SubItems[2].Text != e.Item.SubItems[2].Text))
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(topRect, COLOR_NEW_DAY, ControlPaint.LightLight(color), 90f)) // SystemBrushes.Highlight;
                    {
                        e.Graphics.FillRectangle(brush, topRect);
                    }
                }
                else
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(topRect, color, ControlPaint.LightLight(color), 90f)) // SystemBrushes.Highlight;
                    {
                        e.Graphics.FillRectangle(brush, topRect);
                    }
                }
                using (LinearGradientBrush brush = new LinearGradientBrush(botRect, ControlPaint.LightLight(color), color, 90f)) // SystemBrushes.Highlight;
                {
                    e.Graphics.FillRectangle(brush, botRect);
                }
            }

            // Draw subitem text
            Font font = new Font(this.Font, FontStyle.Regular); // FontStyle.Bold);
            TextRenderer.DrawText(e.Graphics, e.Item.SubItems[e.ColumnIndex].Text, font, e.Bounds, subItemTextColor, TextFormatFlags.EndEllipsis);
        }

        #endregion 

        #region Control Event Handlers

        private void buttonAddNew_Click(object sender, EventArgs e)
        {
            AddTask();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            WlbScheduledTask task = TaskFromItem(lvTaskList.SelectedItems[0]);
            EditTask(task);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (lvTaskList.SelectedItems.Count > 0)
            {
                DialogResult confirmResult;
                using (var dlg = new ThreeButtonDialog(
                                                new ThreeButtonDialog.Details(SystemIcons.Warning,
                                                                                Messages.DELETE_WLB_OPTIMIZATION_SCHEDULE_WARNING,
                                                                                Messages.DELETE_WLB_OPTIMIZATION_SCHEDULE_CAPTION),
                                                ThreeButtonDialog.ButtonYes,
                                                ThreeButtonDialog.ButtonNo))
                {
                    confirmResult = dlg.ShowDialog(this);
                }

                if (confirmResult == DialogResult.Yes)
                {
                    WlbScheduledTask task = TaskFromItem(lvTaskList.SelectedItems[0]);
                    DeleteTask(task);
                    weekView1.Refresh();
                }
            }
        }

        private void lvTaskList_SelectedIndexChanged(object sender, EventArgs e)
        {            
            EnableButtons();
            HighlightTrigger();            
        }

        private void lvTaskList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Edit task on double-click
            ListViewHitTestInfo htInfo = lvTaskList.HitTest(e.Location);
            if (null != htInfo.Item)
            {
                WlbScheduledTask task = TaskFromItem(htInfo.Item);
                EditTask(task);
            }
        }

        private void lvTaskList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (!_initializing)
            {
                if (e.ColumnIndex < minimumColumnWidths.Length)
                {
                    if (lvTaskList.Columns[e.ColumnIndex].Width < minimumColumnWidths[e.ColumnIndex])
                    {
                        lvTaskList.Columns[e.ColumnIndex].Width = minimumColumnWidths[e.ColumnIndex];
                    }
                }
                FixColumnWidths();
            }
        }

        private void lvTaskList_Resize(object sender, EventArgs e)
        {
            if (!_initializing)
            {
                FixColumnWidths();
            }
        }       

        private void lvTaskList_MouseDown(object sender, MouseEventArgs e)
        {
            //Check to see if this is a right-click event
            if (e.Button == MouseButtons.Right)
            {
                //determine which item was clicked
                ListViewHitTestInfo htInfo = lvTaskList.HitTest(e.Location);
                if (null != htInfo.Item)
                {
                    //make sure the right-clicked item is selected
                    // (right-clicking does not always select the item, which makes 
                    // usage a little clunky)
                    htInfo.Item.Selected = true;
                    //create the context menu and figure out which item(s) to show
                    MenuItem enableTask;
                    WlbScheduledTask task = TaskFromItem(htInfo.Item);                    
                    
                    ContextMenu menu = new ContextMenu();
                    if (task.Enabled)
                    {
                        enableTask = new MenuItem(Messages.DISABLE, EnableTask_Click);

                    }
                    else
                    {
                        enableTask = new MenuItem(Messages.ENABLE, EnableTask_Click);
                    }
                    menu.MenuItems.Add(enableTask);
                    menu.Show(lvTaskList, e.Location);
                }
            }           
            
        }

        private void lvTaskList_EnabledChanged(object sender, EventArgs e)
        {
            if (lvTaskList.Enabled)
            {
                InitializeControls(); 
            }
            else
            {
                lvTaskList.Items.Clear();
            }
        }

        private void weekView1_OnTriggerPointClick(object sender, MouseEventArgs e)
        {
            if (weekView1.SelectedTriggerPoint != null)
            {
                SelectTask((int)weekView1.SelectedTriggerPoint.Tag);
            }
        }

        private void weekView1_OnTriggerPointDoubleClick(object sender, MouseEventArgs e)
        {
            if (weekView1.SelectedTriggerPoint != null)
            {
                WlbScheduledTask task = _scheduledTasks.TaskList[weekView1.SelectedTriggerPoint.Tag.ToString()];
                if (null != task)
                {
                    EditTask(task);
                }
            }
        }

        #endregion

        #region ListView Sorting

        private class ListViewItemComparer : System.Collections.IComparer
        {
            private int col;
            public ListViewItemComparer()
            {
                col = 0;
            }
            public ListViewItemComparer(int column)
            {
                col = column;
            }
            public int Compare(object x, object y)
            {
                return CompareDouble(double.Parse(((ListViewItem)x).Text), double.Parse(((ListViewItem)y).Text));
                //WlbScheduledTask taskX = (WlbScheduledTask)((ListViewItem)x).Tag;
                //WlbScheduledTask taskY = (WlbScheduledTask)((ListViewItem)y).Tag;

                //int returnVal = -1;
                //returnVal = CompareInt((int)taskX.DaysOfWeek, (int)taskY.DaysOfWeek);

                //if (returnVal == 0)
                //{
                //    returnVal = CompareDouble(taskX.ExecuteTime.TimeOfDay.TotalMinutes, taskY.ExecuteTime.TimeOfDay.TotalMinutes);
                //}
                //return returnVal;
            }
            
            private int CompareDouble(double x, double y)
            {
                if (x < y)
                {
                    return -1;
                }
                else
                {
                    if (x == y)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            private int CompareInt(int x, int y)
            {
                if (x < y)
                {
                    return -1;
                }
                else
                {
                    if (x == y)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
        }
#endregion

        //private void lvTaskList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        //{
        //    if(e.IsSelected)
        //        SelectTask(TaskFromItem(e.Item).TaskId);            
        //}
        
    }
}
