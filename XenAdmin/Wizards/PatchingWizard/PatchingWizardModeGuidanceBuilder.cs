﻿/* Copyright (c) Citrix Systems Inc. 
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

using System.Collections.Generic;
using System.Text;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard
{
    public class PatchingWizardModeGuidanceBuilder
    {
        public static string ModeRetailPatch(List<Host> servers, Pool_patch patch)
        {
            return Build(servers, patch.after_apply_guidance);
        }

        public static string ModeSuppPack(List<Host> servers)
        {
            List<after_apply_guidance> guidance = new List<after_apply_guidance> { after_apply_guidance.restartHost };
            return Build(servers, guidance);
        }

        public static string ModeNewOem(List<Host> hosts)
        {
            StringBuilder sbLog = new StringBuilder();

            sbLog.AppendLine(Messages.PATCHING_EJECT_CDS);

            //Add master
            Host master = Helpers.GetMaster(hosts[0].Connection);
            Pool pool = Helpers.GetPool(master.Connection);
            if (pool != null && pool.ha_enabled)
            {
                sbLog.AppendLine(Messages.PATCHING_WARNING_HA);
            }
            sbLog.AppendLine(Messages.PATCHINGWIZARD_MODEPAGE_RESTARTSERVERS);
            sbLog.AppendFormat("\t{0}\r\n", master.Name);
            hosts.Remove(master);
            //Add slaves
            foreach (Host host in hosts)
            {
                sbLog.AppendFormat("\t{0}\r\n", host.Name);
            }
            return sbLog.ToString();
        }

        private static string Build(List<Host> servers, List<after_apply_guidance> guidance)
        {
            StringBuilder sbLog = new StringBuilder();

            foreach (after_apply_guidance guide in guidance)
            {
                sbLog.AppendLine(GetGuideMessage(guide));

                switch (guide)
                {
                    case after_apply_guidance.restartHost:
                    case after_apply_guidance.restartXAPI:
                        foreach (Host host in servers)
                        {
                            if (host.IsMaster())
                                sbLog.AppendFormat("\t{0} ({1})\r\n", host.Name, Messages.MASTER);
                            else
                                sbLog.AppendFormat("\t{0}\r\n", host.Name);

                        }
                        break;
                    case after_apply_guidance.restartPV:
                        foreach (VM vm in Helpers.VMsRunningOn(servers))
                        {
                            if (vm.IsHVM)
                                continue;
                            sbLog.AppendFormat("\t{0}\r\n", vm.Name);
                        }
                        break;
                    case after_apply_guidance.restartHVM:
                        foreach (VM vm in Helpers.VMsRunningOn(servers))
                        {
                            if (!vm.IsHVM)
                                continue;
                            sbLog.AppendFormat("\t{0}\r\n", vm.Name);
                        }
                        break;
                }
            }

            if (guidance.Count == 0)
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
