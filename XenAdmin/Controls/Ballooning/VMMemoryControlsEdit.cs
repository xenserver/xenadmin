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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Controls.Ballooning
{
    public class VMMemoryControlsEdit : UserControl
    {
        // The maximum value allowed for memory for this VM
        private long maxMemAllowed = VM.DEFAULT_MEM_ALLOWED;
        private long _origStaticMax;

        protected bool hasBallooning;
        protected List<VM> vms;
        protected VM vm0;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<VM> VMs
        {
            get => vms;
            set
            {
                vms = value ?? new List<VM>();

                if (vms.Count > 0)
                {
                    vm0 = vms[0];
                    _origStaticMax = vm0.memory_static_max;
                    hasBallooning = vm0.SupportsBallooning();
                    maxMemAllowed = vms.Select(v => v.MaxMemAllowed()).Min();
                    Populate();
                }
                else
                {
                    maxMemAllowed = VM.DEFAULT_MEM_ALLOWED;
                }
            }
        }

        // These properties should be abstract, but you can't have an abstract class as the
        // base class of a control, otherwise the control can't be edited in the Designer.

        [Browsable(false)]
        protected virtual double dynamic_min => 0;

        [Browsable(false)]
        protected virtual double dynamic_max => 0;

        [Browsable(false)]
        protected virtual double static_max => 0;

        protected virtual void Populate()
        {
        }

        // A bit hacky, but needed to fix CA-35710. The ballooning dialog has to call this before
        // closing, otherwise the settings on the spinners may not have been taken yet.
        public void UnfocusSpinners()
        {
            SelectNextControl(Controls[0], true, true, true, true);
        }

        protected double maxDynMin = -1;  // signal value for no constraint
        protected void CalcMaxDynMin()
        {
            maxDynMin = -1;
            if (vm0.power_state == vm_power_state.Running || vm0.power_state == vm_power_state.Paused)
            {
                // Calculate the maximum possible value of dynamic_min, by looking at all the other VMs on this host
                // and obeying the constraint sum(dynamic_min) + control domain + virtualisation overhead <= total host memory
                Host host = vm0.Connection.Resolve(vm0.resident_on);
                if (host != null)
                {
                    Host_metrics host_metrics = host.Connection.Resolve(host.metrics);
                    if (host_metrics != null)
                    {
                        double sum_dyn_min = host.memory_overhead;

                        foreach (VM vm in host.Connection.ResolveAll(host.resident_VMs))
                        {
                            sum_dyn_min += vm.memory_overhead;

                            if (vm.is_control_domain)
                            {
                                VM_metrics vmm = vm.Connection.Resolve(vm.metrics);
                                if (vmm != null)
                                    sum_dyn_min += vmm.memory_actual;
                            }

                            else if (!vms.Contains(vm))
                                sum_dyn_min += vm.memory_dynamic_min;
                        }

                        maxDynMin = host_metrics.memory_total - sum_dyn_min;
                        if (maxDynMin < 0)
                            maxDynMin = 0;
                        maxDynMin /= vms.Count;
                    }
                }
            }
        }

        // Maximum for dynamic_min spinner: if we have set a maxDynMin, use it,
        // except when constrained by vm0.memory_static_min and dynamic_max.
        protected double DynMinSpinnerMax
        {
            get
            {
                double maxDM = dynamic_max;
                if (maxDynMin >= 0 && maxDynMin < maxDM)
                {
                    maxDM = maxDynMin;
                    if (maxDM < vm0.memory_static_min)
                        maxDM = vm0.memory_static_min;
                }
                // If we've already managed to exceed that value, don't trash the user's setting
                if (maxDM < dynamic_min)
                    maxDM = dynamic_min;
                return maxDM;
            }
        }

        // Minimum for dynamic_min spinner: constrained by both vm0.memory_static_min, and a proportion of static_max
        protected double DynMinSpinnerMin
        {
            get
            {
                double minDM = vm0.memory_static_min;
                long limit = (long)(static_max * GetMemoryRatio());
                if (limit > minDM)
                    minDM = limit;
                if (minDM > dynamic_min)
                    minDM = dynamic_min;
                return minDM;
            }
        }

        // Static_max also has a corresponding limit: it can't go to more than 1/frac of maxDynMin,
        // or there is no legal value of dynamic_min.
        protected double StatMaxSpinnerMax
        {
            get
            {
                double frac = GetMemoryRatio();
                double maxSM = maxDynMin >= 0 && MemorySpinnerMax * frac > (long)maxDynMin
                                 ? maxDynMin / frac
                                 : MemorySpinnerMax;
                if (maxSM < static_max)
                    maxSM = static_max;
                return maxSM;
            }
        }

        protected double MemorySpinnerMax
        {
            get
            {
                double server = vm0.memory_static_max;
                return (server > maxMemAllowed ? server : maxMemAllowed);
            }
        }

        public static double CalcIncrement(double staticMax, string units)
        {
            if (units == "MB")
            {
                // Calculate a suitable increment if the static_max is small
                int i = 1;
                while (i < staticMax / Util.BINARY_MEGA / 8 && i < 128)
                    i *= 2;
                
                return i * Util.BINARY_MEGA;
            }
            else
            {
               return (0.1 * Util.BINARY_GIGA);                
            }
        }

        protected double GetMemoryRatio()
        {
            return GetMemoryRatio(vms.ToArray());
        }

        /// <summary>
        /// Get the most constrained memory ratio for a set of VMs.
        /// It is assumed they are all on the same pool.
        /// </summary>
        public static double GetMemoryRatio(params VM[] vms)
        {
            bool hvm = false;
            bool pv = false;
            foreach (VM vm in vms)
            {
                if (vm.IsHVM())
                    hvm = true;
                else
                    pv = true;
            }

            Pool pool = Helpers.GetPoolOfOne(vms[0].Connection);
            Dictionary<string, string> poolOtherConfig = Helpers.GetOtherConfig(pool);
            if (!pv) // only HVM
                return GetMemoryRatioValue(poolOtherConfig, true);
            else if (!hvm) // only PV
                return GetMemoryRatioValue(poolOtherConfig, false);
            else // both types of VM: return the more constraining ratio
            {
                double hvmRatio = GetMemoryRatioValue(poolOtherConfig, true);
                double pvRatio = GetMemoryRatioValue(poolOtherConfig, false);
                return (hvmRatio >= pvRatio ? hvmRatio : pvRatio);
            }
        }

        private static double GetMemoryRatioValue(Dictionary<string, string> poolOtherConfig, bool hvm)
        {
            string key = (hvm ? "memory-ratio-hvm" : "memory-ratio-pv");
            string value;
            double frac = -1.0;
            if (poolOtherConfig.TryGetValue(key, out value))
            {
                try { frac = Convert.ToDouble(value, CultureInfo.InvariantCulture); }
                catch (FormatException) { }
                catch (OverflowException) { }
            }
            if (frac < 0.0 || frac > 1.0)
                frac = 0.25;  // default value for both HVM and PV if ratio absent or nonsensical
            return frac;
        }

        public bool ChangeMemorySettings()
        {
            // dynamic_min and dynamic_max should stay equal to static_max for VMs without ballooning

            var dynamicMin = hasBallooning ? (long)dynamic_min : (long)static_max;
            var dynamicMax = hasBallooning ? (long)dynamic_max : (long)static_max;
            var staticMax = (long)static_max;

            if (_origStaticMax / Util.BINARY_MEGA == staticMax / Util.BINARY_MEGA)
            {
                // don't want to show warning dialog just for rounding errors

                if (dynamicMin == staticMax)
                    dynamicMin = _origStaticMax;
                if (dynamicMax == staticMax)
                    dynamicMax = _origStaticMax;
                staticMax = _origStaticMax;
            }
            else
            {
                foreach (VM vm in VMs)
                {
                    if (vm.power_state == vm_power_state.Halted)
                        continue;
                    
                    // If not all VMs Halted, confirm static_max changes, and abort if necessary
                    // Six possible messages depending on the exact configuration.
                    // (Could have an additional message for VMs without ballooning but with tools,
                    // e.g. on a free host, for which we can do a clean shutdown: but having a false
                    // positive for the scarier message about force shutdown is not too bad).
                    // We assume that all VMs have the same has_ballooning.

                    string msg;
                    if (GetType() == typeof(VMMemoryControlsAdvanced))
                        msg = VMs.Count == 1
                            ? Messages.CONFIRM_CHANGE_STATIC_MAX_SINGULAR
                            : Messages.CONFIRM_CHANGE_STATIC_MAX_PLURAL;
                    else if (vm.SupportsBallooning() && !Helpers.FeatureForbidden(vm, Host.RestrictDMC))
                        msg = VMs.Count == 1
                            ? Messages.CONFIRM_CHANGE_MEMORY_MAX_SINGULAR
                            : Messages.CONFIRM_CHANGE_MEMORY_MAX_PLURAL;
                    else
                        msg = VMs.Count == 1
                            ? Messages.CONFIRM_CHANGE_MEMORY_SINGULAR
                            : Messages.CONFIRM_CHANGE_MEMORY_PLURAL;

                    using (var dlg = new WarningDialog(msg,
                        ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo))
                    {
                        if (dlg.ShowDialog(this) != DialogResult.Yes)
                            return false;
                    }

                    break;
                }
            }

            foreach (VM vm in VMs)
            {
                var action = new ChangeMemorySettingsAction(vm,
                    string.Format(Messages.ACTION_CHANGE_MEMORY_SETTINGS, vm.Name()),
                    vm.memory_static_min, dynamicMin, dynamicMax, staticMax,
                    VMOperationCommand.WarningDialogHAInvalidConfig, VMOperationCommand.StartDiagnosisForm, false);
                
                action.RunAsync();
            }

            return VMs.Count > 0;
        }
    }
}
