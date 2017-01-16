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
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace XenAdmin.Core
{
    public class Processes
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /*
         * Based upon
         * http://blogs.msdn.com/jasonz/archive/2007/05/11/code-sample-is-your-process-using-the-silverlight-clr.aspx
         * and http://www.csharpfriends.com/Forums/ShowPost.aspx?PostID=27395.
         */
        /// <summary>
        /// Returns the parent Process of the given one, or null on failure.
        /// </summary>
        /// <returns></returns>
        public static Process FindParent(Process process)
        {
            int procid = process.Id;

            Win32.PROCESSENTRY32 pe32 = new Win32.PROCESSENTRY32();
            pe32.dwSize = (uint)Marshal.SizeOf(pe32);

            Win32.ToolHelpHandle thh = Win32.CreateToolhelp32Snapshot(Win32.SnapshotFlags.Process, 0);
            try
            {
                if (thh.IsInvalid)
                {
                    log.Error("CreateToolhelp32Snapshot failed");
                    return null;
                }

                if (Win32.Process32First(thh, ref pe32))
                {
                    do
                    {
                        if (pe32.th32ProcessID == procid)
                        {
                            return Process.GetProcessById(Convert.ToInt32(pe32.th32ParentProcessID));
                        }
                    }
                    while (Win32.Process32Next(thh, ref pe32));

                    // Maybe the given process has gone away?  I don't know if it's possible for a process
                    // to have no parent.
                    return null;
                }
                else
                {
                    log.Error("Process32First failed");
                    return null;
                }
            }
            finally
            {
                thh.Close();
            }
        }

        public static WindowsIdentity GetWindowsIdentity(Process process)
        {
            IntPtr token;
            try
            {
                if (Win32.OpenProcessToken(process.Handle, Win32.TOKEN_ACCESS.TOKEN_QUERY, out token))
                    return new WindowsIdentity(token);
                else
                    return null;
            }
            catch (Exception exn)
            {
                log.Warn("OpenProcessToken failed", exn);
                return null;
            }
        }

        public static string GetExePath(Process proc)
        {
            StringBuilder sb = new StringBuilder();
            Win32.GetModuleFileNameEx(proc.Handle, IntPtr.Zero, sb, 1024);
            return sb.ToString();
        }
    }
}
