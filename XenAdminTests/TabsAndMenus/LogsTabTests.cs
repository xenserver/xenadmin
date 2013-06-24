/* Copyright (c) Citrix Systems Inc. 
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
using NUnit.Framework;
using XenAdmin;
using XenAdmin.TabPages;
using XenAdmin.Controls;
using XenAPI;

namespace XenAdminTests.TabsAndMenus
{
    public class LogsTabTests : MainWindowLauncher_TestFixture
    {
        private List<CustomHistoryRow> GetVisibleRows(Predicate<List<CustomHistoryRow>> match)
        {
            return GetVisibleRows(match, null);
        }

        private List<CustomHistoryRow> GetVisibleRows(Predicate<List<CustomHistoryRow>> match, string assertMessage)
        {
            var rows = new List<CustomHistoryRow>();
            MWWaitFor(() =>
                {
                    rows = MainWindowWrapper.HistoryPage.CustomHistoryContainer.CustomHistoryPanel.Rows.FindAll(r => r.Visible);
                    return match(rows);
                }, assertMessage);
            return rows;
        }

        private List<CustomHistoryRow> GetUnfinishedRows(Predicate<List<CustomHistoryRow>> match, string assertMessage)
        {
            var rows = new List<CustomHistoryRow>();
            MWWaitFor(() =>
            {
                var visibleRows = MainWindowWrapper.HistoryPage.CustomHistoryContainer.CustomHistoryPanel.Rows.FindAll(r => r.Visible);
                rows = visibleRows.FindAll(r => r is ActionRow && !((ActionRow)r).Action.IsCompleted);
                return match(rows);
            }, assertMessage);
            return rows;
        }

        [TearDown]
        public new void TearDown()
        {
            RemoveStateDBs();
            GoToTabPage(MainWindowWrapper.TabPageHistory);
            ConnectionsManager.History.Clear();
        }

        [SetUp]
        public void Setup()
        {
            ConnectToStateDBs("state4.xml");
            MW(delegate
            {
                // deselect all the default templates - to speed up tests.
                if (MainWindowWrapper.ViewMenuItems.TemplatesToolStripMenuItem.Checked)
                {
                    MainWindowWrapper.ViewMenuItems.TemplatesToolStripMenuItem.PerformClick();
                }
            });
        }

        [Test]
        public void TestAfterConnection()
        {
            foreach (VirtualTreeNode n in GetAllTreeNodes())
            {
                SelectInTree(n.Tag);
                MW(n.EnsureVisible);

                // Switch to the History tab
                GoToTabPage(MainWindowWrapper.TabPageHistory);

                if (n.Tag == null || n.Tag is Pool || n.Text == "inflames")
                {
                    List<CustomHistoryRow> visibleRows = GetVisibleRows(r => r.Count == 1 && r[0].Title.StartsWith("Connected to "));

                    Assert.AreEqual(1, visibleRows.Count, "No connection item found.");
                    Assert.IsTrue(visibleRows[0].Title.StartsWith("Connected to "), "Connected message not found. The message was: " + visibleRows[0].Title);
                }
                else
                {
                    List<CustomHistoryRow> visibleRows = GetVisibleRows(r => r.Count == 0);
                    Assert.AreEqual(0, visibleRows.Count, "Items found when should be empty for [" + n.Text + "]: " + string.Join(", ", visibleRows.ConvertAll(v => v.Title).ToArray()));
                }
            }
        }

        [Test]
        public void TestHistoryItemsAppearForVMShutdown()
        {
            VM vm1 = GetAnyVM(v => v.name_label == "Iscsi Box");
            VM vm2 = GetAnyVM(v => v.name_label == "Windows Server 2003 x64 (1)");

            SelectInTree(vm1, vm2);

            new List<VirtualTreeNode>(MainWindowWrapper.TreeView.SelectedNodes).ForEach(n => MW(n.EnsureVisible));

            // Switch to the History tab
            GoToTabPage(MainWindowWrapper.TabPageHistory);

            GetVisibleRows(r => r.Count == 0, "History page wasn't empty before VMs shut down");

            MW(MainWindowWrapper.MainToolStripItems.ShutDownToolStripButton.PerformClick);

            GetVisibleRows(r => r.Count == 2, "Items weren't added when VMs shut down.");
        }

        [Test]
        public void TestClear()
        {
            SelectInTree(GetAnyPool());
            GoToTabPage(MainWindowWrapper.TabPageHistory);
            GetVisibleRows(r => r.Count == 1, "No connection item found.");

            // wait for everything to finish.
            GetUnfinishedRows(r => r.Count == 0, "Actions didn't finish.");

            MW(MainWindowWrapper.HistoryPage.ClearButton.PerformClick);
            GetVisibleRows(r => r.Count == 0, "Items weren't cleared.");
        }

        [Test]
        public void TestHide()
        {
            SelectInTree(GetAnyPool());
            GoToTabPage(MainWindowWrapper.TabPageHistory);
            GetVisibleRows(r => r.Count == 1, "No connection item found.");

            // this should clear all items as they are all information messages.
            MW(() => MainWindowWrapper.HistoryPage.InformationCheckBox.Checked = false);
            GetVisibleRows(r => r.Count == 0, "Items weren't cleared.");

            // this should bring them back.
            MW(() => MainWindowWrapper.HistoryPage.InformationCheckBox.Checked = true);
            GetVisibleRows(r => r.Count == 1, "Items were cleared.");

            // nothing should change for actions-checkbox
            MW(() => MainWindowWrapper.HistoryPage.ActionsCheckBox.Checked = false);
            GetVisibleRows(r => r.Count == 1, "Items were cleared.");

            // nothing should change for alerts-checkbox
            MW(() => MainWindowWrapper.HistoryPage.AlertsCheckBox.Checked = false);
            GetVisibleRows(r => r.Count == 1, "Items were cleared.");

            // nothing should change for errors-checkbox
            MW(() => MainWindowWrapper.HistoryPage.ErrorsCheckBox.Checked = false);
            GetVisibleRows(r => r.Count == 1, "Items were cleared.");

            // put all these back
            MW(() => MainWindowWrapper.HistoryPage.ActionsCheckBox.Checked = true);
            MW(() => MainWindowWrapper.HistoryPage.AlertsCheckBox.Checked = true);
            MW(() => MainWindowWrapper.HistoryPage.ErrorsCheckBox.Checked = true);
        }
    }
}
