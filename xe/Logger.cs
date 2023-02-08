/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Text;


namespace ThinCLI
{
    internal static class Logger
    {
        internal static void PrintUsage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Usage:");
            sb.AppendLine("  xe -version").AppendLine();
            sb.AppendLine("  xe -help").AppendLine();
            sb.AppendLine("  xe -s <server> -u <username> -pw <password> [options] <command> <arguments>").AppendLine();
            sb.AppendLine("Options:");
            sb.AppendLine("  -p <port>");
            sb.AppendLine("  -debug                     Show more detailed output and debug information");
            sb.AppendLine("  -no-warn-new-certificates  Do not prompt if a new certificate has been detected");
            sb.AppendLine("  -no-warn-certificates      Do not prompt if a certificate has changed");
            sb.AppendLine();
            sb.AppendLine("For command help, use xe -s <server> -u <user> -pw <password> [options] help");

            Console.WriteLine(sb);
        }

        internal static void Debug(string msg, Config conf)
        {
            if (conf.Debug)
                Console.WriteLine("[DEBUG] " + msg);
        }

        internal static void Debug(Exception ex, Config conf)
        {
            if (ex != null)
                Debug(ex.StackTrace, conf);
        }

        internal static void Error(string x)
        {
            Console.WriteLine("[ERROR] " + x);
        }

        internal static void Warn(string x)
        {
            Console.WriteLine("[WARNING] " + x);
        }

        internal static void Info(string x)
        {
            Console.WriteLine(x);
        }
    }
}
