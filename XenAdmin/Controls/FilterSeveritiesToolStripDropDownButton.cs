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
using XenAdmin.Alerts;

namespace XenAdmin.Controls
{
    class FilterSeveritiesToolStripDropDownButton : ToolStripDropDownButton
    {
        [Browsable(true)]
        public event Action FilterChanged;

        private bool internalUpdating;

        private ToolStripMenuItem toolStripMenuItem0;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem toolStripMenuItemAll;

        public FilterSeveritiesToolStripDropDownButton()
        {
            toolStripMenuItem0 = new ToolStripMenuItem
                                     {
                                         Text = Messages.SEVERITY_FILTER_0,
                                         Checked = true,
                                         CheckOnClick = true,
                                         Image = Properties.Resources.alert6_16,
                                         ToolTipText = Messages.SEVERITY_FILTER_0_DETAIL
                                     };
            toolStripMenuItem1 = new ToolStripMenuItem
                                     {
                                         Text = Messages.SEVERITY_FILTER_1,
                                         Checked = true,
                                         CheckOnClick = true,
                                         Image = Properties.Resources.alert1_16,
                                         ToolTipText = Messages.SEVERITY_FILTER_1_DETAIL
                                     };
            toolStripMenuItem2 = new ToolStripMenuItem
                                     {
                                         Text = Messages.SEVERITY_FILTER_2,
                                         Checked = true,
                                         CheckOnClick = true,
                                         Image = Properties.Resources.alert2_16,
                                         ToolTipText = Messages.SEVERITY_FILTER_2_DETAIL
                                     };
            toolStripMenuItem3 = new ToolStripMenuItem
                                     {
                                         Text = Messages.SEVERITY_FILTER_3,
                                         Checked = true,
                                         CheckOnClick = true,
                                         Image = Properties.Resources.alert3_16,
                                         ToolTipText = Messages.SEVERITY_FILTER_3_DETAIL
                                     };
            toolStripMenuItem4 = new ToolStripMenuItem
                                     {
                                         Text = Messages.SEVERITY_FILTER_4,
                                         Checked = true,
                                         CheckOnClick = true,
                                         Image = Properties.Resources.alert4_16,
                                         ToolTipText = Messages.SEVERITY_FILTER_4_DETAIL
                                     };
            toolStripMenuItem5 = new ToolStripMenuItem
                                     {
                                         Text = Messages.SEVERITY_FILTER_5,
                                         Checked = true,
                                         CheckOnClick = true,
                                         Image = Properties.Resources.alert5_16,
                                         ToolTipText = Messages.SEVERITY_FILTER_5_DETAIL
                                     };
            toolStripMenuItemAll = new ToolStripMenuItem
                {
                    Text = Messages.FILTER_SHOW_ALL,
                    Enabled = false
                };
            DropDownItems.AddRange(new ToolStripItem[]
                                       {
                                           toolStripMenuItem1,
                                           toolStripMenuItem2,
                                           toolStripMenuItem3,
                                           toolStripMenuItem4,
                                           toolStripMenuItem5,
                                           toolStripMenuItem0,
                                           new ToolStripSeparator(),
                                           toolStripMenuItemAll
                                       });

            toolStripMenuItem0.CheckedChanged += Item_CheckedChanged;
            toolStripMenuItem1.CheckedChanged += Item_CheckedChanged;
            toolStripMenuItem2.CheckedChanged += Item_CheckedChanged;
            toolStripMenuItem3.CheckedChanged += Item_CheckedChanged;
            toolStripMenuItem4.CheckedChanged += Item_CheckedChanged;
            toolStripMenuItem5.CheckedChanged += Item_CheckedChanged;
        }

        public bool FilterIsOn
        {
            get
            {
                return !toolStripMenuItem0.Checked
                    || !toolStripMenuItem1.Checked
                    || !toolStripMenuItem2.Checked
                    || !toolStripMenuItem3.Checked
                    || !toolStripMenuItem4.Checked
                    || !toolStripMenuItem5.Checked;
            }
        }

        public bool HideBySeverity(AlertPriority priority)
        {
            return !((toolStripMenuItem1.Checked && priority == AlertPriority.Priority1)
                     || (toolStripMenuItem2.Checked && priority == AlertPriority.Priority2)
                     || (toolStripMenuItem3.Checked && priority == AlertPriority.Priority3)
                     || (toolStripMenuItem4.Checked && priority == AlertPriority.Priority4)
                     || (toolStripMenuItem5.Checked && priority == AlertPriority.Priority5)
                     || toolStripMenuItem0.Checked && priority == AlertPriority.Unknown);
        }

        protected override void OnDropDownItemClicked(ToolStripItemClickedEventArgs e)
        {
            base.OnDropDownItemClicked(e);

            if (e.ClickedItem == toolStripMenuItemAll)
            {
                internalUpdating = true;
                toolStripMenuItem0.Checked = true;
                toolStripMenuItem1.Checked = true;
                toolStripMenuItem2.Checked = true;
                toolStripMenuItem3.Checked = true;
                toolStripMenuItem4.Checked = true;
                toolStripMenuItem5.Checked = true;
                internalUpdating = false;
                
                Item_CheckedChanged(null, null);
            }
        }

        private void Item_CheckedChanged(object sender, EventArgs e)
        {
            if (!internalUpdating)
            {
                toolStripMenuItemAll.Enabled = FilterIsOn;

                if (FilterChanged != null)
                    FilterChanged();
            }
        }
    }
}
