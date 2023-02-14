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
using System.IO;
using System.Text.RegularExpressions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Actions
{
    public enum ReportStatus { queued, inProgress, succeeded, failed, cancelled }

    public abstract class StatusReportAction : AsyncAction
    {
        protected readonly string filePath;
        protected readonly string timeString;

        public ReportStatus Status { get; protected set; }
        public Exception Error { get; protected set; }

        protected StatusReportAction(IXenConnection connection, string title, string filePath, string timeString, bool suppressHistory = true)
            : base(connection, title, suppressHistory)
        {
            this.filePath = filePath;
            this.timeString = timeString;
            Status = ReportStatus.queued;
        }

        public new void Cancel()
        {
            base.Cancel();
            Status = ReportStatus.cancelled;
        }
    }

    public class ClientSideStatusReportAction : StatusReportAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<Host> hosts;
        private readonly bool includeClientLogs;

        public ClientSideStatusReportAction(List<Host> hosts, bool includeClientLogs, string filePath, string timeString)
            : base(null,
                includeClientLogs
                    ? string.Format(Messages.BUGTOOL_CLIENT_ACTION_LOGS_META, BrandManager.BrandConsole)
                    : string.Format(Messages.BUGTOOL_CLIENT_ACTION_META, BrandManager.BrandConsole),
                filePath, timeString)
        {
            this.hosts = hosts;
            this.includeClientLogs = includeClientLogs;
        }

        protected override void Run()
        {
            try
            {
                Status = ReportStatus.inProgress;
                CopyClientLogs();
                CompileCoordinatorSupporterInfo();
                CompileClientMetadata();
                Tick(100, Messages.COMPLETED);
                Status = ReportStatus.succeeded;
            }
            catch (CancelledException)
            {
                log.Info("Getting system status cancelled");
                Tick(100, Messages.ACTION_SYSTEM_STATUS_CANCELLED);
                Status = ReportStatus.cancelled;
            }
            catch (Exception e)
            {
                log.Error("Client side data compilation failed", e);
                Tick(100, Messages.BUGTOOL_REPORTSTATUS_FAILED);
                Status = ReportStatus.failed;
            }
        }

        private void CopyClientLogs()
        {
            string logDestination = string.Format("{0}\\{1}-{2}.log", filePath, timeString, BrandManager.BrandConsoleNoSpace);
            if (includeClientLogs)
            {
                string logPath = XenAdminConfigManager.Provider.GetLogFile();
                File.Copy(logPath, logDestination, true);//overwrite if existing

                // Copy old XenCenter.log.* files too
                DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(logPath));
                Regex regex = new Regex(Regex.Escape(Path.GetFileName(logPath)) + @"\.\d+");
                foreach (FileInfo file in di.GetFiles())
                {
                    if (regex.IsMatch(file.Name))
                        File.Copy(file.FullName, Path.Combine(filePath, timeString + "-" + file.Name));
                }
            }
        }

        private void CompileCoordinatorSupporterInfo()
        {
            var coordinatorsInfo = new List<string>();
            foreach (var host in hosts)
            {
                var pool = Helpers.GetPool(host.Connection);
                string info;

                if (pool == null)
                    info = string.Format("Server '{0}' is a stand alone server", host.Name());
                else if (host.IsCoordinator())
                    info = string.Format("Server '{0}' is a coordinator of pool '{1}'", host.Name(), pool.Name());
                else
                    info = string.Format("Server '{0}' is a supporter of pool '{1}'", host.Name(), pool.Name());

                coordinatorsInfo.Add(info);
            }

            string coordinatorsDestination = string.Format("{0}\\{1}-Coordinators.txt", filePath, timeString);
            WriteExtraInfoToFile(coordinatorsInfo, coordinatorsDestination);
        }

        private void CompileClientMetadata()
        {
            var metadata = XenAdminConfigManager.Provider.GetXenCenterMetadata();
            string metadataDestination = string.Format("{0}\\{1}-Metadata.json", filePath, timeString);
            WriteExtraInfoToFile(new List<string> { metadata }, metadataDestination);
        }

        private void WriteExtraInfoToFile(List<string> info, string fileName)
        {
            try
            {
                using (var sw = new StreamWriter(fileName, false)) //overwrite if existing
                {
                    foreach (string s in info)
                        sw.WriteLine(s);

                    sw.Flush();
                }
            }
            catch (Exception e)
            {
                log.Error($"Exception while writing out {fileName}.", e);
            }
        }
    }
}
