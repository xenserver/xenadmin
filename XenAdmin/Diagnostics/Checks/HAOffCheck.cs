﻿/* Copyright (c) Citrix Systems, Inc. 
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

using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAPI;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Problems.PoolProblem;


namespace XenAdmin.Diagnostics.Checks
{
    public class HAOffCheck : Check
    {

        public HAOffCheck(Host host)
            : base(host)
        {
        }

        protected override Problem RunCheck()
        {
            if (!Host.IsLive())
                return new HostNotLiveWarning(this, Host);

            Pool pool = Helpers.GetPoolOfOne(Host.Connection);
            if (pool == null)
                return null;

            if (pool.ha_enabled)
                return new HAEnabledProblem(this, Helpers.GetPoolOfOne(Host.Connection));
            if (Helpers.WlbEnabled(pool.Connection))
                return new WLBEnabledProblem(this, Helpers.GetPoolOfOne(Host.Connection));
            return null;
        }

        public override string Description
        {
            get
            {
                return Messages.HA_CHECK_DESCRIPTION;
            }
        }

        public override string SuccessfulCheckDescription
        {
            get
            {
                var pool = Helpers.GetPool(Host.Connection);
                return string.Format(Messages.PATCHING_WIZARD_HOST_CHECK_OK, pool != null ? pool.Name() : Host.Name(), Description);
            }
        }
    }
}
