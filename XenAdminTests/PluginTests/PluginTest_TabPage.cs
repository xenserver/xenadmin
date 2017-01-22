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
using NUnit.Framework;
using System.Windows.Forms;
using XenAdmin.Plugins;
using XenAdmin;
using System.IO;
using XenAdminTests.TabsAndMenus;
using System.Threading;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.XenSearch;
using XenAdmin.Plugins.UI;
using XenAdminTests.DialogTests;
using XenAdminTests.DialogTests.PluginDialogTests;

namespace XenAdminTests.PluginTests
{
    [TestFixture, Ignore]
    public class PluginTest_TabPage : TabsAndMenus.TabsAndMenus
    {
        public PluginTest_TabPage()
            : base("state3.xml", "snapshots.xml")
        {

        }

        [TestFixtureSetUp]
        public void SetUp_TabPagePlugin()
        {
            CheckHelp = false;
            List<string> enabled_plugins = new List<string>(Settings.EnabledPluginsList());
            enabled_plugins.Add("UnitTest::TabPagePlugin");
            Settings.UpdateEnabledPluginsList(enabled_plugins.ToArray());
            
            MW(delegate()
            {
                Manager.ProcessPlugin(Path.Combine(Program.AssemblyDir, "TestResources\\PluginResources\\TabPagePlugin.xcplugin.xml"), "TabPagePlugin", "UnitTest");
            });
        }

        // causes System.ArgumentException : Delegate to an instance method cannot have null 'this'
        //[Test]
        //public void Test_PluginDialog()
        //{
        //    new PluginDialogTest().RunDialogTests();
        //}

        private string[] InsertPluginTabs(string[] tabs, params string[] plugintabs)
        {
            List<string> newtabs = new List<string>(tabs);
            newtabs.InsertRange(newtabs.Count - 1, plugintabs);
            return newtabs.ToArray();
        }

        [Test]
        public void Test_XenCenter_TabPage()
        {
            MW(Program.MainWindow.UpdateToolbarsCore);
            VerifyTabs(null, InsertPluginTabs(TabsAndMenusGeorge.XenCenterTabs, "XenCenterTabPageTest"));
        }

        [Test]
        public void Test_Pool_TabPage()
        {
            MW(Program.MainWindow.UpdateToolbarsCore);
            VerifyTabs(GetAnyPool(), InsertPluginTabs(TabsAndMenusGeorge.PoolTabs, "PoolTabPageTest", "AllTabPageTest"));
        }

        [Test]
        public void Test_Server_TabPage()
        {
            MW(Program.MainWindow.UpdateToolbarsCore);
            VerifyTabs(GetAnyHost(), InsertPluginTabs(TabsAndMenusGeorge.HostTabs, "ServerTabPageTest", "AllTabPageTest"));
        }

        [Test]
        public void Test_VM_TabPage()
        {
            MW(Program.MainWindow.UpdateToolbarsCore);
            VerifyTabs(GetAnyVM(), InsertPluginTabs(TabsAndMenusGeorge.VMTabs, "VMTabPageTest", "AllTabPageTest"));
        }

        [Test]
        public void Test_SR_TabPage()
        {
            MW(Program.MainWindow.UpdateToolbarsCore);
            VerifyTabs(GetAnySR(), InsertPluginTabs(TabsAndMenusGeorge.SRTabs, "SRTabPageTest", "AllTabPageTest"));
        }

        [Test]
        public void Test_CustomTemplate_TabPage()
        {
            MW(Program.MainWindow.UpdateToolbarsCore);
            VerifyTabs(GetAnyUserTemplate(delegate(VM vm) { return vm.HasProvisionXML; }), InsertPluginTabs(TabsAndMenusGeorge.UserTemplateTabs_Provision, "UserTemplateTabPageTest", "AllTabPageTest"));
        }

        [Test]
        public void Test_DefaultTemplate_TabPage()
        {
            MW(Program.MainWindow.UpdateToolbarsCore);
            VerifyTabs(GetAnyDefaultTemplate(), InsertPluginTabs(TabsAndMenusGeorge.DefaultTemplateTabs, "DefaultTemplateTabPageTest", "AllTabPageTest"));
        }

