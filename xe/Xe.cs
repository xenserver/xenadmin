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
                    if (s.StartsWith("server"))
                    {
                        conf.hostname = s.Split(eqsep)[1];
                    }
                    else if (s.Equals("-s"))
                    {
                        conf.hostname = args[++i];
                    }
                    else if (s.Equals("-u"))
                    {
                        conf.username = args[++i];
                    }
                    else if (s.Equals("-pw"))
                    {
                        conf.password = args[++i];
                    }
                    else if (s.Equals("-p"))
                    {
                        conf.port = int.Parse(args[++i]);
                    }
                    else if (s.Equals("-debug"))
                    {
                        conf.debug = true;
                    }
                    else if (s.Equals("-version"))
                    {
                        Console.WriteLine(Version.ToString());
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

            if (conf.hostname.Equals(""))
            {
                Logger.Error("No hostname was specified.");
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
            Console.WriteLine("Usage:");
            Console.WriteLine("  xe -s <server> -u <username> -pw <password> [-p <port>] <command> <arguments>");
            Console.WriteLine("For help, use xe -s <server> -u <user> -pw <password> [-p <port>] help");
        }

        internal static void Debug(string msg, Config conf)
        {
            if (conf.debug)
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
