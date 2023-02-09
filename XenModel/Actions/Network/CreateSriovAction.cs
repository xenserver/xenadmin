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
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class CreateSriovAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        XenAPI.Network newNetwork;
        private List<PIF> selectedPifs;

        public CreateSriovAction(IXenConnection connection, XenAPI.Network newNetwork, List<PIF> pifs)
            : base(connection,
                string.Format(Messages.NETWORK_ACTION_CREATING_NETWORK_TITLE, newNetwork.Name(), Helpers.GetName(connection)))
        {
            this.newNetwork = newNetwork;
            selectedPifs = pifs;

            ApiMethodsToRoleCheck.AddRange("Network.create", "Network_sriov.async_create");
            //omitted Network.destroy as it's only called in case of an error
            //and failure is inconsequential
        }

        protected override void Run()
        {  
            PIF pifOnCoordinator =null;

            if (selectedPifs.Count == 0)
                return;

            foreach (PIF thePif in selectedPifs)
            {
                Host host = thePif.Connection.Resolve(thePif.host);
                if (host == null)
                    continue;

                if (host.IsCoordinator())
                {
                    pifOnCoordinator = thePif;
                    break;
                }
            }
            Connection.ExpectDisruption = true;

            //Enable SR-IOV network on Pool requires enabling coordinator first.
            if (pifOnCoordinator != null)
            {
                selectedPifs.Remove(pifOnCoordinator);
                selectedPifs.Insert(0, pifOnCoordinator);
            }

            int inc = 100 / selectedPifs.Count;
            int lo = 0;

            // Create the new network            
            XenRef<XenAPI.Network> networkRef = XenAPI.Network.create(Session, newNetwork);

            try
            {
                foreach (PIF thePif in selectedPifs)
                {
                    RelatedTask = Network_sriov.async_create(Session, thePif.opaque_ref, networkRef);
                    PollToCompletion(lo, lo + inc);
                    lo += inc;
                }
            }
            catch (Exception)
            {
                if (lo == 0)
                    DestroyNetwork(networkRef);
                throw;
            }
        }

        protected override void Clean()
        {
            Connection.ExpectDisruption = false;
        }

        private void DestroyNetwork(string network)
        {
            try
            {
                XenAPI.Network.destroy(Session, network);
            }
            catch (Exception exn)
            {
                log.Warn(exn, exn);
            }
        }

    }
}