        [Test]
        public void Test_UrlReplacement_DefaultTemplate()
        {
            MW(delegate()
            {
                SelectInTree(GetAnyDefaultTemplate());
                Placeholders.SubstituteURL(GetAllProperties(), Program.MainWindow.SelectedXenObject);
            });
        }

        [Test]
        public void Test_UrlReplacement_Folder()
        {
            MW(delegate()
            {
                SelectInTree(GetAnyFolder());
                Placeholders.SubstituteURL(GetAllProperties(), Program.MainWindow.SelectedXenObject);
            });
        }

        [Test]
        public void Test_UrlReplacement_Host()
        {
            MW(delegate()
            {
                SelectInTree(GetAnyHost());
                Placeholders.SubstituteURL(GetAllProperties(), Program.MainWindow.SelectedXenObject);
            });
        }

        [Test]
        public void Test_UrlReplacement_Network()
        {
            MW(delegate()
            {
                SelectInTree(GetAnyNetwork());
                Placeholders.SubstituteURL(GetAllProperties(), Program.MainWindow.SelectedXenObject);
            });
        }

        [Test]
        public void Test_UrlReplacement_Pool()
        {
            MW(delegate()
            {
                SelectInTree(GetAnyPool());
                Placeholders.SubstituteURL(GetAllProperties(), Program.MainWindow.SelectedXenObject);
            });
        }

        [Test]
        public void Test_UrlReplacement_PoolOfOne()
        {
            MW(delegate()
            {
                SelectInTree(GetAnyPoolOfOne());
                Placeholders.SubstituteURL(GetAllProperties(), Program.MainWindow.SelectedXenObject);
            });
        }

        [Test]
        public void Test_UrlReplacement_Snapshot()
        {
            MW(delegate()
            {
                SelectInTree(GetAnySnapshot());
                Placeholders.SubstituteURL(GetAllProperties(), Program.MainWindow.SelectedXenObject);
            });
        }

        [Test]
        public void Test_UrlReplacement_SR()
        {
            MW(delegate()
            {
                SelectInTree(GetAnySR());
                Placeholders.SubstituteURL(GetAllProperties(), Program.MainWindow.SelectedXenObject);
            });
        }

        [Test]
        public void Test_UrlReplacement_UserTemplate()
        {
            MW(delegate()
            {
                SelectInTree(GetAnyUserTemplate());
                Placeholders.SubstituteURL(GetAllProperties(), Program.MainWindow.SelectedXenObject);
            });
        }

        [Test]
        public void Test_UrlReplacement_VBD()
        {
            MW(delegate()
            {
                SelectInTree(GetAnyVBD());
                Placeholders.SubstituteURL(GetAllProperties(), Program.MainWindow.SelectedXenObject);
            });
        }

        [Test]
        public void Test_UrlReplacement_VIF()
        {
            MW(delegate()
            {
                SelectInTree(GetAnyVIF());
                Placeholders.SubstituteURL(GetAllProperties(), Program.MainWindow.SelectedXenObject);
            });
        }

        [Test]
        public void Test_UrlReplacement_VM()
        {
            MW(delegate()
            {
                SelectInTree(GetAnyVM());
                Placeholders.SubstituteURL(GetAllProperties(), Program.MainWindow.SelectedXenObject);
            });
        }

        [TestFixtureTearDown]
        public void TearDown_TabPagePlugin()
        {
            MW(delegate()
            {
                Descriptor plugin = Manager.Plugins.Find(new Predicate<Descriptor>(delegate(Descriptor d)
                {
                    return d.Name == "TabPagePlugin";
                }));
                plugin.Dispose();
                Manager.Plugins.Remove(plugin);

                List<string> enabled_plugins = new List<string>(Settings.EnabledPluginsList());
                enabled_plugins.Remove("UnitTest::TabPagePlugin");
                Settings.UpdateEnabledPluginsList(enabled_plugins.ToArray());
            });
        }

        private string GetAllProperties()
        {
            StringBuilder builder = new StringBuilder("http://");
            foreach (string prop in Enum.GetNames(typeof(PropertyNames)))
            {
                builder.AppendFormat("{{${0}}}", prop);
            }
            return builder.ToString();
        }
    }
}
