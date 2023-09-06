﻿/* Copyright (c) Cloud Software Group, Inc. 
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
                AssertCurrentDateIsPlausible(fakeWriter.AddedDates[0]);
            }
        }

        private void AssertCurrentDateIsPlausible(DateTime currentDate)
        {
            //If this is failing check that the number of seconds is enough
            const double seconds = 5.0;
            DateTime maxDate = DateTime.Now.AddSeconds(seconds);
            DateTime minDate = DateTime.Now.AddSeconds(-1.0 * seconds);
            Assert.IsTrue(currentDate > minDate, "Date is > minimum");
            Assert.IsTrue(currentDate < maxDate, "Date is < maximum");
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
            AssertCurrentDateIsPlausible(fakeWriter.AddedDates[0]);
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
            var relativeFilePath = PopulateLongPathArchive(true, longDirectoryPath, longFilePath);
            Assert.Contains(relativeFilePath, fakeWriter.AddedFileNameData);
            // 1 folder and one file
            Assert.AreEqual(2, fakeWriter.AddedFileNameData.Count);
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void CreateArchiveWithLongPath_PathTooLong(bool longDirectoryPath, bool longFilePath)
        {
            //! N.B.: If this test fails it might be because the project has moved to a version of .NET Core
            //! that does not require calls to `StringUtils.ToLongWindowsPath`. Please review its uses
            //! and remove it from the codebase if possible.

            if (!longDirectoryPath)
            {
                if(!longFilePath)
                    Assert.DoesNotThrow(() => PopulateLongPathArchive(false, longDirectoryPath, longFilePath));
                else
                    Assert.Throws<DirectoryNotFoundException>(() => PopulateLongPathArchive(false, longDirectoryPath, longFilePath));
            }
            else
            {
                Assert.Throws<PathTooLongException>(() => PopulateLongPathArchive(false, longDirectoryPath, longFilePath));
            }
        }

        [TestCase(true, true, ExpectedResult = true)]
        [TestCase(true, true, ExpectedResult = true)]
        [TestCase(false, true, ExpectedResult = true)]
        [TestCase(false, false, ExpectedResult = false)]
        public bool PopulateLongPathArchive_ThrowsErrorWhenNecessary(bool longDirectoryPath, bool longFilePath)
        {
            // this test ensures PopulateLongPathArchive's correctness
            // since CreateArchiveWithLongPath depends on it

            var exceptionThrown = false;
            try
            {
                PopulateLongPathArchive(false, longDirectoryPath, longFilePath);
            }
            catch (PathTooLongException)
            {
                exceptionThrown = true;
            }
            catch (DirectoryNotFoundException)
            {
                if (!longDirectoryPath && longFilePath)
                {
                    exceptionThrown = true;
                }
                else
                {
                    throw;
                }
            }
            
            return exceptionThrown;
        }

        /// <summary>
        /// Create an archive with 50 nested folders and one file.
        /// </summary>
        /// <param name="createValidPaths">set to true to ensure folders and files are prepended with \\?\</param>
        /// <returns>the path of the one file that has been added</returns>
        private string PopulateLongPathArchive(bool createValidPaths, bool longDirectoryPath = true, bool longFilePath = true)
        {
            var zipPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(zipPath);

            var directoryPathLength = 248 - zipPath.Length - 1;
            var relativeDirectoryPath = new string('A', directoryPathLength - (longDirectoryPath ? 0 : 1));
            var directoryPath = Path.Combine(zipPath, relativeDirectoryPath);

            var fileName = new string('B', longFilePath ? 13 : 11);

            var filePath = Path.Combine(directoryPath, fileName);
            var relativeFilePath = Path.Combine(relativeDirectoryPath, fileName);

            if (createValidPaths)
            {
                directoryPath = StringUtility.ToLongWindowsPath(directoryPath);
                filePath = StringUtility.ToLongWindowsPath(filePath);
            }

            Directory.CreateDirectory(directoryPath);
            File.WriteAllText(filePath, "Hello, World!");

            fakeWriter.CreateArchive(zipPath);

            // we need to ensure we're deleting the folder
            if (longDirectoryPath || longFilePath)
            {
                directoryPath = StringUtility.ToLongWindowsPath(directoryPath, true);
            }
            Directory.Delete(directoryPath, true);

            // fakeWriter paths have been "cleaned" using ArchiveWriter.CleanRelativePathName
            return relativeFilePath.Replace(@"\\?\", string.Empty).Replace(@"\", "/");
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
                AssertCurrentDateIsPlausible(date);

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
