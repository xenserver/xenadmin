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
using System.Text;
using System.Resources;
using System.Windows.Forms;
using System.Drawing;
using XenAdmin.Dialogs;


namespace XenAdmin.Help
{
    class HelpManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static readonly ResourceManager resources;

        static HelpManager()
        {
            resources = new ResourceManager("XenAdmin.Help.HelpManager", typeof(HelpManager).Assembly);
        }

        internal static string GetID(string pageref)
        {
            int id;
            string s = resources.GetString(pageref);
            return s != null && int.TryParse(s, out id) ? s : null;
        }

        public static void Launch(string pageref)
        {
            MainWindow w = Program.MainWindow;

            if (pageref != null)
            {
                log.DebugFormat("User Request Help ID for {0}", pageref);

                string s = GetID(pageref);
                if (s != null)
                {
                    log.DebugFormat("Help ID for {0} is {1}", pageref, s);
                    if (Properties.Settings.Default.DebugHelp && !Program.RunInAutomatedTestMode)
                    {
                        using (var dlg = new ThreeButtonDialog(
                           new ThreeButtonDialog.Details(
                               SystemIcons.Information,
                               string.Format(Messages.MESSAGEBOX_HELP_TOPICS, s, pageref),
                               Messages.XENCENTER)))
                        {
                            dlg.ShowDialog(w);
                        }
                    }
                    w.ShowHelpTopic(s);
                }
                else
                {
                    log.WarnFormat("Failed to find Help ID for {0}", pageref);
                    // Do not show the help window with TOC if the help ID is not found with the system running in AutomatedTest mode
                    if (!Program.RunInAutomatedTestMode)
                    {
                        if (Properties.Settings.Default.DebugHelp)
                        {
                            using (var dlg = new ThreeButtonDialog(
                               new ThreeButtonDialog.Details(
                                   SystemIcons.Error,
                                   string.Format(Messages.MESSAGEBOX_HELP_TOPIC_NOT_FOUND, pageref),
                                   Messages.MESSAGEBOX_HELP_TOPIC_NOT_FOUND)))
                            {
                                dlg.ShowDialog(w);
                            }
                        }
                        w.ShowHelpTOC();
                    }
                }
            }
            else
            {
                log.WarnFormat("Null help ID passed to Help Manager");
                // Do not show the help window with TOC if the help ID is not found with the system running in AutomatedTest mode
                if (!Program.RunInAutomatedTestMode)
                {
                    if (Properties.Settings.Default.DebugHelp)
                    {
                        using (var dlg = new ThreeButtonDialog(
                           new ThreeButtonDialog.Details(
                               SystemIcons.Error,
                               string.Format(Messages.MESSAGEBOX_HELP_TOPIC_NOT_FOUND, pageref),
                               Messages.MESSAGEBOX_HELP_TOPIC_NOT_FOUND)))
                        {
                            dlg.ShowDialog(w);
                        }
                    }
                    w.ShowHelpTOC();
                }
            }
        }

        public static bool HasHelpFor(string pageref)
        {
            return (pageref != null && pageref != "TabPageUnknown" && GetID(pageref) != null);
        }
    }
}
