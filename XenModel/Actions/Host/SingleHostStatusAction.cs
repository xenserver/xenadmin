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
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class SingleHostStatusAction :  AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly HostWithStatus host;
        private readonly string filepath;
        private readonly string timestring;
        private readonly string[] entries;

        public SingleHostStatusAction(HostWithStatus h, List<string> e, string path, string time)
            : base(h.Host.Connection, string.Format(Messages.ACTION_SYSTEM_STATUS_COMPILING, Helpers.GetName(h.Host)), null, true)
        {
            host = h;
            entries = e.ToArray();
            filepath = path;
            timestring = time;
        }

        protected override void Run()
        {
            host.Status = HostStatus.compiling;

            string hostname = Helpers.GetName(host.Host);
            hostname = ZipStatusReportAction.SanitizeTarPathMember(hostname);
            // Workaround for excessively long filenames: trim the hostname we use
            if (hostname.Length > 20)
            {
                hostname = hostname.Truncate(20);
            }
            string filename = string.Format("{1}\\{2}-bugtool-{0}.tar", hostname, filepath, timestring);

            string entries_string = String.Join(",", entries);

            log.DebugFormat("Getting system status for {0} on {1}", entries_string, hostname);

            try
            {
                host.Status = HostStatus.compiling;
                if (Session == null)
                {
                    throw new Exception(Messages.CONNECTION_IO_EXCEPTION);
                }

                HTTPHelper.Get(this, false, dataRxDelegate, filename, host.Host.address,
                    (HTTP_actions.get_ssss)HTTP_actions.get_system_status,
                    Session.uuid, entries_string, "tar");

                log.DebugFormat("Getting system status from {0} successful", hostname);

                host.Status = HostStatus.succeeded;
                base.PercentComplete = 100;
            }
            catch (CancelledException ce)
            {
                log.Info("Getting system status cancelled");

                Description = Messages.ACTION_SYSTEM_STATUS_CANCELLED;
                host.Status = HostStatus.failed;
                host.error = ce;

                throw;
            }
            catch (Exception e)
            {
                log.Warn(string.Format("Getting system status from {0} failed", hostname), e);

                host.Status = HostStatus.failed;
                host.error = e;

                Description = 
                    Win32.GetHResult(e) == Win32.ERROR_DISK_FULL ? 
                    Messages.ACTION_SYSTEM_STATUS_DISK_FULL : 
                    Messages.ACTION_SYSTEM_STATUS_FAILED;
            }
        }

        public override int PercentComplete
        {
            // We don't want the PercentComplete to be set by HttpGet
            set
            {
            }
        }

        private void dataRxDelegate(long rxd)
        {
            host.Status = HostStatus.downloading;
            host.DataTransferred = rxd;

            if (Cancelling)
                throw new CancelledException();

            if (rxd < host.Size)
                base.PercentComplete = 10 + (int)((rxd * 80L) / host.Size);
            else 
                base.PercentComplete = 90;
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling;
        }
    }
}
