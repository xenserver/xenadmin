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
using XenAdmin;
using XenAdmin.Plugins;
using System.IO;
using XenAdmin.Plugins.UI;
using XenAdminTests.DialogTests;
using XenAdminTests.DialogTests.PluginDialogTests;
using XenAdmin.Core;

namespace XenAdminTests.PluginTests
{
    [TestFixture, Ignore]
    public class PluginTest_XenServerPowerShell : MainWindowLauncher_TestFixture
    {
        [TestFixtureSetUp]
        public void SetUp_XenServerPowerShellPlugin()
        {
            MW(delegate()
            {
                List<string> enabled_plugins = new List<string>(Settings.EnabledPluginsList());
                enabled_plugins.Add("UnitTest::XenServerPowerShellPlugin");
                Settings.UpdateEnabledPluginsList(enabled_plugins.ToArray());
                Manager.ProcessPlugin(Path.Combine(Program.AssemblyDir, "TestResources\\PluginResources\\XenServerPowerShellPlugin.xcplugin.xml"), "XenServerPowerShellPlugin", "UnitTest");
            });
        }

        // causes System.ArgumentException : Delegate to an instance method cannot have null 'this'
        //[Test]
        //public void Test_PluginDialog()
        //{
        //    new PluginDialogTest().RunDialogTests();
        //}

        [Test]
        public void Test_File_XenServerPowerShell1()
        {
            if(Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("File", "file_XenServerPowerShellTest1");
            else
                CheckMenuItemMissing("File", "file_XenServerPowerShellTest1");
        }

        [Test]
        public void Test_View_XenServerPowerShell1()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("View", "view_XenServerPowerShellTest1");
            else
                CheckMenuItemMissing("View", "view_XenServerPowerShellTest1");
        }

        [Test]
        public void Test_Pool_XenServerPowerShell1()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Pool", "pool_XenServerPowerShellTest1");
            else
                CheckMenuItemMissing("Pool", "pool_XenServerPowerShellTest1");
        }

        [Test]
        public void Test_Server_XenServerPowerShell1()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Server", "server_XenServerPowerShellTest1");
            else
                CheckMenuItemMissing("Server", "server_XenServerPowerShellTest1");
        }

        [Test]
        public void Test_VM_XenServerPowerShell1()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("VM", "vm_XenServerPowerShellTest1");
            else
                CheckMenuItemMissing("VM", "vm_XenServerPowerShellTest1");
        }

        [Test]
        public void Test_Storage_XenServerPowerShell1()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Storage", "storage_XenServerPowerShellTest1");
            else
                CheckMenuItemMissing("Storage", "storage_XenServerPowerShellTest1");
        }

        [Test]
        public void Test_Templates_XenServerPowerShell1()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Templates", "templates_XenServerPowerShellTest1");
            else
                CheckMenuItemMissing("Templates", "templates_XenServerPowerShellTest1");
        }

        [Test]
        public void Test_Tools_XenServerPowerShell1()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Tools", "tools_XenServerPowerShellTest1");
            else
                CheckMenuItemMissing("Tools", "tools_XenServerPowerShellTest1");
        }

        [Test]
        public void Test_Window_XenServerPowerShell1()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Window", "window_XenServerPowerShellTest1");
            else
                CheckMenuItemMissing("Window", "window_XenServerPowerShellTest1");
        }

        [Test]
        public void Test_Help_XenServerPowerShell1()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Help", "help_XenServerPowerShellTest1");
            else
                CheckMenuItemMissing("Help", "help_XenServerPowerShellTest1");
        }

        [Test]
        public void Test_File_XenServerPowerShell2()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("File", "file_XenServerPowerShellTest2");
            else
                CheckMenuItemMissing("File", "file_XenServerPowerShellTest2");
        }

        [Test]
        public void Test_View_XenServerPowerShell2()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("View", "view_XenServerPowerShellTest2");
            else
                CheckMenuItemMissing("View", "view_XenServerPowerShellTest2");
        }

        [Test]
        public void Test_Pool_XenServerPowerShell2()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Pool", "pool_XenServerPowerShellTest2");
            else
                CheckMenuItemMissing("Pool", "pool_XenServerPowerShellTest2");
        }

        [Test]
        public void Test_Server_XenServerPowerShell2()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Server", "server_XenServerPowerShellTest2");
            else
                CheckMenuItemMissing("Server", "server_XenServerPowerShellTest2");
        }

        [Test]
        public void Test_VM_XenServerPowerShell2()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("VM", "vm_XenServerPowerShellTest2");
            else
                CheckMenuItemMissing("VM", "vm_XenServerPowerShellTest2");
        }

        [Test]
        public void Test_Storage_XenServerPowerShell2()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Storage", "storage_XenServerPowerShellTest2");
            else
                CheckMenuItemMissing("Storage", "storage_XenServerPowerShellTest2");
        }

        [Test]
        public void Test_Templates_XenServerPowerShell2()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Templates", "templates_XenServerPowerShellTest2");
            else
                CheckMenuItemMissing("Templates", "templates_XenServerPowerShellTest2");
        }

        [Test]
        public void Test_Tools_XenServerPowerShell2()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Tools", "tools_XenServerPowerShellTest2");
            else
                CheckMenuItemMissing("Tools", "tools_XenServerPowerShellTest2");
        }

        [Test]
        public void Test_Window_XenServerPowerShell2()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Window", "window_XenServerPowerShellTest2");
            else
                CheckMenuItemMissing("Window", "window_XenServerPowerShellTest2");
        }

        [Test]
        public void Test_Help_XenServerPowerShell2()
        {
            if (Registry.IsXenServerSnapInInstalled())
                ClickMenuItem("Help", "help_XenServerPowerShellTest2");
            else
                CheckMenuItemMissing("Help", "help_XenServerPowerShellTest2");
        }

        [TestFixtureTearDown]
        public void TearDown_XenServerPowerShellPlugin()
        {
            MW(delegate()
            {
                Descriptor plugin = Manager.Plugins.Find(new Predicate<Descriptor>(delegate(Descriptor d)
                {
                    return d.Name == "XenServerPowerShellPlugin";
                }));
                plugin.Dispose();
                Manager.Plugins.Remove(plugin);

                List<string> enabled_plugins = new List<string>(Settings.EnabledPluginsList());
                enabled_plugins.Remove("UnitTest::XenServerPowerShellPlugin");
                Settings.UpdateEnabledPluginsList(enabled_plugins.ToArray());
            });
        }
    }
}
