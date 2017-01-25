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
using System.Linq;
using XenAPI;


namespace XenAdmin.Actions
{
    public class InstallSupplementalPackAction: PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<Host, VDI> suppPackVdis;

        public InstallSupplementalPackAction(Dictionary<Host, VDI> suppPackVdis, bool suppressHistory)
            : base(null, 
            suppPackVdis.Count > 1
                ? string.Format(Messages.UPDATES_WIZARD_APPLYING_UPDATE_MULTIPLE_HOSTS, suppPackVdis.Count) 
                : string.Format(Messages.UPDATES_WIZARD_APPLYING_UPDATE, suppPackVdis.First().Value, suppPackVdis.First().Key), 
            suppressHistory)
        {
            this.suppPackVdis = suppPackVdis;
        }

        protected override void Run()
        {
            SafeToExit = false;
            foreach (var suppPackVdi in suppPackVdis)
            {
                InstallSuppPack(suppPackVdi.Key, suppPackVdi.Value);
            }
            if (suppPackVdis.Count > 1)
                Description = Messages.ALL_UPDATES_APPLIED;
        }

        private void InstallSuppPack(Host host, VDI vdi)
        {
            // Set the correct connection object, for RecomputeCanCancel
            Connection = host.Connection; 
            Session session = host.Connection.DuplicateSession();

            try
            {
                Description = String.Format(Messages.APPLYING_PATCH, vdi.Name, host.Name);
                Host.call_plugin(session, host.opaque_ref, "install-supp-pack", "install", new Dictionary<string, string> { { "vdi", vdi.uuid } });
                Description = String.Format(Messages.PATCH_APPLIED, vdi.Name, host.Name);
            }
            catch (Failure failure)
            {
                log.ErrorFormat("Plugin call install-supp-pack.install({0}) on {1} failed with {2}", vdi.uuid, host.Name, failure.Message);
                log.ErrorFormat("Supplemental pack installation error description: {0}", string.Join(";", failure.ErrorDescription));
                throw new SupplementalPackInstallFailedException(string.Format(Messages.SUPP_PACK_INSTALL_FAILED, vdi.Name, host.Name), failure);
            }
            finally
            {
                Connection = null;
            }
        }
    }

    public class SupplementalPackInstallFailedException : ApplicationException
    {
        public SupplementalPackInstallFailedException(string message, Exception innerException) :
            base(message, innerException) { }
    }
}
