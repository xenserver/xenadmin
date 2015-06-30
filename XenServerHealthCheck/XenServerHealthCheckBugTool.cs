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
using System.IO;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin;
using System.Globalization;

namespace XenServerHealthCheck
{
    public class XenServerHealthCheckBugTool
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // The logs which are needed by Health Check.
        private static readonly List<string> bugtoolParam =
            new List<string> 
            {
                "xen-info:2",
                "filesystem_summarise:2",
                "xha-liveset:2",
                "high-availability:2",
                "firstboot:2",
                "xenserver-databases:2",
                "multipath:2",
                "disk-info:2",
                "xenserver-logs:2",
                "xenserver-install:2",
                "process-list:2",
                "xapi:2",
                "host-crashdump-logs:2",
                "xapi-subprocess:2",
                "pam:2",
                "tapdisk-logs:2",
                "kernel-info:2",
                "xenserver-config:2",
                "xenserver-domains:2",
                "device-model:2",
                "hardware-info:2",
                "xenopsd:2",
                "loopback-devices:2",
                "system-services:2",
                "system-logs:2",
                "network-status:2",
                "CVSM:2",
                "xcp-rrdd-plugins:2",
                "yum:2",
                "network-config:2",
                "boot-loader:2"
            };

        public readonly string outputFile;

        public XenServerHealthCheckBugTool()
        {
                string name = string.Format("{0}{1}.zip", Messages.BUGTOOL_FILE_PREFIX, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture));

                string folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                if (Directory.Exists(folder))
                    Directory.Delete(folder);
                Directory.CreateDirectory(folder);

                if (!name.EndsWith(".zip"))
                    name = string.Concat(name, ".zip");

                outputFile = string.Format(@"{0}\{1}", folder, name);
        }

        public void RunBugtool(IXenConnection connection, Session session)
        {
            if (connection == null || session == null)
                return;

            // Ensure downloaded filenames are unique even for hosts with the same hostname: append a counter to the timestring
            string filepath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            if (Directory.Exists(filepath))
                Directory.Delete(filepath);
            Directory.CreateDirectory(filepath);

            string timestring = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

            // Collect all master/slave information to output as a separate text file with the report
            List<string> mastersInfo = new List<string>();

            int i = 0;
            Pool p = Helpers.GetPool(connection);
            foreach (Host host in connection.Cache.Hosts)
            {
                // master/slave information
                if (p == null)
                {
                    mastersInfo.Add(string.Format("Server '{0}' is a stand alone server",
                        host.Name));
                }
                else
                {
                    mastersInfo.Add(string.Format("Server '{0}' is a {1} of pool '{2}'",
                        host.Name,
                        p.master.opaque_ref == host.opaque_ref ? "master" : "slave",
                        p.Name));
                }

                HostWithStatus hostWithStatus = new HostWithStatus(host, 0);
                SingleHostStatusAction statAction = new SingleHostStatusAction(hostWithStatus, bugtoolParam, filepath, timestring + "-" + ++i);
                statAction.RunExternal(session);
            }

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

            // Finish the collection of logs with bugtool.
            // Start to zip the files.
            ZipStatusReportAction zipAction = new ZipStatusReportAction(filepath, outputFile);
            zipAction.RunExternal(session);
            log.InfoFormat("Server Status Report is collected: {0}", outputFile);
        }

    }
}
