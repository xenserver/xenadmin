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

using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;

namespace XenAdminTests.TabsAndMenus
{
    public class TabsAndMenus : MainWindowLauncher_TestFixture
    {
        protected bool CheckHelp = true;

        public TabsAndMenus(params string[] databases)
            : base(databases)
        { }

        protected void VerifyTabs(object ixmo, string[] expectedTabs)
        {
            string ixmoString = (ixmo == null ? "XenCenter node" : ixmo.ToString());

            // Select the ixmo in the tree
            Assert.IsTrue(SelectInTree(ixmo), "Couldn't find a node for " + ixmoString + " in the tree");

            // Check that we have the right number of tabs
            TabControl actualTabs = Program.MainWindow.TheTabControl;
            Assert.AreEqual(expectedTabs.Length, actualTabs.TabPages.Count, "Didn't find the expected number of tabs for " + ixmoString);

            for (int i = 0; i < expectedTabs.Length; ++i)
            {
                //// Select the i'th tab and check that the tab has the right name
                string actualText = actualTabs.TabPages[i].Text;
                Assert.AreEqual(expectedTabs[i], actualText, "Tab wrong at position " + i + " for " + ixmoString);

                if (CheckHelp)
                {
                    // Make sure we have help
                    bool hasHelp = MW<bool>(Program.MainWindow.HasHelp);
                    Assert.IsTrue(hasHelp, "No help for tab " + expectedTabs[i] + " for " + ixmoString);
                }
            }
        }

        // In the expected items, represent a separator by a null
        protected void VerifyContextMenu(object ixmo, ExpectedMenuItem[] expectedItems)
        {
            string ixmoString = (ixmo == null ? "XenCenter node" : ixmo.ToString());
            Assert.IsTrue(SelectInTree(ixmo), "Couldn't find a node for " + ixmoString + " in the tree");

            MW(() =>
                {
                    var tree = TestUtils.GetFlickerFreeTreeView(MainWindowWrapper.Item, "navigationPane.navigationView.treeView");
                    // Generate the TreeContextMenu
                    var e = new VirtualTreeNodeMouseClickEventArgs(tree.SelectedNode, MouseButtons.Right, 1, 0, 0);
                    var view = TestUtils.GetNavigationView(MainWindowWrapper.Item, "navigationPane.navigationView");
                    TestUtils.ExecuteMethod(view, "HandleNodeRightClick", new object[] { e });
                });
            var menu = TestUtils.GetContextMenuStrip(MainWindowWrapper.Item, "navigationPane.navigationView.TreeContextMenu");

            AssertToolStripItems(ixmoString, expectedItems, menu.Items, true);

            // Close the menu again
            MW(menu.Close);
            Thread.Sleep(100);
        }

        protected void VerifyToolbar(object ixmo, ExpectedMenuItem[] expectedItems)
        {
            string ixmoString = (ixmo == null ? "XenCenter node" : ixmo.ToString());

            // Select the ixmo in the tree
            Assert.IsTrue(SelectInTree(ixmo), "Couldn't find a node for " + ixmoString + " in the tree");

            List<ToolStripItem> items = new List<ToolStripItem>();

            // skip out forward, back and system alert buttons
            for (int i = 2; i < MainWindowWrapper.MainToolStrip.Items.Count; i++)
                items.Add(MainWindowWrapper.MainToolStrip.Items[i]);

            AssertToolStripItems(ixmoString, expectedItems, new ToolStripItemCollection(MainWindowWrapper.MainToolStrip, items.ToArray()), false);
        }

        /// <summary>
        /// Verifies the toolbar for the currently selected item.
        /// </summary>
        protected void VerifyToolbar(ExpectedMenuItem[] expectedItems)
        {
            List<ToolStripItem> items = new List<ToolStripItem>();

            // skip out forward, back and system alert buttons
            for (int i = 2; i < MainWindowWrapper.MainToolStrip.Items.Count - 1; i++)
            {
                items.Add(MainWindowWrapper.MainToolStrip.Items[i]);
            }

            AssertToolStripItems(MainWindowWrapper.TreeView.SelectedNode.Text, expectedItems, new ToolStripItemCollection(MainWindowWrapper.MainToolStrip, items.ToArray()), false);
        }


