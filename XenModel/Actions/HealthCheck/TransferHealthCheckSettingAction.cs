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

using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAPI;
using XenCenterLib;

namespace XenAdmin.Actions
{
    public abstract class TransferDataToHealthCheckAction : AsyncAction
    {
        protected const char SEPARATOR = '\x202f'; // narrow non-breaking space.
        private const string HEALTHCHECKSERVICENAME = "XenServerHealthCheck";

        protected TransferDataToHealthCheckAction(IXenConnection connection, string title, string description, bool suppressHistory) 
            : base(connection, title, description, suppressHistory)
        {
        }

        protected abstract string GetMessageToBeSent();

        protected override void Run()
        {
            ServiceController sc = new ServiceController(HEALTHCHECKSERVICENAME);
            try
            {
                var services = ServiceController.GetServices();
                var found = services.FirstOrDefault(s => s.ServiceName == HEALTHCHECKSERVICENAME);
                if (found == null)
                    return;//the service is not installed
                if (sc.Status != ServiceControllerStatus.Running)
                    return;
            }
            catch
            {
                return;
            }

            //only collect metadata if needed, i.e. if the service is installed and running
            var message = GetMessageToBeSent();
            if (string.IsNullOrEmpty(message))
                return;

            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            int retryCount = 120;
            do
            {
                try
                {
                    pipeClient.Connect(0);
                }
                catch (System.TimeoutException)
                {
                    throw;
                }
                catch
                {
                    System.Threading.Thread.Sleep(1000);
                    pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
                }
            } while (!pipeClient.IsConnected && retryCount-- != 0);

            var bytes = Encoding.UTF8.GetBytes(message);
            pipeClient.Write(bytes, 0, bytes.Length);

            pipeClient.Write(Encoding.UTF8.GetBytes(HealthCheckSettings.HEALTH_CHECK_PIPE_END_MESSAGE), 0, HealthCheckSettings.HEALTH_CHECK_PIPE_END_MESSAGE.Length);
            pipeClient.Close();
        }
    }

    public class TransferHealthCheckSettingsAction : TransferDataToHealthCheckAction
    {
        private readonly Pool pool;
        HealthCheckSettings healthCheckSettings;
        private readonly string username;
        private readonly string password;

        public TransferHealthCheckSettingsAction(Pool pool, HealthCheckSettings healthCheckSettings, string username, string password, bool suppressHistory)
            : base(pool.Connection, Messages.ACTION_TRANSFER_HEALTHCHECK_SETTINGS, string.Format(Messages.ACTION_TRANSFER_HEALTHCHECK_SETTINGS, pool.Name()), suppressHistory)
        {
            this.pool = pool;
            this.healthCheckSettings = healthCheckSettings;
            this.username = username;
            this.password = password;
        }

        protected override string GetMessageToBeSent()
        {
            var host = Helpers.GetCoordinator(pool.Connection);
            if (host == null)
                return null;

            if (healthCheckSettings.Status == HealthCheckStatus.Enabled && (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)))
                return null; // do not send empty/null username or password (when the Health Check is enabled), as they will be ignored 

            var credential = healthCheckSettings.Status == HealthCheckStatus.Enabled
                ? string.Join(SEPARATOR.ToString(), host.address, username, password)
                : string.Join(SEPARATOR.ToString(), host.address);
            return EncryptionUtils.ProtectForLocalMachine(credential);
        }
    }
}