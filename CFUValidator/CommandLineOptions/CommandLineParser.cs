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
using System.Linq;
using System.Text.RegularExpressions;

namespace CFUValidator.CommandLineOptions
{
    public class CommandLineParser
    {
        private readonly string[] args;

        public CommandLineArgument[] ParsedArguments { get; } =
        {
            new CommandLineArgument(OptionUsage.Help, 'h', "Display this help."),
            new CommandLineArgument(OptionUsage.CheckZipContents, 'c', "Optionally check the zip contents of the hotfixes."),
            new CommandLineArgument(OptionUsage.Url, 'u', "URL of CFU XML. Cannot be used with -f flag."),
            new CommandLineArgument(OptionUsage.File, 'f', "Path to CFU XML file. Cannot be used with -u flag."),
            new CommandLineArgument(OptionUsage.ServerVersion, 's', "Server version to test, eg. 6.0.2."),
            new CommandLineArgument(OptionUsage.Hotfix, 'p', "Space delimited list of patches/hotfixes to test, e.g. XS602E001."),
            new CommandLineArgument(OptionUsage.Username, 'n', "Username for downloading hotfixes, e.g. john123."),
            new CommandLineArgument(OptionUsage.ClientId, 'k', "ClientId to use for downloading hotfixes (alphanumeric string).")
        };

        public CommandLineParser(string[] args)
        {
            this.args = args;
        }

        public void Parse()
        {
            string[] recastArgs = Regex.Split(string.Join(" ", args).Trim(), @"(?=[-])(?<=[ ])");
            foreach (string arg in recastArgs)
            {
                if (String.IsNullOrEmpty(arg))
                    continue;

                string optionSwitch = Regex.Match(arg, "[-]{1}[a-z]{1}").Value.Trim();
                char switchChar = Convert.ToChar(optionSwitch.Substring(1, 1));
                string[] splitArgs = arg.Replace(optionSwitch, "").Trim().Split(' ');

                CommandLineArgument cla = ParsedArguments.FirstOrDefault(c => c.Switch == switchChar);
                if (cla != null)
                {
                    cla.Options = splitArgs.Where(s => !String.IsNullOrEmpty(s)).ToList();
                    cla.IsActiveOption = true;
                }
            }
        }
    }
}
