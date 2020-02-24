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

using XenAdmin.Diagnostics.Checks;
using System;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    class PoolHasPVGuestWarningUrl : WarningWithInformationUrl
    {
        private readonly Pool _pool;

        public PoolHasPVGuestWarningUrl(Check check, Pool pool)
            : base(check)
        {
            _pool = pool;
        }

        private string PVGuestCheckUrl => string.Format(InvisibleMessages.PV_GUESTS_CHECK_URL);
        public override Uri UriToLaunch => new Uri(PVGuestCheckUrl);
        public override string Title => Description;

        public override string Description =>
            string.Format(Messages.POOL_HAS_PV_GUEST_WARNING, _pool.Name(),
                string.Format(Messages.XENSERVER_8_1, BrandManager.ProductVersion81));

        public override string HelpMessage => LinkText;
        public override string LinkText => Messages.LEARN_MORE;
    }
}
