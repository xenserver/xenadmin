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

using Ionic.Zlib;
using System.IO;

namespace XenCenterLib.Compression
{
    /// <summary>
    /// A class that can compress a bzip2 data stream type
    /// </summary>
    class DotNetZipGZipOutputStream : CompressionStream
    {
        public DotNetZipGZipOutputStream()
        {
            
        }

        public DotNetZipGZipOutputStream(Stream outputStream)
        {
            zipStream = new GZipStream(outputStream, CompressionMode.Compress);
        }

        public override void SetBaseStream(Stream outputStream)
        {
            zipStream = new GZipStream(outputStream, CompressionMode.Compress);
        }
    }

    /// <summary>
    /// A class that can decompress a bzip2 data stream type
    /// </summary>
    class DotNetZipGZipInputStream : CompressionStream
    {
        public DotNetZipGZipInputStream()
        {
            
        }

        public DotNetZipGZipInputStream(Stream inputStream)
        {
            zipStream = new GZipStream(inputStream, CompressionMode.Decompress);
        }

        public override void SetBaseStream(Stream inputStream)
        {
            zipStream = new GZipStream(inputStream, CompressionMode.Decompress);
        }

    }
}
