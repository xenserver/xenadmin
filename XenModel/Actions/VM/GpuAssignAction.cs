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
using System.Text;

using XenAdmin.Core;

using XenAPI;

namespace XenAdmin.Actions
{
    public class GpuAssignAction : PureAsyncAction
    {
        private readonly VM vm;
        private readonly List<VGPU> vGpus;

        public GpuAssignAction(VM vm, List<VGPU> vGpus)
            : base(vm.Connection, "Set GPU", true)
        {
            this.vm = vm;
            this.vGpus = vGpus;
        }

        protected override void Run()
        {
            // Remove any existing VGPUs before adding new ones
            foreach (VGPU vgpu in vm.Connection.ResolveAll(vm.VGPUs))
                VGPU.destroy(Session, vgpu.opaque_ref);

            if (vGpus == null || vGpus.Count == 0)  // The VM doesn't want a VGPU
                return;

            // Add the new VGPUs
            int index = 0;
            foreach (var vGpu in vGpus)
            {
                AddGpu(vm.Connection.Resolve(vGpu.GPU_group), vm.Connection.Resolve(vGpu.type), index + 11);
                index++;
            }
        }

        private void AddGpu(GPU_group gpuGroup, VGPU_type vGpuType, int device)
        {
            if (gpuGroup == null)
                return;

            //string device = "0";  // fixed at the moment, see PR-1060
            Dictionary<string, string> other_config = new Dictionary<string, string>();

            if (Helpers.FeatureForbidden(vm, Host.RestrictVgpu) || vGpuType == null)
                VGPU.async_create(Session, vm.opaque_ref, gpuGroup.opaque_ref, device.ToString(), other_config);
            else
                VGPU.async_create(Session, vm.opaque_ref, gpuGroup.opaque_ref, device.ToString(),
                    other_config, vGpuType.opaque_ref);
        }
    }
}
