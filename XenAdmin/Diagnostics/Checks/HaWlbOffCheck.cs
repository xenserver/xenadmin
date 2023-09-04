﻿/* Copyright (c) Cloud Software Group, Inc. 
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

using XenAdmin.Core;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAPI;


namespace XenAdmin.Diagnostics.Checks
{
    class HaWlbOffCheck : PoolCheck
    {
        public HaWlbOffCheck(Pool pool)
            : base(pool)
        {
        }

        protected override Problem RunCheck()
        {
            if (Pool.ha_enabled)
                return new HAEnabledProblem(this, Pool);

            if (Helpers.WlbEnabled(Pool.Connection))
                return new WLBEnabledProblem(this, Pool);

            return null;
        }

        public override string Description => Messages.HA_WLB_CHECK_DESCRIPTION;

        public override string SuccessfulCheckDescription => string.Format(Messages.PATCHING_WIZARD_CHECK_ON_XENOBJECT_OK, Pool.Name(), Description);
    }
}