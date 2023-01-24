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
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;


namespace ThinCLI.Properties
{
    partial class Settings
    {
        public static void SetKnownServers(Dictionary<string, string> knownServers)
        {
            Default.KnownServers = knownServers == null
                ? Array.Empty<string>()
                : knownServers.Select(kvp => $"{kvp.Key} {kvp.Value}").ToArray();
        }

        public static Dictionary<string, string> GetKnownServers()
        {
            var known = new Dictionary<string, string>();

            var knownServers = Default.KnownServers;
            if (knownServers == null)
                return known;

            foreach (string knownHost in knownServers)
            {
                string[] hostCert = knownHost.Split(' ');
                if (hostCert.Length != 2)
                    continue;

                known.Add(hostCert[0], hostCert[1]);
            }

            return known;
        }

        public static void TrySaveSettings(Config conf)
        {
            try
            {
                Default.Save();
                Logger.Debug("Settings saved.", conf);
            }
            catch (Exception e)
            {
                Logger.Warn($"Failed to save settings. {e.Message}");
            }
        }

        /// <summary>
        /// Use this instead of Upgrade() as the latter does not import
        /// settings from previous versions if the program hash has changed.
        /// </summary>
        public static void UpgradeFromPreviousVersion(Config conf)
        {
            if (!Default.DoUpgrade)
                return;

            string userConfigPath;

            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);
                userConfigPath = config.FilePath;
            }
            catch (ConfigurationErrorsException exc)
            {
                userConfigPath = exc.Filename;
            }

            try
            {
                Logger.Debug("Looking for settings from previous versions...", conf);

                var currentVersionFolder = new DirectoryInfo(userConfigPath).Parent;
                var currentVersion = MainClass.Version;

                FileInfo previousConfig = null;
                Version previousVersion = null;

                var directories = currentVersionFolder?.Parent?.Parent?.GetDirectories();
                if (directories != null)
                {
                    foreach (var dir in directories)
                    {
                        var files = dir.GetFiles("user.config", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            var configFolderName = Path.GetFileName(Path.GetDirectoryName(file.FullName));
                            if (configFolderName == null)
                                continue;

                            var configVersion = new Version(configFolderName);

                            if (configVersion < currentVersion && (previousVersion == null || previousVersion < configVersion))
                            {
                                previousVersion = configVersion;
                                previousConfig = file;
                            }
                        }
                    }
                }

                var dirCreated = false;
                var fileCopied = false;
                string destinationFolder = null;
                string destinationFile = null;

                if (previousConfig != null)
                {
                    Logger.Debug($"Found previous settings file {previousConfig.FullName}.", conf);

                    destinationFolder = Path.Combine(currentVersionFolder.Parent.FullName, previousVersion.ToString());

                    if (!Directory.Exists(destinationFolder))
                    {
                        Directory.CreateDirectory(destinationFolder);
                        dirCreated = true;
                    }

                    destinationFile = Path.Combine(destinationFolder, "user.config");
                    if (!File.Exists(destinationFile))
                    {
                        Logger.Debug($"Copying settings from {previousConfig.FullName} to {destinationFile}...", conf);
                        
                        File.Copy(previousConfig.FullName, destinationFile);
                        fileCopied = true;
                    }
                }

                Default.Upgrade();
                Default.DoUpgrade = false;
                Logger.Debug("Settings upgraded.", conf);
                TrySaveSettings(conf);

                try
                {
                    if (dirCreated)
                        Directory.Delete(destinationFolder, true);
                    else if (fileCopied)
                        File.Delete(destinationFile);
                }
                catch
                {
                    //ignore
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("Failed to upgrade settings.", conf);
                Logger.Debug(ex, conf);
            }
        }
    }
}
