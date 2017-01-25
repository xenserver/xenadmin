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
using XenAPI;


namespace XenAdmin.Wlb
{
    /// <summary>
    /// Optimize pool recommendation properties
    /// </summary>
    // WLB: find optId, assume recommendation return string format is ["WLB"; host; optId; recID; reason]
    public enum RecProperties
    {
        /// <summary>
        /// WLB recommendation.
        /// </summary>
        WLB = 0,

        /// <summary>
        /// Recommendate moving to host.
        /// </summary>
        ToHost = 1,

        /// <summary>
        /// Optimize VM set ID
        /// </summary>
        OptId = 2,

        /// <summary>
        /// VM recommendation ID
        /// </summary>
        RecId = 3,

        /// <summary>
        /// Recommendate moving reason
        /// </summary>
        Reason = 4
    }


    /// <summary>
    /// Tag each ListView row in WLBOptimizePool with one of these, and pass them to row update.
    /// </summary>
    public class WlbOptimizationRecommendation
    {
        public readonly VM vm;
        public readonly string reason;
        public readonly Host fromHost;
        public readonly Host toHost;
        public readonly int recId;
        public readonly int optId;
        public readonly string powerOperation;
        public const string OPTIMIZINGPOOL = "wlb_optimizing_pool";

        /// <summary>
        /// Set optimizal vm properties
        /// </summary>
        /// <param name="vm">vm from optimize pool recommendations.</param>
        /// <param name="fromHost">host vm resides on.</param>
        /// <param name="toHost">host to move vm to.</param>
        /// <param name="reason">optimize reason.</param>
        internal WlbOptimizationRecommendation(VM vm, Host fromHost, Host toHost, string reason, int recId, int optId, string powerOperation)
        {
            this.vm = vm;
            this.fromHost = fromHost;
            this.toHost = toHost;
            this.reason = reason;
            this.recId = recId;
            this.optId = optId;
            this.powerOperation = powerOperation;
        }

    }


    public class WlbOptimizationRecommendationCollection : List<WlbOptimizationRecommendation>
    {
        public WlbOptimizationRecommendationCollection(Pool pool, Dictionary<XenRef<VM>, string[]> recommendations)
        {
            LoadSortedRecommendationList(pool, recommendations);
        }


        private void LoadSortedRecommendationList(Pool pool, Dictionary<XenRef<VM>, string[]> recommendations)
        {
            // When there are host powerOn recommendations, the toHost of the recommended move vms are disabled,
            // We use the powerOhHosts list to check if the toHost of the recommended move vms is a powerOnHost when it's disabled 
            List<Host> powerOnHosts = GetAllPowerOnHosts(pool, recommendations);
            bool clearList = false;

            foreach (KeyValuePair<XenRef<VM>, string[]> rec in recommendations)
            {
                if (rec.Value[0].Trim().ToLower() == "wlb")
                {
                    int recId = 0;
                    int optId = 0;
                    int.TryParse(rec.Value[(int)RecProperties.RecId], out recId);
                    int.TryParse(rec.Value[(int)RecProperties.OptId], out optId);

                    //XenObject<Host> toHost = (!vm.Server.is_control_domain || (vm.Server.is_control_domain && (String.Compare(rec.Value[(int)WlbOptimizePool.RecProperties.Reason].ToString(), "PowerOn", true) == 0))) ? pool.Connection.Cache.Find_by_Uuid<Host>(rec.Value[(int)WlbOptimizePool.RecProperties.ToHost]) : null;
                    //XenObject<Host> fromHost = (!vm.Server.is_control_domain || (vm.Server.is_control_domain && (String.Compare(rec.Value[(int)WlbOptimizePool.RecProperties.Reason].ToString(), "PowerOff", true) == 0))) ? pool.Connection.Resolve(vm.Server.resident_on) : null;
                    VM vm = pool.Connection.Resolve(rec.Key);
                    Host toHost = (!vm.is_control_domain) ? pool.Connection.Cache.Find_By_Uuid<Host>(rec.Value[(int)RecProperties.ToHost]) : null;
                    Host fromHost = (!vm.is_control_domain) ? pool.Connection.Resolve(vm.resident_on) : pool.Connection.Cache.Find_By_Uuid<Host>(rec.Value[(int)RecProperties.ToHost]);
                    
                    string powerOperation = Messages.ResourceManager.GetString(String.Format("WLB_OPT_OPERATION_HOST_{0}", rec.Value[(int)RecProperties.Reason].ToString().ToUpper()));
                    
                    string resourcedReasonOutput = Messages.ResourceManager.GetString(String.Format("WLB_OPT_REASON_{0}", rec.Value[(int)RecProperties.Reason].ToString().ToUpper()));
                    if (resourcedReasonOutput == null)
                    {
                        resourcedReasonOutput = Messages.UNKNOWN;
                    }

                    /* Only vms or host have below criteria can be potentially added into recommendation collection
                     *  - if it is a control_domain (powerOn/powerOff host)
                     *    Or
                     *  - if the moving vm is running, toHost not equal to fromHost, and toHost is disable but it's a powerOnHost or toHost is enabled  
                     */ 
                    if ((vm.is_control_domain && fromHost!=null)
                        || (vm.power_state == XenAPI.vm_power_state.Running && toHost != fromHost && ((powerOnHosts.Count > 0 && !toHost.enabled && powerOnHosts.Contains(toHost)) || (toHost != null && toHost.enabled))))
                    {

                        // If it's a powerOn host, add it to the powerOnHosts list
                        //if (vm.Server.is_control_domain && (powerOperation == Messages.WLB_OPT_OPERATION_HOST_POWERON)&& !fromHost.Server.IsLive)
                        /*if(IsHostPowerOn(vm, powerOperation, fromHost))
                        {
                            powerOnHosts.Add(fromHost);
                        }
                        */

                        WlbOptimizationRecommendation optVmSetting = new WlbOptimizationRecommendation(vm, fromHost, toHost, resourcedReasonOutput, recId, optId, powerOperation);

                        /* Clear the recommendation collection if the number of vms on a power off host 
                         * doesn't match the vms in the recommendations
                         */
                        if (IsHostPowerOff(vm, powerOperation, fromHost)
                            && !VmsOnPowerOffHostAreInRecommendations(fromHost, recommendations))
                        {
                            clearList = true;
                            break;
                        }

                        /* Add to the recommendation collection if:
                         *  - it's a vm (not power on/off host)
                         *  Or 
                         *  - it's a power on/off host
                         */
                        //if (!vm.Server.is_control_domain || (vm.Server.is_control_domain && (((powerOperation == Messages.WLB_OPT_OPERATION_HOST_POWERON) && !fromHost.Server.IsLive) || ((powerOperation == Messages.WLB_OPT_OPERATION_HOST_POWEROFF) && fromHost.Server.IsLive))))
                        if (!vm.is_control_domain || IsHostPowerOnOff(vm, fromHost, powerOperation, recommendations))
                        {
                            this.Add(optVmSetting);
                        }
                    }
                }
            }

            if (clearList)
            {
                this.Clear();
            }
            else
            {
                this.Sort(SortByRecommendationId);
            }
        }


