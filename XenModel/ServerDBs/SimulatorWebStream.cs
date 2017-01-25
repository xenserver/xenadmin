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
using System.Text;
using System.IO;
using System.Net;
using XenAPI;
using XenAdmin.ServerDBs.FakeAPI;

namespace XenAdmin.ServerDBs
{
    public class SimulatorWebStream : Stream
    {
        // might be null
        private SimulatorWebProxy simProxy;
        private Uri Url;

        private long position;
        private long Start;
        private bool CycleResponse = true;
        private string HttpResponse = " \n";

        public SimulatorWebStream(Uri url, IWebProxy proxy)
        {
            simProxy = proxy as SimulatorWebProxy;
            Url = url;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {

        }

        public override long Length
        {
            get
            {
                return HttpResponse.Length;
            }
        }

        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            for (int i = offset; i < count; i++)
            {
                if (position >= HttpResponse.Length)
                {
                    if (CycleResponse)
                        position = Start;
                    else
                    {
                        buffer[i] = unchecked((byte)-1);
                        return i - offset;
                    }
                }

                buffer[i] = (byte)HttpResponse[(int)position++];
            }

            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            position += offset;
            return position;
        }

        public override void SetLength(long value)
        {

        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = offset; i < count; i++)
                builder.Append((char)buffer[i]);

            ParseHttpHeader(builder.ToString());
        }

        private void ParseHttpHeader(string header)
        {
            string[] cmds = header.Split(' ');

            if (cmds.Length < 1)
                return;

            switch (cmds[0])
            {
                case "PUT":
                case "GET":
                case "CONNECT":
                    break;

                default:
                    return;
            }

            if (cmds.Length < 2)
                return;

            string[] param = cmds[1].Split('?');

            if (param.Length < 1)
                return;

            Dictionary<string, string> query = param.Length > 1 ? ParseQuery(param[1]) : new Dictionary<string, string>();

            Position = 0;
            Start = 0;
            CycleResponse = false;
            switch (param[0])
            {
                case "/pool_patch_upload":
                    CycleResponse = true;
                    HttpResponse = "HTTP 200 simulator\n \n";
                    PoolPatchUpload(query);
                    break;

                case "/rrd_updates":
                    string host_ref = HostRef();
                    string uuid = (string)simProxy.proxy.db.GetValue("host", host_ref, "uuid");
                    HttpResponse = string.Format(@"HTTP 200 simulator
 
<xport>
  <meta>
    <start>0</start>
    <step>5</step>
    <end>0</end>
    <rows>1</rows>
    <columns>1</columns>
    <legend>
      <entry>AVERAGE:host:{0}:cpu0</entry>
    </legend>
  </meta>
  <data>
    <row>
      <t>{1}</t>
      <v>{2}</v>
    </row>
  </data>
</xport>", uuid, (long)(new TimeSpan(DateTime.Now.Ticks - new DateTime(1970, 1, 1).Ticks).TotalSeconds), CpuStatsForHost(host_ref));
                    break;

                case "/console":
                    StringBuilder response = GetConsoleResponse();
                    if (response != null)
                    {
                        CycleResponse = true;
                        Start = response.Length;
                        response.Append(WriteFlag(false));
                        HttpResponse = response.ToString();
                    }
                    else
                    {
                        HttpResponse = " \n";
                    }
                    break;

                case "/host_backup":
                    HttpResponse = "HTTP 200 simulator\n \n" + header;
                    break;
                default:
                    //Support for export
                    if (param[0].StartsWith("/%3Fsession_id="))
                    {
                        HttpResponse = GetVMExportResponse();
                    }
                    else
                        HttpResponse = " \n";
                    break;
            }
        }

        protected virtual string GetVMExportResponse()
        {
            return null;
        }

        protected virtual StringBuilder GetConsoleResponse()
        {
            return null;
        }

        protected string WriteCard32(int p)
        {
            byte[] bytes = BitConverter.GetBytes(p);
            Array.Reverse(bytes);
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append((char)b);
            }
            return builder.ToString();
        }

        protected string WriteCard16(short p)
        {
            byte[] bytes = BitConverter.GetBytes(p);
            Array.Reverse(bytes);
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append((char)b);
            }
            return builder.ToString();
        }

        protected string WriteCard8(byte p)
        {
            return ((char)p).ToString();
        }

        protected string WriteFlag(bool p)
        {
            return p ? ((char)1).ToString() : ((char)0).ToString();
        }

        protected string WriteString(string p)
        {
            return string.Format("{0}{1}", WriteCard32(p.Length), p);
        }

        private string HostRef()
        {
            return simProxy.proxy.db.ObjectWithFieldValue("host", "address", Url.Host);
        }
        Random rand = new Random();
        private double CpuStatsForHost(string host_ref)
        {
            switch (host_ref)
            {
                // hack for ExpectedResults search test
                case "OpaqueRef:d3a48ddc-8261-33df-64b2-1309e98b395d":
                    return rand.Next(0, 49)/100d;

                case "OpaqueRef:83b5a7dc-4eae-ba93-f9ca-53ef5a664814":
                    return rand.Next(50, 100)/100d;

                default:
                    return rand.Next(0, 100)/100d;

            }
        }

        private void PoolPatchUpload(Dictionary<string, string> query)
        {
            string task_ref = query["task_id"];
            string session_id = query["session_id"];
            Proxy_Task task = simProxy.proxy.GetTask(task_ref);
            Proxy_Pool_patch patch = new Proxy_Pool_patch();

            patch.after_apply_guidance = new string[] { "unknown" };
            patch.name_label = "fake-patch";
            patch.name_description = "";
            patch.size = "0";
            patch.uuid = Guid.NewGuid().ToString();
            patch.version = "1.0";

            task.result = new fakePool_Patch(simProxy.proxy).create(session_id, patch).Value;
            task.finished = DateTime.Now;
            task.status = "success";
        }

        private Dictionary<string, string> ParseQuery(string query)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string q in query.Split('&'))
            {
                string[] var = q.Split('=');
                dict[var[0]] = var[1];
            }
            return dict;
        }
    }
}
