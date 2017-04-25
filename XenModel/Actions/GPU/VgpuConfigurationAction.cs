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

using System;
using System.Collections.Generic;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.GPU
{
    public class VgpuConfigurationAction : PureAsyncAction
    {
        private readonly Dictionary<PGPU, List<XenRef<VGPU_type>>> updatedEnabledVGpuListByPGpu;

        public VgpuConfigurationAction(Dictionary<PGPU, List<XenRef<VGPU_type>>> updatedEnabledVGpuListByPGpu, IXenConnection connection)
            : base(connection, Messages.ACTION_VGPU_CONFIGURATION_SAVING)
        {
            this.updatedEnabledVGpuListByPGpu = updatedEnabledVGpuListByPGpu;
            Description = Messages.ACTION_PREPARING;
            this.Pool = Core.Helpers.GetPool(connection);
        }

        protected override void Run()
        {
            Description = Messages.ACTION_VGPU_CONFIGURATION_SAVING;
            foreach(var kvp in updatedEnabledVGpuListByPGpu)
            {
                var pGpu = kvp.Key;
                PGPU.set_enabled_VGPU_types(Connection.Session,pGpu.opaque_ref, kvp.Value);
            }
            Description = Messages.ACTION_VGPU_CONFIGURATION_SAVED;
        }
    }
}
