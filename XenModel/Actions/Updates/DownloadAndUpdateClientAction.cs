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
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using XenCenterLib;

namespace XenAdmin.Actions.Updates
{
    public class DownloadAndUpdateClientAction : DownloadFileAction, IByteProgressAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _checksum;
        private FileStream _msiStream;

        public DownloadAndUpdateClientAction(string updateName, Uri uri, string outputFileName, string checksum)
            : base(updateName,
                  uri, 
                  outputFileName, 
                  string.Format(Messages.DOWNLOAD_CLIENT_INSTALLER_ACTION_TITLE, updateName), 
                  string.Format(Messages.DOWNLOAD_CLIENT_INSTALLER_ACTION_DESCRIPTION, updateName), 
                  true)
        {
            _checksum = checksum;
        }        

        protected override void Run()
        {
            if (!CanDownloadFile)
                return;

            log.InfoFormat("Downloading '{0}' installer (from '{1}') to '{2}'", FileName, Address, OutputPathAndFileName);
            Description = string.Format(Messages.DOWNLOAD_CLIENT_INSTALLER_ACTION_DESCRIPTION, FileName);
            LogDescriptionChanges = false;
            DownloadFile();
            LogDescriptionChanges = true;

            if (IsCompleted || Cancelled)
                return;

            if (Cancelling)
                throw new CancelledException();

            if (!File.Exists(OutputPathAndFileName))
                throw new Exception(Messages.DOWNLOAD_CLIENT_INSTALLER_MSI_NOT_FOUND);

            ValidateMsi();

            Description = Messages.COMPLETED;
        }

        

        private void ValidateMsi()
        {
            Description = Messages.UPDATE_CLIENT_VALIDATING_INSTALLER;

            _msiStream = new FileStream(OutputPathAndFileName, FileMode.Open, FileAccess.Read);

            var calculatedChecksum = string.Empty; 

            var hash = StreamUtilities.ComputeHash(_msiStream, out _);
            if (hash != null)
                calculatedChecksum = string.Join(string.Empty, hash.Select(b => $"{b:x2}"));

            // Check if calculatedChecksum matches what is in chcupdates.xml
            if (!_checksum.Equals(calculatedChecksum, StringComparison.InvariantCultureIgnoreCase))
                throw new Exception(Messages.UPDATE_CLIENT_INVALID_CHECKSUM );

            bool valid;
            try
            {
                // Check digital signature of .msi
                using (var basicSigner = X509Certificate.CreateFromSignedFile(OutputPathAndFileName))
                {
                    using (var cert = new X509Certificate2(basicSigner))
                        valid = cert.Verify();
                }
            }
            catch (Exception e)
            {
                throw new Exception(Messages.UPDATE_CLIENT_FAILED_CERTIFICATE_CHECK, e);
            }

            if (!valid)
                throw new Exception(Messages.UPDATE_CLIENT_INVALID_DIGITAL_CERTIFICATE);
        }        
    }
}

