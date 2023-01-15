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

using System.Collections.Generic;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    class PowerOniLoProblem : HostProblem
    {
        public PowerOniLoProblem(Check check, Host host)
            : base(check, host)
        {
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            var mode = new KeyValuePair<Host, Host.PowerOnMode>(Server, new Host.PowerOnModeDisabled());
            return new SavePowerOnSettingsAction(Server.Connection,
                new List<KeyValuePair<Host, Host.PowerOnMode>> { mode }, false);
        }

        public override string Description =>
            string.Format(Messages.PROBLEM_POWER_ON_ILO_DESCRIPTION, Server, BrandManager.ProductVersion82);

        public override string HelpMessage => Messages.PROBLEM_POWER_ON_ILO_HELP;
    }

    class PowerOniLoWarning : WarningWithMoreInfo
    {
        private readonly Host host;

        public PowerOniLoWarning(Check check, Host host)
            : base(check)
        {
            this.host = host;
        }

        public override string Description =>
            string.Format(Messages.PROBLEM_POWER_ON_ILO_DESCRIPTION, host, BrandManager.ProductVersion82);

        public override string Message =>
            string.Format(Messages.PROBLEM_POWER_ON_ILO_INFO, BrandManager.ProductVersion82);
    }
}
