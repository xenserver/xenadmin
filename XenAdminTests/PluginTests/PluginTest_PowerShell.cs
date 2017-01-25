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
using XenAdmin;
using XenAdmin.Plugins;
using System.Windows.Forms;
using System.IO;
using XenAdmin.Plugins.UI;
using XenAdminTests.DialogTests;
using XenAdminTests.DialogTests.PluginDialogTests;
using XenAdmin.Core;

namespace XenAdminTests.PluginTests
{
    [TestFixture, Ignore]
    public class PluginTest_PowerShell : MainWindowLauncher_TestFixture
    {
        [TestFixtureSetUp]
        public void SetUp_PowerShellPlugin()
        {
            MW(delegate()
            {
                List<string> enabled_plugins = new List<string>(Settings.EnabledPluginsList());
                enabled_plugins.Add("UnitTest::PowerShellPlugin");
                Settings.UpdateEnabledPluginsList(enabled_plugins.ToArray());
                Manager.ProcessPlugin(Path.Combine(Program.AssemblyDir, "TestResources\\PluginResources\\PowerShellPlugin.xcplugin.xml"), "PowerShellPlugin", "UnitTest");
            });
        }

        // causes System.ArgumentException : Delegate to an instance method cannot have null 'this'
        //[Test]
        //public void Test_PluginDialog()
        //{
        //    new PluginDialogTest().RunDialogTests();
        //}

        [Test]
        public void Test_File_PowerShell()
        {
            if (Registry.IsPowerShellInstalled())
                ClickMenuItem("File", "file_PowerShellTest1");
            else
                CheckMenuItemMissing("File", "file_PowerShellTest1");
        }

        [Test]
        public void Test_View_PowerShell()
        {
            if (Registry.IsPowerShellInstalled())
                ClickMenuItem("View", "view_PowerShellTest1");
            else
                CheckMenuItemMissing("View", "view_PowerShellTest1");
        }

        [Test]
        public void Test_Pool_PowerShell()
        {
            if (Registry.IsPowerShellInstalled())
                ClickMenuItem("Pool", "pool_PowerShellTest1");
            else
                CheckMenuItemMissing("Pool", "pool_PowerShellTest1");
        }

        [Test]
        public void Test_Server_PowerShell()
        {
            if (Registry.IsPowerShellInstalled())
                ClickMenuItem("Server", "server_PowerShellTest1");
            else
                CheckMenuItemMissing("Server", "server_PowerShellTest1");
        }

        [Test]
        public void Test_VM_PowerShell()
        {
            if (Registry.IsPowerShellInstalled())
                ClickMenuItem("VM", "vm_PowerShellTest1");
            else
                CheckMenuItemMissing("VM", "vm_PowerShellTest1");
        }

        [Test]
        public void Test_Storage_PowerShell()
        {
            if (Registry.IsPowerShellInstalled())
                ClickMenuItem("Storage", "storage_PowerShellTest1");
            else
                CheckMenuItemMissing("Storage", "storage_PowerShellTest1");
        }

        [Test]
        public void Test_Templates_PowerShell()
        {
            if (Registry.IsPowerShellInstalled())
                ClickMenuItem("Templates", "templates_PowerShellTest1");
            else
                CheckMenuItemMissing("Templates", "templates_PowerShellTest1");
        }

        [Test]
        public void Test_Tools_PowerShell()
        {
            if (Registry.IsPowerShellInstalled())
                ClickMenuItem("Tools", "tools_PowerShellTest1");
            else
                CheckMenuItemMissing("Tools", "tools_PowerShellTest1");
        }

        [Test]
        public void Test_Window_PowerShell()
        {
            if (Registry.IsPowerShellInstalled())
                ClickMenuItem("Window", "window_PowerShellTest1");
            else
                CheckMenuItemMissing("Window", "window_PowerShellTest1");
        }

        [Test]
        public void Test_Help_PowerShell()
        {
            if (Registry.IsPowerShellInstalled())
                ClickMenuItem("Help", "help_PowerShellTest1");
            else
                CheckMenuItemMissing("Help", "help_PowerShellTest1");
        }

        [TestFixtureTearDown]
        public void TearDown_PowerShellPlugin()
        {
            MW(delegate()
            {
                Descriptor plugin = Manager.Plugins.Find(new Predicate<Descriptor>(delegate(Descriptor d)
                {
                    return d.Name == "PowerShellPlugin";
                }));
                plugin.Dispose();
                Manager.Plugins.Remove(plugin);

                List<string> enabled_plugins = new List<string>(Settings.EnabledPluginsList());
                enabled_plugins.Remove("UnitTest::PowerShellPlugin");
                Settings.UpdateEnabledPluginsList(enabled_plugins.ToArray());
            });
        }
    }
}
