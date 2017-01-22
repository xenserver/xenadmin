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

using System.Collections.Generic;
using XenAPI;
using XenAdmin.Core;
using System;
using System.IO.Pipes;
using System.IO;
using System.Text;
using System.ServiceProcess;
using XenAdmin.Model;

namespace XenAdmin.Actions
{
    public class TransferHealthCheckSettingsAction : PureAsyncAction
    {
        private readonly Pool pool;
        HealthCheckSettings healthCheckSettings;
        string username;
        string password;

        public TransferHealthCheckSettingsAction(Pool pool, HealthCheckSettings healthCheckSettings, string username, string password, bool suppressHistory)
            : base(pool.Connection, Messages.ACTION_TRANSFER_HEALTHCHECK_SETTINGS, string.Format(Messages.ACTION_TRANSFER_HEALTHCHECK_SETTINGS, pool.Name), suppressHistory)
        {
            this.pool = pool;
            this.healthCheckSettings = healthCheckSettings;
            this.username = username;
            this.password = password;
        }

        private const char SEPARATOR = '\x202f'; // narrow non-breaking space.
        private string ProtectCredential(string Host, string username, string passwordSecret)
        {
            if (username == string.Empty || password == string.Empty)
                return EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { Host }));
            else
                return EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { Host, username, passwordSecret }));
        }

        private const string HEALTHCHECKSERVICENAME = "XenServerHealthCheck";

        protected override void Run()
        {
            ServiceController sc = new ServiceController(HEALTHCHECKSERVICENAME);
            try
            {
                if (sc.Status != ServiceControllerStatus.Running)
                    return;
            }
            catch
            {
                return;
            }

            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            int retryCount = 120;
            do
            {
                try
                {
                    pipeClient.Connect(0);
                }
                catch (System.TimeoutException exp)
                {
                    throw exp;
                }
                catch 
                {
                    System.Threading.Thread.Sleep(1000);
                    pipeClient = null;
                    pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
                }
            } while (!pipeClient.IsConnected && retryCount-- != 0);

            foreach (Host host in pool.Connection.Cache.Hosts)
            {
                if (host.IsMaster())
                {
                    string credential;
                    if (healthCheckSettings.Status == HealthCheckStatus.Enabled)
                        credential = ProtectCredential(host.address, username, password);
                    else
                        credential = ProtectCredential(host.address, string.Empty, string.Empty);
                    pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, (Encoding.UTF8.GetBytes(credential)).Length);
                    break;
                }
            }

            pipeClient.Write(Encoding.UTF8.GetBytes(HealthCheckSettings.HEALTH_CHECK_PIPE_END_MESSAGE), 0, HealthCheckSettings.HEALTH_CHECK_PIPE_END_MESSAGE.Length);
            pipeClient.Close();
        }
    }
}