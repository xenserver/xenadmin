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

using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    class ContainerManagementProblem : ProblemWithMoreInfo
    {
        private readonly Pool _pool;

        public ContainerManagementProblem(Check check, Pool pool)
            : base(check)
        {
            _pool = pool;
        }

        public override string Description =>
            string.Format(Messages.PROBLEM_CONTAINER_MANAGEMENT_DESCRIPTION, _pool,
                string.Format(Messages.XENSERVER_8_2, BrandManager.ProductVersion82));

        public override string Message => Messages.PROBLEM_CONTAINER_MANAGEMENT_HELP;
    }

    class ContainerManagementWarning : WarningWithMoreInfo
    {
        private readonly Pool pool;

        public ContainerManagementWarning(Check check, Pool pool)
            : base(check)
        {
            this.pool = pool;
        }

        public override string Title => Check.Description;

        public override string Description =>
            string.Format(Messages.PROBLEM_CONTAINER_MANAGEMENT_DESCRIPTION, pool,
                string.Format(Messages.XENSERVER_8_2, BrandManager.ProductVersion82));

        public override string Message =>
            string.Format(Messages.PROBLEM_CONTAINER_MANAGEMENT_INFO,
                string.Format(Messages.XENSERVER_8_2, BrandManager.ProductVersion82));
    }
}
