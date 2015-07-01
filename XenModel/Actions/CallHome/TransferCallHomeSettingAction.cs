using System.Collections.Generic;
using XenAPI;
using XenAdmin.Core;
using System;
using System.IO.Pipes;
using System.IO;
using System.Text;

namespace XenAdmin.Actions
{
    public class TransferCallHomeSettingsAction : PureAsyncAction
    {
        private readonly Pool pool;
        CallHomeSettings callHomeSettings;
        string username;
        string password;

        public TransferCallHomeSettingsAction(Pool pool, CallHomeSettings callHomeSettings, string username, string password, bool suppressHistory)
            : base(pool.Connection, Messages.ACTION_TRANSFER_CALLHOME_SETTINGS, string.Format(Messages.ACTION_TRANSFER_CALLHOME_SETTINGS, pool.Name), suppressHistory)
        {
            this.pool = pool;
            this.callHomeSettings = callHomeSettings;
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

        protected override void Run()
        {
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", CallHomeSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
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
                    pipeClient = new NamedPipeClientStream(".", CallHomeSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
                }
            } while (!pipeClient.IsConnected && retryCount-- != 0);

            foreach (Host host in pool.Connection.Cache.Hosts)
            {
                if (host.IsMaster())
                {
                    string credential;
                    if (callHomeSettings.Status == CallHomeStatus.Enabled)
                        credential = ProtectCredential(host.address, username, password);
                    else
                        credential = ProtectCredential(host.address, string.Empty, string.Empty);
                    pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
                    break;
                }
            }

            pipeClient.Write(Encoding.UTF8.GetBytes(CallHomeSettings.HEALTH_CHECK_PIPE_END_MESSAGE), 0, CallHomeSettings.HEALTH_CHECK_PIPE_END_MESSAGE.Length);
            pipeClient.Close();
        }
    }
}