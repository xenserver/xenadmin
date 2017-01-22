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
using System.Windows.Forms;
using XenAPI;

namespace XenAdmin.Controls.Ballooning
{
    public class VMMemoryControlsBase : UserControl
    {
        protected List<VM> vms;
        protected VM vm0;
        protected List<VM_metrics> vm_metrics = new List<VM_metrics>();  // metrics for all the VMs
        public virtual List<VM> VMs
        {
            set
            {
                vms = value;
                vm0 = vms[0];  // just an abbreviation for when we don't care which VM we look at
                vm_metrics = new List<VM_metrics>(vms.Count);
                foreach (VM vm in vms)
                    vm_metrics.Add(vm.Connection.Resolve(vm.metrics));
            }
        }

        protected long CalcMemoryUsed()
        {
            long memoryUsed = 0;

            if (vm0.power_state == vm_power_state.Running || vm0.power_state == vm_power_state.Paused)
            {
                // Calculate the average memory used by these VMs
                int count = 0;
                foreach (VM_metrics vm_metric in vm_metrics)
                {
                    if (vm_metric != null)
                    {
                        memoryUsed += vm_metric.memory_actual;
                        ++count;
                    }
                }
                if (count > 0)
                    memoryUsed /= count;
            }

            return memoryUsed;
        }
    }
}
