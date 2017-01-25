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
using System.Linq;
using System.Text;
using XenAdmin;
using XenServerHealthCheck;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdminTests;
using System.IO.Pipes;
using System.IO;
using XenAdmin.Model;
using NUnit.Framework;

namespace XenAdminTests.HealthCheckTests
{
    public class CredentialTests : UnitTester_TestFixture
    {
        private const char SEPARATOR = '\x202f'; // narrow non-breaking space.

        [Test]
        public void CredentialOp()
        {
            CredentialReceiver.instance.Init();
            ServerListHelper.instance.Init();
            string HostName = "Host1";
            string UserName = "User1";
            string Password = "password1";
            int conSize = ServerListHelper.instance.GetServerList().Count;
            //1. Empty credential 
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            string credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName, null, null }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            pipeClient.Close();
            List<ServerInfo> con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize);

            //2. Send credential and check result
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName, UserName, Password }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize + 1);

            //3. Send credential twice and check result
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName, UserName, Password }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize + 1);

            //4. remove credential and check result
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize);

            //5. add long credential size
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
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
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
            pipeClient.Connect();
            credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { HostName }));
            pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
            pipeClient.Close();
            System.Threading.Thread.Sleep(1000);
            con = ServerListHelper.instance.GetServerList();
            Assert.IsTrue(con.Count == conSize);


            //7. semd 2 credential
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
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
            pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
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

            CredentialReceiver.instance.UnInit();
        }
    }
}
