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
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using XenAdmin.Alerts;

namespace CFUValidator.Validators
{
    class PatchURLValidator : AlertFeatureValidator
    {
        private readonly BackgroundWorker workerThread;
        public PatchURLValidator(List<XenServerPatchAlert> alerts) : base(alerts)
        {
            workerThread = new BackgroundWorker();
            workerThread.DoWork += CheckAllPatchURLs;
            workerThread.RunWorkerCompleted += RunWorkerCompletedMethod;
        }

        private bool isComplete;

        public override void Validate()
        {
            isComplete = false;
            ConsoleSpinner spinner = new ConsoleSpinner();
            workerThread.RunWorkerAsync();
            
            while(!isComplete)
                spinner.Turn();

        }

        private void RunWorkerCompletedMethod(object sender, RunWorkerCompletedEventArgs e)
        {
            isComplete = true;
        }

        private void CheckAllPatchURLs(object sender, DoWorkEventArgs e)
        {
            foreach (XenServerPatchAlert alert in alerts)
            {
                if(String.IsNullOrEmpty(alert.Patch.PatchUrl))
                {
                    Results.Add(String.Format("Patch '{0}' URL is missing", alert.Patch.Name));
                    continue;
                }

                HttpWebResponse response = null;
                try
                {
                    WebRequest request = WebRequest.Create(alert.Patch.PatchUrl);
                    request.Method = "HEAD";
                    response = (HttpWebResponse)request.GetResponse();
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                        Results.Add(String.Format("Patch '{0}' URL '{1}' is invalid", alert.Patch.Name, alert.Patch.PatchUrl)); 
                }
                catch(WebException ex)
                {
                    Results.Add(String.Format("Patch '{0}' URL '{1}' failed: {2}", alert.Patch.Name, alert.Patch.PatchUrl, ex.Message)); 
                }
                finally
                {
                    if(response != null)
                        response.Close();
                }
            }
        }

        public override string Description
        {
            get { return "Checking the patch URLs return a suitable http response"; }
        }
    }
}
