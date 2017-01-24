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
using XenAdmin.Diagnostics.Checks;
using XenAPI;
using XenAdmin.Dialogs;
using System.Drawing;

namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    public enum HostNotSafeToUpgradeReason { NotEnoughSpace, Default }

    public class HostNotSafeToUpgradeWarning : Warning
    {
        private readonly Host host;
        private HostNotSafeToUpgradeReason reason;

        public HostNotSafeToUpgradeWarning(Check check, Host host, HostNotSafeToUpgradeReason reason)
            : base(check)
        {
            this.host = host;
            this.reason = reason;
        }

        protected override Actions.AsyncAction CreateAction(out bool cancelled)
        {
            Program.Invoke(Program.MainWindow, delegate()
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, Message)))
                {
                    dlg.ShowDialog();
                }
            });

            cancelled = true;

            return null;
        }

        public override string HelpMessage
        {
            get
            {
                return Messages.PATCHINGWIZARD_MORE_INFO;
            }
        }

        public override string Title
        {
            get { return Description; }
        }

        public override string Description
        {
            get { return String.Format(ShortMessage, host.name_label); }
        }

        private string Message 
        {
            get
            {
                switch (reason)
                {
                    case HostNotSafeToUpgradeReason.NotEnoughSpace :
                        return Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE_LONG;
                    
                    default:
                        return Messages.NOT_SAFE_TO_UPGRADE_DEFAULT_WARNING_LONG;
                }
            }
        }

        private string ShortMessage
        {
            get
            {
                switch (reason)
                {
                    case HostNotSafeToUpgradeReason.NotEnoughSpace:
                        return Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE_SHORT;

                    default:
                        return Messages.NOT_SAFE_TO_UPGRADE_DEFAULT_WARNING_SHORT;
                }
            }
        }
    }
}
