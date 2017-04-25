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
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;


namespace XenAdmin.Actions
{
    public class SystemStatusAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<HostWithStatus> hosts;
        private readonly List<string> entries;

        private List<SingleHostStatusAction> actions = new List<SingleHostStatusAction>();

        public bool SomethingToSave = false;
        private Object completeActionsMonitor = new Object();

        public SystemStatusAction(List<HostWithStatus> hosts,
                                  List<string> entries)
            : base(null, Messages.ACTION_SYSTEM_STATUS_TITLE, null)
        {
            this.hosts = hosts;
            this.entries = entries;
        }

        protected override void Run()
        {
            int n = hosts.Count;
            string filepath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            if (Directory.Exists(filepath))
                Directory.Delete(filepath);
            Directory.CreateDirectory(filepath);

            string timestring = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

            // Copy client logs
            string logDestination = string.Format("{0}\\{1}-{2}", filepath, timestring, InvisibleMessages.LOG_FILENAME);
            if (entries.Contains("client-logs"))
            {
                if (File.Exists(logDestination))
                    File.Delete(logDestination);
                string logPath = XenAdminConfigManager.Provider.GetLogFile(); 
                File.Copy(logPath, logDestination);
                // Copy old XenCenter.log.* files too
                DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(logPath));
                Regex regex = new Regex(Regex.Escape(Path.GetFileName(logPath)) + @"\.\d+");
                foreach (FileInfo file in di.GetFiles())
                {
                    if (regex.IsMatch(file.Name))
                        File.Copy(file.FullName, Path.Combine(filepath, timestring + "-" + file.Name));
                }
            }
            // collect all master/slave information to output as a separate text file with the report
            List<string> mastersInfo = new List<string>();

            int completedActions = hosts.Count;

            int i = 0;
            foreach (HostWithStatus host in hosts)
            {
                Description = string.Format(Messages.ACTION_SYSTEM_STATUS_DESCRIPTION, host.Host.Name);
                Pool = Helpers.GetPoolOfOne(host.Host.Connection);
                // master/slave information
                XenAPI.Pool p = Helpers.GetPool(host.Host.Connection);
                if (p == null)
                {
                    mastersInfo.Add(string.Format("Server '{0}' is a stand alone server",
                        host.Host.Name));
                }
                else
                {
                    mastersInfo.Add(string.Format("Server '{0}' is a {1} of pool '{2}'",
                        host.Host.Name,
                        p.master.opaque_ref == host.Host.opaque_ref ? "master" : "slave",
                        p.Name));
                }

                // Ensure downloaded filenames are unique even for hosts with the same hostname: append a counter to the timestring
                SingleHostStatusAction action = new SingleHostStatusAction(host, entries, filepath, timestring + "-" + ++i);
                actions.Add(action);
                action.Changed += (Action<ActionBase>)delegate
                    {
                        int total = 0;

                        foreach (SingleHostStatusAction a in actions)
                        {
                            total += a.PercentComplete;
                        }

                        PercentComplete = (int)(total / n);
                    };
                action.Completed += (Action<ActionBase>)delegate
                    {
                        lock (completeActionsMonitor)
                        {
                            completedActions--;
                            Monitor.PulseAll(completeActionsMonitor);
                        }
                    };
                action.RunAsync();
            }

            // output the slave/master info while we wait
            string mastersDestination = string.Format("{0}\\{1}-Masters.txt", filepath, timestring);
            if (File.Exists(mastersDestination))
                File.Delete(mastersDestination);

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(mastersDestination);
                foreach (string s in mastersInfo)
                    sw.WriteLine(s);

                sw.Flush();
            }
            catch (Exception e)
            {
                log.ErrorFormat("Exception while writing masters file: {0}", e);
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }

            // now wait for the status actions to return
            lock (completeActionsMonitor)
                while (completedActions > 0 && !Cancelling)
                    Monitor.Wait(completeActionsMonitor);

            if (Cancelling)
                throw new CancelledException();

            int failedHosts = 0;

            foreach (HostWithStatus h in hosts)
            {
                if (h.Status == HostStatus.failed)
                    failedHosts++;
                else
                    SomethingToSave = true;
            }

            Result = filepath;
            if (Cancelling || Cancelled)
                Description = Messages.ACTION_SYSTEM_STATUS_CANCELLED;
            else if (failedHosts > 0)
            {
                if (failedHosts == hosts.Count)
                {
                    if (entries.Contains("client-logs"))
                    {
                        SomethingToSave = true;
                        Description = Messages.ACTION_SYSTEM_STATUS_SUCCESSFUL_WITH_ERRORS_XCLOGS;
                    }
                    else
                        Description = Messages.ACTION_SYSTEM_STATUS_NONE_SUCCEEDED;
                }
                else
                    Description = Messages.ACTION_SYSTEM_STATUS_SUCCESSFUL_WITH_ERROR;
            }
            else
                Description = Messages.ACTION_SYSTEM_STATUS_SUCCESSFUL;
        }

        public override void RecomputeCanCancel()
        {
            if (this.IsCompleted)
            {
                CanCancel = false;
                return;
            }

            foreach (SingleHostStatusAction action in actions)
            {
                if (!action.IsCompleted)
                {
                    CanCancel = true;
                    return;
                }
            }

            CanCancel = false;
        }

        protected override void CancelRelatedTask()
        {
            foreach (SingleHostStatusAction action in actions)
            {
                action.Cancel();
            }

            lock (completeActionsMonitor)
                Monitor.PulseAll(completeActionsMonitor);

            Description = Messages.ACTION_SYSTEM_STATUS_CANCELLED;
        }
    }
}
