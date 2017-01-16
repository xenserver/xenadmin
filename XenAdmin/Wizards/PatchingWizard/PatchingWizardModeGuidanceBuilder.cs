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
using System.Linq;

namespace XenAdmin.Wizards.PatchingWizard
{
    public class PatchingWizardModeGuidanceBuilder
    {
        public static string ModeRetailPatch(List<Host> servers, Pool_patch patch, out bool someHostMayRequireRestart)
        {
            return Build(servers.Where(h => patch != null && patch.AppliedOn(h) == DateTime.MaxValue).ToList(), patch != null ? patch.after_apply_guidance : new List<after_apply_guidance>(), new Dictionary<string,livepatch_status>() , out someHostMayRequireRestart);
        }

        public static string ModeRetailPatch(List<Host> servers, Pool_update update, Dictionary<string, livepatch_status> LivePatchCodesByHost, out bool someHostMayRequireRestart)
        {
            var guidances = GetAfterApplyGuidancesFromUpdate(update);

            return Build(servers.Where(h => update != null && !update.AppliedOn(h)).ToList(), update != null ? guidances : new List<after_apply_guidance>(), LivePatchCodesByHost, out someHostMayRequireRestart);
        }

        private static List<after_apply_guidance> GetAfterApplyGuidancesFromUpdate(Pool_update update)
        {
            //Pool_update guidances are defined as type update_after_apply_guidance and not as after_apply_guidance
            //therefore converting them in place as there is no further usages
            var guidances = new List<after_apply_guidance>();
            var tempGuidance = after_apply_guidance.unknown;

            if (update != null && update.after_apply_guidance != null)
            {
                foreach (var guidance in update.after_apply_guidance)
                {
                    switch (guidance)
                    {
                        case update_after_apply_guidance.restartHost:
                            tempGuidance = after_apply_guidance.restartHost;
                            break;
                        case update_after_apply_guidance.restartHVM:
                            tempGuidance = after_apply_guidance.restartHVM;
                            break;
                        case update_after_apply_guidance.restartPV:
                            tempGuidance = after_apply_guidance.restartPV;
                            break;
                        case update_after_apply_guidance.restartXAPI:
                            tempGuidance = after_apply_guidance.restartXAPI;
                            break;
                    }

                    guidances.Add(tempGuidance);
                }
            }
            return guidances;
        }

        public static string ModeSuppPack(List<Host> servers, out bool someHostMayRequireRestart)
        {
            List<after_apply_guidance> guidance = new List<after_apply_guidance> { after_apply_guidance.restartHost };
            return Build(servers, guidance, new Dictionary<string, livepatch_status>(), out someHostMayRequireRestart);
        }

        private static string Build(List<Host> servers, List<after_apply_guidance> guidance, Dictionary<string, livepatch_status> LivePatchCodesByHost, out bool someHostMayRequireRestart)
        {
            StringBuilder sbLog = new StringBuilder();
            someHostMayRequireRestart = false; // If any host has restartHost guidance this will be set to true

            foreach (after_apply_guidance guide in guidance)
            {
                if (guide == after_apply_guidance.restartHost)
                {
                    someHostMayRequireRestart = true;

                    if (LivePatchCodesByHost != null && servers.TrueForAll(h => LivePatchCodesByHost.ContainsKey(h.uuid) && LivePatchCodesByHost[h.uuid] == livepatch_status.ok_livepatch_complete)) 
                    { 
                        continue; 
                    }
                }
                    
                sbLog.AppendLine(GetGuideMessage(guide));

                switch (guide)
                {
                    case after_apply_guidance.restartHost:
                    case after_apply_guidance.restartXAPI:
                        foreach (Host host in servers)
                        {
                            if (guide == after_apply_guidance.restartHost && LivePatchCodesByHost != null && LivePatchCodesByHost.ContainsKey(host.uuid) && LivePatchCodesByHost[host.uuid] == livepatch_status.ok_livepatch_complete)
                                continue;

                            if (host.IsMaster())
                                sbLog.AppendFormat("\t{0} ({1})\r\n", host.Name, Messages.MASTER);
                            else
                                sbLog.AppendFormat("\t{0}\r\n", host.Name);

                        }
                        break;
                    case after_apply_guidance.restartPV:
                        foreach (VM vm in Helpers.VMsRunningOn(servers))
                        {
                            if (vm.IsHVM || !vm.is_a_real_vm)
                                continue;
                            sbLog.AppendFormat("\t{0}\r\n", vm.Name);
                        }
                        break;
                    case after_apply_guidance.restartHVM:
                        foreach (VM vm in Helpers.VMsRunningOn(servers))
                        {
                            if (!vm.IsHVM || !vm.is_a_real_vm)
                                continue;
                            sbLog.AppendFormat("\t{0}\r\n", vm.Name);
                        }
                        break;
                }
            }

            if (sbLog.Length == 0)
                sbLog.Append(Messages.PATCHINGWIZARD_MODEPAGE_NOACTION);

            return sbLog.ToString();
        }

        private static string GetGuideMessage(after_apply_guidance guidance)
        {
            switch (guidance)
            {
                case after_apply_guidance.restartXAPI:
                    return Messages.PATCHINGWIZARD_MODEPAGE_RESTARTXAPI;
                case after_apply_guidance.restartHost:
                    return Messages.PATCHINGWIZARD_MODEPAGE_RESTARTSERVERS;
                case after_apply_guidance.restartHVM:
                case after_apply_guidance.restartPV:
                    return Messages.PATCHINGWIZARD_MODEPAGE_RESTARTVMS;
                default:
                    return Messages.PATCHINGWIZARD_MODEPAGE_UNKNOWNACTION;
            }
        }
    }
}
