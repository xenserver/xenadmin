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
using System.Net;
using CFUValidator.CommandLineOptions;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network;

namespace CFUValidator
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CommandLineParser parser = new CommandLineParser(args);
                parser.Parse();

                CFUCommandLineOptionManager manager = new CFUCommandLineOptionManager(parser.ParsedArguments);

                var cfuValidator = new CFUValidator(manager.XmlLocationType, manager.XmlLocation,
                    manager.ServerVersion, manager.InstalledHotfixes,
                    manager.CheckHotfixContents, new ConfigProvider(manager.Username, manager.ClientId));

                cfuValidator.Validate(Console.WriteLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                if (!(ex is CFUValidationException))
                    Console.WriteLine(ex.StackTrace);

                Environment.Exit(1);
            }

            Environment.Exit(0);
        }
    }

    public class ConfigProvider : IConfigProvider
    {
        public ConfigProvider(string username, string clientId)
        {
            FileServiceUsername = username;
            FileServiceClientId = clientId;
        }

        public string FileServiceUsername { get; }
        public string FileServiceClientId { get; }

        public string GetCustomTokenUrl()
        {
            return Registry.GetCustomTokenUrl() ?? InvisibleMessages.TOKEN_API_URL;
        }

        public IWebProxy GetProxyFromSettings(IXenConnection connection)
        {
            return null;
        }

        public IWebProxy GetProxyFromSettings(IXenConnection connection, bool isForXenServer)
        {
            return null;
        }
    }
}
