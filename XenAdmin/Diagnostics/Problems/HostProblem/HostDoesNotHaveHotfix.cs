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

using XenAdmin.Diagnostics.Checks;
using XenAdmin.Diagnostics.Hotfixing;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    class HostDoesNotHaveHotfix : HostProblem
    {
        public HostDoesNotHaveHotfix(HostHasHotfixCheck check, Host server)
            : base(check, server)
        {
        }

        public override string Description => string.Format(Messages.REQUIRED_HOTFIX_NOT_INSTALLED, ServerName);

        public override string HelpMessage => Messages.APPLY_HOTFIX;

        protected override Actions.AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            return new Actions.DelegatedAsyncAction(Server.Connection, string.Format(Messages.APPLYING_HOTFIX_TO_HOST, Server), "", "",
                (ss) =>
                    {
                        Hotfix hotfix = HotfixFactory.Hotfix(Server);
                        if (hotfix != null)
                            hotfix.Apply(Server, ss);
                    }, true);
        }
    }

    class HostDoesNotHaveHotfixWarning : Warning
    {
        private readonly Host host;

        public HostDoesNotHaveHotfixWarning(Check check, Host host)
            : base(check)
        {
            this.host = host;
        }

        public override string Title => Check.Description;

        public override string Description => string.Format(Messages.REQUIRED_HOTFIX_NOT_INSTALLED_WARNING, host);
    }
}
