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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using XenAdmin.Controls;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Properties;
using System.Threading;
using System.Drawing;
using System.Drawing.Design;
using System.Collections.ObjectModel;
using XenAdmin.Network;


namespace XenAdmin.Commands
{
    /// <summary>
    /// This is the base ToolStripMenuItem for StartVMOnHostToolStripMenuItem, ResumeVMOnHostToolStripMenuItem and MigrateVMToolStripMenuItem.
    /// </summary>
    internal abstract class VMOperationToolStripMenuItem : CommandToolStripMenuItem
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly vm_operations _operation;

        private readonly bool _resumeAfter;

        protected VMOperationToolStripMenuItem(Command command, bool inContextMenu, vm_operations operation)
            : base(command, inContextMenu)
        {
            if (operation != vm_operations.start_on && operation != vm_operations.resume_on && operation != vm_operations.pool_migrate)
            {
                throw new ArgumentException("Invalid operation", "operation");
            }

            if (operation.Equals(vm_operations.resume_on))
                _resumeAfter = true;

            _operation = operation;
            base.DropDownItems.Add(new ToolStripMenuItem());
        }

        protected override void OnDropDownOpening(EventArgs e)
        {
            base.DropDownItems.Clear();

            // Work around bug in tool kit where disabled menu items show their dropdown menus
            if (!Enabled)
            {
                ToolStripMenuItem emptyMenuItem = new ToolStripMenuItem(Messages.HOST_MENU_EMPTY);
                emptyMenuItem.Font = Program.DefaultFont;
                emptyMenuItem.Enabled = false;
                base.DropDownItems.Add(emptyMenuItem);
                return;
            }

            VisualMenuItemAlignData.ParentStrip = this;
            IXenConnection connection = Command.GetSelection()[0].Connection;
            bool wlb = Helpers.WlbEnabled(connection);

            if (wlb)
            {
                base.DropDownItems.Add(new VMOperationToolStripMenuSubItem(Messages.WLB_OPT_MENU_OPTIMAL_SERVER, Images.StaticImages._000_ServerWlb_h32bit_16));
            }
            else
            {
                base.DropDownItems.Add(new VMOperationToolStripMenuSubItem(Messages.HOME_SERVER_MENU_ITEM, Images.StaticImages._000_ServerHome_h32bit_16));
            }

            List<Host> hosts = new List<Host>(connection.Cache.Hosts);
            hosts.Sort();
            foreach (Host host in hosts)
            {
                VMOperationToolStripMenuSubItem item = new VMOperationToolStripMenuSubItem(String.Format(Messages.MAINWINDOW_CONTEXT_UPDATING, host.name_label.EscapeAmpersands()), Images.StaticImages._000_ServerDisconnected_h32bit_16);
                item.Tag = host;
                base.DropDownItems.Add(item);
            }

            // start a new thread to evaluate which hosts can be used.
            ThreadPool.QueueUserWorkItem(delegate
            {
                SelectedItemCollection selection = Command.GetSelection();
                Session session = selection[0].Connection.DuplicateSession();
                WlbRecommendations recommendations = new WlbRecommendations(selection.AsXenObjects<VM>(), session);
                recommendations.Initialize();

                if (recommendations.IsError)
                {
                    EnableAppropriateHostsNoWlb(session);
                }
                else
                {
                    EnableAppropriateHostsWlb(session, recommendations);
                }
            });
        }

        private void EnableAppropriateHostsWlb(Session session, WlbRecommendations recommendations)
        {
            SelectedItemCollection selection = Command.GetSelection();

            // set the first menu item to be the WLB optimal server menu item
            Program.Invoke(Program.MainWindow, delegate
            {
                VMOperationToolStripMenuSubItem firstItem = (VMOperationToolStripMenuSubItem)base.DropDownItems[0];
                firstItem.Command = new VMOperationWlbOptimalServerCommand(Command.MainWindowCommandInterface, selection, _operation, recommendations);
            });

            List<VMOperationToolStripMenuSubItem> hostMenuItems = new List<VMOperationToolStripMenuSubItem>();
            Program.Invoke(Program.MainWindow, delegate
            {
                foreach (VMOperationToolStripMenuSubItem item in base.DropDownItems)
                {
                    Host host = item.Tag as Host;
                    if (host != null)
                    {
                        item.Command = new VMOperationWlbHostCommand(Command.MainWindowCommandInterface, selection, host, _operation, recommendations.GetStarRating(host));
                        hostMenuItems.Add(item);
                    }
                }
            });

            // Shuffle the list to make it look cool
            Helpers.ShuffleList(hostMenuItems);

            // sort the hostMenuItems by star rating
            hostMenuItems.Sort(new WlbHostStarCompare());

            // refresh the drop-down-items from the menuItems.
            Program.Invoke(Program.MainWindow, delegate()
            {
                foreach (VMOperationToolStripMenuSubItem menuItem in hostMenuItems)
                {
                    base.DropDownItems.Insert(hostMenuItems.IndexOf(menuItem) + 1, menuItem);
                }
            });

            Program.Invoke(Program.MainWindow, () => AddAdditionalMenuItems(selection));
        }

