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
using Ionic.Zip;

namespace XenCenterLib.Archive
{
    class DotNetZipZipWriter : ArchiveWriter
    {
        private ZipOutputStream zip = null;
        private bool disposed;

        public DotNetZipZipWriter(Stream outputStream) : this()
        {
            zip = new ZipOutputStream( outputStream ) {EnableZip64 = Zip64Option.AsNecessary};
        }

        public DotNetZipZipWriter()
        {
            disposed = false;
        }

        public override void SetBaseStream(Stream outputStream)
        {
            zip = new ZipOutputStream(outputStream);
            disposed = false;
        }

        public override void AddDirectory(string directoryName, DateTime modificationTime)
        {
            StringBuilder sb = new StringBuilder(directoryName);

            //Need to add a trailing front-slash to add a directory
            if (!directoryName.EndsWith("/"))
                sb.Append("/");

            ZipEntry entry = zip.PutNextEntry(sb.ToString());
            entry.ModifiedTime = modificationTime;
        }

        public override void Add(Stream filetoAdd, string fileName, DateTime modificationTime)
        {
            ZipEntry entry = zip.PutNextEntry(fileName);
            entry.ModifiedTime = modificationTime;

            //You have to do this because if using a memory stream the pointer will be at the end it
            //it's just been read and this will cause nothing to be written out
            filetoAdd.Position = 0;

            StreamUtilities.BufferedStreamCopy( filetoAdd, zip );
            zip.Flush();
        }

        protected override void Dispose(bool disposing)
        {

            if( !disposed )
            {
                if (disposing)
                {
                    if( zip != null )
                    {
                        zip.Flush();
                        zip.Dispose();  
                    }
                    disposed = true;
                }
            }
            base.Dispose(disposing);
        }
    }
}
