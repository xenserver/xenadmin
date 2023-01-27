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
using System.IO;
using NUnit.Framework;
using XenCenterLib.Compression;

namespace XenAdminTests.CompressionTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class CompressionFactoryTests
    {
        [TestCase(CompressionFactory.Type.Gz, ExpectedResult = typeof(GZipOutputStream))]
        [Test]
        public Type TestWriterGeneration(int archiveType)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var providedStream = CompressionFactory.Writer((CompressionFactory.Type)archiveType, ms))
                    return providedStream.GetType();
            }
        }

        [TestCase(CompressionFactory.Type.Gz, ExpectedResult = typeof(GZipInputStream))]
        [Test]
        public Type TestReaderGenerationWithFile(int archiveType)
        {
            string target = TestUtils.GetTestResource("emptyfile.gz");

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(target)))
            {
                using (var providedStream = CompressionFactory.Reader((CompressionFactory.Type)archiveType, ms))
                    return providedStream.GetType();
            }
        }
    }
}
