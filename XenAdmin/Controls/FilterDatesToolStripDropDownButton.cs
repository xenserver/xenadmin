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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Dialogs;

namespace XenAdmin.Controls
{
    class FilterDatesToolStripDropDownButton : ToolStripDropDownButton
    {
        [Browsable(true)]
        public event Action FilterChanged;

        private ToolStripMenuItem toolStripMenuItemShowAll;
        private ToolStripMenuItem toolStripMenuItemLast24Hours;
        private ToolStripMenuItem toolStripMenuItemLast3Days;
        private ToolStripMenuItem toolStripMenuItemLast7Days;
        private ToolStripMenuItem toolStripMenuItemLast30Days;
        private ToolStripMenuItem toolStripMenuItemCustomDate;
        private DateFilterDialog dateFilterDialog;

        public FilterDatesToolStripDropDownButton()
        {
            toolStripMenuItemShowAll = new ToolStripMenuItem {Checked = true, Text = Messages.FILTER_SHOW_ALL};
            toolStripMenuItemLast24Hours = new ToolStripMenuItem {Text = Messages.DATE_FILTER_LAST_24_HOURS};
            toolStripMenuItemLast3Days = new ToolStripMenuItem {Text = Messages.DATE_FILTER_LAST_3_DAYS};
            toolStripMenuItemLast7Days = new ToolStripMenuItem {Text = Messages.DATE_FILTER_LAST_7_DAYS};
            toolStripMenuItemLast30Days = new ToolStripMenuItem {Text = Messages.DATE_FILTER_LAST_30_DAYS};
            toolStripMenuItemCustomDate = new ToolStripMenuItem {Text = Messages.DATE_FILTER_CUSTOM};

            DropDownItems.AddRange(new ToolStripItem[]
                                       {
                                           toolStripMenuItemLast24Hours,
                                           toolStripMenuItemLast3Days,
                                           toolStripMenuItemLast7Days,
                                           toolStripMenuItemLast30Days,
                                           toolStripMenuItemCustomDate,
                                           new ToolStripSeparator(),
                                           toolStripMenuItemShowAll
                                       });

            dateFilterDialog = new DateFilterDialog();
        }

        /// <summary>
        /// Filter returns true if the alert is dated from before the minimum
        /// datetime filter, or after the maximum datetime filter
        /// </summary>
        public bool HideByDate(DateTime time)
        {
            if (dateFilterDialog.StartDateSet && dateFilterDialog.StartDate > time)
                return true;

            if (dateFilterDialog.EndDateSet && dateFilterDialog.EndDate < time)
                return true;

            return false;
        }
        
        public bool FilterIsOn
        {
            get { return !toolStripMenuItemShowAll.Checked; }
        }        

        protected override void OnDropDownItemClicked(ToolStripItemClickedEventArgs e)
        {
            base.OnDropDownItemClicked(e);

            var menuItem = (ToolStripMenuItem)e.ClickedItem;
            
            //we do not allow unchecking by clicking an already checked item
            if (menuItem.Checked)
                return;

            switch (DropDownItems.IndexOf(menuItem))
            {
                case 0:
                    dateFilterDialog.Set24Hours();
                    break;
                case 1:
                    dateFilterDialog.SetDays(DateFilterDialog.DaysInPastOptions.THREE_DAYS);
                    break;
                case 2:
                    dateFilterDialog.SetDays(DateFilterDialog.DaysInPastOptions.SEVEN_DAYS);
                    break;
                case 3:
                    dateFilterDialog.SetDays(DateFilterDialog.DaysInPastOptions.THIRTY_DAYS);
                    break;
                case 4:
                    DialogResult result = dateFilterDialog.ShowDialog();
                    if (result != DialogResult.OK)
                        return;
                    break;
                case 6:
                    dateFilterDialog.SetNone();
                    break;
            }

            foreach (ToolStripItem t in DropDownItems)
            {
                var mt = t as ToolStripMenuItem;
                if (mt != null)
                    mt.Checked = false;
            }

            menuItem.Checked = true;

            if (FilterChanged != null)
                FilterChanged();
        }
    }
}
