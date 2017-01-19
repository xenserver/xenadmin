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
using System.IO;
using System.Reflection;
using XenAdmin.Plugins;
using XenAdmin;

namespace XenAdminTests.PluginTests
{
    internal class TestPluginLoader : IDisposable
    {
        private readonly string _name;
        private readonly string _xml;
        private readonly PluginManager _pluginManager;
        private string _fullFolder;

        public TestPluginLoader(string name, PluginManager pluginManager)
        {
            _name = name;
            _pluginManager = pluginManager;
        }

        public TestPluginLoader(string name, PluginManager pluginManager, string xml)
            : this(name, pluginManager)
        {
            _xml = xml;
        }

        public void Load()
        {
            string tempPath = Path.GetTempPath();
            string pluginsFolder = Guid.NewGuid().ToString();
            string vendorDir = "plugin-vendor";

            _fullFolder = tempPath + "\\" + pluginsFolder + "\\" + vendorDir + "\\" + _name;
            string fullFile = _fullFolder + "\\" + _name + ".xcplugin.xml";

            Directory.CreateDirectory(_fullFolder);

            if (_xml == null)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream s = assembly.GetManifestResourceStream("XenAdminTests.TestResources.PluginResources." + _name + ".xcplugin.xml");

                byte[] bytes = new byte[s.Length];
                s.Read(bytes, 0, bytes.Length);

                File.WriteAllBytes(fullFile, bytes);
            }
            else
            {
                File.WriteAllText(fullFile, _xml);
            }

            _pluginManager.LoadPlugins(tempPath + "\\" + pluginsFolder);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (Directory.Exists(_fullFolder))
            {
                Directory.Delete(_fullFolder, true);
            }
        }

        #endregion
    }
}
