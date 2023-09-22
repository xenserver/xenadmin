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
using System.IO;
using System.Text;
using NUnit.Framework;
using XenCenterLib;
using XenCenterLib.Archive;

namespace XenAdminTests.ArchiveTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class ArchiveIteratorTests
    {
        #region ArchiveIterator Fake 
        private class ArchiveIteratorFake : ArchiveIterator
        {
            private Stream extractedFile;
            private bool disposed;

            public int NumberOfCallsLeftReturn { private get; set; }

            public Stream ExtractedFileReturn
            {
                set 
                { 
                    extractedFile = value;
                    disposed = false;
                }
            }
            public string CurrentFileNameReturn { get; set; }
            public long CurrentFileSizeReturn { private get; set; }
            public DateTime ModTimeReturn { private get; set; }
            public bool IsDirectoryReturn { private get; set; }

            public ArchiveIteratorFake()
            {
                Reset();
            }

            public void Reset()
            {
                CurrentFileNameReturn = "TestFileName.fake";
                CurrentFileSizeReturn = 100;
                ExtractedFileReturn = new MemoryStream(Encoding.ASCII.GetBytes("This is a test"));
                IsDirectoryReturn = false;
                ModTimeReturn = new DateTime(2011, 4, 1, 11, 04, 01);
                NumberOfCallsLeftReturn = 1;
            }

            public override bool HasNext()
            {
                if (NumberOfCallsLeftReturn < 1)
                    return false;

                NumberOfCallsLeftReturn--;
                return true;
            }

            public override void ExtractCurrentFile(Stream extractedFileContents, Action cancellingDelegate)
            {
                byte[] buffer = new byte[2 * 1024 * 1024];
                int read;
                while ((read = extractedFile.Read(buffer, 0, buffer.Length)) > 0)
                {
                    extractedFileContents.Write(buffer, 0, read);
                }
                extractedFile.Position = 0;
            }

            public override string CurrentFileName()
            {
                if (string.IsNullOrEmpty(CurrentFileNameReturn))
                    return CurrentFileNameReturn;

                return CurrentFileNameReturn + NumberOfCallsLeftReturn;
            }

            public override long CurrentFileSize()
            {
                return CurrentFileSizeReturn;
            }

            public override DateTime CurrentFileModificationTime()
            {
                return ModTimeReturn;
            }

            public override bool IsDirectory()
            {
                return IsDirectoryReturn;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (!disposed && disposing)
                    extractedFile?.Dispose();
            }
        }
        #endregion

        private ArchiveIteratorFake fakeIterator;
        private string tempPath;

        [OneTimeSetUp]
        public void Setup()
        {
            fakeIterator = new ArchiveIteratorFake();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            fakeIterator.Dispose();
        }

        [SetUp]
        public void TestSetup()
        {
            tempPath = null;
            fakeIterator.Reset();
        }

        [TearDown]
        public void TestTearDown()
        {
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
        }


        [Test]
        public void TestExtractToNullDestinationPath()
        {
            Assert.Throws(typeof(ArgumentNullException), () => fakeIterator.ExtractAllContents(null));
        }

        [Test]
        public void TestExtractNullFile()
        {
            fakeIterator.CurrentFileNameReturn = null;
            Assert.Throws(typeof(NullReferenceException), () => fakeIterator.ExtractAllContents(Path.GetTempPath()));
        }

        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void TestExtractFile(bool longDirectoryPath, bool longFilePath)
        {
            tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            
            var dirCharNumber = (longDirectoryPath ? 248 : 247) - tempPath.Length - 2;
            //2 was removed for the combining slash between tempPath and dir, and the combining slash between dir and filename
            var dir = new string('A', dirCharNumber);
            var fileCharNumber = (longFilePath ? 260 : 259) - Path.Combine(tempPath, dir).Length - 2;
            //2 was removed for the combining slash between the full dir path and filename, and the NumberOfCallsLeftReturn
            var fileName = new string('B', fileCharNumber);

            const int numberOfFiles = 3;
            fakeIterator.NumberOfCallsLeftReturn = numberOfFiles;
            fakeIterator.CurrentFileNameReturn = Path.Combine(dir, fileName);
            fakeIterator.ExtractAllContents(tempPath);

            for (var i = 0; i < 3; i++)
            {
                string targetFile = Path.Combine(tempPath, fakeIterator.CurrentFileNameReturn + i);

                if (longDirectoryPath || longFilePath)
                    targetFile = StringUtility.ToLongWindowsPathUnchecked(targetFile);

                Assert.IsTrue(File.Exists(targetFile), "File should exist");
                Assert.IsNotEmpty(File.ReadAllBytes(targetFile), "File should not be empty");

                Assert.IsFalse((File.GetAttributes(targetFile) & FileAttributes.Directory) == FileAttributes.Directory,
                    "It should not have directory attributes");
            }

            //Check recursively that there are only the correct number of files
            var actualFileNumber = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories).Length;
            Assert.AreEqual(numberOfFiles, actualFileNumber, $"There should be {numberOfFiles}");

            if (longDirectoryPath || longFilePath)
                tempPath = StringUtility.ToLongWindowsPathUnchecked(tempPath);
        }


        [TestCase(true)]
        [TestCase(false)]
        public void TestExtractDirectory(bool longDirectoryPath)
        {
            tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var dirCharNumber = (longDirectoryPath ? 248 : 247) - tempPath.Length - 2;
            //2 was removed for the combining slash between tempPath and dir, and the NumberOfCallsLeftReturn
            var dir = new string('A', dirCharNumber);
            
            fakeIterator.IsDirectoryReturn = true;
            fakeIterator.CurrentFileNameReturn = dir;
            fakeIterator.ExtractAllContents(tempPath);

            string targetPath = Path.Combine(tempPath, fakeIterator.CurrentFileName());

            if (longDirectoryPath)
                targetPath = StringUtility.ToLongWindowsPathUnchecked(targetPath);

            Assert.IsFalse(File.Exists(targetPath), "Files should not exist");
            Assert.IsEmpty(Directory.GetFiles(tempPath), "Directory should not have files");

            Assert.IsTrue(Directory.Exists(targetPath), "Directory should exist");
            Assert.IsTrue((File.GetAttributes(targetPath) & FileAttributes.Directory) == FileAttributes.Directory,
                "It should have directory attributes");

            if (longDirectoryPath)
                tempPath = StringUtility.ToLongWindowsPathUnchecked(tempPath);
        }
    }
}
