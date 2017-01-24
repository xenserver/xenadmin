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

namespace XenCenterLib
{
    public class StreamUtilities
    {
        /// <summary>
        /// Perform a copy of the contents of one stream class to another in a buffered fashion
        /// 
        /// Buffer size is a hard-coded 2Mb
        /// </summary>
        /// <param name="inputData">Source data</param>
        /// <param name="outputData">Target stream</param>
        public static void BufferedStreamCopy(Stream inputData, Stream outputData)
        {
            if( inputData == null)
                throw new ArgumentNullException("inputData", "BufferedStreamCopy argument cannot be null");

            if (outputData == null)
                throw new ArgumentNullException("outputData", "BufferedStreamCopy argument cannot be null");

            const long bufferSize = 2*1024*1024;

            byte[] buffer = new byte[bufferSize];
            int n;
            while ((n = inputData.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputData.Write(buffer, 0, n);
            }

            outputData.Flush();
        }
    }
}
