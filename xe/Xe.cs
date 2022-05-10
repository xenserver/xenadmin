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
using System.Reflection;
using System.Text;


namespace ThinCLI
{
    static class MainClass
    {
        public static readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;

        public static void Main(string[] args)
        {
            var conf = new Config();

            string body = "";
            char[] eqsep = {'='};

            for (int i = 0; i < args.Length; i++)
            {
                string s = args[i];
                try
                {
                    if (s.Equals("-s"))
                    {
                        conf.Hostname = args[++i];
                    }
                    else if (s.Equals("-u"))
                    {
                        conf.Username = args[++i];
                    }
                    else if (s.Equals("-pw"))
                    {
                        conf.Password = args[++i];
                    }
                    else if (s.Equals("-p"))
                    {
                        conf.Port = int.Parse(args[++i]);
                    }
                    else if (s.Equals("-debug"))
                    {
                        conf.Debug = true;
                    }
                    else if (s.Equals("-version"))
                    {
                        Console.WriteLine(Version.ToString());
                        Environment.Exit(0);
                    }
                    else if (s.Equals("-help") || s.Equals("/?"))
                    {
                        Logger.Usage();
                        Environment.Exit(0);
                    }
                    else
                    {
                        if (s.Contains("="))
                            conf.EnteredParamValues.Add(s.Split(eqsep)[1]);

                        body += s + "\n";
                    }
                }
                catch
                {
                    Logger.Error("Failed to parse command-line arguments");
                    Logger.Usage();
                    Environment.Exit(1);
                }
            }

            if (string.IsNullOrEmpty(conf.Hostname))
            {
                Logger.Error("No hostname was specified.");
                Logger.Usage();
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(conf.Username))
            {
                Logger.Error("No username was specified.");
                Logger.Usage();
                Environment.Exit(1);
            }

            Messages.PerformCommand(body, conf);
        }
    }

    internal static class Logger
    {
        internal static void Usage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Usage:");
            sb.AppendLine("  xe -version").AppendLine();
            sb.AppendLine("  xe -help").AppendLine();
            sb.AppendLine("  xe -s <server> -u <username> -pw <password> [options] <command> <arguments>").AppendLine();
            sb.AppendLine("Options:");
            sb.AppendLine("  -p <port>");
            sb.AppendLine("  -debug");
            sb.AppendLine();
            sb.AppendLine("For command help, use xe -s <server> -u <user> -pw <password> [options] help");

            Console.WriteLine(sb);
        }

        internal static void Debug(string msg, Config conf)
        {
            if (conf.Debug)
                Console.WriteLine("Debug: " + msg);
        }

        internal static void Error(string x)
        {
            Console.WriteLine("Error: " + x);
        }

        internal static void Info(string x)
        {
            Console.WriteLine(x);
        }
    }
}
