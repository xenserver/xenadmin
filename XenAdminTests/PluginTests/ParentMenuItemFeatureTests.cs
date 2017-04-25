﻿/* Copyright (c) Citrix Systems, Inc. 
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

namespace XenAdminTests.PluginTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class ParentMenuItemFeatureTests
    {
        private PluginManager _pluginManager;
        private TestPluginLoader _pluginLoader;

        [TearDown]
        public void TearDown()
        {
            _pluginLoader.Dispose();
            _pluginManager.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _pluginManager = new PluginManager();
        }

        [Test]
        public void TestLoadPlugin()
        {
            _pluginLoader = new TestPluginLoader("ParentMenuItemFeatureTestPlugin", _pluginManager);
            _pluginLoader.Load();

            Assert.AreEqual(1, _pluginManager.Plugins.Count, "No plugins loaded.");
            Assert.AreEqual(1, _pluginManager.Plugins[0].Features.Count, "No features only");
            Assert.IsInstanceOf<ParentMenuItemFeature>(_pluginManager.Plugins[0].Features[0], "ParentMenuItemFeature wasn't loaded");

            ParentMenuItemFeature parentFeature = (ParentMenuItemFeature)_pluginManager.Plugins[0].Features[0];

            Assert.AreEqual(1, parentFeature.Features.Count, "Child feature wasn't loaded.");
            Assert.AreEqual("file_ShellTest1", parentFeature.Features[0].Name);
            Assert.IsInstanceOf<MenuItemFeature>(parentFeature.Features[0]);
        }
    }
}
