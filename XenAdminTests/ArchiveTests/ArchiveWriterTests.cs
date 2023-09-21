/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Linq;
using System.Text;
using NUnit.Framework;
using XenCenterLib;
using XenCenterLib.Archive;

namespace XenAdminTests.ArchiveTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class ArchiveWriterTests
    {
        private class FakeArchiveWriter : ArchiveWriter
        {
            private bool disposed;

            public List<Stream> AddedStreamData { get; private set; }

            public List<string> AddedFileNameData { get; private set; }

            public List<DateTime> AddedDates { get; private set; }

            public FakeArchiveWriter()
            {
                Reset();
            }

            public void Reset()
            {
                DisposeStreamList();
                AddedStreamData = new List<Stream>();
                AddedFileNameData = new List<string>();
                AddedDates = new List<DateTime>();
            }

            private void DisposeStreamList()
            {
                if (AddedStreamData != null)
                {
                    foreach (Stream stream in AddedStreamData)
                    {
                        if (stream != null)
                            stream.Dispose();
                    }
                }
            }

            public override void Add(Stream fileToAdd, string fileName, DateTime modificationTime, Action cancellingDelegate)
            {
                disposed = false;
                AddedStreamData.Add(fileToAdd);
                AddedFileNameData.Add(fileName);
                AddedDates.Add(modificationTime);
            }

            public override void AddDirectory(string directoryName, DateTime modificationTime)
            {
                AddedFileNameData.Add(directoryName);
                AddedDates.Add(modificationTime);
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    if (!disposed)
                    {
                        DisposeStreamList();
                    }
                    disposed = true;
                }
            }
        }

        private FakeArchiveWriter fakeWriter;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            fakeWriter = new FakeArchiveWriter();
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            fakeWriter.Dispose();
        }

        [SetUp]
        public void TestSetup()
        {
            fakeWriter.Reset();
        }

        [Test]
        public void DatelessAddCallsImplementation()
        {
            const string fileName = "test.file";
            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes("This is a test")))
            {
                fakeWriter.Add(ms, fileName, DateTime.Now, null);
                Assert.AreEqual(1, fakeWriter.AddedFileNameData.Count);
                Assert.AreEqual(1, fakeWriter.AddedStreamData.Count);
                Assert.AreEqual(1, fakeWriter.AddedDates.Count);
                Assert.AreEqual(fileName, fakeWriter.AddedFileNameData[0], "File name");
                Assert.IsTrue(fakeWriter.AddedStreamData[0].Length == 14, "Stream has data");
                Assert.That(fakeWriter.AddedDates[0], Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(5)));
            }
        }

        [Test]
        public void DatelessAddDirectoryCallsImplementation()
        {
            const string dirName = "test.file";
            const int totalAdded = 3;
            for (int i = 0; i < totalAdded; i++)
            {
                fakeWriter.AddDirectory(dirName, DateTime.Now);
            }

            Assert.AreEqual(totalAdded, fakeWriter.AddedFileNameData.Count);
            Assert.AreEqual(0, fakeWriter.AddedStreamData.Count);
            Assert.AreEqual(totalAdded, fakeWriter.AddedDates.Count);
            Assert.AreEqual(dirName, fakeWriter.AddedFileNameData[0], "File name");
            Assert.That(fakeWriter.AddedDates[0], Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public void CreateArchiveThrowsWithBadPath()
        {
            Assert.Throws(typeof(FileNotFoundException), () => fakeWriter.CreateArchive("Yellow brick road - not a path!"));
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void CreateArchiveWithLongPath(bool longDirectoryPath, bool longFilePath)
        {
            //set up the path to zip
            var zipPath = PopulateLongPathArchive(true, longDirectoryPath, longFilePath, out var addedData);

            fakeWriter.CreateArchive(zipPath);

            foreach (var datum in addedData)
                Assert.Contains(datum, fakeWriter.AddedFileNameData);

            // 2 folders and one file
            Assert.AreEqual(addedData.Count, fakeWriter.AddedFileNameData.Count);

            //clean up: we need to ensure we're deleting the folder
            if (longDirectoryPath || longFilePath)
                zipPath = StringUtility.ToLongWindowsPathUnchecked(zipPath);

            Directory.Delete(zipPath, true);
        }

        [Test]
        public void CreateArchiveWithLongPath_PathTooLong()
        {
            //! N.B.: If this test fails it might be because the project has moved to a version of .NET Core
            //! that does not require calls to `StringUtils.ToLongWindowsPath`. Please review its uses
            //! and remove it from the codebase if possible.

            // this test ensures PopulateLongPathArchive's correctness
            // since CreateArchiveWithLongPath depends on it

            Assert.DoesNotThrow(() => PopulateLongPathArchive(false, false, false, out _));
            Assert.Throws<DirectoryNotFoundException>(() => PopulateLongPathArchive(false, false, true, out _));
            Assert.Throws<PathTooLongException>(() => PopulateLongPathArchive(false, true, true, out _));
            Assert.Throws<PathTooLongException>(() => PopulateLongPathArchive(false, true, false, out _));
        }

        /// <summary>
        /// Set up method creating a directory containing 2 subdirectories one of which has a file
        /// </summary>
        /// <param name="createValidPaths">set to true to ensure folders and files are prepended with \\?\</param>
        /// <returns>the path to the folder</returns>
        private string PopulateLongPathArchive(bool createValidPaths, bool longDirectoryPath, bool longFilePath, out List<string> addedData)
        {
            var zipPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(zipPath);

            var dirCharNumber1 = (longDirectoryPath ? 248 : 247) - zipPath.Length - 2;
            //2 was removed for the combining slash between tempPath and dir, and the first character
            var relativeDirectoryPath1 = 0 + new string('A', dirCharNumber1);

            var dirCharNumber2 = (longDirectoryPath ? 248 : 247) - zipPath.Length - 3;
            //3 was removed for the combining slash between zipPath and dir, the first character,
            //and the combining slash between dir and filename
            var relativeDirectoryPath2 = 1 + new string('A', dirCharNumber2);

            var fileCharNumber = (longFilePath ? 260 : 259) - Path.Combine(zipPath, relativeDirectoryPath2).Length - 1;
            //1 was removed for the combining slash between the full dir path and filename
            var fileName = new string('B', fileCharNumber);
            var relativeFilePath = Path.Combine(relativeDirectoryPath2, fileName);

            addedData = new List<string>
            {
                relativeDirectoryPath1.Replace(@"\", "/"),
                relativeDirectoryPath2.Replace(@"\", "/"),
                relativeFilePath.Replace(@"\", "/")
            };

            var directoryPath1 = Path.Combine(zipPath, relativeDirectoryPath1);
            var directoryPath2 = Path.Combine(zipPath, relativeDirectoryPath2);
            var filePath = Path.Combine(directoryPath2, fileName);

            if (createValidPaths)
            {
                directoryPath1 = StringUtility.ToLongWindowsPathUnchecked(directoryPath1);
                directoryPath2 = StringUtility.ToLongWindowsPathUnchecked(directoryPath2);
                filePath = StringUtility.ToLongWindowsPathUnchecked(filePath);
            }

            Directory.CreateDirectory(directoryPath1);
            Directory.CreateDirectory(directoryPath2);
            File.WriteAllText(filePath, "Hello, World!");

            
            return zipPath;
        }

        [Test]
        public void CreateArchiveWorksWithValidDirectoryStructure()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);
            CreateFiles(tempPath, 2);

            for (int i = 0; i < 4; i++)
            {
                string subfolder = Path.Combine(tempPath, Path.GetRandomFileName());
                Directory.CreateDirectory(subfolder);
                CreateFiles(subfolder, i);
            }

            fakeWriter.CreateArchive(tempPath);

            Assert.AreEqual(12, fakeWriter.AddedDates.Count);
            Assert.AreEqual(12, fakeWriter.AddedFileNameData.Count);
            Assert.AreEqual(8, fakeWriter.AddedStreamData.Count);

            foreach (DateTime date in fakeWriter.AddedDates)
                Assert.That(date, Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(5)));

            foreach (string name in fakeWriter.AddedFileNameData)
                Assert.AreEqual(-1, name.IndexOfAny(@":\".ToArray()), "Unwanted chars found in path");

            Directory.Delete(tempPath, true);
        }

        private void CreateFiles(string tempPath, int numberOfFiles)
        {
            for (int i = 0; i < numberOfFiles; i++)
            {
                using (FileStream fs = File.OpenWrite(Path.Combine(tempPath, Path.GetRandomFileName())))
                {
                    fs.Write(Encoding.ASCII.GetBytes("This is a test"), 0, 14);
                    fs.Flush();
                }
            }
        }
    }
}
