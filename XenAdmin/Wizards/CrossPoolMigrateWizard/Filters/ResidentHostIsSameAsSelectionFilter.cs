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
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard.Filters
{
    internal class ResidentHostIsSameAsSelectionFilter : ReasoningFilter
    {
        private readonly List<VM> _preSelectedVMs;

        public ResidentHostIsSameAsSelectionFilter(IXenObject item, List<VM> preSelectedVMs)
            :base(item)
        {
            _preSelectedVMs = preSelectedVMs ?? throw new ArgumentNullException(nameof(preSelectedVMs));
        }

        protected override bool FailureFoundFor(IXenObject itemToFilterOn, out string failureReason)
        {
            failureReason = Messages.HOST_MENU_CURRENT_SERVER;

            var residentHosts = from VM vm in _preSelectedVMs
                let home = vm.Home()
                where home != null
                select home;

            if (itemToFilterOn is Host)
                return residentHosts.Any(h => h == itemToFilterOn);

            if (itemToFilterOn is Pool tempPool)
            {
                if (tempPool.Connection.Cache.Hosts.Length > 1)
                    return false;

                //Pool with one host (or less)
                Host coordinator = tempPool.Connection.Resolve(tempPool.master);
                return residentHosts.Any(h => h == coordinator);
            }

            return false;
        }
    }
}
