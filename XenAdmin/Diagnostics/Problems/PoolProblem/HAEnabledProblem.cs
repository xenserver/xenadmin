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
using XenAdmin.Diagnostics.Checks;
using XenAPI;
using XenAdmin.Actions;
using System.Collections.Generic;


namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    class HAEnabledProblem : PoolProblem
    {
        private readonly long FailuresToTolerate;
        private readonly List<SR> HeartbeatSrs = new List<SR>();

        public HAEnabledProblem(Check check, Pool pool)
            : base(check, pool)
        {
            FailuresToTolerate = pool.ha_host_failures_to_tolerate;
            HeartbeatSrs = pool.GetHAHeartbeatSRs();
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            return new DisableHAAction(Pool);
        }

        public override AsyncAction CreateUnwindChangesAction()
        {
            var pool = Pool.Connection.Resolve(new XenRef<Pool>(Pool.opaque_ref));
            if (pool == null)
            {
                foreach (var xenConnection in ConnectionsManager.XenConnectionsCopy)
                {
                    pool = xenConnection.Resolve(new XenRef<Pool>(Pool.opaque_ref));
                    if (pool != null)
                        break;
                }
            }

            return pool != null ? new EnableHAAction(pool, null, HeartbeatSrs, FailuresToTolerate) : null;
        }

        public override string Description => String.Format(Messages.UPDATES_WIZARD_HA_ON_DESCRIPTION, Pool);

        public override string HelpMessage => Messages.TURN_HA_OFF;
    }

    class DrHAEnabledProblem : HAEnabledProblem
    {
        public DrHAEnabledProblem(Check check, Pool pool)
            : base(check, pool)
        {
        }

        public override string Description => Messages.DR_WIZARD_PROBLEM_HA_ENABLED;
    }

    internal class HaWlbEnabledWarning : Warning
    {
        private readonly Pool _pool;
        private readonly Host _host;

        public HaWlbEnabledWarning(Check check, Pool pool, Host host)
            : base(check)
        {
            _pool = pool;
            _host = host;
        }

        public override string Title => Check.Description;

        public override string Description
        {
            get
            {
                if (_pool.ha_enabled && _pool.wlb_enabled)
                    return string.Format(Messages.UPDATES_WIZARD_HA_AND_WLB_ON_WARNING, _host, _pool);

                if (_pool.ha_enabled)
                    return string.Format(Messages.UPDATES_WIZARD_HA_ON_WARNING, _host, _pool);

                if (_pool.wlb_enabled)
                    return string.Format(Messages.UPDATES_WIZARD_WLB_ON_WARNING, _host, _pool);

                return string.Empty;
            }
        }
    }
}
