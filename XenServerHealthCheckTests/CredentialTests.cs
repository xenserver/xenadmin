using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XenAdmin;
using XenServerHealthCheck;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XenAdminTests;
using System.IO.Pipes;
using System.IO;

namespace XenServerHealthCheckTests
{
    class CredentialTests
    {
        private const char SEPARATOR = '\x202f'; // narrow non-breaking space.
        public static void CredentialReceiverTests()
        {
            CredentialReceiver.instance.Init();
            ServerListHelper.instance.Init();
            string HostName = "Host1";
            string UserName = "User1";
            string Password = "password1";
            int conSize = ServerListHelper.instance.GetServerList().Count;
            try
            {
                //1. Empty credential 
                NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", CallHomeSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
                pipeClient.Connect();
                string credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName }));
                pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
                pipeClient.Close();
                List<ServerInfo> con = ServerListHelper.instance.GetServerList();
                Assert.IsTrue(con.Count == conSize);

                //2. Send credential and check result
                pipeClient = new NamedPipeClientStream(".", CallHomeSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
                pipeClient.Connect();
                credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName, UserName, Password }));
                pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
                pipeClient.Close();
                System.Threading.Thread.Sleep(1000);
                con = ServerListHelper.instance.GetServerList();
                Assert.IsTrue(con.Count == conSize + 1);

                //3. Send credential twice and check result
                pipeClient = new NamedPipeClientStream(".", CallHomeSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
                pipeClient.Connect();
                credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName, UserName, Password }));
                pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
                pipeClient.Close();
                System.Threading.Thread.Sleep(1000);
                con = ServerListHelper.instance.GetServerList();
                Assert.IsTrue(con.Count == conSize + 1);

                //4. remove credential and check result
                pipeClient = new NamedPipeClientStream(".", CallHomeSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
                pipeClient.Connect();
                credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName }));
                pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
                pipeClient.Close();
                System.Threading.Thread.Sleep(1000);
                con = ServerListHelper.instance.GetServerList();
                Assert.IsTrue(con.Count == conSize);

                //5. add long credential size
                pipeClient = new NamedPipeClientStream(".", CallHomeSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
                pipeClient.Connect();
                HostName = "Host01234546789012345467890123454678901234546789012345467890123454678901234546789012345467890123454678901234546789";
                UserName = "User01234546789012345467890123454678901234546789012345467890123454678901234546789012345467890123454678901234546789";
                Password = "password101234546789012345467890123454678901234546789012345467890123454678901234546789012345467890123454678901234546789";
                credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName, UserName, Password }));
                pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
                pipeClient.Close();
                System.Threading.Thread.Sleep(1000);
                con = ServerListHelper.instance.GetServerList();
                Assert.IsTrue(con.Count == conSize + 1);

                //6. remove long credential size
                pipeClient = new NamedPipeClientStream(".", CallHomeSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
                pipeClient.Connect();
                credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName }));
                pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
                pipeClient.Close();
                System.Threading.Thread.Sleep(1000);
                con = ServerListHelper.instance.GetServerList();
                Assert.IsTrue(con.Count == conSize);


                //7. semd 2 credential
                pipeClient = new NamedPipeClientStream(".", CallHomeSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
                pipeClient.Connect();
                HostName = "host3";
                credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName, UserName, Password }));
                pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
                HostName = "host4";
                credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName, UserName, Password }));
                pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
                pipeClient.Close();
                System.Threading.Thread.Sleep(1000);
                con = ServerListHelper.instance.GetServerList();
                Assert.IsTrue(con.Count == conSize + 2);

                //8. remove 2 credential
                pipeClient = new NamedPipeClientStream(".", CallHomeSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
                pipeClient.Connect();
                HostName = "host3";
                credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName }));
                pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
                HostName = "host4";
                credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName }));
                pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
                pipeClient.Close();
                System.Threading.Thread.Sleep(1000);
                con = ServerListHelper.instance.GetServerList();
                Assert.IsTrue(con.Count == conSize);
            }
            catch (Exception)
            { }
            CredentialReceiver.instance.UnInit();
        }
    }
}
