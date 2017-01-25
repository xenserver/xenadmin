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
using System.Threading;

namespace XenAdminTests.PluginTests
{
    internal class ShellCmdTestBatchFile : IDisposable
    {
        public readonly string BatchFile;
        public readonly string FileThatBatchFileWillWrite;
        public readonly string Xml;

        public ShellCmdTestBatchFile()
        {
            string tempPath = Path.GetTempPath();
            FileThatBatchFileWillWrite = tempPath + "\\" + Guid.NewGuid();
            BatchFile = tempPath + "\\" + Guid.NewGuid() + ".bat";
            string batchFileContents = "echo %1 > \"" + FileThatBatchFileWillWrite + "\"";

            File.WriteAllText(BatchFile, batchFileContents);

            StringBuilder xml = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<XenCenterPlugin xmlns=\"http://www.citrix.com/XenCenter/Plugins/schema\" version=\"1\" plugin_name=\"TestPlugin\" plugin_version=\"1.0.0.0\">");
            xml.AppendLine("<MenuItem name=\"ShellCmdTestPlugin\" menu=\"file\" serialized=\"none\">");
            xml.AppendLine("<Shell filename=\"" + BatchFile + "\" window=\"true\"/></MenuItem></XenCenterPlugin>");

            Xml = xml.ToString();
        }

        #region IDisposable Members

        public void Dispose()
        {
            // small sleep here so file is definitely free to be deleted
            Thread.Sleep(1000);

            if (File.Exists(BatchFile))
            {
                File.Delete(BatchFile);
            }

            if (File.Exists(FileThatBatchFileWillWrite))
            {
                File.Delete(FileThatBatchFileWillWrite);
            }
        }

        #endregion
    }
}