        protected void VerifyMainMenu(object ixmo, ExpectedMenuItem[] poolMenu, ExpectedMenuItem[] serverMenu, ExpectedMenuItem[] vmMenu, ExpectedMenuItem[] storageMenu, ExpectedMenuItem[] templatesMenu)
        {
            string ixmoString = (ixmo == null ? "XenCenter node" : ixmo.ToString());

            // Select the ixmo in the tree
            Assert.IsTrue(SelectInTree(ixmo), "Couldn't find a node for " + ixmoString + " in the tree");

            Thread.Sleep(300);
            
            MW(MainWindowWrapper.PoolMenu.Select);
            MW(MainWindowWrapper.PoolMenu.ShowDropDown);
            Thread.Sleep(100);
            AssertToolStripItems(ixmoString, poolMenu, MainWindowWrapper.PoolMenu.DropDownItems, true);
            MW(MainWindowWrapper.PoolMenu.HideDropDown);

            MW(MainWindowWrapper.HostMenu.Select);
            MW(MainWindowWrapper.HostMenu.ShowDropDown);
            Thread.Sleep(100);
            AssertToolStripItems(ixmoString, serverMenu, MainWindowWrapper.HostMenu.DropDownItems, true);
            MW(MainWindowWrapper.HostMenu.HideDropDown);

            MW(MainWindowWrapper.VMMenu.Select);
            MW(MainWindowWrapper.VMMenu.ShowDropDown);
            Thread.Sleep(100);
            AssertToolStripItems(ixmoString, vmMenu, MainWindowWrapper.VMMenu.DropDownItems, true);
            MW(MainWindowWrapper.VMMenu.HideDropDown);

            MW(MainWindowWrapper.StorageMenu.Select);
            MW(MainWindowWrapper.StorageMenu.ShowDropDown);
            Thread.Sleep(100);
            AssertToolStripItems(ixmoString, storageMenu, MainWindowWrapper.StorageMenu.DropDownItems, true);
            MW(MainWindowWrapper.StorageMenu.HideDropDown);

            MW(MainWindowWrapper.TemplatesMenu.Select);
            MW(MainWindowWrapper.TemplatesMenu.ShowDropDown);
            Thread.Sleep(100);
            AssertToolStripItems(ixmoString, templatesMenu, MainWindowWrapper.TemplatesMenu.DropDownItems, true);
            MW(MainWindowWrapper.TemplatesMenu.HideDropDown);
        }

        private static bool ToolStripItemsEqual(ExpectedMenuItem[] expectedItems, List<ToolStripItem> itemsList)
        {
            if (expectedItems.Length != itemsList.Count)
            {
                return false;
            }

            for (int i = 0; i < expectedItems.Length; i++)
            {
                if (expectedItems[i] is ExpectedSeparator && itemsList[i] is ToolStripSeparator)
                {
                    continue;
                }
                else if (expectedItems[i] is ExpectedSeparator && !(itemsList[i] is ToolStripSeparator))
                {
                    return false;
                }
                else if (!(expectedItems[i] is ExpectedSeparator) && itemsList[i] is ToolStripSeparator)
                {
                    return false;
                }
                else if (((ExpectedTextMenuItem)expectedItems[i]).text != itemsList[i].Text.TrimEnd())
                {
                    return false;
                }
            }

            return true;
        }
        
