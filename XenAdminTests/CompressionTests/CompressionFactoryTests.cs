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
using NUnit.Framework;
using XenCenterLib.Compression;

namespace XenAdminTests.CompressionTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class CompressionFactoryTests
    {
        [TestCase(CompressionFactory.Type.Gz, ExpectedResult = typeof(DotNetZipGZipOutputStream))]
        [TestCase(CompressionFactory.Type.Bz2, ExpectedResult = typeof(DotNetZipBZip2OutputStream))]
        [Test]
        public Type TestWriterGeneration(int archiveType)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var providedStream = CompressionFactory.Writer((CompressionFactory.Type)archiveType, ms))
                    return providedStream.GetType();
            }
        }

        [TestCase(CompressionFactory.Type.Gz, ExpectedResult = typeof(DotNetZipGZipInputStream))]
        [TestCase(CompressionFactory.Type.Bz2, ExpectedResult = typeof(DotNetZipBZip2InputStream))]
        [Test]
        public Type TestReaderGenerationWithFile(int archiveType)
        {
            string target = TestUtils.GetTestResource("emptyfile.bz2");
            /*
             * Note: Reading a bzip2 file in as a byte[] works for gzip as well as bzip2 stream 
             * as the implementation of bzip2 must be initialised with a string containing a 
             * header, EOF etc. whereas gzip doesn't mind so the following will work despite
             * opening a gzip compression stream with a bzip2 data
             */
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(target)))
            {
                using (var providedStream = CompressionFactory.Reader((CompressionFactory.Type)archiveType, ms))
                    return providedStream.GetType();
            }
        }


        [TestCase(CompressionFactory.Type.Bz2)]
        [Description("Checks that a BZip2 stream without actual BZip2 data, header, data etc., will throw an exception")]
        [Test]
        public void TestReaderGenerationWithoutFile(int archiveType)
        {
            using (MemoryStream ms = new MemoryStream())
                Assert.Throws(typeof(IOException), () =>
                {
                    using (CompressionFactory.Reader((CompressionFactory.Type)archiveType, ms))
                    {
                    }
                });
        }
    }
}
