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

using System.Collections.Generic;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.XenSearch;
using XenAPI;

namespace XenAdminTests.TabsAndMenus
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class TabsAndMenusGeorge : TabsAndMenus
    {
        public static string[] XenCenterTabs = new string[] { "Home", "Search", "Tags", "Logs" };
        public static string[] PoolTabs = new string[] { "Search", "General", "Storage", "Networking", "HA", "WLB", "Users", "Logs" };
        public static string[] HostTabs = new string[] { "Search", "General", "Storage", "Networking", "NICs", "Console", "Performance", "Users", "Logs" };
        public static string[] VMTabs = new string[] { "General", "Storage", "Networking", "Console", "Performance", "Snapshots", "Logs" };
        public static string[] DefaultTemplateTabs = new string[] { "General", "Networking", "Logs" };
        public static string[] OtherInstallMediaTabs = new string[] { "General", "Storage", "Networking", "Logs" };
        public static string[] UserTemplateTabs_Provision = new string[] { "General", "Networking", "Logs" };
        public static string[] UserTemplateTabs_NoProvision = new string[] { "General", "Storage", "Networking", "Logs" };
        public static string[] SRTabs = new string[] { "General", "Storage", "Logs" };
        public static string[] SnapshotTabs = new string[] { "General", "Networking", "Logs" };
        public static string[] VDITabs = new string[] { "Logs" };
        public static string[] NetworkTabs = new string[] { "Logs" };
        public static string[] GroupingTagTabs = new string[] { "Search", "Logs" };
        public static string[] FolderTabs = new string[] { "Search", "Logs" };

        public TabsAndMenusGeorge()
            : base("state1.xml")
        {
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            DisableAllPlugins();
        }

        [Test]
        public void Tabs_XenCenterNode()
        {
            VerifyTabs(null, XenCenterTabs);
        }

        [Test]
        public void Tabs_Pool()
        {
            VerifyTabs(GetAnyPool(), PoolTabs);
        }

        [Test]
        public void Tabs_Host()
        {
            foreach (Host host in GetAllXenObjects<Host>())
            {
                VerifyTabs(host, HostTabs);
            }
        }

        [Test]
        public void Tabs_VM()
        {
            foreach (VM vm in GetAllXenObjects<VM>(v => !v.is_a_template && !v.is_control_domain))
            {
                VerifyTabs(vm, VMTabs);
            }
        }

        [Test]
        public void Tabs_DefaultTemplate()
        {
            EnsureDefaultTemplatesShown();
            VerifyTabs(GetAnyDefaultTemplate(), DefaultTemplateTabs);
        }

        [Test]
        public void Tabs_OtherInstallMedia()
        {
            EnsureDefaultTemplatesShown();
            VerifyTabs(GetAnyDefaultTemplate(v => v.name_label == "Other install media"), OtherInstallMediaTabs);
        }

        [Test]
        public void Tabs_UserTemplate_Provision()
        {
            EnsureDefaultTemplatesShown();
            foreach (VM vm in GetAllXenObjects<VM>(v => v.is_a_template && !v.DefaultTemplate && !v.is_a_snapshot && v.HasProvisionXML))
            {
                VerifyTabs(vm, UserTemplateTabs_Provision);
            }
        }

        [Test]
        public void Tabs_UserTemplate_NoProvision()
        {
            EnsureDefaultTemplatesShown();
            foreach (VM vm in GetAllXenObjects<VM>(v => v.is_a_template && !v.DefaultTemplate && !v.is_a_snapshot && !v.HasProvisionXML))
            {
                VerifyTabs(vm, UserTemplateTabs_NoProvision);
            }
        }

        [Test]
        public void Tabs_SR()
        {
            EnsureChecked(MainWindowWrapper.ViewMenuItems.LocalStorageToolStripMenuItem);
            foreach (SR sr in GetAllXenObjects<SR>(s => !s.IsToolsSR))
            {
                VerifyTabs(sr, SRTabs);
            }
        }

        [Test]
        public void Tabs_Snapshot()
        {
            PutInOrgView(OBJECT_VIEW);
            try
            {
                foreach (VM snapshot in GetAllXenObjects<VM>(v => v.is_a_snapshot))
                {
                    VerifyTabs(snapshot, SnapshotTabs);
                }
            }
            finally
            {
                PutInOrgView(INFRASTRUCTURE_VIEW);
            }
        }

        [Test]
        public void Tabs_VDI()
        {
            PutInOrgView(OBJECT_VIEW);
            try
            {
                VerifyTabs(GetAnyVDI(v => v.name_label != "base copy"), VDITabs);
            }
            finally
            {
                PutInOrgView(INFRASTRUCTURE_VIEW);
            }
        }

        [Test]
        public void Tabs_Network()
        {
            PutInOrgView(OBJECT_VIEW);
            try
            {
                foreach (XenAPI.Network network in GetAllXenObjects<XenAPI.Network>(n => n.name_label != "Guest installer network"))
                {
                    VerifyTabs(network, NetworkTabs);
                }
            }
            finally
            {
                PutInOrgView(INFRASTRUCTURE_VIEW);
            }
        }

        [Test]
        public void Tabs_GroupingTag()
        {
            PutInOrgView(OBJECT_VIEW);
            try
            {
                VirtualTreeNode n = GetAllTreeNodes().Find(v => v.Tag is GroupingTag);
                VerifyTabs((GroupingTag)n.Tag, GroupingTagTabs);
            }
            finally
            {
                PutInOrgView(INFRASTRUCTURE_VIEW);
            }
        }

        [Test]
        public void Tabs_Folder()
        {
            PutInOrgView(OBJECT_VIEW);
            try
            {
                foreach (Folder folder in GetAllXenObjects<Folder>())
                {
                    VerifyTabs(folder, FolderTabs);
                }
            }
            finally
            {
                PutInOrgView(INFRASTRUCTURE_VIEW);
            }
        }

        [Test]
        public void ContextMenu_XenCenterNode_AllClosed()
        {
            VirtualTreeNode rootNode = FindInTree(null);
            MW(() => rootNode.Collapse());
            VerifyContextMenu(null, new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("&Add...", true),
                new ExpectedTextMenuItem("&New Pool...", true),
                new ExpectedTextMenuItem("C&onnect All", false),
                new ExpectedTextMenuItem("Di&sconnect All", true),
                new ExpectedTextMenuItem("E&xpand All", true)
            });
        }

        [Test]
        public void ContextMenu_XenCenterNode_RootOpen()
        {
            VirtualTreeNode rootNode = FindInTree(null);
            MW(delegate
            {
                rootNode.Collapse();
                rootNode.Expand();
            });
            VerifyContextMenu(null, new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("&Add...", true),
                new ExpectedTextMenuItem("&New Pool...", true),
                new ExpectedTextMenuItem("C&onnect All", false),
                new ExpectedTextMenuItem("Di&sconnect All", true),
                new ExpectedTextMenuItem("E&xpand All", true)
            });
        }

        [Test]
        public void ContextMenu_XenCenterNode_PoolOpen()
        {
            VirtualTreeNode rootNode = FindInTree(null);

            MW(delegate
            {
                rootNode.Collapse();
                rootNode.Expand();
            });
            VirtualTreeNode poolNode = FindInTree(GetAnyPool());

            MW(delegate
            {
                poolNode.Expand();
            });

            VerifyContextMenu(null, new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("&Add...", true),
                new ExpectedTextMenuItem("&New Pool...", true),
                new ExpectedTextMenuItem("C&onnect All", false),
                new ExpectedTextMenuItem("Di&sconnect All", true),
                new ExpectedTextMenuItem("E&xpand All", true),
                new ExpectedTextMenuItem("&Collapse Children", true)
            });
        }

        [Test]
        public void ContextMenu_XenCenterNode_AllOpen()
        {
            VirtualTreeNode rootNode = FindInTree(null);

            MW(delegate
            {
                rootNode.ExpandAll();
            });
            VerifyContextMenu(null, new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("&Add...", true),
                new ExpectedTextMenuItem("&New Pool...", true),
                new ExpectedTextMenuItem("C&onnect All", false),
                new ExpectedTextMenuItem("Di&sconnect All", true),
                new ExpectedTextMenuItem("&Collapse Children", true)
            });
        }

        [Test]
        public void ContextMenu_Pool()
        {
            var pool = GetAnyPool();
            var expected = new List<ExpectedMenuItem>
                               {
                                   new ExpectedTextMenuItem("New V&M...", true),
                                   new ExpectedTextMenuItem("&New SR...", true),
                                   new ExpectedTextMenuItem("&Import...", true),
                                   new ExpectedSeparator(),
                                   new ExpectedTextMenuItem("&High Availability...", true)
                               };
            if (Helpers.CowleyOrGreater(pool.Connection))
                expected.Add(new ExpectedTextMenuItem("VM Pr&otection Policies...", true));
            if (Helpers.BostonOrGreater(pool.Connection))
            {
                expected.Add(new ExpectedTextMenuItem("Manage &vApps...", true));
                expected.Add(new ExpectedTextMenuItem("Di&saster Recovery", true, new ExpectedMenuItem[]
                                                                                      {
                                                                                          new ExpectedTextMenuItem(
                                                                                              "&Configure...", true),
                                                                                          new ExpectedTextMenuItem(
                                                                                              "&Disaster Recovery Wizard...",
                                                                                              true)
                                                                                      }));
            }

            expected.AddRange(new List<ExpectedMenuItem>{
                                   new ExpectedSeparator(),
                                   new ExpectedTextMenuItem("&Add Server", true, false,
                                                            new ExpectedMenuItem[]
                                                                {
                                                                    new ExpectedTextMenuItem("(empty)", false),
                                                                    new ExpectedSeparator(),
                                                                    new ExpectedTextMenuItem("&Add New Server...", true)
                                                                }),
                                   new ExpectedTextMenuItem("&Disconnect", true),
                                   new ExpectedTextMenuItem("Reconnec&t As...", true),
                                   new ExpectedSeparator(),
                                   new ExpectedTextMenuItem("Delete &Pool...",
                                                            pool.Connection.Cache.Hosts.Length > 1 ? false : true),
                                   new ExpectedSeparator(),
                                   new ExpectedTextMenuItem("E&xpand All", true),
                                   new ExpectedTextMenuItem("P&roperties", true)
                               });
            VerifyContextMenu(pool, expected.ToArray());
        }

        [Test]
        public void ContextMenu_Master()
        {
            VerifyContextMenu(GetAnyHost(IsMaster), new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("New V&M...", true),
                new ExpectedTextMenuItem("&New SR...", true),
                new ExpectedTextMenuItem("&Import...", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("&Enter Maintenance Mode...", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Re&boot", true),
                new ExpectedTextMenuItem("S&hut Down", true),
                new ExpectedTextMenuItem("Restart Toolstac&k", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("E&xpand All", true),
                new ExpectedTextMenuItem("P&roperties", true)
            });
        }

        [Test]
        public void ContextMenu_Slave()
        {
            VerifyContextMenu(GetAnyHost(IsSlave), new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("New V&M...", true),
                new ExpectedTextMenuItem("&New SR...", true),
                new ExpectedTextMenuItem("&Import...", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("&Enter Maintenance Mode...", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Re&boot", true),
                new ExpectedTextMenuItem("S&hut Down", true),
                new ExpectedTextMenuItem("Restart Toolstac&k", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("Remove Server from &Pool", false),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("E&xpand All", true),
                new ExpectedTextMenuItem("P&roperties", true)
            });
        }

        [Test]
        public void ContextMenu_VMWithTools()
        {
            foreach (VM vm in GetAllXenObjects<VM>(HasTools))
            {
                List<ExpectedMenuItem> expectedMenuItems = new List<ExpectedMenuItem>();

                expectedMenuItems.Add(new ExpectedTextMenuItem("S&hut Down", true));
                expectedMenuItems.Add(new ExpectedTextMenuItem("S&uspend", true));
                expectedMenuItems.Add(new ExpectedTextMenuItem("Reb&oot", true));
                expectedMenuItems.Add(new ExpectedSeparator());
                expectedMenuItems.Add(new ExpectedTextMenuItem("Force Shutdo&wn", true));
                expectedMenuItems.Add(new ExpectedTextMenuItem("Force Re&boot", true));
                expectedMenuItems.Add(new ExpectedSeparator());

                if (vm.Connection.Resolve<Host>(vm.resident_on).name_label == "inflames")
                {
                    expectedMenuItems.Add(new ExpectedTextMenuItem("M&igrate to Server", true, false,
                        new ExpectedMenuItem[]
                            {
                                new ExpectedTextMenuItem("&Home Server (Current server)", false),
                                new ExpectedTextMenuItem("inflames (Current server)", false, false, true),
                                new ExpectedTextMenuItem("incubus (INTERNAL_ERROR)", false, false, true)
                            }));
                    expectedMenuItems.Add(new ExpectedSeparator());
                }
                else if (Helpers.CowleyOrGreater(vm.Connection) && vm.IsOnSharedStorage() == string.Empty || !Helpers.CowleyOrGreater(vm.Connection))
                {
                    expectedMenuItems.Add(new ExpectedTextMenuItem("M&igrate to Server", true, false,
                        new ExpectedMenuItem[]
                            {
                                new ExpectedTextMenuItem("&Home Server (Current server)", false),
                                new ExpectedTextMenuItem("inflames (INTERNAL_ERROR)", false, false, true),
                                new ExpectedTextMenuItem("incubus (Current server)", false, false, true)
                            }));
                    expectedMenuItems.Add(new ExpectedSeparator());
                }


                expectedMenuItems.Add(new ExpectedTextMenuItem("Ta&ke a Snapshot...", true));
                if (Helpers.CowleyOrGreater(vm.Connection))
                    expectedMenuItems.Add(new ExpectedTextMenuItem("Assign to VM Protection Polic&y", true, new ExpectedMenuItem[] { new ExpectedTextMenuItem("&New Policy...", true), new ExpectedSeparator(), new ExpectedTextMenuItem("&1   Ewan's backups", true), new ExpectedTextMenuItem("&2   grage", true) }));
                expectedMenuItems.Add(new ExpectedSeparator());
                expectedMenuItems.Add(new ExpectedTextMenuItem("P&roperties", true));

                VerifyContextMenu(vm, expectedMenuItems.ToArray());
            }
        }

        [Test]
        public void ContextMenu_VMWithoutTools()
        {
            foreach (VM vm in GetAllXenObjects<VM>(NoTools))
            {
                VerifyContextMenu(vm, new ExpectedMenuItem[] {
                    new ExpectedTextMenuItem("Force Shutdo&wn", true),
                    new ExpectedTextMenuItem("Force Re&boot", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Ta&ke a Snapshot...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Install &XenServer Tools...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", true)
                });
            }
        }

        [Test]
        public void ContextMenu_VMShutdown()
        {
            foreach (VM vm in GetAllXenObjects<VM>(IsShutdown))
            {
                VerifyContextMenu(vm, new ExpectedMenuItem[] {
                    new ExpectedTextMenuItem("S&tart", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("Start on Ser&ver", true, false,
                        new ExpectedMenuItem[]
                            {
                                new ExpectedTextMenuItem("&Home Server (Home Server is not set)", false),
                                new ExpectedTextMenuItem("inflames (INTERNAL_ERROR)", false, false, true),
                                new ExpectedTextMenuItem("incubus (INTERNAL_ERROR)", false, false, true)
                            }),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Copy VM...", true),
                    new ExpectedTextMenuItem("&Export...", true),
                    new ExpectedTextMenuItem("Ta&ke a Snapshot...", true),
                    new ExpectedTextMenuItem("Co&nvert to Template...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Delete VM...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", true)
                });
            }
        }

        [Test]
        public void ContextMenu_SR()
        {
            EnsureChecked(MainWindowWrapper.ViewMenuItems.LocalStorageToolStripMenuItem);

            foreach (SR sr in GetAllXenObjects<SR>(s => !s.IsToolsSR))
            {
                List<ExpectedMenuItem> expectedMenuItems = new List<ExpectedMenuItem>();

                if (CanSetAsDefault(sr))
                {
                    expectedMenuItems.Add(new ExpectedTextMenuItem("Set as Defaul&t", true));
                    expectedMenuItems.Add(new ExpectedSeparator());
                }
                if (CanDetach(sr))
                {
                    expectedMenuItems.Add(new ExpectedTextMenuItem("&Detach...", true));
                }
                if (CanForget(sr))
                {
                    expectedMenuItems.Add(new ExpectedTextMenuItem("&Forget", true));
                }
                if (CanDestroy(sr))
                {
                    expectedMenuItems.Add(new ExpectedTextMenuItem("Destr&oy...", true));
                }
                if (expectedMenuItems.Count > 0 && !(CanSetAsDefault(sr) && expectedMenuItems.Count == 2))
                {
                    expectedMenuItems.Add(new ExpectedSeparator());
                }
                expectedMenuItems.Add(new ExpectedTextMenuItem("P&roperties", true));

                VerifyContextMenu(sr, expectedMenuItems.ToArray());
            }
        }

        [Test]
        public void ContextMenu_Snapshot()
        {
            try
            {
                var expectedForAll = new List<ExpectedMenuItem>
                                       {
                                           new ExpectedTextMenuItem("&New VM From Snapshot...", true),
                                           new ExpectedTextMenuItem("&Create Template From Snapshot...", true),
                                           new ExpectedTextMenuItem("&Export Snapshot As Template...", true),
                                           new ExpectedTextMenuItem("&Delete Snapshot", true),
                                           new ExpectedSeparator(),
                                           new ExpectedTextMenuItem("P&roperties", true)
                                       };

                PutInOrgView(OBJECT_VIEW);//objects view

                foreach (VM snapshot in GetAllXenObjects<VM>(v => v.is_a_snapshot))
                {
                    var expected = new List<ExpectedMenuItem>(expectedForAll);
                    VerifyContextMenu(snapshot, expected.ToArray());
                }

                PutInOrgView(ORGANIZATION_VIEW);//organization view

                foreach (VM snapshot in GetAllXenObjects<VM>(v => v.is_a_snapshot && (v.tags.Length > 0 || Folders.GetFolder(v) != null)))
                {
                    var expected = new List<ExpectedMenuItem>(expectedForAll);

                    if (Helpers.CowleyOrGreater(snapshot.Connection) && snapshot.tags.Length > 0)
                        expected.Insert(5, new ExpectedTextMenuItem("Untag Ob&ject", true));
                    if (Helpers.CowleyOrGreater(snapshot.Connection) && Folders.GetFolder(snapshot) != null)
                        expected.Insert(5, new ExpectedTextMenuItem("Remove from &folder", true));
                    VerifyContextMenu(snapshot, expected.ToArray());
                }
            }
            finally
            {
                PutInOrgView(INFRASTRUCTURE_VIEW);
            }
        }

        [Test]
        public void ContextMenu_VDI()
        {
            PutInOrgView(OBJECT_VIEW);
            try
            {
                foreach (VDI v in GetAllXenObjects<VDI>(v => v.name_label != "base copy" && !v.is_a_snapshot))
                {

                    VerifyContextMenu(v, new ExpectedMenuItem[] {
                        new ExpectedTextMenuItem("&Move Virtual Disk...", v.VBDs.Count == 0), 
                        new ExpectedTextMenuItem("&Delete Virtual Disk", v.allowed_operations.Contains(vdi_operations.destroy)),
                        new ExpectedSeparator(),
                        new ExpectedTextMenuItem("P&roperties", true),
                        
                    });
                }
            }
            finally
            {
                PutInOrgView(INFRASTRUCTURE_VIEW);
            }
        }

        [Test]
        public void ContextMenu_Network()
        {
            PutInOrgView(OBJECT_VIEW);
            try
            {
                foreach (XenAPI.Network network in GetAllXenObjects<XenAPI.Network>(n => n.name_label != "Guest installer network"))
                {
                    VerifyContextMenu(network, new ExpectedMenuItem[] {
                        new ExpectedTextMenuItem("P&roperties", true)
                    });
                }
            }
            finally
            {
                PutInOrgView(INFRASTRUCTURE_VIEW);
            }
        }

        [Test]
        public void ContextMenu_GroupingTag()
        {
            PutInOrgView(ORGANIZATION_VIEW);
            try
            {
                foreach (VirtualTreeNode node in GetAllTreeNodes())
                {
                    GroupingTag gt = node.Tag as GroupingTag;
                    if (gt != null)
                    {
                        var expectedItems = new List<ExpectedMenuItem>();

                        if (gt.Grouping is PropertyGrouping<string>)
                        {
                            expectedItems.Add(new ExpectedTextMenuItem("&Delete Tag...", true));
                            expectedItems.Add(new ExpectedTextMenuItem("&Rename Tag...", true));
                        }

                        AddExpectedExpandAndCollapseItems(node, expectedItems);

                        VerifyContextMenu(gt, expectedItems.ToArray());
                    }
                }
            }
            finally
            {
                PutInOrgView(INFRASTRUCTURE_VIEW);
            }
        }

        [Test]
        public void ContextMenu_Folder()
        {
            PutInOrgView(OBJECT_VIEW);
            try
            {
                foreach (VirtualTreeNode node in GetAllTreeNodes())
                {
                    Folder folder = node.Tag as Folder;

                    if (folder != null)
                    {
                        List<ExpectedMenuItem> expectedMenuItems = new List<ExpectedMenuItem>();

                        expectedMenuItems.Add(new ExpectedTextMenuItem("&New Folder...", true));

                        if (!folder.IsRootFolder)
                        {
                            expectedMenuItems.Add(new ExpectedTextMenuItem("&Rename Folder...", true));
                            expectedMenuItems.Add(new ExpectedTextMenuItem("&Delete Folder...", true));
                        }

                        AddExpectedExpandAndCollapseItems(node, expectedMenuItems);

                        VerifyContextMenu(folder, expectedMenuItems.ToArray());
                    }
                }
            }
            finally
            {
                PutInOrgView(INFRASTRUCTURE_VIEW);
            }
        }

        [Test]
        public void ContextMenu_DefaultTemplate()
        {
            EnsureDefaultTemplatesShown();

            VerifyContextMenu(GetAnyDefaultTemplate(), new ExpectedMenuItem[] {
                new ExpectedTextMenuItem("&New VM wizard...", true),
                new ExpectedSeparator(),                
                new ExpectedTextMenuItem("&Export to File...", true),
                new ExpectedTextMenuItem("&Copy...", true),
                new ExpectedSeparator(),
                new ExpectedTextMenuItem("P&roperties", true)
            });
        }

        [Test]
        public void ContextMenu_UserTemplate_Instant()
        {
            EnsureDefaultTemplatesShown();

            foreach (VM vm in GetAllXenObjects<VM>(v => v.InstantTemplate))
            {
                VerifyContextMenu(vm, new ExpectedMenuItem[] {
                    new ExpectedTextMenuItem("&New VM wizard...", true),
                    new ExpectedTextMenuItem("&Quick Create", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Export to File...", true),
                    new ExpectedTextMenuItem("&Copy...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Delete Template...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", true)
                });
            }
        }

        [Test]
        public void ContextMenu_UserTemplate_NoInstant()
        {
            EnsureDefaultTemplatesShown();

            foreach (VM vm in GetAllXenObjects<VM>(v => v.is_a_template && !v.DefaultTemplate && !v.is_a_snapshot && !v.InstantTemplate))
            {
                VerifyContextMenu(vm, new ExpectedMenuItem[] {
                    new ExpectedTextMenuItem("&New VM wizard...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Export to File...", true),
                    new ExpectedTextMenuItem("&Copy...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("&Delete Template...", true),
                    new ExpectedSeparator(),
                    new ExpectedTextMenuItem("P&roperties", true)
                });
            }
        }
    }
}
