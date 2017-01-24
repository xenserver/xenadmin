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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace XenAdmin.ServerDBs
{
    class XenAdminSimulatorWebStream : SimulatorWebStream
    {
        public XenAdminSimulatorWebStream(Uri url, IWebProxy proxy) : base(url, proxy)
        {
        }

        protected override string GetVMExportResponse()
        {
            StreamReader sr = new StreamReader(new FileStream(@"./TestResources/vmexport.bin", FileMode.Open));
            return "HTTP 200 simulator\n \n" + sr.ReadToEnd();
        }

        protected override StringBuilder GetConsoleResponse()
        {
            Bitmap i = new Bitmap(Image.FromFile(string.Format("{0}\\TestResources\\console.png", Program.AssemblyDir)));
            StringBuilder builder = new StringBuilder();
            builder.Append("HTTP 200 simulator\n \n");
            builder.Append("RFB 003.003\n");
            builder.Append(WriteCard32(1)); // securitiy protocol 1
            builder.Append(WriteCard16((short)i.Width)); // width
            builder.Append(WriteCard16((short)i.Height)); // height
            builder.Append(WriteCard8(32)); // bitsPerPixel
            builder.Append(WriteCard8(32)); // depth
            builder.Append(WriteFlag(false)); // bigEndian
            builder.Append(WriteFlag(true)); // trueColor
            builder.Append(WriteCard16(255)); // redMax
            builder.Append(WriteCard16(255)); // greenMax
            builder.Append(WriteCard16(255)); // blueMax
            builder.Append(WriteCard8(16)); // redShift
            builder.Append(WriteCard8(8)); // greenShift
            builder.Append(WriteCard8(0)); // blueShift
            builder.Append(WriteCard8(0)); // padding1
            builder.Append(WriteCard8(0)); // padding2
            builder.Append(WriteCard8(0)); // padding3
            builder.Append(WriteString("fake")); // string length
            builder.Append(WriteCard8(0)); // frame buffer update
            builder.Append(WriteCard8(0)); // padding
            builder.Append(WriteCard16(1)); // 1 rectangle
            builder.Append(WriteCard16(0)); // x
            builder.Append(WriteCard16(0)); // y
            builder.Append(WriteCard16((short)i.Width)); // width
            builder.Append(WriteCard16((short)i.Height)); // height
            builder.Append(WriteCard32(0)); // raw encoding

            for (int y = 0; y < i.Height; y++)
            {
                for (int x = 0; x < i.Width; x++)
                {
                    Color c = i.GetPixel(x, y);
                    builder.Append(WriteCard32(c.ToArgb()));
                }
            }
            return builder;
        }
    }
}
