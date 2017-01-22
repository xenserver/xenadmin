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
using XenAdmin.Plugins;
using System.IO;
using System.Diagnostics;
using XenAdmin;
using System.Threading;
using XenAPI;
using XenAdmin.Commands;
using XenAdminTests.UnitTests;

namespace XenAdminTests.PluginTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class ShellCmdTests
    {
        private PluginManager _pluginManager;
        private TestPluginLoader _pluginLoader;
        private ShellCmdTestBatchFile _batchFile;

        [TearDown]
        public void TearDown()
        {
            _pluginLoader.Dispose();
            _pluginManager.Dispose();
            _batchFile.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _pluginManager = new PluginManager();
        }

        [Test]
        public void TestLoadPlugin()
        {
            _batchFile = new ShellCmdTestBatchFile();

            _pluginLoader = new TestPluginLoader("ShellCmdTestPlugin", _pluginManager, _batchFile.Xml);
            _pluginLoader.Load();

            Assert.IsTrue(File.Exists(_batchFile.BatchFile), "test batch file wasn't written");
            Assert.IsFalse(File.Exists(_batchFile.FileThatBatchFileWillWrite), "test file shouldn't exist");

            Assert.AreEqual(1, _pluginManager.Plugins.Count, "No plugins loaded.");
            Assert.AreEqual("ShellCmdTestPlugin", _pluginManager.Plugins[0].Name, "Plugin Name incorrect");
            Assert.AreEqual("plugin-vendor", _pluginManager.Plugins[0].Organization, "Plugin Vendor incorrect");
            Assert.IsTrue(_pluginManager.Enabled, "Plugin manager isn't enabled");
            Assert.IsNull(_pluginManager.Plugins[0].Error, "Error when loading plugin manager");
            Assert.AreEqual(1, _pluginManager.Plugins[0].Features.Count, "MenuItem feature wasn't loaded");

            MenuItemFeature menuItemFeature = (MenuItemFeature)_pluginManager.Plugins[0].Features[0];

            Assert.IsNotNull(menuItemFeature.ShellCmd, "ShellCmd wasn't loaded");
            Assert.AreEqual(menuItemFeature.ShellCmd.Filename, _batchFile.BatchFile, "Batch file name wasn't read from plugin XML");

            // now execute the command and see if the test file is written by the batch file.
            menuItemFeature.GetCommand(new MockMainWindow(), new SelectedItem[] { new SelectedItem((IXenObject)null) }).Execute();

            int i = 0;
            bool completed = false;
            while (i < 300)
            {
                if (File.Exists(_batchFile.FileThatBatchFileWillWrite))
                {
                    completed = true;
                    break;
                }

                Thread.Sleep(100);
                i++;
            }

            Assert.IsTrue(completed, "test didn't complete");
        }
    }
}
