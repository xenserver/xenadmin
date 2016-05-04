/* Copyright (c) Citrix Systems Inc. 
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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using System.Linq;
using System.IO;
using XenAdmin.Network;
using XenAdmin.Diagnostics.Checks;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    class PatchPrechecksOnMultipleHostsPlanAction : PlanActionWithSession
    {
        private readonly XenServerPatch patch;
        private readonly List<PoolPatchMapping> mappings;
        private List<Host> hosts = null;

        public PatchPrechecksOnMultipleHostsPlanAction(IXenConnection connection, XenServerPatch patch, List<Host> hosts, List<PoolPatchMapping> mappings)
            : base(connection, string.Format("Checking free space for uploading and installing {0}...", patch.Name))
        {
            this.patch = patch;
            this.hosts = hosts;
            this.mappings = mappings;
        }

        protected override void RunWithSession(ref Session session)
        {
            var mapping = mappings.Find(m => m.XenServerPatch == patch);
            if (mapping != null && mapping.Pool_patch != null)
            {

                foreach (var host in hosts)
                {
                    try
                    {
                        var check = new PatchPrecheckCheck(host, mapping.Pool_patch);
                        var problem = check.RunCheck();
                        
                        if (problem != null)
                        {
                            throw new Exception(problem.Title);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Precheck failed on host {0}", host.Name), ex);
                        throw ex;
                    }
                }
            }

        }
    }
}
