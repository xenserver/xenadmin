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
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using XenAdmin.Alerts;

namespace CFUValidator.Validators
{
    class PatchURLValidator : Validator
    {
        private readonly List<XenServerPatchAlert> alerts;

        public PatchURLValidator(List<XenServerPatchAlert> alerts)
        {
            this.alerts = alerts;
        }

        protected override string Header => "Checking the patch URLs return a suitable http response...";

        protected override string Footer => "Patch URL check completed.";

        protected override string SummaryTitle => "Required patch URL checks:";

        protected override void ValidateCore(Action<string> statusReporter)
        {
            ConsoleSpinner spinner = new ConsoleSpinner();

            using (var workerThread = new BackgroundWorker())
            {
                workerThread.DoWork += CheckAllPatchURLs;
                workerThread.RunWorkerAsync();

                while (workerThread.IsBusy)
                    spinner.Turn();
            }
        }

        private void CheckAllPatchURLs(object sender, DoWorkEventArgs e)
        {
            foreach (XenServerPatchAlert alert in alerts)
            {
                if (String.IsNullOrEmpty(alert.Patch.PatchUrl))
                {
                    Errors.Add($"Patch '{alert.Patch.Name}' URL is missing");
                    continue;
                }

                HttpWebResponse response = null;
                try
                {
                    WebRequest request = WebRequest.Create(alert.Patch.PatchUrl);
                    request.Method = "HEAD";
                    response = request.GetResponse() as HttpWebResponse;
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                        Errors.Add($"Patch '{alert.Patch.Name}' URL '{alert.Patch.PatchUrl}' is invalid");
                }
                catch (WebException ex)
                {
                    Errors.Add($"Patch '{alert.Patch.Name}' URL '{alert.Patch.PatchUrl}' failed: {ex.Message}");
                }
                finally
                {
                    response?.Close();
                }
            }
        }
    }
}
