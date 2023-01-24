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

using System.Collections;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Reflection;
using NUnit.Framework;
using XenAdmin.Core;


namespace XenAdminTests.CodeTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class AssemblyTests
    {
        private static readonly string MainAssemblyName = BrandManager.BrandConsoleNoSpace;

        public class TestDataClass
        {
            public string AssemblyName { get; set; }
            public string Locale { get; set; }
        }

        private static IEnumerable<TestDataClass> TestCasesForI18NFiles
        {
            get
            {
                yield return new TestDataClass {AssemblyName = MainAssemblyName, Locale = "ja"};
                yield return new TestDataClass {AssemblyName = MainAssemblyName, Locale = "zh-CN"};
                yield return new TestDataClass {AssemblyName = "XenModel", Locale = "ja"};
                yield return new TestDataClass {AssemblyName = "XenModel", Locale = "zh-CN"};
                yield return new TestDataClass {AssemblyName = "XenOvf", Locale = "ja"};
                yield return new TestDataClass {AssemblyName = "XenOvf", Locale = "zh-CN"};
            }
        }

        private static IEnumerable<TestDataClass> TestCasesForRoaming
        {
            get
            {
                yield return new TestDataClass {AssemblyName = MainAssemblyName};
                yield return new TestDataClass {AssemblyName = "XenOvf"};
                yield return new TestDataClass {AssemblyName = "XenServerHealthCheck"};
            }
        }


        [Test]
        [Description("Checks that there are no Bitmaps in XenAdmin.Resources" +
                     "without a counterpart in XenAdmin.Images.StaticImages")]
        public void TestEnsureStaticImages()
        {
            var manager = new System.Resources.ResourceManager("XenAdmin.Properties.Resources",
                typeof(XenAdmin.Properties.Resources).Assembly);

            var images = manager.GetResourceSet(CultureInfo.CurrentCulture, true, true)
                .Cast<DictionaryEntry>().Where(e => e.Value is Bitmap).ToList();

            var staticImages = typeof(XenAdmin.Images.StaticImages).GetFields()
                .Where(f => f.IsStatic && f.FieldType == typeof(Bitmap)).ToList();

            var extraImages = (from DictionaryEntry e in images
                let key = e.Key as string
                where !staticImages.Exists(s => s.Name == key)
                select key).ToList();

            var extraStatics = (from FieldInfo s in staticImages
                where !images.Exists(i => s.Name == (string)i.Key)
                select s.Name).ToList();

            Assert.True(extraStatics.Count == 0 && extraImages.Count == 0,
                $"Orphaned static images: {string.Join(", ", extraStatics)}\n" +
                $"Resources without a static counterpart: {string.Join(", ", extraImages)}");
        }

        [Test, TestCaseSource(typeof(AssemblyTests), nameof(TestCasesForI18NFiles))]
        [Description("Checks all resx files in the project have their i18n counterparts in place")]
        public void TestEnsureI18NFilesInPlace(TestDataClass tc)
        {
            var assembly = FindAssemblyByNameRecursively(tc.AssemblyName);
            Assert.NotNull($"Assembly {tc.AssemblyName} was not found.");

            var excludeFromCheck = new[] {"XenAdmin.Help.HelpManager", "DotNetVnc.KeyMap"};
            var missing = new List<string>();
            var extra = new List<string>();

            List<string> defaultResx = new List<string>(assembly.GetManifestResourceNames().Where(resource => resource.EndsWith("resources")));

            CultureInfo cultureInfo = new CultureInfo(tc.Locale);
            Assembly localeDll = assembly.GetSatelliteAssembly(cultureInfo);
            List<string> localeResx = new List<string>(localeDll.GetManifestResourceNames());

            foreach (string def in defaultResx)
            {
                var name = def.Substring(0, def.Length - ".resources".Length);
                var exclude = excludeFromCheck.Contains(name);

                string localName = $"{name}.{tc.Locale}.resources";
                var localized = localeResx.Contains(localName);

                if (localized && exclude)
                    extra.Add(localName);
                else if (!localized && !exclude)
                    missing.Add(localName);
            }

            Assert.IsEmpty(missing, "Missing resources detected.");
            Assert.IsEmpty(extra, "Unnecessary resources detected.");
        }

        [Test, TestCaseSource(typeof(AssemblyTests), nameof(TestCasesForRoaming))]
        [Description("Checks that if there are user-scoped settings in an assembly, these have roaming=true")]
        public void TestUserSettingsAreRoaming(TestDataClass tc)
        {
            var assembly = FindAssemblyByNameRecursively(tc.AssemblyName);
            Assert.NotNull($"Assembly {tc.AssemblyName} was not found.");

            var nonRoaming = new List<string>();

            foreach (var type in assembly.GetTypes())
            {
                if (type.Name == "Settings" && typeof(SettingsBase).IsAssignableFrom(type))
                {
                    var properties = ((SettingsBase)type.GetProperty("Default").GetValue(null, null)).Properties;

                    foreach (SettingsProperty prop in properties)
                    {
                        bool userScope = false;
                        bool roaming = false;

                        foreach (var val in prop.Attributes.Values)
                        {
                            if (val is SettingsManageabilityAttribute sma && sma.Manageability == SettingsManageability.Roaming)
                                roaming = true;
                            else if (val is UserScopedSettingAttribute)
                                userScope = true;
                        }

                        if (userScope && !roaming)
                            nonRoaming.Add(prop.Name);
                    }
                }
            }

            Assert.IsEmpty(nonRoaming, "Detected user-scoped settings that are not roaming.");
        }

        #region Auxiliary private methods

        /// <summary>
        /// Some of the assemblies we need to check may not be in the manifest of the assembly we're
        /// looking at, but are referenced by assemblies in the manifest. This method retrieves them.
        /// </summary>
        private Assembly FindAssemblyByNameRecursively(string name, Assembly startAssembly = null)
        {
            if (startAssembly == null)
                startAssembly = Assembly.GetExecutingAssembly();

            var assemblyNames = startAssembly.GetReferencedAssemblies();
            var assemblyName = assemblyNames.FirstOrDefault(an => an.Name == name);

            if (assemblyName == null)
            {
                foreach (var temp in assemblyNames)
                {
                    var found = FindAssemblyByNameRecursively(name, Assembly.Load(temp));
                    if (found != null)
                        return found;
                }
            }
            else
                return Assembly.Load(assemblyName);

            return null;
        }

        #endregion
    }
}

