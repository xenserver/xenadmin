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
using System.Net;
using System.ComponentModel;
using System.Threading;
using System.IO;
using XenCenterLib.Archive;
using XenAPI;

namespace XenAdmin.Actions
{
    public class UnzipXenServerPatchAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string zippedUpdateFilePath;
        private readonly string updateFileExtension;

        public UnzipXenServerPatchAction(string zippedUpdate, string fileExtension)
            : base(null, string.Format(Messages.UPDATES_WIZARD_EXTRACT_ACTION_TITLE, Path.GetFileName(zippedUpdate)), string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_EXTRACTING_DESC, Path.GetFileName(zippedUpdate)))
        {
            zippedUpdateFilePath = zippedUpdate;
            updateFileExtension = fileExtension;
        }

        public string UnzippedUpdatePatchPath { get; private set; }

        private void ExtractFile()
        {
            ArchiveIterator iterator = null;
            try
            {
                using (Stream stream = new FileStream(zippedUpdateFilePath, FileMode.Open, FileAccess.Read))
                {
                    iterator = ArchiveFactory.Reader(ArchiveFactory.Type.Zip, stream);
                    DotNetZipZipIterator zipIterator = iterator as DotNetZipZipIterator;
                    if (zipIterator != null)
                    {
                        zipIterator.CurrentFileExtractProgressChanged +=
                            archiveIterator_CurrentFileExtractProgressChanged;
                    }
                    
                    while (iterator.HasNext())
                    {
                        string currentExtension = Path.GetExtension(iterator.CurrentFileName());
                        if (currentExtension == updateFileExtension.ToLowerInvariant() || currentExtension == ".iso")
                        {
                            string path = Path.Combine(Path.GetTempPath(), iterator.CurrentFileName());
                            log.DebugFormat(
                                "Found '{0}' in the downloaded archive when looking for a '{1}' file. Extracting...",
                                iterator.CurrentFileName(), currentExtension);
                            using (Stream outputStream = new FileStream(path, FileMode.Create))
                            {
                                iterator.ExtractCurrentFile(outputStream);
                                UnzippedUpdatePatchPath = path;

                                log.DebugFormat("Update file extracted to '{0}'", path);
                            }
                        }
                        break;
                    }
                    if (zipIterator != null)
                    {
                        zipIterator.CurrentFileExtractProgressChanged -=
                            archiveIterator_CurrentFileExtractProgressChanged;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Exception occurred when extracting archive: {0}", ex.Message);
            }
            finally
            {
                if (iterator != null)
                    iterator.Dispose();
            }
        }

        void archiveIterator_CurrentFileExtractProgressChanged(object sender, ExtractProgressChangedEventArgs e)
        {
            int pc = (int)(100.0 * e.BytesTransferred / e.TotalBytesToTransfer);
            if (pc != PercentComplete)
                PercentComplete = pc;
        }

        protected override void Run()
        {
            ExtractFile();
        }
    }
}
