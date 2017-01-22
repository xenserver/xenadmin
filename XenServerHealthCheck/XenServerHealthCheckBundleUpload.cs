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
using System.IO;
using System.Threading;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAPI;


namespace XenServerHealthCheck
{
    public class XenServerHealthCheckBundleUpload
    {
        public XenServerHealthCheckBundleUpload(IXenConnection _connection)
        {
            connection = _connection;
            server.HostName = connection.Hostname;
            server.UserName = connection.Username;
            server.Password = connection.Password;
        }

        private IXenConnection connection;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public const int TIMEOUT = 24 * 60 * 60 * 1000;
        public const int INTERVAL = 10 * 1000;
        public const int VERBOSITY_LEVEL = 2;
        private ServerInfo server = new ServerInfo();

        public void runUpload(System.Threading.CancellationToken serviceStop)
        {
            DateTime startTime = DateTime.UtcNow;
            string uploadToken = "";
            Session session = new Session(connection.Hostname, 80);
            session.APIVersion = API_Version.LATEST;

            try
            {
                session.login_with_password(connection.Username, connection.Password, Helper.APIVersionString(API_Version.LATEST), Session.UserAgent);
                connection.LoadCache(session);
                var pool = Helpers.GetPoolOfOne(connection);
                if (pool != null)
                {
                    try
                    {
                        string opaqueref = Secret.get_by_uuid(session, pool.HealthCheckSettings.UploadTokenSecretUuid);
                        uploadToken = Secret.get_value(session, opaqueref);
                    }
                    catch (Exception e)
                    {
                        log.Error("Exception getting the upload token from the xapi secret", e);
                        uploadToken = null;
                    }
                }

                if (string.IsNullOrEmpty(uploadToken))
                {
                    if (session != null)
                        session.logout();
                    session = null;
                    log.ErrorFormat("The upload token is not retrieved for {0}", connection.Hostname);
                    updateHealthCheckSettings(false, startTime);
                    server.task = null;
                    ServerListHelper.instance.UpdateServerInfo(server);
                    return;
                }

            }
            catch (Exception e)
            {
                if (session != null)
                    session.logout();
                session = null;
                log.Error(e, e);
                updateHealthCheckSettings(false, startTime);
                server.task = null;
                ServerListHelper.instance.UpdateServerInfo(server);
                return;
            }

            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                Func<string> upload = delegate()
                {
                    try
                    {
                        return bundleUpload(connection, session, uploadToken, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        return "";
                    }
                };
                System.Threading.Tasks.Task<string> task = new System.Threading.Tasks.Task<string>(upload);
                task.Start();

                // Check if the task runs to completion before timeout.
                for (int i = 0; i < TIMEOUT; i += INTERVAL)
                {
                    // If the task finishes, set HealthCheckSettings accordingly.
                    if (task.IsCompleted || task.IsCanceled || task.IsFaulted)
                    {
                        if (task.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
                        {
                            string upload_uuid = task.Result;
                            if (!string.IsNullOrEmpty(upload_uuid))
                                updateHealthCheckSettings(true, startTime, upload_uuid);
                            else
                                updateHealthCheckSettings(false, startTime);
                        }
                        else
                            updateHealthCheckSettings(false, startTime);

                        server.task = null;
                        ServerListHelper.instance.UpdateServerInfo(server);
                        return;
                    }

                    // If the main thread (XenServerHealthCheckService) stops,
                    // set the cancel token to notify the working task to return.
                    if (serviceStop.IsCancellationRequested)
                    {
                        cts.Cancel();
                        updateHealthCheckSettings(false, startTime);
                        task.Wait();
                        server.task = null;
                        ServerListHelper.instance.UpdateServerInfo(server);
                        return;
                    }

                    System.Threading.Thread.Sleep(INTERVAL);
                }

                // The task has run for 24h, cancel the task and mark it as a failure upload.
                cts.Cancel();
                updateHealthCheckSettings(false, startTime);
                task.Wait();
                server.task = null;
                ServerListHelper.instance.UpdateServerInfo(server);
                return;
            }
            catch (Exception e)
            {
                if (session != null)
                    session.logout();
                session = null;
                log.Error(e, e);
                server.task = null;
                ServerListHelper.instance.UpdateServerInfo(server);
            }

        }

        public void updateHealthCheckSettings(bool success, DateTime time, string uploadUuid = "")
        {
            Session session = new Session(connection.Hostname, 80);
            session.login_with_password(connection.Username, connection.Password, Helper.APIVersionString(API_Version.LATEST), Session.UserAgent);
            connection.LoadCache(session);

            // Round-trip format time
            DateTime rtime = DateTime.SpecifyKind(time, DateTimeKind.Utc);
            string stime = HealthCheckSettings.DateTimeToString(rtime);

            // record upload_uuid,
            // release the lock,
            // set the time of LAST_SUCCESSFUL_UPLOAD or LAST_FAILED_UPLOAD
            Dictionary<string, string> config = Pool.get_health_check_config(session, connection.Cache.Pools[0].opaque_ref);
            config[HealthCheckSettings.UPLOAD_LOCK] = "";
            if (success)
            {
                config[HealthCheckSettings.LAST_SUCCESSFUL_UPLOAD] = stime;
                config[HealthCheckSettings.UPLOAD_UUID] = uploadUuid;
                // reset the NEW_UPLOAD_REQUEST field, if the current successful upload was started after the request
                DateTime newUploadRequestTime;
                if (HealthCheckSettings.TryParseStringToDateTime(config[HealthCheckSettings.NEW_UPLOAD_REQUEST], out newUploadRequestTime))
                {
                    if (rtime > newUploadRequestTime)
                        config[HealthCheckSettings.NEW_UPLOAD_REQUEST] = "";
                }
            }
            else
                config[HealthCheckSettings.LAST_FAILED_UPLOAD] = stime;
            Pool.set_health_check_config(session, connection.Cache.Pools[0].opaque_ref, config);

            if (session != null)
                session.logout();
            session = null;
        }

        public string bundleUpload(IXenConnection connection, Session session, string uploadToken, System.Threading.CancellationToken cancel)
        {
            // Collect the server status report and generate zip file to upload.
            XenServerHealthCheckBugTool bugTool = new XenServerHealthCheckBugTool();
            try
            {
                bugTool.RunBugtool(connection, session);
            }
            catch (Exception e)
            {
                if (session != null)
                    session.logout();
                session = null;
                log.Error(e, e);
                return "";
            }

            string bundleToUpload = bugTool.outputFile;
            if(string.IsNullOrEmpty(bundleToUpload) || !File.Exists(bundleToUpload))
            {
                log.ErrorFormat("Server Status Report is NOT collected");
                return "";
            }

            // Upload the zip file to CIS uploading server.
            string upload_url = Registry.HealthCheckUploadDomainName;
            log.InfoFormat("Upload report to {0}", upload_url);
            XenServerHealthCheckUpload upload = new XenServerHealthCheckUpload(uploadToken, VERBOSITY_LEVEL, upload_url, connection);

            string upload_uuid = "";
            try
            {
                upload_uuid = upload.UploadZip(bundleToUpload, null, cancel);
            }
            catch (Exception e)
            {
                if (session != null)
                    session.logout();
                session = null;
                log.Error(e, e);
                return "";
            }

            if (File.Exists(bundleToUpload))
                File.Delete(bundleToUpload);

            // Return the uuid of upload.
            if(string.IsNullOrEmpty(upload_uuid))
            {
                // Fail to upload the zip to CIS server.
                log.ErrorFormat("Fail to upload the Server Status Report {0} to CIS server", bundleToUpload);
                return "";
            }

            return upload_uuid;
        }
    }
}
