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

using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Actions
{
    public class NewVtpmAction : AsyncAction
    {
        private readonly VM _vm;

        public VTPM Vtpm { get; private set; }

        public NewVtpmAction(IXenConnection connection, VM vm)
            : base(connection, "", "", false)
        {
            _vm = vm;

            ApiMethodsToRoleCheck.Add("vtpm.create");
        }

        protected override void Run()
        {
            var vtpmRef = VTPM.create(Session, _vm.opaque_ref, true);
            Vtpm = Connection.TryResolveWithTimeout(vtpmRef);
        }
    }

    public class RemoveVtpmAction : AsyncAction
    {
        private readonly VTPM _vtpm;

        public RemoveVtpmAction(IXenConnection connection, VTPM vtpm)
            : base(connection, "", "", false)
        {
            _vtpm = vtpm;

            ApiMethodsToRoleCheck.AddRange("vtpm.destroy");
        }

        protected override void Run()
        {
            VTPM.destroy(Session, _vtpm.opaque_ref);
        }
    }
}
