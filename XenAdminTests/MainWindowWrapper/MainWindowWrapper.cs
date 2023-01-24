/* Copyright (c) Cloud Software Group, Inc. 
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

using XenAdmin;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Plugins;


namespace XenAdminTests
{
    internal class MainWindowWrapper : TestWrapper<MainWindow>
    {
        public MainWindowWrapper(MainWindow mainWindow)
            : base(mainWindow)
        {
        }

        public ToolStripMenuItem ToolsMenu => GetField<ToolStripMenuItem>("toolsToolStripMenuItem");

        public ToolsMenuWrapper ToolsMenuItems => new ToolsMenuWrapper(Item);

        public ToolStripMenuItem HelpMenu => GetField<ToolStripMenuItem>("helpToolStripMenuItem");

        public ToolStripMenuItem ViewMenu => GetField<ToolStripMenuItem>("viewToolStripMenuItem");

        public ToolStripMenuItem FileMenu => GetField<ToolStripMenuItem>("fileToolStripMenuItem");

        public ToolStripMenuItem PoolMenu => GetField<ToolStripMenuItem>("poolToolStripMenuItem");

        public ToolStripMenuItem VMMenu => GetField<ToolStripMenuItem>("VMToolStripMenuItem");

        public ToolStripMenuItem TemplatesMenu => GetField<ToolStripMenuItem>("templatesToolStripMenuItem");

        public ToolStripEx MainToolStrip => GetField<ToolStripEx>("ToolStrip");

        public MainToolBarWrapper MainToolStripItems => new MainToolBarWrapper(Item);

        public PluginManager PluginManager => GetField<PluginManager>("pluginManager");

        public FlickerFreeTreeView TreeView => TestUtils.GetFlickerFreeTreeView(Item, "navigationPane.navigationView.treeView");

        public ToolStripMenuItem StorageMenu => Item.StorageToolStripMenuItem;

        public StorageMenuWrapper StorageMenuItems => new StorageMenuWrapper(Item);

        public ToolStripMenuItem HostMenu => GetField<ToolStripMenuItem>("HostMenuItem");

        public VMMenuWrapper VMMenuItems => new VMMenuWrapper(Item);

        public ViewMenuWrapper ViewMenuItems => new ViewMenuWrapper(Item);

        public TabControl TheTabControl => Item.TheTabControl;

        public MenuStripEx MainMenuBar => GetField<MenuStripEx>("MainMenuBar");

        public NetworkTabPageWrapper NetworkPage => new NetworkTabPageWrapper(Item.NetworkPage);

        public TabPage TabPageNetwork => GetField<TabPage>("TabPageNetwork");

        public Form[] OwnedForms => Item.OwnedForms;
    }
}
