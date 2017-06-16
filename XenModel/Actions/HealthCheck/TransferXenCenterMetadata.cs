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
using System.IO.Pipes;
using System.ServiceProcess;
using System.Text;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAPI;

namespace XenAdmin.Actions
{
    public class TransferXenCenterMetadata : PureAsyncAction
    {
        private string metadata;

        public TransferXenCenterMetadata(string metadata, bool suppressHistory)
            : base(null, Messages.ACTION_TRANSFER_HEALTHCHECK_SETTINGS, Messages.ACTION_TRANSFER_HEALTHCHECK_SETTINGS, suppressHistory)
        {
            this.metadata = metadata;
        }

        private const string HEALTHCHECKSERVICENAME = "XenServerHealthCheck";
        private const char SEPARATOR = '\x202f'; // narrow non-breaking space.

        protected override void Run()
        {
            try
            {
                ServiceController sc = new ServiceController(HEALTHCHECKSERVICENAME);
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

            byte[] metadataBytes = Encoding.UTF8.GetBytes(String.Join(SEPARATOR.ToString(), HealthCheckSettings.XENCENTER_METADATA, metadata));
            pipeClient.Write(metadataBytes, 0, metadataBytes.Length);

            byte[] endMsg = Encoding.UTF8.GetBytes(HealthCheckSettings.HEALTH_CHECK_PIPE_END_MESSAGE);
            pipeClient.Write(endMsg, 0, endMsg.Length);
            pipeClient.Close();
        }
    }
}
