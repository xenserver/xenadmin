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
using System.Linq;
using System.Collections.Generic;
using System.IO;
using XenAdmin.Actions;
using XenAdmin.Alerts;
using XenAdmin.Core;

namespace CFUValidator.Validators
{
    class ZipContentsValidator : Validator
    {
        private readonly List<XenServerPatchAlert> alerts;

        public ZipContentsValidator(List<XenServerPatchAlert> alerts)
        {
            this.alerts = alerts;
        }

        protected override string Header => "Downloading and checking the contents of the zip files in the patch...";

        protected override string Footer => "Download and content check of patch zip files completed.";

        protected override string SummaryTitle => "Required patch zip content checks:";

        protected override void ValidateCore(Action<string> statusReporter)
        {
            foreach (XenServerPatchAlert alert in alerts.OrderBy(a => a.Patch.Name))
            {
                DownloadPatchFile(alert, statusReporter);
            }
        }

        private void DownloadPatchFile(XenServerPatchAlert patch, Action<string> statusReporter)
        {
            if (string.IsNullOrEmpty(patch.Patch.PatchUrl))
            {
                Errors.Add("Patch contained no URL: " + patch.Patch.Name);
                return;
            }

            string tempFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var action = new DownloadAndUnzipXenServerPatchAction(patch.Patch.Name, new Uri(patch.Patch.PatchUrl),
                tempFileName, false, BrandManager.ExtensionUpdate, "iso");

            try
            {
                statusReporter("Download and unzip patch " + patch.Patch.Name);

                ConsoleSpinner spinner = new ConsoleSpinner();
                action.RunAsync();
                while (!action.IsCompleted)
                {
                    spinner.Turn(action.PercentComplete);
                }

                if (!action.Succeeded)
                    Errors.Add("Patch download and unzip unsuccessful: " + action.Exception.Message);
            }
            catch (Exception ex)
            {
                Errors.Add("Patch download error: " + ex.Message);
            }
        }
    }
}