        private void AssertToolStripItems(string ixmoString, ExpectedMenuItem[] expectedItems, ToolStripItemCollection items, bool checkMnemonics)
        {
            List<ToolStripItem> itemsList = GetVisibleToolStripItems(items);

            // some menu items take time before they become their final value.
            MWWaitFor(() => ToolStripItemsEqual(expectedItems, itemsList));
            
            Assert.AreEqual(expectedItems.Length, itemsList.Count,
                string.Format("Wrong number of items in the menu for {0}: {1} != {2}", ixmoString, GetNames(expectedItems), GetNames(itemsList)));

            List<char> usedMnemonics = new List<char>();
            for (int i = 0; i < expectedItems.Length; ++i)
            {
                ToolStripItem item = itemsList[i];

                if (expectedItems[i] is ExpectedSeparator)
                {
                    Assert.IsTrue(item is ToolStripSeparator, "At position " + i + " for " + ixmoString + ", expected separator, found " + item.Text);
                }
                else
                {
                    ToolStripMenuItem menuItem = item as ToolStripMenuItem;

                    ExpectedTextMenuItem expected = expectedItems[i] as ExpectedTextMenuItem;
                    Assert.AreEqual(expected.text, item.Text.TrimEnd(), "Wrong item at position " + i + " for " + ixmoString);
                    Assert.AreEqual(expected.enabled, item.Enabled, "Wrong enablement for item " + item.Text + " at position " + i + " for " + ixmoString);
                    if (checkMnemonics && item.Text.TrimEnd() != "(empty)" && !expected.skipMnemonicCheck)
                    {
                        char c = ExtractMnemonic(item.Text.TrimEnd());
                        if (usedMnemonics.Contains(c))
                            Assert.Fail(string.Format("Item '{0}' duplicates mnemnonic '{1}'", item.Text, c));
                    }
                    if (menuItem != null && menuItem.DropDownItems.Count > 0 && menuItem.Enabled)
                    {
                        MW(menuItem.ShowDropDown);
                        AssertToolStripItems(ixmoString, expected.DropDownItems, menuItem.DropDownItems, checkMnemonics);
                    }

                    if (expected.chcked.HasValue)
                    {
                        ToolStripButton buttonItem = item as ToolStripButton;

                        if (menuItem != null)
                        {
                            Assert.AreEqual(expected.chcked.Value, menuItem.Checked, "Wrong checked state for item " + item.Text + " at position " + i + " for " + ixmoString);
                        }
                        else if (buttonItem != null)
                        {
                            Assert.AreEqual(expected.chcked.Value, buttonItem.Checked, "Wrong checked state for item " + item.Text + " at position " + i + " for " + ixmoString);
                        }
                    }
                }
            }
        }

        private char ExtractMnemonic(string p)
        {
            int mnemonicIndex = p.IndexOf('&');
            if (mnemonicIndex == -1 || mnemonicIndex == p.Length - 1)
                Assert.Fail(string.Format("Menu item '{0}' is missing a mnemonic", p));

            return p[mnemonicIndex + 1];
        }

        protected static List<ToolStripItem> GetVisibleToolStripItems(ToolStripItemCollection items)
        {
            return Util.PopulateList<ToolStripItem>(items).FindAll(t => t.Available);
        }

        private string GetNames(ExpectedMenuItem[] items)
        {
            List<string> l = new List<string>();
            foreach (ExpectedMenuItem i in items)
            {
                l.Add(i.ToString());
            }
            return string.Join(", ", l.ToArray());
        }

        private string GetNames(List<ToolStripItem> items)
        {
            List<string> l = new List<string>();
            foreach (ToolStripItem i in items)
            {
                l.Add(i.Text);
            }
            return string.Join(", ", l.ToArray());
        }

        protected static bool IsMaster(Host host)
        {
            return host.IsMaster();
        }

        protected static bool IsSlave(Host host)
        {
            return !host.IsMaster();
        }

        protected static bool HasTools(VM vm)
        {
            return !vm.is_a_template && vm.IsRunning&&
                vm.virtualisation_status != 0 &&
                !vm.virtualisation_status.HasFlag(VM.VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE);
        }

        protected static bool NoTools(VM vm)
        {
            return !vm.is_control_domain && !vm.is_a_template && vm.IsRunning && !HasTools(vm);
        }

        protected static bool IsShutdown(VM vm)
        {
            return !vm.is_a_template && vm.power_state == vm_power_state.Halted;
        }

        protected static bool IsDefaultTemplate(VM vm)
        {
            return vm.is_a_template && vm.DefaultTemplate && vm.name_label != "XenSource P2V Server";
        }

        protected static bool CanDetach(SR sr)
        {
            return sr.IsDetachable() && !HelpersGUI.GetActionInProgress(sr);
        }

        protected static bool CanForget(SR sr)
        {
            return !sr.HasRunningVMs() && sr.CanCreateWithXenCenter && sr.allowed_operations.Contains(storage_operations.forget) && !HelpersGUI.GetActionInProgress(sr);
        }

