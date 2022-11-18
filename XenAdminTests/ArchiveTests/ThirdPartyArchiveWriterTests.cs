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
using System.IO;
using System.Text;
using NUnit.Framework;
using XenCenterLib.Archive;

namespace XenAdminTests.ArchiveTests
{
    [TestFixture(typeof(TarArchiveIterator), typeof(TarArchiveWriter))]
    [TestFixture(typeof(ZipArchiveIterator), typeof(ZipArchiveWriter))]
    [Category(TestCategories.Unit)]
    public class ThirdPartyArchiveWriterTest<TR, TW>
        where TR : ArchiveIterator, new()
        where TW : ArchiveWriter, new()
    {
        private string tempPath;

        [SetUp]
        public void TestSetUp()
        {
            tempPath = CreateTempFolder();
        }

        [TearDown]
        public void TestTearDown()
        {
            try
            {
                Directory.Delete(tempPath, true);
            }
            catch
            {
                //ignore
            }
        }

        [Test]
        public void CreateAnArchiveAndRereadWhenPossible()
        {
            string tarFileName = Path.Combine(tempPath, Path.GetRandomFileName());
            CheckWriteArchive(tarFileName);
            CheckReadArchive(tarFileName);
        }

        // This test knows the contents of the file written in the archive 
        private void CheckReadArchive(string archiveName)
        {
            var expectedDirs = new List<string> {"adir/", "adir2/a/"};
            var expectedFiles = new List<string> {"tf1", "adir2/a/tf2"};

            using (var fs = File.OpenRead(archiveName))
            using (var reader = new TR())
            {
                reader.SetBaseStream(fs);
                while (reader.HasNext())
                {
                    if (reader.IsDirectory() && expectedDirs.Contains(reader.CurrentFileName()))
                        expectedDirs.Remove(reader.CurrentFileName());

                    if (!reader.IsDirectory() && expectedFiles.Contains(reader.CurrentFileName()))
                    {
                        expectedFiles.Remove(reader.CurrentFileName());
                        using (MemoryStream ms = new MemoryStream())
                        {
                            reader.ExtractCurrentFile(ms, null);
                            Assert.IsTrue(ms.Length > 0, "Extracted file contents have data");
                        }
                    }
                }

                Assert.AreEqual(0, expectedFiles.Count, "Expected files count");
                Assert.AreEqual(0, expectedDirs.Count, "Expected dir count");
            }
        }

        private string CreateTempFolder()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);
            return tempPath;
        }

        private void CheckWriteArchive(string tarFileName)
        {
            using (var ms = File.OpenWrite(tarFileName))
            using (var writer = new TW())
            {
                Assert.AreEqual(0, ms.Length);
                writer.SetBaseStream(ms);

                using (MemoryStream ms1 = new MemoryStream(Encoding.ASCII.GetBytes("This is a test")))
                {
                    writer.Add(ms1, "tf1", DateTime.Now, null);
                }

                using (MemoryStream ms2 = new MemoryStream(Encoding.ASCII.GetBytes("This is a test")))
                {
                    writer.Add(ms2, "adir2/a/tf2", DateTime.Now, null);
                }

                writer.AddDirectory("adir/", DateTime.Now);
                writer.AddDirectory("adir2/a", DateTime.Now);
            }

            Assert.IsTrue(File.Exists(tarFileName), "archive exists");
            Assert.IsTrue(File.ReadAllBytes(tarFileName).Length > 0, "archive has contents");
        }
    }
}
