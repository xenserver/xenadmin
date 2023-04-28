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
using System.Collections.Generic;
using System.Linq;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.UtilityProblem;

namespace XenAdmin.Diagnostics.Checks
{
    internal class UpgradeRequiresEua : UpgradeCheck
    {
        private readonly Uri _targetUri;
        public UpgradeRequiresEua(List<Host> hosts, Uri targetUri)
            : base(hosts)
        {
            _targetUri = targetUri;
        }

        public override bool CanRun() => Hosts.Any(Helpers.Post82X);

        protected override Problem RunCheck()
        {
            return new EuaNotAcceptedProblem( this, Hosts.Where(Helpers.Post82X).ToList(), _targetUri);
        }

        public override string Description => Messages.ACCEPT_EUA_CHECK_DESCRIPTION;

        public override bool Equals(object obj)
        {
            if (!(obj is UpgradeRequiresEua item))
            {
                return false;
            }


            return _targetUri.Equals(item._targetUri) && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _targetUri.GetHashCode() ^ base.GetHashCode();
        }
    }
}
