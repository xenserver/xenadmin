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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFUValidator.CommandLineOptions
{
    internal class CFUCommandLineOptionManager
    {
        public CFUCommandLineOptionManager(CommandLineArgument[] args)
        {
            if (args.First(c => c.Usage == OptionUsage.Help).IsActiveOption ||
                args.All(c => !c.IsActiveOption))
                throw new CFUValidationException(GetHelp(args));

            CommandLineArgument urlArg = args.First(c => c.Usage == OptionUsage.Url);
            CommandLineArgument fileArg = args.First(c => c.Usage == OptionUsage.File);

            if (urlArg.IsActiveOption && fileArg.IsActiveOption)
                throw new CFUValidationException($"Switches '-{fileArg.Switch}' and '-{urlArg.Switch}' cannot be used at the same time");

            if (!urlArg.IsActiveOption && !fileArg.IsActiveOption)
                throw new CFUValidationException($"You must provide either option '-{fileArg.Switch}' or '-{urlArg.Switch}'");

            if (fileArg.IsActiveOption)
            {
                XmlLocation = fileArg.Options.First();
                XmlLocationType = fileArg.Usage;
            }
            else
            {
                XmlLocation = urlArg.Options.First();
                XmlLocationType = urlArg.Usage;
            }

            CheckHotfixContents = args.First(c => c.Usage == OptionUsage.CheckZipContents).IsActiveOption;

            var serverArg = args.First(c => c.Usage == OptionUsage.ServerVersion);
            ServerVersion = serverArg.IsActiveOption ? serverArg.Options.First() : AllVersions;

            var hotfixArg = args.First(c => c.Usage == OptionUsage.Hotfix);
            InstalledHotfixes = hotfixArg.IsActiveOption ? hotfixArg.Options : new List<string>();
        }

        public static string AllVersions = "999.999.999";

        public string XmlLocation { get; }

        public bool CheckHotfixContents { get; }

        public OptionUsage XmlLocationType { get; }

        public string ServerVersion { get; }

        public List<string> InstalledHotfixes { get; }

        private static string GetHelp(CommandLineArgument[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\nUsage:");
            sb.AppendLine("\n    CFUValidator.exe [options]");
            sb.AppendLine("\nOptions:");

            var orderedArgs = args.OrderBy(c => c.Switch);
            foreach (CommandLineArgument cla in orderedArgs)
                sb.AppendLine($"    -{cla.Switch}    {cla.Description}");
            return sb.ToString();
        }
    }
}
