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

using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    class LegacySslProblem : PoolProblem
    {
        public LegacySslProblem(Check check, Pool pool)
            : base(check, pool)
        {
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            return new SetSslLegacyAction(Pool, false);
        }

        public override string Description =>
            string.Format(Messages.PROBLEM_LEGACY_PROTOCOL_DESCRIPTION, Pool, BrandManager.ProductVersion82);

        public override string HelpMessage => Messages.PROBLEM_LEGACY_PROTOCOL_HELP;
    }

    class LegacySslWarning : WarningWithMoreInfo
    {
        private readonly Pool pool;

        public LegacySslWarning(Check check, Pool pool)
            : base(check)
        {
            this.pool = pool;
        }

        public override string Description =>
            string.Format(Messages.PROBLEM_LEGACY_PROTOCOL_DESCRIPTION, pool, BrandManager.ProductVersion82);

        public override string Message =>
            string.Format(pool.IsVisible()
                    ? Messages.PROBLEM_LEGACY_PROTOCOL_INFO_POOL
                    : Messages.PROBLEM_LEGACY_PROTOCOL_INFO_SERVER, BrandManager.ProductVersion82);
    }
}
