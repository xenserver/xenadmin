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

using XenAdmin.Actions;
using XenAdmin.Actions.HostActions;
using XenAdmin.Commands;
using XenAdmin.Diagnostics.Checks;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    public class HostNotLive : HostProblem
    {
        public HostNotLive(Check check, Host host)
            : base(check, host)
        {
        }

        public override string Description
        {
            get { return string.Format(Messages.UPDATES_WIZARD_HOST_NOT_LIVE, ServerName); }
        }

        public override string HelpMessage
        {
            get { return CanStartHost() ? Messages.START_HOST : string.Empty; }
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            if (CanStartHost())
                return new HostPowerOnAction(Server);
            return null;
        }

        private bool CanStartHost()
        {
            bool canStartHost = false;
            Program.Invoke(Program.MainWindow, delegate
               {
                   var powerOnCommand = new PowerOnHostCommand(Program.MainWindow, Server);
                   canStartHost = powerOnCommand.CanExecute();
               });
            return canStartHost;
        }
    }

    class HostNotLiveWarning : Warning
    {
        private readonly Host host;

        public HostNotLiveWarning(Check check, Host host)
            : base(check)
        {
            this.host = host;
        }

        public override string Title
        {
            get { return Check.Description; }
        }

        public override string Description
        {
            get
            {
                return string.Format(Messages.UPDATES_WIZARD_HOST_NOT_LIVE_WARNING, host);
            }
        }
    }
}
