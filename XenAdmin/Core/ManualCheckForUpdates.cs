/* Copyright (c) Citrix Systems Inc. 
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
using System.Linq;
using System.Text;
using XenAdmin.Actions;
using XenAdmin.Alerts;

namespace XenAdmin.Core
{
    class ManualCheckForUpdates
    {
        private static List<XenCenterVersion> XenCenterVersions = new List<XenCenterVersion>();
        private static List<XenServerVersion> XenServerVersions = new List<XenServerVersion>();
        private static List<XenServerPatch> XenServerPatches = new List<XenServerPatch>();

        public event EventHandler<CheckForUpdatesCompletedEventArgs> CheckForUpdatesCompleted;

        public ManualCheckForUpdates()
        {
        }

        public void RunCheck()
        {
            Updates.CheckForUpdates(actionCompleted);
        }

        public List<Alert> UpdateAlerts
        {
            get
            {
                List<Alert> updateAlerts = new List<Alert>();
                XenCenterUpdateAlert xenCenterAlert = Updates.NewXenCenterVersionAlert(XenCenterVersions, false);
                if (xenCenterAlert != null)
                {
                    updateAlerts.Add(xenCenterAlert);
                }

                XenServerUpdateAlert xenServerUpdateAlert = Updates.NewServerVersionAlert(XenServerVersions, false);
                if (xenServerUpdateAlert != null)
                {
                    updateAlerts.Add(xenServerUpdateAlert);
                }

                List<XenServerPatchAlert> xenServerPatchAlerts = Updates.NewServerPatchesAlerts(XenServerVersions,
                                                                                                XenServerPatches,
                                                                                                false);

                if (xenServerPatchAlerts != null)
                {
                    foreach (var xenServerPatchAlert in xenServerPatchAlerts)
                    {
                        updateAlerts.Add(xenServerPatchAlert);
                    }
                }

                return updateAlerts;
            }
        }

        private void actionCompleted(object sender, EventArgs e)
        {
            Program.AssertOffEventThread();
            DownloadUpdatesXmlAction action = sender as DownloadUpdatesXmlAction;
            
            bool succeeded = false;
            string errorMessage = string.Empty;
            
            if (action != null)
            {
                succeeded = action.Succeeded;
                if (succeeded)
                {
                    XenCenterVersions = action.XenCenterVersions;
                    XenServerVersions = action.XenServerVersions;
                    XenServerPatches = action.XenServerPatches;
                }
                else
                {
                    XenCenterVersions.Clear();
                    XenServerVersions.Clear();
                    XenServerPatches.Clear();

                    if (action.Exception != null)
                    {
                        if (action.Exception is System.Net.Sockets.SocketException)
                        {
                            errorMessage = Messages.AVAILABLE_UPDATES_NETWORK_ERROR;
                        }
                        else
                        {
                            // Clean up and remove excess newlines, carriage returns, trailing nonsense
                            string errorText = action.Exception.Message.Trim();
                            errorText = System.Text.RegularExpressions.Regex.Replace(errorText, @"\r\n+", "");
                            errorMessage = string.Format(Messages.AVAILABLE_UPDATES_ERROR, errorText);
                        }
                    }

                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        errorMessage = Messages.AVAILABLE_UPDATES_INTERNAL_ERROR;
                    }
                }
            }

            if (CheckForUpdatesCompleted != null)
            {
                CheckForUpdatesCompleted(this, new CheckForUpdatesCompletedEventArgs(succeeded, errorMessage));
            }
        }
    }

    public class CheckForUpdatesCompletedEventArgs : EventArgs
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }

        public CheckForUpdatesCompletedEventArgs(bool succeeded, string errorMessage)
        {
            Succeeded = succeeded;
            ErrorMessage = errorMessage;
        }
    }
}
