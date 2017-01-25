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

namespace XenAdmin.Wizards.ExportWizard.ApplianceChecks
{
    public abstract class ApplianceCheck
    {
        public enum FileExtension
        {
            ovaovf,
            xva
        }

        public bool IsValid { get; protected set; }
        public abstract string ErrorReason { get; }
        public abstract void Validate();
    }

    public class ApplianceExistsCheck : ApplianceCheck
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string DestinationDirectory { get; set; }
        private string FileName { get; set; }
        private FileExtension Extension { get; set; }

        public ApplianceExistsCheck(string destinationDirectory, string fileName, FileExtension extension)
        {
            DestinationDirectory = destinationDirectory;
            FileName = fileName;
            Extension = extension;
            IsValid = false;
        }

        protected virtual bool DirectoryExists(string directory)
        {
            return Directory.Exists(directory);
        }

        protected virtual bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public override string ErrorReason
        {
            get { return IsValid ? string.Empty : Messages.EXPORT_APPLIANCE_PAGE_ERROR_APP_EXISTS; }
        }

        public override void Validate()
        {
            switch (Extension)
            {
                case(FileExtension.ovaovf):
                    {
                        VerifyOvaOvf();
                        break;
                    }
                case (FileExtension.xva):
                    {
                        VerifyXva();
                        break;
                    }
                default:
                    {
                        log.Debug("Cannot determine extension type to validate, using OVA/OVF validation rules");
                        VerifyOvaOvf();
                        break;
                    }
            }

        }

        private void VerifyXva()
        {
            string filePath = Path.Combine(DestinationDirectory, String.Format("{0}.xva", FileName));
            IsValid = !FileExists(filePath);
        }

        private void VerifyOvaOvf()
        {
            string applFolder = Path.Combine(DestinationDirectory, FileName);
            string ovfFilename = String.Format("{0}.ovf", FileName);
            string ovaFilename = String.Format("{0}.ova", FileName);
            string ovfFilepath = Path.Combine(applFolder, ovfFilename);
            string ovaFilepath = Path.Combine(applFolder, ovaFilename);

            if (DirectoryExists(applFolder) && FileExists(ovfFilepath) || FileExists(ovaFilepath))
            {
                IsValid = false;
                return;
            }

            IsValid = true;
        }
    }
}
