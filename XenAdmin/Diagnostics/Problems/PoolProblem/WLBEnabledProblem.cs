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

using System.Threading;
using XenAdmin.Actions;
using XenAdmin.Actions.Wlb;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    class WLBEnabledProblem : PoolProblem
    {
        public WLBEnabledProblem(Check check, Pool pool)
            : base(check, pool)
        {
        }

        public override string Description
        {
            get { return string.Format(Messages.CHECK_WLB_ENABLED, Pool); }
        }

        public override string HelpMessage
        {
            get { return Messages.HELP_MESSAGE_DISABLE_WLB; }
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            return new DelegatedAsyncAction(Pool.Connection, Messages.HELP_MESSAGE_DISABLE_WLB, "", "",
                                            ss =>
                                                {
                                                    var action = new DisableWLBAction(Pool, false);
                                                    action.RunExternal(ss);
                                                    int count = 0;
                                                    while (Helpers.WlbEnabled(Pool.Connection) && count < 10)
                                                    {
                                                        Thread.Sleep(500);
                                                        count++;
                                                    }
                                                }, true);

        }
    }

    class WLBEnabledWarning : Warning
    {
        private readonly Pool pool;
        private readonly Host host;

        public WLBEnabledWarning(Check check, Pool pool, Host host)
            : base(check)
        {
            this.pool = pool;
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
                return string.Format(Messages.UPDATES_WIZARD_WLB_ON_WARNING, host, pool);
            }
        }
    }
}
