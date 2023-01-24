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
using System.Reflection;
using System.Text;
using ThinCLI.Properties;


namespace ThinCLI
{
    internal static class MainClass
    {
        public static readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;

        public static void Main(string[] args)
        {
            var conf = new Config();

            string body = "";

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
                    else if (s.Equals("-no-warn-new-certificates"))
                    {
                        conf.NoWarnNewCertificates = true;
                    }
                    else if (s.Equals("-no-warn-certificates"))
                    {
                        conf.NoWarnCertificates = true;
                    }
                    else if (s.Equals("-version"))
                    {
                        Console.WriteLine(Version.ToString());
                        return;
                    }
                    else if (s.Equals("-help") || s.Equals("/?"))
                    {
                        Logger.PrintUsage();
                        return;
                    }
                    else
                    {
                        if (s.Contains("="))
                            conf.EnteredParamValues.Add(s.Split('=')[1]);

                        body += s + "\n";
                    }
                }
                catch
                {
                    Logger.Error("Failed to parse command-line arguments");
                    Logger.PrintUsage();
                    Environment.Exit(1);
                }
            }

            Settings.UpgradeFromPreviousVersion(conf);

            if (string.IsNullOrEmpty(conf.Hostname))
            {
                Logger.Error("No hostname was specified.");
                Logger.PrintUsage();
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(conf.Username))
            {
                Logger.Error("No username was specified.");
                Logger.PrintUsage();
                Environment.Exit(1);
            }

            body += $"username={conf.Username}\n";
            body += $"password={conf.Password}"; //do not add a line break after the last string

            var command = "POST /cli HTTP/1.0\r\n" +
                          $"content-length: {Encoding.UTF8.GetBytes(body).Length}\r\n\r\n" +
                          body;

            try
            {
                Marshalling.PerformCommand(command, conf);
            }
            catch (ThinCliProtocolException tcpEx)
            {
                Logger.Error(tcpEx.Message);
                Logger.Debug(tcpEx, conf);
                Environment.Exit(tcpEx.ExitCode);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Debug(ex, conf);
                Environment.Exit(1);
            }
        }
    }

    public class ThinCliProtocolException : Exception
    {
        public ThinCliProtocolException(string msg = null, int exitCode = 1)
            : base(msg)
        {
            ExitCode = exitCode;
        }

        public int ExitCode { get; }
    }
}
