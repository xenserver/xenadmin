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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Diagnostics.Hotfixing;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    class HostDoesNotHaveHotfix : HostProblem
    {
        private readonly HotfixFactory hotfixFactory = new HotfixFactory();

        public HostDoesNotHaveHotfix(HostHasHotfixCheck check, Host server)
            : base(check, server)
        {
        }

        public override string Description
        {
            get { return string.Format(Messages.REQUIRED_HOTFIX_ISNOT_INSTALLED, ServerName); }
        }

        public override string HelpMessage
        {
            get { return Messages.APPLY_HOTFIX; }
        }

        protected override Actions.AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            return new Actions.DelegatedAsyncAction(Server.Connection, string.Format(Messages.APPLYING_HOTFIX_TO_HOST, Server), "", "",
                (ss) =>
                    {
                        Hotfix hotfix = hotfixFactory.Hotfix(Server);
                        if (hotfix != null)
                            hotfix.Apply(Server, ss);
                    }, true);
        }
    }
}
