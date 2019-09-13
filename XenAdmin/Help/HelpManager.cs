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
using System.Resources;
using XenAdmin.Core;


namespace XenAdmin.Help
{
    class HelpManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly ResourceManager resources;

        static HelpManager()
        {
            resources = new ResourceManager("XenAdmin.Help.HelpManager", typeof(HelpManager).Assembly);
        }

        internal static bool TryGetTopicId(string pageRef, out string topicId)
        {
            topicId = null;
            if (pageRef == null)
                return false;

            topicId = resources.GetString(pageRef);
            return topicId != null;
        }

        public static void Launch(string pageRef)
        {
            TryGetTopicId(pageRef, out string topicId);

            if (pageRef == null)
                log.WarnFormat("Attempted to launch help window with null help pageRef");
            else if (topicId == null)
                log.WarnFormat("Failed to find help topic ID for {0}", pageRef);
            else
                log.DebugFormat("Found help topic ID {0} for {1}", topicId, pageRef);

            var helpTopicUrl = GetHelpUrl(topicId);
            Program.OpenURL(helpTopicUrl);

            // record help usage
            Properties.Settings.Default.HelpLastUsed = DateTime.UtcNow.ToString("u");
            Settings.TrySaveSettings();
        }

        private static string GetHelpUrl(string topicId)
        {
            var helpQuery = string.Empty;
            var helpUrl = Registry.CustomHelpUrl;

            if (string.IsNullOrEmpty(helpUrl))
            {
                helpUrl = InvisibleMessages.HELP_URL;

                helpQuery = string.Format(InvisibleMessages.HELP_URL_QUERY,
                    $"{Branding.XENCENTER_VERSION}.{Program.Version.Revision}".Replace('.', '_'),
                    Messages.XENCENTER);
            }

            return (helpUrl + $"{topicId ?? "index"}.html" + helpQuery).ToLowerInvariant();
        }
    }

    internal interface IFormWithHelp
    {
        bool HasHelp();
    }

    internal interface IControlWithHelp
    {
        string HelpID { get; }
    }
}
