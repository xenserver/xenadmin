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
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeReadyToUpgradePage : XenTabPage
    {
        public RollingUpgradeReadyToUpgradePage()
        {
            InitializeComponent();
            listBox.DrawItem += new DrawItemEventHandler(listBox_DrawItem);
        }

        void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox lbox = sender as ListBox;
            var item = lbox.Items[e.Index];

            using (Brush brush = new SolidBrush(SystemColors.WindowText))
            {
                Host host = item as Host;
                if (host != null)
                {
                    e.Graphics.DrawString(string.Format(host.IsMaster()
                                                            ? Messages.UPGRADE_POOL_MASTER_X
                                                            : Messages.UPGRADE_SERVER_X, host.Name.Ellipsise(64)), Program.DefaultFont, brush, e.Bounds);
                    return;
                }

                Pool pool = item as Pool;
                if (pool != null)
                {
                    e.Graphics.DrawString(string.Format(Messages.POOL_X_READYUPGRADE, pool.Name.Ellipsise(64)), Program.DefaultFontBold, brush, e.Bounds);
                    return;
                }

                e.Graphics.DrawString(item.ToString(), Program.DefaultFont, brush, e.Bounds);
            }
        }

        public override string Text
        {
            get
            {
                return Messages.READY_UPGRADE;
            }
        }

        public override string HelpID
        {
            get { return "Readyupgrade"; }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.PERFORM_ROLLING_UPGRADE_INTERACTIVE_MODE;
            }
        }

        public override string NextText(bool isLastPage)
        {
            return Messages.START_UPGRADE;
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            listBox.Items.Clear();
            foreach (var master in SelectedMasters)
            {
                Pool pool = Helpers.GetPoolOfOne(master.Connection);
                if (pool != null)
                {
                    listBox.Items.Add(pool);
                    foreach (var host in pool.HostsToUpgrade)
                    {
                        listBox.Items.Add(host);
                    }
                }
            }
            listBox.Items.Add(Messages.REVERT_POOL_STATE);
        }

        public IEnumerable<Host> SelectedMasters { private get; set; }
    }
}
