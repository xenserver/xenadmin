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

using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAPI;


namespace XenAdmin.Diagnostics.Checks
{
    internal class HaOffCheck : PoolCheck
    {
        public HaOffCheck(Pool pool)
            : base(pool)
        {
        }

        protected override Problem RunCheck()
        {
            return Pool.ha_enabled ? new HAEnabledProblem(this, Pool) : null;
        }

        public override string Description => Messages.HA_CHECK_DESCRIPTION;

        public override string SuccessfulCheckDescription => string.Format(Messages.PATCHING_WIZARD_CHECK_ON_XENOBJECT_OK, Pool.Name(), Description);
    }


    internal class WlbOffCheck : PoolCheck
    {
        public WlbOffCheck(Pool pool)
            : base(pool)
        {
        }

        protected override Problem RunCheck()
        {
            return Pool.wlb_enabled ? new WLBEnabledProblem(this, Pool) : null;
        }

        public override string Description => Messages.WLB_CHECK_DESCRIPTION;

        public override string SuccessfulCheckDescription => string.Format(Messages.PATCHING_WIZARD_CHECK_ON_XENOBJECT_OK, Pool.Name(), Description);
    }
}
