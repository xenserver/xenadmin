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

namespace XenCenterLib.Compression
{
    /// <summary>
    /// Abstract base class for the compression stream class
    /// </summary>
    public abstract class CompressionStream : Stream
    {
        private Stream storedStream = null;
        protected Stream zipStream 
        { 
            set 
            { 
                disposed = false;
                storedStream = value;
            }

            private get { return storedStream; }
        }

        public virtual void SetBaseStream(Stream baseStream)
        {
            throw new NotImplementedException();
        }

        private bool disposed = true;

        protected CompressionStream()
        {
            zipStream = null;
            disposed = true;
        }

        /// <summary>
        /// Write *to* this stream *from* the source stream in a buffered manner analogous to Write()
        /// </summary>
        /// <param name="sourceStream">Stream get data from</param>
        public void BufferedWrite(Stream sourceStream)
        {
            StreamUtilities.BufferedStreamCopy(sourceStream, this);
        }

        /// <summary>
        /// Read *from* this stream and write to the targetStream in a buffered manner as per the Read()
        /// </summary>
        /// <param name="targetStream">Stream to put data into</param>
        public void BufferedRead(Stream targetStream)
        {
            StreamUtilities.BufferedStreamCopy(this, targetStream);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return zipStream.Read(buffer, offset, count);
        }

        public override long Position
        {
            get { return zipStream.Position; }
            set { zipStream.Position = value; }
        }

        protected override void Dispose(bool disposing)
        {
            if( disposing )
            {
                if (!disposed)
                {
                    if (zipStream != null)
                    {
                        zipStream.Dispose();
                        zipStream = null;
                    }
                    disposed = true;
                }  
            }
            base.Dispose(disposing);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            zipStream.Write(buffer, offset, count);
        }

        public override bool CanRead
        {
            get { return zipStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return zipStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return zipStream.CanWrite; }
        }

        public override void Flush()
        {
            zipStream.Flush();
        }

        public override long Length
        {
            get { return zipStream.Length; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return zipStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            zipStream.SetLength(value);
        }

    }  
}
