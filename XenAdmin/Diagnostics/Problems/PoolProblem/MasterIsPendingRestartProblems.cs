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

using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAPI;

namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    class CoordinatorIsPendingRestartHostProblem : PoolProblem
    {
        public CoordinatorIsPendingRestartHostProblem(RestartHostOrToolstackPendingOnCoordinatorCheck check, Pool pool)
            : base(check, pool)
        { }

        public override string Description =>
            string.Format(
                ((RestartHostOrToolstackPendingOnCoordinatorCheck)Check).UpdateUuid != null
                    ? Messages.PROBLEM_COORDINATOR_PENDING_RESTART_HOST_THIS_UPDATE
                    : Messages.PROBLEM_COORDINATOR_PENDING_RESTART_HOST,
                Helpers.GetName(Pool).Ellipsise(30));

        public override string HelpMessage => null;
    }

    class CoordinatorIsPendingRestartToolstackProblem : PoolProblem
    {
        public CoordinatorIsPendingRestartToolstackProblem(RestartHostOrToolstackPendingOnCoordinatorCheck check, Pool pool)
            : base(check, pool)
        { }

        public override string Description =>
            string.Format(
                ((RestartHostOrToolstackPendingOnCoordinatorCheck)Check).UpdateUuid != null
                    ? Messages.PROBLEM_COORDINATOR_PENDING_RESTART_TOOLSTACK_THIS_UPDATE
                    : Messages.PROBLEM_COORDINATOR_PENDING_RESTART_TOOLSTACK,
                Helpers.GetName(Pool).Ellipsise(30));

        public override string HelpMessage => null;
    }
}
