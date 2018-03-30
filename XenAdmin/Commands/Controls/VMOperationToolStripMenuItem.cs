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
        private HostListUpdater hostListUpdater = new HostListUpdater();

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

        private bool _isDropDownClosed;

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            _isDropDownClosed = true;
        }


        protected override void OnDropDownOpening(EventArgs e)
        {
            base.DropDownItems.Clear();
            _isDropDownClosed = false;

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

                hostListUpdater.updateHostList(this, session);
                
            });
        }

        /// <summary>
        /// Hook to add additional members to the menu item
        /// Note: Called on main window thread by executing code
        /// </summary>
        /// <param name="selection"></param>
        protected virtual void AddAdditionalMenuItems(SelectedItemCollection selection) { return; }

        private class HostListUpdater
        {
            private ProduceConsumerQueue workQueue = new ProduceConsumerQueue(1);
            private ProduceConsumerQueue workerQueueWithouWlb = new ProduceConsumerQueue(25);
            readonly object _locker = new object();
            private bool _Stopped;

            private bool Stopped
            {
                set
                {
                    lock (_locker)
                    {
                        _Stopped = value;
                    }
                }
                get
                {
                    lock (_locker)
                    {
                        return _Stopped;
                    }
                }
            }

            public void updateHostList(VMOperationToolStripMenuItem menu, Session session)
            {
                Stopped = true;
                workQueue.CancelWorkers(true);
                Stopped = false;
                workQueue = new ProduceConsumerQueue(1);
                workQueue.EnqueueItem(() =>
                {
                    if (Stopped)
                        return;

                    SelectedItemCollection selection = menu.Command.GetSelection();
                    if (Helpers.WlbEnabled(selection[0].Connection))
                    {
                        workerQueueWithouWlb = new ProduceConsumerQueue(25);
                        WlbRecommendations recommendations = new WlbRecommendations(selection.AsXenObjects<VM>(), session);
                        recommendations.Initialize();
                        if (Stopped)
                            return;

                        if (recommendations.IsError)
                            EnableAppropriateHostsNoWlb(menu, session);
                        else
                            EnableAppropriateHostsWlb(menu, session, recommendations);
                    }
                    else
                    {
                        EnableAppropriateHostsNoWlb(menu, session);
                    }
                });
            }

            private void EnableAppropriateHostsWlb(VMOperationToolStripMenuItem menu, Session session, WlbRecommendations recommendations)
            {
                SelectedItemCollection selection = menu.Command.GetSelection();
                // set the first menu item to be the WLB optimal server menu item
                VMOperationToolStripMenuSubItem firstItem = (VMOperationToolStripMenuSubItem)menu.DropDownItems[0];
                var firstItemCmd = new VMOperationWlbOptimalServerCommand(menu.Command.MainWindowCommandInterface, selection, menu._operation, recommendations);
                var firstItemCmdCanExecute = firstItemCmd.CanExecute();

                if (Stopped)
                    return;

                Program.Invoke(Program.MainWindow, delegate
                {
                    firstItem.Command = firstItemCmd;
                    firstItem.Enabled = firstItemCmdCanExecute;
                });

                List<VMOperationToolStripMenuSubItem> hostMenuItems = new List<VMOperationToolStripMenuSubItem>();
                foreach (VMOperationToolStripMenuSubItem item in menu.DropDownItems)
                {
                    Host host = item.Tag as Host;
                    if (host != null)
                    {
                        var cmd = new VMOperationWlbHostCommand(menu.Command.MainWindowCommandInterface, selection, host, menu._operation, recommendations.GetStarRating(host));
                        var canExecute = cmd.CanExecute();

                        if (Stopped)
                            return;

                        Program.Invoke(Program.MainWindow, delegate
                        {
                            item.Command = cmd;
                            item.Enabled = canExecute;
                        });
                        hostMenuItems.Add(item);
                    }
                }

                // Shuffle the list to make it look cool
                // Helpers.ShuffleList(hostMenuItems);

                // sort the hostMenuItems by star rating
                hostMenuItems.Sort(new WlbHostStarCompare());

                // refresh the drop-down-items from the menuItems.
                foreach (VMOperationToolStripMenuSubItem menuItem in hostMenuItems)
                {
                    if (Stopped)
                        return;

                    Program.Invoke(Program.MainWindow, delegate ()
                    {
                        menu.DropDownItems.Insert(hostMenuItems.IndexOf(menuItem) + 1, menuItem);
                    });
                }

                if (Stopped)
                    return;

                Program.Invoke(Program.MainWindow, () => menu.AddAdditionalMenuItems(selection));

            }

            private void EnableAppropriateHostsNoWlb(VMOperationToolStripMenuItem menu, Session session)
            {
                SelectedItemCollection selection = menu.Command.GetSelection();
                IXenConnection connection = selection[0].Connection;

                VMOperationCommand cmdHome = new VMOperationHomeServerCommand(menu.Command.MainWindowCommandInterface, selection, menu._operation, session);
                Host affinityHost = connection.Resolve(((VM)menu.Command.GetSelection()[0].XenObject).affinity);

                if (Stopped)
                    return;

                Program.Invoke(Program.MainWindow, delegate
                {
                    var firstItem = (VMOperationToolStripMenuSubItem)menu.DropDownItems[0];

                    bool oldMigrateToHomeCmdCanRun = cmdHome.CanExecute();
                    if (affinityHost == null || menu._operation == vm_operations.start_on || oldMigrateToHomeCmdCanRun)
                    {
                        firstItem.Command = cmdHome;
                        firstItem.Enabled = oldMigrateToHomeCmdCanRun;
                    }
                    else
                    {
                        VMOperationCommand cpmCmdHome = new CrossPoolMigrateToHomeCommand(menu.Command.MainWindowCommandInterface, selection, affinityHost);

                        if (cpmCmdHome.CanExecute())
                        {
                            firstItem.Command = cpmCmdHome;
                            firstItem.Enabled = true;
                        }
                        else
                        {
                            firstItem.Command = cmdHome;
                            firstItem.Enabled = false;
                        }
                    }
                });

                List<VMOperationToolStripMenuSubItem> dropDownItems = menu.DropDownItems.Cast<VMOperationToolStripMenuSubItem>().ToList();

                if (Stopped)
                    return;

                // Adds the migrate wizard button, do this before the enable checks on the other items
                Program.Invoke(Program.MainWindow, () => menu.AddAdditionalMenuItems(selection));

                foreach (VMOperationToolStripMenuSubItem item in dropDownItems)
                {
                    if (menu._isDropDownClosed)
                    {
                        // Stop making requests to assert can start on each host after dropdown is closed
                        break;
                    }

                    Host host = item.Tag as Host;
                    if (host != null)
                    {
                        // API calls could happen in CanExecute(), which take time to wait.
                        // So a Producer-Consumer-Queue with size 25 is used here to :
                        //   1. Make API calls for different menu items happen in parallel;
                        //   2. Limit the count of concurrent threads (now it's 25).
                        workerQueueWithouWlb.EnqueueItem(() =>
                        {
                            if (menu._isDropDownClosed)
                                return;
                            VMOperationCommand cmd = new VMOperationHostCommand(menu.Command.MainWindowCommandInterface, selection, delegate { return host; }, host.Name().EscapeAmpersands(), menu._operation, session);
                            CrossPoolMigrateCommand cpmCmd = new CrossPoolMigrateCommand(menu.Command.MainWindowCommandInterface, selection, host, menu._resumeAfter);

                            VMOperationToolStripMenuSubItem tempItem = item;
                            bool oldMigrateCmdCanRun = cmd.CanExecute();
                            if ((menu._operation == vm_operations.start_on) || oldMigrateCmdCanRun)
                            {
                                if (Stopped)
                                    return;

                                Program.Invoke(Program.MainWindow, delegate
                                {
                                    tempItem.Command = cmd;
                                    tempItem.Enabled = oldMigrateCmdCanRun;
                                });
                            }
                            else
                            {
                                bool crossPoolMigrateCmdCanRun = cpmCmd.CanExecute();
                                if (crossPoolMigrateCmdCanRun || !string.IsNullOrEmpty(cpmCmd.CantExecuteReason))
                                {
                                    if (Stopped)
                                        return;

                                    Program.Invoke(Program.MainWindow, delegate
                                    {
                                        tempItem.Command = cpmCmd;
                                        tempItem.Enabled = crossPoolMigrateCmdCanRun;
                                    });
                                }
                                else
                                {
                                    if (Stopped)
                                        return;

                                    Program.Invoke(Program.MainWindow, delegate
                                    {
                                        tempItem.Command = cmd;
                                        tempItem.Enabled = oldMigrateCmdCanRun;
                                    });
                                }
                            }
                        });
                    }
                }
            }

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
}