        private List<Host> GetAllPowerOnHosts(Pool pool, Dictionary<XenRef<VM>, string[]> recommendations)
        {
            List<Host> powerOnHosts = new List<Host>();

            foreach (KeyValuePair<XenRef<VM>, string[]> rec in recommendations)
            {
                if (rec.Value[0].Trim().ToLower() == "wlb")
                {
                    VM vm = pool.Connection.Resolve(rec.Key);
                    Host fromHost = (!vm.is_control_domain) ? pool.Connection.Resolve(vm.resident_on) : pool.Connection.Cache.Find_By_Uuid<Host>(rec.Value[(int)RecProperties.ToHost]);

                    string powerOperation = Messages.ResourceManager.GetString(String.Format("WLB_OPT_OPERATION_HOST_{0}", rec.Value[(int)RecProperties.Reason].ToString().ToUpper()));

                    if (vm.is_control_domain && fromHost!=null && IsHostPowerOn(vm, powerOperation, fromHost))
                    {
                        powerOnHosts.Add(fromHost);
                    }
                }
            }

            return powerOnHosts;
        }


        private int SortByRecommendationId(WlbOptimizationRecommendation x, WlbOptimizationRecommendation y)
        {
            return x.recId.CompareTo(y.recId);
        }


        private bool IsHostPowerOnOff(VM vm, Host fromHost, string powerOperation, Dictionary<XenRef<VM>, string[]> recommendations)
        {
            if (vm.is_control_domain && fromHost!=null)
            {
                if(IsHostPowerOn(vm, powerOperation, fromHost))
                {
                    return true;
                }

                if(IsHostPowerOff(vm, powerOperation, fromHost) 
                    && VmsOnPowerOffHostAreInRecommendations(fromHost, recommendations))
                {
                    return true;
                }
            }

            return false;
        }


        private bool IsHostPowerOn(VM vm, string powerOperation, Host fromHost)
        {
            if (vm != null && !String.IsNullOrEmpty(powerOperation) && fromHost != null)
            {
                return vm.is_control_domain && (powerOperation == Messages.WLB_OPT_OPERATION_HOST_POWERON) && !fromHost.IsLive;
            }
                
            return false;
        }


        private bool IsHostPowerOff(VM vm, string powerOperation, Host fromHost)
        {
            if (vm != null && !String.IsNullOrEmpty(powerOperation) && fromHost != null)
            {
                return vm.is_control_domain && (powerOperation == Messages.WLB_OPT_OPERATION_HOST_POWEROFF) && fromHost.IsLive;
            }
                
            return false;
        }


        private bool VmsOnPowerOffHostAreInRecommendations(Host fromHost, Dictionary<XenRef<VM>, string[]> recommendations)
        {

            foreach (XenRef<VM> vm in fromHost.resident_VMs)
            {
                if(!(recommendations.ContainsKey(vm)))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