        private void EnableAppropriateHostsNoWlb(Session session)
        {
            SelectedItemCollection selection = Command.GetSelection();
            IXenConnection connection = selection[0].Connection;

            VMOperationCommand cmdHome = new VMOperationHomeServerCommand(Command.MainWindowCommandInterface, selection, _operation, session);
            
            Host affinityHost = connection.Resolve(((VM)Command.GetSelection()[0].XenObject).affinity);
            VMOperationCommand cpmCmdHome = new CrossPoolMigrateToHomeCommand(Command.MainWindowCommandInterface, selection, affinityHost);

            Program.Invoke(Program.MainWindow, delegate
            {
                var firstItem = (VMOperationToolStripMenuSubItem)base.DropDownItems[0];

                bool oldMigrateToHomeCmdCanRun = cmdHome.CanExecute();
                if (affinityHost == null || _operation == vm_operations.start_on || !oldMigrateToHomeCmdCanRun && !cpmCmdHome.CanExecute())
                    firstItem.Command = cmdHome;
                else
                    firstItem.Command = oldMigrateToHomeCmdCanRun ? cmdHome : cpmCmdHome;
            });

            List<VMOperationToolStripMenuSubItem> dropDownItems = DropDownItems.Cast<VMOperationToolStripMenuSubItem>().ToList();
            
            foreach (VMOperationToolStripMenuSubItem item in dropDownItems)
            {
                Host host = item.Tag as Host;
                if (host != null)
                {
                    VMOperationCommand cmd = new VMOperationHostCommand(Command.MainWindowCommandInterface, selection, delegate { return host; }, host.Name.EscapeAmpersands(), _operation, session);
                    VMOperationCommand cpmCmd = new CrossPoolMigrateCommand(Command.MainWindowCommandInterface, selection, host, _resumeAfter);

                    VMOperationToolStripMenuSubItem tempItem = item;
                    Program.Invoke(Program.MainWindow, delegate
                                                           {
                                                               bool oldMigrateCmdCanRun = cmd.CanExecute();
                                                               if (_operation == vm_operations.start_on || !oldMigrateCmdCanRun && !cpmCmd.CanExecute())
                                                                   tempItem.Command = cmd;
                                                               else
                                                                   tempItem.Command = oldMigrateCmdCanRun ? cmd : cpmCmd;
                                                            });
                }
            }

            Program.Invoke(Program.MainWindow, () => AddAdditionalMenuItems(selection));
        }

        /// <summary>
        /// Hook to add additional members to the menu item
        /// Note: Called on main window thread by executing code
        /// </summary>
        /// <param name="selection"></param>
        protected virtual void AddAdditionalMenuItems(SelectedItemCollection selection) { return; }

        /// <summary>
        /// This class is an implementation of the 'IComparer' interface 
        /// for sorting vm placement menuItem List when wlb is enabled 
        /// </summary>
        private class WlbHostStarCompare : IComparer<VMOperationToolStripMenuSubItem>
        {
            public int Compare(VMOperationToolStripMenuSubItem x, VMOperationToolStripMenuSubItem y)
            {
                int result = 0;

                // if x and y are enabled, compare their start rating
                if (x.Enabled && y.Enabled)
                    result = y.StarRating.CompareTo(x.StarRating);

                // if x and y are disabled, they are equal
                else if (!x.Enabled && !y.Enabled)
                    result = 0;

                // if x is disabled, y is greater
                else if (!x.Enabled)
                    result = 1;

                // if y is disabled, x is greater
                else if (!y.Enabled)
                    result = -1;

                return result;
            }
        }
    }
}
