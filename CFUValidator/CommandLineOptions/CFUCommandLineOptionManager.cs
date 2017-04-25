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
using System.Linq;
using System.Text;

namespace CFUValidator.CommandLineOptions
{
    internal class CFUCommandLineOptionManager
    {
        private readonly List<CommandLineArgument> clas;
        public CFUCommandLineOptionManager(List<CommandLineArgument> clas)
        {
            this.clas = clas;
        }

        //Not to be called from inside this class to keep IoC
        public static List<CommandLineArgument> EmptyArguments
        {
            get
            {
                return new List<CommandLineArgument>
                       {
                           new CommandLineArgument( OptionUsage.Help, 'h', "Display this help" ),
                           new CommandLineArgument( OptionUsage.CheckZipContents, 'c', "Optionally check the zip contents of the hotfixes" ),
                           new CommandLineArgument( OptionUsage.Url, 'u', "<URL to extract XML from> Cannot be used with -f flag"),
                           new CommandLineArgument( OptionUsage.File, 'f', "<File name to extract XML from> Cannot be used with -u flag" ),
                           new CommandLineArgument( OptionUsage.ServerVersion, 's', "<Server version to test> eg. 6.0.2" ),
                           new CommandLineArgument( OptionUsage.Hotfix, 'p', "<List of patches/hotfixes that server has> eg. XS602E001 (space delimited)" )
                       };
            }
        }

        public static string AllVersions = "999.999.999";

        public string XmlLocation { get { return GetFileUsageCLA().Options.First(); } }

        public bool CheckHotfixContents { get { return clas.First(c => c.Usage == OptionUsage.CheckZipContents).IsActiveOption; } }

        private CommandLineArgument GetFileUsageCLA()
        {
            CommandLineArgument claForUrl = clas.First(c => c.Usage == OptionUsage.Url);
            CommandLineArgument claForFle = clas.First(c => c.Usage == OptionUsage.File);
            if (claForUrl.IsActiveOption && claForFle.IsActiveOption)
                throw new CFUValidationException(String.Format("Switches '-{0}' and '-{1}' cannot be used at the same time", claForFle.Switch, claForUrl.Switch));

            if (!claForUrl.IsActiveOption && !claForFle.IsActiveOption)
                throw new CFUValidationException(String.Format("You must provide either option '-{0}' or '-{1}'", claForFle.Switch, claForUrl.Switch));

            if (claForFle.IsActiveOption)
                return claForFle;

            return claForUrl;
        }

        public OptionUsage FileSource { get {return GetFileUsageCLA().Usage; } }

        public string ServerVersion 
        { 
            get
            {
                CommandLineArgument cla = clas.First(c => c.Usage == OptionUsage.ServerVersion);
                return !cla.IsActiveOption ? AllVersions : cla.Options.First();
            }
        }

        public List<string> InstalledHotfixes
        { 
            get
            {
                CommandLineArgument cla = clas.First(c => c.Usage == OptionUsage.Hotfix);
                if (!cla.IsActiveOption)
                    return new List<string>();
                return cla.Options;
            }
        }


        public bool IsHelpRequired
        {
            get
            {
                return clas.First(c => c.Usage == OptionUsage.Help).IsActiveOption;
            }
        }

        public string Help
        {
            get
            {
                StringBuilder sb = new StringBuilder("Execute the command with the following command line options\n\nOptions:\n");
                foreach (CommandLineArgument cla in clas.OrderBy(c => c.Switch))
                {
                    sb.AppendLine(String.Format("-{0} {1}", cla.Switch, cla.Description));
                }
                return sb.ToString();
            }
        }

    }
}