        protected static bool CanDestroy(SR sr)
        {
            return !sr.HasRunningVMs() && sr.CanCreateWithXenCenter && sr.allowed_operations.Contains(storage_operations.destroy) && !HelpersGUI.GetActionInProgress(sr);
        }

        protected static bool CanSetAsDefault(SR sr)
        {
            return sr.HasPBDs && !SR.IsDefaultSr(sr) && sr.SupportsVdiCreate() && (sr.shared || sr.Connection.Cache.HostCount <= 1) && !HelpersGUI.GetActionInProgress(sr);
        }

        protected static bool CanConvertSR(SR sr)
        {
            return sr != null && Helpers.DundeeOrGreater(sr.Connection) && (sr.type == "lvmohba" || sr.type == "lvmoiscsi") && !sr.IsThinProvisioned;
        }

        /// <summary>
        /// Adds the "Expand All" and "Collapse Children" expected items to the specified list for the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="expectedItems">The expected items to be added to.</param>
        protected void AddExpectedExpandAndCollapseItems(VirtualTreeNode node, List<ExpectedMenuItem> expectedItems)
        {
            bool expandAll = false;
            bool collapseChildren = false;

            MW(delegate
            {
                expandAll = new List<VirtualTreeNode>(node.Descendants).Find(n => !n.IsExpanded) != null;
                collapseChildren = new List<VirtualTreeNode>(node.Nodes).Find(n => n.Nodes.Count > 0 && n.IsExpanded) != null;
            });

            if (expandAll)
            {
                expectedItems.Add(new ExpectedTextMenuItem("E&xpand All", true));
            }

            if (collapseChildren)
            {
                expectedItems.Add(new ExpectedTextMenuItem("&Collapse Children", true));
            }
        }

        protected void SetPluginCheckstate(CheckState state)
        {
            HandleModalDialog<OptionsDialogWrapper>("Options", MainWindowWrapper.ToolsMenuItems.OptionsToolStripMenuItem.PerformClick,
                                                    p =>
                                                        {
                                                            var grid = TestUtils.GetFieldDeep<DataGridView>(p.Item, "pluginOptionsPage1.m_gridPlugins");

                                                            foreach (DataGridViewRow row in grid.Rows)
                                                                row.Cells[0].Value = (state == CheckState.Checked);

                                                            p.OkButton.PerformClick();
                                                        });
        }

        protected void EnableAllPlugins()
        {
            SetPluginCheckstate(CheckState.Checked);
        }

        protected void DisableAllPlugins()
        {
            SetPluginCheckstate(CheckState.Unchecked);
        }

        public class ExpectedMenuItem { }

        public class ExpectedSeparator : ExpectedMenuItem
        {
            public override string ToString()
            {
                return "<separator>";
            }
        }

        public class ExpectedTextMenuItem : ExpectedMenuItem
        {
            public string text;
            public bool enabled;
            public bool? chcked;
            public ExpectedMenuItem[] DropDownItems;
            public bool skipMnemonicCheck = false;

            public ExpectedTextMenuItem(string text, bool enabled)
                : this(text, enabled, new ExpectedMenuItem[0])
            {
            }

            public ExpectedTextMenuItem(string text, bool enabled, ExpectedMenuItem[] dropDownItems)
            {
                this.text = text;
                this.enabled = enabled;
                this.DropDownItems = dropDownItems;
            }

            public ExpectedTextMenuItem(string text, bool enabled, bool chcked, ExpectedMenuItem[] dropDownItems)
                : this(text, enabled, dropDownItems)
            {
                this.chcked = chcked;
            }

            public ExpectedTextMenuItem(string text, bool enabled, bool chcked)
                : this(text, enabled, chcked, new ExpectedMenuItem[0])
            {
            }

            public ExpectedTextMenuItem(string text, bool enabled, bool chcked, bool skipMnemonicCheck)
                : this(text, enabled, chcked, new ExpectedMenuItem[0])
            {
                this.skipMnemonicCheck = skipMnemonicCheck;
            }

            public override string ToString()
            {
                return enabled ? text : string.Format("{0} (disabled)", text);
            }
        }
    }
}
