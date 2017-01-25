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
using System.Text;
using XenAdmin;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.MainWindowControls;
using XenAdmin.Controls.XenSearch;
using XenAdmin.Plugins;
using XenAdmin.Commands;

namespace XenAdminTests
{
    internal class MainWindowWrapper : TestWrapper<MainWindow>
    {
        public MainWindowWrapper(MainWindow mainWindow)
            : base(mainWindow)
        {
        }

        public ToolStripMenuItem ToolsMenu
        {
            get
            {
                return GetField<ToolStripMenuItem>("toolsToolStripMenuItem");
            }
        }

        public ToolsMenuWrapper ToolsMenuItems
        {
            get
            {
                return new ToolsMenuWrapper(Item);
            }
        }

        public ToolStripMenuItem WindowMenu
        {
            get
            {
                return GetField<ToolStripMenuItem>("windowToolStripMenuItem");
            }
        }

        public ToolStripMenuItem HelpMenu
        {
            get
            {
                return GetField<ToolStripMenuItem>("helpToolStripMenuItem");
            }
        }

        public ToolStripMenuItem ViewMenu
        {
            get
            {
                return GetField<ToolStripMenuItem>("viewToolStripMenuItem");
            }
        }


        public ToolStripMenuItem FileMenu
        {
            get
            {
                return GetField<ToolStripMenuItem>("fileToolStripMenuItem");
            }
        }

        public ToolStripMenuItem PoolMenu
        {
            get
            {
                return GetField<ToolStripMenuItem>("poolToolStripMenuItem");
            }
        }

        public ToolStripMenuItem VMMenu
        {
            get
            {
                return GetField<ToolStripMenuItem>("VMToolStripMenuItem");
            }
        }

        public ToolStripMenuItem TemplatesMenu
        {
            get
            {
                return GetField<ToolStripMenuItem>("templatesToolStripMenuItem");
            }
        }

        public ToolStripEx MainToolStrip
        {
            get
            {
                return GetField<ToolStripEx>("ToolStrip");
            }
        }

        public MainToolBarWrapper MainToolStripItems
        {
            get
            {
                return new MainToolBarWrapper(Item);
            }
        }

        public PluginManager PluginManager
        {
            get
            {
                return GetField<PluginManager>("pluginManager");
            }
        }

        public FlickerFreeTreeView TreeView
        {
            get { return TestUtils.GetFlickerFreeTreeView(Item, "navigationPane.navigationView.treeView"); } }

        public CommandToolStripMenuItem AddHostToolStripMenuItemInPoolMenu
        {
            get
            {
                return GetField<CommandToolStripMenuItem>("addServerToolStripMenuItem");
            }
        }

        public ToolStripMenuItem StorageMenu
        {
            get
            {
                return Item.StorageToolStripMenuItem;
            }
        }

        public StorageMenuWrapper StorageMenuItems
        {
            get
            {
                return new StorageMenuWrapper(Item);
            }
        }

        public ToolStripMenuItem HostMenu
        {
            get
            {
                return GetField<ToolStripMenuItem>("HostMenuItem");
            }
        }

        public HostMenuWrapper HostMenuItems
        {
            get
            {
                return new HostMenuWrapper(Item);
            }
        }

        public VMMenuWrapper VMMenuItems
        {
            get
            {
                return new VMMenuWrapper(Item);
            }
        }

        public ViewMenuWrapper ViewMenuItems
        {
            get
            {
                return new ViewMenuWrapper(Item);
            }
        }

        public TabControl TheTabControl
        {
            get
            {
                return Item.TheTabControl;
            }
        }

        public MenuStripEx MainMenuBar
        {
            get
            {
                return GetField<XenAdmin.Controls.MenuStripEx>("MainMenuBar");
            }
        }

        public NetworkTabPageWrapper NetworkPage
        {
            get
            {
                return new NetworkTabPageWrapper(Item.NetworkPage);
            }
        }

        public TabPage TabPageNetwork
        {
            get
            {
                return GetField<TabPage>("TabPageNetwork");
            }
        }
    }
}
