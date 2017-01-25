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

using System.IO;
using System;

namespace XenCenterLib.Compression
{
    /// <summary>
    /// A static factory to create an object that will allow the archiving of data
    /// </summary>
    public static class CompressionFactory
    {
        /// <summary>
        /// Type of compressed stream
        /// </summary>
        public enum Type
        {
            Gz,
            Bz2
        }

        /// <summary>
        /// Instantiate a class that can decompress a data stream type
        /// </summary>
        /// <param name="compressionType">Type of compressed stream to read</param>
        /// <param name="compressedDataSource">The contents of compressed data</param>
        /// <exception cref="NotSupportedException">If there is not a compressor for a specified archive type</exception>
        /// <returns>CompressionStream to allow an read as a stream</returns>
        public static CompressionStream Reader(Type compressionType, Stream compressedDataSource)
        {
            if (compressionType == Type.Gz)
                return new DotNetZipGZipInputStream(compressedDataSource);

            if (compressionType == Type.Bz2)
                return new DotNetZipBZip2InputStream(compressedDataSource);

            throw new NotSupportedException(String.Format("Type: {0} is not supported by CompressionStream Reader", compressionType));
        }

        /// <summary>
        /// Instantiate a class that can compress a data stream type
        /// </summary>
        /// <param name="compressionType">Type of compressed stream to write</param>
        /// <param name="compressedDataTarget">The place where the compressed data will be put</param>
        /// <exception cref="NotSupportedException"> if there is not a compressor for a specified archive type</exception>
        /// <returns>CompressionStream to allow an write as a stream</returns>
        public static CompressionStream Writer(Type compressionType, Stream compressedDataTarget)
        {
            if (compressionType == Type.Gz)
                return new DotNetZipGZipOutputStream(compressedDataTarget);

            if (compressionType == Type.Bz2)
                return new DotNetZipBZip2OutputStream(compressedDataTarget);

            throw new NotSupportedException(String.Format("Type: {0} is not supported by CompressionStream Writer", compressionType));
        }
    }
}
